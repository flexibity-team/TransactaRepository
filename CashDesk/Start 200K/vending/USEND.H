#ifndef _USEND_H
#define _USEND_H

#include <string.h>
#include <conio.h>
#include <stdio.h>
#include <time.h>
#include <dos.h>
#include "umaindef.h"


int WriteCom(PCHAR buffer , WORD len );

extern WORD send_time;
extern WORD base_addr;
extern pBUF Command ;
extern pBUF Answer;
extern BYTE err;
extern void WriteLog(PCHAR buf, WORD len);
extern void DTR_ON(void);
extern void DTR_OFF(void);


#endif //USEND.H