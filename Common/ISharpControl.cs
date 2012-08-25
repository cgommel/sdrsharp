using SDRSharp.Radio;

namespace SDRSharp.Common
{
    public interface ISharpControl
    {
        DetectorType DetectorType { get; set; }
        WindowType FilterType { get; set; }
        int AudioGain { get; set; }
        long CenterFrequency { get; set; }
        int CWShift { get; set; }
        bool FilterAudio { get; set; }
        int FilterBandwidth { get; set; }
        int FilterOrder { get; set; }
        bool FmStereo { get; set; }
        long Frequency { get; set; }
        long FrequencyShift { get; set; }
        bool FrequencyShiftEnabled { get; set; }        
        bool MarkPeaks { get; set; }
        bool SnapToGrid { get; set; }
        bool SquelchEnabled { get; set; }
        int SquelchThreshold { get; set; }
        bool SwapIq { get; set; }
        int AgcThreshold { get; set; }
        int AgcDecay { get; set; }
        int AgcSlope { get; set; }
        
        bool IsPlaying { get; }

        int SAttack { get; set; }
        int SDecay { get; set; }
        int WAttack { get; set; }
        int WDecay { get; set; }
        
        void GetSpectrumSnapshot(byte[] destArray);

        void StartRadio();
        void StopRadio();
    }
}
