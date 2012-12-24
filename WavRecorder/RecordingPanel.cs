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
        
        private readonly SimpleRecorder _audioRecorder;
        private readonly SimpleRecorder _basebandRecorder;

        private WavSampleFormat _wavSampleFormat;

        private DateTime _startTime;

        public RecordingPanel(ISharpControl control)
        {
            InitializeComponent();

            _control = control;
            
            _audioProcessor.Bypass = true;
            _iqObserver.Enabled = false;

            _control.RegisterStreamHook(_iqObserver);
            _control.RegisterStreamHook(_audioProcessor);

            _audioRecorder = new SimpleRecorder(_audioProcessor);
            _basebandRecorder = new SimpleRecorder(_iqObserver);

            _control.PropertyChanged += PropertyChangedHandler;

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
                    if (_audioRecorder.IsRecording)
                    {
                        _audioRecorder.StopRecording();
                    }
                    if (_basebandRecorder.IsRecording)
                    {
                        _basebandRecorder.StopRecording();
                    }
                    ConfigureGUI();
                    break;
            }
        }

        #endregion

        #region Control Handlers

        private void recBtn_Click(object sender, EventArgs e)
        {
            if (!_basebandRecorder.IsRecording && !_audioRecorder.IsRecording)
            {

                PrepareRecorder();

                try
                {                    
                    if (audioCb.Checked)
                    {
                        _audioRecorder.StartRecording();
                    }
                    if (basebandCb.Checked)
                    {
                        _basebandRecorder.StartRecording();
                    }
                }
                catch
                {
                    _audioRecorder.StopRecording();
                    _basebandRecorder.StopRecording();

                    MessageBox.Show("Unable to start recording", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    return;
                }

                _startTime = DateTime.Now;
            }
            else
            {                
                if (_audioRecorder.IsRecording)
                {
                    _audioRecorder.StopRecording();
                }
                if (_basebandRecorder.IsRecording)
                {
                    _basebandRecorder.StopRecording();
                }
            }

            ConfigureGUI();
        }

        private void recDisplayTimer_Tick(object sender, EventArgs e)
        {
            const float bytesToMb = 1.0f / 1048576.0f;

            var ts = DateTime.Now - _startTime;
            var sizeInMB = (_audioRecorder.BytesWritten * bytesToMb) + (_basebandRecorder.BytesWritten * bytesToMb);

            durationLbl.Text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            sizeLbl.Text = string.Format("{0:f2} MB", sizeInMB);
            skippedBufferCountLbl.Text = string.Format("{0}", (_basebandRecorder.SkippedBuffers + _audioRecorder.SkippedBuffers));

            var needConfigure = false;
            if (_audioRecorder.IsStreamFull)
            {
                _audioRecorder.StopRecording();
                needConfigure = true;
            }
            if (_basebandRecorder.IsStreamFull)
            {
                _basebandRecorder.StopRecording();
                needConfigure = true;
            }
            if (needConfigure)
            {
                ConfigureGUI();
            }
        }

        private void sampleFormatCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            _wavSampleFormat = (WavSampleFormat)sampleFormatCombo.SelectedIndex;
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

                recBtn.Text = ((_audioRecorder.IsRecording || _basebandRecorder.IsRecording) ? "Stop" : "Record");

                recDisplayTimer.Enabled = (_audioRecorder.IsRecording || _basebandRecorder.IsRecording);
            }
            else
            {
                recDisplayTimer.Enabled = false;
                recBtn.Enabled = false;
                recBtn.Text = "Record";
                durationLbl.Text = "00:00:00";
                sizeLbl.Text = "0 MB";
                skippedBufferCountLbl.Text = "0";
            }

            sampleFormatCombo.Enabled = !(_audioRecorder.IsRecording||_basebandRecorder.IsRecording);
            audioCb.Enabled = !(_audioRecorder.IsRecording || _basebandRecorder.IsRecording);
            basebandCb.Enabled = !(_audioRecorder.IsRecording||_basebandRecorder.IsRecording);
        }

        #endregion
        
        private string MakeFileName(RecordingMode mode, DateTime time)
        {
            var tunedfrequency = mode == RecordingMode.Baseband ? Math.Abs(_control.CenterFrequency + (_control.FrequencyShiftEnabled ? _control.FrequencyShift : 0)) : Math.Max(_control.Frequency, 0);                       
            
            var frequency = tunedfrequency >= 1000 ? (tunedfrequency / 1000L) : tunedfrequency;

            var unit = tunedfrequency >= 1000 ? "kHz" : "Hz";
            var suffix = mode == RecordingMode.Baseband? "IQ" : "AF";
            var dateString = time.ToString("yyyyMMdd");
            var timeString = time.ToString("HHmmssZ");

            var filename = Path.GetDirectoryName(Application.ExecutablePath);
            filename = Path.Combine("" + filename, string.Format("SDRSharp_{0}_{1}_{2}{3}_{4}.wav", dateString, timeString, frequency, unit,suffix));

            return filename;
        }

        private void PrepareRecorder()
        {
            var startTime = DateTime.UtcNow;

            if (audioCb.Checked)
            {
                _audioRecorder.SampleRate = _audioProcessor.SampleRate;
                _audioRecorder.FileName = MakeFileName(RecordingMode.Audio,startTime);
                _audioRecorder.Format = _wavSampleFormat;
            }

            if (basebandCb.Checked)
            {
                _basebandRecorder.SampleRate = _iqObserver.SampleRate;
                _basebandRecorder.FileName = MakeFileName(RecordingMode.Baseband,startTime);
                _basebandRecorder.Format = _wavSampleFormat;
            }

        }

        public void AbortRecording()
        {            
            if (_audioRecorder != null)
            {
                _audioRecorder.StopRecording();
            }
            if (_basebandRecorder != null)
            {
                _basebandRecorder.StopRecording();
            }
        }
    }
}
