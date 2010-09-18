using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Db4objects.Db4o;

namespace TrueMount
{
    static class Program
    {
        private static Configuration config = null;
        private static IObjectContainer config_db = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            config_db = Db4oFactory.OpenFile(Configuration.ConfigDbFile);
            config = config_db.Query<Configuration>().FirstOrDefault();
            if (config == null)
                config = new Configuration();
            config_db.Close();

            // use mutex to test, if application has been started bevore
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, config.ApplicationName, out createdNew))
            {
                if (config.OnlyOneInstance)
                {
                    if (!createdNew)
                    {
                        // only one application is usefull, inform the user and exit
                        MessageBox.Show("There is already an instance of me running!", "Secound start",
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
