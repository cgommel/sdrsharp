namespace SDRSharp.HackRF
{
    partial class HackRFControllerDialog
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
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.closeButton = new System.Windows.Forms.Button();
            this.deviceComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lnaGainTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.samplerateComboBox = new System.Windows.Forms.ComboBox();
            this.lnaGainLabel = new System.Windows.Forms.Label();
            this.frequencyCorrectionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.tunerTypeLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.vgaGainTrackBar = new System.Windows.Forms.TrackBar();
            this.vgaGainLabel = new System.Windows.Forms.Label();
            this.externalAmpCb = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.lnaGainTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyCorrectionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vgaGainTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(184, 248);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 8;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // deviceComboBox
            // 
            this.deviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceComboBox.FormattingEnabled = true;
            this.deviceComboBox.Location = new System.Drawing.Point(12, 26);
            this.deviceComboBox.Name = "deviceComboBox";
            this.deviceComboBox.Size = new System.Drawing.Size(247, 21);
            this.deviceComboBox.TabIndex = 0;
            this.deviceComboBox.SelectedIndexChanged += new System.EventHandler(this.deviceComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Device";
            // 
            // lnaGainTrackBar
            // 
            this.lnaGainTrackBar.LargeChange = 1;
            this.lnaGainTrackBar.Location = new System.Drawing.Point(2, 113);
            this.lnaGainTrackBar.Maximum = 5;
            this.lnaGainTrackBar.Name = "lnaGainTrackBar";
            this.lnaGainTrackBar.Size = new System.Drawing.Size(267, 45);
            this.lnaGainTrackBar.TabIndex = 6;
            this.lnaGainTrackBar.Scroll += new System.EventHandler(this.lnaGainTrackBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "LNA Gain";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Sample Rate";
            // 
            // samplerateComboBox
            // 
            this.samplerateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.samplerateComboBox.FormattingEnabled = true;
            this.samplerateComboBox.Items.AddRange(new object[] {
            "20 MSPS",
            "16 MSPS",
            "12.5 MSPS",
            "10 MSPS",
            "8 MSPS"});
            this.samplerateComboBox.Location = new System.Drawing.Point(12, 70);
            this.samplerateComboBox.Name = "samplerateComboBox";
            this.samplerateComboBox.Size = new System.Drawing.Size(247, 21);
            this.samplerateComboBox.TabIndex = 1;
            this.samplerateComboBox.SelectedIndexChanged += new System.EventHandler(this.samplerateComboBox_SelectedIndexChanged);
            // 
            // lnaGainLabel
            // 
            this.lnaGainLabel.Location = new System.Drawing.Point(190, 97);
            this.lnaGainLabel.Name = "lnaGainLabel";
            this.lnaGainLabel.Size = new System.Drawing.Size(68, 13);
            this.lnaGainLabel.TabIndex = 26;
            this.lnaGainLabel.Text = "1000dB";
            this.lnaGainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lnaGainLabel.Visible = false;
            // 
            // frequencyCorrectionNumericUpDown
            // 
            this.frequencyCorrectionNumericUpDown.DecimalPlaces = 1;
            this.frequencyCorrectionNumericUpDown.Location = new System.Drawing.Point(169, 222);
            this.frequencyCorrectionNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.frequencyCorrectionNumericUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.frequencyCorrectionNumericUpDown.Name = "frequencyCorrectionNumericUpDown";
            this.frequencyCorrectionNumericUpDown.Size = new System.Drawing.Size(90, 20);
            this.frequencyCorrectionNumericUpDown.TabIndex = 7;
            this.frequencyCorrectionNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.frequencyCorrectionNumericUpDown.ValueChanged += new System.EventHandler(this.frequencyCorrectionNumericUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 224);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Frequency correction (ppm)";
            // 
            // tunerTypeLabel
            // 
            this.tunerTypeLabel.Location = new System.Drawing.Point(166, 9);
            this.tunerTypeLabel.Name = "tunerTypeLabel";
            this.tunerTypeLabel.Size = new System.Drawing.Size(93, 13);
            this.tunerTypeLabel.TabIndex = 29;
            this.tunerTypeLabel.Text = "HackRF";
            this.tunerTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 145);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "VGA Gain";
            // 
            // vgaGainTrackBar
            // 
            this.vgaGainTrackBar.LargeChange = 1;
            this.vgaGainTrackBar.Location = new System.Drawing.Point(2, 164);
            this.vgaGainTrackBar.Maximum = 31;
            this.vgaGainTrackBar.Name = "vgaGainTrackBar";
            this.vgaGainTrackBar.Size = new System.Drawing.Size(267, 45);
            this.vgaGainTrackBar.TabIndex = 33;
            this.vgaGainTrackBar.Scroll += new System.EventHandler(this.vgaGainTrackBar_Scroll);
            // 
            // vgaGainLabel
            // 
            this.vgaGainLabel.Location = new System.Drawing.Point(190, 145);
            this.vgaGainLabel.Name = "vgaGainLabel";
            this.vgaGainLabel.Size = new System.Drawing.Size(68, 13);
            this.vgaGainLabel.TabIndex = 34;
            this.vgaGainLabel.Text = "1000dB";
            this.vgaGainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.vgaGainLabel.Visible = false;
            // 
            // externalAmpCb
            // 
            this.externalAmpCb.AutoSize = true;
            this.externalAmpCb.Location = new System.Drawing.Point(12, 201);
            this.externalAmpCb.Name = "externalAmpCb";
            this.externalAmpCb.Size = new System.Drawing.Size(92, 17);
            this.externalAmpCb.TabIndex = 35;
            this.externalAmpCb.Text = "Amp (Bias-T?)";
            this.externalAmpCb.UseVisualStyleBackColor = true;
            this.externalAmpCb.CheckedChanged += new System.EventHandler(this.externalAmpCb_CheckedChanged);
            // 
            // HackRFControllerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(271, 276);
            this.Controls.Add(this.externalAmpCb);
            this.Controls.Add(this.vgaGainLabel);
            this.Controls.Add(this.vgaGainTrackBar);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tunerTypeLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.frequencyCorrectionNumericUpDown);
            this.Controls.Add(this.lnaGainLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.samplerateComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lnaGainTrackBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.deviceComboBox);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HackRFControllerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HackRF Controller";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HackRFControllerDialog_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.HackRFControllerDialog_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.lnaGainTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyCorrectionNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vgaGainTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ComboBox deviceComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar lnaGainTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox samplerateComboBox;
        private System.Windows.Forms.Label lnaGainLabel;
        private System.Windows.Forms.NumericUpDown frequencyCorrectionNumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label tunerTypeLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar vgaGainTrackBar;
        private System.Windows.Forms.Label vgaGainLabel;
        private System.Windows.Forms.CheckBox externalAmpCb;
    }
}

