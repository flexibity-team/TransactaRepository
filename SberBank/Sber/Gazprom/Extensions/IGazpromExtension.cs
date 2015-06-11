using System;
using RMLib;
using RMLib.Log;

namespace Parking.Gazprom.Extensions
{
  /// <summary>
  /// Интерфейс расширения сервера
  /// </summary>
  public interface IGazpromExtension : IExtensionObject<IGazprom>
  {
    /// <summary>
    /// Уведомление
    /// </summary>
    event Action<LogLevel, string> Notification;
  }
}