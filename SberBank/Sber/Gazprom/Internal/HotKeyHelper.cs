using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Parking.Data;
using RMLib;
using RMLib.Log;
using RMLib.Log.Writers;

namespace Parking.Gazprom
{
  /// <summary>
  /// Действие горячей клавиши
  /// </summary>
  internal enum HotKeyAction : int
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

    private static readonly Dictionary<HotKeyAction, bool> _locks;

    static HotKeyHelper()
    {
      _locks = new Dictionary<HotKeyAction, bool>();
      foreach (HotKeyAction da in RuntimeHelper.GetEnumElements<HotKeyAction>())
        _locks.Add(da, false);
    }

    /// <summary>
    /// Выполняет указанное действие горячей клавиши
    /// </summary>
    public static void Run(HotKeyAction action, IGazpromApplication gazprom, object param)
    {
      if (_locks[action])
      {
        //Trace.WriteLine(String.Format("DiagnosticAction {0} is LOCKED", action));
        return;
      }

      if (ThreadRequired(action))
        ThreadPool.QueueUserWorkItem(p => { RunCore(action, gazprom, param); });
      else
        RunCore(action, gazprom, param);
    }

    /// <summary>
    /// Выполняет указанное действие горячей клавиши
    /// </summary>
    private static void RunCore(HotKeyAction action, IGazpromApplication gazprom, object param)
    {
      _locks[action] = true;
      try
      {
        Console.Beep();
        switch (action)
        {
          case HotKeyAction.OpenLogFolder:
            OpenLogFolder(gazprom);
            break;
          case HotKeyAction.LogRuntimeInfo:
            WriteRuntimeInfoToLog(gazprom);
            break;
          case HotKeyAction.LogComment:
            WriteLogComment(gazprom, param);
            break;
          case HotKeyAction.DeleteLogFiles:
            DeleteLogFiles(gazprom);
            break;
          case HotKeyAction.NetworkControl:
            NetworkControl(gazprom);
            break;
          case HotKeyAction.OpenLogFile:
            OpenLogFile(gazprom);
            break;
          case HotKeyAction.LogControl:
            LogControl(gazprom);
            break;
        }
      }
      catch (Exception e)
      {
        MessageBox.Show(String.Format("Ошибка при выполнении действия {0}\r\n\r\n{1}", action, e.Message), ApplicationServices.GetApplicationName());
      }
      finally
      {
        _locks[action] = false;
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
    private static void OpenLogFolder(IGazpromApplication gazprom)
    {
      Process.Start(gazprom.PathManager.LogPath);
    }

    /// <summary>
    /// Записывает техническую информацию в лог
    /// </summary>
    public static void WriteRuntimeInfoToLog(IGazpromApplication gazprom)
    {
      foreach (string s in gazprom.GetRuntimeInfo())
        gazprom.Logger.Write(LogLevel.Error, s, GazpromLogCategories.Runtime);
    }

    /// <summary>
    /// Записывает комментарий в лог
    /// </summary>
    private static void WriteLogComment(IGazpromApplication gazprom, object param)
    {
      string comment = (string)param;
      if (!String.IsNullOrEmpty(comment))
        gazprom.Logger.Write(LogLevel.Error, Common.GetSafeLogComment(comment), GazpromLogCategories.Comments);
    }

    /// <summary>
    /// Удаляет файлы лога
    /// </summary>
    private static void DeleteLogFiles(IGazpromApplication gazprom)
    {
      foreach (var v in new DirectoryInfo(gazprom.PathManager.LogPath).GetFiles(String.Format("*.{0}", FileLogWriter.Extension)))
        v.Delete();
    }

    /// <summary>
    /// Управление сетевым обменом
    /// </summary>
    private static void NetworkControl(IGazpromApplication gazprom)
    {
      Gazprom.NetworkForceDisable = !Gazprom.NetworkForceDisable;
      gazprom.Logger.Write(Gazprom.NetworkForceDisable ? LogLevel.Warning : LogLevel.Debug, String.Format("Сетевой обмен отключен : {0}", Gazprom.NetworkForceDisable.GetString()));
    }

    /// <summary>
    /// Открывает текущий файл лога
    /// </summary>
    private static void OpenLogFile(IGazpromApplication gazprom)
    {
      DirectoryInfo di = new DirectoryInfo(gazprom.PathManager.LogPath);
      if (!di.Exists)
        return;

      string mask = String.Format("{0}*.{1}", GazpromHelper.LogFileName, FileLogWriter.Extension);
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
    private static void LogControl(IGazpromApplication gazprom)
    {
      gazprom.Logger.Enabled = !gazprom.Logger.Enabled;
    }
  }
}