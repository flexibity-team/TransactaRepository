using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using Parking.Data;

namespace Parking.FiscalDevice
{
    using Data;
    using RMLib;

    /// <summary>
    /// Обслуживание ККМ "Старт 200 К" и аналогичных. Используется библиотека производителя Azimuth.dll
    /// </summary>
    public class Start200Control : Disposable, IFiscalDevice
    {
        #region Инициализация
        private string cashierName;
//        public Start200Control()
        /// <summary>
        ///  Обслуживание ККМ "Старт 200 К" 
        /// </summary>
        public Start200Control()
        {
            cashierName = String.Empty;
        }
        #endregion

        #region  Constants

        private const string Start200Path = @".\Library\Azimuth.dll";
        private const string PswForKkm = "AERF";
        #endregion

        #region  Imports DLL

        /// <summary>
        /// Инициализация DLL
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        private static extern int OpenDLL(string asOper, string asPassw, string asPort, bool ascii);

        /// <summary>
        /// Ведение лог-файла обмена с ККМ  
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        private static extern void LogEnable(byte logOn);

        /// <summary>
        /// Открытие сессии с ККМ  
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        private static extern int StartSeans();

        /// <summary>
        /// Освобождение ККМ
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        private static extern int CloseDLL();

        /// <summary>
        /// Открыть смену
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        private static extern int ShiftOpen(string buf);

        /// <summary>
        /// Закрыть смену и печатать Z-Отчет
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        private static extern int ShiftClose();

        /// <summary>
        /// Задать имя кассира
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern int ChangeOpName(String name);

        /// <summary>
        /// Код ошибки последней операции
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern Int64 GetLastDllError();


        /// <summary>
        /// Печать X отчета
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern int XReport();

        /// <summary>
        /// Электронный отчёт (для получения cуммы денег в кассе)
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern int GetEReport(byte com, byte param);

        /// <summary>
        /// Получение поля суммы в кассе из электронного отчёта
        ///  </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        private static extern double GetFldFloat(byte fieldNum);

        /// <summary>
        /// Получение поля состояния смены (открыта/закрыта)
        ///  </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        private static extern int GetFldWord(byte fieldNum);

        /// <summary>
        /// Внесение денег в кассу
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern int ToCash(int amount);

        /// <summary>
        /// Инкассация денег
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern int FromCash(int amount);


        /// <summary>
        /// Открыть формирование чека (ПФД)
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern int OpenFiscalDoc(         //ККМ-код 0x71, 0x73
            byte docType,       //тип документа
            byte payType,       //вид оплаты
            byte flipFOffs,     //направление печати
            byte pageNum,       //количество копий документа
            byte hCopyNum,      //количество  копий  документа  по горизонтали
            byte vCopyNum,      //количество  копий  документа  по вертикали
            int lOffs,          //смещение копии по горизонтали
            int vGap,           //смещение копии по вертикали
            byte lGap,          //смещение между строками
            int sum             //сумма документа в копейках
            );

        /// <summary>
        /// Добавить строку к чеку
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern void AddPosField(             //ККМ-код 0x71, 0x73
            int lineSerNo,          // номер строки 
            int columnSerNo,        // позиция в строке
            byte fontSerNo,         // вид шрифта
            int lineDocNo,          // номер строки 
            int columnDocNo,        // позиция в строке
            byte fontDocNo,         // вид шрифта
            int lineDate,          // номер строки 
            int columnDate,        // позиция в строке
            byte fontDate,         // вид шрифта
            int lineTime,          // номер строки 
            int columnTime,        // позиция в строке
            byte fontTime,         // вид шрифта
            int lineInn,          // номер строки 
            int columnInn,        // позиция в строке
            byte fontInn,         // вид шрифта
            int lineOper,          // номер строки 
            int columnOper,        // позиция в строке
            byte fontOper,         // вид шрифта
            int lineSum,          // номер строки 
            int columnSum,        // позиция в строке
            byte fontSum         // вид шрифта
            );

        /// <summary>
        /// Добавить строку к чеку
        /// </summary>
        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern void AddFreeField(             //ККМ-код 0x71, 0x73
            int line,          // номер строки 
            int column,        // позиция в строке
            byte font,         // вид шрифта
            byte printMode,    // печать на копии (01 - на оснвном документе, 02 -на копии, 03 - на обоих
            byte jourNo,       // номер  вывода  на  контрольную ленту
            String sText       // собственно строка 
            );

        [DllImport(Start200Path, CharSet = CharSet.Ansi)]
        public static extern int PrintFiscalReceipt();             //ККМ-код 0x73
 
        #endregion

        #region Реализация IFiscalDevice 

        /// <summary>
        /// Производит подключение к фискальному регистратору
        /// </summary>
        public void Initialize(int comPort, string name)
        {
            cashierName = name;
            string port = "COM" + comPort;
            var error = OpenDLL(cashierName, PswForKkm, port, false);
            if (error != 0)
            {
                throw new FiscalDeviceException(error, "Ошибка инициализации ККМ.");
            }
            else
            {
                error = StartSeans();
                if (error != 0)
                    throw new FiscalDeviceException(error, "Ошибка подключения к ККМ.");
            }
        }

        public void LogFileOn(bool onOff)
        {
            LogEnable((byte)(onOff ? 1 : 0));
        }

        public void OpenSession()
       {
            StartSeans();
            int error = ShiftOpen(String.Empty);
            if (error != 0x29 && error != 0)
                throw new FiscalDeviceException("Ошибка открытия смены");
        }

   /// <summary>
    /// Закрывает смену
    /// </summary>
        public void CloseSession()
   {
       StartSeans();
       int error = ShiftClose();
       if (error != 0)
           throw new FiscalDeviceException("Ошибка закрытия смены");
   }
         /// <summary>
        /// Возвращает состояние смены (открыта/закрыта)
        /// </summary>
        public bool IsSessionOpened
        {
            get
            {
                StartSeans();
                var a = BitConverter.GetBytes(GetFldWord(2))[1] & 0x08 ;
                return a != 0;
            }
        }


    /// <summary>
    /// Возвращает количество денег в кассе
    /// </summary>
    public double GetKKMAmount()
    {
        StartSeans();
        int error = GetEReport(0x34, 0);
        if (error != 0)
            throw new FiscalDeviceException(error, "Ошибка запроса суммы в кассе");

        return GetFldFloat(23); 
    }

    /// <summary>
    /// Возвращает выручку за смену 
    /// </summary>
    public double GetSessionAmount()
    {
        return GetKKMAmount();
    }

    /// <summary>
    /// Печатает X-отчет
    /// </summary>
    public void PrintXReport()
    {
        StartSeans(); // Открыть сеанс
        int nErr = XReport();
        if (nErr != 0)
            throw new FiscalDeviceException(nErr, "Ошибка печати X-отчета");
    }

    /// <summary>
    /// Положить деньги в кассу (например, для размена)
    /// </summary>
    public void CashIn(double amount)
    {
        StartSeans();
        int nErr = ToCash((int)amount*100);
        if (nErr != 0)
            throw new FiscalDeviceException(nErr, "Ошибка внесения денег.");
    }

    /// <summary>
    /// Забрать деньги из кассы (инкассация)
    /// </summary>
    public void CashOut(double amount)
    {
        StartSeans();
        int nErr = FromCash((int)amount*100);
        if (nErr != 0)
            throw new FiscalDeviceException(nErr, "Ошибка инкассации.");
    }

        #endregion

        /// <summary>
        /// Оплата
        /// </summary>
        public void Payment(PaymentDocument doc)
        {
            int nError;
            int row = 0;
            // проверим смену
            if (!IsSessionOpened)
                 throw new FiscalDeviceException("Для проведения оплаты необходимо предварительно открыть смену!");


            //Формирование чека

            nError = OpenFiscalDoc((byte)doc.Type, (byte)doc.PaymentType, 0, 1, 1, 2, 32, 3, 24, (int)doc.Amount*100);
            if (nError != 0)
                throw new FiscalDeviceException(nError, "Ошибка формирования чека.");

           ChangeOpName(cashierName);

            String sCardNum = Convert.ToString(doc.CardID, 16);

            switch (doc.PaymentReason)
            {
                case PaymentReason.Parking:
                    AddFreeField(4, 1, 1, 1, 0, "Оплата услуг парковки");
                    AddFreeField(5, 1, 1, 1, 0, "Парковочная карта: " + sCardNum.ToUpper());
                    AddFreeField(6, 1, 1, 1, 0, "Время въезда: " + String.Format("{0:G}", doc.TimeEntry));
                    AddFreeField(7, 1, 1, 1, 0, "Выезд до :    " + String.Format("{0:G}", doc.TimeExit));
                    AddFreeField(8, 1, 1, 1, 0, "Оплачено: " + Utils.TimeToString(doc.TimeExit - doc.TimeEntry));
                    if (doc.Payment <= 0) AddFreeField(9, 1, 1, 1, 0, "                 ");
                    else AddFreeField(9, 1, 1, 1, 0, "Оплачено ранее: " + String.Format("{0:C}", doc.Payment));
                    row = 6;
                    break;
                case PaymentReason.Fine:
                    AddFreeField(4, 1, 1, 1, 0, "Оплата услуг парковки - ШТРАФ");
                    AddFreeField(5, 1, 1, 1, 0, "Парковочная карта: " + sCardNum.ToUpper());
                    AddFreeField(6, 1, 1, 1, 0, "Время въезда: " + String.Format("{0:G}", doc.TimeEntry));
                    AddFreeField(7, 1, 1, 1, 0, "Выезд до :    " + String.Format("{0:G}", doc.TimeExit));
                    AddFreeField(8, 1, 1, 1, 0, "Оплачено: " + Utils.TimeToString(doc.TimeExit - doc.TimeEntry));
                    if (doc.Payment / 100 <= 0) AddFreeField(9, 1, 1, 1, 0, "                 ");
                    else AddFreeField(9, 1, 1, 1, 0, "Оплачено ранее: " + String.Format("{0:C}", doc.Payment));
                    row = 6;
                    break;
                case PaymentReason.ECash:
                    AddFreeField(4, 1, 1, 1, 0, "Пополнение эл. кошелька");
                    AddFreeField(5, 1, 1, 1, 0, "Парковочная карта: " + sCardNum.ToUpper());
                    row = 2;
                    break;
                case PaymentReason.Any:
                    break;
                case PaymentReason.Refund:
                    AddFreeField(4, 1, 1, 1, 0, "Возврат");
                    row = 1;
                    break;
            }

            AddPosField(1, 12, 1, 1, 34, 1, 2, 2, 1, 2, 34, 1, 5+row, 26, 1, 3, 9, 1, 4+row, 9, 1);

            AddFreeField(1, 1, 1, 1, 0, "Серийный № ");
            AddFreeField(1, 24, 1, 1, 0, "Док № ");
            AddFreeField(3, 1, 1, 1, 0, "Касса № ");
            AddFreeField(5 + row, 16, 1, 1, 0, "ИНН: ");
            AddFreeField(4 + row, 1, 1, 1, 0, "ИТОГО: ");

            nError = PrintFiscalReceipt();
            if (nError != 0)
                throw new FiscalDeviceException(nError, "Ошибка печати чека. Проверьте ККМ и повторите операцию.");
        }

        #region [ Disposable ]

        /// <summary>
        /// Закрытие DLL
        /// </summary>
        protected override void DisposeManagedResources()
        {
            CloseDLL();
        }

        #endregion
        public override string ToString()
        {
            return "Старт-200К";
        }


    }
}
