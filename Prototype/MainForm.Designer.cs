namespace EPackageBuilder
{
    partial class MainWindow
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
            if (disposing && (components != null)) {
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
            this.folderBrowserProject = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserSources = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbProjectDescription = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonBrowseZipFolder = new System.Windows.Forms.Button();
            this.tbPathZip = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonZip = new System.Windows.Forms.Button();
            this.buttonBrowseBuildsFolder = new System.Windows.Forms.Button();
            this.tbPathBuilds = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonDoAll = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonBuildPc = new System.Windows.Forms.Button();
            this.buttonBuildRelease = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonMakeSources = new System.Windows.Forms.Button();
            this.cbReqOpenExplorer = new System.Windows.Forms.CheckBox();
            this.buttonGameCheck = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonIncrVersion = new System.Windows.Forms.Button();
            this.buttonShowVersion = new System.Windows.Forms.Button();
            this.buttonRenewVersion = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPathProject = new System.Windows.Forms.TextBox();
            this.buttonBrowseProjectFolder = new System.Windows.Forms.Button();
            this.tbPathSources = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBrowseSourcesFolder = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonSeparator = new System.Windows.Forms.Button();
            this.textLog = new System.Windows.Forms.TextBox();
            this.buttonLogClear = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectGameCheckToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unityPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogContext = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogContext = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserBuilds = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserZip = new System.Windows.Forms.FolderBrowserDialog();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // folderBrowserProject
            // 
            this.folderBrowserProject.Description = "Choose project folder:";
            this.folderBrowserProject.ShowNewFolderButton = false;
            // 
            // folderBrowserSources
            // 
            this.folderBrowserSources.Description = "Choose output sources folder:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbProjectDescription);
            this.groupBox1.Location = new System.Drawing.Point(10, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(668, 74);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project data";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(437, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(215, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Warning: make sure all values are approved";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Description";
            // 
            // tbProjectDescription
            // 
            this.tbProjectDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProjectDescription.Location = new System.Drawing.Point(5, 32);
            this.tbProjectDescription.Name = "tbProjectDescription";
            this.tbProjectDescription.Size = new System.Drawing.Size(647, 20);
            this.tbProjectDescription.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.buttonBrowseZipFolder);
            this.groupBox2.Controls.Add(this.tbPathZip);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.buttonZip);
            this.groupBox2.Controls.Add(this.buttonBrowseBuildsFolder);
            this.groupBox2.Controls.Add(this.tbPathBuilds);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.buttonDoAll);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.buttonCancel);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tbPathProject);
            this.groupBox2.Controls.Add(this.buttonBrowseProjectFolder);
            this.groupBox2.Controls.Add(this.tbPathSources);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.buttonBrowseSourcesFolder);
            this.groupBox2.Location = new System.Drawing.Point(11, 107);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(667, 297);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Paths";
            // 
            // buttonBrowseZipFolder
            // 
            this.buttonBrowseZipFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseZipFolder.Location = new System.Drawing.Point(583, 146);
            this.buttonBrowseZipFolder.Name = "buttonBrowseZipFolder";
            this.buttonBrowseZipFolder.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseZipFolder.TabIndex = 35;
            this.buttonBrowseZipFolder.Text = "Browse";
            this.buttonBrowseZipFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseZipFolder.Click += new System.EventHandler(this.buttonBrowseZipFolder_Click);
            // 
            // tbPathZip
            // 
            this.tbPathZip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPathZip.Location = new System.Drawing.Point(4, 148);
            this.tbPathZip.Name = "tbPathZip";
            this.tbPathZip.Size = new System.Drawing.Size(572, 20);
            this.tbPathZip.TabIndex = 33;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 34;
            this.label7.Text = "Output zip";
            // 
            // buttonZip
            // 
            this.buttonZip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonZip.Location = new System.Drawing.Point(555, 233);
            this.buttonZip.Name = "buttonZip";
            this.buttonZip.Size = new System.Drawing.Size(103, 23);
            this.buttonZip.TabIndex = 32;
            this.buttonZip.Text = "Zip All";
            this.buttonZip.UseVisualStyleBackColor = true;
            this.buttonZip.Click += new System.EventHandler(this.buttonZip_Click);
            // 
            // buttonBrowseBuildsFolder
            // 
            this.buttonBrowseBuildsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseBuildsFolder.Location = new System.Drawing.Point(583, 108);
            this.buttonBrowseBuildsFolder.Name = "buttonBrowseBuildsFolder";
            this.buttonBrowseBuildsFolder.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseBuildsFolder.TabIndex = 31;
            this.buttonBrowseBuildsFolder.Text = "Browse";
            this.buttonBrowseBuildsFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseBuildsFolder.Click += new System.EventHandler(this.buttonBrowseBuildsFolder_Click);
            // 
            // tbPathBuilds
            // 
            this.tbPathBuilds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPathBuilds.Location = new System.Drawing.Point(4, 110);
            this.tbPathBuilds.Name = "tbPathBuilds";
            this.tbPathBuilds.Size = new System.Drawing.Size(572, 20);
            this.tbPathBuilds.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Output builds";
            // 
            // buttonDoAll
            // 
            this.buttonDoAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDoAll.Location = new System.Drawing.Point(555, 204);
            this.buttonDoAll.Name = "buttonDoAll";
            this.buttonDoAll.Size = new System.Drawing.Size(103, 23);
            this.buttonDoAll.TabIndex = 24;
            this.buttonDoAll.Text = "Make All";
            this.buttonDoAll.UseVisualStyleBackColor = true;
            this.buttonDoAll.Click += new System.EventHandler(this.buttonDoAll_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.buttonBuildPc);
            this.groupBox6.Controls.Add(this.buttonBuildRelease);
            this.groupBox6.Location = new System.Drawing.Point(316, 185);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(121, 106);
            this.groupBox6.TabIndex = 28;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Build";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(6, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 29);
            this.label6.TabIndex = 18;
            this.label6.Text = "Warning: Unity must be closed";
            // 
            // buttonBuildPc
            // 
            this.buttonBuildPc.Enabled = false;
            this.buttonBuildPc.Location = new System.Drawing.Point(6, 19);
            this.buttonBuildPc.Name = "buttonBuildPc";
            this.buttonBuildPc.Size = new System.Drawing.Size(103, 23);
            this.buttonBuildPc.TabIndex = 22;
            this.buttonBuildPc.Text = "PC";
            this.buttonBuildPc.UseVisualStyleBackColor = true;
            this.buttonBuildPc.Click += new System.EventHandler(this.buttonBuildPc_Click);
            // 
            // buttonBuildRelease
            // 
            this.buttonBuildRelease.Enabled = false;
            this.buttonBuildRelease.Location = new System.Drawing.Point(6, 48);
            this.buttonBuildRelease.Name = "buttonBuildRelease";
            this.buttonBuildRelease.Size = new System.Drawing.Size(103, 23);
            this.buttonBuildRelease.TabIndex = 23;
            this.buttonBuildRelease.Text = "Release";
            this.buttonBuildRelease.UseVisualStyleBackColor = true;
            this.buttonBuildRelease.Click += new System.EventHandler(this.buttonBuildRelease_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonMakeSources);
            this.groupBox5.Controls.Add(this.cbReqOpenExplorer);
            this.groupBox5.Controls.Add(this.buttonGameCheck);
            this.groupBox5.Location = new System.Drawing.Point(6, 185);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(155, 106);
            this.groupBox5.TabIndex = 27;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Sources";
            // 
            // buttonMakeSources
            // 
            this.buttonMakeSources.Location = new System.Drawing.Point(6, 19);
            this.buttonMakeSources.Name = "buttonMakeSources";
            this.buttonMakeSources.Size = new System.Drawing.Size(137, 23);
            this.buttonMakeSources.TabIndex = 17;
            this.buttonMakeSources.Text = "Make Package";
            this.buttonMakeSources.UseVisualStyleBackColor = true;
            this.buttonMakeSources.Click += new System.EventHandler(this.buttonMakeSources_Click);
            // 
            // cbReqOpenExplorer
            // 
            this.cbReqOpenExplorer.AutoSize = true;
            this.cbReqOpenExplorer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbReqOpenExplorer.Checked = true;
            this.cbReqOpenExplorer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbReqOpenExplorer.Location = new System.Drawing.Point(6, 48);
            this.cbReqOpenExplorer.Name = "cbReqOpenExplorer";
            this.cbReqOpenExplorer.Size = new System.Drawing.Size(105, 17);
            this.cbReqOpenExplorer.TabIndex = 18;
            this.cbReqOpenExplorer.Text = "Show in Explorer";
            this.cbReqOpenExplorer.UseVisualStyleBackColor = true;
            // 
            // buttonGameCheck
            // 
            this.buttonGameCheck.Location = new System.Drawing.Point(6, 71);
            this.buttonGameCheck.Name = "buttonGameCheck";
            this.buttonGameCheck.Size = new System.Drawing.Size(81, 23);
            this.buttonGameCheck.TabIndex = 21;
            this.buttonGameCheck.Text = "Game Check";
            this.buttonGameCheck.UseVisualStyleBackColor = true;
            this.buttonGameCheck.Click += new System.EventHandler(this.buttonGameCheck_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonIncrVersion);
            this.groupBox4.Controls.Add(this.buttonShowVersion);
            this.groupBox4.Controls.Add(this.buttonRenewVersion);
            this.groupBox4.Location = new System.Drawing.Point(167, 185);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(143, 106);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Game Version";
            // 
            // buttonIncrVersion
            // 
            this.buttonIncrVersion.Location = new System.Drawing.Point(6, 19);
            this.buttonIncrVersion.Name = "buttonIncrVersion";
            this.buttonIncrVersion.Size = new System.Drawing.Size(126, 23);
            this.buttonIncrVersion.TabIndex = 20;
            this.buttonIncrVersion.Text = "Increase";
            this.buttonIncrVersion.UseVisualStyleBackColor = true;
            this.buttonIncrVersion.Click += new System.EventHandler(this.buttonIncrVersion_Click);
            // 
            // buttonShowVersion
            // 
            this.buttonShowVersion.Location = new System.Drawing.Point(6, 77);
            this.buttonShowVersion.Name = "buttonShowVersion";
            this.buttonShowVersion.Size = new System.Drawing.Size(126, 23);
            this.buttonShowVersion.TabIndex = 25;
            this.buttonShowVersion.Text = "Show";
            this.buttonShowVersion.UseVisualStyleBackColor = true;
            this.buttonShowVersion.Click += new System.EventHandler(this.buttonShowVersion_Click);
            // 
            // buttonRenewVersion
            // 
            this.buttonRenewVersion.Location = new System.Drawing.Point(6, 48);
            this.buttonRenewVersion.Name = "buttonRenewVersion";
            this.buttonRenewVersion.Size = new System.Drawing.Size(126, 23);
            this.buttonRenewVersion.TabIndex = 24;
            this.buttonRenewVersion.Text = "Rebuild";
            this.buttonRenewVersion.UseVisualStyleBackColor = true;
            this.buttonRenewVersion.Click += new System.EventHandler(this.buttonRenewVersion_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(583, 262);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 19;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Project";
            // 
            // tbPathProject
            // 
            this.tbPathProject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPathProject.Location = new System.Drawing.Point(5, 32);
            this.tbPathProject.Name = "tbPathProject";
            this.tbPathProject.Size = new System.Drawing.Size(572, 20);
            this.tbPathProject.TabIndex = 11;
            // 
            // buttonBrowseProjectFolder
            // 
            this.buttonBrowseProjectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseProjectFolder.Location = new System.Drawing.Point(583, 30);
            this.buttonBrowseProjectFolder.Name = "buttonBrowseProjectFolder";
            this.buttonBrowseProjectFolder.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseProjectFolder.TabIndex = 13;
            this.buttonBrowseProjectFolder.Text = "Browse";
            this.buttonBrowseProjectFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseProjectFolder.Click += new System.EventHandler(this.buttonBrowseProjectFolder_Cick);
            // 
            // tbPathSources
            // 
            this.tbPathSources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPathSources.Location = new System.Drawing.Point(5, 71);
            this.tbPathSources.Name = "tbPathSources";
            this.tbPathSources.Size = new System.Drawing.Size(572, 20);
            this.tbPathSources.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Output sources";
            // 
            // buttonBrowseSourcesFolder
            // 
            this.buttonBrowseSourcesFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseSourcesFolder.Location = new System.Drawing.Point(583, 69);
            this.buttonBrowseSourcesFolder.Name = "buttonBrowseSourcesFolder";
            this.buttonBrowseSourcesFolder.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseSourcesFolder.TabIndex = 16;
            this.buttonBrowseSourcesFolder.Text = "Browse";
            this.buttonBrowseSourcesFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseSourcesFolder.Click += new System.EventHandler(this.buttonBrowseSourcesFolder_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.buttonSeparator);
            this.groupBox3.Controls.Add(this.textLog);
            this.groupBox3.Controls.Add(this.buttonLogClear);
            this.groupBox3.Location = new System.Drawing.Point(12, 410);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(666, 239);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Log";
            // 
            // buttonSeparator
            // 
            this.buttonSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeparator.Location = new System.Drawing.Point(453, 210);
            this.buttonSeparator.Name = "buttonSeparator";
            this.buttonSeparator.Size = new System.Drawing.Size(116, 23);
            this.buttonSeparator.TabIndex = 13;
            this.buttonSeparator.Text = "Write Separator";
            this.buttonSeparator.UseVisualStyleBackColor = true;
            this.buttonSeparator.Click += new System.EventHandler(this.buttonSeparator_Click);
            // 
            // textLog
            // 
            this.textLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textLog.Location = new System.Drawing.Point(7, 19);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textLog.Size = new System.Drawing.Size(643, 185);
            this.textLog.TabIndex = 10;
            // 
            // buttonLogClear
            // 
            this.buttonLogClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLogClear.Location = new System.Drawing.Point(575, 210);
            this.buttonLogClear.Name = "buttonLogClear";
            this.buttonLogClear.Size = new System.Drawing.Size(75, 23);
            this.buttonLogClear.TabIndex = 12;
            this.buttonLogClear.Text = "Clear";
            this.buttonLogClear.UseVisualStyleBackColor = true;
            this.buttonLogClear.Click += new System.EventHandler(this.buttonLogClear_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(690, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadContextToolStripMenuItem,
            this.saveContextToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.fileToolStripMenuItem.Text = "Context";
            // 
            // loadContextToolStripMenuItem
            // 
            this.loadContextToolStripMenuItem.Name = "loadContextToolStripMenuItem";
            this.loadContextToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.loadContextToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.loadContextToolStripMenuItem.Text = "Load";
            this.loadContextToolStripMenuItem.Click += new System.EventHandler(this.loadContextToolStripMenuItem_Click);
            // 
            // saveContextToolStripMenuItem
            // 
            this.saveContextToolStripMenuItem.Name = "saveContextToolStripMenuItem";
            this.saveContextToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveContextToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.saveContextToolStripMenuItem.Text = "Save";
            this.saveContextToolStripMenuItem.Click += new System.EventHandler(this.saveContextToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectGameCheckToolToolStripMenuItem,
            this.unityPathToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // selectGameCheckToolToolStripMenuItem
            // 
            this.selectGameCheckToolToolStripMenuItem.Name = "selectGameCheckToolToolStripMenuItem";
            this.selectGameCheckToolToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.selectGameCheckToolToolStripMenuItem.Text = "GameCheck path";
            this.selectGameCheckToolToolStripMenuItem.Click += new System.EventHandler(this.gameCheckToolStripMenuItem_Click);
            // 
            // unityPathToolStripMenuItem
            // 
            this.unityPathToolStripMenuItem.Name = "unityPathToolStripMenuItem";
            this.unityPathToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.unityPathToolStripMenuItem.Text = "Unity path";
            this.unityPathToolStripMenuItem.Click += new System.EventHandler(this.unityPathToolStripMenuItem_Click);
            // 
            // openFileDialogContext
            // 
            this.openFileDialogContext.FileName = "fields.epbcontext";
            // 
            // saveFileDialogContext
            // 
            this.saveFileDialogContext.FileName = "fields.epbcontext";
            // 
            // folderBrowserBuilds
            // 
            this.folderBrowserBuilds.Description = "Choose output build folder:";
            // 
            // folderBrowserZip
            // 
            this.folderBrowserZip.Description = "Choose output Zip folder:";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(505, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(153, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Warning: avoid using subpaths";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 661);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(596, 568);
            this.Name = "MainWindow";
            this.Text = "E Package Builder";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserProject;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserSources;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbProjectDescription;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbReqOpenExplorer;
        private System.Windows.Forms.TextBox tbPathProject;
        private System.Windows.Forms.Button buttonBrowseProjectFolder;
        private System.Windows.Forms.TextBox tbPathSources;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonMakeSources;
        private System.Windows.Forms.Button buttonBrowseSourcesFolder;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.Button buttonLogClear;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveContextToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogContext;
        private System.Windows.Forms.SaveFileDialog saveFileDialogContext;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonIncrVersion;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectGameCheckToolToolStripMenuItem;
        private System.Windows.Forms.Button buttonGameCheck;
        private System.Windows.Forms.Button buttonRenewVersion;
        private System.Windows.Forms.Button buttonShowVersion;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button buttonDoAll;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button buttonSeparator;
        private System.Windows.Forms.ToolStripMenuItem unityPathToolStripMenuItem;
        private System.Windows.Forms.TextBox tbPathBuilds;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button buttonBuildPc;
        private System.Windows.Forms.Button buttonBuildRelease;
        private System.Windows.Forms.Button buttonBrowseBuildsFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserBuilds;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonZip;
        private System.Windows.Forms.Button buttonBrowseZipFolder;
        private System.Windows.Forms.TextBox tbPathZip;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserZip;
        private System.Windows.Forms.Label label8;

    }
}

