#include "uinit.h"

int InitCom(int port)
{
  PWORDFAR bios_addr= (PWORDFAR) 0x00400000L;
  BYTE tmp,i;

  err=NO_COM;
  base_addr=*(bios_addr+port);
  if (!base_addr) goto Quit;

  tmp=(base_addr+LCR);
  outp(base_addr+LCR, tmp | 0x80);

  tmp= 12;
  outp(base_addr,tmp);
  tmp= 0;
  outp(base_addr+BUD,tmp);
  outp(base_addr+LCR, 0x03);
  outp(base_addr+IER, 0);
  outp(base_addr+MCR, 0x01);

  tmp=inp(base_addr);
  tmp=inp(base_addr+MSR);
  tmp=inp(base_addr+LSR);
  tmp=inp(base_addr);

  strcpy(operator_id,"Terminal");
  err=OK;

Quit:
  return(err);
}