using System;
using System.Windows.Forms;
using System.ComponentModel;

using SDRSharp.Common;

namespace SDRSharp.DNR
{
    public class DNRPlugin: ISharpPlugin
    {
        private const string _displayName = "Digital Noise Reduction";
        private ISharpControl _control;
        private AudioProcessor _audioProcessor;
                        
        private AudioProcessorPanel _guiControl;
        public string DisplayName
        {
            get { return _displayName; }
        }

        public bool HasGui
        {
            get { return true; }
        }

        public UserControl GuiControl
        {
            get { return _guiControl; }
        }

        public void Initialize(ISharpControl control)
        {
            _control = control;

            _control.PropertyChanged += new PropertyChangedEventHandler(PropertyChangedHandler);

            _audioProcessor = new AudioProcessor();
            _control.RegisterStreamHook(_audioProcessor);
            
            
            _guiControl = new AudioProcessorPanel(_audioProcessor);                                            
        }

        void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            // Events
            switch (e.PropertyName)
            {
                case "StartRadio":
                    if (_guiControl != null)
                    {
                        _guiControl.EnableControls();
                    }
                    break;

                case "StopRadio":
                    if (_guiControl != null)
                    {
                        _guiControl.DisableControls();
                    }
                    break;

                default:
                    break;
            }
        }
        

        public void Close()
        {
        }
        
    }
}
