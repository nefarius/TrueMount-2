using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

namespace TrueMount
{
    [Serializable()]
    class Configuration
    {
        public bool FirstStart { get; set; }
        public bool AutostartService { get; set; }
        public bool StartSilent { get; set; }
        public bool ShowSplashScreen { get; set; }
        public bool OnlyOneInstance { get; set; }
        public TrueCryptConfig TrueCrypt { get; set; }
        public List<UsbKeyDevice> KeyDevices { get; set; }
        public List<EncryptedDiskPartition> EncryptedDiskPartitions { get; set; }
        public List<EncryptedContainerFile> EncryptedContainerFiles { get; set; }
        public CultureInfo Language { get; set; }
        public bool IgnoreAssignedDriveLetters { get; set; }
        public bool ForceUnmount { get; set; }
        public bool UnmountWarning { get; set; }
        public bool DisableBalloons { get; set; }
        public int BalloonTimePeriod { get; set; }
        public string ApplicationLocation { get; set; }
        public bool CheckForUpdates { get; set; }

        private const string RUN_LOCATION = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string VALUE_NAME = "TrueMount by Nefarius";

        public Configuration()
        {
            // initiate empty references
            TrueCrypt = new TrueCryptConfig();
            KeyDevices = new List<UsbKeyDevice>();
            EncryptedDiskPartitions = new List<EncryptedDiskPartition>();
            EncryptedContainerFiles = new List<EncryptedContainerFile>();
            
            // set default values
            ShowSplashScreen = true;
            OnlyOneInstance = true;
            BalloonTimePeriod = 3000;
            FirstStart = true;
            ApplicationLocation = CurrentApplicationLocation;
            CheckForUpdates = true;
        }

        /// <summary>
        /// Checks and initializes all null references.
        /// </summary>
        public void InitReferences()
        {
            if (TrueCrypt == null)
                TrueCrypt = new TrueCryptConfig();
            if (KeyDevices == null)
                KeyDevices = new List<UsbKeyDevice>();
            if (EncryptedDiskPartitions == null)
                EncryptedDiskPartitions = new List<EncryptedDiskPartition>();
            if (EncryptedContainerFiles == null)
                EncryptedContainerFiles = new List<EncryptedContainerFile>();
            if (Language == null)
                Language = System.Threading.Thread.CurrentThread.CurrentUICulture;
            if (string.IsNullOrEmpty(ApplicationLocation))
                ApplicationLocation = CurrentApplicationLocation;
        }

        /// <summary>
        /// Path to updater assembly.
        /// </summary>
        public static string UpdaterLocation
        {
            get { return Path.Combine(CurrentApplicationPath, "updater.exe"); }
        }

        /// <summary>
        /// Checks if updater component exists in applications working directory.
        /// </summary>
        public static bool UpdaterExists
        {
            get { return File.Exists(UpdaterLocation); }
        }

        /// <summary>
        /// Returns the current running assembly version information.
        /// </summary>
        public static Version CurrentVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        /// <summary>
        /// Full name of the current running assembly instance.
        /// </summary>
        public static string CurrentApplicationLocation
        {
            get { return Assembly.GetExecutingAssembly().Location; }
        }

        /// <summary>
        /// Path name of the current running assembly instance.
        /// </summary>
        public static string CurrentApplicationPath
        {
            get { return Path.GetDirectoryName(CurrentApplicationLocation); }
        }

        /// <summary>
        /// URL to the projects webpage.
        /// </summary>
        public static string ProjectLocation
        {
            get { return "http://nefarius.darkhosters.net/windows/truemount2#beta"; }
        }

        /// <summary>
        /// Resource reference of available included user interface translations.
        /// </summary>
        public static ResourceManager LanguageDictionary
        {
            get { return new ResourceManager("TrueMount.LanguageDictionary", typeof(TrueMountMainWindow).Assembly); }
        }

        /// <summary>
        /// Directory where the application updates get saved temporary.
        /// </summary>
        public static string UpdateSavePath
        {
            get { return Path.Combine(ConfigurationPath, "update"); }
        }

        /// <summary>
        /// Contains the path where the configuration file and updates are stored.
        /// </summary>
        public static string ConfigurationPath
        {
            get
            {
                string appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrueMount");
#if DEBUG
                appDataDir = Path.Combine(appDataDir, "debug");
#endif
                if (!Directory.Exists(appDataDir))
                    Directory.CreateDirectory(appDataDir);
                return appDataDir;
            }
        }

        /// <summary>
        /// Contains the path of the configuration file.
        /// </summary>
        public static string ConfigurationFile
        {
            get
            {
                return Path.Combine(ConfigurationPath, "config.dat");
            }
        }

        /// <summary>
        /// The Name of the App.
        /// </summary>
        public string ApplicationName
        {
            get { return VALUE_NAME; }
        }

        /// <summary>
        /// Add to windows autostart.
        /// </summary>
        public void SetAutoStart()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.SetValue(VALUE_NAME, CurrentApplicationLocation);
        }

        /// <summary>
        /// Remove from windows autostart.
        /// </summary>
        public void UnSetAutoStart()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.DeleteValue(VALUE_NAME);
        }

        /// <summary>
        /// Checks if application is in autostart list.
        /// </summary>
        public bool IsAutoStartEnabled
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_LOCATION);
                if (key == null)
                    return false;

                string value = (string)key.GetValue(VALUE_NAME);
                if (value == null)
                    return false;
                return (value == CurrentApplicationLocation);
            }
        }

        /// <summary>
        /// Checks if there is at least one valid key device.
        /// </summary>
        public bool IsKeyDeviceConfigOk
        {
            get
            {
                if (KeyDevices.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Saves the given configuration to file.
        /// </summary>
        /// <param name="current">The current active configuration object.</param>
        public static void SaveConfiguration(Configuration current)
        {
            FileStream fsSave = new FileStream(ConfigurationFile, FileMode.Create);
            BinaryFormatter binFormat = new BinaryFormatter();
            binFormat.Serialize(fsSave, current);
            fsSave.Close();
        }

        /// <summary>
        /// Opens the configuration from file.
        /// </summary>
        /// <returns>Returns stored or new empty default configuration reference.</returns>
        public static Configuration OpenConfiguration()
        {
            Configuration stored = new Configuration();
            if (File.Exists(ConfigurationFile))
            {
                FileStream fsFetch = new FileStream(ConfigurationFile, FileMode.Open);
                BinaryFormatter binFormat = new BinaryFormatter();
                try { stored = (Configuration)binFormat.Deserialize(fsFetch); }
                catch { return stored; }
                fsFetch.Close();
                // compatibility workaround: initiates every null reference to avoid crashes
                stored.InitReferences();
            }
            return stored;
        }

        /// <summary>
        /// Tries to start an updater instance and waits for its response.
        /// </summary>
        /// <param name="silent">Set true if you want to suppress dialogs.</param>
        /// <returns>Returns true on successfull update, else false.</returns>
        public bool InvokeUpdateProcess(bool silent = false)
        {
            string lastAppStartPath = Path.GetDirectoryName(this.ApplicationLocation);
            // valid paths are needed to start the updater
            if (!string.IsNullOrEmpty(lastAppStartPath) && !string.IsNullOrEmpty(UpdateSavePath))
            {
                try
                {
                    Process updater = Process.Start(UpdaterLocation);

                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("TrueMountUpdater"))
                    {
                        pipeServer.WaitForConnection();

                        using (StreamWriter outStream = new StreamWriter(pipeServer))
                        {
                            // silent mode?
                            outStream.WriteLine(silent);
                            // directory to store update in
                            outStream.WriteLine(Configuration.UpdateSavePath);
                            // directory to patch
                            outStream.WriteLine(lastAppStartPath);
                            // go!
                            outStream.Flush();
                        }
                    }

                    // wait for updater to finish and continue or exit
                    while (!updater.WaitForExit(1000)) ;
                    if (updater.ExitCode != 2)
                        Environment.Exit(0);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
