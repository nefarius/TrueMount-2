using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace TrueMount.Forms
{
    public partial class TrueMountMainWindow : Form
    {
        #region Definitions
        private Configuration config = null;
        private ManagementScope scope = null;
        private ManagementEventWatcher keyInsertEvent = null;
        private ManagementEventWatcher keyRemoveEvent = null;
        private ResourceManager langRes = null;
        private CultureInfo culture = null;
        private SplashScreen splashScreen = null;
        private List<string> onlineKeyDevices = null;
        private List<EncryptedMedia> mountedVolumes = null;
        private PasswordDialog pwDlg = null;

        // make LogAppend thread safe
        delegate void LogAppendCallback(String line, params string[] text);
        #endregion

        #region Constructor, Destructor, Load and Unload Events
        /// <summary>
        /// Constructor loads the configuration.
        /// </summary>
        public TrueMountMainWindow()
        {
            // get the management scope for the management events
            scope = new ManagementScope(@"root\CIMv2");
            // enable events privileges
            scope.Options.EnablePrivileges = true;
            // load log output languages
            langRes = Configuration.LanguageDictionary;

            // open or create configuration objects
            this.config = Configuration.OpenConfiguration();

            // init new empty list of online key devices
            onlineKeyDevices = new List<string>();
            // init new list of mounted volumes
            mountedVolumes = new List<EncryptedMedia>();

            // if first start and no language available, use the systems default
            if (config.Language != null)
                Thread.CurrentThread.CurrentUICulture = config.Language;
            culture = Thread.CurrentThread.CurrentUICulture;

            // create all controls
            InitializeComponent();

#if DEBUG
            this.Text += " - DEBUG Mode";
#endif
        }

        /// <summary>
        /// Reads some startup configurtion and spawns the listening thread.
        /// </summary>
        private void TrueMountMainWindow_Load(object sender, EventArgs e)
        {
#if DEBUG
            // add some test code here to be executed on the very beginning
            /*
            PasswordDialog pwDlg = new PasswordDialog("U.N. Owen VAULT-TEC enc. Device o9000TB");
            if (pwDlg.ShowDialog() == DialogResult.OK)
                MessageBox.Show(pwDlg.Password);
            Environment.Exit(0);
             * */
#endif

            // start silent?
            if (config.StartSilent)
                HideMainWindow();

            // show splash screen
            if (config.ShowSplashScreen)
            {
                HideMainWindow();
                splashScreen = new SplashScreen();
                splashScreen.OnSplashFinished += new SplashScreen.OnSplashFinishedEventHandler(splashScreen_OnSplashFinished);
                splashScreen.Show();
            }

            // are we allowed to start the listener on our own?
            if (config.AutostartService)
            {
                LogAppend("SAutoEn");
                // fire up the thread
                StartDeviceListener();
            }
            else
                LogAppend("SAutoDi");

            // register the event handler
            this.RegisterDeviceRemoveHandler();

            if (!config.DisableBalloons)
            {
                // final start notification
                notifyIconSysTray.BalloonTipTitle = config.ApplicationName;
                notifyIconSysTray.BalloonTipIcon = ToolTipIcon.Info;
                notifyIconSysTray.BalloonTipText = "TrueMount " + Application.ProductVersion;
#if DEBUG
                notifyIconSysTray.BalloonTipText+=" - DEBUG Mode";
#endif
                notifyIconSysTray.ShowBalloonTip(config.BalloonTimePeriod);
            }

            BuildMountMenu();
        }

        /// <summary>
        /// Close main window and exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrueMountMainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            config.ApplicationLocation = Configuration.CurrentApplicationLocation;
            Configuration.SaveConfiguration(config);
            StopDeviceListener();
        }

        #endregion

        #region Tray menu actions

        /// <summary>
        /// Adds encrypted device items dynamically to the tray menu.
        /// </summary>
        private void BuildMountMenu()
        {
            mountDeviceToolStripMenuItem.DropDownItems.Clear();
            int index = 0;

            // add encrypted partitions
            foreach (EncryptedDiskPartition item in config.EncryptedDiskPartitions)
            {
                ToolStripMenuItem menuItemEncDisk = new ToolStripMenuItem(item.DiskCaption +
                    langRes.GetString("CBoxPartition") +
                    item.PartitionIndex +
                    langRes.GetString("CBoxLetter") +
                    item.DriveLetter);
                menuItemEncDisk.Name = index++.ToString();
                menuItemEncDisk.Image = Properties.Resources._1276786893_drive_disk;
                menuItemEncDisk.Click += new EventHandler(menuItemEncDisk_Click);
                mountDeviceToolStripMenuItem.DropDownItems.Add(menuItemEncDisk);
            }

            // add container files
            foreach (EncryptedContainerFile item in config.EncryptedContainerFiles)
            {
                ToolStripMenuItem menuItemConFile = new ToolStripMenuItem(item.FileName +
                    langRes.GetString("CBoxLetter") +
                    item.DriveLetter);
                menuItemConFile.Name = index++.ToString();
                menuItemConFile.Image = Properties.Resources._1276786893_drive_disk;
                menuItemConFile.Click += new EventHandler(menuItemEncDisk_Click);
                mountDeviceToolStripMenuItem.DropDownItems.Add(menuItemConFile);
            }

            mountDeviceToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// Adds a mounted volume to the unmount tray dropdown list.
        /// </summary>
        /// <param name="encMedia">The successfull mounted media reference.</param>
        private void AddMountedMedia(EncryptedMedia encMedia)
        {
            // add mounted volume to global list
            mountedVolumes.Add(encMedia);
            // create new tool strip sub-entry
            ToolStripMenuItem menuItemMedia =
                new ToolStripMenuItem(encMedia.ToString() +
                    langRes.GetString("CBoxLetter") +
                    encMedia.DriveLetter);
            // set icon
            menuItemMedia.Image = Properties.Resources._1276786893_drive_disk;
            // register event handler
            menuItemMedia.Click += new EventHandler(menuItemUnmountMedia_Click);
            // add it to the sys tray menu item
            unmountDeviceToolStripMenuItem.DropDownItems.Add(menuItemMedia);
            // if there are more than 0 items enable the sub-menu
            if (unmountDeviceToolStripMenuItem.DropDownItems.Count > 0)
                unmountDeviceToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// Event after an encrapted unmount media item has been leftclicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItemUnmountMedia_Click(object sender, EventArgs e)
        {
            // get the clicked item
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            // get the encrypted volume related to it
            EncryptedMedia encMedia = mountedVolumes[unmountDeviceToolStripMenuItem.DropDownItems.IndexOf(item)];
            // try to unmount
            if (UnmountMedia(encMedia))
            {
                // remove from the list of mounted volumes
                mountedVolumes.Remove(encMedia);
                // remove the menu entry
                unmountDeviceToolStripMenuItem.DropDownItems.Remove(item);
                // if the menu is empty now hide it
                if (unmountDeviceToolStripMenuItem.DropDownItems.Count == 0)
                    unmountDeviceToolStripMenuItem.Enabled = false;
                // pop-up a balloon tip
                UnmountBalloonTip(encMedia);
            }
        }

        /// <summary>
        /// Event after an encrypted mount media item has been leftclicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItemEncDisk_Click(object sender, EventArgs e)
        {
            // get the clicked menu item
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            // get the index number
            int index = int.Parse(item.Name);
            bool result = false;
            EncryptedMedia encMedia = null;

            // try to mount the partition or container file
            if (config.EncryptedDiskPartitions.Count > index)
            {
                encMedia = config.EncryptedDiskPartitions[index];
                result = MountPartition((EncryptedDiskPartition)encMedia);
            }
            else
            {
                encMedia = config.EncryptedContainerFiles[index - config.EncryptedDiskPartitions.Count];
                result = MountContainerFile((EncryptedContainerFile)encMedia);
            }

            // on success display a balloon tip
            if (result)
                MountBalloonTip(encMedia);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        #endregion

        #region Balloon tips actions
        /// <summary>
        /// Displayes a balloon tip on mount success.
        /// </summary>
        /// <param name="encMedia">The mounted media.</param>
        private void MountBalloonTip(EncryptedMedia encMedia)
        {
            // display ballon tip on success
            if (!config.DisableBalloons)
            {
                notifyIconSysTray.BalloonTipTitle = langRes.GetString("NewVolumeMounted");
                notifyIconSysTray.BalloonTipIcon = ToolTipIcon.Info;
                notifyIconSysTray.BalloonTipText =
                    string.Format(langRes.GetString("BalloonVolMounted"), encMedia, encMedia.DriveLetter);
                notifyIconSysTray.ShowBalloonTip(config.BalloonTimePeriod);
            }
        }

        /// <summary>
        /// Displays a balloon top on successfull dismount.
        /// </summary>
        /// <param name="encMedia">The dismounted media.</param>
        private void UnmountBalloonTip(EncryptedMedia encMedia)
        {
            // display ballon tip on success
            if (!config.DisableBalloons)
            {
                notifyIconSysTray.BalloonTipTitle = langRes.GetString("VolumeDismounted");
                notifyIconSysTray.BalloonTipIcon = ToolTipIcon.Info;
                notifyIconSysTray.BalloonTipText =
                    string.Format(langRes.GetString("BalloonVolumeDismounted"), encMedia, encMedia.DriveLetter);
                notifyIconSysTray.ShowBalloonTip(config.BalloonTimePeriod);
            }
        }
        #endregion

        #region Show and hide main window
        /// <summary>
        /// Maximize and unhide main window in taskbar.
        /// </summary>
        private void ShowMainWindow()
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();
            this.TopMost = true;
            this.BringToFront();
            this.Focus();
            this.TopMost = false;
        }

        /// <summary>
        /// Minimize and hide main window in taskbar.
        /// </summary>
        private void HideMainWindow()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
        }
        #endregion

        #region Key device listener methods
        /// <summary>
        /// Register usb plug-in event listener
        /// </summary>
        /// <returns>Return true on success.</returns>
        private bool RegisterDeviceInsetHandler()
        {
            WqlEventQuery insertQery;

            try
            {
                insertQery = new WqlEventQuery();
                insertQery.EventClassName = "__InstanceCreationEvent";
                insertQery.WithinInterval = new TimeSpan(0, 0, 3);
                /*
                 * Win32_DiskDrive - detects all new disks
                 * Win32_LogicalDisk - detects all new logical disks WITH drive letter!
                 * Win32_USBControllerDevice - detects all new usb SystemDevices (keyboard, mp3-player...)
                 * Win32_DiskPartition - best option, detects all new accessible partitions
                 * */
                insertQery.Condition = "TargetInstance ISA '" + SystemDevices.Win32_DiskPartition + "'";
                keyInsertEvent = new ManagementEventWatcher(scope, insertQery);
                keyInsertEvent.EventArrived += new EventArrivedEventHandler(keyInsertEvent_EventArrived);
                keyInsertEvent.Start();
            }
            catch (Exception e)
            {
                LogAppend("", e.Message);
                if (keyInsertEvent != null)
                    keyInsertEvent.Stop();
                return false;
            }

            return true;
        }

        void keyInsertEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            LogAppend("USBFound");

            ManagementBaseObject insertedObject = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            string deviceId = insertedObject["DeviceID"].ToString();

            if (!this.CheckOnlineKeyDevices(deviceId))
                LogAppend("USBNotPw");
        }

        /// <summary>
        /// Register usb remove event listener
        /// </summary>
        /// <returns>Return true on success.</returns>
        private bool RegisterDeviceRemoveHandler()
        {
            WqlEventQuery removeQuery;

            try
            {
                removeQuery = new WqlEventQuery();
                removeQuery.EventClassName = "__InstanceDeletionEvent";
                removeQuery.WithinInterval = new TimeSpan(0, 0, 3);
                removeQuery.Condition = "TargetInstance ISA '" + SystemDevices.Win32_DiskPartition + "'";
                keyRemoveEvent = new ManagementEventWatcher(scope, removeQuery);
                keyRemoveEvent.EventArrived += new EventArrivedEventHandler(keyRemoveEvent_EventArrived);
                keyRemoveEvent.Start();
            }
            catch (Exception e)
            {
                LogAppend("", e.Message);
                if (keyRemoveEvent != null)
                    keyRemoveEvent.Stop();
                return false;
            }

            return true;
        }

        void keyRemoveEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            LogAppend("USBRemoved");

            // get the DeviceID of the removed device
            ManagementBaseObject removedObject = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            string deviceId = removedObject["DeviceID"].ToString();

            if (onlineKeyDevices.Contains(deviceId))
                for (int index = mountedVolumes.Count; index != 0; index--)
                    if (mountedVolumes[index - 1].TriggerDismount)
                        menuItemUnmountMedia_Click(unmountDeviceToolStripMenuItem.DropDownItems[index - 1], null);
        }

        #endregion

        #region Start and stop Device Listener
        /// <summary>
        /// Launches the listening thread and adjusts the gui.
        /// </summary>
        private void StartDeviceListener()
        {
            if(!config.IgnoreKeyDevices)
                if (!config.IsKeyDeviceConfigOk)
                {
                    // well, the config is crap
                    LogAppend("ErrNoKeyDev");
                    return;
                }

            // no need for waiting if usb device is already online
            CheckOnlineKeyDevices();

            LogAppend("StartDevListener");
            if (keyInsertEvent == null)
            {
                if (!this.RegisterDeviceInsetHandler())
                    LogAppend("ErrDevListenerStart");
                else
                {
                    LogAppend("DevListenerRun");
                    buttonStartWorker.Enabled = false;
                    buttonStopWorker.Enabled = true;
                }
            }
            else
                LogAppend("DevListenerIsRun");
        }

        /// <summary>
        /// Stop and delete the event listener.
        /// </summary>
        private void StopDeviceListener()
        {
            LogAppend("StopDevListener");
            if (keyInsertEvent != null)
            {
                keyInsertEvent.Stop();
                keyInsertEvent = null;
                LogAppend("DevListenerStop");
                buttonStopWorker.Enabled = false;
                buttonStartWorker.Enabled = true;
            }
            else
                LogAppend("DevListenerNoRun");

            if (keyRemoveEvent != null)
            {
                keyRemoveEvent.Stop();
                keyRemoveEvent = null;
            }
        }
        #endregion

        #region Button and Menu Events
        /// <summary>
        /// Shows the main windows on tray doubleclick.
        /// </summary>
        private void notifyIconSysTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowMainWindow();
        }

        /// <summary>
        /// Close main window,
        /// </summary>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Hide main window.
        /// </summary>
        private void buttonHide_Click(object sender, EventArgs e)
        {
            HideMainWindow();
        }

        /// <summary>
        /// On Tray menu close click close the main window and exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Tray menu show main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showToolStripMenuShow_Click(object sender, EventArgs e)
        {
            ShowMainWindow();
        }

        /// <summary>
        /// Start device listener.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStartWorker_Click(object sender, EventArgs e)
        {
            this.StartDeviceListener();
        }

        /// <summary>
        /// Stop device listener.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStopWorker_Click(object sender, EventArgs e)
        {
            this.StopDeviceListener();
        }

        /// <summary>
        /// Open my website :)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelNefarius_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(linkLabelNefarius.Text);
        }

        /// <summary>
        /// Show settings dialog and reload config after close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSettings_Click(object sender, EventArgs e)
        {
            // create settings dialog and feed with configuration references
            SettingsDialog settings = new SettingsDialog(ref config);
            // bring dialog to front and await user actions
            settings.ShowDialog();
            Configuration.SaveConfiguration(config);
            settings = null;
            BuildMountMenu();
        }

        /// <summary>
        /// Context menu copy action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBoxLog.SelectedText);
        }

        /// <summary>
        /// Try to unmount all devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unmountAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnmountMedia();
        }

        /// <summary>
        /// Systray menu entry to mount all SystemDevices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mountAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MountAllDevices();
        }

        private void backgroundWorkerSplash_DoWork(object sender, DoWorkEventArgs e)
        {
            new SplashScreen().ShowDialog();
        }

        private void settingsToolStripMenuSettings_Click(object sender, EventArgs e)
        {
            this.buttonSettings_Click(sender, e);
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Configuration.UpdaterExists)
                config.InvokeUpdateProcess();
            else
                MessageBox.Show(langRes.GetString("MsgTNoUpdater"), langRes.GetString("MsgHNoUpdater"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion

        #region Mount and Unmount methods
        /// <summary>
        /// Reads configuration and tries to mount every found device.
        /// </summary>
        /// <returns>Returns count of mounted SystemDevices, if none returns zero.</returns>
        private int MountAllDevices()
        {
            int mountedPartitions = 0;

            // this method can't do very much without partitions
            if (config.EncryptedDiskPartitions.Count <= 0)
            {
                LogAppend("WarnNoDisks");
                return mountedPartitions;
            }

            // walk through every partition in configuration
            foreach (EncryptedDiskPartition enc_disk_partition in config.EncryptedDiskPartitions)
                if (MountPartition(enc_disk_partition))
                    mountedPartitions++;

            // walk through every container file in configuration
            foreach (EncryptedContainerFile encContainerFile in config.EncryptedContainerFiles)
                if (MountContainerFile(encContainerFile))
                    mountedPartitions++;

            LogAppend("MountedPartitions", mountedPartitions.ToString());
            return mountedPartitions;
        }

        /// <summary>
        /// Mount a specific encrypted partition.
        /// </summary>
        /// <param name="encDiskPartition">The encrypted partition to mount.</param>
        /// <returns>Returns true on successfull mount, else false.</returns>
        private bool MountPartition(EncryptedDiskPartition encDiskPartition)
        {
            bool mountSuccess = false;

            LogAppend("SearchDiskLocal");

            // is the partition marked as active?
            if (!encDiskPartition.IsActive)
            {
                // log and skip disk if marked as inactive
                LogAppend("DiskDriveConfDisabled", encDiskPartition.DiskCaption);
                return mountSuccess;
            }
            else
                LogAppend("DiskConfEnabled", encDiskPartition.DiskCaption);

            // find local disk
            ManagementObject diskPhysical =
                SystemDevices.GetDiskDriveBySignature(encDiskPartition.DiskCaption,
                    encDiskPartition.DiskSignature);

            // is the disk online? if not, skip it
            if (diskPhysical == null)
            {
                // disk is offline, log and skip
                LogAppend("DiskDriveOffline", encDiskPartition.DiskCaption);
                return mountSuccess;
            }
            else
                LogAppend("DiskIsOnline", diskPhysical["Caption"].ToString());

            // get the index of the parent disk
            uint diskIndex = uint.Parse(diskPhysical["Index"].ToString());
            // get the index of this partition ("real" index is zero-based)
            uint partIndex = encDiskPartition.PartitionIndex - 1;

            // get original device id from local disk
            String deviceId = null;
            try
            {
                if (encDiskPartition.PartitionIndex > 0)
                    deviceId = SystemDevices.GetPartitionByIndex(diskIndex, partIndex)["DeviceID"].ToString();
                else
                    deviceId = SystemDevices.GetTCCompatibleDiskPath(diskIndex);
            }
            catch (NullReferenceException)
            {
                LogAppend("ErrVolumeOffline", encDiskPartition.ToString());
                return mountSuccess;
            }

            LogAppend("DiskDeviceId", deviceId);

            // convert device id in truecrypt compatible name
            String tcDevicePath = SystemDevices.GetTCCompatibleName(deviceId);
            LogAppend("DiskDrivePath", tcDevicePath);

            // try to mount and return true on success
            return MountEncryptedMedia(encDiskPartition, tcDevicePath);
        }

        /// <summary>
        /// Mount a specific container file.
        /// </summary>
        /// <param name="containerFile">The container file to mount.</param>
        /// <returns>Returns true on successfull mount, else false.</returns>
        private bool MountContainerFile(EncryptedContainerFile containerFile)
        {
            bool mountSuccess = false;

            LogAppend("SearchConFile");

            // skip the file if inactive
            if (!containerFile.IsActive)
            {
                LogAppend("ConConfDisabled", containerFile.FileName);
                return mountSuccess;
            }
            else
                LogAppend("ConConfEnabled", containerFile.FileName);

            // check if file exists on local system
            if (!File.Exists(containerFile.FileName))
            {
                LogAppend("ErrConFileNotExists", containerFile.FileName);
                return mountSuccess;
            }
            else
                LogAppend("ConFileFound", containerFile.FileName);

            // try to mount the volume and return true on success
            return MountEncryptedMedia(containerFile, containerFile.FileName);
        }

        /// <summary>
        /// Mounts a specific media.
        /// </summary>
        /// <param name="encMedia">The ecnrypted media to mount.</param>
        /// <param name="encVolume">The device path or file name to mount.</param>
        /// <returns>Returns true on successfull mount, else false.</returns>
        private bool MountEncryptedMedia(EncryptedMedia encMedia, String encVolume)
        {
            String password = string.Empty;
            bool mountSuccess = false;

            // letter we want to assign
            LogAppend("DriveLetter", encMedia.DriveLetter);

            // local drive letter must not be assigned!
            if (SystemDevices.GetLogicalDisk(encMedia.DriveLetter) != null)
            {
                LogAppend("ErrLetterInUse", encMedia.DriveLetter);
                return mountSuccess;
            }

            // read password or let it empty
            if (!string.IsNullOrEmpty(encMedia.PasswordFile))
            {
                // if we have a password file it must exist
                if (File.Exists(encMedia.PasswordFile))
                {
                    LogAppend("PasswordFile", encMedia.PasswordFile);
                    // file must be utf-8 encoded
                    StreamReader pwFileStream = new StreamReader(encMedia.PasswordFile, System.Text.Encoding.UTF8);
                    password = pwFileStream.ReadLine();
                    pwFileStream.Close();
                    LogAppend("PasswordReadOk");
                }
                else
                {
                    LogAppend("ErrPwFileNoExist", encMedia.PasswordFile);
                    // return if we run out of options
                    if (!encMedia.FetchUserPassword)
                        return mountSuccess;
                    else
                    {
                        LogAppend("InfoPasswordDialog");

                        if (pwDlg == null)
                            pwDlg = new PasswordDialog(encMedia.ToString());
                        else
                            pwDlg.VolumeLabel = encMedia.ToString();

                        // launch a new password dialog and annoy the user
                        if (pwDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            LogAppend("PasswordFetchOk");
                            password = pwDlg.Password;
                        }
                        else
                        {
                            LogAppend("PasswordDialogCanceled");
                            return mountSuccess;
                        }
                    }
                }
            }
            else
            {
                LogAppend("PasswordEmptyOk");
                // it will work without a password, but if the user wants to talk to me...
                if (encMedia.FetchUserPassword)
                {
                    LogAppend("InfoPasswordDialog");

                    if (pwDlg == null)
                        pwDlg = new PasswordDialog(encMedia.ToString());
                    else
                        pwDlg.VolumeLabel = encMedia.ToString();

                    // launch a new password dialog and annoy the user
                    if (pwDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        LogAppend("PasswordFetchOk");
                        password = pwDlg.Password;
                    }
                    else
                    {
                        LogAppend("PasswordDialogCanceled");
                        return mountSuccess;
                    }
                }
            }

            // warn if important CLI flags are missing (probably the users fault)
            if (string.IsNullOrEmpty(config.TrueCrypt.CommandLineArguments))
                LogAppend("WarnTCArgs");

            // log what we read
            LogAppend("TCArgumentLine", config.TrueCrypt.CommandLineArguments);

            // fill in the attributes we got above
            String tcArgsReady = config.TrueCrypt.CommandLineArguments +
                "/l" + encMedia.DriveLetter +
                " /v \"" + encVolume + "\"" +
                " /p \"" + password + "\"";
            // unset password (it's now in the argument line)
            password = null;

            // add specified mount options to argument line
            if (!string.IsNullOrEmpty(encMedia.MountOptions))
            {
                LogAppend("AddMountOpts", encMedia.MountOptions);
                tcArgsReady += " " + encMedia.MountOptions;
            }
            else
                LogAppend("NoMountOpts");

            // add key files
            if (!string.IsNullOrEmpty(encMedia.KeyFilesArgumentLine))
            {
                LogAppend("AddKeyFiles", encMedia.KeyFiles.Count.ToString());
                tcArgsReady += " " + encMedia.KeyFilesArgumentLine;
            }
            else
                LogAppend("NoKeyFiles");

            // create new process
            Process truecrypt = new Process();
            // if not exists, exit
            if (string.IsNullOrEmpty(config.TrueCrypt.ExecutablePath))
            {
                // password is in here, so free it
                tcArgsReady = null;
                // damn just a few more steps! -.-
                LogAppend("ErrTCNotFound");
                buttonStartWorker.Enabled = false;
                buttonStopWorker.Enabled = false;
                LogAppend("CheckCfgTC");
                return mountSuccess;
            }
            else
                LogAppend("TCPath", config.TrueCrypt.ExecutablePath);

            // set exec name
            truecrypt.StartInfo.FileName = config.TrueCrypt.ExecutablePath;
            // set arguments
            truecrypt.StartInfo.Arguments = tcArgsReady;
            // no need for shell
            truecrypt.StartInfo.UseShellExecute = false;

            // arrr, fire the canon! - well, try it...
            try
            {
                LogAppend("StartProcess");
                truecrypt.Start();
            }
            catch (Win32Exception ex)
            {
                // dammit, dammit, dammit! something went wrong at the very end...
                LogAppend("ErrGeneral", ex.Message);
                buttonStartWorker.Enabled = false;
                buttonStopWorker.Enabled = false;
                LogAppend("CheckTCConf");
                return mountSuccess;
            }
            LogAppend("ProcessStarted");

            // wait for device to be mounted
            LogAppend("WaitDevLaunch");

            /* we must distinguish between local and removable device!
             * 2 = removable
             * 3 = local (fixed)
             * */
            Cursor.Current = Cursors.WaitCursor;
            int loop = 0;
            // note to myself and every other reader: this loop is crap, we have to do this better :-S
            while (SystemDevices.GetLogicalDisk(encMedia.DriveLetter, (encMedia.Removable) ? 2 : 3) == null)
            {
                Thread.Sleep(1000);
                loop++;
                if (loop == 10)
                {
                    MessageBox.Show(string.Format(langRes.GetString("MsgTDiskTimeout"), encMedia),
                        langRes.GetString("MsgHDiskTimeout"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                    LogAppend("MountCanceled", encMedia.ToString());
                    Cursor.Current = Cursors.Default;
                    return mountSuccess;
                }
            }
            Cursor.Current = Cursors.Default;

            LogAppend("LogicalDiskOnline", encMedia.DriveLetter);
            // mount was successfull
            mountSuccess = true;
            // display balloon tip on successfull mount
            MountBalloonTip(encMedia);

            // if set, open device content in windows explorer
            if (encMedia.OpenExplorer)
            {
                LogAppend("OpenExplorer", encMedia.DriveLetter);
                try
                {
                    Process.Start("explorer.exe", encMedia.DriveLetter + @":\");
                }
                catch (Exception eex)
                {
                    // error in windows explorer (what a surprise)
                    LogAppend("ErrGeneral", eex.Message);
                    LogAppend("ErrExplorerOpen");
                }
            }

            // add the current mounted media to the dismount systray list
            AddMountedMedia(encMedia);

            return mountSuccess;
        }

        /// <summary>
        /// Unmounts a specific encrypted media.
        /// </summary>
        /// <param name="encMedia">The volume reference.</param>
        /// <returns>Returns true on success, else false.</returns>
        private bool UnmountMedia(EncryptedMedia encMedia = null)
        {
            if (config.UnmountWarning)
                if (MessageBox.Show(langRes.GetString("MsgTWarnUmount"), langRes.GetString("MsgHWarnUmount"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    return false;

            if (string.IsNullOrEmpty(config.TrueCrypt.ExecutablePath))
            {
                LogAppend("ErrTCPathWrong");
                return false;
            }

            Process tcUnmount = new Process();
            tcUnmount.StartInfo.FileName = config.TrueCrypt.ExecutablePath;
            tcUnmount.StartInfo.Arguments = "/q background";

            // force dismount?
            if (config.ForceUnmount)
                tcUnmount.StartInfo.Arguments += " /f";
            // general switch for dismount
            tcUnmount.StartInfo.Arguments += " /d ";

            // unmount all if no media given
            if (encMedia == null)
            {
                LogAppend("UnmountAll");
            }
            else
            {
                tcUnmount.StartInfo.Arguments += encMedia.DriveLetter;
                LogAppend("UnmountMedia", encMedia.DriveLetter);
            }

            try
            {
                tcUnmount.Start();
            }
            catch (InvalidOperationException ioex)
            {
                LogAppend("ErrGeneral", ioex.Message);
                LogAppend("ErrCheckConfig");
                return false;
            }

            LogAppend("MsgDone");
            return true;
        }

        #endregion

        #region Helper (SplashScreen, Device Check, Log)
        /// <summary>
        /// Run through all configured key SystemDevices and if one is online, start the mount process.
        /// </summary>
        /// <returns>Returns true if one or more are found, else false.</returns>
        private bool CheckOnlineKeyDevices(string deviceId = null)
        {
            // if the user does not need this function, just try to mount everything
            if ((config.IgnoreKeyDevices || config.IsUserPasswordNeeded)
                && deviceId == null)
            {
                if (MountAllDevices() > 0)
                    return true;
                else
                    return false;
            }

            // run through every key device
            foreach (UsbKeyDevice keyDevice in config.KeyDevices)
            {
                // if a specific device is given, only check this
                if (!string.IsNullOrEmpty(deviceId))
                {
                    if (SystemDevices.IsPartitionOnline(keyDevice.Caption, keyDevice.Signature,
                        keyDevice.PartitionIndex - 1, deviceId))
                    {
                        // add the new device to the online key device list
                        if (!onlineKeyDevices.Contains(deviceId))
                            onlineKeyDevices.Add(deviceId);
                        LogAppend("PDevOnline", keyDevice.Caption);
                        buttonStartWorker.Enabled = false;
                        MountAllDevices();
                        buttonStopWorker.Enabled = true;
                        return true;
                    }
                }
                else // check all online logical devices
                {
                    String driveLetter = SystemDevices.GetDriveLetterBySignature(keyDevice.Caption,
                        keyDevice.Signature, keyDevice.PartitionIndex - 1);
                    if (SystemDevices.IsLogicalDiskOnline(driveLetter))
                    {
                        LogAppend("PDevOnline", keyDevice.Caption);
                        buttonStartWorker.Enabled = false;
                        MountAllDevices();
                        buttonStopWorker.Enabled = true;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// When splash screen closes and silent start is disabled, bring main window to front.
        /// </summary>
        void splashScreen_OnSplashFinished()
        {
            // on first start show settings dialog
            if (config.FirstStart)
            {
                splashScreen.Hide();
                buttonSettings_Click(this, null);
                config.FirstStart = false;
            }

            if (!config.StartSilent)
                ShowMainWindow();
        }

        /// <summary>
        /// Log messages to textbox and consider language.
        /// </summary>
        /// <param name="convVar">Alias name for text saved in resource files.</param>
        /// <param name="text">Values getting inserted to message throu string.Format.</param>
        private void LogAppend(String convVar, params string[] text)
        {
            // thread-safe delegate call
            if (this.richTextBoxLog.InvokeRequired)
            {
                LogAppendCallback lac = new LogAppendCallback(LogAppend);
                this.Invoke(lac, new object[] { convVar, text });
            }
            else
            {
                /* build log line:
                 * TIME - MESSAGE
                 * */
                String logLine = DateTime.Now.ToLongTimeString() + " - ";

                if (string.IsNullOrEmpty(convVar) && text.Count() > 0)
                    logLine += text.First();
                else
                    if (text.Count() == 0)
                        logLine += langRes.GetString(convVar.Trim(), culture);
                    else
                        logLine += string.Format(langRes.GetString(convVar.Trim(), culture), text);

                logLine += Environment.NewLine;
                richTextBoxLog.AppendText(logLine);

                // autoscroll to end
                richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
                richTextBoxLog.ScrollToCaret();
            }
        }

        #endregion
    }
}
