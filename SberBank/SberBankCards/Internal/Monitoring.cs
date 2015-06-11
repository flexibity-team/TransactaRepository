using System;
using System.Diagnostics;
using System.Linq;
using Parking.Data;
using Parking.Monitoring;
using Parking.Network;
using RMLib;
using RMLib.Collections;
using RMLib.Log;

namespace Parking.SberBank
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

    public override sealed void Refresh()
    {
      bool w = !String.IsNullOrEmpty(SberBank.NetworkState);
      string s = "норма";
      if (SberBank.NetworkForceDisable)
      {
        w = true;
        s = "пауза";
      }
      else if (w)
        s = String.Format("ошибка ({0})", SberBank.NetworkState);

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
    private readonly NetworkLogCategory cat;

    public NetworkErrorsMonitoringObject()
      : base("Ошибки сети", (int)MonitoringObjectCategory.Network)
    {
      cat = new NetworkLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override sealed void Refresh()
    {
      int e = SberBank.NetworkErrors;
      State = (e > 100) ? MonitoringObjectState.Warning : MonitoringObjectState.Normal;
      Value = e.ToString();
    }

    public override void Reset()
    {
      SberBank.NetworkErrors = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(SberBankLogCategories.Network, SberBankLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Неполные пакеты
  /// </summary>
  internal class TimedOutPacketsMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private NetworkLogCategory cat;

    public TimedOutPacketsMonitoringObject()
      : base("Неполные пакеты", (int)MonitoringObjectCategory.Network)
    {
      cat = new NetworkLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override sealed void Refresh()
    {
      Value = SberBank.TimedOutPackets.ToString();
    }

    public override void Reset()
    {
      SberBank.TimedOutPackets = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(SberBankLogCategories.Network, SberBankLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Очередь команд
  /// </summary>
  internal class CommandQueueMonitoringObject : MonitoringObject
  {
    private readonly SynchronizedQueue<Packet> queue;

    public CommandQueueMonitoringObject(SynchronizedQueue<Packet> queue)
      : base("Очередь команд", (int)MonitoringObjectCategory.Commands)
    {
      this.queue = queue;
      Refresh();
    }

    #region [ IMonitorObject ]

    public override sealed void Refresh()
    {
      int c = queue.Count;
      Value = c.ToString();
      State = (c > 2) ? MonitoringObjectState.Warning : MonitoringObjectState.Normal;
    }

    public override object GetUnderlyingObject()
    {
      return queue;
    }

    #endregion
  }

  /// <summary>
  /// Транзакции
  /// </summary>
  internal class TransactionsMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private readonly ISupportLogCategory cat;

    public TransactionsMonitoringObject()
      : base("Транзакции (начато/завершено)", (int)MonitoringObjectCategory.Commands)
    {
      cat = new NetworkCommandsLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override sealed void Refresh()
    {
      Value = String.Format("{0}/{1}", SberBank.TransactionStarted, SberBank.TransactionCompleted);
    }

    public override void Reset()
    {
      SberBank.TransactionStarted = 0;
      SberBank.TransactionCompleted = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(SberBankLogCategories.Network, SberBankLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Отмены
  /// </summary>
  internal class TransactionCancelMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private readonly ISupportLogCategory cat;

    public TransactionCancelMonitoringObject()
      : base("Отменено транзакций", (int)MonitoringObjectCategory.Commands)
    {
      cat = new NetworkCommandsLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override sealed void Refresh()
    {
      Value = SberBank.TransactionCancelled.ToString();
    }

    public override void Reset()
    {
      SberBank.TransactionCancelled = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(SberBankLogCategories.Network, SberBankLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Ответы на запрос
  /// </summary>
  internal class AuthResponseMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private readonly ISupportLogCategory cat;

    public AuthResponseMonitoringObject()
      : base("Запросы (успешно/отказ)", (int)MonitoringObjectCategory.Commands)
    {
      cat = new NetworkCommandsLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override sealed void Refresh()
    {
      Value = String.Format("{0}/{1}", SberBank.AuthSuccessfull, SberBank.AuthRejected);
    }

    public override void Reset()
    {
      SberBank.AuthSuccessfull = 0;
      SberBank.AuthRejected = 0;
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(SberBankLogCategories.Network, SberBankLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Сверка итогов
  /// </summary>
  internal class ReconcilationMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private readonly ISupportLogCategory cat;

    public ReconcilationMonitoringObject()
      : base("Сверка итогов (всего/последняя)", (int)MonitoringObjectCategory.Commands)
    {
      cat = new NetworkCommandsLogCategory();
      Refresh();
    }

    #region [ IMonitorObject ]

    public override sealed void Refresh()
    {
      var s = String.Empty;
      DateTime dt = SberBank.ReconcilationRecentTime;

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

      Value = String.Format("{0}/{1}", SberBank.ReconcilationCount, s);
    }

    #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return cat.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(SberBankLogCategories.Network, SberBankLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Очередь сообщений лога
  /// </summary>
  internal class LogQueueMonitoringObject : MonitoringObject
  {
    private readonly Logger logger;
    private readonly ErrorMonitorLogWriter errorMonitor;

    public LogQueueMonitoringObject(ILogger logger)
      : base("Лог (очередь/ошибки)", (int)MonitoringObjectCategory.Other)
    {
      this.logger = logger as Logger;
      errorMonitor = (this.logger != null) ? (ErrorMonitorLogWriter)this.logger.Writers.FirstOrDefault(w => w is ErrorMonitorLogWriter) : null;

      Refresh();
    }

    #region [ IMonitorObject ]

    public override sealed void Refresh()
    {
      if (logger == null)
        return;

      LogStatistics ls = logger.Statistics;
      Value = String.Format("{0}/{1}", (ls.MessageEnqueued - ls.MessageWritten), (errorMonitor != null) ? errorMonitor.ErrorCount.ToString() : "-");
    }

    public override void Reset()
    {
      if (errorMonitor == null)
        return;

      errorMonitor.Reset();
    }

    #endregion
  }

  /// <summary>
  /// Время текущей сессии
  /// </summary>
  internal class SessionTimeMonitoringObject : MonitoringObject, ISupportLogCategory
  {
    private readonly ISberBankApplication sberBank;
    private readonly Stopwatch watch;

    public SessionTimeMonitoringObject(ISberBankApplication sberBank) : base("Время текущей сессии", (int)MonitoringObjectCategory.Other)
    {
      watch = new Stopwatch();

      this.sberBank = sberBank;
      this.sberBank.AfterRun += (state) => { watch.Start(); };
      this.sberBank.AfterShutdown += (state) => { watch.Stop(); watch.Reset(); };
    }

    #region [ IMonitorObject ]

    public override void Refresh()
    {
        Value = watch.Elapsed == TimeSpan.Zero ? String.Empty : Common.GetTimerString(watch.Elapsed);
    }

      #endregion

    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return sberBank.GetLogCategoryName();
    }

    public long GetLogCategoryRange()
    {
      return sberBank.GetLogCategoryRange();
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