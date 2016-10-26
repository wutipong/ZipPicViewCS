namespace ZipPicViewCS
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.thmbnailPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.folderBox = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.thumbnailProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.thumbnailBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.openButton = new System.Windows.Forms.ToolStripSplitButton();
            this.archiveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.zoomFitButton = new System.Windows.Forms.ToolStripButton();
            this.zoomCombo = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.autoButton = new System.Windows.Forms.ToolStripButton();
            this.advanceDurationCombo = new System.Windows.Forms.ToolStripComboBox();
            this.advanceSoundButton = new System.Windows.Forms.ToolStripButton();
            this.pathLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.viewerPictureBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewerPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Archive | *.zip; *.rar| Zip Files | *.zip | Rar Files | *.rar  | All files | *.*";
            // 
            // thmbnailPanel
            // 
            this.thmbnailPanel.AutoScroll = true;
            this.thmbnailPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thmbnailPanel.Location = new System.Drawing.Point(0, 0);
            this.thmbnailPanel.Name = "thmbnailPanel";
            this.thmbnailPanel.Size = new System.Drawing.Size(448, 466);
            this.thmbnailPanel.TabIndex = 3;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton,
            this.pathLabel,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(695, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(695, 500);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(687, 474);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Browser";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.folderBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.thmbnailPanel);
            this.splitContainer1.Size = new System.Drawing.Size(681, 468);
            this.splitContainer1.SplitterDistance = 227;
            this.splitContainer1.TabIndex = 4;
            // 
            // folderBox
            // 
            this.folderBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folderBox.FormattingEnabled = true;
            this.folderBox.Location = new System.Drawing.Point(0, 0);
            this.folderBox.Name = "folderBox";
            this.folderBox.Size = new System.Drawing.Size(225, 466);
            this.folderBox.TabIndex = 0;
            this.folderBox.SelectedIndexChanged += new System.EventHandler(this.folderBox_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.toolStrip2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(687, 474);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Viewer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.thumbnailProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(695, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // thumbnailProgressBar
            // 
            this.thumbnailProgressBar.Name = "thumbnailProgressBar";
            this.thumbnailProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tabControl1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(695, 500);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(695, 547);
            this.toolStripContainer1.TabIndex = 8;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // thumbnailBackgroundWorker
            // 
            this.thumbnailBackgroundWorker.WorkerReportsProgress = true;
            this.thumbnailBackgroundWorker.WorkerSupportsCancellation = true;
            this.thumbnailBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.thumbnailBackgroundWorker_DoWork);
            this.thumbnailBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.thumbnailBackgroundWorker_ProgressChanged);
            this.thumbnailBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.thumbnailBackgroundWorker_RunWorkerCompleted);
            // 
            // openButton
            // 
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archiveMenuItem,
            this.folderToolStripMenuItem});
            this.openButton.Image = ((System.Drawing.Image)(resources.GetObject("openButton.Image")));
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(32, 22);
            this.openButton.Text = "toolStripSplitButton1";
            this.openButton.ToolTipText = "Open";
            this.openButton.ButtonClick += new System.EventHandler(this.toolStripSplitButton1_ButtonClick);
            // 
            // archiveMenuItem
            // 
            this.archiveMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("archiveMenuItem.Image")));
            this.archiveMenuItem.Name = "archiveMenuItem";
            this.archiveMenuItem.Size = new System.Drawing.Size(152, 22);
            this.archiveMenuItem.Text = "Archive";
            this.archiveMenuItem.ToolTipText = "Open Archive";
            this.archiveMenuItem.Click += new System.EventHandler(this.archiveMenuItem_Click);
            // 
            // folderToolStripMenuItem
            // 
            this.folderToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("folderToolStripMenuItem.Image")));
            this.folderToolStripMenuItem.Name = "folderToolStripMenuItem";
            this.folderToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.folderToolStripMenuItem.Text = "Folder";
            this.folderToolStripMenuItem.ToolTipText = "Open Folder";
            this.folderToolStripMenuItem.Click += new System.EventHandler(this.folderToolStripMenuItem_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripButton1,
            this.toolStripSeparator3,
            this.zoomFitButton,
            this.zoomCombo,
            this.toolStripSeparator2,
            this.autoButton,
            this.advanceDurationCombo,
            this.advanceSoundButton});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(681, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // zoomFitButton
            // 
            this.zoomFitButton.Checked = true;
            this.zoomFitButton.CheckOnClick = true;
            this.zoomFitButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.zoomFitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomFitButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomFitButton.Image")));
            this.zoomFitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomFitButton.Name = "zoomFitButton";
            this.zoomFitButton.Size = new System.Drawing.Size(23, 22);
            this.zoomFitButton.Text = "Fit";
            this.zoomFitButton.Click += new System.EventHandler(this.zoomFitButton_Click);
            // 
            // zoomCombo
            // 
            this.zoomCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.zoomCombo.Items.AddRange(new object[] {
            "200%",
            "190%",
            "180%",
            "170%",
            "160%",
            "150%",
            "140%",
            "130%",
            "120%",
            "110%",
            "100%",
            "90%",
            "80%",
            "70%",
            "60%",
            "50%",
            "40%",
            "30%",
            "20%",
            "10%",
            "5%"});
            this.zoomCombo.Name = "zoomCombo";
            this.zoomCombo.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // autoButton
            // 
            this.autoButton.CheckOnClick = true;
            this.autoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.autoButton.Image = ((System.Drawing.Image)(resources.GetObject("autoButton.Image")));
            this.autoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.autoButton.Name = "autoButton";
            this.autoButton.Size = new System.Drawing.Size(23, 22);
            this.autoButton.Text = "Auto Advance";
            // 
            // advanceDurationCombo
            // 
            this.advanceDurationCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.advanceDurationCombo.Items.AddRange(new object[] {
            "30 Minutes",
            "15 Minutes",
            "10 Minutes",
            "5 Minutes",
            "1 Minutes",
            "30 Seconds",
            "15 Seconds",
            "10 Seconds",
            "5 Seconds"});
            this.advanceDurationCombo.Name = "advanceDurationCombo";
            this.advanceDurationCombo.Size = new System.Drawing.Size(121, 25);
            this.advanceDurationCombo.ToolTipText = "Duration between each image.";
            // 
            // advanceSoundButton
            // 
            this.advanceSoundButton.CheckOnClick = true;
            this.advanceSoundButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.advanceSoundButton.Image = ((System.Drawing.Image)(resources.GetObject("advanceSoundButton.Image")));
            this.advanceSoundButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.advanceSoundButton.Name = "advanceSoundButton";
            this.advanceSoundButton.Size = new System.Drawing.Size(23, 22);
            this.advanceSoundButton.Text = "toolStripButton5";
            this.advanceSoundButton.ToolTipText = "Play alert sound betwen images";
            // 
            // pathLabel
            // 
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(52, 22);
            this.pathLabel.Text = "<None>";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(44, 22);
            this.toolStripButton3.Text = "About";
            // 
            // viewerPictureBox
            // 
            this.viewerPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewerPictureBox.Location = new System.Drawing.Point(0, 0);
            this.viewerPictureBox.Name = "viewerPictureBox";
            this.viewerPictureBox.Size = new System.Drawing.Size(681, 443);
            this.viewerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.viewerPictureBox.TabIndex = 1;
            this.viewerPictureBox.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.viewerPictureBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(681, 443);
            this.panel1.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 547);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "MainForm";
            this.Text = "ZipPicView";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewerPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FlowLayoutPanel thmbnailPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSplitButton openButton;
        private System.Windows.Forms.ToolStripMenuItem archiveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar thumbnailProgressBar;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ListBox folderBox;
        private System.ComponentModel.BackgroundWorker thumbnailBackgroundWorker;
        private System.Windows.Forms.ToolStripLabel pathLabel;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton zoomFitButton;
        private System.Windows.Forms.ToolStripComboBox zoomCombo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton autoButton;
        private System.Windows.Forms.ToolStripComboBox advanceDurationCombo;
        private System.Windows.Forms.ToolStripButton advanceSoundButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox viewerPictureBox;
    }
}

