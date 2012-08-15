using System;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.RTL283X
{
    public unsafe class Rtl283xIO : IFrontendController, IDisposable
    {
        private SamplesAvailableDelegate _callback;

        public Rtl283xIO()
        {
        }

        ~Rtl283xIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Open()
        {
            var r = NativeMethods.RTK_DevicePowerON();
            if (!r)
            {
                throw new ApplicationException("No compatible devices detected");
            }
        }

        public void Close()
        {
            NativeMethods.RTK_DevicePowerOFF();
        }

        public void Start(SamplesAvailableDelegate callback)
        {
            _callback = callback;
        }

        public void Stop()
        {
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
        }

        public void HideSettingGUI()
        {
        }

        public double Samplerate
        {
            get { return 0; }
        }

        public long Frequency
        {
            get { return 0; }
            set {  }
        }
    }
}
