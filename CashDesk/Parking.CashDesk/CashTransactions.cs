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
    private IWriter _writer;

    public CashTransactions(string fileName, IWriter writer)
    {
      _writer = writer;

      // Шапка
      if (!File.Exists(fileName) || (new FileInfo(fileName).Length == 0))
      {
        List<String> ParamName = new List<String>();
        ParamName.Add("Время операции");
        ParamName.Add("Наименование");
        ParamName.Add("Сумма");
        ParamName.Add("Время въезда");
        ParamName.Add("Зона");
        ParamName.Add("Номер карты");
        _writer.Write(ParamName);
      }
    }

    private bool WriteToFile(List<String> Param)
    {
      bool bState = _writer.Write(Param);
      return bState;
    }

    public bool Write(DateTime date, String Operation)
    {
      List<String> Param = new List<String>();
      Param.Add(date.ToString());
      Param.Add(Operation);

      return WriteToFile(Param);
    }

    public bool Write(DateTime date, String Operation, double Summa)
    {
      List<String> Param = new List<String>();
      Param.Add(date.ToString());
      Param.Add(Operation);
      Param.Add(Summa.ToString());

      return WriteToFile(Param);
    }

    public bool Write(DateTime date, String Operation, double Summa, DateTime dateIn, byte Zone, String CardNum)
    {
      List<String> Param = new List<String>();
      Param.Add(date.ToString());
      Param.Add(Operation);
      Param.Add(Summa.ToString());
      Param.Add(dateIn.ToString());
      Param.Add(Zone.ToString());
      Param.Add(CardNum);

      return WriteToFile(Param);
    }
  }
}