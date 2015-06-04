namespace Parking.CashDesk
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
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblKKMPortNum = new System.Windows.Forms.Label();
            this.edKKMPortNum = new System.Windows.Forms.TextBox();
            this.lblCashNumbHint = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.edCashNum = new System.Windows.Forms.TextBox();
            this.lblCashNumHint = new System.Windows.Forms.Label();
            this.cbDisplayState = new System.Windows.Forms.CheckBox();
            this.lblDisplaStateHint = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.edDisplayPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnTariffPenaltyRead = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btnTariffPenaltyShow = new System.Windows.Forms.Button();
            this.imgShow = new System.Windows.Forms.ImageList(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.btnChangeCashPass = new System.Windows.Forms.Button();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnTariffMetroRead = new System.Windows.Forms.Button();
            this.tableMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblTariffDiscount = new System.Windows.Forms.Label();
            this.panelTariffPenalty = new System.Windows.Forms.Panel();
            this.panelTariffMetro = new System.Windows.Forms.Panel();
            this.btnTariffMetroShow = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.chbLogOn = new System.Windows.Forms.CheckBox();
            this.tableMain.SuspendLayout();
            this.panelTariffPenalty.SuspendLayout();
            this.panelTariffMetro.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(439, 13);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(358, 13);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "ОК";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblKKMPortNum
            // 
            this.lblKKMPortNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKKMPortNum.AutoSize = true;
            this.lblKKMPortNum.Location = new System.Drawing.Point(13, 16);
            this.lblKKMPortNum.Name = "lblKKMPortNum";
            this.lblKKMPortNum.Size = new System.Drawing.Size(294, 13);
            this.lblKKMPortNum.TabIndex = 2;
            this.lblKKMPortNum.Text = "Порт подключения ККМ";
            // 
            // edKKMPortNum
            // 
            this.edKKMPortNum.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.edKKMPortNum.Location = new System.Drawing.Point(313, 13);
            this.edKKMPortNum.Name = "edKKMPortNum";
            this.edKKMPortNum.Size = new System.Drawing.Size(79, 20);
            this.edKKMPortNum.TabIndex = 3;
            this.edKKMPortNum.Text = "1";
            // 
            // lblCashNumbHint
            // 
            this.lblCashNumbHint.AutoSize = true;
            this.tableMain.SetColumnSpan(this.lblCashNumbHint, 2);
            this.lblCashNumbHint.ForeColor = System.Drawing.Color.Gray;
            this.lblCashNumbHint.Location = new System.Drawing.Point(13, 36);
            this.lblCashNumbHint.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.lblCashNumbHint.Name = "lblCashNumbHint";
            this.lblCashNumbHint.Size = new System.Drawing.Size(393, 13);
            this.lblCashNumbHint.TabIndex = 4;
            this.lblCashNumbHint.Text = "(Введите порт подключения фискального регистратора. Например: 1 или 2)";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(294, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Номер кассы";
            // 
            // edCashNum
            // 
            this.edCashNum.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.edCashNum.Location = new System.Drawing.Point(313, 157);
            this.edCashNum.Name = "edCashNum";
            this.edCashNum.Size = new System.Drawing.Size(79, 20);
            this.edCashNum.TabIndex = 3;
            // 
            // lblCashNumHint
            // 
            this.lblCashNumHint.AutoSize = true;
            this.tableMain.SetColumnSpan(this.lblCashNumHint, 2);
            this.lblCashNumHint.ForeColor = System.Drawing.Color.Gray;
            this.lblCashNumHint.Location = new System.Drawing.Point(13, 180);
            this.lblCashNumHint.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.lblCashNumHint.Name = "lblCashNumHint";
            this.lblCashNumHint.Size = new System.Drawing.Size(404, 13);
            this.lblCashNumHint.TabIndex = 4;
            this.lblCashNumHint.Text = "(Введите уникльный номер кассы. Используется при печати чека и в отчетах)";
            // 
            // cbDisplayState
            // 
            this.cbDisplayState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDisplayState.AutoSize = true;
            this.cbDisplayState.Location = new System.Drawing.Point(13, 62);
            this.cbDisplayState.Name = "cbDisplayState";
            this.cbDisplayState.Size = new System.Drawing.Size(294, 17);
            this.cbDisplayState.TabIndex = 5;
            this.cbDisplayState.Text = "Дублировать информацию на внешнем индикаторе?";
            this.cbDisplayState.UseVisualStyleBackColor = true;
            this.cbDisplayState.CheckedChanged += new System.EventHandler(this.chkDisplayState_CheckedChanged);
            // 
            // lblDisplaStateHint
            // 
            this.lblDisplaStateHint.AutoSize = true;
            this.tableMain.SetColumnSpan(this.lblDisplaStateHint, 2);
            this.lblDisplaStateHint.ForeColor = System.Drawing.Color.Gray;
            this.lblDisplaStateHint.Location = new System.Drawing.Point(13, 82);
            this.lblDisplaStateHint.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.lblDisplaStateHint.Name = "lblDisplaStateHint";
            this.lblDisplaStateHint.Size = new System.Drawing.Size(348, 13);
            this.lblDisplaStateHint.TabIndex = 4;
            this.lblDisplaStateHint.Text = "(Поставьте галочку, если к кассе подключено внешней индикатор)";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(294, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Порт подключения дисплея";
            // 
            // edDisplayPort
            // 
            this.edDisplayPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.edDisplayPort.Enabled = false;
            this.edDisplayPort.Location = new System.Drawing.Point(313, 108);
            this.edDisplayPort.Name = "edDisplayPort";
            this.edDisplayPort.Size = new System.Drawing.Size(79, 20);
            this.edDisplayPort.TabIndex = 3;
            this.edDisplayPort.Text = "2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.tableMain.SetColumnSpan(this.label4, 2);
            this.label4.ForeColor = System.Drawing.Color.Gray;
            this.label4.Location = new System.Drawing.Point(13, 131);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(348, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "(Введите порт подключения внешнего дисплея. Например: 1 или 2)";
            // 
            // btnTariffPenaltyRead
            // 
            this.btnTariffPenaltyRead.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnTariffPenaltyRead.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnTariffPenaltyRead.Location = new System.Drawing.Point(0, 3);
            this.btnTariffPenaltyRead.Name = "btnTariffPenaltyRead";
            this.btnTariffPenaltyRead.Size = new System.Drawing.Size(422, 23);
            this.btnTariffPenaltyRead.TabIndex = 6;
            this.btnTariffPenaltyRead.Text = "Загрузить ключи и штрафной тариф с Мастер-карты";
            this.btnTariffPenaltyRead.UseVisualStyleBackColor = true;
            this.btnTariffPenaltyRead.Click += new System.EventHandler(this.btnTariffPenaltyRead_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.tableMain.SetColumnSpan(this.label2, 2);
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(13, 288);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(471, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "(Нажмите кнопку при первом включении кассы для загрузки ключей и штрафного тарифа" +
    ")";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.tableMain.SetColumnSpan(this.label6, 2);
            this.label6.ForeColor = System.Drawing.Color.Gray;
            this.label6.Location = new System.Drawing.Point(13, 398);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(169, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "(Пароль доступа в данное окно)";
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableMain.SetColumnSpan(this.btnChangePassword, 2);
            this.btnChangePassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnChangePassword.Location = new System.Drawing.Point(13, 372);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(422, 23);
            this.btnChangePassword.TabIndex = 6;
            this.btnChangePassword.Text = "Сменить пароль на доступ к настройкам";
            this.btnChangePassword.UseVisualStyleBackColor = true;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(35, 13);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(195, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "Вернуть значения по умолчанию";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            // 
            // btnTariffPenaltyShow
            // 
            this.btnTariffPenaltyShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnTariffPenaltyShow.ImageList = this.imgShow;
            this.btnTariffPenaltyShow.Location = new System.Drawing.Point(473, 3);
            this.btnTariffPenaltyShow.Name = "btnTariffPenaltyShow";
            this.btnTariffPenaltyShow.Size = new System.Drawing.Size(25, 23);
            this.btnTariffPenaltyShow.TabIndex = 6;
            this.btnTariffPenaltyShow.UseVisualStyleBackColor = true;
            this.btnTariffPenaltyShow.Click += new System.EventHandler(this.btnTariffShow_Click);
            // 
            // imgShow
            // 
            this.imgShow.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgShow.ImageSize = new System.Drawing.Size(16, 16);
            this.imgShow.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.tableMain.SetColumnSpan(this.label7, 2);
            this.label7.ForeColor = System.Drawing.Color.Gray;
            this.label7.Location = new System.Drawing.Point(13, 450);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(460, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "(Пароль доступа к операциям открытия/закрытия смени, внесения денег и инкассации)" +
    "";
            // 
            // btnChangeCashPass
            // 
            this.btnChangeCashPass.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableMain.SetColumnSpan(this.btnChangeCashPass, 2);
            this.btnChangeCashPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnChangeCashPass.Location = new System.Drawing.Point(13, 424);
            this.btnChangeCashPass.Name = "btnChangeCashPass";
            this.btnChangeCashPass.Size = new System.Drawing.Size(422, 23);
            this.btnChangeCashPass.TabIndex = 6;
            this.btnChangeCashPass.Text = "Сменить пароль на доступ к кассовым операциям";
            this.btnChangeCashPass.UseVisualStyleBackColor = true;
            this.btnChangeCashPass.Click += new System.EventHandler(this.btnChangeCashPass_Click);
            // 
            // cboLanguage
            // 
            this.cboLanguage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Items.AddRange(new object[] {
            "Азербайджанский",
            "Русский"});
            this.cboLanguage.Location = new System.Drawing.Point(313, 206);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(198, 21);
            this.cboLanguage.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 210);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(294, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Выберите язык";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.tableMain.SetColumnSpan(this.label9, 2);
            this.label9.ForeColor = System.Drawing.Color.Gray;
            this.label9.Location = new System.Drawing.Point(13, 230);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(167, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "(Выберите язык рабты с табло)";
            // 
            // btnTariffMetroRead
            // 
            this.btnTariffMetroRead.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnTariffMetroRead.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnTariffMetroRead.Location = new System.Drawing.Point(3, 3);
            this.btnTariffMetroRead.Name = "btnTariffMetroRead";
            this.btnTariffMetroRead.Size = new System.Drawing.Size(419, 23);
            this.btnTariffMetroRead.TabIndex = 10;
            this.btnTariffMetroRead.Text = "Загрузить тарифы метро из файлов";
            this.btnTariffMetroRead.UseVisualStyleBackColor = true;
            this.btnTariffMetroRead.Click += new System.EventHandler(this.btnTariffMetroRead_Click);
            // 
            // tableMain
            // 
            this.tableMain.ColumnCount = 2;
            this.tableMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMain.Controls.Add(this.edKKMPortNum, 1, 0);
            this.tableMain.Controls.Add(this.lblKKMPortNum, 0, 0);
            this.tableMain.Controls.Add(this.btnChangeCashPass, 0, 16);
            this.tableMain.Controls.Add(this.label9, 0, 9);
            this.tableMain.Controls.Add(this.btnChangePassword, 0, 14);
            this.tableMain.Controls.Add(this.label6, 0, 15);
            this.tableMain.Controls.Add(this.lblCashNumbHint, 0, 1);
            this.tableMain.Controls.Add(this.cboLanguage, 1, 8);
            this.tableMain.Controls.Add(this.label8, 0, 8);
            this.tableMain.Controls.Add(this.cbDisplayState, 0, 2);
            this.tableMain.Controls.Add(this.lblDisplaStateHint, 0, 3);
            this.tableMain.Controls.Add(this.label2, 0, 11);
            this.tableMain.Controls.Add(this.edDisplayPort, 1, 4);
            this.tableMain.Controls.Add(this.label3, 0, 4);
            this.tableMain.Controls.Add(this.label4, 0, 5);
            this.tableMain.Controls.Add(this.edCashNum, 1, 6);
            this.tableMain.Controls.Add(this.label1, 0, 6);
            this.tableMain.Controls.Add(this.lblCashNumHint, 0, 7);
            this.tableMain.Controls.Add(this.lblTariffDiscount, 0, 13);
            this.tableMain.Controls.Add(this.label7, 0, 17);
            this.tableMain.Controls.Add(this.panelTariffPenalty, 0, 10);
            this.tableMain.Controls.Add(this.panelTariffMetro, 0, 12);
            this.tableMain.Controls.Add(this.chbLogOn, 1, 2);
            this.tableMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMain.Location = new System.Drawing.Point(0, 0);
            this.tableMain.Name = "tableMain";
            this.tableMain.Padding = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.tableMain.RowCount = 18;
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMain.Size = new System.Drawing.Size(527, 474);
            this.tableMain.TabIndex = 11;
            // 
            // lblTariffDiscount
            // 
            this.lblTariffDiscount.AutoSize = true;
            this.tableMain.SetColumnSpan(this.lblTariffDiscount, 2);
            this.lblTariffDiscount.ForeColor = System.Drawing.Color.Gray;
            this.lblTariffDiscount.Location = new System.Drawing.Point(13, 346);
            this.lblTariffDiscount.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.lblTariffDiscount.Name = "lblTariffDiscount";
            this.lblTariffDiscount.Size = new System.Drawing.Size(365, 13);
            this.lblTariffDiscount.TabIndex = 4;
            this.lblTariffDiscount.Text = "(Нажмите при первом включении кассы для загрузки тарифов метро)";
            // 
            // panelTariffPenalty
            // 
            this.panelTariffPenalty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableMain.SetColumnSpan(this.panelTariffPenalty, 2);
            this.panelTariffPenalty.Controls.Add(this.btnTariffPenaltyRead);
            this.panelTariffPenalty.Controls.Add(this.btnTariffPenaltyShow);
            this.panelTariffPenalty.Location = new System.Drawing.Point(13, 256);
            this.panelTariffPenalty.Name = "panelTariffPenalty";
            this.panelTariffPenalty.Size = new System.Drawing.Size(501, 29);
            this.panelTariffPenalty.TabIndex = 11;
            // 
            // panelTariffMetro
            // 
            this.panelTariffMetro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableMain.SetColumnSpan(this.panelTariffMetro, 2);
            this.panelTariffMetro.Controls.Add(this.btnTariffMetroRead);
            this.panelTariffMetro.Controls.Add(this.btnTariffMetroShow);
            this.panelTariffMetro.Location = new System.Drawing.Point(13, 314);
            this.panelTariffMetro.Name = "panelTariffMetro";
            this.panelTariffMetro.Size = new System.Drawing.Size(501, 29);
            this.panelTariffMetro.TabIndex = 12;
            // 
            // btnTariffMetroShow
            // 
            this.btnTariffMetroShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnTariffMetroShow.ImageList = this.imgShow;
            this.btnTariffMetroShow.Location = new System.Drawing.Point(473, 3);
            this.btnTariffMetroShow.Name = "btnTariffMetroShow";
            this.btnTariffMetroShow.Size = new System.Drawing.Size(25, 23);
            this.btnTariffMetroShow.TabIndex = 6;
            this.btnTariffMetroShow.UseVisualStyleBackColor = true;
            this.btnTariffMetroShow.Visible = false;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.button3);
            this.panelBottom.Controls.Add(this.btnCancel);
            this.panelBottom.Controls.Add(this.btnOK);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 474);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(527, 49);
            this.panelBottom.TabIndex = 12;
            // 
            // chbLogOn
            // 
            this.chbLogOn.AutoSize = true;
            this.chbLogOn.Location = new System.Drawing.Point(313, 62);
            this.chbLogOn.Name = "chbLogOn";
            this.chbLogOn.Size = new System.Drawing.Size(121, 17);
            this.chbLogOn.TabIndex = 13;
            this.chbLogOn.Text = "Запись в лог-файл";
            this.chbLogOn.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(527, 523);
            this.Controls.Add(this.tableMain);
            this.Controls.Add(this.panelBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Конфигурация";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.tableMain.ResumeLayout(false);
            this.tableMain.PerformLayout();
            this.panelTariffPenalty.ResumeLayout(false);
            this.panelTariffMetro.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Label lblKKMPortNum;
    private System.Windows.Forms.TextBox edKKMPortNum;
    private System.Windows.Forms.Label lblCashNumbHint;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox edCashNum;
    private System.Windows.Forms.Label lblCashNumHint;
    private System.Windows.Forms.CheckBox cbDisplayState;
    private System.Windows.Forms.Label lblDisplaStateHint;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox edDisplayPort;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button btnTariffPenaltyRead;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Button btnChangePassword;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.Button btnTariffPenaltyShow;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Button btnChangeCashPass;
    private System.Windows.Forms.ComboBox cboLanguage;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TableLayoutPanel tableMain;
    private System.Windows.Forms.Button btnTariffMetroRead;
    private System.Windows.Forms.Label lblTariffDiscount;
    private System.Windows.Forms.Panel panelBottom;
    private System.Windows.Forms.Panel panelTariffPenalty;
    private System.Windows.Forms.Panel panelTariffMetro;
    private System.Windows.Forms.ImageList imgShow;
    private System.Windows.Forms.Button btnTariffMetroShow;
    private System.Windows.Forms.CheckBox chbLogOn;
  }
}