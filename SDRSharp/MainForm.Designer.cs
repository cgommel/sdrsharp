using System.Windows.Forms;
using SDRSharp.PanView;

namespace SDRSharp
{
    public partial class MainForm
    {
        private Button playButton;
        private Button stopButton;
        private GroupBox groupBox1;
        private NumericUpDown audioGainNumericUpDown;
        private Label label2;
        private NumericUpDown frequencyNumericUpDown;
        private Label label3;
        private RadioButton usbRadioButton;
        private RadioButton lsbRadioButton;
        private RadioButton amRadioButton;
        private OpenFileDialog openDlg;
        private CheckBox agcCheckBox;
        private Label label1;
        private NumericUpDown filterBandwidthNumericUpDown;
        private Label label4;
        private NumericUpDown agcAttackNumericUpDown;
        private GroupBox groupBox2;
        private CheckBox swapInQCheckBox;
        private Label label5;
        private NumericUpDown filterOrderNumericUpDown;
        private Label label6;
        private ComboBox filterTypeComboBox;
        private SpectrumAnalyzer spectrumAnalyzer;
        private Waterfall waterfall;
        private Label label9;
        private NumericUpDown centerFreqNumericUpDown;
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.frontEndComboBox = new System.Windows.Forms.ComboBox();
            this.fmRadioButton = new System.Windows.Forms.RadioButton();
            this.correctIQCheckBox = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.centerFreqNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.swapInQCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.filterTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.filterOrderNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.filterBandwidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.usbRadioButton = new System.Windows.Forms.RadioButton();
            this.lsbRadioButton = new System.Windows.Forms.RadioButton();
            this.amRadioButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.frequencyNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.agcAttackNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.agcCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.audioGainNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bufferSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
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
            this.displayTimer = new System.Windows.Forms.Timer(this.components);
            this.highDefinitionCheckBox = new System.Windows.Forms.CheckBox();
            this.panSplitContainer = new System.Windows.Forms.SplitContainer();
            this.spectrumAnalyzer = new SDRSharp.PanView.SpectrumAnalyzer();
            this.waterfall = new SDRSharp.PanView.Waterfall();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.gradientButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.fftWindowComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.viewComboBox = new System.Windows.Forms.ComboBox();
            this.iqTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.centerFreqNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterOrderNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterBandwidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcAttackNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioGainNumericUpDown)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bufferSizeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcDecayNumericUpDown)).BeginInit();
            this.panSplitContainer.Panel1.SuspendLayout();
            this.panSplitContainer.Panel2.SuspendLayout();
            this.panSplitContainer.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.frontEndComboBox);
            this.groupBox1.Controls.Add(this.fmRadioButton);
            this.groupBox1.Controls.Add(this.correctIQCheckBox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.centerFreqNumericUpDown);
            this.groupBox1.Controls.Add(this.swapInQCheckBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.filterTypeComboBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.filterOrderNumericUpDown);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.filterBandwidthNumericUpDown);
            this.groupBox1.Controls.Add(this.usbRadioButton);
            this.groupBox1.Controls.Add(this.lsbRadioButton);
            this.groupBox1.Controls.Add(this.amRadioButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.frequencyNumericUpDown);
            this.groupBox1.Location = new System.Drawing.Point(12, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 263);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Radio settings";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 135);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(52, 13);
            this.label16.TabIndex = 26;
            this.label16.Text = "Filter type";
            // 
            // frontEndComboBox
            // 
            this.frontEndComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frontEndComboBox.FormattingEnabled = true;
            this.frontEndComboBox.Location = new System.Drawing.Point(69, 105);
            this.frontEndComboBox.Name = "frontEndComboBox";
            this.frontEndComboBox.Size = new System.Drawing.Size(135, 21);
            this.frontEndComboBox.TabIndex = 6;
            this.frontEndComboBox.SelectedIndexChanged += new System.EventHandler(this.frontEndComboBox_SelectedIndexChanged);
            // 
            // fmRadioButton
            // 
            this.fmRadioButton.AutoSize = true;
            this.fmRadioButton.Location = new System.Drawing.Point(11, 21);
            this.fmRadioButton.Name = "fmRadioButton";
            this.fmRadioButton.Size = new System.Drawing.Size(40, 17);
            this.fmRadioButton.TabIndex = 0;
            this.fmRadioButton.Text = "FM";
            this.fmRadioButton.UseVisualStyleBackColor = true;
            this.fmRadioButton.CheckedChanged += new System.EventHandler(this.fmRadioButton_CheckedChanged);
            // 
            // correctIQCheckBox
            // 
            this.correctIQCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.correctIQCheckBox.AutoSize = true;
            this.correctIQCheckBox.Checked = true;
            this.correctIQCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.correctIQCheckBox.Location = new System.Drawing.Point(130, 219);
            this.correctIQCheckBox.Name = "correctIQCheckBox";
            this.correctIQCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.correctIQCheckBox.Size = new System.Drawing.Size(74, 17);
            this.correctIQCheckBox.TabIndex = 10;
            this.correctIQCheckBox.Text = "Correct IQ";
            this.correctIQCheckBox.UseVisualStyleBackColor = true;
            this.correctIQCheckBox.CheckedChanged += new System.EventHandler(this.autoCorrectIQCheckBox_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Center";
            // 
            // centerFreqNumericUpDown
            // 
            this.centerFreqNumericUpDown.CausesValidation = false;
            this.centerFreqNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.centerFreqNumericUpDown.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.centerFreqNumericUpDown.Location = new System.Drawing.Point(69, 75);
            this.centerFreqNumericUpDown.Maximum = new decimal(new int[] {
            -1794967296,
            0,
            0,
            0});
            this.centerFreqNumericUpDown.Name = "centerFreqNumericUpDown";
            this.centerFreqNumericUpDown.Size = new System.Drawing.Size(135, 24);
            this.centerFreqNumericUpDown.TabIndex = 5;
            this.centerFreqNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.centerFreqNumericUpDown.ThousandsSeparator = true;
            this.centerFreqNumericUpDown.ValueChanged += new System.EventHandler(this.centerFreqNumericUpDown_ValueChanged);
            // 
            // swapInQCheckBox
            // 
            this.swapInQCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.swapInQCheckBox.AutoSize = true;
            this.swapInQCheckBox.Location = new System.Drawing.Point(125, 242);
            this.swapInQCheckBox.Name = "swapInQCheckBox";
            this.swapInQCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.swapInQCheckBox.Size = new System.Drawing.Size(79, 17);
            this.swapInQCheckBox.TabIndex = 12;
            this.swapInQCheckBox.Text = "Swap I && Q";
            this.swapInQCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.swapInQCheckBox.UseVisualStyleBackColor = true;
            this.swapInQCheckBox.CheckedChanged += new System.EventHandler(this.swapInQCheckBox_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Front end";
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
            this.filterTypeComboBox.Location = new System.Drawing.Point(69, 132);
            this.filterTypeComboBox.Name = "filterTypeComboBox";
            this.filterTypeComboBox.Size = new System.Drawing.Size(135, 21);
            this.filterTypeComboBox.TabIndex = 7;
            this.filterTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.filterTypeComboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(95, 191);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Taps";
            // 
            // filterOrderNumericUpDown
            // 
            this.filterOrderNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.Location = new System.Drawing.Point(132, 189);
            this.filterOrderNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.Name = "filterOrderNumericUpDown";
            this.filterOrderNumericUpDown.Size = new System.Drawing.Size(72, 20);
            this.filterOrderNumericUpDown.TabIndex = 9;
            this.filterOrderNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.filterOrderNumericUpDown.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.filterOrderNumericUpDown.ValueChanged += new System.EventHandler(this.filterOrderNumericUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 165);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Filter bandwidth";
            // 
            // filterBandwidthNumericUpDown
            // 
            this.filterBandwidthNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.Location = new System.Drawing.Point(132, 163);
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
            this.filterBandwidthNumericUpDown.TabIndex = 8;
            this.filterBandwidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.filterBandwidthNumericUpDown.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.filterBandwidthNumericUpDown.ValueChanged += new System.EventHandler(this.filterBandwidthNumericUpDown_ValueChanged);
            // 
            // usbRadioButton
            // 
            this.usbRadioButton.AutoSize = true;
            this.usbRadioButton.Location = new System.Drawing.Point(147, 21);
            this.usbRadioButton.Name = "usbRadioButton";
            this.usbRadioButton.Size = new System.Drawing.Size(47, 17);
            this.usbRadioButton.TabIndex = 3;
            this.usbRadioButton.Text = "USB";
            this.usbRadioButton.UseVisualStyleBackColor = true;
            this.usbRadioButton.CheckedChanged += new System.EventHandler(this.usbRadioButton_CheckedChanged);
            // 
            // lsbRadioButton
            // 
            this.lsbRadioButton.AutoSize = true;
            this.lsbRadioButton.Location = new System.Drawing.Point(100, 21);
            this.lsbRadioButton.Name = "lsbRadioButton";
            this.lsbRadioButton.Size = new System.Drawing.Size(45, 17);
            this.lsbRadioButton.TabIndex = 2;
            this.lsbRadioButton.Text = "LSB";
            this.lsbRadioButton.UseVisualStyleBackColor = true;
            this.lsbRadioButton.CheckedChanged += new System.EventHandler(this.lsbRadioButton_CheckedChanged);
            // 
            // amRadioButton
            // 
            this.amRadioButton.AutoSize = true;
            this.amRadioButton.Checked = true;
            this.amRadioButton.Location = new System.Drawing.Point(58, 21);
            this.amRadioButton.Name = "amRadioButton";
            this.amRadioButton.Size = new System.Drawing.Size(41, 17);
            this.amRadioButton.TabIndex = 1;
            this.amRadioButton.TabStop = true;
            this.amRadioButton.Text = "AM";
            this.amRadioButton.UseVisualStyleBackColor = true;
            this.amRadioButton.CheckedChanged += new System.EventHandler(this.amRadioButton_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Frequency";
            // 
            // frequencyNumericUpDown
            // 
            this.frequencyNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frequencyNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.frequencyNumericUpDown.Location = new System.Drawing.Point(69, 45);
            this.frequencyNumericUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.frequencyNumericUpDown.Name = "frequencyNumericUpDown";
            this.frequencyNumericUpDown.Size = new System.Drawing.Size(135, 24);
            this.frequencyNumericUpDown.TabIndex = 4;
            this.frequencyNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.frequencyNumericUpDown.ThousandsSeparator = true;
            this.frequencyNumericUpDown.ValueChanged += new System.EventHandler(this.frequencyNumericUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Attack";
            // 
            // agcAttackNumericUpDown
            // 
            this.agcAttackNumericUpDown.Location = new System.Drawing.Point(69, 19);
            this.agcAttackNumericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.agcAttackNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.agcAttackNumericUpDown.Name = "agcAttackNumericUpDown";
            this.agcAttackNumericUpDown.Size = new System.Drawing.Size(135, 20);
            this.agcAttackNumericUpDown.TabIndex = 1;
            this.agcAttackNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.agcAttackNumericUpDown.Value = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.agcAttackNumericUpDown.ValueChanged += new System.EventHandler(this.agcAttackNumericUpDown_ValueChanged);
            // 
            // agcCheckBox
            // 
            this.agcCheckBox.AutoSize = true;
            this.agcCheckBox.Checked = true;
            this.agcCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.agcCheckBox.Location = new System.Drawing.Point(11, 1);
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
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "AF Gain";
            // 
            // audioGainNumericUpDown
            // 
            this.audioGainNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.audioGainNumericUpDown.Location = new System.Drawing.Point(69, 16);
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bufferSizeNumericUpDown);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.sampleRateComboBox);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.outputDeviceComboBox);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.inputDeviceComboBox);
            this.groupBox2.Controls.Add(this.audioGainNumericUpDown);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 306);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(211, 153);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Audio settings";
            // 
            // bufferSizeNumericUpDown
            // 
            this.bufferSizeNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.bufferSizeNumericUpDown.Location = new System.Drawing.Point(69, 127);
            this.bufferSizeNumericUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.bufferSizeNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.bufferSizeNumericUpDown.Name = "bufferSizeNumericUpDown";
            this.bufferSizeNumericUpDown.Size = new System.Drawing.Size(135, 20);
            this.bufferSizeNumericUpDown.TabIndex = 29;
            this.bufferSizeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.bufferSizeNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 129);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 30;
            this.label15.Text = "Buffer (ms)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 49);
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
            this.sampleRateComboBox.Location = new System.Drawing.Point(69, 46);
            this.sampleRateComboBox.Name = "sampleRateComboBox";
            this.sampleRateComboBox.Size = new System.Drawing.Size(135, 21);
            this.sampleRateComboBox.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 103);
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
            this.outputDeviceComboBox.Location = new System.Drawing.Point(69, 100);
            this.outputDeviceComboBox.Name = "outputDeviceComboBox";
            this.outputDeviceComboBox.Size = new System.Drawing.Size(135, 21);
            this.outputDeviceComboBox.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 76);
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
            this.inputDeviceComboBox.Location = new System.Drawing.Point(69, 73);
            this.inputDeviceComboBox.Name = "inputDeviceComboBox";
            this.inputDeviceComboBox.Size = new System.Drawing.Size(135, 21);
            this.inputDeviceComboBox.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(38, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Decay";
            // 
            // agcDecayNumericUpDown
            // 
            this.agcDecayNumericUpDown.Location = new System.Drawing.Point(69, 44);
            this.agcDecayNumericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.agcDecayNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.agcDecayNumericUpDown.Name = "agcDecayNumericUpDown";
            this.agcDecayNumericUpDown.Size = new System.Drawing.Size(135, 20);
            this.agcDecayNumericUpDown.TabIndex = 2;
            this.agcDecayNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.agcDecayNumericUpDown.Value = new decimal(new int[] {
            50,
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
            // displayTimer
            // 
            this.displayTimer.Enabled = true;
            this.displayTimer.Interval = 50;
            this.displayTimer.Tick += new System.EventHandler(this.displayTimer_Tick);
            // 
            // highDefinitionCheckBox
            // 
            this.highDefinitionCheckBox.AutoSize = true;
            this.highDefinitionCheckBox.Location = new System.Drawing.Point(109, 71);
            this.highDefinitionCheckBox.Name = "highDefinitionCheckBox";
            this.highDefinitionCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.highDefinitionCheckBox.Size = new System.Drawing.Size(95, 17);
            this.highDefinitionCheckBox.TabIndex = 2;
            this.highDefinitionCheckBox.Text = "High Definition";
            this.highDefinitionCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.highDefinitionCheckBox.UseVisualStyleBackColor = true;
            this.highDefinitionCheckBox.CheckedChanged += new System.EventHandler(this.highDefinitionCheckBox_CheckedChanged);
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
            this.panSplitContainer.Size = new System.Drawing.Size(720, 613);
            this.panSplitContainer.SplitterDistance = 232;
            this.panSplitContainer.TabIndex = 13;
            // 
            // spectrumAnalyzer
            // 
            this.spectrumAnalyzer.BandType = SDRSharp.PanView.BandType.Lower;
            this.spectrumAnalyzer.CenterFrequency = 0;
            this.spectrumAnalyzer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spectrumAnalyzer.FilterBandwidth = 0;
            this.spectrumAnalyzer.Frequency = 0;
            this.spectrumAnalyzer.Location = new System.Drawing.Point(0, 0);
            this.spectrumAnalyzer.Name = "spectrumAnalyzer";
            this.spectrumAnalyzer.Offset = 0;
            this.spectrumAnalyzer.Size = new System.Drawing.Size(720, 232);
            this.spectrumAnalyzer.SpectrumWidth = 0;
            this.spectrumAnalyzer.TabIndex = 0;
            this.spectrumAnalyzer.FrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_FrequencyChanged);
            // 
            // waterfall
            // 
            this.waterfall.BandType = SDRSharp.PanView.BandType.Lower;
            this.waterfall.CenterFrequency = 0;
            this.waterfall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waterfall.FilterBandwidth = 0;
            this.waterfall.Frequency = 0;
            this.waterfall.HighDefinition = false;
            this.waterfall.Location = new System.Drawing.Point(0, 0);
            this.waterfall.Name = "waterfall";
            this.waterfall.Offset = 0;
            this.waterfall.Size = new System.Drawing.Size(720, 377);
            this.waterfall.SpectrumWidth = 0;
            this.waterfall.TabIndex = 0;
            this.waterfall.DoubleClick += new System.EventHandler(this.gradientButton_Click);
            this.waterfall.FrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.panview_FrequencyChanged);
            this.waterfall.CenterFrequencyChanged += new SDRSharp.PanView.ManualFrequencyChange(this.waterfall_CenterFrequencyChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.agcCheckBox);
            this.groupBox3.Controls.Add(this.agcAttackNumericUpDown);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.agcDecayNumericUpDown);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new System.Drawing.Point(12, 460);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(211, 71);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.gradientButton);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.fftWindowComboBox);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.viewComboBox);
            this.groupBox4.Controls.Add(this.highDefinitionCheckBox);
            this.groupBox4.Location = new System.Drawing.Point(12, 532);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(211, 122);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Display";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(126, 97);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "Gradient";
            // 
            // gradientButton
            // 
            this.gradientButton.Location = new System.Drawing.Point(179, 92);
            this.gradientButton.Name = "gradientButton";
            this.gradientButton.Size = new System.Drawing.Size(25, 23);
            this.gradientButton.TabIndex = 15;
            this.gradientButton.Text = "...";
            this.gradientButton.UseVisualStyleBackColor = true;
            this.gradientButton.Click += new System.EventHandler(this.gradientButton_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 47);
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
            this.fftWindowComboBox.Location = new System.Drawing.Point(69, 44);
            this.fftWindowComboBox.Name = "fftWindowComboBox";
            this.fftWindowComboBox.Size = new System.Drawing.Size(135, 21);
            this.fftWindowComboBox.TabIndex = 1;
            this.fftWindowComboBox.SelectedIndexChanged += new System.EventHandler(this.fftWindowComboBox_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 22);
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
            this.viewComboBox.Location = new System.Drawing.Point(69, 19);
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
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(961, 664);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.panSplitContainer);
            this.Controls.Add(this.fileSelectButton);
            this.Controls.Add(this.wavFileRadioButton);
            this.Controls.Add(this.soundCardRadioButton);
            this.Controls.Add(this.wavFileTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.playButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "SDR#";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.centerFreqNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterOrderNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filterBandwidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcAttackNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioGainNumericUpDown)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bufferSizeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.agcDecayNumericUpDown)).EndInit();
            this.panSplitContainer.Panel1.ResumeLayout(false);
            this.panSplitContainer.Panel2.ResumeLayout(false);
            this.panSplitContainer.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Timer displayTimer;
        private System.ComponentModel.IContainer components;
        private CheckBox highDefinitionCheckBox;
        private SplitContainer panSplitContainer;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Label label7;
        private ComboBox viewComboBox;
        private Label label8;
        private ComboBox fftWindowComboBox;
        private CheckBox correctIQCheckBox;
        private Timer iqTimer;
        private RadioButton fmRadioButton;
        private Button gradientButton;
        private Label label14;
        private NumericUpDown bufferSizeNumericUpDown;
        private Label label15;
        private Label label16;
        private ComboBox frontEndComboBox;
    }
}
