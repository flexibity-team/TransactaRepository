using System;

namespace Parking.CashDesk
{
  internal class ConvertLib
  {
    //// Копировать Len элементов массива из bSource начиная с nStart в bDestin 
    //public static void ByteToByteCopy(ref Byte[] bSource, int nSourceStart, ref Byte[] bDestin, int nDestinStart, int nLen)
    //{
    //    int i;

    //    for (i = 0; i < nLen; i++)
    //    {
    //        bDestin[i + nDestinStart] = bSource[i + nSourceStart];
    //    }
    //}

    public static String ByteToString(ref Byte[] bSource, int len)
    {
      int i;
      String sDest = "";

      for (i = 0; i < len; i++)
      {
        sDest += Convert.ToString(bSource[i], 16).ToUpper();
      }
      return sDest;
    }

    ////////////////////////////////////////////////////
    // Преобразование числа в формат BCD
    ////////////////////////////////////////////////////
    public static Byte IntToBCD(int iIn)
    {
      Byte bTH, bTL, bT;

      iIn = iIn - (iIn / 100) * 100;
      bTH = Convert.ToByte((iIn / 10));
      bTL = Convert.ToByte(iIn - bTH * 10);
      bT = Convert.ToByte(bTL + (bTH << 4));
      return bT;
    }

    ////////////////////////////////////////////////////
    //преобразование формата BCD в число
    ////////////////////////////////////////////////////
    public static int BCDToInt(Byte bIn)
    {
      int iT;
      iT = (bIn / 16) * 10 + bIn - (bIn / 16) * 16;
      return iT;
    }

    ////////////////////////////////////////////////////
    //Преобразует дату и время из формата DateTime в массив байт для представления на карте
    ////////////////////////////////////////////////////
    public static void DateToBytes(int iStart, ref Byte[] bDat, DateTime dtDate)
    {
      // Public Sub DateToBytes(ByVal dtValue As DateTime, ByVal iStart As Int16, ByRef bDat() As Byte)
      int dw;
      //TODO: разобраться как передавать день недели.
      //dw = dtDate.DayOfWeek
      dw = 0;
      if (dw == 0) dw = 7;

      if (dtDate.ToOADate() > 0 /*DateTime(1 / 1 / 2000)*/)
      {
        bDat[iStart] = IntToBCD(dtDate.Second);
        bDat[iStart + 1] = IntToBCD(dtDate.Minute);
        bDat[iStart + 2] = IntToBCD(dtDate.Hour);
        bDat[iStart + 3] = IntToBCD(dw);
        bDat[iStart + 4] = IntToBCD(dtDate.Day);
        bDat[iStart + 5] = IntToBCD(dtDate.Month);
        bDat[iStart + 6] = IntToBCD(dtDate.Year);
      }
      else
      {
        Array.Clear(bDat, iStart, 8);
      }

    }

    ////////////////////////////////////////////////////
    // Преобразует дату и время с карты (массив байт) в DateTime
    ////////////////////////////////////////////////////
    public static DateTime BytesToDate(int iStart, ref Byte[] bDat)
    {
      Char[] cT = new Char[5];
      int ho, mi, se, da, mo, ye, dw;//, dw2;
      DateTime dtT;
      try
      {
        da = BCDToInt(bDat[iStart + 4]);
        mo = BCDToInt(bDat[iStart + 5]);
        ye = BCDToInt(bDat[iStart + 6]);
        ye += 2000;

        dw = BCDToInt(bDat[iStart + 3]);
        ho = BCDToInt(bDat[iStart + 2]);
        mi = BCDToInt(bDat[iStart + 1]);
        se = BCDToInt(bDat[iStart]);


        if (ye + mo + da + ho + mi + se == 2000)
        {
          dtT = new DateTime(2000, 1, 1, 0, 0, 0);
          return dtT;
        }
        else dtT = new DateTime(ye, mo, da, ho, mi, se);

        //dw2 = dtT.DayOfWeek;
      }
      catch
      {
        dtT = new DateTime(2000, 1, 1, 0, 0, 0);
      }
      return dtT;
    }

    ///// <summary>
    ///// преобразование эелктронного кошелька в массив байт
    ///// </summary>
    ///// <param name="Summ">Сумма для записи на кошелек</param>
    //public static void SumToE_Payment(Int32 Summ, byte[] bData)
    //{
    //    // Формат Электронного кошелька
    //    // Сумма 4байта, Инвертированная сумма 4байта, Сумма 4байта, код 0xFF00FF00
    //    Int64 uiAd = 0xFF00FF00;
    //    byte[] bSumm = BitConverter.GetBytes(Summ);
    //    Buffer.BlockCopy(bSumm, 0, bData, 0, 4);
    //    byte[] bInvertSumm = BitConverter.GetBytes((Int32)~Summ);
    //    Buffer.BlockCopy(bInvertSumm, 0, bData, 4, 4);
    //    Buffer.BlockCopy(bSumm, 0, bData, 8, 4);
    //    byte[] buiAd = BitConverter.GetBytes((Int32)uiAd);
    //    Buffer.BlockCopy(buiAd, 0, bData, 12, 4);
    //}
  }

  internal static class DateTimeHelper
  {
    public static String DateToString(this DateTime dt)
    {
      return dt.ToString("dd MMMM yyyy  H:mm:ss");
    }
  }
}