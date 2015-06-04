#ifndef _URECEIVE_H
#define _URECEIVE_H

#include <string.h>
#include <conio.h>
#include <dos.h>
#include <time.h>
#include "umaindef.h"


int ReadCom(PCHAR buffer);

extern WORD wait_time;
extern WORD base_addr;
extern pBUF Command ;
extern pBUF Answer;
extern BYTE err;
extern void WriteLog(PCHAR buf, WORD len);
extern void DTR_ON(void);
extern void DTR_OFF(void);


#endif //URECEIVE.H