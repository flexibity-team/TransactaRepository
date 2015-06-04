using Parking;

namespace Parking.CashDesk
{
  internal class CashDeskPathManager : GenericPathManager
  {
    #region [ const ]

//#if DEBUG
 //   public const string LibraryPath = @"..\..\Library\";
//#else
    public const string LibraryPath = @".\Library\";
//#endif

    #endregion

    private string _cardReadersPath;

    #region [ properties ]

    public string CardReadersPath
    {
      get { return _cardReadersPath; }
    }

    #endregion

    public CashDeskPathManager()
    {
//#if DEBUG
//      CalculatorsPath = @"R:\Work\Projects\Parking 3.5\Output\Release\Workstation\Calculators";
//#endif
              CalculatorsPath = @".\Calculators";

//#if DEBUG
//      _cardReadersPath = @"R:\Work\Projects\Parking 3.5\Output\Release\Workstation\CardReaders";
//#else
        _cardReadersPath = @".\CardReaders";
//#endif
    }

    #region [ GenericPathManager ]

    protected override string GetApplicationName()
    {
      return "CashDesk";
    }

    #endregion
  }
}