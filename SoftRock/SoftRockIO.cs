using System;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.SoftRock
{
    public class SoftRockIO : IFrontendController, IDisposable
    {
        private static readonly double _frequencyMultiplier = Utils.GetDoubleSetting("softrockFrequencyMultiplier", 4);
        private IntPtr _srHandle;

        ~SoftRockIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        public void Open()
        {
            if (IsOpen)
            {
                return;
            }

            _srHandle = NativeUsb.srOpen(
                NativeUsb.Vid,
                NativeUsb.Pid,
                string.Empty,
                string.Empty,
                string.Empty);
        }

        public void Start(SamplesAvailableDelegate callback)
        {
        }

        public void Stop()
        {
        }

        public void Close()
        {
            if (IsOpen)
            {
                NativeUsb.srClose();
                _srHandle = IntPtr.Zero;
            }
        }

        public bool IsOpen
        {
            get
            {
                return _srHandle != IntPtr.Zero;
            }
        }

        public bool IsSoundCardBased
        {
            get { return true; }
        }

        public long Frequency
        {
            get
            {
                return GetSi570Frequency();
            }
            set
            {
                SetSi570Frequency(value);
            }
        }

        public void ShowSettingGUI(IntPtr parentHandle)
        {
            MessageBox.Show("Will be implemented later", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static long GetSi570Frequency()
        {
            double mhz;
            NativeUsb.srGetFreq(out mhz);
            return (long)(mhz * 1e6 / _frequencyMultiplier);
        }

        private static void SetSi570Frequency(long frequency)
        {
            var mhz = frequency / 1e6 * _frequencyMultiplier;
            NativeUsb.srSetFreq(mhz, NativeUsb.I2CAddr);
        }


        public string SoundCardHint
        {
            get { return "DirectSound"; }
        }

        public double Samplerate
        {
            get { return 48000; }
        }

        public void HideSettingGUI()
        {
        }
    }
}
