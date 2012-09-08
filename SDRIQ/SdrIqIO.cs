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
            
            NativeMethods.sdriq_initialise();

            _gui = new SDRIQControllerDialog(this);            
        }

        ~SdrIqIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            NativeMethods.sdriq_destroy();
            if (_gui != null)
            {
                _gui.Dispose();
            }
            GC.SuppressFinalize(this);
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

        public void SelectDevice(uint index)
        {
            if (_device != null && _device.Index == index)
            {
                return;
            }
            Close();
            _device = new SdrIqDevice(index);
            _device.SamplesAvailable += sdriqDevice_SamplesAvailable;
            _gui.ConfigureGUI();
            _gui.ConfigureDevice();
        }

        public void Start(SDRSharp.Radio.SamplesAvailableDelegate callback)
        {
            if (_device == null)
            {
                throw new ApplicationException("No device selected");
            }
            _callback = callback;
            
            _device.Start();
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
                    _device.Frequency = (uint)value;
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
