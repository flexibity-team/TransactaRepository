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

    private const String _PASS_ERROR = "Неверный пароль. Доступ запрещен!\r\n";

    #endregion

    private IFiscalDevice _fiscalDevice;
    private CashTransactions _trans;
    private ILogger _logger;

    public CashControl(IFiscalDevice fiscalDevice, CashTransactions trans, ILogger logger)
    {
      InitializeComponent();

      this.SetLogoIcon();

      _fiscalDevice = fiscalDevice;
      _trans = trans;
      _logger = logger;
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

      if (_fiscalDevice.IsSessionOpened)
        lblSessionOpen.Text = String.Format("{0:G}", Settings.Default.SessionOpenTime);
      else
        lblSessionOpen.Text = "Смена закрыта";
    }

    private void btnSessionClose_Click(object sender, EventArgs e)
    {
      if (!CheckPass())
        return;

      //EnableControls(false);

      try
      {
        _fiscalDevice.CloseSession(); // закрытие смены
        UpdateCCMSum();
        _logger.Write(LogLevel.Debug, "Смена закрыта");
      }
      catch (FiscalDeviceException err)
      {
        EnableControls(true);
        tbKKMMessage.Text = "Ошибка закрытия смены! \r\n" + err.ToString();
        _logger.Write(err, err.ToString());

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
      _trans.Write(DateTime.Now, "Смена закрыта");
      EnableControls(true);
    }

    private void btnOpenSession_Click(object sender, EventArgs e)
    {
      if (!CheckPass())
        return;

      //EnableControls(false);

      try
      {
        _fiscalDevice.OpenSession();
        _logger.Write(LogLevel.Debug, "Смена открыта");
      }
      catch (FiscalDeviceException err)
      {
        tbKKMMessage.Text = "Ошибка открытия смены! \r\n" + err.ToString();
        _logger.Write(err, err.ToString());
        EnableControls(true);
        return;
      }
      tbKKMMessage.Text = "Смена открыта.";
      _trans.Write(DateTime.Now, "Смена открыта");
      UpdateCCMSum();

      if (_fiscalDevice.IsSessionOpened)
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
        _fiscalDevice.PrintXReport();
        _logger.Write(LogLevel.Debug, "Печать Х-отчета");
      }
      catch (FiscalDeviceException err)
      {
        tbKKMMessage.Text = "Ошибка печати Х отчета! \r\n" + err.ToString();
        _logger.Write(err, err.ToString());
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
        _logger.Write(LogLevel.Warning, "Неверная сумма. Внесение не будет произведено: " + fSumm.ToString("C"));
        return;
      }

      try
      {
        _fiscalDevice.CashIn(fSumm);  // Внесение суммы
        _logger.Write(LogLevel.Debug, "Внесение денег: " + fSumm.ToString("C"));
      }
      catch (FiscalDeviceException err)
      {
        tbKKMMessage.Text = "Ошибка внесения денег в кассу! \r\n" + err.ToString();
        _logger.Write(err, err.ToString());
        EnableControls(true);
        return;
      }
      Thread.Sleep(2000); // Это ужасно!!!
      tbKKMMessage.Text = "Деньги успешно внесены в кассу";
      _trans.Write(DateTime.Now, "Внесение денег", fSumm);
      UpdateCCMSum();

      EnableControls(true);
    }

    private void btnEncashment_Click(object sender, EventArgs e)
    {
      if (!CheckPass())
        return;

      double fSumm;
      // EnableControls(false);

      fSumm = UserInputAmount();
      // проверим, есть ли что выносить?
      if (fSumm <= 0.01)
      {
        tbKKMMessage.Text = "Неверная сумма. Инкассация произвдится не будет";
        _logger.Write(LogLevel.Warning, "Неверная сумма. Инкассация произвдится не будет: " + fSumm.ToString("C"));
        return;
      }

      try
      {
        _fiscalDevice.CashOut(fSumm);  // Вынесение суммы
        _logger.Write(LogLevel.Debug, "Внесение денег: " + fSumm.ToString("C"));
      }
      catch (FiscalDeviceException err)
      {
        tbKKMMessage.Text = "Ошибка инкассации! \r\n" + err.ToString();
        _logger.Write(err, err.ToString());
        EnableControls(true);
        return;
      }

      Thread.Sleep(2000);
      tbKKMMessage.Text = "Инкассация проведена успешно";
      _trans.Write(DateTime.Now, "Инкассация", fSumm);
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
      double MoneyInCash = _fiscalDevice.GetKKMAmount(); // денег в кассе
      lblMoneyInCash.Text = MoneyInCash.ToString("C");
      _trans.Write(DateTime.Now, "Сумма в кассе", MoneyInCash);
      _logger.Write(LogLevel.Debug, "Сумма в кассе" + MoneyInCash.ToString("C"));

      double MoneyPerChange = _fiscalDevice.GetSessionAmount(); // выручка за смену
      lblWatchSum.Text = MoneyPerChange.ToString("C");
      _trans.Write(DateTime.Now, "Выручка за смену", MoneyPerChange);
      _logger.Write(LogLevel.Debug, "Выручка за смену" + MoneyPerChange.ToString("C"));
    }

    /// <summary>
    /// Проверка пароля на операции. Выдает окно ввода пароля. Отображает сообщение в случае неверного ввода пароля.
    /// </summary>
    /// <returns>True - пароль верный. False - Пароль не верный</returns>
    private bool CheckPass()
    {
      if (Settings.Default.CashPassword == "none") return true;
      PasswordForm pass = new PasswordForm("Введите пароль");
      if (pass.ShowDialog() != DialogResult.OK)
        return false;

      if (pass.edPass.Text != Crypt.Decrypt(Settings.Default.CashPassword, "A23F78C4"))
      {
          _logger.Write(LogLevel.Warning, _PASS_ERROR);
          MessageBox.Show(this, _PASS_ERROR, ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return false;
      }

      return true;
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


      if (double.TryParse(sAmount, out fAmount) == false)
      {
        _logger.Write(LogLevel.Error, "Ошибка конвертирования числа введенного пользователем!");
        EnableControls(true);
        return 0;
      }

      return fAmount;
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