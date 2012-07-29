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
            this.hostBox = new System.Windows.Forms.TextBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.srBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.autoRB = new System.Windows.Forms.RadioButton();
            this.manualRB = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.gainBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // hostBox
            // 
            this.hostBox.Location = new System.Drawing.Point(139, 11);
            this.hostBox.Name = "hostBox";
            this.hostBox.Size = new System.Drawing.Size(133, 20);
            this.hostBox.TabIndex = 0;
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(139, 37);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(100, 20);
            this.portBox.TabIndex = 1;
            // 
            // srBox
            // 
            this.srBox.Location = new System.Drawing.Point(139, 88);
            this.srBox.Name = "srBox";
            this.srBox.Size = new System.Drawing.Size(100, 20);
            this.srBox.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(133, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Update Settings";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Hostname";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Sample Rate";
            // 
            // autoRB
            // 
            this.autoRB.AutoSize = true;
            this.autoRB.Location = new System.Drawing.Point(30, 123);
            this.autoRB.Name = "autoRB";
            this.autoRB.Size = new System.Drawing.Size(72, 17);
            this.autoRB.TabIndex = 7;
            this.autoRB.TabStop = true;
            this.autoRB.Text = "Auto Gain";
            this.autoRB.UseVisualStyleBackColor = true;
            this.autoRB.CheckedChanged += new System.EventHandler(this.autoRB_CheckedChanged);
            // 
            // manualRB
            // 
            this.manualRB.AutoSize = true;
            this.manualRB.Location = new System.Drawing.Point(139, 123);
            this.manualRB.Name = "manualRB";
            this.manualRB.Size = new System.Drawing.Size(85, 17);
            this.manualRB.TabIndex = 8;
            this.manualRB.TabStop = true;
            this.manualRB.Text = "Manual Gain";
            this.manualRB.UseVisualStyleBackColor = true;
            this.manualRB.CheckedChanged += new System.EventHandler(this.manualRB_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 151);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Gain setting";
            // 
            // gainBox
            // 
            this.gainBox.Location = new System.Drawing.Point(139, 148);
            this.gainBox.Name = "gainBox";
            this.gainBox.Size = new System.Drawing.Size(100, 20);
            this.gainBox.TabIndex = 10;
            // 
            // RTLTcpSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 220);
            this.Controls.Add(this.gainBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.manualRB);
            this.Controls.Add(this.autoRB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.srBox);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.hostBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RTLTcpSettings";
            this.ShowInTaskbar = false;
            this.Text = "RTLTcpSettings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.RTLTcpSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox hostBox;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.TextBox srBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton autoRB;
        private System.Windows.Forms.RadioButton manualRB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox gainBox;
    }
}