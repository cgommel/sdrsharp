namespace SDRSharp.RTLSDR
{
    partial class RtlSdrControllerDialog
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
            this.tunerGainTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.samplerateComboBox = new System.Windows.Forms.ComboBox();
            this.tunerAgcCheckBox = new System.Windows.Forms.CheckBox();
            this.gainLabel = new System.Windows.Forms.Label();
            this.frequencyCorrectionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.tunerTypeLabel = new System.Windows.Forms.Label();
            this.rtlAgcCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.tunerGainTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyCorrectionNumericUpDown)).BeginInit();
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
            this.closeButton.Location = new System.Drawing.Point(184, 238);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 6;
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
            // tunerGainTrackBar
            // 
            this.tunerGainTrackBar.Enabled = false;
            this.tunerGainTrackBar.Location = new System.Drawing.Point(3, 161);
            this.tunerGainTrackBar.Maximum = 10000;
            this.tunerGainTrackBar.Name = "tunerGainTrackBar";
            this.tunerGainTrackBar.Size = new System.Drawing.Size(267, 45);
            this.tunerGainTrackBar.TabIndex = 4;
            this.tunerGainTrackBar.Scroll += new System.EventHandler(this.tunerGainTrackBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "RF Gain";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 58);
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
            "3.2 MSPS",
            "2.8 MSPS",
            "2.4 MSPS",
            "2.048 MSPS",
            "1.92 MSPS",
            "1.8 MSPS",
            "1.4 MSPS",
            "1.024 MSPS",
            "0.900001 MSPS",
            "0.25 MSPS"});
            this.samplerateComboBox.Location = new System.Drawing.Point(12, 75);
            this.samplerateComboBox.Name = "samplerateComboBox";
            this.samplerateComboBox.Size = new System.Drawing.Size(247, 21);
            this.samplerateComboBox.TabIndex = 1;
            this.samplerateComboBox.SelectedIndexChanged += new System.EventHandler(this.samplerateComboBox_SelectedIndexChanged);
            // 
            // tunerAgcCheckBox
            // 
            this.tunerAgcCheckBox.AutoSize = true;
            this.tunerAgcCheckBox.Checked = true;
            this.tunerAgcCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tunerAgcCheckBox.Location = new System.Drawing.Point(12, 125);
            this.tunerAgcCheckBox.Name = "tunerAgcCheckBox";
            this.tunerAgcCheckBox.Size = new System.Drawing.Size(79, 17);
            this.tunerAgcCheckBox.TabIndex = 3;
            this.tunerAgcCheckBox.Text = "Tuner AGC";
            this.tunerAgcCheckBox.UseVisualStyleBackColor = true;
            this.tunerAgcCheckBox.CheckedChanged += new System.EventHandler(this.tunerAgcCheckBox_CheckedChanged);
            // 
            // gainLabel
            // 
            this.gainLabel.Location = new System.Drawing.Point(191, 145);
            this.gainLabel.Name = "gainLabel";
            this.gainLabel.Size = new System.Drawing.Size(68, 13);
            this.gainLabel.TabIndex = 26;
            this.gainLabel.Text = "1000dB";
            this.gainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.gainLabel.Visible = false;
            // 
            // frequencyCorrectionNumericUpDown
            // 
            this.frequencyCorrectionNumericUpDown.Location = new System.Drawing.Point(169, 207);
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
            this.frequencyCorrectionNumericUpDown.TabIndex = 5;
            this.frequencyCorrectionNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.frequencyCorrectionNumericUpDown.ValueChanged += new System.EventHandler(this.frequencyCorrectionNumericUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 209);
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
            this.tunerTypeLabel.Text = "E4000";
            this.tunerTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rtlAgcCheckBox
            // 
            this.rtlAgcCheckBox.AutoSize = true;
            this.rtlAgcCheckBox.Location = new System.Drawing.Point(12, 102);
            this.rtlAgcCheckBox.Name = "rtlAgcCheckBox";
            this.rtlAgcCheckBox.Size = new System.Drawing.Size(72, 17);
            this.rtlAgcCheckBox.TabIndex = 2;
            this.rtlAgcCheckBox.Text = "RTL AGC";
            this.rtlAgcCheckBox.UseVisualStyleBackColor = true;
            this.rtlAgcCheckBox.CheckedChanged += new System.EventHandler(this.rtlAgcCheckBox_CheckedChanged);
            // 
            // RtlSdrControllerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(271, 271);
            this.Controls.Add(this.rtlAgcCheckBox);
            this.Controls.Add(this.tunerTypeLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.frequencyCorrectionNumericUpDown);
            this.Controls.Add(this.gainLabel);
            this.Controls.Add(this.tunerAgcCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.samplerateComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tunerGainTrackBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.deviceComboBox);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RtlSdrControllerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RTL-SDR Controller";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RtlSdrControllerDialog_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.RtlSdrControllerDialog_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.tunerGainTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyCorrectionNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ComboBox deviceComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar tunerGainTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox samplerateComboBox;
        private System.Windows.Forms.CheckBox tunerAgcCheckBox;
        private System.Windows.Forms.Label gainLabel;
        private System.Windows.Forms.NumericUpDown frequencyCorrectionNumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label tunerTypeLabel;
        private System.Windows.Forms.CheckBox rtlAgcCheckBox;
    }
}

