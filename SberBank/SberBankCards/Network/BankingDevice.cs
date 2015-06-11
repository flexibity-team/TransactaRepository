namespace Parking.SberBank.Network
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Timers;
    using Data;
    using RMLib;
    using Timer = System.Timers.Timer;

    internal enum OperationType
    {
        Payment = 1,
        TotalsReconcilation = 4,
        RequestWorkKey = 5
     }

  internal enum EquipmentState
  {
    Idle = 0,
    Started = 1,
    WaitingForCard = 2,
    CardInserted = 4,
    PinEntered = 8,
    AuthCompleted = 16,
    CardTaken = 32
  }

  internal enum TransactionResult
  {
    Unknown = 0,
    OK = 1,
    Error = 2,
    Cancelled = 1
  }

  internal enum ReconcilationState : byte
  {
    Idle = 0x00,
    Started = 0x08,
    Completed = 0x18
  }

  internal enum CancelState
  {
    None = 0,
    Pending = 1,
    Completed = 2
  }

  internal sealed class AuthResponse
  {
    public readonly byte[] Source;
    public readonly int TerminalID;
    public readonly string MerchantID;
    public readonly byte CashBoxUID;
    public readonly string ReceiptID;
    public readonly string PAN;
    public readonly string ExpiryDate;
    public readonly int Amount;
    public readonly int Discount;
    public readonly int Total;
    public readonly string Date;
    public readonly string Time;
    public readonly string InvoiceNumber;
    public readonly string IssuerName;
    public readonly string Currency;
    public readonly int ResponseCode;
    public readonly string VisualResponseCode;
    public readonly string AuthorizationID;
    public readonly string RRN;
    public readonly string ApplicationID;
    public readonly string TC;
    public readonly string AppLab;
    public readonly string DateFull;
    public readonly string TimeFull;
    public readonly int GCSCode;
    public readonly string VisualGCSCode;
    public readonly string AuthResult;
    public readonly string VisualHostResponse;
    public readonly char Approved;
    public readonly string Flags;
    public readonly string STAN;
    public readonly string LocalTime;
    public readonly string RequestUID;
    public readonly string Cryptogram;

    #region [ static ]

    public static AuthResponse Parse(string response, byte[] source)
    {
      string[] sa = response.Split(',');
      int j = sa.Length;
      if (j < 33)
        throw new FormatException(String.Format("Недостаточно параметров в ответе : {0}", j));

      return new AuthResponse(sa, source);
    }

    public static bool IsValidCode(int code)
    {
      return ((code == 0) || (code == 3) || (code == 20) || (code == 959));
    }

    #endregion

    private AuthResponse(string[] sa, byte[] src)
    {
      Source = src;

      if (!Int32.TryParse(sa[0], out TerminalID)) TerminalID = -1;
      MerchantID = sa[1];
      if (!Byte.TryParse(sa[2], out CashBoxUID)) CashBoxUID = 0;
      ReceiptID = sa[3];
      PAN = sa[4];
      ExpiryDate = sa[5];
      if (!Int32.TryParse(sa[6], out Amount)) Amount = -1;
      if (!Int32.TryParse(sa[7], out Discount)) Discount = -1;
      if (!Int32.TryParse(sa[8], out Total)) Total = -1;
      Date = sa[9];
      Time = sa[10];
      InvoiceNumber = sa[11];
      IssuerName = sa[12];
      Currency = sa[13];
      if (!Int32.TryParse(sa[14], out ResponseCode)) ResponseCode = -1;
      VisualResponseCode = sa[15];
      AuthorizationID = sa[16];
      RRN = sa[17];
      ApplicationID = sa[18];
      TC = sa[19];
      AppLab = sa[20];
      DateFull = sa[21];
      TimeFull = sa[22];
      if (!Int32.TryParse(sa[23], out GCSCode)) GCSCode = -1;
      VisualGCSCode = sa[24];
      AuthResult = sa[25];
      VisualHostResponse = sa[26];
      Approved = sa[27].FirstOrDefault();
      Flags = sa[28];
      STAN = sa[29];
      LocalTime = sa[30];
      RequestUID = sa[31];
      Cryptogram = sa[32];
    }

    public override string ToString()
    {
      return String.Format("ResponseCode = {0}\r\nGSCCode = {1}\r\nReceiptID = {2}\r\nAmount = {3}\r\nTotal = {4}\r\nPAN = {5}", ResponseCode, GCSCode, ReceiptID,
        DataFormatter.FormatMoney(MoneyConverter.ToMoney(Amount)), DataFormatter.FormatMoney(MoneyConverter.ToMoney(Total)), PAN);
    }
  }

  internal sealed class Instance : Disposable
  {
    private int id;

    #region [ properties ]

    public int Id
    {
      get { return id; }
    }

    public bool IsValid
    {
      get { return (id > 0); }
    }

    #endregion

    public Instance()
    {
      id = EgateAPI.egInitInstance(Path.Combine(ApplicationServices.GetApplicationRootPath(), EgateAPI.EgateConfigPath));
    }

    #region [ Disposable ]

    protected override void DisposeManagedResources()
    {
      if (!IsValid)
        return;

      EgateAPI.egReleaseInstance(id);
      id = 0;
    }

    #endregion
  }

  internal class BankingDevice : Disposable
  {
    private readonly byte cashBoxUid;
    private bool keyAcqiured;
    private EquipmentState state;
    private TransactionResult lastTransactionResult;
    private CancelState cancel;
    private ReconcilationState reconcilation;
    private Timer timer;
    private Instance current;
    
    #region [ properties ]

    public bool Ready
    {
      get { return keyAcqiured; }
    }

    public EquipmentState State
    {
      get { return state; }
    }

    public TransactionResult LastTransactionResult
    {
      get { return lastTransactionResult; }
    }

    public ReconcilationState Reconcilation
    {
      get { return reconcilation; }
    }

    #endregion

    #region [ events ]

    public event Action<EquipmentState, string> StateChanged;
    public event Action<TransactionResult, AuthResponse> TransactionCompleted;
    public event Action<string, bool> Message;

    private void OnStateChanged(EquipmentState state, string message)
    {
      if (StateChanged != null)
        StateChanged(state, message);
    }

    private void OnTransactionCompleted(TransactionResult result, AuthResponse response)
    {
      if (TransactionCompleted != null)
        TransactionCompleted(result, response);
    }

    private void OnError(Instance instance)
    {
      int error = EgateAPI.egGetLastError(instance.Id);
      OnMessage(new StringBuilder().AppendFormat("Ошибка #{0}", error).AppendLineIfNotEmpty(EgateAPI.egGetErrorDescription(error).GetString()).ToString(), true);
    }

    private void OnMessage(string message, bool error)
    {
      if (Message != null)
        Message(message, error);
    }

    #endregion

    #region [ static ]

    public static string Version;

    static BankingDevice()
    {
      try
      {
        uint v = EgateAPI.egGetVersion();
        Version = String.Format("API v{0}.{1}.{2}.{3} ({4})", (int)(v / 1000000), (int)((v % 1000000) / 10000), (int)((v % 10000) / 100), v % 100,
          EgateAPI.egGetVersionDescription().GetString());
      }
      catch (Exception e)
      {
        Version = String.Concat("Не удалось определить версию", Environment.NewLine, e.Message);
      }
    }

    #endregion

    public BankingDevice(byte cashBoxUID)
    {
      cashBoxUid = cashBoxUID;
      keyAcqiured = false;
      state = EquipmentState.Idle;
      lastTransactionResult = TransactionResult.Unknown;
      cancel = CancelState.None;
      reconcilation = ReconcilationState.Idle;

      timer = new Timer(100);
      timer.Elapsed += ProcessTransaction;

      current = null;
    }

    #region [ Disposable ]

    protected override void DisposeManagedResources()
    {
      Stop();

      timer.Elapsed -= ProcessTransaction;
      timer.Dispose();
    }

    #endregion

    #region [ management ]

    public bool Start()
    {
      AcquireKey();

      return true;
    }

    public void Stop()
    {
      timer.Stop();
    }

    private Instance CreateInstance()
    {
      var v = new Instance();
      if (!v.IsValid)
        OnError(v);
      else
        OnMessage(String.Format("Получен экземпляр : {0}", v.Id), false);

      return v;
    }

    private void ReleaseInstance(Instance instance)
    {
      if ((instance == null) || (!instance.IsValid))
        return;

      int id = instance.Id;
      instance.Dispose();

      OnMessage(String.Format("Экземпляр освобожден : {0}", id), false);
    }

    private void AcquireKey()
    {
      if (Ready)
        return;

      Instance instance = null;
      try
      {
        instance = CreateInstance();
        string result = EgateAPI.egAuthRequest(instance.Id, EgateAPI.ProtocolId, CreateRequest(OperationType.RequestWorkKey).ToString()).GetString();
        if (!IsValidResult(result))
        {
          OnError(instance);
          return;
        }

        OnMessage(String.Format("Ответ на запрос ключа : {0}", result), false);

        result = EgateAPI.egGetAuthResult(instance.Id).GetString();

        if (!IsValidResult(result))
        {
          OnError(instance);
          return;
        }

        AuthResponse response = null;

        try
        {
          response = AuthResponse.Parse(result, null);
        }
        catch (Exception x)
        {
          OnMessage(x.Message, true);
        }

        if ((response == null) || !AuthResponse.IsValidCode(response.GCSCode))
          return;

        keyAcqiured = true;
        OnMessage(String.Format("Ключ получен успешно : {0}", response), false);
      }
      finally
      {
        ReleaseInstance(instance);
      }
    }

    #endregion

    #region [ transaction ]

    public bool BeginTransaction(int receiptId, int amount)
    {
      AcquireKey();
      if (!Ready)
      {
        OnMessage("Оборудование не готово", true);
        return false;
      }

      if (state != EquipmentState.Idle)
      {
        OnMessage("Транзакция уже выполняется", true);
        return false;
      }

      string request = CreateRequest(OperationType.Payment)
        .Append(EgateAPI.ParameterSeparator)
        .Append(amount)
        .Append(EgateAPI.ParameterSeparator)
        .Append(receiptId)
        .ToString();

      OnMessage(String.Format("Транзакция начинается : {0}", request), false);

      current = CreateInstance();
      string result = EgateAPI.egAuthRequestAsync(current.Id, EgateAPI.ProtocolId, request).GetString();

      if (!result.StartsWith(EgateAPI.AsyncRequestStartedString))
      {
        OnError(current);
        return false;
      }

      OnMessage(String.Format("Запрос отправлен : {0}", result), false);

      cancel = CancelState.None;
      state = EquipmentState.Started;
      OnStateChanged(state, String.Empty);
      timer.Start();

      return true;
    }

    private void ProcessTransaction(object sender, ElapsedEventArgs e)
    {
      string status = EgateAPI.egGetOpStatus(current.Id, false).GetString();
      if (status[0] == EgateAPI.StatusResult)
      {
        timer.Stop();
        OnMessage(String.Format("Запрос обработан : {0}", status.Substring(1)), false);

        AuthResponse response = null;
        try
        {
          byte[] ba;
          string result = EgateAPI.egGetAuthResult(current.Id).GetString(out ba);
          if (IsValidResult(result))
            lastTransactionResult = (cancel == CancelState.Completed) ? TransactionResult.Cancelled : TransactionResult.OK;
          else
          {
            lastTransactionResult = TransactionResult.Error;
            OnError(current);
          }

          response = AuthResponse.Parse(result, ba);
        }
        catch (Exception x)
        {
          OnMessage(x.Message, true);
        }
        finally
        {
          OnTransactionCompleted(lastTransactionResult, response);
          state = EquipmentState.CardTaken;
          OnStateChanged(state, status.Substring(1));
        }
      }
      else if (status[0] == EgateAPI.StatusBusy)
      {
        EquipmentState s = state;
        switch (status[1])
        {
          case EgateAPI.StatusWaitCard:
            //if (_state == EquipmentState.Started)
            s = EquipmentState.WaitingForCard;
            break;
          case EgateAPI.StatusCardEntered:
            //if (_state == EquipmentState.WaitingForCard)
            s = EquipmentState.CardInserted;
            break;
          case EgateAPI.StatusPinEntered:
            if (state == EquipmentState.CardInserted)
              s = EquipmentState.PinEntered;
            break;
          case EgateAPI.StatusAuthCompleted:
            if ((state != EquipmentState.Started) && (state != EquipmentState.WaitingForCard))
              s = EquipmentState.AuthCompleted;
            break;
          default:
            s = state;
            break;
        }

          if (state == s) return;
          state = s;
          OnStateChanged(state, status.Substring(2));
      }
    }

    public bool EndTransaction()
    {
      if (state == EquipmentState.Idle)
        return false;

      ReleaseInstance(current);

      lastTransactionResult = TransactionResult.Unknown;
      state = EquipmentState.Idle;
      OnStateChanged(state, String.Empty);
      OnMessage("Транзакция завершена", false);

      return true;
    }

    public bool CancelTransaction()
    {
      if (state == EquipmentState.Idle)
        return false;
      if (cancel != CancelState.None)
        return false;

      if (!ThreadPool.QueueUserWorkItem(ProcessCancelTransaction))
        return false;

      cancel = CancelState.Pending;
      OnMessage("Отмена транзакции начинается", false);

      return true;
    }

    private void ProcessCancelTransaction(object state)
    {
      string status = EgateAPI.egGetOpStatus(current.Id, true).GetString();
      cancel = (status[0] == EgateAPI.StatusResult) ? CancelState.Completed : CancelState.None;
      OnMessage(String.Format("Транзакция отменена : {0}", cancel), false);

      if (cancel == CancelState.Completed)
        SberBank.TransactionCancelled++;
    }

    #endregion

    #region [ reconcilation ]

    public bool BeginReconcilation()
    {
      if (!Ready)
      {
        OnMessage("Оборудование не готово", true);
        return false;
      }

      if ((state != EquipmentState.Idle) || (reconcilation != ReconcilationState.Idle))
        return false;

      if (!ThreadPool.QueueUserWorkItem(ProcessReconcilation))
        return false;

      reconcilation = ReconcilationState.Started;
      OnMessage("Сверка итогов начинается", false);

      return true;
    }

    private void ProcessReconcilation(object state)
    {
      var instance = CreateInstance();
      if (!instance.IsValid) return;

      string result = String.Empty;
      try
      {
        result = EgateAPI.egAuthRequest(instance.Id, EgateAPI.ProtocolId, CreateRequest(OperationType.TotalsReconcilation).Append(" 1 1").ToString()).GetString();
      }
      finally
      {
        ReleaseInstance(instance);
      }

      if (!IsValidResult(result))
        OnError(instance);

      reconcilation = ReconcilationState.Completed;
      OnMessage(String.Format("Результат сверки итогов : {0}", result), false);
    }

    public bool EndReconcilation()
    {
      if (reconcilation != ReconcilationState.Completed)
        return false;

      reconcilation = ReconcilationState.Idle;
      OnMessage("Сверка итогов завершена", false);

      keyAcqiured = false;
      AcquireKey();

      return true;
    }

    #endregion

    private StringBuilder CreateRequest(OperationType op)
    {
      return new StringBuilder()
        .Append(cashBoxUid)
        .Append(EgateAPI.ParameterSeparator)
        .Append((int)op);
    }

    private int GetStatusCode(string result)
    {
      int j = result.IndexOf(' ');
      if (j > 0)
        if (!Int32.TryParse(result.Substring(0, j), out j))
          j = -1;

      return j;
    }

    private bool IsValidResult(string result)
    {
      return !String.IsNullOrEmpty(result);
    }

    public override string ToString()
    {
      return Version;
    }
  }
}