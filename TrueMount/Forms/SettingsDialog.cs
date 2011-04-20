using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace TrueMount.Forms
{
    partial class SettingsDialog : Form
    {
        #region Definitions
        public Configuration config = null;
        private Dictionary<int, List<ManagementObject>> keyDeviceList = null;
        private Dictionary<int, List<ManagementObject>> encDeviceList = null;
        private ResourceManager langRes = null;
        private CultureInfo culture = null;
        private List<List<string>> diskKeyFilesList = null;
        private List<List<string>> containerKeyFilesList = null;
        private bool editInProgress = false;
        #endregion

        #region Constructor and Load Events
        /// <summary>
        /// Creates new settings dialog with current configuration data.
        /// </summary>
        public SettingsDialog(ref Configuration config)
        {
            this.config = config;

            // new list ob available SystemDevices
            keyDeviceList = new Dictionary<int, List<ManagementObject>>();
            encDeviceList = new Dictionary<int, List<ManagementObject>>();

            // list of key files
            diskKeyFilesList = new List<List<string>>();
            containerKeyFilesList = new List<List<string>>();

            // load translation resources
            langRes = new ResourceManager("TrueMount.LanguageDictionary",
                typeof(TrueMountMainWindow).Assembly);

            // set language from config or system default
            if (config.Language != null)
                Thread.CurrentThread.CurrentUICulture = config.Language;
            culture = Thread.CurrentThread.CurrentUICulture;

            InitializeComponent();
        }

        /// <summary>
        /// Gets executed on form load.
        /// </summary>
        private void SettingsDialog_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // load application settings
            checkBoxWindowsStartup.Checked = config.IsAutoStartEnabled;
            checkBoxAutostart.Checked = config.AutostartService;
            checkBoxHidden.Checked = config.StartSilent;
            checkBoxSplashScreen.Checked = config.ShowSplashScreen;
            checkBoxOneInstance.Checked = config.OnlyOneInstance;
            checkBoxIgnoreDriveLetters.Checked = config.IgnoreAssignedDriveLetters;
            checkBoxForceUnmountAll.Checked = config.ForceUnmount;
            checkBoxWarnUnmountAll.Checked = config.UnmountWarning;
            checkBoxDisableBalloons.Checked = config.DisableBalloons;
            numericUpDownBalloonTime.Value = config.BalloonTimePeriod;
            checkBoxCheckUpdates.Checked = config.CheckForUpdates;
            checkBoxWarnOnExit.Checked = config.WarnOnExit;

            // load available languages
            if (config.Language != null)
                comboBoxLanguage.SelectedItem = config.Language.DisplayName;
            else
                comboBoxLanguage.SelectedItem = Thread.CurrentThread.CurrentUICulture.DisplayName.Split(' ')[0];

            // if no truecrypt path specified open search window
            if (!string.IsNullOrEmpty(config.TrueCrypt.ExecutablePath))
                textBoxTrueCryptExec.Text = config.TrueCrypt.ExecutablePath;
            else
            {
                buttonSearchTrueCrypt_Click(this, null);
            }

            checkBoxBeep.Checked = config.TrueCrypt.Beep;
            checkBoxCache.Checked = config.TrueCrypt.Cache;
            checkBoxExplorer.Checked = config.TrueCrypt.Explorer;
            checkBoxShowLauncherError.Checked = config.TrueCrypt.ShowErrors;

            // read key devices and add them to the list
            listBoxKeyDevices.Items.AddRange(config.KeyDevices.ToArray());

            // read disk drives and add them to their list
            foreach (EncryptedDiskPartition encDisk in config.EncryptedDiskPartitions)
            {
                diskKeyFilesList.Add(encDisk.KeyFiles);
                listBoxDisks.Items.Add(encDisk);
                listBoxDisks.Enabled = true;
            }

            // insert available container files
            foreach (EncryptedContainerFile conFile in config.EncryptedContainerFiles)
            {
                containerKeyFilesList.Add(conFile.KeyFiles);
                listBoxContainerFiles.Items.Add(conFile);
                listBoxContainerFiles.Enabled = true;
            }

            // create dictionary with indexd management objects (disks, partitions, ...)
            BuildDeviceList();

            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Gets called on form closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (editInProgress)
                if (MessageBox.Show(langRes.GetString("MsgTWarnNotSaved"), langRes.GetString("MsgHWarnNotSaved"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    e.Cancel = true;
        }

        #endregion

        #region Application Tab Events

        /// <summary>
        /// Set or unset autostart entry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxWindowsStartup_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWindowsStartup.Checked)
                config.SetAutoStart();
            else
                config.UnSetAutoStart();
        }

        #region Checkbox (un)set events
        private void checkBoxAutostart_CheckedChanged(object sender, EventArgs e)
        {
            config.AutostartService = checkBoxAutostart.Checked;
        }

        private void checkBoxHidden_CheckedChanged(object sender, EventArgs e)
        {
            config.StartSilent = checkBoxHidden.Checked;
        }

        private void checkBoxSplashScreen_CheckedChanged(object sender, EventArgs e)
        {
            config.ShowSplashScreen = checkBoxSplashScreen.Checked;
        }

        private void checkBoxOneInstance_CheckedChanged(object sender, EventArgs e)
        {
            config.OnlyOneInstance = checkBoxOneInstance.Checked;
        }

        private void checkBoxIgnoreDriveLetters_CheckedChanged(object sender, EventArgs e)
        {
            config.IgnoreAssignedDriveLetters = checkBoxIgnoreDriveLetters.Checked;
        }

        private void checkBoxForceUnmountAll_CheckedChanged(object sender, EventArgs e)
        {
            config.ForceUnmount = checkBoxForceUnmountAll.Checked;
        }

        private void checkBoxWarnUnmountAll_CheckedChanged(object sender, EventArgs e)
        {
            config.UnmountWarning = checkBoxWarnUnmountAll.Checked;
        }

        private void checkBoxDisableBallons_CheckedChanged(object sender, EventArgs e)
        {
            config.DisableBalloons = checkBoxDisableBalloons.Checked;
        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLanguage.Text == "English")
                config.Language = new CultureInfo("en");
            if (comboBoxLanguage.Text == "Deutsch")
                config.Language = new CultureInfo("de");
        }

        private void numericUpDownBalloonTime_ValueChanged(object sender, EventArgs e)
        {
            config.BalloonTimePeriod = (int)numericUpDownBalloonTime.Value;
        }

        private void checkBoxCheckUpdates_CheckedChanged(object sender, EventArgs e)
        {
            config.CheckForUpdates = checkBoxCheckUpdates.Checked;
        }

        private void checkBoxWarnOnExit_CheckedChanged(object sender, EventArgs e)
        {
            config.WarnOnExit = checkBoxWarnOnExit.Checked;
        }
        #endregion

        #endregion

        #region Key Devices Tab Events

        /// <summary>
        /// Adds a new key device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddKeyDevice_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            ManagementObject diskDrive = keyDeviceList[comboBoxUSBKeyDevice.SelectedIndex][0];
            ManagementObject diskPartition = keyDeviceList[comboBoxUSBKeyDevice.SelectedIndex][1];

            UsbKeyDevice newKeyDevice = new UsbKeyDevice();
            newKeyDevice.Caption = diskDrive["Caption"].ToString();
            newKeyDevice.Signature = (uint)diskDrive["Signature"];
            newKeyDevice.PartitionIndex = ((uint)diskPartition["Index"] + 1);
            newKeyDevice.IsActive = checkBoxKeyDeviceActive.Checked;

            // detect if disk is already configured
            if (!config.KeyDevices.Contains(newKeyDevice))
            {
                config.KeyDevices.Add(newKeyDevice);
            }
            else
            {
                // this key device already exists
                Cursor.Current = Cursors.Default;
                MessageBox.Show(langRes.GetString("MsgTDevDouble"), langRes.GetString("MsgHDevDouble"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            listBoxKeyDevices.Items.Add(newKeyDevice);
            listBoxKeyDevices.SelectedItem = newKeyDevice;
            listBoxKeyDevices.Enabled = true;
            panelKeyDevice.Visible = true;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Gets called on key device selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxKeyDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxKeyDevices.SelectedIndex != -1 &&
                config.KeyDevices.Count > 0 &&
                config.KeyDevices.Count >= listBoxKeyDevices.Items.Count)
            {
                UsbKeyDevice keyDevice = config.KeyDevices[listBoxKeyDevices.SelectedIndex];
                labelKeyDevCaption.Text = keyDevice.Caption;
                labelKeyDevSignature.Text = keyDevice.Signature.ToString();
                labelKeyDevPartition.Text = keyDevice.PartitionIndex.ToString();
                checkBoxKeyDeviceActive.Checked = keyDevice.IsActive;
                panelKeyDevice.Visible = true;
                return;
            }

            if (listBoxKeyDevices.Items.Count > 0 && config.KeyDevices.Count > 0)
            {
                listBoxKeyDevices.SelectedItem = config.KeyDevices.First();
                panelKeyDevice.Visible = true;
                return;
            }

            // if nothing to do, reset components and return
            labelKeyDevCaption.Text = string.Empty;
            labelKeyDevSignature.Text = string.Empty;
            labelKeyDevPartition.Text = string.Empty;
            panelKeyDevice.Visible = false;
        }

        /// <summary>
        /// Deletes key device from configuration and tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteKeyDevice_Click(object sender, EventArgs e)
        {
            config.KeyDevices.RemoveAt(listBoxKeyDevices.SelectedIndex);
            listBoxKeyDevices.Items.Remove(listBoxKeyDevices.SelectedItem);

            if (config.KeyDevices.Count <= 0)
            {
                panelKeyDevice.Visible = false;
                listBoxKeyDevices.Enabled = false;
            }
        }

        /// <summary>
        /// Activates key device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxKeyDeviceActive_CheckedChanged(object sender, EventArgs e)
        {
            config.KeyDevices[listBoxKeyDevices.SelectedIndex].IsActive = checkBoxKeyDeviceActive.Checked;
        }

        #endregion

        #region Disk Drives Tab Events

        /// <summary>
        /// Add new encrypted disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddDisk_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            // no new disk until new disk is removed or saved
            groupBoxLocalDiskDrives.Enabled = false;

            // get disk and partition from linked list
            ManagementObject newDisk = encDeviceList[comboBoxDiskDrives.SelectedIndex][0];
            // if the entire disk is encrypted there is no partition
            ManagementObject newPart = null;
            // if there is no partition, the partition index is 0
            if (int.Parse(comboBoxDiskPartitions.SelectedItem.ToString()) > 0)
                newPart = encDeviceList[comboBoxDiskDrives.SelectedIndex][int.Parse(comboBoxDiskPartitions.SelectedItem.ToString())];

            // temporary encrypted disk object to check for doubles
            EncryptedDiskPartition temp =
                new EncryptedDiskPartition((string)newDisk["Caption"],
                    (uint)newDisk["Signature"],
                    (newPart != null) ? (uint)newPart["Index"] : 0);

            if (config.EncryptedDiskPartitions.Contains(temp))
            {
                MessageBox.Show(langRes.GetString("MsgTDiskDouble"), langRes.GetString("MsgHDiskDouble"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                groupBoxLocalDiskDrives.Enabled = true;
                return;
            }

            // fill information into form elements
            labelDiskCaption.Text = (string)newDisk["Caption"];
            labelDiskSignature.Text = ((uint)newDisk["Signature"]).ToString();
            if (newPart != null)
                labelDiskPartition.Text = ((uint)newPart["Index"] + 1).ToString();
            else
                labelDiskPartition.Text = comboBoxDiskPartitions.SelectedItem.ToString();
            textBoxDiskPasswordFile.Text = null;
            checkBoxDiskActive.Checked = true;
            checkBoxDiskOpenExplorer.Checked = false;
            checkBoxFetchDiskPassword.Checked = false;

            string diskTitle = labelDiskCaption.Text + ", Partition: " + labelDiskPartition.Text;

            // new disk, new node in tree
            listBoxDisks.Items.Add(diskTitle);
            listBoxDisks.SelectedItem = diskTitle;

            // new key files list
            diskKeyFilesList.Add(new List<string>());

            // unsaved content will raise a save dialog box
            editInProgress = true;
            // panel to add disks must be hidden
            panelLocalDiskDrives.Visible = false;
            // enable disk details area and make visible
            groupBoxDiskDetails.Text = diskTitle;
            groupBoxDiskDetails.Visible = true;
            groupBoxDiskDetails.Enabled = true;
            // restore old color
            tabPageDiskDrives.BackColor = Color.Transparent;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Search for password file and save path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearchDiskPasswordFile_Click(object sender, EventArgs e)
        {
            // search for password file, no error handling needed here
            if (openFileDialogGeneral.ShowDialog() == DialogResult.OK)
                textBoxDiskPasswordFile.Text = openFileDialogGeneral.FileName;
        }

        /// <summary>
        /// Re-sets the partition dropdown list after selection of new disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxDiskDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            // add them to partition dropdown list
            comboBoxDiskPartitions.BeginUpdate();
            comboBoxDiskPartitions.Items.Clear();
            comboBoxDiskPartitions.Items.Add("0");
            foreach (ManagementObject dPartition in encDeviceList[comboBoxDiskDrives.SelectedIndex][0].GetRelated(SystemDevices.Win32_DiskPartition))
                comboBoxDiskPartitions.Items.Add(((uint)dPartition["Index"] + 1).ToString());
            comboBoxDiskPartitions.SelectedIndex = 0;
            comboBoxDiskPartitions.EndUpdate();
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Save Disk in configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveDisk_Click(object sender, EventArgs e)
        {
            EncryptedDiskPartition newEncDiskPartition = new EncryptedDiskPartition(labelDiskCaption.Text,
                uint.Parse(labelDiskSignature.Text), uint.Parse(labelDiskPartition.Text));

            // save all the params in the new object
            newEncDiskPartition.PasswordFile = textBoxDiskPasswordFile.Text;
            newEncDiskPartition.IsActive = checkBoxDiskActive.Checked;
            newEncDiskPartition.OpenExplorer = checkBoxDiskOpenExplorer.Checked;
            newEncDiskPartition.Readonly = checkBoxDiskRo.Checked;
            newEncDiskPartition.Removable = checkBoxDiskRm.Checked;
            newEncDiskPartition.System = checkBoxDiskSm.Checked;
            newEncDiskPartition.Timestamp = checkBoxDiskTs.Checked;
            newEncDiskPartition.KeyFiles = diskKeyFilesList[listBoxDisks.SelectedIndex];
            newEncDiskPartition.TriggerDismount = checkBoxDiskDismountTrigger.Checked;
            newEncDiskPartition.FetchUserPassword = checkBoxFetchDiskPassword.Checked;
            if(radioButtonDiskDLFixed.Checked)
                newEncDiskPartition.DriveLetter = comboBoxDiskDriveLetter.Text;
            else
            {
                newEncDiskPartition.DriveLetter = string.Empty;
                newEncDiskPartition.NextFreeLetter = radioButtonDiskDLFree.Checked;
                newEncDiskPartition.RandomFreeLetter = radioButtonDiskDLRandom.Checked;
            }

            // replace the disk with the new settings
            if (config.EncryptedDiskPartitions.Contains(newEncDiskPartition))
                config.EncryptedDiskPartitions[config.EncryptedDiskPartitions.IndexOf(newEncDiskPartition)] = newEncDiskPartition;
            else
                config.EncryptedDiskPartitions.Add(newEncDiskPartition);

            MessageBox.Show(langRes.GetString("MsgTSaved"), langRes.GetString("MsgHSaved"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            // we saved the content, no warn dialog on close needed
            editInProgress = false;
            groupBoxExistingDisks.Enabled = true;
            pictureBoxAddDisk.Enabled = true;
        }

        /// <summary>
        /// Deletes disk from config and tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteDiskDrive_Click(object sender, EventArgs e)
        {
            // we need disks to delete
            if (config.EncryptedDiskPartitions.Count > listBoxDisks.SelectedIndex)
            {
                config.EncryptedDiskPartitions.RemoveAt(listBoxDisks.SelectedIndex);
                diskKeyFilesList.RemoveAt(listBoxDisks.SelectedIndex);
            }

            listBoxDisks.Items.Remove(listBoxDisks.SelectedItem);

            editInProgress = false;
            groupBoxDiskDetails.Visible = false; // first!
            groupBoxLocalDiskDrives.Enabled = true;
            listBoxDisks.Enabled = true;
        }

        /// <summary>
        /// Opens key files edit dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEditDiskKeyFiles_Click(object sender, EventArgs e)
        {
            KeyFilesDialog kfd = new KeyFilesDialog();
            kfd.KeyFiles = diskKeyFilesList[listBoxDisks.SelectedIndex];
            if (kfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                diskKeyFilesList[listBoxDisks.SelectedIndex] = kfd.KeyFiles;
            kfd = null;
        }

        /// <summary>
        /// Gets called on selected disk change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxDisks_SelectedIndexChanged(object sender, EventArgs e)
        {
            // re-create the list of available drive letters
            comboBoxDiskDriveLetter.BeginUpdate();
            comboBoxDiskDriveLetter.Items.Clear();
            if (!config.IgnoreAssignedDriveLetters)
                comboBoxDiskDriveLetter.Items.AddRange(SystemDevices.FreeDriveLetters.ToArray());
            else
                comboBoxDiskDriveLetter.Items.AddRange(SystemDevices.AllDriveLetters.ToArray());
            if (comboBoxDiskDriveLetter.Items.Count > 0)
                comboBoxDiskDriveLetter.SelectedIndex = 0;
            comboBoxDiskDriveLetter.EndUpdate();

            // we must have disks available to display it
            if (config.EncryptedDiskPartitions.Count > 0 &&
                config.EncryptedDiskPartitions.Count >= listBoxDisks.Items.Count &&
                listBoxDisks.SelectedIndex != -1)
            {
                EncryptedDiskPartition encDiskPartition =
                    config.EncryptedDiskPartitions[listBoxDisks.SelectedIndex];

                // fill up informations
                labelDiskCaption.Text = encDiskPartition.DiskCaption;
                labelDiskSignature.Text = encDiskPartition.DiskSignature.ToString();
                labelDiskPartition.Text = encDiskPartition.PartitionIndex.ToString();
                textBoxDiskPasswordFile.Text = encDiskPartition.PasswordFile;
                checkBoxDiskActive.Checked = encDiskPartition.IsActive;
                checkBoxDiskOpenExplorer.Checked = encDiskPartition.OpenExplorer;
                checkBoxDiskRo.Checked = encDiskPartition.Readonly;
                checkBoxDiskRm.Checked = encDiskPartition.Removable;
                checkBoxDiskTs.Checked = encDiskPartition.Timestamp;
                checkBoxDiskSm.Checked = encDiskPartition.System;
                checkBoxDiskDismountTrigger.Checked = encDiskPartition.TriggerDismount;
                checkBoxFetchDiskPassword.Checked = encDiskPartition.FetchUserPassword;

                // decide if a fixed letter is assigned or free/random is checked
                if (!string.IsNullOrEmpty(encDiskPartition.DriveLetter))
                {
                    // if the letter of the drive is not in the list, add it an first position
                    if (!comboBoxDiskDriveLetter.Items.Contains(encDiskPartition.DriveLetter))
                        comboBoxDiskDriveLetter.Items.Insert(0, encDiskPartition.DriveLetter);
                    comboBoxDiskDriveLetter.SelectedItem = encDiskPartition.DriveLetter;
                    radioButtonDiskDLFixed.Checked = true;
                }
                else
                {
                    radioButtonDiskDLFree.Checked = encDiskPartition.NextFreeLetter;
                    radioButtonDiskDLRandom.Checked = encDiskPartition.RandomFreeLetter;                        
                }

                // after everything is filled with data, make panel visible
                groupBoxDiskDetails.Text = encDiskPartition.ToString();
                groupBoxDiskDetails.Visible = true;
            }
        }

        private void pictureBoxAddDisk_Click(object sender, EventArgs e)
        {
            // gray out background
            tabPageDiskDrives.BackColor = Color.LightGray;
            // disable list of existing disks to prevent events
            groupBoxExistingDisks.Enabled = false;
            // disable disk details area
            groupBoxDiskDetails.Enabled = false;
            // disable Add Disk button
            pictureBoxAddDisk.Enabled = false;
            // make disk panel visible and bring it to front
            panelLocalDiskDrives.Visible = true;
            panelLocalDiskDrives.Enabled = true;
            panelLocalDiskDrives.BringToFront();
        }

        private void linkLabelAddDiskCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pictureBoxAddDisk.Enabled = true;
            // restore old color
            tabPageDiskDrives.BackColor = Color.Transparent;
            // panel to add disks must be hidden
            panelLocalDiskDrives.Visible = false;
            // enable disk details area and make visible
            groupBoxDiskDetails.Enabled = true;
            groupBoxExistingDisks.Enabled = true;
        }

        private void radioButtonDiskDLFixed_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDiskDLFixed.Checked)
                comboBoxDiskDriveLetter.Enabled = true;
            else
                comboBoxDiskDriveLetter.Enabled = false;
        }

        #endregion

        #region TrueCrypt Tab Events

        private void checkBoxCache_CheckedChanged(object sender, EventArgs e)
        {
            config.TrueCrypt.Cache = checkBoxCache.Checked;
        }

        private void checkBoxExplorer_CheckedChanged(object sender, EventArgs e)
        {
            config.TrueCrypt.Explorer = checkBoxExplorer.Checked;
        }

        private void checkBoxBeep_CheckedChanged(object sender, EventArgs e)
        {
            config.TrueCrypt.Beep = checkBoxBeep.Checked;
        }

        /// <summary>
        /// Search for TrueCrypt on local file system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearchTrueCrypt_Click(object sender, EventArgs e)
        {
            if (openFileDialogTrueCrypt.ShowDialog() == DialogResult.OK)
            {
                textBoxTrueCryptExec.Text = openFileDialogTrueCrypt.FileName;
                config.TrueCrypt.ExecutablePath = openFileDialogTrueCrypt.FileName;
            }
            else
            {
                if (string.IsNullOrEmpty(textBoxTrueCryptExec.Text))
                    MessageBox.Show(langRes.GetString("MsgTNoTCSet"), langRes.GetString("MsgHNoTCSet"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void pictureBoxTrueCryptHeader_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.truecrypt.org/");
        }

        private void checkBoxShowLauncherError_CheckedChanged(object sender, EventArgs e)
        {
            config.TrueCrypt.ShowErrors = checkBoxShowLauncherError.Checked;
        }

        #endregion

        #region Container Files Tab Events

        private void buttonAddContainer_Click(object sender, EventArgs e)
        {
            if (openFileDialogGeneral.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!listBoxContainerFiles.Items.Contains(openFileDialogGeneral.FileName))
                    listBoxContainerFiles.Items.Add(openFileDialogGeneral.FileName);
            }
            else return;

            textBoxConPasswordFile.Text = null;
            checkBoxOpenConExplorer.Checked = false;
            checkBoxConActive.Checked = true;
            checkBoxConRo.Checked = false;
            checkBoxConRm.Checked = false;
            checkBoxConSm.Checked = false;
            checkBoxConTs.Checked = false;
            checkBoxFetchConPassword.Checked = false;

            listBoxContainerFiles.SelectedItem = openFileDialogGeneral.FileName;

            // new empty key files list
            containerKeyFilesList.Add(new List<string>());

            buttonAddContainer.Enabled = false;
            listBoxContainerFiles.Enabled = false;
            groupBoxConSettings.Enabled = true;
        }

        private void listBoxContainerFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxContainerFiles.Items.Count == 0)
            {
                buttonRemoveContainer.Enabled = false;
                buttonSaveContainer.Enabled = false;
                buttonEditContainerKeyFiles.Enabled = false;
                groupBoxConSettings.Enabled = false;
            }
            else
            {
                buttonRemoveContainer.Enabled = true;
                buttonSaveContainer.Enabled = true;
                buttonEditContainerKeyFiles.Enabled = true;
            }

            // insert free or all drive letters
            comboBoxConLetter.BeginUpdate();
            comboBoxConLetter.Items.Clear();
            if (!config.IgnoreAssignedDriveLetters)
                comboBoxConLetter.Items.AddRange(SystemDevices.FreeDriveLetters.ToArray());
            else
                comboBoxConLetter.Items.AddRange(SystemDevices.AllDriveLetters.ToArray());
            if (comboBoxConLetter.Items.Count > 0)
                comboBoxConLetter.SelectedIndex = 0;
            comboBoxConLetter.EndUpdate();

            // we must have files available to display it
            if (config.EncryptedContainerFiles.Count > 0 &&
                config.EncryptedContainerFiles.Count >= listBoxContainerFiles.Items.Count &&
                listBoxContainerFiles.SelectedIndex != -1)
            {
                EncryptedContainerFile encContainerFile =
                    config.EncryptedContainerFiles[listBoxContainerFiles.SelectedIndex];

                // fill up informations
                textBoxConPasswordFile.Text = encContainerFile.PasswordFile;
                checkBoxConActive.Checked = encContainerFile.IsActive;
                checkBoxOpenConExplorer.Checked = encContainerFile.OpenExplorer;
                checkBoxConRo.Checked = encContainerFile.Readonly;
                checkBoxConRm.Checked = encContainerFile.Removable;
                checkBoxConTs.Checked = encContainerFile.Timestamp;
                checkBoxConSm.Checked = encContainerFile.System;
                checkBoxConDismountTrigger.Checked = encContainerFile.TriggerDismount;
                checkBoxFetchConPassword.Checked = encContainerFile.FetchUserPassword;

                if (encContainerFile.DriveLetter != null)
                {
                    // if the letter of the drive is not in the list, add it an first position
                    if (!comboBoxConLetter.Items.Contains(encContainerFile.DriveLetter))
                        comboBoxConLetter.Items.Insert(0, encContainerFile.DriveLetter);
                    comboBoxConLetter.SelectedItem = encContainerFile.DriveLetter;
                    radioButtonConDLFixed.Checked = true;
                }
                else
                {
                    radioButtonConDLFree.Checked = encContainerFile.NextFreeLetter;
                    radioButtonConDLRandom.Checked = encContainerFile.RandomFreeLetter;
                }

                groupBoxConSettings.Enabled = true;
            }
        }

        private void buttonRemoveContainer_Click(object sender, EventArgs e)
        {
            // we need container files to delete
            if (config.EncryptedContainerFiles.Count > listBoxContainerFiles.SelectedIndex)
            {
                config.EncryptedContainerFiles.RemoveAt(listBoxContainerFiles.SelectedIndex);
                containerKeyFilesList.RemoveAt(listBoxContainerFiles.SelectedIndex);
            }

            listBoxContainerFiles.Items.Remove(listBoxContainerFiles.SelectedItem);

            if (listBoxContainerFiles.Items.Count > 0)
                listBoxContainerFiles.SelectedIndex = 0;

            editInProgress = false;
            buttonAddContainer.Enabled = true;
        }

        private void buttonSearchConPassword_Click(object sender, EventArgs e)
        {
            if (openFileDialogGeneral.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBoxConPasswordFile.Text = openFileDialogGeneral.FileName;
        }

        private void buttonSaveContainer_Click(object sender, EventArgs e)
        {
            EncryptedContainerFile newContainerFile =
                new EncryptedContainerFile(listBoxContainerFiles.SelectedItem.ToString());

            newContainerFile.IsActive = checkBoxConActive.Checked;
            newContainerFile.KeyFiles = containerKeyFilesList[listBoxContainerFiles.SelectedIndex];
            newContainerFile.OpenExplorer = checkBoxOpenConExplorer.Checked;
            newContainerFile.PasswordFile = textBoxConPasswordFile.Text;
            newContainerFile.Readonly = checkBoxConRo.Checked;
            newContainerFile.Removable = checkBoxConRm.Checked;
            newContainerFile.System = checkBoxConSm.Checked;
            newContainerFile.Timestamp = checkBoxConTs.Checked;
            newContainerFile.TriggerDismount = checkBoxConDismountTrigger.Checked;
            newContainerFile.FetchUserPassword = checkBoxFetchConPassword.Checked;
            if (radioButtonConDLFixed.Checked)
                newContainerFile.DriveLetter = comboBoxDiskDriveLetter.Text;
            else
            {
                newContainerFile.DriveLetter = string.Empty;
                newContainerFile.NextFreeLetter = radioButtonConDLFree.Checked;
                newContainerFile.RandomFreeLetter = radioButtonConDLRandom.Checked;
            }

            if (config.EncryptedContainerFiles.Contains(newContainerFile))
                config.EncryptedContainerFiles[config.EncryptedContainerFiles.IndexOf(newContainerFile)] = newContainerFile;
            else
                config.EncryptedContainerFiles.Add(newContainerFile);

            MessageBox.Show(langRes.GetString("MsgTSaved"), langRes.GetString("MsgHSaved"),
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

            editInProgress = false;
            listBoxContainerFiles.Enabled = true;
            buttonAddContainer.Enabled = true;
        }

        private void buttonEditContainerKeyFiles_Click(object sender, EventArgs e)
        {
            // open new key files dialog with existing key files
            KeyFilesDialog kfd = new KeyFilesDialog(containerKeyFilesList[listBoxContainerFiles.SelectedIndex]);
            // if the user edited something (dialog closed with save/OK) overwrite old key files list
            if (kfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                containerKeyFilesList[listBoxContainerFiles.SelectedIndex] = kfd.KeyFiles;
            kfd = null;
        }

        private void radioButtonConDLFixed_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonConDLFixed.Checked)
                comboBoxConLetter.Enabled = true;
            else
                comboBoxConLetter.Enabled = false;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Builds the device lists.
        /// </summary>
        private void BuildDeviceList()
        {
            // fill up device list
            int keyIndex = 0, encIndex = 0;
            List<ManagementObject> keyDeviceMobjects = null, encDeviceMobjects = null;
            comboBoxUSBKeyDevice.BeginUpdate();
            comboBoxDiskDrives.BeginUpdate();
            // walk through every physical disk
            foreach (ManagementObject dDrive in SystemDevices.DiskDrives)
            {
                encDeviceMobjects = new List<ManagementObject>();
                encDeviceMobjects.Add(dDrive);
                // add disk caption to dropdown list
                comboBoxDiskDrives.Items.Add(dDrive["Caption"].ToString());
                // walk through every partition
                foreach (ManagementObject dPartition in dDrive.GetRelated(SystemDevices.Win32_DiskPartition))
                {
                    encDeviceMobjects.Add(dPartition);
                    // get the logical disk
                    foreach (ManagementObject lDisk in dPartition.GetRelated(SystemDevices.Win32_LogicalDisk))
                    {
                        // create title of key device and add it to dropdown list
                        comboBoxUSBKeyDevice.Items.Add(dDrive["Caption"].ToString() +
                            langRes.GetString("CBoxPartition") +
                            ((uint)dPartition["Index"] + 1).ToString() +
                            langRes.GetString("CBoxLetter") +
                            lDisk["Name"].ToString());
                        // we have all the information, let's save it
                        keyDeviceMobjects = new List<ManagementObject>();
                        // add disk
                        keyDeviceMobjects.Add(dDrive);
                        // add partition
                        keyDeviceMobjects.Add(dPartition);
                        // add logical disk
                        keyDeviceMobjects.Add(lDisk);
                        // save it in list
                        keyDeviceList.Add(keyIndex++, keyDeviceMobjects);
                    }
                }
                encDeviceList.Add(encIndex++, encDeviceMobjects);
            }
            comboBoxUSBKeyDevice.EndUpdate();
            comboBoxDiskDrives.EndUpdate();

            // there is nothing to select if empty
            if (comboBoxUSBKeyDevice.Items.Count > 0)
                comboBoxUSBKeyDevice.SelectedIndex = 0;
            if (comboBoxDiskDrives.Items.Count > 0)
                comboBoxDiskDrives.SelectedIndex = 0;
        }

        private void buttonAVMessage_Click(object sender, EventArgs e)
        {
            new AVWarningDialog().ShowDialog();
        }

        #endregion
    }
}
