using System.IO;
using System.Xml.Linq;
using RMLib;
using RMLib.Xml;

namespace Parking.FiscalDevice
{
  /// <summary>
  /// Виртуальный фискальный регистратор.
  /// Используется для работы без физического фискального регистратора.
  /// Оперирует теми же данными что и Прим-08ТК
  /// </summary>
  public class VirtualFiscalDevice : Disposable, IFiscalDevice
  {
    /// <summary>
    /// Имитирует память фискального регистратора, сохраняя данные в файле конфигурации приложения
    /// </summary>
    private class VFDMemory : AssemblyConfiguration
    {
      #region [ const ]

      private const string SettingsFileName = "VirtualFiscalDevice.xml";

      #endregion

      private XElement root;
      private string xmlFileData;

      #region [ properties ]

      /// <summary>
      /// Денег в кассе
      /// </summary>
      public double MoneyInKKM
      {
        get
        {
          LoadXML();
          XElement root = xmlFileData.LoadXml();
          XElement xe = root.Element("MoneyInKKM");

          double val;
          if (!double.TryParse(xe.Value.ToString(), out val))
            throw new FiscalDeviceException("Ошибка в фискальной памяти! Сумма в кассе не определена.");

          return val;
        }
        set
        {
          LoadXML();
          XElement root = xmlFileData.LoadXml();
          root.SetElementValue("MoneyInKKM", value);
          File.WriteAllText(SettingsFileName, root.ToString());
        }
      }

      /// <summary>
      /// Выручка за день
      /// </summary>
      public double Receipts
      {
        get
        {
          LoadXML();
          XElement root = xmlFileData.LoadXml();
          XElement xe = root.Element("Receipts");

          double val;
          if (!double.TryParse(xe.Value, out val))
            throw new FiscalDeviceException("Ошибка в фискальной памяти! Выручка за день не определена.");

          return val;
        }
        set
        {
          LoadXML();
          XElement root = xmlFileData.LoadXml();
          root.SetElementValue("Receipts", value);
          File.WriteAllText(SettingsFileName, root.ToString());
        }
      }

      /// <summary>
      /// Состояние сессии (True - открыта/ False - закрыта)
      /// </summary>
      public bool IsSessionOpen
      {
        get
        {
          LoadXML();
          XElement root = xmlFileData.LoadXml();
          XElement xe = root.Element("IsSessionOpen");

          bool val = true;
          if (!bool.TryParse(xe.Value, out val))
            throw new FiscalDeviceException("Ошибка в фискальной памяти! Невозможно определить состояние смены.");

          return val;
        }
        set
        {
          LoadXML();
          XElement root = xmlFileData.LoadXml();
          root.SetElementValue("IsSessionOpen", value);
          File.WriteAllText(SettingsFileName, root.ToString());
        }
      }

      #endregion

      public VFDMemory()
      {
        root = new XElement("FiscalDevice");
      }

        private void LoadXML()
      {
        try
        {
          xmlFileData = File.ReadAllText(SettingsFileName);
        }
        catch
        {
          throw new FiscalDeviceException("Ошибка! Не найден файл конфигурации фискального устройства");
        }
      }
    }

    private readonly VFDMemory memory;

    public VirtualFiscalDevice()
    {
      memory = new VFDMemory();
    }

    #region [ Disposable ]

    protected override void DisposeManagedResources()
    {
      memory.Save();
    }

    #endregion

    #region [ IFiscalDevice ]

    /// <summary>
    /// Возвращает состояние смены (открыта/закрыта)
    /// </summary>
    public bool IsSessionOpened
    {
      get { return memory.IsSessionOpen; }
    }

    /// <summary>
    /// Производит подключение к фискальному регистратору
    /// </summary>
    public void Initialize(int comPort, string name)
    {
      memory.Load();
    }

      public void LogFileOn(bool onOff)
      {
//          throw new System.NotImplementedException();
      }

      /// <summary>
    /// Возвращает количество денег в кассе
    /// </summary>
    public double GetKKMAmount()
    {
      return memory.MoneyInKKM;  // денег в кассе
    }

    /// <summary>
    /// Возвращает выручку за смену 
    /// </summary>
    public double GetSessionAmount()
    {
      return memory.Receipts;
    }

    /// <summary>
    /// Открывает смену
    /// </summary>
    public void OpenSession()
    {
      memory.IsSessionOpen = true;
      memory.Save();
    }

    /// <summary>
    /// Закрывает смену
    /// </summary>
    public void CloseSession()
    {
      memory.MoneyInKKM = 0; // Инкассация
      memory.Receipts = 0; // Обнуляем выручку за день
      memory.IsSessionOpen = false;
      memory.Save();
    }

    /// <summary>
    /// Печатает X-отчет
    /// </summary>
    public void PrintXReport()
    {
      return;
    }

    /// <summary>
    /// Положить деньги в кассу (например, для размена)
    /// </summary>
    public void CashIn(double amount)
    {
      // Очень важно, что мы прибавляем к сумме денег в кассе, а не заменяем их
      memory.MoneyInKKM += amount;
      memory.Save();
    }

    /// <summary>
    /// Забрать деньги из кассы (инкассация)
    /// </summary>
    public void CashOut(double amount)
    {
      // Очень важно, что мы вычитаем из денег в кассе, а не заменяем их
      if (amount > memory.MoneyInKKM) throw new FiscalDeviceException("Недостаточно денег для инкассации");
      else
      {
        memory.MoneyInKKM -= amount;
        memory.Save();
      }
    }

    /// <summary>
    /// Оплата
    /// </summary>
    public void Payment(PaymentDocument document)
    {
      switch (document.PaymentReason)
      {
        case PaymentReason.Parking:
        case PaymentReason.Fine:
        case PaymentReason.ECash:
        case PaymentReason.Any:
          memory.MoneyInKKM += document.Amount;
          memory.Save();
          break;
        case PaymentReason.Refund:
          if (document.Amount > memory.MoneyInKKM)
            throw new FiscalDeviceException("Недостаточно денег для возврата");

          memory.MoneyInKKM -= document.Amount;
          memory.Save();
          break;
      }
    }

    #endregion

    public override string ToString()
    {
      return "Виртуальный";
    }
  }
}