using Parking.Gazprom.Extensions;
using RMLib;
using RMLib.Log;

namespace Parking.Gazprom
{
  /// <summary>
  /// Группы категорий сообщения лога
  /// </summary>
  internal enum GazpromLogCategoryGroup : int
  {
    /// <summary>
    /// Сервер
    /// </summary>
    Gazprom = 0,

    /// <summary>
    /// Сетевой обмен
    /// </summary>
    Network = 1,

    /// <summary>
    /// Расширения
    /// </summary>
    Extension = 2
  }

  /// <summary>
  /// Классы представления диапазона категорий лога.
  /// Используются для создания файла категорий лога
  /// </summary>
  internal class TechnicalLogCategory : ISupportLogCategory
  {
    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return "Техническая информация";
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Runtime);
    }

    #endregion
  }

  internal class CommentsLogCategory : ISupportLogCategory
  {
    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return "Заметки";
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Comments);
    }

    #endregion
  }

  internal class DebugLogCategory : ISupportLogCategory
  {
    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return "Отладка";
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Debug);
    }

    #endregion
  }

  internal class NetworkLogCategory : ISupportLogCategory
  {
    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return "Сетевой обмен";
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.Network);
    }

    #endregion
  }

  internal class NetworkPacketsLogCategory : ISupportLogCategory
  {
    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return "Пакеты";
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.NetworkPacket);
    }

    #endregion
  }

  internal class NetworkCommandsLogCategory : ISupportLogCategory
  {
    #region [ ISupportLogCategory ]

    public string GetLogCategoryName()
    {
      return "Команды";
    }

    public long GetLogCategoryRange()
    {
      return (long)new IntRange(GazpromLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Категории сообщений лога сервера
  /// </summary>
  internal static class GazpromLogCategories
  {
    #region [ const ]

    public const int GazpromBase = LogContract.DefaultCategory;  //сервер
    public const int Runtime = GazpromBase + 1;                  //техническая информация
    public const int Comments = GazpromBase + 2;                 //заметки
    public const int Debug = GazpromBase + 3;                    //отладка

    public const int Network = GazpromBase + 10;                 //сетевой обмен
    public const int NetworkPacket = GazpromBase + 11;           //пакеты
    public const int NetworkCommand = GazpromBase + 12;          //команды

    #endregion

    /// <summary>
    /// Возвращает категорию сообщения лога
    /// </summary>
    public static GazpromLogCategoryGroup GetCategoryGroup(this ILogMessage message)
    {
      int category = message.Category;
      if (category < Network)
        return GazpromLogCategoryGroup.Gazprom;

      if (category < GazpromExtensionLogCategories.ExtensionBase)
        return GazpromLogCategoryGroup.Network;

      return GazpromLogCategoryGroup.Extension;
    }
  }
}