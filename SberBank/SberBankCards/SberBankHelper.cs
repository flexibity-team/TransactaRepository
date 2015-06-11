using RMLib.Log;
using RMLib.Log.Formatters;
using RMLib.Log.Writers;
using RMLib.Services;

namespace Parking.SberBank
{
  internal static class SberBankHelper
  {
    #region [ const ]

    public const string LogFileName = "SberBank";
    public const string LogFileNamePolling = "Polling";

    #endregion

    /// <summary>
    /// Создаёт, регистрирует и возвращает логгер
    /// </summary>
    public static ILogger RegisterLogger(this IServiceContainer container, LogLevel threshold, string logPath)
    {
      container.RegisterInstance<ILogFormatter>(new MultilineStringLogFormatter());
      container.RegisterInstance<ILogWriter>(new SberBankLogWriter(logPath));
      container.RegisterInstance<ILogWriter>(new PollingLogWriter(logPath), "Polling");
      container.RegisterInstance<ILogWriter>(new ErrorMonitorLogWriter(), "ErrorMonitor");
#if !DEBUG
      if (threshold != LogLevel.Error)
        LogMessage.MaxStackDepth = 5;
#endif

        var logger = new Logger(10000) {Threshold = threshold};
        container.RegisterInstance<ILogger>(logger);

      return logger;
    }

    /// <summary>
    /// Фильтр сообщений лога сервера для трассировки
    /// </summary>
    public static bool SberBankTraceMessageFilter(ILogMessage message)
    {
      return (message.GetCategoryGroup() != SberBankLogCategoryGroup.Network);
    }

    /// <summary>
    /// Фильтр сообщений лога поллинга для трассировки
    /// </summary>
    public static bool PollingTraceMessageFilter(ILogMessage message)
    {
      return (message.GetCategoryGroup() == SberBankLogCategoryGroup.Network);
    }
  }
}