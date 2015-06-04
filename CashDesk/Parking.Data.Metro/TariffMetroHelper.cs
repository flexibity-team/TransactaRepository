using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Parking.Data.Metro
{
  public static class TariffMetroHelper
  {
    #region [ const ]

    private const char CommandStartSign = '=';
    private const char CommandEndSign = ';';
    private const char CommandPropertySeparator = ',';
    private const char CommandPropertySplitter = '/';
    
    private const string CommandTypeCalculateInterval = "SdT";
    private const string CommandTypeGoto = "GO";
    private const string CommandTypeEnd = "END";

    private const string PropertyNames = "ABCDEF";
    private const long PropertyDefaultValue = DataContract.DefaultID;

    #endregion

    /// <summary>
    /// Возвращает строковое представление типа команды тарифа метро
    /// </summary>
    public static string GetString(this TariffMetroCommandType type)
    {
      string s = String.Empty;
      switch (type)
      {
        case TariffMetroCommandType.CalculateInterval:
          s = "Расчёт суммы за интервал";
          break;
        case TariffMetroCommandType.Goto:
          s = "Переход";
          break;
        case TariffMetroCommandType.End:
          s = "Завершение";
          break;
        default:
          s = type.ToString();
          break;
      }

      return s;
    }

    /// <summary>
    /// Создаёт тариф метро из указанного файла
    /// </summary>
    public static Tariff LoadFromFile(string fileName)
    {
      //check file exists
      if (!File.Exists(fileName))
        throw new FileNotFoundException("Файл не найден", fileName);

      //read valid entries
      IEnumerable<string> entries = File.ReadAllLines(fileName, Encoding.Default)
        .Where(s => ((s.Length > 3) && (s[0] == CommandStartSign)));

      //create commands and properties
      Dictionary<int, TariffMetroCommand> commands = new Dictionary<int, TariffMetroCommand>();
      Dictionary<char, int> properties = new Dictionary<char, int>();
      foreach (char p in PropertyNames)
        properties.Add(p, (int)PropertyDefaultValue);

      //parse entries
      foreach (string e in entries)
      {
        //split entry
        string[] sa = SplitEntry(e);

        //get entry name
        char name = sa[0].ToUpper()[1];

        //entry is command?
        if (Char.IsDigit(name))
        {
          int n = (int)Char.GetNumericValue(name);
          TariffMetroCommand cmd = ParseCommand(sa);

          //stop parsing at end command
          if (cmd.Type == TariffMetroCommandType.End)
            break;

          cmd.ID = n;
          commands.Add(n, cmd);
          continue;
        }

        //entry is property?
        if (properties.ContainsKey(name))
        {
          properties[name] = ParseProperty(name, sa);
          continue;
        }
      }

      //check command sequence
      int[] ids = commands.Keys.OrderBy(k => k).ToArray();
      if (commands.Count > 1)
      {
        int firstID = ids[0];
        for (int i = 1; i < ids.Length; i++)
          if ((ids[i] - ids[i - 1]) > 1)
            throw new InvalidDataException("Неверная последовательность команд");
      }

      //check goto command destinations
      if (commands.Values
          .Where(c => c.Type == TariffMetroCommandType.Goto)
          .Cast<TariffMetroCommandGoto>()
          .Any(g => !ids.Contains(g.Destination)))
        throw new InvalidDataException("Не найдено назначение команды перехода");

      //check cyclic goto (1 level)
      if (commands.Values
          .Where(c => c.Type == TariffMetroCommandType.Goto)
          .Cast<TariffMetroCommandGoto>()
          .Any(g => 
          {
            TariffMetroCommand g2 = commands[g.Destination];
            if (g2.Type != TariffMetroCommandType.Goto)
              return false;

            return (((TariffMetroCommandGoto)g2).Destination == g.ID);
          }))
        throw new InvalidDataException("Обнаружен циклический переход");

      //check properties
      if (properties.Values.Any(v => v == PropertyDefaultValue))
        throw new InvalidDataException("Не найдены все свойства");

      //create tariff
      TariffMetro tariff = new TariffMetro();
      int id = properties['C'];
      id += (properties['B'] << 8);
      id += (properties['A'] << 16);
      tariff.ID = id;

      tariff.PaymentDiscrete = ConvertMoney(properties['D']);
      tariff.FreeTimeAtStart = TimeSpan.FromSeconds(properties['E']);
      tariff.FreeTimeAfterPayment = TimeSpan.FromSeconds(properties['F']);

      for (int i = 0; i < ids.Length; i++)
        tariff.AddCommand(commands[ids[i]]);

      return tariff;
    }

    private static int ParseInt32(string s)
    {
      StringBuilder sb = new StringBuilder(s.Length);
      for (int i = 0; i < s.Length; i++)
        if (Char.IsDigit(s[i]))
          sb.Append(s[i]);
        else if (sb.Length > 0)
          break;

      if (sb.Length == 0)
        throw new FormatException(String.Format("Число не найдено\r\n{0}", s));

      return Int32.Parse(sb.ToString());
    }

    private static TimeSpan ParseTimeSpan(string s)
    {
      TimeSpan ts = TimeSpan.Zero;
      for (int i = 0; i < s.Length; i++)
        if (Char.IsDigit(s[i]))
          if (TimeSpan.TryParse(s.Substring(i, 8), out ts))
            return ts;

      throw new FormatException(String.Format("Время не найдено\r\n{0}", s));
    }

    private static TariffMetroCommand ParseCommand(string[] sa)
    {
      string type = sa[1];
      if (String.Compare(type, CommandTypeCalculateInterval, true) == 0)
      {
        TariffMetroCommandCalculateInterval cmdCalulateInterval = new TariffMetroCommandCalculateInterval();
        cmdCalulateInterval.StartTime = ParseTimeSpan(sa[2]);
        cmdCalulateInterval.IntervalAmount = ConvertMoney(ParseInt32(sa[3]));
        cmdCalulateInterval.RountToNext = (ParseInt32(sa[5]) != 0);
        
        string[] discretes = sa[4].Split(CommandPropertySplitter);
        if (discretes.Length != 2)
          throw new FormatException(String.Format("Неверные данные дискрета", sa[4]));

        cmdCalulateInterval.PerDiscreteAmount = ConvertMoney(ParseInt32(discretes[0]));
        cmdCalulateInterval.Discrete = ParseTimeSpan(discretes[1]);

        return cmdCalulateInterval;
      }
      else if (String.Compare(type, CommandTypeGoto, true) == 0)
      {
        TariffMetroCommandGoto cmdGoto = new TariffMetroCommandGoto();
        cmdGoto.Destination = ParseInt32(sa[2]);

        return cmdGoto;
      }
      else if (String.Compare(type, CommandTypeEnd, true) == 0)
        return new TariffMetroCommandEnd();

      throw new FormatException(String.Format("Неизвестная команда\r\n{0}", type));
    }

    private static int ParseProperty(char name, string[] sa)
    {
      string value = sa[1];
      int j = 0;
      switch (name)
      {
        case 'A':
        case 'B':
        case 'C':
        case 'D':
          j = ParseInt32(value);
          break;
        case 'E':
        case 'F':
          j = (int)ParseTimeSpan(value).TotalSeconds;
          break;
      }

      return j;
    }

    private static decimal ConvertMoney(int m)
    {
      return m / 100;
    }

    private static string[] SplitEntry(string e)
    {
      string[] sa = e.Split(CommandPropertySeparator);
      for (int i = 0; i < sa.Length; i++)
      {
        //remove spaces
        string s = sa[i].Replace(" ", String.Empty);

        //trim last token
        if (i == (sa.Length - 1))
        {
          int j = s.IndexOf(CommandEndSign);
          if (j > 0)
            s = s.Substring(0, j);
        }

        //ok
        sa[i] = s;
      }

      return sa;
    }
  }
}