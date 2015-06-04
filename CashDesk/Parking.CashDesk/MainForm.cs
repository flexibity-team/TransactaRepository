using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using CryptLib;
using CSVWritter;
using Parking.Calculators;
using Parking.CardReaders;
using Parking.CashDesk.Properties;
using Parking.Data;
using Parking.Data.Devices;
using Parking.Data.Metro;
using Parking.FiscalDevice;
using Parking.Network;
using Parking.Network.Tcp;
using Parking.SmartCards;
using Parking.SmartCards.Keys;
using ParkTime.StandardForm;
using RMLib;
using RMLib.Forms;
using RMLib.Log;
using RMLib.Log.Formatters;
using RMLib.Log.Writers;
using Parking.Data.Devices.Commands;

namespace Parking.CashDesk
{
  public partial class MainForm : Form
  {
    #region [ const ]

    private const string _NO_DATA = "��� ������";
    private const string _NEED_PAY = "��������� ������";

    private const string _READER_NOT_FOUND = "����������� �� ������ ��� ����� ������ �����������!\r\n";
    private const string _FISCAL_DEVICE_NOT_FOUND = "��� �� �������. \r\n��������� ������� � ����������� ���, � ��� �� ��������� ���������.\r\n";
    private const string _DISPLAY_NOT_FOUND = "������� ������������ �� ������. \r\n��������� ������� � ����������� �������, � ��� �� ��������� ���������.\r\n";
    private const string _READ_CARD_ERROR = "������ ������ �����. ���������� ��� ���.\r\n";
    private const string _TARIF_ERROR = "������ ��� ������� ��������� ������. �������� �������� ����� �� �����.\r\n";
    private const string _TARIF_PARAM_ERROR = "������ � ���������� ������. �������� �������� ����� �� �����.\r\n";
    private const string _TARIF_DATEOUT_ERROR = "������ � ����������� ������� ������.\r\n";
    private const string _ERROR_LOAD_KEYS = "������ �������� ������ � �����������\r\n";
    private const string _DECRYPT_ERROR = "������ ������������ ������. ��������� ��������� ���.\r\n";
    private const string _PAY_ERROR = "������ ������!\r\n";
    private const string _PASS_ERROR = "�������� ������. ������ ��������!\r\n";
    private const string _NETWORK_CREATE_ERROR = "������ ��� �������� �������� ���������";

    #endregion

    private static int CounterDelay = Settings.Default.CounterDelay;

    // ��� �������� ������ �� ����� ����� �����
    private bool _dragging;
    private int _dragX, _dragY;

    private ILogger _logger;       // ���
    private IFiscalDevice _fiscalDevice; // ����� ������ � ���
    private UserDisplay _display; // ������� �������
    private SmartCardManager _smartCardManager; // �������� ����
    private ISmartCardReader _smartCardReader; // �����
    private ICalculator _calculator; // �����������
    private CashTransactions _trans; // ��������� ���������� ���������� ������ � ���� CSV
    private Tariff _penaltyTariff;

    // ������ �� ������� ��������
    private PaymentDocument _currentDocument; // ����� ������ � ������ ��������� �� ������� ��������
    private int _cardID;
    private ISmartCardPx _smartCard;
    // ���� - ����� ������ �� DateTimePicker ����� ���� �������� �������������
    private bool _timeExitReadyToChange;

    private ResourceManager _rm;
    private CultureInfo _cultureInfo;
    private SettingsForm _configForm;
    private ParkingAboutForm _aboutForm;

    private MetroDiscount _metroDiscount;
    private Tariff _metroTariffK;
    private Tariff _metroTariffL;
    private ICalculator _calculatorMetro;

    private bool _indicatorReseived;
    private bool _indicatorSent;
    private Image _imageIdle;
    private Image _imageReceived;
    private Image _imageSent;

    private VirtualCashBox _device;
    private INetworkProtocol _network;
    
    private int _counterReceived;
    private int _counterSent;
    private int _counterDelay;
    private DateTime _lastPacketTime;

    public MainForm()
    {
      InitializeComponent();

      this.SetLogoIcon();

      _cultureInfo = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = _cultureInfo;
      Thread.CurrentThread.CurrentUICulture = _cultureInfo;

      //lblExitTimeLimit.Text = _rm.GetString("InPay");
      //OnCardDisConnected();
      //gbCurrentTime.Font = Fonts.CreateFont("DS Crystal", 14, FontStyle.Regular);
      //gbOperDeskription.Font = Fonts.CreateFont("DS Crystal", 14, FontStyle.Regular);
      //gbSumma.Font = Fonts.CreateFont("DS Crystal", 14, FontStyle.Regular);
      //gbTimeInOut.Font = Fonts.CreateFont("DS Crystal", 14, FontStyle.Regular);
      //lblSumma.Font = Fonts.CreateFont("DS Crystal", 40, FontStyle.Regular);
      //btnCashOper.Font = Fonts.CreateFont("DS Crystal", 14, FontStyle.Regular);
      //btnPinalty.Font = Fonts.CreateFont("DS Crystal", 14, FontStyle.Regular);
      //btn_eCash.Font = Fonts.CreateFont("DS Crystal", 14, FontStyle.Regular);
      //btnRandomPayment.Font = Fonts.CreateFont("DS Crystal", 14, FontStyle.Regular);
      //btnPay.Font = Fonts.CreateFont("DS Crystal", 20, FontStyle.Regular);

      _aboutForm = null;

      imageList.Images.Add(Images.Exit);
      imageList.Images.Add(Images.Settings);
      imageList.Images.Add(Images.Help);
      btnExit.ImageIndex = 0;
      btnSetup.ImageIndex = 1;
      btnAbout.ImageIndex = 2;

      _dragging = false;
      _dragX = 0;
      _dragY = 0;

      _penaltyTariff = null;

      _metroDiscount = null;
      _metroTariffK = null;
      _metroTariffL = null;
      _calculatorMetro = null;

      _counterReceived = 0;
      _counterSent = 0;
      _counterDelay = CounterDelay;
      _lastPacketTime = DateTime.Now;
    }

    #region [ window events ]

    private void MainForm_Load(object sender, EventArgs e)
    {
      //logger
      CashDeskPathManager pathManager = new CashDeskPathManager();
      ILogWriter writer = new FileLogWriter(pathManager.LogPath, "CashDesk");
      ILogFormatter formatter = new MultilineStringLogFormatter();
      _logger = new Logger(1000, new ILogWriter[] { writer }, formatter);
      _logger.Threshold = Settings.Default.LogLevel;
      _logger.Write(LogLevel.Debug, "������ ���������");

      //init controls
      CheckConfiguration();

      ShowMessage("��� ������ ��������, ������ ��� ���������� ������������ �������� ��������� �����.", LogLevel.Information);
      ShowMessage("��� ������������ ������ ��� �������� ����� ����� �� ������������.", LogLevel.Information, true);

      ShowTimeTimer.Enabled = true; // ������ ������� ����������� �������� ������� �� �����

      lblPenaltyCount.Text = Settings.Default.PenaltyCardCount.ToString();

      //create calculator
      _calculator = CalculatorLoader.CreateCalculator(pathManager.CalculatorsPath, Settings.Default.CalculatorVersion);
      _calculatorMetro = new CalculatorMetro();

      //get devices
      _fiscalDevice = GetFiscalDevice(); // #### ����������� ��� �� ����� ������������ ####
      UpdateFiscalState(_fiscalDevice.ToString());
      _display = GetDisplay(); // ������� ������� ################################
      string s = Application.StartupPath + @"\Transaction.csv";
      _trans = new CashTransactions(s, new CSVFileWriter(s));

      //smart card reader
      UpdateReaderState("��� �����");
      _smartCardReader = GetReader(pathManager.CardReadersPath);
      if (_smartCardReader != null)
      {
        _smartCardManager = new SmartCardManager(_smartCardReader, null, GetKeys()); // ������ � ������
        _smartCardReader.ConnectedCard += () =>
        {
          this.SafeInvoke(new Action(OnCardConnected)); // ������� - ����� �� ������ 
        };
        _smartCardReader.RemovedCard += () =>
        {
          this.SafeInvoke(new Action(OnCardDisconnected)); // ������� - ����� ������ � ������ 
        };

        // ���� ��� ������� ����� ��� ������ �� �����������
        if (_smartCardReader.CardState == SCardState.Connected)
          OnCardConnected();
        else
          OnCardDisconnected();

        //s = Settings.Default.ReaderType.GetString();
      }
      else
        UpdateReaderState("����������");


      //indicators
      _indicatorReseived = false;
      _indicatorSent = false;
      _imageIdle = Resources.idle;
      _imageReceived = Resources.received;
      _imageSent = Resources.sent;

      //network
      _device = new VirtualCashBox(GetCashBox());
      lblCaption.Text = String.Format("��� ������ ����� #{0}", _device.Device.NetworkAddress);
      _network = GetNetworkProtocol();
      if (_network != null)
        timerIndicator.Start();
      else
      {
        tslblStatusReceived.Visible = false;
        tslblStatusSent.Visible = false;
        tslblStatusNetwork.Text += " ����������";
      }

      //create config form
      _configForm = new SettingsForm(_logger, _smartCardManager);

      //apply localization
      Assembly assembly = Assembly.GetExecutingAssembly();
      _rm = new ResourceManager("Parking.CashDesk.Localization.Translate", assembly);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      _logger.Write(LogLevel.Debug, "�������� ���������");

      timerIndicator.Stop();
      if (_network != null)
        _network.Close();

      // ������������ �����������
      if (_smartCardReader != null)
        _smartCardReader.Dispose();

      if (_smartCardManager != null)
        _smartCardManager.Dispose();

      if (_fiscalDevice != null)
        _fiscalDevice.Dispose(); // ���������� ������ � ���

      ((Logger)_logger).Dispose();

      ShowTimeTimer.Enabled = false;
    }

    private void MainForm_MouseMove(object sender, MouseEventArgs e)
    {
      if (!_dragging)
        return;

      int x, y;
      x = e.X - _dragX;
      y = e.Y - _dragY;
      Point newpos = new Point(Location.X + x, Location.Y + y);
      Location = newpos;
    }

    private void MainForm_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;

      _dragging = true;
      _dragX = e.X;
      _dragY = e.Y;
      Cursor.Current = System.Windows.Forms.Cursors.SizeAll;
    }

    private void MainForm_MouseUp(object sender, MouseEventArgs e)
    {
      _dragging = false;
      Cursor.Current = System.Windows.Forms.Cursors.Arrow;
    }

    private void btnPay_Click(object sender, EventArgs e)
    {
      // �������� ����������
      DisableControls();
      if (_currentDocument == null)
      {
        _logger.Write(LogLevel.Warning, "��� ������ �� �������. ������ ����������.");
        MessageBox.Show(this, "��� ������ �� �������. ������ ����������.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        // ��� ������ �� �������, ������ � ������ ������������ �� �����.
        return;
      }

      try
      {
        Pay();
      }
      catch (Exception ex)
      {
        ShowError(ex);
        return;
      }

      // ���������� �� ������ ������. ��� �� ����� ��������� �� ������������� ����� ������������ ����� ��� ��������
      // ������������ ������ ����� ������� (��� ����������� ������ �� �����, � ��������� ������ ��� ���������� �� �����-
      // ������ ����� ������).
      try
      {
        Calculate(dtpOut.Value); // ������ ����� ������
      }
      catch (Exception ex)
      {
        ShowError(ex);
      }

      _currentDocument = null;

      // �������� ����������
      EnableControls();
    }

    private void btnCashOper_Click(object sender, EventArgs e)
    {
      using (CashControl frmCashControl = new CashControl(_fiscalDevice, _trans, _logger))
        frmCashControl.ShowDialog();
    }

    private void ShowTimeTimer_Tick(object sender, EventArgs e)
    {
      lblTime.Text = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
    }

    private void btnPenalty_Click(object sender, EventArgs e)
    {
      //�������� ����������
      DisableControls();

      // ���������� �������� �����
      // ��� �������� �������� ����� ������������ ��������� ��������:
      // ����� ������ ������������ � �������. � ���������� ������ ����������
      // � �������� �������, �������� ����� ����� ������������� �����.
      // ���������� ������� �� ��������� ��������� ������. ������ �����
      // ������������� ����� �� ��������� �������� �����.
      TimeSpan tsTemp = new TimeSpan(0, 0, 0, 10); // ������� ������������ ������� ����� ������� � ������� - 10���. � �� ����������� ��������.
      _smartCard.TimeEntry = DateTime.Now - tsTemp;
      _smartCard.TimeExit = DateTime.Now;

      // ��������� ����� ��� ���������, �� ���������������, ...
      _smartCard.CounterEntry = 1;
      _smartCard.CounterExit = 0;
      _smartCard.Blocked = false;
      _smartCard.Payment = 0;
      _smartCard.Debt = 0;
      _smartCard.Discount = 0;

      _smartCard.CustomerType = CustomerType.OnceOnly;
      _smartCard.ZoneCurrentID = 1; //TODO: ������� ����������� ������ ����.
      _smartCard.ZonePreviousID = 0;

      // �������� ����� �� ��������
      _smartCard.Tariff = _penaltyTariff;
      _smartCard.State = CardState.Penalty;

      // ����������� ����� ������ (true - �� ��������� ������)
      try
      {
        Calculate(_smartCard.TimeExit);
      }
      catch (Exception ex)
      {
        ShowError(ex);
        return;
      }

      UpdateParameters(_currentDocument.TimeExit);

      try
      {
        Pay();
      }
      catch (Exception ex)
      {
        ShowError(ex);
        return;
      }

      // ���������� �� ������ ������. ��� �� ����� ��������� �� ������������� ����� ������������ ����� ��� ��������
      // ������������ ������ ����� ������� (��� ����������� ������ �� �����, � ��������� ������ ��� ���������� �� �����-
      // ������ ����� ������).
      try
      {
        Calculate(_smartCard.TimeExit);
      }
      catch (Exception ex)
      {
        ShowError(ex);
        _currentDocument = null;
        return;
      }

      _currentDocument = null;

      ShowMessage("�������� ����� �������. ��������� ����� �������.", LogLevel.Information);
      // ���������� ����� �������� �������� �������
      Settings.Default["PenaltyCardCount"] = Settings.Default.PenaltyCardCount + 1;
      Settings.Default.Save();
      lblPenaltyCount.Text = Settings.Default.PenaltyCardCount.ToString(); // ���������� ����� �������� �������� ����

      // ��������� ���������� �� �����. ����� �������� ������ ����� �� �����������
    }

    private void dtpOut_ValueChanged(object sender, EventArgs e)
    {
      if (!_timeExitReadyToChange)
        return;

      _timeExitReadyToChange = false;
      try
      {
        Calculate(dtpOut.Value);
      }
      catch (Exception err)
      {
        ShowError(err);
      }
    }

    private void mnuE_Cash_Click(object sender, EventArgs e)
    {
      // ���� ����� �������� �����
      string sSumm = String.Empty;
      if (InputBox.Show("���������� ������������ ��������", "������� �����", true, ref sSumm) != DialogResult.OK)
        return;

      long Summ = 0;
      if (!String.IsNullOrEmpty(sSumm))
        Summ = Convert.ToInt64(sSumm); // �����
      else
      {
        EnableControls();
        return;
      }
      _currentDocument = new PaymentDocument(Summ, PaymentReason.ECash, DocumentType.Selling,
        PaymentType.Cash, _cardID, DateTime.Now, DateTime.Now, 0, 0, (double)_smartCard.ECash);

      try
      {
        Pay();
      }
      catch (Exception ex)
      {
        ShowError(ex);
      }
    }

    private void btnRandomPayment_Click(object sender, EventArgs e)
    {
      // ���� ����� �������� �����
      string sSumm = String.Empty;
      if (InputBox.Show("������ �� ������������ �����", "������� �����", true, ref sSumm) != DialogResult.OK)
        return;

      long Summ = 0;
      if (!String.IsNullOrEmpty(sSumm))
        Summ = Convert.ToInt64(sSumm); // �����
      else
      {
        EnableControls();
        return;
      }
      _currentDocument = new PaymentDocument(Summ, PaymentReason.Any, DocumentType.Selling,
        PaymentType.Cash, _cardID, DateTime.Now, DateTime.Now, 0, 0, 0);

      try
      {
        Pay();
      }
      catch (Exception ex)
      {
        ShowError(ex);
      }
    }

    private void btnExit_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btnRepayment_Click(object sender, EventArgs e)
    {
      // ���� ����� �������� �����
      string sSumm = String.Empty;
      if (InputBox.Show("�������", "������� �����", true, ref sSumm) != DialogResult.OK)
        return;

      long Summ = 0;
      if (!String.IsNullOrEmpty(sSumm))
        Summ = Convert.ToInt64(sSumm); // ����� � ��������
      else
      {
        EnableControls();
        return;
      }

      _currentDocument = new PaymentDocument(Summ, PaymentReason.Refund, DocumentType.Repayment,
        PaymentType.Cash, _cardID, DateTime.Now, DateTime.Now, 0, 0, 0);

      try
      {
        Pay();
      }
      catch (Exception ex)
      {
        ShowError(ex);
      }
    }

    private void pbPower_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btnAbout_Click(object sender, EventArgs e)
    {
      if (_aboutForm == null)
        _aboutForm = new ParkingAboutForm();

      _aboutForm.ShowAbout(this);
    }

    private void btnSetup_Click(object sender, EventArgs e)
    {
      PasswordForm passwordForm = new PasswordForm("������� ������");
      if (passwordForm.ShowDialog() != DialogResult.OK)
        return;

      if (passwordForm.edPass.Text != Crypt.Decrypt(Settings.Default.Password, "t23F78C4"))
      {
        ShowMessage(_PASS_ERROR, LogLevel.Error);
        return;
      }
      
      _configForm.ShowDialog();

      try
      {
        _penaltyTariff = CashDeskHelper.DeserializeTariff(Settings.Default.PenaltyTarif);
      }
      catch (Exception ex)
      {
        _logger.Write(ex, "������ ������ ��������� ������");
      }

      try
      {
        _metroTariffK = CashDeskHelper.DeserializeTariff(Settings.Default.MetroTariffK);
        _metroTariffL = CashDeskHelper.DeserializeTariff(Settings.Default.MetroTariffL);
      }
      catch (Exception ex)
      {
        _logger.Write(ex, "������ ������ ������� �����");
      }
    }

    private void timerIndicator_Tick(object sender, EventArgs e)
    {
      if ((DateTime.Now - _lastPacketTime).TotalSeconds > 1)
        SwitchOff();
    }

    #endregion

    #region [ initialization ]

    /// <summary>
    /// ���������� ������ ������������� ������� ����������
    /// </summary>
    private UserDisplay GetDisplay()
    {
      UserDisplay display = null;
      if (Settings.Default.DisplayEnabled) // ���� ��������� �������� �� �������
      {
        display = new UserDisplay(_logger);
        int nState = display.Initialize(Settings.Default.DisplayPort, Settings.Default.DisplayCodePage);
        if (nState != 0)
        {
          _logger.Write(LogLevel.Error, "���������� ���������������� ������� �������. �������� �� ������� VFDDisplay.dll.");
          ShowMessage(_DISPLAY_NOT_FOUND, LogLevel.Error, true);
        }
        else
        {
          display.Clear();
          _logger.Write(LogLevel.Debug, "������� ���������.");
        }
      }
      else
        _logger.Write(LogLevel.Debug, "����������� ������� �� ���������.");

      return display;
    }

    /// <summary>
    /// ���������� ������ ����������� ����������
    /// </summary>
    private IFiscalDevice GetFiscalDevice()
    {
        IFiscalDevice f = null;
       if (Settings.Default.FiscalDeviceEnabled)
              f = new Start200Control(); // �����-200�
      else
        f = new VirtualFiscalDevice(); // ��� �����������

      try
      {
          f.LogFileOn(Settings.Default.LogOn);
          f.Initialize(Settings.Default.FiscalDevicePort, Settings.Default.CashNumber.ToString()); // ������������� ���
      }
      catch (FiscalDeviceException err)
      {
          _logger.Write(err, "���������� ���������������� ���. �������� �� ������� Azimuth.dll.\r\n" + err.ToString());

        ShowMessage(_FISCAL_DEVICE_NOT_FOUND, LogLevel.Error, true);
      }

      _logger.Write(LogLevel.Debug, "��� ����������.");

      return f;
    }

    /// <summary>
    /// ��������� ����� �� ����� ������������ � �������������� ��
    /// </summary>
    private Dictionary<int, IAccessKey> GetKeys()
    {
      Dictionary<int, IAccessKey> keys = new Dictionary<int, IAccessKey>();
      try
      {
        byte[] KeyB1 = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyB1, "t23F78C4"));
        keys.Add(1, new AccessKey(KeyType.KeyB, KeyB1)); // B1
        byte[] KeyB2 = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyB2, "t23F78C4"));
        keys.Add(2, new AccessKey(KeyType.KeyB, KeyB2)); // B2
        byte[] KeyB3 = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyB3, "t23F78C4"));
        keys.Add(3, new AccessKey(KeyType.KeyB, KeyB3)); // B3
        keys.Add(4, new AccessKey(KeyType.KeyB, KeyB3)); // B3
        keys.Add(5, new AccessKey(KeyType.KeyB, KeyB3)); // B3
        keys.Add(6, new AccessKey(KeyType.KeyB, KeyB3)); // B3
        byte[] KeyB4 = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyB4, "t23F78C4"));
        keys.Add(7, new AccessKey(KeyType.KeyB, KeyB4)); // B4
        keys.Add(8, new AccessKey(KeyType.KeyB, KeyB4)); // B4
        keys.Add(9, new AccessKey(KeyType.KeyB, KeyB4)); // B4
        byte[] KeyBM = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyBM, "t23F78C4"));
        keys.Add(SmartCardLayoutHelper.MetroSectorIndex, new AccessKey(KeyType.KeyB, KeyBM)); // BM
      }
      catch (Exception ex)
      {
        keys.Add(1, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B1B1B1"))); // B1
        keys.Add(2, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B2B2B2"))); // B2
        keys.Add(3, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B3B3B3"))); // B3
        keys.Add(4, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B3B3B3"))); // B3
        keys.Add(5, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B3B3B3"))); // B3
        keys.Add(6, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B3B3B3"))); // B3
        keys.Add(7, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B4B4B4"))); // B4
        keys.Add(8, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B4B4B4"))); // B4
        keys.Add(9, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B4B4B4"))); // B4
        keys.Add(SmartCardLayoutHelper.MetroSectorIndex, new AccessKey(KeyType.KeyB, AccessKeyHelper.GetKeyFromString("B4B4B4"))); // B4
        
        ShowMessage(_DECRYPT_ERROR, LogLevel.Error, true);
        _logger.Write(ex, @"������ ������������ ������. �������� ����������� ���� ������������. ����������� ����� �� ���������.");
      }

      return keys;
    }

    /// <summary>
    /// ���������� ������ ������ �������� ���������� � ����� ������������
    /// </summary>
    private ISmartCardReader GetReader(string cardReadersPath)
    {
      ISmartCardReader reader = null;
      try
      {
        reader = CardReaderLoader.CreateCardReader(cardReadersPath, Settings.Default.ReaderType);
        SmartCardOptions.EnableMetroSector = Settings.Default.UseMetro;

        _logger.Write(LogLevel.Debug, String.Format("������ ����������� {0}", Settings.Default.ReaderType.GetString()));
      }
      catch (Exception err)
      {
        _logger.Write(err, "����������� �� ������.", 0);
        ShowMessage(_READER_NOT_FOUND, LogLevel.Error, true);
      }

      return reader;
    }

    /// <summary>
    /// ������� ����������
    /// </summary>
    private ManualCashBox GetCashBox()
    {
      ManualCashBox c = new ManualCashBox();

      c.ID = Settings.Default.DeviceID;
      c.NetworkAddress = Settings.Default.DeviceNetworkAddress;
      c.Name = "������ �����";

      return c;
    }

    /// <summary>
    /// ������� ������� ��������
    /// </summary>
    private INetworkProtocol GetNetworkProtocol()
    {
      INetworkProtocol n = null;
      try
      {
        n = new TcpProtocolServer(_logger);
        n.PacketReceived += OnNetworkPacketReceived;
        n.Open();
      }
      catch (Exception e)
      {
        _logger.Write(e, _NETWORK_CREATE_ERROR);
        ShowMessage(_NETWORK_CREATE_ERROR, LogLevel.Error, true);
      }

      return n;
    }

    /// <summary>
    /// �������� ����������� ���������� ����� ������������
    /// </summary>
    private void CheckConfiguration()
    {
      if (Settings.Default.KeyB1 == "none")
        Settings.Default["KeyB1"] = Crypt.Encrypt("A1A1A1", "t23F78C4");

      if (Settings.Default.KeyB2 == "none")
        Settings.Default["KeyB2"] = Crypt.Encrypt("A2A2A2", "t23F78C4");

      if (Settings.Default.KeyB3 == "none")
        Settings.Default["KeyB3"] = Crypt.Encrypt("A3A3A3", "t23F78C4");

      if (Settings.Default.KeyB4 == "none")
        Settings.Default["KeyB4"] = Crypt.Encrypt("A4A4A4", "t23F78C4");

      if (Settings.Default.KeyBM == "none")
        Settings.Default["KeyBM"] = Crypt.Encrypt("A4A4A4", "t23F78C4");

      if (Settings.Default.Password == "none")
        Settings.Default["Password"] = Crypt.Encrypt("admin", "t23F78C4");

      if (Settings.Default.CashPassword == "none")
        Settings.Default["CashPassword"] = Crypt.Encrypt("admin", "t23F78C4");

      if (Settings.Default.FiscalDevicePort == 0)
        Settings.Default["FiscalDevicePort"] = 1;
      
      if (Settings.Default.DisplayPort == 0)
        Settings.Default["DisplayPort"] = 2;
      
      if (Settings.Default.CashNumber == 0)
        Settings.Default["CashNumber"] = (byte)1;

      if (String.IsNullOrEmpty(Settings.Default.PenaltyTarif))
      {
        Settings.Default["PenaltyTarif"] = CashDeskHelper.SerializeTariff(GetDefaultPenaltyTariff());
        MessageBox.Show(this, "������ ������ ��������� ������ �� ����� ������������. ��������� ����� � ������-�����.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      else
      {
        try
        {
          _penaltyTariff = CashDeskHelper.DeserializeTariff(Settings.Default.PenaltyTarif);
        }
        catch (Exception e)
        {
          _penaltyTariff = GetDefaultPenaltyTariff();
          _logger.Write(e, String.Format("������ ������ ��������� ������ �� ����� ������������\r\n�����:\r\n{0}", Settings.Default.PenaltyTarif));
          MessageBox.Show(this, "������ ������ ��������� ������ �� ����� ������������. ��������� ����� � ������-�����.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }

      if (Settings.Default.UseMetro)
        if (String.IsNullOrEmpty(Settings.Default.MetroTariffK) || String.IsNullOrEmpty(Settings.Default.MetroTariffL))
        {
          string s = CashDeskHelper.SerializeTariff(GetDefaultMetroTariff());
          Settings.Default["MetroTariffK"] = s;
          Settings.Default["MetroTariffL"] = s;
          MessageBox.Show(this, "������ ������ ������� ����� �� ����� ������������. ��������� ������ ����� �� ������.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
          try
          {
            _metroTariffK = CashDeskHelper.DeserializeTariff(Settings.Default.MetroTariffK);
            _metroTariffL = CashDeskHelper.DeserializeTariff(Settings.Default.MetroTariffL);
          }
          catch (Exception e)
          {
            _metroTariffK = GetDefaultMetroTariff();
            _metroTariffL = GetDefaultMetroTariff();
            _logger.Write(e, String.Format("������ ������ ������� ����� �� ����� ������������\r\n{0}\r\n{1}", Settings.Default.MetroTariffK, Settings.Default.MetroTariffL));
            MessageBox.Show(this, "������ ������ ������� ����� �� ����� ������������. ��������� ������ ����� �� ������.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }

      //DateTime TempDate = new DateTime(2001,01, 01, 00,00,00);
      //if (Settings.Default.SessionClose == TempDate) Settings.Default.SessionClose = DateTime.Now;
      //if (Settings.Default.SessionOpen == TempDate) Settings.Default.SessionOpen = DateTime.Now;
      Settings.Default.Save();
    }

    #endregion

    #region [ smart card ]

    private void OnCardConnected()
    {
      UpdateReaderState(DataFormatter.ToHex(_smartCardReader.ReadCardNumber()));

      DisableControls();
      _currentDocument = null;

      ShowMessage("������������ ����� ����� ������. ���������� �����...", LogLevel.Information);

      _smartCard = ReadCard(out _cardID);
      if (_smartCard == null)
      {
        ShowMessage(_READ_CARD_ERROR, LogLevel.Error);
        return; // ������ ������ �����
      }

      _metroDiscount = ReadMetroDiscount();

      CardPresentControlState();

      try
      {
        Calculate(DateTime.Now); // ������ ����� ������
      }
      catch (Exception err)
      {
        ShowError(err);
        return;
      }

      if (_currentDocument.Amount > 0)
        CanPayControlState(); // ���� ����, ��� �������, �� ���������� ������.
    }

    private void OnCardDisconnected()
    {
      UpdateReaderState("��� �����");
      NoCardControlState();
      _timeExitReadyToChange = false;
      _smartCard = null;
      _metroDiscount = null;
      _cardID = 0;
      _currentDocument = null;
      UpdateParameters(DateTime.Now);
      ShowMessage("��� ������ ��������, ������ ��� ���������� ������������ �������� ��������� �����.", LogLevel.Information);
      ShowMessage("��� ������������ ������ ��� �������� ����� ����� �� ������������.", LogLevel.Information, true);
    }

    private ISmartCardPx ReadCard(out int cardID)
    {
      SmartCardsP3x smartCard = new SmartCardsP3x();
      cardID = 0;

      try
      {
        cardID = (int)_smartCardManager.GetCardID();
        _smartCardManager.ReadCard(smartCard);
        _logger.Write(LogLevel.Debug, "����� ������� ���������");
      }
      catch (Exception err)
      {
        _logger.Write(err, "������ ������ �����: ���������� ��������� ���������� �� �����");
        EnableControls();
        return null;
      }

      return smartCard;
    }

    private void WriteCard(ISmartCardPx smartCard)
    {
      try
      {
        _smartCardManager.WriteCard(smartCard);
        _logger.Write(LogLevel.Debug, "����� ������� ��������");
      }
      catch (Exception err)
      {
        _logger.Write(err, "������ ������ �����: ���������� �������� ���������� �� �����");
        throw new Exception(_READ_CARD_ERROR, err);
      }
    }

    private MetroDiscount ReadMetroDiscount()
    {
      MetroDiscount md = null;
      if (Settings.Default.UseMetro)
        try
        {
          //check tariff
          Tariff30 t = (Tariff30)_smartCard.Tariff;
          if (t.Type == Tariff30Type.WeekeySimple)
          {
            //read sector
            byte[] ba = new byte[SmartCardLayoutHelper.UserSectorSize];
            _smartCardManager.Reader.ReadSector(SmartCardLayoutHelper.MetroSectorIndex,
              _smartCardManager.KeysB[SmartCardLayoutHelper.MetroSectorIndex], ba, 0);

            md = new MetroDiscount();
            md.UnpackData(ba);

            //int parkingID = md.ParkingID;
            //if ((parkingID > 0) && (parkingID != Settings.Default.ParkingID))
            //{
            //  md = null;
            //  throw new ApplicationException(String.Format("������������� �������� � ������� ����� ({0}) �� ��������� �� ��������� � ����� ������������ ({1})", parkingID, Settings.Default.ParkingID));
            //}
          }
        }
        catch (Exception e)
        {
          _logger.Write(LogLevel.Warning, String.Format("������ ������ ���������� �� ������� �����\r\n{0}", e.Message));
        }

      return md;
    }

    private void WriteMetroDiscount(MetroDiscount md)
    {
      if (!Settings.Default.UseMetro)
        return;

      try
      {
        byte[] ba = new byte[SmartCardLayoutHelper.UserSectorSize];
        md.PackData(ba);

        _smartCardManager.Reader.WriteSector(SmartCardLayoutHelper.MetroSectorIndex,
          _smartCardManager.KeysB[SmartCardLayoutHelper.MetroSectorIndex], ba);
      }
      catch (Exception e)
      {
        _logger.Write(LogLevel.Warning, String.Format("������ ������ ���������� � ������\r\n{0}", e.Message));
      }
    }

    #endregion

    #region [ calculation and payment ]

    /// <summary>
    /// ������ ����� ������ � �������� ���������
    /// </summary>
    /// <param name="TimeOut">�������� ����� ������</param>
    private void Calculate(DateTime TimeOut)
    {
      _timeExitReadyToChange = false;
      Decimal ToPay = 0;
      DateTime EstimatedTimeOut;

      //select tariff and calculator
      Tariff t = _smartCard.Tariff;
      ICalculator c = _calculator;
      if (_metroDiscount != null)
      {
        bool b = _metroDiscount.CanUseDiscount;
        _logger.Write(LogLevel.Debug, String.Format("������ �� ������ ����� : ", b ? "��������" : "������������"));

        t = b ? _metroTariffL : _metroTariffK;
        if (t is TariffMetro)
          c = _calculatorMetro;
      }

      //calculate
      try
      {
        ToPay = c.Calculate(t, _smartCard.TimeEntry, TimeOut, out EstimatedTimeOut);
      }
      catch (Exception err)
      {
        _logger.Write(err, "������ ��� ������� ����� ������");
        throw new Exception("������ ��� ������� ����� ������:\r\n", err);
      }

      decimal discountTotal = 0;
      for (int i = 0; i < _smartCard.ServicesCount; i++)
        discountTotal += _smartCard.Discounts[i].Amount;

      // ����� = ��������� �������� + ����� ����������� - ���������� ����� + ����� �����/������
      decimal amount = ToPay + _smartCard.Debt - _smartCard.Payment + discountTotal; // - _smartCard.Discount;
      _logger.Write(LogLevel.Debug, "����� � ������: " + amount.ToString("C"));
      _currentDocument = new PaymentDocument((double)amount, PaymentReason.Parking, DocumentType.Selling,
        PaymentType.Cash, _cardID, _smartCard.TimeEntry, EstimatedTimeOut, (double)_smartCard.Payment,
        (double)_smartCard.Debt, (double)_smartCard.ECash);

      _logger.Write(LogLevel.Debug, "������ ��������: " + _currentDocument.ToString());

      UpdateParameters(TimeOut); // ���������� ������ �� �����
      ShowMessage("������ ����� ������ ������� ��������.", LogLevel.Information);

      _timeExitReadyToChange = true;// ����������� �� ������� ��������� ���� ������
    }

    /// <summary>
    /// ������, ������ �� ����� � ������ ����
    /// </summary>
    private void Pay()
    {
      ShowMessage("������������ �������� ������. ���������� �����...", LogLevel.Information);

      //��������, �������-�� ����?
      if (_currentDocument.Amount <= 0)
      {
        ShowMessage("������ �� ���������.", LogLevel.Warning);
        return;
      }

      //������ ����� ���������� �����������
      try
      {
        _logger.Write(LogLevel.Debug, "������ �� ���������: " + _currentDocument.ToString());
        _fiscalDevice.Payment(_currentDocument);
      }
      catch (FiscalDeviceException err)
      {
        _logger.Write(err, "������ ���������� ������ � ���������� ������������!");
        throw new Exception(err.ToString());
      }

      //������ �� �����
      ShowMessage("�� �������� ����� �� �����������. ������������ ������ �� �����.", LogLevel.Warning);

      switch (_currentDocument.PaymentReason)
      {
        case PaymentReason.Parking:
        case PaymentReason.Fine:
          _smartCard.TimePayment = DateTime.Now;
          _smartCard.TimeExit = _currentDocument.TimeExit;
          _smartCard.Payment += (Decimal)_currentDocument.Amount; // ���������� � ����, ��� ��� �������� (��� ����� �����).
          _smartCard.CashBoxID = Settings.Default.CashNumber; // �������� ������ �����
          // ��������� ����� - ��������.
          if (_currentDocument.PaymentReason == PaymentReason.Fine)
            _smartCard.State = CardState.Penalty;
          else if (_currentDocument.PaymentReason == PaymentReason.Parking)
            _smartCard.State = CardState.Paid;

          // TODO: ���������� ������ �� �����
          // ������ ������ �� �����. ��������������� ������
          _logger.Write(LogLevel.Debug, "������ �� �����");
          WriteCard(_smartCard);

          if (_metroDiscount != null)
          {
            _metroDiscount.Time = DateTime.Now;
            _metroDiscount.State = MetroDiscountState.Paid;
            WriteMetroDiscount(_metroDiscount);
          }

          _trans.Write(DateTime.Now, _currentDocument.PaymentReason.GetString(), _currentDocument.Amount,
            _smartCard.TimeEntry, _smartCard.ZoneCurrentID, _cardID.ToString("X"));

          break;
        case PaymentReason.ECash:
          // ���������� ���������� ������������ ��������
          _smartCard.ECash += (Decimal)_currentDocument.Amount;
          WriteCard(_smartCard);

          _trans.Write(DateTime.Now, _currentDocument.PaymentReason.GetString(), _currentDocument.Amount,
            _smartCard.TimeEntry, _smartCard.ZoneCurrentID, _cardID.ToString("X"));

          break;
        case PaymentReason.Any:
          _trans.Write(DateTime.Now, _currentDocument.PaymentReason.GetString(), _currentDocument.Amount);
          break;
        case PaymentReason.Refund: // �������
          _trans.Write(DateTime.Now, _currentDocument.PaymentReason.GetString(), _currentDocument.Amount);
          break;
      }

      //send transaction to server
      //_device.AppendResponse(CreateCashTransaction35());

      //ok
      lblSumma.Text = String.Format("{0:C}", 0);
      _logger.Write(LogLevel.Debug, "������: ������ ������� ���������.");
      ShowMessage("�������� ������� ���������.", LogLevel.Information);
    }

    #endregion

    #region [ controls ]
    
    // �������������� ������ �� �����
    private void DisableControls()
    {
      btnPay.Enabled = false;
      btnPinalty.Enabled = false;
      btnCashOper.Enabled = false;
      btn_eCash.Enabled = false;
      btnRandomPayment.Enabled = false;
      btnRepayment.Enabled = false;
      dtpOut.Enabled = false;
    }

    // ������������ ������ �� �����
    private void EnableControls()
    {
      btnPay.Enabled = true;
      btnPinalty.Enabled = true;
      btnCashOper.Enabled = true;
      btn_eCash.Enabled = true;
      btnRandomPayment.Enabled = true;
      btnRepayment.Enabled = true;
      dtpOut.Enabled = true;
    }

    // �������������� ������ ����������� ��� ����������� �����
    private void NoCardControlState()
    {
      btnPay.Enabled = false;
      btnPinalty.Enabled = false;
      btnCashOper.Enabled = true;
      btn_eCash.Enabled = false;
      btnRandomPayment.Enabled = true;
      btnRepayment.Enabled = true;
      dtpOut.Enabled = false;
    }

    // �������������� ������ ����������� ��� ����������� �����
    private void CardPresentControlState()
    {
      btnPay.Enabled = false;
      btnPinalty.Enabled = true;
      btnCashOper.Enabled = true;
      btn_eCash.Enabled = true;
      btnRandomPayment.Enabled = false;
      btnRepayment.Enabled = false;
      dtpOut.Enabled = false;
    }

    // ��������� ������ ��� ����� �� ������
    private void CanPayControlState()
    {
      btnPay.Enabled = true;
      btnPinalty.Enabled = true;
      btnCashOper.Enabled = true;
      btn_eCash.Enabled = true;
      btnRandomPayment.Enabled = false;
      btnRepayment.Enabled = false;
      dtpOut.Enabled = true;
    }

    #endregion

    #region [ messages ]

    /// <summary>
    /// ���������� ��������� � ���� ���������.
    /// </summary>
    /// <param name="Message">���������</param>
    /// <param name="MessageType">��� ��������� (���������� ��� ���� - ������ - �������, �������������� - ���������
    /// ��������� - ������</param>
    /// <param name="Add">True - �������� ���������, False - ���������� �����</param>
    private void ShowMessage(String Message, LogLevel MessageType, bool Add)
    {
      switch (MessageType)
      {
        case LogLevel.Error:
          lblOperationDesc.ForeColor = Color.FromArgb(192, 64, 0);
          break;
        case LogLevel.Warning:
          lblOperationDesc.ForeColor = Color.Orange;
          break;
        case LogLevel.Information:
        case LogLevel.Debug:
        case LogLevel.Verbose:
        default:
          lblOperationDesc.ForeColor = Color.CadetBlue;
          break;
      }

      if (Add == true)
        lblOperationDesc.Text += "\r\n" + Message;
      else
        lblOperationDesc.Text = Message;
      
      lblOperationDesc.Update();
    }

    /// <summary>
    /// ���������� ��������� � ���� ���������.
    /// </summary>
    /// <param name="Message">���������</param>
    /// <param name="MessageType">��� ��������� (���������� ��� ���� - ������ - �������, �������������� - ���������
    /// ��������� - ������</param>
    private void ShowMessage(String Message, LogLevel MessageType)
    {
      ShowMessage(Message, MessageType, false);
    }

    private void ShowError(Exception err)
    {
      Exception ex = err;
      while (ex != null)
      {
        ShowMessage(ex.Message, LogLevel.Error, true);
        ex = ex.InnerException;
      }
    }

    #endregion

    #region [ network ]

    /// <summary>
    /// ������� ����� �� �������
    /// </summary>
    private void OnNetworkPacketReceived(Packet packet)
    {
      //System.Diagnostics.Trace.WriteLine("-> OnNetworkPacketReceived #" + packet.Address.ToString());

      if (packet.Address != _device.Device.NetworkAddress)
        return;

      _counterReceived++;
      if ((_counterReceived % _counterDelay) == 0)
        SwitchReceived();
      if (_counterReceived == Int32.MaxValue)
        _counterReceived = 0;

      //DateTime timeStart = DateTime.Now;

      Packet answer = _device.GetResponse(packet);
      if (_network.State == NetworkProtocolState.Opened)
        if (_network.SendPacket(answer))
        {
          _counterSent++;
          if ((_counterSent % _counterDelay) == 0)
            SwitchSent();
          if (_counterSent == Int32.MaxValue)
            _counterSent = 0;
        }

      //TimeSpan ts = DateTime.Now - timeStart;
      //if (ts > network.PacketTimeout)
      //  _logger.Write(LogLevel.Error, String.Format("�� ����� ��������, ������� = {0}", ts.ToString()), 2);

      _lastPacketTime = DateTime.Now;
    }

    ///// <summary>
    ///// ������ �������� ���������� (������ 3.5)
    ///// </summary>
    //private Packet CreateCashTransaction35()
    //{
    //  Packet p = new Packet(PacketType.Long);
    //  p.LongCommand = (byte)LongCommands.CashTransaction;

    //  byte[] data = p.Data;
    //  ManualCashBox c = _device.Device;

    //  //networkAddress
    //  data[0] = (byte)c.NetworkAddress;
    //  //version
    //  data[1] = 1;
    //  //type
    //  data[2] = (byte)CashMessageType.Transaction;
    //  //timeStamp
    //  Utils.PackDateTime(data, 3, DateTime.Now, false);
    //  //isOnline
    //  data[9] = 1;
    //  //lastSessionOpenTime
    //  //_fiscalDevice.
    //  //Utils.PackDateTime(data, 10, DateTime.Now.AddHours(-MathHelper.Rnd.Next(4, 24)), false);

    //  //transactionType
    //  CashTransactionType transactionType = IsOneOf(50) ? RuntimeHelper.GetRandomValue<CashTransactionType>() : CashTransactionType.Payment;
    //  data[16] = (byte)transactionType;
    //  //paymentMethod
    //  PaymentMethod m = PaymentMethod.BankingCard; //IsOneOf(5) ? RuntimeHelper.GetRandomValue<PaymentMethod>() : PaymentMethod.Cash;
    //  data[17] = (byte)m;
    //  //sessionID
    //  CashCounter c = SafeGetCounter(c.ID);
    //  short sessionID = (short)(c.GetTotalCount() & 0xFFFF);
    //  data[18] = (byte)(sessionID & 0xFF);
    //  data[19] = (byte)((sessionID >> 8) & 0xFF);
    //  //amount = amountInCurrentZone + debt - discount + fine - prepayment
    //  int amount = 0;
    //  int amountInCurrentZone = GenerateMoney(50, 5000);
    //  int debt = IsOneOf(5) ? 0 : GenerateMoney(50, 5000);
    //  amount = amountInCurrentZone + debt;
    //  int discount = (IsOneOf(10) ? amount : (IsOneOf(10) ? (int)(amount * Math.Min(MathHelper.Rnd.NextDouble(), 0.5)) : 0)) / MoneyDiscrete;
    //  discount *= MoneyDiscrete;
    //  amount -= discount;
    //  int fine = IsOneOf(10) ? 0 : GenerateMoney(50, 1000);
    //  amount += fine;
    //  int prepayment = IsOneOf(10) ? 0 : GenerateMoney(200, 2000);
    //  amount -= prepayment;
    //  amount = Math.Max(0, amount);
    //  BufferHelper.Int32ToBuffer(data, 20, MoneyConverter.FromMoney(amount));
    //  //completed
    //  data[24] = 0;

    //  long cardID = BufferHelper.BufferToInt32(transaction.Data, 8);
    //  if (((CardType)transaction.Data[13]).IsLightweight())
    //    cardID = DataExtensions.ConvertLightweightCardID(cardID);

    //  Card card = GetService<ICardManager>()[cardID];
    //  if (card != null)
    //  {
    //    //customerFacilityID
    //    data[25] = (byte)card.Type;
    //    //customerParams
    //    BufferHelper.Int32ToBuffer(data, 26, (int)(card.ID & 0xFFFFFFFF));
    //    //BufferHelper.IntToBuffer(data, 30, (int)(card.ID >> 32));
    //    //customerType
    //    data[46] = (byte)card.CustomerType;
    //    //customerGroupID
    //    data[47] = (byte)(card.CustomerGroupID & 0xFF);
    //  }

    //  Tariff tariff = GetService<ITariffManager>().Items.Random();
    //  if (tariff != null)
    //  {
    //    Tariff30 tariff30 = tariff as Tariff30;
    //    if (tariff30 != null)
    //    {
    //      //tariffType
    //      data[48] = (byte)tariff30.Type;
    //      //tariffGroup
    //      data[49] = (byte)tariff30.Class;
    //    }
    //    //tariffID
    //    data[50] = (byte)(tariff.ID & 0xFF);
    //  }

    //  //timeEntry
    //  DateTime dte = Utils.UnpackDateTime(transaction.Data, 40);
    //  Utils.PackDateTime(data, 51, dte, false);
    //  //timePayment
    //  DateTime dtp = dte.AddMinutes(MathHelper.Rnd.Next(5, 300));
    //  Utils.PackDateTime(data, 57, dtp, false);
    //  //timeExit
    //  Utils.PackDateTime(data, 63, IsOneOf(2) ? dtp : dtp.AddMinutes(-dtp.Minute).AddHours(1), false);
    //  //amountInCurrentZone
    //  BufferHelper.Int32ToBuffer(data, 69, MoneyConverter.FromMoney(amountInCurrentZone));
    //  //debt
    //  BufferHelper.Int32ToBuffer(data, 73, MoneyConverter.FromMoney(debt));
    //  //discount
    //  BufferHelper.Int32ToBuffer(data, 77, MoneyConverter.FromMoney(discount));
    //  //fine
    //  BufferHelper.Int32ToBuffer(data, 81, MoneyConverter.FromMoney(fine));
    //  //prepayment
    //  BufferHelper.Int32ToBuffer(data, 85, MoneyConverter.FromMoney(prepayment));
    //  //income
    //  int income = ((m == PaymentMethod.Cash) ? (IsOneOf(10) ? (int)(amount * 1.5) : amount) : amount) / MoneyDiscrete;
    //  income *= MoneyDiscrete;
    //  BufferHelper.Int32ToBuffer(data, 89, MoneyConverter.FromMoney(income));
    //  //change
    //  int change = income - amount;
    //  BufferHelper.Int32ToBuffer(data, 93, MoneyConverter.FromMoney(change));
    //  //amountLeftOnError
    //  int amountLeftOnError = ((m == PaymentMethod.Cash) ? (IsOneOf(100) ? (int)(change * Math.Min(MathHelper.Rnd.NextDouble(), 0.5)) : 0) : 0) / MoneyDiscrete;
    //  amountLeftOnError *= MoneyDiscrete;
    //  BufferHelper.Int32ToBuffer(data, 97, MoneyConverter.FromMoney(amountLeftOnError));

    //  //receiptState
    //  EjectionResult receiptState = (IsOneOf(20) ? RuntimeHelper.GetRandomValue<EjectionResult>() : EjectionResult.OK);
    //  data[101] = (byte)receiptState;
    //  //bankingCardState
    //  EjectionResult bankingCardState = ((m == PaymentMethod.BankingCard) ? (IsOneOf(20) ? RuntimeHelper.GetRandomValue<EjectionResult>() : EjectionResult.OK) : EjectionResult.Unknown);
    //  data[102] = (byte)bankingCardState;
    //  //ticketState
    //  EjectionResult ticketState = (IsOneOf(20) ? RuntimeHelper.GetRandomValue<EjectionResult>() : EjectionResult.OK);
    //  data[103] = (byte)ticketState;
    //  //errorCode
    //  byte errorCode = (byte)(((receiptState == EjectionResult.NotEjected) || (bankingCardState == EjectionResult.NotEjected) || (ticketState == EjectionResult.NotEjected)) ? MathHelper.Rnd.Next(1, 255) : 0);
    //  data[104] = errorCode;

    //  if (m == PaymentMethod.Cash)
    //  {
    //    IEnumerable<int> wb = null;
    //    IEnumerable<int> wc = null;
    //    int j = 105;
    //    if (amount > 0)
    //    {
    //      //banknotes
    //      int banknoteAmount = IsOneOf(10) ? (3 * amount / 5) : amount;
    //      int coinAmount = amount - banknoteAmount;
    //      int r;
    //      wb = MoneyConverter.BanknoteNominals.Weigh(banknoteAmount, out r);
    //      foreach (int n in wb)
    //        data[j++] = (byte)n;

    //      j += 2; //reserved
    //      coinAmount += r;

    //      CashFiscalState f = SafeGetFiscalState(c.ID);
    //      f.UpdateBanknotes(wb);

    //      //coins
    //      if (coinAmount > 0)
    //      {
    //        wc = MoneyConverter.CoinNominals.Weigh(MoneyConverter.FromMoney(coinAmount));
    //        foreach (int n in wc)
    //        {
    //          data[j] = (byte)n;
    //          j += 2;
    //        }

    //        f.UpdateCoins(wc);
    //      }
    //    }

    //    //upperDispenserCassetteOutput
    //    //BufferHelper.Int16ToBuffer(data, j, (wb != null) ? wb.Sum() : 0);
    //    j += 2;
    //    //middleDispenserCassetteOutput
    //    j += 2;
    //    //lowerDispenserCassetteOutput
    //    j += 2;
    //    //nearHopperOutput
    //    //BufferHelper.Int16ToBuffer(data, j, (wc != null) ? wc.Sum() : 0);
    //    j += 2;
    //    //farHopperOutput
    //    //j += 2;

    //    p.DataLength = 138;
    //  }
    //  else if (m == PaymentMethod.BankingCard)
    //  {
    //    //referenceNumber
    //    StringBuilder sb = new StringBuilder(12);
    //    for (int i = 0; i < 3; i++)
    //      sb.AppendFormat("{0:0000}", MathHelper.Rnd.Next(1, 9999));

    //    byte[] ba = CashMessageEncoding.GetBytes(sb.ToString());
    //    Buffer.BlockCopy(ba, 0, data, 105, 12);

    //    //answerCode
    //    ba = CashMessageEncoding.GetBytes("00");
    //    Buffer.BlockCopy(ba, 0, data, 117, 2);

    //    //card info
    //    sb.Length = 0;
    //    for (int i = 0; i < 4; i++)
    //      sb.AppendFormat("{0:0000}", MathHelper.Rnd.Next(1, 9999));
    //    string cn = sb.ToString();
    //    sb.Length = 0;
    //    sb.AppendFormat("{1}{0}{2}{0}{3}{0}{4}{0}", (char)0x1B,
    //      MathHelper.Rnd.Next(1, 9999999), cn, "VISA", String.Empty);
    //    ba = CashMessageEncoding.GetBytes(sb.ToString());
    //    Buffer.BlockCopy(ba, 0, data, 119, ba.Length);

    //    p.DataLength = (byte)(119 + ba.Length);
    //  }

    //  //update counters for extended status
    //  c.Update(m, amount, income, change, amountLeftOnError);
    //}

    #endregion

    #region [ status ]

    private void UpdateReaderState(string s)
    {
      lblReaderState.Text = String.Format("����������� : {0}", s);
    }

    private void UpdateFiscalState(string s)
    {
      lblFiscalState.Text = String.Format("�� : {0}", s);
    }

    private void SwitchReceived()
    {
      this.SafeInvoke(new Action(SwitchReceivedCore));
    }

    private void SwitchReceivedCore()
    {
      _indicatorReseived = !_indicatorReseived;
      tslblStatusReceived.Image = _indicatorReseived ? _imageReceived : _imageIdle;
    }

    private void SwitchSent()
    {
      this.SafeInvoke(new Action(SwitchSentCore));
    }

    private void SwitchSentCore()
    {
      _indicatorSent = !_indicatorSent;
      tslblStatusSent.Image = _indicatorReseived ? _imageSent : _imageIdle;
    }

    private void SwitchOff()
    {
      _indicatorReseived = true;
      SwitchReceivedCore();
      _indicatorSent = true;
      SwitchSentCore();
    }

    #endregion

    /// <summary>
    /// ���������� �������� ����� �� ���������
    /// </summary>
    private Tariff30 GetDefaultPenaltyTariff()
    {
      Tariff30 tariff = new Tariff30();
      tariff.Type = Tariff30Type.MoneyByPeriodTotal;
      tariff.Class = Tariff30Class.Penalty;
//      tariff.DailyRateCount = 1;
      tariff.DailyRateTimeUnitCost[0] = (Decimal)500;
      tariff.DailyRateTimeUnitLength[0] = new TimeSpan(1, 0, 0);

      return tariff;
    }

    /// <summary>
    /// ���������� ����� ����� �� ���������
    /// </summary>
    private Tariff GetDefaultMetroTariff()
    {
      return new TariffMetro();
    }

    /// <summary>
    /// ���������� ������ ������� �� �����
    /// </summary>
    private void UpdateParameters(DateTime timeExit)
    {
      //check
      if ((_smartCard == null) || (_currentDocument == null)) // �� ��� ������
      {
        lblTimeIn.Text = DateTime.Now.DateToString(); // ToString("dd MMMM yyyy  H:mm:ss");
        dtpOut.Value = DateTime.Now;
        lblTimeInParking.Text = _NO_DATA;
        lblTimePaid.Text = _NO_DATA;
        lblExitTimeLimit.Text = _NO_DATA;

        lblPaid.Text = String.Format("{0:C}", 0);
        lblDebt.Text = String.Format("{0:C}", 0);
        lblSumma.Text = String.Format("{0:C}", 0);
        lblECash.Text = String.Format("{0:C}", 0);

        return;
      }

      //process
      lblTimeIn.Text = _smartCard.TimeEntry.DateToString();
      dtpOut.Value = timeExit;
      lblTimeInParking.Text = Utils.TimeToString(DateTime.Now - _currentDocument.TimeEntry);// ����� �� �������;
      lblPaid.Text = _currentDocument.Payment.ToString("C");
      lblDebt.Text = _currentDocument.Debt.ToString("C");
      lblECash.Text = _currentDocument.ECash.ToString("C");
      if (Settings.Default.DisplayEnabled) // ����� �� �������
      {
        _display.Clear();
        _display.Text(1, GetLocalizedDateTimeString(lblTimeInParking.Text)); // ����� ����������� �� ��������
      }

      if (_currentDocument.Amount <= 0) // ������ �� ���������
      {
        lblTimePaid.Text = Utils.TimeToString(_currentDocument.TimeExit - DateTime.Now); // � ������ ������ ����� ���������� �� �����
        lblExitTimeLimit.Text = _currentDocument.TimeExit.DateToString();//����� ��
        lblSumma.Text = String.Format("{0:C}", 0);
        if (Settings.Default.DisplayEnabled)
          _display.Text(2, String.Format(_rm.GetString("InPay") + ": {0:C}", 0));
      }
      else // ��������� ������
      {
        lblTimePaid.Text = _NEED_PAY; // � ������ ������ ����� ���������� �� �����
        lblExitTimeLimit.Text = _NEED_PAY;  // ����� ��
        lblSumma.Text = String.Format("{0:C}", _currentDocument.Amount);
        try
        {
          if (Settings.Default.DisplayEnabled)
            _display.Text(2, String.Format(_rm.GetString("InPay") + ": {0:C}", _currentDocument.Amount));
        }
        catch (Exception ex)
        {
          MessageBox.Show(this, ex.Message, ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    /// <summary>
    /// ����������� ���� � �������������� ������
    /// </summary>
    private string GetLocalizedDateTimeString(string s)
    {
      s = s.Replace("���", _rm.GetString("Days")).Replace("���", _rm.GetString("Hour")).Replace("���", _rm.GetString("Min")).Replace("���", _rm.GetString("Sec"));
      return s;
    }
  }
}