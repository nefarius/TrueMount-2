namespace TrueMount
{
    partial class UpdateDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateDialog));
            this.backgroundWorkerDownload = new System.ComponentModel.BackgroundWorker();
            this.labelCurrentAction = new System.Windows.Forms.Label();
            this.progressBarDownload = new System.Windows.Forms.ProgressBar();
            this.buttonPerformUpdate = new System.Windows.Forms.Button();
            this.buttonCancelUpdate = new System.Windows.Forms.Button();
            this.panelUpdateQuestion = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabelViewChanges = new System.Windows.Forms.LinkLabel();
            this.panelProgress = new System.Windows.Forms.Panel();
            this.panelChanges = new System.Windows.Forms.Panel();
            this.labelChangeLog = new System.Windows.Forms.Label();
            this.richTextBoxChanges = new System.Windows.Forms.RichTextBox();
            this.panelUpdateQuestion.SuspendLayout();
            this.panelProgress.SuspendLayout();
            this.panelChanges.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorkerDownload
            // 
            this.backgroundWorkerDownload.WorkerSupportsCancellation = true;
            this.backgroundWorkerDownload.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDownload_DoWork);
            this.backgroundWorkerDownload.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerDownload_RunWorkerCompleted);
            // 
            // labelCurrentAction
            // 
            resources.ApplyResources(this.labelCurrentAction, "labelCurrentAction");
            this.labelCurrentAction.Name = "labelCurrentAction";
            // 
            // progressBarDownload
            // 
            resources.ApplyResources(this.progressBarDownload, "progressBarDownload");
            this.progressBarDownload.Name = "progressBarDownload";
            // 
            // buttonPerformUpdate
            // 
            resources.ApplyResources(this.buttonPerformUpdate, "buttonPerformUpdate");
            this.buttonPerformUpdate.Name = "buttonPerformUpdate";
            this.buttonPerformUpdate.UseVisualStyleBackColor = true;
            this.buttonPerformUpdate.Click += new System.EventHandler(this.buttonPerformUpdate_Click);
            // 
            // buttonCancelUpdate
            // 
            resources.ApplyResources(this.buttonCancelUpdate, "buttonCancelUpdate");
            this.buttonCancelUpdate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancelUpdate.Name = "buttonCancelUpdate";
            this.buttonCancelUpdate.UseVisualStyleBackColor = true;
            this.buttonCancelUpdate.Click += new System.EventHandler(this.buttonCancelUpdate_Click);
            // 
            // panelUpdateQuestion
            // 
            resources.ApplyResources(this.panelUpdateQuestion, "panelUpdateQuestion");
            this.panelUpdateQuestion.Controls.Add(this.label1);
            this.panelUpdateQuestion.Controls.Add(this.linkLabelViewChanges);
            this.panelUpdateQuestion.Controls.Add(this.buttonPerformUpdate);
            this.panelUpdateQuestion.Controls.Add(this.buttonCancelUpdate);
            this.panelUpdateQuestion.Name = "panelUpdateQuestion";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // linkLabelViewChanges
            // 
            resources.ApplyResources(this.linkLabelViewChanges, "linkLabelViewChanges");
            this.linkLabelViewChanges.Name = "linkLabelViewChanges";
            this.linkLabelViewChanges.TabStop = true;
            this.linkLabelViewChanges.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelViewChanges_LinkClicked);
            // 
            // panelProgress
            // 
            resources.ApplyResources(this.panelProgress, "panelProgress");
            this.panelProgress.Controls.Add(this.progressBarDownload);
            this.panelProgress.Controls.Add(this.labelCurrentAction);
            this.panelProgress.Name = "panelProgress";
            // 
            // panelChanges
            // 
            resources.ApplyResources(this.panelChanges, "panelChanges");
            this.panelChanges.Controls.Add(this.labelChangeLog);
            this.panelChanges.Controls.Add(this.richTextBoxChanges);
            this.panelChanges.Name = "panelChanges";
            // 
            // labelChangeLog
            // 
            resources.ApplyResources(this.labelChangeLog, "labelChangeLog");
            this.labelChangeLog.Name = "labelChangeLog";
            // 
            // richTextBoxChanges
            // 
            resources.ApplyResources(this.richTextBoxChanges, "richTextBoxChanges");
            this.richTextBoxChanges.BackColor = System.Drawing.Color.White;
            this.richTextBoxChanges.Name = "richTextBoxChanges";
            this.richTextBoxChanges.ReadOnly = true;
            // 
            // UpdateDialog
            // 
            this.AcceptButton = this.buttonPerformUpdate;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancelUpdate;
            this.Controls.Add(this.panelProgress);
            this.Controls.Add(this.panelChanges);
            this.Controls.Add(this.panelUpdateQuestion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UpdateDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.UpdateProgressDialog_Load);
            this.panelUpdateQuestion.ResumeLayout(false);
            this.panelUpdateQuestion.PerformLayout();
            this.panelProgress.ResumeLayout(false);
            this.panelProgress.PerformLayout();
            this.panelChanges.ResumeLayout(false);
            this.panelChanges.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorkerDownload;
        private System.Windows.Forms.Label labelCurrentAction;
        private System.Windows.Forms.ProgressBar progressBarDownload;
        private System.Windows.Forms.Button buttonPerformUpdate;
        private System.Windows.Forms.Button buttonCancelUpdate;
        private System.Windows.Forms.Panel panelUpdateQuestion;
        private System.Windows.Forms.LinkLabel linkLabelViewChanges;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelProgress;
        private System.Windows.Forms.Panel panelChanges;
        private System.Windows.Forms.RichTextBox richTextBoxChanges;
        private System.Windows.Forms.Label labelChangeLog;

    }
}