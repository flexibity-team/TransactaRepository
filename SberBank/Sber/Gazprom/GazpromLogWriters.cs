using System;
using RMLib.Log;
using RMLib.Log.Formatters;
using RMLib.Log.Writers;

namespace Parking.Gazprom
{
  /// <summary>
  /// Регистратор сообщений лога для сообщений сервера.
  /// Регистрирует все сообщения, кроме поступающих из потока поллинга
  /// </summary>
  internal class GazpromLogWriter : RollingFileLogWriter
  {
    public GazpromLogWriter(string path)
      : base(path, GazpromHelper.LogFileName, Common.MaxLogFileSize)
    {
      //
    }

    #region [ FileLogWriter ]

    protected override void WriteMessageCore(ILogMessage message, ILogFormatter formatter)
    {
      if (message.GetCategoryGroup() == GazpromLogCategoryGroup.Network)
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
      : base(path, GazpromHelper.LogFileNamePolling, Common.MaxLogFileSize)
    {
      //
    }

    #region [ FileLogWriter ]

    protected override void WriteMessageCore(ILogMessage message, ILogFormatter formatter)
    {
      if (message.GetCategoryGroup() != GazpromLogCategoryGroup.Network)
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
    private int _errorCount;

    #region [ properties ]

    public int ErrorCount
    {
      get { return _errorCount; }
    }

    #endregion

    public ErrorMonitorLogWriter()
    {
      _errorCount = 0;
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

      _errorCount++;
    }

    #endregion

    public void Reset()
    {
      _errorCount = 0;
    }
  }
}