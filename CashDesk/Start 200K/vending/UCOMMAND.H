#ifndef _UCOMMAND_H
#define _UCOMMAND_H

#include <stdlib.h>
//#include <stdio.h>
#include <dos.h>
#include <conio.h>
#include <string.h>
#include <extend.h>

#include "ucommdef.h"

extern CLIPPER InitCom (void);
extern CLIPPER ResetCom (void);

extern BOOLEAN TestInstall(void);
extern int WriteCom(PCHAR buffer , WORD len );
extern int ReadCom(PCHAR buffer);
extern int XchgMessage(PCHAR buffer, WORD len, WORD Command);
extern int XchgMessageTCP(PCHAR buffer, WORD len);
extern BYTE RunCommand(BYTE Command, PCHAR IParams);
extern WORD FieldCount(void);
extern PCHAR GetField(PCHAR buf, BYTE n);
extern BOOLEAN PChar2HexWord(PCHAR Source, WORD Len, PWORD Result);
extern void bug(PCHAR Step);
extern int RdWrTCP( PCHAR buffer, WORD len, BYTE cmd );

extern WORD        base_addr;
extern WORD        send_time;
extern WORD        wait_time;
extern HEADER      ComH;
extern BOOLEAN     ISPrinter;
extern ANSWER_BUFF *Answer;
extern FreeDocSIT  *PFreeDocSIT;
extern FreeDocDIT  *PFreeDocDIT;
extern BYTE        err, tcp_err;

CLIPPER Session(void);
CLIPPER ShiftOpen(void);
CLIPPER SetOper(void);
CLIPPER SetHost(void);
CLIPPER GetOper(void);
CLIPPER ShiftClose(void);
CLIPPER ShiftCloseM(void);
CLIPPER XReport(void);
CLIPPER ToCash(void);
CLIPPER FromCash(void);
CLIPPER OpenFDoc(void);
CLIPPER AddFPos(void);
CLIPPER AddFField(void);
CLIPPER PrintFSlip(void);
CLIPPER PrintFRec(void);
CLIPPER OpenFDoc74(void);
CLIPPER AddFPos74(void);
CLIPPER AddPay74(void);
CLIPPER AddSubDep74(void);
CLIPPER AddField74(void);
CLIPPER PrintFS74(void);
CLIPPER PrintFR74(void);
CLIPPER CancelFDoc(void);
CLIPPER PrintEJ(void);
CLIPPER FDocOpen(void);
CLIPPER FDocPrint(void);
CLIPPER FDocClose(void);
CLIPPER PrintFDoc(void);
CLIPPER SetDeparts(void);
CLIPPER GetDepart(void);
CLIPPER GetArticles(void);
CLIPPER SetTime(void);
CLIPPER SetParam(void);
CLIPPER GetParam(void);
CLIPPER RunCmdC(void);
CLIPPER InitDispC(void);
CLIPPER ShowDispH(void);
CLIPPER ShowDispP(void);
CLIPPER WriteComC(void);
CLIPPER ReadComC(void);
CLIPPER ReadLastErr(void);
CLIPPER SetHeader(void);
CLIPPER GetVersion(void);
CLIPPER SetATime(void);
CLIPPER GetTimeAnswer(void);
CLIPPER SetSTime(void);
CLIPPER GetTimeSend(void);
CLIPPER StartDoc(void);
CLIPPER ItemDoc(void);
CLIPPER STotalDoc(void);
CLIPPER TotalDoc(void);
CLIPPER TenderDoc(void);
CLIPPER DrvOpen(void);
CLIPPER CloseDoc(void);
CLIPPER CancelDoc(void);
CLIPPER GetSField(void);
CLIPPER GetWField(void);
CLIPPER FieldC(void);
CLIPPER GetAnsw(void);
CLIPPER DayMony(void);
CLIPPER GetNumDoc(void);
CLIPPER GetLastErr(void);
CLIPPER GetErrTCP(void);

#ifdef UCOMMAND_LOG
CLIPPER SetLogPath(void);
extern  PCHAR LogPath;
#endif

#endif //UCOMMAND.H