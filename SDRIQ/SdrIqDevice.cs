using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

using SDRSharp.Radio;

namespace SDRSharp.SDRIQ
{
    public unsafe class SdrIqDevice
    {
        private const float InputGain = 0.01f;
        private const uint DefaultFrequency = 15000000;
        private const int DefaultSamplerate = 158730;
             
        private IntPtr _dev;
        private uint _index;
        
        private GCHandle _gcHandle;
        private UnsafeBuffer _iqBuffer;
        private Complex* _iqPtr;

        private uint _centerFrequency = DefaultFrequency;
        private uint _sampleRate = DefaultSamplerate;
        private sbyte _rfGain;
        private sbyte _ifGain;

        private Thread _worker;

        private static readonly SdrIqReadAsyncDelegate _sdriqCallback = SdrIqSamplesAvailable;
        private readonly SamplesAvailableEventArgs _eventArgs = new SamplesAvailableEventArgs();

        private static readonly int _readBlockCount = (int) Utils.GetIntSetting("SDRIQReadBlockCount", 1);
        private static readonly uint _outFifoBlockCount = (uint)Utils.GetIntSetting("SDRIQOutFifoBlockCount", 0);
        
        public SdrIqDevice(uint index)
        {
            _index = index;
            
            var r = NativeMethods.sdriq_open(_index, _outFifoBlockCount, out _dev);
            if (r != 0)
            {
                throw new ApplicationException("Cannot open SDR-IQ.");
            }
            
            _gcHandle = GCHandle.Alloc(this);         
        }

        ~SdrIqDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
            NativeMethods.sdriq_close(_dev);
            if (_gcHandle.IsAllocated)
            {
                _gcHandle.Free();
            }
            _dev = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        public bool IsStreaming
        {
            get { return _worker != null; }
        }

        public uint Frequency
        {
            get
            {
                return _centerFrequency;
            }
            set
            {
                _centerFrequency = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.sdriq_set_center_frequency(_dev, _centerFrequency);
                }
            }
        }

        public uint Samplerate
        {
            get
            {
                return _sampleRate;
            }
            set
            {
                _sampleRate = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.sdriq_set_out_samplerate(_dev, _sampleRate);
                }
            }
        }

        public sbyte RfGain
        {
            get { return _rfGain; }
            set
            {
                _rfGain = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.sdriq_set_rf_gain(_dev, value);
                }
            }
        }

        public sbyte IfGain
        {
            get { return _ifGain; }
            set
            {
                _ifGain = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.sdriq_set_if_gain(_dev, value);
                }
            }
        }

        public event SamplesAvailableDelegate SamplesAvailable;

        public void Start()
        {
            if (_worker != null)
            {
                throw new ApplicationException("Already running");
            }

            var r = NativeMethods.sdriq_set_out_samplerate(_dev, _sampleRate);
            if (r != 0)
            {
                throw new ApplicationException("Cannot access SDR-IQ");
            }
            
            r = NativeMethods.sdriq_set_center_frequency(_dev, _centerFrequency);
            if (r != 0)
            {
                throw new ApplicationException("Cannot access SDR-IQ");
            }
            
            _worker = new Thread(StreamProc);
            _worker.Priority = ThreadPriority.Highest;
            _worker.Start();
        }

        public void Stop()
        {
            if (_worker == null)
            {
                return;
            }
            NativeMethods.sdriq_async_cancel(_dev);
            if (_worker.ThreadState == ThreadState.Running)
            {
                _worker.Join();
            }
            _worker = null;
        }

        public uint Index
        {
            get { return _index; }
        }

        private void StreamProc()
        {

            NativeMethods.sdriq_async_read(_dev, (IntPtr)_gcHandle, _sdriqCallback, _readBlockCount);           
        }

        private static void SdrIqSamplesAvailable(short* buf, uint len, IntPtr ctx)
        {
            var gcHandle = GCHandle.FromIntPtr(ctx);
            if (!gcHandle.IsAllocated)
            {
                return;
            } 
            var instance = (SdrIqDevice) gcHandle.Target;

            var sampleCount = (int) len / 2;
            if (instance._iqBuffer == null || instance._iqBuffer.Length != sampleCount)
            {
                instance._iqBuffer = UnsafeBuffer.Create(sampleCount, sizeof(Complex));
                instance._iqPtr = (Complex*) instance._iqBuffer;
            }

            const float scale = InputGain / 32767.0f;
            var output = instance._iqPtr;
            for (int i = 0; i < sampleCount; i++)
            {
                output->Imag = *buf++ * scale;
                output->Real = *buf++ * scale;
                output++;
            }

            instance.ComplexSamplesAvailable(instance._iqPtr, instance._iqBuffer.Length);
        }

        private void ComplexSamplesAvailable(Complex* buffer, int length)
        {
            if (SamplesAvailable != null)
            {
                _eventArgs.Buffer = buffer;
                _eventArgs.Length = length;
                SamplesAvailable(this, _eventArgs);
                
            }
        }

    }

    public delegate void SamplesAvailableDelegate(object sender, SamplesAvailableEventArgs e);

    public unsafe sealed class SamplesAvailableEventArgs : EventArgs
    {
        public int Length { get; set; }
        public Complex* Buffer { get; set; }
    }
}
