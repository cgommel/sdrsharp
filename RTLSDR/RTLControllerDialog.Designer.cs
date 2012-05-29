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
            this.rfGainTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.samplerateComboBox = new System.Windows.Forms.ComboBox();
            this.gainModeCheckBox = new System.Windows.Forms.CheckBox();
            this.gainLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.rfGainTrackBar)).BeginInit();
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
            this.closeButton.Location = new System.Drawing.Point(184, 178);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 3;
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
            // rfGainTrackBar
            // 
            this.rfGainTrackBar.Enabled = false;
            this.rfGainTrackBar.Location = new System.Drawing.Point(2, 127);
            this.rfGainTrackBar.Maximum = 17;
            this.rfGainTrackBar.Name = "rfGainTrackBar";
            this.rfGainTrackBar.Size = new System.Drawing.Size(267, 45);
            this.rfGainTrackBar.TabIndex = 2;
            this.rfGainTrackBar.Scroll += new System.EventHandler(this.rfGainTrackBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 111);
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
            "2.024 MSPS",
            "1.8 MSPS",
            "1.4 MSPS",
            "1.024 MSPS",
            "0.900001 MSPS"});
            this.samplerateComboBox.Location = new System.Drawing.Point(12, 75);
            this.samplerateComboBox.Name = "samplerateComboBox";
            this.samplerateComboBox.Size = new System.Drawing.Size(247, 21);
            this.samplerateComboBox.TabIndex = 1;
            this.samplerateComboBox.SelectedIndexChanged += new System.EventHandler(this.samplerateComboBox_SelectedIndexChanged);
            // 
            // gainModeCheckBox
            // 
            this.gainModeCheckBox.AutoSize = true;
            this.gainModeCheckBox.Checked = true;
            this.gainModeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gainModeCheckBox.Location = new System.Drawing.Point(64, 110);
            this.gainModeCheckBox.Name = "gainModeCheckBox";
            this.gainModeCheckBox.Size = new System.Drawing.Size(73, 17);
            this.gainModeCheckBox.TabIndex = 25;
            this.gainModeCheckBox.Text = "Automatic";
            this.gainModeCheckBox.UseVisualStyleBackColor = true;
            this.gainModeCheckBox.CheckedChanged += new System.EventHandler(this.gainModeCheckBox_CheckedChanged);
            // 
            // gainLabel
            // 
            this.gainLabel.Location = new System.Drawing.Point(191, 111);
            this.gainLabel.Name = "gainLabel";
            this.gainLabel.Size = new System.Drawing.Size(68, 13);
            this.gainLabel.TabIndex = 26;
            this.gainLabel.Text = "1000dB";
            this.gainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RtlSdrControllerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(271, 213);
            this.Controls.Add(this.gainLabel);
            this.Controls.Add(this.gainModeCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.samplerateComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rfGainTrackBar);
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
            this.Load += new System.EventHandler(this.FCDControllerDialog_Load);
            this.VisibleChanged += new System.EventHandler(this.RtlSdrControllerDialog_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.rfGainTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ComboBox deviceComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar rfGainTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox samplerateComboBox;
        private System.Windows.Forms.CheckBox gainModeCheckBox;
        private System.Windows.Forms.Label gainLabel;
    }
}

