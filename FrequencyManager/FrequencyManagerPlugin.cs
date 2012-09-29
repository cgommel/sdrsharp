using System;
using System.Windows.Forms;

using SDRSharp.Common;


namespace SDRSharp.FrequencyManager
{
    public class FrequencyManagerPlugin: ISharpPlugin
    {
        private const string _displayName = "Frequency Manager";
        private ISharpControl _controlInterface;
        private FrequencyManagerPanel _frequencyManagerPanel;

        public bool HasGui
        {
            get { return true; }
        }

        public UserControl GuiControl
        {
            get { return _frequencyManagerPanel; }
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        public void Close()
        {
        }

        public void Initialize(ISharpControl control)
        {
            _controlInterface = control;
            _frequencyManagerPanel = new FrequencyManagerPanel(_controlInterface); 
        }
    }
}
