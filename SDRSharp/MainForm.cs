using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SDRSharp.Common;
using SDRSharp.Radio;
using SDRSharp.PanView;
using SDRSharp.Radio.PortAudio;
using System.Collections.Specialized;

namespace SDRSharp
{
    public unsafe partial class MainForm : Form, ISharpControl, INotifyPropertyChanged
    {
        #region Private fields

        private static readonly string _baseTitle = "SDR# v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        private const int DefaultNFMBandwidth = 12500;
        private const int DefaultWFMBandwidth = 180000;
        private const int DefaultAMBandwidth = 10000;
        private const int DefaultDSBBandwidth = 6000;
        private const int DefaultSSBBandwidth = 2400;
        private const int DefaultCWBandwidth = 300;

        private WindowType _fftWindowType;
        private IFrontendController _frontendController;
        private readonly Dictionary<string, IFrontendController> _frontendControllers = new Dictionary<string, IFrontendController>();
        private readonly IQBalancer _iqBalancer = new IQBalancer();
        private readonly Vfo _vfo = new Vfo();
        private readonly StreamHookManager _streamHookManager = new StreamHookManager();
        private readonly StreamControl _streamControl;
        private readonly ComplexFifoStream _fftStream = new ComplexFifoStream(BlockMode.BlockingRead);
        private readonly SharpEvent _fftEvent = new SharpEvent(false);
        private readonly object _fftResolutionLockObject = new object();
        private UnsafeBuffer _iqBuffer;
        private Complex* _iqPtr;
        private UnsafeBuffer _fftBuffer;
        private Complex* _fftPtr;
        private UnsafeBuffer _fftWindow;
        private float* _fftWindowPtr;
        private UnsafeBuffer _fftSpectrum;
        private float* _fftSpectrumPtr;
        private UnsafeBuffer _scaledFFTSpectrum;
        private byte* _scaledFFTSpectrumPtr;
        private System.Windows.Forms.Timer _fftTimer;
        private System.Windows.Forms.Timer _performTimer;
        private int _fftSamplesPerFrame;
        private double _fftOverlapRatio;
        private long _frequencyToSet;
        private long _frequencySet;
        private long _frequencyShift;
        private int _maxIQSamples;
        private int _fftBins;
        private int _stepSize;
        private bool _fftSpectrumAvailable;
        private bool _fftSpectrumResized;
        private bool _fftBufferIsWaiting;
        private bool _extioChangingFrequency;
        private bool _extioChangingSamplerate;
        private bool _terminated;
        private string _waveFile;
        private Point _lastLocation;
        private Size _lastSize;
        private bool _initializing;

        private readonly Dictionary<string, ISharpPlugin> _sharpPlugins = new Dictionary<string, ISharpPlugin>();
        private SharpControlProxy _sharpControlProxy;

        private readonly float _fftOffset = (float) Utils.GetDoubleSetting("fftOffset", -40.0);

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Public Properties

        public DetectorType DetectorType
        {
            get { return _vfo.DetectorType; }
            set
            {
                switch (value)
                {
                    case DetectorType.AM:
                        amRadioButton.Checked = true;
                        break;

                    case DetectorType.CWL:
                        cwlRadioButton.Checked = true;
                        break;

                    case DetectorType.CWU:
                        cwuRadioButton.Checked = true;
                        break;

                    case DetectorType.DSB:
                        dsbRadioButton.Checked = true;
                        break;

                    case DetectorType.LSB:
                        lsbRadioButton.Checked = true;
                        break;

                    case DetectorType.USB:
                        usbRadioButton.Checked = true;
                        break;

                    case DetectorType.NFM:
                        nfmRadioButton.Checked = true;
                        break;

                    case DetectorType.WFM:
                        wfmRadioButton.Checked = true;
                        break;
                }
            }
        }

        public WindowType FilterType
        {
            get { return (WindowType)filterTypeComboBox.SelectedIndex + 1; }
            set { filterTypeComboBox.SelectedIndex = (int)value - 1; }
        }

        public bool IsPlaying
        {
            get { return _streamControl.IsPlaying; }
        }
        
        public long Frequency
        {
            get { return vfoFrequencyEdit.Frequency; }
            set { vfoFrequencyEdit.Frequency = value; }
        }

        public long CenterFrequency
        {
            get { return centerFrequencyEdit.Frequency; }
            set
            {
                if (_frontendController == null)
                {
                    throw new ApplicationException("Cannot set the center frequency when no front end is connected");
                }
                centerFrequencyEdit.Frequency = value;
            }
        }

        public long FrequencyShift
        {
            get { return (long)frequencyShiftNumericUpDown.Value; }
            set { frequencyShiftNumericUpDown.Value = value; }
        }

        public bool FrequencyShiftEnabled
        {
            get { return frequencyShiftCheckBox.Checked; }
            set { frequencyShiftCheckBox.Checked = value; }
        }

        public int FilterBandwidth
        {
            get { return (int)filterBandwidthNumericUpDown.Value; }
            set { filterBandwidthNumericUpDown.Value = value; }
        }

        public int FilterOrder
        {
            get { return (int)filterOrderNumericUpDown.Value; }
            set { filterOrderNumericUpDown.Value = value; }
        }

        public bool SquelchEnabled
        {
            get { return useSquelchCheckBox.Checked; }
            set { useSquelchCheckBox.Checked = value; }
        }

        public int SquelchThreshold
        {
            get { return (int)squelchNumericUpDown.Value; }
            set { squelchNumericUpDown.Value = value; }
        }

        public int CWShift
        {
            get { return (int)cwShiftNumericUpDown.Value; }
            set { cwShiftNumericUpDown.Value = value; }
        }

        public bool SnapToGrid
        {
            get { return snapFrequencyCheckBox.Checked; }
            set { snapFrequencyCheckBox.Checked = value; }
        }

        public bool SwapIq
        {
            get { return swapIQCheckBox.Checked; }
            set { swapIQCheckBox.Checked = value; }
        }

        public bool FmStereo
        {
            get { return fmStereoCheckBox.Checked; }
            set { fmStereoCheckBox.Checked = value; }
        }

        public bool MarkPeaks
        {
            get { return markPeaksCheckBox.Checked; }
            set { markPeaksCheckBox.Checked = value; }
        }

        public int AudioGain
        {
            get { return audioGainTrackBar.Value; }
            set { audioGainTrackBar.Value = value; }
        }

        public bool FilterAudio
        {
            get { return filterAudioCheckBox.Checked; }
            set { filterAudioCheckBox.Checked = value; }
        }

        public bool UseAgc
        {
            get { return agcCheckBox.Checked; }
            set { agcCheckBox.Checked = value; }
        }

        public bool AgcHang
        {
            get { return agcUseHangCheckBox.Checked; }
            set { agcUseHangCheckBox.Checked = value; }
        }

        public int AgcThreshold
        {
            get { return (int) agcThresholdNumericUpDown.Value; }
            set { agcThresholdNumericUpDown.Value = value; }
        }

        public int AgcDecay
        {
            get { return (int) agcDecayNumericUpDown.Value; }
            set { agcDecayNumericUpDown.Value = value; }
        }

        public int AgcSlope
        {
            get { return (int) agcSlopeNumericUpDown.Value; }
            set { agcSlopeNumericUpDown.Value = value; }
        }

        public int SAttack
        {
            get { return sAttackTrackBar.Value; }
            set { sAttackTrackBar.Value = value; }
        }

        public int SDecay
        {
            get { return sDecayTrackBar.Value; }
            set { sDecayTrackBar.Value = value; }
        }

        public int WAttack
        {
            get { return wAttackTrackBar.Value; }
            set { wAttackTrackBar.Value = value; }
        }

        public int WDecay
        {
            get { return wDecayTrackBar.Value; }
            set { wDecayTrackBar.Value = value; }
        }

        public bool UseTimeMarkers
        {
            get { return useTimestampsCheckBox.Checked; }
            set
            {
                useTimestampsCheckBox.Checked = value;
            }
        }

        public string RdsProgramService
        {
            get { return _vfo.RdsStationName; }
        }

        public string RdsRadioText
        {
            get { return _vfo.RdsStationText; }
        }

        public int RFBandwidth
        {
            get { return (int) _vfo.SampleRate; }
        }

        public bool SourceIsWaveFile
        {
            get { return iqSourceComboBox.SelectedIndex == iqSourceComboBox.Items.Count - 2; }
        }

        public bool IsSquelchOpen
        {
            get { return _vfo.IsSquelchOpen; }
        }

        public int FFTResolution
        {
            get { return _fftBins; }
        }

        public int StepSize
        {
            get { return _stepSize; }
        }
        
        #endregion

        #region Initialization and Termination

        public MainForm()
        {
            _streamControl = new StreamControl(_streamHookManager);

            InitializeComponent();
            InitializeGUI();            
            InitialiseSharpPlugins();            
        }

        private void InitializeGUI()
        {
            _initializing = true;

            #region Tunning

            ThreadPool.QueueUserWorkItem(TuneThreadProc);

            #endregion

            #region Audio devices

            var defaultIndex = 0;
            var savedIndex = -1;
            var devices = AudioDevice.GetDevices(DeviceDirection.Input);
            var savedDeviceName = Utils.GetStringSetting("inputDevice", string.Empty);
            for (var i = 0; i < devices.Count; i++)
            {
                inputDeviceComboBox.Items.Add(devices[i]);
                if (devices[i].IsDefault)
                {
                    defaultIndex = i;
                }
                if (devices[i].ToString() == savedDeviceName)
                {
                    savedIndex = i;
                }
            }
            if (inputDeviceComboBox.Items.Count > 0)
            {
                inputDeviceComboBox.SelectedIndex = savedIndex >= 0 ? savedIndex : defaultIndex;
            }

            defaultIndex = 0;
            devices = AudioDevice.GetDevices(DeviceDirection.Output);
            savedDeviceName = Utils.GetStringSetting("outputDevice", string.Empty);
            for (int i = 0; i < devices.Count; i++)
            {
                outputDeviceComboBox.Items.Add(devices[i]);
                if (devices[i].IsDefault)
                {
                    defaultIndex = i;
                }
                if (devices[i].ToString() == savedDeviceName)
                {
                    savedIndex = i;
                }
            }
            if (outputDeviceComboBox.Items.Count > 0)
            {
                outputDeviceComboBox.SelectedIndex = savedIndex >= 0 ? savedIndex : defaultIndex;
            }

            _streamControl.BufferNeeded += ProcessBuffer;

            #endregion

            #region VFO

            DetectorType = (DetectorType) Utils.GetIntSetting("detectorType", (int) DetectorType.AM);
            modeRadioButton_CheckStateChanged(null, null);

            filterOrderNumericUpDown.Value = Utils.GetIntSetting("filterOrder", 400);
            filterOrderNumericUpDown_ValueChanged(null, null);

            cwShiftNumericUpDown.Value = Utils.GetIntSetting("cwShift", Vfo.DefaultCwSideTone);
            cwShiftNumericUpDown_ValueChanged(null, null);

            agcCheckBox.Checked = Utils.GetBooleanSetting("useAGC");
            agcCheckBox_CheckedChanged(null, null);

            agcThresholdNumericUpDown.Value = Utils.GetIntSetting("agcThreshold", -100);
            agcThresholdNumericUpDown_ValueChanged(null, null);

            agcDecayNumericUpDown.Value = Utils.GetIntSetting("agcDecay", 100);
            agcDecayNumericUpDown_ValueChanged(null, null);

            agcSlopeNumericUpDown.Value = Utils.GetIntSetting("agcSlope", 0);
            agcSlopeNumericUpDown_ValueChanged(null, null);

            agcUseHangCheckBox.Checked = Utils.GetBooleanSetting("agcHang");
            agcUseHangCheckBox_CheckedChanged(null, null);

            squelchNumericUpDown.Value = Utils.GetIntSetting("squelchThreshold", 50);
            squelchNumericUpDown_ValueChanged(null, null);

            useSquelchCheckBox.Checked = Utils.GetBooleanSetting("squelchEnabled");
            useSquelchCheckBox_CheckedChanged(null, null);

            var filterBandwidth = Utils.GetIntSetting("filterBandwidth", -1);
            if (filterBandwidth >= 0)
            {
                filterBandwidthNumericUpDown.Value = filterBandwidth;
            }
            filterBandwidthNumericUpDown_ValueChanged(null, null);

            centerFrequencyEdit.Frequency = 0;
            centerFrequencyEdit_FrequencyChanged(null, null);

            frequencyShiftNumericUpDown.Value = Utils.GetLongSetting("frequencyShift", 0);
            frequencyShiftNumericUpDown_ValueChanged(null, null);

            frequencyShiftCheckBox.Checked = Utils.GetBooleanSetting("frequencyShiftEnabled");
            frequencyShiftCheckBox_CheckStateChanged(null, null);

            var stepSizeIndex = Utils.GetIntSetting("stepSize", -1);
            if (stepSizeIndex >= 0)
            {
                stepSizeComboBox.SelectedIndex = stepSizeIndex;
            }
            snapFrequencyCheckBox.Checked = Utils.GetBooleanSetting("snapToGrid");
            stepSizeComboBox_SelectedIndexChanged(null, null);

            swapIQCheckBox.Checked = Utils.GetBooleanSetting("swapIQ");
            swapIQCheckBox_CheckedChanged(null, null);

            correctIQCheckBox.Checked = Utils.GetBooleanSetting("correctIQ");
            autoCorrectIQCheckBox_CheckStateChanged(null, null);

            markPeaksCheckBox.Checked = Utils.GetBooleanSetting("markPeaks");
            markPeaksCheckBox_CheckedChanged(null, null);

            fmStereoCheckBox.Checked = Utils.GetBooleanSetting("fmStereo");
            fmStereoCheckBox_CheckedChanged(null, null);

            filterAudioCheckBox.Checked = Utils.GetBooleanSetting("filterAudio");
            filterAudioCheckBox_CheckStateChanged(null, null);

            audioGainTrackBar.Value = Utils.GetIntSetting("audioGain", 30);
            audioGainTrackBar_ValueChanged(null, null);

            latencyNumericUpDown.Value = Utils.GetIntSetting("latency", 100);
            sampleRateComboBox.Text = Utils.GetStringSetting("sampleRate", "48000 sample/sec");

            WindowState = (FormWindowState) Utils.GetIntSetting("windowState", (int) FormWindowState.Normal);

            var locationArray = Utils.GetIntArraySetting("windowPosition", null);
            if (locationArray != null)
            {
                _lastLocation.X = locationArray[0];
                _lastLocation.Y = locationArray[1];
                Location = _lastLocation;
            }
            else
            {
                _lastLocation = Location;
            }

            var sizeArray = Utils.GetIntArraySetting("windowSize", null);
            if (sizeArray != null)
            {
                _lastSize.Width = sizeArray[0];
                _lastSize.Height = sizeArray[1];
                Size = _lastSize;
            }
            else
            {
                _lastSize = Size;
            }

            panSplitContainer.SplitterDistance = Utils.GetIntSetting("splitterPosition", panSplitContainer.SplitterDistance);

            waterfall.FilterOffset = Vfo.MinSSBAudioFrequency;
            spectrumAnalyzer.FilterOffset = Vfo.MinSSBAudioFrequency;
            
            #endregion

            #region FFT display

            _fftTimer = new System.Windows.Forms.Timer(components);
            _fftTimer.Tick += fftTimer_Tick;
            _fftTimer.Enabled = true;

            _performTimer = new System.Windows.Forms.Timer(components);
            _performTimer.Tick += performTimer_Tick;
            _performTimer.Interval = 40;
            _performTimer.Enabled = true;

            viewComboBox.SelectedIndex = Utils.GetIntSetting("fftView", 2);
            fftResolutionComboBox.SelectedIndex = Utils.GetIntSetting("fftResolution", 3);
            fftWindowComboBox.SelectedIndex = Utils.GetIntSetting("fftWindowType", (int) WindowType.BlackmanHarris);
            filterTypeComboBox.SelectedIndex = Utils.GetIntSetting("filterType", (int) WindowType.BlackmanHarris - 1);
            
            fftSpeedTrackBar.Value = Utils.GetIntSetting("fftSpeed", 40);
            fftSpeedTrackBar_ValueChanged(null, null);

            fftContrastTrackBar.Value = Utils.GetIntSetting("fftContrast", 0);
            fftContrastTrackBar_Changed(null, null);

            spectrumAnalyzer.Attack = Utils.GetDoubleSetting("spectrumAnalyzerAttack", 0.9);
            sAttackTrackBar.Value = (int) (spectrumAnalyzer.Attack * sAttackTrackBar.Maximum);

            spectrumAnalyzer.Decay = Utils.GetDoubleSetting("spectrumAnalyzerDecay", 0.3);
            sDecayTrackBar.Value = (int) (spectrumAnalyzer.Decay * sDecayTrackBar.Maximum);

            waterfall.Attack = Utils.GetDoubleSetting("waterfallAttack", 0.9);
            wAttackTrackBar.Value = (int) (waterfall.Attack * wAttackTrackBar.Maximum);

            waterfall.Decay = Utils.GetDoubleSetting("waterfallDecay", 0.5);
            wDecayTrackBar.Value = (int) (waterfall.Decay * wDecayTrackBar.Maximum);

            useTimestampsCheckBox.Checked = Utils.GetBooleanSetting("useTimeMarkers");
            useTimestampCheckBox_CheckedChanged(null, null);

            fftOffsetTrackBar.Value = Utils.GetIntSetting("fftDisplayOffset", 0);
            fftOffsetTrackBar_Scroll(null, null);

            fftRangeTrackBar.Value = Utils.GetIntSetting("fftDisplayRange", 13);
            fftRangeTrackBar_Scroll(null, null);

            #endregion

            #region Initialize the plugins

            var frontendPlugins = (NameValueCollection) ConfigurationManager.GetSection("frontendPlugins");

            foreach (string key in frontendPlugins.Keys)
            {
                try
                {
                    var fullyQualifiedTypeName = frontendPlugins[key];
                    var patterns = fullyQualifiedTypeName.Split(',');
                    var typeName = patterns[0];
                    var assemblyName = patterns[1];
                    var objectHandle = Activator.CreateInstance(assemblyName, typeName);
                    var controller = (IFrontendController) objectHandle.Unwrap();
                    _frontendControllers.Add(key, controller);
                    iqSourceComboBox.Items.Add(key);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading '" + frontendPlugins[key] + "' - " + ex.Message);
                }
            }

            var extIOs = Directory.GetFiles(".", "ExtIO_*.dll");

            var dropDownWidth = iqSourceComboBox.Width;
            var graphics = iqSourceComboBox.CreateGraphics();

            foreach (var extIO in extIOs)
            {
                try
                {
                    var controller = new ExtIOController(extIO);
                    controller.HideSettingGUI();
                    var displayName = string.IsNullOrEmpty(ExtIO.HWName) ? "" + Path.GetFileName(extIO) : ExtIO.HWName;
                    if (!string.IsNullOrEmpty(ExtIO.HWModel))
                    {
                        displayName += " (" + ExtIO.HWModel + ")";
                    }
                    displayName += " - " + Path.GetFileName(extIO);
                    var size = graphics.MeasureString(displayName, iqSourceComboBox.Font);
                    if (size.Width > dropDownWidth)
                    {
                        dropDownWidth = (int)size.Width;
                    }
                    _frontendControllers.Add(displayName, controller);
                    iqSourceComboBox.Items.Add(displayName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading '" + Path.GetFileName(extIO) + "'\r\n" + ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            iqSourceComboBox.DropDownWidth = dropDownWidth;

            ExtIO.SampleRateChanged += ExtIO_SampleRateChanged;
            ExtIO.LOFreqChanged += ExtIO_LOFreqChanged;

            iqSourceComboBox.Items.Add("IQ file (*.wav)");

            iqSourceComboBox.Items.Add("Other (Sound card)");

            #endregion

            #region Source selection

            _waveFile = Utils.GetStringSetting("waveFile", string.Empty);
            var sourceIndex = Utils.GetIntSetting("iqSource", iqSourceComboBox.Items.Count - 1);
            iqSourceComboBox.SelectedIndex = sourceIndex < iqSourceComboBox.Items.Count ? sourceIndex : (iqSourceComboBox.Items.Count - 1);
            if (iqSourceComboBox.SelectedIndex != iqSourceComboBox.Items.Count - 2)
            {
                centerFrequencyEdit.Frequency = Utils.GetLongSetting("centerFrequency", centerFrequencyEdit.Frequency);
                var vfo = Utils.GetLongSetting("vfo", centerFrequencyEdit.Frequency);
                if (vfo >= vfoFrequencyEdit.Minimum && vfo <= vfoFrequencyEdit.Maximum)
                {
                    vfoFrequencyEdit.Frequency = vfo;
                }
                else
                {
                    var newFrequency = centerFrequencyEdit.Frequency + _frequencyShift;
                    vfoFrequencyEdit.Minimum = newFrequency;
                    vfoFrequencyEdit.Maximum = newFrequency;
                    vfoFrequencyEdit.Frequency = newFrequency;
                }
            }

            #endregion

            _initializing = false;
        }

        private void ExtIO_LOFreqChanged(int frequency)
        {
            BeginInvoke(new Action(() =>
                            {

                                _extioChangingFrequency = true;
                                centerFrequencyEdit.Frequency = frequency;
                                _extioChangingFrequency = false;
                            }));
        }

        private void ExtIO_SampleRateChanged(int newSamplerate)
        {
            BeginInvoke(new Action(() =>
                            {
                                if (_streamControl.IsPlaying)
                                {
                                    _extioChangingSamplerate = true;
                                    try
                                    {
                                        _streamControl.Stop();
                                        Open();
                                        _streamControl.Play();
                                    }
                                    finally
                                    {
                                        _extioChangingSamplerate = false;
                                    }
                                }
                            }));
        }

        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            _terminated = true;
            _streamControl.Stop();
            _fftEvent.Set();
            if (_frontendController != null)
            {
                _frontendController.Close();
                _frontendController = null;
            }

            #region ISharpPlugin Teardown
            
            foreach(var plugin in _sharpPlugins.Values)
            {
                plugin.Close();
            }
            
            #endregion

            Utils.SaveSetting("spectrumAnalyzerAttack", spectrumAnalyzer.Attack);
            Utils.SaveSetting("spectrumAnalyzerDecay", spectrumAnalyzer.Decay);
            Utils.SaveSetting("waterfallAttack", waterfall.Attack);
            Utils.SaveSetting("waterfallDecay", waterfall.Decay);
            Utils.SaveSetting("useTimeMarkers", useTimestampsCheckBox.Checked);
            Utils.SaveSetting("fftSpeed", fftSpeedTrackBar.Value);
            Utils.SaveSetting("fftContrast", fftContrastTrackBar.Value);
            Utils.SaveSetting("fftWindowType", fftWindowComboBox.SelectedIndex);
            Utils.SaveSetting("fftView", viewComboBox.SelectedIndex);
            Utils.SaveSetting("fftResolution", fftResolutionComboBox.SelectedIndex);
            Utils.SaveSetting("filterType", filterTypeComboBox.SelectedIndex);
            Utils.SaveSetting("cwShift", cwShiftNumericUpDown.Value);
            Utils.SaveSetting("detectorType", (int) DetectorType);
            Utils.SaveSetting("filterOrder", (int) filterOrderNumericUpDown.Value);
            Utils.SaveSetting("filterBandwidth", (int) filterBandwidthNumericUpDown.Value);
            Utils.SaveSetting("squelchEnabled", useSquelchCheckBox.Checked);
            Utils.SaveSetting("squelchThreshold", (int) squelchNumericUpDown.Value);
            Utils.SaveSetting("useAGC", agcCheckBox.Checked);
            Utils.SaveSetting("agcThreshold", (int) agcThresholdNumericUpDown.Value);
            Utils.SaveSetting("agcDecay", (int) agcDecayNumericUpDown.Value);
            Utils.SaveSetting("agcSlope", (int) agcSlopeNumericUpDown.Value);
            Utils.SaveSetting("agcHang", agcUseHangCheckBox.Checked);
            Utils.SaveSetting("stepSize", stepSizeComboBox.SelectedIndex);
            Utils.SaveSetting("snapToGrid", snapFrequencyCheckBox.Checked);
            Utils.SaveSetting("frequencyShift", (long) frequencyShiftNumericUpDown.Value);
            Utils.SaveSetting("frequencyShiftEnabled", frequencyShiftCheckBox.Checked);
            Utils.SaveSetting("swapIQ", swapIQCheckBox.Checked);
            Utils.SaveSetting("correctIQ", correctIQCheckBox.Checked);
            Utils.SaveSetting("markPeaks", markPeaksCheckBox.Checked);
            Utils.SaveSetting("fmStereo", fmStereoCheckBox.Checked);
            Utils.SaveSetting("filterAudio", filterAudioCheckBox.Checked);
            Utils.SaveSetting("latency", (int) latencyNumericUpDown.Value);
            Utils.SaveSetting("sampleRate", sampleRateComboBox.Text);
            Utils.SaveSetting("audioGain", audioGainTrackBar.Value);
            Utils.SaveSetting("windowState", (int) WindowState);
            Utils.SaveSetting("windowPosition", Utils.IntArrayToString(_lastLocation.X, _lastLocation.Y));
            Utils.SaveSetting("windowSize", Utils.IntArrayToString(_lastSize.Width, _lastSize.Height));
            Utils.SaveSetting("collapsiblePanelStates", Utils.IntArrayToString(GetCollapsiblePanelStates()));
            Utils.SaveSetting("splitterPosition", panSplitContainer.SplitterDistance);
            Utils.SaveSetting("iqSource", iqSourceComboBox.SelectedIndex);
            Utils.SaveSetting("waveFile", "" + _waveFile);
            Utils.SaveSetting("centerFrequency", centerFrequencyEdit.Frequency);
            Utils.SaveSetting("vfo", vfoFrequencyEdit.Frequency);
            Utils.SaveSetting("fftDisplayOffset", fftOffsetTrackBar.Value);
            Utils.SaveSetting("fftDisplayRange", fftRangeTrackBar.Value);
            Utils.SaveSetting("inputDevice", inputDeviceComboBox.SelectedItem);
            Utils.SaveSetting("outputDevice", outputDeviceComboBox.SelectedItem);
        }

        #endregion

        #region IQ FFT and DSP handlers

        private void ProcessBuffer(Complex* iqBuffer, float* audioBuffer, int length)
        {
            _iqBalancer.Process(iqBuffer, length);
            if (_fftStream.Length < _maxIQSamples)
            {
                _fftStream.Write(iqBuffer, length);
                if (_fftBufferIsWaiting)
                {
                    _fftBufferIsWaiting = false;
                    _fftEvent.Set();
                }
            }
            _vfo.ProcessBuffer(iqBuffer, audioBuffer, length);
        }

        private void ProcessFFT(object parameter)
        {
            while (_streamControl.IsPlaying || _extioChangingSamplerate)
            {
                Monitor.Enter(_fftResolutionLockObject);

                #region Configure

                var fftRate = _fftBins / (_fftTimer.Interval * 0.001);
                _fftOverlapRatio = _streamControl.SampleRate / fftRate;
                var samplesToConsume = (int)(_fftBins * _fftOverlapRatio);
                _fftSamplesPerFrame = Math.Min(samplesToConsume, _fftBins);
                var excessSamples = samplesToConsume - _fftSamplesPerFrame;
                _maxIQSamples = (int) (samplesToConsume / (double) _fftTimer.Interval * _streamControl.BufferSizeInMs * 1.5);

                #endregion

                #region Shift data for overlapped mode)

                if (_fftSamplesPerFrame < _fftBins)
                {
                    Utils.Memcpy(_iqPtr, _iqPtr + _fftSamplesPerFrame, (_fftBins - _fftSamplesPerFrame) * sizeof(Complex));
                }

                #endregion

                #region Read IQ data

                var targetLength = _fftSamplesPerFrame;

                var total = 0;
                while (_streamControl.IsPlaying && total < targetLength)
                {
                    var len = targetLength - total;
                    total += _fftStream.Read(_iqPtr, _fftBins - targetLength + total, len);
                }

                _fftStream.Advance(excessSamples);

                #endregion

                if (!_fftSpectrumAvailable)
                {
                    #region Process FFT gain

                    // http://www.designnews.com/author.asp?section_id=1419&doc_id=236273&piddl_msgid=522392
                    var fftGain = (float)(10.0 * Math.Log10((double) _fftBins / 2));
                    var compensation = 24.0f - fftGain + _fftOffset;

                    #endregion

                    #region Calculate FFT

                    Utils.Memcpy(_fftPtr, _iqPtr, _fftBins * sizeof(Complex));
                    Fourier.ApplyFFTWindow(_fftPtr, _fftWindowPtr, _fftBins);
                    Fourier.ForwardTransform(_fftPtr, _fftBins);
                    Fourier.SpectrumPower(_fftPtr, _fftSpectrumPtr, _fftBins, compensation);

                    #endregion

                    _fftSpectrumResized = false;
                    _fftSpectrumAvailable = true;
                }

                Monitor.Exit(_fftResolutionLockObject);

                if (_fftStream.Length <= _maxIQSamples)
                {
                    _fftBufferIsWaiting = true;
                    _fftEvent.WaitOne();
                }
            }
            _fftStream.Flush();
        }

        private void RenderFFT()
        {
            if (_fftSpectrumResized)
            {
                return;
            }
            if (!panSplitContainer.Panel1Collapsed)
            {
                spectrumAnalyzer.Render(_fftSpectrumPtr, _fftBins);
            }
            if (!panSplitContainer.Panel2Collapsed)
            {
                waterfall.Render(_fftSpectrumPtr, _fftBins);
            }
        }

        private void performTimer_Tick(object sender, EventArgs e)
        {
            spectrumAnalyzer.Perform();
            waterfall.Perform();
        }

        private void fftTimer_Tick(object sender, EventArgs e)
        {
            if (_streamControl.IsPlaying)
            {
                RenderFFT();
                _fftSpectrumAvailable = false;
                if (_fftBufferIsWaiting)
                {
                    _fftBufferIsWaiting = false;
                    _fftEvent.Set();
                }
            }
        }

        private void iqTimer_Tick(object sender, EventArgs e)
        {
            Text = string.Format(_baseTitle + " - IQ Imbalance: Gain = {0:F3} Phase = {1:F3}°", _iqBalancer.Gain, _iqBalancer.Phase * 180 / Math.PI);
            if (_vfo.SignalIsStereo)
            {
                Text += " ((( stereo )))";
            }

            spectrumAnalyzer.StatusText = string.Empty;
            if (_vfo.DetectorType == DetectorType.WFM)
            {
                if (!string.IsNullOrEmpty(_vfo.RdsStationName.Trim()))
                {
                    spectrumAnalyzer.StatusText = _vfo.RdsStationName;
                }
                if (!string.IsNullOrEmpty(_vfo.RdsStationText))
                {
                    spectrumAnalyzer.StatusText += " [ " + _vfo.RdsStationText + " ]";
                }
            }
        }

        private void BuildFFTWindow()
        {
            var window = FilterBuilder.MakeWindow(_fftWindowType, _fftBins);
            fixed (float *windowPtr = window)
            {
                Utils.Memcpy(_fftWindow, windowPtr, _fftBins * sizeof(float));
            }
        }

        private void InitFFTBuffers()
        {
            _iqBuffer = UnsafeBuffer.Create(_fftBins, sizeof(Complex));
            _fftBuffer = UnsafeBuffer.Create(_fftBins, sizeof(Complex));
            _fftWindow = UnsafeBuffer.Create(_fftBins, sizeof(float));
            _fftSpectrum = UnsafeBuffer.Create(_fftBins, sizeof(float));
            _scaledFFTSpectrum = UnsafeBuffer.Create(_fftBins, sizeof(byte));

            _iqPtr = (Complex*) _iqBuffer;
            _fftPtr = (Complex*) _fftBuffer;
            _fftWindowPtr = (float*) _fftWindow;
            _fftSpectrumPtr = (float*) _fftSpectrum;
            _scaledFFTSpectrumPtr = (byte*) _scaledFFTSpectrum;
        }

        #endregion

        #region IQ source selection

        private void iqSourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopRadio();

            if (iqSourceComboBox.SelectedIndex == iqSourceComboBox.Items.Count - 1)
            {
                if (_frontendController != null)
                {
                    _frontendController.Close();
                    _frontendController = null;
                }

                inputDeviceComboBox.Enabled = true;
                outputDeviceComboBox.Enabled = true;
                sampleRateComboBox.Enabled = true;
                centerFrequencyEdit.Enabled = true;
                centerFrequencyEdit.Frequency = 0;
                centerFrequencyEdit_FrequencyChanged(null, null);
                vfoFrequencyEdit.Frequency = _frequencyShift;
                vfoFrequencyEdit_FrequencyChanged(null, null);
                frequencyShiftCheckBox.Enabled = true;
                frequencyShiftNumericUpDown.Enabled = frequencyShiftCheckBox.Checked;
                configureSourceButton.Text = "Configure";
                configureSourceButton.Enabled = false;
                return;
            }
            
            configureSourceButton.Enabled = true;

            if (iqSourceComboBox.SelectedIndex == iqSourceComboBox.Items.Count - 2)
            {
                configureSourceButton.Text = "Select";
                sampleRateComboBox.Enabled = false;
                inputDeviceComboBox.Enabled = false;
                outputDeviceComboBox.Enabled = true;
                latencyNumericUpDown.Enabled = true;
                centerFrequencyEdit.Enabled = false;
                frequencyShiftCheckBox.Enabled = false;
                frequencyShiftNumericUpDown.Enabled = false;

                _frequencyShift = 0;
                centerFrequencyEdit.Frequency = 0;
                centerFrequencyEdit_FrequencyChanged(null, null);
                vfoFrequencyEdit.Frequency = 0;
                frequencyShiftCheckBox.Checked = false;

                if (!_initializing)
                {
                    SelectWaveFile();
                }
                return;
            }

            configureSourceButton.Text = "Configure";
            frequencyShiftCheckBox.Enabled = true;
            frequencyShiftNumericUpDown.Enabled = frequencyShiftCheckBox.Checked;

            var frontendName = (string) iqSourceComboBox.SelectedItem;
            try
            {
                if (_frontendController != null)
                {
                    _frontendController.HideSettingGUI();
                    _frontendController.Close();
                }
                _frontendController = _frontendControllers[frontendName];
                _frontendController.Open();
                inputDeviceComboBox.Enabled = _frontendController.IsSoundCardBased;
                sampleRateComboBox.Enabled = _frontendController.IsSoundCardBased;
                if (_frontendController.IsSoundCardBased)
                {
                    var regex = new Regex(_frontendController.SoundCardHint, RegexOptions.IgnoreCase);
                    for (var i = 0; i < inputDeviceComboBox.Items.Count; i++)
                    {
                        var item = inputDeviceComboBox.Items[i].ToString();
                        if (regex.IsMatch(item))
                        {
                            inputDeviceComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    if (_frontendController.Samplerate > 0)
                    {
                        sampleRateComboBox.Text = _frontendController.Samplerate.ToString();
                    }
                }
                if (_frontendController.Samplerate > 0)
                {
                    waterfall.SpectrumWidth = (int) _frontendController.Samplerate;
                    spectrumAnalyzer.SpectrumWidth = (int) _frontendController.Samplerate;
                }
                _vfo.SampleRate = _frontendController.Samplerate;
                _vfo.Frequency = 0;
                centerFrequencyEdit.Enabled = true;
                centerFrequencyEdit.Frequency = _frontendController.Frequency;
                centerFrequencyEdit_FrequencyChanged(null, null);
                vfoFrequencyEdit.Frequency = _frontendController.Frequency + _frequencyShift;
                vfoFrequencyEdit_FrequencyChanged(null, null);
            }
            catch
            {
                iqSourceComboBox.SelectedIndex = iqSourceComboBox.Items.Count - 1;
                if (_frontendController != null)
                {
                    _frontendController.Close();
                }
                _frontendController = null;
                if (!_initializing)
                {
                    MessageBox.Show(
                        frontendName + " is either not connected or its driver is not working properly.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void SelectWaveFile()
        {
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                StopRadio();
                _waveFile = openDlg.FileName;
            }
        }

        #endregion

        #region Audio settings

        private void Open()
        {
            var inputDevice = (AudioDevice) inputDeviceComboBox.SelectedItem;
            var outputDevice = (AudioDevice) outputDeviceComboBox.SelectedItem;
            var oldCenterFrequency = centerFrequencyEdit.Frequency;
            Match match;
            if (SourceIsWaveFile)
            {
                if (!File.Exists(_waveFile))
                {
                    throw new ApplicationException("No such file");
                }
                _streamControl.OpenFile(_waveFile, outputDevice.Index, (int)latencyNumericUpDown.Value);

                var friendlyFilename = "" + Path.GetFileName(_waveFile);
                match = Regex.Match(friendlyFilename, "([0-9]+)kHz", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var center = int.Parse(match.Groups[1].Value) * 1000;
                    centerFrequencyEdit.Frequency = center;
                }
                else
                {
                    centerFrequencyEdit.Frequency = 0;
                }
                centerFrequencyEdit_FrequencyChanged(null, null);
            }
            else
            {
                if (_frontendController == null || _frontendController.IsSoundCardBased)
                {
                    var sampleRate = 0.0;
                    match = Regex.Match(sampleRateComboBox.Text, "([0-9\\.]+)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        sampleRate = double.Parse(match.Groups[1].Value);
                    }
                    _streamControl.OpenSoundDevice(inputDevice.Index, outputDevice.Index, sampleRate, (int) latencyNumericUpDown.Value);
                }
                else
                {
                    _streamControl.OpenPlugin(_frontendController, outputDevice.Index, (int) latencyNumericUpDown.Value);
                }
            }

            _vfo.SampleRate = _streamControl.SampleRate;
            _vfo.DecimationStageCount = _streamControl.DecimationStageCount;
            spectrumAnalyzer.SpectrumWidth = (int)_streamControl.SampleRate;
            waterfall.SpectrumWidth = spectrumAnalyzer.SpectrumWidth;

            vfoFrequencyEdit.Maximum = centerFrequencyEdit.Frequency + (int) (_streamControl.SampleRate / 2) + _frequencyShift;
            vfoFrequencyEdit.Minimum = centerFrequencyEdit.Frequency - (int) (_streamControl.SampleRate / 2) + _frequencyShift;

            if (centerFrequencyEdit.Frequency != oldCenterFrequency)
            {
                vfoFrequencyEdit.Frequency = centerFrequencyEdit.Frequency + _frequencyShift;

                fftZoomTrackBar.Value = 0;
                fftZoomTrackBar_ValueChanged(null, null);
            }

            _frequencySet = 0L;
            vfoFrequencyEdit_FrequencyChanged(null, null);

            BuildFFTWindow();
        }

        private void audioGainTrackBar_ValueChanged(object sender, EventArgs e)
        {
            _streamControl.AudioGain = audioGainTrackBar.Value;

            NotifyPropertyChanged("AudioGain");
        }

        private void filterAudioCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            _vfo.FilterAudio = filterAudioCheckBox.Checked;

            NotifyPropertyChanged("FilterAudio");
        }

        #endregion

        #region Main controls

        private void playStopButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_streamControl.IsPlaying)
                {
                    StopRadio();
                }
                else
                {
                    StartRadio();
                }
            }
            catch (Exception ex)
            {
                StopRadio();
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Radio settings

        #region Frequency and filters

        private void vfoFrequencyEdit_FrequencyChanged(object sender, EventArgs e)
        {
            waterfall.Frequency = vfoFrequencyEdit.Frequency;
            spectrumAnalyzer.Frequency = vfoFrequencyEdit.Frequency;
            _vfo.Frequency = (int)(waterfall.Frequency - centerFrequencyEdit.Frequency - _frequencyShift);
            if (_vfo.DetectorType == DetectorType.WFM)
            {
                _vfo.RdsReset();
            }

            NotifyPropertyChanged("Frequency");
        }

        private void centerFrequencyEdit_FrequencyChanged(object sender, EventArgs e)
        {
            var newCenterFreq = centerFrequencyEdit.Frequency;
            waterfall.CenterFrequency = newCenterFreq + _frequencyShift;
            spectrumAnalyzer.CenterFrequency = newCenterFreq + _frequencyShift;

            vfoFrequencyEdit.Maximum = long.MaxValue;
            vfoFrequencyEdit.Minimum = long.MinValue;
            vfoFrequencyEdit.Frequency = newCenterFreq + _vfo.Frequency + _frequencyShift;
            vfoFrequencyEdit.Maximum = newCenterFreq + (int)(_vfo.SampleRate / 2) + _frequencyShift;
            vfoFrequencyEdit.Minimum = newCenterFreq - (int)(_vfo.SampleRate / 2) + _frequencyShift;

            if (snapFrequencyCheckBox.Checked)
            {
                vfoFrequencyEdit.Maximum = (vfoFrequencyEdit.Maximum) / waterfall.StepSize * waterfall.StepSize;
                vfoFrequencyEdit.Minimum = 2 * spectrumAnalyzer.CenterFrequency - vfoFrequencyEdit.Maximum;
            }

            if (_frontendController != null && !SourceIsWaveFile && !_extioChangingFrequency)
            {
                lock (this)
                {
                    _frequencyToSet = newCenterFreq;
                }
            }

            if (_vfo.DetectorType == DetectorType.WFM)
            {
                _vfo.RdsReset();
            }

            NotifyPropertyChanged("CenterFrequency");
        }

        private void TuneThreadProc(object state)
        {
            while (!_terminated)
            {
                long copyOfFrequencyToSet;
                lock (this)
                {
                    copyOfFrequencyToSet = _frequencyToSet;
                }
                if (_frontendController != null && _frequencySet != copyOfFrequencyToSet)
                {
                    _frequencySet = copyOfFrequencyToSet;
                    _frontendController.Frequency = copyOfFrequencyToSet;
                }
                Thread.Sleep(1);
            }
        }

        private void panview_FrequencyChanged(object sender, FrequencyEventArgs e)
        {
            if (e.Frequency >= vfoFrequencyEdit.Minimum &&
                e.Frequency <= vfoFrequencyEdit.Maximum)
            {
                vfoFrequencyEdit.Frequency = e.Frequency;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void panview_CenterFrequencyChanged(object sender, FrequencyEventArgs e)
        {
            if (SourceIsWaveFile)
            {
                e.Cancel = true;
            }
            else
            {
                var f = e.Frequency - _frequencyShift;
                if (f < 0)
                {
                    f = 0;
                    e.Cancel = true;
                }
                centerFrequencyEdit.Frequency = f;
            }
        }

        private void filterBandwidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.Bandwidth = (int)filterBandwidthNumericUpDown.Value;
            waterfall.FilterBandwidth = _vfo.Bandwidth;
            spectrumAnalyzer.FilterBandwidth = _vfo.Bandwidth;

            if (_vfo.DetectorType == DetectorType.CWL || _vfo.DetectorType == DetectorType.CWU)
            {
                waterfall.FilterOffset = _vfo.CWToneShift - _vfo.Bandwidth / 2;
                spectrumAnalyzer.FilterOffset = waterfall.FilterOffset;
            }

            NotifyPropertyChanged("FilterBandwidth");
        }

        private void filterOrderNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.FilterOrder = (int)filterOrderNumericUpDown.Value;

            NotifyPropertyChanged("FilterOrder");
        }

        private void filterTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _vfo.WindowType = (WindowType)(filterTypeComboBox.SelectedIndex + 1);

            NotifyPropertyChanged("FilterType");
        }

        private void autoCorrectIQCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            _iqBalancer.AutoBalanceIQ = correctIQCheckBox.Checked;

            NotifyPropertyChanged("CorrectIq");
        }

        private void frequencyShiftCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            frequencyShiftNumericUpDown.Enabled = frequencyShiftCheckBox.Checked;
            vfoFrequencyEdit.Minimum = long.MinValue;
            vfoFrequencyEdit.Maximum = long.MaxValue;
            if (frequencyShiftCheckBox.Checked)
            {
                _frequencyShift = (long)frequencyShiftNumericUpDown.Value;
                vfoFrequencyEdit.Frequency += _frequencyShift;
            }
            else
            {
                var shift = _frequencyShift;
                _frequencyShift = 0;
                vfoFrequencyEdit.Frequency -= shift;
            }
            centerFrequencyEdit_FrequencyChanged(null, null);

            NotifyPropertyChanged("FrequencyShiftEnabled");
        }

        private void frequencyShiftNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _frequencyShift = (long)frequencyShiftNumericUpDown.Value;
            centerFrequencyEdit_FrequencyChanged(null, null);

            NotifyPropertyChanged("FrequencyShift");
        }

        #endregion

        #region Mode selection

        private void modeRadioButton_CheckStateChanged(object sender, EventArgs e)
        {
            //filterBandwidthNumericUpDown.Enabled = !wfmRadioButton.Checked;
            filterOrderNumericUpDown.Enabled = !wfmRadioButton.Checked;

            agcDecayNumericUpDown.Enabled = !wfmRadioButton.Checked && !nfmRadioButton.Checked;
            agcSlopeNumericUpDown.Enabled = !wfmRadioButton.Checked && !nfmRadioButton.Checked;
            agcThresholdNumericUpDown.Enabled = !wfmRadioButton.Checked && !nfmRadioButton.Checked;
            agcUseHangCheckBox.Enabled = !wfmRadioButton.Checked && !nfmRadioButton.Checked;
            agcCheckBox.Enabled = !wfmRadioButton.Checked && !nfmRadioButton.Checked;

            fmStereoCheckBox.Enabled = wfmRadioButton.Checked;

            useSquelchCheckBox.Enabled = nfmRadioButton.Checked || amRadioButton.Checked;
            squelchNumericUpDown.Enabled = useSquelchCheckBox.Enabled && useSquelchCheckBox.Checked;
            cwShiftNumericUpDown.Enabled = cwlRadioButton.Checked || cwuRadioButton.Checked;

            if (wfmRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultWFMBandwidth;
                _vfo.DetectorType = DetectorType.WFM;
                _vfo.Bandwidth = DefaultWFMBandwidth;
                waterfall.BandType = BandType.Center;
                spectrumAnalyzer.BandType = BandType.Center;
                stepSizeComboBox.SelectedIndex = 14;

                waterfall.FilterOffset = 0;
                spectrumAnalyzer.FilterOffset = 0;
            }
            else if (nfmRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultNFMBandwidth;
                _vfo.DetectorType = DetectorType.NFM;
                _vfo.Bandwidth = DefaultNFMBandwidth;
                waterfall.BandType = BandType.Center;
                spectrumAnalyzer.BandType = BandType.Center;
                stepSizeComboBox.SelectedIndex = 11;
                useSquelchCheckBox.Checked = true;

                waterfall.FilterOffset = 0;
                spectrumAnalyzer.FilterOffset = 0;
            }
            else if (amRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultAMBandwidth;
                _vfo.DetectorType = DetectorType.AM;
                _vfo.Bandwidth = DefaultAMBandwidth;
                waterfall.BandType = BandType.Center;
                spectrumAnalyzer.BandType = BandType.Center;
                stepSizeComboBox.SelectedIndex = 4;
                useSquelchCheckBox.Checked = false;

                waterfall.FilterOffset = 0;
                spectrumAnalyzer.FilterOffset = 0;
            }
            else if (lsbRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultSSBBandwidth;
                _vfo.DetectorType = DetectorType.LSB;
                _vfo.Bandwidth = DefaultSSBBandwidth;
                waterfall.BandType = BandType.Lower;
                spectrumAnalyzer.BandType = BandType.Lower;
                stepSizeComboBox.SelectedIndex = 2;

                waterfall.FilterOffset = Vfo.MinSSBAudioFrequency;
                spectrumAnalyzer.FilterOffset = Vfo.MinSSBAudioFrequency;
            }
            else if (usbRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultSSBBandwidth;
                _vfo.DetectorType = DetectorType.USB;
                _vfo.Bandwidth = DefaultSSBBandwidth;
                waterfall.BandType = BandType.Upper;
                spectrumAnalyzer.BandType = BandType.Upper;
                stepSizeComboBox.SelectedIndex = 2;

                waterfall.FilterOffset = Vfo.MinSSBAudioFrequency;
                spectrumAnalyzer.FilterOffset = Vfo.MinSSBAudioFrequency;
            }
            else if (dsbRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultDSBBandwidth;
                _vfo.DetectorType = DetectorType.DSB;
                _vfo.Bandwidth = DefaultDSBBandwidth;
                waterfall.BandType = BandType.Center;
                spectrumAnalyzer.BandType = BandType.Center;
                stepSizeComboBox.SelectedIndex = 2;

                waterfall.FilterOffset = 0;
                spectrumAnalyzer.FilterOffset = 0;
            }
            else if (cwlRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultCWBandwidth;
                _vfo.DetectorType = DetectorType.CWL;
                _vfo.Bandwidth = DefaultCWBandwidth;
                waterfall.BandType = BandType.Lower;
                spectrumAnalyzer.BandType = BandType.Lower;
                stepSizeComboBox.SelectedIndex = 2;

                waterfall.FilterOffset = _vfo.CWToneShift - _vfo.Bandwidth / 2;
                spectrumAnalyzer.FilterOffset = waterfall.FilterOffset;
            }
            else if (cwuRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultCWBandwidth;
                _vfo.DetectorType = DetectorType.CWU;
                _vfo.Bandwidth = DefaultCWBandwidth;
                waterfall.BandType = BandType.Upper;
                spectrumAnalyzer.BandType = BandType.Upper;
                stepSizeComboBox.SelectedIndex = 2;

                waterfall.FilterOffset = _vfo.CWToneShift - _vfo.Bandwidth / 2;
                spectrumAnalyzer.FilterOffset = waterfall.FilterOffset;
            }

            NotifyPropertyChanged("DetectorType");
        }
        
        private void fmStereoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _vfo.FmStereo = fmStereoCheckBox.Checked;

            NotifyPropertyChanged("FmStereo");
        }

        private void cwShiftNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.CWToneShift = (int)cwShiftNumericUpDown.Value;
            waterfall.FilterOffset = _vfo.CWToneShift - _vfo.Bandwidth / 2;
            spectrumAnalyzer.FilterOffset = waterfall.FilterOffset;

            NotifyPropertyChanged("CWShift");
        }

        private void squelchNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.SquelchThreshold = (int)squelchNumericUpDown.Value;

            NotifyPropertyChanged("SquelchThreshold");
        }

        private void useSquelchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            squelchNumericUpDown.Enabled = useSquelchCheckBox.Checked;
            if (useSquelchCheckBox.Checked)
            {
                _vfo.SquelchThreshold = (int) squelchNumericUpDown.Value;
            }
            else
            {
                _vfo.SquelchThreshold = 0;
            }

            NotifyPropertyChanged("SquelchEnabled");
        }

        private void stepSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            waterfall.UseSnap = snapFrequencyCheckBox.Checked;
            spectrumAnalyzer.UseSnap = snapFrequencyCheckBox.Checked;

            var stepSize = 0;
            var match = Regex.Match(stepSizeComboBox.Text, "([0-9\\.]+) kHz", RegexOptions.None);
            if (match.Success)
            {
                stepSize = (int)(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) * 1000);
            }
            else
            {
                match = Regex.Match(stepSizeComboBox.Text, "([0-9]+) Hz", RegexOptions.None);
                if (match.Success)
                {
                    stepSize = int.Parse(match.Groups[1].Value);
                }
            }
            if (stepSize > 0)
            {
                //centerFreqNumericUpDown.Increment = stepSize;
                //loFrequencyEdit.Increment = stepSize;
                waterfall.StepSize = stepSize;
                spectrumAnalyzer.StepSize = stepSize;

                if (snapFrequencyCheckBox.Checked && !SourceIsWaveFile)
                {
                    vfoFrequencyEdit.Maximum = long.MaxValue;
                    vfoFrequencyEdit.Minimum = long.MinValue;

                    centerFrequencyEdit.Frequency = (centerFrequencyEdit.Frequency + stepSize / 2) / stepSize * stepSize;
                    vfoFrequencyEdit.Frequency = (vfoFrequencyEdit.Frequency + stepSize / 2) / stepSize * stepSize;

                    vfoFrequencyEdit.Maximum = centerFrequencyEdit.Frequency + _frequencyShift + (int)(_vfo.SampleRate / 2);
                    vfoFrequencyEdit.Minimum = centerFrequencyEdit.Frequency + _frequencyShift - (int)(_vfo.SampleRate / 2);

                    vfoFrequencyEdit.Maximum = (vfoFrequencyEdit.Maximum) / waterfall.StepSize * waterfall.StepSize;
                    vfoFrequencyEdit.Minimum = 2 * spectrumAnalyzer.CenterFrequency - vfoFrequencyEdit.Maximum;
                }
            }
            _stepSize = stepSize;
            if (sender == snapFrequencyCheckBox)
            {
                NotifyPropertyChanged("SnapToGrid");
            }
            NotifyPropertyChanged("StepSize");
        }

        private void panview_BandwidthChanged(object sender, BandwidthEventArgs e)
        {
            if (e.Bandwidth < filterBandwidthNumericUpDown.Minimum)
            {
                e.Bandwidth = (int)filterBandwidthNumericUpDown.Minimum;
            }
            else if (e.Bandwidth > filterBandwidthNumericUpDown.Maximum)
            {
                e.Bandwidth = (int)filterBandwidthNumericUpDown.Maximum;
            }

            filterBandwidthNumericUpDown.Value = e.Bandwidth;
        }

        private void frontendGuiButton_Click(object sender, EventArgs e)
        {
            if (SourceIsWaveFile)
            {
                SelectWaveFile();
            }
            else if (_frontendController != null)
            {
                _frontendController.ShowSettingGUI(this);
            }
        }
       
        #endregion

        #endregion

        #region AGC

        private void agcCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _vfo.UseAGC = agcCheckBox.Checked;
            agcThresholdNumericUpDown.Enabled = agcCheckBox.Checked;
            agcDecayNumericUpDown.Enabled = agcCheckBox.Checked;
            agcSlopeNumericUpDown.Enabled = agcCheckBox.Checked;
            agcUseHangCheckBox.Enabled = agcCheckBox.Checked;

            NotifyPropertyChanged("UseAgc");
        }

        private void agcUseHangCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _vfo.AgcHang = agcUseHangCheckBox.Checked;

            NotifyPropertyChanged("UseHang");
        }

        private void agcDecayNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.AgcDecay = (int) agcDecayNumericUpDown.Value;

            NotifyPropertyChanged("AgcDecay");
        }

        private void agcThresholdNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.AgcThreshold = (int) agcThresholdNumericUpDown.Value;

            NotifyPropertyChanged("AgcThreshold");
        }

        private void agcSlopeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.AgcSlope = (int) agcSlopeNumericUpDown.Value;

            NotifyPropertyChanged("AgcSlope");
        }

        private void swapIQCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _streamControl.SwapIQ = swapIQCheckBox.Checked;

            NotifyPropertyChanged("SwapIq");
        }

        #endregion
        
        #region Display settings

        private void viewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (viewComboBox.SelectedIndex)
            {
                case 0:
                    panSplitContainer.Panel1Collapsed = false;
                    panSplitContainer.Panel2Collapsed = true;
                    break;

                case 1:
                    panSplitContainer.Panel1Collapsed = true;
                    panSplitContainer.Panel2Collapsed = false;
                    break;

                case 2:
                    panSplitContainer.Panel1Collapsed = false;
                    panSplitContainer.Panel2Collapsed = false;
                    break;
            }
        }

        private void fftResolutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (_fftResolutionLockObject)
            {
                _fftBins = int.Parse(fftResolutionComboBox.SelectedItem.ToString());
                InitFFTBuffers();
                BuildFFTWindow();
                _fftSpectrumResized = true;
            }

            NotifyPropertyChanged("FFTResolution");
        }

        private void fftWindowComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _fftWindowType = (WindowType)fftWindowComboBox.SelectedIndex;
            BuildFFTWindow();
        }

        private void gradientButton_Click(object sender, EventArgs e)
        {
            var gradient = GradientDialog.GetGradient(waterfall.GradientColorBlend);
            if (gradient != null && gradient.Positions.Length > 0)
            {
                waterfall.GradientColorBlend = gradient;
                Utils.SaveSetting("gradient", GradientToString(gradient.Colors));
            }
        }

        private static string GradientToString(Color[] colors)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < colors.Length; i++)
            {
                sb.AppendFormat(",{0:X2}{1:X2}{2:X2}", colors[i].R, colors[i].G, colors[i].B);
            }
            return sb.ToString().Substring(1);
        }

        private void fftContrastTrackBar_Changed(object sender, EventArgs e)
        {
            waterfall.Contrast = fftContrastTrackBar.Value * 100 / (fftContrastTrackBar.Maximum - fftContrastTrackBar.Minimum);
        }

        private void sAttackTrackBar_ValueChanged(object sender, EventArgs e)
        {
            spectrumAnalyzer.Attack = sAttackTrackBar.Value / (double) sAttackTrackBar.Maximum;

            NotifyPropertyChanged("SAttack");
        }

        private void sDecayTrackBar_ValueChanged(object sender, EventArgs e)
        {
            spectrumAnalyzer.Decay = sDecayTrackBar.Value / (double)sDecayTrackBar.Maximum;

            NotifyPropertyChanged("SDecay");
        }

        private void wAttackTrackBar_ValueChanged(object sender, EventArgs e)
        {
            waterfall.Attack = wAttackTrackBar.Value / (double)wAttackTrackBar.Maximum;

            NotifyPropertyChanged("WAttack");
        }

        private void wDecayTrackBar_ValueChanged(object sender, EventArgs e)
        {
            waterfall.Decay = wDecayTrackBar.Value / (double)wDecayTrackBar.Maximum;

            NotifyPropertyChanged("WDecay");
        }

        private void markPeaksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            spectrumAnalyzer.MarkPeaks = markPeaksCheckBox.Checked;

            NotifyPropertyChanged("MarkPeaks");
        }

        private void useTimestampCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            waterfall.UseTimestamps = useTimestampsCheckBox.Checked;

            NotifyPropertyChanged("UseTimeMarkers");
        }

        private void fftSpeedTrackBar_ValueChanged(object sender, EventArgs e)
        {
            _fftTimer.Interval = (int) (1.0 / fftSpeedTrackBar.Value * 1000.0);
        }

        private void fftZoomTrackBar_ValueChanged(object sender, EventArgs e)
        {
            spectrumAnalyzer.Zoom = fftZoomTrackBar.Value * 100 / fftZoomTrackBar.Maximum;
            waterfall.Zoom = spectrumAnalyzer.Zoom;
        }

        private void MainForm_Move(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                _lastLocation = Location;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                _lastSize = Size;
            }
        }

        private int[] GetCollapsiblePanelStates()
        {
            var states = new List<int>();
            var currentPanel = radioCollapsiblePanel;
            while (currentPanel != null)
            {
                states.Add((int) currentPanel.PanelState);
                currentPanel = currentPanel.NextPanel;
            }
            return states.ToArray();
        }

        private void fftOffsetTrackBar_Scroll(object sender, EventArgs e)
        {
            spectrumAnalyzer.DisplayOffset = - fftOffsetTrackBar.Value * 10;
        }

        private void fftRangeTrackBar_Scroll(object sender, EventArgs e)
        {
            spectrumAnalyzer.DisplayRange = fftRangeTrackBar.Value * 10;
        }

        #endregion
        
        #region Plugin Methods

        private void InitialiseSharpPlugins()
        {
            _sharpControlProxy = new SharpControlProxy(this);
            var sharpPlugins = (NameValueCollection) ConfigurationManager.GetSection("sharpPlugins");

            if (sharpPlugins == null)
            {
                MessageBox.Show(
                    "Configuration section 'sharpPlugins' was not found. Please check 'SDRSharp.exe.config'.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        
            foreach (string key in sharpPlugins.Keys)
            {
                try
                {
                    var fullyQualifiedTypeName = sharpPlugins[key];
                    var patterns = fullyQualifiedTypeName.Split(',');
                    var typeName = patterns[0];
                    var assemblyName = patterns[1];
                    var objectHandle = Activator.CreateInstance(assemblyName, typeName);
                    var plugin = (ISharpPlugin) objectHandle.Unwrap();

                    _sharpPlugins.Add(key, plugin);

                    plugin.Initialize(_sharpControlProxy);
                    if (plugin.HasGui)
                    {
                        CreatePluginCollapsiblePanel(plugin);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading '" + sharpPlugins[key] + "' - " + ex.Message);
                }
            }

            var collapsiblePanelStates = Utils.GetIntArraySetting("collapsiblePanelStates", null);
            if (collapsiblePanelStates != null)
            {
                CollapsiblePanel.CollapsiblePanel currentPanel = radioCollapsiblePanel;
                for (var i = 0; i < collapsiblePanelStates.Length && currentPanel != null; i++)
                {
                    currentPanel.PanelState = (CollapsiblePanel.PanelStateOptions)collapsiblePanelStates[i];
                    currentPanel = currentPanel.NextPanel;
                }
            }
        }

        private void CreatePluginCollapsiblePanel(ISharpPlugin plugin)
        {
            var panelContents = plugin.GuiControl;

            if (panelContents != null)
            {
                panelContents.Padding = new Padding(0, 20, 0, 0);

                var newPanel = new CollapsiblePanel.CollapsiblePanel();
                newPanel.PanelTitle = plugin.DisplayName + " (Plugin)";

                newPanel.PanelState = CollapsiblePanel.PanelStateOptions.Collapsed;
                newPanel.Controls.Add(panelContents);

                if (displayCollapsiblePanel.NextPanel == null)
                {
                    displayCollapsiblePanel.NextPanel = newPanel;
                }
                else
                {
                    newPanel.NextPanel = displayCollapsiblePanel.NextPanel;
                    displayCollapsiblePanel.NextPanel = newPanel;
                }

                newPanel.Width = displayCollapsiblePanel.Width;
                newPanel.ExpandedHeight = panelContents.Height;

                panelContents.Width = newPanel.Width;

                controlPanel.Controls.Add(newPanel);
            }
        }

        public void StartRadio()
        {
            playStopButton.Text = "Stop";
            Open();
            _streamControl.Play();
            ThreadPool.QueueUserWorkItem(ProcessFFT);
            sampleRateComboBox.Enabled = false;
            inputDeviceComboBox.Enabled = false;
            outputDeviceComboBox.Enabled = false;
            latencyNumericUpDown.Enabled = false;

            NotifyPropertyChanged("StartRadio");
        }

        public void StopRadio()
        {
            playStopButton.Text = "Play";
            _streamControl.Stop();
            _iqBalancer.Reset();
            _fftStream.Flush();
            if (!SourceIsWaveFile)
            {
                inputDeviceComboBox.Enabled = _frontendController == null ? true : _frontendController.IsSoundCardBased;
                sampleRateComboBox.Enabled = _frontendController == null ? true : _frontendController.IsSoundCardBased;
            }
            outputDeviceComboBox.Enabled = true;
            latencyNumericUpDown.Enabled = true;
            _fftEvent.Set();

            NotifyPropertyChanged("StopRadio");
        }

        public void GetSpectrumSnapshot(byte[] destArray)
        {
            lock (_fftResolutionLockObject)
            {
                Fourier.ScaleFFT(_fftSpectrumPtr, _scaledFFTSpectrumPtr, _fftBins, -130.0f, 0.0f);
                fixed (byte* destPtr = destArray)
                {
                    Fourier.SmoothCopy(_scaledFFTSpectrumPtr, destPtr, _fftBins, destArray.Length, 1.0f, 0);
                }
            }
        }

        public void RegisterStreamHook(object streamHook)
        {
            if (!IsPlaying)
            {
                _streamHookManager.RegisterStreamHook(streamHook);
            }
        }

        public void UnregisterStreamHook(object streamHook)
        {
            if (!IsPlaying)
            {
                _streamHookManager.UnregisterStreamHook(streamHook);
            }
        }


        #endregion

        #region INotifyPropertyChanged

        private void NotifyPropertyChanged(string property)
        {

            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
