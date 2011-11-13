using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SDRSharp.Radio;
using SDRSharp.PanView;
using SDRSharp.SoftRock;
using System.Drawing;
using SDRSharp.Radio.PortAudio;

namespace SDRSharp
{
    public partial class MainForm : Form
    {
        private const string BaseTitle = "SDR#";

        private const int DefaultFMBandwidth = 12500;
        private const int DefaultAMBandwidth = 10000;
        private const int DefaultSSBBandwidth = 2400;

        private const int MaxSpectrumBins = 2048;
        private const int MinSpectrumBins = 512;

        private int _spectrumBins;
        private WindowType _fftWindowType;
        private SoftRockIO _softRockIO;
        private readonly IQBalancer _iqBalancer = new IQBalancer();
        private readonly Vfo _vfo = new Vfo();
        private readonly AudioControl _audioControl = new AudioControl();
        private readonly FifoStream<Complex> _fftStream = new FifoStream<Complex>();
        private readonly Complex[] _spectrumIQ = new Complex[MaxSpectrumBins];
        private readonly double[] _spectrumAmplitude = new double[MaxSpectrumBins];
        private readonly double[] _fftWindow = new double[MaxSpectrumBins];

        public MainForm()
        {
            InitializeComponent();
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
            _spectrumBins = MinSpectrumBins;

            _iqBalancer.ImbalanceEstimationSucceeded += iqBalancer_ImbalanceEstimationSucceeded;
            _iqBalancer.ImbalanceEstimationFailed += iqBalancer_ImbalanceEstimationFailed;

            viewComboBox.SelectedIndex = 2;
            sampleRateComboBox.SelectedIndex = 4;

            _fftWindowType = WindowType.BlackmanHarris;
            fftWindowComboBox.SelectedIndex = (int) _fftWindowType;
            filterTypeComboBox.SelectedIndex = (int) WindowType.BlackmanHarris;

            _vfo.DetectorType = DetectorType.AM;
            _vfo.Bandwidth = DefaultAMBandwidth;
            _vfo.FilterOrder = 300;
            _vfo.UseAGC = true;
            _vfo.AgcAttack = 800;
            _vfo.AgcDecay = 50;
            _audioControl.AudioGain = 25.0;
            _audioControl.BufferNeeded += ProcessBuffer;

            waterfall.FilterBandwidth = _vfo.Bandwidth;
            waterfall.Frequency = _vfo.Frequency;
            waterfall.Offset = Vfo.MinSSBAudioFrequency;
            waterfall.BandType = BandType.Center;

            spectrumAnalyzer.FilterBandwidth = _vfo.Bandwidth;
            spectrumAnalyzer.Frequency = _vfo.Frequency;
            spectrumAnalyzer.Offset = Vfo.MinSSBAudioFrequency;
            spectrumAnalyzer.BandType = BandType.Center;

            frequencyNumericUpDown.Value = 0;

            try
            {
                _softRockIO = new SoftRockIO();
                centerFreqNumericUpDown.Value = _softRockIO.Frequency;
                centerFreqNumericUpDown_ValueChanged(null, null);
            }
            catch (DllNotFoundException)
            {
                MessageBox.Show(
                    "SRDLL.dll or SoftRock driver not found.\n\nSi570 support will be disabled.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void iqBalancer_ImbalanceEstimationSucceeded(object sender, EventArgs e)
        {
            BeginInvoke(new Action(ShowImbalance));
        }

        private void ShowImbalance()
        {
            var msg = string.Format("Gain\t= {0:F3}\r\nPhase\t= {1:F3}°", _iqBalancer.Gain, _iqBalancer.Phase * 180 / Math.PI);
            MessageBox.Show(this, msg, "IQ Optimization Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void iqBalancer_ImbalanceEstimationFailed(object sender, EventArgs e)
        {
            BeginInvoke(new Action(ShowOptimizationFailure));
        }

        private void ShowOptimizationFailure()
        {
            const string msg = "The IQ balancer was unable to improve the current image rejection.\r\nThere must be other parameters than phase and gain imbalance involved.\r\nYou may have more luck next time :-)";
            MessageBox.Show(this, msg, "IQ Optimization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            _audioControl.Stop();
            if (_softRockIO != null)
            {
                _softRockIO.Close();
                _softRockIO = null;
            }
        }

        private void ProcessBuffer(Complex[] iqBuffer, double[] audioBuffer)
        {
            _iqBalancer.Process(iqBuffer);
            if (_fftStream.Length < _spectrumBins * 2)
            {
                var len = Math.Min(_spectrumBins * 2, iqBuffer.Length);
                _fftStream.Write(iqBuffer, 0, len);
            }
            _vfo.ProcessBuffer(iqBuffer, audioBuffer);
        }

        private void correctIQButton_Click(object sender, EventArgs e)
        {
            _iqBalancer.BalanceIQ();
        }

        private void displayTimer_Tick(object sender, EventArgs e)
        {
            if (_fftStream.Length < _spectrumBins)
            {
                return;
            }

            _fftStream.Read(_spectrumIQ, 0, _spectrumBins);

            Fourier.ApplyFFTWindow(_spectrumIQ, _fftWindow, _spectrumBins);
            Fourier.ForwardTransform(_spectrumIQ, _spectrumBins);
            Fourier.SpectrumPower(_spectrumIQ, _spectrumAmplitude, _spectrumBins);

            if (!panSplitContainer.Panel1Collapsed)
            {
                spectrumAnalyzer.Render(_spectrumAmplitude, _spectrumBins);
            }
            if (!panSplitContainer.Panel2Collapsed)
            {
                waterfall.Render(_spectrumAmplitude, _spectrumBins);
            }
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
                Open();
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
                Open();
            }
        }

        private void fileSelectButton_Click(object sender, EventArgs e)
        {
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                wavFileTextBox.Text = openDlg.FileName;
                Open();
                playButton.Enabled = true;
                stopButton.Enabled = false;
            }
        }

        private void Open()
        {
            var inputDevice = (AudioDevice) inputDeviceComboBox.SelectedItem;
            var outputDevice = (AudioDevice) outputDeviceComboBox.SelectedItem;
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
                    centerFreqNumericUpDown_ValueChanged(null, null);
                }
            }

            _vfo.SampleRate = _audioControl.SampleRate;
            frequencyNumericUpDown.Maximum = (int)centerFreqNumericUpDown.Value + _vfo.SampleRate / 2;
            frequencyNumericUpDown.Minimum = (int)centerFreqNumericUpDown.Value - _vfo.SampleRate / 2;

            BuildFFTWindow();

            spectrumAnalyzer.SpectrumWidth = _vfo.SampleRate;
            waterfall.SpectrumWidth = _vfo.SampleRate;
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            Open();
            try
            {
                if (_audioControl.Play())
                {
                    playButton.Enabled = false;
                    stopButton.Enabled = true;
                    sampleRateComboBox.Enabled = false;
                    inputDeviceComboBox.Enabled = false;
                    outputDeviceComboBox.Enabled = false;
                    bufferSizeNumericUpDown.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _audioControl.Stop();
            _fftStream.Close();
            playButton.Enabled = true;
            stopButton.Enabled = false;
            sampleRateComboBox.Enabled = true;
            inputDeviceComboBox.Enabled = true;
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
            frequencyNumericUpDown.Update();
        }

        private void centerFreqNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var newCenterFreq = (int) centerFreqNumericUpDown.Value;
            spectrumAnalyzer.CenterFrequency = newCenterFreq;
            waterfall.CenterFrequency = newCenterFreq;
            spectrumAnalyzer.CenterFrequency = newCenterFreq;

            frequencyNumericUpDown.Maximum = newCenterFreq + _vfo.SampleRate / 2;
            frequencyNumericUpDown.Minimum = newCenterFreq - _vfo.SampleRate / 2;
            frequencyNumericUpDown.Value = newCenterFreq + _vfo.Frequency;

            if (_softRockIO != null)
            {
                _softRockIO.Frequency = newCenterFreq;
            }
        }

        private void panview_FrequencyChanged(object sender, FrequencyEventArgs e)
        {
            frequencyNumericUpDown.Value = e.Frequency;
        }

        private void waterfall_CenterFrequencyChanged(object sender, FrequencyEventArgs e)
        {
            centerFreqNumericUpDown.Value = e.Frequency;
        }

        private void fmRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fmRadioButton.Checked)
            {
                filterBandwidthNumericUpDown.Value = DefaultFMBandwidth;
                _vfo.DetectorType = DetectorType.FM;
                _vfo.Bandwidth = DefaultFMBandwidth;
                waterfall.BandType = BandType.Center;
                spectrumAnalyzer.BandType = BandType.Center;
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
            if (_vfo.Bandwidth > Vfo.DefaultCwSideTone)
            {
                waterfall.Offset = Vfo.MinSSBAudioFrequency;
                spectrumAnalyzer.Offset = Vfo.MinSSBAudioFrequency;
            }
            else
            {
                waterfall.Offset = Vfo.DefaultCwSideTone - _vfo.Bandwidth / 2;
                spectrumAnalyzer.Offset = waterfall.Offset;
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

        private void highDefinitionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            waterfall.HighDefinition = highDefinitionCheckBox.Checked;
            spectrumAnalyzer.HighDefinition = highDefinitionCheckBox.Checked;
            _spectrumBins = highDefinitionCheckBox.Checked ? MaxSpectrumBins : MinSpectrumBins;
            BuildFFTWindow();
        }

        private void fftWindowComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _fftWindowType = (WindowType) fftWindowComboBox.SelectedIndex;
            BuildFFTWindow();
        }

        private void autoCorrectIQCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            correctIQButton.Enabled = !autoCorrectIQCheckBox.Checked;
            _iqBalancer.AutoBalanceIQ = autoCorrectIQCheckBox.Checked;
        }

        private void BuildFFTWindow()
        {
            var window = FilterBuilder.MakeWindow(_fftWindowType, _spectrumBins);
            Array.Copy(window, _fftWindow, _spectrumBins);
        }

        private void iqTimer_Tick(object sender, EventArgs e)
        {
            Text = string.Format(BaseTitle + " - IQ Imbalance: Gain = {0:F3} Phase = {1:F3}°", _iqBalancer.Gain, _iqBalancer.Phase * 180 / Math.PI);
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
            for (var i = 0; i < colors.Length - 1; i++)
            {
                sb.AppendFormat(",{0:X2}{1:X2}{2:X2}", colors[i].R, colors[i].G, colors[i].B);
            }
            return sb.ToString().Substring(1);
        }
    }
}