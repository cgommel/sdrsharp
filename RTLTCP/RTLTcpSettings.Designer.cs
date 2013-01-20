namespace SDRSharp.RTLTCP
{
    partial class RTLTcpSettings
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
            this.hostBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rtlAgcCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.frequencyCorrectionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tunerAgcCheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tunerGainTrackBar = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.samplerateComboBox = new System.Windows.Forms.ComboBox();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.portNumberUpDown = new System.Windows.Forms.NumericUpDown();
            this.tunerLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyCorrectionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tunerGainTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.portNumberUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // hostBox
            // 
            this.hostBox.Location = new System.Drawing.Point(119, 12);
            this.hostBox.Name = "hostBox";
            this.hostBox.Size = new System.Drawing.Size(138, 20);
            this.hostBox.TabIndex = 0;
            this.hostBox.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Host";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port";
            // 
            // rtlAgcCheckBox
            // 
            this.rtlAgcCheckBox.AutoSize = true;
            this.rtlAgcCheckBox.Location = new System.Drawing.Point(10, 111);
            this.rtlAgcCheckBox.Name = "rtlAgcCheckBox";
            this.rtlAgcCheckBox.Size = new System.Drawing.Size(72, 17);
            this.rtlAgcCheckBox.TabIndex = 4;
            this.rtlAgcCheckBox.Text = "RTL AGC";
            this.rtlAgcCheckBox.UseVisualStyleBackColor = true;
            this.rtlAgcCheckBox.CheckedChanged += new System.EventHandler(this.rtlAgcCheckBox_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 218);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "Frequency correction (ppm)";
            // 
            // frequencyCorrectionNumericUpDown
            // 
            this.frequencyCorrectionNumericUpDown.Location = new System.Drawing.Point(167, 216);
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
            // tunerAgcCheckBox
            // 
            this.tunerAgcCheckBox.AutoSize = true;
            this.tunerAgcCheckBox.Checked = true;
            this.tunerAgcCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tunerAgcCheckBox.Location = new System.Drawing.Point(10, 134);
            this.tunerAgcCheckBox.Name = "tunerAgcCheckBox";
            this.tunerAgcCheckBox.Size = new System.Drawing.Size(79, 17);
            this.tunerAgcCheckBox.TabIndex = 5;
            this.tunerAgcCheckBox.Text = "Tuner AGC";
            this.tunerAgcCheckBox.UseVisualStyleBackColor = true;
            this.tunerAgcCheckBox.CheckedChanged += new System.EventHandler(this.tunerAgcCheckBox_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 154);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "RF Gain";
            // 
            // tunerGainTrackBar
            // 
            this.tunerGainTrackBar.Enabled = false;
            this.tunerGainTrackBar.Location = new System.Drawing.Point(1, 170);
            this.tunerGainTrackBar.Maximum = 10000;
            this.tunerGainTrackBar.Name = "tunerGainTrackBar";
            this.tunerGainTrackBar.Size = new System.Drawing.Size(267, 45);
            this.tunerGainTrackBar.TabIndex = 6;
            this.tunerGainTrackBar.Scroll += new System.EventHandler(this.tunerGainTrackBar_Scroll);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 66);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Sample Rate";
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
            this.samplerateComboBox.Location = new System.Drawing.Point(10, 84);
            this.samplerateComboBox.Name = "samplerateComboBox";
            this.samplerateComboBox.Size = new System.Drawing.Size(247, 21);
            this.samplerateComboBox.TabIndex = 3;
            this.samplerateComboBox.SelectedIndexChanged += new System.EventHandler(this.samplerateComboBox_SelectedIndexChanged);
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // portNumberUpDown
            // 
            this.portNumberUpDown.Location = new System.Drawing.Point(119, 38);
            this.portNumberUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.portNumberUpDown.Name = "portNumberUpDown";
            this.portNumberUpDown.Size = new System.Drawing.Size(138, 20);
            this.portNumberUpDown.TabIndex = 2;
            this.portNumberUpDown.Value = new decimal(new int[] {
            1234,
            0,
            0,
            0});
            // 
            // tunerLabel
            // 
            this.tunerLabel.Location = new System.Drawing.Point(186, 154);
            this.tunerLabel.Name = "tunerLabel";
            this.tunerLabel.Size = new System.Drawing.Size(68, 13);
            this.tunerLabel.TabIndex = 38;
            this.tunerLabel.Text = "Tuner";
            this.tunerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RTLTcpSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 257);
            this.Controls.Add(this.tunerLabel);
            this.Controls.Add(this.portNumberUpDown);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.samplerateComboBox);
            this.Controls.Add(this.rtlAgcCheckBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.frequencyCorrectionNumericUpDown);
            this.Controls.Add(this.tunerAgcCheckBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tunerGainTrackBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hostBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RTLTcpSettings";
            this.ShowInTaskbar = false;
            this.Text = "RTL-TCP Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RTLTcpSettings_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.RTLTcpSettings_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.frequencyCorrectionNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tunerGainTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.portNumberUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox hostBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox rtlAgcCheckBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown frequencyCorrectionNumericUpDown;
        private System.Windows.Forms.CheckBox tunerAgcCheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar tunerGainTrackBar;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox samplerateComboBox;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.NumericUpDown portNumberUpDown;
        private System.Windows.Forms.Label tunerLabel;
    }
}