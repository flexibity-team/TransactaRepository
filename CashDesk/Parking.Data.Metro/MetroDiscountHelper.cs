using System;

namespace Parking.Data.Metro
{
  public static class MetroDiscountHelper
  {
    /// <summary>
    /// Возвращает строковое представление состояния скидки
    /// </summary>
    public static string GetString(this MetroDiscountState state)
    {
      string s = String.Empty;
      switch (state)
      {
        case MetroDiscountState.Entered:
          s = "Произведён въезд";
          break;
        case MetroDiscountState.Discounted:
          s = "Предоставлена";
          break;
        case MetroDiscountState.Paid:
          s = "Оплачена";
          break;
        default:
          s = state.ToString();
          break;
      }

      return s;
    }
  }
}