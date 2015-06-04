#include "ureceive.h"
#ifdef LOG
#include "ulog.h"
#endif

int ReadCom(PCHAR buffer)
{
  WORD nRead=0;
  BOOLEAN fFind = FALSE;
  BYTE timeout;
  BYTE B,I,L;
  static time_t start;
  timeout= wait_time ? wait_time:40;
  start=time(NULL);

  DTR_ON();

  do
  {
   L=inp(base_addr+LSR);
   if ((L & 0x01) ==0x01)
    {
     DTR_OFF();
     B=inp(base_addr);
     if (!(L & 0x1E))
     {
      buffer[nRead++]=B;
      start = time(NULL);
      timeout = 2;
     }
     DTR_ON();
    }

    if (nRead>=0x05)
    {
      if (buffer[nRead-0x5] == 0x03) fFind=TRUE;
    }
  }
  while ((!fFind) && (start+timeout>time(NULL)));

#ifdef LOG
  WriteLog((PCHAR)(buffer), nRead);
  WriteLog("\xd\xa", 2);
#endif

  if ((!fFind) && (nRead==0))
  {
 /* printf("ТАЙМАУТ !!! \r\n Успешно приняли: %d\r\n%Fs\r\n",addr_com_byff.com_count,addr_com_byff.com_buffer);*/
    return ERR_RTIMEOUT;
  }
  /*printf("Успешно приняли: %d\r\n%Fs\r\n",addr_com_byff->com_count,addr_com_byff->com_buffer);*/
  return OK;
}