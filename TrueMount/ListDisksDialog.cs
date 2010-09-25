using System;
using System.Management;
using System.Windows.Forms;

namespace TrueMount
{
    public partial class ListDisksDialog : Form
    {
        public ListDisksDialog()
        {
            InitializeComponent();
        }

        private void ListDisksDialog_Load(object sender, EventArgs e)
        {
            richTextBoxOutput.AppendText("--- DiskDrives ---" + Environment.NewLine);
            foreach (ManagementObject disk in SystemDevices.DiskDrives)
            {
                richTextBoxOutput.AppendText("Index: " + disk["Index"] + ", Caption: " + disk["Caption"].ToString() +
                    ", Signature: " + disk["Signature"].ToString() + Environment.NewLine);
            }

            richTextBoxOutput.AppendText("--- DiskPartitions ---" + Environment.NewLine);
            foreach (ManagementObject partition in SystemDevices.DiskPartitions)
            {
                richTextBoxOutput.AppendText("DiskDrive Index: " + partition["DiskIndex"] + ", DiskPartition: " + partition["Index"] + Environment.NewLine);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBoxOutput.SelectedText);
        }
    }
}
