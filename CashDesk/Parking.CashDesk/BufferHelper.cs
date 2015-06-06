using System;

namespace Parking.CashDesk
{
  internal static class BufferHelper
  {
    public static int SearchPos(Byte[] bBuffer, int len, Byte[] bSearching, int searchLen)
    {
      bool isFound = false;
      int i, j;
      for (i = 0; i < len - 1; i++)
      {
        if (bBuffer[i] == bSearching[0])
        {
          for (j = 1; j < searchLen - 2; j++)
          {
            if (bBuffer[i + j] != bSearching[j])
            {
              isFound = false;
              break;
            }
            isFound = true;
          }
          if (isFound == true) break;
        }
      }
      if (isFound == true) return i;

      return -1;

    }

    public static void Int32ToBuffer(Byte[] bBuffer, int nStart, int nNumber)
    {
      bBuffer[nStart] = (byte)(nNumber & 0xFF);
      bBuffer[nStart + 1] = (byte)((nNumber >> 8) & 0xFF);
      bBuffer[nStart + 2] = (byte)((nNumber >> 16) & 0xFF);
      bBuffer[nStart + 3] = (byte)((nNumber >> 24) & 0xFF);
    }

    public static int BufferToInt32(Byte[] bBuffer, int nStart)
    {
      int result = 0;
      result += bBuffer[nStart];
      result += bBuffer[nStart + 1] << 8;
      result += bBuffer[nStart + 2] << 16;
      result += bBuffer[nStart + 3] << 24;

      return result;
    }

    public static void Int16ToBuffer(Byte[] bBuffer, int nStart, int nNumber)
    {
      bBuffer[nStart] = (byte)(nNumber & 0xFF);
      bBuffer[nStart + 1] = (byte)((nNumber >> 8) & 0xFF);
    }

    public static short BufferToInt16(Byte[] bBuffer, int nStart)
    {
      short result = 0;
      result += (short)bBuffer[nStart];
      result += (short)(bBuffer[nStart + 1] << 8);

      return result;
    }

    // Копировать Len элементов массива из bSource начиная с nStart в bDestin 
    public static void ByteToByteCopy(Byte[] bSource, int nSourceStart, Byte[] bDestin, int nDestinStart, int nLen)
    {
      int i;

      for (i = 0; i < nLen; i++)
      {
        bDestin[i + nDestinStart] = bSource[i + nSourceStart];
      }
    }

    public static String ByteToString(Byte[] bSource, int len)
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
    // Преобразование даты в строковое представление.
    ////////////////////////////////////////////////////
    public static String DateToString(DateTime dt)
    {
        return DateTime.Compare(dt, new DateTime(2000, 1, 1, 0, 0, 0)) <= 0 ? "Неопределенная дата" : String.Format("{0:G}", dt);
    }

      ////////////////////////////////////////////////////
    // Преобразование числа в формат BCD
    ////////////////////////////////////////////////////
    public static Byte IntToBcd(int iIn)
    {
        iIn = iIn - (iIn / 100) * 100;
      var bTH = Convert.ToByte((iIn / 10));
      var bTL = Convert.ToByte(iIn - bTH * 10);
      var bT = Convert.ToByte(bTL + (bTH << 4));
      return bT;
    }

    ////////////////////////////////////////////////////
    //преобразование формата BCD в число
    ////////////////////////////////////////////////////
    public static int BcdToInt(Byte bIn)
    {
        var iT = (bIn / 16) * 10 + bIn - (bIn / 16) * 16;
        return iT;
    }

      ////////////////////////////////////////////////////
    //Преобразует дату и время из формата DateTime в массив байт для представления на карте
    ////////////////////////////////////////////////////
    public static void DateToBytes(int iStart, Byte[] bDat, DateTime dtDate)
    {
      // Public Sub DateToBytes(ByVal dtValue As DateTime, ByVal iStart As Int16, ByRef bDat() As Byte)
        //TODO: разобраться как передавать день недели.
      //dw = dtDate.DayOfWeek
      var dw = 0;
      if (dw == 0) dw = 7;

      if (dtDate.ToOADate() > 0 /*DateTime(1 / 1 / 2000)*/)
      {
        bDat[iStart] = IntToBcd(dtDate.Second);
        bDat[iStart + 1] = IntToBcd(dtDate.Minute);
        bDat[iStart + 2] = IntToBcd(dtDate.Hour);
        bDat[iStart + 3] = IntToBcd(dw);
        bDat[iStart + 4] = IntToBcd(dtDate.Day);
        bDat[iStart + 5] = IntToBcd(dtDate.Month);
        bDat[iStart + 6] = IntToBcd(dtDate.Year);
      }
      else
      {
        Array.Clear(bDat, iStart, 8);
      }

    }

    ////////////////////////////////////////////////////
    // Преобразует дату и время с карты (массив байт) в DateTime
    ////////////////////////////////////////////////////
    public static DateTime BytesToDate(int iStart, Byte[] bDat)
    {
        DateTime dtT;
      try
      {
        var da = BcdToInt(bDat[iStart + 4]);//, dw2;
        var mo = BcdToInt(bDat[iStart + 5]);//, dw2;
        var ye = BcdToInt(bDat[iStart + 6]);//, dw2;
        ye += 2000;

        BcdToInt(bDat[iStart + 3]);
        var ho = BcdToInt(bDat[iStart + 2]);//, dw2;
        var mi = BcdToInt(bDat[iStart + 1]);//, dw2;
        var se = BcdToInt(bDat[iStart]);//, dw2;


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
  }
}