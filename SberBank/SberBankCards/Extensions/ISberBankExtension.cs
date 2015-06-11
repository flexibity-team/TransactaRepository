using System;
using RMLib;
using RMLib.Log;

namespace Parking.SberBank.Extensions
{
  /// <summary>
  /// Интерфейс расширения сервера
  /// </summary>
  public interface ISberBankExtension : IExtensionObject<ISberBank>
  {
    /// <summary>
    /// Уведомление
    /// </summary>
    event Action<LogLevel, string> Notification;
  }
}