namespace Parking.SberBank
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Data;
    using Parking.Network;
    using Parking.Network.Serial;
    using Properties;
    using RMLib;
    using RMLib.Forms;
    using RMLib.Log;
    using RMLib.Services;

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
        SberBankPathManager pathManager = new SberBankPathManager();
        container.RegisterInstance<PathManager>(pathManager);

        //логгер
        ILogger logger = container.RegisterLogger(settings.LogLevel, pathManager.LogPath);

        container.Rebuild<ILogger>();

 

        //создаём список предупреждений для записи в лог после запуска
        var warnings = new List<ILogMessage>();

        //обработчик исключений и окно отображения ошибок (до создания любого окна)
        container.RegisterInstance<IApplicationReportProvider>(new ApplicationReportProvider(true));
        container.RegisterErrorServices(xh);
        container.RegisterErrorWindow();

        //значок панели задач
        container.RegisterInstance<TrayService>(new SberBankTray());

        //заставка (она же окно "О программе")
        container.RegisterAboutWindow(!args.IsNoSplash, new SberBankAboutForm());

        //сеть
        container.RegisterInstance<INetworkProtocol>(new SerialPortProtocol());

        //сервер
        container.RegisterInstance<IApplication>(new SberBank(settings.Mode));
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