
namespace Parking.SberBank
{
  public class SberBankPathManager : GenericPathManager
  {
    public SberBankPathManager()
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
      return "SberBank";
    }

    #endregion
  }
}