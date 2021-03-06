using System;
using System.Threading;
using System.Windows.Forms;
using CryptLib;
using Parking.CashDesk.Properties;
using Parking.Data;
using Parking.FiscalDevice;
using ParkTime.StandardForm;
using RMLib;
using RMLib.Log;

namespace Parking.CashDesk
{
  public partial class CashControl : Form
  {
    #region [ const ]

    private const String PASS_ERROR = "Неверный пароль. Доступ запрещен!\r\n";

    #endregion

    private readonly IFiscalDevice fiscalDevice;
    private readonly CashTransactions trans;
    private readonly ILogger logger;

    public CashControl(IFiscalDevice fiscalDevice, CashTransactions trans, ILogger logger)
    {
      InitializeComponent();

      this.SetLogoIcon();

      this.fiscalDevice = fiscalDevice;
      this.trans = trans;
      this.logger = logger;
    }

    #region [ window events ]

    private void CashControl_Load(object sender, EventArgs e)
    {
        try
      {
        UpdateCCMSum();
      }
      catch (Exception err)
      {
        tbKKMMessage.Text = err.Message;
        return;
      }

        lblSessionOpen.Text = fiscalDevice.IsSessionOpened ? String.Format("{0:G}", Settings.Default.SessionOpenTime) : "Смена закрыта";
    }

      private void btnSessionClose_Click(object sender, EventArgs e)
    {
      if (!CheckPass())
        return;

      //EnableControls(false);

      try
      {
        fiscalDevice.CloseSession(); // закрытие смены
        UpdateCCMSum();
        logger.Write(LogLevel.Debug, "Смена закрыта");
      }
      catch (FiscalDeviceException err)
      {
        EnableControls(true);
        tbKKMMessage.Text = "Ошибка закрытия смены! \r\n" + err.ToString();
        logger.Write(err, err.ToString());

        return;
      }

      //if (pPrim.isOpened())
      //{
      //  lblSessionOpen.Text = String.Format("{0:G}", pConfig.SessionOpen);
      //  tbKKMMessage.Text = "Ошибка закрытия смены! \r\n" + pPrim.GetTextKKMError();
      //  EnableControls(true);
      //  return;
      //}

      lblSessionOpen.Text = "Смена закрыта";
      tbKKMMessage.Text = "Смена успешно закрыта";
      trans.Write(DateTime.Now, "Смена закрыта");
      EnableControls(true);
    }

    private void btnOpenSession_Click(object sender, EventArgs e)
    {
      if (!CheckPass())
        return;

      //EnableControls(false);

      try
      {
        fiscalDevice.OpenSession();
        logger.Write(LogLevel.Debug, "Смена открыта");
      }
      catch (FiscalDeviceException err)
      {
        tbKKMMessage.Text = "Ошибка открытия смены! \r\n" + err.ToString();
        logger.Write(err, err.ToString());
        EnableControls(true);
        return;
      }
      tbKKMMessage.Text = "Смена открыта.";
      trans.Write(DateTime.Now, "Смена открыта");
      UpdateCCMSum();

      if (fiscalDevice.IsSessionOpened)
      {
        // Необходимо сохранить время открытия смены в файле конфигурации
        Settings.Default["SessionOpenTime"] = DateTime.Now;
        Settings.Default.Save();
        // Время открытие смены (текущее)
        lblSessionOpen.Text = String.Format("{0:G}", Settings.Default.SessionOpenTime);
      }
      else
      {
        lblSessionOpen.Text = "Смена закрыта";
        tbKKMMessage.Text = "Ошибка открытия смены! Перезагрузите программу и устройство, и попробуйте еще раз\r\n";
        EnableControls(true);
        return;
      }

      EnableControls(true);
    }

    private void btnXReport_Click(object sender, EventArgs e)
    {
      if (!CheckPass())
        return;

      try
      {
        fiscalDevice.PrintXReport();
        logger.Write(LogLevel.Debug, "Печать Х-отчета");
      }
      catch (FiscalDeviceException err)
      {
        tbKKMMessage.Text = "Ошибка печати Х отчета! \r\n" + err.ToString();
        logger.Write(err, err.ToString());
        EnableControls(true);
        return;
      }
      tbKKMMessage.Text = "Х-Отчет отправлен на печать.";

      UpdateCCMSum();
      EnableControls(true);
    }

    private void btnMoneyToCash_Click(object sender, EventArgs e)
    {
      if (!CheckPass())
        return;

      double fSumm;
      //EnableControls(false);

      fSumm = UserInputAmount();
      // проверим, есть ли что вносить?
      if (fSumm <= 0.01)
      {
        tbKKMMessage.Text = "Неверная сумма. Внесение не будет произведено";
        logger.Write(LogLevel.Warning, "Неверная сумма. Внесение не будет произведено: " + fSumm.ToString("C"));
        return;
      }

      try
      {
        fiscalDevice.CashIn(fSumm);  // Внесение суммы
        logger.Write(LogLevel.Debug, "Внесение денег: " + fSumm.ToString("C"));
      }
      catch (FiscalDeviceException err)
      {
        tbKKMMessage.Text = "Ошибка внесения денег в кассу! \r\n" + err.ToString();
        logger.Write(err, err.ToString());
        EnableControls(true);
        return;
      }
      Thread.Sleep(2000); // Это ужасно!!!
      tbKKMMessage.Text = "Деньги успешно внесены в кассу";
      trans.Write(DateTime.Now, "Внесение денег", fSumm);
      UpdateCCMSum();

      EnableControls(true);
    }

    private void btnEncashment_Click(object sender, EventArgs e)
    {
      if (!CheckPass())
        return;

        // EnableControls(false);

      var fSumm = UserInputAmount();
      // проверим, есть ли что выносить?
      if (fSumm <= 0.01)
      {
        tbKKMMessage.Text = "Неверная сумма. Инкассация произвдится не будет";
        logger.Write(LogLevel.Warning, "Неверная сумма. Инкассация произвдится не будет: " + fSumm.ToString("C"));
        return;
      }

      try
      {
        fiscalDevice.CashOut(fSumm);  // Вынесение суммы
        logger.Write(LogLevel.Debug, "Внесение денег: " + fSumm.ToString("C"));
      }
      catch (FiscalDeviceException err)
      {
        tbKKMMessage.Text = "Ошибка инкассации! \r\n" + err.ToString();
        logger.Write(err, err.ToString());
        EnableControls(true);
        return;
      }

      Thread.Sleep(2000);
      tbKKMMessage.Text = "Инкассация проведена успешно";
      trans.Write(DateTime.Now, "Инкассация", fSumm);
      UpdateCCMSum();

      EnableControls(true);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      Close();
    }

    #endregion

    /// <summary>
    /// Проверка денег в ККМ
    /// </summary>
    /// <returns>0 - ОК, -1 - ошибка связи с ККМ, -2 - ошибка получения электронного отчета</returns>
    private void UpdateCCMSum()
    {
      double MoneyInCash = fiscalDevice.GetKKMAmount(); // денег в кассе
      lblMoneyInCash.Text = MoneyInCash.ToString("C");
      trans.Write(DateTime.Now, "Сумма в кассе", MoneyInCash);
      logger.Write(LogLevel.Debug, "Сумма в кассе" + MoneyInCash.ToString("C"));

      double MoneyPerChange = fiscalDevice.GetSessionAmount(); // выручка за смену
      lblWatchSum.Text = MoneyPerChange.ToString("C");
      trans.Write(DateTime.Now, "Выручка за смену", MoneyPerChange);
      logger.Write(LogLevel.Debug, "Выручка за смену" + MoneyPerChange.ToString("C"));
    }

    /// <summary>
    /// Проверка пароля на операции. Выдает окно ввода пароля. Отображает сообщение в случае неверного ввода пароля.
    /// </summary>
    /// <returns>True - пароль верный. False - Пароль не верный</returns>
    private bool CheckPass()
    {
//      if (Settings.Default.CashPassword == "none")
          return true;
      var pass = new PasswordForm("Введите пароль");
      if (pass.ShowDialog() != DialogResult.OK)
        return false;

        if (pass.edPass.Text == Crypt.Decrypt(Settings.Default.CashPassword, "A23F78C4"))
            return true;

        logger.Write(LogLevel.Warning, PASS_ERROR);
        MessageBox.Show(this, PASS_ERROR, ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
    }

    /// <summary>
    /// Возвращает величину введенную пользователем
    /// </summary>
    private double UserInputAmount()
    {
      string sAmount = String.Empty;
      double fAmount = 0;

      // Окно ввода вносимой суммы
      if (InputBox.Show("Введите значение", "Введите сумму", true, ref sAmount) != DialogResult.OK)
        return 0;


        if (double.TryParse(sAmount, out fAmount))
            return fAmount;
        logger.Write(LogLevel.Error, "Ошибка конвертирования числа введенного пользователем!");
        EnableControls(true);
        return 0;
    }

    /// <summary>
    /// Разрешает/запрещает элементы управления
    /// </summary>
    private void EnableControls(bool state)
    {
      btnOpenSession.Enabled = true;
      btnSessionClose.Enabled = true;
      btnXReport.Enabled = true;
      btnMoneyToCash.Enabled = true;
      btnEncashment.Enabled = true;
    }
  }
}