using System;

namespace Parking.FiscalDevice
{
  /// <summary>
  /// Тип документа
  /// </summary>
  public enum DocumentType
  {
    /// <summary>
    /// Продажа
    /// </summary>
    Selling = 0,

    /// <summary>
    /// Возврат продажи
    /// </summary>
    SaleReturn = 1,

    /// <summary>
    /// Возврат
    /// </summary>
    Repayment = 2,

    /// <summary>
    /// Покупка
    /// </summary>
    Buying = 4,

    /// <summary>
    /// Возврат покупки
    /// </summary>
    BuyReturn = 5
  }

  /// <summary>
  /// Тип оплаты
  /// </summary>
  public enum PaymentType
  {
    /// <summary>
    /// Наличные
    /// </summary>
    Cash = 0,

    /// <summary>
    /// Карта
    /// </summary>
    Card = 2
  }

  /// <summary>
  /// Причина оплаты
  /// </summary>
  public enum PaymentReason
  {
    /// <summary>
    /// Оплата парковочной карты
    /// </summary>
    Parking = 0,

    /// <summary>
    /// Оплата штрафной карты
    /// </summary>
    Fine = 1,

    /// <summary>
    /// Пополнение электронного кошелька
    /// </summary>
    ECash = 2,

    /// <summary>
    /// Платеж на произвольную сумму
    /// </summary>
    Any = 3,

    /// <summary>
    /// Возврат денег
    /// </summary>
    Refund = 4
  }

  /// <summary>
  /// Данные платежа
  /// </summary>
  public class PaymentDocument
  {
      #region [ properties ]

    /// <summary>
    /// Сумма на электронном кошельке
    /// </summary>
    public double ECash { get; set; }

      /// <summary>
    /// Сумма платежа
    /// </summary>
    public double Amount { get; set; }

      /// <summary>
    /// Причина оплаты
    /// </summary>
    public PaymentReason PaymentReason { get; set; }

      /// <summary>
    /// Тип документа
    /// </summary>
    public DocumentType Type { get; set; }

      /// <summary>
    /// Тип оплаты
    /// </summary>
    public PaymentType PaymentType { get; set; }

      /// <summary>
    /// Номер карты
    /// </summary>
    public int CardId { get; set; }

      /// <summary>
    /// Время въезда
    /// </summary>
    public DateTime TimeEntry { get; set; }

      /// <summary>
    /// Время до которого оплачивается
    /// </summary>
    public DateTime TimeExit { get; set; }

      /// <summary>
    /// Сумма, которая уже оплачена
    /// </summary>
    public double Payment { get; set; }

      /// <summary>
    /// Задолжность
    /// </summary>
    public double Debt { get; set; }

      #endregion

    public PaymentDocument()
    {
      Amount = 0;
      PaymentReason = PaymentReason.Any;
      Type = DocumentType.Buying;
      PaymentType = PaymentType.Cash;
      CardId = -1;
      TimeEntry = DateTime.Now;
      TimeExit = DateTime.Now;
      Payment = 0;
      Debt = 0;
      ECash = 0;
    }

    public PaymentDocument(double amount, PaymentReason paymentReason, DocumentType documentType, PaymentType paymentType,
        int cardId, DateTime timeEntry, DateTime timeExit, double payment, double debt, double eCash)
    {
      Amount = amount;
      PaymentReason = paymentReason;
      Type = documentType;
      PaymentType = paymentType;
      CardId = cardId;
      TimeEntry = timeEntry;
      TimeExit = timeExit;
      Payment = payment;
      Debt = debt;
      ECash = eCash;
    }

    /// <summary>
    /// Возвращает строковое представление данных платежа
    /// </summary>
    public override string ToString()
    {
      return String.Format("\r\nAmount = {0:C}\r\n PayType = {1}\r\n Type = {2}\r\n CurrencyType = {3}\r\n" +
                                  "CardNumber = {4:X}\r\n TimeIn = {5}\r\n TimeOut = {5}\r\n Paid = {7:C}\r\n" +
                                  "ECash = {8:C}",
                                  Amount, PaymentReason.GetString(), Type, PaymentType, CardId,
                                  TimeEntry, TimeExit, Payment, ECash);
    }
  }
}