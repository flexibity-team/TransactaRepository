using System;

namespace Parking.Gazprom
{
  /// <summary>
  /// Режим работы сервера
  /// </summary>
  [Flags]
  public enum GazpromMode : int
  {
    /// <summary>
    /// Обычный
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Запуск без поддержки трассировки
    /// </summary>
    NoTrace = 16
  }
}