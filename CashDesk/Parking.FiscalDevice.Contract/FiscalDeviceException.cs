using System;
using System.Text;

namespace Parking.FiscalDevice
{
  /// <summary>
  /// Исключения вызываемые фискальным регистратором
  /// </summary>
  public class FiscalDeviceException : Exception
  {
    private String _FDError;
    private String _FDErrorType;
    private String _FDErrorPrompt;

    #region [ static ]

    private static string[] _sErrType = 
             {"Прекратить работу с ККМ. Сообщить в ЦТО обстоятельства появления ошибки.",
             "Исправить ошибку формирования команды в компьютере.",
             "Изменить параметры команды или выполнить требуемую команду.",
             "Выполнение команды, которая вызвала эту ошибку, возможно только после обращения в ЦТО. Допускается выполнение незаблокированных команд.",
             "Ошибка не должна возникать. Прекратить работу с ККМ. Сообщить разработчикам (ЦТО) обстоятельства появления ошибки.",
             "Оператору проверить состояние принтера.",
             "Послать команду ещё раз."};

    private static string[] _sErrString =
             {"Ошибок нет. Счётчики обновлены.",
              "Неверный формат сообщения.",
              "Неверный формат поля.",
              "Неверные дата/время. Невозможно установить переданные дату/время.",
              "Неверная контрольная сумма (BCC).",
              "Неверный пароль передачи данных. Пароль по умолчанию \"AERF\".",
              "Нет команды с таким номером.",
              "Необходима команда \"Начало сеанса\".",
              "Время изменилось более чем на 24 часа.",
              "Превышена максимальная длина строкового поля.",
              "Превышена максимальная длина сообщения.",
              "Неправильная операция.",
              "Значение поля вне диапазона.",
              "При данном значении аргумента эта команда недопустима.",
              "Обязательное строковое поле имеет нулевую длину.",
              "Слишком большой результат.",
              "Переполнение денежного счётчика.",
              "Обратная операция невозможна из-за отсутствия прямой.",
              "Недостаточно наличных для выполнения операции.",
              "Обратная операция превысила итог по прямой операции.",
              "Необходимо выполнить сертификацию (ввод заводского номера).",
              "Необходимо выполнить Z-отчёт (закрытие смены).",
              "Таймаут при печати.",
              "Неисправимая ошибка принтера.",
              "Принтер не готов к печати.",
              "Мало бумаги.",
              "Необходимо провести Фискализацию.",
              "Неверный пароль доступа к фискальной памяти. Выполните команду, используя правильный пароль.",
              "ККМ уже сертифицирована.",
              "Исчерпано число фискализаций.",
              "Неверный буфер печати (для команды 70).",
              "Неверное G-поле (для команды 73).",
              "Неверный номер типа оплаты.",
              "Таймаут приёма.",
              "Ошибка приёма.",
              "Неверное состояние ККМ.",
              "Слишком много операций в документе. Необходима команда \"Аннулировать\".",
              "Необходима команда \"Открытие смены\".",
              "Нет такой ошибки.",
              "Неверный номер вида платежа.",
              "Неверное состояние принтера.",
              "Смена уже открыта.",
              "Нет такой ошибки.",
              "Неверная дата.",
              "Нет места для добавления отдела/составляющей (суммарное число >400).",
              "Индекс отдела/составляющей уже существует.",
              "Невозможно удаление отдела - есть составляющие.",
              "Индекс отдела/составляющей не обнаружен.",
              "Фискальная память неисправна.",
              "Дата последней существующей записи в фискальной памяти больше, чем дата операции, которую пытались выполнить.",
              "Необходима инициализация фискальной памяти.",
              "Заполнена вся фискальная память. Блокируются все команды, кроме снятия фискальных отчётов и формирования нефискальных документов.",
              "Некорректный стартовый символ на приёме.",
              "Неопознанный ответ от ЭКЛЗ.",
              "Неизвестная команда ЭКЛЗ.",
              "Неверное состояние ЭКЛЗ.",
              "Таймаут приёма от ЭКЛЗ.",
              "Таймаут передачи в ЭКЛЗ.",
              "Неверная контрольная сумма ответа ЭКЛЗ.",
              "Аварийное состояние ЭКЛЗ.",
              "Переполнение ЭКЛЗ.",
              "Неверная контрольная сумма в команде ЭКЛЗ.",
              "Контроллер ЭКЛЗ не обнаружен.",
              "Данные в ЭКЛЗ отсутствуют.",
              "Данные в ЭКЛЗ не синхронизированы.",
              "Аварийное состояние РИК.",
              "Неверные дата и время в команде ЭКЛЗ.",
              "Закончилось время эксплуатации ЭКЛЗ.",
              "Нет свободного места в ЭКЛЗ.",
              "Число активизаций исчерпано.",
              "Нет связи с ККМ."};

    #endregion

    #region [ properties ]

    /// <summary>
    /// Возвращает текст ошибки
    /// </summary>
    public String FDError
    {
      get { return _FDError; }
    }

    /// <summary>
    /// Возвращает текст типа ошибки
    /// </summary>
    public String FDErrorType
    {
      get { return _FDErrorType; }
    }

    /// <summary>
    /// Возвращает рекомендации по устранению ошибки
    /// </summary>
    public String FDErrorPrompt
    {
      get { return _FDErrorPrompt; }
    }

    #endregion

    /// <summary>
    /// Создает исключение по номеру ошибки фискального регистратора
    /// </summary>
    /// <param name="nErr">Номер ошибки</param>
    public FiscalDeviceException(int nErr)
      : base("Ошибка при работе с фискальным регистратором.")
    {
      GetErrorString(nErr, out _FDError, out _FDErrorType, out _FDErrorPrompt);
    }

    /// <summary>
    /// Создает исключение по номеру ошибки фискального регистратора
    /// </summary>
    /// <param name="nErr">Номер ошибки</param>
    /// <param name="Msg">Дополнительный текст описания ошибки</param>
    public FiscalDeviceException(int nErr, String Msg)
      : base(Msg)
    {
      GetErrorString(nErr, out _FDError, out _FDErrorType, out _FDErrorPrompt);
    }

    /// <summary>
    /// Создает исключение по номеру ошибки фискального регистратора
    /// </summary>
    /// <param name="err">Ошибка</param>
    /// <param name="InnerException">Изначальное исключение</param>
    public FiscalDeviceException(String Msg, FiscalDeviceException InnerException)
      : base(Msg, InnerException)
    {
      _FDError = InnerException.FDError;
      _FDErrorType = InnerException.FDErrorType;
      _FDErrorPrompt = InnerException.FDErrorPrompt;
    }

    /// <summary>
    /// Создает исключение фискального регистратора с текстовым сообщением
    /// </summary>
    /// <param name="Message">Текстовое сообщение</param>
    public FiscalDeviceException(String Message)
      : base(Message)
    {
      _FDError = String.Empty;
      _FDErrorType = String.Empty;
      _FDErrorPrompt = String.Empty;
    }

    /// <summary>
    /// Возвращает ошибку, тип ошибки и рекомендации по устранению в текстовом виде
    /// </summary>
    /// <param name="nErr">Код ошибки</param>
    /// <param name="sErrName">Текст ошибки</param>
    /// <param name="sErrType">Тип ошибки</param>
    /// <param name="sErrExt">Рекомендации по устранению</param>
    private void GetErrorString(int nErr, out String sErrName, out String sErrType, out String sErrExt)
    {
      int bbError = nErr & 0xFF;
      int bbErrorEx = (nErr >> 8) & 0xFF;
      int bbErrorType = 0;

      // Текст ошибки
      sErrName = String.Empty;
      if (bbError < _sErrString.Length)
        sErrName = _sErrString[bbError];

      switch (bbError)
      {
        case 0:
          sErrType = "Ошибки нет";
          bbErrorType = -1;
          break;
        case 1:
        case 2:
        case 5:
        case 6:
        case 9:
        case 10:
        case 12:
        case 14:
          sErrType = _sErrType[1];
          bbErrorType = 1;
          break;
        case 3:
        case 7:
        case 11:
        case 13:
        case 16:
        case 17:
        case 18:
        case 19:
        case 21:
        case 26:
        case 27:
        case 30:
        case 31:
        case 32:
        case 36:
        case 37:
        case 39:
        case 41:
        case 43:
        case 44:
        case 45:
        case 46:
        case 47:
          sErrType = _sErrType[2];
          bbErrorType = 2;
          break;
        case 4:
        case 8:
        case 33:
        case 34:
          sErrType = _sErrType[6];
          bbErrorType = 6;
          break;
        case 15:
        case 20:
        case 28:
        case 50:
          sErrType = _sErrType[4];
          bbErrorType = 4;
          break;
        case 22:
        case 23:
        case 24:
        case 25:
        case 40:
          sErrType = _sErrType[5];
          bbErrorType = 5;
          break;
        case 29:
        case 51:
          sErrType = _sErrType[3];
          bbErrorType = 3;
          break;
        case 35:
        case 48:
        case 52:
        case 53:
        case 54:
        case 55:
        case 56:
        case 57:
        case 58:
        case 59:
        case 60:
        case 61:
        case 62:
        case 63:
        case 64:
        case 65:
        case 66:
        case 67:
        case 68:
        case 69:
          sErrType = _sErrType[0];
          bbErrorType = 0;
          break;
        default:
          sErrType = "Неизвестен!";
          bbErrorType = 0xFF;
          break;
      }

      //Текст расширеной ошибки

      sErrExt = String.Empty;
      switch (bbError)
      {
        case 0:
          if (bbErrorEx == 1)
            sErrExt = "Произошла ошибка печати документа.";

          break;
        case 2:
        case 9:
        case 12:
        case 14:
          sErrExt = "№ поля: " + Convert.ToString(bbErrorEx);
          break;
        case 16:
          switch (bbErrorEx)
          {
            case 0xFF:
              sErrExt = "Счётчика товара на чеке.";
              break;
            case 0xFE:
              sErrExt = "Итог чека.";
              break;
            case 0xFD:
              sErrExt = "Дневного денежного счётчика по операциям.";
              break;
            case 0xFC:
              sErrExt = "Наличных в кассе.";
              break;
            case 0xFB:
              sErrExt = "Нарастающего итога.";
              break;
            case 0xFA:
              sErrExt = "Вычисленный процент скидки/наценки превышает 999.99%";
              break;
          }
          break;
        case 48:
          sErrExt = "№ неисправности: " + Convert.ToString(bbErrorEx);
          break;
      }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(base.Message);
      if (!String.IsNullOrEmpty(_FDError))
      {
        sb.AppendLine();
        sb.Append(_FDError);
      }
      if (!String.IsNullOrEmpty(_FDErrorType))
      {
        sb.AppendLine();
        sb.Append(_FDErrorType);
      }
      if (!String.IsNullOrEmpty(_FDErrorPrompt))
      {
        sb.AppendLine();
        sb.Append(_FDErrorPrompt);
      }

      return sb.ToString();
    }
  }
}
