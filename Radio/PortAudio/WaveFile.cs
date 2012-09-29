using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio.PortAudio
{
	public sealed unsafe class WaveFile : IDisposable
	{
		private readonly Stream _stream;
	    private bool _isPCM;
		private long _dataPos;
	    private short _formatTag;
	    private int _sampleRate;
	    private int _avgBytesPerSec;
	    private int _length;
	    private short _blockAlign;
        private short _bitsPerSample;
        private UnsafeBuffer _tempBuffer;
        private byte[] _temp;
        private byte* _tempPtr;

        ~WaveFile()
        {
            Dispose();
        }

        public WaveFile(string fileName)
        {
            _stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            ReadHeader();
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            if (_stream != null)
                _stream.Close();
        }

	    private static string ReadChunk(BinaryReader reader)
		{
			var ch = new byte[4];
			reader.Read(ch, 0, ch.Length);
			return System.Text.Encoding.ASCII.GetString(ch);
		}

		private void ReadHeader()
		{
			var reader = new BinaryReader(_stream);
			if (ReadChunk(reader) != "RIFF")
				throw new Exception("Invalid file format");

			reader.ReadInt32(); // File length minus first 8 bytes of RIFF description, we don't use it

			if (ReadChunk(reader) != "WAVE")
				throw new Exception("Invalid file format");

			if (ReadChunk(reader) != "fmt ")
				throw new Exception("Invalid file format");

			int len = reader.ReadInt32();
			if (len < 16) // bad format chunk length
				throw new Exception("Invalid file format");

            _formatTag = reader.ReadInt16();
            _isPCM = _formatTag == 1;
			var nChannels = reader.ReadInt16();
            if (nChannels != 2)
                throw new Exception("Invalid file format");
			_sampleRate = reader.ReadInt32();
			_avgBytesPerSec = reader.ReadInt32();
			_blockAlign = reader.ReadInt16();
			_bitsPerSample = reader.ReadInt16(); 

			// advance in the stream to skip the wave format block 
            len -= 16; // minimum format size
            while (len > 0)
            {
                reader.ReadByte();
                len--;
            }

			// assume the data chunk is aligned
            while (_stream.Position < _stream.Length && ReadChunk(reader) != "data")
            {
                len = reader.ReadInt32();
                while (_stream.Position < _stream.Length && len > 0)
                {
                    reader.ReadByte();
                    len--;
                }
            }

			if (_stream.Position >= _stream.Length)
				throw new Exception("Invalid file format");

			_length = reader.ReadInt32();
            _dataPos = _stream.Position;
		}

        public void Read(Complex* iqBuffer, int length)
        {
            if (_temp == null || _temp.Length != _blockAlign * length)
            {
                _temp = new byte[_blockAlign * length];
                _tempBuffer = UnsafeBuffer.Create(_temp);
                _tempPtr = (byte*) _tempBuffer;
            }
            var pos = 0;
            var size = _tempBuffer.Length;
            while (pos < size)
            {
                int toget = size - pos;
                int got = _stream.Read(_temp, pos, toget);
                if (got < toget)
                    _stream.Position = _dataPos; // loop if the file ends
                if (got <= 0)
                    break;
                pos += got;
            }
            FillIQ(iqBuffer, length);
        }

        private void FillIQ(Complex* iqPtr, int length)
        {
            if (_isPCM)
            {
                if (_blockAlign == 6)
                {
                    const float scale = 1.0f / 8388608.0f;
                    var ptr = (Int24*) _tempPtr;
                    for (var i = 0; i < length; i++)
                    {
                        iqPtr->Real = *ptr++ * scale;
                        iqPtr->Imag = *ptr++ * scale;
                        iqPtr++;
                    }
                }
                else if (_blockAlign == 4)
                {
                    const float scale = 1.0f / 32767.0f;
                    var ptr = (Int16*) _tempPtr;
                    for (var i = 0; i < length; i++)
                    {
                        iqPtr->Real = *ptr++ * scale;
                        iqPtr->Imag = *ptr++ * scale;
                        iqPtr++;
                    }
                }
                else if (_blockAlign == 2)
                {
                    const float scale = 1.0f / 128.0f;
                    var ptr = (sbyte*) _tempPtr;
                    for (var i = 0; i < length; i++)
                    {
                        iqPtr->Real = *ptr++ * scale;
                        iqPtr->Imag = *ptr++ * scale;
                        iqPtr++;
                    }
                }
            }
            else
            {
                var ptr = (float*) _tempPtr;
                for (var i = 0; i < length; i++)
                {
                    iqPtr->Real = *ptr++;
                    iqPtr->Imag = *ptr++;
                    iqPtr++;
                }
            }
        }

		public long Position
		{
			get { return _stream.Position - _dataPos; }
            set { _stream.Seek(value + _dataPos, SeekOrigin.Begin); }
		}

	    public short FormatTag
	    {
	        get { return _formatTag; }
	    }

	    public int SampleRate
	    {
	        get { return _sampleRate; }
	    }

	    public int AvgBytesPerSec
	    {
	        get { return _avgBytesPerSec; }
	    }

	    public short BlockAlign
	    {
	        get { return _blockAlign; }
	    }

	    public short BitsPerSample
	    {
	        get { return _bitsPerSample; }
	    }

	    public int Length
	    {
	        get { return _length; }
	    }
	}

    [StructLayout(LayoutKind.Sequential)]
    public struct Int24
    {
        public byte C;
        public byte B;
        public sbyte A;
            
        public static implicit operator float (Int24 i)
        {
            return (i.C << 8 | i.B << 16 | i.A << 24) >> 8;
        }
    }
}
