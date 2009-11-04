﻿namespace Medical.GUI
{
    partial class SimpleMandibleControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleMandibleControl));
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.bothForwardBack = new System.Windows.Forms.TrackBar();
            this.leftForwardBack = new System.Windows.Forms.TrackBar();
            this.openTrackBar = new System.Windows.Forms.TrackBar();
            this.distortionButton = new System.Windows.Forms.Button();
            this.rightForwardBack = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bothForwardBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftForwardBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.openTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightForwardBack)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(4, 256);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 35;
            this.label8.Text = "Both";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(4, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Left";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(4, 232);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Right";
            // 
            // bothForwardBack
            // 
            this.bothForwardBack.LargeChange = 2000;
            this.bothForwardBack.Location = new System.Drawing.Point(35, 251);
            this.bothForwardBack.Maximum = 10000;
            this.bothForwardBack.Name = "bothForwardBack";
            this.bothForwardBack.Size = new System.Drawing.Size(164, 45);
            this.bothForwardBack.SmallChange = 1000;
            this.bothForwardBack.TabIndex = 23;
            this.bothForwardBack.TickFrequency = 10000;
            this.bothForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // leftForwardBack
            // 
            this.leftForwardBack.LargeChange = 2000;
            this.leftForwardBack.Location = new System.Drawing.Point(35, 1);
            this.leftForwardBack.Maximum = 10000;
            this.leftForwardBack.Name = "leftForwardBack";
            this.leftForwardBack.Size = new System.Drawing.Size(164, 45);
            this.leftForwardBack.SmallChange = 1000;
            this.leftForwardBack.TabIndex = 22;
            this.leftForwardBack.TickFrequency = 10000;
            this.leftForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // openTrackBar
            // 
            this.openTrackBar.LargeChange = 2000;
            this.openTrackBar.Location = new System.Drawing.Point(209, 49);
            this.openTrackBar.Maximum = 3000;
            this.openTrackBar.Minimum = -10000;
            this.openTrackBar.Name = "openTrackBar";
            this.openTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.openTrackBar.Size = new System.Drawing.Size(45, 180);
            this.openTrackBar.SmallChange = 1000;
            this.openTrackBar.TabIndex = 26;
            this.openTrackBar.TickFrequency = 10000;
            this.openTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // distortionButton
            // 
            this.distortionButton.Location = new System.Drawing.Point(56, 278);
            this.distortionButton.Name = "distortionButton";
            this.distortionButton.Size = new System.Drawing.Size(125, 23);
            this.distortionButton.TabIndex = 20;
            this.distortionButton.Text = "Reset";
            this.distortionButton.UseVisualStyleBackColor = true;
            this.distortionButton.Click += new System.EventHandler(this.distortionButton_Click);
            // 
            // rightForwardBack
            // 
            this.rightForwardBack.LargeChange = 2000;
            this.rightForwardBack.Location = new System.Drawing.Point(35, 227);
            this.rightForwardBack.Maximum = 10000;
            this.rightForwardBack.Name = "rightForwardBack";
            this.rightForwardBack.Size = new System.Drawing.Size(164, 45);
            this.rightForwardBack.SmallChange = 1000;
            this.rightForwardBack.TabIndex = 21;
            this.rightForwardBack.TickFrequency = 10000;
            this.rightForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(1, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(206, 192);
            this.panel1.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(156, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Opening";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Left";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Right";
            // 
            // SimpleMandibleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonImageIndex = 7;
            this.ButtonText = "Mandible Control";
            this.ClientSize = new System.Drawing.Size(396, 384);
            this.Controls.Add(this.distortionButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.bothForwardBack);
            this.Controls.Add(this.leftForwardBack);
            this.Controls.Add(this.openTrackBar);
            this.Controls.Add(this.rightForwardBack);
            this.DockAreas = ((Medical.DockAreas)(((Medical.DockAreas.Float | Medical.DockAreas.DockLeft)
                        | Medical.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SimpleMandibleControl";
            this.ShortcutKey = System.Windows.Forms.Keys.M;
            this.ShowHint = Medical.DockState.DockLeft;
            this.Text = "Mandible Control";
            this.ToolStripName = "Manipulation";
            ((System.ComponentModel.ISupportInitialize)(this.bothForwardBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftForwardBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.openTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightForwardBack)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar bothForwardBack;
        private System.Windows.Forms.TrackBar leftForwardBack;
        private System.Windows.Forms.TrackBar openTrackBar;
        private System.Windows.Forms.Button distortionButton;
        private System.Windows.Forms.TrackBar rightForwardBack;
    }
}