using System;
using SDRSharp.Radio;

namespace SDRSharp.SoftRock
{
    public class SoftRockIO : IFrontendController, IDisposable
    {
        private static readonly double _frequencyMultiplier = Utils.GetDoubleSetting("softrockFrequencyMultiplier", 4);
        private IntPtr _srHandle;

        public void Dispose()
        {
            Close();
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

        public int Frequency
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

        public void ShowSettingsDialog(IntPtr parentHandle)
        {
        }

        private int GetSi570Frequency()
        {
            double mhz;
            NativeUsb.srGetFreq(out mhz);
            return (int)(mhz * 1e6 / _frequencyMultiplier);
        }

        private void SetSi570Frequency(int frequency)
        {
            var mhz = frequency / 1e6 * _frequencyMultiplier;
            NativeUsb.srSetFreq(mhz, NativeUsb.I2CAddr);
        }
    }
}
