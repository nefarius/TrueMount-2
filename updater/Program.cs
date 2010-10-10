using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace updater
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // parse argument line
            foreach (string item in args)
            {
                switch (item)
                {
                    case "patch":
                        try
                        {
                            string srcDir = string.Empty;
                            string dstDir = string.Empty;
                            // get important directories from shared memory
                            using (NamedPipeClientStream inPipe = new NamedPipeClientStream("TrueMountUpdater"))
                            {
                                // wait 3 secounds (must be enough)
                                inPipe.Connect(3000);

                                // read directory information
                                using (StreamReader inStream = new StreamReader(inPipe))
                                {
                                    srcDir = inStream.ReadLine();
                                    dstDir = inStream.ReadLine();
                                }
                            }

                            // wait till the server updater process closed himself
                            while (Process.GetProcessesByName("updater").Count() > 1) Thread.Sleep(1000);
                            // patch the current version
                            AutoUpdater.CopyFolder(srcDir, dstDir);
                            // launch the new version of truemount
                            Process.Start(Path.Combine(dstDir, "TrueMount.exe"));
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace); }
                        return 0;
                    default:
                        break;
                }
            }

            UpdateDialog updateDialog = new UpdateDialog();
            Application.Run(updateDialog);

            // if no new version available or error occured return 2
            // 0 will close the truemount parent process
            if (updateDialog.DialogResult == DialogResult.Cancel)
                return 2;
            else
                return 0;
        }
    }
}
