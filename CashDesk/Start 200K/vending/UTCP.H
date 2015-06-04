#ifndef _UTCP_H
#define _UTCP_H

 // for TCP *21.09.2000*

#include <string.h>
#include <conio.h>
#include <stdio.h>
#include <time.h>
#include "utools.h"
#include "ucommdef.h"

int RdWrTCP( PCHAR buffer, WORD len, BYTE cmd );

extern COMBUFF *addr_com_byff;
extern HEADER  ComH;
extern BYTE    tcp_err;
extern void WriteLog(PCHAR buf, WORD len);
extern BOOLEAN     ISPrinter;

#endif //UTCP.H