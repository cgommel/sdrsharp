using System;
using System.Runtime.InteropServices;

namespace SDRSharp.SoftRock
{
    public static class NativeUsb
    {
        public const int I2CAddr = 0x55;
        public const int Vid = 0x16c0;
        public const int Pid = 0x05dc;

        public const string Manufacturer = "www.obdev.at";
        public const string Product = "DG8SAQ - I2C";
        public const string SerialNumber = "PE0FKO-0";

        [DllImport("SRDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr srOpen(
            int vid,
            int pid,
            [MarshalAs(UnmanagedType.LPStr)] string pManufacturer,
            [MarshalAs(UnmanagedType.LPStr)] string pProduct,
            [MarshalAs(UnmanagedType.LPStr)] string pSerialNumber);

        [DllImport("SRDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern void srClose();

        [DllImport("SRDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool srIsOpen();

        [DllImport("SRDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool srGetVersion(out int major, out int minor);

        [DllImport("SRDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool srGetFreq(out double mgz);

        [DllImport("SRDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool srSetFreq(double mhz, int i2CAddr);
    }
}
