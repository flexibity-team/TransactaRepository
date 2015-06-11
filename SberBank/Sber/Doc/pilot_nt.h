#ifndef _PILOT_NT
#define _PILOT_NT
/***********************************************************

  PILOT_NT
  -------
  Библиотека для работы с картами СБЕРКАРТ и международными
  картами с магнитной полосой (Visa/Maestro/MasterCard/Amex/DinersClub).

  Платформа:                             Win32.

************************************************************/

typedef unsigned long  DWORD;
typedef unsigned short USHORT;

//Типы операций
#define OP_PURCHASE     1   //Оплата покупки
#define OP_RETURN       3   //Возврат либо отмена покупки
#define OP_FUNDS        6   //Безнал.перевод

#define OP_PREAUTH     51   //Предавторизация
#define OP_COMPLETION  52   //Завершение расчета
#define OP_CASHIN      53   //Взнос наличных
#define OP_CASHIN_COMP 54   //Подтверждение взноса


//Типы карт
#define CT_USER     0  //Выбор из меню

#define CT_VISA     1  //Visa
#define CT_EUROCARD 2  //Eurocard/Mastercard
#define CT_CIRRUS   3  //Cirrus/Maestro
#define CT_AMEX     4  //Amex
#define CT_DINERS   5  //DinersCLub
#define CT_ELECTRON 6  //VisaElectron
#define CT_PRO100   7  //PRO100
#define CT_CASHIER  8  //Cashier card
#define CT_SBERCARD 9  //Sbercard

//Структура, используемая для описания транзакции
// и возврата результатов
#pragma pack(1)
struct auth_answer{
   int TType;             //вход: тип транзакции
   unsigned long Amount;  //вход: сумма
   char RCode[3];         //выход: код результата авторизации
   char AMessage[16];     //выход: словесное пояснение результата
   int  CType;            //вход/выход: тип карты
   char* Check;           //выход: образ чека, должен освобождаться
                          // GlobalFree в вызывающей программе
};

//Расширенная структура
struct auth_answer2{
   struct auth_answer auth_answ;
   char AuthCode[7];      //выход: код авторизации
};

//Еще одна расширенная структура
struct auth_answer3{
  struct auth_answer auth_answ;
  char AuthCode[7]; //выход: код авторизации
  char CardID[25];  //выход: идентификатор карты
};

//Еще более расширенная структура
struct auth_answer4{
  struct auth_answer auth_answ;
  char AuthCode[7];   //выход: код авторизации
  char CardID[25];    //выход: идентификатор карты
  int  ErrorCode;     //выход: код ошибки
  char TransDate[20]; //выход: дата и время операции
  int  TransNumber;   //выход: номер операции
};

//Еще более расширенная структура
struct auth_answer5{
  struct auth_answer auth_answ;
  char   RRN[13];
  char   AuthCode[7];  // added after communication with HRS
};

//Еще более расширенная структура
struct auth_answer6{
  struct auth_answer auth_answ;
  char   AuthCode[7];   //выход: код авторизации
  char   CardID[25];    //выход: идентификатор (номер) карты
  int    ErrorCode;     //выход: подробный код ошибки
  char   TransDate[20]; //выход: дата и время операции
  int    TransNumber;   //выход: номер операции за день
  char   RRN[13];       //вход/выход: ссылочный номер предавторизации
};

//Еще более расширенная структура
struct auth_answer7{
  struct auth_answer auth_answ;
  char   AuthCode[7];   //выход: код авторизации
  char   CardID [25];   //выход: идентификатор (номер) карты
  int    SberOwnCard;   //выход: флаг принадлежности карты Сбербанку
};

//Еще более расширенная структура
#define MAX_ENCR_DATA  32
struct auth_answer8{
  struct auth_answer auth_answ;
  char   AuthCode[7];      //выход: код авторизации
  char   CardID[25];       //выход: идентификатор (номер) карты
  int    ErrorCode;        //выход: подробный код ошибки
  char   TransDate[20];    //выход: дата и время операции
  int    TransNumber;      //выход: номер операции за день
  char   RRN[13];          //вход/выход: ссылочный номер предавторизации
  char   EncryptedData[MAX_ENCR_DATA*2+1];//вход/выход: шифрованый номер карты и срок действия
};

struct preauth_rec{
  unsigned long  Amount;          //вход: сумма предавторизации в копейках
  char           RRN[13];         //вход: ссылочный номер предавторизации
  char           Last4Digits[5];  //вход: последние 4 цифры номера карты
  unsigned short ErrorCode;       //выход: код завершения: 0 - успешно.       
};

struct auth_answer9{
  auth_answer   ans;           //вход/выход: основные параметры операции (см.выше)
  char          AuthCode[7];   //выход: код авторизации
  char          CardID [25];   //выход: номер карты
  int           SberOwnCard;   //выход: флаг принадлежности карты Сбербанку 
  char          Hash[41];      //выход: хеш от номера карты, в формате ASCII с нулевым байтом в конце 
};

struct auth_answer10{
  auth_answer   ans;           //вход/выход: основные параметры операции (см.выше)
  char   AuthCode[7];   //выход: код авторизации
  char   CardID[25];    //выход: идентификатор карты
  int    ErrorCode;     //выход: код ошибки
  char   TransDate[20]; //выход: дата и время операции
  int    TransNumber;   //выход: номер операции

  int    SberOwnCard;   //выход: флаг принадлежности карты Сбербанку 
  char   Hash[41];      //выход: хеш от номера карты, в формате ASCII с нулевым байтом в конце 
};

struct auth_answer11{
  auth_answer   ans;           //вход/выход: основные параметры операции (см.выше)
  char   AuthCode[7];   //выход: код авторизации
  char   CardID[25];    //выход: идентификатор карты
  int    ErrorCode;     //выход: код ошибки
  char   TransDate[20]; //выход: дата и время операции
  int    TransNumber;   //выход: номер операции

  int    SberOwnCard;   //выход: флаг принадлежности карты Сбербанку 
  char   Hash[41];      //выход: хеш от номера карты, в формате ASCII с нулевым байтом в конце 
  char   Track3[108];   //выход: третья дорожка карты
};


#pragma pack()

#ifdef __cplusplus
extern "C"{
#endif

//Выполнение операций по картам.
//track2 - данные дорожки карты с магнитной полосой. Если NULL, то
//будет предложено считать карту
//auth_answer - см. описание полей структуры
//
 __declspec(dllexport) int card_authorize(char *track2,
			     struct auth_answer *auth_answer);


//Закрытие дня. Поля TType,Amount,CType заполнять не нужно.
//
__declspec(dllexport) int close_day(struct auth_answer *auth_answer);


//Получение текущего отчета. При значении поля TType = 0 формируется
//краткий отчет, иначе - полный
//
__declspec(dllexport) int get_statistics(struct auth_answer *auth_answer);


//Выполнение операций по картам с возвратом дополнительных данных.
//  track2 - данные дорожки карты с магнитной полосой. Если NULL, то
//  будет предложено считать карту
//
//  auth_answer2...auth_answer7 - см. описание полей структуры
//
__declspec(dllexport) int  card_authorize2(char *track2,
                                   		     struct auth_answer2 *auth_answer);

__declspec(dllexport) int  card_authorize3(char *track2,
                                			     struct auth_answer3 *auth_answer);

__declspec(dllexport) int  card_authorize4(char *track2,
                                			     struct auth_answer4 *auth_answer);

__declspec(dllexport) int  card_authorize5(char *track2,
                                           struct auth_answer5 *auth_answer);

__declspec(dllexport) int  card_authorize6(char *track2,
                                           struct auth_answer6 *auth_answer);

__declspec(dllexport) int  card_authorize7(char *track2,
    		                           struct auth_answer7 *auth_answer);

__declspec(dllexport) int  card_authorize8(char *track2,
    		                           struct auth_answer8 *auth_answer);

__declspec(dllexport) int card_complete_multi_auth8(char* track2,
                                                    struct auth_answer8* auth_ans,
                                                    struct preauth_rec*  pPreAuthList,
                                                    int NumAuths);

//Получение номера версии
//
__declspec(dllexport) unsigned int GetVer();

//Деинициализация
//
__declspec(dllexport) void Done();



//Чтение карты (возвращаются 4 последние цифры и хеш от номера карты)
//
__declspec(dllexport) int ReadCard  (char *Last4Digits, char *Hash);
__declspec(dllexport) int ReadCardSB(char *Last4Digits, char *Hash);



//Чтение карты (возвращется полный номер и срок действия карты в формате YYMM)
//
//  Номер может иметь длину от 13 до 19 цифр.
//  Чтобы потом использовать эти данные для авторизации, их нужно будет
//  сформатировать так:
//
//    sprintf(track2,"%s=%s", CardNo, ValidThru)      
//
__declspec(dllexport) int ReadCardFull(char *CardNo, char *ValidThru);



//Чтение полной второй дорожки карты
//
//  Данные второй дорожки могут иметь длину до 40 символов.
//  Вторая дорожка имеет формат:
//
//    nnnn...nn=yymmddd...d
//
//  где     '=' - символ-разделитель
//      nnn...n - номер карты
//      yymm    - срок действия карты (ГГММ)
//      ddd...d - служебные данные карты
//
__declspec(dllexport) int ReadTrack2(char *Track2);



//Чтение карты Сберкарт (возвращется полный номер карты и фамилия клиента)
//
__declspec(dllexport) int ReadSbercard(char *CardNo, char* ClientName);


//Чтение карты асинхронное
//
__declspec(dllexport)int EnableReader (HWND hDestWindow, UINT message);
__declspec(dllexport)int DisableReader();

//Проверка готовности пинпада
//
__declspec(dllexport) int TestPinpad();

// Перевод последней транзакции в "подвешенное" состояние
//
__declspec(dllexport) int SuspendTrx (DWORD dwAmount, char* pAuthCode);

// Возвращение последней транзакции в "успешное" состояние
//
__declspec(dllexport) int CommitTrx  (DWORD dwAmount, char* pAuthCode);

// Форсированная отмена последней транзакции
//
__declspec(dllexport) int RollbackTrx(DWORD dwAmount, char* pAuthCode);

// Получить номер терминала
//
__declspec(dllexport) int GetTerminalID(char* pTerminalID);


// Войти в техническое меню.
// При выходе поле Check может содержать образ документа для печати.
//
__declspec(dllexport) int ServiceMenu(struct auth_answer *auth_answer);

// Установить хендлы для вывода на экран
//
__declspec(dllexport) int SetGUIHandles(int hText, int hEdit);


__declspec(dllexport) int  card_authorize9(char *track2,
    		                           struct auth_answer9 *auth_answer);

__declspec(dllexport) int  card_authorize10(char *track2,
    		                           struct auth_answer10 *auth_answer);

__declspec(dllexport) int  card_authorize11(char *track2,
    		                           struct auth_answer11 *auth_answer);

__declspec(dllexport) int ReadCardTrack3(char *Last4Digits, char *Hash, char* pTrack3);

__declspec(dllexport) int AbortTransaction();

#ifdef __cplusplus
};
#endif

#endif
