using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Diagnostics;
using System.IO.Pipes;

namespace TrueMount
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // load configuration
            Configuration config = Configuration.OpenConfiguration();
            // load languages
            ResourceManager langRes = Configuration.LanguageDictionary;

            // if this instance is launched from the update directry, init an update
            if (Configuration.IsUpdate)
            {
                string lastAppStartPath = Path.GetDirectoryName(config.ApplicationLocation);
                try
                {
                    String updaterPath = Path.Combine(Configuration.UpdateSavePath, "updater.exe");
                    Process.Start(updaterPath);
                }
                catch
                {
                    MessageBox.Show(langRes.GetString("MsgTUpdateFail"), langRes.GetString("MsgHUpdateFail"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("TrueMountUpdater"))
                {
                    pipeServer.WaitForConnection();

                    using (StreamWriter outStream = new StreamWriter(pipeServer))
                    {
                        outStream.WriteLine(Configuration.UpdateSavePath);
                        outStream.WriteLine(lastAppStartPath);
                        outStream.Flush();
                    }
                }
                return;
            }

            if (config.CheckForUpdates)
            {
                AutoUpdater updater = new AutoUpdater();
                if (updater.DownloadVersionInfo())
                    if (updater.NewVersionAvailable)
                        new UpdateDialog(updater).ShowDialog();
            }

            // clean old updates
            if (Directory.Exists(Configuration.UpdateSavePath))
                Directory.Delete(Configuration.UpdateSavePath, true);
            string updaterFile = Path.Combine(Configuration.CurrentApplicationPath, "updater.exe");
            if (File.Exists(updaterFile))
                File.Delete(updaterFile);

            // use mutex to test if application has been started bevore
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, config.ApplicationName, out createdNew))
            {
                if (config.OnlyOneInstance)
                {
                    if (!createdNew)
                    {
                        // only one application is usefull, inform the user and exit
                        MessageBox.Show(langRes.GetString("MsgTInsranceInfo"), langRes.GetString("MsgHInsranceInfo"),
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                Application.Run(new TrueMountMainWindow());
            }
        }
    }
}
