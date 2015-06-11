
namespace Parking.Gazprom
{
  /// <summary>
  /// Категории объектов мониторинга
  /// </summary>
  public enum MonitoringObjectCategory : int
  {
    /// <summary>
    /// Сеть
    /// </summary>
    Network = 1,

    /// <summary>
    /// Команды
    /// </summary>
    Commands = 2,

    /// <summary>
    /// Расширения
    /// </summary>
    Extensions = 3,

    /// <summary>
    /// Прочее
    /// </summary>
    Other = 4
  }
}