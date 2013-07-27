using System;
using System.Runtime.InteropServices;
using System.Threading;
using SDRSharp.Radio;

namespace SDRSharp.HackRF
{    
    public unsafe sealed class HackRFDevice : IDisposable
    {
        private const uint DefaultFrequency = 105500000;
        private const int DefaultSamplerate = 10000000;
        private const string DeviceName = "HackRF Jawbreaker";
        
        private static readonly float* _lutPtr;
        private static readonly UnsafeBuffer _lutBuffer = UnsafeBuffer.Create(256, sizeof(float));
        
        private IntPtr _dev;              
        private long _centerFrequency = DefaultFrequency;
        private double _sampleRate = DefaultSamplerate;        
        private uint _lnaGain;
        private uint _vgaGain;
        private bool _amp;
        
        private GCHandle _gcHandle;
        private UnsafeBuffer _iqBuffer;
        private Complex* _iqPtr;
        private bool _isStreaming;
        private readonly SamplesAvailableEventArgs _eventArgs = new SamplesAvailableEventArgs();
        private static readonly hackrf_sample_block_cb_fn _HackRFCallback = HackRFSamplesAvailable;
        private static readonly uint _readLength = (uint)Utils.GetIntSetting("HackRFBufferLength", 16 * 1024);

        static HackRFDevice()
        {
            _lutPtr = (float*)_lutBuffer;

            const float scale = 1.0f / 127.0f;
            for (var i = 0; i < 256; i++)
            {
                _lutPtr[i] = (i - 128) * scale;
            }
        }

        public HackRFDevice()
        {
            var r = NativeMethods.hackrf_init();
            if (r != 0)
            {
                throw new ApplicationException("Cannot init HackRF device. Is the device locked somewhere?");
            }

            r = NativeMethods.hackrf_open(out _dev);
            if (r != 0)
            {
                throw new ApplicationException("Cannot open HackRF device. Is the device locked somewhere?");
            }
                     
            _gcHandle = GCHandle.Alloc(this);
        }

        ~HackRFDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
            NativeMethods.hackrf_close(_dev);
            NativeMethods.hackrf_exit();
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
            if (_isStreaming)
            {
                throw new ApplicationException("Start() Already running");
            }

            var r = NativeMethods.hackrf_set_sample_rate(_dev, _sampleRate);
            if (r != 0)
            {
                throw new ApplicationException("hackrf_sample_rate_set() error");
            }

            r = NativeMethods.hackrf_set_amp_enable(_dev, (byte)(_amp ? 1 : 0));
            if (r != 0)
            {
                throw new ApplicationException("hackrf_set_amp_enable() error");
            }

            r = NativeMethods.hackrf_set_lna_gain(_dev, _lnaGain);
            if (r != 0)
            {
                throw new ApplicationException("hackrf_set_lna_gain() error");
            }

            r = NativeMethods.hackrf_set_vga_gain(_dev, _vgaGain);
            if (r != 0)
            {
                throw new ApplicationException("hackrf_set_vga_gain() error");
            }
            
            var baseband_filter_bw_hz = NativeMethods.hackrf_compute_baseband_filter_bw_round_down_lt((uint)_sampleRate);            
            r = NativeMethods.hackrf_set_baseband_filter_bandwidth(_dev, baseband_filter_bw_hz);
            if (r != 0)
            {
                throw new ApplicationException("hackrf_baseband_filter_bandwidth_set() error");
            }

            r = NativeMethods.hackrf_set_freq(_dev, _centerFrequency);
            if (r != 0)
            {
                throw new ApplicationException("hackrf_set_freq() error");
            }

            r = NativeMethods.hackrf_start_rx(_dev, _HackRFCallback, (IntPtr)_gcHandle);
            if (r != 0)
            {
                throw new ApplicationException("hackrf_start_rx() error");
            }

            r = NativeMethods.hackrf_is_streaming(_dev);
            if (r != 1)
            {
                throw new ApplicationException("hackrf_is_streaming() Error");
            }

            _isStreaming = true;
        }

        public void Stop()
        {
            if (!_isStreaming)
            {
                return;
            }

            NativeMethods.hackrf_stop_rx(_dev);
            _isStreaming = false;
        }

        public uint Index
        {
            get { return 0; }
        }

        public string Name
        {
            get { return DeviceName; }
        }

        public uint LNAGain
        {
            get { return _lnaGain; }
            set
            {
                _lnaGain = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.hackrf_set_lna_gain(_dev, _lnaGain);                    
                }
            }
        }

        public uint VGAGain
        {
            get { return _vgaGain; }
            set
            {
                _vgaGain = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.hackrf_set_vga_gain(_dev, _vgaGain);
                    
                }
            }
        }

        public bool EnableAmp
        {
            get { return _amp; }
            set
            {
                _amp = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.hackrf_set_amp_enable(_dev, (byte)(_amp ? 1 : 0));
                }
            }
        }

        public double SampleRate
        {
            get { return _sampleRate; }            
            set
            {
                _sampleRate = value;
                if (_dev != IntPtr.Zero)
                {                    
                    NativeMethods.hackrf_set_sample_rate(_dev, _sampleRate);
                }
            }
        }

        public long Frequency
        {
            get { return _centerFrequency; }
            set
            {
                _centerFrequency = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.hackrf_set_freq(_dev, _centerFrequency);
                }
            }
        }
        
        public bool IsStreaming
        {
            get { return _isStreaming; }
        }

        #region Streaming methods

        private void ComplexSamplesAvailable(Complex* buffer, int length)
        {
            if (SamplesAvailable != null)
            {
                _eventArgs.Buffer = buffer;
                _eventArgs.Length = length;
                SamplesAvailable(this, _eventArgs);
            }
        }

        private static int HackRFSamplesAvailable(hackrf_transfer* ptr)
        {
            byte* buf = ptr->buffer;
            int len = ptr->buffer_length;
            IntPtr ctx = ptr->rx_ctx;

            var gcHandle = GCHandle.FromIntPtr(ctx);
            if (!gcHandle.IsAllocated)
            {
                return -1;
            }
            var instance = (HackRFDevice)gcHandle.Target;

            var sampleCount = (int)len / 2;
            if (instance._iqBuffer == null || instance._iqBuffer.Length != sampleCount)
            {
                instance._iqBuffer = UnsafeBuffer.Create(sampleCount, sizeof(Complex));
                instance._iqPtr = (Complex*)instance._iqBuffer;
            }

            var ptrIq = instance._iqPtr;
            for (var i = 0; i < sampleCount; i++)
            {
                ptrIq->Imag = _lutPtr[*buf++];
                ptrIq->Real = _lutPtr[*buf++];
                ptrIq++;
            }

            instance.ComplexSamplesAvailable(instance._iqPtr, instance._iqBuffer.Length);
            return 0;
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
