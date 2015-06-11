namespace Parking.SberBank
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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.ColumnHeader columnHeader8;
      System.Windows.Forms.ColumnHeader columnHeader9;
      System.Windows.Forms.ColumnHeader columnHeader7;
      System.Windows.Forms.ColumnHeader columnHeader12;
      System.Windows.Forms.ColumnHeader columnHeader13;
      System.Windows.Forms.ColumnHeader columnHeader10;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.imgServices = new System.Windows.Forms.ImageList(this.components);
      this.status = new System.Windows.Forms.StatusStrip();
      this.tslblStatusState = new System.Windows.Forms.ToolStripStatusLabel();
      this.tslblStatusDatabase = new System.Windows.Forms.ToolStripStatusLabel();
      this.tslblStatusNetwork = new System.Windows.Forms.ToolStripStatusLabel();
      this.tslblStatusCalculator = new System.Windows.Forms.ToolStripStatusLabel();
      this.lvwExtensions = new RMLib.Forms.BufferedListView();
      this.picTop = new System.Windows.Forms.PictureBox();
      this.linkMenu = new System.Windows.Forms.LinkLabel();
      this.timerMonitoring = new System.Windows.Forms.Timer(this.components);
      this.tabMain = new System.Windows.Forms.TabControl();
      this.tabPageExtensions = new System.Windows.Forms.TabPage();
      this.btnExtensionsControl = new System.Windows.Forms.Button();
      this.btnExtensionsLoad = new System.Windows.Forms.Button();
      this.lblExtensionsWarning = new System.Windows.Forms.Label();
      this.tabPageMonitor = new System.Windows.Forms.TabPage();
      this.btnMonitoringRefresh = new System.Windows.Forms.Button();
      this.btnMonitoringReset = new System.Windows.Forms.Button();
      this.lblMonitorWarning = new System.Windows.Forms.Label();
      this.lvwMonitor = new RMLib.Forms.BufferedListView();
      columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.status.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picTop)).BeginInit();
      this.picTop.SuspendLayout();
      this.tabMain.SuspendLayout();
      this.tabPageExtensions.SuspendLayout();
      this.tabPageMonitor.SuspendLayout();
      this.SuspendLayout();
      // 
      // columnHeader8
      // 
      columnHeader8.Text = "Имя";
      columnHeader8.Width = 282;
      // 
      // columnHeader9
      // 
      columnHeader9.Text = "Описание";
      columnHeader9.Width = 715;
      // 
      // columnHeader7
      // 
      columnHeader7.Text = "";
      columnHeader7.Width = 29;
      // 
      // columnHeader12
      // 
      columnHeader12.Text = "Параметр";
      columnHeader12.Width = 338;
      // 
      // columnHeader13
      // 
      columnHeader13.Text = "Значение";
      columnHeader13.Width = 650;
      // 
      // columnHeader10
      // 
      columnHeader10.Text = "";
      columnHeader10.Width = 29;
      // 
      // imgServices
      // 
      this.imgServices.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imgServices.ImageSize = new System.Drawing.Size(16, 16);
      this.imgServices.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // status
      // 
      this.status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslblStatusState,
            this.tslblStatusDatabase,
            this.tslblStatusNetwork,
            this.tslblStatusCalculator});
      this.status.Location = new System.Drawing.Point(0, 450);
      this.status.Name = "status";
      this.status.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
      this.status.Size = new System.Drawing.Size(900, 22);
      this.status.TabIndex = 1;
      // 
      // tslblStatusState
      // 
      this.tslblStatusState.AutoSize = false;
      this.tslblStatusState.BackColor = System.Drawing.Color.Transparent;
      this.tslblStatusState.Name = "tslblStatusState";
      this.tslblStatusState.Size = new System.Drawing.Size(100, 17);
      this.tslblStatusState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tslblStatusDatabase
      // 
      this.tslblStatusDatabase.BackColor = System.Drawing.Color.Transparent;
      this.tslblStatusDatabase.Margin = new System.Windows.Forms.Padding(20, 3, 0, 2);
      this.tslblStatusDatabase.Name = "tslblStatusDatabase";
      this.tslblStatusDatabase.Size = new System.Drawing.Size(241, 17);
      this.tslblStatusDatabase.Spring = true;
      this.tslblStatusDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // tslblStatusNetwork
      // 
      this.tslblStatusNetwork.BackColor = System.Drawing.Color.Transparent;
      this.tslblStatusNetwork.Margin = new System.Windows.Forms.Padding(20, 3, 0, 2);
      this.tslblStatusNetwork.Name = "tslblStatusNetwork";
      this.tslblStatusNetwork.Size = new System.Drawing.Size(241, 17);
      this.tslblStatusNetwork.Spring = true;
      this.tslblStatusNetwork.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // tslblStatusCalculator
      // 
      this.tslblStatusCalculator.BackColor = System.Drawing.Color.Transparent;
      this.tslblStatusCalculator.Margin = new System.Windows.Forms.Padding(20, 3, 0, 2);
      this.tslblStatusCalculator.Name = "tslblStatusCalculator";
      this.tslblStatusCalculator.Size = new System.Drawing.Size(241, 17);
      this.tslblStatusCalculator.Spring = true;
      this.tslblStatusCalculator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lvwExtensions
      // 
      this.lvwExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lvwExtensions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader7,
            columnHeader8,
            columnHeader9});
      this.lvwExtensions.FullRowSelect = true;
      this.lvwExtensions.GridLines = true;
      this.lvwExtensions.HideSelection = false;
      this.lvwExtensions.Location = new System.Drawing.Point(3, 4);
      this.lvwExtensions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.lvwExtensions.MultiSelect = false;
      this.lvwExtensions.Name = "lvwExtensions";
      this.lvwExtensions.ShowItemToolTips = true;
      this.lvwExtensions.Size = new System.Drawing.Size(883, 287);
      this.lvwExtensions.SmallImageList = this.imgServices;
      this.lvwExtensions.TabIndex = 0;
      this.lvwExtensions.UseCompatibleStateImageBehavior = false;
      this.lvwExtensions.View = System.Windows.Forms.View.Details;
      this.lvwExtensions.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwExtensions_ItemSelectionChanged);
      this.lvwExtensions.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvwExtensions_MouseClick);
      this.lvwExtensions.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvwExtensions_MouseDoubleClick);
      // 
      // picTop
      // 
      this.picTop.Controls.Add(this.linkMenu);
      this.picTop.Dock = System.Windows.Forms.DockStyle.Top;
      this.picTop.Location = new System.Drawing.Point(0, 0);
      this.picTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.picTop.Name = "picTop";
      this.picTop.Size = new System.Drawing.Size(900, 71);
      this.picTop.TabIndex = 3;
      this.picTop.TabStop = false;
      this.picTop.Click += new System.EventHandler(this.picTop_Click);
      // 
      // linkMenu
      // 
      this.linkMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.linkMenu.AutoSize = true;
      this.linkMenu.BackColor = System.Drawing.Color.Transparent;
      this.linkMenu.Enabled = false;
      this.linkMenu.Location = new System.Drawing.Point(812, 8);
      this.linkMenu.Name = "linkMenu";
      this.linkMenu.Size = new System.Drawing.Size(78, 16);
      this.linkMenu.TabIndex = 5;
      this.linkMenu.TabStop = true;
      this.linkMenu.Text = "Управление";
      this.linkMenu.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.linkMenu.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMenu_LinkClicked);
      // 
      // timerMonitoring
      // 
      this.timerMonitoring.Interval = 5000;
      this.timerMonitoring.Tick += new System.EventHandler(this.timerMonitoring_Tick);
      // 
      // tabMain
      // 
      this.tabMain.Controls.Add(this.tabPageExtensions);
      this.tabMain.Controls.Add(this.tabPageMonitor);
      this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabMain.ImageList = this.imgServices;
      this.tabMain.Location = new System.Drawing.Point(0, 71);
      this.tabMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tabMain.Name = "tabMain";
      this.tabMain.SelectedIndex = 0;
      this.tabMain.Size = new System.Drawing.Size(900, 379);
      this.tabMain.TabIndex = 4;
      this.tabMain.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabMain_Selected);
      // 
      // tabPageExtensions
      // 
      this.tabPageExtensions.Controls.Add(this.btnExtensionsControl);
      this.tabPageExtensions.Controls.Add(this.btnExtensionsLoad);
      this.tabPageExtensions.Controls.Add(this.lblExtensionsWarning);
      this.tabPageExtensions.Controls.Add(this.lvwExtensions);
      this.tabPageExtensions.Location = new System.Drawing.Point(4, 25);
      this.tabPageExtensions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tabPageExtensions.Name = "tabPageExtensions";
      this.tabPageExtensions.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tabPageExtensions.Size = new System.Drawing.Size(892, 350);
      this.tabPageExtensions.TabIndex = 1;
      this.tabPageExtensions.Text = "Расширения";
      this.tabPageExtensions.UseVisualStyleBackColor = true;
      // 
      // btnExtensionsControl
      // 
      this.btnExtensionsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnExtensionsControl.Location = new System.Drawing.Point(589, 303);
      this.btnExtensionsControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.btnExtensionsControl.Name = "btnExtensionsControl";
      this.btnExtensionsControl.Size = new System.Drawing.Size(139, 31);
      this.btnExtensionsControl.TabIndex = 7;
      this.btnExtensionsControl.UseVisualStyleBackColor = true;
      this.btnExtensionsControl.Click += new System.EventHandler(this.btnExtensionsControl_Click);
      // 
      // btnExtensionsLoad
      // 
      this.btnExtensionsLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnExtensionsLoad.Location = new System.Drawing.Point(742, 303);
      this.btnExtensionsLoad.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.btnExtensionsLoad.Name = "btnExtensionsLoad";
      this.btnExtensionsLoad.Size = new System.Drawing.Size(139, 31);
      this.btnExtensionsLoad.TabIndex = 7;
      this.btnExtensionsLoad.Text = "Загрузить";
      this.btnExtensionsLoad.UseVisualStyleBackColor = true;
      this.btnExtensionsLoad.Click += new System.EventHandler(this.btnExtensionsLoad_Click);
      // 
      // lblExtensionsWarning
      // 
      this.lblExtensionsWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblExtensionsWarning.ForeColor = System.Drawing.Color.Red;
      this.lblExtensionsWarning.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.lblExtensionsWarning.ImageList = this.imgServices;
      this.lblExtensionsWarning.Location = new System.Drawing.Point(6, 297);
      this.lblExtensionsWarning.Name = "lblExtensionsWarning";
      this.lblExtensionsWarning.Size = new System.Drawing.Size(568, 42);
      this.lblExtensionsWarning.TabIndex = 6;
      this.lblExtensionsWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.lblExtensionsWarning.Visible = false;
      // 
      // tabPageMonitor
      // 
      this.tabPageMonitor.Controls.Add(this.btnMonitoringRefresh);
      this.tabPageMonitor.Controls.Add(this.btnMonitoringReset);
      this.tabPageMonitor.Controls.Add(this.lblMonitorWarning);
      this.tabPageMonitor.Controls.Add(this.lvwMonitor);
      this.tabPageMonitor.Location = new System.Drawing.Point(4, 25);
      this.tabPageMonitor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tabPageMonitor.Name = "tabPageMonitor";
      this.tabPageMonitor.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tabPageMonitor.Size = new System.Drawing.Size(892, 350);
      this.tabPageMonitor.TabIndex = 2;
      this.tabPageMonitor.Text = "Мониторинг";
      this.tabPageMonitor.UseVisualStyleBackColor = true;
      // 
      // btnMonitoringRefresh
      // 
      this.btnMonitoringRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnMonitoringRefresh.Location = new System.Drawing.Point(597, 303);
      this.btnMonitoringRefresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.btnMonitoringRefresh.Name = "btnMonitoringRefresh";
      this.btnMonitoringRefresh.Size = new System.Drawing.Size(139, 31);
      this.btnMonitoringRefresh.TabIndex = 6;
      this.btnMonitoringRefresh.Text = "Обновить";
      this.btnMonitoringRefresh.UseVisualStyleBackColor = true;
      this.btnMonitoringRefresh.Click += new System.EventHandler(this.btnMonitoringRefresh_Click);
      // 
      // btnMonitoringReset
      // 
      this.btnMonitoringReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnMonitoringReset.Location = new System.Drawing.Point(742, 303);
      this.btnMonitoringReset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.btnMonitoringReset.Name = "btnMonitoringReset";
      this.btnMonitoringReset.Size = new System.Drawing.Size(139, 31);
      this.btnMonitoringReset.TabIndex = 6;
      this.btnMonitoringReset.Text = "Сброс";
      this.btnMonitoringReset.UseVisualStyleBackColor = true;
      this.btnMonitoringReset.Click += new System.EventHandler(this.btnMonitoringReset_Click);
      // 
      // lblMonitorWarning
      // 
      this.lblMonitorWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblMonitorWarning.ForeColor = System.Drawing.Color.Red;
      this.lblMonitorWarning.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.lblMonitorWarning.ImageList = this.imgServices;
      this.lblMonitorWarning.Location = new System.Drawing.Point(6, 297);
      this.lblMonitorWarning.Name = "lblMonitorWarning";
      this.lblMonitorWarning.Size = new System.Drawing.Size(585, 42);
      this.lblMonitorWarning.TabIndex = 5;
      this.lblMonitorWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.lblMonitorWarning.Visible = false;
      // 
      // lvwMonitor
      // 
      this.lvwMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lvwMonitor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader10,
            columnHeader12,
            columnHeader13});
      this.lvwMonitor.FullRowSelect = true;
      this.lvwMonitor.GridLines = true;
      this.lvwMonitor.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.lvwMonitor.Location = new System.Drawing.Point(3, 4);
      this.lvwMonitor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.lvwMonitor.MultiSelect = false;
      this.lvwMonitor.Name = "lvwMonitor";
      this.lvwMonitor.Size = new System.Drawing.Size(883, 287);
      this.lvwMonitor.SmallImageList = this.imgServices;
      this.lvwMonitor.TabIndex = 1;
      this.lvwMonitor.UseCompatibleStateImageBehavior = false;
      this.lvwMonitor.View = System.Windows.Forms.View.Details;
      this.lvwMonitor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvwMonitor_MouseDoubleClick);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(900, 472);
      this.Controls.Add(this.tabMain);
      this.Controls.Add(this.status);
      this.Controls.Add(this.picTop);
      this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.Name = "MainForm";
      this.status.ResumeLayout(false);
      this.status.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picTop)).EndInit();
      this.picTop.ResumeLayout(false);
      this.picTop.PerformLayout();
      this.tabMain.ResumeLayout(false);
      this.tabPageExtensions.ResumeLayout(false);
      this.tabPageMonitor.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStripStatusLabel tslblStatusState;
    private System.Windows.Forms.ToolStripStatusLabel tslblStatusDatabase;
    private RMLib.Forms.BufferedListView lvwExtensions;
    private System.Windows.Forms.ToolStripStatusLabel tslblStatusNetwork;
    private System.Windows.Forms.PictureBox picTop;
    private System.Windows.Forms.Timer timerMonitoring;
    private System.Windows.Forms.TabControl tabMain;
    private System.Windows.Forms.TabPage tabPageExtensions;
    private System.Windows.Forms.TabPage tabPageMonitor;
    private RMLib.Forms.BufferedListView lvwMonitor;
    private System.Windows.Forms.ToolStripStatusLabel tslblStatusCalculator;
    private System.Windows.Forms.Label lblMonitorWarning;
    private System.Windows.Forms.Label lblExtensionsWarning;
    private System.Windows.Forms.Button btnMonitoringReset;
    private System.Windows.Forms.Button btnMonitoringRefresh;
    private System.Windows.Forms.Button btnExtensionsLoad;
    private System.Windows.Forms.StatusStrip status;
    private System.Windows.Forms.LinkLabel linkMenu;
    private System.Windows.Forms.ImageList imgServices;
    private System.Windows.Forms.Button btnExtensionsControl;
  }
}

