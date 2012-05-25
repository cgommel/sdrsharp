using System;

namespace SDRSharp.Radio
{
    public class ExtIOController : IFrontendController
    {
        private readonly string _filename;

        public ExtIOController(string filename)
        {
            _filename = filename;
            ExtIO.UseLibrary(_filename);
            ExtIO.OpenHW();
        }

        public void Open()
        {
            ExtIO.UseLibrary(_filename);
        }

        public void Close()
        {
            ExtIO.StopHW();
            ExtIO.HideGUI();
            //ExtIO.CloseHW();
        }

        public bool IsOpen
        {
            get { return ExtIO.IsHardwareOpen; }
        }

        public string Filename
        {
            get { return _filename; }
        }

        public bool IsSoundCardBased
        {
            get { return ExtIO.HWType == ExtIO.HWTypes.Soundcard; }
        }

        public string SoundCardHint
        {
            get { return "DirectSound"; }
        }

        public double Samplerate
        {
            get { return ExtIO.GetHWSR(); }
        }

        public long Frequency
        {
            get
            {
                return ExtIO.GetHWLO();
            }
            set
            {
                unchecked
                {
                    ExtIO.SetHWLO((int) value);
                }
            }
        }

        public void ShowSettingGUI(IntPtr parentHandle)
        {
            ExtIO.ShowGUI();
        }

        public void HideSettingGUI()
        {
            ExtIO.HideGUI();
        }
    }
}
