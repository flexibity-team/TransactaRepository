using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTerminal
{
    public class PilotNt
    {
        #region Constants
        //Типы операций
        private const int OpPurchase = 1;   //Оплата покупки
        private const int OpReturn = 3;   //Возврат либо отмена покупки
        private const int OpFunds = 6;   //Безнал.перевод

        private const int OpPreauth = 51;   //Предавторизация
        private const int OpCompletion = 52;   //Завершение расчета
        private const int OpCashin = 53;   //Взнос наличных
        private const int OpCashinComp = 54;   //Подтверждение взноса

        //Типы карт
        private const int CtUser = 0;  //Выбор из меню
        private const int CtVisa = 1;  //Visa
        private const int CtEurocard = 2;  //Eurocard/Mastercard
        private const int CtCirrus = 3;  //Cirrus/Maestro
        private const int CtAmex = 4;  //Amex
        private const int CtDiners = 5;  //DinersCLub
        private const int CtElectron = 6;  //VisaElectron
        private const int CtSbercard = 9;  //Sbercard

        #endregion

#region Structures
//Структура, используемая для описания транзакции
// и возврата результатов
        struct AuthAnswer
        {
            int transactionType;             //вход: тип транзакции
            ulong amount;                //вход: сумма
            string resultCode;         //выход: код результата авторизации
            string resultMessage;     //выход: словесное пояснение результата
            int cardType;            //вход/выход: тип карты
            IntPtr receipt;           //выход: образ чека, должен освобождаться GlobalFree в вызывающей программе
        };
        //Расширенная структура

        struct AuthAnswerExtention
        {
            AuthAnswer authAnswer;
            string   authCode;   //выход: код авторизации
            string   CardId;    //выход: идентификатор (номер) карты
            int    errorCode;     //выход: подробный код ошибки
            string   transactionDateTime; //выход: дата и время операции
            int    transactionNumber;   //выход: номер операции за день
            string   referenceNumber;       //вход/выход: ссылочный номер предавторизации
        };
#endregion
       
    }
}
