using System;

namespace SDRSharp.Radio
{
    public interface IFrontendController
    {
        void Open();
        void Close();
        bool IsOpen { get; }
        bool IsSoundCardBased { get; }
        string SoundCardHint { get; }
        double Samplerate { get; }
        long Frequency { get; set; }
        void ShowSettingGUI(IntPtr parentHandle);
        void HideSettingGUI();
    }
}
