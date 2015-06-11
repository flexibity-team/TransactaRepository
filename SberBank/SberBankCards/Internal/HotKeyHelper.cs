namespace Parking.SberBank.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using Data;
    using Network;
    using RMLib;
    using RMLib.Log;
    using RMLib.Log.Writers;

    /// <summary>
  /// Действие горячей клавиши
  /// </summary>
  internal enum HotKeyAction
    {
    /// <summary>
    /// Не указано
    /// </summary>
    None = 0,

    /// <summary>
    /// Открыть папку с лог-файлами
    /// </summary>
    OpenLogFolder = 1,

    /// <summary>
    /// Записать техническую информацию в лог
    /// </summary>
    LogRuntimeInfo = 2,

    /// <summary>
    /// Записать комментарий в лог
    /// </summary>
    LogComment = 3,

    /// <summary>
    /// Удалить файлы лога
    /// </summary>
    DeleteLogFiles = 4,

    /// <summary>
    /// Управление сетевым обменом
    /// </summary>
    NetworkControl = 5,

    /// <summary>
    /// Открывает текущий файл лога
    /// </summary>
    OpenLogFile = 6,

    /// <summary>
    /// Открывает окно редактирования списка загружаемых расширений
    /// </summary>
    ShowExtensionFilter = 7,

    /// <summary>
    /// Включение/отключение логгера
    /// </summary>
    LogControl = 8
  }

  /// <summary>
  /// Обработка горячих клавиш сервера
  /// </summary>
  internal static class HotKeyHelper
  {
    #region [ const ]

    private const string MonitorObjectsFileName = "MonitorObjects.log";
    private const string DumpSeparator = "---------";
    private const int DeviceCommandQueueDumpSize = 200;
    private const int TransactionQueueDumpSize = 10;

    #endregion

    private static readonly Dictionary<HotKeyAction, bool> locks;

    static HotKeyHelper()
    {
      locks = new Dictionary<HotKeyAction, bool>();
      foreach (HotKeyAction da in RuntimeHelper.GetEnumElements<HotKeyAction>())
        locks.Add(da, false);
    }

    /// <summary>
    /// Выполняет указанное действие горячей клавиши
    /// </summary>
    public static void Run(HotKeyAction action, ISberBankApplication sberBank, object param)
    {
      if (locks[action])
      {
        //Trace.WriteLine(String.Format("DiagnosticAction {0} is LOCKED", action));
        return;
      }

      if (ThreadRequired(action))
        ThreadPool.QueueUserWorkItem(p => { RunCore(action, sberBank, param); });
      else
        RunCore(action, sberBank, param);
    }

    /// <summary>
    /// Выполняет указанное действие горячей клавиши
    /// </summary>
    private static void RunCore(HotKeyAction action, ISberBankApplication sberBank, object param)
    {
      locks[action] = true;
      try
      {
        Console.Beep();
        switch (action)
        {
          case HotKeyAction.OpenLogFolder:
            OpenLogFolder(sberBank);
            break;
          case HotKeyAction.LogRuntimeInfo:
            WriteRuntimeInfoToLog(sberBank);
            break;
          case HotKeyAction.LogComment:
            WriteLogComment(sberBank, param);
            break;
          case HotKeyAction.DeleteLogFiles:
            DeleteLogFiles(sberBank);
            break;
          case HotKeyAction.NetworkControl:
            NetworkControl(sberBank);
            break;
          case HotKeyAction.OpenLogFile:
            OpenLogFile(sberBank);
            break;
          case HotKeyAction.LogControl:
            LogControl(sberBank);
            break;
        }
      }
      catch (Exception e)
      {
        MessageBox.Show(String.Format("Ошибка при выполнении действия {0}\r\n\r\n{1}", action, e.Message), ApplicationServices.GetApplicationName());
      }
      finally
      {
        locks[action] = false;
      }
    }

    /// <summary>
    /// Требуется ли выполнение в отдельном потоке
    /// </summary>
    private static bool ThreadRequired(HotKeyAction action)
    {
      return (action != HotKeyAction.LogComment);
    }

    /// <summary>
    /// Открывает папку, содержащую логи сервера
    /// </summary>
    private static void OpenLogFolder(ISberBankApplication sberBank)
    {
      Process.Start(sberBank.PathManager.LogPath);
    }

    /// <summary>
    /// Записывает техническую информацию в лог
    /// </summary>
    public static void WriteRuntimeInfoToLog(ISberBankApplication sberBank)
    {
      foreach (string s in sberBank.GetRuntimeInfo())
        sberBank.Logger.Write(LogLevel.Error, s, SberBankLogCategories.Runtime);
    }

    /// <summary>
    /// Записывает комментарий в лог
    /// </summary>
    private static void WriteLogComment(ISberBankApplication sberBank, object param)
    {
      string comment = (string)param;
      if (!String.IsNullOrEmpty(comment))
        sberBank.Logger.Write(LogLevel.Error, Common.GetSafeLogComment(comment), SberBankLogCategories.Comments);
    }

    /// <summary>
    /// Удаляет файлы лога
    /// </summary>
    private static void DeleteLogFiles(ISberBankApplication sberBank)
    {
      foreach (var v in new DirectoryInfo(sberBank.PathManager.LogPath).GetFiles(String.Format("*.{0}", FileLogWriter.Extension)))
        v.Delete();
    }

    /// <summary>
    /// Управление сетевым обменом
    /// </summary>
    private static void NetworkControl(ISberBankApplication sberBank)
    {
      SberBank.NetworkForceDisable = !SberBank.NetworkForceDisable;
      sberBank.Logger.Write(SberBank.NetworkForceDisable
          ? LogLevel.Warning
          : LogLevel.Debug, String.Format("Сетевой обмен отключен : {0}", SberBank.NetworkForceDisable.GetString()));
    }

    /// <summary>
    /// Открывает текущий файл лога
    /// </summary>
    private static void OpenLogFile(ISberBankApplication sberBank)
    {
      DirectoryInfo di = new DirectoryInfo(sberBank.PathManager.LogPath);
      if (!di.Exists)
        return;

      string mask = String.Format("{0}*.{1}", SberBankHelper.LogFileName, FileLogWriter.Extension);
      FileInfo fi = di.GetFiles(mask).TakeRecent();
      if (fi == null)
        return;

      try
      {
        Process.Start(fi.FullName);
      }
      catch
      {
        //
      }
    }

    /// <summary>
    /// Включение/отключение логгера
    /// </summary>
    private static void LogControl(ISberBankApplication sberBank)
    {
      sberBank.Logger.Enabled = !sberBank.Logger.Enabled;
    }
  }
}