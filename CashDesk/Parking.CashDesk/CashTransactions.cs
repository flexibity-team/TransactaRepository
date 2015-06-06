using System;
using System.Collections.Generic;
using System.IO;
using CSVWritter;

namespace Parking.CashDesk
{
  /// <summary>
  /// Транзакции в ручной кассе (Дата; вид оплаты; сумма оплаты)
  /// </summary>
  public class CashTransactions
  {
    private readonly IWriter writer;

    public CashTransactions(string fileName, IWriter writer)
    {
      this.writer = writer;

      // Шапка
        if (File.Exists(fileName) && (new FileInfo(fileName).Length != 0))
            return;

        List<String> paramName = new List<String>();
        paramName.Add("Время операции");
        paramName.Add("Наименование");
        paramName.Add("Сумма");
        paramName.Add("Время въезда");
        paramName.Add("Зона");
        paramName.Add("Номер карты");
        this.writer.Write(paramName);
    }

    private bool WriteToFile(List<String> param)
    {
      bool bState = writer.Write(param);
      return bState;
    }

    public bool Write(DateTime date, String operation)
    {
      List<String> Param = new List<String>();
      Param.Add(date.ToString());
      Param.Add(operation);

      return WriteToFile(Param);
    }

    public bool Write(DateTime date, String operation, double summa)
    {
      List<String> param = new List<String>();
      param.Add(date.ToString());
      param.Add(operation);
      param.Add(summa.ToString());

      return WriteToFile(param);
    }

    public bool Write(DateTime date, String operation, double summa, DateTime dateIn, byte zone, String cardNum)
    {
      List<String> param = new List<String>();
      param.Add(date.ToString());
      param.Add(operation);
      param.Add(summa.ToString());
      param.Add(dateIn.ToString());
      param.Add(zone.ToString());
      param.Add(cardNum);

      return WriteToFile(param);
    }
  }
}