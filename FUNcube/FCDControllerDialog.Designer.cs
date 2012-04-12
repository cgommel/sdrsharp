namespace FUNCubeDongleController
{
    partial class FCDControllerDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FCDControllerDialog));
            this.IFGainModeLabel = new System.Windows.Forms.Label();
            this.BiasCurrentLabel = new System.Windows.Forms.Label();
            this.IFGainModeComboBox = new System.Windows.Forms.ComboBox();
            this.BiasCurrentComboBox = new System.Windows.Forms.ComboBox();
            this.BandLabel = new System.Windows.Forms.Label();
            this.LNAEnhanceLabel = new System.Windows.Forms.Label();
            this.BandComboBox = new System.Windows.Forms.ComboBox();
            this.LNAEnhanceComboBox = new System.Windows.Forms.ComboBox();
            this.DefaultButton = new System.Windows.Forms.Button();
            this.IFGain6ComboBox = new System.Windows.Forms.ComboBox();
            this.IFGain6Label = new System.Windows.Forms.Label();
            this.RFFilterLabel = new System.Windows.Forms.Label();
            this.IFGain5Label = new System.Windows.Forms.Label();
            this.IFGain5ComboBox = new System.Windows.Forms.ComboBox();
            this.LNAGainLabel = new System.Windows.Forms.Label();
            this.IFFilterLabel = new System.Windows.Forms.Label();
            this.IFFilterComboBox = new System.Windows.Forms.ComboBox();
            this.RFFilterComboBox = new System.Windows.Forms.ComboBox();
            this.LNAGainComboBox = new System.Windows.Forms.ComboBox();
            this.IFGain4Label = new System.Windows.Forms.Label();
            this.IFGain3Label = new System.Windows.Forms.Label();
            this.IFRCFilterComboBox = new System.Windows.Forms.ComboBox();
            this.IFRCFilterLabel = new System.Windows.Forms.Label();
            this.IFGain2Label = new System.Windows.Forms.Label();
            this.IFGain1Label = new System.Windows.Forms.Label();
            this.IFGain1ComboBox = new System.Windows.Forms.ComboBox();
            this.IFGain4ComboBox = new System.Windows.Forms.ComboBox();
            this.MixerFilterLabel = new System.Windows.Forms.Label();
            this.IFGain3ComboBox = new System.Windows.Forms.ComboBox();
            this.mixerFilterComboBox = new System.Windows.Forms.ComboBox();
            this.MixerGainLabel = new System.Windows.Forms.Label();
            this.IFGain2ComboBox = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mixerGainComboBox = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // IFGainModeLabel
            // 
            this.IFGainModeLabel.AutoSize = true;
            this.IFGainModeLabel.Location = new System.Drawing.Point(249, 170);
            this.IFGainModeLabel.Name = "IFGainModeLabel";
            this.IFGainModeLabel.Size = new System.Drawing.Size(68, 13);
            this.IFGainModeLabel.TabIndex = 119;
            this.IFGainModeLabel.Text = "IF gain mode";
            // 
            // BiasCurrentLabel
            // 
            this.BiasCurrentLabel.AutoSize = true;
            this.BiasCurrentLabel.Location = new System.Drawing.Point(153, 170);
            this.BiasCurrentLabel.Name = "BiasCurrentLabel";
            this.BiasCurrentLabel.Size = new System.Drawing.Size(63, 13);
            this.BiasCurrentLabel.TabIndex = 118;
            this.BiasCurrentLabel.Text = "Bias current";
            // 
            // IFGainModeComboBox
            // 
            this.IFGainModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IFGainModeComboBox.FormattingEnabled = true;
            this.IFGainModeComboBox.Items.AddRange(new object[] {
            "Linearity",
            "Sensitivity"});
            this.IFGainModeComboBox.Location = new System.Drawing.Point(252, 188);
            this.IFGainModeComboBox.Name = "IFGainModeComboBox";
            this.IFGainModeComboBox.Size = new System.Drawing.Size(91, 21);
            this.IFGainModeComboBox.TabIndex = 117;
            this.IFGainModeComboBox.SelectedIndexChanged += new System.EventHandler(this.IFGainModeComboBox_SelectedIndexChanged);
            // 
            // BiasCurrentComboBox
            // 
            this.BiasCurrentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BiasCurrentComboBox.FormattingEnabled = true;
            this.BiasCurrentComboBox.Items.AddRange(new object[] {
            "00 L band",
            "01",
            "10",
            "11 V/U band"});
            this.BiasCurrentComboBox.Location = new System.Drawing.Point(156, 188);
            this.BiasCurrentComboBox.Name = "BiasCurrentComboBox";
            this.BiasCurrentComboBox.Size = new System.Drawing.Size(90, 21);
            this.BiasCurrentComboBox.TabIndex = 116;
            this.BiasCurrentComboBox.SelectedIndexChanged += new System.EventHandler(this.BiasCurrentComboBox_SelectedIndexChanged);
            // 
            // BandLabel
            // 
            this.BandLabel.AutoSize = true;
            this.BandLabel.Location = new System.Drawing.Point(90, 170);
            this.BandLabel.Name = "BandLabel";
            this.BandLabel.Size = new System.Drawing.Size(32, 13);
            this.BandLabel.TabIndex = 115;
            this.BandLabel.Text = "Band";
            // 
            // LNAEnhanceLabel
            // 
            this.LNAEnhanceLabel.AutoSize = true;
            this.LNAEnhanceLabel.Location = new System.Drawing.Point(14, 170);
            this.LNAEnhanceLabel.Name = "LNAEnhanceLabel";
            this.LNAEnhanceLabel.Size = new System.Drawing.Size(73, 13);
            this.LNAEnhanceLabel.TabIndex = 114;
            this.LNAEnhanceLabel.Text = "LNA enhance";
            // 
            // BandComboBox
            // 
            this.BandComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BandComboBox.Enabled = false;
            this.BandComboBox.FormattingEnabled = true;
            this.BandComboBox.Items.AddRange(new object[] {
            "VHF II",
            "VHF III",
            "UHF",
            "LBAND"});
            this.BandComboBox.Location = new System.Drawing.Point(93, 188);
            this.BandComboBox.Name = "BandComboBox";
            this.BandComboBox.Size = new System.Drawing.Size(57, 21);
            this.BandComboBox.TabIndex = 113;
            // 
            // LNAEnhanceComboBox
            // 
            this.LNAEnhanceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LNAEnhanceComboBox.FormattingEnabled = true;
            this.LNAEnhanceComboBox.Items.AddRange(new object[] {
            "Off",
            "0",
            "1",
            "2",
            "3"});
            this.LNAEnhanceComboBox.Location = new System.Drawing.Point(17, 188);
            this.LNAEnhanceComboBox.Name = "LNAEnhanceComboBox";
            this.LNAEnhanceComboBox.Size = new System.Drawing.Size(70, 21);
            this.LNAEnhanceComboBox.TabIndex = 112;
            this.LNAEnhanceComboBox.SelectedIndexChanged += new System.EventHandler(this.LNAEnhanceComboBox_SelectedIndexChanged);
            // 
            // DefaultButton
            // 
            this.DefaultButton.Location = new System.Drawing.Point(458, 186);
            this.DefaultButton.Name = "DefaultButton";
            this.DefaultButton.Size = new System.Drawing.Size(75, 23);
            this.DefaultButton.TabIndex = 111;
            this.DefaultButton.Text = "Default";
            this.DefaultButton.UseVisualStyleBackColor = true;
            this.DefaultButton.Click += new System.EventHandler(this.DefaultButton_Click);
            // 
            // IFGain6ComboBox
            // 
            this.IFGain6ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IFGain6ComboBox.FormattingEnabled = true;
            this.IFGain6ComboBox.Items.AddRange(new object[] {
            "+3 dB",
            "+6 dB",
            "+9 dB",
            "+12 dB",
            "+15 dB"});
            this.IFGain6ComboBox.Location = new System.Drawing.Point(1016, 136);
            this.IFGain6ComboBox.Name = "IFGain6ComboBox";
            this.IFGain6ComboBox.Size = new System.Drawing.Size(74, 21);
            this.IFGain6ComboBox.TabIndex = 97;
            this.IFGain6ComboBox.SelectedIndexChanged += new System.EventHandler(this.IFGain6ComboBox_SelectedIndexChanged);
            // 
            // IFGain6Label
            // 
            this.IFGain6Label.AutoSize = true;
            this.IFGain6Label.Location = new System.Drawing.Point(1013, 119);
            this.IFGain6Label.Name = "IFGain6Label";
            this.IFGain6Label.Size = new System.Drawing.Size(48, 13);
            this.IFGain6Label.TabIndex = 102;
            this.IFGain6Label.Text = "IF gain 6";
            // 
            // RFFilterLabel
            // 
            this.RFFilterLabel.AutoSize = true;
            this.RFFilterLabel.Location = new System.Drawing.Point(94, 119);
            this.RFFilterLabel.Name = "RFFilterLabel";
            this.RFFilterLabel.Size = new System.Drawing.Size(43, 13);
            this.RFFilterLabel.TabIndex = 110;
            this.RFFilterLabel.Text = "RF filter";
            // 
            // IFGain5Label
            // 
            this.IFGain5Label.AutoSize = true;
            this.IFGain5Label.Location = new System.Drawing.Point(933, 119);
            this.IFGain5Label.Name = "IFGain5Label";
            this.IFGain5Label.Size = new System.Drawing.Size(48, 13);
            this.IFGain5Label.TabIndex = 100;
            this.IFGain5Label.Text = "IF gain 5";
            // 
            // IFGain5ComboBox
            // 
            this.IFGain5ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IFGain5ComboBox.FormattingEnabled = true;
            this.IFGain5ComboBox.Items.AddRange(new object[] {
            "+3 dB",
            "+6 dB",
            "+9 dB",
            "+12 dB",
            "+15 dB"});
            this.IFGain5ComboBox.Location = new System.Drawing.Point(936, 136);
            this.IFGain5ComboBox.Name = "IFGain5ComboBox";
            this.IFGain5ComboBox.Size = new System.Drawing.Size(74, 21);
            this.IFGain5ComboBox.TabIndex = 95;
            this.IFGain5ComboBox.SelectedIndexChanged += new System.EventHandler(this.IFGain5ComboBox_SelectedIndexChanged);
            // 
            // LNAGainLabel
            // 
            this.LNAGainLabel.AutoSize = true;
            this.LNAGainLabel.Location = new System.Drawing.Point(14, 119);
            this.LNAGainLabel.Name = "LNAGainLabel";
            this.LNAGainLabel.Size = new System.Drawing.Size(51, 13);
            this.LNAGainLabel.TabIndex = 109;
            this.LNAGainLabel.Text = "LNA gain";
            // 
            // IFFilterLabel
            // 
            this.IFFilterLabel.AutoSize = true;
            this.IFFilterLabel.Location = new System.Drawing.Point(835, 119);
            this.IFFilterLabel.Name = "IFFilterLabel";
            this.IFFilterLabel.Size = new System.Drawing.Size(38, 13);
            this.IFFilterLabel.TabIndex = 98;
            this.IFFilterLabel.Text = "IF filter";
            // 
            // IFFilterComboBox
            // 
            this.IFFilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IFFilterComboBox.FormattingEnabled = true;
            this.IFFilterComboBox.Items.AddRange(new object[] {
            "2.15 MHz",
            "2.20 MHz",
            "2.24 MHz",
            "2.28 MHz",
            "2.30 MHz",
            "2.40 MHz",
            "2.45 MHz",
            "2.50 MHz",
            "2.55 MHz",
            "2.60 MHz",
            "2.70 MHz",
            "2.75 MHz",
            "2.80 MHz",
            "2.90 MHz",
            "2.95 MHz",
            "3.00 MHz",
            "3.10 MHz",
            "3.20 MHz",
            "3.30 MHz",
            "3.40 MHz",
            "3.60 MHz",
            "3.70 MHz",
            "3.80 MHz",
            "3.90 MHz",
            "4.10 MHz",
            "4.30 MHz",
            "4.40 MHz",
            "4.60 MHz",
            "4.80 MHz",
            "5.00 MHz",
            "5.30 MHz",
            "5.50 MHz"});
            this.IFFilterComboBox.Location = new System.Drawing.Point(838, 136);
            this.IFFilterComboBox.Name = "IFFilterComboBox";
            this.IFFilterComboBox.Size = new System.Drawing.Size(92, 21);
            this.IFFilterComboBox.TabIndex = 96;
            this.IFFilterComboBox.SelectedIndexChanged += new System.EventHandler(this.IFFilterComboBox_SelectedIndexChanged);
            // 
            // RFFilterComboBox
            // 
            this.RFFilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RFFilterComboBox.FormattingEnabled = true;
            this.RFFilterComboBox.Items.AddRange(new object[] {
            "268 MHz LPF",
            "299 MHz LPF"});
            this.RFFilterComboBox.Location = new System.Drawing.Point(97, 136);
            this.RFFilterComboBox.Name = "RFFilterComboBox";
            this.RFFilterComboBox.Size = new System.Drawing.Size(117, 21);
            this.RFFilterComboBox.TabIndex = 108;
            this.RFFilterComboBox.SelectedIndexChanged += new System.EventHandler(this.RFFilterComboBox_SelectedIndexChanged);
            // 
            // LNAGainComboBox
            // 
            this.LNAGainComboBox.DisplayMember = "1";
            this.LNAGainComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LNAGainComboBox.FormattingEnabled = true;
            this.LNAGainComboBox.Items.AddRange(new object[] {
            "-5.0 dB",
            "-2.5 dB",
            "+0.0 dB",
            "+2.5 dB",
            "+5.0 dB",
            "+7.5 dB",
            "+10.0 dB",
            "+12.5 dB",
            "+15.0 dB",
            "+17.5 dB",
            "+20.0 dB",
            "+25.0 dB",
            "+30.0 dB"});
            this.LNAGainComboBox.Location = new System.Drawing.Point(17, 136);
            this.LNAGainComboBox.Name = "LNAGainComboBox";
            this.LNAGainComboBox.Size = new System.Drawing.Size(74, 21);
            this.LNAGainComboBox.TabIndex = 107;
            this.LNAGainComboBox.SelectedIndexChanged += new System.EventHandler(this.LNAGainComboBox_SelectedIndexChanged);
            // 
            // IFGain4Label
            // 
            this.IFGain4Label.AutoSize = true;
            this.IFGain4Label.Location = new System.Drawing.Point(755, 119);
            this.IFGain4Label.Name = "IFGain4Label";
            this.IFGain4Label.Size = new System.Drawing.Size(48, 13);
            this.IFGain4Label.TabIndex = 106;
            this.IFGain4Label.Text = "IF gain 4";
            // 
            // IFGain3Label
            // 
            this.IFGain3Label.AutoSize = true;
            this.IFGain3Label.Location = new System.Drawing.Point(675, 119);
            this.IFGain3Label.Name = "IFGain3Label";
            this.IFGain3Label.Size = new System.Drawing.Size(48, 13);
            this.IFGain3Label.TabIndex = 105;
            this.IFGain3Label.Text = "IF gain 3";
            // 
            // IFRCFilterComboBox
            // 
            this.IFRCFilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IFRCFilterComboBox.FormattingEnabled = true;
            this.IFRCFilterComboBox.Items.AddRange(new object[] {
            "1.0 MHz",
            "1.2 MHz",
            "1.8 MHz",
            "2.6 MHz",
            "3.4 MHz",
            "4.4 MHz",
            "5.3 MHz",
            "6.4 MHz",
            "7.7 MHz",
            "9.0 MHz",
            "10.6 MHz",
            "12.4 MHz",
            "14.7 MHz",
            "17.6 MHz",
            "21.0 MHz",
            "21.4 MHz"});
            this.IFRCFilterComboBox.Location = new System.Drawing.Point(494, 136);
            this.IFRCFilterComboBox.Name = "IFRCFilterComboBox";
            this.IFRCFilterComboBox.Size = new System.Drawing.Size(98, 21);
            this.IFRCFilterComboBox.TabIndex = 90;
            this.IFRCFilterComboBox.SelectedIndexChanged += new System.EventHandler(this.IFRCFilterComboBox_SelectedIndexChanged);
            // 
            // IFRCFilterLabel
            // 
            this.IFRCFilterLabel.AutoSize = true;
            this.IFRCFilterLabel.Location = new System.Drawing.Point(491, 119);
            this.IFRCFilterLabel.Name = "IFRCFilterLabel";
            this.IFRCFilterLabel.Size = new System.Drawing.Size(56, 13);
            this.IFRCFilterLabel.TabIndex = 94;
            this.IFRCFilterLabel.Text = "IF RC filter";
            // 
            // IFGain2Label
            // 
            this.IFGain2Label.AutoSize = true;
            this.IFGain2Label.Location = new System.Drawing.Point(598, 119);
            this.IFGain2Label.Name = "IFGain2Label";
            this.IFGain2Label.Size = new System.Drawing.Size(48, 13);
            this.IFGain2Label.TabIndex = 104;
            this.IFGain2Label.Text = "IF gain 2";
            // 
            // IFGain1Label
            // 
            this.IFGain1Label.AutoSize = true;
            this.IFGain1Label.Location = new System.Drawing.Point(411, 119);
            this.IFGain1Label.Name = "IFGain1Label";
            this.IFGain1Label.Size = new System.Drawing.Size(48, 13);
            this.IFGain1Label.TabIndex = 93;
            this.IFGain1Label.Text = "IF gain 1";
            // 
            // IFGain1ComboBox
            // 
            this.IFGain1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IFGain1ComboBox.FormattingEnabled = true;
            this.IFGain1ComboBox.Items.AddRange(new object[] {
            "-3 dB",
            "+6 dB"});
            this.IFGain1ComboBox.Location = new System.Drawing.Point(414, 136);
            this.IFGain1ComboBox.Name = "IFGain1ComboBox";
            this.IFGain1ComboBox.Size = new System.Drawing.Size(74, 21);
            this.IFGain1ComboBox.TabIndex = 89;
            this.IFGain1ComboBox.SelectedIndexChanged += new System.EventHandler(this.IFGain1ComboBox_SelectedIndexChanged);
            // 
            // IFGain4ComboBox
            // 
            this.IFGain4ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IFGain4ComboBox.FormattingEnabled = true;
            this.IFGain4ComboBox.Items.AddRange(new object[] {
            "0 dB",
            "+1 dB",
            "+2 dB"});
            this.IFGain4ComboBox.Location = new System.Drawing.Point(758, 136);
            this.IFGain4ComboBox.Name = "IFGain4ComboBox";
            this.IFGain4ComboBox.Size = new System.Drawing.Size(74, 21);
            this.IFGain4ComboBox.TabIndex = 103;
            this.IFGain4ComboBox.SelectedIndexChanged += new System.EventHandler(this.IFGain4ComboBox_SelectedIndexChanged);
            // 
            // MixerFilterLabel
            // 
            this.MixerFilterLabel.AutoSize = true;
            this.MixerFilterLabel.Location = new System.Drawing.Point(297, 119);
            this.MixerFilterLabel.Name = "MixerFilterLabel";
            this.MixerFilterLabel.Size = new System.Drawing.Size(54, 13);
            this.MixerFilterLabel.TabIndex = 92;
            this.MixerFilterLabel.Text = "Mixer filter";
            // 
            // IFGain3ComboBox
            // 
            this.IFGain3ComboBox.DisplayMember = "1";
            this.IFGain3ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IFGain3ComboBox.FormattingEnabled = true;
            this.IFGain3ComboBox.Items.AddRange(new object[] {
            "0 dB",
            "+3 dB",
            "+6 dB",
            "+9 dB"});
            this.IFGain3ComboBox.Location = new System.Drawing.Point(678, 136);
            this.IFGain3ComboBox.Name = "IFGain3ComboBox";
            this.IFGain3ComboBox.Size = new System.Drawing.Size(74, 21);
            this.IFGain3ComboBox.TabIndex = 101;
            this.IFGain3ComboBox.SelectedIndexChanged += new System.EventHandler(this.IFGain3ComboBox_SelectedIndexChanged);
            // 
            // mixerFilterComboBox
            // 
            this.mixerFilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mixerFilterComboBox.FormattingEnabled = true;
            this.mixerFilterComboBox.Items.AddRange(new object[] {
            "1.9 MHz",
            "2.3 MHz",
            "2.7 MHz",
            "3.0 MHz",
            "3.4 MHz",
            "3.8 MHz",
            "4.2 MHz",
            "4.6 MHz",
            "27 MHz"});
            this.mixerFilterComboBox.Location = new System.Drawing.Point(300, 136);
            this.mixerFilterComboBox.Name = "mixerFilterComboBox";
            this.mixerFilterComboBox.Size = new System.Drawing.Size(108, 21);
            this.mixerFilterComboBox.TabIndex = 88;
            this.mixerFilterComboBox.SelectedIndexChanged += new System.EventHandler(this.mixerFilterComboBox_SelectedIndexChanged);
            // 
            // MixerGainLabel
            // 
            this.MixerGainLabel.AutoSize = true;
            this.MixerGainLabel.Location = new System.Drawing.Point(217, 119);
            this.MixerGainLabel.Name = "MixerGainLabel";
            this.MixerGainLabel.Size = new System.Drawing.Size(55, 13);
            this.MixerGainLabel.TabIndex = 91;
            this.MixerGainLabel.Text = "Mixer gain";
            // 
            // IFGain2ComboBox
            // 
            this.IFGain2ComboBox.DisplayMember = "1";
            this.IFGain2ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IFGain2ComboBox.FormattingEnabled = true;
            this.IFGain2ComboBox.Items.AddRange(new object[] {
            "0 dB",
            "+3 dB",
            "+6 dB",
            "+9 dB"});
            this.IFGain2ComboBox.Location = new System.Drawing.Point(598, 136);
            this.IFGain2ComboBox.Name = "IFGain2ComboBox";
            this.IFGain2ComboBox.Size = new System.Drawing.Size(74, 21);
            this.IFGain2ComboBox.TabIndex = 99;
            this.IFGain2ComboBox.SelectedIndexChanged += new System.EventHandler(this.IFGain2ComboBox_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(7, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1093, 100);
            this.pictureBox1.TabIndex = 86;
            this.pictureBox1.TabStop = false;
            // 
            // mixerGainComboBox
            // 
            this.mixerGainComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mixerGainComboBox.FormattingEnabled = true;
            this.mixerGainComboBox.Items.AddRange(new object[] {
            "4 dB",
            "12 dB"});
            this.mixerGainComboBox.Location = new System.Drawing.Point(220, 136);
            this.mixerGainComboBox.Name = "mixerGainComboBox";
            this.mixerGainComboBox.Size = new System.Drawing.Size(74, 21);
            this.mixerGainComboBox.TabIndex = 87;
            this.mixerGainComboBox.SelectedIndexChanged += new System.EventHandler(this.mixerGainComboBox_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FCDControllerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 216);
            this.Controls.Add(this.IFGainModeLabel);
            this.Controls.Add(this.BiasCurrentLabel);
            this.Controls.Add(this.IFGainModeComboBox);
            this.Controls.Add(this.BiasCurrentComboBox);
            this.Controls.Add(this.BandLabel);
            this.Controls.Add(this.LNAEnhanceLabel);
            this.Controls.Add(this.BandComboBox);
            this.Controls.Add(this.LNAEnhanceComboBox);
            this.Controls.Add(this.DefaultButton);
            this.Controls.Add(this.IFGain6ComboBox);
            this.Controls.Add(this.IFGain6Label);
            this.Controls.Add(this.RFFilterLabel);
            this.Controls.Add(this.IFGain5Label);
            this.Controls.Add(this.IFGain5ComboBox);
            this.Controls.Add(this.LNAGainLabel);
            this.Controls.Add(this.IFFilterLabel);
            this.Controls.Add(this.IFFilterComboBox);
            this.Controls.Add(this.RFFilterComboBox);
            this.Controls.Add(this.LNAGainComboBox);
            this.Controls.Add(this.IFGain4Label);
            this.Controls.Add(this.IFGain3Label);
            this.Controls.Add(this.IFRCFilterComboBox);
            this.Controls.Add(this.IFRCFilterLabel);
            this.Controls.Add(this.IFGain2Label);
            this.Controls.Add(this.IFGain1Label);
            this.Controls.Add(this.IFGain1ComboBox);
            this.Controls.Add(this.IFGain4ComboBox);
            this.Controls.Add(this.MixerFilterLabel);
            this.Controls.Add(this.IFGain3ComboBox);
            this.Controls.Add(this.mixerFilterComboBox);
            this.Controls.Add(this.MixerGainLabel);
            this.Controls.Add(this.IFGain2ComboBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.mixerGainComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FCDControllerDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FUNCube Dongle Controller";
            this.Load += new System.EventHandler(this.FCDControllerDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label IFGainModeLabel;
        private System.Windows.Forms.Label BiasCurrentLabel;
        private System.Windows.Forms.ComboBox IFGainModeComboBox;
        private System.Windows.Forms.ComboBox BiasCurrentComboBox;
        private System.Windows.Forms.Label BandLabel;
        private System.Windows.Forms.Label LNAEnhanceLabel;
        private System.Windows.Forms.ComboBox BandComboBox;
        private System.Windows.Forms.ComboBox LNAEnhanceComboBox;
        private System.Windows.Forms.Button DefaultButton;
        private System.Windows.Forms.ComboBox IFGain6ComboBox;
        private System.Windows.Forms.Label IFGain6Label;
        private System.Windows.Forms.Label RFFilterLabel;
        private System.Windows.Forms.Label IFGain5Label;
        private System.Windows.Forms.ComboBox IFGain5ComboBox;
        private System.Windows.Forms.Label LNAGainLabel;
        private System.Windows.Forms.Label IFFilterLabel;
        private System.Windows.Forms.ComboBox IFFilterComboBox;
        private System.Windows.Forms.ComboBox RFFilterComboBox;
        private System.Windows.Forms.ComboBox LNAGainComboBox;
        private System.Windows.Forms.Label IFGain4Label;
        private System.Windows.Forms.Label IFGain3Label;
        private System.Windows.Forms.ComboBox IFRCFilterComboBox;
        private System.Windows.Forms.Label IFRCFilterLabel;
        private System.Windows.Forms.Label IFGain2Label;
        private System.Windows.Forms.Label IFGain1Label;
        private System.Windows.Forms.ComboBox IFGain1ComboBox;
        private System.Windows.Forms.ComboBox IFGain4ComboBox;
        private System.Windows.Forms.Label MixerFilterLabel;
        private System.Windows.Forms.ComboBox IFGain3ComboBox;
        private System.Windows.Forms.ComboBox mixerFilterComboBox;
        private System.Windows.Forms.Label MixerGainLabel;
        private System.Windows.Forms.ComboBox IFGain2ComboBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox mixerGainComboBox;
        private System.Windows.Forms.Timer timer1;
    }
}

