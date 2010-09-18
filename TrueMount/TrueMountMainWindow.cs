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
using Db4objects.Db4o;

namespace TrueMount
{
    public partial class TrueMountMainWindow : Form
    {
        private IObjectContainer config_db = null;
        private Configuration config = null;
        private ManagementScope scope = null;
        private ManagementEventWatcher usb_insert_event = null;
        private ManagementEventWatcher usb_remove_event = null;
        private ResourceManager rm = null;
        private CultureInfo culture = null;

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
            rm = new ResourceManager("TrueMount.LogMessages",
                typeof(TrueMountMainWindow).Assembly);

            // load configuration from database
            GetConfigurationFromDb();

            // if first start and no language available, use the systems default
            if (config.Language != null)
                Thread.CurrentThread.CurrentUICulture = config.Language;
            culture = Thread.CurrentThread.CurrentUICulture;

            // create all controls
            InitializeComponent();
        }

        /// <summary>
        /// Fetch configuration from database file.
        /// </summary>
        private void GetConfigurationFromDb()
        {
            config_db = Db4oFactory.OpenFile(Configuration.ConfigDbFile);
            config = config_db.Query<Configuration>().FirstOrDefault();
            if (config == null)
                config = new Configuration();
            config_db.Close();
        }

        /// <summary>
        /// Reads some startup configurtion and spawns the listening thread.
        /// </summary>
        private void TrueMountMainWindow_Load(object sender, EventArgs e)
        {
            /* if reading the configuration has failed, close
             * SIDE EFFECT: do not put code in TrueMountMainWindow_FormClosing
             * which depends on valid data from configuration!
             * */
            if (config == null)
            {
                this.Close();
                return;
            }

            // show splash screen
            if (config.ShowSplashScreen)
            {
                SplashScreen sp_screen = new SplashScreen();
                sp_screen.OnSplashFinished += new SplashScreen.OnSplashFinishedEventHandler(sp_screen_OnSplashFinished);
                sp_screen.Show();
            }
            else
            {
                // start silent?
                if (!config.StartSilent)
                    ShowMainWindow();
            }

            // are we allowed to start the listener on our own?
            if (config.AutostartService)
            {
                LogAppend("SAutoEn");
                // fire up the thread
                StartDeviceListener();
            }
            else
            {
                // we will wait for the users command
                LogAppend("SAutoDi");
            }

            // register the event handler
            this.RegisterRemoveUSBHandler();
        }

        /// <summary>
        /// Maximize and unhide main window in taskbar.
        /// </summary>
        private void ShowMainWindow()
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();
            this.BringToFront();
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
        void sp_screen_OnSplashFinished()
        {
            if (!config.StartSilent)
                ShowMainWindow();
        }

        /// <summary>
        /// Register usb plug-in event listener
        /// </summary>
        /// <returns>Return true on success.</returns>
        private bool RegisterInsetUSBHandler()
        {
            WqlEventQuery insert_qery;

            try
            {
                insert_qery = new WqlEventQuery();
                insert_qery.EventClassName = "__InstanceCreationEvent";
                insert_qery.WithinInterval = new TimeSpan(0, 0, 3);
                /*
                 * Win32_DiskDrive - detects all new disks
                 * Win32_LogicalDisk - detects all new logical disks WITH drive letter!
                 * Win32_USBControllerDevice - detects all new usb SystemDevices (keyboard, mp3-player...)
                 * Win32_DiskPartition - best option, detects all new accessible partitions
                 * */
                insert_qery.Condition = "TargetInstance ISA '" + SystemDevices.Win32_DiskPartition + "'";
                usb_insert_event = new ManagementEventWatcher(scope, insert_qery);
                usb_insert_event.EventArrived += new EventArrivedEventHandler(USBLogicalDiskAdded);
                usb_insert_event.Start();
            }
            catch (Exception e)
            {
                LogAppend(e.Message);
                if (usb_insert_event != null)
                    usb_insert_event.Stop();
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
            WqlEventQuery remove_query;

            try
            {
                remove_query = new WqlEventQuery();
                remove_query.EventClassName = "__InstanceDeletionEvent";
                remove_query.WithinInterval = new TimeSpan(0, 0, 3);
                remove_query.Condition = "TargetInstance ISA '" + SystemDevices.Win32_DiskPartition + "'";
                usb_remove_event = new ManagementEventWatcher(scope, remove_query);
                usb_remove_event.EventArrived += new EventArrivedEventHandler(USBLogicalDiskRemoved);
                usb_remove_event.Start();
            }
            catch (Exception e)
            {
                LogAppend(e.Message);
                if (usb_remove_event != null)
                    usb_remove_event.Stop();
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
            if (usb_insert_event == null)
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
            if (usb_insert_event != null)
            {
                usb_insert_event.Stop();
                usb_insert_event = null;
                LogAppend("DevListenerStop");
                buttonStopWorker.Enabled = false;
                buttonStartWorker.Enabled = true;
            }
            else
                LogAppend("DevListenerNoRun");

            if (usb_remove_event != null)
            {
                usb_remove_event.Stop();
                usb_remove_event = null;
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
            int mounted_partitions = 0;

            if (config.EncryptedDiskPartitions.Count <= 0)
            {
                LogAppend("WarnNoDisks");
                return mounted_partitions;
            }

            // walk through every disk partition in configuration
            foreach (EncryptedDiskPartition enc_disk_partition in config.EncryptedDiskPartitions)
            {
                LogAppend("SearchDiskLocal");

                // is the partition marked as active?
                if (enc_disk_partition.IsActive)
                {
                    LogAppend("DiskConfEnabled", enc_disk_partition.DiskCaption);

                    // find local disk
                    ManagementObject disk_physical = null;
                    try
                    {
                        // well, we try to find it
                        disk_physical = SystemDevices.GetDiskDriveBySignature(enc_disk_partition.DiskCaption,
                            enc_disk_partition.DiskSignature);
                    }
                    catch (FormatException fex)
                    {
                        // if something is wrong, inform the user and skip this device
                        LogAppend("WarnGeneral", fex.Message);
                        LogAppend("CheckConfig");
                    }

                    // is the disk online? if not, skip it
                    if (disk_physical != null)
                    {
                        LogAppend("DiskIsOnline", disk_physical["Caption"].ToString());

                        LogAppend("PartConfEnabled", enc_disk_partition.PartitionIndex.ToString());
                        // get the index of the parent disk
                        uint disk_index = uint.Parse(disk_physical["Index"].ToString());
                        // get the index of this partition
                        uint part_index = enc_disk_partition.PartitionIndex - 1;

                        // get original device id from local disk
                        String device_id = SystemDevices.GetPartitionByIndex(disk_index, part_index)["DeviceID"].ToString();
                        LogAppend("DiskDeviceId", device_id);

                        // convert device id in truecrypt compatible name
                        String tc_device_path = SystemDevices.GetTCCompatibleName(device_id);
                        LogAppend("DiskDrivePath", tc_device_path);

                        // letter we want to assign
                        String tc_device_letter = enc_disk_partition.DriveLetter;
                        LogAppend("DiskPartLetter", tc_device_letter);

                        // local drive letter must not be assigned!
                        if (SystemDevices.GetLogicalDisk(tc_device_letter) == null)
                        {
                            // get the password file
                            String pw_file = enc_disk_partition.PasswordFile;
                            LogAppend("PasswordFile", pw_file);

                            // we store the password here
                            String password = null;
                            // file must exist, else skip and continue
                            if (File.Exists(pw_file))
                            {
                                // file must be utf-8 encoded
                                StreamReader pw_file_stream = new StreamReader(pw_file, System.Text.Encoding.UTF8);
                                /* only FIRST LINE will be read!
                                 * you may fill up the file with 500kB crap and name it .dll :)
                                 * */
                                password = pw_file_stream.ReadLine();
                                pw_file_stream.Close();
                                LogAppend("PasswordReadOk");

                                if (string.IsNullOrEmpty(config.TrueCrypt.CommandLineArguments))
                                    LogAppend("WarnTCArgs");

                                // log what I read
                                LogAppend("TCArgumentLine", config.TrueCrypt.CommandLineArguments);

                                // fill in the attributes we got above
                                String tc_args_ready = config.TrueCrypt.CommandLineArguments +
                                    "/l" + tc_device_letter +
                                    " /v " + tc_device_path +
                                    " /p \"" + password + "\"";
                                // unset password (it's now in the argument line)
                                password = null;

                                // add specified mount options to argument line
                                if (!string.IsNullOrEmpty(enc_disk_partition.MountOptions))
                                {
                                    LogAppend("AddMountOpts", enc_disk_partition.MountOptions);
                                    tc_args_ready += " " + enc_disk_partition.MountOptions;
                                    // DEBUG
                                    //LogAppend("Argument line: " + tc_args_ready);
                                }
                                else
                                    LogAppend("NoMountOpts");

                                // add key files
                                if (!string.IsNullOrEmpty(enc_disk_partition.KeyFilesArgumentLine))
                                {
                                    LogAppend("AddKeyFiles", enc_disk_partition.KeyFiles.Count.ToString());
                                    tc_args_ready += " " + enc_disk_partition.KeyFilesArgumentLine;
                                }
                                else
                                    LogAppend("NoKeyFiles");

                                // create new process
                                Process truecrypt = new Process();
                                // if not exists, exit
                                if (string.IsNullOrEmpty(config.TrueCrypt.ExecutablePath))
                                {
                                    // password is in here, so free it
                                    tc_args_ready = null;
                                    // damn just a few more steps! -.-
                                    LogAppend("ErrTCNotFound");
                                    buttonStartWorker.Enabled = false;
                                    buttonStopWorker.Enabled = false;
                                    LogAppend("CheckCfgTC");
                                    return mounted_partitions;
                                }
                                LogAppend("TCPath", config.TrueCrypt.ExecutablePath);

                                // set exec name
                                truecrypt.StartInfo.FileName = config.TrueCrypt.ExecutablePath;
                                // set arguments
                                truecrypt.StartInfo.Arguments = tc_args_ready;
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
                                    return mounted_partitions;
                                }
                                LogAppend("ProcessStarted");

                                // wait for device to be mounted
                                LogAppend("WaitDevLaunch");

                                /* we must distinguish between local and removable device!
                                 * 2 = removable
                                 * 3 = local (fixed)
                                 * */
                                Cursor.Current = Cursors.WaitCursor;
                                while (SystemDevices.GetLogicalDisk(tc_device_letter, (enc_disk_partition.Removable) ? 2 : 3) == null)
                                {
                                    Thread.Sleep(500);
                                }
                                Cursor.Current = Cursors.Default;

                                //LogAppend(truecrypt.StandardOutput.ReadToEnd());
                                LogAppend("LogicalDiskOnline", tc_device_letter);
                                // increment successfull mounts
                                mounted_partitions++;

                                // if set, open device content in windows explorer
                                if (enc_disk_partition.OpenExplorer)
                                {
                                    LogAppend("OpenExplorer", tc_device_letter);
                                    try
                                    {
                                        Process.Start("explorer.exe", tc_device_letter + @":\");
                                    }
                                    catch (Exception eex)
                                    {
                                        // error in windows explorer (what a surprise)
                                        LogAppend("ErrGeneral", eex.Message);
                                        LogAppend("ErrExplorerOpen");
                                    }
                                }
                            }
                            else
                            {
                                // no password file found, but we can still continue to the next one
                                LogAppend("ErrPwFileNoExist", pw_file);
                            }
                        }
                        else
                        {
                            // device letter is not free
                            LogAppend("WarnLetterInUse", tc_device_letter);
                        }
                    }
                    else
                    {
                        // disk is offline, log and skip
                        LogAppend("DiskDriveOffline", enc_disk_partition.DiskCaption);
                    }
                }
                else
                {
                    // log and skip disk if marked as inactive
                    LogAppend("DiskDriveConfDisabled" + enc_disk_partition.DiskCaption);
                }
            }

            LogAppend("MountedPartitions", mounted_partitions.ToString());
            return mounted_partitions;
        }

        /// <summary>
        /// Close main window and exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrueMountMainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
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
                try
                {
                    // correct errors made by accidental whitespaces :)
                    conv_var = conv_var.Trim();
                    /* build log line:
                     * TIME - MESSAGE
                     * */
                    richTextBoxLog.AppendText(DateTime.Now.ToLongTimeString() +
                        " - " +
                        ((text == null) ? rm.GetString(conv_var, culture) : string.Format(rm.GetString(conv_var, culture), text)) +
                        Environment.NewLine);
                    // autoscroll to end
                    richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
                    richTextBoxLog.ScrollToCaret();
                }
                catch { }
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
            // open settings dialog
            new SettingsDialog().ShowDialog();
            // fetch new configuration
            this.GetConfigurationFromDb();
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
            Process tc_unmount = new Process();

            if (config.TrueCrypt.ExecutablePath != null)
            {
                tc_unmount.StartInfo.FileName = config.TrueCrypt.ExecutablePath;
                tc_unmount.StartInfo.Arguments = "/d /q background";
                LogAppend("UnmountAll");
                try
                {
                    tc_unmount.Start();
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
            new SettingsDialog().ShowDialog();
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
