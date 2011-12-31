using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SDRSharp.FUNcube
{
    static class UsbAPI
    {
        public const int DigcfInterfaceDevice = 0x00000010;
        public const int DigcfPresent = 0x00000002;
        public const int SpdrpHardwareId = 0x00000001;

        public const uint GenericRead = 0x80000000;
        public const uint GenericWrite = 0x40000000;
        public const uint FileShareRead = 0x00000001;
        public const uint FileShareWrite = 0x00000002;
        public const int OpenExisting = 3;

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr deviceInfoSet,
            ref SpDevInfoData deviceInfoData,
            uint property,
            out UInt32 propertyRegDataType,
            StringBuilder propertyBuffer,
            uint propertyBufferSize,
            out UInt32 requiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(
            IntPtr deviceInfoSet,
            uint memberIndex,
            ref SpDevInfoData deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetupDiEnumDeviceInterfaces(
           IntPtr hDevInfo,
           IntPtr devInfo,
           ref Guid interfaceClassGuid,
           UInt32 memberIndex,
           ref SpDeviceInterfaceData deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetupDiGetDeviceInterfaceDetail(
            IntPtr hDevInfo,
            ref SpDeviceInterfaceData deviceInterfaceData,
            ref SpDeviceInterfaceDetailData deviceInterfaceDetailData,
            int deviceInterfaceDetailDataSize,
            IntPtr requiredSize,
            IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetupDiGetDeviceInterfaceDetail(
            IntPtr hDevInfo,
            ref SpDeviceInterfaceData deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            int deviceInterfaceDetailDataSize,
            out int requiredSize,
            IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(
           ref Guid classGuid,
           IntPtr enumerator,
           IntPtr hwndParent,
           int flags);

        [DllImport("setupapi.dll")]
        public static extern int SetupDiDestroyDeviceInfoList(IntPtr hDevInfo);

        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(out Guid lpHidGuid);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            UInt32 dwDesiredAccess,
            UInt32 dwShareMode,
            IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes,
            IntPtr hTemplateFile);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SpDevInfoData
    {
        public uint cbSize;
        public Guid ClassGuid;
        public uint DevInst;
        public uint Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SpDeviceInterfaceData
    {
        public int cbSize;
        public Guid InterfaceClassGuid;
        public int Flags;
        public int Reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct SpDeviceInterfaceDetailData
    {
        public int cbSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024 * 4)]
        public string DevicePath;
    }
}
