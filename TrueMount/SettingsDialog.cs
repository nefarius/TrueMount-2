using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace TrueMount
{
    partial class SettingsDialog : Form
    {
        private Configuration config = null;
        private Dictionary<int, List<ManagementObject>> keyDeviceList = null;
        private Dictionary<int, List<ManagementObject>> encDeviceList = null;
        private ResourceManager langRes = null;
        private CultureInfo culture = null;
        private List<List<string>> keyFilesList = null;
        private bool editInProgress = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsDialog(ref Configuration config)
        {
            this.config = config;

            // new list ob available SystemDevices
            keyDeviceList = new Dictionary<int, List<ManagementObject>>();
            encDeviceList = new Dictionary<int, List<ManagementObject>>();

            // list of key files
            keyFilesList = new List<List<string>>();

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
        /// Returns the changed configuration references.
        /// </summary>
        /// <param name="config">The Configuration object.</param>
        public void UpdateConfiguration(ref Configuration config)
        {
            config = this.config;
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

            checkBoxBackground.Checked = config.TrueCrypt.Background;
            checkBoxBeep.Checked = config.TrueCrypt.Beep;
            checkBoxCache.Checked = config.TrueCrypt.Cache;
            checkBoxExplorer.Checked = config.TrueCrypt.Explorer;
            checkBoxSilent.Checked = config.TrueCrypt.Silent;

            // read key SystemDevices and create nodes
            foreach (UsbKeyDevice keyDevice in config.KeyDevices)
            {
                TreeNode keyDeviceNode = new TreeNode(keyDevice.Caption);
                treeViewKeyDevices.Nodes.Add(keyDeviceNode);
                treeViewKeyDevices.Enabled = true;
            }
            treeViewKeyDevices.ExpandAll();

            // read disk drives and create nodes
            foreach (EncryptedDiskPartition encDiskPartition in config.EncryptedDiskPartitions)
            {
                keyFilesList.Add(encDiskPartition.KeyFiles);
                TreeNode encDeviceNode = new TreeNode(encDiskPartition.DiskCaption +
                    ", Partition: " + encDiskPartition.PartitionIndex.ToString());
                treeViewDisks.Nodes.Add(encDeviceNode);
                treeViewDisks.Enabled = true;
            }
            treeViewDisks.ExpandAll();

            // create dictionary with indexd management objects (disks, partitions, ...)
            BuildDeviceList();

            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Builds the device lists (the most evil code I ever wrote :/)
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

        private void checkBoxOneInstance_CheckedChanged(object sender, EventArgs e)
        {
            config.OnlyOneInstance = checkBoxOneInstance.Checked;
        }

        /// <summary>
        /// Add new encrypted disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddDisk_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            groupBoxLocalDiskDrives.Enabled = false;

            // get disk and partition from linked list
            ManagementObject newDisk = encDeviceList[comboBoxDiskDrives.SelectedIndex][0];

            ManagementObject newPart = null;
            if (int.Parse(comboBoxDiskPartitions.SelectedItem.ToString()) > 0)
                newPart = encDeviceList[comboBoxDiskDrives.SelectedIndex][int.Parse(comboBoxDiskPartitions.SelectedItem.ToString())];

            // fill information into form elements
            textBoxDiskCaption.Text = (string)newDisk["Caption"];
            textBoxDiskSignature.Text = ((uint)newDisk["Signature"]).ToString();
            if (newPart != null)
                textBoxDiskPartition.Text = ((uint)newPart["Index"] + 1).ToString();
            else
                textBoxDiskPartition.Text = comboBoxDiskPartitions.SelectedItem.ToString();
            textBoxPasswordFile.Text = null;
            checkBoxDiskActive.Checked = true;
            checkBoxDiskOpenExplorer.Checked = false;

            // new disk, new node in tree
            TreeNode diskNode = new TreeNode(comboBoxDiskDrives.Text);
            treeViewDisks.Nodes.Add(diskNode);
            treeViewDisks.SelectedNode = diskNode;

            // new key files list
            keyFilesList.Add(new List<string>());

            editInProgress = true;
            panelDisks.Visible = true;
            treeViewDisks.Enabled = false;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Event after something in the disk tree has been changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewDisks_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // re-create the list of available drive letters
            comboBoxDriveLetter.BeginUpdate();
            comboBoxDriveLetter.Items.Clear();
            if (!config.IgnoreAssignedDriveLetters)
                comboBoxDriveLetter.Items.AddRange(SystemDevices.FreeDriveLetters.ToArray());
            else
                comboBoxDriveLetter.Items.AddRange(SystemDevices.AllDriveLetters.ToArray());
            if (comboBoxDriveLetter.Items.Count > 0)
                comboBoxDriveLetter.SelectedIndex = 0;
            comboBoxDriveLetter.EndUpdate();

            // we must have disks available to display it
            if (config.EncryptedDiskPartitions.Count > 0)
            {
                if (config.EncryptedDiskPartitions.Count > treeViewDisks.SelectedNode.Index)
                {
                    EncryptedDiskPartition encDiskPartition =
                        config.EncryptedDiskPartitions[treeViewDisks.SelectedNode.Index];

                    // fill up informations
                    textBoxDiskCaption.Text = encDiskPartition.DiskCaption;
                    textBoxDiskSignature.Text = encDiskPartition.DiskSignature.ToString();
                    textBoxDiskPartition.Text = encDiskPartition.PartitionIndex.ToString();
                    textBoxPasswordFile.Text = encDiskPartition.PasswordFile;
                    checkBoxDiskActive.Checked = encDiskPartition.IsActive;
                    checkBoxDiskOpenExplorer.Checked = encDiskPartition.OpenExplorer;
                    checkBoxRo.Checked = encDiskPartition.Readonly;
                    checkBoxRm.Checked = encDiskPartition.Removable;
                    checkBoxTs.Checked = encDiskPartition.Timestamp;
                    checkBoxSm.Checked = encDiskPartition.System;

                    // if the letter of the drive is not in the list, add it an first position
                    if (!comboBoxDriveLetter.Items.Contains(encDiskPartition.DriveLetter))
                        comboBoxDriveLetter.Items.Insert(0, encDiskPartition.DriveLetter);
                    comboBoxDriveLetter.SelectedItem = encDiskPartition.DriveLetter;

                    // after everything is filled with data, make panel visible
                    panelDisks.Visible = true;
                }
            }
        }

        /// <summary>
        /// Search for password file and save path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearchPasswordFile_Click(object sender, EventArgs e)
        {
            // search for password file, no error handling needed here
            if (openFileDialogPassword.ShowDialog() == DialogResult.OK)
                textBoxPasswordFile.Text = openFileDialogPassword.FileName;
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
            EncryptedDiskPartition newEncDiskPartition = new EncryptedDiskPartition(textBoxDiskCaption.Text,
                uint.Parse(textBoxDiskSignature.Text), uint.Parse(textBoxDiskPartition.Text));

            // save all the params in the new object
            newEncDiskPartition.PasswordFile = textBoxPasswordFile.Text;
            newEncDiskPartition.DriveLetter = comboBoxDriveLetter.Text;
            newEncDiskPartition.IsActive = checkBoxDiskActive.Checked;
            newEncDiskPartition.OpenExplorer = checkBoxDiskOpenExplorer.Checked;
            newEncDiskPartition.Readonly = checkBoxRo.Checked;
            newEncDiskPartition.Removable = checkBoxRm.Checked;
            newEncDiskPartition.System = checkBoxSm.Checked;
            newEncDiskPartition.Timestamp = checkBoxTs.Checked;
            newEncDiskPartition.KeyFiles = keyFilesList[treeViewDisks.SelectedNode.Index];

            // if exactly the same object exists in the databse, delete and re-save it
            if (!config.EncryptedDiskPartitions.Contains(newEncDiskPartition))
            {
                config.EncryptedDiskPartitions.Add(newEncDiskPartition);
            }
            else
            {
                config.EncryptedDiskPartitions.Remove(newEncDiskPartition);
                config.EncryptedDiskPartitions.Add(newEncDiskPartition);
            }

            MessageBox.Show(langRes.GetString("MsgTSaved"), langRes.GetString("MsgHSaved"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            editInProgress = false;
            groupBoxLocalDiskDrives.Enabled = true;
            treeViewDisks.Enabled = true;
        }

        /// <summary>
        /// Gets called after every selection change of the key device tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewKeyDevices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (config.KeyDevices.Count > 0)
            {
                if (config.KeyDevices.Count > treeViewKeyDevices.SelectedNode.Index)
                {
                    UsbKeyDevice keyDevice = config.KeyDevices[treeViewKeyDevices.SelectedNode.Index];
                    textBoxUSBCaption.Text = keyDevice.Caption;
                    textBoxUSBSignature.Text = keyDevice.Signature.ToString();
                    textBoxUSBPartition.Text = keyDevice.PartitionIndex.ToString();
                    checkBoxKeyDeviceActive.Checked = keyDevice.IsActive;
                    panelKeyDevice.Visible = true;
                    return;
                }
            }

            // if nothing to do, reset components and return
            textBoxUSBCaption.Text = null;
            textBoxUSBSignature.Text = null;
            textBoxUSBPartition.Text = null;
            panelKeyDevice.Visible = false;
        }

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

            // if everything went well, build the node and view it
            TreeNode newKeyDeviceNode = new TreeNode(newKeyDevice.Caption);
            treeViewKeyDevices.Nodes.Add(newKeyDeviceNode);
            treeViewKeyDevices.SelectedNode = newKeyDeviceNode;
            treeViewKeyDevices.ExpandAll();
            treeViewKeyDevices.Focus();
            treeViewKeyDevices.Enabled = true;
            panelKeyDevice.Visible = true;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Deletes key device from configuration and tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteKeyDevice_Click(object sender, EventArgs e)
        {
            config.KeyDevices.RemoveAt(treeViewKeyDevices.SelectedNode.Index);
            treeViewKeyDevices.SelectedNode.Remove();

            if (config.KeyDevices.Count <= 0)
            {
                panelKeyDevice.Visible = false;
                treeViewKeyDevices.Enabled = false;
            }
        }

        /// <summary>
        /// Deletes disk from config and tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteDiskDrive_Click(object sender, EventArgs e)
        {
            int index = treeViewDisks.SelectedNode.Index;
            // we need disks to delete
            if (config.EncryptedDiskPartitions.Count > index)
            {
                config.EncryptedDiskPartitions.RemoveAt(index);
                keyFilesList.RemoveAt(index);
            }

            editInProgress = false;
            panelDisks.Visible = false; // first!
            treeViewDisks.SelectedNode.Remove();
            groupBoxLocalDiskDrives.Enabled = true;
            treeViewDisks.Enabled = true;
        }

        private void checkBoxKeyDeviceActive_CheckedChanged(object sender, EventArgs e)
        {
            config.KeyDevices[treeViewKeyDevices.SelectedNode.Index].IsActive = checkBoxKeyDeviceActive.Checked;
        }

        private void buttonListDisks_Click(object sender, EventArgs e)
        {
            new ListDisksDialog().ShowDialog();
        }

        private void checkBoxBackground_CheckedChanged(object sender, EventArgs e)
        {
            config.TrueCrypt.Background = checkBoxBackground.Checked;
            if (checkBoxBackground.Checked)
            {
                //checkBoxSilent.Checked = true;
                checkBoxSilent.Enabled = true;
            }
            else
            {
                checkBoxSilent.Checked = false;
                checkBoxSilent.Enabled = false;
            }
        }

        private void checkBoxSilent_CheckedChanged(object sender, EventArgs e)
        {
            config.TrueCrypt.Silent = checkBoxSilent.Checked;
        }

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

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLanguage.Text == "English")
                config.Language = new CultureInfo("en");
            if (comboBoxLanguage.Text == "Deutsch")
                config.Language = new CultureInfo("de");
        }

        private void checkBoxIgnoreDriveLetters_CheckedChanged(object sender, EventArgs e)
        {
            config.IgnoreAssignedDriveLetters = checkBoxIgnoreDriveLetters.Checked;
        }

        private void buttonEditKeyFiles_Click(object sender, EventArgs e)
        {
            int index = treeViewDisks.SelectedNode.Index;
            KeyFilesDialog kfd = new KeyFilesDialog();
            kfd.KeyFiles = keyFilesList[index];
            if (kfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                keyFilesList[index] = kfd.KeyFiles;
            kfd = null;
        }

        private void SettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (editInProgress)
                if (MessageBox.Show(langRes.GetString("MsgTWarnNotSaved"), langRes.GetString("MsgHWarnNotSaved"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    e.Cancel = true;
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

        private void pictureBoxTrueCryptHeader_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.truecrypt.org/");
        }
    }
}
