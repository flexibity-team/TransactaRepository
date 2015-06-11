using System.Configuration;

namespace Parking.Server.Properties
{
  internal partial class Settings
  {
    protected override void OnSettingsLoaded(object sender, SettingsLoadedEventArgs e)
    {
      ServerMode mode = (ServerMode)this["Mode"];
      ServerArguments args = new ServerArguments();

      //get fix mode
      bool fixMode = ((mode & ServerMode.Fix) != 0) || args.IsFix;
      //assert fix mode
      if (fixMode)
        mode |= ServerMode.Fix;

      //adjust mode
      if (fixMode)
      {
        //switch off network and wcf
        mode |= ServerMode.NoNetwork;
        mode |= ServerMode.NoWCF;

        //switch on database
        if ((mode & ServerMode.NoDatabase) != 0)
          mode ^= ServerMode.NoDatabase;

        //update mode
        this["Mode"] = mode;
      }

      base.OnSettingsLoaded(sender, e);
    }
  }
}