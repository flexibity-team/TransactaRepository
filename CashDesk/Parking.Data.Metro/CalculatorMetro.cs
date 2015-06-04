using System;
using System.Threading;
using Parking.Calculators;

namespace Parking.Data.Metro
{
  /// <summary>
  /// Калькулятор для тарифов метро
  /// </summary>
  public class CalculatorMetro : Calculator
  {
    private ReaderWriterLockSlim _locker;

    public CalculatorMetro()
      : base(CalculatorVersion.Version40)
    {
      _locker = new ReaderWriterLockSlim();
    }

    #region [ Calculator ]

    public override bool IsTariffSupported(TariffVersion version)
    {
      return (version == TariffVersion.Version40);
    }

    protected override decimal CalculateCore(Tariff tariff, DateTime dateEntry, DateTime dateExitAssumed, out DateTime dateExitEstimated)
    {
      _locker.EnterWriteLock();
      dateExitEstimated = dateExitAssumed;
      decimal d = -1;
      try
      {
        //проверка версии
        TariffMetro tm = tariff as TariffMetro;
        if (tm == null)
          throw new InvalidOperationException(String.Format("Калькулятор не поддерживает тариф типа {0} версии {1}", tariff.GetType().Name, tariff.Version.GetString()));

        //расчёт
        dateExitEstimated = dateEntry; //t_опл = t_въезда
        d = 0; //S_опл = 0
        DateTime dtEstimatedDate = dateEntry.Date; //t_опл/дата = t_въезда/дата

        //макрос1
        TariffMetroCommandCalculateInterval command = tm.First(); //U_к = 1я команда
        if (command == null)
          return 0;

        DateTime endTime = dtEstimatedDate; //объявление t_интNкон
        while (true)
        {
          //вызов макроса2; обновление t_интNкон и t_опл/дата
          endTime = GetNextEndTime(tm, command, ref dtEstimatedDate);
          if (dateExitEstimated < endTime) //t_опл < t_интNкон
            break;

          command = tm.Next(command); //U_к = U_к + 1
        }

        //main
        while (true)
        {
          d += command.IntervalAmount; //S_опл = S_опл + S_интN

          //вызов макроса2; обновление t_интNкон и t_опл/дата
          endTime = GetNextEndTime(tm, command, ref dtEstimatedDate);

          if (command.Discrete == TimeSpan.Zero) //dt = 0
          {
            dateExitEstimated = endTime; //t_опл = t_интNкон
            if (dateExitEstimated >= dateExitAssumed) //t_опл >= t_тек
              break;
          }
          else
          {
            while (true)
            {
              dateExitEstimated += command.Discrete; //t_опл = t_опл + dt
              d += command.PerDiscreteAmount; //S_опл = S_опл + ds

              if (dateExitEstimated >= dateExitAssumed) //t_опл >= t_тек
                break;
              if (dateExitEstimated >= endTime) //t_опл >= t_интNкон
                break;
            }

            if (dateExitEstimated >= dateExitAssumed) //t_опл >= t_тек
            {
              if (dateExitEstimated < endTime) //t_опл >= t_интNкон
                break;
              if (!command.RountToNext)
                break;

              dateExitEstimated = endTime; //t_опл = t_интNкон
              if (dateExitEstimated >= dateExitAssumed) //t_опл >= t_тек
                break;
            }
          }

          command = tm.Next(command); //U_к = U_к + 1
        }

        //выберем максимальное время выезда
        endTime = dateExitAssumed + tm.FreeTimeAfterPayment;
        if (endTime > dateExitEstimated)
          dateExitEstimated = endTime;
      }
      finally
      {
        _locker.ExitWriteLock();
      }

      return d;
    }

    #endregion

    private DateTime GetNextEndTime(TariffMetro tariff, TariffMetroCommandCalculateInterval currentCommand, ref DateTime dtEstimatedDate)
    {
      //TimeSpan t1 = currentCommand.StartTime; //t1 = t_интN
      TariffMetroCommandCalculateInterval nextCommand = tariff.Next(currentCommand); //U_к = U_к + 1

      //TimeSpan t2 = nextCommand.StartTime; //t2 = t_интN+1
      DateTime endTime = dtEstimatedDate.Add(nextCommand.StartTime); //t_интNкон = t_опл/дата + t_интN+1

      if (nextCommand.StartTime <= currentCommand.StartTime) //t2 <= t1
      {
        endTime += TimeSpan.FromDays(1); //t_интNкон = t_интNкон + 24ч
        dtEstimatedDate += TimeSpan.FromDays(1); //t_опл/дата = t_опл/дата + 24ч
      }

      return endTime;
    }

    //private decimal Test(TariffMetro tariff, DateTime dtEntry, DateTime dtExit, out DateTime dtEstimated)
    //{
    //  dtEstimated = dtEntry; //t_опл = t_въезда
    //  decimal sum = 0; //S_опл = 0
    //  DateTime dtEstimatedDate = dtEntry.Date; //t_опл/дата = t_въезда/дата

    //  //макрос1
    //  TariffMetroCommandCalculateInterval command = tariff.First(); //U_к = 1я команда
    //  if (command == null)
    //    return 0;

    //  DateTime endTime = dtEstimatedDate; //объявление t_интNкон
    //  while (true)
    //  {
    //    //вызов макроса2; обновление t_интNкон и t_опл/дата
    //    endTime = GetNextEndTime(tariff, command, ref dtEstimatedDate);
    //    if (dtEstimated < endTime) //t_опл < t_интNкон
    //      break;

    //    command = tariff.Next(command); //U_к = U_к + 1
    //  }

    //  //main
    //  while (true)
    //  {
    //    sum += command.IntervalAmount; //S_опл = S_опл + S_интN

    //    //вызов макроса2; обновление t_интNкон и t_опл/дата
    //    endTime = GetNextEndTime(tariff, command, ref dtEstimatedDate);

    //    if (command.Discrete == TimeSpan.Zero) //dt = 0
    //    {
    //      dtEstimated = endTime; //t_опл = t_интNкон
    //      if (dtEstimated >= dtExit) //t_опл >= t_тек
    //        break;
    //    }
    //    else
    //    {
    //      while (true)
    //      {
    //        dtEstimated += command.Discrete; //t_опл = t_опл + dt
    //        sum += command.PerDiscreteAmount; //S_опл = S_опл + ds

    //        if (dtEstimated >= dtExit) //t_опл >= t_тек
    //          break;
    //        if (dtEstimated >= endTime) //t_опл >= t_интNкон
    //          break;
    //      }

    //      if (dtEstimated >= dtExit) //t_опл >= t_тек
    //      {
    //        if (dtEstimated < endTime) //t_опл >= t_интNкон
    //          break;
    //        if (!command.RountToNext)
    //          break;

    //        dtEstimated = endTime; //t_опл = t_интNкон
    //        if (dtEstimated >= dtExit) //t_опл >= t_тек
    //          break;
    //      }
    //    }

    //    command = tariff.Next(command); //U_к = U_к + 1
    //  }

    //  //выберем максимальное время выезда
    //  endTime = dtExit + tariff.FreeTimeAfterPayment;
    //  if (endTime > dtEstimated)
    //    dtEstimated = endTime;

    //  return sum;
    //}
  }
}