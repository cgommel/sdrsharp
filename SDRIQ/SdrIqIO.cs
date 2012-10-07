using System;
using SDRSharp.Radio;

namespace SDRSharp.SDRIQ
{
    public unsafe class SdrIqIO : IFrontendController, IDisposable
    {
        private SdrIqDevice _device;
        private readonly SDRIQControllerDialog _gui;
        private Radio.SamplesAvailableDelegate _callback;

        public SdrIqIO()
        {
            _gui = new SDRIQControllerDialog(this);
        }

        ~SdrIqIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_gui != null)
            {
                _gui.Dispose();
            }
            try
            {
                NativeMethods.sdriq_destroy();
            }
            catch (DllNotFoundException)
            {
            }
            GC.SuppressFinalize(this);
        }

        public void Open()
        {
            NativeMethods.sdriq_destroy();
            NativeMethods.sdriq_initialise();
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
            NativeMethods.sdriq_destroy();
            if (devices.Length > 0)
            {
                throw new ApplicationException(devices.Length + " compatible devices have been found but are all busy");
            }
            throw new ApplicationException("No compatible devices found");
        }

        public void SelectDevice(uint index)
        {
            Close();
            _device = new SdrIqDevice(index);
            _device.SamplesAvailable += sdriqDevice_SamplesAvailable;
            _gui.ConfigureGUI();
            _gui.ConfigureDevice();
        }

        public void Start(Radio.SamplesAvailableDelegate callback)
        {
            if (_device == null)
            {
                throw new ApplicationException("No device selected");
            }
            _callback = callback;

            try
            {
                _device.Start();
            }
            catch
            {
                Open();
                _device.Start();
            }
        }

        public void Stop()
        {
            _device.Stop();
        }

        public void Close()
        {
            if (_device != null)
            {
                _device.Stop();
                _device.SamplesAvailable -= sdriqDevice_SamplesAvailable;
                _device.Dispose();
                _device = null;
            }
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
            get { return _device == null ? 0.0 : _device.Samplerate; }
        }

        public long Frequency
        {
            get { return _device == null ? 0 : _device.Frequency; }
            set
            {
                if (_device != null)
                {
                    _device.Frequency = (uint) value;
                }
            }
        }

        public SdrIqDevice Device
        {
            get { return _device; }
        }

        public void ShowSettingGUI(System.Windows.Forms.IWin32Window parent)
        {
            _gui.Show();
        }

        public void HideSettingGUI()
        {
            _gui.Hide();
        }

        private void sdriqDevice_SamplesAvailable(object sender, SamplesAvailableEventArgs e)
        {
            _callback(this, e.Buffer, e.Length);
        }
    }
}
