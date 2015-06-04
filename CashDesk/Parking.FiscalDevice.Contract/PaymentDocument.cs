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
  public enum PaymentReason : int
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
    private double amount;
    private PaymentReason paymentReason;
    private DocumentType documentType;
    private PaymentType paymentType;
    private int cardID;
    private DateTime timeEntry;
    private DateTime timeExit;
    private double payment;
    private double debt;
    private double eCash;

    #region [ properties ]

    /// <summary>
    /// Сумма на электронном кошельке
    /// </summary>
    public double ECash
    {
      get { return eCash; }
      set { eCash = value; }
    }

    /// <summary>
    /// Сумма платежа
    /// </summary>
    public double Amount
    {
      get { return amount; }
      set { amount = value; }
    }

    /// <summary>
    /// Причина оплаты
    /// </summary>
    public PaymentReason PaymentReason
    {
      get { return paymentReason; }
      set { paymentReason = value; }
    }

    /// <summary>
    /// Тип документа
    /// </summary>
    public DocumentType Type
    {
      get { return documentType; }
      set { documentType = value; }
    }

    /// <summary>
    /// Тип оплаты
    /// </summary>
    public PaymentType PaymentType
    {
      get { return paymentType; }
      set { paymentType = value; }
    }

    /// <summary>
    /// Номер карты
    /// </summary>
    public int CardID
    {
      get { return cardID; }
      set { cardID = value; }
    }

    /// <summary>
    /// Время въезда
    /// </summary>
    public DateTime TimeEntry
    {
      get { return timeEntry; }
      set { timeEntry = value; }
    }

    /// <summary>
    /// Время до которого оплачивается
    /// </summary>
    public DateTime TimeExit
    {
      get { return timeExit; }
      set { timeExit = value; }
    }

    /// <summary>
    /// Сумма, которая уже оплачена
    /// </summary>
    public double Payment
    {
      get { return payment; }
      set { payment = value; }
    }

    /// <summary>
    /// Задолжность
    /// </summary>
    public double Debt
    {
      get { return debt; }
      set { debt = value; }
    }

    #endregion

    public PaymentDocument()
    {
      amount = 0;
      paymentReason = FiscalDevice.PaymentReason.Any;
      documentType = DocumentType.Buying;
      paymentType = PaymentType.Cash;
      cardID = -1;
      timeEntry = DateTime.Now;
      timeExit = DateTime.Now;
      payment = 0;
      debt = 0;
      eCash = 0;
    }

    public PaymentDocument(double amount, PaymentReason paymentReason, DocumentType documentType, PaymentType paymentType,
        int cardID, DateTime timeEntry, DateTime timeExit, double payment, double debt, double eCash)
    {
      this.amount = amount;
      this.paymentReason = paymentReason;
      this.documentType = documentType;
      this.paymentType = paymentType;
      this.cardID = cardID;
      this.timeEntry = timeEntry;
      this.timeExit = timeExit;
      this.payment = payment;
      this.debt = debt;
      this.eCash = eCash;
    }

    /// <summary>
    /// Возвращает строковое представление данных платежа
    /// </summary>
    public override string ToString()
    {
      return String.Format("\r\nAmount = {0:C}\r\n PayType = {1}\r\n Type = {2}\r\n CurrencyType = {3}\r\n" +
                                  "CardNumber = {4:X}\r\n TimeIn = {5}\r\n TimeOut = {5}\r\n Paid = {7:C}\r\n" +
                                  "ECash = {8:C}",
                                  amount, paymentReason.GetString(), documentType, paymentType, cardID,
                                  timeEntry, timeExit, payment, eCash);
    }
  }
}