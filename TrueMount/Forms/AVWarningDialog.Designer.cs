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
            this.pictureBoxSkull = new System.Windows.Forms.PictureBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.linkLabelVirusTotal = new System.Windows.Forms.LinkLabel();
            this.labelAdvice = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSkull)).BeginInit();
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
            // pictureBoxSkull
            // 
            resources.ApplyResources(this.pictureBoxSkull, "pictureBoxSkull");
            this.pictureBoxSkull.Image = global::TrueMount.Properties.Resources._1295709941_head;
            this.pictureBoxSkull.Name = "pictureBoxSkull";
            this.pictureBoxSkull.TabStop = false;
            // 
            // labelMessage
            // 
            resources.ApplyResources(this.labelMessage, "labelMessage");
            this.labelMessage.Name = "labelMessage";
            // 
            // linkLabelVirusTotal
            // 
            resources.ApplyResources(this.linkLabelVirusTotal, "linkLabelVirusTotal");
            this.linkLabelVirusTotal.Name = "linkLabelVirusTotal";
            this.linkLabelVirusTotal.TabStop = true;
            this.linkLabelVirusTotal.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelVirusTotal_LinkClicked);
            // 
            // labelAdvice
            // 
            resources.ApplyResources(this.labelAdvice, "labelAdvice");
            this.labelAdvice.Name = "labelAdvice";
            // 
            // AVWarningDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelAdvice);
            this.Controls.Add(this.linkLabelVirusTotal);
            this.Controls.Add(this.pictureBoxSkull);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.linkLabelVirusChief);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AVWarningDialog";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSkull)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabelVirusChief;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.PictureBox pictureBoxSkull;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.LinkLabel linkLabelVirusTotal;
        private System.Windows.Forms.Label labelAdvice;
    }
}