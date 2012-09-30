namespace SDRSharp.WavRecorder
{
    partial class RecordingPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.recBtn = new System.Windows.Forms.Button();
            this.recDisplayTimer = new System.Windows.Forms.Timer(this.components);
            this.durationLbl = new System.Windows.Forms.Label();
            this.sampleFormatCombo = new System.Windows.Forms.ComboBox();
            this.sampleFormatLbl = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sizeLbl = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.audioRadioBtn = new System.Windows.Forms.RadioButton();
            this.basebandRadioBtn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // recBtn
            // 
            this.recBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recBtn.Enabled = false;
            this.recBtn.Location = new System.Drawing.Point(118, 161);
            this.recBtn.Name = "recBtn";
            this.recBtn.Size = new System.Drawing.Size(119, 23);
            this.recBtn.TabIndex = 0;
            this.recBtn.Text = "Record";
            this.recBtn.UseVisualStyleBackColor = true;
            this.recBtn.Click += new System.EventHandler(this.recBtn_Click);
            // 
            // recDisplayTimer
            // 
            this.recDisplayTimer.Interval = 1000;
            this.recDisplayTimer.Tick += new System.EventHandler(this.recDisplayTimer_Tick);
            // 
            // durationLbl
            // 
            this.durationLbl.AutoSize = true;
            this.durationLbl.Location = new System.Drawing.Point(109, 49);
            this.durationLbl.Name = "durationLbl";
            this.durationLbl.Size = new System.Drawing.Size(49, 13);
            this.durationLbl.TabIndex = 3;
            this.durationLbl.Text = "00:00:00";
            // 
            // sampleFormatCombo
            // 
            this.sampleFormatCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sampleFormatCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sampleFormatCombo.DropDownWidth = 120;
            this.sampleFormatCombo.FormattingEnabled = true;
            this.sampleFormatCombo.Items.AddRange(new object[] {
            "8 Bit PCM",
            "16 Bit PCM",
            "32 Bit IEEE Float"});
            this.sampleFormatCombo.Location = new System.Drawing.Point(112, 19);
            this.sampleFormatCombo.Name = "sampleFormatCombo";
            this.sampleFormatCombo.Size = new System.Drawing.Size(113, 21);
            this.sampleFormatCombo.TabIndex = 4;
            this.sampleFormatCombo.SelectedIndexChanged += new System.EventHandler(this.sampleFormatCombo_SelectedIndexChanged);
            // 
            // sampleFormatLbl
            // 
            this.sampleFormatLbl.AutoSize = true;
            this.sampleFormatLbl.Location = new System.Drawing.Point(6, 22);
            this.sampleFormatLbl.Name = "sampleFormatLbl";
            this.sampleFormatLbl.Size = new System.Drawing.Size(77, 13);
            this.sampleFormatLbl.TabIndex = 5;
            this.sampleFormatLbl.Text = "Sample Format";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Duration";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "File Size";
            // 
            // sizeLbl
            // 
            this.sizeLbl.AutoSize = true;
            this.sizeLbl.Location = new System.Drawing.Point(109, 25);
            this.sizeLbl.Name = "sizeLbl";
            this.sizeLbl.Size = new System.Drawing.Size(32, 13);
            this.sizeLbl.TabIndex = 6;
            this.sizeLbl.Text = "0 MB";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.recBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 208);
            this.panel1.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.audioRadioBtn);
            this.groupBox2.Controls.Add(this.sampleFormatCombo);
            this.groupBox2.Controls.Add(this.sampleFormatLbl);
            this.groupBox2.Controls.Add(this.basebandRadioBtn);
            this.groupBox2.Location = new System.Drawing.Point(6, 82);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(231, 73);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mode";
            // 
            // audioRadioBtn
            // 
            this.audioRadioBtn.AutoSize = true;
            this.audioRadioBtn.Location = new System.Drawing.Point(112, 50);
            this.audioRadioBtn.Name = "audioRadioBtn";
            this.audioRadioBtn.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.audioRadioBtn.Size = new System.Drawing.Size(52, 17);
            this.audioRadioBtn.TabIndex = 1;
            this.audioRadioBtn.Text = "Audio";
            this.audioRadioBtn.UseVisualStyleBackColor = true;
            this.audioRadioBtn.CheckedChanged += new System.EventHandler(this.SampleSource_CheckedChanged);
            // 
            // basebandRadioBtn
            // 
            this.basebandRadioBtn.AutoSize = true;
            this.basebandRadioBtn.Checked = true;
            this.basebandRadioBtn.Location = new System.Drawing.Point(10, 50);
            this.basebandRadioBtn.Name = "basebandRadioBtn";
            this.basebandRadioBtn.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.basebandRadioBtn.Size = new System.Drawing.Size(73, 17);
            this.basebandRadioBtn.TabIndex = 0;
            this.basebandRadioBtn.TabStop = true;
            this.basebandRadioBtn.Text = "Baseband";
            this.basebandRadioBtn.UseVisualStyleBackColor = true;
            this.basebandRadioBtn.CheckedChanged += new System.EventHandler(this.SampleSource_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.sizeLbl);
            this.groupBox1.Controls.Add(this.durationLbl);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(231, 73);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // RecordingPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "RecordingPanel";
            this.Size = new System.Drawing.Size(248, 208);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button recBtn;
        private System.Windows.Forms.Timer recDisplayTimer;
        private System.Windows.Forms.Label durationLbl;
        private System.Windows.Forms.ComboBox sampleFormatCombo;
        private System.Windows.Forms.Label sampleFormatLbl;
        private System.Windows.Forms.Label sizeLbl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton audioRadioBtn;
        private System.Windows.Forms.RadioButton basebandRadioBtn;
    }
}
