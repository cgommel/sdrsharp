using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SDRSharp.SDRIQ
{

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void SdrIqReadAsyncDelegate(short* buf, uint len, IntPtr ctx);

    public class NativeMethods
    {
        private const string LibSDRIQ = "sdriq";
        
        [DllImport(LibSDRIQ, EntryPoint = "sdriq_initialise", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sdriq_initialise();

        [DllImport(LibSDRIQ, EntryPoint = "sdriq_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sdriq_destroy();
        
        [DllImport(LibSDRIQ, EntryPoint = "sdriq_get_device_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint sdriq_get_device_count();

        [DllImport(LibSDRIQ, EntryPoint = "sdriq_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdriq_open(uint devIndex, uint buffersCount, out IntPtr dev);

        [DllImport(LibSDRIQ, EntryPoint = "sdriq_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdriq_close(IntPtr dev);

        [DllImport(LibSDRIQ, EntryPoint = "sdriq_async_read", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdriq_async_read(IntPtr dev, IntPtr context, SdrIqReadAsyncDelegate callback, int readBlocks);

        [DllImport(LibSDRIQ, EntryPoint = "sdriq_async_cancel", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdriq_async_cancel(IntPtr dev);

        [DllImport(LibSDRIQ, EntryPoint = "sdriq_set_center_frequency", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdriq_set_center_frequency(IntPtr dev, uint frequency);
        
        [DllImport(LibSDRIQ, EntryPoint = "sdriq_set_out_samplerate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdriq_set_out_samplerate(IntPtr dev, uint rate);

        [DllImport(LibSDRIQ, EntryPoint = "sdriq_set_if_gain", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdriq_set_if_gain(IntPtr dev, sbyte value);

        [DllImport(LibSDRIQ, EntryPoint = "sdriq_set_rf_gain", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdriq_set_rf_gain(IntPtr dev, sbyte value);

        [DllImport(LibSDRIQ, EntryPoint = "sdriq_get_serial_number", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sdriq_get_serial_number_native(uint devNo);

        public static string sdriq_get_serial_number(uint index)
        {
            var strptr = sdriq_get_serial_number_native(index);
            return Marshal.PtrToStringAnsi(strptr);
        }

    }
}
