
namespace Parking.Gazprom
{
  public class GazpromPathManager : GenericPathManager
  {
    public GazpromPathManager()
    {
#if DEBUG
      ExtensionsPath = JoinPaths(new string[]
      {
        //@"..\..\..\Extensions\Core\bin\Debug",
      });
#endif
    }

    #region [ GenericPathManager ]

    protected override string GetApplicationName()
    {
      return "Gazprom";
    }

    #endregion
  }
}