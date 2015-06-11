using System;
using System.Runtime.InteropServices;
using System.Threading;
using Parking.Data;
using RMLib;

namespace Parking.FiscalDevice
{
  /// <summary>
  /// ККМ Прим-08ТК
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
    /// Инициализация ККМ
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    private static extern bool _Init(int ComPortNumber, ref Byte asPassw, bool CreateSuspended);

    /// <summary>
    /// Подготовка ККМ к работе 
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    private static extern bool _Resume();

    /// <summary>
    /// Освобождение ККМ
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    private static extern int _Close();

    /// <summary>
    /// Задание время начала операционного дня
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern int _SetOperDateTime(double date);

    /// <summary>
    /// Открытие сеанса обмена с ККМ
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern int _SessionOpen();

    /// <summary>
    /// Состояние ККМ
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern int _GetErrorNumber();

    /// <summary>
    /// Состояние выполнения транзакции (true - простой, false - занят выполнением операции)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _GetTransState();

    /// <summary>
    /// Печать Z отчета
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _ZAccount();

    /// <summary>
    /// Печать X отчета
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _XAccount();

    /// <summary>
    /// Электронный отчёт
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _GetAccount();

    /// <summary>
    /// Состояние смены (true - открыта, false - закрыта)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _GetOpened();

    /// <summary>
    /// Получить количество денег в кассе
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern double _GetMoneyInCCM();

    /// <summary>
    /// Получить выручку за смену
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
    /// Установка суммы денег для проведения операции
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetCurrentMoney(double fSumm);

    /// <summary>
    /// Добавить строку к чеку
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _AddRequisite(int iV, int iH, int iF, String sText);

    /// <summary>
    /// Задать имя кассира
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetCashierName(String sText);

    /// <summary>
    /// Задать ID кассира (в исходном варианте всегда 0)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetCashierID(Byte bID);

    /// <summary>
    /// Внесение денег в кассу
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _CashIn();

    /// <summary>
    /// Инкассация денег
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _CashOut();

    /// <summary>
    /// Проверка занятости ККМ
    /// (true - свободен, false - занят)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _isExecuted();

    /// <summary>
    /// Очистка чека (перед заполнением)
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _ClearRequisites();

    /// <summary>
    /// Печать чека
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern bool _WholeCheck();

    /// <summary>
    /// Назначение типа оплаты
    /// </summary>
    [DllImport(Prim08Path, CharSet = CharSet.Ansi)]
    public static extern void _SetCurrentPayType(int fPayType);

    /// <summary>
    /// Назначение типа документа
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
    /// Возвращает состояние смены (открыта/закрыта)
    /// </summary>
    public bool IsSessionOpened
    {
      get { return _GetOpened(); }
    }

    /// <summary>
    /// Производит подключение к фискальному регистратору
    /// </summary>
    public void Initialize(int comPort, string name)
    {
      _cashName = name;
      byte[] _bPass = new Byte[] { 0x41, 0x45, 0x52, 0x46 };  // AERF
      if (!_Init(comPort, ref _bPass[0], true))
        throw new FiscalDeviceException("Ошибка подключения к фискальному регистратору.");

      Thread.Sleep(100);
      _Resume();

      OpenSessionInternal(); // Проверка пустым действием. Если фискальник не на связи, будет исключение
    }

    /// <summary>
    /// Возвращает количество денег в кассе
    /// </summary>
    public double GetKKMAmount()
    {
      GetDigAccount();
      GetDigAccount();

      return _GetMoneyInCCM();
    }

    /// <summary>
    /// Возвращает выручку за смену 
    /// </summary>
    public double GetSessionAmount()
    {
      //проверить правильная ли функция вызывается !!!
      GetDigAccount();
      GetDigAccount();

      return _GetMoneyInCCM();
    }

    /// <summary>
    /// Открывает смену
    /// </summary>
    public void OpenSession()
    {
      OpenSessionInternal(); // Открыть сеанс

      SetOperDateTime(DateTime.Now); // Задать время операции
      SetChangeName();

      _ChangeOpen();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка открытия смены");
    }

    /// <summary>
    /// Закрывает смену
    /// </summary>
    public void CloseSession()
    {
      try
      {
        OpenSessionInternal();
      }
      catch (Exception)
      {
        // Тут нам ошибок не нужно.
      }

      try
      {
        SetOperDateTime(DateTime.Now); // Задать время операции
      }
      catch (Exception)
      {
        // Тут нам ошибок не нужно.
      }

      //  Z - отчёт
      _ZAccount();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if ((nErr != 0) && (nErr != 21))
        throw new FiscalDeviceException(nErr, "Ошибка печати Z-отчета");
    }

    /// <summary>
    /// Печатает X-отчет
    /// </summary>
    public void PrintXReport()
    {
      OpenSessionInternal(); // Открыть сеанс

      SetOperDateTime(DateTime.Now); // Задать время операции

      _XAccount();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка печати X-отчета");
    }

    /// <summary>
    /// Положить деньги в кассу (например, для размена)
    /// </summary>
    public void CashIn(double amount)
    {
      // TODO: Может тут тоже нужно сначала сессию открыть?

      SetOperDateTime(DateTime.Now);

      // Установка денег в кассе
      SetCurrentMoney(amount);

      _CashIn();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка внесения денег.");
    }

    /// <summary>
    /// Забрать деньги из кассы (инкассация)
    /// </summary>
    public void CashOut(double amount)
    {
      // TODO: Может тут тоже нужно сначала сессию открыть?

      SetOperDateTime(DateTime.Now);

      // Установка денег в кассе
      SetCurrentMoney(amount);

      _CashOut();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка инкассации.");
    }

    /// <summary>
    /// Оплата
    /// </summary>
    public void Payment(PaymentDocument doc)
    {
      int nErr;

      // Открыть сеанс
      OpenSessionInternal();

      // проверим смену
      if (!_GetOpened())
        throw new FiscalDeviceException("Для проведения оплаты необходимо предварительно открыть смену!");


      //Формирование чека
      SetCurrentMoney((double)doc.Amount);

      _SetCurrentDocType((int)doc.Type);
      _SetCurrentPayType((int)doc.PaymentType); // Тип = Наличные
      //Settings.Default.CashNumber.ToString()
      _SetCashierName(_cashName);
      _SetCashierID(0);

      _ClearRequisites();
      String sCardNum = Convert.ToString(doc.CardId, 16);

      switch (doc.PaymentReason)
      {
        case PaymentReason.Parking:
          _AddRequisite(1, 1, 1, "Оплата услуг парковки");
          _AddRequisite(2, 1, 1, "Парковочная карта: " + sCardNum.ToUpper());
          _AddRequisite(3, 1, 1, "Время въезда: " + String.Format("{0:G}", doc.TimeEntry));
          _AddRequisite(4, 1, 1, "Выезд до :    " + String.Format("{0:G}", doc.TimeExit));
          _AddRequisite(5, 1, 1, "Оплачено: " + Utils.TimeToString(doc.TimeExit - doc.TimeEntry));
          if (doc.Payment <= 0) _AddRequisite(6, 1, 1, "                 ");
          else _AddRequisite(6, 1, 1, "Оплачено ранее: " + String.Format("{0:C}", doc.Payment));
          break;
        case PaymentReason.Fine:
          _AddRequisite(1, 1, 1, "Оплата услуг парковки - ШТРАФ");
          _AddRequisite(2, 1, 1, "Парковочная карта: " + sCardNum.ToUpper());
          _AddRequisite(3, 1, 1, "Время въезда: " + String.Format("{0:G}", doc.TimeEntry));
          _AddRequisite(4, 1, 1, "Выезд до :    " + String.Format("{0:G}", doc.TimeExit));
          _AddRequisite(5, 1, 1, "Оплачено: " + Utils.TimeToString(doc.TimeExit - doc.TimeEntry));
          if (doc.Payment / 100 <= 0) _AddRequisite(6, 1, 1, "                 ");
          else _AddRequisite(6, 1, 1, "Оплачено ранее: " + String.Format("{0:C}", doc.Payment));
          break;
        case PaymentReason.ECash:
          _AddRequisite(1, 1, 1, "Пополнение эл. кошелька");
          _AddRequisite(2, 1, 1, "Парковочная карта: " + sCardNum.ToUpper());
          break;
        case PaymentReason.Any:
          break;
        case PaymentReason.Refund:
          _AddRequisite(1, 1, 1, "Возврат");
          break;
        default:
          break;
      }

      _WholeCheck();
      WaitForOperation();
      nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка печати чека. Проверьте ККМ и повторите операцию.");
    }

    #endregion

    /// <summary>
    /// Задает время операции в фискальном регистраторе
    /// </summary>
    private void SetOperDateTime(DateTime date)
    {
      _SetOperDateTime(date.ToOADate());
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка задания времени операции.");
    }

    /// <summary>
    /// Назначает имя сессии
    /// </summary>
    private void SetChangeName()
    {
      Byte[] _bName = new Byte[] { 0x00, 0x00, 0x00 };  //  
      _SetChangeName(ref _bName[0]);
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка задания имени сессии");
    }

    /// <summary>
    /// Устанавливает сумму в фискальном регистраторе для дальнейшей операции внесения или вынесения
    /// </summary>
    private void SetCurrentMoney(double fSumm)
    {
      // Установка денег в кассе
      _SetCurrentMoney(fSumm);
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr);
    }

    /// <summary>
    /// Открывает смену
    /// </summary>
    private void OpenSessionInternal()
    {
      SetOperDateTime(DateTime.Now);

      _SessionOpen();
      WaitForOperation();

      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка открытия сессии");
    }

    /// <summary>
    /// Печать Z отчета
    /// </summary>
    private void PrintZReport()
    {
      Thread.Sleep(100);
      _ZAccount();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка печати Z-отчета");
    }

    // Получить электронный отчет (что это?)
    // Результат: 0 - ОК, -1 - ошибка связи с ККМ, -2 - ошибка получения электронного отчета
    private void GetDigAccount()
    {
      // Открыть сеанс
      OpenSessionInternal();

      //  электронный отчёт
      SetOperDateTime(DateTime.Now);

      _GetAccount();
      WaitForOperation();
      int nErr = _GetErrorNumber();
      if (nErr != 0)
        throw new FiscalDeviceException(nErr, "Ошибка получения счета");
    }

    // Отображает окно с сообщением о состоянии ККМ (при наличие ошибок).
    // При отсутствии ошибок форма не отображается.
    // Функция обеспечивает ожидание отсутствия ошибок (заменяет задержки)
    private void WaitForOperation()
    {
      //KKMStateForm frmKKMState = new KKMStateForm();
      //frmKKMState.Show();
      //frmKKMState.SetMessage("Обмен данными с ККМ...");
      //frmKKMState.Update();
      Thread.Sleep(100);

      //while (!_GetTransState())
      while (!_isExecuted())
        Thread.Sleep(200);

      //frmKKMState.Hide();
      //if (GetErrorNumber() != 0)
      //{
      //frmKKMState.SetMessage(GetTextKKMError()); // Вывод сообщения об ошибке
      //frmKKMState.ShowDialog();
      //frmKKMState.Dispose();
      //}
      //frmKKMState.Dispose();
    }

    public override string ToString()
    {
      return "Прим-08ТК";
    }
  }
}