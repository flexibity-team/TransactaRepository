using System.Linq;

namespace Parking.Data.Metro
{
  internal static class CalculatorMetroHelper
  {
    public static TariffMetroCommandCalculateInterval First(this TariffMetro tariff)
    {
      TariffMetroCommand cmd = tariff.Commands.FirstOrDefault();
      if (cmd == null)
        return null;

      while (cmd.Type == TariffMetroCommandType.Goto)
        cmd = tariff.Goto(cmd);

      return (TariffMetroCommandCalculateInterval)cmd;
    }

    public static TariffMetroCommandCalculateInterval Next(this TariffMetro tariff, TariffMetroCommand command)
    {
      TariffMetroCommand cmd = tariff.Commands.FirstOrDefault(c => c.ID == command.ID + 1);
      if (cmd == null)
        return null;

      while (cmd.Type == TariffMetroCommandType.Goto)
        cmd = tariff.Goto(cmd);

      return (TariffMetroCommandCalculateInterval)cmd;
    }

    private static TariffMetroCommand Goto(this TariffMetro tariff, TariffMetroCommand command)
    {
      var g = command as TariffMetroCommandGoto;
      return g == null ? command : tariff.Commands.FirstOrDefault(c => c.ID == g.Destination);
    }
  }
}