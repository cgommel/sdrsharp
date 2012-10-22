namespace SDRSharp.FUNcubeProPlus
{
    partial class FCDProPlusControllerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FCDProPlusControllerDialog));
            this.defaultButton = new System.Windows.Forms.Button();
            this.rfFilterCombo = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.frequencyCorrectionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ifFilterCombo = new System.Windows.Forms.ComboBox();
            this.lnaEnableCb = new System.Windows.Forms.CheckBox();
            this.mixerGainEnableCb = new System.Windows.Forms.CheckBox();
            this.ifGainNumbericUpDown = new System.Windows.Forms.NumericUpDown();
            this.biasTEnabledCb = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyCorrectionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ifGainNumbericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // defaultButton
            // 
            this.defaultButton.Location = new System.Drawing.Point(467, 105);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Size = new System.Drawing.Size(75, 23);
            this.defaultButton.TabIndex = 17;
            this.defaultButton.Text = "Default";
            this.defaultButton.UseVisualStyleBackColor = true;
            this.defaultButton.Click += new System.EventHandler(this.DefaultButton_Click);
            // 
            // rfFilterCombo
            // 
            this.rfFilterCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rfFilterCombo.Enabled = false;
            this.rfFilterCombo.FormattingEnabled = true;
            this.rfFilterCombo.Items.AddRange(new object[] {
            "150kHz-4MHz",
            "4MHz-8MHz",
            "8MHz-16MHz",
            "16MHz-32MHz",
            "32MHz-75MHz",
            "75MHz-125MHz",
            "125MHz-250MHz",
            "144MHz-146MHz",
            "420MHz-875MHz",
            "430MHz-440MHz",
            "875MHz-1900MHz"});
            this.rfFilterCombo.Location = new System.Drawing.Point(12, 168);
            this.rfFilterCombo.Name = "rfFilterCombo";
            this.rfFilterCombo.Size = new System.Drawing.Size(95, 21);
            this.rfFilterCombo.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(2, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(459, 154);
            this.pictureBox1.TabIndex = 86;
            this.pictureBox1.TabStop = false;
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(467, 134);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 18;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(280, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 122;
            this.label1.Text = "Frequency correction (ppm)";
            // 
            // frequencyCorrectionNumericUpDown
            // 
            this.frequencyCorrectionNumericUpDown.DecimalPlaces = 1;
            this.frequencyCorrectionNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.frequencyCorrectionNumericUpDown.Location = new System.Drawing.Point(283, 28);
            this.frequencyCorrectionNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.frequencyCorrectionNumericUpDown.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.frequencyCorrectionNumericUpDown.Name = "frequencyCorrectionNumericUpDown";
            this.frequencyCorrectionNumericUpDown.Size = new System.Drawing.Size(133, 20);
            this.frequencyCorrectionNumericUpDown.TabIndex = 16;
            this.frequencyCorrectionNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.frequencyCorrectionNumericUpDown.Value = new decimal(new int[] {
            115,
            0,
            0,
            -2147483648});
            this.frequencyCorrectionNumericUpDown.ValueChanged += new System.EventHandler(this.frequencyCorrectionNumericUpDown_ValueChanged);
            // 
            // ifFilterCombo
            // 
            this.ifFilterCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ifFilterCombo.Enabled = false;
            this.ifFilterCombo.FormattingEnabled = true;
            this.ifFilterCombo.Items.AddRange(new object[] {
            "200kHz",
            "300kHz",
            "600kHz",
            "1536kHz",
            "5MHz",
            "6MHz",
            "7MHz",
            "8MHz"});
            this.ifFilterCombo.Location = new System.Drawing.Point(283, 167);
            this.ifFilterCombo.Name = "ifFilterCombo";
            this.ifFilterCombo.Size = new System.Drawing.Size(81, 21);
            this.ifFilterCombo.TabIndex = 123;
            // 
            // lnaEnableCb
            // 
            this.lnaEnableCb.AutoSize = true;
            this.lnaEnableCb.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lnaEnableCb.Location = new System.Drawing.Point(113, 167);
            this.lnaEnableCb.Name = "lnaEnableCb";
            this.lnaEnableCb.Size = new System.Drawing.Size(68, 31);
            this.lnaEnableCb.TabIndex = 124;
            this.lnaEnableCb.Text = "LNA Enable";
            this.lnaEnableCb.UseVisualStyleBackColor = true;
            this.lnaEnableCb.CheckedChanged += new System.EventHandler(this.lnaEnableCb_CheckedChanged);
            // 
            // mixerGainEnableCb
            // 
            this.mixerGainEnableCb.AutoSize = true;
            this.mixerGainEnableCb.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.mixerGainEnableCb.Location = new System.Drawing.Point(199, 167);
            this.mixerGainEnableCb.Name = "mixerGainEnableCb";
            this.mixerGainEnableCb.Size = new System.Drawing.Size(61, 31);
            this.mixerGainEnableCb.TabIndex = 125;
            this.mixerGainEnableCb.Text = "Mixer Gain";
            this.mixerGainEnableCb.UseVisualStyleBackColor = true;
            this.mixerGainEnableCb.CheckedChanged += new System.EventHandler(this.mixerGainEnableCb_CheckedChanged);
            // 
            // ifGainNumbericUpDown
            // 
            this.ifGainNumbericUpDown.Location = new System.Drawing.Point(380, 168);
            this.ifGainNumbericUpDown.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.ifGainNumbericUpDown.Name = "ifGainNumbericUpDown";
            this.ifGainNumbericUpDown.Size = new System.Drawing.Size(67, 20);
            this.ifGainNumbericUpDown.TabIndex = 126;
            this.ifGainNumbericUpDown.ValueChanged += new System.EventHandler(this.ifGainNumbericUpDown_ValueChanged);
            // 
            // biasTEnabledCb
            // 
            this.biasTEnabledCb.AutoSize = true;
            this.biasTEnabledCb.Location = new System.Drawing.Point(467, 82);
            this.biasTEnabledCb.Name = "biasTEnabledCb";
            this.biasTEnabledCb.Size = new System.Drawing.Size(56, 17);
            this.biasTEnabledCb.TabIndex = 127;
            this.biasTEnabledCb.Text = "Bias T";
            this.biasTEnabledCb.UseVisualStyleBackColor = true;
            this.biasTEnabledCb.CheckedChanged += new System.EventHandler(this.biasTEnabledCb_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 197);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 128;
            this.label2.Text = "RF Filter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(299, 197);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 129;
            this.label3.Text = "IF Filter";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(393, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 130;
            this.label4.Text = "IF Gain";
            // 
            // FCDProPlusControllerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(546, 219);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.biasTEnabledCb);
            this.Controls.Add(this.ifGainNumbericUpDown);
            this.Controls.Add(this.mixerGainEnableCb);
            this.Controls.Add(this.lnaEnableCb);
            this.Controls.Add(this.ifFilterCombo);
            this.Controls.Add(this.frequencyCorrectionNumericUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.defaultButton);
            this.Controls.Add(this.rfFilterCombo);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FCDProPlusControllerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FUNCube Dongle Pro+ Controller";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FCDControllerDialog_FormClosing);
            this.Load += new System.EventHandler(this.FCDControllerDialog_Load);
            this.VisibleChanged += new System.EventHandler(this.FCDControllerDialog_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyCorrectionNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ifGainNumbericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button defaultButton;
        private System.Windows.Forms.ComboBox rfFilterCombo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown frequencyCorrectionNumericUpDown;
        private System.Windows.Forms.ComboBox ifFilterCombo;
        private System.Windows.Forms.CheckBox lnaEnableCb;
        private System.Windows.Forms.CheckBox mixerGainEnableCb;
        private System.Windows.Forms.NumericUpDown ifGainNumbericUpDown;
        private System.Windows.Forms.CheckBox biasTEnabledCb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

