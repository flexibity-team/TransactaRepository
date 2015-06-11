using System;
using System.ServiceModel;
using RMLib;
using RMLib.Log;

namespace Parking.Gazprom
{
  /// <summary>
  /// Интерфейс сервера
  /// </summary>
  public interface IGazprom : IExtensibleObject<IGazprom>, IApplication
  {
    /// <summary>
    /// Возвращает логгер
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// Вызывает событие сервера с указанными параметрами
    /// </summary>
    void InvokeEvent(Type handlerType, params object[] eventParameters);
  }
}