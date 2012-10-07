using System;
using System.Runtime.InteropServices;
using System.Threading;
using SDRSharp.Radio;

namespace SDRSharp.RTLSDR
{
    public enum SamplingMode
    {
        Quadrature = 0,
        DirectSamplingI,
        DirectSamplingQ
    }

    public unsafe sealed class RtlDevice : IDisposable
    {
        private const uint DefaultFrequency = 105500000;
        private const int DefaultSamplerate = 2048000;

        private readonly uint _index;
        private IntPtr _dev;
        private readonly string _name;
        private readonly int[] _supportedGains;
        private bool _useTunerAGC = true;
        private bool _useRtlAGC;
        private int _tunerGain;
        private uint _centerFrequency = DefaultFrequency;
        private uint _sampleRate = DefaultSamplerate;
        private int _frequencyCorrection;
        private SamplingMode _samplingMode;
        private bool _useOffsetTuning;
        private bool _supportsOffsetTuning;

        private GCHandle _gcHandle;
        private UnsafeBuffer _iqBuffer;
        private Complex* _iqPtr;
        private Thread _worker;
        private readonly SamplesAvailableEventArgs _eventArgs = new SamplesAvailableEventArgs();
        private static readonly RtlSdrReadAsyncDelegate _rtlCallback = RtlSdrSamplesAvailable;
        private static readonly uint _readLength = (uint) Utils.GetIntSetting("RTLBufferLength", 16 * 1024);

        public RtlDevice(uint index)
        {
            _index = index;
            var r = NativeMethods.rtlsdr_open(out _dev, _index);
            if (r != 0)
            {
                throw new ApplicationException("Cannot open RTL device. Is the device locked somewhere?");
            }
            var count = _dev == IntPtr.Zero ? 0 : NativeMethods.rtlsdr_get_tuner_gains(_dev, null);
            if (count < 0)
            {
                count = 0;
            }
            _supportsOffsetTuning = NativeMethods.rtlsdr_set_offset_tuning(_dev, 0) != -2;
            _supportedGains = new int[count];
            if (count >= 0)
            {
                NativeMethods.rtlsdr_get_tuner_gains(_dev, _supportedGains);
            }
            _name = NativeMethods.rtlsdr_get_device_name(_index);
            _gcHandle = GCHandle.Alloc(this);
        }

        ~RtlDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
            NativeMethods.rtlsdr_close(_dev);
            if (_gcHandle.IsAllocated)
            {
                _gcHandle.Free();
            }
            _dev = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        public event SamplesAvailableDelegate SamplesAvailable;

        public void Start()
        {
            if (_worker != null)
            {
                throw new ApplicationException("Already running");
            }

            var r = NativeMethods.rtlsdr_set_sample_rate(_dev, _sampleRate);
            if (r != 0)
            {
                throw new ApplicationException("Cannot access RTL device");
            }
            r = NativeMethods.rtlsdr_set_center_freq(_dev, _centerFrequency);
            if (r != 0)
            {
                throw new ApplicationException("Cannot access RTL device");
            }
            r = NativeMethods.rtlsdr_set_tuner_gain_mode(_dev, _useTunerAGC ? 0 : 1);
            if (r != 0)
            {
                throw new ApplicationException("Cannot access RTL device");
            }
            if (!_useTunerAGC)
            {
                r = NativeMethods.rtlsdr_set_tuner_gain(_dev, _tunerGain);
                if (r != 0)
                {
                    throw new ApplicationException("Cannot access RTL device");
                }
            }
            r = NativeMethods.rtlsdr_reset_buffer(_dev);
            if (r != 0)
            {
                throw new ApplicationException("Cannot access RTL device");
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
            NativeMethods.rtlsdr_cancel_async(_dev);
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

        public string Name
        {
            get { return _name; }
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
                    NativeMethods.rtlsdr_set_sample_rate(_dev, _sampleRate);
                }
            }
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
                    NativeMethods.rtlsdr_set_center_freq(_dev, _centerFrequency);
                }
            }
        }

        public bool UseRtlAGC
        {
            get { return _useRtlAGC; }
            set
            {
                _useRtlAGC = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.rtlsdr_set_agc_mode(_dev, _useRtlAGC ? 1 : 0);
                }
            }
        }

        public bool UseTunerAGC
        {
            get { return _useTunerAGC; }
            set
            {
                _useTunerAGC = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.rtlsdr_set_tuner_gain_mode(_dev, _useTunerAGC ? 0 : 1);
                }
            }
        }

        public SamplingMode SamplingMode
        {
            get { return _samplingMode; }
            set
            {
                _samplingMode = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.rtlsdr_set_direct_sampling(_dev, (int) _samplingMode);
                }
            }
        }

        public bool SupportsOffsetTuning
        {
            get { return _supportsOffsetTuning; }
        }

        public bool UseOffsetTuning
        {
            get { return _useOffsetTuning; }
            set
            {
                _useOffsetTuning = value;

                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.rtlsdr_set_offset_tuning(_dev, _useOffsetTuning ? 1 : 0);
                }
            }
        }

        public int[] SupportedGains
        {
            get { return _supportedGains; }
        }

        public int Gain
        {
            get { return _tunerGain; }
            set
            {
                _tunerGain = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.rtlsdr_set_tuner_gain(_dev, _tunerGain);
                }
            }
        }

        public int FrequencyCorrection
        {
            get
            {
                return _frequencyCorrection;
            }
            set
            {
                _frequencyCorrection = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.rtlsdr_set_freq_correction(_dev, _frequencyCorrection);
                }
            }
        }

        public RtlSdrTunerType TunerType
        {
            get
            {
                return _dev == IntPtr.Zero ? RtlSdrTunerType.Unknown : NativeMethods.rtlsdr_get_tuner_type(_dev);
            }
        }

        public bool IsStreaming
        {
            get { return _worker != null; }
        }

        #region Streaming methods

        private void StreamProc()
        {
            NativeMethods.rtlsdr_read_async(_dev, _rtlCallback, (IntPtr) _gcHandle, 0, _readLength);
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

        private static void RtlSdrSamplesAvailable(byte* buf, uint len, IntPtr ctx)
        {
            var gcHandle = GCHandle.FromIntPtr(ctx);
            if (!gcHandle.IsAllocated)
            {
                return;
            }
            var instance = (RtlDevice) gcHandle.Target;

            var sampleCount = (int) len / 2;
            if (instance._iqBuffer == null || instance._iqBuffer.Length != sampleCount)
            {
                instance._iqBuffer = UnsafeBuffer.Create(sampleCount, sizeof(Complex));
                instance._iqPtr = (Complex*) instance._iqBuffer;
            }

            const float scale = 1.0f / 128.0f;
            var ptr = instance._iqPtr;
            for (var i = 0; i < sampleCount; i++)
            {
                ptr->Imag = (*buf++ - 128) * scale;
                ptr->Real = (*buf++ - 128) * scale;
                ptr++;
            }

            instance.ComplexSamplesAvailable(instance._iqPtr, instance._iqBuffer.Length);
        }

        #endregion
    }

    public delegate void SamplesAvailableDelegate(object sender, SamplesAvailableEventArgs e);

    public unsafe sealed class SamplesAvailableEventArgs : EventArgs
    {
        public int Length { get; set; }
        public Complex* Buffer { get; set; }
    }
}
