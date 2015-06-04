#include "usend.h"

int WriteCom( PCHAR buffer, WORD len )
{
  BYTE timeout;
  WORD i=0;
  static time_t start;

 /* printf("%s\r\n\r\n",buffer);*/

  memset(Answer, 0x0, sizeof(Answer)); /*(fmem)*/

#ifdef LOG
  WriteLog(buffer, len);
  WriteLog("\xd\xa", 2);
#endif

  start=time(NULL);

  timeout= send_time ? send_time:5;
  delay(2);
  DTR_OFF();

  if (len==0) return OK;
  do
  {
    if ((inp(base_addr+LSR) & 0x60) == 0x60)
    {
      if ((inp(base_addr+MSR) & 0x20) == 0x20)
      {
	  outp(base_addr,buffer[i++]);
	  start=time(NULL);
      }
    }
  }
  while ((i<len) && (start+timeout>time(NULL)));

//  DTR_ON();

  if (i!=len)
  {
    return ERR_STIMEOUT;
  }

  return OK;
}

