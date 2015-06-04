using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using RMLib.Xml;

namespace Parking.Data.Metro
{
  /// <summary>
  /// Тип команды тарифа метро
  /// </summary>
  [DataContract(Namespace = DataContract.Namespace)]
  public enum TariffMetroCommandType : int
  {
    /// <summary>
    /// Подсчёт суммы за интервал
    /// </summary>
    [EnumMember]
    CalculateInterval = 1,

    /// <summary>
    /// Переход
    /// </summary>
    [EnumMember]
    Goto = 256,

    /// <summary>
    /// Завершение
    /// </summary>
    [EnumMember]
    End = 512
  }

  /// <summary>
  /// Абстрактный базовый класс команды тарифа метро
  /// </summary>
  [DataContract(Namespace = DataContract.Namespace)]
  [KnownType(typeof(TariffMetroCommandCalculateInterval))]
  [KnownType(typeof(TariffMetroCommandGoto))]
  [KnownType(typeof(TariffMetroCommandEnd))]
  public abstract class TariffMetroCommand : DataItem
  {
    #region [ const ]

    private const string TypeString = "Type";

    #endregion

    [DataMember(Name = TypeString)]
    private TariffMetroCommandType _type; //тип команды

    #region [ properties ]

    /// <summary>
    /// Возвращает тип команды
    /// </summary>
    public TariffMetroCommandType Type
    {
      get { return _type; }
    }

    #endregion

    protected TariffMetroCommand(TariffMetroCommandType type)
    {
      _type = type;
    }

    protected TariffMetroCommand(TariffMetroCommand command)
      : base(command.ID)
	  {
      _type = command.Type;
	  }

    #region [ ICloneable ]

    /// <summary>
    /// Возвращает копию команды
    /// </summary>
    public override object Clone()
    {
      TariffMetroCommand command = null;
      switch (_type)
      {
        case TariffMetroCommandType.CalculateInterval:
          command = new TariffMetroCommandCalculateInterval((TariffMetroCommandCalculateInterval)this);
          break;
        case TariffMetroCommandType.Goto:
          command = new TariffMetroCommandGoto((TariffMetroCommandGoto)this);
          break;
      }

      return command;
    }

    #endregion

    public override string ToString()
    {
      return _type.GetString();
    }
  }

  /// <summary>
  /// Подсчёт суммы за интервал [команда тарифа метро]
  /// </summary>
  [DataContract(Namespace = DataContract.Namespace)]
  public class TariffMetroCommandCalculateInterval : TariffMetroCommand
  {
    #region [ const ]

    private const string StartTimeString = "StartTime";
    private const string IntervalAmountString = "IntervalAmount";
    private const string DiscreteString = "Discrete";
    private const string PerDiscreteAmountString = "PerDiscreteAmount";
    private const string RountToNextString = "RountToNext";

    #endregion

    [DataMember(Name = StartTimeString)]
    private TimeSpan _startTime;        //время начала интервала
    [DataMember(Name = IntervalAmountString)]
    private decimal _intervalAmount;    //сумма за интервал
    [DataMember(Name = DiscreteString)]
    private TimeSpan _discrete;         //дискрет
    [DataMember(Name = PerDiscreteAmountString)]
    private decimal _perDiscreteAmount; //сумма за дискрет
    [DataMember(Name = RountToNextString)]
    private bool _roundToNext;          //округлять время до следующего интервала

    #region [ properties ]

    /// <summary>
    /// Возвращает/устанавливает время начала интервала
    /// </summary>
    public TimeSpan StartTime
    {
      get { return _startTime; }
      set { _startTime = value; }
    }

    /// <summary>
    /// Возвращает/устанавливает сумму за интервал
    /// </summary>
    public decimal IntervalAmount
    {
      get { return _intervalAmount; }
      set { _intervalAmount = value; }
    }

    /// <summary>
    /// Возвращает/устанавливает дискрет
    /// </summary>
    public TimeSpan Discrete
    {
      get { return _discrete; }
      set { _discrete = value; }
    }

    /// <summary>
    /// Возвращает/устанавливает сумму за дискрет
    /// </summary>
    public decimal PerDiscreteAmount
    {
      get { return _perDiscreteAmount; }
      set { _perDiscreteAmount = value; }
    }

    /// <summary>
    /// Возвращает/устанавливает принак округления времени
    /// </summary>
    public bool RountToNext
    {
      get { return _roundToNext; }
      set { _roundToNext = value; }
    }

    /// <summary>
    /// Возвращает true, если может быть произведён расчёт за интервал, иначе false
    /// </summary>
    public bool CanCalculateInterval
    {
      get { return (_intervalAmount > 0); }
    }

    /// <summary>
    /// Возвращает true, если может быть произведён расчёт за дискрет, иначе false
    /// </summary>
    public bool CanCalculateDiscrete
    {
      get { return ((_discrete > TimeSpan.Zero) && (_perDiscreteAmount > 0)); }
    }

    #endregion

    public TariffMetroCommandCalculateInterval()
      : base(TariffMetroCommandType.CalculateInterval)
    {
      _startTime = TimeSpan.Zero;
      _intervalAmount = 0;
      _discrete = TimeSpan.Zero;
      _perDiscreteAmount = 0;
      _roundToNext = false;
    }

    public TariffMetroCommandCalculateInterval(TariffMetroCommandCalculateInterval command)
      : base(command)
    {
      _startTime = command.StartTime;
      _intervalAmount = command.IntervalAmount;
      _discrete = command.Discrete;
      _perDiscreteAmount = command.PerDiscreteAmount;
      _roundToNext = command.RountToNext;
    }
  }

  /// <summary>
  /// Переход [команда тарифа метро]
  /// </summary>
  [DataContract(Namespace = DataContract.Namespace)]
  public class TariffMetroCommandGoto : TariffMetroCommand
  {
    #region [ const ]

    private const string DestinationString = "Destination";

    #endregion

    [DataMember(Name = DestinationString)]
    private int _destination; //ID команды назначения

    #region [ properties ]

    /// <summary>
    /// Возвращает/устанавливает ID команды назначения
    /// </summary>
    public int Destination
    {
      get { return _destination; }
      set { _destination = value; }
    }

    #endregion

    public TariffMetroCommandGoto()
      : base(TariffMetroCommandType.Goto)
    {
      _destination = (int)DataContract.DefaultID;
    }

    public TariffMetroCommandGoto(TariffMetroCommandGoto command)
      : base(command)
    {
      _destination = command.Destination;
    }
  }

  /// <summary>
  /// Завершение [команда тарифа метро]
  /// </summary>
  [DataContract(Namespace = DataContract.Namespace)]
  public class TariffMetroCommandEnd : TariffMetroCommand
  {
    public TariffMetroCommandEnd()
      : base(TariffMetroCommandType.End)
    {
      //
    }

    public TariffMetroCommandEnd(TariffMetroCommandEnd command)
      : base(command)
    {
      //
    }
  }

  /// <summary>
  /// Тариф метро
  /// </summary>
  [DataContract(Namespace = DataContract.Namespace)]
  [KnownType(typeof(TariffMetroCommand))]
  public class TariffMetro : Tariff
  {
    #region [ const ]

    private const string CommandString = "Command";
    private const string CommandsString = "Commands";

    #endregion

    [DataMember(Name = CommandsString)]
    private List<TariffMetroCommand> _commands; //команды

    #region [ properties ]

    /// <summary>
    /// Возвращает список команд
    /// </summary>
    [LogProperty]
    public IEnumerable<TariffMetroCommand> Commands
    {
      get { return _commands; }
    }

    #endregion

    #region [ static ]

    static TariffMetro()
    {
      Tariff.Register<TariffMetro>(TariffVersion.Version40);
    }

    #endregion

    public TariffMetro()
      : base(TariffVersion.Version40)
    {
      _commands = new List<TariffMetroCommand>();
    }

    public TariffMetro(TariffMetro tariffMetro)
      : base(tariffMetro)
    {
      _commands = new List<TariffMetroCommand>(tariffMetro.Commands);
    }

    #region [ DataItem ]

    protected override void SerializeCustomPropertiesCore(XElement propertiesRoot)
    {
      base.SerializeCustomPropertiesCore(propertiesRoot);

      propertiesRoot.Add(_commands.ToXml(CommandsString, CommandString));
    }

    protected override void DeserializeCustomPropertiesCore(XElement propertiesRoot)
    {
      base.DeserializeCustomPropertiesCore(propertiesRoot);

      XElement xe = propertiesRoot.Element(CommandsString);
      if (xe != null)
      {
        _commands.Clear();
        _commands.AddRange(xe.FromXml<TariffMetroCommand>());
      }
    }

    #endregion

    #region [ ICloneable ]

    public override object Clone()
    {
      return new TariffMetro(this);
    }

    #endregion

    /// <summary>
    /// Добавляет команду
    /// </summary>
    public void AddCommand(TariffMetroCommand command)
    {
      if (_commands.Any(c => c.ID == command.ID))
        throw new ArgumentException(String.Format("Команда с ID = {0} уже есть в тарифе", command.ID), "command");

      _commands.Add(command);
    }

    /// <summary>
    /// Удаляет все команды
    /// </summary>
    public void RemoveAllCommands()
    {
      _commands.Clear();
    }
  }
}