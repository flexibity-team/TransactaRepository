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

      private XElement _root;
      private string _xmlFileData;

      #region [ properties ]

      /// <summary>
      /// Денег в кассе
      /// </summary>
      public double MoneyInKKM
      {
        get
        {
          LoadXML();
          XElement root = _xmlFileData.LoadXml();
          XElement xe = root.Element("MoneyInKKM");

          double val = 0;
          if (!double.TryParse(xe.Value.ToString(), out val))
            throw new FiscalDeviceException("Ошибка в фискальной памяти! Сумма в кассе не определена.");

          return val;
        }
        set
        {
          LoadXML();
          XElement root = _xmlFileData.LoadXml();
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
          XElement root = _xmlFileData.LoadXml();
          XElement xe = root.Element("Receipts");

          double val = 0;
          if (!double.TryParse(xe.Value.ToString(), out val))
            throw new FiscalDeviceException("Ошибка в фискальной памяти! Выручка за день не определена.");

          return val;
        }
        set
        {
          LoadXML();
          XElement root = _xmlFileData.LoadXml();
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
          XElement root = _xmlFileData.LoadXml();
          XElement xe = root.Element("IsSessionOpen");

          bool val = true;
          if (!bool.TryParse(xe.Value.ToString(), out val))
            throw new FiscalDeviceException("Ошибка в фискальной памяти! Невозможно определить состояние смены.");

          return val;
        }
        set
        {
          LoadXML();
          XElement root = _xmlFileData.LoadXml();
          root.SetElementValue("IsSessionOpen", value);
          File.WriteAllText(SettingsFileName, root.ToString());
        }
      }

      #endregion

      public VFDMemory()
      {
        _root = new XElement("FiscalDevice");
      }

        private void LoadXML()
      {
        try
        {
          _xmlFileData = File.ReadAllText(SettingsFileName);
        }
        catch
        {
          throw new FiscalDeviceException("Ошибка! Не найден файл конфигурации фискального устройства");
        }
      }
    }

    private VFDMemory _memory;

    public VirtualFiscalDevice()
    {
      _memory = new VFDMemory();
    }

    #region [ Disposable ]

    protected override void DisposeManagedResources()
    {
      _memory.Save();
    }

    #endregion

    #region [ IFiscalDevice ]

    /// <summary>
    /// Возвращает состояние смены (открыта/закрыта)
    /// </summary>
    public bool IsSessionOpened
    {
      get { return _memory.IsSessionOpen; }
    }

    /// <summary>
    /// Производит подключение к фискальному регистратору
    /// </summary>
    public void Initialize(int comPort, string name)
    {
      _memory.Load();
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
      return _memory.MoneyInKKM;  // денег в кассе
    }

    /// <summary>
    /// Возвращает выручку за смену 
    /// </summary>
    public double GetSessionAmount()
    {
      return _memory.Receipts;
    }

    /// <summary>
    /// Открывает смену
    /// </summary>
    public void OpenSession()
    {
      _memory.IsSessionOpen = true;
      _memory.Save();
    }

    /// <summary>
    /// Закрывает смену
    /// </summary>
    public void CloseSession()
    {
      _memory.MoneyInKKM = 0; // Инкассация
      _memory.Receipts = 0; // Обнуляем выручку за день
      _memory.IsSessionOpen = false;
      _memory.Save();
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
      _memory.MoneyInKKM += amount;
      _memory.Save();
    }

    /// <summary>
    /// Забрать деньги из кассы (инкассация)
    /// </summary>
    public void CashOut(double amount)
    {
      // Очень важно, что мы вычитаем из денег в кассе, а не заменяем их
      if (amount > _memory.MoneyInKKM) throw new FiscalDeviceException("Недостаточно денег для инкассации");
      else
      {
        _memory.MoneyInKKM -= amount;
        _memory.Save();
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
          _memory.MoneyInKKM += document.Amount;
          _memory.Save();
          break;
        case PaymentReason.Refund:
          if (document.Amount > _memory.MoneyInKKM)
            throw new FiscalDeviceException("Недостаточно денег для возврата");

          _memory.MoneyInKKM -= document.Amount;
          _memory.Save();
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