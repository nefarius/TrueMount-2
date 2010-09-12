using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;

namespace TrueMount
{
    public partial class SettingsDialog : Form
    {
        IObjectContainer config_db = null;
        private Configuration config = null;
        private Dictionary<int, List<ManagementObject>> key_devices = null;
        private ResourceManager rm = null;
        private CultureInfo culture = null;
        private const string TC_CLI_URL = "http://www.truecrypt.org/docs/?s=command-line-usage";

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsDialog()
        {
            GetConfigFromDb();

            // new list ob available SystemDevices
            key_devices = new Dictionary<int, List<ManagementObject>>();

            // load translation resources
            rm = new ResourceManager("TrueMount.SettingsMessages",
                typeof(TrueMountMainWindow).Assembly);

            // set language from config or system default
            if (config.Language != null)
                Thread.CurrentThread.CurrentUICulture = config.Language;
            culture = Thread.CurrentThread.CurrentUICulture;

            InitializeComponent();
        }

        /// <summary>
        /// Load the configuration from the database.
        /// </summary>
        private void GetConfigFromDb()
        {
            // update settings
            Db4oFactory.Configure().ObjectClass(typeof(Configuration)).CascadeOnUpdate(true);
            Db4oFactory.Configure().ObjectClass(typeof(EncryptedDiskPartition)).CascadeOnUpdate(true);
            Db4oFactory.Configure().ObjectClass(typeof(UsbKeyDevice)).CascadeOnUpdate(true);
            // load database
            config_db = Db4oFactory.OpenFile(Configuration.ConfigDbFile);

            IEnumerable<Configuration> result =
                from Configuration c in config_db
                select c;

            if (result.Count() > 0)
                config = (Configuration)result.First();
            else
                config = new Configuration();
        }

        /// <summary>
        /// Gets executed on form load
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
            foreach (UsbKeyDevice key_device in config.KeyDevices)
            {
                TreeNode key_device_node = new TreeNode(key_device.Caption);
                treeViewKeyDevices.Nodes.Add(key_device_node);
                treeViewKeyDevices.Enabled = true;
            }
            treeViewKeyDevices.ExpandAll();

            // read disk drives and create nodes
            foreach (EncryptedDiskPartition enc_disk_partition in config.EncryptedDiskPartitions)
            {
                TreeNode enc_device_node = new TreeNode(enc_disk_partition.DiskCaption +
                    ", Partition: " + enc_disk_partition.PartitionIndex.ToString());
                treeViewDisks.Nodes.Add(enc_device_node);
                treeViewDisks.Enabled = true;
            }
            treeViewDisks.ExpandAll();

            // fill up device list
            int key_index = 0;
            List<ManagementObject> key_device_mobjects = null;
            comboBoxUSBKeyDevice.BeginUpdate();
            comboBoxDiskDrives.BeginUpdate();
            // walk through every physical disk
            foreach (ManagementObject ddrive in SystemDevices.DiskDrives)
            {
                // add disk caption to dropdown list
                comboBoxDiskDrives.Items.Add(ddrive["Caption"].ToString());
                // walk through every partition
                foreach (ManagementObject partition in ddrive.GetRelated(SystemDevices.Win32_DiskPartition))
                {
                    // get the logical disk
                    foreach (ManagementObject ldisk in partition.GetRelated(SystemDevices.Win32_LogicalDisk))
                    {
                        // create title of key device and add it to dropdown list
                        comboBoxUSBKeyDevice.Items.Add(ddrive["Caption"].ToString() +
                            rm.GetString("CBoxPartition") +
                            ((uint)partition["Index"] + 1).ToString() +
                            rm.GetString("CBoxLetter") +
                            ldisk["Name"].ToString());
                        // we have all the information, let's save it
                        key_device_mobjects = new List<ManagementObject>();
                        // add disk
                        key_device_mobjects.Add(ddrive);
                        // add partition
                        key_device_mobjects.Add(partition);
                        // add logical disk
                        key_device_mobjects.Add(ldisk);
                        // save it in list
                        key_devices.Add(key_index++, key_device_mobjects);
                    }
                }
            }
            comboBoxUSBKeyDevice.EndUpdate();
            comboBoxDiskDrives.EndUpdate();

            // there is nothing to select if empty
            if (comboBoxUSBKeyDevice.Items.Count > 0)
                comboBoxUSBKeyDevice.SelectedIndex = 0;
            if (comboBoxDiskDrives.Items.Count > 0)
                comboBoxDiskDrives.SelectedIndex = 0;

            Cursor.Current = Cursors.Default;
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
                    MessageBox.Show(rm.GetString("MsgTNoTCSet"), rm.GetString("MsgHNoTCSet"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(TC_CLI_URL);
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
            // new disk, new node in tree :)
            TreeNode disk_node = new TreeNode(comboBoxDiskDrives.Text);
            treeViewDisks.Nodes.Add(disk_node);
            treeViewDisks.SelectedNode = disk_node;

            // fetch drive information with disks caption
            var diskQuery =
                from ManagementObject disk in SystemDevices.DiskDrives
                where (string)disk["Caption"] == comboBoxDiskDrives.Text
                select disk;

            // first disk is what we want :)
            ManagementObject found_disk = diskQuery.First();

            // fill information into form elements
            textBoxDiskCaption.Text = (string)found_disk["Caption"];
            textBoxDiskSignature.Text = ((uint)found_disk["Signature"]).ToString();
            textBoxDiskPartition.Text = comboBoxDiskPartitions.Text;
            textBoxPasswordFile.Text = null;
            checkBoxDiskActive.Checked = true;
            checkBoxDiskOpenExplorer.Checked = false;

            // view and enable new disk node
            treeViewDisks.Focus();
            treeViewDisks.Enabled = true;
            panelDisks.Visible = true;
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
            comboBoxDriveLetter.Items.AddRange(SystemDevices.FreeDriveLetters.ToArray());
            comboBoxDriveLetter.SelectedIndex = 0;
            comboBoxDriveLetter.EndUpdate();

            // we must have disks available to display it
            if (config.EncryptedDiskPartitions.Count > 0)
            {
                if (config.EncryptedDiskPartitions.Count > treeViewDisks.SelectedNode.Index)
                {
                    EncryptedDiskPartition enc_disk_partition =
                    config.EncryptedDiskPartitions[treeViewDisks.SelectedNode.Index];

                    // fill up informations
                    textBoxDiskCaption.Text = enc_disk_partition.DiskCaption;
                    textBoxDiskSignature.Text = enc_disk_partition.DiskSignature.ToString();
                    textBoxDiskPartition.Text = enc_disk_partition.PartitionIndex.ToString();
                    textBoxPasswordFile.Text = enc_disk_partition.PasswordFile;
                    checkBoxDiskActive.Checked = enc_disk_partition.IsActive;
                    checkBoxDiskOpenExplorer.Checked = enc_disk_partition.OpenExplorer;
                    checkBoxRo.Checked = enc_disk_partition.Readonly;
                    checkBoxRm.Checked = enc_disk_partition.Removable;
                    checkBoxTs.Checked = enc_disk_partition.Timestamp;
                    checkBoxSm.Checked = enc_disk_partition.System;

                    if (!comboBoxDriveLetter.Items.Contains(enc_disk_partition.DriveLetter))
                        comboBoxDriveLetter.Items.Insert(0, enc_disk_partition.DriveLetter);
                    comboBoxDriveLetter.SelectedItem = enc_disk_partition.DriveLetter;

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
            // find related partitions
            var partQuery =
                from ManagementObject disk in SystemDevices.DiskDrives
                where (string)disk["Caption"] == comboBoxDiskDrives.Text
                from ManagementObject partition in disk.GetRelated(SystemDevices.Win32_DiskPartition)
                select partition;

            // add them to partition dropdown list
            comboBoxDiskPartitions.BeginUpdate();
            comboBoxDiskPartitions.Items.Clear();
            foreach (ManagementObject partition in partQuery)
                comboBoxDiskPartitions.Items.Add(((uint)partition["Index"] + 1).ToString());
            if (comboBoxDiskPartitions.Items.Count > 0)
                comboBoxDiskPartitions.SelectedIndex = 0;
            comboBoxDiskPartitions.EndUpdate();
        }

        /// <summary>
        /// Save Disk in configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveDisk_Click(object sender, EventArgs e)
        {
            EncryptedDiskPartition new_enc_disk_partition = new EncryptedDiskPartition(textBoxDiskCaption.Text,
                uint.Parse(textBoxDiskSignature.Text), uint.Parse(textBoxDiskPartition.Text));

            if (textBoxPasswordFile.Text.Length > 0)
            {
                new_enc_disk_partition.PasswordFile = textBoxPasswordFile.Text;
            }
            else
            {
                MessageBox.Show(rm.GetString("MsgTNoPw"), rm.GetString("MsgHNoPw"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            new_enc_disk_partition.DriveLetter = comboBoxDriveLetter.Text;
            new_enc_disk_partition.IsActive = checkBoxDiskActive.Checked;
            new_enc_disk_partition.OpenExplorer = checkBoxDiskOpenExplorer.Checked;
            new_enc_disk_partition.Readonly = checkBoxRo.Checked;
            new_enc_disk_partition.Removable = checkBoxRm.Checked;
            new_enc_disk_partition.System = checkBoxSm.Checked;
            new_enc_disk_partition.Timestamp = checkBoxTs.Checked;

            if (!config.EncryptedDiskPartitions.Contains(new_enc_disk_partition))
            {
                config.EncryptedDiskPartitions.Add(new_enc_disk_partition);
            }
            else
            {
                config.EncryptedDiskPartitions.Remove(new_enc_disk_partition);
                config.EncryptedDiskPartitions.Add(new_enc_disk_partition);
            }

            MessageBox.Show(rm.GetString("MsgTSaved"), rm.GetString("MsgHSaved"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Closes the Settings Dialog and save the configuration in the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            config_db.Store(this.config);
            config_db.Close();
        }

        private void treeViewKeyDevices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (config.KeyDevices.Count > 0)
            {
                if (config.KeyDevices.Count > treeViewKeyDevices.SelectedNode.Index)
                {
                    UsbKeyDevice key_device = config.KeyDevices[treeViewKeyDevices.SelectedNode.Index];
                    textBoxUSBCaption.Text = key_device.Caption;
                    textBoxUSBSignature.Text = key_device.Signature.ToString();
                    textBoxUSBPartition.Text = key_device.PartitionIndex.ToString();
                    checkBoxKeyDeviceActive.Checked = key_device.IsActive;
                    panelKeyDevice.Visible = true;
                    return;
                }
            }

            textBoxUSBCaption.Text = null;
            textBoxUSBSignature.Text = null;
            textBoxUSBPartition.Text = null;
            panelKeyDevice.Visible = false;
        }

        private void buttonAddKeyDevice_Click(object sender, EventArgs e)
        {
            ManagementObject disk_drive = key_devices[comboBoxUSBKeyDevice.SelectedIndex][0];
            ManagementObject disk_partition = key_devices[comboBoxUSBKeyDevice.SelectedIndex][1];

            UsbKeyDevice new_key_device = new UsbKeyDevice();
            new_key_device.Caption = disk_drive["Caption"].ToString();
            new_key_device.Signature = (uint)disk_drive["Signature"];
            new_key_device.PartitionIndex = ((uint)disk_partition["Index"] + 1);
            new_key_device.IsActive = checkBoxKeyDeviceActive.Checked;

            if (!config.KeyDevices.Contains(new_key_device))
            {
                config.KeyDevices.Add(new_key_device);
            }
            else
            {
                MessageBox.Show(rm.GetString("MsgTDevDouble"), rm.GetString("MsgHDevDouble"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeNode new_key_device_node = new TreeNode(new_key_device.Caption);
            treeViewKeyDevices.Nodes.Add(new_key_device_node);
            treeViewKeyDevices.SelectedNode = new_key_device_node;
            treeViewKeyDevices.ExpandAll();
            treeViewKeyDevices.Focus();
            treeViewKeyDevices.Enabled = true;
            panelKeyDevice.Visible = true;
        }

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

        private void buttonDeleteDiskDrive_Click(object sender, EventArgs e)
        {
            if (config.EncryptedDiskPartitions.Count > treeViewDisks.SelectedNode.Index)
            {
                config.EncryptedDiskPartitions.RemoveAt(treeViewDisks.SelectedNode.Index);
            }

            treeViewDisks.SelectedNode.Remove();
            panelDisks.Visible = false;
        }

        private void checkBoxKeyDeviceActive_CheckedChanged(object sender, EventArgs e)
        {
            if (config.KeyDevices[treeViewKeyDevices.SelectedNode.Index].IsActive != checkBoxKeyDeviceActive.Checked)
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
                checkBoxSilent.Checked = true;
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
    }
}
