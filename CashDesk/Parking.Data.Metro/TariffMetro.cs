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
  public enum TariffMetroCommandType
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
    private TariffMetroCommandType type; //тип команды

    #region [ properties ]

    /// <summary>
    /// Возвращает тип команды
    /// </summary>
    public TariffMetroCommandType Type
    {
      get { return type; }
    }

    #endregion

    protected TariffMetroCommand(TariffMetroCommandType type)
    {
      this.type = type;
    }

    protected TariffMetroCommand(TariffMetroCommand command)
      : base(command.ID)
	  {
      type = command.Type;
	  }

    #region [ ICloneable ]

    /// <summary>
    /// Возвращает копию команды
    /// </summary>
    public override object Clone()
    {
      TariffMetroCommand command = null;
      switch (type)
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
      return type.GetString();
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
    private TimeSpan startTime;        //время начала интервала
    [DataMember(Name = IntervalAmountString)]
    private decimal intervalAmount;    //сумма за интервал
    [DataMember(Name = DiscreteString)]
    private TimeSpan discrete;         //дискрет
    [DataMember(Name = PerDiscreteAmountString)]
    private decimal perDiscreteAmount; //сумма за дискрет
    [DataMember(Name = RountToNextString)]
    private bool roundToNext;          //округлять время до следующего интервала

    #region [ properties ]

    /// <summary>
    /// Возвращает/устанавливает время начала интервала
    /// </summary>
    public TimeSpan StartTime
    {
      get { return startTime; }
      set { startTime = value; }
    }

    /// <summary>
    /// Возвращает/устанавливает сумму за интервал
    /// </summary>
    public decimal IntervalAmount
    {
      get { return intervalAmount; }
      set { intervalAmount = value; }
    }

    /// <summary>
    /// Возвращает/устанавливает дискрет
    /// </summary>
    public TimeSpan Discrete
    {
      get { return discrete; }
      set { discrete = value; }
    }

    /// <summary>
    /// Возвращает/устанавливает сумму за дискрет
    /// </summary>
    public decimal PerDiscreteAmount
    {
      get { return perDiscreteAmount; }
      set { perDiscreteAmount = value; }
    }

    /// <summary>
    /// Возвращает/устанавливает принак округления времени
    /// </summary>
    public bool RountToNext
    {
      get { return roundToNext; }
      set { roundToNext = value; }
    }

    /// <summary>
    /// Возвращает true, если может быть произведён расчёт за интервал, иначе false
    /// </summary>
    public bool CanCalculateInterval
    {
      get { return (intervalAmount > 0); }
    }

    /// <summary>
    /// Возвращает true, если может быть произведён расчёт за дискрет, иначе false
    /// </summary>
    public bool CanCalculateDiscrete
    {
      get { return ((discrete > TimeSpan.Zero) && (perDiscreteAmount > 0)); }
    }

    #endregion

    public TariffMetroCommandCalculateInterval()
      : base(TariffMetroCommandType.CalculateInterval)
    {
      startTime = TimeSpan.Zero;
      intervalAmount = 0;
      discrete = TimeSpan.Zero;
      perDiscreteAmount = 0;
      roundToNext = false;
    }

    public TariffMetroCommandCalculateInterval(TariffMetroCommandCalculateInterval command)
      : base(command)
    {
      startTime = command.StartTime;
      intervalAmount = command.IntervalAmount;
      discrete = command.Discrete;
      perDiscreteAmount = command.PerDiscreteAmount;
      roundToNext = command.RountToNext;
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
    private int destination; //ID команды назначения

    #region [ properties ]

    /// <summary>
    /// Возвращает/устанавливает ID команды назначения
    /// </summary>
    public int Destination
    {
      get { return destination; }
      set { destination = value; }
    }

    #endregion

    public TariffMetroCommandGoto()
      : base(TariffMetroCommandType.Goto)
    {
      destination = (int)DataContract.DefaultID;
    }

    public TariffMetroCommandGoto(TariffMetroCommandGoto command)
      : base(command)
    {
      destination = command.Destination;
    }
  }

  /// <summary>
  /// Завершение [команда тарифа метро]
  /// </summary>
  [DataContract(Namespace = DataContract.Namespace)]
  public class TariffMetroCommandEnd : TariffMetroCommand
  {
      private readonly TariffMetroCommandEnd command;

      public TariffMetroCommandEnd()
      : base(TariffMetroCommandType.End)
    {
      //
    }

    public TariffMetroCommandEnd(TariffMetroCommandEnd command)
      : base(command)
    {
        this.command = command;
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
    private List<TariffMetroCommand> commands; //команды

    #region [ properties ]

    /// <summary>
    /// Возвращает список команд
    /// </summary>
    [LogProperty]
    public IEnumerable<TariffMetroCommand> Commands
    {
      get { return commands; }
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
      commands = new List<TariffMetroCommand>();
    }

    public TariffMetro(TariffMetro tariffMetro)
      : base(tariffMetro)
    {
      commands = new List<TariffMetroCommand>(tariffMetro.Commands);
    }

    #region [ DataItem ]

    protected override void SerializeCustomPropertiesCore(XElement propertiesRoot)
    {
      base.SerializeCustomPropertiesCore(propertiesRoot);

      propertiesRoot.Add(commands.ToXml(rootName: CommandsString, nodeName: CommandString));
    }

    protected override void DeserializeCustomPropertiesCore(XElement propertiesRoot)
    {
      base.DeserializeCustomPropertiesCore(propertiesRoot);

      XElement xe = propertiesRoot.Element(name: CommandsString);
      if (xe != null)
      {
        commands.Clear();
        commands.AddRange(xe.FromXml<TariffMetroCommand>());
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
      if (commands.Any(c => c.ID == command.ID))
        throw new ArgumentException(String.Format("Команда с ID = {0} уже есть в тарифе", command.ID), "command");

      commands.Add(command);
    }

    /// <summary>
    /// Удаляет все команды
    /// </summary>
    public void RemoveAllCommands()
    {
      commands.Clear();
    }
  }
}