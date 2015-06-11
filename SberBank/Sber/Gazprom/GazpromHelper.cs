using RMLib.Log;
using RMLib.Log.Formatters;
using RMLib.Log.Writers;
using RMLib.Services;

namespace Parking.Gazprom
{
  internal static class GazpromHelper
  {
    #region [ const ]

    public const string LogFileName = "Gazprom";
    public const string LogFileNamePolling = "Polling";

    #endregion

    /// <summary>
    /// Создаёт, регистрирует и возвращает логгер
    /// </summary>
    public static ILogger RegisterLogger(this IServiceContainer container, LogLevel threshold, string logPath)
    {
      container.RegisterInstance<ILogFormatter>(new MultilineStringLogFormatter());
      container.RegisterInstance<ILogWriter>(new GazpromLogWriter(logPath));
      container.RegisterInstance<ILogWriter>(new PollingLogWriter(logPath), "Polling");
      container.RegisterInstance<ILogWriter>(new ErrorMonitorLogWriter(), "ErrorMonitor");
      //container.RegisterTraceWindowLogWriter<TraceWindowLogWriter>();
#if !DEBUG
      if (threshold != LogLevel.Error)
        LogMessage.MaxStackDepth = 5;
#endif

      ILogger logger = new Logger(10000);
      logger.Threshold = threshold;
      container.RegisterInstance<ILogger>(logger);

      return logger;
    }

    /// <summary>
    /// Фильтр сообщений лога сервера для трассировки
    /// </summary>
    public static bool GazpromTraceMessageFilter(ILogMessage message)
    {
      return (message.GetCategoryGroup() != GazpromLogCategoryGroup.Network);
    }

    /// <summary>
    /// Фильтр сообщений лога поллинга для трассировки
    /// </summary>
    public static bool PollingTraceMessageFilter(ILogMessage message)
    {
      return (message.GetCategoryGroup() == GazpromLogCategoryGroup.Network);
    }
  }
}