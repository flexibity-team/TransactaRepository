namespace Parking.Gazprom
{
  partial class SettingsForm
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
      this.tabMain = new System.Windows.Forms.TabControl();
      this.tabPageSystem = new System.Windows.Forms.TabPage();
      this.lblMonitorIntervalSec = new System.Windows.Forms.Label();
      this.updMonitorInterval = new System.Windows.Forms.NumericUpDown();
      this.lblMonitorInterval = new System.Windows.Forms.Label();
      this.lblLogLevel = new System.Windows.Forms.Label();
      this.cboLogLevel = new System.Windows.Forms.ComboBox();
      this.bwServerList = new System.ComponentModel.BackgroundWorker();
      this.bwProgress = new System.ComponentModel.BackgroundWorker();
      this.panelButtons.SuspendLayout();
      this.tabMain.SuspendLayout();
      this.tabPageSystem.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.updMonitorInterval)).BeginInit();
      this.SuspendLayout();
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(331, 8);
      this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.btnCancel.Size = new System.Drawing.Size(82, 30);
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(242, 8);
      this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.btnOK.Size = new System.Drawing.Size(82, 30);
      // 
      // panelButtons
      // 
      this.panelButtons.Location = new System.Drawing.Point(0, 227);
      this.panelButtons.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.panelButtons.Size = new System.Drawing.Size(425, 47);
      // 
      // tabMain
      // 
      this.tabMain.Controls.Add(this.tabPageSystem);
      this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabMain.Location = new System.Drawing.Point(0, 0);
      this.tabMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tabMain.Name = "tabMain";
      this.tabMain.SelectedIndex = 0;
      this.tabMain.Size = new System.Drawing.Size(425, 227);
      this.tabMain.TabIndex = 9;
      // 
      // tabPageSystem
      // 
      this.tabPageSystem.Controls.Add(this.lblMonitorIntervalSec);
      this.tabPageSystem.Controls.Add(this.updMonitorInterval);
      this.tabPageSystem.Controls.Add(this.lblMonitorInterval);
      this.tabPageSystem.Controls.Add(this.lblLogLevel);
      this.tabPageSystem.Controls.Add(this.cboLogLevel);
      this.tabPageSystem.Location = new System.Drawing.Point(4, 25);
      this.tabPageSystem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tabPageSystem.Name = "tabPageSystem";
      this.tabPageSystem.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tabPageSystem.Size = new System.Drawing.Size(417, 198);
      this.tabPageSystem.TabIndex = 0;
      this.tabPageSystem.Text = "Система";
      this.tabPageSystem.UseVisualStyleBackColor = true;
      // 
      // lblMonitorIntervalSec
      // 
      this.lblMonitorIntervalSec.AutoSize = true;
      this.lblMonitorIntervalSec.Location = new System.Drawing.Point(279, 75);
      this.lblMonitorIntervalSec.Name = "lblMonitorIntervalSec";
      this.lblMonitorIntervalSec.Size = new System.Drawing.Size(31, 16);
      this.lblMonitorIntervalSec.TabIndex = 4;
      this.lblMonitorIntervalSec.Text = "сек.";
      // 
      // updMonitorInterval
      // 
      this.updMonitorInterval.Location = new System.Drawing.Point(185, 71);
      this.updMonitorInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.updMonitorInterval.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
      this.updMonitorInterval.Name = "updMonitorInterval";
      this.updMonitorInterval.Size = new System.Drawing.Size(83, 23);
      this.updMonitorInterval.TabIndex = 3;
      // 
      // lblMonitorInterval
      // 
      this.lblMonitorInterval.AutoSize = true;
      this.lblMonitorInterval.Location = new System.Drawing.Point(17, 75);
      this.lblMonitorInterval.Name = "lblMonitorInterval";
      this.lblMonitorInterval.Size = new System.Drawing.Size(143, 16);
      this.lblMonitorInterval.TabIndex = 2;
      this.lblMonitorInterval.Text = "Интервал мониторинга";
      // 
      // lblLogLevel
      // 
      this.lblLogLevel.AutoSize = true;
      this.lblLogLevel.Location = new System.Drawing.Point(17, 27);
      this.lblLogLevel.Name = "lblLogLevel";
      this.lblLogLevel.Size = new System.Drawing.Size(87, 16);
      this.lblLogLevel.TabIndex = 1;
      this.lblLogLevel.Text = "Уровень лога";
      // 
      // cboLogLevel
      // 
      this.cboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboLogLevel.FormattingEnabled = true;
      this.cboLogLevel.Location = new System.Drawing.Point(131, 23);
      this.cboLogLevel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.cboLogLevel.Name = "cboLogLevel";
      this.cboLogLevel.Size = new System.Drawing.Size(203, 24);
      this.cboLogLevel.TabIndex = 0;
      // 
      // SettingsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(425, 274);
      this.Controls.Add(this.tabMain);
      this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.Name = "SettingsForm";
      this.Text = "Настройки";
      this.Controls.SetChildIndex(this.panelButtons, 0);
      this.Controls.SetChildIndex(this.tabMain, 0);
      this.panelButtons.ResumeLayout(false);
      this.tabMain.ResumeLayout(false);
      this.tabPageSystem.ResumeLayout(false);
      this.tabPageSystem.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.updMonitorInterval)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabMain;
    private System.Windows.Forms.TabPage tabPageSystem;
    private System.Windows.Forms.Label lblLogLevel;
    private System.Windows.Forms.ComboBox cboLogLevel;
    private System.Windows.Forms.Label lblMonitorInterval;
    private System.Windows.Forms.Label lblMonitorIntervalSec;
    private System.Windows.Forms.NumericUpDown updMonitorInterval;
    private System.ComponentModel.BackgroundWorker bwServerList;
    private System.ComponentModel.BackgroundWorker bwProgress;

  }
}