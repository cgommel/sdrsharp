//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE.
//
//  This material may not be duplicated in whole or in part, except for 
//  personal use, without the express written consent of the author. 
//
//  Email:  ianier@hotmail.com
//
//  Copyright (C) 1999-2003 Ianier Munoz. All Rights Reserved.

using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace WaveLib
{
	internal class WaveInHelper
	{
		public static void Try(int err)
		{
			if (err != WaveNative.MMSYSERR_NOERROR)
				throw new Exception(err.ToString());
		}
	}

	public delegate void BufferDoneEventHandler(IntPtr data, int size);

	internal class WaveInBuffer : IDisposable
	{
		public WaveInBuffer NextBuffer;

		private AutoResetEvent m_RecordEvent = new AutoResetEvent(false);
		private IntPtr m_WaveIn;

		private WaveNative.WaveHdr m_Header;
		private byte[] m_HeaderData;
		private GCHandle m_HeaderHandle;
		private GCHandle m_HeaderDataHandle;

		private bool m_Recording;

		internal static void WaveInProc(IntPtr hdrvr, int uMsg, int dwUser, ref WaveNative.WaveHdr wavhdr, int dwParam2)
		{
			if (uMsg == WaveNative.MM_WIM_DATA)
			{
				try
				{
					GCHandle h = (GCHandle)wavhdr.dwUser;
					WaveInBuffer buf = (WaveInBuffer)h.Target;
					buf.OnCompleted();
				}
				catch
				{
				}
			}
		}

		public WaveInBuffer(IntPtr waveInHandle, int size)
		{
			m_WaveIn = waveInHandle;

			m_HeaderHandle = GCHandle.Alloc(m_Header, GCHandleType.Pinned);
			m_Header.dwUser = (IntPtr)GCHandle.Alloc(this);
			m_HeaderData = new byte[size];
			m_HeaderDataHandle = GCHandle.Alloc(m_HeaderData, GCHandleType.Pinned);
			m_Header.lpData = m_HeaderDataHandle.AddrOfPinnedObject();
			m_Header.dwBufferLength = size;
			WaveInHelper.Try(WaveNative.waveInPrepareHeader(m_WaveIn, ref m_Header, Marshal.SizeOf(m_Header)));
		}
		~WaveInBuffer()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (m_Header.lpData != IntPtr.Zero)
			{
				WaveNative.waveInUnprepareHeader(m_WaveIn, ref m_Header, Marshal.SizeOf(m_Header));
				m_HeaderHandle.Free();
				m_Header.lpData = IntPtr.Zero;
			}
			m_RecordEvent.Close();
			if (m_HeaderDataHandle.IsAllocated)
				m_HeaderDataHandle.Free();
			GC.SuppressFinalize(this);
		}

		public int Size
		{
			get { return m_Header.dwBufferLength; }
		}

		public IntPtr Data
		{
			get { return m_Header.lpData; }
		}

		public bool Record()
		{
			lock(this)
			{
				m_RecordEvent.Reset();
				try
				{
					m_Recording = WaveNative.waveInAddBuffer(m_WaveIn, ref m_Header, Marshal.SizeOf(m_Header)) == WaveNative.MMSYSERR_NOERROR;
				}
				catch
				{
					m_Recording = false;
				}
                return m_Recording;
			}
		}

		public void WaitFor()
		{
			if (m_Recording)
				m_Recording = m_RecordEvent.WaitOne(1000, false);
			else
				Thread.Sleep(0);
		}

		private void OnCompleted()
		{
			m_RecordEvent.Set();
			m_Recording = false;
		}
	}

	public class WaveInRecorder : IDisposable
	{
		private IntPtr m_WaveIn;
		private WaveInBuffer m_Buffers; // linked list
		private WaveInBuffer m_CurrentBuffer;
		private Thread m_Thread;
		private BufferDoneEventHandler m_DoneProc;
		private bool m_Finished;

		private WaveNative.WaveDelegate m_BufferProc = new WaveNative.WaveDelegate(WaveInBuffer.WaveInProc);

		public static int DeviceCount
		{
			get { return WaveNative.waveInGetNumDevs(); }
		}

		public WaveInRecorder(int device, WaveFormat format, int bufferSize, int bufferCount, BufferDoneEventHandler doneProc)
		{
			m_DoneProc = doneProc;
			WaveInHelper.Try(WaveNative.waveInOpen(out m_WaveIn, device, format, m_BufferProc, 0, WaveNative.CALLBACK_FUNCTION));
			AllocateBuffers(bufferSize, bufferCount);
			for (int i = 0; i < bufferCount; i++)
			{
				SelectNextBuffer();
				m_CurrentBuffer.Record();
			}
			WaveInHelper.Try(WaveNative.waveInStart(m_WaveIn));
			m_Thread = new Thread(new ThreadStart(ThreadProc));
			m_Thread.Start();
		}
		~WaveInRecorder()
		{
			Dispose();
		}
		public void Dispose()
		{
			if (m_Thread != null)
				try
				{
					m_Finished = true;
					if (m_WaveIn != IntPtr.Zero)
					{
						while (WaveNative.waveInClose(m_WaveIn) != 0)
						{
							WaveNative.waveInReset(m_WaveIn);
						}
					}
					WaitForAllBuffers();
					m_Thread.Join();
					m_DoneProc = null;
					FreeBuffers();
					if (m_WaveIn != IntPtr.Zero)
						WaveNative.waveInClose(m_WaveIn);
				}
				finally
				{
					m_Thread = null;
					m_WaveIn = IntPtr.Zero;
				}
			GC.SuppressFinalize(this);
		}
		private void ThreadProc()
		{
			while (!m_Finished)
			{
				Advance();
				if (m_DoneProc != null && !m_Finished)
					m_DoneProc(m_CurrentBuffer.Data, m_CurrentBuffer.Size);
				m_CurrentBuffer.Record();
			}
		}
		private void AllocateBuffers(int bufferSize, int bufferCount)
		{
			FreeBuffers();
			if (bufferCount > 0)
			{
				m_Buffers = new WaveInBuffer(m_WaveIn, bufferSize);
				WaveInBuffer Prev = m_Buffers;
				try
				{
					for (int i = 1; i < bufferCount; i++)
					{
						WaveInBuffer Buf = new WaveInBuffer(m_WaveIn, bufferSize);
						Prev.NextBuffer = Buf;
						Prev = Buf;
					}
				}
				finally
				{
					Prev.NextBuffer = m_Buffers;
				}
			}
		}
		private void FreeBuffers()
		{
			m_CurrentBuffer = null;
			if (m_Buffers != null)
			{
				WaveInBuffer First = m_Buffers;
				m_Buffers = null;

				WaveInBuffer Current = First;
				do
				{
					WaveInBuffer Next = Current.NextBuffer;
					Current.Dispose();
					Current = Next;
				} while(Current != First);
			}
		}
		private void Advance()
		{
			SelectNextBuffer();
			m_CurrentBuffer.WaitFor();
		}
		private void SelectNextBuffer()
		{
			m_CurrentBuffer = m_CurrentBuffer == null ? m_Buffers : m_CurrentBuffer.NextBuffer;
		}
		private void WaitForAllBuffers()
		{
			WaveInBuffer Buf = m_Buffers;
			while (Buf.NextBuffer != m_Buffers)
			{
				Buf.WaitFor();
				Buf = Buf.NextBuffer;
			}
		}
	}
}
