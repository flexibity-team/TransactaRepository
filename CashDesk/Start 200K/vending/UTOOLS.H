#ifndef _UTOOLS_H
#define _UTOOLS_H

#include <stdlib.h>
#include <dos.h>
//#include <stdio.h>
#include <string.h>
#include <conio.h>
#include <time.h>
#include "ucommdef.h"

extern int WriteCom(PCHAR buffer , WORD len );
extern int ReadCom(PCHAR buffer);

void INIT_FIFO(void);
void RESET_FIFO(void);
BOOLEAN UART_FIFO(void);
void DTR_ON(void);
void DTR_OFF(void);
void GetTime(PCHAR S);
void GetDate(PCHAR S);
void GetDateTime(PCHAR S);
void GetCommTail(PCHAR S);
void GetCommHeader(PCHAR S,BYTE Command);
BOOLEAN CalcBCC(PCHAR S, PWORD CRC);
PCHAR Long2Asc(LONGINT Value, PCHAR StrP, BYTE Radix, BOOLEAN UpCase);
BYTE DoCommand(BYTE Command, PCHAR IParams);
void Word2HexPChar(WORD Source, PCHAR Dest);
void Byte2HexPChar(BYTE Source, PCHAR Dest);
BOOLEAN PChar2HexWord(PCHAR Source, WORD Len, PWORD Result);
PCHAR GetField(PCHAR buf, BYTE n);
WORD SwapBytes(WORD *W);
void FormByte2HexPChar(BYTE Source, PCHAR Dest);
void FormWord2HexPChar(WORD Source, PCHAR Dest);
BOOLEAN TestInstall(void);
int WriteCom(PCHAR buffer , WORD len );
int ReadCom(PCHAR buffer);
int GetErrCode(BYTE Command);
int XchgMessage(PCHAR buffer, WORD len, WORD Command);
int XchgMessageTCP(PCHAR buffer, WORD len);
BYTE RunCommand(BYTE Command, PCHAR IParams);
WORD FieldCount(void);

extern WORD    base_addr;
extern HEADER  ComH;
extern ANSWER_BUFF *Answer;
extern TByteArray  *PComBlock;
extern FreeDocSIT  *PFreeDocSIT;
extern WORD    wait_time;
extern TCommDesc DescArray[UserCommandNum];
extern BYTE      err;
extern int RdWrTCP( PCHAR buffer, WORD len, BYTE cmd );
#endif //UTOOLS.H