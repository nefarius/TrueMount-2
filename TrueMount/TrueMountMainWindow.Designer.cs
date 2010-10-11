namespace TrueMount
{
    partial class TrueMountMainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrueMountMainWindow));
            this.notifyIconSysTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripSysTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuShow = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mountDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unmountDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mountAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unmountAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripClipboard = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabelNefarius = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTipMainWindow = new System.Windows.Forms.ToolTip(this.components);
            this.buttonStopWorker = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.buttonStartWorker = new System.Windows.Forms.Button();
            this.buttonHide = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.contextMenuStripSysTray.SuspendLayout();
            this.contextMenuStripClipboard.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIconSysTray
            // 
            this.notifyIconSysTray.ContextMenuStrip = this.contextMenuStripSysTray;
            resources.ApplyResources(this.notifyIconSysTray, "notifyIconSysTray");
            this.notifyIconSysTray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconSysTray_MouseDoubleClick);
            // 
            // contextMenuStripSysTray
            // 
            this.contextMenuStripSysTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuShow,
            this.settingsToolStripMenuSettings,
            this.mountDeviceToolStripMenuItem,
            this.unmountDeviceToolStripMenuItem,
            this.mountAllToolStripMenuItem,
            this.unmountAllToolStripMenuItem,
            this.checkForUpdatesToolStripMenuItem,
            this.closeToolStripMenuClose});
            this.contextMenuStripSysTray.Name = "contextMenuStripSysTray";
            resources.ApplyResources(this.contextMenuStripSysTray, "contextMenuStripSysTray");
            // 
            // showToolStripMenuShow
            // 
            this.showToolStripMenuShow.Image = global::TrueMount.Properties.Resources._1277233011_window_list;
            this.showToolStripMenuShow.Name = "showToolStripMenuShow";
            resources.ApplyResources(this.showToolStripMenuShow, "showToolStripMenuShow");
            this.showToolStripMenuShow.Click += new System.EventHandler(this.showToolStripMenuShow_Click);
            // 
            // settingsToolStripMenuSettings
            // 
            this.settingsToolStripMenuSettings.Image = global::TrueMount.Properties.Resources._1276790510_gear;
            this.settingsToolStripMenuSettings.Name = "settingsToolStripMenuSettings";
            resources.ApplyResources(this.settingsToolStripMenuSettings, "settingsToolStripMenuSettings");
            this.settingsToolStripMenuSettings.Click += new System.EventHandler(this.settingsToolStripMenuSettings_Click);
            // 
            // mountDeviceToolStripMenuItem
            // 
            resources.ApplyResources(this.mountDeviceToolStripMenuItem, "mountDeviceToolStripMenuItem");
            this.mountDeviceToolStripMenuItem.Image = global::TrueMount.Properties.Resources._1285440550_Radioactive;
            this.mountDeviceToolStripMenuItem.Name = "mountDeviceToolStripMenuItem";
            // 
            // unmountDeviceToolStripMenuItem
            // 
            resources.ApplyResources(this.unmountDeviceToolStripMenuItem, "unmountDeviceToolStripMenuItem");
            this.unmountDeviceToolStripMenuItem.Image = global::TrueMount.Properties.Resources._1286800325_unmount_overlay;
            this.unmountDeviceToolStripMenuItem.Name = "unmountDeviceToolStripMenuItem";
            // 
            // mountAllToolStripMenuItem
            // 
            this.mountAllToolStripMenuItem.Image = global::TrueMount.Properties.Resources._1279794104_hdd_mount;
            this.mountAllToolStripMenuItem.Name = "mountAllToolStripMenuItem";
            resources.ApplyResources(this.mountAllToolStripMenuItem, "mountAllToolStripMenuItem");
            this.mountAllToolStripMenuItem.Click += new System.EventHandler(this.mountAllToolStripMenuItem_Click);
            // 
            // unmountAllToolStripMenuItem
            // 
            this.unmountAllToolStripMenuItem.Image = global::TrueMount.Properties.Resources._1277232971_hdd_unmount;
            this.unmountAllToolStripMenuItem.Name = "unmountAllToolStripMenuItem";
            resources.ApplyResources(this.unmountAllToolStripMenuItem, "unmountAllToolStripMenuItem");
            this.unmountAllToolStripMenuItem.Click += new System.EventHandler(this.unmountAllToolStripMenuItem_Click);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Image = global::TrueMount.Properties.Resources._1284820964_system_software_update;
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            resources.ApplyResources(this.checkForUpdatesToolStripMenuItem, "checkForUpdatesToolStripMenuItem");
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // closeToolStripMenuClose
            // 
            this.closeToolStripMenuClose.Image = global::TrueMount.Properties.Resources._1276771116_dialog_close;
            this.closeToolStripMenuClose.Name = "closeToolStripMenuClose";
            resources.ApplyResources(this.closeToolStripMenuClose, "closeToolStripMenuClose");
            this.closeToolStripMenuClose.Click += new System.EventHandler(this.closeToolStripMenuClose_Click);
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.BackColor = System.Drawing.Color.Black;
            this.richTextBoxLog.ContextMenuStrip = this.contextMenuStripClipboard;
            resources.ApplyResources(this.richTextBoxLog, "richTextBoxLog");
            this.richTextBoxLog.ForeColor = System.Drawing.Color.White;
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            // 
            // contextMenuStripClipboard
            // 
            this.contextMenuStripClipboard.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuStripClipboard.Name = "contextMenuStripClipboard";
            resources.ApplyResources(this.contextMenuStripClipboard, "contextMenuStripClipboard");
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::TrueMount.Properties.Resources._1277233069_copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBoxLog);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // linkLabelNefarius
            // 
            resources.ApplyResources(this.linkLabelNefarius, "linkLabelNefarius");
            this.linkLabelNefarius.Name = "linkLabelNefarius";
            this.linkLabelNefarius.TabStop = true;
            this.toolTipMainWindow.SetToolTip(this.linkLabelNefarius, resources.GetString("linkLabelNefarius.ToolTip"));
            this.linkLabelNefarius.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelNefarius_LinkClicked);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTipMainWindow.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.DarkRed;
            this.label3.Name = "label3";
            this.toolTipMainWindow.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // buttonStopWorker
            // 
            this.buttonStopWorker.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.buttonStopWorker, "buttonStopWorker");
            this.buttonStopWorker.Image = global::TrueMount.Properties.Resources._1276770923_power_off;
            this.buttonStopWorker.Name = "buttonStopWorker";
            this.toolTipMainWindow.SetToolTip(this.buttonStopWorker, resources.GetString("buttonStopWorker.ToolTip"));
            this.buttonStopWorker.UseVisualStyleBackColor = false;
            this.buttonStopWorker.Click += new System.EventHandler(this.buttonStopWorker_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.buttonSettings, "buttonSettings");
            this.buttonSettings.Image = global::TrueMount.Properties.Resources._1276790510_gear;
            this.buttonSettings.Name = "buttonSettings";
            this.toolTipMainWindow.SetToolTip(this.buttonSettings, resources.GetString("buttonSettings.ToolTip"));
            this.buttonSettings.UseVisualStyleBackColor = false;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // buttonStartWorker
            // 
            this.buttonStartWorker.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.buttonStartWorker, "buttonStartWorker");
            this.buttonStartWorker.Image = global::TrueMount.Properties.Resources._1276770921_power_on;
            this.buttonStartWorker.Name = "buttonStartWorker";
            this.toolTipMainWindow.SetToolTip(this.buttonStartWorker, resources.GetString("buttonStartWorker.ToolTip"));
            this.buttonStartWorker.UseVisualStyleBackColor = false;
            this.buttonStartWorker.Click += new System.EventHandler(this.buttonStartWorker_Click);
            // 
            // buttonHide
            // 
            this.buttonHide.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.buttonHide, "buttonHide");
            this.buttonHide.Image = global::TrueMount.Properties.Resources._1276771048_Cloud_Download_On;
            this.buttonHide.Name = "buttonHide";
            this.toolTipMainWindow.SetToolTip(this.buttonHide, resources.GetString("buttonHide.ToolTip"));
            this.buttonHide.UseVisualStyleBackColor = false;
            this.buttonHide.Click += new System.EventHandler(this.buttonHide_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Image = global::TrueMount.Properties.Resources._1276771116_dialog_close;
            this.buttonClose.Name = "buttonClose";
            this.toolTipMainWindow.SetToolTip(this.buttonClose, resources.GetString("buttonClose.ToolTip"));
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonStopWorker);
            this.groupBox3.Controls.Add(this.buttonSettings);
            this.groupBox3.Controls.Add(this.buttonStartWorker);
            this.groupBox3.Controls.Add(this.buttonHide);
            this.groupBox3.Controls.Add(this.buttonClose);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // TrueMountMainWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabelNefarius);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "TrueMountMainWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrueMountMainWindow_FormClosing);
            this.Load += new System.EventHandler(this.TrueMountMainWindow_Load);
            this.contextMenuStripSysTray.ResumeLayout(false);
            this.contextMenuStripClipboard.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIconSysTray;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSysTray;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuShow;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonStartWorker;
        private System.Windows.Forms.Button buttonStopWorker;
        private System.Windows.Forms.LinkLabel linkLabelNefarius;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripClipboard;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem unmountAllToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTipMainWindow;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonHide;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ToolStripMenuItem mountAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuSettings;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mountDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unmountDeviceToolStripMenuItem;
    }
}

