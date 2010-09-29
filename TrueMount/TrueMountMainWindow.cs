using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace TrueMount
{
    public partial class TrueMountMainWindow : Form
    {
        private Configuration config = null;
        private ManagementScope scope = null;
        private ManagementEventWatcher keyInsertEvent = null;
        private ManagementEventWatcher keyRemoveEvent = null;
        private ResourceManager langRes = null;
        private CultureInfo culture = null;
        SplashScreen splashScreen = null;

        // make LogAppend thread safe
        delegate void LogAppendCallback(String line, params string[] text);

        /// <summary>
        /// Constructor loads the configuration.
        /// </summary>
        public TrueMountMainWindow()
        {
            scope = new ManagementScope(@"root\CIMv2");
            scope.Options.EnablePrivileges = true;
            // load log output languages
            langRes = Configuration.LanguageDictionary;

            // open or create configuration objects
            this.config = Configuration.OpenConfiguration();

            // if first start and no language available, use the systems default
            if (config.Language != null)
                Thread.CurrentThread.CurrentUICulture = config.Language;
            culture = Thread.CurrentThread.CurrentUICulture;

            // create all controls
            InitializeComponent();
        }

        /// <summary>
        /// Reads some startup configurtion and spawns the listening thread.
        /// </summary>
        private void TrueMountMainWindow_Load(object sender, EventArgs e)
        {
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
            this.RegisterRemoveUSBHandler();

            if (!config.DisableBalloons)
            {
                // final start notification
                notifyIconSysTray.BalloonTipTitle = config.ApplicationName;
                notifyIconSysTray.BalloonTipIcon = ToolTipIcon.Info;
                notifyIconSysTray.BalloonTipText = "TrueMount " + Application.ProductVersion;
                notifyIconSysTray.ShowBalloonTip(config.BalloonTimePeriod);
            }

            BuildMountMenu();
        }

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
        /// Event after an encrypted media item has been leftclicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItemEncDisk_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
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

            if (result)
                MountBalloonTip(encMedia);
        }

        /// <summary>
        /// Displayes a balloon tip on mount success.
        /// </summary>
        /// <param name="result">The result of the last mount process.</param>
        /// <param name="encMedia">The mounted media.</param>
        private void MountBalloonTip(EncryptedMedia encMedia)
        {
            // display ballon tip on success
            if (!config.DisableBalloons)
            {
                notifyIconSysTray.BalloonTipTitle = langRes.GetString("NewVolumeMounted");
                notifyIconSysTray.BalloonTipIcon = ToolTipIcon.Info;
                notifyIconSysTray.BalloonTipText = string.Format(langRes.GetString("BalloonVolMounted"), encMedia, encMedia.DriveLetter);
                notifyIconSysTray.ShowBalloonTip(config.BalloonTimePeriod);
            }
        }

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

        /// <summary>
        /// When splash screen closes and silent start is disabled, bring main window to front.
        /// </summary>
        void splashScreen_OnSplashFinished()
        {
            // on first start show settings dialog
            if (config.FirstStart)
            {
                splashScreen.Hide();
                Process.Start(Configuration.ProjectLocation);
                buttonSettings_Click(this, null);
                config.FirstStart = false;
            }

            if (!config.StartSilent)
                ShowMainWindow();
        }

        /// <summary>
        /// Register usb plug-in event listener
        /// </summary>
        /// <returns>Return true on success.</returns>
        private bool RegisterInsetUSBHandler()
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
                keyInsertEvent.EventArrived += new EventArrivedEventHandler(USBLogicalDiskAdded);
                keyInsertEvent.Start();
            }
            catch (Exception e)
            {
                LogAppend(e.Message);
                if (keyInsertEvent != null)
                    keyInsertEvent.Stop();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Register usb remove event listener
        /// </summary>
        /// <returns>Return true on success.</returns>
        private bool RegisterRemoveUSBHandler()
        {
            WqlEventQuery removeQuery;

            try
            {
                removeQuery = new WqlEventQuery();
                removeQuery.EventClassName = "__InstanceDeletionEvent";
                removeQuery.WithinInterval = new TimeSpan(0, 0, 3);
                removeQuery.Condition = "TargetInstance ISA '" + SystemDevices.Win32_DiskPartition + "'";
                keyRemoveEvent = new ManagementEventWatcher(scope, removeQuery);
                keyRemoveEvent.EventArrived += new EventArrivedEventHandler(USBLogicalDiskRemoved);
                keyRemoveEvent.Start();
            }
            catch (Exception e)
            {
                LogAppend(e.Message);
                if (keyRemoveEvent != null)
                    keyRemoveEvent.Stop();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Will be called if new USB device has been found.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void USBLogicalDiskAdded(object sender, EventArgs e)
        {
            LogAppend("USBFound");
            if (!this.IsUsbKeyDeviceOnline())
                LogAppend("USBNotPw");
        }

        /// <summary>
        /// Will be called if USB device is removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void USBLogicalDiskRemoved(object sender, EventArrivedEventArgs e)
        {
            LogAppend("USBRemoved");
            // Debug
            /*
            foreach (PropertyData pd in e.NewEvent.Properties)
            {
                ManagementBaseObject mbo = null;
                if ((mbo = pd.Value as ManagementBaseObject) != null)
                {
                    foreach (PropertyData prop in mbo.Properties)
                        Console.WriteLine("{0} - {1}", prop.Name, prop.Value);
                }
            }
             * */
        }

        /// <summary>
        /// Run through all configured key SystemDevices and if one is online, start the mount process.
        /// </summary>
        /// <returns>Returns true if one or more are found, else false.</returns>
        private bool IsUsbKeyDeviceOnline()
        {
            foreach (UsbKeyDevice usb_key_device in config.KeyDevices)
            {
                String drive_letter = SystemDevices.GetDriveLetterBySignature(usb_key_device.Caption,
                    usb_key_device.Signature, usb_key_device.PartitionIndex - 1);
                if (SystemDevices.IsLogicalDiskOnline(drive_letter))
                {
                    LogAppend("PDevOnline", usb_key_device.Caption);
                    buttonStartWorker.Enabled = false;
                    MountAllDevices();
                    buttonStopWorker.Enabled = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Launches the listening thread and adjusts the gui.
        /// </summary>
        private void StartDeviceListener()
        {
            if (!config.IsUsbDeviceConfigOk)
            {
                // well, the config is crap
                LogAppend("ErrNoKeyDev");
                return;
            }

            // no need for waiting if usb device is already online
            this.IsUsbKeyDeviceOnline();

            LogAppend("StartDevListener");
            if (keyInsertEvent == null)
            {
                if (!this.RegisterInsetUSBHandler())
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
            if (encDiskPartition.PartitionIndex > 0)
                deviceId = SystemDevices.GetPartitionByIndex(diskIndex, partIndex)["DeviceID"].ToString();
            else
                deviceId = SystemDevices.GetTCCompatibleDiskPath(diskIndex);
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
                    return mountSuccess;
                }
            }
            else
                LogAppend("PasswordEmptyOk");

            if (string.IsNullOrEmpty(config.TrueCrypt.CommandLineArguments))
                LogAppend("WarnTCArgs");

            // log what I read
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
                    if (MessageBox.Show(string.Format(langRes.GetString("MsgTDiskTimeout"), encMedia),
                        langRes.GetString("MsgHDiskTimeout"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        System.Windows.Forms.DialogResult.Yes)
                    {
                        LogAppend("MountCanceled", encMedia.ToString());
                        Cursor.Current = Cursors.Default;
                        return mountSuccess;
                    }
                    else
                        loop = 0;
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

            return mountSuccess;
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
        /// Log messages to textbox and consider language.
        /// </summary>
        /// <param name="conv_var">Alias name for text saved in resource files.</param>
        /// <param name="text">Values getting inserted to message throu string.Format.</param>
        private void LogAppend(String conv_var, params string[] text)
        {
            // thread-safe delegate call
            if (this.richTextBoxLog.InvokeRequired)
            {
                LogAppendCallback lac = new LogAppendCallback(LogAppend);
                this.Invoke(lac, new object[] { conv_var, text });
            }
            else
            {
                // correct errors made by accidental whitespaces :)
                conv_var = conv_var.Trim();
                /* build log line:
                 * TIME - MESSAGE
                 * */
                String log_line = DateTime.Now.ToLongTimeString() + " - ";
                if (text.Count() == 0)
                    log_line += langRes.GetString(conv_var, culture);
                else
                    log_line += string.Format(langRes.GetString(conv_var, culture), text);
                log_line += Environment.NewLine;

                richTextBoxLog.AppendText(log_line);

                // autoscroll to end
                richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
                richTextBoxLog.ScrollToCaret();
            }
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
            // get new configuration and set it program wide
            settings.UpdateConfiguration(ref config);
            Configuration.SaveConfiguration(config);
            config = Configuration.OpenConfiguration();
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
            if (config.UnmountWarning)
                if (MessageBox.Show(langRes.GetString("MsgTWarnUmount"), langRes.GetString("MsgHWarnUmount"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    return;

            Process tcUnmount = new Process();

            if (config.TrueCrypt.ExecutablePath != null)
            {
                tcUnmount.StartInfo.FileName = config.TrueCrypt.ExecutablePath;
                tcUnmount.StartInfo.Arguments = "/d /q background";
                if (config.ForceUnmount)
                    tcUnmount.StartInfo.Arguments += " /f";

                LogAppend("UnmountAll");
                try
                {
                    tcUnmount.Start();
                }
                catch (InvalidOperationException ioex)
                {
                    LogAppend("ErrGeneral", ioex.Message);
                    LogAppend("ErrCheckConfig");
                    return;
                }
            }
            else
            {
                LogAppend("ErrTCPathWrong");
                return;
            }

            LogAppend("MsgDone");
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
            AutoUpdater updater = new AutoUpdater();
            if (updater.DownloadVersionInfo())
                if (updater.NewVersionAvailable)
                    if (MessageBox.Show(langRes.GetString("MsgTNewVersion"), langRes.GetString("MsgHNewVersion"),
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        new UpdateProgressDialog().ShowDialog();
        }
    }
}
