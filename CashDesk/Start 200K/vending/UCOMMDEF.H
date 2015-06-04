#ifndef _UCOMMDEF_H
#define _UCOMMDEF_H

#include "umaindef.h"
/*--------------- Parameter typedef structs --------------*/
#define P_B     0x01 /* Byte */
#define P_C     0x02 /* Char */
#define P_W     0x03 /* Word */
#define P_D     0x04 /* BCD */
#define P_S2    0x05 /* String2 */
#define P_S3    0x06 /* String3 */
#define P_S4    0x07 /* String4 */
#define P_S6    0x08 /* String6 */
#define P_S10   0x09 /* String10 */
#define P_S11   0x0A /* String11 */
#define P_S12   0x0B /* String12 */
#define P_S15   0x0C /* String15 */
#define P_S16   0x0D /* String16 */
#define P_S19   0x0E /* String19 */
#define P_S20   0x0F /* String20 */
#define P_S30   0x10 /* String30 */
#define P_S38   0x11 /* String38 */
#define P_S40   0x12 /* String40 */
#define P_S80   0x13 /* String80 */
#define P_TP    0x14 /* TPos */
#define P_P     0x15 /* PChar */
#define P_FR57  0x16 /* TFRecs - in 0x57 command */
#define P_FR71  0x17 /* TFRecs71 - in 0x71,0x73 commands */
#define P_FP74  0x18 /* TPayRecs74 */
#define P_FS74  0x19 /* TDepRecs74 */
#define P_FR97  0x20 /* String, converting to byte fields on every byte */
#define P_DR60  0x21 /* TDRecs60 in $60 command*/
#define P_AR63  0x22 /* TArticles in $63 command*/

#define P_PNum  0x50 /* Number of parameters */

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

/*--------------------- Errors -------------------*/
#define ER_CommandOk                 0x00
#define ER_InvalidCmdStructure       0x01
#define ER_InvalidFld                0x02
#define ER_InvalidTime               0x03
#define ER_InvalidBCC                0x04
#define ER_InvalidPassword           0x05
#define ER_InvalidCommand            0x06
#define ER_StartRequired             0x07
#define ER_TooLargeTime              0x08
#define ER_StringTooLarge            0x09
#define ER_CmdTooLarge               0x0A
#define ER_InvalidOperation          0x0B
#define ER_FldOutOfRange             0x0C
#define ER_IncorrectDocState         0x0D
#define ER_OutRequiredFld            0x0E
#define ER_AmountTooLarge            0x0F
#define ER_AmountOverflow            0x10
#define ER_NegativeOperCounter       0x11
#define ER_NegativeInCash            0x12
#define ER_NegativeDayTotal          0x13
#define ER_SertificationRequired     0x14
#define ER_ZRequired                 0x15
#define ER_PrinterTimeOut            0x16
#define ER_PrinterConnectError       0x17
#define ER_PrinterNotReady           0x18
#define ER_OutOfPaper                0x19
#define ER_FRequired                 0x1A
#define ER_InvalidInspPassword       0x1B
#define ER_AlreadySertifed           0x1C
#define ER_NoMoreFiscalizations      0x1D
#define ER_Invalid70Buffer           0x1E
#define ER_InvalidGField             0x1F
#define ER_InvalidPayType            0x20
#define ER_RegTimeOut                0x21
#define ER_RegGetErr                 0x22
#define ER_BadState                  0x23
#define ER_DocOverflow               0x24
#define ER_ShiftOpenRequired         0x25
#define ER_PrintEJournalRequired     0x26
#define ER_InvalidOPaymentIndex      0x27
#define ER_InvalidPrinterState       0x28
#define ER_ShiftAlreadyOpened        0x29
#define ER_SlipTimeOut               0x2A
#define ER_InvalidDate               0x2B

#define ER_FlashFail                 0x30
#define ER_InvalidFiscalDate         0x31
#define ER_InitializationRequired    0x32
#define ER_FiscalMemoryFull          0x33

// for TCP *21.09.2000*
#define cm_WRITE                     0x01
#define cm_READ                      0x02


/*....................... FOR FreeDoc71(73) function ...........................*/
typedef struct
{
  WORD    Line;
  WORD    Col;
  BYTE    Font;
}TPos;

typedef TPos *PTPos;

typedef struct
{
  WORD         Line;
  WORD         Col;
  BYTE         Font;
  BYTE         PrintMode;
  BYTE         JourNo;     /* Order No on journal, 0 - if not print*/
  STRING80     Info;
}TFreeRec71;
typedef TFreeRec71 *PFreeRec71;

#define FIELD_MAX 20

typedef TFreeRec71 TFreeRecs71[FIELD_MAX];

typedef struct
{
  BYTE            RecNum;
  TFreeRecs71     Recs;
}TFRecs71;
typedef TFRecs71 *PFRecs71;

/*....................... FOR FreeDoc74(75) function ...........................*/
typedef struct
{
  WORD            Line;
  WORD            Col;
  BYTE            Font;
  BYTE            PayMode;
  STRING19        PaySum;
}TPayRec74;
typedef TPayRec74 *PPayRec74;

typedef TPayRec74 TPayRecs74[5];

typedef struct
{
  BYTE           RecNum;
  TPayRecs74     Recs;
}TPRecs74;
typedef TPRecs74 *PPRecs74;

typedef struct
{
 BYTE           SubNum;
 STRING19       Sum;
}TSubRec74;
typedef TSubRec74 *PSubRec74;

typedef TSubRec74 TSubRecs74[5];

typedef struct
{
  BYTE          RecNum;
  TSubRecs74    Recs;
}TSRecs74;
typedef TSRecs74 *PSRecs74;


/*....................... FOR SetDepRec function ...........................*/
typedef struct
{
  BYTE          DepNum;
  STRING10      DepName;
}TDepRec60;
typedef TDepRec60 *PDepRec60;

typedef TDepRec60 TDepRecs60[100];

typedef struct
{
  BYTE          RecNum;
  TDepRecs60    Recs;
}TDRecs60;
typedef TDRecs60 *PDRecs60;

/*....................... FOR SetArtRec function ...........................*/

typedef struct
{
  BYTE          ArtNum;
  STRING10      ArtName;
  BYTE          ArtFlag;
}TArtRec63;

typedef TArtRec63 TArtRecs63[100];

typedef struct
{
  BYTE          RecNum;
  TArtRecs63    Recs;
}TARecs63;
typedef TARecs63 *PARecs63;

/*....................... FOR LinesSlipNF function ...........................*/

typedef struct
{
  BYTE         Font;
  WORD         LeftOffs;
  WORD         DownGap;
  STRING80     Line;
}TFreeRec57;

typedef TFreeRec57 TFreeRecs57[0x1];
typedef TFreeRecs57 *PFreeRecs57;

typedef struct
{
  BYTE            RecNum;
  PFreeRecs57     Recs;
}TFRecs57;
typedef TFRecs57 *PFRecs57;

/* 0x01 ========================== Start session ==============================
   Input Parameters - None
   Output Parameters - None
*/

/* 0x02 ========================== Open new shift =============================
   Input Parameters:
     CurDateString6 - As is
     CurTimeString4 - As is
     AdditP PChar   - Addition text {with '|' delimiters}

   Output Parameters - None
*/

typedef struct
{
  PCHAR AdditP;
} StartShiftIT;

/* 0x03 ================= Get some information from registrator ===============
 Input Parameters:  - None
 Output Parameters
   ReRers:Byte      - Reregistrations left
   Sessions:Word    - Sessions left
   LastRepNo:Word   - Last Z-report No
   FDocDate:String6 - First document date in session
   FDocTime:String4 - First document time in session
*/

typedef struct
{
  BYTE      ReRers;
  WORD      Sessions;
  WORD      LastRepNo;
  STRING6   FDocDate;
  STRING4   FDocTime;
} GetResourcesOT;

/* 0x04 ========================== Fiscalization ==============================
 Input Parameters:
   OldPass:String16     - Old inspector password
   NewPass:String16     - New inspector password
   NewRegNo:String10    - New registration No
   NewCode:String12     - New customer code
   Group:Byte           - As is
   TotalInFlash:Boolean - Permit store info in flash

 Output Parameters - None
*/

typedef struct
{
  STRING16     OldPass;
  STRING16     NewPass;
  STRING10     NewRegNo;
  STRING12     NewCode;
  BYTE         Group;
  BOOLEAN      TotalInFlash;
} FiscalizationIT;

/* 0x05,0x07 ============= Short & full reports on date ========================
 Input Parameters:
   Pass:String16     - Inspector password
   StartDate:String6 - Start date for report
   EndDate:String6   - End date for report

 Output Parameters:
   SaleSum:String19  - Sale sum
   PurSum:String19   - Purchase sum
*/

typedef struct
{
  STRING16    Pass;
  STRING6     StartDate;
  STRING6     EndDate;
} GetDateReportIT;

typedef struct
{
  STRING19    SaleSum;
  STRING19    PurSum;
} GetDateReportOT;

/* 0x06,0x08 ============= Short & full reports on No ==========================
 Input Parameters:
   Pass:String16    - Inspector password
   StartNo:Word     - Start No for report
   EndNo:Word       - End No for report

 Output Parameters:
   SaleSum:String19 - Sale sum
   PurSum:String19  - Purchase sum
*/

typedef struct
{
  STRING16    Pass;
  WORD        StartNo;
  WORD        EndNo;
} GetNoReportIT;

typedef struct
{
  STRING19    SaleSum;
  STRING19    PurSum;
} GetNoReportOT;

/* 0x10 =========================== Open new receipt ==========================
   Input Parameters:
   Doctypedef struct:Byte       - Document typedef struct {0..5}
   CashRegID:String16 - Cash register ID {Only for hotels}
   TableNo:String16   - Table{Room} No {Only for hotels}
   PlaceNo:String16   - As is
   Copies:Byte        - Number of copies
   AccountNo:String30 - As is
   AdditP:PChar       - Remark fields - Addition text {with '|' delimiters}

   Output Parameters:
   ReceiptNo:Word     - Current receipt No
*/

typedef struct
{
  BYTE        DocType;
  STRING16    CashRegNo;
  STRING16    TableNo;
  STRING16    PlaceNo;
  BYTE        Copies;
  STRING30    AccountNo;
  PCHAR       AdditP;
} StartReceiptIT;

/* 0x11,0x21 ================= Operation on receipt or slip ====================
 Input Parameters:
   WareName:String40 - Ware{service} name
   WareCode:String20 - As is
   Price:BCD         - As is
   Count:BCD         - Ware number{weight}
   Measure:String3   - Measurement
   Waretypedef struct:Byte     - As is
   SecID:String20    - Section ID
   AdditP:PChar      - Remark fields - Addition text {with '|' delimiters}

 Output Parameters:
   SaleSum:String19  - As is
   DocSum:String19   - Current document sum
*/

typedef struct
{
  STRING40    WareName;
  STRING20    WareCode;
  STRING19       Price;
  STRING19       Count;
  STRING3     Measure;
  BYTE        WareType;
  STRING20    SecID;
  PCHAR       AdditP;
} ItemDocIT;

typedef struct
{
  STRING19      SaleSum;
  STRING19      DocSum;
} ItemDocOT;

/* 0x12,0x16,0x22,0x26 == Total/subtotal on receipt or slip ======================
 Input Parameters - None
 Output Parameters :
   Sum:String19 - As is
*/

/* 0x13,0x23 ============== Tendering on receipt or slip =======================
 Input Parameters:
   Paytypedef struct:Byte      - As is
   TenderSum:BCD     - Sum from customer
   CardName:String40 - Paycard name
   AdditP:PChar      - Remark fields - Addition text {with '|' delimiters}

 Output Parameters:
   ExtraPay:String19 - Extra payment
   Change:String19   - As is
*/
typedef struct
{
  BYTE        PayType;
  STRING19    TenderSum;
  STRING40    CardName;
  PCHAR       AdditP;
} TenderDocIT;

typedef struct
{
  STRING19    ExtraPay;
  STRING19    Change;
} TenderDocOT;

/* 0x14,0x24 ================= Close receipt or slip ===========================
 Input Parameters - None
 Output Parameters - None
*/

/* 0x15,0x25 ============== Comission on receipt or slip =======================
 Input Parameters:
   Otypedef struct:Byte       - Operation typedef struct: 0-Overvalue, 1-Discount
   Percent:BCD      - Percent of overvalue {discount}
   Sum:BCD          - Sum of overvalue {discount}
   AdditP:PChar     - Remark fields - Addition text {with '|' delimiters}

 Output Parameters:
   Percent:String19 - Percent of overvalue {discount}
   SumA:String19    - Sum of overvalue {discount}
   DocSum:String19  - Current document sum
*/
typedef struct
{
  BYTE       OType;
  STRING19   Percent;
  STRING19   Sum;
  PCHAR      AdditP;
} ComissionDocIT;

typedef struct
{
  STRING19     Percent;
  STRING19     SumA;
  STRING19     DocSum;
} ComissionDocOT;

/* 0x17,0x27 ================ Cancel receipt or slip ===========================
 Input Parameters - None
 Output Parameters - None
*/

/* 0x20 ======================== Open new slip ================================
 Input Parameters:
   Doctypedef struct:Byte       - Document typedef struct {0..5}
   CashRegID:String16 - Cash register ID - Only for hotels & restaurants
   TableNo:String16   - Table{Room} No - Only for hotels & restaurants
   PlaceNo:String16   - As is
   AccountNo:String30 - As is
   Copies:Byte        - Number of copies
   CopiesH:Byte       - Number of copies on horizontal
   CopiesV:Byte       - Number of copies on vertical
   OffsL:Word         - Left offset on slip in points
   OffsT:Word         - Top offset on slip in points
   OffsBH:Word        - Horizontal offset between copies on slip in points
   OffsBV:Word        - Vertical offset between copies on slip in points
   OffsBS:Byte        - Offset between stocks on slip in points
   AdditP:PChar       - Remark fields - Addition text {with '|' delimiters}

 Output Parameters:
   SlipNo:Word        - Current slip No
*/
typedef struct
{
  BYTE        DocType;
  STRING16    CashRegID;
  STRING16    TableNo;
  STRING16    PlaceNo;
  STRING30    AccountNo;
  BYTE        Copies;
  BYTE        CopiesH;
  BYTE        CopiesV;
  WORD        OffsL;
  WORD        OffsT;
  WORD        OffsBH;
  WORD        OffsBV;
  BYTE        OffsBS;
  PCHAR       AdditP;
} StartSlipIT;

/* 0x30,0x31 =================== Get X or Z reports ============================
 Input Parameters - None
 Output Parameters - None
*/

/* 0x32,0x33  == Cash register operations {money to cash and from cash} ========
 Input Parameters:
   Sum:BCD     - As is

 Output Parameters:
   Before:String19  - As is
   After:String19   - As is
*/
typedef struct
{
  STRING19  Before;
  STRING19  After;
} CashRegOT;

/* 0x34 =================== Full report to host ===============================
 Input Parameters - None
 Output Parameters:
   CashSale:String19;
   CashSaleReverse:String19;
   CashRefund:String19;
   CashRefundReverse:String19;
   CashPurchase:String19;
   CashPurchaseReverse:String19;
   CreditSale:String19;
   CreditSaleReverse:String19;
   CardSale:String19;
   CardSaleReverse:String19;
   TotalSale:String19;
   BoxesSale:String19;
   TotalSaleReverse:String19;
   BoxesSaleReverse:String19;
   BoxesRefund:String19;
   BoxesRefundReverse:String19;
   ToCashReg:String19;
   FromCashReg:String19;
   InCashReg:String19;
*/
typedef struct
{
  STRING19    CashSale;
  STRING19    CashSaleReverse;
  STRING19    CashRefund;
  STRING19    CashRefundReverse;
  STRING19    CashPurchase;
  STRING19    CashPurchaseReverse;
  STRING19    CreditSale;
  STRING19    CreditSaleReverse;
  STRING19    CardSale;
  STRING19    CardSaleReverse;
  STRING19    TotalSale;
  STRING19    BoxesSale;
  STRING19    TotalSaleReverse;
  STRING19    BoxesSaleReverse;
  STRING19    BoxesRefund;
  STRING19    BoxesRefundReverse;
  STRING19    ToCashReg;
  STRING19    FromCashReg;
  STRING19    InCashReg;
} Report2HostOT;

/* 0x35 =============== Get last numbers ======================================
 Input Parameters - None
 Output Parameters:
   LastDocNo    Word;
   LastReceiptNoWord;
   LastSlipNo   Word;
*/

typedef struct
{
  WORD    LastDocNo;
  WORD    LastReceiptNo;
  WORD    LastSlipNo;
} GetLastNumsOT;

/* 0x36 =================== Payment report to host ===============================
 Input Parameters - PayType :Byte
 Output Parameters:
   CashSale:String19;
   CashSaleReverse:String19;
   CashRefund:String19;
   CashRefundReverse:String19;
   CashPurchase:String19;
   CashPurchaseReverse:String19;
   CreditSale:String19;
   CreditSaleReverse:String19;
   CardSale:String19;
   CardSaleReverse:String19;
   TotalSale:String19;
   BoxesSale:String19;
   TotalSaleReverse:String19;
   BoxesSaleReverse:String19;
   BoxesRefund:String19;
   BoxesRefundReverse:String19;
   ToCashReg:String19;
   FromCashReg:String19;
   InCashReg:String19;
*/
typedef struct
{
  STRING19    PaymentSale;
  STRING19    PaymentSaleReverse;
  STRING19    PaymentRefund;
  STRING19    PaymentRefundReverse;
  STRING19    PaymentPurchase;
  STRING19    PaymentPurchaseReverse;

} Report3HostOT;

/*{ $37 =================== Full report to host ===============================
 Input Parameters - None
 Output Parameters:
   CashSale:String19;
   CashSaleReverse:String19;
   CashRefund:String19;
   CashRefundReverse:String19;
   CashPurchase:String19;
   CashPurchaseReverse:String19;
   CreditSale:String19;
   CreditSaleReverse:String19;
   CardSale:String19;
   CardSaleReverse:String19;
   TotalSale:String19;
   BoxesSale:String19;
   TotalSaleReverse:String19;
   BoxesSaleReverse:String19;
   BoxesRefund:String19;
   BoxesRefundReverse:String19;
   ToCashReg:String19;
   FromCashReg:String19;
   InCashReg:String19;
}
*/
typedef struct
{
  STRING19    Sale;
  STRING19    SaleReverse;
  STRING19    Refund;
  STRING19    RefundReverse;
  STRING19    Purchase;
  STRING19    PurchaseReverse;

  STRING19    BoxesSale;
  STRING19    BoxesSaleReverse;
  STRING19    BoxesRefund;
  STRING19    BoxesRefundReverse;
  STRING19    BoxesPurchase;
  STRING19    BoxesPurchaseReverse;

  STRING19    ToCashReg;
  STRING19    FromCashReg;
  STRING19    InCashReg;

  STRING19    GroundTotalSale;
  STRING19    GroundRefund;
  STRING19    GroundPurchase;

} Report4HostOT;

/* 0x40 ============== Set new connection password ============================
 Input Parameters:
   NewPassword:String4 - As is
 Output Parameters - None
*/

/* 0x41 ====================== Set receipt header ==============================
  Input Parameters:
    [4] OF String38 - 4 lines in header
  Output Parameters - None
*/
typedef STRING38 SetHeaderIT[4];

/* 0x42 ================= Set registrator date & time =========================
 Input Parameters:
   Date:String6 - New date
   Time:String4 - New time

 Output Parameters - None
*/

/* 0x43 ================ Get registrator date & time ==========================
 Input Parameters - None

 Output Parameters:
   Date:String6 - Current date
   Time:String4 - Current time

 See DateTimeT for 0x42 command
*/

/* 0x44 =================== Set drawer parameters =============================
 Input Parameters:
   OnTime:Byte     - "On" impulse length {*10 ms}
   OffTime:Byte    - "Off" impulse length {*10 ms}
   MiscParams:Word - Miscellanous receipt parameters

 Output Parameters - None
*/
typedef struct
{
  BYTE    OnTime;
  BYTE    OffTime;
  BYTE    MiscParams;
} DrawerParamsT;

/* 0x45 =================== Get drawer parameters =============================
 Input Parameters - None

 Output Parameters:
   OnTime:Byte     - "On" impulse length {*10 ms}
   OffTime:Byte    - "Off" impulse length {*10 ms}
   MiscParams:Word - Miscellanous receipt parameters

 See DrawerParamsT for 0x44
*/

/* 0x46 ====================== Set receipt tail ================================
  Input Parameters:
    [4] OF String40 - 4 lines in tail
  Output Parameters - None
*/
typedef STRING40 SetTailIT[4];

/* 0x47 ==================== Set new operation names ============================
  Input Parameters:
    Str1:String15 - <Payment> operation name
    Str2:String15 - <Cancel> operation name
    Str3:String15 - <Refund> operation name

  Output Parameters - None
*/
typedef struct
{
  STRING15    Str1;
  STRING15    Str2;
  STRING15    Str3;
} SetOperNameIT;

/* 0x4A ==================== Set payment ========================================
  Input Parameters:
    Index        Byte - Payment number {0..5}
    PName        String19
    IsSecondLine Boolean - True if exist second line in payment name
    IsChange     Boolean
    CurrencyIndexByte {0,1}
    PermOperationByte {1,7} - If 1 only sell permitted else buy and refund also
    CrossCourse  String19 - Cross course to currency 0

  Output Parameters - None
*/
typedef struct
{
  BYTE       Index;
  STRING19   PName;
  BOOLEAN    IsSecondLine;
  BOOLEAN    IsChange;
  BYTE       CurrencyIndex;
  BYTE       PermOperation;
  STRING19   CrossCourse;
} PaymentT;

/* 0x4B ==================== Get payment ========================================
  Input Parameters:
    Index        Byte - Payment number {0..5}

  Output Parameters:
    Index        Byte - Payment number {0..5}
    PName        String19
    IsSecondLine Boolean - True if exist second line in payment name
    IsChange     Boolean
    CurrencyIndexByte {0,1}
    PermOperationByte {1,7} - If 1 only sell permitted else buy and refund also
    CrossCourse  String19 - Cross course to currency 0
*/

/* 0x4C ====================== Set registrator parameters ======================
 Input Parameters:
   MiscParams1Word - Miscellanous receipt parameters {same in 0x44 command}
   MiscParams2Word - ------------------------ /* -------------------------
   SlipTimeOutWord - Slip waiting timeout in secs

 Output Parameters - None
*/
typedef struct
{
  WORD    MiscParams1;
  WORD    MiscParams2;
  WORD    SlipTimeOut;
} ParamsT;

/* 0x4D ====================== Get registrator parameters ======================
 Input Parameters - None

 Output Parameters:
   MiscParams1Word - Miscellanous receipt parameters {same in 0x44 command}
   MiscParams2Word - ---------------------- /* ---------------------------
   SlipTimeOutWord - Slip waiting timeout in secs
*/

/* 0x4E =============== Set new receipt header {new format} ====================
  Input Parameters:
    [5] OF String40 - 6 lines in header

  Output Parameters - None
*/
typedef STRING40 SetHeaderNewIT[6];

/* 0x50 =============== Open new non-fiscal receipt  ==========================
 Input Parameters - None
 Output Parameters - None
*/

/* 0x51 =============== Print NF string on receipt ============================
 Input Parameters:
   Line:String40 - NF string
 Output Parameters - None
*/

/* 0x52,0x55 ============ Close NF receipt or slip ==============================
 Input Parameters - None
 Output Parameters - None
*/

/* 0x53 ================= Open new non-fiscal slip =============================
 Input Parameters:
  TopOffs:Word - Start top offset in points
 Output Parameters - None
*/

/* 0x54 ================= Print NF string on slip ==============================
 Input Parameters:
   Font:Byte     - Font typedef struct
   LeftOffs:Word - Left offset in points before printing string
   DownGap:Word  - Gap down in points after printing string
   Line:String80 - NF string{80 symbols}

 Output Parameters - None
*/

/* 0x56 ==================== Print NF strings on receipt =======================
  Input Parameters:
    NumByte - Lines number
    [40] OF String40 - Lines to print
  Output Parameters - None
*/
typedef struct
{
  BYTE      Num;
  STRING40  Info[70];
} LinesReceiptNFIT;

/* 0x57 ===================== Print NF strings on slip =========================
  Input Parameters:
    FreeRecsTFRecs57 - Free records

  Output Parameters - None
*/

/* 0x60 ===================== Set departments =========================*/

/* 0x61 ===================== Get Departments =========================*/

/* 0x63 ===================== SetArticrles =========================
  Input Parameters:
    FreeRecs : TFRecs60 - Free records array

  Output Parameters - None
*/
typedef struct
{
  BYTE          DepNum;
  TARecs63      ArtRecs;
}TArticles;
typedef TArticles *PArticles;

/* 0x64 ===================== GetArticrles =========================
  Input Parameters:
    FreeRecs : TFRecs60 - Free records array
  Output Parameters - None
*/

/* 0x71,0x73 ============= Print free fiscal doc on slip =======================
   Doctypedef struct:Byte       - Document typedef struct {0..5}
   FlipFOffs:Byte     - Flip font offset {if 0 - normal font} <Not use for 0x73>
   PageNum:Byte       - As is
   HCopyNum:Byte      - Horizontal copies number <Not use for 0x73>
   VCopyNum:Byte      - Vertical copies number <Not use for 0x73>
   LOffs:Word         - Left offset of second copy <Not use for 0x73>
   VGap:Word          - Vertical gap between copies <Not use for 0x73>
   LGap:Byte          - Gap between lines <Not use for 0x73>
   SerNoPos:TPos      - Place for serial No
   DocNoPos:TPos      - Place for document No
   DataPos:TPos       - Place for data
   TimePos:TPos       - Place for time
   OperPos:TPos       - Place for No operator name
   OperName:String80  - Operator name
   SumPos:TPos        - Place for sum
   Sum:BCD            - As is
   FreeRecs:TFRecs71  - Free records
*/
typedef struct
{
  BYTE        DocType;
  BYTE        PayType;
  BYTE        FlipFOffs;
  BYTE        PageNum;
  BYTE        HCopyNum;
  BYTE        VCopyNum;
  WORD        LOffs;
  WORD        VGap;
  BYTE        LGap;
  TPos        SerNoPos;
  TPos        DocNoPos;
  TPos        DatePos;
  TPos        TimePos;
  TPos        InnPos;
  TPos        OperPos;
  STRING80    OperName;
  TPos        SumPos;
  STRING19    Sum;
  TFRecs71    FreeRecs;
} FreeDocSIT;

/* 0x72 =================== Print electronic journal ==========================
 Input Parameters - None
 Output Parameters - None
*/

/* 0x74,0x75 ============= Print free fiscal doc on slip =======================
/*
typedef struct
{
  BYTE        DocType;
  BYTE        FlipFOffs;
  BYTE        PageNum;
  BYTE        HCopyNum;
  BYTE        VCopyNum;
  WORD        LOffs;
  WORD        VGap;
  BYTE        LGap;

  TPos        SerNoPos;
  TPos        DocNoPos;
  TPos        OperPos;
  TPos        DatePos;
  TPos        TimePos;
  TPos        InnPos;

  TPos        DepartDocPos;
  BYTE        DepartNum;
  TPos        ArticlesPos;
  BYTE        ArticlesNum;
  TPos        OperPos;
  STRING80    OperName;
  TPos        SumPos;
  STRING19    Sum;
  TSRecs74    SubRecs;
  TPRecs74    PayRecs;
  TFRecs71    FreeRecs;
} FreeDocDIT;
*/
typedef struct
{
  BYTE        DocType;
  BYTE        FlipFOffs;
  BYTE        PageNum;
  BYTE        HCopyNum;
  BYTE        VCopyNum;
  WORD        LOffs;
  WORD        VGap;
  BYTE        LGap;

  TPos        SerNoPos;
  TPos        DocNoPos;
  TPos        OperNoPos;
  TPos        DatePos;
  TPos        TimePos;
  TPos        InnPos;

  TPos        DepartDocPos;
  BYTE        DepartNum;
  TPos        ArticlesPos;
  BYTE        ArticlesNum;
  TPos        OperPos;
  STRING80    OperName;
  TPos        SumPos;
  STRING19    Sum;

  TSRecs74    SubRecs;
  TPRecs74    PayRecs;
  TFRecs71    FreeRecs;
} FreeDocDIT;

/* 0x92 =================== Registrator sertification =========================
 Input Parameters:
   SerialNo:String11 - As is
 Output Parameters - None
*/

/* 0x93 =================== Registrator sertification =========================
 Input Parameters  - None
 Output Parameters - None
*/

/* 0x94 ======================= Registrator setup =============================
 Input Parameters:
   sBaudRate:String6   - As is
   sIs5Wires:Boolean   - True if DTR/DSR using
   sIsNetwork:Boolean  - True if need network prefix
   sStationID:Char     - As is
   sIsDateTime:Boolean - True if date/time in command
   sIsSAnswer:Boolean  - True if short answers
 Output Parameters - None
*/
typedef struct
{
  STRING6    sBaudRate;
  BOOLEAN    sIs5Wires;
  BOOLEAN    sIsNetwork;
  CHAR       sStationID;
  BOOLEAN    sIsDateTime;
  BOOLEAN    sIsSAnswer;
} SetupIT;

/* 0x95 ===================== Get registrator parameters ======================
 Input Parameters - None
 Output Parameters:
   sVersion:String10   - Registrator version
   sBaudRate:String6   - As is
   sIs5Wires:Boolean   - True if DTR/DSR using
   sIsNetwork:Boolean  - True if need network prefix
   sStationID:Char     - As is
   sIsDateTime:Boolean - True if date/time in command
   sIsSAnswer:Boolean  - True if short answers
*/
typedef struct
{
  STRING10   sVersion;
  STRING6    sBaudRate;
  BOOLEAN    sIs5Wires;
  BOOLEAN    sIsNetwork;
  CHAR       sStationID;
  BOOLEAN    sIsDateTime;
  BOOLEAN    sIsSAnswer;
} GetParamsOT;

/* 0x96 ===================== Get registrator serial No =======================
 Input Parameters - None
 Output Parameters:
  SerialNo:String11
*/

/* 0x97 ========================= Write User CMOS =============================
 Input Parameters:
  OffsByte - Offset in user CMOS area
  InfoString[100]

 Output Parameters - None
*/

typedef struct
{
  BYTE         OffsByte;
  STRING100    InfoString100;
} UserCMOSIT;

/* 0x98 ========================== Read User CMOS =============================
 Input Parameters:
  OffsByte - Offset in user CMOS area
  Num Byte - Byte number

 Output Parameters:
  InfoString[100] - User Bytes; Must set OutNum in DoCommand
*/

typedef struct
{
  BYTE      Offs;
  BYTE      Num;
} UserCMOSOT;

/* 0x99 ===================== Get hardware registers ==========================
 Input Parameters - None
 Output Parameters:
   sLHardFail String2   - Last session HardwareFailExt
   sCHardFail String2   - Current session HardwareFailExt
   sFlashStateString4   - Current session FlashState
   sCMOSPagesSString4   - Current session CMOSPagesState
   sCMOSState String4   - Current session CMOSState
   sDate1     String6   - BugDate1
   sDate2     String6   - BugDate2
   sDate3     String6   - BugDate3
*/

/* 0x9A ==================== Set currency =======================================
  Input Parameters:
    IndexByte - Currency number {0,1}
    CNameString4 - Currency name
    Prec Byte - Presicion{Digits after point}
    PPrecBoolean - TRUE if need print digits after point
  Output Parameters - None
*/
typedef struct
{
  BYTE      Index;
  STRING4   CName;
  BYTE      Prec;
  BOOLEAN   PPrec;
} CurrencyT;

/* 0x9B ==================== Get currency =======================================
  Input Parameters:
    IndexByte - Currency number {0,1}

  Output Parameters:
    IndexByte - Currency number {0,1}
    CNameString4 - Currency name
    Prec Byte - Presicion{Digits after point}
    PPrecBoolean - TRUE if need print digits after point
*/

/*---------------------------- Descriptions ----------------------------------*/
typedef  struct
{
  BYTE           No;          /* Command No*/
  BYTE           Md;          /* Mode Byte: Bit 0 - 1 if date/time in command*/
                              /*      Bit 1 - 1 if field num gets from answer*/
  BYTE           INum;        /* Input parameters number*/
  BYTE           ONum;        /* Output parameters number*/
  PByteArray     IPar;        /* Input parameters*/
  PByteArray     OPar;        /* Output parameters*/
} TCommDesc;

#define MaxCommand     0xA0U
#define UserCommandNum 64

#endif //UCOMMDEF.H