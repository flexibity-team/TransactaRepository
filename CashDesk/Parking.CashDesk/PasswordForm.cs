using System.Windows.Forms;
using Parking.Data;

namespace Parking.CashDesk
{
  public partial class PasswordForm : Form
  {
    public PasswordForm()
    {
      InitializeComponent();

      this.SetLogoIcon();
    }

    public PasswordForm(string caption)
    {
      InitializeComponent();

      lblPass.Text = caption;
    }
  }
}