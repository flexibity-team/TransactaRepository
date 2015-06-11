using System;
using System.Text;

namespace Parking.Gazprom
{
  public partial class GazpromAboutForm : ParkingAboutForm
  {
    public GazpromAboutForm()
    {
      InitializeComponent();
    }

    #region [ AboutForm ]

    protected override string GetProductVersion()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(SystemVersion.Version.ToString());
      if (!String.IsNullOrEmpty(SystemVersion.Edition))
        sb.AppendFormat(" {0}", SystemVersion.Edition);

      sb.AppendLine();
      sb.Append(BankingDevice.Version);

      return sb.ToString();
    }

    #endregion
  }
}
