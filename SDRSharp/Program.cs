using System;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
#if !LINUX
            Utils.TimeBeginPeriod(1);
#endif

            Control.CheckForIllegalCrossThreadCalls = false;
            Application.EnableVisualStyles();
            Application.Run(new MainForm());

#if !LINUX
            Utils.TimeEndPeriod(1);
#endif
        }
    }
}
