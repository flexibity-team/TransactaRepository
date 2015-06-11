using System;
using System.Diagnostics;
using System.Linq;
using Parking.Data;
using Parking.Monitoring;
using Parking.Network;
using RMLib;
using RMLib.Collections;
using RMLib.Log;

namespace Parking.Gazprom
{
  /// <summary>
  /// Состояние сети
  /// </summary>
  internal class NetworkStateMonitoringObject : MonitoringObject
  {
    public NetworkStateMonitoringObject()
      : base("Состояние сети", (int)MonitoringObjectCategory.Network)
    {
      Refresh();
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      bool w = !String.IsNullOrEmpty(Gazprom.NetworkState);
      string s = "норма";
      if (Gazprom.NetworkForceDisable)
      {
        w = true;
        s = "пауза";
      }
      else if (w)
        s = String.Format("ошибка ({0})", Gazprom.NetworkState);

      State = w ? MonitoringObjectState.Warning : MonitoringObjectState.Normal;
      Value = s;
    }

    #endregion
  }

  /// <summary>
  /// Ошибки сети
  /// </summary>
  internal class NetworkErrorsMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private NetworkLogCategory _cat;

    public NetworkErrorsMonitoringObject()
      : base("Ошибки сети", (int)MonitoringObjectCategory.Network)
    {
      _cat = new NetworkLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      int e = Gazprom.NetworkErrors;
      State = (e > 100) ? MonitoringObjectState.Warning : MonitoringObjectState.Normal;
      Value = e.ToString();
    }

    public override void Reset()
    {
      Gazprom.NetworkErrors = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return _cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Network, GazpromLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Неполные пакеты
  /// </summary>
  internal class TimedOutPacketsMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private NetworkLogCategory _cat;

    public TimedOutPacketsMonitoringObject()
      : base("Неполные пакеты", (int)MonitoringObjectCategory.Network)
    {
      _cat = new NetworkLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      Value = Gazprom.TimedOutPackets.ToString();
    }

    public override void Reset()
    {
      Gazprom.TimedOutPackets = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return _cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Network, GazpromLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Очередь команд
  /// </summary>
  internal class CommandQueueMonitoringObject : MonitoringObject
  {
    private SynchronizedQueue<Packet> _queue;

    public CommandQueueMonitoringObject(SynchronizedQueue<Packet> queue)
      : base("Очередь команд", (int)MonitoringObjectCategory.Commands)
    {
      _queue = queue;
      Refresh();
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      int c = _queue.Count;
      Value = c.ToString();
      State = (c > 2) ? MonitoringObjectState.Warning : MonitoringObjectState.Normal;
    }

    public override object GetUnderlyingObject()
    {
      return _queue;
    }

    #endregion
  }

  /// <summary>
  /// Транзакции
  /// </summary>
  internal class TransactionsMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private ISupportLogCategory _cat;

    public TransactionsMonitoringObject()
      : base("Транзакции (начато/завершено)", (int)MonitoringObjectCategory.Commands)
    {
      _cat = new NetworkCommandsLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      Value = String.Format("{0}/{1}", Gazprom.TransactionStarted, Gazprom.TransactionCompleted);
    }

    public override void Reset()
    {
      Gazprom.TransactionStarted = 0;
      Gazprom.TransactionCompleted = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return _cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Network, GazpromLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Отмены
  /// </summary>
  internal class TransactionCancelMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private ISupportLogCategory _cat;

    public TransactionCancelMonitoringObject()
      : base("Отменено транзакций", (int)MonitoringObjectCategory.Commands)
    {
      _cat = new NetworkCommandsLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      Value = Gazprom.TransactionCancelled.ToString();
    }

    public override void Reset()
    {
      Gazprom.TransactionCancelled = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return _cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Network, GazpromLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Ответы на запрос
  /// </summary>
  internal class AuthResponseMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private ISupportLogCategory _cat;

    public AuthResponseMonitoringObject()
      : base("Запросы (успешно/отказ)", (int)MonitoringObjectCategory.Commands)
    {
      _cat = new NetworkCommandsLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      Value = String.Format("{0}/{1}", Gazprom.AuthSuccessfull, Gazprom.AuthRejected);
    }

    public override void Reset()
    {
      Gazprom.AuthSuccessfull = 0;
      Gazprom.AuthRejected = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return _cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Network, GazpromLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Сверка итогов
  /// </summary>
  internal class ReconcilationMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private ISupportLogCategory _cat;

    public ReconcilationMonitoringObject()
      : base("Сверка итогов (всего/последняя)", (int)MonitoringObjectCategory.Commands)
    {
      _cat = new NetworkCommandsLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      string s = String.Empty;
      DateTime dt = Gazprom.ReconcilationRecentTime;
      if (dt == DateTime.MinValue)
        s = "-";
      else
      {
        TimeSpan ts = dt - DataContract.DefaultDateTime;
        TimeSpan tsNow = DateTime.Now - DataContract.DefaultDateTime;
        if ((tsNow.TotalDays - ts.TotalDays) < 1)
          s = DataFormatter.FormatTimeHM(dt.TimeOfDay);
        else
          s = dt.Date.ToString("dd.MM.yyyy");
      }

      Value = String.Format("{0}/{1}", Gazprom.ReconcilationCount, s);
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return _cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Network, GazpromLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Очередь сообщений лога
  /// </summary>
  internal class LogQueueMonitoringObject : MonitoringObject
  {
    private Logger _logger;
    private ErrorMonitorLogWriter _errorMonitor;

    public LogQueueMonitoringObject(ILogger logger)
      : base("Лог (очередь/ошибки)", (int)MonitoringObjectCategory.Other)
    {
      _logger = logger as Logger;
      _errorMonitor = (_logger != null) ? (ErrorMonitorLogWriter)_logger.Writers.FirstOrDefault(w => w is ErrorMonitorLogWriter) : null;

      Refresh();
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      if (_logger == null)
        return;

      LogStatistics ls = _logger.Statistics;
      Value = String.Format("{0}/{1}", (ls.MessageEnqueued - ls.MessageWritten), (_errorMonitor != null) ? _errorMonitor.ErrorCount.ToString() : "-");
    }

    public override void Reset()
    {
      if (_errorMonitor == null)
        return;

      _errorMonitor.Reset();
    }

    #endregion
  }

  /// <summary>
  /// Время текущей сессии
  /// </summary>
  internal class SessionTimeMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private IGazpromApplication _gazprom;
    private Stopwatch _watch;

    public SessionTimeMonitoringObject(IGazpromApplication gazprom)
      : base("Время текущей сессии", (int)MonitoringObjectCategory.Other)
    {
      _watch = new Stopwatch();

      _gazprom = gazprom;
      _gazprom.AfterRun += (state) => { _watch.Start(); };
      _gazprom.AfterShutdown += (state) => { _watch.Stop(); _watch.Reset(); };
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
      if (_watch.Elapsed == TimeSpan.Zero)
        Value = String.Empty;
      else
        Value = Common.GetTimerString(_watch.Elapsed);
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return _gazprom.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return _gazprom.GetLogCategoryRange();
    }

    #endregion
  }

  /// <summary>
  /// Методы для работы с мониторингом
  /// </summary>
  internal static class MonitoringExtensions
  {
    /// <summary>
    /// Возвращает строковое представление категории объекта мониторинга
    /// </summary>
    public static string GetString(this MonitoringObjectCategory category)
    {
      string s = String.Empty;
      switch (category)
      {
        case MonitoringObjectCategory.Network:
          s = "Сеть";
          break;
        case MonitoringObjectCategory.Commands:
          s = "Команды";
          break;
        case MonitoringObjectCategory.Extensions:
          s = "Расширения";
          break;
        case MonitoringObjectCategory.Other:
          s = "Прочее";
          break;
        default:
          s = category.ToString();
          break;
      }

      return s;
    }
  }
}