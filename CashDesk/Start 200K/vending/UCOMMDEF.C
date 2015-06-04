#ifndef NULL
#include <_null.h>
#endif

#include "ucommdef.h"

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

typedef struct
{
  PCHAR AdditP;
} StartShiftIT;
*/

#define IN_02 1
static BYTE IP_02[IN_02] = {P_P};

/* 0x03 ================= Get some information from registrator ===============
 Input Parameters:
   ReRers:Byte      - Reregistrations left
   Sessions:Word    - Sessions left
   LastRepNo:Word   - Last Z-report No
   FDocDate:String6 - First document date in session
   FDocTime:String4 - First document time in session

 Output Parameters - None

typedef struct
{
  BYTE      ReRers;
  WORD      Sessions;
  WORD      LastRepNo;
  STRING6   FDocDate;
  STRING4   FDocTime;
} GetResourcesOT;
*/

#define ON_03 5
static BYTE OP_03[ON_03] = {P_B,P_W,P_W,P_S6,P_S4};

/* 0x04 ========================== Fiscalization ==============================
 Input Parameters:
   OldPass:String16     - Old inspector password
   NewPass:String16     - New inspector password
   NewRegNo:String10    - New registration No
   NewCode:String12     - New customer code
   Group:Byte           - As is
   TotalInFlash:Boolean - Permit store info in flash

 Output Parameters - None

typedef struct
{
  STRING16     OldPass;
  STRING16     NewPass;
  STRING10     NewRegNo;
  STRING12     NewCode;
  BYTE         Group;
  BOOLEAN      TotalInFlash;
} FiscalizationIT;
*/

#define IN_04 6
static BYTE IP_04[IN_04] = {P_S16,P_S16,P_S10,P_S12,P_B,P_B};

/* 0x05,0x07 ============= Short & full reports on date ========================
 Input Parameters:
   Pass:String16     - Inspector password
   StartDate:String6 - Start date for report
dDate:String6   - End date for report

 Output Parameters:
   SaleSum:String19  - Sale sum
   PurSum:String19   - Purchase sum

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
*/

#define IN_05 3
static BYTE IP_05 [IN_05] = {P_S16,P_S6,P_S6};
#define ON_05 2
static BYTE OP_05 [ON_05] = {P_S19,P_S19};

/* 0x06,0x08 ============= Short & full reports on No ==========================
 Input Parameters:
   Pass:String16    - Inspector password
   StartNo:Word     - Start No for report
   EndNo:Word       - End No for report

 Output Parameters:
   SaleSum:String19 - Sale sum
   PurSum:String19  - Purchase sum

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
*/

#define IN_06 3
static BYTE IP_06 [IN_06] = {P_S16,P_W,P_W};
#define ON_06 2
static BYTE OP_06 [ON_06] = {P_S19,P_S19};

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
*/

#define IN_10 7
static BYTE IP_10 [IN_10] = {P_B,P_S16,P_S16,P_S16,P_B,P_S30,P_P};
#define ON_10 1
static BYTE OP_10 [ON_10] = {P_W};

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
*/

#define IN_11 8
static BYTE IP_11[IN_11] = {P_S40,P_S20,P_S19,P_S19,P_S3,P_B,P_S20,P_P};
#define ON_11 2
static BYTE OP_11[ON_11] = {P_S19,P_S19};

/* 0x12,0x16,0x22,0x26 == Total/subtotal on receipt or slip ======================
 Input Parameters - None
 Output Parameters :
   Sum:String19 - As is
*/
#define ON_12 1
static BYTE OP_12[ON_12] = {P_S19};

/* 0x13,0x23 ============== Tendering on receipt or slip =======================
 Input Parameters:
   Paytypedef struct:Byte      - As is
   TenderSum:BCD     - Sum from customer
   CardName:String40 - Paycard name
   AdditP:PChar      - Remark fields - Addition text {with '|' delimiters}

 Output Parameters:
   ExtraPay:String19 - Extra payment
   Change:String19   - As is

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
*/

#define IN_13 4
static BYTE IP_13[IN_13] = {P_B,P_S19,P_S40,P_P};
#define ON_13 2
static BYTE OP_13[ON_13] = {P_S19,P_S19};

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
*/

#define IN_15 4
static BYTE IP_15[IN_15] = {P_B,P_S19,P_S19,P_P};
#define ON_15 3
static BYTE OP_15[ON_15] = {P_S19,P_S19,P_S19};

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
*/

#define IN_20 14
static BYTE IP_20[IN_20] = {P_B,P_S16,P_S16,P_S16,P_S30,P_B,P_B,
                                         P_B,P_W,P_W,P_W,P_W,P_B,P_P};
#define ON_20 1
static BYTE OP_20[ON_20] = {P_W};


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

typedef struct
{
  STRING19  Before;
  STRING19  After;
} CashRegOT;
*/

#define IN_32 1
static BYTE IP_32[IN_32] = {P_S19};
#define ON_32 2
static BYTE OP_32[ON_32] = {P_S19,P_S19};

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
*/

#define ON_34 19
static BYTE OP_34[ON_34] = {P_S19,P_S19,P_S19,P_S19,P_S19,P_S19,
                                         P_S19,P_S19,P_S19,P_S19,P_S19,P_S19,
                                         P_S19,P_S19,P_S19,P_S19,P_S19,P_S19,
                                         P_S19};
/* 0x35 =============== Get last numbers ======================================
 Input Parameters - None
 Output Parameters:
   LastDocNo    Word;
   LastReceiptNoWord;
   LastSlipNo   Word;

typedef struct
{
  WORD    LastDocNo;
  WORD    LastReceiptNo;
  WORD    LastSlipNo;
} GetLastNumsOT;
*/

#define ON_35 3
static BYTE OP_35[ON_35] = {P_W,P_W,P_W};

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

typedef struct
{
  STRING19    PaymentSale;
  STRING19    PaymentSaleReverse;
  STRING19    PaymentRefund;
  STRING19    PaymentRefundReverse;
  STRING19    PaymentPurchase;
  STRING19    PaymentPurchaseReverse;

} Report3HostOT;
*/

#define IN_36 1
static BYTE IP_36[IN_36] = {P_B};

#define ON_36 6
static BYTE OP_36[ON_36] = {P_S19,P_S19,P_S19,P_S19,P_S19,P_S19};

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
*/

#define ON_37 18
static BYTE OP_37[ON_37] = {P_S19,P_S19,P_S19,P_S19,P_S19,P_S19,
                            P_S19,P_S19,P_S19,P_S19,P_S19,P_S19,
                            P_S19,P_S19,P_S19,P_S19,P_S19,P_S19};

/* 0x40 ============== Set new connection password ============================
 Input Parameters:
   NewPassword:String4 - As is
 Output Parameters - None
*/
#define IN_40 1
static BYTE IP_40[IN_40] = {P_S4};

/* 0x41 ====================== Set receipt header ==============================
  Input Parameters:
    [4] OF String38 - 4 lines in header
  Output Parameters - None

typedef STRING38 SetHeaderIT[4];
*/

#define IN_41 4
static BYTE IP_41[IN_41] = {P_S38,P_S38,P_S38,P_S38};

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

#define ON_43 2
static BYTE OP_43[ON_43] = {P_S6,P_S4};

/* 0x44 =================== Set drawer parameters =============================
 Input Parameters:
   OnTime:Byte     - "On" impulse length {*10 ms}
   OffTime:Byte    - "Off" impulse length {*10 ms}
   MiscParams:Word - Miscellanous receipt parameters

 Output Parameters - None

typedef struct
{
  BYTE    OnTime;
  BYTE    OffTime;
  BYTE    MiscParams;
} DrawerParamsT;
*/

#define IN_44 3
static BYTE IP_44[IN_44] = {P_B,P_B,P_W};

/* 0x45 =================== Get drawer parameters =============================
 Input Parameters - None

 Output Parameters:
   OnTime:Byte     - "On" impulse length {*10 ms}
   OffTime:Byte    - "Off" impulse length {*10 ms}
   MiscParams:Word - Miscellanous receipt parameters

 See DrawerParamsT for 0x44
*/

#define ON_45 3
static BYTE OP_45[ON_45] = {P_B,P_B,P_W};

/* 0x46 ====================== Set receipt tail ================================
  Input Parameters:
    [4] OF String40 - 4 lines in tail
  Output Parameters - None

typedef STRING40 SetTailIT[4];
*/

#define IN_46 4
static BYTE IP_46[IN_46] = {P_S40,P_S40,P_S40,P_S40};

/* 0x47 ==================== Set new operation names ============================
  Input Parameters:
    Str1:String15 - <Payment> operation name
    Str2:String15 - <Cancel> operation name
    Str3:String15 - <Refund> operation name

  Output Parameters - None

typedef struct
{
  STRING15    Str1;
  STRING15    Str2;
  STRING15    Str3;
} SetOperNameIT;
*/

#define IN_47 3
static BYTE IP_47[IN_47] = {P_S15,P_S15,P_S15};

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
*/

#define IN_4A 7
static BYTE IP_4A[IN_4A] = {P_B,P_S19,P_B,P_B,P_B,P_B,P_S19};

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

#define IN_4B 1
static BYTE IP_4B[IN_4B] = {P_B};
#define ON_4B 7
static BYTE OP_4B[ON_4B] = {P_B,P_S19,P_B,P_B,P_B,P_B,P_S19};

/* 0x4C ====================== Set registrator parameters ======================
 Input Parameters:
   MiscParams1Word - Miscellanous receipt parameters {same in 0x44 command}
   MiscParams2Word - ------------------------ /* -------------------------
   SlipTimeOutWord - Slip waiting timeout in secs

 Output Parameters - None

typedef struct
{
  WORD    MiscParams1;
  WORD    MiscParams2;
  WORD    SlipTimeOut;
} ParamsT;
*/

#define IN_4C 3
static BYTE IP_4C[IN_4C] = {P_W,P_W,P_W};

/* 0x4D ====================== Get registrator parameters ======================
 Input Parameters - None

 Output Parameters:
   MiscParams1Word - Miscellanous receipt parameters {same in 0x44 command}
   MiscParams2Word - ---------------------- /* ---------------------------
   SlipTimeOutWord - Slip waiting timeout in secs
*/

#define ON_4D 3
static BYTE OP_4D[ON_4D] = {P_W,P_W,P_W};

/* 0x4E =============== Set new receipt header {new format} ====================
  Input Parameters:
    [5] OF String40 - 6 lines in header

  Output Parameters - None

typedef STRING40 SetHeaderNewIT[6];
*/

#define IN_4E 6
static BYTE IP_4E[IN_4E] = {P_S40,P_S40,P_S40,P_S40,P_S40,P_S40};

/* 0x50 =============== Open new non-fiscal receipt  ==========================
 Input Parameters - None
 Output Parameters - None
*/

/* 0x51 =============== Print NF string on receipt ============================
 Input Parameters:
   Line:String40 - NF string
 Output Parameters - None
*/

#define IN_51 1
static BYTE IP_51[IN_51] = {P_S40};

/* 0x52,0x55 ============ Close NF receipt or slip ==============================
 Input Parameters - None
 Output Parameters - None
*/

/* 0x53 ================= Open new non-fiscal slip =============================
 Input Parameters:
  TopOffs:Word - Start top offset in points
 Output Parameters - None
*/

#define IN_53 1
static BYTE IP_53[IN_53] = {P_W};

/* 0x54 ================= Print NF string on slip ==============================
 Input Parameters:
   Font:Byte     - Font typedef struct
   LeftOffs:Word - Left offset in points before printing string
   DownGap:Word  - Gap down in points after printing string
   Line:String80 - NF string{80 symbols}

 Output Parameters - None
*/

#define IN_54 4
static BYTE IP_54[IN_54] = {P_B,P_W,P_W,P_S80};

/* 0x56 ==================== Print NF strings on receipt =======================
  Input Parameters:
    NumByte - Lines number
    [40] OF String40 - Lines to print
  Output Parameters - None

typedef struct
{
  BYTE      Num;
  STRING40  Info[70];
} LinesReceiptNFIT;
*/

#define IN_56 2
static BYTE IP_56[IN_56] = {P_PNum,P_S40};

/* 0x57 ===================== Print NF strings on slip =========================
  Input Parameters:
    FreeRecsTFRecs57 - Free records

  Output Parameters - None
*/

#define IN_57 1
static BYTE IP_57[IN_57] = {P_FR57};

/* 0x60 ===================== Set departments =========================*/

#define IN_60 1
static BYTE IP_60[IN_60] = {P_DR60};

/* 0x61 ===================== Get Departments =========================*/

#define ON_61 1
static BYTE OP_61[ON_61] = {P_DR60};

/* 0x63 ===================== SetArticrles =========================
  Input Parameters:
    FreeRecs : TFRecs60 - Free records array

  Output Parameters - None

typedef struct
{
  BYTE          DepNum;
  TARecs63      ArtRecs;
}TArticles;
typedef TArticles *PArticles;

*/

#define IN_63 2
static BYTE IP_63[IN_63] = {P_B,P_AR63};

/* 0x64 ===================== GetArticrles =========================
  Input Parameters:
    FreeRecs : TFRecs60 - Free records array

  Output Parameters - None
*/

#define IN_64 1
static BYTE IP_64[IN_64] = {P_B};
#define ON_64 1
static BYTE OP_64[ON_64] = {P_AR63};

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
*/

#define IN_71 19
static BYTE IP_71[IN_71] = {P_B,P_B,P_B,P_B,P_B,P_B,P_W,P_W,P_B,P_TP,
                       P_TP,P_TP,P_TP,P_TP,P_TP,P_S80,P_TP,P_S19,P_FR71};
/* 0x72 =================== Print electronic journal ==========================
 Input Parameters - None
 Output Parameters - None
*/

/* 0x74,0x75 ============= Print free fiscal doc on slip =======================
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
  TPos        DepartNum;
  TPos        ArticlesPos;
  TPos        ArticlesNum;
  TPos        OperPos;

  STRING80    OperName;

  TPos        SumPos;

  STRING19    Sum;

  TSRecs74    SubDepRecs;
  TPRecs74    PayRecs;
  TFRecs71    FreeRecs;
} FreeDocDIT;
*/

#define IN_74 25
static BYTE IP_74[IN_74] = {P_B,P_B,P_B,P_B,P_B,P_W,P_W,P_B,
                            P_TP,P_TP,P_TP,P_TP,P_TP,P_TP,
                            P_TP,P_B,P_TP,P_B,P_TP,P_S80,P_TP,P_S19,
                            P_FS74,P_FP74,P_FR71};

/* 0x92 =================== Registrator sertification =========================
 Input Parameters:
   SerialNo:String11 - As is
 Output Parameters - None
*/

#define IN_92 1
static BYTE IP_92[IN_92] = {P_S11};

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

typedef struct
{
  STRING6    sBaudRate;
  BOOLEAN    sIs5Wires;
  BOOLEAN    sIsNetwork;
  CHAR       sStationID;
  BOOLEAN    sIsDateTime;
  BOOLEAN    sIsSAnswer;
} SetupIT;
*/

#define IN_94 6
static BYTE IP_94[IN_94] = {P_S6,P_B,P_B,P_C,P_B,P_B};

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
*/

#define ON_95 7
static BYTE OP_95[ON_95] = {P_S10,P_S6,P_B,P_B,P_C,P_B,P_B};

/* 0x96 ===================== Get registrator serial No =======================
 Input Parameters - None
 Output Parameters:
  SerialNo:String11
*/

#define ON_96 1
static BYTE OP_96[ON_96] = {P_S11};

/* 0x97 ========================= Write User CMOS =============================
 Input Parameters:
  OffsByte - Offset in user CMOS area
  InfoString[100]

 Output Parameters - None

typedef struct
{
  BYTE         OffsByte;
  STRING100    InfoString100;
} UserCMOSIT;
*/

#define IN_97 2
static BYTE IP_97[IN_97] = {P_B,P_FR97};

/* 0x98 ========================== Read User CMOS =============================
 Input Parameters:
  OffsByte - Offset in user CMOS area
  Num Byte - Byte number

 Output Parameters:
  InfoString[100] - User Bytes; Must set OutNum in DoCommand

typedef struct
{
  BYTE      Offs;
  BYTE      Num;
} UserCMOSOT;
*/

#define IN_98 2
static BYTE IP_98[IN_98] = {P_B,P_B};
#define ON_98 1
static BYTE OP_98[ON_98] = {P_B};

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

typedef struct
{
  STRING2    sLHardFail;
  STRING2    sCHardFail;
  STRING4    sFlashState;
  STRING4    sCMOSPagesS;
  STRING4    sCMOSState;
  STRING6    sDate1;
  STRING6    sDate2;
  STRING6    sDate3;
} GetHardwareStatusOT;
*/

#define ON_99 8
static BYTE OP_99[ON_99] = {P_S2,P_S2,P_S4,P_S4,P_S4,P_S6,P_S6,P_S6};

/* 0x9A ==================== Set currency =======================================
  Input Parameters:
    IndexByte - Currency number {0,1}
    CNameString4 - Currency name
    Prec Byte - Presicion{Digits after point}
    PPrecBoolean - TRUE if need print digits after point
  Output Parameters - None

typedef struct
{
  BYTE      Index;
  STRING4   CName;
  BYTE      Prec;
  BOOLEAN   PPrec;
} CurrencyT;
*/

#define IN_9A 4
static BYTE IP_9A[IN_9A] = {P_B,P_S4,P_B,P_B};

/* 0x9B ==================== Get currency =======================================
  Input Parameters:
    IndexByte - Currency number {0,1}

  Output Parameters:
    IndexByte - Currency number {0,1}
    CNameString4 - Currency name
    Prec Byte - Presicion{Digits after point}
    PPrecBoolean - TRUE if need print digits after point
*/

#define IN_9B 1
static BYTE IP_9B[IN_9B] = {P_B};
#define ON_9B 4
static BYTE OP_9B[ON_9B] = {P_B,P_S4,P_B,P_B};

TCommDesc DescArray[UserCommandNum] =
{
  {0x01, 0x01, 0,     0,     NULL,               NULL               },
  {0x02, 0x01, IN_02, 0,     (PByteArray)&IP_02, NULL               },
  {0x03, 0x01, 0,     ON_03, NULL,               (PByteArray)&OP_03 },
  {0x10, 0x01, IN_10, ON_10, (PByteArray)&IP_10, (PByteArray)&OP_10 },
  {0x11, 0x00, IN_11, ON_11, (PByteArray)&IP_11, (PByteArray)&OP_11 },
  {0x12, 0x00, 0,     ON_12, NULL,               (PByteArray)&OP_12 },
  {0x13, 0x00, IN_13, ON_13, (PByteArray)&IP_13, (PByteArray)&OP_13 },
  {0x14, 0x00, 0,     0,     NULL,               NULL               },
  {0x15, 0x00, IN_15, ON_15, (PByteArray)&IP_15, (PByteArray)&OP_15 },
  {0x16, 0x00, 0,     ON_12, NULL,               (PByteArray)&OP_12 },
  {0x17, 0x01, 0,     0,     NULL,               NULL               },
  {0x20, 0x01, IN_20, ON_20, (PByteArray)&IP_20, (PByteArray)&OP_20 },
  {0x21, 0x00, IN_11, ON_11, (PByteArray)&IP_11, (PByteArray)&OP_11 },
  {0x22, 0x00, 0,     ON_12, NULL,               (PByteArray)&OP_12 },
  {0x23, 0x00, IN_13, ON_13, (PByteArray)&IP_13, (PByteArray)&OP_13 },
  {0x24, 0x00, 0,     0,     NULL,               NULL               },
  {0x25, 0x00, IN_15, ON_15, (PByteArray)&IP_15, (PByteArray)&OP_15 },
  {0x26, 0x00, 0,     ON_12, NULL,               (PByteArray)&OP_12 },
  {0x27, 0x01, 0,     0,     NULL,               NULL               },
  {0x30, 0x01, 0,     0,     NULL,               NULL               },
  {0x31, 0x01, 0,     0,     NULL,               NULL               },
  {0x32, 0x01, IN_32, ON_32, (PByteArray)&IP_32, (PByteArray)&OP_32 },
  {0x33, 0x01, IN_32, ON_32, (PByteArray)&IP_32, (PByteArray)&OP_32 },
  {0x34, 0x01, 0,     ON_34, NULL,               (PByteArray)&OP_34 },
  {0x35, 0x00, 0,     ON_35, NULL,               (PByteArray)&OP_35 },
  {0x36, 0x01, IN_36, ON_36, (PByteArray)&IP_36, (PByteArray)&OP_36 },
  {0x37, 0x01, 0,     ON_37, NULL,               (PByteArray)&OP_37 },
  {0x40, 0x01, IN_40, 0,     (PByteArray)&IP_40, NULL               },
  {0x41, 0x01, IN_41, 0,     (PByteArray)&IP_41, NULL               },
  {0x42, 0x01, 0,     0,     0,                  NULL               },
  {0x43, 0x00, 0,     ON_43, NULL,               (PByteArray)&OP_43 },
  {0x44, 0x00, IN_44, 0,     (PByteArray)&IP_44, NULL               },
  {0x45, 0x00, 0,     ON_45, NULL,               (PByteArray)&OP_45 },
  {0x46, 0x01, IN_46, 0,     (PByteArray)&IP_46, NULL               },
  {0x47, 0x00, IN_47, 0,     (PByteArray)&IP_47, NULL               },
  {0x4A, 0x00, IN_4A, 0,     (PByteArray)&IP_4A, NULL               },
  {0x4B, 0x00, IN_4B, ON_4B, (PByteArray)&IP_4B, (PByteArray)&OP_4B },
  {0x4C, 0x00, IN_4C, 0,     (PByteArray)&IP_4C, NULL               },
  {0x4D, 0x00, 0,     ON_4D, NULL,               (PByteArray)&OP_4D },
  {0x4E, 0x00, IN_4E, 0,     (PByteArray)&IP_4E, NULL               },
  {0x50, 0x01, 0,     0,     NULL,               NULL               },
  {0x51, 0x00, IN_51, 0,     (PByteArray)&IP_51, NULL               },
  {0x52, 0x00, 0,     0,     NULL,               NULL               },
  {0x53, 0x01, IN_53, 0,     (PByteArray)&IP_53, NULL               },
  {0x54, 0x00, IN_54, 0,     (PByteArray)&IP_54, NULL               },
  {0x55, 0x00, 0,     0,     NULL,               NULL               },
  {0x56, 0x00, IN_56, 0,     (PByteArray)&IP_56, NULL               },
  {0x57, 0x00, IN_57, 0,     (PByteArray)&IP_57, NULL               },
  {0x60, 0x00, IN_60, 0,     (PByteArray)&IP_60, NULL               },
  {0x61, 0x00, 0,     ON_61, NULL,               (PByteArray)&OP_61 },
  {0x63, 0x00, IN_63, 0,     (PByteArray)&IP_63, NULL               },
  {0x64, 0x00, IN_64, ON_64, (PByteArray)&IP_64, (PByteArray)&OP_64 },
  {0x70, 0x00, 0,     0,     NULL,               NULL               },
  {0x71, 0x00, IN_71, 0,     (PByteArray)&IP_71, NULL               },
  {0x72, 0x00, 0,     0,     NULL,               NULL               },
  {0x73, 0x00, IN_71, 0,     (PByteArray)&IP_71, NULL               },
  {0x74, 0x00, IN_74, 0,     (PByteArray)&IP_74, NULL               },
  {0x74, 0x00, IN_74, 0,     (PByteArray)&IP_74, NULL               },
  {0x94, 0x00, IN_94, 0,     (PByteArray)&IP_94, NULL               },
  {0x95, 0x00, 0,     ON_95, NULL,               (PByteArray)&OP_95 },
  {0x96, 0x00, 0,     ON_96, NULL,               (PByteArray)&OP_96 },
  {0x97, 0x00, IN_97, 0,     (PByteArray)&IP_97, NULL               },
  {0x98, 0x02, IN_98, ON_98, (PByteArray)&IP_98, (PByteArray)&OP_98 },
  {0x99, 0x00, 0,     ON_99, NULL,               (PByteArray)&OP_99 }
 };

FreeDocSIT  *PFreeDocSIT;
FreeDocDIT  *PFreeDocDIT;
WORD        wait_time=0;
WORD        send_time=0;
BOOLEAN     FIFO=FALSE;
WORD        base_addr;
HEADER      ComH={'N','N',"\0","\0",'0'};
ISR         old_isr;
BOOLEAN     ISPrinter=FALSE;
BYTE        err=0, tcp_err=0;

#ifdef UCOMMAND_LOG
PCHAR       LogPath=NULL;
#endif

COM_STATE old;

COMBUFF *addr_com_byff;
ANSWER_BUFF *Answer;
TByteArray  *PComBlock;

void interrupt ( *oldhandler )(void);

