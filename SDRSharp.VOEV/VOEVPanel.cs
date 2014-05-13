using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SDRSharp.Common;

namespace SDRSharp.VOEV
{
    [DesignTimeVisible(true)]
    [Category ("SDRSharp")]
    [Description("VOEV View Panel")]
    public unsafe partial class VOEVPanel : UserControl
    {
        private ISharpControl _control;
        private Graphics _graphics;

         public VOEVPanel(ISharpControl control)
        {
            InitializeComponent();
            // set cache mode only if no internet avaible
            _control = control;
            //Bitmap _buffer = new Bitmap(scopePanel.ClientRectangle.Width, scopePanel.ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            //_graphics = scopePanel.CreateGraphics();
           // ConfigureGraphics(_graphics);
             
        }
        public void updatecnt(int cnt)
        {
            button1.Text = cnt.ToString();
        }
    }
}
