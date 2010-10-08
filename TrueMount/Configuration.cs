using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            // initiate empty lists
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

        public static Version CurrentVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public static string WebUserAgent
        {
            get { return "TrueMount/" + CurrentVersion; }
        }

        /// <summary>
        /// Is this instance launched from the update directory?
        /// </summary>
        public static bool IsUpdate
        {
            get { return (Path.GetDirectoryName(CurrentApplicationLocation).Equals(UpdateSavePath)); }
        }

        /// <summary>
        /// Location of the XML file containing the update information.
        /// </summary>
        public static string UpdateVersionFileURL
        {
            get
            {
#if DEBUG
                return "http://localhost/truemountversion.xml";
#endif
                return "http://nefarius.darkhosters.net/_media/windows/TrueMountVersion.xml";
            }
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
        public bool IsUsbDeviceConfigOk
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
            if (File.Exists(ConfigurationFile))
            {
                FileStream fsFetch = new FileStream(ConfigurationFile, FileMode.Open);
                BinaryFormatter binFormat = new BinaryFormatter();
                Configuration stored = (Configuration)binFormat.Deserialize(fsFetch);
                fsFetch.Close();
                // compatibility workaround: initiates every null reference to avoid crashes
                stored.InitReferences();
                return stored;
            }
            return new Configuration();
        }
    }
}
