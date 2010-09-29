using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Diagnostics;

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
            ResourceManager langRes = new ResourceManager("TrueMount.LanguageDictionary", typeof(TrueMountMainWindow).Assembly);

            if (config.CheckForUpdates)
            {
                AutoUpdater updater = new AutoUpdater();
                if (updater.DownloadVersionInfo())
                    if (updater.NewVersionAvailable)
                        if (MessageBox.Show(langRes.GetString("MsgTNewVersion"), langRes.GetString("MsgHNewVersion"),
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            new UpdateProgressDialog().ShowDialog();
            }

            // clean old updates
            if (Directory.Exists(Configuration.UpdateSavePath))
                Directory.Delete(Configuration.UpdateSavePath, true);
            string updaterFile = Path.Combine(Configuration.CurrentApplicationPath, "updater.exe");
            if (File.Exists(updaterFile))
                File.Delete(updaterFile);

            // use mutex to test, if application has been started bevore
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
