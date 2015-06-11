using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Parking.Data;
using Parking.Gazprom.Properties;
using Parking.Network;
using Parking.Network.Serial;
using RMLib;
using RMLib.Forms;
using RMLib.Log;
using RMLib.Services;
using System.Linq;
using System.Text;

namespace Parking.Gazprom
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

      //uint v = EgateAPI.egGetVersion();
      //string d = EgateAPI.egGetVersionDescription().GetString();
      //MessageBox.Show(String.Format("v{0} {1}", v, d));
      //return;

      //string s1 = "00600215,600008,032,0001,548999******5026,1512,1000,0,1000,2706,1645,400,MC-сети-ГПБ,643,0,0,314804,002504314804,,140150148,,270614,164523,000,OK,,,,\x80\x80\x80,,,1,,";
      //string s2 = ",,032,0001,,0000,,0,1000,0000,0000,399,Mastercard,643,0,0,,,,000000,,,,000,OK,,,,\x80\x80\x80,,,5,,";
      //AuthResponse a1 = AuthResponse.Parse(s1);
      //AuthResponse a2 = AuthResponse.Parse(s2);
      //return;

      //string s = @"30 30 36 30 30 32 31 35 2C 36 30 30 30 30 38 2C 30 33 32 2C 30 30 30 30 2C 35 34 38 39 39 39 2A 2A 2A 2A 2A 2A 35 30 32 36 2C 31 35 31 32 2C 31 30 30 30 30 2C 30 2C 31 30 30 30 30 2C 31 34 30 37 2C 31 32 33 33 2C 34 33 32 2C 4D 43 2D 3F 3F 3F 3F 2D 3F 3F 3F 2C 36 34 33 2C 30 2C 30 2C 33 37 36 30 38 32 2C 30 30 32 35 30 34 33 37 36 30 38 32 2C 2C 31 34 30 31 35 30 31 34 38 2C 2C 31 34 30 37 31 34 2C 31 32 33 33 32 35 2C 30 30 30 2C 4F 4B 2C 2C 2C 2C 5C 78 38 30 5C 78 38 30 5C 78 38 30 2C 2C 2C 31 2C 2C";
      //string s = "1F,26,0B,4D,43,20,F1,E5,F2,E8,20,C3,CF,C1";
      //string s = "30 30 36 30 30 32 31 35 2C 36 30 30 30 30 38 2C 30 33 32 2C 30 30 30 30 2C 35 34 38 39 39 39 2A 2A 2A 2A 2A 2A 30 30 31 33 2C 31 35 31 32 2C 31 30 30 30 30 2C 30 2C 31 30 30 30 30 2C 31 37 30 37 2C 31 33 30 35 2C 34 37 32 2C 4D 61 73 74 65 72 63 61 72 64 2C 36 34 33 2C 30 2C 30 2C 33 38 35 32 39 31 2C 30 30 32 35 30 34 33 38 35 32 39 31 2C 61 30 30 30 30 30 30 30 30 34 31 30 31 30 2C 31 34 30 31 35 38 31 32 38 2C 4D 61 73 74 65 72 43 61 72 64 2C 31 37 30 37 31 34 2C 31 33 30 36 34 30 2C 30 30 30 2C 4F 4B 2C 2C 2C 2C 5C 78 38 30 5C 78 38 30 5C 78 38 31 2C 2C 2C 31 2C 2C";
      //string[] sa = s.Split(' ');
      //byte[] ba = sa.Select(x => { long l; x.FromHex(out l); return (byte)(l & 0xFF); }).ToArray();
      //s = Encoding.GetEncoding(1251).GetString(ba);
      //return;

      //проверка повторного запуска
      if (!ApplicationFormServices.EnsureNotRunning())
        return;

      //обработчик исключений
      LoggedExceptionHandler xh = new LoggedExceptionHandler();

      //статическая инициализация
      AssemblyInitializer.UseCurrentDomain();

      //создаём контейнер и регистрируем классы
      using (IServiceContainer container = new ServiceContainer())
      {
        //получаем агрументы
        GenericArguments args = new GenericArguments();

        //получаем настройки
        Settings settings = Settings.Default;
        settings.SettingsSaving += AssemblyConfigurationHelper.SettingsSaveHandler;

        //менеджер путей
        GazpromPathManager pathManager = new GazpromPathManager();
        container.RegisterInstance<PathManager>(pathManager);

        //логгер
        ILogger logger = container.RegisterLogger(settings.LogLevel, pathManager.LogPath);
        //if (!args.IsNoTrace && ((settings.Mode & GazpromMode.NoTrace) == 0))
        //{
        //  container.RegisterTraceLogWriter(TraceHelper.GazpromTraceName, LogCategoryMap.Load(CommonHelper.GetCategoryMapFileName(pathManager, GazpromHelper.LogFileName)), GazpromHelper.GazpromTraceMessageFilter);
        //  container.RegisterTraceLogWriter(TraceHelper.GazpromPollingTraceName, LogCategoryMap.Load(CommonHelper.GetCategoryMapFileName(pathManager, GazpromHelper.LogFileNamePolling)), GazpromHelper.PollingTraceMessageFilter);
        //}
        container.Rebuild<ILogger>();

        //BankingDevice d = new BankingDevice(32);
        //d.Message += (s, e) => { logger.Write(e ? LogLevel.Error : LogLevel.Verbose, s); };
        //d.StateChanged += (s, i) => { logger.Write(LogLevel.Debug, String.Format("{0}\r\n{1}", s, i)); };
        //d.TransactionCompleted += (r, z) => { logger.Write(LogLevel.Information, String.Format("{0}\r\n{1}", r, z)); };
        //try
        //{
        //  if (d.Start())
        //  {
        //    int r = 1;
        //    int a = 1000;
        //    while (true)
        //    {
        //      if (d.BeginTransaction(r++, a))
        //      {
        //        while (d.State != EquipmentState.CardTaken)
        //          System.Threading.Thread.Sleep(500);

        //        d.EndTransaction();
        //        a += 100;
        //      }

        //      if (MessageBox.Show("Следующий платеж?", "Debug", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        //        break;
        //    }
        //  }
        //}
        //catch (Exception e)
        //{
        //  logger.Write(e);
        //}
        //finally
        //{
        //  d.Stop();
        //}

        //return;

        //создаём список предупреждений для записи в лог после запуска
        List<ILogMessage> warnings = new List<ILogMessage>();

        //обработчик исключений и окно отображения ошибок (до создания любого окна)
        container.RegisterInstance<IApplicationReportProvider>(new ApplicationReportProvider(true));
        container.RegisterErrorServices(xh);
        container.RegisterErrorWindow();

        //значок панели задач
        container.RegisterInstance<TrayService>(new GazpromTray());

        //заставка (она же окно "О программе")
        container.RegisterAboutWindow(!args.IsNoSplash, new GazpromAboutForm());

        //сеть
        container.RegisterInstance<INetworkProtocol>(new SerialPortProtocol());

        //сервер
        container.RegisterInstance<IApplication>(new Gazprom(settings.Mode));
        container.RegisterInstance<Form>(new MainForm());

        //инъектируем зависимости
        container.Rebuild();

        //write warnings
        foreach (LogMessage m in warnings)
          logger.Write(m);

        //запускаем приложение с помощью зарегистрированных в контейнере классов
        container.Run(false);
      }
    }
  }
}