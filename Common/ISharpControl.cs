using System.ComponentModel;

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
        bool IsSquelchOpen { get; }
        bool SwapIq { get; set; }
        bool UseAgc { get; set; }
        bool AgcHang { get; set; }
        int AgcThreshold { get; set; }
        int AgcDecay { get; set; }
        int AgcSlope { get; set; }

        int FFTResolution { get; }
        float FFTRange { get; }
        float FFTOffset { get; }
        int StepSize { get; }
        bool IsPlaying { get; }

        int SAttack { get; set; }
        int SDecay { get; set; }
        int WAttack { get; set; }
        int WDecay { get; set; }

        bool UseTimeMarkers { get; set; }

        string RdsProgramService { get; }
        string RdsRadioText { get; }

        int RFBandwidth { get; }

        bool SourceIsSoundCard { get; }
        bool SourceIsWaveFile { get; }
        bool SourceIsTunable { get; }
        
        void GetSpectrumSnapshot(byte[] destArray);

        void StartRadio();
        void StopRadio();
 
        void RegisterStreamHook(object streamHook);
        void UnregisterStreamHook(object streamHook);

        event PropertyChangedEventHandler PropertyChanged;
    }
}
