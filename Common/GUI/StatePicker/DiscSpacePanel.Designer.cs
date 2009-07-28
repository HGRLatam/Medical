﻿namespace Medical.GUI
{
    partial class DiscSpacePanel
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
            this.rightDiscSpace = new System.Windows.Forms.TrackBar();
            this.leftDiscSpace = new System.Windows.Forms.TrackBar();
            this.horizontalDisc = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.rightDiscSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftDiscSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalDisc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // rightDiscSpace
            // 
            this.rightDiscSpace.LargeChange = 1;
            this.rightDiscSpace.Location = new System.Drawing.Point(15, 24);
            this.rightDiscSpace.Maximum = 4;
            this.rightDiscSpace.Name = "rightDiscSpace";
            this.rightDiscSpace.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.rightDiscSpace.Size = new System.Drawing.Size(45, 104);
            this.rightDiscSpace.TabIndex = 0;
            this.rightDiscSpace.Value = 1;
            // 
            // leftDiscSpace
            // 
            this.leftDiscSpace.LargeChange = 1;
            this.leftDiscSpace.Location = new System.Drawing.Point(334, 24);
            this.leftDiscSpace.Maximum = 4;
            this.leftDiscSpace.Name = "leftDiscSpace";
            this.leftDiscSpace.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.leftDiscSpace.Size = new System.Drawing.Size(45, 104);
            this.leftDiscSpace.TabIndex = 1;
            this.leftDiscSpace.Value = 1;
            // 
            // horizontalDisc
            // 
            this.horizontalDisc.LargeChange = 1;
            this.horizontalDisc.Location = new System.Drawing.Point(135, 176);
            this.horizontalDisc.Maximum = 2;
            this.horizontalDisc.Minimum = -2;
            this.horizontalDisc.Name = "horizontalDisc";
            this.horizontalDisc.Size = new System.Drawing.Size(104, 45);
            this.horizontalDisc.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Right Condyle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(317, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Left Condyle";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(65, 34);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(252, 132);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // DiscSpacePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.horizontalDisc);
            this.Controls.Add(this.leftDiscSpace);
            this.Controls.Add(this.rightDiscSpace);
            this.Name = "DiscSpacePanel";
            this.Size = new System.Drawing.Size(386, 219);
            ((System.ComponentModel.ISupportInitialize)(this.rightDiscSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftDiscSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalDisc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar rightDiscSpace;
        private System.Windows.Forms.TrackBar leftDiscSpace;
        private System.Windows.Forms.TrackBar horizontalDisc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
