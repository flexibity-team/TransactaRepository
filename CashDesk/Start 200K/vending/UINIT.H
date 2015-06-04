#ifndef _UINIT_H
#define _UINIT_H
#include "umaindef.h"
#include <conio.h>
#include <string.h>

extern STRING20 operator_id;
extern WORD base_addr;
extern pBUF Command ;
extern pBUF Answer;
extern BYTE err;

int InitCom (int port);

#endif //UINIT.H