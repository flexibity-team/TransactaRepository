using System;
using System.Runtime.InteropServices;
using System.Threading;
using Parking.Data;
using RMLib;

namespace Parking.FiscalDevice
{
  /// <summary>
  /// ��� ����-08��
  /// </summary>
  public class Prim08ControlOld : Disposable//, IFiscalDevice
  {
    #region [ const ]

//#if DEBUG
//    private const string Prim08Path = @"..\..\Library\Prim08.dll";
//#else
      private const string Prim08Path = @".\Library\Prim08.dll";
      //private const string Prim08Path = "Prim08.dll";
      //#endif

    #endregion

    #region [ imports ]

    /// <summary>
    /// ������������� ���
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    private static extern bool _Init(int ComPortNumber, ref Byte asPassw, bool CreateSuspended);

    /// <summary>
    /// ���������� ��� � ������ 
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    private static extern bool _Resume();

    /// <summary>
    /// ������������ ���
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    private static extern int _Close();

    /// <summary>
    /// ������� ����� ������ ������������� ���
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern int _SetOperDateTime(double date);

    /// <summary>
    /// �������� ������ ������ � ���
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern int _SessionOpen();

    /// <summary>
    /// ��������� ���
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern int _GetErrorNumber();

    /// <summary>
    /// ��������� ���������� ���������� (true - �������, false - ����� ����������� ��������)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _GetTransState();

    /// <summary>
    /// ������ Z ������
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _ZAccount();

    /// <summary>
    /// ������ X ������
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _XAccount();

    /// <summary>
    /// ����������� �����
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _GetAccount();

    /// <summary>
    /// ��������� ����� (true - �������, false - �������)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _GetOpened();

    /// <summary>
    /// �������� ���������� ����� � �����
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern double _GetMoneyInCCM();

    /// <summary>
    /// �������� ������� �� �����
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern double _GetMoneyPerChange();

    /// <summary>
    /// 
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetChangeName(ref Byte name);

    /// <summary>
    /// 
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _ChangeOpen();

    /// <summary>
    /// ��������� ����� ����� ��� ���������� ��������
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetCurrentMoney(double fSumm);

    /// <summary>
    /// �������� ������ � ����
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _AddRequisite(int iV, int iH, int iF, String sText);

    /// <summary>
    /// ������ ��� �������
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetCashierName(String sText);

    /// <summary>
    /// ������ ID ������� (� �������� �������� ������ 0)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetCashierID(Byte bID);

    /// <summary>
    /// �������� ����� � �����
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _CashIn();

    /// <summary>
    /// ���������� �����
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _CashOut();

    /// <summary>
    /// �������� ��������� ���
    /// (true - ��������, false - �����)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _isExecuted();

    /// <summary>
    /// ������� ���� (����� �����������)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _ClearRequisites();

    /// <summary>
    /// ������ ����
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _WholeCheck();

    /// <summary>
    /// ���������� ���� ������
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetCurrentPayType(int fPayType);

    /// <summary>
    /// ���������� ���� ���������
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetCurrentDocType(int fPayType);

    #endregion

    private string _cashName;

    public Prim08ControlOld()
    {
      _cashName = String.Empty;
    }

    #region [ Disposable ]

    protected override void  DisposeManagedResources()
    {
      _Close();
    }

    #endregion

    #region [ IFiscalDevice ]

    /// <summary>
    /// ���������� ��������� ����� (�������/�������)
    /// </summary>
    public bool IsSessionOpened
    {
      get { return _GetOpened(); }
    }

    /// <summary>
    /// ���������� ����������� � ����������� ������������
    /// </summary>
    public void Initialize(int comPort, string name)
    {
      _cashName = name;
      byte[] _bPass = new Byte[] { 0x41, 0x45, 0x52, 0x46 };  // AERF
      if (!_Init(comPort, ref _bPass[0], true))
        throw new FiscalDeviceException("������ ����������� � ����������� ������������.");

      Thread.Sleep(100);
      _Resume();

      OpenSessionInternal(); // �������� ������ ���������. ���� ���������� �� �� �����, ����� ����������
    }

    /// <summary>
    /// ���������� ���������� ����� � �����
    /// </summary>
    public double GetKKMAmount()
    {
      GetDigAccount();
      GetDigAccount();

      return _GetMoneyInCCM();
    }

    /// <summary>
    /// ���������� ������� �� ����� 
    /// </summary>
    public double GetSessionAmount()
    {
      //��������� ���������� �� ������� ���������� !!!
      GetDigAccount();
      GetDigAccount();

      return _GetMoneyInCCM();
    }

    /// <summary>
    /// ��������� �����
    /// </summary>
    public void OpenSession()
    {
      OpenSessionInternal(); // ������� �����

      SetOperDateTime(DateTime.Now); // ������ ����� ��������
      SetChangeName();

      _ChangeOpen();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ �������� �����");
    }

    /// <summary>
    /// ��������� �����
    /// </summary>
    public void CloseSession()
    {
      try
      {
        OpenSessionInternal();
      }
      catch (Exception)
      {
        // ��� ��� ������ �� �����.
      }

      try
      {
        SetOperDateTime(DateTime.Now); // ������ ����� ��������
      }
      catch (Exception)
      {
        // ��� ��� ������ �� �����.
      }

      //  Z - �����
      _ZAccount();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if ((nErr != 0) && (nErr != 21))
        throw new FiscalDeviceException(nErr, "������ ������ Z-������");
    }

    /// <summary>
    /// �������� X-�����
    /// </summary>
    public void PrintXReport()
    {
      OpenSessionInternal(); // ������� �����

      SetOperDateTime(DateTime.Now); // ������ ����� ��������

      _XAccount();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ ������ X-������");
    }

    /// <summary>
    /// �������� ������ � ����� (��������, ��� �������)
    /// </summary>
    public void CashIn(double amount)
    {
      // TODO: ����� ��� ���� ����� ������� ������ �������?

      SetOperDateTime(DateTime.Now);

      // ��������� ����� � �����
      SetCurrentMoney(amount);

      _CashIn();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ �������� �����.");
    }

    /// <summary>
    /// ������� ������ �� ����� (����������)
    /// </summary>
    public void CashOut(double amount)
    {
      // TODO: ����� ��� ���� ����� ������� ������ �������?

      SetOperDateTime(DateTime.Now);

      // ��������� ����� � �����
      SetCurrentMoney(amount);

      _CashOut();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ ����������.");
    }

    /// <summary>
    /// ������
    /// </summary>
    public void Payment(PaymentDocument doc)
    {
      int nErr;

      // ������� �����
      OpenSessionInternal();

      // �������� �����
      if (!_GetOpened())
        throw new FiscalDeviceException("��� ���������� ������ ���������� �������������� ������� �����!");


      //������������ ����
      SetCurrentMoney((double)doc.Amount);

      _SetCurrentDocType((int)doc.Type);
      _SetCurrentPayType((int)doc.PaymentType); // ��� = ��������
      //Settings.Default.CashNumber.ToString()
      _SetCashierName(_cashName);
      _SetCashierID(0);

      _ClearRequisites();
      String sCardNum = Convert.ToString(doc.CardId, 16);

      switch (doc.PaymentReason)
      {
        case PaymentReason.Parking:
          _AddRequisite(1, 1, 1, "������ ����� ��������");
          _AddRequisite(2, 1, 1, "����������� �����: " + sCardNum.ToUpper());
          _AddRequisite(3, 1, 1, "����� ������: " + String.Format("{0:G}", doc.TimeEntry));
          _AddRequisite(4, 1, 1, "����� �� :    " + String.Format("{0:G}", doc.TimeExit));
          _AddRequisite(5, 1, 1, "��������: " + Utils.TimeToString(doc.TimeExit - doc.TimeEntry));
          if (doc.Payment <= 0) _AddRequisite(6, 1, 1, "                 ");
          else _AddRequisite(6, 1, 1, "�������� �����: " + String.Format("{0:C}", doc.Payment));
          break;
        case PaymentReason.Fine:
          _AddRequisite(1, 1, 1, "������ ����� �������� - �����");
          _AddRequisite(2, 1, 1, "����������� �����: " + sCardNum.ToUpper());
          _AddRequisite(3, 1, 1, "����� ������: " + String.Format("{0:G}", doc.TimeEntry));
          _AddRequisite(4, 1, 1, "����� �� :    " + String.Format("{0:G}", doc.TimeExit));
          _AddRequisite(5, 1, 1, "��������: " + Utils.TimeToString(doc.TimeExit - doc.TimeEntry));
          if (doc.Payment / 100 <= 0) _AddRequisite(6, 1, 1, "                 ");
          else _AddRequisite(6, 1, 1, "�������� �����: " + String.Format("{0:C}", doc.Payment));
          break;
        case PaymentReason.ECash:
          _AddRequisite(1, 1, 1, "���������� ��. ��������");
          _AddRequisite(2, 1, 1, "����������� �����: " + sCardNum.ToUpper());
          break;
        case PaymentReason.Any:
          break;
        case PaymentReason.Refund:
          _AddRequisite(1, 1, 1, "�������");
          break;
        default:
          break;
      }

      _WholeCheck();
      WaitForOperation();
      nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ ������ ����. ��������� ��� � ��������� ��������.");
    }

    #endregion

    /// <summary>
    /// ������ ����� �������� � ���������� ������������
    /// </summary>
    private void SetOperDateTime(DateTime date)
    {
      _SetOperDateTime(date.ToOADate());
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ ������� ������� ��������.");
    }

    /// <summary>
    /// ��������� ��� ������
    /// </summary>
    private void SetChangeName()
    {
      Byte[] _bName = new Byte[] { 0x00, 0x00, 0x00 };  //  
      _SetChangeName(ref _bName[0]);
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ ������� ����� ������");
    }

    /// <summary>
    /// ������������� ����� � ���������� ������������ ��� ���������� �������� �������� ��� ���������
    /// </summary>
    private void SetCurrentMoney(double fSumm)
    {
      // ��������� ����� � �����
      _SetCurrentMoney(fSumm);
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr);
    }

    /// <summary>
    /// ��������� �����
    /// </summary>
    private void OpenSessionInternal()
    {
      SetOperDateTime(DateTime.Now);

      _SessionOpen();
      WaitForOperation();

      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ �������� ������");
    }

    /// <summary>
    /// ������ Z ������
    /// </summary>
    private void PrintZReport()
    {
      Thread.Sleep(100);
      _ZAccount();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ ������ Z-������");
    }

    // �������� ����������� ����� (��� ���?)
    // ���������: 0 - ��, -1 - ������ ����� � ���, -2 - ������ ��������� ������������ ������
    private void GetDigAccount()
    {
      // ������� �����
      OpenSessionInternal();

      //  ����������� �����
      SetOperDateTime(DateTime.Now);

      _GetAccount();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "������ ��������� �����");
    }

    // ���������� ���� � ���������� � ��������� ��� (��� ������� ������).
    // ��� ���������� ������ ����� �� ������������.
    // ������� ������������ �������� ���������� ������ (�������� ��������)
    private void WaitForOperation()
    {
      //KKMStateForm frmKKMState = new KKMStateForm();
      //frmKKMState.Show();
      //frmKKMState.SetMessage("����� ������� � ���...");
      //frmKKMState.Update();
      Thread.Sleep(100);

      //while (!_GetTransState())
      while (!_isExecuted())
        Thread.Sleep(200);

      //frmKKMState.Hide();
      //if (GetErrorNumber() != 0)
      //{
      //frmKKMState.SetMessage(GetTextKKMError()); // ����� ��������� �� ������
      //frmKKMState.ShowDialog();
      //frmKKMState.Dispose();
      //}
      //frmKKMState.Dispose();
    }

    public override string ToString()
    {
      return "����-08��";
    }
  }
}