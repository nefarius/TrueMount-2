using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Resources;

namespace TrueMount
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // load configuration
            Configuration config = Configuration.OpenConfiguration();
            // load languages
            ResourceManager langRes = new ResourceManager("TrueMount.LanguageDictionary", typeof(TrueMountMainWindow).Assembly);

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

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TrueMountMainWindow());
            }
        }
    }
}
