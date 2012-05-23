using System;
using System.Diagnostics;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var process = Process.GetCurrentProcess();
            process.PriorityBoostEnabled = true;
            process.PriorityClass = ProcessPriorityClass.RealTime;
#if !LINUX
            Utils.TimeBeginPeriod(1);
#endif

            Control.CheckForIllegalCrossThreadCalls = false;
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
            
#if !LINUX
            Utils.TimeEndPeriod(1);
#endif
            Application.Exit(); // ExtIO may have some thread still running preventing the program from terminating
        }
    }
}
