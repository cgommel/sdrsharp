using System;
using System.Collections;
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
using SDRSharp.Radio;
using SDRSharp.PanView;
using SDRSharp.Radio.PortAudio;
using System.Runtime.CompilerServices;

namespace SDRSharp
{
    public unsafe partial class MainForm : Form
    {
        #region Private fields

        private static readonly string _baseTitle = "SDR# v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        private const int DefaultNFMBandwidth = 12500;
        private const int DefaultWFMBandwidth = 180000;
        private const int DefaultAMBandwidth = 10000;
        private const int DefaultDSBBandwidth = 6000;
        private const int DefaultSSBBandwidth = 2400;
        private const int DefaultCWBandwidth = 300;
        private const int MaxFFTBins = 1024 * 1024 * 4;
        private const int MaxFFTQueue = 3;
        private const int FFTOverlapLimit = 65536 / 2;

        private int _fftBins;
        private int _fftFillPosition;
        private bool _fftOverlap;
        private bool _fftSpectrumAvailable;
        private bool _extioChangingFrequency;
        private bool _extioChangingSamplerate;

        private WindowType _fftWindowType;
        private IFrontendController _frontendController;
        private readonly Dictionary<string, IFrontendController> _frontendControllers = new Dictionary<string, IFrontendController>();
        private readonly IQBalancer _iqBalancer = new IQBalancer();
        private readonly Vfo _vfo = new Vfo();
        private readonly StreamControl _streamControl = new StreamControl();
        private readonly ComplexFifoStream _fftStream = new ComplexFifoStream();
        private readonly Complex[] _iqBuffer = new Complex[MaxFFTBins];
        private readonly Complex[] _fftBuffer = new Complex[MaxFFTBins];
        private readonly float[] _fftWindow = new float[MaxFFTBins];
        private readonly float[] _fftSpectrum = new float[MaxFFTBins];
        private readonly byte[] _scaledFFTSpectrum = new byte[MaxFFTBins];
        private readonly Pipe<byte[]> _fftQueue = new Pipe<byte[]>(MaxFFTQueue);
        private readonly System.Windows.Forms.Timer _fftTimer;
        private readonly System.Threading.Timer _tuneTimer;
        private long _frequencyToSet;
        private long _frequencySet;
        private long _frequencyShift;

        #endregion

        #region Initialization and Termination

        public MainForm()
        {
            InitializeComponent();
            _fftTimer = new System.Windows.Forms.Timer(components);
            _tuneTimer = new System.Threading.Timer(tuneTimer_Callback, null, 0, 10);

            for (var i = 0; i < MaxFFTQueue; i++)
            {
                _fftQueue.AdvanceWrite();
                _fftQueue.Head = new byte[FFTOverlapLimit];
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Initialize audio devices

            var defaultIndex = 0;
            var devices = AudioDevice.GetDevices(DeviceDirection.Input);
            for (var i = 0; i < devices.Count; i++)
            {
                inputDeviceComboBox.Items.Add(devices[i]);
                if (devices[i].IsDefault)
                {
                    defaultIndex = i;
                }
            }
            if (inputDeviceComboBox.Items.Count > 0)
            {
                inputDeviceComboBox.SelectedIndex = defaultIndex;
            }

            defaultIndex = 0;
            devices = AudioDevice.GetDevices(DeviceDirection.Output);
            for (int i = 0; i < devices.Count; i++)
            {
                outputDeviceComboBox.Items.Add(devices[i]);
                if (devices[i].IsDefault)
                {
                    defaultIndex = i;
                }
            }
            if (outputDeviceComboBox.Items.Count > 0)
            {
                outputDeviceComboBox.SelectedIndex = defaultIndex;
            }
            
            _streamControl.AudioGain = 30.0f;
            _streamControl.BufferNeeded += ProcessBuffer;

            #endregion

            #region Initialize the VFO

            _vfo.DetectorType = DetectorType.AM;
            _vfo.Bandwidth = DefaultAMBandwidth;
            _vfo.FilterOrder = 400;
            _vfo.SquelchThreshold = 0;
            _vfo.UseAGC = true;
            _vfo.AgcThreshold = -100.0f;
            _vfo.AgcDecay = 100;
            _vfo.AgcSlope = 0;
            _vfo.AgcHang = true;
            _vfo.CWToneShift = Vfo.DefaultCwSideTone;

            stepSizeComboBox.SelectedIndex = 4;

            #endregion

            #region Initialize FFT display

            _fftBins = 4096;
            _fftOverlap = true;

            viewComboBox.SelectedIndex = 2;
            fftResolutionComboBox.SelectedIndex = 3;
            sampleRateComboBox.SelectedIndex = 7;

            _fftWindowType = WindowType.BlackmanHarris;
            fftWindowComboBox.SelectedIndex = (int) _fftWindowType;
            filterTypeComboBox.SelectedIndex = (int) WindowType.BlackmanHarris - 1;

            cwShiftNumericUpDown.Value = Vfo.DefaultCwSideTone;
            
            waterfall.FilterBandwidth = _vfo.Bandwidth;
            waterfall.Frequency = _vfo.Frequency;
            waterfall.FilterOffset = Vfo.MinSSBAudioFrequency;
            waterfall.BandType = BandType.Center;

            spectrumAnalyzer.FilterBandwidth = _vfo.Bandwidth;
            spectrumAnalyzer.Frequency = _vfo.Frequency;
            spectrumAnalyzer.FilterOffset = Vfo.MinSSBAudioFrequency;
            spectrumAnalyzer.BandType = BandType.Center;

            frequencyNumericUpDown.Value = 0;

            _fftTimer.Tick += fftTimer_Tick;
            _fftTimer.Interval = Utils.GetIntSetting("displayTimerInterval", 50);
            _fftTimer.Enabled = true;

            spectrumAnalyzer.Attack = Utils.GetDoubleSetting("spectrumAnalyzerAttack", 0.9);
            sAttackTrackBar.Value = (int)(spectrumAnalyzer.Attack * sAttackTrackBar.Maximum);

            spectrumAnalyzer.Decay = Utils.GetDoubleSetting("spectrumAnalyzerDecay", 0.3);
            sDecayTrackBar.Value = (int)(spectrumAnalyzer.Decay * sDecayTrackBar.Maximum);

            waterfall.Attack = Utils.GetDoubleSetting("waterfallAttack", 0.9);
            wAttackTrackBar.Value = (int)(waterfall.Attack * wAttackTrackBar.Maximum);

            waterfall.Decay = Utils.GetDoubleSetting("waterfallDecay", 0.5);
            wDecayTrackBar.Value = (int)(waterfall.Decay * wDecayTrackBar.Maximum);

            #endregion

            #region Initialize the plugins

            var frontendPlugins = (Hashtable) ConfigurationManager.GetSection("frontendPlugins");

            foreach (string key in frontendPlugins.Keys)
            {
                try
                {
                    var fullyQualifiedTypeName = (string) frontendPlugins[key];
                    var patterns = fullyQualifiedTypeName.Split(',');
                    var typeName = patterns[0];
                    var assemblyName = patterns[1];
                    var objectHandle = Activator.CreateInstance(assemblyName, typeName);
                    var controller = (IFrontendController) objectHandle.Unwrap();
                    _frontendControllers.Add(key, controller);
                    frontEndComboBox.Items.Add(key);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading '" + frontendPlugins[key] + "' - " + ex.Message);
                }
            }

            var extIOs = Directory.GetFiles(".", "ExtIO_*.dll");

            var dropDownWidth = frontEndComboBox.Width;
            var graphics = frontEndComboBox.CreateGraphics();

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
                    var size = graphics.MeasureString(displayName, frontEndComboBox.Font);
                    if (size.Width > dropDownWidth)
                    {
                        dropDownWidth = (int) size.Width;
                    }
                    _frontendControllers.Add(displayName, controller);
                    frontEndComboBox.Items.Add(displayName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading '" + Path.GetFileName(extIO) + "'\r\n" + ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            frontEndComboBox.DropDownWidth = dropDownWidth;

            ExtIO.SampleRateChanged += ExtIO_SampleRateChanged;
            ExtIO.LOFreqChanged += ExtIO_LOFreqChanged;

            frontEndComboBox.Items.Add("Other");
            frontEndComboBox.SelectedIndex = frontEndComboBox.Items.Count - 1;

            #endregion
        }

        private void ExtIO_LOFreqChanged(int frequency)
        {
            BeginInvoke(new Action(() =>
                            {

                                _extioChangingFrequency = true;
                                centerFreqNumericUpDown.Value = frequency;
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
            _tuneTimer.Dispose();
            _streamControl.Stop();
            if (_frontendController != null)
            {
                _frontendController.Close();
                _frontendController = null;
            }

            Utils.SaveSetting("spectrumAnalyzerAttack", spectrumAnalyzer.Attack.ToString(CultureInfo.InvariantCulture));
            Utils.SaveSetting("spectrumAnalyzerDecay", spectrumAnalyzer.Decay.ToString(CultureInfo.InvariantCulture));
            Utils.SaveSetting("waterfallAttack", waterfall.Attack.ToString(CultureInfo.InvariantCulture));
            Utils.SaveSetting("waterfallDecay", waterfall.Decay.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region IQ FFT and DSP handlers

        private void ProcessBuffer(Complex* iqBuffer, float* audioBuffer, int length)
        {
            _iqBalancer.Process(iqBuffer, length);
            if (_fftStream.Length < _fftBins || _fftOverlap)
                _fftStream.Write(iqBuffer, length);
            _vfo.ProcessBuffer(iqBuffer, audioBuffer, length);
        }

        private void ProcessFFT()
        {
            while (_streamControl.IsPlaying || _extioChangingSamplerate)
            {
                // http://www.designnews.com/author.asp?section_id=1419&doc_id=236273&piddl_msgid=522392
                var fftGain = (float) (10.0 * Math.Log10((double) _fftBins / 2));
                var compensation = 24.0f - fftGain;

                while (_streamControl.IsPlaying)
                {
                    if (_fftOverlap)
                    {
                        var fftRate = _fftBins / (_fftTimer.Interval * 0.001);
                        var overlapRatio = _streamControl.SampleRate / fftRate;

                        var bytes = (int)(_fftBins * overlapRatio);

                        if (_fftStream.Length < bytes)
                        {
                            break;
                        }

                        if (bytes > _fftBins)
                        {
                            _fftStream.Advance(bytes - _fftBins);
                        }

                        var toRead = Math.Min(bytes, _fftBins);
                        Array.Copy(_iqBuffer, toRead, _iqBuffer, 0, _fftBins - toRead);

                        var total = 0;
                        while (_streamControl.IsPlaying && total < toRead)
                        {
                            var len = Math.Max(1000, _fftStream.Length);
                            len = Math.Min(len, toRead - total);
                            total += _fftStream.Read(_iqBuffer, _fftBins - toRead + total, len);
                        }

                        Array.Copy(_iqBuffer, _fftBuffer, _fftBins);
                        Fourier.ApplyFFTWindow(_fftBuffer, _fftWindow, _fftBins);
                        Fourier.ForwardTransform(_fftBuffer, _fftBins);
                        Fourier.SpectrumPower(_fftBuffer, _fftSpectrum, _fftBins, compensation);

                        lock (_fftQueue)
                        {
                            if (_fftQueue.Head.Length >= _fftBins)
                            {
                                Fourier.ScaleFFT(_fftSpectrum, _fftQueue.Head, _fftBins);
                                _fftQueue.AdvanceWrite();
                            }
                        }
                    }
                    else
                    {
                        var len = Math.Min(_fftBins - _fftFillPosition, _fftStream.Length);
                        _fftFillPosition += _fftStream.Read(_fftBuffer, _fftFillPosition, len);

                        if (_fftFillPosition < _fftBins)
                        {
                            break;
                        }
                        _fftFillPosition = 0;

                        Fourier.ApplyFFTWindow(_fftBuffer, _fftWindow, _fftBins);
                        Fourier.ForwardTransform(_fftBuffer, _fftBins);
                        Fourier.SpectrumPower(_fftBuffer, _fftSpectrum, _fftBins, compensation);
                        lock (_scaledFFTSpectrum)
                        {
                            if (_scaledFFTSpectrum.Length >= _fftBins)
                            {
                                Fourier.ScaleFFT(_fftSpectrum, _scaledFFTSpectrum, _fftBins);
                                _fftSpectrumAvailable = true;
                            }
                        }
                    }
                }

                Thread.Sleep(10);
            }
            _fftStream.Flush();
        }

        private void fftTimer_Tick(object sender, EventArgs e)
        {
            if (_streamControl.IsPlaying)
            {
                if (_fftOverlap)
                {
                    lock (_fftQueue)
                    {
                        Array.Copy(_fftQueue.Tail, _scaledFFTSpectrum, _fftBins);
                        _fftQueue.AdvanceRead();
                    }
                    if (!panSplitContainer.Panel1Collapsed)
                    {
                        spectrumAnalyzer.Render(_scaledFFTSpectrum, _fftBins);
                    }
                    if (!panSplitContainer.Panel2Collapsed)
                    {
                        waterfall.Render(_scaledFFTSpectrum, _fftBins);
                    }
                }
                else if (_fftSpectrumAvailable)
                {
                    lock (_scaledFFTSpectrum)
                    {
                        if (!panSplitContainer.Panel1Collapsed)
                        {
                            spectrumAnalyzer.Render(_scaledFFTSpectrum, _fftBins);
                        }
                        if (!panSplitContainer.Panel2Collapsed)
                        {
                            waterfall.Render(_scaledFFTSpectrum, _fftBins);
                        }
                        _fftSpectrumAvailable = false;
                    }
                }
            }

            spectrumAnalyzer.Perform();
            waterfall.Perform();
        }

        private void iqTimer_Tick(object sender, EventArgs e)
        {
            Text = string.Format(_baseTitle + " - IQ Imbalance: Gain = {0:F3} Phase = {1:F3}°", _iqBalancer.Gain, _iqBalancer.Phase * 180 / Math.PI);
            if (_vfo.SignalIsStereo)
            {
                Text += " ((( stereo )))";
            }
        }

        private void BuildFFTWindow()
        {
            var window = FilterBuilder.MakeWindow(_fftWindowType, _fftBins);
            Array.Copy(window, _fftWindow, _fftBins);
        }

        #endregion

        #region IQ source selection

        private void iqStreamRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (iqStreamRadioButton.Checked)
            {
                _streamControl.Stop();
                wavFileTextBox.Enabled = false;
                fileSelectButton.Enabled = false;
                playButton.Enabled = true;
                stopButton.Enabled = false;
                sampleRateComboBox.Enabled = true;
                inputDeviceComboBox.Enabled = true;
                outputDeviceComboBox.Enabled = true;
                latencyNumericUpDown.Enabled = true;
                centerFreqNumericUpDown.Enabled = true;
                frontEndComboBox.Enabled = true;
                frontendGuiButton.Enabled = true;
                frequencyShiftCheckBox.Enabled = true;
                frequencyShiftNumericUpDown.Enabled = frequencyShiftCheckBox.Checked;

                frontEndComboBox_SelectedIndexChanged(null, null);
            }
        }

        private void waveFileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (waveFileRadioButton.Checked)
            {
                _streamControl.Stop();
                wavFileTextBox.Enabled = true;
                fileSelectButton.Enabled = true;
                playButton.Enabled = true;
                stopButton.Enabled = false;
                sampleRateComboBox.Enabled = false;
                inputDeviceComboBox.Enabled = false;
                outputDeviceComboBox.Enabled = true;
                latencyNumericUpDown.Enabled = true;
                centerFreqNumericUpDown.Enabled = false;
                frontEndComboBox.Enabled = false;
                frontendGuiButton.Enabled = false;
                frequencyShiftCheckBox.Enabled = false;
                frequencyShiftNumericUpDown.Enabled = false;

                centerFreqNumericUpDown.Value = 0;
                centerFreqNumericUpDown_ValueChanged(null, null);
                frequencyNumericUpDown.Value = 0;
                frequencyNumericUpDown_ValueChanged(null, null);
            }
        }

        private void frontEndComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var frontendName = (string) frontEndComboBox.SelectedItem;
            if (frontendName == "Other")
            {
                if (_frontendController != null)
                {
                    _frontendController.Close();
                    _frontendController = null;
                }

                inputDeviceComboBox.Enabled = true;
                sampleRateComboBox.Enabled = true;
                centerFreqNumericUpDown.Value = 0;
                centerFreqNumericUpDown_ValueChanged(null, null);
                frequencyNumericUpDown.Value = _frequencyShift;
                frequencyNumericUpDown_ValueChanged(null, null);
                frontendGuiButton.Enabled = false;
                frequencyShiftCheckBox.Enabled = true;
                frequencyShiftNumericUpDown.Enabled = frequencyShiftCheckBox.Checked;
                return;
            }
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
                    sampleRateComboBox.Text = _frontendController.Samplerate.ToString();
                }
                if (_frontendController.Samplerate > 0)
                {
                    waterfall.SpectrumWidth = (int) _frontendController.Samplerate;
                    spectrumAnalyzer.SpectrumWidth = (int) _frontendController.Samplerate;
                }
                _vfo.SampleRate = _frontendController.Samplerate;
                _vfo.Frequency = 0;
                centerFreqNumericUpDown.Value = _frontendController.Frequency;
                centerFreqNumericUpDown_ValueChanged(null, null);
                frequencyNumericUpDown.Value = _frontendController.Frequency + _frequencyShift;
                frequencyNumericUpDown_ValueChanged(null, null);
            }
            catch
            {
                frontEndComboBox.SelectedIndex = frontEndComboBox.Items.Count - 1;
                if (_frontendController != null)
                {
                    _frontendController.Close();
                }
                _frontendController = null;
                MessageBox.Show(
                    frontendName + " is either not connected or its driver is not working properly.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            frontendGuiButton.Enabled = frontEndComboBox.SelectedIndex < frontEndComboBox.Items.Count - 1;
        }

        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                wavFileTextBox.Text = openDlg.FileName;
                playButton.Enabled = true;
                stopButton.Enabled = false;
                _streamControl.Stop();
            }
        }

        #endregion

        #region Audio settings

        private void Open()
        {
            var inputDevice = (AudioDevice) inputDeviceComboBox.SelectedItem;
            var outputDevice = (AudioDevice) outputDeviceComboBox.SelectedItem;
            var oldCenterFrequency = centerFreqNumericUpDown.Value;
            Match match;
            if (iqStreamRadioButton.Checked)
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
            else
            {
                if (!File.Exists(wavFileTextBox.Text))
                {
                    return;
                }
                _streamControl.OpenFile(wavFileTextBox.Text, outputDevice.Index, (int) latencyNumericUpDown.Value);

                var friendlyFilename = "" + Path.GetFileName(wavFileTextBox.Text);
                match = Regex.Match(friendlyFilename, "([0-9]+)kHz", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var center = int.Parse(match.Groups[1].Value) * 1000;
                    centerFreqNumericUpDown.Value = center;
                }
                else
                {
                    centerFreqNumericUpDown.Value = 0;
                }
                centerFreqNumericUpDown_ValueChanged(null, null);
            }

            _vfo.SampleRate = _streamControl.SampleRate;
            _vfo.DecimationStageCount = _streamControl.DecimationStageCount;
            spectrumAnalyzer.SpectrumWidth = (int) _streamControl.SampleRate;
            waterfall.SpectrumWidth = spectrumAnalyzer.SpectrumWidth;

            frequencyNumericUpDown.Maximum = (long) centerFreqNumericUpDown.Value + (int) (_streamControl.SampleRate / 2);
            frequencyNumericUpDown.Minimum = (long) centerFreqNumericUpDown.Value - (int) (_streamControl.SampleRate / 2);

            if (centerFreqNumericUpDown.Value != oldCenterFrequency)
            {
                frequencyNumericUpDown.Value = centerFreqNumericUpDown.Value + _frequencyShift;

                zoomTrackBar.Value = 0;
                zoomTrackBar_Scroll(null, null);
            }
            
            frequencyNumericUpDown_ValueChanged(null, null);

            BuildFFTWindow();
        }

        private void audioGainTrackBar_ValueChanged(object sender, EventArgs e)
        {
            _streamControl.AudioGain = audioGainTrackBar.Value;
        }

        private void filterAudioCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _vfo.FilterAudio = filterAudioCheckBox.Checked;
        }

        #endregion

        #region Main controls

        private void playButton_Click(object sender, EventArgs e)
        {
            Open();
            try
            {
                _streamControl.Play();
                new Thread(ProcessFFT).Start();
                playButton.Enabled = false;
                stopButton.Enabled = true;
                sampleRateComboBox.Enabled = false;
                inputDeviceComboBox.Enabled = false;
                outputDeviceComboBox.Enabled = false;
                latencyNumericUpDown.Enabled = false;
                frontEndComboBox.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _streamControl.Stop();
            _fftStream.Flush();
            playButton.Enabled = true;
            stopButton.Enabled = false;
            if (iqStreamRadioButton.Checked)
            {
                inputDeviceComboBox.Enabled = _frontendController == null ? true : _frontendController.IsSoundCardBased;
                sampleRateComboBox.Enabled = _frontendController == null ? true : _frontendController.IsSoundCardBased;
                frontEndComboBox.Enabled = true;
            }
            outputDeviceComboBox.Enabled = true;
            latencyNumericUpDown.Enabled = true;
        }

        #endregion

        #region Radio settings

        #region Frequency and filters

        private void frequencyNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            waterfall.Frequency = (long) frequencyNumericUpDown.Value;
            spectrumAnalyzer.Frequency = (long) frequencyNumericUpDown.Value;
            _vfo.Frequency = (int) (waterfall.Frequency - (long) centerFreqNumericUpDown.Value - _frequencyShift);
        }

        private void centerFreqNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var newCenterFreq = (long) centerFreqNumericUpDown.Value;
            waterfall.CenterFrequency = newCenterFreq + _frequencyShift;
            spectrumAnalyzer.CenterFrequency = newCenterFreq + _frequencyShift;

            frequencyNumericUpDown.Maximum = decimal.MaxValue;
            frequencyNumericUpDown.Minimum = decimal.MinValue;
            frequencyNumericUpDown.Value = newCenterFreq + _vfo.Frequency + _frequencyShift;
            frequencyNumericUpDown.Maximum = newCenterFreq + (int) (_vfo.SampleRate / 2) + _frequencyShift;
            frequencyNumericUpDown.Minimum = newCenterFreq - (int) (_vfo.SampleRate / 2) + _frequencyShift;

            if (snapFrequencyCheckBox.Checked)
            {
                frequencyNumericUpDown.Maximum = ((long) frequencyNumericUpDown.Maximum) / waterfall.StepSize * waterfall.StepSize;
                frequencyNumericUpDown.Minimum = 2 * spectrumAnalyzer.CenterFrequency - frequencyNumericUpDown.Maximum;
            }

            if (_frontendController != null && iqStreamRadioButton.Checked && !_extioChangingFrequency)
            {
                lock (_tuneTimer)
                {
                    _frequencyToSet = newCenterFreq;
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void tuneTimer_Callback(object state)
        {
            long copyOfFrequencyToSet;
            lock (_tuneTimer)
            {
                copyOfFrequencyToSet = _frequencyToSet;
            }
            if (_frontendController != null  && _frequencySet != copyOfFrequencyToSet)
            {
                _frequencySet = copyOfFrequencyToSet;
                _frontendController.Frequency = copyOfFrequencyToSet;
            }
        }

        private void panview_FrequencyChanged(object sender, FrequencyEventArgs e)
        {
            if (e.Frequency >= frequencyNumericUpDown.Minimum &&
                e.Frequency <= frequencyNumericUpDown.Maximum)
            {
                frequencyNumericUpDown.Value = e.Frequency;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void panview_CenterFrequencyChanged(object sender, FrequencyEventArgs e)
        {
            if (iqStreamRadioButton.Checked)
            {
                centerFreqNumericUpDown.Value = e.Frequency - _frequencyShift;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void filterBandwidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.Bandwidth = (int) filterBandwidthNumericUpDown.Value;
            waterfall.FilterBandwidth = _vfo.Bandwidth;
            spectrumAnalyzer.FilterBandwidth = _vfo.Bandwidth;

            if (_vfo.DetectorType == DetectorType.CWL || _vfo.DetectorType == DetectorType.CWU)
            {
                waterfall.FilterOffset = _vfo.CWToneShift - _vfo.Bandwidth / 2;
                spectrumAnalyzer.FilterOffset = waterfall.FilterOffset;
            }
        }

        private void filterOrderNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.FilterOrder = (int) filterOrderNumericUpDown.Value;
        }

        private void filterTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _vfo.WindowType = (WindowType) (filterTypeComboBox.SelectedIndex + 1);
        }

        private void autoCorrectIQCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _iqBalancer.AutoBalanceIQ = correctIQCheckBox.Checked;
        }

        private void frequencyShiftCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            frequencyShiftNumericUpDown.Enabled = frequencyShiftCheckBox.Checked;
            frequencyNumericUpDown.Minimum = long.MinValue;
            frequencyNumericUpDown.Maximum = long.MaxValue;
            if (frequencyShiftCheckBox.Checked)
            {
                _frequencyShift = (long) frequencyShiftNumericUpDown.Value;
                frequencyNumericUpDown.Value += _frequencyShift;
            }
            else
            {
                var shift = _frequencyShift;
                _frequencyShift = 0;
                frequencyNumericUpDown.Value -= shift;
            }
            centerFreqNumericUpDown_ValueChanged(null, null);
        }

        private void frequencyShiftNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _frequencyShift = (long) frequencyShiftNumericUpDown.Value;
            centerFreqNumericUpDown_ValueChanged(null, null);
        }

        #endregion

        #region Mode selection

        private void modeRadioButton_CheckedChanged(object sender, EventArgs e)
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
        }

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

        private void fmStereoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _vfo.FmStereo = fmStereoCheckBox.Checked;
        }

        private void cwShiftNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.CWToneShift = (int) cwShiftNumericUpDown.Value;
            waterfall.FilterOffset = _vfo.CWToneShift - _vfo.Bandwidth / 2;
            spectrumAnalyzer.FilterOffset = waterfall.FilterOffset;
        }

        private void squelchNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.SquelchThreshold = (int) squelchNumericUpDown.Value;
        }

        private void useSquelchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            squelchNumericUpDown.Enabled = useSquelchCheckBox.Checked;
            if (useSquelchCheckBox.Checked)
            {
                _vfo.SquelchThreshold = (int)squelchNumericUpDown.Value;
            }
            else
            {
                _vfo.SquelchThreshold = 0;
            }
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
                centerFreqNumericUpDown.Increment = stepSize;
                frequencyNumericUpDown.Increment = stepSize;
                waterfall.StepSize = stepSize;
                spectrumAnalyzer.StepSize = stepSize;

                if (snapFrequencyCheckBox.Checked && iqStreamRadioButton.Checked)
                {
                    frequencyNumericUpDown.Maximum = decimal.MaxValue;
                    frequencyNumericUpDown.Minimum = decimal.MinValue;

                    centerFreqNumericUpDown.Value = ((long) centerFreqNumericUpDown.Value + stepSize / 2) / stepSize * stepSize;
                    frequencyNumericUpDown.Value = ((long) frequencyNumericUpDown.Value + stepSize / 2) / stepSize * stepSize;

                    frequencyNumericUpDown.Maximum = centerFreqNumericUpDown.Value + _frequencyShift + (int) (_vfo.SampleRate / 2);
                    frequencyNumericUpDown.Minimum = centerFreqNumericUpDown.Value + _frequencyShift - (int)(_vfo.SampleRate / 2);

                    frequencyNumericUpDown.Maximum = ((long) frequencyNumericUpDown.Maximum) / waterfall.StepSize * waterfall.StepSize;
                    frequencyNumericUpDown.Minimum = 2 * spectrumAnalyzer.CenterFrequency - frequencyNumericUpDown.Maximum;
                }
            }
        }

        private void panview_BandwidthChanged(object sender, BandwidthEventArgs e)
        {
            if (e.Bandwidth < filterBandwidthNumericUpDown.Minimum)
            {
                e.Bandwidth = (int) filterBandwidthNumericUpDown.Minimum;
            }
            else if (e.Bandwidth > filterBandwidthNumericUpDown.Maximum)
            {
                e.Bandwidth = (int) filterBandwidthNumericUpDown.Maximum;
            }

            filterBandwidthNumericUpDown.Value = e.Bandwidth;
        }

        private void frontendGuiButton_Click(object sender, EventArgs e)
        {
            if (_frontendController != null)
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
        }

        private void agcUseHangCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _vfo.AgcHang = agcUseHangCheckBox.Checked;
        }

        private void agcDecayNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.AgcDecay = (int)agcDecayNumericUpDown.Value;
        }

        private void agcThresholdNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.AgcThreshold = (int)agcThresholdNumericUpDown.Value;
        }

        private void agcSlopeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.AgcSlope = (int)agcSlopeNumericUpDown.Value;
        }

        private void swapInQCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _streamControl.SwapIQ = swapInQCheckBox.Checked;
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
            _fftBins = int.Parse(fftResolutionComboBox.SelectedItem.ToString());
            var overlap = _fftBins <= FFTOverlapLimit;
            if (_fftOverlap != overlap)
            {
                _fftFillPosition = 0;
            }
            _fftOverlap = overlap;
            waterfall.UseSmoothing = overlap;
            spectrumAnalyzer.UseSmoothing = overlap;
            BuildFFTWindow();
        }

        private void fftWindowComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _fftWindowType = (WindowType) fftWindowComboBox.SelectedIndex;
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

        private void contrastTrackBar_Scroll(object sender, EventArgs e)
        {
            waterfall.Contrast = contrastTrackBar.Value * 100 / (contrastTrackBar.Maximum - contrastTrackBar.Minimum);
        }

        private void zoomTrackBar_Scroll(object sender, EventArgs e)
        {
            spectrumAnalyzer.Zoom = zoomTrackBar.Value * 100 / zoomTrackBar.Maximum;
            waterfall.Zoom = spectrumAnalyzer.Zoom;
        }

        private void sAttackTrackBar_Scroll(object sender, EventArgs e)
        {
            spectrumAnalyzer.Attack = sAttackTrackBar.Value / (double) sAttackTrackBar.Maximum;
        }

        private void sDecayTrackBar_Scroll(object sender, EventArgs e)
        {
            spectrumAnalyzer.Decay = sDecayTrackBar.Value / (double)sDecayTrackBar.Maximum;
        }

        private void wAttackTrackBar_Scroll(object sender, EventArgs e)
        {
            waterfall.Attack = wAttackTrackBar.Value / (double)wAttackTrackBar.Maximum;
        }

        private void wDecayTrackBar_Scroll(object sender, EventArgs e)
        {
            waterfall.Decay = wDecayTrackBar.Value / (double) wDecayTrackBar.Maximum;
        }

        private void markPeaksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            spectrumAnalyzer.MarkPeaks = markPeaksCheckBox.Checked;
        }

        #endregion

        #region Frequency Manager

        private void frequencyManagerPanel_MemoryInfoNeeded(object sender, FrequencyManager.MemoryInfoEventArgs e)
        {
            e.MemoryEntry.CenterFrequency = (long) centerFreqNumericUpDown.Value;
            e.MemoryEntry.Frequency = (long) frequencyNumericUpDown.Value;
            e.MemoryEntry.DetectorType = DetectorType;
            e.MemoryEntry.FilterBandwidth = (long) filterBandwidthNumericUpDown.Value;
            e.MemoryEntry.Shift = _frequencyShift;
        }

        private void frequencyManagerPanel_MemoryInfoAvailable(object sender, FrequencyManager.MemoryInfoEventArgs e)
        {
            if (!_streamControl.IsPlaying)
            {
                return;
            }

            filterBandwidthNumericUpDown.Value = e.MemoryEntry.FilterBandwidth;
            frequencyShiftCheckBox.Checked = e.MemoryEntry.Shift != 0;
            if (frequencyShiftCheckBox.Checked)
            {
                frequencyShiftNumericUpDown.Value = e.MemoryEntry.Shift;
            }

            centerFreqNumericUpDown.Value = e.MemoryEntry.CenterFrequency;
            if (e.MemoryEntry.Frequency >= frequencyNumericUpDown.Minimum && e.MemoryEntry.Frequency < frequencyNumericUpDown.Maximum)
            {
                frequencyNumericUpDown.Value = e.MemoryEntry.Frequency;
            }
            DetectorType = e.MemoryEntry.DetectorType;
        }

        #endregion

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D)
            {
                frequencyManagerPanel.Bookmark();
                e.Handled = true;
            }
        }
    }
}