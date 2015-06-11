using System;
using System.Collections.Generic;

namespace Parking.SberBank.Extensions
{
    using Monitoring;
    using RMLib;
    using RMLib.Log;

    /// <summary>
  /// Абстрактный базовый класс расширения сервера
  /// </summary>
  public abstract class SberBankExtension : LoggedExtension<ISberBank>, ISberBankExtension, ISupportMonitoring
  {
    private readonly List<IMonitoringObject> monitoringObjects; //объекты мониторинга

    protected SberBankExtension(string name, string description)
      : this(name, description, Int32.MaxValue, SberBankExtensionLogCategories.ExtensionBase)
    {
      //
    }

    protected SberBankExtension(string name, string description, int priority, int logCategoryBase)
      : base(name, description, priority, new IntRange(logCategoryBase, logCategoryBase + SberBankExtensionLogCategories.GroupSize - 1))
    {
      monitoringObjects = new List<IMonitoringObject>();
    }

    #region [ ExtensionObject ]

    protected override void OnAttach(ISberBank owner)
    {
      base.OnAttach(owner);

      Logger = owner.Logger;
    }

    protected override void OnDetach(ISberBank owner)
    {
      Logger = null;

      base.OnDetach(owner);
    }

    #endregion

    #region [ ISberBankExtension ]

    public event Action<LogLevel, string> Notification;

    protected virtual void OnNotification(LogLevel level, string message)
    {
      if (Notification != null)
        Notification(level, message);
    }

    #endregion

    #region [ ISupportMonitoring ]

    public IEnumerable<IMonitoringObject> MonitoringObjects
    {
      get { return monitoringObjects; }
    }

    #endregion

    /// <summary>
    /// Регистрирует объект мониторинга
    /// </summary>
    protected void RegisterMonitoringObject(IMonitoringObject monitoringObject)
    {
      monitoringObjects.Add(monitoringObject);
    }
  }
}