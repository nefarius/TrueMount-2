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
    public partial class UpdateProgressDialog : Form
    {
        AutoUpdater updater = null;
        ResourceManager langRes = null;
        delegate void SetProgressInfo(int downloadInfo);
        bool downloadSuccess = false;

        public UpdateProgressDialog()
        {
            langRes = Configuration.LanguageDictionary;
            updater = new AutoUpdater();
            updater.OnDownloadProgressChanged += new AutoUpdater.OnDownloadProgressChangedEventHandler(updater_OnDownloadProgressChanged);

            InitializeComponent();
        }

        void updater_OnDownloadProgressChanged(int downloadProgress)
        {
            if (this.progressBarDownload.InvokeRequired || this.labelCurrentAction.InvokeRequired)
            {
                SetProgressInfo pInfo = new SetProgressInfo(updater_OnDownloadProgressChanged);
                this.Invoke(pInfo, downloadProgress);
            }
            if (downloadProgress > this.progressBarDownload.Value)
                this.progressBarDownload.Value = downloadProgress;
            this.labelCurrentAction.Text = string.Format(langRes.GetString("DownloadProgress"), downloadProgress);
        }

        private void UpdateProgressDialog_Load(object sender, EventArgs e)
        {
            this.labelCurrentAction.Text = langRes.GetString("BeginUpdate");
            this.backgroundWorkerDownload.RunWorkerAsync();
        }

        private void backgroundWorkerDownload_DoWork(object sender, DoWorkEventArgs e)
        {
            if (updater.DownloadVersionInfo())
                downloadSuccess = updater.DownloadNewVersion();
        }

        private void backgroundWorkerDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (downloadSuccess)
            {
                MessageBox.Show(langRes.GetString("MsgTUDownloadOk"), langRes.GetString("MsgHUDownloadOk"),
                       MessageBoxButtons.OK, MessageBoxIcon.Information);

                Configuration config = Configuration.OpenConfiguration();

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
    }
}
