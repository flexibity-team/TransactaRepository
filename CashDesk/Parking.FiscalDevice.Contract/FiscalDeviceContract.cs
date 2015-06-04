using System;

namespace Parking.FiscalDevice
{
  public static class FiscalDeviceContract
  {
    /// <summary>
    /// Возвращает строковое представление причины оплаты
    /// </summary>
    public static String GetString(this PaymentReason reason)
    {
      string s = String.Empty;
      switch (reason)
      {
        case PaymentReason.Parking:
          s = "Оплата парковки";
          break;
        case PaymentReason.Fine:
          s = "Штраф";
          break;
        case PaymentReason.ECash:
          s = "Пополнение электронного кошелька";
          break;
        case PaymentReason.Any:
          s = "Произвольный платеж";
          break;
        case PaymentReason.Refund:
          s = "Возврат денег";
          break;
        default:
          s = reason.ToString();
          break;
      }

      return s;
    }
  }
}