namespace SDRSharp.CollapsiblePanel
{
    partial class CollapsiblePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollapsiblePanel));
            this.collapsiblePanelImageList = new System.Windows.Forms.ImageList(this.components);
            this.titlePanel = new System.Windows.Forms.Panel();
            this.lblPanelTitle = new System.Windows.Forms.Label();
            this.togglingImage = new System.Windows.Forms.PictureBox();
            this.titlePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.togglingImage)).BeginInit();
            this.SuspendLayout();
            // 
            // collapsiblePanelImageList
            // 
            this.collapsiblePanelImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("collapsiblePanelImageList.ImageStream")));
            this.collapsiblePanelImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.collapsiblePanelImageList.Images.SetKeyName(0, "ExpandIcon.jpg");
            // 
            // titlePanel
            // 
            this.titlePanel.BackColor = System.Drawing.Color.DarkGray;
            this.titlePanel.BackgroundImage = global::StaffDotNet.CollapsiblePanel.Properties.Resources.titleBackground;
            this.titlePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.titlePanel.Controls.Add(this.lblPanelTitle);
            this.titlePanel.Controls.Add(this.togglingImage);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(0, 0);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(150, 20);
            this.titlePanel.TabIndex = 0;
            // 
            // lblPanelTitle
            // 
            this.lblPanelTitle.AutoSize = true;
            this.lblPanelTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblPanelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPanelTitle.Location = new System.Drawing.Point(22, 3);
            this.lblPanelTitle.Name = "lblPanelTitle";
            this.lblPanelTitle.Size = new System.Drawing.Size(64, 13);
            this.lblPanelTitle.TabIndex = 1;
            this.lblPanelTitle.Text = "Panel title";
            this.lblPanelTitle.Click += new System.EventHandler(this.ToggleState);
            // 
            // togglingImage
            // 
            this.togglingImage.BackColor = System.Drawing.Color.Transparent;
            this.togglingImage.Image = global::StaffDotNet.CollapsiblePanel.Properties.Resources.ExpandedIcon;
            this.togglingImage.Location = new System.Drawing.Point(3, 3);
            this.togglingImage.Name = "togglingImage";
            this.togglingImage.Size = new System.Drawing.Size(14, 13);
            this.togglingImage.TabIndex = 0;
            this.togglingImage.TabStop = false;
            this.togglingImage.Click += new System.EventHandler(this.ToggleState);
            // 
            // CollapsiblePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.titlePanel);
            this.Name = "CollapsiblePanel";
            this.titlePanel.ResumeLayout(false);
            this.titlePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.togglingImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel titlePanel;
        private System.Windows.Forms.PictureBox togglingImage;
        private System.Windows.Forms.ImageList collapsiblePanelImageList;
        private System.Windows.Forms.Label lblPanelTitle;
    }
}
