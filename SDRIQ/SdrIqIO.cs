using System;
using SDR14XLib;
using SDRSharp.Radio;

namespace SDRSharp.SDRIQ
{
    public class SdrIqIO : IFrontendController, IDisposable
    {
        private SDR14X _device; 

        public void Open()
        {
            _device = new SDR14X();
        }

        public void Start(SamplesAvailableDelegate callback)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool IsSoundCardBased
        {
            get { throw new NotImplementedException(); }
        }

        public string SoundCardHint
        {
            get { throw new NotImplementedException(); }
        }

        public double Samplerate
        {
            get { throw new NotImplementedException(); }
        }

        public long Frequency
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void ShowSettingGUI(System.Windows.Forms.IWin32Window parent)
        {
            throw new NotImplementedException();
        }

        public void HideSettingGUI()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
