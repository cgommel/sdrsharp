using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using SDRSharp.Common;

namespace SDRSharp.WavRecorder
{
    public partial class RecordingPanel : UserControl
    {

        private readonly ISharpControl _control;
        private readonly RecordingIQObserver _iqObserver = new RecordingIQObserver();
        private readonly RecordingAudioProcessor _audioProcessor = new RecordingAudioProcessor();
        private readonly SimpleRecorder _simpleRecorder;
        private WavSampleFormat _wavSampleFormat;

        private DateTime _startTime;

        public RecordingPanel(ISharpControl control)
        {
            InitializeComponent();

            _control = control;
            _control.PropertyChanged += PropertyChangedHandler;

            _audioProcessor.Bypass = true;
            _iqObserver.Enabled = false;

            _control.RegisterStreamHook(_iqObserver);
            _control.RegisterStreamHook(_audioProcessor);

            _simpleRecorder = new SimpleRecorder(_iqObserver, _audioProcessor);
            _simpleRecorder.Mode = RecordingMode.Baseband;

            InitializeGUI();
            ConfigureGUI();
        }

        #region INotifyPropertyChanged Handler

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {

                case "StartRadio":
                    ConfigureGUI();
                    break;
                case "StopRadio":
                    if (_simpleRecorder.IsRecording)
                    {
                        _simpleRecorder.StopRecording();
                    }
                    ConfigureGUI();
                    break;
            }
        }

        #endregion

        #region Control Handlers

        private void recBtn_Click(object sender, EventArgs e)
        {
            if (!_simpleRecorder.IsRecording)
            {
                _simpleRecorder.FileName = MakeFileName();
                _simpleRecorder.Format = _wavSampleFormat;

                if (audioRadioBtn.Checked)
                {
                    _simpleRecorder.SampleRate = _audioProcessor.SampleRate;
                }
                else
                {
                    _simpleRecorder.SampleRate = _iqObserver.SampleRate;
                }

                try
                {
                    _simpleRecorder.StartRecording();
                }
                catch
                {
                    MessageBox.Show("Unable to start recording", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _startTime = DateTime.Now;

            }
            else
            {
                _simpleRecorder.StopRecording();
            }

            ConfigureGUI();
        }

        private void recDisplayTimer_Tick(object sender, EventArgs e)
        {
            const float bytesToMb = 1.0f / 1048576.0f;

            var ts = DateTime.Now - _startTime;
            var sizeInMB = (_simpleRecorder.BytesWritten * bytesToMb);

            durationLbl.Text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            sizeLbl.Text = string.Format("{0:f2} MB", sizeInMB);

            if (_simpleRecorder.IsStreamFull)
            {
                _simpleRecorder.StopRecording();
                ConfigureGUI();
            }
        }

        private void sampleFormatCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            _wavSampleFormat = (WavSampleFormat)sampleFormatCombo.SelectedIndex;
        }

        private void SampleSource_CheckedChanged(object sender, EventArgs e)
        {
            if (audioRadioBtn.Checked)
            {
                _simpleRecorder.Mode = RecordingMode.Audio;
            }
            else
            {
                _simpleRecorder.Mode = RecordingMode.Baseband;
            }
        }

        #endregion

        #region GUI Configuration

        private void InitializeGUI()
        {
            sampleFormatCombo.SelectedIndex = 1;

            sampleFormatCombo_SelectedIndexChanged(null, null);
        }

        private void ConfigureGUI()
        {
            if (_control.IsPlaying)
            {
                recBtn.Enabled = true;

                recBtn.Text = (_simpleRecorder.IsRecording ? "Stop" : "Record");

                recDisplayTimer.Enabled = _simpleRecorder.IsRecording;
            }
            else
            {
                recDisplayTimer.Enabled = false;
                recBtn.Enabled = false;
                recBtn.Text = "Record";
                durationLbl.Text = "00:00:00";
                sizeLbl.Text = "0 MB";
            }

            sampleFormatCombo.Enabled = !_simpleRecorder.IsRecording;
            audioRadioBtn.Enabled = !_simpleRecorder.IsRecording;
            basebandRadioBtn.Enabled = !_simpleRecorder.IsRecording;
        }

        #endregion

        private string MakeFileName()
        {
            // Should not happen, but it is long afterall.
            var absFrequency = Math.Abs(_control.CenterFrequency);

            var frequency = absFrequency >= 1000 ? (absFrequency / 1000L) : absFrequency;
            var unit = absFrequency >= 1000 ? "kHz" : "Hz";
            var dateString = DateTime.Now.ToString("yyyyMMdd");
            var timeString = DateTime.Now.ToString("HHmmssZ");

            var filename = Path.GetDirectoryName(Application.ExecutablePath);
            filename = Path.Combine("" + filename, string.Format("SDRSharp_{0}_{1}_{2}{3}.wav", dateString, timeString, frequency, unit));

            return filename;
        }

        public void AbortRecording()
        {
            if (_simpleRecorder != null)
            {
                _simpleRecorder.StopRecording();
            }
        }
    }
}
