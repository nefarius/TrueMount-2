using System;
using System.ComponentModel;
using System.Resources;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace TrueMount
{
    partial class UpdateDialog : Form
    {
        AutoUpdater updater = null;
        ResourceManager langRes = null;
        delegate void SetProgressInfo(int downloadInfo);
        bool downloadSuccess = false;

        /// <summary>
        /// Creates new update dialog.
        /// </summary>
        /// <param name="updater">The reference of an AutoUpdater object.</param>
        public UpdateDialog(AutoUpdater updater)
        {
            // load languages
            langRes = Configuration.LanguageDictionary;
            // init updater
            this.updater = updater;
            // register download progress handler
            updater.OnDownloadProgressChanged += new AutoUpdater.OnDownloadProgressChangedEventHandler(updater_OnDownloadProgressChanged);

            InitializeComponent();
        }

        /// <summary>
        /// Reports new download percentage if progress has changed.
        /// </summary>
        /// <param name="downloadProgress">The download progress in percent.</param>
        void updater_OnDownloadProgressChanged(int downloadProgress)
        {
            // thread-safe call
            if (this.progressBarDownload.InvokeRequired || this.labelCurrentAction.InvokeRequired)
            {
                SetProgressInfo pInfo = new SetProgressInfo(updater_OnDownloadProgressChanged);
                this.Invoke(pInfo, downloadProgress);
            }

            // only update progress if values are different
            if (downloadProgress > this.progressBarDownload.Value)
            {
                this.progressBarDownload.Value = downloadProgress;
                this.labelCurrentAction.Text = string.Format(langRes.GetString("DownloadProgress"), downloadProgress);
            }
        }

        /// <summary>
        /// Executes code on form load.
        /// </summary>
        private void UpdateProgressDialog_Load(object sender, EventArgs e)
        {
            // read and set change log
            richTextBoxChanges.Text = updater.ChangeLog;
            labelChangeLog.Text += updater.NewVersion;
            // init height to dialog box stale
            this.Height = panelUpdateQuestion.Height + 40;
        }

        /// <summary>
        /// Launches the download thread.
        /// </summary>
        private void backgroundWorkerDownload_DoWork(object sender, DoWorkEventArgs e)
        {
            downloadSuccess = updater.DownloadNewVersion();
        }

        /// <summary>
        /// Gets called after download is complete.
        /// </summary>
        private void backgroundWorkerDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // download must be successfull
            if (downloadSuccess)
            {
                // success message
                MessageBox.Show(langRes.GetString("MsgTUDownloadOk"), langRes.GetString("MsgHUDownloadOk"),
                       MessageBoxButtons.OK, MessageBoxIcon.Information);

                // get last saved configuration
                Configuration config = Configuration.OpenConfiguration();

                // execute new downloaded version with "update" flag
                Process updater = new Process();
                updater.StartInfo.UseShellExecute = false;
                updater.StartInfo.FileName = Path.Combine(Configuration.UpdateSavePath, "TrueMount.exe");
                updater.StartInfo.Arguments = "update";

                try
                {
                    updater.Start();
                    Environment.Exit(0);
                }
                catch
                {
                    MessageBox.Show(langRes.GetString("MsgTUpdateFail"), langRes.GetString("MsgHUpdateFail"),
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Close();
            }
            else
            {
                MessageBox.Show(langRes.GetString("MsgTUDownloadFail"), langRes.GetString("MsgHUDownloadFail"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void buttonPerformUpdate_Click(object sender, EventArgs e)
        {
            // adjust GUI
            panelChanges.Visible = false;
            panelUpdateQuestion.Visible = false;
            panelProgress.Visible = true;
            this.Height = (panelProgress.Height + 40);

            // start update download
            labelCurrentAction.Text = langRes.GetString("BeginUpdate");
            this.backgroundWorkerDownload.RunWorkerAsync();
        }

        private void linkLabelViewChanges_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // adjust GUI
            linkLabelViewChanges.Enabled = false;
            this.Height = 460;
            richTextBoxChanges.Text = updater.ChangeLog;
            panelChanges.Visible = true;
        }

        private void buttonCancelUpdate_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
