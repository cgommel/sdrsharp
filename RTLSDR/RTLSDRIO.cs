using System;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.RTLSDR
{
    public unsafe class RtlSdrIO : IFrontendController, IDisposable
    {
        private readonly RtlSdrControllerDialog _gui;
        private RtlDevice _rtlDevice;
        private Radio.SamplesAvailableDelegate _callback;

        public RtlSdrIO()
        {
            _gui = new RtlSdrControllerDialog(this);
        }

        ~RtlSdrIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_gui != null)
            {
                _gui.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public void SelectDevice(uint index)
        {
            if (_rtlDevice != null && _rtlDevice.Index == index)
            {
                return;
            }
            Close();
            _rtlDevice = new RtlDevice(index);
            _rtlDevice.SamplesAvailable += rtlDevice_SamplesAvailable;
            _gui.ConfigureGUI();
            _gui.ConfigureDevice();
        }

        public RtlDevice Device
        {
            get { return _rtlDevice; }
        }

        public void Open()
        {
            var devices = DeviceDisplay.GetActiveDevices();
            foreach (var device in devices)
            {
                try
                {
                    SelectDevice(device.Index);
                    return;
                }
                catch (ApplicationException)
                {
                    // Just ignore it
                }
            }
            if (devices.Length > 0)
            {
                throw new ApplicationException(devices.Length + " compatible devices have been found but are all busy");
            }
            throw new ApplicationException("No compatible devices found");
        }

        public void Close()
        {
            if (_rtlDevice != null)
            {
                _rtlDevice.Stop();
                _rtlDevice.SamplesAvailable -= rtlDevice_SamplesAvailable;
                _rtlDevice.Dispose();
                _rtlDevice = null;
            }
        }

        public void Start(Radio.SamplesAvailableDelegate callback)
        {
            if (_rtlDevice == null)
            {
                throw new ApplicationException("No device selected");
            }
            _callback = callback;
            _rtlDevice.Start();
        }

        public void Stop()
        {
            _rtlDevice.Stop();
        }

        public bool IsSoundCardBased
        {
            get { return false; }
        }

        public string SoundCardHint
        {
            get { return string.Empty; }
        }

        public void ShowSettingGUI(IWin32Window parent)
        {
            _gui.Show();
        }

        public void HideSettingGUI()
        {
            _gui.Hide();
        }

        public double Samplerate
        {
            get { return _rtlDevice == null ? 0.0 : _rtlDevice.Samplerate; }
        }

        public long Frequency
        {
            get { return _rtlDevice == null ? 0 : _rtlDevice.Frequency; }
            set
            {
                if (_rtlDevice != null)
                {
                    _rtlDevice.Frequency = (uint) value;
                }
            }
        }

        private void rtlDevice_SamplesAvailable(object sender, SamplesAvailableEventArgs e)
        {
            _callback(this, e.Buffer, e.Length);
        }
    }
}
