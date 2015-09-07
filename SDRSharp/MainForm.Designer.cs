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
            this.playStopButton = new System.Windows.Forms.Button();
            this.panSplitContainer = new System.Windows.Forms.SplitContainer();
            this.spectrumAnalyzer = new SDRSharp.PanView.SpectrumAnalyzer();
            this.waterfall = new SDRSharp.PanView.Waterfall();
            this.iqTimer = new System.Windows.Forms.Timer(this.components);
            this.fftContrastTrackBar = new System.Windows.Forms.TrackBar();
            this.fftZoomTrackBar = new System.Windows.Forms.TrackBar();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.radioCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            this.rawRadioButton = new System.Windows.Forms.RadioButton();
            this.frequencyShiftCheckBox = new System.Windows.Forms.CheckBox();
            this.markPeaksCheckBox = new System.Windows.Forms.CheckBox();
            this.frequencyShiftNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.cwRadioButton = new System.Windows.Forms.RadioButton();
            this.nfmRadioButton = new System.Windows.Forms.RadioButton();
            this.useSquelchCheckBox = new System.Windows.Forms.CheckBox();
            this.fmStereoCheckBox = new System.Windows.Forms.CheckBox();
            this.dsbRadioButton = new System.Windows.Forms.RadioButton();
            this.wfmRadioButton = new System.Windows.Forms.RadioButton();
            this.amRadioButton = new System.Windows.Forms.RadioButton();
            this.snapFrequencyCheckBox = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.lsbRadioButton = new System.Windows.Forms.RadioButton();
            this.cwShiftNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.usbRadioButton = new System.Windows.Forms.RadioButton();
            this.correctIQCheckBox = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.stepSizeComboBox = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.filterBandwidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.filterOrderNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.squelchNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.filterTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.swapIQCheckBox = new System.Windows.Forms.CheckBox();
            this.audioCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            this.unityGainCheckBox = new System.Windows.Forms.CheckBox();
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
            this.showMaxLineCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fftOffsetTrackBar = new System.Windows.Forms.TrackBar();
            this.fftRangeTrackBar = new System.Windows.Forms.TrackBar();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.smoothingGroupBox = new System.Windows.Forms.GroupBox();
            this.sAttackTrackBar = new System.Windows.Forms.TrackBar();
            this.sDecayTrackBar = new System.Windows.Forms.TrackBar();
            this.wAttackTrackBar = new System.Windows.Forms.TrackBar();
            this.wDecayTrackBar = new System.Windows.Forms.TrackBar();
            this.label23 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.useTimestampsCheckBox = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.fftResolutionComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.viewComboBox = new System.Windows.Forms.ComboBox();
            this.gradientButton = new System.Windows.Forms.Button();
            this.fftWindowComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.configureSourceButton = new System.Windows.Forms.Button();
            this.iqSourceComboBox = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.fftSpeedTrackBar = new System.Windows.Forms.TrackBar();
            this.scrollPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.vfoFrequencyEdit = new SDRSharp.FrequencyEdit.FrequencyEdit();
            this.panSplitContainer.Panel1.SuspendLayout();
            this.panSplitContainer.Panel2.SuspendLayout();
            this.panSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fftContrastTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fftZoomTrackBar)).BeginInit();
            this.controlPanel.SuspendLayout();
            this.radioCollapsiblePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyShiftNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cwShiftNumericUpDown)).BeginInit();
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
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fftOffsetTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fftRangeTrackBar)).BeginInit();
            this.smoothingGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sAttackTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sDecayTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wAttackTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wDecayTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fftSpeedTrackBar)).BeginInit();
            this.scrollPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // openDlg
            // 
            this.openDlg.DefaultExt = "wav";
            this.openDlg.Filter = "WAV files|*.wav";
            // 
            // playStopButton
            // 
            this.playStopButton.Location = new System.Drawing.Point(12, 11);
            this.playStopButton.Name = "playStopButton";
            this.playStopButton.Size = new System.Drawing.Size(50, 25);
            this.playStopButton.TabIndex = 0;
            this.playStopButton.Text = "Play";
            this.playStopButton.Click += new System.EventHandler(this.playStopButton_Click);
            // 
            // panSplitContainer
            // 
            this.panSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panSplitContainer.Location = new System.Drawing.Point(256, 42);
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
            this.panSplitContainer.Size = new System.Drawing.Size(627, 662);
            this.panSplitContainer.SplitterDistance = 212;
            this.panSplitContainer.TabIndex = 13;
            // 
            // spectrumAnalyzer
            // 
            this.spectrumAnalyzer.Attack = 0.9D;
            this.spectrumAnalyzer.AutoSize = true;
            this.spectrumAnalyzer.BandType = SDRSharp.PanView.BandType.Center;
            this.spectrumAnalyzer.CenterFrequency = ((long)(0));
            this.spectrumAnalyzer.Decay = 0.3D;
            this.spectrumAnalyzer.DisplayOffset = 0;
            this.spectrumAnalyzer.DisplayRange = 130;
            this.spectrumAnalyzer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spectrumAnalyzer.FilterBandwidth = 10000;
            this.spectrumAnalyzer.FilterOffset = 100;
            this.spectrumAnalyzer.Frequency = ((long)(0));
            this.spectrumAnalyzer.Location = new System.Drawing.Point(0, 0);
            this.spectrumAnalyzer.MarkPeaks = false;
            this.spectrumAnalyzer.Name = "spectrumAnalyzer";
            this.spectrumAnalyzer.Size = new System.Drawing.Size(627, 212);
            this.spectrumAnalyzer.SpectrumWidth = 48000;
            this.spectrumAnalyzer.StatusText = null;
            this.spectrumAnalyzer.StepSize = 1000;
            this.spectrumAnalyzer.TabIndex = 0;
            this.spectrumAnalyzer.UseSmoothing = true;
            this.spectrumAnalyzer.UseSnap = false;
            this.spectrumAnalyzer.Zoom = 0;
            this.spectrumAnalyzer.FrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_FrequencyChanged);
            this.spectrumAnalyzer.CenterFrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_CenterFrequencyChanged);
            this.spectrumAnalyzer.BandwidthChanged += new SDRSharp.PanView.ManualBandwidthChange(this.panview_BandwidthChanged);
            // 
            // waterfall
            // 
            this.waterfall.Attack = 0.9D;
            this.waterfall.AutoSize = true;
            this.waterfall.BandType = SDRSharp.PanView.BandType.Center;
            this.waterfall.CenterFrequency = ((long)(0));
            this.waterfall.Contrast = 0;
            this.waterfall.Decay = 0.5D;
            this.waterfall.DisplayOffset = 0;
            this.waterfall.DisplayRange = 130;
            this.waterfall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waterfall.FilterBandwidth = 10000;
            this.waterfall.FilterOffset = 0;
            this.waterfall.Frequency = ((long)(0));
            this.waterfall.Location = new System.Drawing.Point(0, 0);
            this.waterfall.Name = "waterfall";
            this.waterfall.Size = new System.Drawing.Size(627, 446);
            this.waterfall.SpectrumWidth = 48000;
            this.waterfall.StepSize = 0;
            this.waterfall.TabIndex = 0;
            this.waterfall.TimestampInterval = 100;
            this.waterfall.UseSmoothing = true;
            this.waterfall.UseSnap = false;
            this.waterfall.UseTimestamps = false;
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
            // fftContrastTrackBar
            // 
            this.fftContrastTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fftContrastTrackBar.Location = new System.Drawing.Point(890, 262);
            this.fftContrastTrackBar.Maximum = 24;
            this.fftContrastTrackBar.Minimum = -24;
            this.fftContrastTrackBar.Name = "fftContrastTrackBar";
            this.fftContrastTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.fftContrastTrackBar.RightToLeftLayout = true;
            this.fftContrastTrackBar.Size = new System.Drawing.Size(45, 184);
            this.fftContrastTrackBar.TabIndex = 17;
            this.fftContrastTrackBar.TickFrequency = 6;
            this.fftContrastTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.fftContrastTrackBar.ValueChanged += new System.EventHandler(this.fftContrastTrackBar_Changed);
            // 
            // fftZoomTrackBar
            // 
            this.fftZoomTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fftZoomTrackBar.Location = new System.Drawing.Point(890, 58);
            this.fftZoomTrackBar.Maximum = 50;
            this.fftZoomTrackBar.Name = "fftZoomTrackBar";
            this.fftZoomTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.fftZoomTrackBar.RightToLeftLayout = true;
            this.fftZoomTrackBar.Size = new System.Drawing.Size(45, 184);
            this.fftZoomTrackBar.TabIndex = 18;
            this.fftZoomTrackBar.TickFrequency = 5;
            this.fftZoomTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.fftZoomTrackBar.ValueChanged += new System.EventHandler(this.fftZoomTrackBar_ValueChanged);
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(895, 42);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(34, 13);
            this.label19.TabIndex = 19;
            this.label19.Text = "Zoom";
            // 
            // label20
            // 
            this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(889, 246);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(46, 13);
            this.label20.TabIndex = 20;
            this.label20.Text = "Contrast";
            // 
            // controlPanel
            // 
            this.controlPanel.AutoSize = true;
            this.controlPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.controlPanel.Controls.Add(this.radioCollapsiblePanel);
            this.controlPanel.Controls.Add(this.audioCollapsiblePanel);
            this.controlPanel.Controls.Add(this.displayCollapsiblePanel);
            this.controlPanel.Controls.Add(this.agcCollapsiblePanel);
            this.controlPanel.Location = new System.Drawing.Point(0, 0);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(220, 1090);
            this.controlPanel.TabIndex = 25;
            // 
            // radioCollapsiblePanel
            // 
            this.radioCollapsiblePanel.Controls.Add(this.rawRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.frequencyShiftCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.markPeaksCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.frequencyShiftNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.cwRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.nfmRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.useSquelchCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.fmStereoCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.dsbRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.wfmRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.amRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.snapFrequencyCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.label15);
            this.radioCollapsiblePanel.Controls.Add(this.lsbRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.cwShiftNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.usbRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.correctIQCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.label18);
            this.radioCollapsiblePanel.Controls.Add(this.stepSizeComboBox);
            this.radioCollapsiblePanel.Controls.Add(this.label16);
            this.radioCollapsiblePanel.Controls.Add(this.filterBandwidthNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.filterOrderNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.label1);
            this.radioCollapsiblePanel.Controls.Add(this.squelchNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.filterTypeComboBox);
            this.radioCollapsiblePanel.Controls.Add(this.label5);
            this.radioCollapsiblePanel.Controls.Add(this.swapIQCheckBox);
            this.radioCollapsiblePanel.ExpandedHeight = 277;
            this.radioCollapsiblePanel.Location = new System.Drawing.Point(0, 2);
            this.radioCollapsiblePanel.Name = "radioCollapsiblePanel";
            this.radioCollapsiblePanel.NextPanel = this.audioCollapsiblePanel;
            this.radioCollapsiblePanel.PanelTitle = "Radio";
            this.radioCollapsiblePanel.Size = new System.Drawing.Size(217, 297);
            this.radioCollapsiblePanel.TabIndex = 21;
            // 
            // rawRadioButton
            // 
            this.rawRadioButton.AutoSize = true;
            this.rawRadioButton.Location = new System.Drawing.Point(154, 49);
            this.rawRadioButton.Name = "rawRadioButton";
            this.rawRadioButton.Size = new System.Drawing.Size(51, 17);
            this.rawRadioButton.TabIndex = 7;
            this.rawRadioButton.Text = "RAW";
            this.rawRadioButton.UseVisualStyleBackColor = true;
            this.rawRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckStateChanged);
            // 
            // frequencyShiftCheckBox
            // 
            this.frequencyShiftCheckBox.AutoSize = true;
            this.frequencyShiftCheckBox.Location = new System.Drawing.Point(9, 77);
            this.frequencyShiftCheckBox.Name = "frequencyShiftCheckBox";
            this.frequencyShiftCheckBox.Size = new System.Drawing.Size(47, 17);
            this.frequencyShiftCheckBox.TabIndex = 10;
            this.frequencyShiftCheckBox.Text = "Shift";
            this.frequencyShiftCheckBox.UseVisualStyleBackColor = true;
            this.frequencyShiftCheckBox.CheckedChanged += new System.EventHandler(this.frequencyShiftCheckBox_CheckStateChanged);
            // 
            // markPeaksCheckBox
            // 
            this.markPeaksCheckBox.Location = new System.Drawing.Point(111, 273);
            this.markPeaksCheckBox.Name = "markPeaksCheckBox";
            this.markPeaksCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.markPeaksCheckBox.Size = new System.Drawing.Size(94, 17);
            this.markPeaksCheckBox.TabIndex = 33;
            this.markPeaksCheckBox.Text = "Mark Peaks";
            this.markPeaksCheckBox.UseVisualStyleBackColor = true;
            this.markPeaksCheckBox.CheckedChanged += new System.EventHandler(this.markPeaksCheckBox_CheckedChanged);
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
            this.frequencyShiftNumericUpDown.Location = new System.Drawing.Point(70, 72);
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
            this.frequencyShiftNumericUpDown.Size = new System.Drawing.Size(134, 24);
            this.frequencyShiftNumericUpDown.TabIndex = 11;
            this.frequencyShiftNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.frequencyShiftNumericUpDown.ThousandsSeparator = true;
            this.frequencyShiftNumericUpDown.ValueChanged += new System.EventHandler(this.frequencyShiftNumericUpDown_ValueChanged);
            // 
            // cwRadioButton
            // 
            this.cwRadioButton.AutoSize = true;
            this.cwRadioButton.Location = new System.Drawing.Point(106, 49);
            this.cwRadioButton.Name = "cwRadioButton";
            this.cwRadioButton.Size = new System.Drawing.Size(43, 17);
            this.cwRadioButton.TabIndex = 6;
            this.cwRadioButton.Text = "CW";
            this.cwRadioButton.UseVisualStyleBackColor = true;
            this.cwRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckStateChanged);
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
            this.nfmRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckStateChanged);
            // 
            // useSquelchCheckBox
            // 
            this.useSquelchCheckBox.Location = new System.Drawing.Point(10, 165);
            this.useSquelchCheckBox.Name = "useSquelchCheckBox";
            this.useSquelchCheckBox.Size = new System.Drawing.Size(69, 17);
            this.useSquelchCheckBox.TabIndex = 17;
            this.useSquelchCheckBox.Text = "Squelch";
            this.useSquelchCheckBox.UseVisualStyleBackColor = true;
            this.useSquelchCheckBox.CheckedChanged += new System.EventHandler(this.useSquelchCheckBox_CheckedChanged);
            // 
            // fmStereoCheckBox
            // 
            this.fmStereoCheckBox.Enabled = false;
            this.fmStereoCheckBox.Location = new System.Drawing.Point(6, 273);
            this.fmStereoCheckBox.Name = "fmStereoCheckBox";
            this.fmStereoCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.fmStereoCheckBox.Size = new System.Drawing.Size(94, 17);
            this.fmStereoCheckBox.TabIndex = 24;
            this.fmStereoCheckBox.Text = "FM Stereo";
            this.fmStereoCheckBox.UseVisualStyleBackColor = true;
            this.fmStereoCheckBox.CheckedChanged += new System.EventHandler(this.fmStereoCheckBox_CheckedChanged);
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
            this.dsbRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckStateChanged);
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
            this.wfmRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckStateChanged);
            // 
            // amRadioButton
            // 
            this.amRadioButton.AutoSize = true;
            this.amRadioButton.Location = new System.Drawing.Point(60, 26);
            this.amRadioButton.Name = "amRadioButton";
            this.amRadioButton.Size = new System.Drawing.Size(41, 17);
            this.amRadioButton.TabIndex = 1;
            this.amRadioButton.Text = "AM";
            this.amRadioButton.UseVisualStyleBackColor = true;
            this.amRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckStateChanged);
            // 
            // snapFrequencyCheckBox
            // 
            this.snapFrequencyCheckBox.Location = new System.Drawing.Point(6, 225);
            this.snapFrequencyCheckBox.Name = "snapFrequencyCheckBox";
            this.snapFrequencyCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.snapFrequencyCheckBox.Size = new System.Drawing.Size(94, 17);
            this.snapFrequencyCheckBox.TabIndex = 20;
            this.snapFrequencyCheckBox.Text = "Snap to grid";
            this.snapFrequencyCheckBox.UseVisualStyleBackColor = true;
            this.snapFrequencyCheckBox.CheckedChanged += new System.EventHandler(this.stepSizeComboBox_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(112, 166);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(49, 13);
            this.label15.TabIndex = 32;
            this.label15.Text = "CW Shift";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.lsbRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckStateChanged);
            // 
            // cwShiftNumericUpDown
            // 
            this.cwShiftNumericUpDown.Enabled = false;
            this.cwShiftNumericUpDown.Location = new System.Drawing.Point(111, 183);
            this.cwShiftNumericUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.cwShiftNumericUpDown.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
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
            // usbRadioButton
            // 
            this.usbRadioButton.AutoSize = true;
            this.usbRadioButton.Location = new System.Drawing.Point(154, 26);
            this.usbRadioButton.Name = "usbRadioButton";
            this.usbRadioButton.Size = new System.Drawing.Size(47, 17);
            this.usbRadioButton.TabIndex = 3;
            this.usbRadioButton.Text = "USB";
            this.usbRadioButton.UseVisualStyleBackColor = true;
            this.usbRadioButton.CheckedChanged += new System.EventHandler(this.modeRadioButton_CheckStateChanged);
            // 
            // correctIQCheckBox
            // 
            this.correctIQCheckBox.Location = new System.Drawing.Point(6, 250);
            this.correctIQCheckBox.Name = "correctIQCheckBox";
            this.correctIQCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.correctIQCheckBox.Size = new System.Drawing.Size(94, 17);
            this.correctIQCheckBox.TabIndex = 22;
            this.correctIQCheckBox.Text = "Correct IQ";
            this.correctIQCheckBox.UseVisualStyleBackColor = true;
            this.correctIQCheckBox.CheckedChanged += new System.EventHandler(this.autoCorrectIQCheckBox_CheckStateChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(112, 206);
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
            "7.5 kHz",
            "8.33 kHz",
            "9 kHz",
            "10 kHz",
            "12.5 kHz",
            "15 kHz",
            "20 kHz",
            "25 kHz",
            "30 kHz",
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
            this.stepSizeComboBox.Location = new System.Drawing.Point(111, 223);
            this.stepSizeComboBox.Name = "stepSizeComboBox";
            this.stepSizeComboBox.Size = new System.Drawing.Size(94, 21);
            this.stepSizeComboBox.TabIndex = 21;
            this.stepSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.stepSizeComboBox_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 105);
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
            this.filterBandwidthNumericUpDown.Location = new System.Drawing.Point(6, 143);
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
            this.filterOrderNumericUpDown.Location = new System.Drawing.Point(111, 143);
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
            this.label1.Location = new System.Drawing.Point(6, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Filter bandwidth";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // squelchNumericUpDown
            // 
            this.squelchNumericUpDown.Enabled = false;
            this.squelchNumericUpDown.Location = new System.Drawing.Point(6, 183);
            this.squelchNumericUpDown.Name = "squelchNumericUpDown";
            this.squelchNumericUpDown.Size = new System.Drawing.Size(94, 20);
            this.squelchNumericUpDown.TabIndex = 18;
            this.squelchNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.squelchNumericUpDown.ValueChanged += new System.EventHandler(this.squelchNumericUpDown_ValueChanged);
            // 
            // filterTypeComboBox
            // 
            this.filterTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterTypeComboBox.FormattingEnabled = true;
            this.filterTypeComboBox.Items.AddRange(new object[] {
            "Hamming",
            "Blackman",
            "Blackman-Harris 4",
            "Blackman-Harris 7",
            "Hann-Poisson",
            "Youssef"});
            this.filterTypeComboBox.Location = new System.Drawing.Point(70, 102);
            this.filterTypeComboBox.Name = "filterTypeComboBox";
            this.filterTypeComboBox.Size = new System.Drawing.Size(134, 21);
            this.filterTypeComboBox.TabIndex = 14;
            this.filterTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.filterTypeComboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(112, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Filter order";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // swapIQCheckBox
            // 
            this.swapIQCheckBox.Location = new System.Drawing.Point(111, 250);
            this.swapIQCheckBox.Name = "swapIQCheckBox";
            this.swapIQCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.swapIQCheckBox.Size = new System.Drawing.Size(94, 17);
            this.swapIQCheckBox.TabIndex = 23;
            this.swapIQCheckBox.Text = "Swap I && Q";
            this.swapIQCheckBox.UseVisualStyleBackColor = true;
            this.swapIQCheckBox.CheckedChanged += new System.EventHandler(this.swapIQCheckBox_CheckedChanged);
            // 
            // audioCollapsiblePanel
            // 
            this.audioCollapsiblePanel.Controls.Add(this.unityGainCheckBox);
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
            this.audioCollapsiblePanel.Location = new System.Drawing.Point(0, 299);
            this.audioCollapsiblePanel.Name = "audioCollapsiblePanel";
            this.audioCollapsiblePanel.NextPanel = this.agcCollapsiblePanel;
            this.audioCollapsiblePanel.PanelTitle = "Audio";
            this.audioCollapsiblePanel.Size = new System.Drawing.Size(217, 196);
            this.audioCollapsiblePanel.TabIndex = 22;
            // 
            // unityGainCheckBox
            // 
            this.unityGainCheckBox.Location = new System.Drawing.Point(9, 177);
            this.unityGainCheckBox.Name = "unityGainCheckBox";
            this.unityGainCheckBox.Size = new System.Drawing.Size(87, 17);
            this.unityGainCheckBox.TabIndex = 5;
            this.unityGainCheckBox.Text = "Unity Gain";
            this.unityGainCheckBox.UseVisualStyleBackColor = true;
            this.unityGainCheckBox.CheckStateChanged += new System.EventHandler(this.unityGainCheckBox_CheckStateChanged);
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
            this.latencyNumericUpDown.Location = new System.Drawing.Point(125, 151);
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
            1,
            0,
            0,
            0});
            // 
            // filterAudioCheckBox
            // 
            this.filterAudioCheckBox.Location = new System.Drawing.Point(125, 177);
            this.filterAudioCheckBox.Name = "filterAudioCheckBox";
            this.filterAudioCheckBox.Size = new System.Drawing.Size(87, 17);
            this.filterAudioCheckBox.TabIndex = 6;
            this.filterAudioCheckBox.Text = "Filter Audio";
            this.filterAudioCheckBox.UseVisualStyleBackColor = true;
            this.filterAudioCheckBox.CheckedChanged += new System.EventHandler(this.filterAudioCheckBox_CheckStateChanged);
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
            this.agcCollapsiblePanel.Location = new System.Drawing.Point(0, 495);
            this.agcCollapsiblePanel.Name = "agcCollapsiblePanel";
            this.agcCollapsiblePanel.NextPanel = this.displayCollapsiblePanel;
            this.agcCollapsiblePanel.PanelTitle = "AGC";
            this.agcCollapsiblePanel.Size = new System.Drawing.Size(217, 123);
            this.agcCollapsiblePanel.TabIndex = 23;
            // 
            // agcUseHangCheckBox
            // 
            this.agcUseHangCheckBox.AutoSize = true;
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
            this.agcSlopeNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
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
            2000,
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
            this.displayCollapsiblePanel.Controls.Add(this.showMaxLineCheckBox);
            this.displayCollapsiblePanel.Controls.Add(this.groupBox1);
            this.displayCollapsiblePanel.Controls.Add(this.smoothingGroupBox);
            this.displayCollapsiblePanel.Controls.Add(this.useTimestampsCheckBox);
            this.displayCollapsiblePanel.Controls.Add(this.label21);
            this.displayCollapsiblePanel.Controls.Add(this.fftResolutionComboBox);
            this.displayCollapsiblePanel.Controls.Add(this.label7);
            this.displayCollapsiblePanel.Controls.Add(this.label14);
            this.displayCollapsiblePanel.Controls.Add(this.viewComboBox);
            this.displayCollapsiblePanel.Controls.Add(this.gradientButton);
            this.displayCollapsiblePanel.Controls.Add(this.fftWindowComboBox);
            this.displayCollapsiblePanel.Controls.Add(this.label8);
            this.displayCollapsiblePanel.ExpandedHeight = 449;
            this.displayCollapsiblePanel.Location = new System.Drawing.Point(0, 618);
            this.displayCollapsiblePanel.Name = "displayCollapsiblePanel";
            this.displayCollapsiblePanel.NextPanel = null;
            this.displayCollapsiblePanel.PanelTitle = "FFT Display";
            this.displayCollapsiblePanel.Size = new System.Drawing.Size(217, 469);
            this.displayCollapsiblePanel.TabIndex = 24;
            // 
            // showMaxLineCheckBox
            // 
            this.showMaxLineCheckBox.AutoSize = true;
            this.showMaxLineCheckBox.Checked = true;
            this.showMaxLineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showMaxLineCheckBox.Location = new System.Drawing.Point(9, 125);
            this.showMaxLineCheckBox.Name = "showMaxLineCheckBox";
            this.showMaxLineCheckBox.Size = new System.Drawing.Size(100, 17);
            this.showMaxLineCheckBox.TabIndex = 33;
            this.showMaxLineCheckBox.Text = "Show Maximum";
            this.showMaxLineCheckBox.UseVisualStyleBackColor = true;
            this.showMaxLineCheckBox.CheckedChanged += new System.EventHandler(this.showMaxLineCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.fftOffsetTrackBar);
            this.groupBox1.Controls.Add(this.fftRangeTrackBar);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Location = new System.Drawing.Point(0, 352);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 111);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spectrum";
            // 
            // fftOffsetTrackBar
            // 
            this.fftOffsetTrackBar.Location = new System.Drawing.Point(58, 19);
            this.fftOffsetTrackBar.Maximum = 15;
            this.fftOffsetTrackBar.Name = "fftOffsetTrackBar";
            this.fftOffsetTrackBar.Size = new System.Drawing.Size(153, 45);
            this.fftOffsetTrackBar.TabIndex = 27;
            this.fftOffsetTrackBar.Scroll += new System.EventHandler(this.fftOffsetTrackBar_Scroll);
            // 
            // fftRangeTrackBar
            // 
            this.fftRangeTrackBar.LargeChange = 10;
            this.fftRangeTrackBar.Location = new System.Drawing.Point(58, 62);
            this.fftRangeTrackBar.Maximum = 15;
            this.fftRangeTrackBar.Minimum = 2;
            this.fftRangeTrackBar.Name = "fftRangeTrackBar";
            this.fftRangeTrackBar.Size = new System.Drawing.Size(153, 45);
            this.fftRangeTrackBar.TabIndex = 28;
            this.fftRangeTrackBar.Value = 13;
            this.fftRangeTrackBar.Scroll += new System.EventHandler(this.fftRangeTrackBar_Scroll);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(8, 71);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(39, 13);
            this.label27.TabIndex = 30;
            this.label27.Text = "Range";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(8, 27);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(35, 13);
            this.label28.TabIndex = 29;
            this.label28.Text = "Offset";
            // 
            // smoothingGroupBox
            // 
            this.smoothingGroupBox.Controls.Add(this.sAttackTrackBar);
            this.smoothingGroupBox.Controls.Add(this.sDecayTrackBar);
            this.smoothingGroupBox.Controls.Add(this.wAttackTrackBar);
            this.smoothingGroupBox.Controls.Add(this.wDecayTrackBar);
            this.smoothingGroupBox.Controls.Add(this.label23);
            this.smoothingGroupBox.Controls.Add(this.label25);
            this.smoothingGroupBox.Controls.Add(this.label24);
            this.smoothingGroupBox.Controls.Add(this.label26);
            this.smoothingGroupBox.Location = new System.Drawing.Point(0, 146);
            this.smoothingGroupBox.Name = "smoothingGroupBox";
            this.smoothingGroupBox.Size = new System.Drawing.Size(217, 200);
            this.smoothingGroupBox.TabIndex = 31;
            this.smoothingGroupBox.TabStop = false;
            this.smoothingGroupBox.Text = "Smoothing";
            // 
            // sAttackTrackBar
            // 
            this.sAttackTrackBar.Location = new System.Drawing.Point(58, 19);
            this.sAttackTrackBar.Maximum = 50;
            this.sAttackTrackBar.Name = "sAttackTrackBar";
            this.sAttackTrackBar.Size = new System.Drawing.Size(153, 45);
            this.sAttackTrackBar.TabIndex = 5;
            this.sAttackTrackBar.TickFrequency = 5;
            this.sAttackTrackBar.ValueChanged += new System.EventHandler(this.sAttackTrackBar_ValueChanged);
            // 
            // sDecayTrackBar
            // 
            this.sDecayTrackBar.Location = new System.Drawing.Point(58, 63);
            this.sDecayTrackBar.Maximum = 50;
            this.sDecayTrackBar.Name = "sDecayTrackBar";
            this.sDecayTrackBar.Size = new System.Drawing.Size(153, 45);
            this.sDecayTrackBar.TabIndex = 6;
            this.sDecayTrackBar.TickFrequency = 5;
            this.sDecayTrackBar.ValueChanged += new System.EventHandler(this.sDecayTrackBar_ValueChanged);
            // 
            // wAttackTrackBar
            // 
            this.wAttackTrackBar.Location = new System.Drawing.Point(58, 107);
            this.wAttackTrackBar.Maximum = 50;
            this.wAttackTrackBar.Name = "wAttackTrackBar";
            this.wAttackTrackBar.Size = new System.Drawing.Size(153, 45);
            this.wAttackTrackBar.TabIndex = 7;
            this.wAttackTrackBar.TickFrequency = 5;
            this.wAttackTrackBar.ValueChanged += new System.EventHandler(this.wAttackTrackBar_ValueChanged);
            // 
            // wDecayTrackBar
            // 
            this.wDecayTrackBar.Location = new System.Drawing.Point(58, 151);
            this.wDecayTrackBar.Maximum = 50;
            this.wDecayTrackBar.Name = "wDecayTrackBar";
            this.wDecayTrackBar.Size = new System.Drawing.Size(153, 45);
            this.wDecayTrackBar.TabIndex = 8;
            this.wDecayTrackBar.TickFrequency = 5;
            this.wDecayTrackBar.ValueChanged += new System.EventHandler(this.wDecayTrackBar_ValueChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 27);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(48, 13);
            this.label23.TabIndex = 23;
            this.label23.Text = "S-Attack";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 157);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(52, 13);
            this.label25.TabIndex = 26;
            this.label25.Text = "W-Decay";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(6, 71);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(48, 13);
            this.label24.TabIndex = 24;
            this.label24.Text = "S-Decay";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 114);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(52, 13);
            this.label26.TabIndex = 25;
            this.label26.Text = "W-Attack";
            // 
            // useTimestampsCheckBox
            // 
            this.useTimestampsCheckBox.Location = new System.Drawing.Point(9, 105);
            this.useTimestampsCheckBox.Name = "useTimestampsCheckBox";
            this.useTimestampsCheckBox.Size = new System.Drawing.Size(107, 17);
            this.useTimestampsCheckBox.TabIndex = 3;
            this.useTimestampsCheckBox.Text = "Use time markers";
            this.useTimestampsCheckBox.UseVisualStyleBackColor = true;
            this.useTimestampsCheckBox.CheckedChanged += new System.EventHandler(this.useTimestampCheckBox_CheckedChanged);
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
            // gradientButton
            // 
            this.gradientButton.Location = new System.Drawing.Point(180, 101);
            this.gradientButton.Name = "gradientButton";
            this.gradientButton.Size = new System.Drawing.Size(25, 23);
            this.gradientButton.TabIndex = 4;
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
            "Blackman-Harris 4",
            "Blackman-Harris 7",
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
            // configureSourceButton
            // 
            this.configureSourceButton.FlatAppearance.BorderSize = 0;
            this.configureSourceButton.Location = new System.Drawing.Point(256, 11);
            this.configureSourceButton.Name = "configureSourceButton";
            this.configureSourceButton.Size = new System.Drawing.Size(70, 25);
            this.configureSourceButton.TabIndex = 3;
            this.configureSourceButton.Text = "Configure";
            this.configureSourceButton.UseVisualStyleBackColor = true;
            this.configureSourceButton.Click += new System.EventHandler(this.frontendGuiButton_Click);
            // 
            // iqSourceComboBox
            // 
            this.iqSourceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.iqSourceComboBox.DropDownWidth = 135;
            this.iqSourceComboBox.FormattingEnabled = true;
            this.iqSourceComboBox.Location = new System.Drawing.Point(68, 13);
            this.iqSourceComboBox.Name = "iqSourceComboBox";
            this.iqSourceComboBox.Size = new System.Drawing.Size(182, 21);
            this.iqSourceComboBox.TabIndex = 2;
            this.iqSourceComboBox.SelectedIndexChanged += new System.EventHandler(this.iqSourceComboBox_SelectedIndexChanged);
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(894, 449);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(38, 13);
            this.label17.TabIndex = 27;
            this.label17.Text = "Speed";
            // 
            // fftSpeedTrackBar
            // 
            this.fftSpeedTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fftSpeedTrackBar.Location = new System.Drawing.Point(890, 465);
            this.fftSpeedTrackBar.Maximum = 100;
            this.fftSpeedTrackBar.Minimum = 1;
            this.fftSpeedTrackBar.Name = "fftSpeedTrackBar";
            this.fftSpeedTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.fftSpeedTrackBar.RightToLeftLayout = true;
            this.fftSpeedTrackBar.Size = new System.Drawing.Size(45, 184);
            this.fftSpeedTrackBar.TabIndex = 26;
            this.fftSpeedTrackBar.TickFrequency = 10;
            this.fftSpeedTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.fftSpeedTrackBar.Value = 50;
            this.fftSpeedTrackBar.ValueChanged += new System.EventHandler(this.fftSpeedTrackBar_ValueChanged);
            // 
            // scrollPanel
            // 
            this.scrollPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.scrollPanel.AutoScroll = true;
            this.scrollPanel.Controls.Add(this.controlPanel);
            this.scrollPanel.Location = new System.Drawing.Point(12, 42);
            this.scrollPanel.Name = "scrollPanel";
            this.scrollPanel.Size = new System.Drawing.Size(238, 662);
            this.scrollPanel.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(332, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "VFO";
            // 
            // vfoFrequencyEdit
            // 
            this.vfoFrequencyEdit.AutoSize = true;
            this.vfoFrequencyEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.vfoFrequencyEdit.BackColor = System.Drawing.Color.Transparent;
            this.vfoFrequencyEdit.Frequency = ((long)(0));
            this.vfoFrequencyEdit.Location = new System.Drawing.Point(366, 11);
            this.vfoFrequencyEdit.Name = "vfoFrequencyEdit";
            this.vfoFrequencyEdit.Size = new System.Drawing.Size(234, 25);
            this.vfoFrequencyEdit.StepSize = 0;
            this.vfoFrequencyEdit.TabIndex = 30;
            this.vfoFrequencyEdit.FrequencyChanged += new System.EventHandler(this.vfoFrequencyEdit_FrequencyChanged);
            this.vfoFrequencyEdit.FrequencyChanging += new System.EventHandler<SDRSharp.FrequencyEdit.FrequencyChangingEventArgs>(this.vfoFrequencyEdit_FrequencyChanging);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(947, 716);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.vfoFrequencyEdit);
            this.Controls.Add(this.scrollPanel);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.fftSpeedTrackBar);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.fftZoomTrackBar);
            this.Controls.Add(this.fftContrastTrackBar);
            this.Controls.Add(this.panSplitContainer);
            this.Controls.Add(this.playStopButton);
            this.Controls.Add(this.configureSourceButton);
            this.Controls.Add(this.iqSourceComboBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SDR#";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Move += new System.EventHandler(this.MainForm_Move);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.panSplitContainer.Panel1.ResumeLayout(false);
            this.panSplitContainer.Panel1.PerformLayout();
            this.panSplitContainer.Panel2.ResumeLayout(false);
            this.panSplitContainer.Panel2.PerformLayout();
            this.panSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fftContrastTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fftZoomTrackBar)).EndInit();
            this.controlPanel.ResumeLayout(false);
            this.radioCollapsiblePanel.ResumeLayout(false);
            this.radioCollapsiblePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyShiftNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cwShiftNumericUpDown)).EndInit();
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fftOffsetTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fftRangeTrackBar)).EndInit();
            this.smoothingGroupBox.ResumeLayout(false);
            this.smoothingGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sAttackTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sDecayTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wAttackTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wDecayTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fftSpeedTrackBar)).EndInit();
            this.scrollPanel.ResumeLayout(false);
            this.scrollPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.IContainer components;

        private Button playStopButton;
        private Label label3;
        private OpenFileDialog openDlg;
        private CheckBox agcCheckBox;
        private Label label4;
        private NumericUpDown agcThresholdNumericUpDown;
        private SpectrumAnalyzer spectrumAnalyzer;
        private Waterfall waterfall;
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
        private TrackBar fftContrastTrackBar;
        private TrackBar fftZoomTrackBar;
        private Label label19;
        private Label label20;
        private Label label21;
        private ComboBox fftResolutionComboBox;
        private NumericUpDown agcSlopeNumericUpDown;
        private Label label22;
        private RadioButton nfmRadioButton;
        private RadioButton rawRadioButton;
        private RadioButton cwRadioButton;
        private RadioButton amRadioButton;
        private RadioButton dsbRadioButton;
        private RadioButton wfmRadioButton;
        private Button configureSourceButton;
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
        private ComboBox iqSourceComboBox;
        private ComboBox filterTypeComboBox;
        private CheckBox swapIQCheckBox;
        private CheckBox correctIQCheckBox;
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
        private CheckBox useTimestampsCheckBox;
        private Label label17;
        private TrackBar fftSpeedTrackBar;
        private GroupBox groupBox1;
        private TrackBar fftOffsetTrackBar;
        private TrackBar fftRangeTrackBar;
        private Label label27;
        private Label label28;
        private GroupBox smoothingGroupBox;
        private Panel scrollPanel;
        private FrequencyEdit.FrequencyEdit vfoFrequencyEdit;
        private Label label2;
        private CheckBox unityGainCheckBox;
        private CheckBox showMaxLineCheckBox;
    }
}
