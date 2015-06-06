using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using CryptLib;
using Parking.CashDesk.Properties;
using Parking.Data;
using Parking.SmartCards;
using Parking.SmartCards.Keys;
using Parking.Workstation.Extensions;
using RMLib;
using RMLib.Forms;
using RMLib.Log;

namespace Parking.CashDesk
{
  public partial class SettingsForm : Form
  {
    private readonly SmartCardManager smartCardManager;
    private readonly ILogger logger;
    private TariffViewForm tariffForm;
    private CashDeskLanguage cashDeskLanguage;

    public SettingsForm(ILogger logger, SmartCardManager smartCardManager)
    {
      InitializeComponent();

      this.SetLogoIcon();

      imgShow.Images.Add(Images.View);
      btnTariffPenaltyShow.ImageIndex = 0;
      btnTariffMetroShow.ImageIndex = 0;

      this.logger = logger;
      this.smartCardManager = smartCardManager;
      tariffForm = null;
      cboLanguage.SetEnum<CashDeskLanguage>(CashDeskHelper.GetString);
      cashDeskLanguage = Settings.Default.CashDeskLanguage;
      ApplyLanguage();
    }

    #region [ window events ]

    private void SettingsForm_Load(object sender, EventArgs e)
    {
      edKKMPortNum.Text = Convert.ToString(Settings.Default.FiscalDevicePort);
      cbDisplayState.Checked = Settings.Default.DisplayEnabled;
        chbLogOn.Checked = Settings.Default.LogOn;
      edDisplayPort.Text = Convert.ToString(Settings.Default.DisplayPort);
      edCashNum.Text = Convert.ToString(Settings.Default.CashNumber);

      cboLanguage.SelectedIndex = (int)cashDeskLanguage;

        if (Settings.Default.UseMetro)
            return;

        tableMain.ShowRow(12, 0);
        tableMain.ShowRow(13, 0);
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      Settings.Default["FiscalDevicePort"] = Convert.ToInt32(edKKMPortNum.Text);
      Settings.Default["DisplayEnabled"] = cbDisplayState.Checked;
      Settings.Default["LogOn"] = chbLogOn.Checked;
      Settings.Default["DisplayPort"] = Convert.ToInt32(edDisplayPort.Text);
      Settings.Default["CashNumber"] = Convert.ToByte(edCashNum.Text);

      cashDeskLanguage = (CashDeskLanguage)cboLanguage.SelectedIndex;
      Settings.Default["CashDeskLanguage"] = cashDeskLanguage;
      Settings.Default.Save();
      ApplyLanguage();
    }

    private void chkDisplayState_CheckedChanged(object sender, EventArgs e)
    {
      edDisplayPort.Enabled = cbDisplayState.Checked;
    }

    private void btnTariffPenaltyRead_Click(object sender, EventArgs e)
    {
      // Загружает ключи с мастер карты и записывает их в считыватель
      ISmartCardMaster master = new SmartCardMaster();
      try
      {
        smartCardManager.ReadMasterCard(master);
      }
      catch (Exception ex)
      {
        logger.Write(ex, "Ошибка чтения мастер-карты");
        MessageBox.Show(this, "Ошибка чтения мастер-карты", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      
      // Распределение ключей. На мастер карте хранится 4 ключа для 9 секторов.
      smartCardManager.KeysB[1] = master.KeysB[0];
      smartCardManager.KeysB[2] = master.KeysB[1];
      smartCardManager.KeysB[3] = master.KeysB[2];
      smartCardManager.KeysB[4] = master.KeysB[2];
      smartCardManager.KeysB[5] = master.KeysB[2];
      smartCardManager.KeysB[6] = master.KeysB[2];
      smartCardManager.KeysB[7] = master.KeysB[3];
      smartCardManager.KeysB[8] = master.KeysB[3];
      smartCardManager.KeysB[9] = master.KeysB[3];
      smartCardManager.KeysB[SmartCardLayoutHelper.MetroSectorIndex] = GetKeyForDiscountSector(master.KeysB[3]);

      // Шифрование и Запись прочитанных с мастер-карты ключей в конфигурацию.
      // Непосредственно запись произойдет, если пользователь нажмет кнопку "ОК".
      try
      {
        Settings.Default["KeyB1"] = Convert.ToBase64String(Crypt.Encrypt(master.KeysB[0].Value, "A23F78C4"));
        Settings.Default["KeyB2"] = Convert.ToBase64String(Crypt.Encrypt(master.KeysB[1].Value, "A23F78C4"));
        Settings.Default["KeyB3"] = Convert.ToBase64String(Crypt.Encrypt(master.KeysB[2].Value, "A23F78C4"));
        Settings.Default["KeyB4"] = Convert.ToBase64String(Crypt.Encrypt(master.KeysB[3].Value, "A23F78C4"));
        Settings.Default["KeyBM"] = Convert.ToBase64String(Crypt.Encrypt(GetKeyForDiscountSector(master.KeysB[3]).Value, "A23F78C4"));
      }
      catch (Exception ex)
      {
        logger.Write(ex, "Ошибка кодирования полученных с мастер-карты ключей");
        MessageBox.Show(this, "Ошибка кодирования полученных с мастер-карты ключей", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      string tariff = String.Empty;
      try
      {
        tariff = CashDeskHelper.SerializeTariff(master.Tariff);
      }
      catch (Exception ex)
      {
        logger.Write(ex, "Ошибка чтения тарифа с мастер-карты");
        MessageBox.Show(this, "Ошибка чтения тарифа с мастер-карты", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      Settings.Default["PenaltyTarif"] = tariff;
      Settings.Default.Save();

      MessageBox.Show(this, "Мастер-карта успешно прочитана", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void btnTariffMetroRead_Click(object sender, EventArgs e)
    {
      string tm = String.Empty;
      string tmd = String.Empty;
      try
      {
        tm = CashDeskHelper.SerializeTariff(CashDeskHelper.LoadTariff(CashDeskHelper.TariffMetroFileName));
        tmd = CashDeskHelper.SerializeTariff(CashDeskHelper.LoadTariff(CashDeskHelper.TariffMetroDiscountFileName));
      }
      catch (Exception ex)
      {
        logger.Write(ex, "Ошибка чтения тарифов метро");
        MessageBox.Show(this, "Ошибка чтения тарифов метро", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      Settings.Default["MetroTariffK"] = tm;
      Settings.Default["MetroTariffL"] = tmd;
      Settings.Default.Save();

      MessageBox.Show(this, "Тарифы метро успешно прочитаны", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void btnTariffShow_Click(object sender, EventArgs e)
    {
      var tariff = (sender == btnTariffPenaltyShow) ? Settings.Default.PenaltyTarif : String.Empty;

      if (String.IsNullOrEmpty(tariff))
      {
        MessageBox.Show(this, "Необходимо задать тариф с мастер-карты!", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (tariffForm == null)
        tariffForm = new TariffViewForm();

      try
      {
        tariffForm.Setup(CashDeskHelper.DeserializeTariff(tariff));
        tariffForm.ShowDialog();
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, "Ошибка чтения тарифа из файла конфигурации: \r\n" + ex.Message, ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnChangePassword_Click(object sender, EventArgs e)
    {
        var pass = new PasswordForm {lblPass = {Text = "Введите новый пароль"}};
        if (pass.ShowDialog() != DialogResult.OK)
            return;
        var passFirstEnter = pass.edPass.Text;
        pass.edPass.Text = "";
        pass.lblPass.Text = "Повторите ввод пароля";
        if (pass.ShowDialog() != DialogResult.OK) 
            return;
        if (pass.edPass.Text != passFirstEnter)
        { // Если пользователь не смог 2 раза ввести одинаковые пароли
            MessageBox.Show(this, "Введенные пароли не совпадают! Попробуйте еще раз.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
            Settings.Default["Password"] = Crypt.Encrypt(passFirstEnter, "t23F78C4");
    }

    private void btnChangeCashPass_Click(object sender, EventArgs e)
    {
      String passFirstEnter;
      PasswordForm pass = new PasswordForm("Введите новый пароль");
        if (pass.ShowDialog() != DialogResult.OK)
            return;
        passFirstEnter = pass.edPass.Text;
        pass.edPass.Text = "";
        pass.lblPass.Text = "Повторите ввод пароля";
        if (pass.ShowDialog() != DialogResult.OK)
            return;
        if (pass.edPass.Text != passFirstEnter)
        { // Если пользователь не смог 2 раза ввести одинаковые пароли
            MessageBox.Show(this, "Введенные пароли не совпадают! Попробуйте еще раз.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
            Settings.Default["CashPassword"] = String.IsNullOrEmpty(passFirstEnter) ? "none" : Crypt.Encrypt(passFirstEnter, "A23F78C4");
    }

    #endregion

    private void ApplyLanguage()
    {
      if (cashDeskLanguage == CashDeskLanguage.Az)
      {
        var cultureInfo = new CultureInfo("az-Latn-AZ");
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
      }
      else
      {
        var cultureInfo = new CultureInfo("ru-RU");
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
      }

    }

    private IAccessKey GetKeyForDiscountSector(IAccessKey key)
    {
      return key;

      //AccessKey k = new AccessKey();
      //k.Type = key.Type;

      //short parkingID = Settings.Default.ParkingID;
      //byte[] ba = key.Value;
      //for (int i = 0; i < AccessKey.KeySize; i += 2)
      //{
      //  short z = ba[i];
      //  z += (short)(ba[i + 1] << 8);
      //  z ^= parkingID;
      //  ba[i] = (byte)(z & 0xFF);
      //  ba[i + 1] = (byte)((z >> 8) & 0xFF);
      //}

      //return k;
    }
  }
}