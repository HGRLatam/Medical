﻿namespace Medical.GUI
{
    partial class PictureWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PictureWindow));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.zoomStrechButton = new System.Windows.Forms.ToolStripButton();
            this.exploreButton = new System.Windows.Forms.ToolStripButton();
            this.picturePanel = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.toolStrip.SuspendLayout();
            this.picturePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveButton,
            this.zoomStrechButton,
            this.exploreButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(345, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // saveButton
            // 
            this.saveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(23, 22);
            this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // zoomStrechButton
            // 
            this.zoomStrechButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomStrechButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomStrechButton.Image")));
            this.zoomStrechButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomStrechButton.Name = "zoomStrechButton";
            this.zoomStrechButton.Size = new System.Drawing.Size(23, 22);
            this.zoomStrechButton.Text = "Zoom";
            this.zoomStrechButton.Click += new System.EventHandler(this.zoomStrechButton_Click);
            // 
            // exploreButton
            // 
            this.exploreButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.exploreButton.Enabled = false;
            this.exploreButton.Image = ((System.Drawing.Image)(resources.GetObject("exploreButton.Image")));
            this.exploreButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exploreButton.Name = "exploreButton";
            this.exploreButton.Size = new System.Drawing.Size(23, 22);
            this.exploreButton.Text = "Explore";
            this.exploreButton.Click += new System.EventHandler(this.exploreButton_Click);
            // 
            // picturePanel
            // 
            this.picturePanel.AutoScroll = true;
            this.picturePanel.Controls.Add(this.pictureBox);
            this.picturePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picturePanel.Location = new System.Drawing.Point(0, 25);
            this.picturePanel.Name = "picturePanel";
            this.picturePanel.Size = new System.Drawing.Size(345, 310);
            this.picturePanel.TabIndex = 1;
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(46, 44);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // PictureWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picturePanel);
            this.Controls.Add(this.toolStrip);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PictureWindow";
            this.Size = new System.Drawing.Size(345, 335);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.picturePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.Panel picturePanel;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ToolStripButton zoomStrechButton;
        private System.Windows.Forms.ToolStripButton exploreButton;
    }
}