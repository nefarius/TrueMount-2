using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
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

        public FormUpdate(string[] args)
        {
            try
            {
                log = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "updater.log"));
            }
            catch { MessageBox.Show("Fatal error!"); }

            // backwards compatibility
            if (args.Count() == 2)
            {
                if (!string.IsNullOrEmpty(args[0]))
                    this.sourcePath = args[0];
                if (!string.IsNullOrEmpty(args[1]))
                    this.destinationPath = args[1];
            }

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
            log.WriteLine(DateTime.Now.ToShortTimeString() + " - " + ex.Message +
                Environment.NewLine + ex.StackTrace);
        }

        private void WriteLog(String line)
        {
            log.WriteLine(DateTime.Now.ToShortTimeString() + " - " + line);
        }

        private void FormUpdate_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.sourcePath) && string.IsNullOrEmpty(this.destinationPath))
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream("TrueMountUpdater"))
                {
                    pipeClient.Connect();

                    using (StreamReader inStream = new StreamReader(pipeClient))
                    {
                        this.sourcePath = inStream.ReadLine();
                        this.destinationPath = inStream.ReadLine();
                    }
                }

            this.backgroundWorkerUpdate.RunWorkerAsync();
        }

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
            if (log != null)
                log.Close();
        }
    }
}
