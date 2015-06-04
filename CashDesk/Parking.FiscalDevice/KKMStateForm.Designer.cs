namespace Parking.FiscalDevice
{
  partial class KKMStateForm
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
      this.gbMessage = new System.Windows.Forms.GroupBox();
      this.tbKKMMessage = new System.Windows.Forms.TextBox();
      this.btnClose = new System.Windows.Forms.Button();
      this.gbMessage.SuspendLayout();
      this.SuspendLayout();
      // 
      // gbMessage
      // 
      this.gbMessage.Controls.Add(this.tbKKMMessage);
      this.gbMessage.Location = new System.Drawing.Point(12, 12);
      this.gbMessage.Name = "gbMessage";
      this.gbMessage.Size = new System.Drawing.Size(496, 158);
      this.gbMessage.TabIndex = 2;
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
      this.tbKKMMessage.Size = new System.Drawing.Size(484, 133);
      this.tbKKMMessage.TabIndex = 1;
      this.tbKKMMessage.Text = "Обмен данными с ККМ...";
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Location = new System.Drawing.Point(433, 176);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Закрыть";
      this.btnClose.UseVisualStyleBackColor = true;
      // 
      // KKMStateForm
      // 
      this.AcceptButton = this.btnClose;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(520, 211);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.gbMessage);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "KKMStateForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Сообщения от ККМ";
      this.Load += new System.EventHandler(this.KKMStateForm_Load);
      this.gbMessage.ResumeLayout(false);
      this.gbMessage.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox gbMessage;
    private System.Windows.Forms.TextBox tbKKMMessage;
    private System.Windows.Forms.Button btnClose;
  }
}