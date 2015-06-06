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

    private const string NO_DATA = "Нет данных";
    private const string NEED_PAY = "Требуется оплата";

    private const string READER_NOT_FOUND = "Считыватель не найден или занят другим приложением!\r\n";
    private const string FISCAL_DEVICE_NOT_FOUND = "ККМ не найдена. \r\nПроверьте питание и подключение ККМ, а так же настройки программы.\r\n";
    private const string DISPLAY_NOT_FOUND = "Дисплей пользователя не найден. \r\nПроверьте питание и подключение дисплея, а так же настройки программы.\r\n";
    private const string READ_CARD_ERROR = "Ошибка чтения карты. Попробуйте еще раз.\r\n";
    private const string TARIF_ERROR = "Ошибка при попытке обработки тарифа. Возможно неверный тириф на карте.\r\n";
    private const string TARIF_PARAM_ERROR = "Ошибка в параметрах тарифа. Возможно неверный тириф на карте.\r\n";
    private const string TARIF_DATEOUT_ERROR = "Ошибка в определении времени выезда.\r\n";
    private const string ERROR_LOAD_KEYS = "Ошибка загрузки ключей в считыватель\r\n";
    private const string DECRYPT_ERROR = "Ошибка дешифрования ключей. Повторите настройку АРМ.\r\n";
    private const string PAY_ERROR = "Ошибка оплаты!\r\n";
    private const string PASS_ERROR = "Неверный пароль. Доступ запрещен!\r\n";
    private const string NETWORK_CREATE_ERROR = "Ошибка при создании сетевого протокола";

    #endregion

    private static int CounterDelay = Settings.Default.CounterDelay;

    // Для таскания мышкой за любую часть формы
    private bool dragging;
    private int dragX, dragY;

    private ILogger logger;       // Лог
    private IFiscalDevice fiscalDevice; // Класс работы с ККМ
    private UserDisplay display; // Дисплей клиента
    private SmartCardManager smartCardManager; // Менеджер карт
    private ISmartCardReader smartCardReader; // ридер
    private ICalculator calculator; // Калькулятор
    private CashTransactions trans; // Позволяет записывать транзакции оплаты в файл CSV
    private Tariff penaltyTariff;

    // Данные по текущей операции
    private PaymentDocument currentDocument; // Сумма оплаты и другие параметры по текущей операции
    private int cardId;
    private ISmartCardPx smartCard;
    // Флаг - Время выезда на DateTimePicker может быть изменено пользователем
    private bool timeExitReadyToChange;

    private ResourceManager rm;
    private readonly CultureInfo cultureInfo;
    private SettingsForm configForm;
    private ParkingAboutForm aboutForm;

    private MetroDiscount metroDiscount;
    private Tariff metroTariffK;
    private Tariff metroTariffL;
    private ICalculator calculatorMetro;

    private bool indicatorReseived;
    private bool indicatorSent;
    private Image imageIdle;
    private Image imageReceived;
    private Image imageSent;

    private VirtualCashBox device;
    private INetworkProtocol network;
    
    private int counterReceived;
    private int counterSent;
    private readonly int counterDelay;
    private DateTime lastPacketTime;

    public MainForm()
    {
      InitializeComponent();

      this.SetLogoIcon();

      cultureInfo = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = cultureInfo;
      Thread.CurrentThread.CurrentUICulture = cultureInfo;

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

      aboutForm = null;

      imageList.Images.Add(Images.Exit);
      imageList.Images.Add(Images.Settings);
      imageList.Images.Add(Images.Help);
      btnExit.ImageIndex = 0;
      btnSetup.ImageIndex = 1;
      btnAbout.ImageIndex = 2;

      dragging = false;
      dragX = 0;
      dragY = 0;

      penaltyTariff = null;

      metroDiscount = null;
      metroTariffK = null;
      metroTariffL = null;
      calculatorMetro = null;

      counterReceived = 0;
      counterSent = 0;
      counterDelay = CounterDelay;
      lastPacketTime = DateTime.Now;
    }

    #region [ window events ]

    private void MainForm_Load(object sender, EventArgs e)
    {
      //logger
      CashDeskPathManager pathManager = new CashDeskPathManager();
      ILogWriter writer = new FileLogWriter(pathManager.LogPath, "CashDesk");
      ILogFormatter formatter = new MultilineStringLogFormatter();
      logger = new Logger(1000, new ILogWriter[] { writer }, formatter);
      logger.Threshold = Settings.Default.LogLevel;
      logger.Write(LogLevel.Debug, "Запуск программы");

      //init controls
      CheckConfiguration();

      ShowMessage("Для оплаты парковки, штрафа или пополнения электронного кошелька приложите карту.", LogLevel.Information);
      ShowMessage("Для произвольной оплаты или возврата денег карта не используется.", LogLevel.Information, true);

      ShowTimeTimer.Enabled = true; // Запуск таймера отображения текущего времени на форме

      lblPenaltyCount.Text = Settings.Default.PenaltyCardCount.ToString();

      //create calculator
      calculator = CalculatorLoader.CreateCalculator(pathManager.CalculatorsPath, Settings.Default.CalculatorVersion);
      calculatorMetro = new CalculatorMetro();

      //get devices
      fiscalDevice = GetFiscalDevice(); // #### Подключение ККМ из файла конфигурации ####
      UpdateFiscalState(fiscalDevice.ToString());
      display = GetDisplay(); // Дисплей клиента ################################
      string s = Application.StartupPath + @"\Transaction.csv";
      trans = new CashTransactions(s, new CSVFileWriter(s));

      //smart card reader
      UpdateReaderState("нет карты");
      smartCardReader = GetReader(pathManager.CardReadersPath);
      if (smartCardReader != null)
      {
        smartCardManager = new SmartCardManager(smartCardReader, null, GetKeys()); // Работа с картой
        smartCardReader.ConnectedCard += () =>
        {
          this.SafeInvoke(new Action(OnCardConnected)); // Собитие - карта на ридере 
        };
        smartCardReader.RemovedCard += () =>
        {
          this.SafeInvoke(new Action(OnCardDisconnected)); // Собитие - карту убрали с ридера 
        };

        // Если при запуске карта уже лежала на считывателе
        if (smartCardReader.CardState == SCardState.Connected)
          OnCardConnected();
        else
          OnCardDisconnected();

        //s = Settings.Default.ReaderType.GetString();
      }
      else
        UpdateReaderState("недоступен");


      //indicators
      indicatorReseived = false;
      indicatorSent = false;
      imageIdle = Resources.idle;
      imageReceived = Resources.received;
      imageSent = Resources.sent;

      //network
      device = new VirtualCashBox(GetCashBox());
      lblCaption.Text = String.Format("АРМ Ручная касса #{0}", device.Device.NetworkAddress);
      network = GetNetworkProtocol();
      if (network != null)
        timerIndicator.Start();
      else
      {
        tslblStatusReceived.Visible = false;
        tslblStatusSent.Visible = false;
        tslblStatusNetwork.Text += " недоступна";
      }

      //create config form
      configForm = new SettingsForm(logger, smartCardManager);

      //apply localization
      Assembly assembly = Assembly.GetExecutingAssembly();
      rm = new ResourceManager("Parking.CashDesk.Localization.Translate", assembly);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      logger.Write(LogLevel.Debug, "Закрытие программы");

      timerIndicator.Stop();
      if (network != null)
        network.Close();

      // Освобождение считывателя
      if (smartCardReader != null)
        smartCardReader.Dispose();

      if (smartCardManager != null)
        smartCardManager.Dispose();

      if (fiscalDevice != null)
        fiscalDevice.Dispose(); // Заверщение работы с ККМ

      ((Logger)logger).Dispose();

      ShowTimeTimer.Enabled = false;
    }

    private void MainForm_MouseMove(object sender, MouseEventArgs e)
    {
      if (!dragging)
        return;

      var x = e.X - dragX;
      var y = e.Y - dragY;
      Point newpos = new Point(Location.X + x, Location.Y + y);
      Location = newpos;
    }

    private void MainForm_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;

      dragging = true;
      dragX = e.X;
      dragY = e.Y;
      Cursor.Current = System.Windows.Forms.Cursors.SizeAll;
    }

    private void MainForm_MouseUp(object sender, MouseEventArgs e)
    {
      dragging = false;
      Cursor.Current = System.Windows.Forms.Cursors.Arrow;
    }

    private void btnPay_Click(object sender, EventArgs e)
    {
      // запретим управление
      DisableControls();
      if (currentDocument == null)
      {
        logger.Write(LogLevel.Warning, "Нет данных по расчету. Оплата невозможна.");
        MessageBox.Show(this, "Нет данных по расчету. Оплата невозможна.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        // Нет данных по расчету, значит и кнопки активировать не нужно.
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

      // Перерасчет на основе оплаты. Что бы сумма постоянно не нарасщивалась будем использовать время для которого
      // производился расчет перед полатой (оно сохранилось только на форме, в остальных местах оно заменилось на расчи-
      // танное время выезда).
      try
      {
        Calculate(dtpOut.Value); // Расчет суммы оплаты
      }
      catch (Exception ex)
      {
        ShowError(ex);
      }

      currentDocument = null;

      // разрешим управление
      EnableControls();
    }

    private void btnCashOper_Click(object sender, EventArgs e)
    {
      using (CashControl frmCashControl = new CashControl(fiscalDevice, trans, logger))
        frmCashControl.ShowDialog();
    }

    private void ShowTimeTimer_Tick(object sender, EventArgs e)
    {
      lblTime.Text = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
    }

    private void btnPenalty_Click(object sender, EventArgs e)
    {
      //запретим управление
      DisableControls();

      // Подготовка штрафной карты
      // Для создания штрафной карты используется небольшая хитрость:
      // Время въезда выставляется в текущее. В результате расчет начинается
      // с текущего времени, позволяя взять сразу фиксированную сумму.
      // Дальнейшее зависит от настройки штрафного тарифа. Обычно после
      // фиксированной суммы он повторяет основной тариф.
      TimeSpan tsTemp = new TimeSpan(0, 0, 0, 10); // создаем искуственную разницу между въездом и выездом - 10сек. А то калькулятор ругается.
      smartCard.TimeEntry = DateTime.Now - tsTemp;
      smartCard.TimeExit = DateTime.Now;

      // Настройка карты как въехавшей, не заблокированной, ...
      smartCard.CounterEntry = 1;
      smartCard.CounterExit = 0;
      smartCard.Blocked = false;
      smartCard.Payment = 0;
      smartCard.Debt = 0;
      smartCard.Discount = 0;

      smartCard.CustomerType = CustomerType.OnceOnly;
      smartCard.ZoneCurrentID = 1; //TODO: сделать возможность выбора зоны.
      smartCard.ZonePreviousID = 0;

      // Заменяем тариф на штрафной
      smartCard.Tariff = penaltyTariff;
      smartCard.State = CardState.Penalty;

      // Расчитываем сумму оплаты (true - по штрафному тарифу)
      try
      {
        Calculate(smartCard.TimeExit);
      }
      catch (Exception ex)
      {
        ShowError(ex);
        return;
      }

      UpdateParameters(currentDocument.TimeExit);

      try
      {
        Pay();
      }
      catch (Exception ex)
      {
        ShowError(ex);
        return;
      }

      // Перерасчет на основе оплаты. Что бы сумма постоянно не нарасщивалась будем использовать время для которого
      // производился расчет перед оплатой (оно сохранилось только на форме, в остальных местах оно заменилось на расчи-
      // танное время выезда).
      try
      {
        Calculate(smartCard.TimeExit);
      }
      catch (Exception ex)
      {
        ShowError(ex);
        currentDocument = null;
        return;
      }

      currentDocument = null;

      ShowMessage("Штрафная карта создана. Передайте карту клиенту.", LogLevel.Information);
      // Запоминаем число выданных штрафных тарифов
      Settings.Default["PenaltyCardCount"] = Settings.Default.PenaltyCardCount + 1;
      Settings.Default.Save();
      lblPenaltyCount.Text = Settings.Default.PenaltyCardCount.ToString(); // Отображает число выданных штрафных карт

      // разрешать управление не будем. Пусть оператор уберет карту со считывателя
    }

    private void dtpOut_ValueChanged(object sender, EventArgs e)
    {
      if (!timeExitReadyToChange)
        return;

      timeExitReadyToChange = false;
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
      // Окно ввода вносимой суммы
      string sSumm = String.Empty;
      if (InputBox.Show("Пополнение Электронного Кошелька", "Введите сумму", true, ref sSumm) != DialogResult.OK)
        return;

      long summ = 0;
      if (!String.IsNullOrEmpty(sSumm))
        summ = Convert.ToInt64(sSumm); // Сумма
      else
      {
        EnableControls();
        return;
      }
      currentDocument = new PaymentDocument(summ, PaymentReason.ECash, DocumentType.Selling,
        PaymentType.Cash, cardId, DateTime.Now, DateTime.Now, 0, 0, (double)smartCard.ECash);

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
      // Окно ввода вносимой суммы
      string sSumm = String.Empty;
      if (InputBox.Show("Платеж на произвольную сумму", "Введите сумму", true, ref sSumm) != DialogResult.OK)
        return;

      long summ = 0;
      if (!String.IsNullOrEmpty(sSumm))
        summ = Convert.ToInt64(sSumm); // Сумма
      else
      {
        EnableControls();
        return;
      }
      currentDocument = new PaymentDocument(summ, PaymentReason.Any, DocumentType.Selling,
        PaymentType.Cash, cardId, DateTime.Now, DateTime.Now, 0, 0, 0);

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
      // Окно ввода вносимой суммы
      string sSumm = String.Empty;
      if (InputBox.Show("Возврат", "Введите сумму", true, ref sSumm) != DialogResult.OK)
        return;

      long summ = 0;
      if (!String.IsNullOrEmpty(sSumm))
        summ = Convert.ToInt64(sSumm); // Сумма в копейках
      else
      {
        EnableControls();
        return;
      }

      currentDocument = new PaymentDocument(summ, PaymentReason.Refund, DocumentType.Repayment,
        PaymentType.Cash, cardId, DateTime.Now, DateTime.Now, 0, 0, 0);

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
      if (aboutForm == null)
        aboutForm = new ParkingAboutForm();

      aboutForm.ShowAbout(this);
    }

    private void btnSetup_Click(object sender, EventArgs e)
    {
      PasswordForm passwordForm = new PasswordForm("Введите пароль");
      if (passwordForm.ShowDialog() != DialogResult.OK)
        return;

      if (passwordForm.edPass.Text != Crypt.Decrypt(Settings.Default.Password, "t23F78C4"))
      {
        ShowMessage(PASS_ERROR, LogLevel.Error);
        return;
      }
      
      configForm.ShowDialog();

      try
      {
        penaltyTariff = CashDeskHelper.DeserializeTariff(Settings.Default.PenaltyTarif);
      }
      catch (Exception ex)
      {
        logger.Write(ex, "Ошибка чтения штрафного тарифа");
      }

      try
      {
        metroTariffK = CashDeskHelper.DeserializeTariff(Settings.Default.MetroTariffK);
        metroTariffL = CashDeskHelper.DeserializeTariff(Settings.Default.MetroTariffL);
      }
      catch (Exception ex)
      {
        logger.Write(ex, "Ошибка чтения тарифов метро");
      }
    }

    private void timerIndicator_Tick(object sender, EventArgs e)
    {
      if ((DateTime.Now - lastPacketTime).TotalSeconds > 1)
        SwitchOff();
    }

    #endregion

    #region [ initialization ]

    /// <summary>
    /// Возвращает объект подключенного дисплея покупателя
    /// </summary>
    private UserDisplay GetDisplay()
    {
      UserDisplay userDisplay = null;
      if (Settings.Default.DisplayEnabled) // Если требуется выводить на дисплей
      {
        userDisplay = new UserDisplay(logger);
        int nState = userDisplay.Initialize(Settings.Default.DisplayPort, Settings.Default.DisplayCodePage);
        if (nState != 0)
        {
          logger.Write(LogLevel.Error, "Невозможно инициализировать дисплей клиента. Возможно не найдена VFDDisplay.dll.");
          ShowMessage(DISPLAY_NOT_FOUND, LogLevel.Error, true);
        }
        else
        {
          userDisplay.Clear();
          logger.Write(LogLevel.Debug, "Дисплей подключен.");
        }
      }
      else
        logger.Write(LogLevel.Debug, "Подключение дисплея не требуется.");

      return userDisplay;
    }

    /// <summary>
    /// Возвращает объект фискального устройства
    /// </summary>
    private IFiscalDevice GetFiscalDevice()
    {
        IFiscalDevice f = null;
       if (Settings.Default.FiscalDeviceEnabled)
              f = new Start200Control(); // Старт-200К
      else
        f = new VirtualFiscalDevice(); // Без фискальника

      try
      {
          f.LogFileOn(Settings.Default.LogOn);
          f.Initialize(Settings.Default.FiscalDevicePort, Settings.Default.CashNumber.ToString()); // Инициализация ККМ
      }
      catch (FiscalDeviceException err)
      {
          logger.Write(err, "Невозможно инициализировать ККМ. Возможно не найдена Azimuth.dll.\r\n" + err.ToString());

        ShowMessage(FISCAL_DEVICE_NOT_FOUND, LogLevel.Error, true);
      }

      logger.Write(LogLevel.Debug, "ККМ подключена.");

      return f;
    }

    /// <summary>
    /// Загружает ключи из файла конфигурации и расшифровывает их
    /// </summary>
    private Dictionary<int, IAccessKey> GetKeys()
    {
      Dictionary<int, IAccessKey> keys = new Dictionary<int, IAccessKey>();
      try
      {
        byte[] keyB1 = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyB1, "t23F78C4"));
        keys.Add(1, new AccessKey(KeyType.KeyB, keyB1)); // B1
        byte[] keyB2 = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyB2, "t23F78C4"));
        keys.Add(2, new AccessKey(KeyType.KeyB, keyB2)); // B2
        byte[] keyB3 = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyB3, "t23F78C4"));
        keys.Add(3, new AccessKey(KeyType.KeyB, keyB3)); // B3
        keys.Add(4, new AccessKey(KeyType.KeyB, keyB3)); // B3
        keys.Add(5, new AccessKey(KeyType.KeyB, keyB3)); // B3
        keys.Add(6, new AccessKey(KeyType.KeyB, keyB3)); // B3
        byte[] keyB4 = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyB4, "t23F78C4"));
        keys.Add(7, new AccessKey(KeyType.KeyB, keyB4)); // B4
        keys.Add(8, new AccessKey(KeyType.KeyB, keyB4)); // B4
        keys.Add(9, new AccessKey(KeyType.KeyB, keyB4)); // B4
        byte[] keyBm = AccessKeyHelper.GetKeyFromString(Crypt.Decrypt(Settings.Default.KeyBM, "t23F78C4"));
        keys.Add(SmartCardLayoutHelper.MetroSectorIndex, new AccessKey(KeyType.KeyB, keyBm)); // BM
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
        
        ShowMessage(DECRYPT_ERROR, LogLevel.Error, true);
        logger.Write(ex, @"Ошибка дешифрования ключей. Возможно отсутствует файл конфигурации. Установлены ключи по умолчанию.");
      }

      return keys;
    }

    /// <summary>
    /// Возвращает объект ридера согласно указанному в файле конфигурации
    /// </summary>
    private ISmartCardReader GetReader(string cardReadersPath)
    {
      ISmartCardReader reader = null;
      try
      {
        reader = CardReaderLoader.CreateCardReader(cardReadersPath, Settings.Default.ReaderType);
        SmartCardOptions.EnableMetroSector = Settings.Default.UseMetro;

        logger.Write(LogLevel.Debug, String.Format("Найден считыватель {0}", Settings.Default.ReaderType.GetString()));
      }
      catch (Exception err)
      {
        logger.Write(err, "Считыватель не найден.", 0);
        ShowMessage(READER_NOT_FOUND, LogLevel.Error, true);
      }

      return reader;
    }

    /// <summary>
    /// Создает устройство
    /// </summary>
    private ManualCashBox GetCashBox()
    {
        var c = new ManualCashBox
            {
                ID = Settings.Default.DeviceID,
                NetworkAddress = Settings.Default.DeviceNetworkAddress,
                Name = "Ручная касса"
            };


        return c;
    }

    /// <summary>
    /// Создает сетевой протокол
    /// </summary>
    private INetworkProtocol GetNetworkProtocol()
    {
      INetworkProtocol n = null;
      try
      {
        n = new TcpProtocolServer(logger);
        n.PacketReceived += OnNetworkPacketReceived;
        n.Open();
      }
      catch (Exception e)
      {
        logger.Write(e, NETWORK_CREATE_ERROR);
        ShowMessage(NETWORK_CREATE_ERROR, LogLevel.Error, true);
      }

      return n;
    }

    /// <summary>
    /// Создание необходимых параметров файла конфигурации
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
        MessageBox.Show(this, "Ошибка чтения штрафного тарифа из файла конфигурации. Загрузите тариф с мастер-карты.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      else
      {
        try
        {
          penaltyTariff = CashDeskHelper.DeserializeTariff(Settings.Default.PenaltyTarif);
        }
        catch (Exception e)
        {
          penaltyTariff = GetDefaultPenaltyTariff();
          logger.Write(e, String.Format("Ошибка чтения штрафного тарифа из файла конфигурации\r\nТариф:\r\n{0}", Settings.Default.PenaltyTarif));
          MessageBox.Show(this, "Ошибка чтения штрафного тарифа из файла конфигурации. Загрузите тариф с мастер-карты.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }

      if (Settings.Default.UseMetro)
        if (String.IsNullOrEmpty(Settings.Default.MetroTariffK) || String.IsNullOrEmpty(Settings.Default.MetroTariffL))
        {
          string s = CashDeskHelper.SerializeTariff(GetDefaultMetroTariff());
          Settings.Default["MetroTariffK"] = s;
          Settings.Default["MetroTariffL"] = s;
          MessageBox.Show(this, "Ошибка чтения тарифов метро из файла конфигурации. Загрузите тарифы метро из файлов.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
          try
          {
            metroTariffK = CashDeskHelper.DeserializeTariff(Settings.Default.MetroTariffK);
            metroTariffL = CashDeskHelper.DeserializeTariff(Settings.Default.MetroTariffL);
          }
          catch (Exception e)
          {
            metroTariffK = GetDefaultMetroTariff();
            metroTariffL = GetDefaultMetroTariff();
            logger.Write(e, String.Format("Ошибка чтения тарифов метро из файла конфигурации\r\n{0}\r\n{1}", Settings.Default.MetroTariffK, Settings.Default.MetroTariffL));
            MessageBox.Show(this, "Ошибка чтения тарифов метро из файла конфигурации. Загрузите тарифы метро из файлов.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
      UpdateReaderState(DataFormatter.ToHex(smartCardReader.ReadCardNumber()));

      DisableControls();
      currentDocument = null;

      ShowMessage("Производится рачет суммы оплаты. Пожалуйста ждите...", LogLevel.Information);

      smartCard = ReadCard(out cardId);
      if (smartCard == null)
      {
        ShowMessage(READ_CARD_ERROR, LogLevel.Error);
        return; // Ошибка чтения карты
      }

      metroDiscount = ReadMetroDiscount();

      CardPresentControlState();

      try
      {
        Calculate(DateTime.Now); // Расчет суммы оплаты
      }
      catch (Exception err)
      {
        ShowError(err);
        return;
      }

      if (currentDocument.Amount > 0)
        CanPayControlState(); // Если есть, что платить, то активируем кнопку.
    }

    private void OnCardDisconnected()
    {
      UpdateReaderState("нет карты");
      NoCardControlState();
      timeExitReadyToChange = false;
      smartCard = null;
      metroDiscount = null;
      cardId = 0;
      currentDocument = null;
      UpdateParameters(DateTime.Now);
      ShowMessage("Для оплаты парковки, штрафа или пополнения электронного кошелька приложите карту.", LogLevel.Information);
      ShowMessage("Для произвольной оплаты или возврата денег карта не используется.", LogLevel.Information, true);
    }

    private ISmartCardPx ReadCard(out int cardID)
    {
      SmartCardsP3x smartCard = new SmartCardsP3x();
      cardID = 0;

      try
      {
        cardID = (int)smartCardManager.GetCardID();
        smartCardManager.ReadCard(smartCard);
        logger.Write(LogLevel.Debug, "Карта успешно прочитана");
      }
      catch (Exception err)
      {
        logger.Write(err, "Ошибка чтения карты: Невозможно прочитать информацию на карте");
        EnableControls();
        return null;
      }

      return smartCard;
    }

    private void WriteCard(ISmartCardPx smartCard)
    {
      try
      {
        smartCardManager.WriteCard(smartCard);
        logger.Write(LogLevel.Debug, "Карта успешно записана");
      }
      catch (Exception err)
      {
        logger.Write(err, "Ошибка записи карты: Невозможно записать информацию на карту");
        throw new Exception(READ_CARD_ERROR, err);
      }
    }

    private MetroDiscount ReadMetroDiscount()
    {
      MetroDiscount md = null;
      if (Settings.Default.UseMetro)
        try
        {
          //check tariff
          Tariff30 t = (Tariff30)smartCard.Tariff;
          if (t.Type == Tariff30Type.WeekeySimple)
          {
            //read sector
            byte[] ba = new byte[SmartCardLayoutHelper.UserSectorSize];
            smartCardManager.Reader.ReadSector(SmartCardLayoutHelper.MetroSectorIndex,
              smartCardManager.KeysB[SmartCardLayoutHelper.MetroSectorIndex], ba, 0);

            md = new MetroDiscount();
            md.UnpackData(ba);

            //int parkingID = md.ParkingID;
            //if ((parkingID > 0) && (parkingID != Settings.Default.ParkingID))
            //{
            //  md = null;
            //  throw new ApplicationException(String.Format("Идентификатор парковки в секторе метро ({0}) не совпадает со значением в файле конфигурации ({1})", parkingID, Settings.Default.ParkingID));
            //}
          }
        }
        catch (Exception e)
        {
          logger.Write(LogLevel.Warning, String.Format("Ошибка чтения информации из сектора метро\r\n{0}", e.Message));
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

        smartCardManager.Reader.WriteSector(SmartCardLayoutHelper.MetroSectorIndex,
          smartCardManager.KeysB[SmartCardLayoutHelper.MetroSectorIndex], ba);
      }
      catch (Exception e)
      {
        logger.Write(LogLevel.Warning, String.Format("Ошибка записи информации о скидке\r\n{0}", e.Message));
      }
    }

    #endregion

    #region [ calculation and payment ]

    /// <summary>
    /// Расчет суммы оплаты и создание документа
    /// </summary>
    /// <param name="TimeOut">Желаемое время выезда</param>
    private void Calculate(DateTime TimeOut)
    {
      timeExitReadyToChange = false;
      Decimal ToPay = 0;
      DateTime EstimatedTimeOut;

      //select tariff and calculator
      Tariff t = smartCard.Tariff;
      ICalculator c = calculator;
      if (metroDiscount != null)
      {
        bool b = metroDiscount.CanUseDiscount;
        logger.Write(LogLevel.Debug, String.Format("Расчёт по тарифу метро : ", b ? "льготный" : "коммерческий"));

        t = b ? metroTariffL : metroTariffK;
        if (t is TariffMetro)
          c = calculatorMetro;
      }

      //calculate
      try
      {
        ToPay = c.Calculate(t, smartCard.TimeEntry, TimeOut, out EstimatedTimeOut);
      }
      catch (Exception err)
      {
        logger.Write(err, "Ошибка при расчете суммы оплаты");
        throw new Exception("Ошибка при расчете суммы оплаты:\r\n", err);
      }

      decimal discountTotal = 0;
      for (int i = 0; i < smartCard.ServicesCount; i++)
        discountTotal += smartCard.Discounts[i].Amount;

      // Сумма = Расчетное значение + Сумма задолжности - Оплаченная сумма + Сумма услуг/скидок
      decimal amount = ToPay + smartCard.Debt - smartCard.Payment + discountTotal; // - _smartCard.Discount;
      logger.Write(LogLevel.Debug, "Сумма к оплате: " + amount.ToString("C"));
      currentDocument = new PaymentDocument((double)amount, PaymentReason.Parking, DocumentType.Selling,
        PaymentType.Cash, cardId, smartCard.TimeEntry, EstimatedTimeOut, (double)smartCard.Payment,
        (double)smartCard.Debt, (double)smartCard.ECash);

      logger.Write(LogLevel.Debug, "Создан документ: " + currentDocument.ToString());

      UpdateParameters(TimeOut); // Обновление данных на форме
      ShowMessage("Расчет суммы оплаты успешно завершен.", LogLevel.Information);

      timeExitReadyToChange = true;// Реагировать на событие изменения даны выезда
    }

    /// <summary>
    /// Оплата, запись на карту и печать чека
    /// </summary>
    private void Pay()
    {
      ShowMessage("Производится операция оплаты. Пожалуйста ждите...", LogLevel.Information);

      //проверим, платить-то надо?
      if (currentDocument.Amount <= 0)
      {
        ShowMessage("Оплата не требуется.", LogLevel.Warning);
        return;
      }

      //Оплата через фискальный регистратор
      try
      {
        logger.Write(LogLevel.Debug, "Оплата по документу: " + currentDocument.ToString());
        fiscalDevice.Payment(currentDocument);
      }
      catch (FiscalDeviceException err)
      {
        logger.Write(err, "Ошибка проведения оплаты в фискальном регистраторе!");
        throw new Exception(err.ToString());
      }

      //Запись на карту
      ShowMessage("Не убирайте карту со считывателя. Производится запись на карту.", LogLevel.Warning);

      switch (currentDocument.PaymentReason)
      {
        case PaymentReason.Parking:
        case PaymentReason.Fine:
          smartCard.TimePayment = DateTime.Now;
          smartCard.TimeExit = currentDocument.TimeExit;
          smartCard.Payment += (Decimal)currentDocument.Amount; // Прибавляем к тому, что уже оплачено (это очень важно).
          smartCard.CashBoxID = Settings.Default.CashNumber; // Передача номера кассы
          // Установка флага - оплачено.
          if (currentDocument.PaymentReason == PaymentReason.Fine)
            smartCard.State = CardState.Penalty;
          else if (currentDocument.PaymentReason == PaymentReason.Parking)
            smartCard.State = CardState.Paid;

          // TODO: переделать запись на карту
          // Запись оплаты на карту. Непосредственно запись
          logger.Write(LogLevel.Debug, "Запись на карту");
          WriteCard(smartCard);

          if (metroDiscount != null)
          {
            metroDiscount.Time = DateTime.Now;
            metroDiscount.State = MetroDiscountState.Paid;
            WriteMetroDiscount(metroDiscount);
          }

          trans.Write(DateTime.Now, currentDocument.PaymentReason.GetString(), currentDocument.Amount,
            smartCard.TimeEntry, smartCard.ZoneCurrentID, cardId.ToString("X"));

          break;
        case PaymentReason.ECash:
          // Переделать пополнение электронного кошелька
          smartCard.ECash += (Decimal)currentDocument.Amount;
          WriteCard(smartCard);

          trans.Write(DateTime.Now, currentDocument.PaymentReason.GetString(), currentDocument.Amount,
            smartCard.TimeEntry, smartCard.ZoneCurrentID, cardId.ToString("X"));

          break;
        case PaymentReason.Any:
          trans.Write(DateTime.Now, currentDocument.PaymentReason.GetString(), currentDocument.Amount);
          break;
        case PaymentReason.Refund: // Возврат
          trans.Write(DateTime.Now, currentDocument.PaymentReason.GetString(), currentDocument.Amount);
          break;
      }

      //send transaction to server
      //_device.AppendResponse(CreateCashTransaction35());

      //ok
      lblSumma.Text = String.Format("{0:C}", 0);
      logger.Write(LogLevel.Debug, "Оплата: Оплата успешно проведена.");
      ShowMessage("Операция успешно проведена.", LogLevel.Information);
    }

    #endregion

    #region [ controls ]
    
    // Деактивировать кнопки на форме
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

    // Активировать кнопки на форме
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

    // Деактивировать кнопки недоступные при отсутствиии карты
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

    // Деактивировать кнопки недоступные при отсутствиии карты
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

    // Настройка кнопок при карте на ридере
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
    /// Отображает сообщение в поле сообщений.
    /// </summary>
    /// <param name="Message">Сообщение</param>
    /// <param name="MessageType">Тип сообщения (определяет его цвет - Ошибка - красный, Предупреждение - оранжевый
    /// Остальные - черный</param>
    /// <param name="Add">True - добавить сообщение, False - отобразить новое</param>
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
    /// Отображает сообщение в поле сообщений.
    /// </summary>
    /// <param name="Message">Сообщение</param>
    /// <param name="MessageType">Тип сообщения (определяет его цвет - Ошибка - красный, Предупреждение - оранжевый
    /// Остальные - черный</param>
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
    /// Получен пакет от сервера
    /// </summary>
    private void OnNetworkPacketReceived(Packet packet)
    {
      //System.Diagnostics.Trace.WriteLine("-> OnNetworkPacketReceived #" + packet.Address.ToString());

      if (packet.Address != device.Device.NetworkAddress)
        return;

      counterReceived++;
      if ((counterReceived % counterDelay) == 0)
        SwitchReceived();
      if (counterReceived == Int32.MaxValue)
        counterReceived = 0;

      //DateTime timeStart = DateTime.Now;

      Packet answer = device.GetResponse(packet);
      if (network.State == NetworkProtocolState.Opened)
        if (network.SendPacket(answer))
        {
          counterSent++;
          if ((counterSent % counterDelay) == 0)
            SwitchSent();
          if (counterSent == Int32.MaxValue)
            counterSent = 0;
        }

      //TimeSpan ts = DateTime.Now - timeStart;
      //if (ts > network.PacketTimeout)
      //  _logger.Write(LogLevel.Error, String.Format("Не успел ответить, таймаут = {0}", ts.ToString()), 2);

      lastPacketTime = DateTime.Now;
    }

    ///// <summary>
    ///// Создаёт кассовую транзакцию (версия 3.5)
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
      lblReaderState.Text = String.Format("Считыватель : {0}", s);
    }

    private void UpdateFiscalState(string s)
    {
      lblFiscalState.Text = String.Format("ФР : {0}", s);
    }

    private void SwitchReceived()
    {
      this.SafeInvoke(new Action(SwitchReceivedCore));
    }

    private void SwitchReceivedCore()
    {
      indicatorReseived = !indicatorReseived;
      tslblStatusReceived.Image = indicatorReseived ? imageReceived : imageIdle;
    }

    private void SwitchSent()
    {
      this.SafeInvoke(new Action(SwitchSentCore));
    }

    private void SwitchSentCore()
    {
      indicatorSent = !indicatorSent;
      tslblStatusSent.Image = indicatorReseived ? imageSent : imageIdle;
    }

    private void SwitchOff()
    {
      indicatorReseived = true;
      SwitchReceivedCore();
      indicatorSent = true;
      SwitchSentCore();
    }

    #endregion

    /// <summary>
    /// Возвращает штрафной тариф по умолчанию
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
    /// Возвращает тариф метро по умолчанию
    /// </summary>
    private Tariff GetDefaultMetroTariff()
    {
      return new TariffMetro();
    }

    /// <summary>
    /// Обновление данных расчета на форме
    /// </summary>
    private void UpdateParameters(DateTime timeExit)
    {
      //check
      if ((smartCard == null) || (currentDocument == null)) // Не все готово
      {
        lblTimeIn.Text = DateTime.Now.DateToString(); // ToString("dd MMMM yyyy  H:mm:ss");
        dtpOut.Value = DateTime.Now;
        lblTimeInParking.Text = NO_DATA;
        lblTimePaid.Text = NO_DATA;
        lblExitTimeLimit.Text = NO_DATA;

        lblPaid.Text = String.Format("{0:C}", 0);
        lblDebt.Text = String.Format("{0:C}", 0);
        lblSumma.Text = String.Format("{0:C}", 0);
        lblECash.Text = String.Format("{0:C}", 0);

        return;
      }

      //process
      lblTimeIn.Text = smartCard.TimeEntry.DateToString();
      dtpOut.Value = timeExit;
      lblTimeInParking.Text = Utils.TimeToString(DateTime.Now - currentDocument.TimeEntry);// время на стоянке;
      lblPaid.Text = currentDocument.Payment.ToString("C");
      lblDebt.Text = currentDocument.Debt.ToString("C");
      lblECash.Text = currentDocument.ECash.ToString("C");
      if (Settings.Default.DisplayEnabled) // Вывод на дисплей
      {
        display.Clear();
        display.Text(1, GetLocalizedDateTimeString(lblTimeInParking.Text)); // Время проведенное на парковке
      }

      if (currentDocument.Amount <= 0) // Оплата не требуется
      {
        lblTimePaid.Text = Utils.TimeToString(currentDocument.TimeExit - DateTime.Now); // В данном случае время оставшееся на выезд
        lblExitTimeLimit.Text = currentDocument.TimeExit.DateToString();//Выезд до
        lblSumma.Text = String.Format("{0:C}", 0);
        if (Settings.Default.DisplayEnabled)
          display.Text(2, String.Format(rm.GetString("InPay") + ": {0:C}", 0));
      }
      else // Требуется оплата
      {
        lblTimePaid.Text = NEED_PAY; // В данном случае время оставшееся на выезд
        lblExitTimeLimit.Text = NEED_PAY;  // Выезд до
        lblSumma.Text = String.Format("{0:C}", currentDocument.Amount);
        try
        {
          if (Settings.Default.DisplayEnabled)
            display.Text(2, String.Format(rm.GetString("InPay") + ": {0:C}", currentDocument.Amount));
        }
        catch (Exception ex)
        {
          MessageBox.Show(this, ex.Message, ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    /// <summary>
    /// Форматируем дату в локализованный формат
    /// </summary>
    private string GetLocalizedDateTimeString(string s)
    {
      s = s.Replace("сут", rm.GetString("Days")).Replace("час", rm.GetString("Hour")).Replace("мин", rm.GetString("Min")).Replace("сек", rm.GetString("Sec"));
      return s;
    }
  }
}