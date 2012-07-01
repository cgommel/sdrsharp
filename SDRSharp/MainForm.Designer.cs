using System.Windows.Forms;
using SDRSharp.CollapsiblePanel;
using SDRSharp.PanView;

namespace SDRSharp
{
    public partial class MainForm
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openDlg = new System.Windows.Forms.OpenFileDialog();
            this.playButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.wavFileTextBox = new System.Windows.Forms.TextBox();
            this.iqStreamRadioButton = new System.Windows.Forms.RadioButton();
            this.waveFileRadioButton = new System.Windows.Forms.RadioButton();
            this.fileSelectButton = new System.Windows.Forms.Button();
            this.panSplitContainer = new System.Windows.Forms.SplitContainer();
            this.spectrumAnalyzer = new SDRSharp.PanView.SpectrumAnalyzer();
            this.waterfall = new SDRSharp.PanView.Waterfall();
            this.iqTimer = new System.Windows.Forms.Timer(this.components);
            this.contrastTrackBar = new System.Windows.Forms.TrackBar();
            this.zoomTrackBar = new System.Windows.Forms.TrackBar();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.radioCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            this.frequencyShiftCheckBox = new System.Windows.Forms.CheckBox();
            this.frequencyShiftNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.cwuRadioButton = new System.Windows.Forms.RadioButton();
            this.fmStereoCheckBox = new System.Windows.Forms.CheckBox();
            this.useSquelchCheckBox = new System.Windows.Forms.CheckBox();
            this.cwlRadioButton = new System.Windows.Forms.RadioButton();
            this.nfmRadioButton = new System.Windows.Forms.RadioButton();
            this.dsbRadioButton = new System.Windows.Forms.RadioButton();
            this.frequencyNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.snapFrequencyCheckBox = new System.Windows.Forms.CheckBox();
            this.wfmRadioButton = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cwShiftNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.frontendGuiButton = new System.Windows.Forms.Button();
            this.amRadioButton = new System.Windows.Forms.RadioButton();
            this.lsbRadioButton = new System.Windows.Forms.RadioButton();
            this.usbRadioButton = new System.Windows.Forms.RadioButton();
            this.frontEndComboBox = new System.Windows.Forms.ComboBox();
            this.centerFreqNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.stepSizeComboBox = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.filterBandwidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.filterOrderNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.squelchNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.filterTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.swapInQCheckBox = new System.Windows.Forms.CheckBox();
            this.correctIQCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.audioCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            this.audioGainTrackBar = new System.Windows.Forms.TrackBar();
            this.latencyNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.filterAudioCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.sampleRateComboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.outputDeviceComboBox = new System.Windows.Forms.ComboBox();
            this.inputDeviceComboBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.agcCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            this.agcUseHangCheckBox = new System.Windows.Forms.CheckBox();
            this.agcSlopeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.agcCheckBox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.agcThresholdNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.agcDecayNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.displayCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.wDecayTrackBar = new System.Windows.Forms.TrackBar();
            this.wAttackTrackBar = new System.Windows.Forms.TrackBar();
            this.label21 = new System.Windows.Forms.Label();
            this.fftResolutionComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.sDecayTrackBar = new System.Windows.Forms.TrackBar();
            this.label14 = new System.Windows.Forms.Label();
            this.viewComboBox = new System.Windows.Forms.ComboBox();
            this.sAttackTrackBar = new System.Windows.Forms.TrackBar();
            this.gradientButton = new System.Windows.Forms.Button();
            this.fftWindowComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.markPeaksCheckBox = new System.Windows.Forms.CheckBox();
            this.panSplitContainer.Panel1.SuspendLayout();
            this.panSplitContainer.Panel2.SuspendLayout();
            this.panSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).BeginInit();
            this.controlPanel.SuspendLayout();
            this.radioCollapsiblePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyShiftNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cwShiftNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerFreqNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterBandwidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterOrderNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.squelchNumericUpDown)).BeginInit();
            this.audioCollapsiblePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.audioGainTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.latencyNumericUpDown)).BeginInit();
            this.agcCollapsiblePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.agcSlopeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcThresholdNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcDecayNumericUpDown)).BeginInit();
            this.displayCollapsiblePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.wDecayTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wAttackTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sDecayTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sAttackTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // openDlg
            // 
            this.openDlg.DefaultExt = "wav";
            this.openDlg.Filter = "WAV files|*.wav";
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(12, 12);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(50, 24);
            this.playButton.TabIndex = 0;
            this.playButton.Text = "Play";
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(68, 12);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(50, 24);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // wavFileTextBox
            // 
            this.wavFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wavFileTextBox.Enabled = false;
            this.wavFileTextBox.Location = new System.Drawing.Point(278, 15);
            this.wavFileTextBox.Name = "wavFileTextBox";
            this.wavFileTextBox.Size = new System.Drawing.Size(639, 20);
            this.wavFileTextBox.TabIndex = 4;
            // 
            // iqStreamRadioButton
            // 
            this.iqStreamRadioButton.AutoSize = true;
            this.iqStreamRadioButton.Checked = true;
            this.iqStreamRadioButton.Location = new System.Drawing.Point(124, 16);
            this.iqStreamRadioButton.Name = "iqStreamRadioButton";
            this.iqStreamRadioButton.Size = new System.Drawing.Size(72, 17);
            this.iqStreamRadioButton.TabIndex = 2;
            this.iqStreamRadioButton.TabStop = true;
            this.iqStreamRadioButton.Text = "IQ Stream";
            this.iqStreamRadioButton.UseVisualStyleBackColor = true;
            this.iqStreamRadioButton.CheckedChanged += new System.EventHandler(this.iqStreamRadioButton_CheckedChanged);
            // 
            // waveFileRadioButton
            // 
            this.waveFileRadioButton.AutoSize = true;
            this.waveFileRadioButton.Location = new System.Drawing.Point(202, 16);
            this.waveFileRadioButton.Name = "waveFileRadioButton";
            this.waveFileRadioButton.Size = new System.Drawing.Size(70, 17);
            this.waveFileRadioButton.TabIndex = 3;
            this.waveFileRadioButton.Text = "Wave file";
            this.waveFileRadioButton.UseVisualStyleBackColor = true;
            this.waveFileRadioButton.CheckedChanged += new System.EventHandler(this.waveFileRadioButton_CheckedChanged);
            // 
            // fileSelectButton
            // 
            this.fileSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelectButton.Enabled = false;
            this.fileSelectButton.Location = new System.Drawing.Point(923, 13);
            this.fileSelectButton.Name = "fileSelectButton";
            this.fileSelectButton.Size = new System.Drawing.Size(26, 23);
            this.fileSelectButton.TabIndex = 5;
            this.fileSelectButton.Text = "...";
            this.fileSelectButton.UseVisualStyleBackColor = true;
            this.fileSelectButton.Click += new System.EventHandler(this.fileSelectButton_Click);
            // 
            // panSplitContainer
            // 
            this.panSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panSplitContainer.Location = new System.Drawing.Point(247, 42);
            this.panSplitContainer.Name = "panSplitContainer";
            this.panSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // panSplitContainer.Panel1
            // 
            this.panSplitContainer.Panel1.Controls.Add(this.spectrumAnalyzer);
            this.panSplitContainer.Panel1MinSize = 10;
            // 
            // panSplitContainer.Panel2
            // 
            this.panSplitContainer.Panel2.Controls.Add(this.waterfall);
            this.panSplitContainer.Panel2MinSize = 10;
            this.panSplitContainer.Size = new System.Drawing.Size(651, 619);
            this.panSplitContainer.SplitterDistance = 199;
            this.panSplitContainer.TabIndex = 13;
            // 
            // spectrumAnalyzer
            // 
            this.spectrumAnalyzer.Attack = 0.9D;
            this.spectrumAnalyzer.BandType = SDRSharp.PanView.BandType.Center;
            this.spectrumAnalyzer.CenterFrequency = ((long)(0));
            this.spectrumAnalyzer.Decay = 0.3D;
            this.spectrumAnalyzer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spectrumAnalyzer.FilterBandwidth = 10000;
            this.spectrumAnalyzer.FilterOffset = 100;
            this.spectrumAnalyzer.Frequency = ((long)(0));
            this.spectrumAnalyzer.Location = new System.Drawing.Point(0, 0);
            this.spectrumAnalyzer.MarkPeaks = false;
            this.spectrumAnalyzer.Name = "spectrumAnalyzer";
            this.spectrumAnalyzer.Size = new System.Drawing.Size(651, 199);
            this.spectrumAnalyzer.SpectrumWidth = 48000;
            this.spectrumAnalyzer.StepSize = 1000;
            this.spectrumAnalyzer.TabIndex = 0;
            this.spectrumAnalyzer.UseSmoothing = false;
            this.spectrumAnalyzer.UseSnap = false;
            this.spectrumAnalyzer.Zoom = 0;
            this.spectrumAnalyzer.FrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_FrequencyChanged);
            this.spectrumAnalyzer.CenterFrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_CenterFrequencyChanged);
            this.spectrumAnalyzer.BandwidthChanged += new SDRSharp.PanView.ManualBandwidthChange(this.panview_BandwidthChanged);
            // 
            // waterfall
            // 
            this.waterfall.Attack = 0.9D;
            this.waterfall.BandType = SDRSharp.PanView.BandType.Center;
            this.waterfall.CenterFrequency = ((long)(0));
            this.waterfall.Contrast = 0;
            this.waterfall.Decay = 0.5D;
            this.waterfall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waterfall.FilterBandwidth = 10000;
            this.waterfall.FilterOffset = 0;
            this.waterfall.Frequency = ((long)(0));
            this.waterfall.Location = new System.Drawing.Point(0, 0);
            this.waterfall.Name = "waterfall";
            this.waterfall.Size = new System.Drawing.Size(651, 416);
            this.waterfall.SpectrumWidth = 48000;
            this.waterfall.StepSize = 0;
            this.waterfall.TabIndex = 0;
            this.waterfall.UseSmoothing = false;
            this.waterfall.UseSnap = false;
            this.waterfall.Zoom = 0;
            this.waterfall.FrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_FrequencyChanged);
            this.waterfall.CenterFrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_CenterFrequencyChanged);
            this.waterfall.BandwidthChanged += new SDRSharp.PanView.ManualBandwidthChange(this.panview_BandwidthChanged);
            // 
            // iqTimer
            // 
            this.iqTimer.Enabled = true;
            this.iqTimer.Interval = 500;
            this.iqTimer.Tick += new System.EventHandler(this.iqTimer_Tick);
            // 
            // contrastTrackBar
            // 
            this.contrastTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.contrastTrackBar.Location = new System.Drawing.Point(905, 262);
            this.contrastTrackBar.Maximum = 24;
            this.contrastTrackBar.Minimum = -24;
            this.contrastTrackBar.Name = "contrastTrackBar";
            this.contrastTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.contrastTrackBar.RightToLeftLayout = true;
            this.contrastTrackBar.Size = new System.Drawing.Size(45, 184);
            this.contrastTrackBar.TabIndex = 17;
            this.contrastTrackBar.TickFrequency = 6;
            this.contrastTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.contrastTrackBar.Scroll += new System.EventHandler(this.contrastTrackBar_Scroll);
            // 
            // zoomTrackBar
            // 
            this.zoomTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomTrackBar.Location = new System.Drawing.Point(905, 58);
            this.zoomTrackBar.Maximum = 50;
            this.zoomTrackBar.Name = "zoomTrackBar";
            this.zoomTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.zoomTrackBar.RightToLeftLayout = true;
            this.zoomTrackBar.Size = new System.Drawing.Size(45, 184);
            this.zoomTrackBar.TabIndex = 18;
            this.zoomTrackBar.TickFrequency = 5;
            this.zoomTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.zoomTrackBar.Scroll += new System.EventHandler(this.zoomTrackBar_Scroll);
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(910, 42);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(34, 13);
            this.label19.TabIndex = 19;
            this.label19.Text = "Zoom";
            // 
            // label20
            // 
            this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(904, 246);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(46, 13);
            this.label20.TabIndex = 20;
            this.label20.Text = "Contrast";
            // 
            // controlPanel
            // 
            this.controlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.controlPanel.AutoScroll = true;
            this.controlPanel.Controls.Add(this.radioCollapsiblePanel);
            this.controlPanel.Controls.Add(this.displayCollapsiblePanel);
            this.controlPanel.Controls.Add(this.audioCollapsiblePanel);
            this.controlPanel.Controls.Add(this.agcCollapsiblePanel);
            this.controlPanel.Location = new System.Drawing.Point(12, 42);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(229, 619);
            this.controlPanel.TabIndex = 25;
            // 
            // radioCollapsiblePanel
            // 
            this.radioCollapsiblePanel.Controls.Add(this.markPeaksCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.frequencyShiftCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.frequencyShiftNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.cwuRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.useSquelchCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.fmStereoCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.cwlRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.nfmRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.dsbRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.frequencyNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.snapFrequencyCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.wfmRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.label15);
            this.radioCollapsiblePanel.Controls.Add(this.label2);
            this.radioCollapsiblePanel.Controls.Add(this.cwShiftNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.frontendGuiButton);
            this.radioCollapsiblePanel.Controls.Add(this.amRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.lsbRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.usbRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.frontEndComboBox);
            this.radioCollapsiblePanel.Controls.Add(this.centerFreqNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.label18);
            this.radioCollapsiblePanel.Controls.Add(this.stepSizeComboBox);
            this.radioCollapsiblePanel.Controls.Add(this.label16);
            this.radioCollapsiblePanel.Controls.Add(this.filterBandwidthNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.filterOrderNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.label1);
            this.radioCollapsiblePanel.Controls.Add(this.squelchNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.filterTypeComboBox);
            this.radioCollapsiblePanel.Controls.Add(this.label9);
            this.radioCollapsiblePanel.Controls.Add(this.swapInQCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.correctIQCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.label5);
            this.radioCollapsiblePanel.ExpandedHeight = 362;
            this.radioCollapsiblePanel.Location = new System.Drawing.Point(0, 0);
            this.radioCollapsiblePanel.Name = "radioCollapsiblePanel";
            this.radioCollapsiblePanel.NextPanel = this.audioCollapsiblePanel;
            this.radioCollapsiblePanel.PanelTitle = "Radio";
            this.radioCollapsiblePanel.Size = new System.Drawing.Size(229, 382);
            this.radioCollapsiblePanel.TabIndex = 21;
            // 
            // frequencyShiftCheckBox
            // 
            this.frequencyShiftCheckBox.AutoSize = true;
            this.frequencyShiftCheckBox.Location = new System.Drawing.Point(9, 138);
            this.frequencyShiftCheckBox.Name = "frequencyShiftCheckBox";
            this.frequencyShiftCheckBox.Size = new System.Drawing.Size(47, 17);
            this.frequencyShiftCheckBox.TabIndex = 10;
            this.frequencyShiftCheckBox.Text = "Shift";
            this.frequencyShiftCheckBox.UseVisualStyleBackColor = true;
            this.frequencyShiftCheckBox.CheckedChanged += new System.EventHandler(this.frequencyShiftCheckBox_CheckedChanged);
            // 
            // frequencyShiftNumericUpDown
            // 
            this.frequencyShiftNumericUpDown.Enabled = false;
            this.frequencyShiftNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frequencyShiftNumericUpDown.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.frequencyShiftNumericUpDown.Location = new System.Drawing.Point(69, 133);
            this.frequencyShiftNumericUpDown.Maximum = new decimal(new int[] {
            276447232,
            23283,
            0,
            0});
            this.frequencyShiftNumericUpDown.Minimum = new decimal(new int[] {
            276447232,
            23283,
            0,
            -2147483648});
            this.frequencyShiftNumericUpDown.Name = "frequencyShiftNumericUpDown";
            this.frequencyShiftNumericUpDown.Size = new System.Drawing.Size(135, 24);
            this.frequencyShiftNumericUpDown.TabIndex = 11;
            this.frequencyShiftNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.frequencyShiftNumericUpDown.ThousandsSeparator = true;
            this.frequencyShiftNumericUpDown.ValueChanged += new System.EventHandler(this.frequencyShiftNumericUpDown_ValueChanged);
            // 
            // cwuRadioButton
            // 
            this.cwuRadioButton.AutoSize = true;
            this.cwuRadioButton.Location = new System.Drawing.Point(154, 49);
            this.cwuRadioButton.Name = "cwuRadioButton";
            this.cwuRadioButton.Size = new System.Drawing.Size(54, 17);
            this.cwuRadioButton.TabIndex = 7;
            this.cwuRadioButton.Text = "CW-U";
            this.cwuRadioButton.UseVisualStyleBackColor = true;
            this.cwuRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckedChanged);
            // 
            // fmStereoCheckBox
            // 
            this.fmStereoCheckBox.Checked = true;
            this.fmStereoCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fmStereoCheckBox.Enabled = false;
            this.fmStereoCheckBox.Location = new System.Drawing.Point(5, 361);
            this.fmStereoCheckBox.Name = "fmStereoCheckBox";
            this.fmStereoCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.fmStereoCheckBox.Size = new System.Drawing.Size(94, 17);
            this.fmStereoCheckBox.TabIndex = 24;
            this.fmStereoCheckBox.Text = "FM Stereo";
            this.fmStereoCheckBox.UseVisualStyleBackColor = true;
            this.fmStereoCheckBox.CheckedChanged += new System.EventHandler(this.fmStereoCheckBox_CheckedChanged);
            // 
            // useSquelchCheckBox
            // 
            this.useSquelchCheckBox.Location = new System.Drawing.Point(9, 253);
            this.useSquelchCheckBox.Name = "useSquelchCheckBox";
            this.useSquelchCheckBox.Size = new System.Drawing.Size(69, 17);
            this.useSquelchCheckBox.TabIndex = 17;
            this.useSquelchCheckBox.Text = "Squelch";
            this.useSquelchCheckBox.UseVisualStyleBackColor = true;
            this.useSquelchCheckBox.CheckedChanged += new System.EventHandler(this.useSquelchCheckBox_CheckedChanged);
            // 
            // cwlRadioButton
            // 
            this.cwlRadioButton.AutoSize = true;
            this.cwlRadioButton.Location = new System.Drawing.Point(106, 49);
            this.cwlRadioButton.Name = "cwlRadioButton";
            this.cwlRadioButton.Size = new System.Drawing.Size(52, 17);
            this.cwlRadioButton.TabIndex = 6;
            this.cwlRadioButton.Text = "CW-L";
            this.cwlRadioButton.UseVisualStyleBackColor = true;
            this.cwlRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckedChanged);
            // 
            // nfmRadioButton
            // 
            this.nfmRadioButton.AutoSize = true;
            this.nfmRadioButton.Location = new System.Drawing.Point(7, 26);
            this.nfmRadioButton.Name = "nfmRadioButton";
            this.nfmRadioButton.Size = new System.Drawing.Size(48, 17);
            this.nfmRadioButton.TabIndex = 0;
            this.nfmRadioButton.Text = "NFM";
            this.nfmRadioButton.UseVisualStyleBackColor = true;
            this.nfmRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckedChanged);
            // 
            // dsbRadioButton
            // 
            this.dsbRadioButton.AutoSize = true;
            this.dsbRadioButton.Location = new System.Drawing.Point(60, 49);
            this.dsbRadioButton.Name = "dsbRadioButton";
            this.dsbRadioButton.Size = new System.Drawing.Size(47, 17);
            this.dsbRadioButton.TabIndex = 5;
            this.dsbRadioButton.Text = "DSB";
            this.dsbRadioButton.UseVisualStyleBackColor = true;
            this.dsbRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckedChanged);
            // 
            // frequencyNumericUpDown
            // 
            this.frequencyNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frequencyNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.frequencyNumericUpDown.Location = new System.Drawing.Point(70, 73);
            this.frequencyNumericUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.frequencyNumericUpDown.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.frequencyNumericUpDown.Name = "frequencyNumericUpDown";
            this.frequencyNumericUpDown.Size = new System.Drawing.Size(135, 24);
            this.frequencyNumericUpDown.TabIndex = 8;
            this.frequencyNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.frequencyNumericUpDown.ThousandsSeparator = true;
            this.frequencyNumericUpDown.ValueChanged += new System.EventHandler(this.frequencyNumericUpDown_ValueChanged);
            // 
            // snapFrequencyCheckBox
            // 
            this.snapFrequencyCheckBox.Location = new System.Drawing.Point(5, 313);
            this.snapFrequencyCheckBox.Name = "snapFrequencyCheckBox";
            this.snapFrequencyCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.snapFrequencyCheckBox.Size = new System.Drawing.Size(94, 17);
            this.snapFrequencyCheckBox.TabIndex = 20;
            this.snapFrequencyCheckBox.Text = "Snap to grid";
            this.snapFrequencyCheckBox.UseVisualStyleBackColor = true;
            this.snapFrequencyCheckBox.CheckedChanged += new System.EventHandler(this.stepSizeComboBox_SelectedIndexChanged);
            // 
            // wfmRadioButton
            // 
            this.wfmRadioButton.AutoSize = true;
            this.wfmRadioButton.Location = new System.Drawing.Point(7, 49);
            this.wfmRadioButton.Name = "wfmRadioButton";
            this.wfmRadioButton.Size = new System.Drawing.Size(51, 17);
            this.wfmRadioButton.TabIndex = 4;
            this.wfmRadioButton.Text = "WFM";
            this.wfmRadioButton.UseVisualStyleBackColor = true;
            this.wfmRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(111, 254);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(49, 13);
            this.label15.TabIndex = 32;
            this.label15.Text = "CW Shift";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Frequency";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cwShiftNumericUpDown
            // 
            this.cwShiftNumericUpDown.Enabled = false;
            this.cwShiftNumericUpDown.Location = new System.Drawing.Point(110, 271);
            this.cwShiftNumericUpDown.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.cwShiftNumericUpDown.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.cwShiftNumericUpDown.Name = "cwShiftNumericUpDown";
            this.cwShiftNumericUpDown.Size = new System.Drawing.Size(94, 20);
            this.cwShiftNumericUpDown.TabIndex = 19;
            this.cwShiftNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.cwShiftNumericUpDown.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.cwShiftNumericUpDown.ValueChanged += new System.EventHandler(this.cwShiftNumericUpDown_ValueChanged);
            // 
            // frontendGuiButton
            // 
            this.frontendGuiButton.Enabled = false;
            this.frontendGuiButton.FlatAppearance.BorderSize = 0;
            this.frontendGuiButton.Location = new System.Drawing.Point(5, 163);
            this.frontendGuiButton.Name = "frontendGuiButton";
            this.frontendGuiButton.Size = new System.Drawing.Size(61, 21);
            this.frontendGuiButton.TabIndex = 12;
            this.frontendGuiButton.Text = "Front end";
            this.frontendGuiButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.frontendGuiButton.UseVisualStyleBackColor = true;
            this.frontendGuiButton.Click += new System.EventHandler(this.frontendGuiButton_Click);
            // 
            // amRadioButton
            // 
            this.amRadioButton.AutoSize = true;
            this.amRadioButton.Checked = true;
            this.amRadioButton.Location = new System.Drawing.Point(60, 26);
            this.amRadioButton.Name = "amRadioButton";
            this.amRadioButton.Size = new System.Drawing.Size(41, 17);
            this.amRadioButton.TabIndex = 1;
            this.amRadioButton.TabStop = true;
            this.amRadioButton.Text = "AM";
            this.amRadioButton.UseVisualStyleBackColor = true;
            this.amRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckedChanged);
            // 
            // lsbRadioButton
            // 
            this.lsbRadioButton.AutoSize = true;
            this.lsbRadioButton.Location = new System.Drawing.Point(106, 26);
            this.lsbRadioButton.Name = "lsbRadioButton";
            this.lsbRadioButton.Size = new System.Drawing.Size(45, 17);
            this.lsbRadioButton.TabIndex = 2;
            this.lsbRadioButton.Text = "LSB";
            this.lsbRadioButton.UseVisualStyleBackColor = true;
            this.lsbRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckedChanged);
            // 
            // usbRadioButton
            // 
            this.usbRadioButton.AutoSize = true;
            this.usbRadioButton.Location = new System.Drawing.Point(154, 26);
            this.usbRadioButton.Name = "usbRadioButton";
            this.usbRadioButton.Size = new System.Drawing.Size(47, 17);
            this.usbRadioButton.TabIndex = 3;
            this.usbRadioButton.Text = "USB";
            this.usbRadioButton.UseVisualStyleBackColor = true;
            this.usbRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckedChanged);
            // 
            // frontEndComboBox
            // 
            this.frontEndComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frontEndComboBox.DropDownWidth = 135;
            this.frontEndComboBox.FormattingEnabled = true;
            this.frontEndComboBox.Location = new System.Drawing.Point(69, 163);
            this.frontEndComboBox.Name = "frontEndComboBox";
            this.frontEndComboBox.Size = new System.Drawing.Size(135, 21);
            this.frontEndComboBox.TabIndex = 13;
            this.frontEndComboBox.SelectedIndexChanged += new System.EventHandler(this.frontEndComboBox_SelectedIndexChanged);
            // 
            // centerFreqNumericUpDown
            // 
            this.centerFreqNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.centerFreqNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.centerFreqNumericUpDown.Location = new System.Drawing.Point(70, 103);
            this.centerFreqNumericUpDown.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.centerFreqNumericUpDown.Name = "centerFreqNumericUpDown";
            this.centerFreqNumericUpDown.Size = new System.Drawing.Size(135, 24);
            this.centerFreqNumericUpDown.TabIndex = 9;
            this.centerFreqNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.centerFreqNumericUpDown.ThousandsSeparator = true;
            this.centerFreqNumericUpDown.ValueChanged += new System.EventHandler(this.centerFreqNumericUpDown_ValueChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(111, 294);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 13);
            this.label18.TabIndex = 30;
            this.label18.Text = "Step size";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stepSizeComboBox
            // 
            this.stepSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stepSizeComboBox.FormattingEnabled = true;
            this.stepSizeComboBox.Items.AddRange(new object[] {
            "1 Hz",
            "10 Hz",
            "100 Hz",
            "500 Hz",
            "1 kHz",
            "2.5 kHz",
            "5 kHz",
            "6.25 kHz",
            "8.33 kHz",
            "9 kHz",
            "10 kHz",
            "12.5 kHz",
            "25 kHz",
            "50 kHz",
            "100 kHz",
            "150 kHz",
            "200 kHz",
            "250 kHz",
            "300 kHz",
            "350 kHz",
            "400 kHz",
            "450 kHz",
            "500 kHz"});
            this.stepSizeComboBox.Location = new System.Drawing.Point(110, 311);
            this.stepSizeComboBox.Name = "stepSizeComboBox";
            this.stepSizeComboBox.Size = new System.Drawing.Size(94, 21);
            this.stepSizeComboBox.TabIndex = 21;
            this.stepSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.stepSizeComboBox_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 193);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(52, 13);
            this.label16.TabIndex = 26;
            this.label16.Text = "Filter type";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // filterBandwidthNumericUpDown
            // 
            this.filterBandwidthNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.Location = new System.Drawing.Point(5, 231);
            this.filterBandwidthNumericUpDown.Maximum = new decimal(new int[] {
            250000,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.Name = "filterBandwidthNumericUpDown";
            this.filterBandwidthNumericUpDown.Size = new System.Drawing.Size(94, 20);
            this.filterBandwidthNumericUpDown.TabIndex = 15;
            this.filterBandwidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.filterBandwidthNumericUpDown.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.ValueChanged += new System.EventHandler(this.filterBandwidthNumericUpDown_ValueChanged);
            // 
            // filterOrderNumericUpDown
            // 
            this.filterOrderNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.Location = new System.Drawing.Point(110, 231);
            this.filterOrderNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.Name = "filterOrderNumericUpDown";
            this.filterOrderNumericUpDown.Size = new System.Drawing.Size(94, 20);
            this.filterOrderNumericUpDown.TabIndex = 16;
            this.filterOrderNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.filterOrderNumericUpDown.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.ValueChanged += new System.EventHandler(this.filterOrderNumericUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Filter bandwidth";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // squelchNumericUpDown
            // 
            this.squelchNumericUpDown.Enabled = false;
            this.squelchNumericUpDown.Location = new System.Drawing.Point(5, 271);
            this.squelchNumericUpDown.Name = "squelchNumericUpDown";
            this.squelchNumericUpDown.Size = new System.Drawing.Size(94, 20);
            this.squelchNumericUpDown.TabIndex = 18;
            this.squelchNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.squelchNumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.squelchNumericUpDown.ValueChanged += new System.EventHandler(this.squelchNumericUpDown_ValueChanged);
            // 
            // filterTypeComboBox
            // 
            this.filterTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterTypeComboBox.FormattingEnabled = true;
            this.filterTypeComboBox.Items.AddRange(new object[] {
            "Hamming",
            "Blackman",
            "Blackman-Harris",
            "Hann-Poisson",
            "Youssef"});
            this.filterTypeComboBox.Location = new System.Drawing.Point(69, 190);
            this.filterTypeComboBox.Name = "filterTypeComboBox";
            this.filterTypeComboBox.Size = new System.Drawing.Size(135, 21);
            this.filterTypeComboBox.TabIndex = 14;
            this.filterTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.filterTypeComboBox_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 109);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Center";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // swapInQCheckBox
            // 
            this.swapInQCheckBox.Location = new System.Drawing.Point(110, 338);
            this.swapInQCheckBox.Name = "swapInQCheckBox";
            this.swapInQCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.swapInQCheckBox.Size = new System.Drawing.Size(94, 17);
            this.swapInQCheckBox.TabIndex = 23;
            this.swapInQCheckBox.Text = "Swap I && Q";
            this.swapInQCheckBox.UseVisualStyleBackColor = true;
            this.swapInQCheckBox.CheckedChanged += new System.EventHandler(this.swapInQCheckBox_CheckedChanged);
            // 
            // correctIQCheckBox
            // 
            this.correctIQCheckBox.Checked = true;
            this.correctIQCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.correctIQCheckBox.Location = new System.Drawing.Point(5, 338);
            this.correctIQCheckBox.Name = "correctIQCheckBox";
            this.correctIQCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.correctIQCheckBox.Size = new System.Drawing.Size(94, 17);
            this.correctIQCheckBox.TabIndex = 22;
            this.correctIQCheckBox.Text = "Correct IQ";
            this.correctIQCheckBox.UseVisualStyleBackColor = true;
            this.correctIQCheckBox.CheckedChanged += new System.EventHandler(this.autoCorrectIQCheckBox_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(111, 214);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Filter order";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // audioCollapsiblePanel
            // 
            this.audioCollapsiblePanel.Controls.Add(this.audioGainTrackBar);
            this.audioCollapsiblePanel.Controls.Add(this.latencyNumericUpDown);
            this.audioCollapsiblePanel.Controls.Add(this.filterAudioCheckBox);
            this.audioCollapsiblePanel.Controls.Add(this.label6);
            this.audioCollapsiblePanel.Controls.Add(this.label13);
            this.audioCollapsiblePanel.Controls.Add(this.sampleRateComboBox);
            this.audioCollapsiblePanel.Controls.Add(this.label12);
            this.audioCollapsiblePanel.Controls.Add(this.label3);
            this.audioCollapsiblePanel.Controls.Add(this.outputDeviceComboBox);
            this.audioCollapsiblePanel.Controls.Add(this.inputDeviceComboBox);
            this.audioCollapsiblePanel.Controls.Add(this.label11);
            this.audioCollapsiblePanel.ExpandedHeight = 176;
            this.audioCollapsiblePanel.Location = new System.Drawing.Point(0, 382);
            this.audioCollapsiblePanel.Name = "audioCollapsiblePanel";
            this.audioCollapsiblePanel.NextPanel = this.agcCollapsiblePanel;
            this.audioCollapsiblePanel.PanelTitle = "Audio";
            this.audioCollapsiblePanel.Size = new System.Drawing.Size(229, 196);
            this.audioCollapsiblePanel.TabIndex = 22;
            // 
            // audioGainTrackBar
            // 
            this.audioGainTrackBar.Location = new System.Drawing.Point(57, 24);
            this.audioGainTrackBar.Maximum = 40;
            this.audioGainTrackBar.Minimum = 15;
            this.audioGainTrackBar.Name = "audioGainTrackBar";
            this.audioGainTrackBar.Size = new System.Drawing.Size(156, 45);
            this.audioGainTrackBar.TabIndex = 0;
            this.audioGainTrackBar.TickFrequency = 5;
            this.audioGainTrackBar.Value = 30;
            this.audioGainTrackBar.ValueChanged += new System.EventHandler(this.audioGainTrackBar_ValueChanged);
            // 
            // latencyNumericUpDown
            // 
            this.latencyNumericUpDown.Location = new System.Drawing.Point(126, 151);
            this.latencyNumericUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.latencyNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.latencyNumericUpDown.Name = "latencyNumericUpDown";
            this.latencyNumericUpDown.Size = new System.Drawing.Size(79, 20);
            this.latencyNumericUpDown.TabIndex = 4;
            this.latencyNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.latencyNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // filterAudioCheckBox
            // 
            this.filterAudioCheckBox.Checked = true;
            this.filterAudioCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterAudioCheckBox.Location = new System.Drawing.Point(118, 177);
            this.filterAudioCheckBox.Name = "filterAudioCheckBox";
            this.filterAudioCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.filterAudioCheckBox.Size = new System.Drawing.Size(87, 17);
            this.filterAudioCheckBox.TabIndex = 34;
            this.filterAudioCheckBox.Text = "Filter Audio";
            this.filterAudioCheckBox.UseVisualStyleBackColor = true;
            this.filterAudioCheckBox.CheckedChanged += new System.EventHandler(this.filterAudioCheckBox_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Latency (ms)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 73);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 13);
            this.label13.TabIndex = 28;
            this.label13.Text = "Samplerate";
            // 
            // sampleRateComboBox
            // 
            this.sampleRateComboBox.FormattingEnabled = true;
            this.sampleRateComboBox.Items.AddRange(new object[] {
            "8000 sample/sec",
            "11025 sample/sec",
            "16000 sample/sec",
            "22050 sample/sec",
            "24000 sample/sec",
            "32000 sample/sec",
            "44100 sample/sec",
            "48000 sample/sec",
            "80000 sample/sec",
            "96000 sample/sec",
            "120000 sample/sec",
            "125000 sample/sec",
            "150000 sample/sec",
            "192000 sample/sec"});
            this.sampleRateComboBox.Location = new System.Drawing.Point(70, 70);
            this.sampleRateComboBox.Name = "sampleRateComboBox";
            this.sampleRateComboBox.Size = new System.Drawing.Size(135, 21);
            this.sampleRateComboBox.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 127);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(39, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "Output";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "AF Gain";
            // 
            // outputDeviceComboBox
            // 
            this.outputDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outputDeviceComboBox.DropDownWidth = 300;
            this.outputDeviceComboBox.FormattingEnabled = true;
            this.outputDeviceComboBox.Location = new System.Drawing.Point(70, 124);
            this.outputDeviceComboBox.Name = "outputDeviceComboBox";
            this.outputDeviceComboBox.Size = new System.Drawing.Size(135, 21);
            this.outputDeviceComboBox.TabIndex = 3;
            // 
            // inputDeviceComboBox
            // 
            this.inputDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputDeviceComboBox.DropDownWidth = 300;
            this.inputDeviceComboBox.FormattingEnabled = true;
            this.inputDeviceComboBox.Location = new System.Drawing.Point(70, 97);
            this.inputDeviceComboBox.Name = "inputDeviceComboBox";
            this.inputDeviceComboBox.Size = new System.Drawing.Size(135, 21);
            this.inputDeviceComboBox.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 100);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(31, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "Input";
            // 
            // agcCollapsiblePanel
            // 
            this.agcCollapsiblePanel.Controls.Add(this.agcUseHangCheckBox);
            this.agcCollapsiblePanel.Controls.Add(this.agcSlopeNumericUpDown);
            this.agcCollapsiblePanel.Controls.Add(this.label22);
            this.agcCollapsiblePanel.Controls.Add(this.agcCheckBox);
            this.agcCollapsiblePanel.Controls.Add(this.label10);
            this.agcCollapsiblePanel.Controls.Add(this.agcThresholdNumericUpDown);
            this.agcCollapsiblePanel.Controls.Add(this.agcDecayNumericUpDown);
            this.agcCollapsiblePanel.Controls.Add(this.label4);
            this.agcCollapsiblePanel.ExpandedHeight = 103;
            this.agcCollapsiblePanel.Location = new System.Drawing.Point(0, 578);
            this.agcCollapsiblePanel.Name = "agcCollapsiblePanel";
            this.agcCollapsiblePanel.NextPanel = this.displayCollapsiblePanel;
            this.agcCollapsiblePanel.PanelState = SDRSharp.CollapsiblePanel.PanelStateOptions.Collapsed;
            this.agcCollapsiblePanel.PanelTitle = "AGC";
            this.agcCollapsiblePanel.Size = new System.Drawing.Size(229, 20);
            this.agcCollapsiblePanel.TabIndex = 23;
            // 
            // agcUseHangCheckBox
            // 
            this.agcUseHangCheckBox.AutoSize = true;
            this.agcUseHangCheckBox.Checked = true;
            this.agcUseHangCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.agcUseHangCheckBox.Location = new System.Drawing.Point(125, 27);
            this.agcUseHangCheckBox.Name = "agcUseHangCheckBox";
            this.agcUseHangCheckBox.Size = new System.Drawing.Size(74, 17);
            this.agcUseHangCheckBox.TabIndex = 1;
            this.agcUseHangCheckBox.Text = "Use Hang";
            this.agcUseHangCheckBox.UseVisualStyleBackColor = true;
            this.agcUseHangCheckBox.CheckedChanged += new System.EventHandler(this.agcUseHangCheckBox_CheckedChanged);
            // 
            // agcSlopeNumericUpDown
            // 
            this.agcSlopeNumericUpDown.Location = new System.Drawing.Point(125, 97);
            this.agcSlopeNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.agcSlopeNumericUpDown.Name = "agcSlopeNumericUpDown";
            this.agcSlopeNumericUpDown.Size = new System.Drawing.Size(79, 20);
            this.agcSlopeNumericUpDown.TabIndex = 4;
            this.agcSlopeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.agcSlopeNumericUpDown.ValueChanged += new System.EventHandler(this.agcSlopeNumericUpDown_ValueChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 99);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(56, 13);
            this.label22.TabIndex = 13;
            this.label22.Text = "Slope (dB)";
            // 
            // agcCheckBox
            // 
            this.agcCheckBox.AutoSize = true;
            this.agcCheckBox.Checked = true;
            this.agcCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.agcCheckBox.Location = new System.Drawing.Point(9, 27);
            this.agcCheckBox.Name = "agcCheckBox";
            this.agcCheckBox.Size = new System.Drawing.Size(70, 17);
            this.agcCheckBox.TabIndex = 0;
            this.agcCheckBox.Text = "Use AGC";
            this.agcCheckBox.UseVisualStyleBackColor = true;
            this.agcCheckBox.CheckedChanged += new System.EventHandler(this.agcCheckBox_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 74);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Decay (ms)";
            // 
            // agcThresholdNumericUpDown
            // 
            this.agcThresholdNumericUpDown.Location = new System.Drawing.Point(125, 47);
            this.agcThresholdNumericUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.agcThresholdNumericUpDown.Minimum = new decimal(new int[] {
            160,
            0,
            0,
            -2147483648});
            this.agcThresholdNumericUpDown.Name = "agcThresholdNumericUpDown";
            this.agcThresholdNumericUpDown.Size = new System.Drawing.Size(79, 20);
            this.agcThresholdNumericUpDown.TabIndex = 2;
            this.agcThresholdNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.agcThresholdNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.agcThresholdNumericUpDown.ValueChanged += new System.EventHandler(this.agcThresholdNumericUpDown_ValueChanged);
            // 
            // agcDecayNumericUpDown
            // 
            this.agcDecayNumericUpDown.Location = new System.Drawing.Point(125, 72);
            this.agcDecayNumericUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.agcDecayNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.agcDecayNumericUpDown.Name = "agcDecayNumericUpDown";
            this.agcDecayNumericUpDown.Size = new System.Drawing.Size(79, 20);
            this.agcDecayNumericUpDown.TabIndex = 3;
            this.agcDecayNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.agcDecayNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.agcDecayNumericUpDown.ValueChanged += new System.EventHandler(this.agcDecayNumericUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Threshold (dB)";
            // 
            // displayCollapsiblePanel
            // 
            this.displayCollapsiblePanel.Controls.Add(this.label25);
            this.displayCollapsiblePanel.Controls.Add(this.label26);
            this.displayCollapsiblePanel.Controls.Add(this.label24);
            this.displayCollapsiblePanel.Controls.Add(this.label23);
            this.displayCollapsiblePanel.Controls.Add(this.wDecayTrackBar);
            this.displayCollapsiblePanel.Controls.Add(this.wAttackTrackBar);
            this.displayCollapsiblePanel.Controls.Add(this.label21);
            this.displayCollapsiblePanel.Controls.Add(this.fftResolutionComboBox);
            this.displayCollapsiblePanel.Controls.Add(this.label7);
            this.displayCollapsiblePanel.Controls.Add(this.sDecayTrackBar);
            this.displayCollapsiblePanel.Controls.Add(this.label14);
            this.displayCollapsiblePanel.Controls.Add(this.viewComboBox);
            this.displayCollapsiblePanel.Controls.Add(this.sAttackTrackBar);
            this.displayCollapsiblePanel.Controls.Add(this.gradientButton);
            this.displayCollapsiblePanel.Controls.Add(this.fftWindowComboBox);
            this.displayCollapsiblePanel.Controls.Add(this.label8);
            this.displayCollapsiblePanel.ExpandedHeight = 259;
            this.displayCollapsiblePanel.Location = new System.Drawing.Point(0, 598);
            this.displayCollapsiblePanel.Name = "displayCollapsiblePanel";
            this.displayCollapsiblePanel.NextPanel = null;
            this.displayCollapsiblePanel.PanelState = SDRSharp.CollapsiblePanel.PanelStateOptions.Collapsed;
            this.displayCollapsiblePanel.PanelTitle = "FFT Display";
            this.displayCollapsiblePanel.Size = new System.Drawing.Size(229, 20);
            this.displayCollapsiblePanel.TabIndex = 24;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 238);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(52, 13);
            this.label25.TabIndex = 26;
            this.label25.Text = "W-Decay";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(7, 203);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(52, 13);
            this.label26.TabIndex = 25;
            this.label26.Text = "W-Attack";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(6, 167);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(48, 13);
            this.label24.TabIndex = 24;
            this.label24.Text = "S-Decay";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(7, 132);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(48, 13);
            this.label23.TabIndex = 23;
            this.label23.Text = "S-Attack";
            // 
            // wDecayTrackBar
            // 
            this.wDecayTrackBar.Location = new System.Drawing.Point(57, 236);
            this.wDecayTrackBar.Maximum = 50;
            this.wDecayTrackBar.Name = "wDecayTrackBar";
            this.wDecayTrackBar.Size = new System.Drawing.Size(156, 45);
            this.wDecayTrackBar.TabIndex = 22;
            this.wDecayTrackBar.TickFrequency = 5;
            this.wDecayTrackBar.Scroll += new System.EventHandler(this.wDecayTrackBar_Scroll);
            // 
            // wAttackTrackBar
            // 
            this.wAttackTrackBar.Location = new System.Drawing.Point(57, 200);
            this.wAttackTrackBar.Maximum = 50;
            this.wAttackTrackBar.Name = "wAttackTrackBar";
            this.wAttackTrackBar.Size = new System.Drawing.Size(156, 45);
            this.wAttackTrackBar.TabIndex = 21;
            this.wAttackTrackBar.TickFrequency = 5;
            this.wAttackTrackBar.Scroll += new System.EventHandler(this.wAttackTrackBar_Scroll);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(7, 79);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(57, 13);
            this.label21.TabIndex = 18;
            this.label21.Text = "Resolution";
            // 
            // fftResolutionComboBox
            // 
            this.fftResolutionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fftResolutionComboBox.FormattingEnabled = true;
            this.fftResolutionComboBox.Items.AddRange(new object[] {
            "512",
            "1024",
            "2048",
            "4096",
            "8192",
            "16384",
            "32768",
            "65536",
            "131072",
            "262144",
            "524288",
            "1048576",
            "2097152",
            "4194304"});
            this.fftResolutionComboBox.Location = new System.Drawing.Point(70, 76);
            this.fftResolutionComboBox.Name = "fftResolutionComboBox";
            this.fftResolutionComboBox.Size = new System.Drawing.Size(135, 21);
            this.fftResolutionComboBox.TabIndex = 2;
            this.fftResolutionComboBox.SelectedIndexChanged += new System.EventHandler(this.fftResolutionComboBox_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "View";
            // 
            // sDecayTrackBar
            // 
            this.sDecayTrackBar.Location = new System.Drawing.Point(57, 164);
            this.sDecayTrackBar.Maximum = 50;
            this.sDecayTrackBar.Name = "sDecayTrackBar";
            this.sDecayTrackBar.Size = new System.Drawing.Size(156, 45);
            this.sDecayTrackBar.TabIndex = 20;
            this.sDecayTrackBar.TickFrequency = 5;
            this.sDecayTrackBar.Scroll += new System.EventHandler(this.sDecayTrackBar_Scroll);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(127, 106);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "Gradient";
            // 
            // viewComboBox
            // 
            this.viewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.viewComboBox.FormattingEnabled = true;
            this.viewComboBox.Items.AddRange(new object[] {
            "Spectrum Analyzer",
            "Waterfall",
            "Both"});
            this.viewComboBox.Location = new System.Drawing.Point(70, 26);
            this.viewComboBox.Name = "viewComboBox";
            this.viewComboBox.Size = new System.Drawing.Size(135, 21);
            this.viewComboBox.TabIndex = 0;
            this.viewComboBox.SelectedIndexChanged += new System.EventHandler(this.viewComboBox_SelectedIndexChanged);
            // 
            // sAttackTrackBar
            // 
            this.sAttackTrackBar.Location = new System.Drawing.Point(57, 128);
            this.sAttackTrackBar.Maximum = 50;
            this.sAttackTrackBar.Name = "sAttackTrackBar";
            this.sAttackTrackBar.Size = new System.Drawing.Size(156, 45);
            this.sAttackTrackBar.TabIndex = 19;
            this.sAttackTrackBar.TickFrequency = 5;
            this.sAttackTrackBar.Scroll += new System.EventHandler(this.sAttackTrackBar_Scroll);
            // 
            // gradientButton
            // 
            this.gradientButton.Location = new System.Drawing.Point(180, 101);
            this.gradientButton.Name = "gradientButton";
            this.gradientButton.Size = new System.Drawing.Size(25, 23);
            this.gradientButton.TabIndex = 3;
            this.gradientButton.Text = "...";
            this.gradientButton.UseVisualStyleBackColor = true;
            this.gradientButton.Click += new System.EventHandler(this.gradientButton_Click);
            // 
            // fftWindowComboBox
            // 
            this.fftWindowComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fftWindowComboBox.FormattingEnabled = true;
            this.fftWindowComboBox.Items.AddRange(new object[] {
            "None",
            "Hamming",
            "Blackman",
            "Blackman-Harris",
            "Hann-Poisson",
            "Youssef"});
            this.fftWindowComboBox.Location = new System.Drawing.Point(70, 51);
            this.fftWindowComboBox.Name = "fftWindowComboBox";
            this.fftWindowComboBox.Size = new System.Drawing.Size(135, 21);
            this.fftWindowComboBox.TabIndex = 1;
            this.fftWindowComboBox.SelectedIndexChanged += new System.EventHandler(this.fftWindowComboBox_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Window";
            // 
            // markPeaksCheckBox
            // 
            this.markPeaksCheckBox.Location = new System.Drawing.Point(110, 361);
            this.markPeaksCheckBox.Name = "markPeaksCheckBox";
            this.markPeaksCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.markPeaksCheckBox.Size = new System.Drawing.Size(94, 17);
            this.markPeaksCheckBox.TabIndex = 33;
            this.markPeaksCheckBox.Text = "Mark Peaks";
            this.markPeaksCheckBox.UseVisualStyleBackColor = true;
            this.markPeaksCheckBox.CheckedChanged += new System.EventHandler(this.markPeaksCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(962, 673);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.zoomTrackBar);
            this.Controls.Add(this.contrastTrackBar);
            this.Controls.Add(this.panSplitContainer);
            this.Controls.Add(this.fileSelectButton);
            this.Controls.Add(this.waveFileRadioButton);
            this.Controls.Add(this.iqStreamRadioButton);
            this.Controls.Add(this.wavFileTextBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.playButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDR#";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panSplitContainer.Panel1.ResumeLayout(false);
            this.panSplitContainer.Panel2.ResumeLayout(false);
            this.panSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).EndInit();
            this.controlPanel.ResumeLayout(false);
            this.radioCollapsiblePanel.ResumeLayout(false);
            this.radioCollapsiblePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyShiftNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cwShiftNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerFreqNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterBandwidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterOrderNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.squelchNumericUpDown)).EndInit();
            this.audioCollapsiblePanel.ResumeLayout(false);
            this.audioCollapsiblePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.audioGainTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.latencyNumericUpDown)).EndInit();
            this.agcCollapsiblePanel.ResumeLayout(false);
            this.agcCollapsiblePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.agcSlopeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcThresholdNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcDecayNumericUpDown)).EndInit();
            this.displayCollapsiblePanel.ResumeLayout(false);
            this.displayCollapsiblePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.wDecayTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wAttackTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sDecayTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sAttackTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.IContainer components;

        private Button playButton;
        private Button stopButton;
        private Label label3;
        private OpenFileDialog openDlg;
        private CheckBox agcCheckBox;
        private Label label4;
        private NumericUpDown agcThresholdNumericUpDown;
        private SpectrumAnalyzer spectrumAnalyzer;
        private Waterfall waterfall;
        private TextBox wavFileTextBox;
        private RadioButton iqStreamRadioButton;
        private RadioButton waveFileRadioButton;
        private Button fileSelectButton;
        private Label label10;
        private NumericUpDown agcDecayNumericUpDown;
        private Label label12;
        private ComboBox outputDeviceComboBox;
        private Label label11;
        private ComboBox inputDeviceComboBox;
        private Label label13;
        private ComboBox sampleRateComboBox;
        private SplitContainer panSplitContainer;
        private Label label7;
        private ComboBox viewComboBox;
        private Label label8;
        private ComboBox fftWindowComboBox;
        private Timer iqTimer;
        private Button gradientButton;
        private Label label14;
        private TrackBar contrastTrackBar;
        private TrackBar zoomTrackBar;
        private Label label19;
        private Label label20;
        private Label label21;
        private ComboBox fftResolutionComboBox;
        private NumericUpDown agcSlopeNumericUpDown;
        private Label label22;
        private RadioButton nfmRadioButton;
        private RadioButton cwuRadioButton;
        private RadioButton cwlRadioButton;
        private RadioButton amRadioButton;
        private RadioButton dsbRadioButton;
        private NumericUpDown frequencyNumericUpDown;
        private RadioButton wfmRadioButton;
        private Label label2;
        private Button frontendGuiButton;
        private RadioButton lsbRadioButton;
        private Label label18;
        private RadioButton usbRadioButton;
        private ComboBox stepSizeComboBox;
        private NumericUpDown filterBandwidthNumericUpDown;
        private Label label1;
        private NumericUpDown squelchNumericUpDown;
        private NumericUpDown filterOrderNumericUpDown;
        private Label label16;
        private Label label5;
        private ComboBox frontEndComboBox;
        private ComboBox filterTypeComboBox;
        private Label label9;
        private CheckBox swapInQCheckBox;
        private CheckBox correctIQCheckBox;
        private NumericUpDown centerFreqNumericUpDown;
        private CollapsiblePanel.CollapsiblePanel radioCollapsiblePanel;
        private CollapsiblePanel.CollapsiblePanel audioCollapsiblePanel;
        private CollapsiblePanel.CollapsiblePanel agcCollapsiblePanel;
        private CheckBox agcUseHangCheckBox;
        private CollapsiblePanel.CollapsiblePanel displayCollapsiblePanel;
        private NumericUpDown latencyNumericUpDown;
        private Label label6;
        private Label label15;
        private NumericUpDown cwShiftNumericUpDown;
        private Panel controlPanel;
        private Label label25;
        private Label label26;
        private Label label24;
        private Label label23;
        private TrackBar wDecayTrackBar;
        private TrackBar wAttackTrackBar;
        private TrackBar sDecayTrackBar;
        private TrackBar sAttackTrackBar;
        private CheckBox snapFrequencyCheckBox;
        private TrackBar audioGainTrackBar;
        private CheckBox fmStereoCheckBox;
        private CheckBox filterAudioCheckBox;
        private CheckBox useSquelchCheckBox;
        private CheckBox frequencyShiftCheckBox;
        private NumericUpDown frequencyShiftNumericUpDown;
        private CheckBox markPeaksCheckBox;
    }
}
