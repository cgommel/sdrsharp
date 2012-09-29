using System.Windows.Forms;


namespace SDRSharp.Common
{
    public interface ISharpPlugin
    {
        void Initialize(ISharpControl control);        
        void Close();

        bool HasGui { get; }
        UserControl GuiControl { get; }

        string DisplayName { get; }
    }
}
