using System;
using System.Reflection;

namespace Parking.SberBank
{
  /// <summary>
  /// Методы для работы с версией системы
  /// </summary>
  public static class SystemVersion
  {
    /// <summary>
    /// Возвращает версию системы
    /// </summary>
    public static readonly Version Version;

    /// <summary>
    /// Возвращает редакцию системы
    /// </summary>
    public static readonly string Edition;

    /// <summary>
    /// Возвращает версию .NET Framework, необходимую для работы текущей версии системы
    /// </summary>
    public static readonly Version RequiredFrameworkVersion;

    /// <summary>
    /// Возвращает номер пакета обновления .NET Framework, необходимый для работы текущей версии системы
    /// </summary>
    public static readonly int RequiredFrameworkServicePackLevel;

    /// <summary>
    /// Возвращает номер обновления БД, необходимый для работы текущей версии системы
    /// </summary>
    public static readonly int RequiredDatabasePatchIndex;

    static SystemVersion()
    {
      Version = Assembly.GetExecutingAssembly().GetName().Version;
      Edition = String.Empty;

      RequiredFrameworkVersion = new Version(4, 5, 0, 0);
      RequiredFrameworkServicePackLevel = 1;
      RequiredDatabasePatchIndex = 0;
    }
  }
}