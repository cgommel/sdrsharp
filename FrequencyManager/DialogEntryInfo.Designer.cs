namespace SDRSharp.FrequencyManager
{
    partial class DialogEntryInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.Label();
            this.comboGroupName = new System.Windows.Forms.ComboBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.frequencyNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.shiftNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nudFilterBandwidth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shiftNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFilterBandwidth)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select an existing group or enter a new group name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 37);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Group:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 67);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 98);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Frequency:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 187);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Mode:";
            // 
            // lblMode
            // 
            this.lblMode.Location = new System.Drawing.Point(89, 187);
            this.lblMode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(120, 17);
            this.lblMode.TabIndex = 5;
            // 
            // comboGroupName
            // 
            this.comboGroupName.FormattingEnabled = true;
            this.comboGroupName.Location = new System.Drawing.Point(85, 34);
            this.comboGroupName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboGroupName.Name = "comboGroupName";
            this.comboGroupName.Size = new System.Drawing.Size(178, 21);
            this.comboGroupName.TabIndex = 6;
            this.comboGroupName.TextChanged += new System.EventHandler(this.Control_TextChanged);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(85, 64);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(178, 20);
            this.textBoxName.TabIndex = 7;
            this.textBoxName.TextChanged += new System.EventHandler(this.Control_TextChanged);
            // 
            // frequencyNumericUpDown
            // 
            this.frequencyNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.frequencyNumericUpDown.Location = new System.Drawing.Point(85, 96);
            this.frequencyNumericUpDown.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.frequencyNumericUpDown.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.frequencyNumericUpDown.Name = "frequencyNumericUpDown";
            this.frequencyNumericUpDown.Size = new System.Drawing.Size(124, 20);
            this.frequencyNumericUpDown.TabIndex = 9;
            this.frequencyNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.frequencyNumericUpDown.ThousandsSeparator = true;
            this.frequencyNumericUpDown.ValueChanged += new System.EventHandler(this.Control_TextChanged);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(149, 212);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(56, 23);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "O&K";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(209, 212);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // shiftNumericUpDown
            // 
            this.shiftNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.shiftNumericUpDown.Location = new System.Drawing.Point(85, 126);
            this.shiftNumericUpDown.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.shiftNumericUpDown.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.shiftNumericUpDown.Name = "shiftNumericUpDown";
            this.shiftNumericUpDown.Size = new System.Drawing.Size(124, 20);
            this.shiftNumericUpDown.TabIndex = 13;
            this.shiftNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.shiftNumericUpDown.ThousandsSeparator = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 129);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Shift:";
            // 
            // nudFilterBandwidth
            // 
            this.nudFilterBandwidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudFilterBandwidth.Location = new System.Drawing.Point(85, 156);
            this.nudFilterBandwidth.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudFilterBandwidth.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.nudFilterBandwidth.Name = "nudFilterBandwidth";
            this.nudFilterBandwidth.Size = new System.Drawing.Size(124, 20);
            this.nudFilterBandwidth.TabIndex = 15;
            this.nudFilterBandwidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudFilterBandwidth.ThousandsSeparator = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 160);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Filter BW:";
            // 
            // DialogEntryInfo
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(276, 246);
            this.Controls.Add(this.nudFilterBandwidth);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.shiftNumericUpDown);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.frequencyNumericUpDown);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.comboGroupName);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogEntryInfo";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Entry Information";
            ((System.ComponentModel.ISupportInitialize)(this.frequencyNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shiftNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFilterBandwidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ComboBox comboGroupName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.NumericUpDown frequencyNumericUpDown;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown shiftNumericUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudFilterBandwidth;
        private System.Windows.Forms.Label label7;
    }
}