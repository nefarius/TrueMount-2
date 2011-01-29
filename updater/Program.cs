using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Resources;
using System.Reflection;
using System.Security;

namespace updater
{
    static class Program
    {
        private static ResourceManager langRes;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // load languages
            langRes = new ResourceManager("updater.LanguageDictionary", Assembly.GetExecutingAssembly());

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
                                // wait 3 seconds (must be enough)
                                inPipe.Connect(3000);

                                // read directory information
                                using (StreamReader inStream = new StreamReader(inPipe))
                                {
                                    srcDir = inStream.ReadLine();
                                    dstDir = inStream.ReadLine();
                                }
                            }

                            if (string.IsNullOrEmpty(srcDir) || string.IsNullOrEmpty(dstDir))
                            {
                                MessageBox.Show(langRes.GetString("MsgTErrDirData"), langRes.GetString("MsgHErrDirData"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return 2;
                            }

                            // wait till the server updater process closed himself
                            while (Process.GetProcessesByName("updater").Count() > 1) Thread.Sleep(1000);

                            try
                            {
                                // patch the current version
                                AutoUpdater.CopyFolder(srcDir, dstDir);
                            }
                            catch
                            {
                                MessageBox.Show(string.Format(langRes.GetString("MsgTErrWritePerm"), dstDir), 
                                    langRes.GetString("MsgHErrWritePerm"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return 2;
                            }

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
