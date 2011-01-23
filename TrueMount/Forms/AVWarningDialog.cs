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
    public partial class AVWarningDialog : Form
    {
        public AVWarningDialog()
        {
            InitializeComponent();
        }

        private void linkLabelVirusChief_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.viruschief.com/report.html?report_id=b8707fa94dc79ed185a0686589a05465db774b2b");
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.HWnd == pictureBoxSkull.Handle ||
                m.HWnd == labelHeading.Handle ||
                m.HWnd == labelMessage.Handle ||
                m.HWnd == labelAdvice.Handle)
            {
                if (m.Msg == 0x201)
                {
                    this.Close();
                    return true;
                }
            }

            return false;
        }

        private void linkLabelVirusTotal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.virustotal.com/file-scan/report.html?id=bf25c559d7647ce7af5fa8d5ddc6199e657521123370e6956d17ed636a1e2592-1295731804");
        }
    }
}
