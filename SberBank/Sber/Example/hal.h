#ifndef _HAL_VM_H
#define _HAL_VM_H

#define _FAR_

#ifndef __BYTE__
#define __BYTE__
    typedef unsigned char  BYTE;
#endif

#ifndef __WORD__
#define __WORD__
    typedef unsigned short WORD;
#endif

#ifndef __DWORD__
#define __DWORD__
    typedef unsigned long  DWORD;
#endif

#ifdef __cplusplus
extern "C" {
#endif

#ifdef DREAM_BOX
  #include "drv.h"
#else

#define DRV_BADMEDIA                 251
#define DRV_NOTSUPP                  252
#define DRV_FAILURE                  253
#define DRV_TIMEOUT                  254

#define MAX_TRACK1     79
#define MAX_TRACK2     40
#define MAX_TRACK3    107

#define UP_MAX_DIR_LENGTH 250
#define KBD_ENTER      0x0D

#define KEY_REMOVED            98
#define KEY_NOT_LOADED         99

#pragma pack(1)
typedef struct{
  long  FicDay;
  long  FicMon;
  long  FicYear;
  DWORD dwLastLoadDate;
  DWORD dwLastLoadTime;
  WORD  WaitCardTime;
  WORD  WaitPinTime;
  BYTE  MinPinLength;
  BYTE  bKeyLoaded[3];
  BYTE  bKeyOptions[3];
  BYTE  MyMacIV[8];
  BYTE  Reserve1;      /* must be 0x00 */
  BYTE  PinpadSpeed;
  BYTE  Reserve2[22];

#ifdef NO_KLK_SUPPORTED
  BYTE  MyKLK[16]; /* for SC552 or other devices w/o a KLK */
#endif

  BYTE  OperMode;

#ifdef FAKED_SERIAL_SUPPORTED
  char  FakedSerNo[33];
#endif

  BYTE  bPPInterface;
  BYTE  bUserIntHere;
  BYTE  bPPScreenWidth;
  BYTE  bPPScreenHeight;
  BYTE  bNeedRemLoad;
  BYTE  bAutoSetDone;
  WORD  MerchTrxNeedsFix;

#ifdef TERMINAL_APPLICATION
  BYTE  PinpadSN[12];
#endif

#ifdef _WAY4
  BYTE  bNewKeysReceived;
#endif

  char  bSizeCheckField;  /* this should always be the last field */
  //////M PCIMode Version?
  BYTE  bPCIVersion;
  /////~M
}TGlobalParams;

typedef struct{
  BYTE*  Track1;
  BYTE*  Track2;
  BYTE*  Track3;
  BYTE   Stat1;
  BYTE   Stat2;
  BYTE   Stat3;
}TMagnCard;

typedef struct{
  BYTE* pIV;
  BYTE* pWorkKey;
  BYTE* pData;
  BYTE* pMAC;
  int   KeyLength;
  int   Limit;
}TMacInfo;

typedef struct{
  BYTE* pPAN;
  BYTE* pWorkKey;
  BYTE* pPINblock;
  int   timeout;
  int   KeyLength;
  BYTE  MinPinLen;
  BYTE  MaxPinLen;
  DWORD dwAmount;
}TPinInfo;


typedef struct{
  BYTE* pDataIn;
  BYTE* pDataOut;
  BYTE  CLA,INS,P1,P2,Lc,Le;
  BYTE  SW1SW2[2];
}TApdu;
#pragma pack()

extern void  _FAR_ drvInit(void);
extern void  _FAR_ drvShutdown(void);
extern void  _FAR_ tspSaveParams();

extern BYTE  _FAR_ drvEnterMagnPin(TPinInfo* pPin);
extern BYTE  _FAR_ drvCalcMAC(TMacInfo* pMac);
extern BYTE  _FAR_ drvLoadKey(BYTE* thekey, BYTE idx, int Length, BYTE bCrypt);
extern void  _FAR_ drvDeleteKey(BYTE idx);
extern BYTE  _FAR_ drvIsKeyPresent(BYTE idx);
extern int   _FAR_ drvGetSerialNum(char* buf);

extern BYTE  _FAR_ drvICCExchange(BYTE cid, TApdu* pExchange);
extern BYTE  _FAR_ drvICCReset(BYTE cid);
extern BYTE  _FAR_ drvICCTest(BYTE cid);
extern void  _FAR_ drvICCOff(BYTE cid);
extern short _FAR_ drvICCHistoryBytes(BYTE cid, BYTE* dest);
extern void  _FAR_ drvEjectClientCard(void);

extern BYTE  _FAR_ drvOpenMagnReader(void);
extern BYTE  _FAR_ drvTestMagnCard(TMagnCard* pCard);
extern void  _FAR_ drvCloseMagnReader(void);

extern BYTE  _FAR_ drvGetKey(void);
extern void  _FAR_ drvDispLine(char* text, int idx);
extern void  _FAR_ drvClearDisplay(void);
#endif


#define ICMD_DISP_STRING_1  1
#define ICMD_DISP_STRING_2  2
#define ICMD_DISP_CLEAR     3
#define ICMD_SHOW_INPUT     4
#define ICMD_GET_KEY        5

#define MAX_HAL_LIBS        5


typedef int (*T_INTERFACE_FUNC)(int code, char* par1, int par2);

extern void _FAR_  SetUIHandles(int p1, int p2);

void pccv_InitDisplayHAL(void);
void pccv_DoneDisplayHAL(void);
int drvGetSerialNumber(char* dest);
void drvDispLineHAL(char* str, int line);
extern char CurrentDir[UP_MAX_DIR_LENGTH];
extern TGlobalParams      Params;
extern char CurrentDir[UP_MAX_DIR_LENGTH];
extern HINSTANCE Self;
#define HALAPI

typedef void  HALAPI (*DRV_INIT)         (void);
typedef void  HALAPI (*DRV_SHUTDOWN)     (void);

typedef void  HALAPI (*DRV_SET_UI)       (int, int);

typedef BYTE  HALAPI (*DRV_GETKEY)       (void);
typedef void  HALAPI (*DRV_DISPLINE)     (const char*, int );
typedef void  HALAPI (*DRV_CLEARDISP)    (void);
typedef void  HALAPI (*DRV_SETBIGLINE)   (int );
typedef void  HALAPI (*DRV_HIGHLITE)     (char* , int , BYTE );
typedef void  HALAPI (*DRV_BEEP)         (void);

typedef BYTE  HALAPI (*DRV_ENTERMAGNPIN) (TPinInfo* );
typedef BYTE  HALAPI (*DRV_CALCMAC)      (TMacInfo* );
typedef BYTE  HALAPI (*DRV_LOADKEY)      (BYTE*, BYTE, int, BYTE );
typedef BYTE  HALAPI (*DRV_DELETEKEY)    (BYTE );
typedef BYTE  HALAPI (*DRV_ISKEYPRESENT) (BYTE );

typedef BYTE  HALAPI (*DRV_ICCEXCHANGE)  (BYTE, TApdu* );
typedef BYTE  HALAPI (*DRV_ICCRESET)     (BYTE );
typedef BYTE  HALAPI (*DRV_ICCTEST)      (BYTE );
typedef void  HALAPI (*DRV_ICCOFF)       (BYTE );
typedef short HALAPI (*DRV_ICCHISTORY)   (BYTE, BYTE* );

typedef BYTE  HALAPI (*DRV_OPENMAGN)     (void);
typedef BYTE  HALAPI (*DRV_CLOSEMAGN)    (void);
typedef BYTE  HALAPI (*DRV_TESTMAGN)     (TMagnCard* );
typedef BYTE  HALAPI (*DRV_EJECTCARD)    (void);
typedef int   HALAPI (*DRV_GETSERIAL)    (char* );
typedef void  HALAPI (*DRVFUNC)          (void);


extern DRV_INIT         g_fInit         ;
extern DRV_SHUTDOWN     g_fShutdown     ;
extern DRV_GETKEY       g_fGetKey       ;
extern DRV_DISPLINE     g_fDispLine     ;
extern DRV_CLEARDISP    g_fClearDisp    ;
extern DRV_SETBIGLINE   g_fSetBigLine   ;
extern DRV_HIGHLITE     g_fHighlite     ;
extern DRV_BEEP         g_fBeepOK       ;
extern DRV_BEEP         g_fBeepError    ;
extern DRV_ENTERMAGNPIN g_fEnterMagnPin ;
extern DRV_CALCMAC      g_fCalcMac      ;
extern DRV_LOADKEY      g_fLoadKey      ;
extern DRV_DELETEKEY    g_fDeleteKey    ;
extern DRV_ISKEYPRESENT g_fIsKeyPresent ;
extern DRV_ICCEXCHANGE  g_fIccExchange  ;
extern DRV_ICCRESET     g_fIccReset     ;
extern DRV_ICCTEST      g_fIccTest      ;
extern DRV_ICCOFF       g_fIccOff       ;
extern DRV_ICCHISTORY   g_fIccHistory   ;
extern DRV_OPENMAGN     g_fOpenMagn     ;
extern DRV_CLOSEMAGN    g_fCloseMagn    ;
extern DRV_TESTMAGN     g_fTestMagn     ;
extern DRV_EJECTCARD    g_fEjectCard    ;
extern DRV_GETSERIAL    g_fGetSerial    ;

extern DRV_SET_UI       g_fSetUI        ;


#ifdef __cplusplus
};
#endif

#endif

