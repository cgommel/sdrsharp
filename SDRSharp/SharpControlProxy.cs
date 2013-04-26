using System.ComponentModel;
using System.Windows.Forms;
using SDRSharp.Radio;
using SDRSharp.Common;

namespace SDRSharp
{
    public class SharpControlProxy : ISharpControl, INotifyPropertyChanged
    {
        private readonly MainForm _owner;

        public event PropertyChangedEventHandler PropertyChanged;

        public SharpControlProxy(MainForm owner)
        {
            _owner = owner;

            _owner.PropertyChanged += PropertyChangedEventHandler;
            
        }

        #region Public Properties

        public DetectorType DetectorType
        {
            get { return _owner.DetectorType; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.DetectorType = value; }));
                }
                else
                {
                    _owner.DetectorType = value;
                }
            }                                       
        }

        public WindowType FilterType
        {
            get { return _owner.FilterType; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.FilterType = value; }));
                }
                else
                {
                    _owner.FilterType = value;
                }
            }
        }

        public int AudioGain
        {
            get { return _owner.AudioGain; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.AudioGain = value; }));
                }
                else
                {
                    _owner.AudioGain = value;
                }
            }
        }

        public long CenterFrequency
        {
            get { return _owner.CenterFrequency; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.CenterFrequency = value; }));
                }
                else
                {
                    _owner.CenterFrequency = value;
                }
            }
        }

        public int CWShift
        {
            get { return _owner.CWShift; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.CWShift = value; }));
                }
                else
                {
                    _owner.CWShift = value;
                }
            }
        }

        public bool FilterAudio
        {
            get { return _owner.FilterAudio; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.FilterAudio = value; }));
                }
                else
                {
                    _owner.FilterAudio = value;
                }
            }
        }


        public int FilterBandwidth
        {
            get { return _owner.FilterBandwidth; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.FilterBandwidth = value; }));
                }
                else
                {
                    _owner.FilterBandwidth = value;
                }
            }            
        }

        public int FilterOrder
        {
            get { return _owner.FilterOrder; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.FilterOrder = value; }));
                }
                else
                {
                    _owner.FilterOrder = value;
                }
            }
        }

        public bool FmStereo
        {
            get { return _owner.FmStereo; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.FmStereo = value; }));
                }
                else
                {
                    _owner.FmStereo = value;
                }
            }
        }

        public long Frequency
        {
            get { return _owner.Frequency; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.Frequency = value; }));
                }
                else
                {
                    _owner.Frequency = value;
                }
            }
        }

        public long FrequencyShift
        {
            get { return _owner.FrequencyShift; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.FrequencyShift = value; }));
                }
                else
                {
                    _owner.FrequencyShift = value;
                }
            }
        }

        public bool FrequencyShiftEnabled
        {
            get { return _owner.FrequencyShiftEnabled; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.FrequencyShiftEnabled = value; }));
                }
                else
                {
                    _owner.FrequencyShiftEnabled = value;
                }
            }
        }

        public bool UseAgc
        {
            get { return _owner.UseAgc; }
            set 
            {
                if(_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.UseAgc = value; }));
                }
                else
                {
                    _owner.UseAgc = value;
                }
            }
        }

        public bool AgcHang
        {
            get { return _owner.AgcHang; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.AgcHang = value; }));
                }
                else
                {
                    _owner.AgcHang = value;
                }
            }
        }

        public int AgcThreshold
        {
            get { return _owner.AgcThreshold; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.AgcThreshold = value; }));
                }
                else
                {
                    _owner.AgcThreshold = value;
                }
            }
        }

        public int AgcDecay
        {
            get { return _owner.AgcDecay; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.AgcDecay = value; }));
                }
                else
                {
                    _owner.AgcDecay = value;
                }
            }
        }

        public int AgcSlope
        {
            get { return _owner.AgcSlope; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.AgcSlope = value; }));
                }
                else
                {
                    _owner.AgcSlope = value;
                }
            }
        }

        public bool MarkPeaks
        {
            get { return _owner.MarkPeaks; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.MarkPeaks = value; }));
                }
                else
                {
                    _owner.MarkPeaks = value;
                }
            }
        }

        public bool SnapToGrid
        {
            get { return _owner.SnapToGrid; }                            
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.SnapToGrid = value; }));
                }
                else
                {
                    _owner.SnapToGrid = value;
                }
            }
        }

        public bool SquelchEnabled
        {
            get { return _owner.SquelchEnabled; }                                    
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.SquelchEnabled = value; }));
                }
                else
                {
                    _owner.SquelchEnabled = value;
                }
            }
        }

        public int SquelchThreshold
        {
            get { return _owner.SquelchThreshold; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.SquelchThreshold = value; }));
                }
                else
                {
                    _owner.SquelchThreshold = value;
                }
            }
        }

        public bool SwapIq
        {
            get { return _owner.SwapIq; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.SwapIq = value; }));
                }
                else
                {
                    _owner.SwapIq = value;
                }
            }
        }

        public bool IsPlaying
        {
            get { return _owner.IsPlaying; }
        }

        public int WAttack
        {
            get { return _owner.WAttack; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.WAttack = value; }));
                }
                else
                {
                    _owner.WAttack = value;
                }
            }
        }

        public int WDecay
        {
            get { return _owner.WDecay; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.WDecay = value; }));
                }
                else
                {
                    _owner.WDecay = value;
                }
            }
        }
        
        public int SAttack
        {
            get { return _owner.SAttack; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.SAttack = value; }));
                }
                else
                {
                    _owner.SAttack = value;
                }
            }
        }

        public int SDecay
        {
            get { return _owner.SDecay; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.SDecay = value; }));
                }
                else
                {
                    _owner.SDecay = value;
                }
            }
        }

        public bool UseTimeMarkers
        {

            get { return _owner.UseTimeMarkers; }
            set
            {
                if (_owner.InvokeRequired)
                {
                    _owner.Invoke(new MethodInvoker(() => { _owner.UseTimeMarkers = value; }));
                }
                else
                {
                    _owner.UseTimeMarkers = value;
                }
            }

        }

        public string RdsProgramService
        {
            get { return _owner.RdsProgramService; }
        }

        public string RdsRadioText
        {
            get { return _owner.RdsRadioText; }
        }
        
        public bool IsSquelchOpen
        {
            get { return _owner.IsSquelchOpen; }
        }

        public int RFBandwidth
        {
            get { return _owner.RFBandwidth; }
        }

        public int FFTResolution
        {
            get { return _owner.FFTResolution; }
        }

        public int StepSize
        {
            get { return _owner.StepSize; }
        }

        public bool SourceIsWaveFile
        {
            get { return _owner.SourceIsWaveFile; }
        }

        public bool SourceIsSoundCard
        {
            get { return _owner.SourceIsSoundCard; }
        }

        public bool SourceIsTunable
        {
            get { return _owner.SourceIsTunable; }
        }

        #endregion


        public void GetSpectrumSnapshot(byte[] destArray)
        {

            _owner.GetSpectrumSnapshot(destArray);

        }

        public void StartRadio()
        {
            if (_owner.InvokeRequired)
            {
                _owner.Invoke(new MethodInvoker(() => _owner.StartRadio()));
            }
            else
            {
                _owner.StartRadio();
            }
        }

        public void StopRadio()
        {
            if (_owner.InvokeRequired)
            {
                _owner.Invoke(new MethodInvoker(() => _owner.StopRadio()));
            }
            else
            {
                _owner.StopRadio();
            }
        }

        public void RegisterStreamHook(object streamHook)
        {
            if (_owner.InvokeRequired)
            {
                _owner.Invoke(new MethodInvoker(() => _owner.RegisterStreamHook(streamHook)));
            }
            else
            {
                _owner.RegisterStreamHook(streamHook);
            }
        }

        public void UnregisterStreamHook(object streamHook)
        {
            if (_owner.InvokeRequired)
            {
                _owner.Invoke(new MethodInvoker(() => _owner.UnregisterStreamHook(streamHook)));
            }
            else
            {
                _owner.UnregisterStreamHook(streamHook);
            }
        }

        #region INotifyPropertyChanged

        private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {              
                case "AudioGain":
                case "FilterAudio":
                case "Frequency":
                case "CenterFrequency":
                case "FilterBandwidth":
                case "FilterOrder":
                case "FilterType":
                case "CorrectIq":
                case "FrequencyShiftEnabled":
                case "FrequencyShift":
                case "DetectorType":
                case "FmStereo":
                case "CWShift":
                case "SquelchThreshold":
                case "SquelchEnabled":
                case "SnapToGrid":
                case "StepSize":
                case "UseAgc":
                case "UseHang":
                case "AgcDecay":
                case "AgcThreshold":
                case "AgcSlope":
                case "SwapIq":
                case "SAttack":
                case "SDecay":
                case "WAttack":
                case "WDecay":
                case "MarkPeaks":
                case "UseTimeMarkers":
                case "StartRadio":
                case "StopRadio":
                case "FFTResolution":

                    var handler = PropertyChanged;
                    if (handler != null)
                        PropertyChanged(sender, new PropertyChangedEventArgs(e.PropertyName));

                    break;

                default:
                    break;
            }

        }

        #endregion
    }
}
