using System;
using Parking.SberBank.Properties;
using RMLib;
using RMLib.Forms;
using RMLib.Log;

namespace Parking.SberBank
{
  /// <summary>
  /// Окно настройки сервера
  /// </summary>
  internal partial class SettingsForm : EditForm
  {
    public SettingsForm()
    {
      InitializeComponent();

      cboLogLevel.SetEnum<LogLevel>(LogContract.GetString);

      InputFilter.AddControl(cboLogLevel, InputFilterOptions.Any);
      InputFilter.AddControl(updMonitorInterval, InputFilterOptions.UnsignedInteger | InputFilterOptions.AllowZero, new DoubleRange(1, 60));
    }

    #region [ EditForm ]

    protected override void OnSetup(object item)
    {
      var settings = Settings.Default;
      cboLogLevel.SelectValue(settings.LogLevel);
      updMonitorInterval.Value = new IntRange(1, 60).Coerce((int)settings.MonitorRefreshTime.TotalSeconds);
    }

    protected override object OnGetResult(object item)
    {
      Settings settings = Settings.Default;
      settings["LogLevel"] = cboLogLevel.GetSelectedValue<LogLevel>();
      settings["MonitorRefreshTime"] = TimeSpan.FromSeconds((int)updMonitorInterval.Value);

      return settings;
    }

    #endregion
  }
}