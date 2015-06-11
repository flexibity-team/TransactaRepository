using System;
using System.Collections.Generic;
using Parking.Monitoring;
using RMLib;
using RMLib.Log;

namespace Parking.Gazprom.Extensions
{
  /// <summary>
  /// Абстрактный базовый класс расширения сервера
  /// </summary>
  public abstract class GazpromExtension : LoggedExtension<IGazprom>, IGazpromExtension, ISupportMonitoring
  {
    private List<IMonitoringObject> _monitoringObjects; //объекты мониторинга

    protected GazpromExtension(string name, string description)
      : this(name, description, Int32.MaxValue, GazpromExtensionLogCategories.ExtensionBase)
    {
      //
    }

    protected GazpromExtension(string name, string description, int priority, int logCategoryBase)
      : base(name, description, priority, new IntRange(logCategoryBase, logCategoryBase + GazpromExtensionLogCategories.GroupSize - 1))
    {
      _monitoringObjects = new List<IMonitoringObject>();
    }

    #region [ ExtensionObject ]

    protected override void OnAttach(IGazprom owner)
    {
      base.OnAttach(owner);

      Logger = owner.Logger;
    }

    protected override void OnDetach(IGazprom owner)
    {
      Logger = null;

      base.OnDetach(owner);
    }

    #endregion

    #region [ IGazpromExtension ]

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
      get { return _monitoringObjects; }
    }

    #endregion

    /// <summary>
    /// Регистрирует объект мониторинга
    /// </summary>
    protected void RegisterMonitoringObject(IMonitoringObject monitoringObject)
    {
      _monitoringObjects.Add(monitoringObject);
    }
  }
}