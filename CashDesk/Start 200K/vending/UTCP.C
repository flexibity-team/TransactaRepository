#include "utcp.h"

int RdWrTCP( PCHAR buffer, WORD len, BYTE cmd )
{
  static WORD  bstruct[9] = {11,22,33,44,55,66,77,88,99};
  static CHAR  host [128];
  static union REGS       ireg, oreg;

  PWORD p_bstruct = bstruct;
  PCHAR p_host    = host;
  PBYTE p_t_buf   = buffer;
  PBYTE p_r_buf   = addr_com_byff->com_buffer;

  strcpy ( host, ComH.id_host );
  memset(addr_com_byff->com_buffer, 0x0, ARRAY_SIZE); /*(fmem)*/

#ifdef UCOMMAND_LOG
  WriteLog(buffer, len);
  if (!ISPrinter) WriteLog("\xd\xa", 2);
#endif

  bstruct[0] = FP_OFF (p_host);
  bstruct[1] = FP_SEG (p_host);
  bstruct[2] = FP_OFF (p_t_buf);
  bstruct[3] = FP_SEG (p_t_buf);
  bstruct[4] = FP_OFF (p_r_buf);
  bstruct[5] = FP_SEG (p_r_buf);
  bstruct[6] = 0; /* RC */

  if ((cmd & TCP_W) == TCP_W) bstruct[7] = len;
   else bstruct[7] = 0;

  if ((cmd & TCP_R) == TCP_R) bstruct[8] = ARRAY_SIZE;
   else bstruct[8] = 0;

  ireg.x.ax = 0xBB;
  ireg.x.bx = FP_OFF (p_bstruct);
  ireg.x.dx = FP_SEG (p_bstruct);

  int86 ( 0xE6, &ireg, &oreg );

  if ((cmd & TCP_R) == TCP_R)
   {
    addr_com_byff->com_count = bstruct[6] ? 0 : bstruct[8];
    strcpy((PCHAR)Answer,((PCHAR)(addr_com_byff->com_buffer)));
#ifdef UCOMMAND_LOG
     WriteLog((PCHAR)Answer, addr_com_byff->com_count);
#endif
   }

  tcp_err=(BYTE)bstruct[6];

  return (tcp_err);
}
