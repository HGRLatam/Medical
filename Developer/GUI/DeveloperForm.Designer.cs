﻿namespace Medical.GUI
{
    partial class DeveloperForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeveloperForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distortionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.newDistortion = new System.Windows.Forms.ToolStripMenuItem();
            this.openDistortion = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDistortion = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDistortionAs = new System.Windows.Forms.ToolStripMenuItem();
            this.sequenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSequence = new System.Windows.Forms.ToolStripMenuItem();
            this.openSequence = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSequence = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSequenceAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oneWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.twoWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.threeWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fourWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kryptonManager = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            this.toolStripContainer.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 312);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(521, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(521, 1);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(521, 26);
            this.toolStripContainer.TabIndex = 1;
            this.toolStripContainer.Text = "toolStripContainer";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.distortionToolStripMenuItem,
            this.sequenceToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(521, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.newToolStripMenuItem.Text = "Change Scene";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // distortionToolStripMenuItem
            // 
            this.distortionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveStateToolStripMenuItem,
            this.toolStripSeparator1,
            this.newDistortion,
            this.openDistortion,
            this.saveDistortion,
            this.saveDistortionAs});
            this.distortionToolStripMenuItem.Name = "distortionToolStripMenuItem";
            this.distortionToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.distortionToolStripMenuItem.Text = "Distortion";
            // 
            // saveStateToolStripMenuItem
            // 
            this.saveStateToolStripMenuItem.Name = "saveStateToolStripMenuItem";
            this.saveStateToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveStateToolStripMenuItem.Text = "Add Current State";
            this.saveStateToolStripMenuItem.Click += new System.EventHandler(this.addStateToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // newDistortion
            // 
            this.newDistortion.Name = "newDistortion";
            this.newDistortion.Size = new System.Drawing.Size(168, 22);
            this.newDistortion.Text = "New";
            this.newDistortion.Click += new System.EventHandler(this.newDistortion_Click);
            // 
            // openDistortion
            // 
            this.openDistortion.Name = "openDistortion";
            this.openDistortion.Size = new System.Drawing.Size(168, 22);
            this.openDistortion.Text = "Open...";
            this.openDistortion.Click += new System.EventHandler(this.openDistortion_Click);
            // 
            // saveDistortion
            // 
            this.saveDistortion.Name = "saveDistortion";
            this.saveDistortion.Size = new System.Drawing.Size(168, 22);
            this.saveDistortion.Text = "Save";
            this.saveDistortion.Click += new System.EventHandler(this.saveDistortion_Click);
            // 
            // saveDistortionAs
            // 
            this.saveDistortionAs.Name = "saveDistortionAs";
            this.saveDistortionAs.Size = new System.Drawing.Size(168, 22);
            this.saveDistortionAs.Text = "Save As...";
            this.saveDistortionAs.Click += new System.EventHandler(this.saveDistortionAs_Click);
            // 
            // sequenceToolStripMenuItem
            // 
            this.sequenceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSequence,
            this.openSequence,
            this.saveSequence,
            this.saveSequenceAs});
            this.sequenceToolStripMenuItem.Name = "sequenceToolStripMenuItem";
            this.sequenceToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.sequenceToolStripMenuItem.Text = "Sequence";
            // 
            // newSequence
            // 
            this.newSequence.Name = "newSequence";
            this.newSequence.Size = new System.Drawing.Size(123, 22);
            this.newSequence.Text = "New";
            this.newSequence.Click += new System.EventHandler(this.newSequence_Click);
            // 
            // openSequence
            // 
            this.openSequence.Name = "openSequence";
            this.openSequence.Size = new System.Drawing.Size(123, 22);
            this.openSequence.Text = "Open...";
            this.openSequence.Click += new System.EventHandler(this.openSequence_Click);
            // 
            // saveSequence
            // 
            this.saveSequence.Name = "saveSequence";
            this.saveSequence.Size = new System.Drawing.Size(123, 22);
            this.saveSequence.Text = "Save";
            this.saveSequence.Click += new System.EventHandler(this.saveSequence_Click);
            // 
            // saveSequenceAs
            // 
            this.saveSequenceAs.Name = "saveSequenceAs";
            this.saveSequenceAs.Size = new System.Drawing.Size(123, 22);
            this.saveSequenceAs.Text = "Save As...";
            this.saveSequenceAs.Click += new System.EventHandler(this.saveSequenceAs_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layoutToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // layoutToolStripMenuItem
            // 
            this.layoutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oneWindowToolStripMenuItem,
            this.twoWindowsToolStripMenuItem,
            this.threeWindowsToolStripMenuItem,
            this.fourWindowToolStripMenuItem});
            this.layoutToolStripMenuItem.Name = "layoutToolStripMenuItem";
            this.layoutToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.layoutToolStripMenuItem.Text = "Layout";
            // 
            // oneWindowToolStripMenuItem
            // 
            this.oneWindowToolStripMenuItem.Name = "oneWindowToolStripMenuItem";
            this.oneWindowToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.oneWindowToolStripMenuItem.Text = "One Window";
            this.oneWindowToolStripMenuItem.Click += new System.EventHandler(this.oneWindowToolStripMenuItem_Click);
            // 
            // twoWindowsToolStripMenuItem
            // 
            this.twoWindowsToolStripMenuItem.Name = "twoWindowsToolStripMenuItem";
            this.twoWindowsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.twoWindowsToolStripMenuItem.Text = "Two Windows";
            this.twoWindowsToolStripMenuItem.Click += new System.EventHandler(this.twoWindowsToolStripMenuItem_Click);
            // 
            // threeWindowsToolStripMenuItem
            // 
            this.threeWindowsToolStripMenuItem.Name = "threeWindowsToolStripMenuItem";
            this.threeWindowsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.threeWindowsToolStripMenuItem.Text = "Three Windows";
            this.threeWindowsToolStripMenuItem.Click += new System.EventHandler(this.threeWindowsToolStripMenuItem_Click);
            // 
            // fourWindowToolStripMenuItem
            // 
            this.fourWindowToolStripMenuItem.Name = "fourWindowToolStripMenuItem";
            this.fourWindowToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.fourWindowToolStripMenuItem.Text = "Four Windows";
            this.fourWindowToolStripMenuItem.Click += new System.EventHandler(this.fourWindowToolStripMenuItem_Click);
            // 
            // kryptonManager
            // 
            this.kryptonManager.GlobalPaletteMode = ComponentFactory.Krypton.Toolkit.PaletteModeManager.ProfessionalSystem;
            // 
            // DeveloperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 334);
            this.Controls.Add(this.toolStripContainer);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DeveloperForm";
            this.Text = "Articulometrics Developer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distortionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oneWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem twoWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem threeWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fourWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDistortion;
        private System.Windows.Forms.ToolStripMenuItem saveDistortion;
        private System.Windows.Forms.ToolStripMenuItem saveDistortionAs;
        private System.Windows.Forms.ToolStripMenuItem sequenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSequence;
        private System.Windows.Forms.ToolStripMenuItem newSequence;
        private System.Windows.Forms.ToolStripMenuItem saveSequence;
        private System.Windows.Forms.ToolStripMenuItem saveSequenceAs;
        private System.Windows.Forms.ToolStripMenuItem newDistortion;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;

    }
}