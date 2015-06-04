#ifndef _URESET_H
#define _URESET_H
#include "umaindef.h"
#include <conio.h>

int ResetCom(void);

extern WORD    base_addr;
extern BYTE    err;

#ifdef UCOMMAND_LOG
extern PCHAR LogPath;
#endif

#endif //URESET.H