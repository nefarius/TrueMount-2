﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace updater
{
    public partial class FormUpdate : Form
    {
        private string sourcePath = string.Empty;
        private string destinationPath = string.Empty;
        private TextWriter log = null;

        public FormUpdate(string[] args)
        {
            try
            {
                log = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "updater.log"));
            }
            catch { MessageBox.Show("Fatal error!"); }

            if (args.Count() == 2)
            {
                this.sourcePath = args[0];
                this.destinationPath = args[1];
            }
            else
            {
                log.WriteLine(DateTime.Now.ToShortTimeString() + " - no cli options. exiting.");
                log.Close();
                Environment.Exit(1);
            }

            InitializeComponent();
        }

        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            EmptyFolder(destinationPath);
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
            this.backgroundWorkerUpdate.RunWorkerAsync();
        }

        private void EmptyFolder(String directoryName)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryName);
            foreach (FileInfo file in directory.GetFiles())
            {
                try
                {
                    file.Delete();
                    WriteLog(file.FullName + " deleted.");
                }
                catch (Exception ex) { WriteExceptionLog(ex); }
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                try
                {
                    directory.Delete(true);
                    WriteLog(subDirectory.FullName + " deleted.");
                }
                catch (Exception ex) { WriteExceptionLog(ex); }
            }
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