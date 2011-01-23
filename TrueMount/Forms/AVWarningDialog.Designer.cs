namespace TrueMount.Forms
{
    partial class AVWarningDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AVWarningDialog));
            this.linkLabelVirusChief = new System.Windows.Forms.LinkLabel();
            this.labelHeading = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.labelMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // linkLabelVirusChief
            // 
            resources.ApplyResources(this.linkLabelVirusChief, "linkLabelVirusChief");
            this.linkLabelVirusChief.Name = "linkLabelVirusChief";
            this.linkLabelVirusChief.TabStop = true;
            this.linkLabelVirusChief.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelVirusChief_LinkClicked);
            // 
            // labelHeading
            // 
            resources.ApplyResources(this.labelHeading, "labelHeading");
            this.labelHeading.Name = "labelHeading";
            // 
            // pictureBox3
            // 
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.Image = global::TrueMount.Properties.Resources._1295709941_head;
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // labelMessage
            // 
            resources.ApplyResources(this.labelMessage, "labelMessage");
            this.labelMessage.Name = "labelMessage";
            // 
            // AVWarningDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.linkLabelVirusChief);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AVWarningDialog";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AVWarningDialog_FormClosing);
            this.Click += new System.EventHandler(this.AVWarningDialog_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabelVirusChief;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label labelMessage;
    }
}