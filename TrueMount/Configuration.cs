using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using System.Resources;

namespace TrueMount
{
    [Serializable()]
    class Configuration
    {
        public bool AutostartService { get; set; }
        public bool StartSilent { get; set; }
        public bool ShowSplashScreen { get; set; }
        public bool OnlyOneInstance { get; set; }
        public TrueCryptConfig TrueCrypt { get; set; }
        public List<UsbKeyDevice> KeyDevices { get; set; }
        public List<EncryptedDiskPartition> EncryptedDiskPartitions { get; set; }
        public CultureInfo Language { get; set; }
        public bool IgnoreAssignedDriveLetters { get; set; }
        public bool ForceUnmount { get; set; }
        public bool UnmountWarning { get; set; }
        public bool DisableBalloons { get; set; }

        private const string RUN_LOCATION = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string VALUE_NAME = "TrueMount by Nefarius";

        public Configuration()
        {
            // initiate empty lists
            TrueCrypt = new TrueCryptConfig();
            KeyDevices = new List<UsbKeyDevice>();
            EncryptedDiskPartitions = new List<EncryptedDiskPartition>();

            // set default values
            ShowSplashScreen = true;
            OnlyOneInstance = true;
        }

        public static ResourceManager LanguageDictionary
        {
            get { return new ResourceManager("TrueMount.LanguageDictionary", typeof(TrueMountMainWindow).Assembly); }
        }

        /// <summary>
        /// Contains the path of the configuration file.
        /// </summary>
        public static string ConfigurationFile
        {
            get
            {
                string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TrueMount";
                string configFileName = @"\config.dat";

                if (!Directory.Exists(appDataDir))
                    Directory.CreateDirectory(appDataDir);

                return appDataDir + configFileName;
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
            key.SetValue(VALUE_NAME, Assembly.GetExecutingAssembly().Location);
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
                return (value == Assembly.GetExecutingAssembly().Location);
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

        public static void SaveConfiguration(Configuration current)
        {
            FileStream fsSave = new FileStream(ConfigurationFile, FileMode.Create);
            BinaryFormatter binFormat = new BinaryFormatter();
            binFormat.Serialize(fsSave, current);
            fsSave.Close();
        }

        public static Configuration OpenConfiguration()
        {
            if (File.Exists(ConfigurationFile))
            {
                FileStream fsFetch = new FileStream(ConfigurationFile, FileMode.Open);
                BinaryFormatter binFormat = new BinaryFormatter();
                Configuration stored = (Configuration)binFormat.Deserialize(fsFetch);
                fsFetch.Close();
                return stored;
            }
            return new Configuration();
        }
    }
}
