using System.Windows.Forms;
using SDRSharp.Common;

namespace SDRSharp.WavRecorder
{
    public class WavRecorderPlugin: ISharpPlugin
    {
        private const string DefaultDisplayName = "Recording";
   
        private ISharpControl _control;
        private RecordingPanel _guiControl;
      
        public bool HasGui
        {
            get { return true; }
        }

        public UserControl GuiControl
        {
            get { return _guiControl; }
        }

        public string DisplayName
        {
            get { return DefaultDisplayName; }
        }

        public void Initialize(ISharpControl control)
        {
            _control = control;
            _guiControl = new RecordingPanel(_control);
        }

        public void Close()
        {
            if (_guiControl != null)
            {
                _guiControl.AbortRecording();
            }
        }
    }
}
