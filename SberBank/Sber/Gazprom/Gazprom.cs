using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Parking.Data;
using Parking.Gazprom.Extensions;
using Parking.Gazprom.Properties;
using Parking.Monitoring;
using Parking.Network;
using RMLib;
using RMLib.Collections;
using RMLib.Forms;
using RMLib.Log;
using RMLib.Services;
using RMLib.Threading;

namespace Parking.Gazprom
{
  /// <summary>
  /// Интерфейс приложения сервера
  /// </summary>
  internal interface IGazpromApplication : IGazprom, IApplication, ISupportMonitoring, ISupportLogCategory
  {
    /// <summary>
    /// Расширение загружено
    /// </summary>
    event Action<IEnumerable<IGazpromExtension>, int> RuntimeExtensionsLoaded;

    /// <summary>
    /// Расширение подключено/отключено
    /// </summary>
    event Action<IGazpromExtension, bool, bool> ExtensionControlCompleted;

    /// <summary>
    /// Получить состояние оконного потока
    /// </summary>
    event Func<System.Threading.ThreadState> GetThreadState;

    /// <summary>
    /// Возвращает режим работы сервера
    /// </summary>
    GazpromMode Mode { get; }

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
    void ExtensionControl(IGazpromExtension extension, bool attach);

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
  internal class GazpromApplicationState
  {
    public bool WillRun;
    public readonly bool RunErrors;
    public readonly IEnumerable<string> FailedExtensions;

    public GazpromApplicationState(bool willRun, bool runErrors, IEnumerable<string> failedExtensions)
    {
      WillRun = willRun;
      RunErrors = runErrors;
      FailedExtensions = failedExtensions;
    }
  }

  /// <summary>
  /// Сервер
  /// </summary>
  public partial class Gazprom : ApplicationObject<IGazprom, IGazpromExtension>, IGazprom, IGazpromApplication, ISupportLogCategory, ISupportMonitoring
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

    private GazpromMode _mode;
    private bool _runErrors;
    private List<string> _failedExtensions;
    private Dictionary<Type, Delegate> _events;

    private BankingDevice _device;
    private Packet _receivedPacket;
    private ManualResetEvent _networkEvent;
    private INetworkProtocol _networkProtocol;
    private ThreadHost _networkThread;
    private SynchronizedQueue<Packet> _queue;

    private ILogger _logger;
    private PathManager _pathManager;

    private List<IMonitoringObject> _monitorObjects;
    private IProgressState _startupProgress;
    private int _extensionsFiltered;

    #region [ injection ]

    [InjectionProperty(Required = false, ExactBinding = false, Name = ApplicationFormServices.AboutWindowName)]
    private IProgressState AboutFormProgress
    {
      set { _startupProgress = value; }
    }

    #endregion

    #region [ server events ]

    //private event Action<Packet> PacketOutput;
    //private event Action<Packet> PacketInput;
    //private event Action<Device, DeviceCommand> DeviceCommandOutput;
    //private event Action<Device, DeviceCommand> DeviceCommandInput;

    //private void OnPacketOutput(Packet packet)
    //{
    //  if (PacketOutput != null)
    //    PacketOutput.SafeInvoke<Action<Packet>>(
    //      (action) => { action(packet); },
    //      OnExtensionException);
    //}

    //private void OnPacketInput(Packet packet)
    //{
    //  if (PacketInput != null)
    //    PacketInput.SafeInvoke<Action<Packet>>(
    //      (action) => { action(packet); },
    //      OnExtensionException);
    //}

    //private void OnDeviceCommandOutput(Device device, DeviceCommand command)
    //{
    //  if (DeviceCommandOutput != null)
    //    DeviceCommandOutput.SafeInvoke<Action<Device, DeviceCommand>>(
    //      (action) => { action(device, command); },
    //      OnExtensionException);
    //}

    //private void OnDeviceCommandInput(Device device,DeviceCommand command)
    //{
    //  if (DeviceCommandInput != null)
    //    DeviceCommandInput.SafeInvoke<Action<Device, DeviceCommand>>(
    //      (action) => { action(device, command); },
    //      OnExtensionException);
    //}

    #endregion

    public Gazprom(GazpromMode mode)
    {
      _mode = mode;
      _runErrors = false;
      _failedExtensions = new List<string>();
      _events = new Dictionary<Type, Delegate>();

      _device = new BankingDevice((byte)(Settings.Default.CashBoxID & 0xFF));
      _device.Message += (s, e) => { _logger.Write(e ? LogLevel.Error : LogLevel.Verbose, s); };
      _device.StateChanged += (s, i) => { _logger.Write(LogLevel.Debug, String.Format("{0}\r\n{1}", s, i)); };
      _device.TransactionCompleted += OnTransactionCompleted;

      _receivedPacket = null;
      _networkEvent = new ManualResetEvent(false);
      _networkProtocol = null;
      _networkThread = new ThreadHost(NetworkThread, false, true, true);
      _networkThread.Name = "Network Dispatcher";

      _queue = new SynchronizedQueue<Packet>();

      _logger = null;
      _pathManager = null;

      _monitorObjects = new List<IMonitoringObject>();
      _startupProgress = null;
      _extensionsFiltered = 0;
    }

    #region [ ApplicationObject ]

    protected override bool RunCore()
    {
      Stopwatch w = Stopwatch.StartNew();

      string s = String.Format(LogApplicationStarting, new AssemblyInfo().Version.ToString());
      _logger.Write(LogLevel.Information, s);
      UpdateStartupProgress(s);
      UpdateStartupProgress(null);

      _runErrors = false;
      _failedExtensions.Clear();

      //load extensions
      _logger.Write(LogLevel.Information, LogExtensionsAttaching);
      UpdateStartupProgress(LogExtensionsAttaching);
      LoadExtensions(_pathManager.ExtensionsPath, CreateExtensionFilter());
      s = String.Format(LogExtensionsAttached, Extensions.Count, _extensionsFiltered);
      _logger.Write(LogLevel.Information, s);
      UpdateStartupProgress(s);

      //log categories description
      CreateLogCategoriesMap();

      //startup network
      try
      {
        _logger.Write(LogLevel.Information, LogNetworkOpening);
        UpdateStartupProgress(LogNetworkOpening);
        _networkProtocol.Open();
        _logger.Write(LogLevel.Debug, BankingDevice.Version);
        _device.Start();
        s = String.Format(LogNetworkOpened, _networkProtocol);
        _logger.Write(LogLevel.Information, s);
        UpdateStartupProgress(s);
      }
      catch (Exception e)
      {
        _logger.Write(e, String.Format(LogNetworkOpenError, _networkProtocol));
        _runErrors = true;
      }

      //start thread
      try
      {
        _logger.Write(LogLevel.Information, LogThreadsStarting);
        UpdateStartupProgress(LogThreadsStarting);
        _networkThread.StartThread();
        _logger.Write(LogLevel.Information, LogThreadsStarted);
        UpdateStartupProgress(LogThreadsStarted);
      }
      catch (Exception e)
      {
        throw new ApplicationException(LogThreadsStartError, e);
      }

      //add monitor objects
      _monitorObjects.AddRange(new IMonitoringObject[]
      {
        new NetworkStateMonitoringObject(),
        new NetworkErrorsMonitoringObject(),
        new TimedOutPacketsMonitoringObject(),
        new CommandQueueMonitoringObject(_queue),
        new TransactionsMonitoringObject(),
        new TransactionCancelMonitoringObject(),
        new AuthResponseMonitoringObject(),
        new ReconcilationMonitoringObject(),
        new LogQueueMonitoringObject(_logger),
        new SessionTimeMonitoringObject(this)
      });

      //ok
      s = String.Format(LogApplicationStarted, _mode);
      _logger.Write(LogLevel.Information, s);
      UpdateStartupProgress(s);

      //time
      w.Stop();
      _logger.Write(w.Elapsed < TimeSpan.FromSeconds(20) ? LogLevel.Debug : LogLevel.Warning,
        String.Format(LogApplicationStartupTime, Common.GetTimerString(w.Elapsed)));

      return true;
    }

    protected override void ShutdownCore()
    {
      _logger.Write(LogLevel.Information, LogApplicationStopping);
      _monitorObjects.Clear();

      //stop thread
      if (_networkThread.Started)
      {
        _logger.Write(LogLevel.Information, LogThreadsStopping);
        _networkThread.StopThread(true);
        _logger.Write(LogLevel.Information, LogThreadsStopped);
      }

      //shutdown network
      try
      {
        _logger.Write(LogLevel.Information, String.Format(LogNetworkClosing, _networkProtocol));
        _device.Stop();
        //_queue.Clear();
        _networkProtocol.Close();
        _logger.Write(LogLevel.Information, LogNetworkClosed);
      }
      catch (Exception e)
      {
        _logger.Write(e, String.Format(LogNetworkCloseError, _networkProtocol));
      }
      finally
      {
        _networkEvent.Close();
      }

      //remove extensions
      if (Extensions.Count > 0)
      {
        _logger.Write(LogLevel.Information, LogExtensionsDetaching);
        RemoveExtensions();
        _logger.Write(LogLevel.Information, LogExtensionsDetached);
      }

      //write session time
      SessionTimeMonitoringObject smo = _monitorObjects.OfType<SessionTimeMonitoringObject>().FirstOrDefault();
      if (smo != null)
      {
        smo.Refresh();
        _logger.Write(LogLevel.Information, String.Format(LogApplicationSessionTime, smo.Value));
      }
      _monitorObjects.Clear();

      //ok
      _logger.Write(LogLevel.Information, LogApplicationStopped);
    }

    protected override void ApplySettingsCore(SettingsResult result)
    {
      Settings settings = Settings.Default;
      _logger.Threshold = settings.LogLevel;
    }

    protected override void OnAfterRun(object state)
    {
      base.OnAfterRun(new GazpromApplicationState(IsRunning, _runErrors, _failedExtensions));
    }

    #endregion

    #region [ IGazprom ]

    /// <summary>
    /// Возвращает интерфейс логгера
    /// </summary>
    [InjectionProperty]
    public ILogger Logger
    {
      get { return _logger; }
      private set { _logger = value; }
    }

    /// <summary>
    /// Вызывает событие сервера с указанными параметрами
    /// </summary>
    public void InvokeEvent(Type handlerType, params object[] eventParameters)
    {
      _events[handlerType].DynamicInvoke(eventParameters);
    }

    #endregion

    #region [ IGazpromApplication ]

    /// <summary>
    /// Расширение загружено
    /// </summary>
    public event Action<IEnumerable<IGazpromExtension>, int> RuntimeExtensionsLoaded;

    /// <summary>
    /// Расширение подключено/отключено
    /// </summary>
    public event Action<IGazpromExtension, bool, bool> ExtensionControlCompleted;

    /// <summary>
    /// Получить состояние оконного потока
    /// </summary>
    public event Func<System.Threading.ThreadState> GetThreadState;

    /// <summary>
    /// Возвращает режим работы сервера
    /// </summary>
    public GazpromMode Mode
    {
      get { return _mode; }
    }

    /// <summary>
    /// Возвращает менеждер путей
    /// </summary>
    [InjectionProperty]
    public PathManager PathManager
    {
      get { return _pathManager; }
      private set { _pathManager = value; }
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
      get { return _networkProtocol; }
      private set
      {
        _networkProtocol = value;
        _networkProtocol.PacketReceived += OnPacketReceived;
        _networkProtocol.Error += OnNetworkError;
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
      List<IGazpromExtension> loadedExtensions = new List<IGazpromExtension>();
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
        _logger.Write(e);
      }

      if (RuntimeExtensionsLoaded != null)
        RuntimeExtensionsLoaded(loadedExtensions, failedCount);
    }

    /// <summary>
    /// Подключает/отключает расширение
    /// </summary>
    public void ExtensionControl(IGazpromExtension extension, bool attach)
    {
      ThreadPool.QueueUserWorkItem(state =>
      {
        ExtensionControlInternal(extension, attach, false);
      });
    }
    private void ExtensionControlInternal(IGazpromExtension extension, bool attach, bool throwException)
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

        _logger.Write(LogLevel.Debug, String.Format(attach ? LogExtensionLoaded : LogExtensionRemoved, extension.Name));
      }
      catch (Exception e)
      {
        _logger.Write(e, String.Format(attach ? LogExtensionLoadError : LogExtensionRemoveError, extension.Name));
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
      sb.AppendFormat("Monitoring = {0}", _monitorObjects.Count);
      foreach (IMonitoringObject mo in _monitorObjects)
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
        IGazpromExtension x = Extensions.FirstOrDefault(e => e.GetType().Name == n) as IGazpromExtension;
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
      return (long)new IntRange(GazpromLogCategories.GazpromBase);
    }

    #endregion

    #region [ ISupportMonitoring ]

    public IEnumerable<IMonitoringObject> MonitoringObjects
    {
      get { return _monitorObjects; }
    }

    #endregion

    #region [ extensions ]

    protected override bool OnExtensionLoading(IGazpromExtension extension)
    {
      string s = String.Format(LogExtensionLoading, extension.Name);
      _logger.Write(LogLevel.Verbose, s);
      if (!IsRunning)
        UpdateStartupProgress(s);

      return true;
    }

    protected override void OnExtensionLoaded(IGazpromExtension extension)
    {
      BindExtension(extension);

      string s = String.Format(LogExtensionLoaded, extension.Name);
      _logger.Write(LogLevel.Debug, s);
      if (!IsRunning)
        UpdateStartupProgress(s);
    }

    protected override void OnExtensionLoadFailed(IGazpromExtension extension, Type type, Assembly assembly, Exception exception)
    {
      string s = Common.GetExtensionDescription(extension, type, assembly);
      _logger.Write(exception, String.Format(LogExtensionLoadError, s));
      _failedExtensions.Add(s);
      _runErrors = true;
    }

    protected override bool OnExtensionRemoving(IGazpromExtension extension)
    {
      _logger.Write(LogLevel.Verbose, String.Format(LogExtensionRemoving, extension.Name));

      return true;
    }

    protected override void OnExtensionRemoved(IGazpromExtension extension)
    {
      UnbindExtension(extension);

      _logger.Write(LogLevel.Debug, String.Format(LogExtensionRemoved, extension.Name));
    }

    protected override void OnExtensionRemoveFailed(IGazpromExtension extension, Exception exception)
    {
      _logger.Write(exception, String.Format(LogExtensionRemoveError, extension.Name));
    }

    private void OnExtensionException(Delegate action, Exception exception)
    {
      _logger.Write(exception, String.Format(LogExtensionError,
        Common.GetExtensionDescription((IExtensionObject)action.Target, action.Method.DeclaringType, null)));
    }

    private bool BindExtension(IGazpromExtension extension)
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

    private void UnbindExtension(IGazpromExtension extension)
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
            _extensionsFiltered++;

          return !filtered;
        });
      }
      catch (Exception e)
      {
        _logger.Write(e, LogExtensionErrorLoadingFilter);
        _runErrors = true;
      }

      return filter;
    }

    #endregion

    /// <summary>
    /// Транзакция завершена
    /// </summary>
    private void OnTransactionCompleted(TransactionResult r, AuthResponse z)
    {
      _logger.Write(LogLevel.Information, String.Format("{0}\r\n{1}", r, z));
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
      _receivedPacket = packet;
      _networkEvent.Set();
    }

    /// <summary>
    /// Ошибка сети
    /// </summary>
    private void OnNetworkError(string message, byte[] data)
    {
      if (_logger == null)
        return;

      string s = message;
      if (data != null)
        s = String.Concat(s, Environment.NewLine, DataFormatter.FormatByteArray(data));

      _logger.Write(LogLevel.Warning, s, GazpromLogCategories.Network);
    }

    /// <summary>
    /// Создаёт карту категорий лога
    /// </summary>
    private void CreateLogCategoriesMap()
    {
      try
      {
        bool b = false;
        string fileName = Common.MakeLogCategoryMapFileName(_pathManager.LogPath, GazpromHelper.LogFileName);
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

        fileName = Common.MakeLogCategoryMapFileName(_pathManager.LogPath, GazpromHelper.LogFileNamePolling);
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
          _logger.Write(LogLevel.Verbose, "Создан файл описания категорий лога");
      }
      catch (Exception e)
      {
        _logger.Write(e, "Ошибка при создании файла описания категорий лога");
      }
    }

    /// <summary>
    /// Обновляет строку процесса загрузки
    /// </summary>
    private void UpdateStartupProgress(string text)
    {
      if (_startupProgress == null)
        return;

      if (text != null)
        _startupProgress.ProgressChanged(text);
      else
        _startupProgress.ProgressChanged(-1);
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