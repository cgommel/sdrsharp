using System.Windows.Forms;
using SDRSharp.PanView;

namespace SDRSharp
{
    public partial class MainForm
    {
        private Button playButton;
        private Button stopButton;
        private NumericUpDown audioGainNumericUpDown;
        private Label label3;
        private OpenFileDialog openDlg;
        private CheckBox agcCheckBox;
        private Label label4;
        private NumericUpDown agcThresholdNumericUpDown;
        private SpectrumAnalyzer spectrumAnalyzer;
        private Waterfall waterfall;
        private TextBox wavFileTextBox;
        private RadioButton soundCardRadioButton;
        private RadioButton wavFileRadioButton;
        private Button fileSelectButton;
        private Label label10;
        private NumericUpDown agcDecayNumericUpDown;
        private Label label12;
        private ComboBox outputDeviceComboBox;
        private Label label11;
        private ComboBox inputDeviceComboBox;
        private Label label13;
        private ComboBox sampleRateComboBox;

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
            this.label4 = new System.Windows.Forms.Label();
            this.agcThresholdNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.agcCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.audioGainNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.sampleRateComboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.outputDeviceComboBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.inputDeviceComboBox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.agcDecayNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.wavFileTextBox = new System.Windows.Forms.TextBox();
            this.soundCardRadioButton = new System.Windows.Forms.RadioButton();
            this.wavFileRadioButton = new System.Windows.Forms.RadioButton();
            this.fileSelectButton = new System.Windows.Forms.Button();
            this.panSplitContainer = new System.Windows.Forms.SplitContainer();
            this.spectrumAnalyzer = new SDRSharp.PanView.SpectrumAnalyzer();
            this.waterfall = new SDRSharp.PanView.Waterfall();
            this.agcSlopeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.fftResolutionComboBox = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.gradientButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.fftWindowComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.viewComboBox = new System.Windows.Forms.ComboBox();
            this.iqTimer = new System.Windows.Forms.Timer(this.components);
            this.contrastTrackBar = new System.Windows.Forms.TrackBar();
            this.zoomTrackBar = new System.Windows.Forms.TrackBar();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.frequencyNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.amRadioButton = new System.Windows.Forms.RadioButton();
            this.lsbRadioButton = new System.Windows.Forms.RadioButton();
            this.usbRadioButton = new System.Windows.Forms.RadioButton();
            this.filterBandwidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.filterOrderNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.filterTypeComboBox = new System.Windows.Forms.ComboBox();
            this.swapInQCheckBox = new System.Windows.Forms.CheckBox();
            this.centerFreqNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.correctIQCheckBox = new System.Windows.Forms.CheckBox();
            this.nfmRadioButton = new System.Windows.Forms.RadioButton();
            this.frontEndComboBox = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.fmSquelchNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.stepSizeComboBox = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.frontendGuiButton = new System.Windows.Forms.Button();
            this.wfmRadioButton = new System.Windows.Forms.RadioButton();
            this.dsbRadioButton = new System.Windows.Forms.RadioButton();
            this.cwlRadioButton = new System.Windows.Forms.RadioButton();
            this.cwuRadioButton = new System.Windows.Forms.RadioButton();
            this.radioCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            this.audioCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            this.latencyNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.agcCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            this.agcUseHangCheckBox = new System.Windows.Forms.CheckBox();
            this.displayCollapsiblePanel = new SDRSharp.CollapsiblePanel.CollapsiblePanel();
            ((System.ComponentModel.ISupportInitialize)(this.agcThresholdNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioGainNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcDecayNumericUpDown)).BeginInit();
            this.panSplitContainer.Panel1.SuspendLayout();
            this.panSplitContainer.Panel2.SuspendLayout();
            this.panSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.agcSlopeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterBandwidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterOrderNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerFreqNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fmSquelchNumericUpDown)).BeginInit();
            this.radioCollapsiblePanel.SuspendLayout();
            this.audioCollapsiblePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.latencyNumericUpDown)).BeginInit();
            this.agcCollapsiblePanel.SuspendLayout();
            this.displayCollapsiblePanel.SuspendLayout();
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Threshold (dB)";
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "AF Gain";
            // 
            // audioGainNumericUpDown
            // 
            this.audioGainNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.audioGainNumericUpDown.Location = new System.Drawing.Point(70, 26);
            this.audioGainNumericUpDown.Name = "audioGainNumericUpDown";
            this.audioGainNumericUpDown.Size = new System.Drawing.Size(135, 24);
            this.audioGainNumericUpDown.TabIndex = 0;
            this.audioGainNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.audioGainNumericUpDown.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.audioGainNumericUpDown.ValueChanged += new System.EventHandler(this.audioGainNumericUpDown_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(30, 13);
            this.label13.TabIndex = 28;
            this.label13.Text = "Rate";
            // 
            // sampleRateComboBox
            // 
            this.sampleRateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sampleRateComboBox.FormattingEnabled = true;
            this.sampleRateComboBox.Items.AddRange(new object[] {
            "22k Sample/s",
            "24k Sample/s",
            "32k Sample/s",
            "44.1k Sample/s",
            "48k Sample/s",
            "80k Sample/s",
            "96k Sample/s",
            "192k Sample/s"});
            this.sampleRateComboBox.Location = new System.Drawing.Point(70, 56);
            this.sampleRateComboBox.Name = "sampleRateComboBox";
            this.sampleRateComboBox.Size = new System.Drawing.Size(135, 21);
            this.sampleRateComboBox.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 113);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(39, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "Output";
            // 
            // outputDeviceComboBox
            // 
            this.outputDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outputDeviceComboBox.DropDownWidth = 300;
            this.outputDeviceComboBox.FormattingEnabled = true;
            this.outputDeviceComboBox.Location = new System.Drawing.Point(70, 110);
            this.outputDeviceComboBox.Name = "outputDeviceComboBox";
            this.outputDeviceComboBox.Size = new System.Drawing.Size(135, 21);
            this.outputDeviceComboBox.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 86);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(31, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "Input";
            // 
            // inputDeviceComboBox
            // 
            this.inputDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputDeviceComboBox.DropDownWidth = 300;
            this.inputDeviceComboBox.FormattingEnabled = true;
            this.inputDeviceComboBox.Location = new System.Drawing.Point(70, 83);
            this.inputDeviceComboBox.Name = "inputDeviceComboBox";
            this.inputDeviceComboBox.Size = new System.Drawing.Size(135, 21);
            this.inputDeviceComboBox.TabIndex = 2;
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
            // wavFileTextBox
            // 
            this.wavFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wavFileTextBox.Enabled = false;
            this.wavFileTextBox.Location = new System.Drawing.Point(280, 15);
            this.wavFileTextBox.Name = "wavFileTextBox";
            this.wavFileTextBox.Size = new System.Drawing.Size(636, 20);
            this.wavFileTextBox.TabIndex = 4;
            // 
            // soundCardRadioButton
            // 
            this.soundCardRadioButton.AutoSize = true;
            this.soundCardRadioButton.Checked = true;
            this.soundCardRadioButton.Location = new System.Drawing.Point(124, 16);
            this.soundCardRadioButton.Name = "soundCardRadioButton";
            this.soundCardRadioButton.Size = new System.Drawing.Size(80, 17);
            this.soundCardRadioButton.TabIndex = 2;
            this.soundCardRadioButton.TabStop = true;
            this.soundCardRadioButton.Text = "Sound card";
            this.soundCardRadioButton.UseVisualStyleBackColor = true;
            this.soundCardRadioButton.CheckedChanged += new System.EventHandler(this.soundCardRadioButton_CheckedChanged);
            // 
            // wavFileRadioButton
            // 
            this.wavFileRadioButton.AutoSize = true;
            this.wavFileRadioButton.Location = new System.Drawing.Point(210, 17);
            this.wavFileRadioButton.Name = "wavFileRadioButton";
            this.wavFileRadioButton.Size = new System.Drawing.Size(64, 17);
            this.wavFileRadioButton.TabIndex = 3;
            this.wavFileRadioButton.Text = "Wav file";
            this.wavFileRadioButton.UseVisualStyleBackColor = true;
            this.wavFileRadioButton.CheckedChanged += new System.EventHandler(this.waveFileRadioButton_CheckedChanged);
            // 
            // fileSelectButton
            // 
            this.fileSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelectButton.Enabled = false;
            this.fileSelectButton.Location = new System.Drawing.Point(922, 13);
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
            this.panSplitContainer.Location = new System.Drawing.Point(229, 42);
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
            this.panSplitContainer.Size = new System.Drawing.Size(668, 724);
            this.panSplitContainer.SplitterDistance = 225;
            this.panSplitContainer.TabIndex = 13;
            // 
            // spectrumAnalyzer
            // 
            this.spectrumAnalyzer.BandType = SDRSharp.PanView.BandType.Center;
            this.spectrumAnalyzer.CenterFrequency = 0;
            this.spectrumAnalyzer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spectrumAnalyzer.FilterBandwidth = 10000;
            this.spectrumAnalyzer.FilterOffset = 100;
            this.spectrumAnalyzer.Frequency = 0;
            this.spectrumAnalyzer.Location = new System.Drawing.Point(0, 0);
            this.spectrumAnalyzer.Name = "spectrumAnalyzer";
            this.spectrumAnalyzer.Size = new System.Drawing.Size(668, 225);
            this.spectrumAnalyzer.SpectrumWidth = 48000;
            this.spectrumAnalyzer.TabIndex = 0;
            this.spectrumAnalyzer.UseSmoothing = false;
            this.spectrumAnalyzer.Zoom = 0;
            this.spectrumAnalyzer.FrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_FrequencyChanged);
            this.spectrumAnalyzer.CenterFrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_CenterFrequencyChanged);
            this.spectrumAnalyzer.BandwidthChanged += new SDRSharp.PanView.ManualBandwidthChange(this.panview_BandwidthChanged);
            // 
            // waterfall
            // 
            this.waterfall.BandType = SDRSharp.PanView.BandType.Center;
            this.waterfall.CenterFrequency = 0;
            this.waterfall.Contrast = 0;
            this.waterfall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waterfall.FilterBandwidth = 10000;
            this.waterfall.FilterOffset = 0;
            this.waterfall.Frequency = 0;
            this.waterfall.Location = new System.Drawing.Point(0, 0);
            this.waterfall.Name = "waterfall";
            this.waterfall.Size = new System.Drawing.Size(668, 495);
            this.waterfall.SpectrumWidth = 48000;
            this.waterfall.TabIndex = 0;
            this.waterfall.UseSmoothing = false;
            this.waterfall.Zoom = 0;
            this.waterfall.FrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_FrequencyChanged);
            this.waterfall.CenterFrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_CenterFrequencyChanged);
            this.waterfall.BandwidthChanged += new SDRSharp.PanView.ManualBandwidthChange(this.panview_BandwidthChanged);
            this.waterfall.DoubleClick += new System.EventHandler(this.gradientButton_Click);
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
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(127, 106);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "Gradient";
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
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Window";
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
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "View";
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
            // iqTimer
            // 
            this.iqTimer.Enabled = true;
            this.iqTimer.Interval = 500;
            this.iqTimer.Tick += new System.EventHandler(this.iqTimer_Tick);
            // 
            // contrastTrackBar
            // 
            this.contrastTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.contrastTrackBar.Location = new System.Drawing.Point(904, 262);
            this.contrastTrackBar.Maximum = 24;
            this.contrastTrackBar.Minimum = -24;
            this.contrastTrackBar.Name = "contrastTrackBar";
            this.contrastTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.contrastTrackBar.RightToLeftLayout = true;
            this.contrastTrackBar.Size = new System.Drawing.Size(45, 184);
            this.contrastTrackBar.TabIndex = 17;
            this.contrastTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.contrastTrackBar.Scroll += new System.EventHandler(this.contrastTrackBar_Scroll);
            // 
            // zoomTrackBar
            // 
            this.zoomTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomTrackBar.Location = new System.Drawing.Point(904, 58);
            this.zoomTrackBar.Maximum = 50;
            this.zoomTrackBar.Name = "zoomTrackBar";
            this.zoomTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.zoomTrackBar.RightToLeftLayout = true;
            this.zoomTrackBar.Size = new System.Drawing.Size(45, 184);
            this.zoomTrackBar.TabIndex = 18;
            this.zoomTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.zoomTrackBar.Scroll += new System.EventHandler(this.zoomTrackBar_Scroll);
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(909, 42);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(34, 13);
            this.label19.TabIndex = 19;
            this.label19.Text = "Zoom";
            // 
            // label20
            // 
            this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(903, 246);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(46, 13);
            this.label20.TabIndex = 20;
            this.label20.Text = "Contrast";
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Frequency";
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
            this.amRadioButton.CheckedChanged += new System.EventHandler(this.amRadioButton_CheckedChanged);
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
            this.lsbRadioButton.CheckedChanged += new System.EventHandler(this.lsbRadioButton_CheckedChanged);
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
            this.usbRadioButton.CheckedChanged += new System.EventHandler(this.usbRadioButton_CheckedChanged);
            // 
            // filterBandwidthNumericUpDown
            // 
            this.filterBandwidthNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.Location = new System.Drawing.Point(133, 187);
            this.filterBandwidthNumericUpDown.Maximum = new decimal(new int[] {
            180000,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.Name = "filterBandwidthNumericUpDown";
            this.filterBandwidthNumericUpDown.Size = new System.Drawing.Size(72, 20);
            this.filterBandwidthNumericUpDown.TabIndex = 13;
            this.filterBandwidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.filterBandwidthNumericUpDown.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.ValueChanged += new System.EventHandler(this.filterBandwidthNumericUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 189);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Filter bandwidth";
            // 
            // filterOrderNumericUpDown
            // 
            this.filterOrderNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.Location = new System.Drawing.Point(133, 213);
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
            this.filterOrderNumericUpDown.Size = new System.Drawing.Size(72, 20);
            this.filterOrderNumericUpDown.TabIndex = 14;
            this.filterOrderNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.filterOrderNumericUpDown.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.ValueChanged += new System.EventHandler(this.filterOrderNumericUpDown_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(71, 215);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Filter order";
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
            this.filterTypeComboBox.Location = new System.Drawing.Point(70, 160);
            this.filterTypeComboBox.Name = "filterTypeComboBox";
            this.filterTypeComboBox.Size = new System.Drawing.Size(135, 21);
            this.filterTypeComboBox.TabIndex = 12;
            this.filterTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.filterTypeComboBox_SelectedIndexChanged);
            // 
            // swapInQCheckBox
            // 
            this.swapInQCheckBox.AutoSize = true;
            this.swapInQCheckBox.Location = new System.Drawing.Point(126, 291);
            this.swapInQCheckBox.Name = "swapInQCheckBox";
            this.swapInQCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.swapInQCheckBox.Size = new System.Drawing.Size(79, 17);
            this.swapInQCheckBox.TabIndex = 18;
            this.swapInQCheckBox.Text = "Swap I && Q";
            this.swapInQCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.swapInQCheckBox.UseVisualStyleBackColor = true;
            this.swapInQCheckBox.CheckedChanged += new System.EventHandler(this.swapInQCheckBox_CheckedChanged);
            // 
            // centerFreqNumericUpDown
            // 
            this.centerFreqNumericUpDown.CausesValidation = false;
            this.centerFreqNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.centerFreqNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.centerFreqNumericUpDown.Location = new System.Drawing.Point(70, 103);
            this.centerFreqNumericUpDown.Maximum = new decimal(new int[] {
            -1794967296,
            0,
            0,
            0});
            this.centerFreqNumericUpDown.Name = "centerFreqNumericUpDown";
            this.centerFreqNumericUpDown.Size = new System.Drawing.Size(135, 24);
            this.centerFreqNumericUpDown.TabIndex = 9;
            this.centerFreqNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.centerFreqNumericUpDown.ThousandsSeparator = true;
            this.centerFreqNumericUpDown.ValueChanged += new System.EventHandler(this.centerFreqNumericUpDown_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 109);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Center";
            // 
            // correctIQCheckBox
            // 
            this.correctIQCheckBox.AutoSize = true;
            this.correctIQCheckBox.Checked = true;
            this.correctIQCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.correctIQCheckBox.Location = new System.Drawing.Point(45, 291);
            this.correctIQCheckBox.Name = "correctIQCheckBox";
            this.correctIQCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.correctIQCheckBox.Size = new System.Drawing.Size(74, 17);
            this.correctIQCheckBox.TabIndex = 17;
            this.correctIQCheckBox.Text = "Correct IQ";
            this.correctIQCheckBox.UseVisualStyleBackColor = true;
            this.correctIQCheckBox.CheckedChanged += new System.EventHandler(this.autoCorrectIQCheckBox_CheckedChanged);
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
            this.nfmRadioButton.CheckedChanged += new System.EventHandler(this.nfmRadioButton_CheckedChanged);
            // 
            // frontEndComboBox
            // 
            this.frontEndComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frontEndComboBox.FormattingEnabled = true;
            this.frontEndComboBox.Location = new System.Drawing.Point(70, 133);
            this.frontEndComboBox.Name = "frontEndComboBox";
            this.frontEndComboBox.Size = new System.Drawing.Size(135, 21);
            this.frontEndComboBox.TabIndex = 10;
            this.frontEndComboBox.SelectedIndexChanged += new System.EventHandler(this.frontEndComboBox_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(7, 163);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(52, 13);
            this.label16.TabIndex = 26;
            this.label16.Text = "Filter type";
            // 
            // fmSquelchNumericUpDown
            // 
            this.fmSquelchNumericUpDown.Enabled = false;
            this.fmSquelchNumericUpDown.Location = new System.Drawing.Point(133, 239);
            this.fmSquelchNumericUpDown.Name = "fmSquelchNumericUpDown";
            this.fmSquelchNumericUpDown.Size = new System.Drawing.Size(72, 20);
            this.fmSquelchNumericUpDown.TabIndex = 15;
            this.fmSquelchNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.fmSquelchNumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.fmSquelchNumericUpDown.ValueChanged += new System.EventHandler(this.fmSquelchNumericUpDown_ValueChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(64, 241);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(64, 13);
            this.label17.TabIndex = 28;
            this.label17.Text = "FM Squelch";
            this.label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // stepSizeComboBox
            // 
            this.stepSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stepSizeComboBox.FormattingEnabled = true;
            this.stepSizeComboBox.Items.AddRange(new object[] {
            "1 Hz",
            "10 Hz",
            "100 Hz",
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
            "100 kHz"});
            this.stepSizeComboBox.Location = new System.Drawing.Point(133, 265);
            this.stepSizeComboBox.Name = "stepSizeComboBox";
            this.stepSizeComboBox.Size = new System.Drawing.Size(72, 21);
            this.stepSizeComboBox.TabIndex = 16;
            this.stepSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.stepSizeComboBox_SelectedIndexChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(78, 269);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 13);
            this.label18.TabIndex = 30;
            this.label18.Text = "Step size";
            this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // frontendGuiButton
            // 
            this.frontendGuiButton.FlatAppearance.BorderSize = 0;
            this.frontendGuiButton.Location = new System.Drawing.Point(6, 133);
            this.frontendGuiButton.Name = "frontendGuiButton";
            this.frontendGuiButton.Size = new System.Drawing.Size(61, 21);
            this.frontendGuiButton.TabIndex = 11;
            this.frontendGuiButton.Text = "Front end";
            this.frontendGuiButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.frontendGuiButton.UseVisualStyleBackColor = true;
            this.frontendGuiButton.Click += new System.EventHandler(this.frontendGuiButton_Click);
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
            this.wfmRadioButton.CheckedChanged += new System.EventHandler(this.wfmRadioButton_CheckedChanged);
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
            this.dsbRadioButton.CheckedChanged += new System.EventHandler(this.dsbRadioButton_CheckedChanged);
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
            this.cwlRadioButton.CheckedChanged += new System.EventHandler(this.cwlRadioButton_CheckedChanged);
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
            this.cwuRadioButton.CheckedChanged += new System.EventHandler(this.cwuRadioButton_CheckedChanged);
            // 
            // radioCollapsiblePanel
            // 
            this.radioCollapsiblePanel.Controls.Add(this.cwuRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.cwlRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.nfmRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.dsbRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.frequencyNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.wfmRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.label2);
            this.radioCollapsiblePanel.Controls.Add(this.frontendGuiButton);
            this.radioCollapsiblePanel.Controls.Add(this.amRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.label18);
            this.radioCollapsiblePanel.Controls.Add(this.lsbRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.stepSizeComboBox);
            this.radioCollapsiblePanel.Controls.Add(this.usbRadioButton);
            this.radioCollapsiblePanel.Controls.Add(this.label17);
            this.radioCollapsiblePanel.Controls.Add(this.filterBandwidthNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.fmSquelchNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.label1);
            this.radioCollapsiblePanel.Controls.Add(this.label16);
            this.radioCollapsiblePanel.Controls.Add(this.filterOrderNumericUpDown);
            this.radioCollapsiblePanel.Controls.Add(this.frontEndComboBox);
            this.radioCollapsiblePanel.Controls.Add(this.label5);
            this.radioCollapsiblePanel.Controls.Add(this.filterTypeComboBox);
            this.radioCollapsiblePanel.Controls.Add(this.correctIQCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.swapInQCheckBox);
            this.radioCollapsiblePanel.Controls.Add(this.label9);
            this.radioCollapsiblePanel.Controls.Add(this.centerFreqNumericUpDown);
            this.radioCollapsiblePanel.ExpandedHeight = 291;
            this.radioCollapsiblePanel.Location = new System.Drawing.Point(12, 42);
            this.radioCollapsiblePanel.Name = "radioCollapsiblePanel";
            this.radioCollapsiblePanel.NextPanel = this.audioCollapsiblePanel;
            this.radioCollapsiblePanel.PanelTitle = "Radio";
            this.radioCollapsiblePanel.Size = new System.Drawing.Size(211, 311);
            this.radioCollapsiblePanel.TabIndex = 21;
            // 
            // audioCollapsiblePanel
            // 
            this.audioCollapsiblePanel.Controls.Add(this.latencyNumericUpDown);
            this.audioCollapsiblePanel.Controls.Add(this.label6);
            this.audioCollapsiblePanel.Controls.Add(this.label13);
            this.audioCollapsiblePanel.Controls.Add(this.sampleRateComboBox);
            this.audioCollapsiblePanel.Controls.Add(this.audioGainNumericUpDown);
            this.audioCollapsiblePanel.Controls.Add(this.label12);
            this.audioCollapsiblePanel.Controls.Add(this.label3);
            this.audioCollapsiblePanel.Controls.Add(this.outputDeviceComboBox);
            this.audioCollapsiblePanel.Controls.Add(this.inputDeviceComboBox);
            this.audioCollapsiblePanel.Controls.Add(this.label11);
            this.audioCollapsiblePanel.ExpandedHeight = 143;
            this.audioCollapsiblePanel.Location = new System.Drawing.Point(12, 353);
            this.audioCollapsiblePanel.Name = "audioCollapsiblePanel";
            this.audioCollapsiblePanel.NextPanel = this.agcCollapsiblePanel;
            this.audioCollapsiblePanel.PanelTitle = "Audio";
            this.audioCollapsiblePanel.Size = new System.Drawing.Size(211, 163);
            this.audioCollapsiblePanel.TabIndex = 22;
            // 
            // latencyNumericUpDown
            // 
            this.latencyNumericUpDown.Location = new System.Drawing.Point(126, 137);
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 139);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Latency (ms)";
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
            this.agcCollapsiblePanel.Location = new System.Drawing.Point(13, 516);
            this.agcCollapsiblePanel.Name = "agcCollapsiblePanel";
            this.agcCollapsiblePanel.NextPanel = this.displayCollapsiblePanel;
            this.agcCollapsiblePanel.PanelTitle = "AGC";
            this.agcCollapsiblePanel.Size = new System.Drawing.Size(210, 123);
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
            // displayCollapsiblePanel
            // 
            this.displayCollapsiblePanel.Controls.Add(this.label21);
            this.displayCollapsiblePanel.Controls.Add(this.fftResolutionComboBox);
            this.displayCollapsiblePanel.Controls.Add(this.label7);
            this.displayCollapsiblePanel.Controls.Add(this.label14);
            this.displayCollapsiblePanel.Controls.Add(this.viewComboBox);
            this.displayCollapsiblePanel.Controls.Add(this.gradientButton);
            this.displayCollapsiblePanel.Controls.Add(this.fftWindowComboBox);
            this.displayCollapsiblePanel.Controls.Add(this.label8);
            this.displayCollapsiblePanel.ExpandedHeight = 107;
            this.displayCollapsiblePanel.Location = new System.Drawing.Point(13, 639);
            this.displayCollapsiblePanel.Name = "displayCollapsiblePanel";
            this.displayCollapsiblePanel.NextPanel = null;
            this.displayCollapsiblePanel.PanelTitle = "FFT Display";
            this.displayCollapsiblePanel.Size = new System.Drawing.Size(210, 127);
            this.displayCollapsiblePanel.TabIndex = 24;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(961, 778);
            this.Controls.Add(this.displayCollapsiblePanel);
            this.Controls.Add(this.agcCollapsiblePanel);
            this.Controls.Add(this.audioCollapsiblePanel);
            this.Controls.Add(this.radioCollapsiblePanel);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.zoomTrackBar);
            this.Controls.Add(this.contrastTrackBar);
            this.Controls.Add(this.panSplitContainer);
            this.Controls.Add(this.fileSelectButton);
            this.Controls.Add(this.wavFileRadioButton);
            this.Controls.Add(this.soundCardRadioButton);
            this.Controls.Add(this.wavFileTextBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.playButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "MainForm";
            this.Text = "SDR#";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.agcThresholdNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioGainNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcDecayNumericUpDown)).EndInit();
            this.panSplitContainer.Panel1.ResumeLayout(false);
            this.panSplitContainer.Panel2.ResumeLayout(false);
            this.panSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.agcSlopeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterBandwidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterOrderNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerFreqNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fmSquelchNumericUpDown)).EndInit();
            this.radioCollapsiblePanel.ResumeLayout(false);
            this.radioCollapsiblePanel.PerformLayout();
            this.audioCollapsiblePanel.ResumeLayout(false);
            this.audioCollapsiblePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.latencyNumericUpDown)).EndInit();
            this.agcCollapsiblePanel.ResumeLayout(false);
            this.agcCollapsiblePanel.PerformLayout();
            this.displayCollapsiblePanel.ResumeLayout(false);
            this.displayCollapsiblePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.IContainer components;
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
        private Label label17;
        private Label label1;
        private NumericUpDown fmSquelchNumericUpDown;
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
    }
}
