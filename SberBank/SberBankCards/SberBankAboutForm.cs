using System;
using System.Text;

namespace Parking.SberBank
{
    using Network;

    public partial class SberBankAboutForm : ParkingAboutForm
  {
    public SberBankAboutForm()
    {
      InitializeComponent();
    }

    #region [ AboutForm ]

    protected override string GetProductVersion()
    {
      var sb = new StringBuilder();
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
