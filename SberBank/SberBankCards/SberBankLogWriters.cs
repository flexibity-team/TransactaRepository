using System;
using RMLib.Log;
using RMLib.Log.Formatters;
using RMLib.Log.Writers;

namespace Parking.SberBank
{
  /// <summary>
  /// Регистратор сообщений лога для сообщений сервера.
  /// Регистрирует все сообщения, кроме поступающих из потока поллинга
  /// </summary>
  internal class SberBankLogWriter : RollingFileLogWriter
  {
    public SberBankLogWriter(string path)
      : base(path, SberBankHelper.LogFileName, Common.MaxLogFileSize)
    {
      //
    }

    #region [ FileLogWriter ]

    protected override void WriteMessageCore(ILogMessage message, ILogFormatter formatter)
    {
      if (message.GetCategoryGroup() == SberBankLogCategoryGroup.Network)
        return;

      base.WriteMessageCore(message, formatter);
    }

    #endregion
  }

  /// <summary>
  /// Регистратор сообщений лога для сообщений, поступающих из потока поллинга
  /// </summary>
  internal class PollingLogWriter : RollingFileLogWriter
  {
    public PollingLogWriter(string path)
      : base(path, SberBankHelper.LogFileNamePolling, Common.MaxLogFileSize)
    {
      //
    }

    #region [ FileLogWriter ]

    protected override void WriteMessageCore(ILogMessage message, ILogFormatter formatter)
    {
      if (message.GetCategoryGroup() != SberBankLogCategoryGroup.Network)
        return;

      base.WriteMessageCore(message, formatter);
    }

    #endregion
  }

  /// <summary>
  /// Подсчитывает количество сообщений об ошибоках, поступивших в логгер
  /// </summary>
  internal class ErrorMonitorLogWriter : LogWriterBase
  {
    private int errorCount;

    #region [ properties ]

    public int ErrorCount
    {
      get { return errorCount; }
    }

    #endregion

    public ErrorMonitorLogWriter()
    {
      errorCount = 0;
    }

    #region [ LogWriterBase ]

    public override string Destination
    {
      get { return String.Empty; }
    }

    protected override void WriteMessageCore(ILogMessage message, ILogFormatter formatter)
    {
      if (message.Level != LogLevel.Error)
        return;

      errorCount++;
    }

    #endregion

    public void Reset()
    {
      errorCount = 0;
    }
  }
}