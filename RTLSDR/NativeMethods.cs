using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SDRSharp.RTLSDR
{
    /// Return Type: void
    ///buf: unsigned char*
    ///len: unsigned int
    ///ctx: void*
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void RtlSdrReadAsyncDelegate(byte* buf, uint len, IntPtr ctx);

    public unsafe class NativeMethods
    {
        private const string LibRtlSdr = "rtlsdr.dll";

        /// Return Type: unsigned int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_device_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rtlsdr_get_device_count();

        /// Return Type: char*
        ///index: unsigned int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_device_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern string rtlsdr_get_device_name(uint index);

        /// Return Type: int
        ///index: unsigned int
        ///manufact: char*
        ///product: char*
        ///serial: char*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_device_usb_strings", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_device_usb_strings(uint index, StringBuilder manufact, StringBuilder product, StringBuilder serial);

        /// Return Type: int
        ///dev: void**
        ///index: unsigned int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_open(ref IntPtr dev, uint index);

        /// Return Type: int
        ///dev: void*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_close(IntPtr dev);

        /// Return Type: int
        ///dev: void*
        ///rtl_freq: unsigned int
        ///tuner_freq: unsigned int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_xtal_freq", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_xtal_freq(IntPtr dev, uint rtlFreq, uint tunerFreq);

        /// Return Type: int
        ///dev: void*
        ///rtl_freq: unsigned int*
        ///tuner_freq: unsigned int*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_xtal_freq", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_xtal_freq(IntPtr dev, ref uint rtlFreq, ref uint tunerFreq);

        /// Return Type: int
        ///dev: void*
        ///manufact: char*
        ///product: char*
        ///serial: char*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_usb_strings", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_usb_strings(IntPtr dev, StringBuilder manufact, StringBuilder product, StringBuilder serial);

        /// Return Type: int
        ///dev: void*
        ///freq: unsigned int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_center_freq", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_center_freq(IntPtr dev, uint freq);

        /// Return Type: unsigned int
        ///dev: void*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_center_freq", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rtlsdr_get_center_freq(IntPtr dev);

        /// Return Type: int
        ///dev: void*
        ///ppm: int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_freq_correction", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_freq_correction(IntPtr dev, int ppm);

        /// Return Type: int
        ///dev: void*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_freq_correction", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_freq_correction(IntPtr dev);

        /// Return Type: int
        ///dev: void*
        ///gain: int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_tuner_gain", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_tuner_gain(IntPtr dev, int gain);

        /// Return Type: int
        ///dev: void*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_tuner_gain", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_get_tuner_gain(IntPtr dev);

        /// Return Type: int
        ///dev: void*
        ///manual: int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_tuner_gain_mode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_tuner_gain_mode(IntPtr dev, int manual);

        /// Return Type: int
        ///dev: void*
        ///rate: unsigned int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_sample_rate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_sample_rate(IntPtr dev, uint rate);

        /// Return Type: unsigned int
        ///dev: void*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_sample_rate", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rtlsdr_get_sample_rate(IntPtr dev);

        /// Return Type: int
        ///dev: void*
        ///on: int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_set_testmode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_set_testmode(IntPtr dev, int on);

        /// Return Type: int
        ///dev: void*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_reset_buffer", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_reset_buffer(IntPtr dev);

        /// Return Type: int
        ///dev: void*
        ///buf: void*
        ///len: int
        ///n_read: int*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_read_sync", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_read_sync(IntPtr dev, IntPtr buf, int len, ref int nRead);

        /// Return Type: int
        ///dev: void*
        ///cb: rtlsdr_read_async_cb_t
        ///ctx: void*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_wait_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_wait_async(IntPtr dev, RtlSdrReadAsyncDelegate cb, IntPtr ctx);

        /// Return Type: int
        ///dev: void*
        ///cb: rtlsdr_read_async_cb_t
        ///ctx: void*
        ///buf_num: unsigned int
        ///buf_len: unsigned int
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_read_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_read_async(IntPtr dev, RtlSdrReadAsyncDelegate cb, IntPtr ctx, uint bufNum, uint bufLen);

        /// Return Type: int
        ///dev: void*
        [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_cancel_async", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rtlsdr_cancel_async(IntPtr dev);
    }
}
