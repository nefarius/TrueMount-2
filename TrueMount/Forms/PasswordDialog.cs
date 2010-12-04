using System;
using System.Windows.Forms;

namespace TrueMount.Forms
{
    public partial class PasswordDialog : Form
    {
        public PasswordDialog(String encMediaName)
        {
            InitializeComponent();

            labelVolumeName.Text = encMediaName;
        }

        public string VolumeLabel
        {
            get { return labelVolumeName.Text; }
            set { labelVolumeName.Text = value; }
        }

        public string Password
        {
            get { return textBoxPassword.Text; }
            set { textBoxPassword.Text = value; }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            textBoxPassword.Clear();
        }

        private void PasswordDialog_Activated(object sender, EventArgs e)
        {
            textBoxPassword.SelectAll();
        }
    }
}
