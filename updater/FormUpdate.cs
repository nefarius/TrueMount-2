using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Windows.Forms;

namespace updater
{
    public partial class FormUpdate : Form
    {
        private string sourcePath = string.Empty;
        private string destinationPath = string.Empty;
        private TextWriter log = null;
        private List<string> data = new List<string>();

        public FormUpdate()
        {
            // try to log errors to a file
            try
            {
                log = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "updater.log"));
            }
            catch { MessageBox.Show("Fatal error!"); }

            InitializeComponent();
        }

        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            CopyFolder(sourcePath, destinationPath);
        }

        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Process.Start(Path.Combine(destinationPath, "TrueMount.exe"));
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
            this.Close();
        }

        private void WriteExceptionLog(Exception ex)
        {
            WriteLog(ex.Message + Environment.NewLine + ex.StackTrace);
        }

        private void WriteLog(String line)
        {
            log.WriteLine(DateTime.Now.ToShortTimeString() + " - " + line);
        }

        private void FormUpdate_Load(object sender, EventArgs e)
        {
            // connect to the parent server instance and get the update paths
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream("TrueMountUpdater"))
            {
                pipeClient.Connect();

                using (StreamReader inStream = new StreamReader(pipeClient))
                {
                    this.sourcePath = inStream.ReadLine();
                    this.destinationPath = inStream.ReadLine();
                }
            }

            // begin with the update
            this.backgroundWorkerUpdate.RunWorkerAsync();
        }

        /// <summary>
        /// Copies a folder recursively.
        /// </summary>
        /// <param name="sourceFolder">The source directory.</param>
        /// <param name="destFolder">The target directory.</param>
        private void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, true);
                WriteLog(file + " copied to " + destFolder);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
                WriteLog(folder + " copied to " + destFolder);
            }
        }

        private void FormUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            // close the log if opened
            if (log != null)
                log.Close();
        }
    }
}
