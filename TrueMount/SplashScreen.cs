using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace TrueMount
{
    public partial class SplashScreen : Form
    {
        public delegate void OnSplashFinishedEventHandler();
        public event OnSplashFinishedEventHandler OnSplashFinished;

        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SpalshScreen_Load(object sender, EventArgs e)
        {
            backgroundWorkerWait.RunWorkerAsync();
        }

        private void backgroundWorkerWait_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(7000);
        }

        private void backgroundWorkerWait_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.OnSplashFinished.Invoke();
            this.Close();
        }
    }
}
