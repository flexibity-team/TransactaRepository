using System;
using System.Windows.Forms;
using Parking.CashDesk.Properties;
using RMLib;

namespace Parking.CashDesk
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      //статическая инициализация
      AssemblyInitializer.UseCurrentDomain();

      Settings.Default.SettingsSaving += AssemblyConfigurationHelper.SettingsSaveHandler;

      Application.Run(new MainForm());
    }
  }
}