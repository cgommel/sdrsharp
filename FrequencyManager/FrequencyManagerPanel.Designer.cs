namespace SDRSharp.FrequencyManager
{
    partial class FrequencyManagerPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrequencyManagerPanel));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnNewEntry = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.label17 = new System.Windows.Forms.Label();
            this.frequencyDataGridView = new System.Windows.Forms.DataGridView();
            this.comboGroups = new System.Windows.Forms.ComboBox();
            this.mainImageList = new System.Windows.Forms.ImageList(this.components);
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.frequencyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.memoryEntryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryEntryBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainToolStrip.AutoSize = false;
            this.mainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewEntry,
            this.btnEdit,
            this.btnDelete});
            this.mainToolStrip.Location = new System.Drawing.Point(1, 3);
            this.mainToolStrip.MinimumSize = new System.Drawing.Size(205, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(234, 22);
            this.mainToolStrip.Stretch = true;
            this.mainToolStrip.TabIndex = 7;
            this.mainToolStrip.Text = "toolStrip1";
            // 
            // btnNewEntry
            // 
            this.btnNewEntry.Image = ((System.Drawing.Image)(resources.GetObject("btnNewEntry.Image")));
            this.btnNewEntry.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewEntry.Name = "btnNewEntry";
            this.btnNewEntry.Size = new System.Drawing.Size(48, 19);
            this.btnNewEntry.Text = "New";
            this.btnNewEntry.Click += new System.EventHandler(this.btnNewEntry_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(45, 19);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(58, 19);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(2, 32);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(39, 13);
            this.label17.TabIndex = 5;
            this.label17.Text = "Group:";
            // 
            // frequencyDataGridView
            // 
            this.frequencyDataGridView.AllowUserToAddRows = false;
            this.frequencyDataGridView.AllowUserToDeleteRows = false;
            this.frequencyDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.frequencyDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.frequencyDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.frequencyDataGridView.AutoGenerateColumns = false;
            this.frequencyDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.frequencyDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.frequencyDataGridViewTextBoxColumn});
            this.frequencyDataGridView.DataSource = this.memoryEntryBindingSource;
            this.frequencyDataGridView.Location = new System.Drawing.Point(0, 54);
            this.frequencyDataGridView.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.frequencyDataGridView.Name = "frequencyDataGridView";
            this.frequencyDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.frequencyDataGridView.RowHeadersVisible = false;
            this.frequencyDataGridView.RowTemplate.Height = 24;
            this.frequencyDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.frequencyDataGridView.ShowCellErrors = false;
            this.frequencyDataGridView.ShowCellToolTips = false;
            this.frequencyDataGridView.ShowEditingIcon = false;
            this.frequencyDataGridView.ShowRowErrors = false;
            this.frequencyDataGridView.Size = new System.Drawing.Size(236, 304);
            this.frequencyDataGridView.TabIndex = 6;
            this.frequencyDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.frequencyDataGridView_CellDoubleClick);
            this.frequencyDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.frequencyDataGridView_CellFormatting);
            this.frequencyDataGridView.SelectionChanged += new System.EventHandler(this.frequencyDataGridView_SelectionChanged);
            // 
            // comboGroups
            // 
            this.comboGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboGroups.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboGroups.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGroups.FormattingEnabled = true;
            this.comboGroups.Location = new System.Drawing.Point(47, 29);
            this.comboGroups.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboGroups.Name = "comboGroups";
            this.comboGroups.Size = new System.Drawing.Size(189, 21);
            this.comboGroups.TabIndex = 4;
            this.comboGroups.SelectedIndexChanged += new System.EventHandler(this.comboGroups_SelectedIndexChanged);
            // 
            // mainImageList
            // 
            this.mainImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mainImageList.ImageStream")));
            this.mainImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mainImageList.Images.SetKeyName(0, "NewDocumentHS.BMP");
            this.mainImageList.Images.SetKeyName(1, "DeleteHS.bmp");
            this.mainImageList.Images.SetKeyName(2, "EditInformationHS.BMP");
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // frequencyDataGridViewTextBoxColumn
            // 
            this.frequencyDataGridViewTextBoxColumn.DataPropertyName = "Frequency";
            this.frequencyDataGridViewTextBoxColumn.HeaderText = "Frequency";
            this.frequencyDataGridViewTextBoxColumn.Name = "frequencyDataGridViewTextBoxColumn";
            this.frequencyDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // memoryEntryBindingSource
            // 
            this.memoryEntryBindingSource.DataSource = typeof(SDRSharp.FrequencyManager.MemoryEntry);
            // 
            // FrequencyManagerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainToolStrip);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.frequencyDataGridView);
            this.Controls.Add(this.comboGroups);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FrequencyManagerPanel";
            this.Size = new System.Drawing.Size(236, 358);
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frequencyDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoryEntryBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton btnNewEntry;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.DataGridView frequencyDataGridView;
        private System.Windows.Forms.ComboBox comboGroups;
        private System.Windows.Forms.ImageList mainImageList;
        private System.Windows.Forms.BindingSource memoryEntryBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn frequencyDataGridViewTextBoxColumn;
    }
}
