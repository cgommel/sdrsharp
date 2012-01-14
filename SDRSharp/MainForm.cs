using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using SDRSharp.Radio;
using SDRSharp.PanView;
using SDRSharp.Radio.PortAudio;
using System.Collections;

namespace SDRSharp
{
    public partial class MainForm : Form
    {
        private static readonly string _baseTitle = "SDR# v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        private const int DefaultFMBandwidth = 12500;
        private const int DefaultAMBandwidth = 10000;
        private const int DefaultSSBBandwidth = 2400;

        private const int MaxFFTBins = 1024 * 64;

        private int _fftBins;
        private WindowType _fftWindowType;
        private IFrontendController _frontendController;
        private readonly Dictionary<string, IFrontendController> _frontendControllers = new Dictionary<string, IFrontendController>();
        private readonly IQBalancer _iqBalancer = new IQBalancer();
        private readonly Vfo _vfo = new Vfo();
        private readonly AudioControl _audioControl = new AudioControl();
        private readonly FifoStream<Complex> _fftStream = new FifoStream<Complex>();
        private readonly Complex[] _fftBuffer = new Complex[MaxFFTBins];
        private readonly Complex[] _iqBuffer = new Complex[MaxFFTBins];
        private readonly double[] _spectrumPower = new double[MaxFFTBins];
        private readonly double[] _fftWindow = new double[MaxFFTBins];
        private readonly Timer _fftTimer;


        public MainForm()
        {
            InitializeComponent();
            _fftTimer = new Timer(components);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var devices = AudioDevice.GetDevices(DeviceDirection.Input);

            foreach (var device in devices)
            {
                inputDeviceComboBox.Items.Add(device);
            }
            if (inputDeviceComboBox.Items.Count > 0)
            {
                inputDeviceComboBox.SelectedIndex = 0;
            }

            devices = AudioDevice.GetDevices(DeviceDirection.Output);
            foreach (var device in devices)
            {
                outputDeviceComboBox.Items.Add(device);
            }
            if (outputDeviceComboBox.Items.Count > 0)
            {
                outputDeviceComboBox.SelectedIndex = 0;
            }
            _fftBins = 1024;

            viewComboBox.SelectedIndex = 2;
            fftResolutionComboBox.SelectedIndex = 1;
            sampleRateComboBox.SelectedIndex = 4;

            _fftWindowType = WindowType.BlackmanHarris;
            fftWindowComboBox.SelectedIndex = (int) _fftWindowType;
            filterTypeComboBox.SelectedIndex = (int) WindowType.BlackmanHarris;

            _vfo.DetectorType = DetectorType.AM;
            _vfo.Bandwidth = DefaultAMBandwidth;
            _vfo.FilterOrder = 100;
            _vfo.FmSquelch = 50;
            _vfo.UseAGC = true;
            _vfo.AgcAttack = 800;
            _vfo.AgcDecay = 50;
            _audioControl.AudioGain = 25.0;
            _audioControl.BufferNeeded += ProcessBuffer;

            waterfall.FilterBandwidth = _vfo.Bandwidth;
            waterfall.Frequency = _vfo.Frequency;
            waterfall.FilterOffset = Vfo.MinSSBAudioFrequency;
            waterfall.BandType = BandType.Center;

            spectrumAnalyzer.FilterBandwidth = _vfo.Bandwidth;
            spectrumAnalyzer.Frequency = _vfo.Frequency;
            spectrumAnalyzer.FilterOffset = Vfo.MinSSBAudioFrequency;
            spectrumAnalyzer.BandType = BandType.Center;

            frequencyNumericUpDown.Value = 0;

            stepSizeComboBox.SelectedIndex = 2;

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

            frontEndComboBox.Items.Add("Other");
            frontEndComboBox.SelectedIndex = frontEndComboBox.Items.Count - 1;

            _fftTimer.Tick += fftTimer_Tick;
            _fftTimer.Interval = GetIntSetting("displayTimerInterval", 50);
            _fftTimer.Enabled = true;
        }

        private static int GetIntSetting(string name, int defaultValue)
        {
            var strValue = ConfigurationManager.AppSettings[name];
            int result;
            if (int.TryParse(strValue, out result))
            {
                return result;
            }
            return defaultValue;
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
                centerFreqNumericUpDown.Value = 0;
                centerFreqNumericUpDown_ValueChanged(null, null);
                frequencyNumericUpDown.Value = 0;
                frequencyNumericUpDown_ValueChanged(null, null);
                return;
            }
            try
            {
                if (_frontendController != null)
                {
                    _frontendController.Close();
                }
                _frontendController = _frontendControllers[frontendName];
                _frontendController.Open();
                centerFreqNumericUpDown.Value = _frontendController.Frequency;
                centerFreqNumericUpDown_ValueChanged(null, null);
                frequencyNumericUpDown.Value = _frontendController.Frequency;
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
        }

        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            _audioControl.Stop();
            if (_frontendController != null)
            {
                _frontendController.Close();
                _frontendController = null;
            }
        }

        private void ProcessBuffer(Complex[] iqBuffer, double[] audioBuffer)
        {
            _iqBalancer.Process(iqBuffer);
            if (_fftStream.Length < iqBuffer.Length)
            {
                _fftStream.Write(iqBuffer, 0, iqBuffer.Length);
            }
            _vfo.ProcessBuffer(iqBuffer, audioBuffer);
        }

        private void fftTimer_Tick(object sender, EventArgs e)
        {
            if (!playButton.Enabled)
            {
                var fftRate = _fftBins / (_fftTimer.Interval * 0.001);
                var overlapRatio = _audioControl.SampleRate / fftRate;
                var excessBuffer = Math.Max(0, _fftStream.Length - _audioControl.BufferSize);
                if (overlapRatio > 1.0)
                {
                    _fftStream.Advance(excessBuffer);
                    _fftStream.Read(_iqBuffer, 0, _fftBins);
                }
                else
                {
                    var bytes = (int)(_fftBins * overlapRatio);
                    var toRead = Math.Min(excessBuffer + bytes, _fftBins);
                    toRead = Math.Min(toRead, _fftStream.Length);
                    if (toRead > 0)
                    {
                        Array.Copy(_iqBuffer, toRead, _iqBuffer, 0, _fftBins - toRead);
                        _fftStream.Read(_iqBuffer, _fftBins - toRead, toRead);
                    }
                }

                // http://www.designnews.com/author.asp?section_id=1419&doc_id=236273&piddl_msgid=522392
                var fftGain = 10.0 * Math.Log10(_fftBins / 2);
                var compensation = 24.0 - fftGain;

                Array.Copy(_iqBuffer, _fftBuffer, _fftBins);
                Fourier.ApplyFFTWindow(_fftBuffer, _fftWindow, _fftBins);
                Fourier.ForwardTransform(_fftBuffer, _fftBins);
                Fourier.SpectrumPower(_fftBuffer, _spectrumPower, _fftBins, compensation);

                if (!panSplitContainer.Panel1Collapsed)
                {
                    spectrumAnalyzer.Render(_spectrumPower, _fftBins);
                }
                if (!panSplitContainer.Panel2Collapsed)
                {
                    waterfall.Render(_spectrumPower, _fftBins);
                }
            }

            spectrumAnalyzer.Perform();
            waterfall.Perform();
        }

        private void soundCardRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (soundCardRadioButton.Checked)
            {
                _audioControl.Stop();
                wavFileTextBox.Enabled = false;
                fileSelectButton.Enabled = false;
                playButton.Enabled = true;
                stopButton.Enabled = false;
                sampleRateComboBox.Enabled = true;
                inputDeviceComboBox.Enabled = true;
                outputDeviceComboBox.Enabled = true;
                centerFreqNumericUpDown.Enabled = true;
                frontEndComboBox.Enabled = true;

                frontEndComboBox_SelectedIndexChanged(null, null);
            }
        }

        private void waveFileRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (wavFileRadioButton.Checked)
            {
                _audioControl.Stop();
                wavFileTextBox.Enabled = true;
                fileSelectButton.Enabled = true;
                playButton.Enabled = true;
                stopButton.Enabled = false;
                sampleRateComboBox.Enabled = false;
                inputDeviceComboBox.Enabled = false;
                outputDeviceComboBox.Enabled = true;
                bufferSizeNumericUpDown.Enabled = true;
                centerFreqNumericUpDown.Enabled = false;
                frontEndComboBox.Enabled = false;

                centerFreqNumericUpDown.Value = 0;
                centerFreqNumericUpDown_ValueChanged(null, null);
                frequencyNumericUpDown.Value = 0;
                frequencyNumericUpDown_ValueChanged(null, null);
            }
        }

        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                wavFileTextBox.Text = openDlg.FileName;
                playButton.Enabled = true;
                stopButton.Enabled = false;
                _audioControl.Stop();
            }
        }

        private void Open()
        {
            var inputDevice = (AudioDevice) inputDeviceComboBox.SelectedItem;
            var outputDevice = (AudioDevice) outputDeviceComboBox.SelectedItem;
            var oldCenterFrequency = centerFreqNumericUpDown.Value;
            Match match;
            if (soundCardRadioButton.Checked)
            {
                var sampleRate = 0;
                match = Regex.Match(sampleRateComboBox.Text, "([0-9\\.]+)k", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    sampleRate = (int)(double.Parse(match.Groups[1].Value) * 1000);
                }
                _audioControl.OpenDevice(inputDevice.Index, outputDevice.Index, sampleRate, (int) bufferSizeNumericUpDown.Value);
            }
            else
            {
                if (!File.Exists(wavFileTextBox.Text))
                {
                    return;
                }
                _audioControl.OpenFile(wavFileTextBox.Text, outputDevice.Index, (int) bufferSizeNumericUpDown.Value);

                var friendlyFilename = "" + Path.GetFileName(wavFileTextBox.Text);
                match = Regex.Match(friendlyFilename, "([0-9]+)KHz", RegexOptions.IgnoreCase);
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

            _vfo.SampleRate = _audioControl.SampleRate;
            spectrumAnalyzer.SpectrumWidth = _audioControl.SampleRate;
            waterfall.SpectrumWidth = _audioControl.SampleRate;

            frequencyNumericUpDown.Maximum = (int) centerFreqNumericUpDown.Value + _audioControl.SampleRate / 2;
            frequencyNumericUpDown.Minimum = (int) centerFreqNumericUpDown.Value - _audioControl.SampleRate / 2;

            if (centerFreqNumericUpDown.Value != oldCenterFrequency)
            {
                frequencyNumericUpDown.Value = centerFreqNumericUpDown.Value;

                zoomTrackBar.Value = 0;
                zoomTrackBar_Scroll(null, null);
            }
            
            frequencyNumericUpDown_ValueChanged(null, null);

            BuildFFTWindow();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            Open();
            try
            {
                _audioControl.Play();
                playButton.Enabled = false;
                stopButton.Enabled = true;
                sampleRateComboBox.Enabled = false;
                inputDeviceComboBox.Enabled = false;
                outputDeviceComboBox.Enabled = false;
                bufferSizeNumericUpDown.Enabled = false;
                frontEndComboBox.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _audioControl.Stop();
            _fftStream.Flush();
            playButton.Enabled = true;
            stopButton.Enabled = false;
            if (soundCardRadioButton.Checked)
            {
                sampleRateComboBox.Enabled = true;
                inputDeviceComboBox.Enabled = true;
                frontEndComboBox.Enabled = true;
            }
            outputDeviceComboBox.Enabled = true;
            bufferSizeNumericUpDown.Enabled = true;
        }

        private void audioGainNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _audioControl.AudioGain = (int) audioGainNumericUpDown.Value;
        }

        private void frequencyNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            waterfall.Frequency = (int) frequencyNumericUpDown.Value;
            spectrumAnalyzer.Frequency = (int) frequencyNumericUpDown.Value;
            _vfo.Frequency = waterfall.Frequency - (int) centerFreqNumericUpDown.Value;
        }

        private void centerFreqNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var newCenterFreq = (int) centerFreqNumericUpDown.Value;
            waterfall.CenterFrequency = newCenterFreq;
            spectrumAnalyzer.CenterFrequency = newCenterFreq;

            frequencyNumericUpDown.Maximum = newCenterFreq + _vfo.SampleRate / 2;
            frequencyNumericUpDown.Minimum = newCenterFreq - _vfo.SampleRate / 2;
            frequencyNumericUpDown.Value = newCenterFreq + _vfo.Frequency;

            if (_frontendController != null && soundCardRadioButton.Checked)
            {
                _frontendController.Frequency = newCenterFreq;
            }
        }

        private void panview_FrequencyChanged(object sender, FrequencyEventArgs e)
        {
            if (e.Frequency >= frequencyNumericUpDown.Minimum &&
                e.Frequency <= frequencyNumericUpDown.Maximum)
            {
                frequencyNumericUpDown.Value = e.Frequency;
            }
        }

        private void panview_CenterFrequencyChanged(object sender, FrequencyEventArgs e)
        {
            if (soundCardRadioButton.Checked)
            {
                centerFreqNumericUpDown.Value = e.Frequency;
            }
        }

        private void fmRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            fmSquelchNumericUpDown.Enabled = fmRadioButton.Checked;
            if (fmRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultFMBandwidth;
                _vfo.DetectorType = DetectorType.FM;
                _vfo.Bandwidth = DefaultFMBandwidth;
                waterfall.BandType = BandType.Center;
                spectrumAnalyzer.BandType = BandType.Center;
                waterfall.FilterOffset = 0;
                spectrumAnalyzer.FilterOffset = 0;
            }
        }

        private void amRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (amRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultAMBandwidth;
                _vfo.DetectorType = DetectorType.AM;
                _vfo.Bandwidth = DefaultAMBandwidth;
                waterfall.BandType = BandType.Center;
                spectrumAnalyzer.BandType = BandType.Center;
                waterfall.FilterOffset = 0;
                spectrumAnalyzer.FilterOffset = 0;
            }
        }

        private void lsbRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (lsbRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultSSBBandwidth;
                _vfo.DetectorType = DetectorType.LSB;
                _vfo.Bandwidth = DefaultSSBBandwidth;
                waterfall.BandType = BandType.Lower;
                spectrumAnalyzer.BandType = BandType.Lower;
                waterfall.FilterOffset = Vfo.MinSSBAudioFrequency;
                spectrumAnalyzer.FilterOffset = Vfo.MinSSBAudioFrequency;
            }
        }

        private void usbRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (usbRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultSSBBandwidth;
                _vfo.DetectorType = DetectorType.USB;
                _vfo.Bandwidth = DefaultSSBBandwidth;
                waterfall.BandType = BandType.Upper;
                spectrumAnalyzer.BandType = BandType.Upper;
                waterfall.FilterOffset = Vfo.MinSSBAudioFrequency;
                spectrumAnalyzer.FilterOffset = Vfo.MinSSBAudioFrequency;
            }
        }

        private void agcCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _vfo.UseAGC = agcCheckBox.Checked;
            agcAttackNumericUpDown.Enabled = agcCheckBox.Checked;
            agcDecayNumericUpDown.Enabled = agcCheckBox.Checked;
        }

        private void agcDecayNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.AgcDecay = (int) agcDecayNumericUpDown.Value;
        }

        private void agcAttackNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.AgcAttack = (int) agcAttackNumericUpDown.Value;
        }

        private void swapInQCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _audioControl.SwapIQ = swapInQCheckBox.Checked;
        }

        private void filterBandwidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.Bandwidth = (int)filterBandwidthNumericUpDown.Value;
            waterfall.FilterBandwidth = _vfo.Bandwidth;
            spectrumAnalyzer.FilterBandwidth = _vfo.Bandwidth;
            if (_vfo.DetectorType == DetectorType.LSB || _vfo.DetectorType == DetectorType.USB)
            {
                if (_vfo.Bandwidth > Vfo.DefaultCwSideTone)
                {
                    waterfall.FilterOffset = Vfo.MinSSBAudioFrequency;
                    spectrumAnalyzer.FilterOffset = Vfo.MinSSBAudioFrequency;
                }
                else
                {
                    waterfall.FilterOffset = Vfo.DefaultCwSideTone - _vfo.Bandwidth/2;
                    spectrumAnalyzer.FilterOffset = waterfall.FilterOffset;
                }
            }
            else
            {
                waterfall.FilterOffset = 0;
                spectrumAnalyzer.FilterOffset = 0;
            }
        }

        private void filterOrderNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.FilterOrder = (int) filterOrderNumericUpDown.Value;
        }

        private void filterTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _vfo.WindowType = (WindowType) (filterTypeComboBox.SelectedIndex - 1);
        }

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
            BuildFFTWindow();
        }

        private void fftWindowComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _fftWindowType = (WindowType) fftWindowComboBox.SelectedIndex;
            BuildFFTWindow();
        }

        private void autoCorrectIQCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _iqBalancer.AutoBalanceIQ = correctIQCheckBox.Checked;
        }

        private void BuildFFTWindow()
        {
            var window = FilterBuilder.MakeWindow(_fftWindowType, _fftBins);
            Array.Copy(window, _fftWindow, _fftBins);
        }

        private void iqTimer_Tick(object sender, EventArgs e)
        {
            Text = string.Format(_baseTitle + " - IQ Imbalance: Gain = {0:F3} Phase = {1:F3}°", _iqBalancer.Gain, _iqBalancer.Phase * 180 / Math.PI);
        }

        private void gradientButton_Click(object sender, EventArgs e)
        {
            var gradient = GradientDialog.GetGradient(waterfall.GradientColorBlend);
            if (gradient != null && gradient.Positions.Length > 0)
            {
                waterfall.GradientColorBlend = gradient;
                SaveSetting("gradient", GradientToString(gradient.Colors));
            }
        }

        private static void SaveSetting(string key, string value)
        {
            var configurationFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configurationFile.AppSettings.Settings.Remove(key);
            configurationFile.AppSettings.Settings.Add(key, value);
            configurationFile.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
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

        private void fmSquelchNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _vfo.FmSquelch = (int) fmSquelchNumericUpDown.Value;
        }

        private void stepSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var match = Regex.Match(stepSizeComboBox.Text, "([0-9\\.]+) kHz", RegexOptions.None);
            if (match.Success)
            {
                centerFreqNumericUpDown.Increment = (int) (double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) * 1000);
            }
            else
            {
                match = Regex.Match(stepSizeComboBox.Text, "([0-9]+) Hz", RegexOptions.None);
                if (match.Success)
                {
                    centerFreqNumericUpDown.Increment = int.Parse(match.Groups[1].Value);
                }
            }
        }

        private void panview_BandwidthChanged(object sender, BandwidthEventArgs e)
        {
            if (e.Bandwidth >= filterBandwidthNumericUpDown.Minimum && e.Bandwidth <= filterBandwidthNumericUpDown.Maximum)
            {
                filterBandwidthNumericUpDown.Value = e.Bandwidth;
            }
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
    }
}