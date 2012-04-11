using System;
using System.Configuration;
using System.Globalization;
using SDRSharp.Radio;

namespace SDRSharp.FUNcube
{
    enum TunerLNAGain
    {
	    N5_0DB=0,
	    N2_5DB=1,
	    P0_0DB=4,
	    P2_5DB=5,
	    P5_0DB=6,
	    P7_5DB=7,
	    P10_0DB=8,
	    P12_5DB=9,
	    P15_0DB=10,
	    P17_5DB=11,
	    P20_0DB=12,
	    P25_0DB=13,
	    P30_0DB=14
    }

    public class FunCubeIO : IFrontendController, IDisposable
    {
        private readonly double _freqCorrection = GetFrequencyCorrection();

        private static double GetFrequencyCorrection()
        {
            double result;
            if (!double.TryParse(ConfigurationManager.AppSettings["funcubeFrequencyCorrection"],
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out result))
            {
                result = 0.999976;
            }
            return result;
        }

        public void Open()
        {
            SetLNAGain(TunerLNAGain.P10_0DB);
        }

        public void Close()
        {
        }

        public bool IsOpen
        {
            get
            {
                return true;
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
                return (int) (GetFrequency() / _freqCorrection);
            }
            set
            {
                SetFrequency((int) (value * _freqCorrection));
            }
        }

        public void ShowSettingsDialog(IntPtr parentHandle)
        {
        }

        public void Dispose()
        {
            Close();
        }

        private static int GetFrequency()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = 102;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                var result = au8BufIn[3] |
                    au8BufIn[4] << 8 |
                    au8BufIn[5] << 16 |
                    au8BufIn[6] << 24;

                return result;
            }
        }

        private static bool SetFrequency(int frequency)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = 101;
                au8BufOut[2] = (byte)(frequency & 0x000000ff);
                au8BufOut[3] = (byte)((frequency & 0x0000ff00) >> 8);
                au8BufOut[4] = (byte)((frequency & 0x00ff0000) >> 16);
                au8BufOut[5] = (byte)((frequency & 0xff000000) >> 24);
                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);
                return au8BufIn[2] == 1;
            }
        }

        private static bool SetLNAGain(TunerLNAGain gain)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = 110;
                au8BufOut[2] = (byte) gain;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }
    }
}
