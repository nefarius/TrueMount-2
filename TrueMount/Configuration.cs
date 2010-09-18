using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TrueMount
{
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

        /// <summary>
        /// Contains the path to the config database file.
        /// </summary>
        public static String ConfigDbFile
        {
            get { return Path.GetDirectoryName(Application.ExecutablePath) + "\\config.yap"; }
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
    }
}
