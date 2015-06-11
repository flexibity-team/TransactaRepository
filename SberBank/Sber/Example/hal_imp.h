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


#define ICMD_DISP_STRING_1  1
#define ICMD_DISP_STRING_2  2
#define ICMD_DISP_CLEAR     3
#define ICMD_SHOW_INPUT     4
#define ICMD_GET_KEY        5

#define MAX_HALL_LIBS       5


typedef int (*T_INTERFACE_FUNC)(int code, char* par1, int par2);

extern void _FAR_  SetUIHandles(int p1, int p2);

void pccv_InitDisplayHAL(void);
void pccv_DoneDisplayHAL(void);
int drvGetSerialNumber(char* dest);
void drvDispLineHAL(const char* str, int line);
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

