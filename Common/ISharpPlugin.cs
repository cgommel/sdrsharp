using System.Windows.Forms;


namespace SDRSharp.Common
{
    public interface ISharpPlugin
    {

        void Initialise(ISharpControl control);        
        void Closing();

        bool HasGui { get; }
        UserControl GuiControl { get; }

        string DisplayName { get; }
    }
}
