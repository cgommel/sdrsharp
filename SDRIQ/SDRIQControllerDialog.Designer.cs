namespace SDRSharp.SDRIQ
{
    partial class SDRIQControllerDialog
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
            this.deviceComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.samplerateComboBox = new System.Windows.Forms.ComboBox();
            this.rfGainTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.ifGainTrackBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.ifGainLabel = new System.Windows.Forms.Label();
            this.rfGainLabel = new System.Windows.Forms.Label();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.rfGainTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ifGainTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // deviceComboBox
            // 
            this.deviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceComboBox.FormattingEnabled = true;
            this.deviceComboBox.Location = new System.Drawing.Point(12, 26);
            this.deviceComboBox.Name = "deviceComboBox";
            this.deviceComboBox.Size = new System.Drawing.Size(237, 21);
            this.deviceComboBox.TabIndex = 1;
            this.deviceComboBox.SelectedIndexChanged += new System.EventHandler(this.deviceComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Device";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Sample Rate";
            // 
            // samplerateComboBox
            // 
            this.samplerateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.samplerateComboBox.FormattingEnabled = true;
            this.samplerateComboBox.Items.AddRange(new object[] {
            "8138 Hz",
            "16276 Hz",
            "37793 Hz",
            "55556 Hz ",
            "111111 Hz",
            "158730 Hz",
            "196078 Hz"});
            this.samplerateComboBox.Location = new System.Drawing.Point(12, 79);
            this.samplerateComboBox.Name = "samplerateComboBox";
            this.samplerateComboBox.Size = new System.Drawing.Size(237, 21);
            this.samplerateComboBox.TabIndex = 25;
            this.samplerateComboBox.SelectedIndexChanged += new System.EventHandler(this.samplerateComboBox_SelectedIndexChanged);
            // 
            // rfGainTrackBar
            // 
            this.rfGainTrackBar.LargeChange = 1;
            this.rfGainTrackBar.Location = new System.Drawing.Point(12, 138);
            this.rfGainTrackBar.Maximum = 3;
            this.rfGainTrackBar.Name = "rfGainTrackBar";
            this.rfGainTrackBar.Size = new System.Drawing.Size(237, 45);
            this.rfGainTrackBar.TabIndex = 27;
            this.rfGainTrackBar.Scroll += new System.EventHandler(this.rfGainTrackBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "RF Gain";
            // 
            // ifGainTrackBar
            // 
            this.ifGainTrackBar.LargeChange = 1;
            this.ifGainTrackBar.Location = new System.Drawing.Point(12, 204);
            this.ifGainTrackBar.Maximum = 5;
            this.ifGainTrackBar.Name = "ifGainTrackBar";
            this.ifGainTrackBar.Size = new System.Drawing.Size(237, 45);
            this.ifGainTrackBar.TabIndex = 29;
            this.ifGainTrackBar.Scroll += new System.EventHandler(this.ifGainTrackBar_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "IF Gain";
            // 
            // ifGainLabel
            // 
            this.ifGainLabel.AutoSize = true;
            this.ifGainLabel.Location = new System.Drawing.Point(199, 188);
            this.ifGainLabel.Name = "ifGainLabel";
            this.ifGainLabel.Size = new System.Drawing.Size(44, 13);
            this.ifGainLabel.TabIndex = 31;
            this.ifGainLabel.Text = "1000dB";
            // 
            // rfGainLabel
            // 
            this.rfGainLabel.AutoSize = true;
            this.rfGainLabel.Location = new System.Drawing.Point(199, 122);
            this.rfGainLabel.Name = "rfGainLabel";
            this.rfGainLabel.Size = new System.Drawing.Size(44, 13);
            this.rfGainLabel.TabIndex = 32;
            this.rfGainLabel.Text = "1000dB";
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // SDRIQControllerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 261);
            this.Controls.Add(this.rfGainLabel);
            this.Controls.Add(this.ifGainLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ifGainTrackBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rfGainTrackBar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.samplerateComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.deviceComboBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SDRIQControllerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SDR-IQ Controller";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SDRIQControllerDialog_FormClosing);            
            this.VisibleChanged += new System.EventHandler(this.SDRIQControllerDialog_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.rfGainTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ifGainTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox deviceComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox samplerateComboBox;
        private System.Windows.Forms.TrackBar rfGainTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar ifGainTrackBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label ifGainLabel;
        private System.Windows.Forms.Label rfGainLabel;
        private System.Windows.Forms.Timer refreshTimer;
    }
}