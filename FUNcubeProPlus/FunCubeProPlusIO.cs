using System;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.FUNcubeProPlus
{

    #region Public Enums

    public enum TunerRFFilter
    {
	    TRFE_0_4,
	    TRFE_4_8,
	    TRFE_8_16,
	    TRFE_16_32,
	    TRFE_32_75,
	    TRFE_75_125,
	    TRFE_125_250,
	    TRFE_145,
	    TRFE_410_875,
	    TRFE_435,
	    TRFE_875_2000
    } 

    public enum TunerIFFilter
    {
        TIFE_200KHZ=0,
        TIFE_300KHZ=1,
        TIFE_600KHZ=2,
        TIFE_1536KHZ=3,
        TIFE_5MHZ=4,
        TIFE_6MHZ=5,
        TIFE_7MHZ=6,
        TIFE_8MHZ=7
    } 


    #endregion

    public class FunCubeProPlusIO : IFrontendController, IDisposable
    {
        #region FUNcube Pro+ Dongle Commands

        private const byte FCD_HID_CMD_QUERY = 1;

        private const byte FCD_HID_CMD_SET_FREQUENCY = 100;
                
        private const byte FCD_HID_CMD_SET_FREQUENCY_HZ = 101; 
        private const byte FCD_HID_CMD_GET_FREQUENCY_HZ = 102; 

        private const byte FCD_HID_CMD_SET_LNA_GAIN = 110; 
        private const byte FCD_HID_CMD_SET_RF_FILTER = 113; 
        private const byte FCD_HID_CMD_SET_MIXER_GAIN = 114; 
        private const byte FCD_HID_CMD_SET_IF_GAIN = 117; 
        private const byte FCD_HID_CMD_SET_IF_FILTER = 122; 
        private const byte FCD_HID_CMD_SET_BIAS_TEE = 126; 

        private const byte FCD_HID_CMD_GET_LNA_GAIN = 150; 
        private const byte FCD_HID_CMD_GET_RF_FILTER = 153; 
        private const byte FCD_HID_CMD_GET_MIXER_GAIN = 154; 
        private const byte FCD_HID_CMD_GET_IF_GAIN = 157; 
        private const byte FCD_HID_CMD_GET_IF_FILTER = 162; 

        private const byte FCD_HID_CMD_GET_BIAS_TEE = 166; 
        
        #endregion

        private const int FCDPlusMaxIFGain = 59;
        private const string FCDPlusVidPid = "Vid_04d8&Pid_fb31";

        private double _freqCorrection = Utils.GetDoubleSetting("funcubeProPlusFrequencyCorrection", -120.0);
        private long _frequency;
        private readonly FCDProPlusControllerDialog _dialog;

        public FunCubeProPlusIO()
        {
             _dialog = new FCDProPlusControllerDialog(this);
        }

        ~FunCubeProPlusIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            Close();
            _dialog.Close();
            _dialog.Dispose();
            GC.SuppressFinalize(this);
        }

        #region Properties

        public double FrequencyCorrection
        {
            get
            {
                return _freqCorrection;
            }
            set
            {
                _freqCorrection = value;
                Frequency = _frequency;
            }
        }

        public TunerIFFilter IFFilter
        {
            get { return GetIFFilter(); }
        }

        public TunerRFFilter RFFilter
        {
            get { return GetRFFilter(); }
        }

        public bool LNAEnabled
        {
            get { return GetLNAState(); }
            set { SetLNAState(value); }
        }

        public bool MixerGainEnabled
        {
            get { return GetMixerGainState(); }
            set { SetMixerGainState(value); }
        }

        public bool BiasTeeEnabled
        {
            get { return GetBiasTeeState(); }
            set { SetBiasTeeState(value); }
        }

        public int IFGain
        {
            get { return (int)GetIFGain(); }
            set { SetIFGain((byte)value); }
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

        public long Frequency
        {
            get
            {
                return (long) (GetFrequency() / (1 + _freqCorrection * 0.000001));
            }
            set
            {
                SetFrequency((uint) (value * (1 + _freqCorrection * 0.000001)));
                _frequency = value;
            }
        }

        public string SoundCardHint
        {
            get { return ".*DirectSound.*funcube.*"; }
        }

        public double Samplerate
        {
            get { return 192000.0; }
        }

        #endregion

        public void Open()
        {
            
        }

        public void ShowSettingGUI(IWin32Window parent)
        {
            _dialog.Show();
            _dialog.Activate();
        }

        public void HideSettingGUI()
        {
            _dialog.Hide();
        }
                        
        public void Start(SamplesAvailableDelegate callback)
        {
        }

        public void Stop()
        {
        }

        public void Close()
        {
            _dialog.Hide();
        }

        #region Private methods

        private static bool WriteCommand(byte command, byte value)
        {
            using (var usb = UsbDevice.Open(FCDPlusVidPid))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0; // First byte is report ID. Ignored by HID Class firmware as only config'd for one report
                au8BufOut[1] = command;
                au8BufOut[2] = value;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                return au8BufIn[2] == 1;
            }
        }

        private static byte ReadFlag(byte command)
        {
            using (var usb = UsbDevice.Open(FCDPlusVidPid))
            {
                var au8BufOut = new byte[65]; // endpoint size + 1
                var au8BufIn = new byte[65]; // endpoint size + 1

                au8BufOut[0] = 0;
                au8BufOut[1] = command;

                usb.Write(au8BufOut, 0, au8BufOut.Length);
                usb.Read(au8BufIn, 0, au8BufIn.Length);

                if (au8BufIn[2] != 1)
                {
                    return 0;
                }

                return au8BufIn[3];
            }
        }

        private static long GetFrequency()
        {
            using (var usb = UsbDevice.Open(FCDPlusVidPid))
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

                var result = (uint) (au8BufIn[3] |
                    au8BufIn[4] << 8 |
                    au8BufIn[5] << 16 |
                    au8BufIn[6] << 24);

                return result;
            }
        }

        private static bool SetFrequency(uint frequency)
        {
            using (var usb = UsbDevice.Open(FCDPlusVidPid))
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

        private static int GetIFGain()
        {
            return ReadFlag(FCD_HID_CMD_GET_IF_GAIN);
        }

        private static void SetIFGain(byte ifGain)
        {
            if (ifGain > FCDPlusMaxIFGain)
                ifGain = FCDPlusMaxIFGain;
            WriteCommand(FCD_HID_CMD_SET_IF_GAIN, (byte)ifGain);
        }

        private static bool GetLNAState()
        {
            return ReadFlag(FCD_HID_CMD_GET_LNA_GAIN) == 1 ? true : false;
        }

        private static void SetLNAState(bool enable)
        {
            byte state = (byte)(enable == true? 1 : 0);
            WriteCommand(FCD_HID_CMD_SET_LNA_GAIN, state);            
        }

        private static bool GetMixerGainState()
        {
            return ReadFlag(FCD_HID_CMD_GET_MIXER_GAIN) == 1 ? true : false;
        }

        private static void SetMixerGainState(bool enable)
        {
            byte state = (byte)(enable == true ? 1 : 0);
            WriteCommand(FCD_HID_CMD_SET_MIXER_GAIN, state);
        }

        private static bool GetBiasTeeState()
        {
            return ReadFlag(FCD_HID_CMD_GET_BIAS_TEE) == 1 ? true : false;
        }

        private static void SetBiasTeeState(bool enable)
        {
            byte state = (byte)(enable == true ? 1 : 0);
            WriteCommand(FCD_HID_CMD_SET_BIAS_TEE, state);
        }

        private static TunerRFFilter GetRFFilter()
        {
            return (TunerRFFilter)ReadFlag(FCD_HID_CMD_GET_RF_FILTER);
        }

        private static TunerIFFilter GetIFFilter()
        {
            return (TunerIFFilter)ReadFlag(FCD_HID_CMD_GET_IF_FILTER);
        }

        #endregion

    }
}
