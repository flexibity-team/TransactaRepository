using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Parking.Data;
using Parking.SberBank.Extensions;
using Parking.SberBank.Properties;
using Parking.Monitoring;
using Parking.Network;
using RMLib;
using RMLib.Collections;
using RMLib.Forms;
using RMLib.Log;
using RMLib.Services;
using RMLib.Threading;

namespace Parking.SberBank
{
    using Network;

    /// <summary>
  /// Интерфейс приложения сервера
  /// </summary>
  internal interface ISberBankApplication : ISberBank, IApplication, ISupportMonitoring, ISupportLogCategory
  {
    /// <summary>
    /// Расширение загружено
    /// </summary>
    event Action<IEnumerable<ISberBankExtension>, int> RuntimeExtensionsLoaded;

    /// <summary>
    /// Расширение подключено/отключено
    /// </summary>
    event Action<ISberBankExtension, bool, bool> ExtensionControlCompleted;

    /// <summary>
    /// Получить состояние оконного потока
    /// </summary>
    event Func<System.Threading.ThreadState> GetThreadState;

    /// <summary>
    /// Возвращает режим работы сервера
    /// </summary>
    SberBankMode Mode { get; }

    /// <summary>
    /// Возвращает менеждер путей
    /// </summary>
    PathManager PathManager { get; }

    /// <summary>
    /// Возвращает настройки фильтра расширений
    /// </summary>
    TypeFilterSettings ExtensionFilterSettings { get; }

    /// <summary>
    /// Возвращает сетевой протокол
    /// </summary>
    INetworkProtocol Network { get; }

    /// <summary>
    /// Загружает расширения во время выполнения
    /// </summary>
    void LoadExtensionsRuntime(string fileName);

    /// <summary>
    /// Подключает/отключает расширение
    /// </summary>
    void ExtensionControl(ISberBankExtension extension, bool attach);

    /// <summary>
    /// Возвращает техническую информацию
    /// </summary>
    IEnumerable<string> GetRuntimeInfo();

    /// <summary>
    /// Выполнение удаленной команды
    /// </summary>
    object ExecuteRemoteCommand(string command, object parameter);
  }

  /// <summary>
  /// Состояние приложения сервера
  /// </summary>
  internal class SberBankApplicationState
  {
    public bool WillRun;
    public readonly bool RunErrors;
    public readonly IEnumerable<string> FailedExtensions;

    public SberBankApplicationState(bool willRun, bool runErrors, IEnumerable<string> failedExtensions)
    {
      WillRun = willRun;
      RunErrors = runErrors;
      FailedExtensions = failedExtensions;
    }
  }

  /// <summary>
  /// Сервер
  /// </summary>
  public partial class SberBank : ApplicationObject<ISberBank, ISberBankExtension>, ISberBank, ISberBankApplication, ISupportLogCategory, ISupportMonitoring
  {
    #region [ const ]

    private const string LogApplicationStarting = "Запуск сервера (версия {0}) ...";
    private const string LogApplicationStarted = "Сервер запущен в режиме {0}";
    private const string LogApplicationStartupTime = "Время запуска {0}";
    private const string LogApplicationStopping = "Останов сервера ...";
    private const string LogApplicationSessionTime = "Время сессии {0}";
    private const string LogApplicationStopped = "Сервер остановлен";

    private const string LogNetworkOpening = "Открытие сети ...";
    private const string LogNetworkOpened = "Сеть {0} открыта";
    private const string LogNetworkClosing = "Закрытие сети {0} ...";
    private const string LogNetworkClosed = "Сеть закрыта";
    private const string LogNetworkOpenError = "Ошибка при открытии сети {0}";
    private const string LogNetworkCloseError = "Ошибка при закрытии сети {0}";

    private const string LogExtensionsAttaching = "Подключение расширений ...";
    private const string LogExtensionsAttached = "{0} расширений подключено, {1} отфильтровано";
    private const string LogExtensionsDetaching = "Отключение расширений ...";
    private const string LogExtensionsDetached = "Расширения отключены";

    private const string LogExtensionLoading = "Подключение расширения {0}";
    private const string LogExtensionLoaded = "Расширение {0} подключено";
    private const string LogExtensionRemoving = "Отключение расширения {0}";
    private const string LogExtensionRemoved = "Расширение {0} отключено";
    private const string LogExtensionLoadError = "Ошибка при подключении расширения {0}";
    private const string LogExtensionRemoveError = "Ошибка при отключении расширения {0}";
    private const string LogExtensionError = "Ошибка в расширении {0}";
    private const string LogExtensionErrorAlreadyLoaded = "Расширение уже подключено";
    private const string LogExtensionErrorLoadingFilter = "Не удалось загрузить фильтр расширений";

    private const string LogThreadsStarting = "Запуск потоков ...";
    private const string LogThreadsStarted = "Потоки запущены";
    private const string LogThreadsStopping = "Останов потоков ...";
    private const string LogThreadsStopped = "Потоки остановлены";
    private const string LogThreadsStartError = "Ошибка при запуске потоков";

    #endregion

    private readonly SberBankMode mode;
    private bool runErrors;
    private List<string> failedExtensions;
    private Dictionary<Type, Delegate> events;

    private BankingDevice device;
    private Packet receivedPacket;
    private ManualResetEvent networkEvent;
    private INetworkProtocol networkProtocol;
    private ThreadHost networkThread;
    private SynchronizedQueue<Packet> queue;

    private ILogger logger;
    private PathManager pathManager;

    private List<IMonitoringObject> monitorObjects;
    private IProgressState startupProgress;
    private int extensionsFiltered;

    #region [ injection ]

    [InjectionProperty(Required = false, ExactBinding = false, Name = ApplicationFormServices.AboutWindowName)]
    private IProgressState AboutFormProgress
    {
      set { startupProgress = value; }
    }

    #endregion

    #region [ server events ]

 
    #endregion

    public SberBank(SberBankMode mode)
    {
      this.mode = mode;
      runErrors = false;
      failedExtensions = new List<string>();
      events = new Dictionary<Type, Delegate>();

      device = new BankingDevice((byte)(Settings.Default.CashBoxID & 0xFF));
      device.Message += (s, e) => { logger.Write(e ? LogLevel.Error : LogLevel.Verbose, s); };
      device.StateChanged += (s, i) => { logger.Write(LogLevel.Debug, String.Format("{0}\r\n{1}", s, i)); };
      device.TransactionCompleted += OnTransactionCompleted;

      receivedPacket = null;
      networkEvent = new ManualResetEvent(false);
      networkProtocol = null;
      networkThread = new ThreadHost(NetworkThread, false, true, true);
      networkThread.Name = "Network Dispatcher";

      queue = new SynchronizedQueue<Packet>();

      logger = null;
      pathManager = null;

      monitorObjects = new List<IMonitoringObject>();
      startupProgress = null;
      extensionsFiltered = 0;
    }

    #region [ ApplicationObject ]

    protected override bool RunCore()
    {
      Stopwatch w = Stopwatch.StartNew();

      string s = String.Format(LogApplicationStarting, new AssemblyInfo().Version.ToString());
      logger.Write(LogLevel.Information, s);
      UpdateStartupProgress(s);
      UpdateStartupProgress(null);

      runErrors = false;
      failedExtensions.Clear();

      //load extensions
      logger.Write(LogLevel.Information, LogExtensionsAttaching);
      UpdateStartupProgress(LogExtensionsAttaching);
      LoadExtensions(pathManager.ExtensionsPath, CreateExtensionFilter());
      s = String.Format(LogExtensionsAttached, Extensions.Count, extensionsFiltered);
      logger.Write(LogLevel.Information, s);
      UpdateStartupProgress(s);

      //log categories description
      CreateLogCategoriesMap();

      //startup network
      try
      {
        logger.Write(LogLevel.Information, LogNetworkOpening);
        UpdateStartupProgress(LogNetworkOpening);
        networkProtocol.Open();
        logger.Write(LogLevel.Debug, BankingDevice.Version);
        device.Start();
        s = String.Format(LogNetworkOpened, networkProtocol);
        logger.Write(LogLevel.Information, s);
        UpdateStartupProgress(s);
      }
      catch (Exception e)
      {
        logger.Write(e, String.Format(LogNetworkOpenError, networkProtocol));
        runErrors = true;
      }

      //start thread
      try
      {
        logger.Write(LogLevel.Information, LogThreadsStarting);
        UpdateStartupProgress(LogThreadsStarting);
        networkThread.StartThread();
        logger.Write(LogLevel.Information, LogThreadsStarted);
        UpdateStartupProgress(LogThreadsStarted);
      }
      catch (Exception e)
      {
        throw new ApplicationException(LogThreadsStartError, e);
      }

      //add monitor objects
      monitorObjects.AddRange(new IMonitoringObject[]
      {
        new NetworkStateMonitoringObject(),
        new NetworkErrorsMonitoringObject(),
        new TimedOutPacketsMonitoringObject(),
        new CommandQueueMonitoringObject(queue),
        new TransactionsMonitoringObject(),
        new TransactionCancelMonitoringObject(),
        new AuthResponseMonitoringObject(),
        new ReconcilationMonitoringObject(),
        new LogQueueMonitoringObject(logger),
        new SessionTimeMonitoringObject(this)
      });

      //ok
      s = String.Format(LogApplicationStarted, mode);
      logger.Write(LogLevel.Information, s);
      UpdateStartupProgress(s);

      //time
      w.Stop();
      logger.Write(w.Elapsed < TimeSpan.FromSeconds(20) ? LogLevel.Debug : LogLevel.Warning,
        String.Format(LogApplicationStartupTime, Common.GetTimerString(w.Elapsed)));

      return true;
    }

    protected override void ShutdownCore()
    {
      logger.Write(LogLevel.Information, LogApplicationStopping);
      monitorObjects.Clear();

      //stop thread
      if (networkThread.Started)
      {
        logger.Write(LogLevel.Information, LogThreadsStopping);
        networkThread.StopThread(true);
        logger.Write(LogLevel.Information, LogThreadsStopped);
      }

      //shutdown network
      try
      {
        logger.Write(LogLevel.Information, String.Format(LogNetworkClosing, networkProtocol));
        device.Stop();
        //_queue.Clear();
        networkProtocol.Close();
        logger.Write(LogLevel.Information, LogNetworkClosed);
      }
      catch (Exception e)
      {
        logger.Write(e, String.Format(LogNetworkCloseError, networkProtocol));
      }
      finally
      {
        networkEvent.Close();
      }

      //remove extensions
      if (Extensions.Count > 0)
      {
        logger.Write(LogLevel.Information, LogExtensionsDetaching);
        RemoveExtensions();
        logger.Write(LogLevel.Information, LogExtensionsDetached);
      }

      //write session time
      SessionTimeMonitoringObject smo = monitorObjects.OfType<SessionTimeMonitoringObject>().FirstOrDefault();
      if (smo != null)
      {
        smo.Refresh();
        logger.Write(LogLevel.Information, String.Format(LogApplicationSessionTime, smo.Value));
      }
      monitorObjects.Clear();

      //ok
      logger.Write(LogLevel.Information, LogApplicationStopped);
    }

    protected override void ApplySettingsCore(SettingsResult result)
    {
      Settings settings = Settings.Default;
      logger.Threshold = settings.LogLevel;
    }

    protected override void OnAfterRun(object state)
    {
      base.OnAfterRun(new SberBankApplicationState(IsRunning, runErrors, failedExtensions));
    }

    #endregion

    #region [ ISberBank ]

    /// <summary>
    /// Возвращает интерфейс логгера
    /// </summary>
    [InjectionProperty]
    public ILogger Logger
    {
      get { return logger; }
      private set { logger = value; }
    }

    /// <summary>
    /// Вызывает событие сервера с указанными параметрами
    /// </summary>
    public void InvokeEvent(Type handlerType, params object[] eventParameters)
    {
      events[handlerType].DynamicInvoke(eventParameters);
    }

    #endregion

    #region [ ISberBankApplication ]

    /// <summary>
    /// Расширение загружено
    /// </summary>
    public event Action<IEnumerable<ISberBankExtension>, int> RuntimeExtensionsLoaded;

    /// <summary>
    /// Расширение подключено/отключено
    /// </summary>
    public event Action<ISberBankExtension, bool, bool> ExtensionControlCompleted;

    /// <summary>
    /// Получить состояние оконного потока
    /// </summary>
    public event Func<System.Threading.ThreadState> GetThreadState;

    /// <summary>
    /// Возвращает режим работы сервера
    /// </summary>
    public SberBankMode Mode
    {
      get { return mode; }
    }

    /// <summary>
    /// Возвращает менеждер путей
    /// </summary>
    [InjectionProperty]
    public PathManager PathManager
    {
      get { return pathManager; }
      private set { pathManager = value; }
    }

    /// <summary>
    /// Возвращает настройки фильтра расширений
    /// </summary>
    public TypeFilterSettings ExtensionFilterSettings
    {
      get { return new TypeFilterSettings("filter.xml"); }
    }

    /// <summary>
    /// Возвращает сетевой протокол
    /// </summary>
    [InjectionProperty]
    public INetworkProtocol Network
    {
      get { return networkProtocol; }
      private set
      {
        networkProtocol = value;
        networkProtocol.PacketReceived += OnPacketReceived;
        networkProtocol.Error += OnNetworkError;
      }
    }

    /// <summary>
    /// Загружает расширения во время выполнения
    /// </summary>
    public void LoadExtensionsRuntime(string fileName)
    {
      ThreadPool.QueueUserWorkItem(state =>
      {
        LoadExtensionsRuntimeInternal(fileName);
      });
    }
    private void LoadExtensionsRuntimeInternal(string fileName)
    {
      int failedCount = 0;
      List<ISberBankExtension> loadedExtensions = new List<ISberBankExtension>();
      try
      {
        LoadExtensionsFromFile(fileName, null,
        (se) =>
        {
          loadedExtensions.Add(se);
          OnExtensionLoaded(se);
        },
        (se, t, a, e) =>
        {
          failedCount++;
          OnExtensionLoadFailed(se, t, a, e);
        });
      }
      catch (Exception e)
      {
        logger.Write(e);
      }

      if (RuntimeExtensionsLoaded != null)
        RuntimeExtensionsLoaded(loadedExtensions, failedCount);
    }

    /// <summary>
    /// Подключает/отключает расширение
    /// </summary>
    public void ExtensionControl(ISberBankExtension extension, bool attach)
    {
      ThreadPool.QueueUserWorkItem(state =>
      {
        ExtensionControlInternal(extension, attach, false);
      });
    }
    private void ExtensionControlInternal(ISberBankExtension extension, bool attach, bool throwException)
    {
      bool b = false;
      try
      {
        if (attach)
        {
          Extensions.Add(extension);
          b = true;
        }
        else
        {
          if (Common.IsExtensionRequired(extension.GetType()))
            throw new InvalidOperationException(String.Format("Расширение {0} не может быть отключено", extension.Name));

          b = Extensions.Remove(extension);
        }

        logger.Write(LogLevel.Debug, String.Format(attach ? LogExtensionLoaded : LogExtensionRemoved, extension.Name));
      }
      catch (Exception e)
      {
        logger.Write(e, String.Format(attach ? LogExtensionLoadError : LogExtensionRemoveError, extension.Name));
        if (throwException)
          throw e;
      }

      if (ExtensionControlCompleted != null)
        ExtensionControlCompleted(extension, attach, b);
    }

    /// <summary>
    /// Возвращает техническую информацию
    /// </summary>
    public IEnumerable<string> GetRuntimeInfo()
    {
      List<string> info = new List<string>();

      //thread state
      int count = ThreadBase.RunningThreads.Count();
      string windowThreadState = String.Empty;
      if (GetThreadState != null)
      {
        windowThreadState = GetThreadState().ToString();
        count++;
      }

      StringBuilder sb = new StringBuilder(256);
      sb.AppendFormat("Threads = {0}", count);
      sb.AppendLine();
      if (!String.IsNullOrEmpty(windowThreadState))
        sb.AppendLine(String.Format("Main Window ({0})", windowThreadState));

      sb.AppendLine(ThreadBase.RunningThreads.GetString(t => String.Format("{0} ({1})", ((Thread)t).Name, ((Thread)t).ThreadState), Environment.NewLine));

      info.Add(sb.ToString());

      //monitoring
      sb.Length = 0;
      sb.AppendFormat("Monitoring = {0}", monitorObjects.Count);
      foreach (IMonitoringObject mo in monitorObjects)
      {
        mo.Refresh();
        sb.AppendLine();
        sb.AppendFormat("{0} = {1}", mo.Name, mo.Value);
      }

      info.Add(sb.ToString());

      return info;
    }

    /// <summary>
    /// Выполнение удаленной команды
    /// </summary>
    public object ExecuteRemoteCommand(string command, object parameter)
    {
      if (command != "ext_stop")
        throw new NotImplementedException(String.Format("Command {0} not supported", command));

      string s = String.Empty;
      try
      {
        string n = ((object[])parameter)[0].ToString();
        ISberBankExtension x = Extensions.FirstOrDefault(e => e.GetType().Name == n) as ISberBankExtension;
        if (x == null)
          throw new ApplicationException(String.Format("Extension {0} not found", n));

        ExtensionControlInternal(x, false, true);
        s = String.Format("Extension {0} stopped", n);
      }
      catch (Exception e)
      {
        s = e.Message;
      }

      return s;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return this.ToString();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(SberBankLogCategories.SberBankBase);
    }

    #endregion

    #region [ ISupportMonitoring ]

    public IEnumerable<IMonitoringObject> MonitoringObjects
    {
      get { return monitorObjects; }
    }

    #endregion

    #region [ extensions ]

    protected override bool OnExtensionLoading(ISberBankExtension extension)
    {
      string s = String.Format(LogExtensionLoading, extension.Name);
      logger.Write(LogLevel.Verbose, s);
      if (!IsRunning)
        UpdateStartupProgress(s);

      return true;
    }

    protected override void OnExtensionLoaded(ISberBankExtension extension)
    {
      BindExtension(extension);

      string s = String.Format(LogExtensionLoaded, extension.Name);
      logger.Write(LogLevel.Debug, s);
      if (!IsRunning)
        UpdateStartupProgress(s);
    }

    protected override void OnExtensionLoadFailed(ISberBankExtension extension, Type type, Assembly assembly, Exception exception)
    {
      string s = Common.GetExtensionDescription(extension, type, assembly);
      logger.Write(exception, String.Format(LogExtensionLoadError, s));
      failedExtensions.Add(s);
      runErrors = true;
    }

    protected override bool OnExtensionRemoving(ISberBankExtension extension)
    {
      logger.Write(LogLevel.Verbose, String.Format(LogExtensionRemoving, extension.Name));

      return true;
    }

    protected override void OnExtensionRemoved(ISberBankExtension extension)
    {
      UnbindExtension(extension);

      logger.Write(LogLevel.Debug, String.Format(LogExtensionRemoved, extension.Name));
    }

    protected override void OnExtensionRemoveFailed(ISberBankExtension extension, Exception exception)
    {
      logger.Write(exception, String.Format(LogExtensionRemoveError, extension.Name));
    }

    private void OnExtensionException(Delegate action, Exception exception)
    {
      logger.Write(exception, String.Format(LogExtensionError,
        Common.GetExtensionDescription((IExtensionObject)action.Target, action.Method.DeclaringType, null)));
    }

    private bool BindExtension(ISberBankExtension extension)
    {
      bool bound = false;

      //IOutputPacketHandler outputPacketHandler = extension as IOutputPacketHandler;
      //if (outputPacketHandler != null)
      //{
      //  PacketOutput += outputPacketHandler.OnPacketOutput;
      //  bound = true;
      //}

      //IInputPacketHandler inputPacketHandler = extension as IInputPacketHandler;
      //if (inputPacketHandler != null)
      //{
      //  PacketInput += inputPacketHandler.OnPacketInput;
      //  bound = true;
      //}

      return bound;
    }

    private void UnbindExtension(ISberBankExtension extension)
    {
      //IOutputPacketHandler outputPacketHandler = extension as IOutputPacketHandler;
      //if (outputPacketHandler != null)
      //  PacketOutput -= outputPacketHandler.OnPacketOutput;

      //IInputPacketHandler inputPacketHandler = extension as IInputPacketHandler;
      //if (inputPacketHandler != null)
      //  PacketInput -= inputPacketHandler.OnPacketInput;
    }

    private Predicate<Type> CreateExtensionFilter()
    {
      Predicate<Type> filter = null;
      TypeFilterSettings settings = ExtensionFilterSettings;
      try
      {
        settings.Load();
        filter = new Predicate<Type>(t =>
        {
          bool filtered = settings[t];
          if (filtered)
            extensionsFiltered++;

          return !filtered;
        });
      }
      catch (Exception e)
      {
        logger.Write(e, LogExtensionErrorLoadingFilter);
        runErrors = true;
      }

      return filter;
    }

    #endregion

    /// <summary>
    /// Транзакция завершена
    /// </summary>
    private void OnTransactionCompleted(TransactionResult r, AuthResponse z)
    {
      logger.Write(LogLevel.Information, String.Format("{0}\r\n{1}", r, z));
      if (r == TransactionResult.OK)
        AuthSuccessfull++;
      else //if (r == TransactionResult.Error)
        AuthRejected++;

      CreateTransactionPackets(z);
    }

    /// <summary>
    /// Получен пакет
    /// </summary>
    private void OnPacketReceived(Packet packet)
    {
      receivedPacket = packet;
      networkEvent.Set();
    }

    /// <summary>
    /// Ошибка сети
    /// </summary>
    private void OnNetworkError(string message, byte[] data)
    {
      if (logger == null)
        return;

      string s = message;
      if (data != null)
        s = String.Concat(s, Environment.NewLine, DataFormatter.FormatByteArray(data));

      logger.Write(LogLevel.Warning, s, SberBankLogCategories.Network);
    }

    /// <summary>
    /// Создаёт карту категорий лога
    /// </summary>
    private void CreateLogCategoriesMap()
    {
      try
      {
        bool b = false;
        string fileName = Common.MakeLogCategoryMapFileName(pathManager.LogPath, SberBankHelper.LogFileName);
        if (!File.Exists(fileName))
        {
          List<ISupportLogCategory> loggedObjects = new List<ISupportLogCategory>();
          loggedObjects.Add(this);
          loggedObjects.Add(new TechnicalLogCategory());
          loggedObjects.Add(new CommentsLogCategory());
          loggedObjects.Add(new DebugLogCategory());
          loggedObjects.AddRange(Extensions.Cast<ISupportLogCategory>());
          LogCategoryMap.Create(loggedObjects, fileName);
          b = true;
        }

        fileName = Common.MakeLogCategoryMapFileName(pathManager.LogPath, SberBankHelper.LogFileNamePolling);
        if (!File.Exists(fileName))
        {
          List<ISupportLogCategory> loggedObjects = new List<ISupportLogCategory>();
          loggedObjects.Add(new NetworkLogCategory());
          loggedObjects.Add(new NetworkPacketsLogCategory());
          loggedObjects.Add(new NetworkCommandsLogCategory());
          LogCategoryMap.Create(loggedObjects, fileName);
          b = true;
        }

        if (b)
          logger.Write(LogLevel.Verbose, "Создан файл описания категорий лога");
      }
      catch (Exception e)
      {
        logger.Write(e, "Ошибка при создании файла описания категорий лога");
      }
    }

    /// <summary>
    /// Обновляет строку процесса загрузки
    /// </summary>
    private void UpdateStartupProgress(string text)
    {
      if (startupProgress == null)
        return;

      if (text != null)
        startupProgress.ProgressChanged(text);
      else
        startupProgress.ProgressChanged(-1);
    }

    /// <summary>
    /// Возвращает строковое представление сервера
    /// </summary>
    public override string ToString()
    {
      return "Сервер";
    }
  }
}