using Parking.SberBank.Extensions;
using RMLib;
using RMLib.Log;

namespace Parking.SberBank
{
  /// <summary>
  /// Группы категорий сообщения лога
  /// </summary>
  internal enum SberBankLogCategoryGroup : int
  {
    /// <summary>
    /// Сервер
    /// </summary>
    SberBank = 0,

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
      return (long)new IntRange(SberBankLogCategories.Runtime);
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
      return (long)new IntRange(SberBankLogCategories.Comments);
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
      return (long)new IntRange(SberBankLogCategories.Debug);
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
      return (long)new IntRange(SberBankLogCategories.Network);
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
      return (long)new IntRange(SberBankLogCategories.NetworkPacket);
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
      return (long)new IntRange(SberBankLogCategories.NetworkCommand);
    }

    #endregion
  }

  /// <summary>
  /// Категории сообщений лога сервера
  /// </summary>
  internal static class SberBankLogCategories
  {
    #region [ const ]

    public const int SberBankBase = LogContract.DefaultCategory;  //сервер
    public const int Runtime = SberBankBase + 1;                  //техническая информация
    public const int Comments = SberBankBase + 2;                 //заметки
    public const int Debug = SberBankBase + 3;                    //отладка

    public const int Network = SberBankBase + 10;                 //сетевой обмен
    public const int NetworkPacket = SberBankBase + 11;           //пакеты
    public const int NetworkCommand = SberBankBase + 12;          //команды

    #endregion

    /// <summary>
    /// Возвращает категорию сообщения лога
    /// </summary>
    public static SberBankLogCategoryGroup GetCategoryGroup(this ILogMessage message)
    {
      int category = message.Category;
      if (category < Network)
        return SberBankLogCategoryGroup.SberBank;

      if (category < SberBankExtensionLogCategories.ExtensionBase)
        return SberBankLogCategoryGroup.Network;

      return SberBankLogCategoryGroup.Extension;
    }
  }
}