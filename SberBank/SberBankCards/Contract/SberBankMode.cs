using System;

namespace Parking.SberBank
{
  /// <summary>
  /// Режим работы сервера
  /// </summary>
  [Flags]
  public enum SberBankMode : int
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