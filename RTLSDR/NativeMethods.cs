using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SDRSharp.RTLSDR
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void RtlSdrReadAsyncDelegate(byte* buf, uint len, IntPtr ctx);

    public enum RtlSdrTunerType
    {
        Unknown = 0,
        E4000,
        FC0012,
        FC0013,
        FC2580
    }

    public class NativeMethods
    {
        private const string LibRtlSdr = "rtlsdr";

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_device_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rtlsdr_get_device_count();

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_device_name", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr rtlsdr_get_device_name_native(uint index);

        public static string rtlsdr_get_device_name(uint index)
        {
            var strptr = rtlsdr_get_device_name_native(index);
            return Marshal.PtrToStringAnsi(strptr);
        }

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_device_usb_strings", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_device_usb_strings(uint index, StringBuilder manufact, StringBuilder product, StringBuilder serial);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_open(out IntPtr dev, uint index);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_close(IntPtr dev);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_xtal_freq", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_xtal_freq(IntPtr dev, uint rtlFreq, uint tunerFreq);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_xtal_freq", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_xtal_freq(IntPtr dev, out uint rtlFreq, out uint tunerFreq);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_usb_strings", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_usb_strings(IntPtr dev, StringBuilder manufact, StringBuilder product, StringBuilder serial);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_center_freq", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_center_freq(IntPtr dev, uint freq);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_center_freq", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rtlsdr_get_center_freq(IntPtr dev);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_freq_correction", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_freq_correction(IntPtr dev, int ppm);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_freq_correction", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_freq_correction(IntPtr dev);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_tuner_gains", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_tuner_gains(IntPtr dev, [In, Out] int[] gains);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_tuner_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern RtlSdrTunerType rtlsdr_get_tuner_type(IntPtr dev);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_tuner_gain", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_tuner_gain(IntPtr dev, int gain);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_tuner_gain", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_tuner_gain(IntPtr dev);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_tuner_gain_mode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_tuner_gain_mode(IntPtr dev, int manual);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_agc_mode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_agc_mode(IntPtr dev, int on);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_direct_sampling", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_direct_sampling(IntPtr dev, int on);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_offset_tuning", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_offset_tuning(IntPtr dev, int on);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_sample_rate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_sample_rate(IntPtr dev, uint rate);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_sample_rate", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rtlsdr_get_sample_rate(IntPtr dev);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_testmode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_testmode(IntPtr dev, int on);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_reset_buffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_reset_buffer(IntPtr dev);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_read_sync", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_read_sync(IntPtr dev, IntPtr buf, int len, out int nRead);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_wait_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_wait_async(IntPtr dev, RtlSdrReadAsyncDelegate cb, IntPtr ctx);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_read_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_read_async(IntPtr dev, RtlSdrReadAsyncDelegate cb, IntPtr ctx, uint bufNum, uint bufLen);

        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_cancel_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_cancel_async(IntPtr dev);
    }
}
