using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace updater
{
    partial class UpdateDialog : Form
    {
        private AutoUpdater updater = null;
        private ResourceManager langRes = null;
        private delegate void SetProgressInfo(int downloadInfo);
        private bool downloadSuccess = false;
        private bool silent = false;
        private string updateSavePath = string.Empty;
        private string applicationPath = string.Empty;

        /// <summary>
        /// Creates new update dialog.
        /// </summary>
        /// <param name="updater">The reference of an AutoUpdater object.</param>
        public UpdateDialog()
        {
            // load languages
            langRes = new ResourceManager("updater.LanguageDictionary", Assembly.GetExecutingAssembly());
            // get full path to current truemount assembly to fetch version information
            String mainAssembly = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TrueMount.exe");
            // init updater
            this.updater = new AutoUpdater(mainAssembly);
            // register download progress handler
            updater.OnDownloadProgressChanged += new AutoUpdater.OnDownloadProgressChangedEventHandler(updater_OnDownloadProgressChanged);

            InitializeComponent();
        }

        /// <summary>
        /// Executes code on form load.
        /// </summary>
        private void UpdateProgressDialog_Load(object sender, EventArgs e)
        {
            // connect to the parent server instance and get the update options
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream("TrueMountUpdater"))
            {
                try
                {
                    pipeClient.Connect(3000);

                    using (StreamReader inStream = new StreamReader(pipeClient))
                    {
                        this.silent = bool.Parse(inStream.ReadLine());
                        this.updateSavePath = inStream.ReadLine();
                        this.applicationPath = inStream.ReadLine();
                    }
                }
                // no parent process is running and serving
                catch (TimeoutException)
                {
                    Environment.Exit(1);
                }
            }

            // if no new version is available, inform the user (or not) and exit
            if (!updater.NewVersionAvailable)
            {
                if (!silent)
                {
                    MessageBox.Show(langRes.GetString("MsgTNoNewVersion"), langRes.GetString("MsgHNoNewVersion"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }

            // read and set change log
            richTextBoxChanges.Text = updater.ChangeLog;
            labelChangeLog.Text += updater.NewVersion;
            // init height to dialog box stale
            this.Height = panelUpdateQuestion.Height + 40;
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
            else
            {
                // only update progress if values are different
                if (downloadProgress > this.progressBarDownload.Value)
                {
                    this.progressBarDownload.Value = downloadProgress;
                    this.labelCurrentAction.Text = string.Format(langRes.GetString("DownloadProgress"), downloadProgress);
                }
            }
        }

        /// <summary>
        /// Launches the download thread.
        /// </summary>
        private void backgroundWorkerDownload_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Text = langRes.GetString("TitleDownloading");
            downloadSuccess = updater.DownloadNewVersion(this.updateSavePath);
        }

        /// <summary>
        /// Gets called after download is complete.
        /// </summary>
        private void backgroundWorkerDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // download must be successfull
            if (downloadSuccess)
            {
                // adjust form title
                this.Text = langRes.GetString("TitleFinished");
                // success message
                MessageBox.Show(langRes.GetString("MsgTUDownloadOk"), langRes.GetString("MsgHUDownloadOk"),
                       MessageBoxButtons.OK, MessageBoxIcon.Information);

                // execute new downloaded version with "patch" flag
                Process updateProc = new Process();
                updateProc.StartInfo.UseShellExecute = false;
                updateProc.StartInfo.FileName = Path.Combine(this.updateSavePath, "updater.exe");
                updateProc.StartInfo.Arguments = "patch";
                // on Vista/7/2008 ask for admin permissions
                if (Environment.OSVersion.Version.Major >= 6)
                    updateProc.StartInfo.Verb = "runas";

                try
                {
                    // start downloaded version of the updater with "patch" flag
                    updateProc.Start();
                    // wait and serve important path data to the updater client
                    using (NamedPipeServerStream outPipe = new NamedPipeServerStream("TrueMountUpdater"))
                    {
                        outPipe.WaitForConnection();

                        using (StreamWriter outStream = new StreamWriter(outPipe))
                        {
                            outStream.WriteLine(this.updateSavePath);
                            outStream.WriteLine(this.applicationPath);
                            outStream.Flush();
                        }
                    }
                    // after successfull connect exit immediately
                    Environment.Exit(0);
                }
                catch
                {
                    this.Text = langRes.GetString("TitleFailed");
                    MessageBox.Show(langRes.GetString("MsgTUpdateFail"), langRes.GetString("MsgHUpdateFail"),
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Close();
            }
            else
            {
                this.Text = langRes.GetString("TitleFailed");
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
            this.Text = labelCurrentAction.Text;
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
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
