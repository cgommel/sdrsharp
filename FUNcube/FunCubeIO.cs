using System;
using System.Configuration;
using System.Globalization;
using FUNCubeDongleController;
using SDRSharp.Radio;

namespace SDRSharp.FUNcube
{
    #region Enum Types definition

    public enum TunerLNAGain
    {
	    N5_0DB = 0,
	    N2_5DB = 1,
	    P0_0DB = 4,
	    P2_5DB = 5,
	    P5_0DB = 6,
	    P7_5DB = 7,
	    P10_0DB = 8,
	    P12_5DB = 9,
	    P15_0DB = 10,
	    P17_5DB = 11,
	    P20_0DB = 12,
	    P25_0DB = 13,
	    P30_0DB = 14
    }

    public enum TunerLNAEnhance
    {
        TLEE_OFF = 0,
        TLEE_0 = 1,
        TLEE_1 = 3,
        TLEE_2 = 5,
        TLEE_3 = 7
    }

    public enum TunerBand
    {
        TBE_VHF2,
        TBE_VHF3,
        TBE_UHF,
        TBE_LBAND
    }

    public enum TunerRFFilter
    {
        // Band 0, VHF II
        TRFE_LPF268MHZ = 0,
        TRFE_LPF299MHZ = 8,
        // Band 1, VHF III
        TRFE_LPF509MHZ = 0,
        TRFE_LPF656MHZ = 8,
        // Band 2, UHF
        TRFE_BPF360MHZ = 0,
        TRFE_BPF380MHZ = 1,
        TRFE_BPF405MHZ = 2,
        TRFE_BPF425MHZ = 3,
        TRFE_BPF450MHZ = 4,
        TRFE_BPF475MHZ = 5,
        TRFE_BPF505MHZ = 6,
        TRFE_BPF540MHZ = 7,
        TRFE_BPF575MHZ = 8,
        TRFE_BPF615MHZ = 9,
        TRFE_BPF670MHZ = 10,
        TRFE_BPF720MHZ = 11,
        TRFE_BPF760MHZ = 12,
        TRFE_BPF840MHZ = 13,
        TRFE_BPF890MHZ = 14,
        TRFE_BPF970MHZ = 15,
        // Band 2, L band
        TRFE_BPF1300MHZ = 0,
        TRFE_BPF1320MHZ = 1,
        TRFE_BPF1360MHZ = 2,
        TRFE_BPF1410MHZ = 3,
        TRFE_BPF1445MHZ = 4,
        TRFE_BPF1460MHZ = 5,
        TRFE_BPF1490MHZ = 6,
        TRFE_BPF1530MHZ = 7,
        TRFE_BPF1560MHZ = 8,
        TRFE_BPF1590MHZ = 9,
        TRFE_BPF1640MHZ = 10,
        TRFE_BPF1660MHZ = 11,
        TRFE_BPF1680MHZ = 12,
        TRFE_BPF1700MHZ = 13,
        TRFE_BPF1720MHZ = 14,
        TRFE_BPF1750MHZ = 15
    }

    public enum TunerMixerGain
    {
        TMGE_P4_0DB = 0,
        TMGE_P12_0DB = 1
    }

    public enum TunerBiasCurrent
    {
        TBCE_LBAND = 0,
        TBCE_1 = 1,
        TBCE_2 = 2,
        TBCE_VUBAND = 3
    }

    public enum TunerMixerFilter
    {
        TMFE_27_0MHZ = 0,
        TMFE_4_6MHZ = 8,
        TMFE_4_2MHZ = 9,
        TMFE_3_8MHZ = 10,
        TMFE_3_4MHZ = 11,
        TMFE_3_0MHZ = 12,
        TMFE_2_7MHZ = 13,
        TMFE_2_3MHZ = 14,
        TMFE_1_9MHZ = 15
    }

    public enum TunerIFGainMode
    {
        TIGME_LINEARITY = 0,
        TIGME_SENSITIVITY = 1
    }

    public enum TunerIFRCFilter
    {
        TIRFE_21_4MHZ = 0,
        TIRFE_21_0MHZ = 1,
        TIRFE_17_6MHZ = 2,
        TIRFE_14_7MHZ = 3,
        TIRFE_12_4MHZ = 4,
        TIRFE_10_6MHZ = 5,
        TIRFE_9_0MHZ = 6,
        TIRFE_7_7MHZ = 7,
        TIRFE_6_4MHZ = 8,
        TIRFE_5_3MHZ = 9,
        TIRFE_4_4MHZ = 10,
        TIRFE_3_4MHZ = 11,
        TIRFE_2_6MHZ = 12,
        TIRFE_1_8MHZ = 13,
        TIRFE_1_2MHZ = 14,
        TIRFE_1_0MHZ = 15
    }

    public enum TunerIFGain1 
    {
        TIG1E_N3_0DB = 0,
        TIG1E_P6_0DB = 1
    }

    public enum TunerIFGain2
    {
        TIG2E_P0_0DB = 0,
        TIG2E_P3_0DB = 1,
        TIG2E_P6_0DB = 2,
        TIG2E_P9_0DB = 3
    }

    public enum TunerIFGain3
    {
        TIG3E_P0_0DB = 0,
        TIG3E_P3_0DB = 1,
        TIG3E_P6_0DB = 2,
        TIG3E_P9_0DB = 3
    }

    public enum TunerIFGain4 
    {
        TIG4E_P0_0DB = 0,
        TIG4E_P1_0DB = 1,
        TIG4E_P2_0DB = 2
    }

    public enum TunerIFGain5 
    {
        TIG5E_P3_0DB = 0,
        TIG5E_P6_0DB = 1,
        TIG5E_P9_0DB = 2,
        TIG5E_P12_0DB = 3,
        TIG5E_P15_0DB = 4
    }

    public enum TunerIFGain6
    {
        TIG6E_P3_0DB = 0,
        TIG6E_P6_0DB = 1,
        TIG6E_P9_0DB = 2,
        TIG6E_P12_0DB = 3,
        TIG6E_P15_0DB = 4
    }

    public enum TunerIFFilter
    {
        TIFE_5_50MHZ = 0,
        TIFE_5_30MHZ = 1,
        TIFE_5_00MHZ = 2,
        TIFE_4_80MHZ = 3,
        TIFE_4_60MHZ = 4,
        TIFE_4_40MHZ = 5,
        TIFE_4_30MHZ = 6,
        TIFE_4_10MHZ = 7,
        TIFE_3_90MHZ = 8,
        TIFE_3_80MHZ = 9,
        TIFE_3_70MHZ = 10,
        TIFE_3_60MHZ = 11,
        TIFE_3_40MHZ = 12,
        TIFE_3_30MHZ = 13,
        TIFE_3_20MHZ = 14,
        TIFE_3_10MHZ = 15,
        TIFE_3_00MHZ = 16,
        TIFE_2_95MHZ = 17,
        TIFE_2_90MHZ = 18,
        TIFE_2_80MHZ = 19,
        TIFE_2_75MHZ = 20,
        TIFE_2_70MHZ = 21,
        TIFE_2_60MHZ = 22,
        TIFE_2_55MHZ = 23,
        TIFE_2_50MHZ = 24,
        TIFE_2_45MHZ = 25,
        TIFE_2_40MHZ = 26,
        TIFE_2_30MHZ = 27,
        TIFE_2_28MHZ = 28,
        TIFE_2_24MHZ = 29,
        TIFE_2_20MHZ = 30,
        TIFE_2_15MHZ = 31
    }

    #endregion

    public class FunCubeIO : IFrontendController, IDisposable
    {
        #region FUNCube Dongle HID Commands
        private const byte FCD_HID_CMD_SET_FREQUENCY_HZ = 101; // Send with 4 byte unsigned little endian frequency in Hz, returns wit actual frequency set in Hz
        private const byte FCD_HID_CMD_GET_FREQUENCY_HZ = 102; // Returns 4 byte unsigned little endian frequency in Hz.
        
        private const byte FCD_HID_CMD_SET_LNA_GAIN = 110;
        private const byte FCD_HID_CMD_SET_LNA_ENHANCE = 111;
        private const byte FCD_HID_CMD_SET_BAND = 112;
        private const byte FCD_HID_CMD_SET_RF_FILTER = 113;
        private const byte FCD_HID_CMD_SET_MIXER_GAIN = 114;
        private const byte FCD_HID_CMD_SET_BIAS_CURRENT = 115;
        private const byte FCD_HID_CMD_SET_MIXER_FILTER = 116;
        private const byte FCD_HID_CMD_SET_IF_GAIN1 = 117;
        private const byte FCD_HID_CMD_SET_IF_GAIN_MODE = 118;
        private const byte FCD_HID_CMD_SET_IF_RC_FILTER = 119;
        private const byte FCD_HID_CMD_SET_IF_GAIN2 = 120;
        private const byte FCD_HID_CMD_SET_IF_GAIN3 = 121;
        private const byte FCD_HID_CMD_SET_IF_FILTER = 122;
        private const byte FCD_HID_CMD_SET_IF_GAIN4 = 123;
        private const byte FCD_HID_CMD_SET_IF_GAIN5 = 124;
        private const byte FCD_HID_CMD_SET_IF_GAIN6 = 125;

        private const byte FCD_HID_CMD_GET_LNA_GAIN = 150; // Retrieve a 1 byte value, see enums for reference
        private const byte FCD_HID_CMD_GET_LNA_ENHANCE = 151;
        private const byte FCD_HID_CMD_GET_BAND = 152;
        private const byte FCD_HID_CMD_GET_RF_FILTER = 153;
        private const byte FCD_HID_CMD_GET_MIXER_GAIN = 154;
        private const byte FCD_HID_CMD_GET_BIAS_CURRENT = 155;
        private const byte FCD_HID_CMD_GET_MIXER_FILTER = 156;
        private const byte FCD_HID_CMD_GET_IF_GAIN1 = 157;
        private const byte FCD_HID_CMD_GET_IF_GAIN_MODE = 158;
        private const byte FCD_HID_CMD_GET_IF_RC_FILTER = 159;
        private const byte FCD_HID_CMD_GET_IF_GAIN2 = 160;
        private const byte FCD_HID_CMD_GET_IF_GAIN3 = 161;
        private const byte FCD_HID_CMD_GET_IF_FILTER = 162;
        private const byte FCD_HID_CMD_GET_IF_GAIN4 = 163;
        private const byte FCD_HID_CMD_GET_IF_GAIN5 = 164;
        private const byte FCD_HID_CMD_GET_IF_GAIN6 = 165;

        #endregion

        private readonly double _freqCorrection = GetFrequencyCorrection();

        #region Properties

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
            using (var dialog = new FCDControllerDialog())
            {
                dialog.ShowDialog();
            }
        }

        public TunerLNAGain LNAGain
        {
            get 
            {
                return GetLNAGain(); 
            }
            set 
            { 
                SetLNAGain(value);
            }
        }

        public TunerRFFilter RFFilter
        {
            get
            {
                return GetRFFilter();
            }
            set
            {
                SetRFFilter(value);
            }
        }

        public TunerMixerGain MixerGain
        {
            get
            {
                return GetMixerGain();
            }
            set
            {
                SetMixerGain(value);
            }
        }

        public TunerMixerFilter MixerFilter
        {
            get
            {
                return GetMixerFilter();
            }
            set
            {
                SetMixerFilter(value);
            }
        }

        public TunerIFGain1 IFGain1
        {
            get
            {
                return GetIFGain1();
            }
            set
            {
                SetIFGain1(value);
            }
        }

        public TunerIFRCFilter IFRCFilter
        {
            get
            {
                return GetIFRCFilter();
            }
            set
            {
                SetIFRCFilter(value);
            }
        }

        public TunerIFGain2 IFGain2
        {
            get
            {
                return GetIFGain2();
            }
            set
            {
                SetIFGain2(value);
            }
        }

        public TunerIFGain3 IFGain3
        {
            get
            {
                return GetIFGain3();
            }
            set
            {
                SetIFGain3(value);
            }
        }

        public TunerIFGain4 IFGain4
        {
            get
            {
                return GetIFGain4();
            }
            set
            {
                SetIFGain4(value);
            }
        }

        public TunerIFFilter IFFilter
        {
            get
            {
                return GetIFFilter();
            }
            set
            {
                SetIFFilter(value);
            }
        }

        public TunerIFGain5 IFGain5
        {
            get
            {
                return GetIFGain5();
            }
            set
            {
                SetIFGain5(value);
            }
        }

        public TunerIFGain6 IFGain6
        {
            get
            {
                return GetIFGain6();
            }
            set
            {
                SetIFGain6(value);
            }
        }

        public TunerLNAEnhance LNAEnhance
        {
            get
            {
                return GetLNAEnhance();
            }
            set
            {
                SetLNAEnhance(value);
            }
        }

        public TunerBand Band
        {
            get
            {
                return GetBand();
            }
            set
            {
                SetBand(value);
            }
        }

        public TunerBiasCurrent BiasCurrent
        {
            get
            {
                return GetBiasCurrent();
            }
            set
            {
                SetBiasCurrent(value);
            }
        }

        public TunerIFGainMode IFGainMode
        {
            get
            {
                return GetIFGainMode();
            }
            set
            {
                SetIFGainMode(value);
            }
        }

        #endregion

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
                au8BufOut[1] = FCD_HID_CMD_GET_FREQUENCY_HZ;

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
                au8BufOut[1] = FCD_HID_CMD_SET_FREQUENCY_HZ;
                au8BufOut[2] = (byte)(frequency & 0x000000ff);
                au8BufOut[3] = (byte)((frequency & 0x0000ff00) >> 8);
                au8BufOut[4] = (byte)((frequency & 0x00ff0000) >> 16);
                au8BufOut[5] = (byte)((frequency & 0xff000000) >> 24);
                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);
                return au8BufIn[2] == 1;
            }
        }

        private static TunerLNAGain GetLNAGain()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_LNA_GAIN;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerLNAGain) au8BufIn[3];
            }
        }

        private static bool SetLNAGain(TunerLNAGain gain)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_LNA_GAIN;
                au8BufOut[2] = (byte) gain;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerRFFilter GetRFFilter()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_RF_FILTER;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerRFFilter)au8BufIn[3];
            }
        }

        private static bool SetRFFilter(TunerRFFilter filter)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_RF_FILTER;
                au8BufOut[2] = (byte)filter;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerMixerGain GetMixerGain()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_MIXER_GAIN;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerMixerGain)au8BufIn[3];
            }
        }

        private static bool SetMixerGain(TunerMixerGain mixerGain)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_MIXER_GAIN;
                au8BufOut[2] = (byte)mixerGain;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerMixerFilter GetMixerFilter()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_MIXER_FILTER;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerMixerFilter)au8BufIn[3];
            }
        }

        private static bool SetMixerFilter(TunerMixerFilter mixerFilter)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_MIXER_FILTER;
                au8BufOut[2] = (byte)mixerFilter;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerIFGain1 GetIFGain1()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_IF_GAIN1;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerIFGain1)au8BufIn[3];
            }
        }

        private static bool SetIFGain1(TunerIFGain1 gain)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_IF_GAIN1;
                au8BufOut[2] = (byte)gain;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerIFRCFilter GetIFRCFilter()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_IF_RC_FILTER;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerIFRCFilter)au8BufIn[3];
            }
        }

        private static bool SetIFRCFilter(TunerIFRCFilter filter)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_IF_RC_FILTER;
                au8BufOut[2] = (byte)filter;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerIFGain2 GetIFGain2()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_IF_GAIN2;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerIFGain2)au8BufIn[3];
            }
        }

        private static bool SetIFGain2(TunerIFGain2 gain)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_IF_GAIN2;
                au8BufOut[2] = (byte)gain;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerIFGain3 GetIFGain3()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_IF_GAIN3;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerIFGain3)au8BufIn[3];
            }
        }

        private static bool SetIFGain3(TunerIFGain3 gain)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_IF_GAIN3;
                au8BufOut[2] = (byte)gain;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerIFGain4 GetIFGain4()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_IF_GAIN4;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerIFGain4)au8BufIn[3];
            }
        }

        private static bool SetIFGain4(TunerIFGain4 gain)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_IF_GAIN4;
                au8BufOut[2] = (byte)gain;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerIFFilter GetIFFilter()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_IF_FILTER;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerIFFilter)au8BufIn[3];
            }
        }

        private static bool SetIFFilter(TunerIFFilter filter)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_IF_FILTER;
                au8BufOut[2] = (byte)filter;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerIFGain5 GetIFGain5()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_IF_GAIN5;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerIFGain5)au8BufIn[3];
            }
        }

        private static bool SetIFGain5(TunerIFGain5 gain)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_IF_GAIN5;
                au8BufOut[2] = (byte)gain;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerIFGain6 GetIFGain6()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_IF_GAIN6;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerIFGain6)au8BufIn[3];
            }
        }

        private static bool SetIFGain6(TunerIFGain6 gain)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_IF_GAIN6;
                au8BufOut[2] = (byte)gain;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerLNAEnhance GetLNAEnhance()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_LNA_ENHANCE;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerLNAEnhance)au8BufIn[3];
            }
        }

        private static bool SetLNAEnhance(TunerLNAEnhance enhance)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_LNA_ENHANCE;
                au8BufOut[2] = (byte)enhance;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerBand GetBand()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_BAND;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerBand)au8BufIn[3];
            }
        }

        private static bool SetBand(TunerBand band)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_BAND;
                au8BufOut[2] = (byte)band;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerBiasCurrent GetBiasCurrent()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_BIAS_CURRENT;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerBiasCurrent)au8BufIn[3];
            }
        }

        private static bool SetBiasCurrent(TunerBiasCurrent biasCurrent)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_BIAS_CURRENT;
                au8BufOut[2] = (byte)biasCurrent;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static TunerIFGainMode GetIFGainMode()
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = FCD_HID_CMD_GET_IF_GAIN_MODE;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return (TunerIFGainMode)au8BufIn[3];
            }
        }

        private static bool SetIFGainMode(TunerIFGainMode gainMode)
        {
            using (var usb = UsbDevice.Open("Vid_04d8&Pid_fb56"))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = FCD_HID_CMD_SET_IF_GAIN_MODE;
                au8BufOut[2] = (byte)gainMode;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }
    }
}
