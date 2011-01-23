using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace TrueMount.Forms
{
    public partial class AVWarningDialog : Form, IMessageFilter
    {
        public AVWarningDialog()
        {
            Application.AddMessageFilter(this);
            InitializeComponent();
        }

        private void linkLabelVirusChief_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.viruschief.com/report.html?report_id=b8707fa94dc79ed185a0686589a05465db774b2b");
        }

        private void AVWarningDialog_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x201)
            {
                this.Close();
                return true;
            }

            return false;
        }

        private void AVWarningDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.RemoveMessageFilter(this);
        }
    }
}
