#ifndef _UMAINDEF_H
#define _UMAINDEF_H

#define LOG

#define OK                 0
#define NO_COM             80
#define NO_DRV             81
#define NO_ID              82
#define NO_DRV_SIGN        83
#define ERR_RTIMEOUT       84
#define INVALID_COMMAND    85
#define NO_MEMORY          86
#define INIT_REQUIRE       87
#define ALLREADY_INIT      88
#define BAD_CONNECT        89
#define NOT_OPEN           90
#define ALLREADY_OPEN      91
#define FIELD_EMPTY        92
#define ERR_STIMEOUT       93

#define IER       1               /* регистр разрешения прерываний*/
#define BUD       1               /* регистр скорости*/
#define Ierrx     00000001b       /* доступны данные приема */
#define Iertx     00000010b       /* регистр передатчика пуст */
#define Ierls     00000100b       /* состояние линии */
#define IIR       2               /* регистр идентификации прерывания */
#define Iirpend   00000001b       /* = 0 - запрос прерывания */
#define LCR       3               /* регистр управления линией */
#define Data8     00000011b       /* 8-битовые данные */
#define Dlab      10000000b       /* бит доступа к делителю частоты */
#define MCR       4               /* регистр управления модемом */
#define Mcrdtr    00000001b       /* данные готовы */
#define Mcrrts    00000010b       /* запрос на передачу */
#define Mcrout2   00001000b       /* вывод OUT2 (строб прерывания) */
#define LSR       5               /* регистр состояния линии */
#define Lsrthre   00100000b       /* регистр передатчика пуст */
#define Lsrtsre   01000000b       /* сдвиговый регистр передатчика пуст */

#define MSR       6               /* регистр состояния модема */
#define Msrcts    00010000b       /* состояние CTS */
#define Msrdsr    00100000b       /* данные готовы */
#define Msrrlsd   10000000b       /* принят сигнал несущей частоты */
#define SCR       7               /* регистр FIFO */

#define CM_Start                     0x01
#define CM_StartShift                0x02
#define CM_Resources                 0x03
#define CM_Fiscalize                 0x04
#define CM_FDateRep                  0x05
#define CM_FNoRep                    0x06
#define CM_FDateRepFull              0x07
#define CM_FNoRepFull                0x08


#define CM_ReceiptStart              0x10
#define CM_ReceiptItem               0x11
#define CM_ReceiptTotal              0x12
#define CM_ReceiptTender             0x13
#define CM_ReceiptClose              0x14
#define CM_ReceiptComission          0x15
#define CM_ReceiptSubtotal           0x16
#define CM_ReceiptCancel             0x17

#define CM_SlipStart                 0x20
#define CM_SlipItem                  0x21
#define CM_SlipTotal                 0x22
#define CM_SlipTender                0x23
#define CM_SlipClose                 0x24
#define CM_SlipComission             0x25
#define CM_SlipSubtotal              0x26
#define CM_SlipCancel                0x27

#define CM_XReport                   0x30
#define CM_ZReport                   0x31
#define CM_ToCash                    0x32
#define CM_FromCash                  0x33
#define CM_Report2Host               0x34
#define CM_No2Host                   0x35
#define CM_Report2HostAnyPay         0x36
#define CM_R2HTotal                  0x37

#define CM_SetPassword               0x40
#define CM_SetHeader                 0x41
#define CM_SetDateTime               0x42
#define CM_GetDateTime               0x43
#define CM_SetDrawerTime             0x44
#define CM_GetDrawerTime             0x45
#define CM_SetTail                   0x46
#define CM_SetOperationNames         0x47

#define CM_SetPayment                0x4A
#define CM_GetPayment                0x4B
#define CM_SetDocSettings            0x4C
#define CM_GetDocSettings            0x4D
#define CM_SetHeaderNew              0x4E

#define CM_ReceiptStartUserDoc       0x50
#define CM_ReceiptUserDocLine        0x51
#define CM_ReceiptCloseUserDoc       0x52
#define CM_SlipStartUserDoc          0x53
#define CM_SlipUserDocLine           0x54
#define CM_SlipCloseUserDoc          0x55
#define CM_ReceiptUserDocLines       0x56
#define CM_SlipUserDocLines          0x57

#define CM_SetDepart                 0x60
#define CM_GetDepart                 0x61
#define CM_SetArticl                 0x63
#define CM_GetArticl                 0x64

#define CM_FreeDocNF                 0x70
#define CM_FreeDoc                   0x71
#define CM_PrintEJournal             0x72
#define CM_FreeReceipt               0x73
#define CM_DepDocSlip                0x74
#define CM_DepDocReceipt             0x75

#define CM_WriteUserBytes            0x81
#define CM_ReadUserBytes             0x82
#define CM_GetHardwareStatus         0x83

/* technical*/
#define CM_SetCurrency               0x90
#define CM_GetCurrency               0x91
#define CM_Sertification             0x92
#define CM_StartMonitor              0x93
#define CM_Setup                     0x94
#define CM_GetSetupInfo              0x95
#define CM_GetSerialNo               0x96

typedef unsigned char CHAR;
typedef unsigned char BYTE;
typedef unsigned char BOOLEAN;
typedef unsigned int  WORD;
typedef unsigned long DWORD;

typedef CHAR          pBUF[1024+1];
typedef CHAR          STRING[255+1];
typedef CHAR          STRING2[2+1];
typedef CHAR          STRING3[3+1];
typedef CHAR          STRING4[4+1];
typedef CHAR          STRING6[6+1];
typedef CHAR          STRING10[10+1];
typedef CHAR          STRING11[11+1];
typedef CHAR          STRING12[12+1];
typedef CHAR          STRING15[15+1];
typedef CHAR          STRING16[16+1];
typedef CHAR          STRING19[19+1];
typedef CHAR          STRING20[20+1];
typedef CHAR          STRING30[30+1];
typedef CHAR          STRING34[34+1];
typedef CHAR          STRING38[38+1];
typedef CHAR          STRING40[40+1];
typedef CHAR          STRING80[80+1];
typedef CHAR          STRING100[100+1];
typedef CHAR          STRING255[255+1];
typedef CHAR          STRING1000[1000+1];

typedef char *        PCHAR;
typedef WORD *        PWORD;
typedef BYTE *        PBYTE;
typedef DWORD *       PDWORD;
typedef STRING *      PSTRING;
typedef STRING2 *     PSTRING2;
typedef STRING4 *     PSTRING4;
typedef STRING6 *     PSTRING6;
typedef STRING10 *    PSTRING10;
typedef STRING16 *    PSTRING16;
typedef STRING19 *    PSTRING19;
typedef STRING40 *    PSTRING40;

typedef far *         PFAR;
typedef char far *    PCHARFAR;
typedef WORD far *   PWORDFAR;
typedef BYTE far *   PBYTEFAR;
typedef DWORD far *  PDWORDFAR;
typedef STRING far *  PSTRINGFAR;

#define   TRUE        1
#define   FALSE       0
#define   sSTART      2
#define   sSTOP       3
#define   sDELIM      0x1C

typedef struct
{
  CHAR     id_install;
  CHAR     id_fiscaldoc;
  STRING4  id_password;
  STRING16 id_operator;
  CHAR     id_packet;
  CHAR     id_host[128];            // for TCP *21.09.2000*
} HEADER;

typedef struct
{
  char mcr;
  char ier;
  char lcr;
  char baud0;
  char baud1;
} COM_STATE;


typedef struct
{
  BYTE     StartByte;
  BYTE     Packet;
  STRING2  Command;
  STRING2  FlashSt;
  STRING4  CurrentSt;
  STRING4  ErrorSt;
  STRING10 PrinterSt;
  STRING19 Other[21];
} ANSWER_BUFF;

#define ARRAY_SIZE 1024*1L

typedef struct
{
  WORD  com_count;
  BYTE  com_buffer[ARRAY_SIZE];
} COMBUFF;

typedef CHAR TByteArray[ARRAY_SIZE];
typedef TByteArray *PByteArray;

#endif //UMAINDEF.H