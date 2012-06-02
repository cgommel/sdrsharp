using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.RTLSDR
{
    public unsafe class RtlSdrIO : IFrontendController, IDisposable
    {
        private const uint DefaultReadLength = 1024 * 16;
        private const uint DefaultFrequency = 105500000;

        private const float InputGain = 0.001f;
        private const int DefaultSamplerate = 2048000;

        private IntPtr _dev;
        private uint _deviceIndex;
        private string _deviceName;
        private bool _useAutomaticGain;
        private int[] _supportedGains;
        private GCHandle _gcHandle;
        private UnsafeBuffer _iqBuffer;
        private Complex* _iqPtr;
        private Thread _worker;
        private SamplesAvailableDelegate _managedCallback;
        private readonly RtlSdrControllerDialog _gui;
        private static readonly RtlSdrReadAsyncDelegate _rtlCallback = RtlSdrSamplesAvailable;


        public RtlSdrIO()
        {
            _gcHandle = GCHandle.Alloc(this);
            _gui = new RtlSdrControllerDialog(this);
        }

        ~RtlSdrIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
            Close();
            if (_gcHandle.IsAllocated)
            {
                _gcHandle.Free();
            }
            _gui.Dispose();
            GC.SuppressFinalize(this);
        }

        public string DeviceName
        {
            get { return _deviceName; }
        }

        public uint DeviceIndex
        {
            get
            {
                return _deviceIndex;
            }
            set
            {
                if (_deviceIndex != value)
                {
                    _deviceIndex = value;
                    _deviceName = NativeMethods.rtlsdr_get_device_name(_deviceIndex);
                    if (_dev != IntPtr.Zero)
                    {
                        Close();
                        Open();
                    }

                    var count = NativeMethods.rtlsdr_get_tuner_gains(_dev, null);
                    _supportedGains = new int[count];
                    NativeMethods.rtlsdr_get_tuner_gains(_dev, _supportedGains);
                }
            }
        }

        public void Open()
        {
            if (NativeMethods.rtlsdr_get_device_count() <= 0)
            {
                throw new ApplicationException("No compatible device detected");
            }
            NativeMethods.rtlsdr_open(out _dev, _deviceIndex);
            NativeMethods.rtlsdr_set_sample_rate(_dev, DefaultSamplerate);
            NativeMethods.rtlsdr_set_center_freq(_dev, DefaultFrequency);
        }

        public void Start(SamplesAvailableDelegate callback)
        {
            if (_worker != null)
            {
                return;
            }
                
            NativeMethods.rtlsdr_reset_buffer(_dev);

            _managedCallback = callback;
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
            _worker.Join();
            _worker = null;
            _managedCallback = null;
        }

        public void Close()
        {
            if (_dev != IntPtr.Zero)
            {
                NativeMethods.rtlsdr_close(_dev);
                _dev = IntPtr.Zero;
            }
        }

        public bool IsStreaming
        {
            get { return _worker != null; }
        }

        public bool IsSoundCardBased
        {
            get { return false; }
        }

        public string SoundCardHint
        {
            get { return string.Empty; }
        }

        public double Samplerate
        {
            get
            {
                return NativeMethods.rtlsdr_get_sample_rate(_dev);
            }
            set
            {
                NativeMethods.rtlsdr_set_sample_rate(_dev, (uint) value);
            }
        }

        public long Frequency
        {
            get
            {
                return NativeMethods.rtlsdr_get_center_freq(_dev);
            }
            set
            {
                NativeMethods.rtlsdr_set_center_freq(_dev, unchecked ((uint) value));
            }
        }

        public bool UseAutomaticGain
        {
            get { return _useAutomaticGain; }
            set
            {
                _useAutomaticGain = value;
                NativeMethods.rtlsdr_set_tuner_gain_mode(_dev, _useAutomaticGain ? 0 : 1);
            }
        }

        public int[] SupportedGains
        {
            get { return _supportedGains; }
        }

        public int Gain
        {
            get { return NativeMethods.rtlsdr_get_tuner_gain(_dev); }
            set { NativeMethods.rtlsdr_set_tuner_gain(_dev, value); }
        }

        public int FrequencyCorrection
        {
            get { return NativeMethods.rtlsdr_get_freq_correction(_dev); }
            set { NativeMethods.rtlsdr_set_freq_correction(_dev, value); }
        }

        public void ShowSettingGUI(IWin32Window parent)
        {
            _gui.Show();
        }

        public void HideSettingGUI()
        {
            _gui.Hide();
        }

        #region Streaming methods

        private void StreamProc()
        {
            NativeMethods.rtlsdr_read_async(_dev, _rtlCallback, (IntPtr) _gcHandle, 0, DefaultReadLength);
        }

        protected void ComplexSamplesAvailable(Complex* buffer, int length)
        {
            if (_managedCallback != null)
            {
                _managedCallback(this, buffer, length);
            }
        }

        private static void RtlSdrSamplesAvailable(byte* buf, uint len, IntPtr ctx)
        {
            var gcHandle = GCHandle.FromIntPtr(ctx);
            if (!gcHandle.IsAllocated)
            {
                return;
            }
            var instance = (RtlSdrIO) gcHandle.Target;

            if (instance._iqBuffer == null || instance._iqBuffer.Length != len)
            {
                instance._iqBuffer = UnsafeBuffer.Create((int) len / 2, sizeof (Complex));
                instance._iqPtr = (Complex*) instance._iqBuffer;
            }

            for (int i = 0; i < instance._iqBuffer.Length; i++)
            {
                instance._iqPtr[i].Real = (*(buf + i * 2 + 1) - 128) / 128.0f * InputGain;
                instance._iqPtr[i].Imag = (*(buf + i * 2) - 128) / 128.0f * InputGain;
            }

            instance.ComplexSamplesAvailable(instance._iqPtr, instance._iqBuffer.Length);
        }

        #endregion
    }
}
