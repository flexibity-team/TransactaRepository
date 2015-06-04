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
    private SmartCardManager _smartCardManager;
    private ILogger _logger;
    private TariffViewForm _tariffForm;
    private CashDeskLanguage _cashDeskLanguage;

    public SettingsForm(ILogger logger, SmartCardManager smartCardManager)
    {
      InitializeComponent();

      this.SetLogoIcon();

      imgShow.Images.Add(Images.View);
      btnTariffPenaltyShow.ImageIndex = 0;
      btnTariffMetroShow.ImageIndex = 0;

      _logger = logger;
      _smartCardManager = smartCardManager;
      _tariffForm = null;
      cboLanguage.SetEnum<CashDeskLanguage>(CashDeskHelper.GetString);
      _cashDeskLanguage = Settings.Default.CashDeskLanguage;
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

      cboLanguage.SelectedIndex = (int)_cashDeskLanguage;

      if (!Settings.Default.UseMetro)
      {
        tableMain.ShowRow(12, 0);
        tableMain.ShowRow(13, 0);
      }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      Settings.Default["FiscalDevicePort"] = Convert.ToInt32(edKKMPortNum.Text);
      Settings.Default["DisplayEnabled"] = cbDisplayState.Checked;
      Settings.Default["LogOn"] = chbLogOn.Checked;
      Settings.Default["DisplayPort"] = Convert.ToInt32(edDisplayPort.Text);
      Settings.Default["CashNumber"] = Convert.ToByte(edCashNum.Text);

      _cashDeskLanguage = (CashDeskLanguage)cboLanguage.SelectedIndex;
      Settings.Default["CashDeskLanguage"] = _cashDeskLanguage;
      Settings.Default.Save();
      ApplyLanguage();
    }

    private void chkDisplayState_CheckedChanged(object sender, EventArgs e)
    {
      edDisplayPort.Enabled = cbDisplayState.Checked;
    }

    private void btnTariffPenaltyRead_Click(object sender, EventArgs e)
    {
      // ��������� ����� � ������ ����� � ���������� �� � �����������
      ISmartCardMaster master = new SmartCardMaster();
      try
      {
        _smartCardManager.ReadMasterCard(master);
      }
      catch (Exception ex)
      {
        _logger.Write(ex, "������ ������ ������-�����");
        MessageBox.Show(this, "������ ������ ������-�����", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      
      // ������������� ������. �� ������ ����� �������� 4 ����� ��� 9 ��������.
      _smartCardManager.KeysB[1] = master.KeysB[0];
      _smartCardManager.KeysB[2] = master.KeysB[1];
      _smartCardManager.KeysB[3] = master.KeysB[2];
      _smartCardManager.KeysB[4] = master.KeysB[2];
      _smartCardManager.KeysB[5] = master.KeysB[2];
      _smartCardManager.KeysB[6] = master.KeysB[2];
      _smartCardManager.KeysB[7] = master.KeysB[3];
      _smartCardManager.KeysB[8] = master.KeysB[3];
      _smartCardManager.KeysB[9] = master.KeysB[3];
      _smartCardManager.KeysB[SmartCardLayoutHelper.MetroSectorIndex] = GetKeyForDiscountSector(master.KeysB[3]);

      // ���������� � ������ ����������� � ������-����� ������ � ������������.
      // ��������������� ������ ����������, ���� ������������ ������ ������ "��".
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
        _logger.Write(ex, "������ ����������� ���������� � ������-����� ������");
        MessageBox.Show(this, "������ ����������� ���������� � ������-����� ������", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      string tariff = String.Empty;
      try
      {
        tariff = CashDeskHelper.SerializeTariff(master.Tariff);
      }
      catch (Exception ex)
      {
        _logger.Write(ex, "������ ������ ������ � ������-�����");
        MessageBox.Show(this, "������ ������ ������ � ������-�����", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      Settings.Default["PenaltyTarif"] = tariff;
      Settings.Default.Save();

      MessageBox.Show(this, "������-����� ������� ���������", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        _logger.Write(ex, "������ ������ ������� �����");
        MessageBox.Show(this, "������ ������ ������� �����", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      Settings.Default["MetroTariffK"] = tm;
      Settings.Default["MetroTariffL"] = tmd;
      Settings.Default.Save();

      MessageBox.Show(this, "������ ����� ������� ���������", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void btnTariffShow_Click(object sender, EventArgs e)
    {
      string tariff = (sender == btnTariffPenaltyShow) ? Settings.Default.PenaltyTarif : String.Empty;

      if (String.IsNullOrEmpty(tariff))
      {
        MessageBox.Show(this, "���������� ������ ����� � ������-�����!", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (_tariffForm == null)
        _tariffForm = new TariffViewForm();

      try
      {
        _tariffForm.Setup(CashDeskHelper.DeserializeTariff(tariff));
        _tariffForm.ShowDialog();
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, "������ ������ ������ �� ����� ������������: \r\n" + ex.Message, ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnChangePassword_Click(object sender, EventArgs e)
    {
      String passFirstEnter;
      PasswordForm pass = new PasswordForm();
      pass.lblPass.Text = "������� ����� ������";
      if (pass.ShowDialog() == DialogResult.OK)   // �������� ������� ����� ������ 1 ���
      {
        passFirstEnter = pass.edPass.Text;
        pass.edPass.Text = "";
        pass.lblPass.Text = "��������� ���� ������";
        if (pass.ShowDialog() == DialogResult.OK) // �������� ������� ����� ������ 2 ��� (��������)
          if (pass.edPass.Text != passFirstEnter)
          { // ���� ������������ �� ���� 2 ���� ������ ���������� ������
            MessageBox.Show(this, "��������� ������ �� ���������! ���������� ��� ���.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
          else
            Settings.Default["Password"] = Crypt.Encrypt(passFirstEnter, "t23F78C4");
      }
    }

    private void btnChangeCashPass_Click(object sender, EventArgs e)
    {
      String passFirstEnter;
      PasswordForm pass = new PasswordForm("������� ����� ������");
      if (pass.ShowDialog() == DialogResult.OK)   // �������� ������� ����� ������ 1 ���
      {
        passFirstEnter = pass.edPass.Text;
        pass.edPass.Text = "";
        pass.lblPass.Text = "��������� ���� ������";
        if (pass.ShowDialog() == DialogResult.OK) // �������� ������� ����� ������ 2 ��� (��������)
          if (pass.edPass.Text != passFirstEnter)
          { // ���� ������������ �� ���� 2 ���� ������ ���������� ������
            MessageBox.Show(this, "��������� ������ �� ���������! ���������� ��� ���.", ApplicationServices.GetApplicationName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
          else
            Settings.Default["CashPassword"] = String.IsNullOrEmpty(passFirstEnter) ? "none" : Crypt.Encrypt(passFirstEnter, "A23F78C4");
      }
    }

    #endregion

    private void ApplyLanguage()
    {
      if (_cashDeskLanguage == CashDeskLanguage.Az)
      {
        CultureInfo _cultureInfo = new CultureInfo("az-Latn-AZ");
        Thread.CurrentThread.CurrentUICulture = _cultureInfo;
      }
      else
      {
        CultureInfo _cultureInfo = new CultureInfo("ru-RU");
        Thread.CurrentThread.CurrentUICulture = _cultureInfo;
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