using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TrueMount.Forms
{
    public partial class KeyFilesDialog : Form
    {
        /// <summary>
        /// Gets or sets the list of key files.
        /// </summary>
        public List<string> KeyFiles
        {
            get
            {
                List<string> temp = new List<string>();
                foreach (string item in listBoxKeyFiles.Items)
                    temp.Add(item);
                return temp;
            }
            set
            {
                this.listBoxKeyFiles.Items.Clear();
                if (value != null)
                    this.listBoxKeyFiles.Items.AddRange(value.ToArray<string>());
            }
        }

        /// <summary>
        /// Create new KeyFilesDialog object.
        /// </summary>
        public KeyFilesDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create new KeyFilesDialog object with key files list.
        /// </summary>
        /// <param name="keyFiles">The list of key files.</param>
        public KeyFilesDialog(List<string> keyFiles)
            : this()
        {
            this.KeyFiles = keyFiles;
        }

        /// <summary>
        /// Opens file search dialog and adds selected files to the list. Doubles will be ignored.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Disables buttons it their actions are impossible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Removes selected items from list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Gets executed on form load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyFilesDialog_Load(object sender, EventArgs e)
        {
            buttonRemove.Enabled = false;
            if (listBoxKeyFiles.Items.Count == 0)
                buttonRemoveAll.Enabled = false;
        }

        /// <summary>
        /// Opens path searcher and adds selected path to list.
        /// </summary>
        private void buttonAddPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialogKeyDir.ShowDialog() == DialogResult.OK)
            {
                if (!listBoxKeyFiles.Items.Contains(folderBrowserDialogKeyDir.SelectedPath))
                    listBoxKeyFiles.Items.Add(folderBrowserDialogKeyDir.SelectedPath);
                else
                {
                    // FIXME: Implement this!
                }
            }
        }

        /// <summary>
        /// Removes all items from list.
        /// </summary>
        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            listBoxKeyFiles.Items.Clear();
            listBoxKeyFiles_SelectedIndexChanged(this, null);
        }

        /// <summary>
        /// Closes form.
        /// </summary>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Saves items in key files list.
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
