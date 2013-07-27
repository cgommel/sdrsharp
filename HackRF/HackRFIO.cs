using System;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.HackRF
{
    public unsafe class HackRFIO : IFrontendController, IDisposable
    {
        private readonly HackRFControllerDialog _gui;
        private HackRFDevice _hackRFDevice;
        private long _frequency = 105500000;
        private double _frequencyCorrection;
        private Radio.SamplesAvailableDelegate _callback;

        public HackRFIO()
        {
            _gui = new HackRFControllerDialog(this);
        }

        ~HackRFIO()
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
            Close();
            _hackRFDevice = new HackRFDevice();
            _hackRFDevice.SamplesAvailable += HackRFDevice_SamplesAvailable;
            _hackRFDevice.Frequency = _frequency;
            _gui.ConfigureGUI();
            _gui.ConfigureDevice();
        }

        public HackRFDevice Device
        {
            get { return _hackRFDevice; }
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
            if (_hackRFDevice != null)
            {
                _hackRFDevice.SamplesAvailable -= HackRFDevice_SamplesAvailable;
                _hackRFDevice.Dispose();
                _hackRFDevice = null;
            }
        }

        public void Start(Radio.SamplesAvailableDelegate callback)
        {
            if (_hackRFDevice == null)
            {
                throw new ApplicationException("No device selected");
            }
            _callback = callback;
            try
            {
                _hackRFDevice.Start();
            }
            catch
            {
                Open();
                _hackRFDevice.Start();
            }
        }

        public void Stop()
        {
            _hackRFDevice.Stop();
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
            if (!_gui.IsDisposed)
            {
                _gui.Show();
            }
        }

        public void HideSettingGUI()
        {
            if (!_gui.IsDisposed)
            {
                _gui.Hide();
            }
        }

        public double Samplerate
        {
            get { return _hackRFDevice == null ? 0.0 : _hackRFDevice.SampleRate; }
        }

        public long Frequency
        {

            get { return _frequency; }
            set
            {                
                if (_hackRFDevice != null)
                {
                    _hackRFDevice.Frequency = (long)(value * (1 + _frequencyCorrection * 0.000001));
                    _frequency = value;
                }                
            }
        }

        public double FrequencyCorrection
        {
            get { return _frequencyCorrection; }
            set
            {
                _frequencyCorrection = value;
                Frequency = _frequency;
            }
        }

        private void HackRFDevice_SamplesAvailable(object sender, SamplesAvailableEventArgs e)
        {
            _callback(this, e.Buffer, e.Length);
        }
    }
}
