/* ExtIO DLL C# Wrapper for SDR# 
 * -----------------------------
 * 
 * Written by Ian Gilmour (MM6DOS) and Youssef Touil (CN8???)
 * 
 * THIS CODE IS PLACED IN PUBLIC DOMAIN.
 * 
 * 
 * - Provide callback for SamplesAvailable(Complex *samples, int len)
 * - Call UseLibrary("xx_extio.dll")
 * - InitHW() will be called and callback address provided to DLL
 * - Call OpenHW() -> StartHW()
 * - Audio samples will arrive from SamplesAvailable event 
 * 
 * Other events are available.  See ExtIO_StatusEvent enums
 *               
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SDRSharp.Radio.PortAudio;

namespace SDRSharp.Radio
{
    public unsafe delegate void SamplesAvailableDelegate(Complex* data, int len);
    public delegate void SampleRateChangedDelegate(int newSamplerate);
    public delegate void LOFrequencyChangedDelegate(int frequency);
    public delegate void LOFrequencyChangeAcceptedDelegate();
    public delegate void ProhibitLOChangesDelegate();

    public unsafe static class ExtIO
    {
        private const float InputGain = 0.01f;

        #region ExtIO Enums

        public enum HWTypes
        {
            Aud16BInt = 3, /* 16 Bit integer audio samples */
            Soundcard = 4, /* Soundcard based device */
            Aud24BInt = 5, /* 24 Bit integer audio samples */
            Aud32BInt = 6, /* 32 Bit integer audio samples */
            Aud32BFloat = 7 /* 32 Bit float audio samples */
        }

        public enum StatusEvent
        {
            SrChange = 100, /* Sample rate has changed by hardware */
            LOChange = 101, /* LO has changed by hardware */
            ProhibLO = 102, /* Prohibit LO changes */
            LOChangeOk = 103, /* LO change accepted */
            TuneChange = 105, /* Tune freq changed by hardware */
            DemodChange = 106, /* Demodulator changed by hardware */
            RsqStart = 107, /* Request to start */
            RsqStop = 108, /* Request to stop */
            FiltChange = 109 /* Filters have been changed by hardware */
        }

        #endregion

        #region Win32 Native Methods

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        #endregion

        #region Events

        /* This delegate is called when samples arrive from the DLL */
        /* Place your own hook here */

        public static event SamplesAvailableDelegate SamplesAvailable;
        public static event SampleRateChangedDelegate SampleRateChanged;
        public static event LOFrequencyChangedDelegate LOFreqChanged;
        public static event LOFrequencyChangeAcceptedDelegate LOFreqChangedAccepted;
        public static event ProhibitLOChangesDelegate ProhibitLOChanged;
        
        #endregion

        #region ExtIO Callback

        /* Note: The calling convention seems to differ for the callback!? */
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ExtIOManagedCallbackDelegate(int a, int b, float c, byte* data);

        #endregion ExtIO_Callback

        #region Entry point delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int InitHWDelegate(StringBuilder name, StringBuilder model, out int type);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int OpenHWDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int StartHWDelegate(int freq);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void StopHWDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void CloseHWDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetHWLODelegate(int freq);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetHWLODelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetHWSRDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetStatusDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void ShowGUIDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void HideGUIDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void SetCallbackDelegate(ExtIOManagedCallbackDelegate callbackAddr);

        #endregion

        #region Private fields

        private static InitHWDelegate _initHW;
        private static OpenHWDelegate _openHW;
        private static StartHWDelegate _startHW;
        private static StopHWDelegate _stopHW;
        private static CloseHWDelegate _closeHW;
        private static SetHWLODelegate _setHWLO;
        private static GetHWLODelegate _getHWLO;
        private static GetHWSRDelegate _getHWSR;
        private static GetStatusDelegate _getStatus;
        private static ShowGUIDelegate _showGUI;
        private static HideGUIDelegate _hideGUI;
        private static SetCallbackDelegate _setCallback;

        private static IntPtr _dllHandle;
        private static HWTypes _hwType;
        private static string _name;
        private static string _model;
        private static UnsafeBuffer _iqBuffer;
        private static Complex* _iqPtr;
        private static int _sampleCount;
        private static bool _isHWStarted;
        private static string _dllName;
        private static readonly Dictionary<string, IntPtr> _handles = new Dictionary<string,IntPtr>();

        private static readonly ExtIOManagedCallbackDelegate _callbackInst = ExtIOCallback;

        #endregion

        #region Initialisation

        static ExtIO()
        {
            GCHandle.Alloc(_callbackInst);
        }

        public static void UseLibrary(string fileName)
        {
            _dllName = fileName;

            if (_handles.ContainsKey(_dllName))
            {
                _dllHandle = _handles[_dllName];
            }
            else
            {
                _dllHandle = LoadLibrary(_dllName);
            }

            if (_dllHandle == IntPtr.Zero)
                throw new Exception("Unable to load ExtIO library");

            IntPtr pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "InitHW");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _initHW = (InitHWDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(InitHWDelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "OpenHW");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _openHW = (OpenHWDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(OpenHWDelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "StartHW");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _startHW = (StartHWDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(StartHWDelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "StopHW");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _stopHW = (StopHWDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(StopHWDelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "CloseHW");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _closeHW = (CloseHWDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(CloseHWDelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "SetCallback");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _setCallback = (SetCallbackDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(SetCallbackDelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "SetHWLO");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _setHWLO = (SetHWLODelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(SetHWLODelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "GetHWLO");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _getHWLO = (GetHWLODelegate)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(GetHWLODelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "GetHWSR");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _getHWSR = (GetHWSRDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(GetHWSRDelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "GetStatus");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _getStatus = (GetStatusDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(GetStatusDelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "ShowGUI");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _showGUI = (ShowGUIDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(ShowGUIDelegate));

            pAddressOfFunctionToCall = GetProcAddress(_dllHandle, "HideGUI");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                _hideGUI = (HideGUIDelegate) Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(HideGUIDelegate));
            
            if (_initHW == null || _openHW == null || _startHW == null || _setHWLO == null ||
               _getStatus == null || _setCallback == null || _stopHW == null || _closeHW == null)
            {
                //FreeLibrary(_dllHandle);
                _dllHandle = IntPtr.Zero;
                throw new ApplicationException("ExtIO DLL is not valid");
            }
            
            var name = new StringBuilder(256);
            var model = new StringBuilder(256);
            int type;

            var result = _initHW(name, model, out type);

            _name = name.ToString();
            _model = model.ToString();

            if (result < 1)
            {
                //FreeLibrary(_dllHandle);
                _dllHandle = IntPtr.Zero;
                throw new ApplicationException("InitHW() returned " + result);
            }

            _hwType = (HWTypes) type;

            /* Give the library the managed callback address */
            _setCallback(_callbackInst);
        }

        #endregion

        #region ExtIO Methods / Properties

        public static HWTypes HWType
        {
            get { return _hwType; }
        }

        public static bool IsHardwareStarted
        {
            get
            {
                return _isHWStarted;
            }
        }

        public static bool IsHardwareOpen
        {
            get
            {
                return _dllHandle != IntPtr.Zero;
            }
        }

        public static string DllName
        {
            get
            {
                return _dllName;
            }
        }

        public static string HWName
        {
            get
            {
                if (_dllHandle != IntPtr.Zero)
                    return _name;
                return string.Empty;
            }
        }

        public static string HWModel
        {
            get
            {
                if (_dllHandle != IntPtr.Zero)
                    return _model;
                return string.Empty;
            }
        }

        public static int GetHWSR()
        {
            if (_dllHandle != IntPtr.Zero && _getHWSR != null)
                return _getHWSR();
            return 0;
        }

        public static int GetHWLO()
        {
            if (_dllHandle != IntPtr.Zero && _getHWLO != null)
                return _getHWLO();
            return 0;
        }

        public static void SetHWLO(int freq)
        {
            if (_dllHandle != IntPtr.Zero)
                _setHWLO(freq);
        }

        public static void ShowGUI()
        {
            if (_dllHandle != IntPtr.Zero && _showGUI != null)
                _showGUI();
        }

        public static void HideGUI()
        {
            if (_dllHandle != IntPtr.Zero && _hideGUI != null)
                _hideGUI();
        }

        public static void StartHW(int freq)
        {
            if (_dllHandle == IntPtr.Zero)
                return;

            int result = _startHW(freq);
            if (result < 0)
                throw new Exception("ExtIO StartHW() returned " + result);
            
            _isHWStarted = true;
            _sampleCount = result;

            /* Allocate the sample buffers */
            /* We must do it here since we do not know the size until the hardware is started! */
            _iqBuffer = UnsafeBuffer.Create(_sampleCount, sizeof(Complex));
            _iqPtr = (Complex*) _iqBuffer;
        }

        public static int OpenHW()
        {
            if (_dllHandle != IntPtr.Zero && !_isHWStarted)
                return _openHW();
            return 0;
        }

        public static void StopHW()
        {
            if (_dllHandle != IntPtr.Zero && _isHWStarted)
            {
                _stopHW();
                _isHWStarted = false;
            }
        }

        public static void CloseHW()
        {
            if (_dllHandle != IntPtr.Zero)
            {
                _closeHW();
                _setCallback(null);
                _isHWStarted = false;
                _dllHandle = IntPtr.Zero;
            }
        }

        #endregion

        #region ExtIO Callback

        private static void ExtIOCallback(int count, int status, float iqOffs, byte* dataPtr)
        {
            /* Non-negative count means samples are ready. */           
            /* Negative count means status change */
            if (count >= 0)
            {
                /* Buffers cannot be allocated until AFTER the hardware is started because size is unknown
                 * Therefore the callback could be called before buffers are allocated
                 */
                //if (_iqBuffer == null || _iqBuffer.Length != _sampleCount)
                //{
                //    _iqBuffer = UnsafeBuffer.Create(_sampleCount, sizeof(Complex));
                //    _iqPtr = (Complex*) _iqBuffer;
                //}
                
                /* Convert samples to double */

                /* 16 bit integer samples */
                if (_hwType == HWTypes.Aud16BInt)
                {
                    for (var i = 0; i < _iqBuffer.Length; i++)
                    {
                        _iqPtr[i].Real = *(Int16*)(dataPtr + i * 4 + 2) / 32767.0f * InputGain;
                        _iqPtr[i].Imag = *(Int16*)(dataPtr + i * 4) / 32767.0f * InputGain;
                    }
                }

                /* 24 bit integer samples */
                else if (_hwType == HWTypes.Aud24BInt)
                {
                    for (int i = 0; i < _iqBuffer.Length; i++)
                    {
                        _iqPtr[i].Real = *(Int24*)(dataPtr + i * 6 + 3) / 8388608.0f * InputGain;
                        _iqPtr[i].Imag = *(Int24*)(dataPtr + i * 6) / 8388608.0f * InputGain;
                    }
                }

                /* 32 bit integer samples */
                else if (_hwType == HWTypes.Aud32BInt)
                {
                    for (int i = 0; i < _iqBuffer.Length; i++)
                    {
                        _iqPtr[i].Real = *(Int32*)(dataPtr + i * 8 + 4) / 2147483648.0f * InputGain;
                        _iqPtr[i].Imag = *(Int32*)(dataPtr + i * 8) / 2147483648.0f * InputGain;
                    }
                }

                /* 32 bit float samples */
                else if (_hwType == HWTypes.Aud32BFloat)
                {
                    for (int i = 0; i < _iqBuffer.Length; i++)
                    {
                        _iqPtr[i].Real = *(float*)(dataPtr + i * 8 + 4) * InputGain;
                        _iqPtr[i].Imag = *(float*)(dataPtr + i * 8) * InputGain;
                    }
                }

                if (SamplesAvailable != null)
                {
                    SamplesAvailable(_iqPtr, _iqBuffer.Length);
                }
            }
            else
            {
                /* Handle ExtIO status events. */
                /* Only the interesting ones for now */
                switch ((StatusEvent) status)
                {
                    case StatusEvent.LOChange:
                        if (LOFreqChanged != null)
                            LOFreqChanged(GetHWLO());
                        break;

                    case StatusEvent.LOChangeOk:
                        if (LOFreqChangedAccepted != null)
                            LOFreqChangedAccepted();
                        break;

                    case StatusEvent.SrChange:
                        if (SampleRateChanged != null)
                            SampleRateChanged(GetHWSR());
                        break;

                    case StatusEvent.ProhibLO:
                        if (ProhibitLOChanged != null)
                            ProhibitLOChanged();
                        break;
                }
            }
        }
    
        #endregion
    }
}
