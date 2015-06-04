using System;

namespace Parking.FiscalDevice
{
  /// <summary>
  /// Определяет интерфейс работы с фискальным регистратором
  /// </summary>
  public interface IFiscalDevice : IDisposable
  {
    /// <summary>
    /// Возвращает состояние смены (открыта/закрыта)
    /// </summary>
    bool IsSessionOpened { get; }

    /// <summary>
    /// Производит подключение к фискальному регистратору
    /// </summary>
    void Initialize(int comPort, string name);

    /// <summary>
    /// Ведение лог-файла обмена с ККМ  
    /// </summary>
    void LogFileOn(bool onOff);

    /// <summary>
    /// Возвращает количество денег в кассе
    /// </summary>
    double GetKKMAmount();

    /// <summary>
    /// Возвращает выручку за смену 
    /// </summary>
    double GetSessionAmount();

    /// <summary>
    /// Открывает смену
    /// </summary>
    void OpenSession();

    /// <summary>
    /// Закрывает смену
    /// </summary>
    void CloseSession();

    /// <summary>
    /// Печатает X-отчет
    /// </summary>
    void PrintXReport();

    /// <summary>
    /// Положить деньги в кассу (например, для размена)
    /// </summary>
    void CashIn(double amount);

    /// <summary>
    /// Забрать деньги из кассы (инкассация)
    /// </summary>
    void CashOut(double amount);

    /// <summary>
    /// Оплата
    /// </summary>
    void Payment(PaymentDocument document);
  }
}