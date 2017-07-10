namespace MultiRunner
{
    partial class MultiRunner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiRunner));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            this.CmbGateway = new System.Windows.Forms.ComboBox();
            this.CmbClient1 = new System.Windows.Forms.ComboBox();
            this.LblClient = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BtExecutoinLog = new System.Windows.Forms.Button();
            this.CmbTests = new System.Windows.Forms.ComboBox();
            this.labelTestsSet = new System.Windows.Forms.Label();
            this.BtCl1Log = new System.Windows.Forms.Button();
            this.BtCl2Log = new System.Windows.Forms.Button();
            this.CmbClient2 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BtGwLog = new System.Windows.Forms.Button();
            this.BtStop = new System.Windows.Forms.Button();
            this.BtRun = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.BtDown = new System.Windows.Forms.Button();
            this.BtUp = new System.Windows.Forms.Button();
            this.ChkAll = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textStatus = new System.Windows.Forms.Label();
            this.testsClicked = new System.Windows.Forms.Label();
            this.DG = new System.Windows.Forms.DataGridView();
            this.ChkTests = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TestName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestRes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GwStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CLStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FailReason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StripOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.StripSaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StripDebug = new System.Windows.Forms.ToolStripMenuItem();
            this.StripLoop = new System.Windows.Forms.ToolStripMenuItem();
            this.StripTestsSets = new System.Windows.Forms.ToolStripMenuItem();
            this.StripAboutInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StripVersionInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.DialogFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.CmbClient4 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.CmbClient3 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DG)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CmbGateway
            // 
            this.CmbGateway.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbGateway.FormattingEnabled = true;
            this.CmbGateway.Items.AddRange(new object[] {
            "127.0.0.1 1",
            "127.0.0.1 2",
            "127.0.0.1 3",
            "127.0.0.1 4",
            "127.0.0.1 5",
            "127.0.0.1 6",
            "127.0.0.1 7",
            "127.0.0.1 8"});
            this.CmbGateway.Location = new System.Drawing.Point(82, 47);
            this.CmbGateway.Name = "CmbGateway";
            this.CmbGateway.Size = new System.Drawing.Size(124, 21);
            this.CmbGateway.Sorted = true;
            this.CmbGateway.TabIndex = 16;
            this.CmbGateway.SelectedIndexChanged += new System.EventHandler(this.CmbGateway_SelectedIndexChanged);
            // 
            // CmbClient1
            // 
            this.CmbClient1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbClient1.FormattingEnabled = true;
            this.CmbClient1.Items.AddRange(new object[] {
            "127.0.0.1 1",
            "127.0.0.1 2",
            "127.0.0.1 3",
            "127.0.0.1 4",
            "127.0.0.1 5",
            "127.0.0.1 6",
            "127.0.0.1 7",
            "127.0.0.1 8",
            "None"});
            this.CmbClient1.Location = new System.Drawing.Point(82, 73);
            this.CmbClient1.Name = "CmbClient1";
            this.CmbClient1.Size = new System.Drawing.Size(124, 21);
            this.CmbClient1.Sorted = true;
            this.CmbClient1.TabIndex = 15;
            // 
            // LblClient
            // 
            this.LblClient.AutoSize = true;
            this.LblClient.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblClient.Location = new System.Drawing.Point(21, 76);
            this.LblClient.Name = "LblClient";
            this.LblClient.Size = new System.Drawing.Size(52, 14);
            this.LblClient.TabIndex = 13;
            this.LblClient.Text = "Client1 :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 11;
            this.label2.Text = "Gateway :";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.CmbClient3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.CmbClient4);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.BtExecutoinLog);
            this.panel1.Controls.Add(this.CmbTests);
            this.panel1.Controls.Add(this.labelTestsSet);
            this.panel1.Controls.Add(this.BtCl1Log);
            this.panel1.Controls.Add(this.BtCl2Log);
            this.panel1.Controls.Add(this.CmbClient2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.BtGwLog);
            this.panel1.Controls.Add(this.BtStop);
            this.panel1.Controls.Add(this.BtRun);
            this.panel1.Controls.Add(this.CmbGateway);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.CmbClient1);
            this.panel1.Controls.Add(this.LblClient);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(264, 427);
            this.panel1.TabIndex = 18;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(72, 333);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(103, 85);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 33;
            this.pictureBox1.TabStop = false;
            // 
            // BtExecutoinLog
            // 
            this.BtExecutoinLog.Location = new System.Drawing.Point(24, 186);
            this.BtExecutoinLog.Name = "BtExecutoinLog";
            this.BtExecutoinLog.Size = new System.Drawing.Size(232, 23);
            this.BtExecutoinLog.TabIndex = 32;
            this.BtExecutoinLog.Text = "Execution Log";
            this.BtExecutoinLog.UseVisualStyleBackColor = true;
            this.BtExecutoinLog.Click += new System.EventHandler(this.ButExecutoinLog_Click);
            // 
            // CmbTests
            // 
            this.CmbTests.FormattingEnabled = true;
            this.CmbTests.Location = new System.Drawing.Point(82, 10);
            this.CmbTests.Name = "CmbTests";
            this.CmbTests.Size = new System.Drawing.Size(124, 21);
            this.CmbTests.Sorted = true;
            this.CmbTests.TabIndex = 29;
            this.CmbTests.DropDown += new System.EventHandler(this.CmbTests_DropDown);
            this.CmbTests.SelectedIndexChanged += new System.EventHandler(this.CmbTests_SelectedIndexChanged);
            // 
            // labelTestsSet
            // 
            this.labelTestsSet.AutoSize = true;
            this.labelTestsSet.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTestsSet.Location = new System.Drawing.Point(5, 12);
            this.labelTestsSet.Name = "labelTestsSet";
            this.labelTestsSet.Size = new System.Drawing.Size(68, 14);
            this.labelTestsSet.TabIndex = 30;
            this.labelTestsSet.Text = "Tests Set :";
            // 
            // BtCl1Log
            // 
            this.BtCl1Log.Location = new System.Drawing.Point(212, 73);
            this.BtCl1Log.Name = "BtCl1Log";
            this.BtCl1Log.Size = new System.Drawing.Size(44, 21);
            this.BtCl1Log.TabIndex = 28;
            this.BtCl1Log.Text = "Log";
            this.BtCl1Log.UseVisualStyleBackColor = true;
            this.BtCl1Log.Click += new System.EventHandler(this.ButCl1Log_Click);
            // 
            // BtCl2Log
            // 
            this.BtCl2Log.Location = new System.Drawing.Point(212, 100);
            this.BtCl2Log.Name = "BtCl2Log";
            this.BtCl2Log.Size = new System.Drawing.Size(44, 21);
            this.BtCl2Log.TabIndex = 27;
            this.BtCl2Log.Text = "Log";
            this.BtCl2Log.UseVisualStyleBackColor = true;
            this.BtCl2Log.Click += new System.EventHandler(this.ButCl2Log_Click);
            // 
            // CmbClient2
            // 
            this.CmbClient2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbClient2.FormattingEnabled = true;
            this.CmbClient2.Items.AddRange(new object[] {
            "127.0.0.1 1",
            "127.0.0.1 2",
            "127.0.0.1 3",
            "127.0.0.1 4",
            "127.0.0.1 5",
            "127.0.0.1 6",
            "127.0.0.1 7",
            "127.0.0.1 8",
            "None"});
            this.CmbClient2.Location = new System.Drawing.Point(82, 100);
            this.CmbClient2.Name = "CmbClient2";
            this.CmbClient2.Size = new System.Drawing.Size(124, 21);
            this.CmbClient2.Sorted = true;
            this.CmbClient2.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(21, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 14);
            this.label3.TabIndex = 25;
            this.label3.Text = "Client2 :";
            // 
            // BtGwLog
            // 
            this.BtGwLog.Location = new System.Drawing.Point(213, 47);
            this.BtGwLog.Name = "BtGwLog";
            this.BtGwLog.Size = new System.Drawing.Size(44, 21);
            this.BtGwLog.TabIndex = 20;
            this.BtGwLog.Text = "Log";
            this.BtGwLog.UseVisualStyleBackColor = true;
            this.BtGwLog.Click += new System.EventHandler(this.ButGwLog_Click);
            // 
            // BtStop
            // 
            this.BtStop.Location = new System.Drawing.Point(135, 246);
            this.BtStop.Name = "BtStop";
            this.BtStop.Size = new System.Drawing.Size(103, 48);
            this.BtStop.TabIndex = 20;
            this.BtStop.Text = "STOP";
            this.BtStop.UseVisualStyleBackColor = true;
            this.BtStop.Click += new System.EventHandler(this.ButStop_Click);
            // 
            // BtRun
            // 
            this.BtRun.Location = new System.Drawing.Point(28, 246);
            this.BtRun.Name = "BtRun";
            this.BtRun.Size = new System.Drawing.Size(103, 48);
            this.BtRun.TabIndex = 17;
            this.BtRun.Text = "RUN";
            this.BtRun.UseVisualStyleBackColor = true;
            this.BtRun.Click += new System.EventHandler(this.BtRun_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 270F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.DG, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1272, 468);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.BtDown);
            this.panel3.Controls.Add(this.BtUp);
            this.panel3.Controls.Add(this.ChkAll);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(273, 436);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(996, 29);
            this.panel3.TabIndex = 20;
            // 
            // BtDown
            // 
            this.BtDown.BackColor = System.Drawing.SystemColors.Control;
            this.BtDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtDown.Image = ((System.Drawing.Image)(resources.GetObject("BtDown.Image")));
            this.BtDown.Location = new System.Drawing.Point(910, 0);
            this.BtDown.Name = "BtDown";
            this.BtDown.Size = new System.Drawing.Size(43, 29);
            this.BtDown.TabIndex = 5;
            this.toolTip.SetToolTip(this.BtDown, "Move Row Down");
            this.BtDown.UseVisualStyleBackColor = true;
            this.BtDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // BtUp
            // 
            this.BtUp.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtUp.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.BtUp.FlatAppearance.BorderSize = 0;
            this.BtUp.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtUp.Image = ((System.Drawing.Image)(resources.GetObject("BtUp.Image")));
            this.BtUp.Location = new System.Drawing.Point(953, 0);
            this.BtUp.Name = "BtUp";
            this.BtUp.Size = new System.Drawing.Size(43, 29);
            this.BtUp.TabIndex = 4;
            this.toolTip.SetToolTip(this.BtUp, "Move Row Up");
            this.BtUp.UseVisualStyleBackColor = true;
            this.BtUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // ChkAll
            // 
            this.ChkAll.AutoSize = true;
            this.ChkAll.Location = new System.Drawing.Point(0, 10);
            this.ChkAll.Name = "ChkAll";
            this.ChkAll.Size = new System.Drawing.Size(70, 17);
            this.ChkAll.TabIndex = 3;
            this.ChkAll.Text = "Select All";
            this.ChkAll.UseVisualStyleBackColor = true;
            this.ChkAll.CheckedChanged += new System.EventHandler(this.ChkAll_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textStatus);
            this.panel2.Controls.Add(this.testsClicked);
            this.panel2.Location = new System.Drawing.Point(3, 436);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(264, 29);
            this.panel2.TabIndex = 19;
            // 
            // textStatus
            // 
            this.textStatus.Location = new System.Drawing.Point(3, 12);
            this.textStatus.Name = "textStatus";
            this.textStatus.Size = new System.Drawing.Size(121, 16);
            this.textStatus.TabIndex = 21;
            // 
            // testsClicked
            // 
            this.testsClicked.Location = new System.Drawing.Point(132, 11);
            this.testsClicked.Name = "testsClicked";
            this.testsClicked.Size = new System.Drawing.Size(129, 16);
            this.testsClicked.TabIndex = 20;
            // 
            // DG
            // 
            this.DG.AllowUserToAddRows = false;
            this.DG.AllowUserToDeleteRows = false;
            this.DG.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DG.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DG.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.DG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DG.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChkTests,
            this.TestName,
            this.TestRes,
            this.GwStatus,
            this.CLStatus,
            this.FailReason});
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle20.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DG.DefaultCellStyle = dataGridViewCellStyle20;
            this.DG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DG.Location = new System.Drawing.Point(273, 3);
            this.DG.MultiSelect = false;
            this.DG.Name = "DG";
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle21.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DG.RowHeadersDefaultCellStyle = dataGridViewCellStyle21;
            this.DG.RowHeadersVisible = false;
            this.DG.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DG.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DG.Size = new System.Drawing.Size(996, 427);
            this.DG.TabIndex = 2;
            // 
            // ChkTests
            // 
            this.ChkTests.FillWeight = 40F;
            this.ChkTests.HeaderText = "";
            this.ChkTests.Name = "ChkTests";
            // 
            // TestName
            // 
            this.TestName.FillWeight = 150F;
            this.TestName.HeaderText = "Test Name";
            this.TestName.Name = "TestName";
            this.TestName.ReadOnly = true;
            // 
            // TestRes
            // 
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.TestRes.DefaultCellStyle = dataGridViewCellStyle16;
            this.TestRes.FillWeight = 110F;
            this.TestRes.HeaderText = "Test Result";
            this.TestRes.Name = "TestRes";
            this.TestRes.ReadOnly = true;
            // 
            // GwStatus
            // 
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GwStatus.DefaultCellStyle = dataGridViewCellStyle17;
            this.GwStatus.FillWeight = 110F;
            this.GwStatus.HeaderText = "GW Status";
            this.GwStatus.Name = "GwStatus";
            this.GwStatus.ReadOnly = true;
            // 
            // CLStatus
            // 
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.CLStatus.DefaultCellStyle = dataGridViewCellStyle18;
            this.CLStatus.FillWeight = 110F;
            this.CLStatus.HeaderText = "CL Status";
            this.CLStatus.Name = "CLStatus";
            this.CLStatus.ReadOnly = true;
            // 
            // FailReason
            // 
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.FailReason.DefaultCellStyle = dataGridViewCellStyle19;
            this.FailReason.FillWeight = 650F;
            this.FailReason.HeaderText = "Failure Reason";
            this.FailReason.Name = "FailReason";
            this.FailReason.ReadOnly = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.StripAboutInfo});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1272, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StripOpenFile,
            this.StripSaveFile});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // StripOpenFile
            // 
            this.StripOpenFile.Name = "StripOpenFile";
            this.StripOpenFile.Size = new System.Drawing.Size(103, 22);
            this.StripOpenFile.Text = "Open";
            this.StripOpenFile.Click += new System.EventHandler(this.StripOpenFile_Click);
            // 
            // StripSaveFile
            // 
            this.StripSaveFile.Name = "StripSaveFile";
            this.StripSaveFile.Size = new System.Drawing.Size(103, 22);
            this.StripSaveFile.Text = "Save";
            this.StripSaveFile.Click += new System.EventHandler(this.StripSaveFile_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StripDebug,
            this.StripLoop,
            this.StripTestsSets});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // StripDebug
            // 
            this.StripDebug.CheckOnClick = true;
            this.StripDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StripDebug.Name = "StripDebug";
            this.StripDebug.Size = new System.Drawing.Size(143, 22);
            this.StripDebug.Text = "Debug Mode";
            this.StripDebug.CheckedChanged += new System.EventHandler(this.StripDebug_CheckedChanged);
            // 
            // StripLoop
            // 
            this.StripLoop.CheckOnClick = true;
            this.StripLoop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StripLoop.Name = "StripLoop";
            this.StripLoop.Size = new System.Drawing.Size(143, 22);
            this.StripLoop.Text = "Run in Loop";
            // 
            // StripTestsSets
            // 
            this.StripTestsSets.Name = "StripTestsSets";
            this.StripTestsSets.Size = new System.Drawing.Size(143, 22);
            this.StripTestsSets.Text = "Tests Sets";
            this.StripTestsSets.Click += new System.EventHandler(this.StripTestsSets_Click);
            // 
            // StripAboutInfo
            // 
            this.StripAboutInfo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem,
            this.StripVersionInfo});
            this.StripAboutInfo.Name = "StripAboutInfo";
            this.StripAboutInfo.Size = new System.Drawing.Size(52, 20);
            this.StripAboutInfo.Text = "About";
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.logToolStripMenuItem.Text = "Log Folder";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.StripLogFolder_Click);
            // 
            // StripVersionInfo
            // 
            this.StripVersionInfo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.StripVersionInfo.Name = "StripVersionInfo";
            this.StripVersionInfo.Size = new System.Drawing.Size(152, 22);
            this.StripVersionInfo.Text = "Version";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Text = "4.4";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(212, 154);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(44, 21);
            this.button1.TabIndex = 36;
            this.button1.Text = "Log";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ButCl4Log_Click);
            // 
            // CmbClient4
            // 
            this.CmbClient4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbClient4.FormattingEnabled = true;
            this.CmbClient4.Items.AddRange(new object[] {
            "127.0.0.1 1",
            "127.0.0.1 2",
            "127.0.0.1 3",
            "127.0.0.1 4",
            "127.0.0.1 5",
            "127.0.0.1 6",
            "127.0.0.1 7",
            "127.0.0.1 8",
            "None"});
            this.CmbClient4.Location = new System.Drawing.Point(82, 154);
            this.CmbClient4.Name = "CmbClient4";
            this.CmbClient4.Size = new System.Drawing.Size(124, 21);
            this.CmbClient4.Sorted = true;
            this.CmbClient4.TabIndex = 35;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 156);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 14);
            this.label1.TabIndex = 34;
            this.label1.Text = "Client4 :";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(212, 127);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(44, 21);
            this.button2.TabIndex = 39;
            this.button2.Text = "Log";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ButCl3Log_Click);
            // 
            // CmbClient3
            // 
            this.CmbClient3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbClient3.FormattingEnabled = true;
            this.CmbClient3.Items.AddRange(new object[] {
            "127.0.0.1 1",
            "127.0.0.1 2",
            "127.0.0.1 3",
            "127.0.0.1 4",
            "127.0.0.1 5",
            "127.0.0.1 6",
            "127.0.0.1 7",
            "127.0.0.1 8",
            "None"});
            this.CmbClient3.Location = new System.Drawing.Point(82, 127);
            this.CmbClient3.Name = "CmbClient3";
            this.CmbClient3.Size = new System.Drawing.Size(124, 21);
            this.CmbClient3.Sorted = true;
            this.CmbClient3.TabIndex = 38;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(21, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 14);
            this.label4.TabIndex = 37;
            this.label4.Text = "Client3 :";
            // 
            // MultiRunner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1272, 492);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MultiRunner";
            this.Text = "Multi Runner";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MultiRunner_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MultiRunner_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DG)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CmbGateway;
        private System.Windows.Forms.ComboBox CmbClient1;
        private System.Windows.Forms.Label LblClient;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtRun;
        private System.Windows.Forms.Button BtStop;
        private System.Windows.Forms.Button BtGwLog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView DG;
        private System.Windows.Forms.CheckBox ChkAll;
        private System.Windows.Forms.ComboBox CmbClient2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BtCl1Log;
        private System.Windows.Forms.Button BtCl2Log;
        private System.Windows.Forms.ComboBox CmbTests;
        private System.Windows.Forms.Label labelTestsSet;
        private System.Windows.Forms.Button BtExecutoinLog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StripDebug;
        private System.Windows.Forms.ToolStripMenuItem StripLoop;
        private System.Windows.Forms.FolderBrowserDialog DialogFolderBrowser;
        private System.Windows.Forms.ToolStripMenuItem StripAboutInfo;
        private System.Windows.Forms.ToolStripMenuItem StripVersionInfo;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StripOpenFile;
        private System.Windows.Forms.ToolStripMenuItem StripSaveFile;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem StripTestsSets;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label testsClicked;
        private System.Windows.Forms.Label textStatus;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button BtDown;
        private System.Windows.Forms.Button BtUp;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ChkTests;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestRes;
        private System.Windows.Forms.DataGridViewTextBoxColumn GwStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn CLStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn FailReason;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox CmbClient3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox CmbClient4;
        private System.Windows.Forms.Label label1;
    }
}

