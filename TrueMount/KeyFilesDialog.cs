using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrueMount
{
    public partial class KeyFilesDialog : Form
    {
        public List<string> KeyFilesList { get; set; }

        public KeyFilesDialog()
        {
            this.KeyFilesList = new List<string>();

            InitializeComponent();
        }

        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogKeyFile.ShowDialog() == DialogResult.OK)
            {
                foreach (string item in openFileDialogKeyFile.FileNames)
                    if (!listBoxKeyFiles.Items.Contains(item))
                        listBoxKeyFiles.Items.Add(item);
                listBoxKeyFiles_SelectedIndexChanged(this, null);
            }
        }

        private void listBoxKeyFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxKeyFiles.Items.Count > 0)
            {
                buttonRemove.Enabled = true;
                buttonRemoveAll.Enabled = true;
            }
            else
            {
                buttonRemove.Enabled = false;
                buttonRemoveAll.Enabled = false;
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection Obj = new ListBox.SelectedObjectCollection(listBoxKeyFiles);
            for (int i = Obj.Count - 1; i >= 0; i--)
            {
                listBoxKeyFiles.Items.Remove(Obj[i]);
            }

            if (listBoxKeyFiles.SelectedItem == null)
                buttonRemove.Enabled = false;
        }

        private void KeyFilesDialog_Load(object sender, EventArgs e)
        {
            buttonRemove.Enabled = false;
            buttonRemoveAll.Enabled = false;

            listBoxKeyFiles.Items.Clear(); // important!
            foreach (string item in this.KeyFilesList)
                listBoxKeyFiles.Items.Add(item);
        }

        private void buttonAddPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialogKeyDir.ShowDialog() == DialogResult.OK)
            {
                if (!listBoxKeyFiles.Items.Contains(folderBrowserDialogKeyDir.SelectedPath))
                    listBoxKeyFiles.Items.Add(folderBrowserDialogKeyDir.SelectedPath);
                else
                    throw new NotImplementedException();
            }
        }

        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            listBoxKeyFiles.Items.Clear();
            listBoxKeyFiles_SelectedIndexChanged(this, null);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.KeyFilesList.Clear();
            foreach (string item in listBoxKeyFiles.Items)
                this.KeyFilesList.Add(item);
            this.Close();
        }
    }
}
