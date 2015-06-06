using System;
using System.Windows.Forms;

namespace Parking.FiscalDevice
{
  /// <summary>
  /// 
  /// </summary>
  public partial class KKMStateForm : Form
  {
    public KKMStateForm()
    {
      InitializeComponent();
    }

    #region [ window events ]

    private void KKMStateForm_Load(object sender, EventArgs e)
    {
      Update();
    }

    #endregion

    /// <summary>
    /// Отображает сообщение на экране
    /// </summary>
    public void SetMessage(string message)
    {
      tbKKMMessage.Clear();
      tbKKMMessage.Text = message;
      Update();
    }
  }
}