using System;
using System.IO;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

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

#if !DEBUG
            // if update checking is allowed, do it
            if (config.CheckForUpdates)
                if (Configuration.UpdaterExists)
                    if (!config.InvokeUpdateProcess(true))
                        MessageBox.Show(langRes.GetString("MsgTUpdateFail"), langRes.GetString("MsgHUpdateFail"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

            // clean old updates
            if (Directory.Exists(Configuration.UpdateSavePath))
                Directory.Delete(Configuration.UpdateSavePath, true);
#else
            if (!config.InvokeUpdateProcess(true))
                MessageBox.Show(langRes.GetString("MsgTUpdateFail"), langRes.GetString("MsgHUpdateFail"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif

            // use mutex to test if application has been started bevore
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, config.ApplicationName, out createdNew))
            {
#if !DEBUG
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
#endif

                // we made it to the main window, load it!
                Application.Run(new TrueMountMainWindow());
            }
        }
    }
}
