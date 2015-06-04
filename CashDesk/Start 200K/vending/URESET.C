#include "ureset.h"

int ResetCom (void)
{
  int tmp;
  BYTE i;

  err=OK;

 /*   outp(base_addr, 0x00);*/
  tmp=inp(base_addr);
  tmp=inp(base_addr+MSR);
  tmp=inp(base_addr+LSR);

  return(err);
}