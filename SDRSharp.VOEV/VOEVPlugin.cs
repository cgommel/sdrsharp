using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDRSharp.Common;
using SDRSharp.Radio;

namespace SDRSharp.VOEV
{
    public unsafe class VOEVPlugin:ISharpPlugin,IAudioProcessor
    {
        private double _sampleRate;
        private bool _bypass;
        private ISharpControl _controlInterface;
        private VOEVPanel _voevPanel;

        public void Initialize(ISharpControl control)
        {
            _controlInterface = control;
            _voevPanel = new VOEVPanel(_controlInterface);
            _controlInterface.RegisterStreamHook(this);           
        }

        public void Close()
        {
           
        }

        public bool HasGui
        {
            get { return true; }
        }

        public System.Windows.Forms.UserControl GuiControl
        {
            get { return _voevPanel; }
        }

        public string DisplayName
        {
            get { return "VÖV Decoder (Display Name)"; }
        }
        
        public double SampleRate
        {
            set { _sampleRate = value; }
        }

       
        public bool Bypass
        {
            get
            {
                return _bypass;
            }
            set
            {
                _bypass = value; 
            }
        }
        int t = 0;
        public void Process(float* audioBuffer, int length)
        {
            double average = 0;
            for (int i = 0; i < length;i++ )
                average += Math.Abs(audioBuffer[i]);

            average = Math.Log10(average + 0.00000000001);
            t=(int)(average*100)+200;
            _voevPanel.updatecnt(t);

        }
    }
}
