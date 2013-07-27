using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SDRSharp.HackRF
{

    #region HackRF Transfer Structure

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct hackrf_transfer
	{
		public IntPtr device;
		public byte* buffer;
		public int buffer_length;
		public int valid_length;
        public IntPtr rx_ctx;
        public IntPtr tx_ctx;
	}

    #endregion

    #region HackRF Callback Delegate

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
	public unsafe delegate int hackrf_sample_block_cb_fn(hackrf_transfer* ptr);

    #endregion

    public class NativeMethods
    {
        private const string LibHackRF = "libhackrf";

        #region Native Methods

        [DllImport(LibHackRF, EntryPoint = "hackrf_init", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_init();

        [DllImport(LibHackRF, EntryPoint = "hackrf_exit", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_exit();

        [DllImport(LibHackRF, EntryPoint = "hackrf_open", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_open(out IntPtr dev);

        [DllImport(LibHackRF, EntryPoint = "hackrf_close", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_close(IntPtr dev);

        [DllImport(LibHackRF, EntryPoint = "hackrf_start_rx", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_start_rx(IntPtr dev, hackrf_sample_block_cb_fn cb, IntPtr rx_ctx);

        [DllImport(LibHackRF, EntryPoint = "hackrf_stop_rx", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_stop_rx(IntPtr dev);
        
        [DllImport(LibHackRF, EntryPoint = "hackrf_is_streaming", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_is_streaming(IntPtr dev);

        [DllImport(LibHackRF, EntryPoint = "hackrf_board_id_name", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr hackrf_board_id_name_native(uint index);
		
		public static string hackrf_board_id_name(uint index)
        {
            try
            {
                var strptr = hackrf_board_id_name_native(index);
                return Marshal.PtrToStringAnsi(strptr);
            }
            catch (EntryPointNotFoundException e)
            {
                Console.WriteLine("{0}:\n   {1}", e.GetType().Name,  e.Message);
                return "HackRF";
            }
        }
        
        [DllImport(LibHackRF, EntryPoint = "hackrf_set_sample_rate", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_sample_rate(IntPtr dev, double rate);

        [DllImport(LibHackRF, EntryPoint = "hackrf_set_freq", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_freq(IntPtr dev, long freq);

        [DllImport(LibHackRF, EntryPoint = "hackrf_set_amp_enable", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_amp_enable(IntPtr dev, byte value);

        [DllImport(LibHackRF, EntryPoint = "hackrf_set_baseband_filter_bandwidth", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_baseband_filter_bandwidth(IntPtr dev, uint bandwidth_hz);

        [DllImport(LibHackRF, EntryPoint = "hackrf_compute_baseband_filter_bw_round_down_lt", CallingConvention = CallingConvention.StdCall)]
        public static extern uint hackrf_compute_baseband_filter_bw_round_down_lt(uint bandwidth_hz);

        [DllImport(LibHackRF, EntryPoint = "hackrf_compute_baseband_filter_bw", CallingConvention = CallingConvention.StdCall)]
        public static extern uint hackrf_compute_baseband_filter_bw(uint bandwidth_hz);
        
        [DllImport(LibHackRF, EntryPoint = "hackrf_set_lna_gain", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_lna_gain(IntPtr dev, uint value);

        [DllImport(LibHackRF, EntryPoint = "hackrf_set_vga_gain", CallingConvention = CallingConvention.StdCall)]
        public static extern int hackrf_set_vga_gain(IntPtr dev, uint value);

        #endregion
    }
}
