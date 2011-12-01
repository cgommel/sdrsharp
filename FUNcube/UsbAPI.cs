using System;
using System.Runtime.InteropServices;

namespace SDRSharp.FUNcube
{
    static class UsbAPI
    {
        public const int DigcfInterfaceDevice = 0x00000010;
        public const int DigcfPresent = 0x00000002;
        public const uint GenericRead = 0x80000000;
        public const uint GenericWrite = 0x40000000;
        public const uint FileShareRead = 0x00000001;
        public const uint FileShareWrite = 0x00000002;
        public const int OpenExisting = 3;

        [DllImport("setupapi.dll")]
        public static extern int SetupDiEnumDeviceInterfaces(
            int deviceInfoSet,
            int deviceInfoData,
            ref Guid lpHidGuid,
            int memberIndex,
            ref SpDeviceInterfaceData lpDeviceInterfaceData);

        [DllImport("setupapi.dll")]
        public static extern int SetupDiGetDeviceInterfaceDetail(
            int deviceInfoSet,
            ref SpDeviceInterfaceData lpDeviceInterfaceData,
            IntPtr aPtr,
            int detailSize,
            out int requiredSize,
            IntPtr bPtr);

        [DllImport("setupapi.dll")]
        public static extern int SetupDiGetDeviceInterfaceDetail(
            int deviceInfoSet,
            ref SpDeviceInterfaceData lpDeviceInterfaceData,
            ref PspDeviceInterfaceDetailData interfaceDetailData,
            int detailSize,
            ref int requiredSize,
            IntPtr bPtr);

        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(out Guid lpHidGuid);

        [DllImport("setupapi.dll")]
        public static extern int SetupDiGetClassDevs(
            ref Guid lpHidGuid,
            IntPtr enumerator,
            IntPtr hwndParent,
            int flags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            uint hTemplateFile);

        [DllImport("setupapi.dll")]
        public static extern int SetupDiDestroyDeviceInfoList(int deviceInfoSet);
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SpDeviceInterfaceData
    {
        public int cbSize;
        public Guid InterfaceClassGuid;
        public int Flags;
        public int Reserved;
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct PspDeviceInterfaceDetailData
    {
        public int cbSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DevicePath;
    }
}
