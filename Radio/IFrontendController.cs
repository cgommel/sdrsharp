using System;
using System.Windows.Forms;

namespace SDRSharp.Radio
{
    public unsafe delegate void SamplesAvailableDelegate(IFrontendController sender, Complex* data, int len);

    public interface IFrontendController
    {
        void Open();
        void Start(SamplesAvailableDelegate callback);
        void Stop();
        void Close();
        bool IsSoundCardBased { get; }
        string SoundCardHint { get; }
        double Samplerate { get; }
        long Frequency { get; set; }
        void ShowSettingGUI(IWin32Window parent);
        void HideSettingGUI();
    }
}
