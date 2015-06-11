using System;
using System.Windows.Forms;
using RMLib;

namespace Parking.SberBank
{
  internal class SberBankTray : GenericTray
  {
    #region [ TrayService ]

    protected override void OnTrayBalloonClick(object sender, EventArgs e)
    {
      MainForm mainForm = this.MainForm as MainForm;
      if (mainForm != null)
      {
        mainForm.ShowFromTray();
        return;
      }

      base.OnTrayBalloonClick(sender, e);
    }

    protected override bool GetApplicationState(object state)
    {
      return ((SberBankApplicationState)state).WillRun;
    }

    protected override void OnApplicationReady(object state)
    {
      if (((SberBankApplicationState)state).RunErrors)
      {
        Tray.BalloonTipIcon = ToolTipIcon.Warning;
        SetBalloonTipText("При запуске возникли ошибки");
      }

      base.OnApplicationReady(state);
    }

    #endregion

    #region [ MenuTrayService ]

    protected override void OnMenuTrayExitClick(object sender, EventArgs e)
    {
#if !DEBUG
      if (MessageBox.Show(MainForm, "Вы уверены, что хотите завершить работу транслятора?", ApplicationServices.GetApplicationName(),
        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        return;
#endif

      base.OnMenuTrayExitClick(sender, e);
    }

    #endregion
  }
}