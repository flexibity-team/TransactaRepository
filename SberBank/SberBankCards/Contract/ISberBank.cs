using System;
using System.ServiceModel;
using RMLib;
using RMLib.Log;

namespace Parking.SberBank
{
  /// <summary>
  /// Интерфейс сервера
  /// </summary>
  public interface ISberBank : IExtensibleObject<ISberBank>, IApplication
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