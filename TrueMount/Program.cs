using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;
using System.IO;
using TrueMount.Properties;

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

            IEnumerable<Configuration> result =
                from Configuration c in config_db
                select c;

            if (result.Count() > 0)
                config = (Configuration)result.First();
            else
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
