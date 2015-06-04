namespace Parking.CashDesk
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
      this.ShowTimeTimer = new System.Windows.Forms.Timer(this.components);
      this.panel3 = new System.Windows.Forms.Panel();
      this.btnSetup = new System.Windows.Forms.Button();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.btnAbout = new System.Windows.Forms.Button();
      this.btnExit = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.lblFiscalState = new System.Windows.Forms.Label();
      this.lblPenaltyCount = new System.Windows.Forms.Label();
      this.lblReaderState = new System.Windows.Forms.Label();
      this.lblReader = new System.Windows.Forms.Label();
      this.tslblStatusNetwork = new System.Windows.Forms.Label();
      this.tslblStatusSent = new System.Windows.Forms.Label();
      this.tslblStatusReceived = new System.Windows.Forms.Label();
      this.lblCaption = new System.Windows.Forms.Label();
      this.gbSumma = new System.Windows.Forms.GroupBox();
      this.lblECash = new System.Windows.Forms.Label();
      this.lblDebt = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.lblDebtText = new System.Windows.Forms.Label();
      this.lblPaid = new System.Windows.Forms.Label();
      this.lblPaidText = new System.Windows.Forms.Label();
      this.lblSumma = new System.Windows.Forms.Label();
      this.gbOperDeskription = new System.Windows.Forms.GroupBox();
      this.lblOperationDesc = new System.Windows.Forms.Label();
      this.gbCurrentTime = new System.Windows.Forms.GroupBox();
      this.lblTime = new System.Windows.Forms.Label();
      this.gbTimeInOut = new System.Windows.Forms.GroupBox();
      this.dtpOut = new System.Windows.Forms.DateTimePicker();
      this.lblExitTimeLimit = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.lblTimePaid = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.lblTimeInParking = new System.Windows.Forms.Label();
      this.lblParkingTimeText = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.lblTimeIn = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.btnPay = new System.Windows.Forms.Button();
      this.btnRandomPayment = new System.Windows.Forms.Button();
      this.btnCashOper = new System.Windows.Forms.Button();
      this.btnRepayment = new System.Windows.Forms.Button();
      this.btnPinalty = new System.Windows.Forms.Button();
      this.btn_eCash = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.timerIndicator = new System.Windows.Forms.Timer(this.components);
      this.panel3.SuspendLayout();
      this.panel2.SuspendLayout();
      this.gbSumma.SuspendLayout();
      this.gbOperDeskription.SuspendLayout();
      this.gbCurrentTime.SuspendLayout();
      this.gbTimeInOut.SuspendLayout();
      this.SuspendLayout();
      // 
      // ShowTimeTimer
      // 
      this.ShowTimeTimer.Interval = 1000;
      this.ShowTimeTimer.Tick += new System.EventHandler(this.ShowTimeTimer_Tick);
      // 
      // panel3
      // 
      this.panel3.BackgroundImage = global::Parking.CashDesk.Properties.Resources.right;
      this.panel3.Controls.Add(this.btnSetup);
      this.panel3.Controls.Add(this.btnAbout);
      this.panel3.Controls.Add(this.btnExit);
      this.panel3.Location = new System.Drawing.Point(908, 0);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(86, 578);
      this.panel3.TabIndex = 8;
      this.panel3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
      this.panel3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
      this.panel3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
      // 
      // btnSetup
      // 
      this.btnSetup.BackColor = System.Drawing.Color.Transparent;
      this.btnSetup.FlatAppearance.BorderSize = 0;
      this.btnSetup.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
      this.btnSetup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnSetup.ImageList = this.imageList;
      this.btnSetup.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnSetup.Location = new System.Drawing.Point(47, 189);
      this.btnSetup.Name = "btnSetup";
      this.btnSetup.Size = new System.Drawing.Size(30, 30);
      this.btnSetup.TabIndex = 0;
      this.btnSetup.UseVisualStyleBackColor = true;
      this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageList.ImageSize = new System.Drawing.Size(24, 24);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // btnAbout
      // 
      this.btnAbout.BackColor = System.Drawing.Color.Transparent;
      this.btnAbout.FlatAppearance.BorderSize = 0;
      this.btnAbout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
      this.btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnAbout.ImageList = this.imageList;
      this.btnAbout.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnAbout.Location = new System.Drawing.Point(47, 239);
      this.btnAbout.Name = "btnAbout";
      this.btnAbout.Size = new System.Drawing.Size(30, 30);
      this.btnAbout.TabIndex = 0;
      this.btnAbout.UseVisualStyleBackColor = true;
      this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
      // 
      // btnExit
      // 
      this.btnExit.BackColor = System.Drawing.Color.Transparent;
      this.btnExit.FlatAppearance.BorderSize = 0;
      this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
      this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnExit.ImageList = this.imageList;
      this.btnExit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnExit.Location = new System.Drawing.Point(47, 141);
      this.btnExit.Name = "btnExit";
      this.btnExit.Size = new System.Drawing.Size(30, 30);
      this.btnExit.TabIndex = 0;
      this.btnExit.UseVisualStyleBackColor = true;
      this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
      // 
      // panel2
      // 
      this.panel2.BackgroundImage = global::Parking.CashDesk.Properties.Resources.fon;
      this.panel2.Controls.Add(this.lblFiscalState);
      this.panel2.Controls.Add(this.lblPenaltyCount);
      this.panel2.Controls.Add(this.lblReaderState);
      this.panel2.Controls.Add(this.lblReader);
      this.panel2.Controls.Add(this.tslblStatusNetwork);
      this.panel2.Controls.Add(this.tslblStatusSent);
      this.panel2.Controls.Add(this.tslblStatusReceived);
      this.panel2.Controls.Add(this.lblCaption);
      this.panel2.Controls.Add(this.gbSumma);
      this.panel2.Controls.Add(this.gbOperDeskription);
      this.panel2.Controls.Add(this.gbCurrentTime);
      this.panel2.Controls.Add(this.gbTimeInOut);
      this.panel2.Controls.Add(this.btnPay);
      this.panel2.Controls.Add(this.btnRandomPayment);
      this.panel2.Controls.Add(this.btnCashOper);
      this.panel2.Controls.Add(this.btnRepayment);
      this.panel2.Controls.Add(this.btnPinalty);
      this.panel2.Controls.Add(this.btn_eCash);
      this.panel2.Location = new System.Drawing.Point(86, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(822, 578);
      this.panel2.TabIndex = 8;
      this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
      this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
      this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
      // 
      // lblFiscalState
      // 
      this.lblFiscalState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblFiscalState.AutoSize = true;
      this.lblFiscalState.BackColor = System.Drawing.Color.Transparent;
      this.lblFiscalState.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblFiscalState.Location = new System.Drawing.Point(357, 551);
      this.lblFiscalState.Name = "lblFiscalState";
      this.lblFiscalState.Size = new System.Drawing.Size(25, 16);
      this.lblFiscalState.TabIndex = 6;
      this.lblFiscalState.Text = "ФР";
      // 
      // lblPenaltyCount
      // 
      this.lblPenaltyCount.AutoSize = true;
      this.lblPenaltyCount.BackColor = System.Drawing.Color.Transparent;
      this.lblPenaltyCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
      this.lblPenaltyCount.ForeColor = System.Drawing.Color.CadetBlue;
      this.lblPenaltyCount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblPenaltyCount.Location = new System.Drawing.Point(260, 498);
      this.lblPenaltyCount.Name = "lblPenaltyCount";
      this.lblPenaltyCount.Size = new System.Drawing.Size(18, 20);
      this.lblPenaltyCount.TabIndex = 0;
      this.lblPenaltyCount.Text = "0";
      // 
      // lblReaderState
      // 
      this.lblReaderState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblReaderState.AutoSize = true;
      this.lblReaderState.BackColor = System.Drawing.Color.Transparent;
      this.lblReaderState.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblReaderState.Location = new System.Drawing.Point(13, 551);
      this.lblReaderState.Name = "lblReaderState";
      this.lblReaderState.Size = new System.Drawing.Size(86, 16);
      this.lblReaderState.TabIndex = 6;
      this.lblReaderState.Text = "Считыватель";
      // 
      // lblReader
      // 
      this.lblReader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblReader.AutoSize = true;
      this.lblReader.BackColor = System.Drawing.Color.Transparent;
      this.lblReader.Location = new System.Drawing.Point(13, 550);
      this.lblReader.Name = "lblReader";
      this.lblReader.Size = new System.Drawing.Size(73, 13);
      this.lblReader.TabIndex = 6;
      this.lblReader.Text = "Считыватель";
      // 
      // tslblStatusNetwork
      // 
      this.tslblStatusNetwork.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.tslblStatusNetwork.AutoSize = true;
      this.tslblStatusNetwork.BackColor = System.Drawing.Color.Transparent;
      this.tslblStatusNetwork.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.tslblStatusNetwork.Location = new System.Drawing.Point(695, 551);
      this.tslblStatusNetwork.Name = "tslblStatusNetwork";
      this.tslblStatusNetwork.Size = new System.Drawing.Size(36, 16);
      this.tslblStatusNetwork.TabIndex = 6;
      this.tslblStatusNetwork.Text = "Сеть";
      // 
      // tslblStatusSent
      // 
      this.tslblStatusSent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.tslblStatusSent.BackColor = System.Drawing.Color.Transparent;
      this.tslblStatusSent.Image = global::Parking.CashDesk.Properties.Resources.idle;
      this.tslblStatusSent.Location = new System.Drawing.Point(775, 550);
      this.tslblStatusSent.Name = "tslblStatusSent";
      this.tslblStatusSent.Size = new System.Drawing.Size(19, 19);
      this.tslblStatusSent.TabIndex = 5;
      // 
      // tslblStatusReceived
      // 
      this.tslblStatusReceived.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.tslblStatusReceived.BackColor = System.Drawing.Color.Transparent;
      this.tslblStatusReceived.Image = global::Parking.CashDesk.Properties.Resources.idle;
      this.tslblStatusReceived.Location = new System.Drawing.Point(741, 550);
      this.tslblStatusReceived.Name = "tslblStatusReceived";
      this.tslblStatusReceived.Size = new System.Drawing.Size(19, 19);
      this.tslblStatusReceived.TabIndex = 5;
      // 
      // lblCaption
      // 
      this.lblCaption.BackColor = System.Drawing.Color.Transparent;
      this.lblCaption.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblCaption.ForeColor = System.Drawing.Color.Black;
      this.lblCaption.Location = new System.Drawing.Point(4, 12);
      this.lblCaption.Name = "lblCaption";
      this.lblCaption.Size = new System.Drawing.Size(814, 26);
      this.lblCaption.TabIndex = 4;
      this.lblCaption.Text = "АРМ Ручная касса";
      this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.lblCaption.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
      this.lblCaption.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
      this.lblCaption.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
      // 
      // gbSumma
      // 
      this.gbSumma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.gbSumma.BackColor = System.Drawing.Color.Transparent;
      this.gbSumma.Controls.Add(this.lblECash);
      this.gbSumma.Controls.Add(this.lblDebt);
      this.gbSumma.Controls.Add(this.label3);
      this.gbSumma.Controls.Add(this.lblDebtText);
      this.gbSumma.Controls.Add(this.lblPaid);
      this.gbSumma.Controls.Add(this.lblPaidText);
      this.gbSumma.Controls.Add(this.lblSumma);
      this.gbSumma.Font = new System.Drawing.Font("Tahoma", 12F);
      this.gbSumma.ForeColor = System.Drawing.Color.CadetBlue;
      this.gbSumma.Location = new System.Drawing.Point(436, 120);
      this.gbSumma.Name = "gbSumma";
      this.gbSumma.Size = new System.Drawing.Size(380, 155);
      this.gbSumma.TabIndex = 1;
      this.gbSumma.TabStop = false;
      this.gbSumma.Text = "К оплате";
      // 
      // lblECash
      // 
      this.lblECash.AutoSize = true;
      this.lblECash.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblECash.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblECash.Location = new System.Drawing.Point(156, 128);
      this.lblECash.Name = "lblECash";
      this.lblECash.Size = new System.Drawing.Size(20, 23);
      this.lblECash.TabIndex = 2;
      this.lblECash.Text = "0";
      // 
      // lblDebt
      // 
      this.lblDebt.AutoSize = true;
      this.lblDebt.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblDebt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblDebt.Location = new System.Drawing.Point(156, 105);
      this.lblDebt.Name = "lblDebt";
      this.lblDebt.Size = new System.Drawing.Size(20, 23);
      this.lblDebt.TabIndex = 2;
      this.lblDebt.Text = "0";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label3.Location = new System.Drawing.Point(13, 128);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(123, 23);
      this.label3.TabIndex = 1;
      this.label3.Text = "эл. кошелек:";
      // 
      // lblDebtText
      // 
      this.lblDebtText.AutoSize = true;
      this.lblDebtText.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblDebtText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblDebtText.Location = new System.Drawing.Point(13, 105);
      this.lblDebtText.Name = "lblDebtText";
      this.lblDebtText.Size = new System.Drawing.Size(132, 23);
      this.lblDebtText.TabIndex = 1;
      this.lblDebtText.Text = "Задолжность:";
      // 
      // lblPaid
      // 
      this.lblPaid.AutoSize = true;
      this.lblPaid.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblPaid.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblPaid.Location = new System.Drawing.Point(156, 82);
      this.lblPaid.Name = "lblPaid";
      this.lblPaid.Size = new System.Drawing.Size(20, 23);
      this.lblPaid.TabIndex = 2;
      this.lblPaid.Text = "0";
      // 
      // lblPaidText
      // 
      this.lblPaidText.AutoSize = true;
      this.lblPaidText.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblPaidText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblPaidText.Location = new System.Drawing.Point(13, 82);
      this.lblPaidText.Name = "lblPaidText";
      this.lblPaidText.Size = new System.Drawing.Size(141, 23);
      this.lblPaidText.TabIndex = 1;
      this.lblPaidText.Text = "Уже оплачено:";
      // 
      // lblSumma
      // 
      this.lblSumma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblSumma.AutoSize = true;
      this.lblSumma.Font = new System.Drawing.Font("Arial", 36F, System.Drawing.FontStyle.Bold);
      this.lblSumma.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
      this.lblSumma.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblSumma.Location = new System.Drawing.Point(7, 23);
      this.lblSumma.Name = "lblSumma";
      this.lblSumma.Size = new System.Drawing.Size(51, 56);
      this.lblSumma.TabIndex = 0;
      this.lblSumma.Text = "0";
      this.lblSumma.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // gbOperDeskription
      // 
      this.gbOperDeskription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.gbOperDeskription.BackColor = System.Drawing.Color.Transparent;
      this.gbOperDeskription.Controls.Add(this.lblOperationDesc);
      this.gbOperDeskription.Font = new System.Drawing.Font("Tahoma", 12F);
      this.gbOperDeskription.ForeColor = System.Drawing.Color.CadetBlue;
      this.gbOperDeskription.Location = new System.Drawing.Point(6, 281);
      this.gbOperDeskription.Name = "gbOperDeskription";
      this.gbOperDeskription.Size = new System.Drawing.Size(810, 136);
      this.gbOperDeskription.TabIndex = 1;
      this.gbOperDeskription.TabStop = false;
      this.gbOperDeskription.Text = " Операция ";
      // 
      // lblOperationDesc
      // 
      this.lblOperationDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lblOperationDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
      this.lblOperationDesc.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblOperationDesc.Location = new System.Drawing.Point(6, 23);
      this.lblOperationDesc.Name = "lblOperationDesc";
      this.lblOperationDesc.Size = new System.Drawing.Size(798, 100);
      this.lblOperationDesc.TabIndex = 1;
      // 
      // gbCurrentTime
      // 
      this.gbCurrentTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.gbCurrentTime.BackColor = System.Drawing.Color.Transparent;
      this.gbCurrentTime.Controls.Add(this.lblTime);
      this.gbCurrentTime.Font = new System.Drawing.Font("Tahoma", 12F);
      this.gbCurrentTime.ForeColor = System.Drawing.Color.CadetBlue;
      this.gbCurrentTime.Location = new System.Drawing.Point(436, 59);
      this.gbCurrentTime.Name = "gbCurrentTime";
      this.gbCurrentTime.Size = new System.Drawing.Size(380, 55);
      this.gbCurrentTime.TabIndex = 3;
      this.gbCurrentTime.TabStop = false;
      this.gbCurrentTime.Text = " Текущее время ";
      // 
      // lblTime
      // 
      this.lblTime.AutoSize = true;
      this.lblTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblTime.Location = new System.Drawing.Point(9, 23);
      this.lblTime.Name = "lblTime";
      this.lblTime.Size = new System.Drawing.Size(33, 19);
      this.lblTime.TabIndex = 0;
      this.lblTime.Text = "???";
      // 
      // gbTimeInOut
      // 
      this.gbTimeInOut.BackColor = System.Drawing.Color.Transparent;
      this.gbTimeInOut.Controls.Add(this.dtpOut);
      this.gbTimeInOut.Controls.Add(this.lblExitTimeLimit);
      this.gbTimeInOut.Controls.Add(this.label5);
      this.gbTimeInOut.Controls.Add(this.lblTimePaid);
      this.gbTimeInOut.Controls.Add(this.label4);
      this.gbTimeInOut.Controls.Add(this.lblTimeInParking);
      this.gbTimeInOut.Controls.Add(this.lblParkingTimeText);
      this.gbTimeInOut.Controls.Add(this.label2);
      this.gbTimeInOut.Controls.Add(this.lblTimeIn);
      this.gbTimeInOut.Controls.Add(this.label1);
      this.gbTimeInOut.Font = new System.Drawing.Font("Tahoma", 12F);
      this.gbTimeInOut.ForeColor = System.Drawing.Color.CadetBlue;
      this.gbTimeInOut.Location = new System.Drawing.Point(6, 59);
      this.gbTimeInOut.Name = "gbTimeInOut";
      this.gbTimeInOut.Size = new System.Drawing.Size(424, 216);
      this.gbTimeInOut.TabIndex = 0;
      this.gbTimeInOut.TabStop = false;
      this.gbTimeInOut.Text = " Время на карте";
      // 
      // dtpOut
      // 
      this.dtpOut.CustomFormat = "dd MMMM yyyy     H:mm:ss";
      this.dtpOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
      this.dtpOut.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.dtpOut.Location = new System.Drawing.Point(192, 61);
      this.dtpOut.Name = "dtpOut";
      this.dtpOut.Size = new System.Drawing.Size(226, 22);
      this.dtpOut.TabIndex = 1;
      this.dtpOut.ValueChanged += new System.EventHandler(this.dtpOut_ValueChanged);
      // 
      // lblExitTimeLimit
      // 
      this.lblExitTimeLimit.AutoSize = true;
      this.lblExitTimeLimit.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblExitTimeLimit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblExitTimeLimit.Location = new System.Drawing.Point(192, 166);
      this.lblExitTimeLimit.Name = "lblExitTimeLimit";
      this.lblExitTimeLimit.Size = new System.Drawing.Size(113, 23);
      this.lblExitTimeLimit.TabIndex = 0;
      this.lblExitTimeLimit.Text = "Нет данных";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label5.Location = new System.Drawing.Point(6, 166);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(91, 23);
      this.label5.TabIndex = 0;
      this.label5.Text = "Выезд до";
      // 
      // lblTimePaid
      // 
      this.lblTimePaid.AutoSize = true;
      this.lblTimePaid.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblTimePaid.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblTimePaid.Location = new System.Drawing.Point(192, 131);
      this.lblTimePaid.Name = "lblTimePaid";
      this.lblTimePaid.Size = new System.Drawing.Size(113, 23);
      this.lblTimePaid.TabIndex = 0;
      this.lblTimePaid.Text = "Нет данных";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label4.Location = new System.Drawing.Point(6, 131);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(172, 23);
      this.label4.TabIndex = 0;
      this.label4.Text = "Ост. опл. времени";
      // 
      // lblTimeInParking
      // 
      this.lblTimeInParking.AutoSize = true;
      this.lblTimeInParking.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblTimeInParking.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblTimeInParking.Location = new System.Drawing.Point(192, 96);
      this.lblTimeInParking.Name = "lblTimeInParking";
      this.lblTimeInParking.Size = new System.Drawing.Size(113, 23);
      this.lblTimeInParking.TabIndex = 0;
      this.lblTimeInParking.Text = "Нет данных";
      // 
      // lblParkingTimeText
      // 
      this.lblParkingTimeText.AutoSize = true;
      this.lblParkingTimeText.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblParkingTimeText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblParkingTimeText.Location = new System.Drawing.Point(6, 96);
      this.lblParkingTimeText.Name = "lblParkingTimeText";
      this.lblParkingTimeText.Size = new System.Drawing.Size(165, 23);
      this.lblParkingTimeText.TabIndex = 0;
      this.lblParkingTimeText.Text = "Время на стоянке";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label2.Location = new System.Drawing.Point(4, 61);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(133, 23);
      this.label2.TabIndex = 0;
      this.label2.Text = "Время выезда";
      // 
      // lblTimeIn
      // 
      this.lblTimeIn.AutoSize = true;
      this.lblTimeIn.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.lblTimeIn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.lblTimeIn.Location = new System.Drawing.Point(192, 26);
      this.lblTimeIn.Name = "lblTimeIn";
      this.lblTimeIn.Size = new System.Drawing.Size(113, 23);
      this.lblTimeIn.TabIndex = 0;
      this.lblTimeIn.Text = "Нет данных";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F);
      this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label1.Location = new System.Drawing.Point(6, 26);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(131, 23);
      this.label1.TabIndex = 0;
      this.label1.Text = "Время въезда";
      // 
      // btnPay
      // 
      this.btnPay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnPay.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold);
      this.btnPay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnPay.Location = new System.Drawing.Point(613, 423);
      this.btnPay.Name = "btnPay";
      this.btnPay.Size = new System.Drawing.Size(203, 105);
      this.btnPay.TabIndex = 3;
      this.btnPay.Text = "Оплатить";
      this.btnPay.UseVisualStyleBackColor = true;
      this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
      // 
      // btnRandomPayment
      // 
      this.btnRandomPayment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnRandomPayment.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
      this.btnRandomPayment.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnRandomPayment.Location = new System.Drawing.Point(484, 423);
      this.btnRandomPayment.Name = "btnRandomPayment";
      this.btnRandomPayment.Size = new System.Drawing.Size(123, 104);
      this.btnRandomPayment.TabIndex = 2;
      this.btnRandomPayment.Text = "Произвольная оплата";
      this.btnRandomPayment.UseVisualStyleBackColor = true;
      this.btnRandomPayment.Click += new System.EventHandler(this.btnRandomPayment_Click);
      // 
      // btnCashOper
      // 
      this.btnCashOper.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnCashOper.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
      this.btnCashOper.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnCashOper.Location = new System.Drawing.Point(6, 424);
      this.btnCashOper.Name = "btnCashOper";
      this.btnCashOper.Size = new System.Drawing.Size(119, 103);
      this.btnCashOper.TabIndex = 0;
      this.btnCashOper.Text = "Операции с кассой";
      this.btnCashOper.UseVisualStyleBackColor = true;
      this.btnCashOper.Click += new System.EventHandler(this.btnCashOper_Click);
      // 
      // btnRepayment
      // 
      this.btnRepayment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnRepayment.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
      this.btnRepayment.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnRepayment.Location = new System.Drawing.Point(131, 424);
      this.btnRepayment.Name = "btnRepayment";
      this.btnRepayment.Size = new System.Drawing.Size(114, 103);
      this.btnRepayment.TabIndex = 1;
      this.btnRepayment.Text = "Возврат";
      this.btnRepayment.UseVisualStyleBackColor = true;
      this.btnRepayment.Click += new System.EventHandler(this.btnRepayment_Click);
      // 
      // btnPinalty
      // 
      this.btnPinalty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnPinalty.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
      this.btnPinalty.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnPinalty.Location = new System.Drawing.Point(251, 424);
      this.btnPinalty.Name = "btnPinalty";
      this.btnPinalty.Size = new System.Drawing.Size(114, 103);
      this.btnPinalty.TabIndex = 1;
      this.btnPinalty.Text = " Штраф";
      this.btnPinalty.UseVisualStyleBackColor = true;
      this.btnPinalty.Click += new System.EventHandler(this.btnPenalty_Click);
      // 
      // btn_eCash
      // 
      this.btn_eCash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btn_eCash.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
      this.btn_eCash.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btn_eCash.Location = new System.Drawing.Point(369, 423);
      this.btn_eCash.Name = "btn_eCash";
      this.btn_eCash.Size = new System.Drawing.Size(109, 104);
      this.btn_eCash.TabIndex = 1;
      this.btn_eCash.Text = "Электронный кошелек";
      this.btn_eCash.UseVisualStyleBackColor = true;
      this.btn_eCash.Click += new System.EventHandler(this.mnuE_Cash_Click);
      // 
      // panel1
      // 
      this.panel1.BackgroundImage = global::Parking.CashDesk.Properties.Resources.left;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(86, 578);
      this.panel1.TabIndex = 8;
      this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
      this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
      this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
      // 
      // timerIndicator
      // 
      this.timerIndicator.Interval = 1000;
      this.timerIndicator.Tick += new System.EventHandler(this.timerIndicator_Tick);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.ClientSize = new System.Drawing.Size(1003, 588);
      this.Controls.Add(this.panel3);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.KeyPreview = true;
      this.MinimumSize = new System.Drawing.Size(683, 523);
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "АРМ Ручная касса";
      this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
      this.panel3.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.gbSumma.ResumeLayout(false);
      this.gbSumma.PerformLayout();
      this.gbOperDeskription.ResumeLayout(false);
      this.gbCurrentTime.ResumeLayout(false);
      this.gbCurrentTime.PerformLayout();
      this.gbTimeInOut.ResumeLayout(false);
      this.gbTimeInOut.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox gbTimeInOut;
    private System.Windows.Forms.GroupBox gbSumma;
    private System.Windows.Forms.GroupBox gbOperDeskription;
    private System.Windows.Forms.GroupBox gbCurrentTime;
    private System.Windows.Forms.Button btnCashOper;
    private System.Windows.Forms.Button btnPinalty;
    private System.Windows.Forms.Button btnPay;
    private System.Windows.Forms.Label lblSumma;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label lblParkingTimeText;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.DateTimePicker dtpOut;
    private System.Windows.Forms.Label lblTime;
    private System.Windows.Forms.Timer ShowTimeTimer;
    private System.Windows.Forms.Label lblDebt;
    private System.Windows.Forms.Label lblPaid;
    private System.Windows.Forms.Label lblDebtText;
    private System.Windows.Forms.Label lblPaidText;
    private System.Windows.Forms.Label lblPenaltyCount;
    private System.Windows.Forms.Button btn_eCash;
    private System.Windows.Forms.Button btnRandomPayment;
    private System.Windows.Forms.Label lblOperationDesc;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.Label lblExitTimeLimit;
    private System.Windows.Forms.Label lblTimePaid;
    private System.Windows.Forms.Label lblTimeInParking;
    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.Button btnRepayment;
    private System.Windows.Forms.Button btnAbout;
    private System.Windows.Forms.Button btnSetup;
    private System.Windows.Forms.Label lblTimeIn;
    private System.Windows.Forms.Label lblECash;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label lblCaption;
    private System.Windows.Forms.Label tslblStatusNetwork;
    private System.Windows.Forms.Label tslblStatusSent;
    private System.Windows.Forms.Label tslblStatusReceived;
    private System.Windows.Forms.Timer timerIndicator;
    private System.Windows.Forms.Label lblReaderState;
    private System.Windows.Forms.Label lblReader;
    private System.Windows.Forms.Label lblFiscalState;
    private System.Windows.Forms.ImageList imageList;
  }
}

