using System;
using System.Windows.Forms;

namespace SDRSharp
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }
    }
}
