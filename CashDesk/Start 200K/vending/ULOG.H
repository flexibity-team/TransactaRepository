#ifndef _ULOG_H
#define _ULOG_H

#include <string.h>
#include <conio.h>
#include <stdio.h>
#include "umaindef.h"

extern PCHAR LogPath;
void         WriteLog( PCHAR buf, WORD len );
WORD         FOpen(PCHARFAR FName);
WORD         FCreate(PCHARFAR FName);
DWORD        FSize(WORD H);
BOOLEAN      FWrite(WORD H, PCHARFAR Buffer,WORD Bytes);
void         FClose(WORD H);
#endif //ULOGH