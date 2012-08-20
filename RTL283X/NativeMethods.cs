using System;
using System.Runtime.InteropServices;

namespace SDRSharp.RTL283X
{
    public enum IRType
    {
        RC6 = 0,
        RC5,
        NEC
    }

    public enum DemodType
    {
        DVBT = 0,
        DTMB,
        DVBC
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FMDemodInfo
    {
        public byte txMode;
        public byte rxMode;
        public byte inputQuality;
        public byte lockPLL;
        public byte RDSsync;
        public int sliding;
        public int inputSNR;
        public int inputSNR2;
        public int pilotPower;
        public int carrierPLL;
        public int errPLL;
        public int CR;
        public int RDSSNR;
        public int RDSTR;
        public int rawBlockErr;
        public int blockErrRate;
        public int mute;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct DABDemodInfo
    {
        public byte demodStatus;
        public byte transmissionMode;
        public byte avaliableSC;
        public int carrierFreqOffset;
        public int integerFreqOffset;
        public int SNR;
        public int samplingFreqOffset;
        public int D_l;
    }

    public unsafe static class NativeMethods
    {
        private const string RTL283XDLL = "RTL283XAccess.dll";

        [DllImport(RTL283XDLL, EntryPoint = "RTK_BDAFilterInit", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_BDAFilterInit(IntPtr hDlgHandle);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_BDAFilterRelease", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_BDAFilterRelease(IntPtr hDlgHandle);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_HaveAtLeastOneDevice", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_HaveAtLeastOneDevice();

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DeviceIsOk", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DeviceIsOk();

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DeviceIsOnProcess", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DeviceIsOnProcess();

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DeviceIsOffProcess", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DeviceIsOffProcess();

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Tuner_Byte_Read", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Tuner_Byte_Read(ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Tuner_Byte_Write", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Tuner_Byte_Write(ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Demod_Byte_Read", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Demod_Byte_Read(byte page, ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Demod_Byte_Write", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Demod_Byte_Write(byte page, ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK2836_Demod_Byte_Read", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK2836_Demod_Byte_Read(byte page, ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK2836_Demod_Byte_Write", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK2836_Demod_Byte_Write(byte page, ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK2840_Demod_Byte_Read", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK2840_Demod_Byte_Read(byte page, ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK2840_Demod_Byte_Write", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK2840_Demod_Byte_Write(byte page, ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_USB_Byte_Read", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_USB_Byte_Read(ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_USB_Byte_Write", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_USB_Byte_Write(ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_SYS_Byte_Read", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_SYS_Byte_Read(ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_SYS_Byte_Write", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_SYS_Byte_Write(ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_I2C_Read", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_I2C_Read(byte baseaddress, ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_I2C_Write", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_I2C_Write(byte baseaddress, ushort offset, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_STDI2C_Read", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_STDI2C_Read(byte baseaddress, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_STDI2C_Write", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_STDI2C_Write(byte baseaddress, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Set_Frequency", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Set_Frequency(uint freqency);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Set_Bandwidth", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Set_Bandwidth(uint bandwidth);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Set_QAM", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Set_QAM(uint qam);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Set_SymbolRate", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Set_SymbolRate(uint symbolRate);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Set_Alpha", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Set_Alpha(uint alpha);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Get_QAM", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Get_QAM(out uint pQam);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Get_SymbolRate", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Get_SymbolRate(out uint pSymbolRate);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Get_Alpha", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Get_Alpha(out uint pAlpha);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_ScanChannel", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_ScanChannel();

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DeviceUpdate", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DeviceUpdate();

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Read_CurrentBandwidth", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Read_CurrentBandwidth(out uint pBandwidth);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Read_CurrentFreqence", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Read_CurrentFreqence(out uint pFreqency);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Read_DemodPage", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Read_DemodPage(out byte pPage);
        
        [DllImport(RTL283XDLL, EntryPoint = "RTK_Set_DemodPage", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Set_DemodPage(byte page);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetSignalStrength", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetSignalStrength(out uint pSignalStrength);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetSignalQuality", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetSignalQuality(out uint pSignalQuality);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetDemodStage", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetDemodStage(out uint pStage);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetRfAgc", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetRfAgc(out int pRfAgc);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetIfAgc", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetIfAgc(out int pIfAgc);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetCrOffsetHz", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetCrOffsetHz(out int pCrOffsetHz);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetTrOffsetPpm", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetTrOffsetPpm(out int pTrOffsetPpm);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetBer", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetBer(out double pBer);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetSnrDb", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetSnrDb(out double pSnr);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_IsTpsLocked", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_IsTpsLocked(out int pAnswer);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_IsSignalLocked", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_IsSignalLocked(out int pAnswer);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DVBT_GetRSErrorNumber", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DVBT_GetRSErrorNumber(out uint pRSErrorNumber);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_GetData", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_GetData(byte* data, uint buflength, out uint getlength, out uint discardlength);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_SetDABEventHandle", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_SetDABEventHandle(IntPtr handle);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_ReleaseDABEventHandle", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_ReleaseDABEventHandle(IntPtr handle);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_SetEventHandle", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_SetEventHandle(IntPtr handle);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_ReleaseEventHandle", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_ReleaseEventHandle(IntPtr handle);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Get_TunerType", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Get_TunerType(out uint pTunerType);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_SetFMDemodInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_SetFMDemodInfo(ref FMDemodInfo pFMDemodInfo);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_GetFMDemodInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_GetFMDemodInfo(out FMDemodInfo pFMDemodInfo);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_SetDABDemodInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_SetDABDemodInfo(ref DABDemodInfo pDABDemodInfo);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_GetDABDemodInfo", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_GetDABDemodInfo(out DABDemodInfo pDABDemodInfo);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_Get_Suspend_Status", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_Get_Suspend_Status(out uint status);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_IR_Read", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_IR_Read(ushort address, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_IR_Write", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_IR_Write(ushort address, uint length, byte* data);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_GetDemodSupportType", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_GetDemodSupportType(out uint pDemodSupportType);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_SetDemodType", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_SetDemodType(uint demodType);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_GetDemodType", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_GetDemodType(out uint pDemodType);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DevicePowerON", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DevicePowerON();

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DevicePowerOFF", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DevicePowerOFF();

        [DllImport(RTL283XDLL, EntryPoint = "RTK_InitialAPModeIRParameter", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_InitialAPModeIRParameter();

        [DllImport(RTL283XDLL, EntryPoint = "RTK_GetAPModeIRCode", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_GetAPModeIRCode(out ushort irSingle, byte* irCode, uint length);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_GetAPModeIRCurrentIRType", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_GetAPModeIRCurrentIRType(byte* irType);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DTMB_GetCrOffsetHz", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DTMB_GetCrOffsetHz(out int pCrOffsetHz);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DTMB_GetSnrDb", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DTMB_GetSnrDb(out double pSnrDb);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DTMB_GetPer", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DTMB_GetPer(out double pPer);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DTMB_GetChannelPower", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DTMB_GetChannelPower(out double pPower);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DTMB_GetSingleCarrierConstellationIQ", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DTMB_GetSingleCarrierConstellationIQ(double* pConstellationI, double* pConstellationQ);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DTMB_GetMutiCarrierConstellationIQ", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DTMB_GetMutiCarrierConstellationIQ(double* pConstellationI, double* pConstellationQ);

        [DllImport(RTL283XDLL, EntryPoint = "RTK_DTMB_GetConstellationIQ", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RTK_DTMB_GetConstellationIQ(double* pConstellationI, double* pConstellationQ);
    }
}
