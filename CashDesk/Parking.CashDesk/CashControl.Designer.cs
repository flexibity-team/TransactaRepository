namespace Parking.CashDesk
{
  partial class CashControl
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
      this.btnSessionClose = new System.Windows.Forms.Button();
      this.gbProperty = new System.Windows.Forms.GroupBox();
      this.lblWatchSum = new System.Windows.Forms.Label();
      this.lblMoneyInCash = new System.Windows.Forms.Label();
      this.lblSessionOpen = new System.Windows.Forms.Label();
      this.lblSessionOpenTxt = new System.Windows.Forms.Label();
      this.lblWatchSumTxt = new System.Windows.Forms.Label();
      this.lblMoneyInCashTxt = new System.Windows.Forms.Label();
      this.btnOpenSession = new System.Windows.Forms.Button();
      this.btnXReport = new System.Windows.Forms.Button();
      this.btnMoneyToCash = new System.Windows.Forms.Button();
      this.btnEncashment = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.gbMessage = new System.Windows.Forms.GroupBox();
      this.tbKKMMessage = new System.Windows.Forms.TextBox();
      this.gbProperty.SuspendLayout();
      this.gbMessage.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnSessionClose
      // 
      this.btnSessionClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.btnSessionClose.Location = new System.Drawing.Point(12, 94);
      this.btnSessionClose.Name = "btnSessionClose";
      this.btnSessionClose.Size = new System.Drawing.Size(180, 30);
      this.btnSessionClose.TabIndex = 0;
      this.btnSessionClose.Text = "Закрыть смену";
      this.btnSessionClose.UseVisualStyleBackColor = true;
      this.btnSessionClose.Click += new System.EventHandler(this.btnSessionClose_Click);
      // 
      // gbProperty
      // 
      this.gbProperty.Controls.Add(this.lblWatchSum);
      this.gbProperty.Controls.Add(this.lblMoneyInCash);
      this.gbProperty.Controls.Add(this.lblSessionOpen);
      this.gbProperty.Controls.Add(this.lblSessionOpenTxt);
      this.gbProperty.Controls.Add(this.lblWatchSumTxt);
      this.gbProperty.Controls.Add(this.lblMoneyInCashTxt);
      this.gbProperty.Location = new System.Drawing.Point(198, 41);
      this.gbProperty.Name = "gbProperty";
      this.gbProperty.Size = new System.Drawing.Size(401, 190);
      this.gbProperty.TabIndex = 2;
      this.gbProperty.TabStop = false;
      this.gbProperty.Text = "Параметры";
      // 
      // lblWatchSum
      // 
      this.lblWatchSum.AutoSize = true;
      this.lblWatchSum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblWatchSum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
      this.lblWatchSum.Location = new System.Drawing.Point(169, 166);
      this.lblWatchSum.Name = "lblWatchSum";
      this.lblWatchSum.Size = new System.Drawing.Size(19, 20);
      this.lblWatchSum.TabIndex = 2;
      this.lblWatchSum.Text = "0";
      // 
      // lblMoneyInCash
      // 
      this.lblMoneyInCash.AutoSize = true;
      this.lblMoneyInCash.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblMoneyInCash.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
      this.lblMoneyInCash.Location = new System.Drawing.Point(169, 130);
      this.lblMoneyInCash.Name = "lblMoneyInCash";
      this.lblMoneyInCash.Size = new System.Drawing.Size(19, 20);
      this.lblMoneyInCash.TabIndex = 2;
      this.lblMoneyInCash.Text = "0";
      // 
      // lblSessionOpen
      // 
      this.lblSessionOpen.AutoSize = true;
      this.lblSessionOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblSessionOpen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
      this.lblSessionOpen.Location = new System.Drawing.Point(170, 22);
      this.lblSessionOpen.Name = "lblSessionOpen";
      this.lblSessionOpen.Size = new System.Drawing.Size(39, 20);
      this.lblSessionOpen.TabIndex = 1;
      this.lblSessionOpen.Text = "???";
      // 
      // lblSessionOpenTxt
      // 
      this.lblSessionOpenTxt.AutoSize = true;
      this.lblSessionOpenTxt.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblSessionOpenTxt.Location = new System.Drawing.Point(6, 22);
      this.lblSessionOpenTxt.Name = "lblSessionOpenTxt";
      this.lblSessionOpenTxt.Size = new System.Drawing.Size(66, 19);
      this.lblSessionOpenTxt.TabIndex = 1;
      this.lblSessionOpenTxt.Text = "Смена:";
      // 
      // lblWatchSumTxt
      // 
      this.lblWatchSumTxt.AutoSize = true;
      this.lblWatchSumTxt.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblWatchSumTxt.Location = new System.Drawing.Point(6, 166);
      this.lblWatchSumTxt.Name = "lblWatchSumTxt";
      this.lblWatchSumTxt.Size = new System.Drawing.Size(153, 19);
      this.lblWatchSumTxt.TabIndex = 1;
      this.lblWatchSumTxt.Text = "Выручка за смену";
      // 
      // lblMoneyInCashTxt
      // 
      this.lblMoneyInCashTxt.AutoSize = true;
      this.lblMoneyInCashTxt.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblMoneyInCashTxt.Location = new System.Drawing.Point(6, 130);
      this.lblMoneyInCashTxt.Name = "lblMoneyInCashTxt";
      this.lblMoneyInCashTxt.Size = new System.Drawing.Size(123, 19);
      this.lblMoneyInCashTxt.TabIndex = 0;
      this.lblMoneyInCashTxt.Text = "Сумма в кассе";
      // 
      // btnOpenSession
      // 
      this.btnOpenSession.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.btnOpenSession.Location = new System.Drawing.Point(12, 58);
      this.btnOpenSession.Name = "btnOpenSession";
      this.btnOpenSession.Size = new System.Drawing.Size(180, 30);
      this.btnOpenSession.TabIndex = 0;
      this.btnOpenSession.Text = "Открыть смену";
      this.btnOpenSession.UseVisualStyleBackColor = true;
      this.btnOpenSession.Click += new System.EventHandler(this.btnOpenSession_Click);
      // 
      // btnXReport
      // 
      this.btnXReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.btnXReport.Location = new System.Drawing.Point(12, 130);
      this.btnXReport.Name = "btnXReport";
      this.btnXReport.Size = new System.Drawing.Size(180, 30);
      this.btnXReport.TabIndex = 0;
      this.btnXReport.Text = "Х Отчет";
      this.btnXReport.UseVisualStyleBackColor = true;
      this.btnXReport.Click += new System.EventHandler(this.btnXReport_Click);
      // 
      // btnMoneyToCash
      // 
      this.btnMoneyToCash.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.btnMoneyToCash.Location = new System.Drawing.Point(12, 166);
      this.btnMoneyToCash.Name = "btnMoneyToCash";
      this.btnMoneyToCash.Size = new System.Drawing.Size(180, 30);
      this.btnMoneyToCash.TabIndex = 0;
      this.btnMoneyToCash.Text = "Внесение денег";
      this.btnMoneyToCash.UseVisualStyleBackColor = true;
      this.btnMoneyToCash.Click += new System.EventHandler(this.btnMoneyToCash_Click);
      // 
      // btnEncashment
      // 
      this.btnEncashment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.btnEncashment.Location = new System.Drawing.Point(12, 202);
      this.btnEncashment.Name = "btnEncashment";
      this.btnEncashment.Size = new System.Drawing.Size(180, 30);
      this.btnEncashment.TabIndex = 0;
      this.btnEncashment.Text = "Инкассация";
      this.btnEncashment.UseVisualStyleBackColor = true;
      this.btnEncashment.Click += new System.EventHandler(this.btnEncashment_Click);
      // 
      // btnClose
      // 
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Location = new System.Drawing.Point(511, 12);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(88, 23);
      this.btnClose.TabIndex = 0;
      this.btnClose.Text = "Закрыть";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // gbMessage
      // 
      this.gbMessage.Controls.Add(this.tbKKMMessage);
      this.gbMessage.Location = new System.Drawing.Point(12, 238);
      this.gbMessage.Name = "gbMessage";
      this.gbMessage.Size = new System.Drawing.Size(587, 168);
      this.gbMessage.TabIndex = 3;
      this.gbMessage.TabStop = false;
      this.gbMessage.Text = "Состояние ККМ";
      // 
      // tbKKMMessage
      // 
      this.tbKKMMessage.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.tbKKMMessage.Location = new System.Drawing.Point(6, 19);
      this.tbKKMMessage.Multiline = true;
      this.tbKKMMessage.Name = "tbKKMMessage";
      this.tbKKMMessage.ReadOnly = true;
      this.tbKKMMessage.Size = new System.Drawing.Size(575, 143);
      this.tbKKMMessage.TabIndex = 1;
      // 
      // CashControl
      // 
      this.AcceptButton = this.btnClose;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(611, 417);
      this.Controls.Add(this.gbMessage);
      this.Controls.Add(this.gbProperty);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnEncashment);
      this.Controls.Add(this.btnMoneyToCash);
      this.Controls.Add(this.btnXReport);
      this.Controls.Add(this.btnOpenSession);
      this.Controls.Add(this.btnSessionClose);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CashControl";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Операции с ККМ";
      this.Load += new System.EventHandler(this.CashControl_Load);
      this.gbProperty.ResumeLayout(false);
      this.gbProperty.PerformLayout();
      this.gbMessage.ResumeLayout(false);
      this.gbMessage.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnSessionClose;
    private System.Windows.Forms.GroupBox gbProperty;
    private System.Windows.Forms.Label lblWatchSum;
    private System.Windows.Forms.Label lblMoneyInCash;
    private System.Windows.Forms.Label lblWatchSumTxt;
    private System.Windows.Forms.Label lblMoneyInCashTxt;
    private System.Windows.Forms.Label lblSessionOpen;
    private System.Windows.Forms.Label lblSessionOpenTxt;
    private System.Windows.Forms.Button btnOpenSession;
    private System.Windows.Forms.Button btnXReport;
    private System.Windows.Forms.Button btnMoneyToCash;
    private System.Windows.Forms.Button btnEncashment;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.GroupBox gbMessage;
    private System.Windows.Forms.TextBox tbKKMMessage;
  }
}