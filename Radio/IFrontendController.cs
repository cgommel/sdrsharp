using System;

namespace SDRSharp.Radio
{
    public interface IFrontendController
    {
        void Open();
        void Close();
        bool IsOpen { get; }
        bool IsSoundCardBased { get; }
        int Frequency { get; set; }
        void ShowSettingsDialog(IntPtr parentHandle);
    }
}
