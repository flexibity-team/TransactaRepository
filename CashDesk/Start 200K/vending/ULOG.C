#include "umaindef.h"

#ifdef LOG
#include "ulog.h"

void WriteLog(PCHAR buf, WORD len)
{
  WORD F;
  if (LogPath != NULL)
   {
    F = FOpen((PCHARFAR)&LogPath);
    if (F == 0xffff)
    {
     F = FCreate((PCHARFAR)&LogPath);
    }
    FSize(F);
    FWrite(F, (PCHARFAR)buf, len);
    FClose(F);
   }
}

//--------------------------------- FOpen ------------------------------------
WORD FOpen(PCHARFAR FName)
{
 asm {
  push  ds
  mov   ax,0x3D02
  lds   dx,FName
  int   0x21
  pop   ds
  mov   bx,ax
  jnc   L_Exit
  mov   bx,0xffff
 }

L_Exit:

 return _BX;
}

//-------------------------------- FCreate -----------------------------------
WORD FCreate(PCHARFAR FName)
{
 asm {
  push  ds
  mov   ah,0x3C
  mov   cx,0
  lds   dx,FName
  int   0x21
  pop   ds
  mov   bx,ax
  jnc   L_Exit
  mov   bx,0xffff
 }

L_Exit:

 return _BX;
}

#pragma warn -rvl
//--------------------------------- FSize ------------------------------------
DWORD FSize(WORD H)
{
 asm {
  mov   ax,0x4202
  mov   bx,H
  xor   cx,cx
  mov   dx,cx
  int   0x21
 }
}
#pragma warn +rvl

//--------------------------------- FWrite -----------------------------------
BOOLEAN FWrite(WORD H, PCHARFAR Buffer,WORD Bytes)
{
 asm {
  mov   si,0
  push  ds
  mov   ah,0x40
  mov   bx,H
  mov   cx,Bytes
  lds   dx,Buffer
  int   0x21
  pop   ds
  jc    L_Exit
  cmp   cx,ax
  jne   L_Exit
  mov   si,1
 }

L_Exit:

 return _SI;
}

//--------------------------------- FClose -----------------------------------
void FClose(WORD H)
{
 asm {
  mov   ah,0x3E
  mov   bx,H
  int   0x21
 }
}
#endif
