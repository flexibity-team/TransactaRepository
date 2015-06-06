using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Parking.Data;
using Parking.Data.Metro;
using RMLib;
using RMLib.Xml;

namespace Parking.CashDesk
{
  /// <summary>
  /// Язык ручной кассы
  /// </summary>
  public enum CashDeskLanguage
  {
    /// <summary>
    /// Русский
    /// </summary>
    Ru = 0,

    /// <summary>
    /// Азербайджанский
    /// </summary>
    Az = 1
  }

  /// <summary>
  /// Вспомогательные методы
  /// </summary>
  internal static class CashDeskHelper
  {
    #region [ const ]

    public const string TariffMetroFileName = "tariff_metro_k.txt";
    public const string TariffMetroDiscountFileName = "tariff_metro_l.txt";

    #endregion

    private static readonly Type[] TariffKnownTypes;

    static CashDeskHelper()
    {
      TariffKnownTypes = new[] { typeof(Tariff30), typeof(TariffMetro) };
    }

    /// <summary>
    /// Возвращает строковое представление языка ручной кассы
    /// </summary>
    public static string GetString(this CashDeskLanguage language)
    {
      string s = String.Empty;
      switch (language)
      {
        case CashDeskLanguage.Ru:
          s = "Русский";
          break;
        case CashDeskLanguage.Az:
          s = "Азербайджанский";
          break;
        default:
          s = language.ToString();
          break;
      }

      return s;
    }

    /// <summary>
    /// Загружает тариф из указанного файла
    /// </summary>
    public static Tariff LoadTariff(string fileName)
    {
      if (!File.Exists(fileName))
        throw new FileNotFoundException();

      Tariff tariff = null;
      string s = File.ReadAllText(fileName);
      Exception x = null;

      //try to load tariff
      try
      {
        XElement xe = s.LoadXml();
        xe = xe.Elements("Tariff").First();
        tariff = DeserializeTariff(xe.Value);
      }
      catch (Exception e)
      {
        x = e;
      }

      //failed, try to load tariff metro
      if (tariff == null)
        try
        {
          tariff = TariffMetroHelper.LoadFromFile(fileName);
        }
        catch (Exception e)
        {
          x = e;
        }

      //check
      if (tariff == null)
        throw x;

      return tariff;
    }

    /// <summary>
    /// Сериализует тариф в строку
    /// </summary>
    public static string SerializeTariff(Tariff tariff)
    {
      return tariff.Serialize(TariffKnownTypes);
    }

    /// <summary>
    /// Десериализует тариф из строки
    /// </summary>
    public static Tariff DeserializeTariff(string s)
    {
      return s.Deserialize<Tariff>(TariffKnownTypes);
    }
  }
}