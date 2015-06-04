#include "utools.h"

/*
void step(PCHAR S)
{
 printf("%s",S);
 getch();
}
*/

void INIT_FIFO(void)
{
 outp(base_addr+SCR,0x07); /*FIFO enable D0, clear buff D1-D2, 1 byte per int D6-D7*/
}

void RESET_FIFO(void)
{
 outp(base_addr+SCR,0x00); /* FIFO stop*/
}

BOOLEAN UART_FIFO(void)
{
 int in_iir,save_scr,in_scr;

 /* test 8250 */
 save_scr=inp(base_addr+SCR);
 outp(base_addr+SCR,0x5a);
 in_scr=inp(base_addr+SCR);

 if (in_scr != 0x5a)
  {
   outp(base_addr+SCR,save_scr);
   return FALSE;
  }

 outp(base_addr+SCR,0xa5);
 in_scr=inp(base_addr+SCR);
 outp(base_addr+SCR,save_scr);

 if (in_scr != 0xa5) return FALSE;

 /* test 165X0X*/
 outp(base_addr+IIR,01);
 in_iir=inp(base_addr+IIR);

 if (in_iir & 0x40)
 {
/*  printf("FIFO !!!\r\n");*/
  return TRUE;
 }

 outp(base_addr+IIR,00);
 return FALSE;
}

void DTR_ON(void)
{
  outp(base_addr+MCR, inp(base_addr+MCR) | 0x01);
}

void DTR_OFF(void)
{
  outp(base_addr+MCR, inp(base_addr+MCR) & 0xfe);
}

WORD FieldCount(void)
{
 WORD  Count,i;
 PCHAR AddP;

 Count=0;i=0;
 AddP=(PCHAR)Answer;
 while (AddP[i]!=03)
 {
  while (AddP[i]!=0x1c) i++;
  Count++;
  i++;
 }
 return Count-1;
}

PCHAR GetField(PCHAR buf, BYTE n)
{
 int i=0,j=0;
 PCHAR tmp=(PCHAR)Answer;

 while (tmp[i]!=03)
 {
  while (tmp[i]!=0x1c) i++;
  i++;
  if (--n==0)
   {
    while ((tmp[i]!=0x1c) && (tmp[i]!=03)) buf[j++]=tmp[i++];
    return buf;
   }
 }
 buf[0]=0;
return buf;
}

/*
------------------------------ SwapBytes --------------------------------
Swap bytes in word
*/
WORD SwapBytes(PWORD W)
{
 static union REGS r;

 r.x.ax=*W;
 r.x.bx=r.x.ax;
 r.h.al=r.h.bh;
 r.h.ah=r.h.bl;
 *W=r.x.ax;
 return r.x.ax;

}

/*
---------------------------- PChar2HexWord ---------------------------------
Convert PChar to hexadecimal word;
*/
BOOLEAN PChar2HexWord(PCHAR Source, WORD Len, PWORD Result)
{
 WORD I;
 BYTE B;
 WORD Tmp;

 Tmp=0;
 if ((Len != 2) && (Len != 4)) return FALSE;
 for (I=0; I < Len; I++)
  {
   B=Source[I];
   if ((B >= '0') && (B <= '9')) B-='0';
   else
    {
     if ((B >= 'A') && (B <= 'F')) B-='A'-10;
     else
      {
       if ((B >= 'a') && (B <= 'f')) B-='a'-10;
       else return FALSE;
      }
    }
   Tmp=(Tmp << 4) + B;
  }

 if (Len == 4) *Result=SwapBytes(&Tmp);
 else *(PBYTE)Result=(BYTE)Tmp;

 return TRUE;
}

/*---------------------------- Long2Asc --------------------------------------
Convert LongInt to PChar
*/
PCHAR Long2Asc(LONGINT Value, PCHAR StrP, BYTE Radix, BOOLEAN UpCase)
{
 PBYTE  Buff;
 BYTE  B;
 int   I,Num;
 PCHAR TmpP=StrP;

 Buff=_xgrab(sizeof(STRING34));
/* If invalid radix then return empty PChar*/
 if ((Radix > 36) || (Radix < 2)) goto L_End;

 if (Value < 0)
  {
   *TmpP++='-';
   Value=-Value; /* Stay absolute*/
  }

/*
Now loop, taking each digit as modulo radix, and reducing the value
by dividing by radix, until the value is zeroed.  Note that
at least one loop occurs even if the value begins as 0,
since we want "0" to be generated rather than "".
*/

 Num=0;
 do
  {
   Buff[Num++]=Value % Radix;
   Value/=Radix;
  }
 while (Value != 0);
 Num--;

/*
The digits in the buffer must now be copied in reverse order into
the target string, translating to ASCII as they are moved.
*/

 for(I=Num; I >= 0; I--)
  {
   B=Buff[I];
   if (B < 10) B+='0';
   else
    if (UpCase == TRUE) B+='A'-10;
    else B+='a'-10;
   *TmpP++=B;
  }

L_End:

 *TmpP=0;       /* Finish PChar*/
 _xfree(Buff);
 return StrP;
}

/*
------------------------------ Byte2HexPChar ----------------------------
Convert byte to PChar in hexadecimal format
*/
void Byte2HexPChar(BYTE Source, PCHAR Dest)
{
 Long2Asc(Source,Dest,0x10,TRUE);
 if (Source < 0x10)
  {
   Dest[1]=Dest[0];
   Dest[0]='0';
   Dest[2]=0;
  }
}

/*------------------------------ Word2HexPChar ----------------------------
Convert word to PChar in hexadecimal format - LSB first !
*/
void Word2HexPChar(WORD Source, PCHAR Dest)
{
 PCHAR aStr;
 WORD aWord;

 aStr=_xgrab(sizeof(STRING4));

 aWord=Source&0x00ff;
 Byte2HexPChar((BYTE)aWord, aStr);
 Dest[0]=aStr[0]; Dest[1]=aStr[1];
 aWord=Source >> 8;
 Byte2HexPChar((BYTE)aWord, aStr);
 Dest[2]=aStr[0]; Dest[3]=aStr[1];
 Dest[4]=0;

 _xfree(aStr);
}


void FormByte2HexPChar(BYTE Source, PCHAR Dest)
{
  Byte2HexPChar(Source,Dest);
  Dest[strlen(Dest)] = sDELIM;
}

void FormWord2HexPChar(WORD Source, PCHAR Dest)
{
  Word2HexPChar(Source,Dest);
  Dest[strlen(Dest)] = sDELIM;
}

/*
----------------------------- CountBCC -------------------------------------
 ---Return true if BCC equal,
 ---BCC - real BCC
*/
BOOLEAN CalcBCC(PCHAR Source, PWORD BCC)
{
 WORD I;
 WORD CommandBCC;

 I=0;
 *BCC=2;
 CommandBCC=0;

 if (Source[I] != sSTART) return FALSE;
 do
  *BCC+=(BYTE)Source[++I];
 while (Source[I] != sSTOP);
 PChar2HexWord(&Source[I+1],4,&CommandBCC);
 return *BCC == CommandBCC;
}

/*
=================== GetCommHeader ================================}
*/
void GetCommHeader(PCHAR S,BYTE Command)
{
 PCHAR S1;
 S1=_xgrab(sizeof(STRING2));

 *S=sSTART;
 ComH.id_packet++;
 strcat(S,(PCHAR)&ComH.id_password);
 if (ComH.id_packet>0x39) ComH.id_packet=0x30;
 S[strlen(S)] = ComH.id_packet;
 Byte2HexPChar(Command,S1);
 strcat(S,S1);
 S[strlen(S)] = sDELIM;

 _xfree(S1);
}

/*
==================== GetCommTail =================================
*/
void GetCommTail(PCHAR S)
{
WORD BCC=0;

 if (S[(strlen(S))-1] != sDELIM) S[strlen(S)] = sDELIM;
 S[strlen(S)] = sSTOP;
 CalcBCC(S,&BCC);
 Word2HexPChar(BCC,(PCHAR)&S[strlen(S)]);
}

BOOLEAN TestInstall(void)
{
  if (strlen(ComH.id_operator)==0) return  FALSE;
  return (ComH.id_install == 'A');
}

/*
===================== GetTime ====================================
*/
void Get_Time(PCHAR S)
{
  static time_t  timetuday;
  struct  tm* t;
  PCHAR   S1;

  S1=_xgrab(sizeof(STRING10));

  timetuday= time(NULL);
  t=localtime(&timetuday);

  if (t->tm_hour<10) strcat(S,"0");
  strcat(S,itoa(t->tm_hour,S1,10));

  if (t->tm_min<10) strcat(S,"0");
  strcat(S,itoa(t->tm_min,S1,10));

  _xfree(S1);
}

/*
===================== GetDate ====================================
*/
void Get_Date(PCHAR S)
{
  static time_t  timetuday;
  struct  tm* d;
  PCHAR   S1;

  S1=_xgrab(sizeof(STRING10));

  timetuday= time(NULL);
  d=localtime(&timetuday);

  if (d->tm_mday<10) strcat(S,"0");
  strcat(S,itoa(d->tm_mday,S1,10));

  if (d->tm_mon<9) strcat(S,"0");
  strcat(S,itoa(d->tm_mon+1,S1,10));

  itoa(d->tm_year,(PCHAR)S1,10);
  if (d->tm_year>99) strcpy(S1,(PCHAR)&(S1[1]));
  strcat(S,(PCHAR)S1);

  _xfree(S1);
}
/*
===================== GetDateTime ====================================
*/
void GetDateTime(PCHAR S)
{
  Get_Date(S);
  S[strlen(S)] = sDELIM;
  Get_Time(S);
  S[strlen(S)] = sDELIM;
}

BYTE DoCommand(BYTE Command, PCHAR IParams)
{
 BYTE         I=1,Index=0,FR,FType;
 WORD         J=0;
 PCHAR        S,CurP,P,P1;
 WORD         W,InpNum;
 PTPos        P2;
 PFRecs71     P3;
 PSRecs74     P4;
 PPRecs74     P5;
 PDRecs60     P6;
 PARecs63     P7;

 if (Command>MaxCommand) return (BYTE)INVALID_COMMAND;
 while ((Index<=UserCommandNum) && (DescArray[Index].No != Command)) Index++;
 if (DescArray[Index].No != Command) return (BYTE)INVALID_COMMAND;

 S=_xgrab(sizeof(STRING));

 memset(S,0,sizeof(STRING));
 GetCommHeader(S,Command);

 if ((DescArray[Index].Md & 0x01) != 0) GetDateTime(S);

 if (((DescArray[Index].INum != 0) && (DescArray[Index].IPar == NULL)) ||
     ((DescArray[Index].ONum != 0) && (DescArray[Index].OPar == NULL)))
 {
   _xfree(S);
   return (BYTE)INVALID_COMMAND;
 }

 memset(PComBlock,0,ARRAY_SIZE);
 strcpy((PCHAR)PComBlock,(PCHAR)S);

 CurP=IParams;
 InpNum=DescArray[Index].INum;
 P1=(PCHAR)DescArray[Index].IPar;

 while (I <= InpNum)
 {
   if (J > DescArray[Index].INum) J=DescArray[Index].INum;
   FType=P1[J++];
   memset(S,0,sizeof(STRING));

switch (FType)
   {
     case P_B:
              {
                FormByte2HexPChar((BYTE)*(PBYTE)CurP,S);
                CurP += sizeof(BYTE);
                break;
              }

     case P_C:
              {
                S[strlen(S)]=*(PBYTE)CurP;
                CurP += sizeof(CHAR);
                S[strlen(S)] = sDELIM;
                break;
              }
     case P_W:
              {
                FormWord2HexPChar(*(PWORD)CurP,S);
                CurP += sizeof(WORD);
                break;
              }

     case P_S3:
     case P_S4:
     case P_S6:
     case P_S10:
     case P_S11:
     case P_S12:
     case P_S15:
     case P_S16:
     case P_S19:
     case P_S20:
     case P_S30:
     case P_S38:
     case P_S40:
     case P_S80:
              {

                switch (FType)
                  {
                    case P_S3: { W=sizeof(STRING3); break;}
                    case P_S4: { W=sizeof(STRING4); break;}
                    case P_S6: { W=sizeof(STRING6); break;}
                    case P_S10:{ W=sizeof(STRING10); break;}
                    case P_S11:{ W=sizeof(STRING11); break;}
                    case P_S12:{ W=sizeof(STRING12); break;}
                    case P_S15:{ W=sizeof(STRING15); break;}
                    case P_S16:{ W=sizeof(STRING16); break;}
                    case P_S19:{ W=sizeof(STRING19); break;}
                    case P_S20:{ W=sizeof(STRING20); break;}
                    case P_S30:{ W=sizeof(STRING30); break;}
                    case P_S38:{ W=sizeof(STRING38); break;}
                    case P_S40:{ W=sizeof(STRING40); break;}
                    case P_S80:{ W=sizeof(STRING80); break;}
                  };
                strncpy(S,CurP,W-1);
                S[strlen(S)] = sDELIM;
                CurP +=W; break;
              };

     case P_TP:
              {
                P2=(PTPos)CurP;
                FormWord2HexPChar(P2->Line,S);
                FormWord2HexPChar(P2->Col,&S[strlen(S)]);
                FormByte2HexPChar(P2->Font,&S[strlen(S)]);
                CurP +=sizeof(TPos);
                break;
              };
     case P_P:
              {
                P=(PCHAR)*(PDWORD)CurP;
                if (P!=NULL) strncat((PCHAR)PComBlock,P,ARRAY_SIZE-1);
                CurP +=sizeof(PCHAR);
                *S='\0'; break;
               }
   case P_DR60:
                {
                  P6=(PDRecs60)CurP;
                  for (FR=0;FR<P6->RecNum;FR++)
                  {
                    FormByte2HexPChar(P6->Recs[FR].DepNum,S);
                    strcat((PCHAR)S,((PCHAR)&(P6->Recs[FR].DepName)));
                    S[strlen(S)] = sDELIM;
                    strncat((PCHAR)PComBlock,S,ARRAY_SIZE-1);
                    memset(S,0,sizeof(STRING));
                  }
                  CurP += sizeof(PDRecs60);
                  *S='\0'; break;
                }
   case P_AR63:
                {
                  P7=(PARecs63)CurP;
                  for (FR=0;FR<P7->RecNum;FR++)
                  {
                    FormByte2HexPChar(P7->Recs[FR].ArtNum,S);
                    FormByte2HexPChar(P7->Recs[FR].ArtFlag,&S[strlen(S)]);
                    strcat((PCHAR)S,((PCHAR)&(P7->Recs[FR].ArtName)));
                    S[strlen(S)] = sDELIM;
                    strncat((PCHAR)PComBlock,S,ARRAY_SIZE-1);
                    memset(S,0,sizeof(STRING));
                  }
                  CurP += sizeof(TARecs63);
                  *S='\0'; break;
                }
   case P_FS74:
                {
                  P4=(PSRecs74)CurP;
                  FormByte2HexPChar(P4->RecNum,S);
                  strncat((PCHAR)PComBlock,S,ARRAY_SIZE-1);
                  for (FR=0;FR<P4->RecNum;FR++)
                  {
                    FormWord2HexPChar(P4->Recs[FR].SubNum,S);
                    strcat((PCHAR)S,((PCHAR)&(P4->Recs[FR].Sum)));
                    S[strlen(S)] = sDELIM;
                    strncat((PCHAR)PComBlock,S,ARRAY_SIZE-1);
                    memset(S,0,sizeof(STRING));
                  }
                  CurP += sizeof(TSRecs74);
                  *S='\0'; break;
                }
   case P_FP74:
                {
                  P5=(PPRecs74)CurP;
                  FormByte2HexPChar(P5->RecNum,S);
                  strncat((PCHAR)PComBlock,S,ARRAY_SIZE-1);
                  for (FR=0;FR<P5->RecNum;FR++)
                  {
                    FormWord2HexPChar(P5->Recs[FR].Line,S);
                    FormWord2HexPChar(P5->Recs[FR].Col,&S[strlen(S)]);
                    FormByte2HexPChar(P5->Recs[FR].Font,&S[strlen(S)]);
                    FormByte2HexPChar(P5->Recs[FR].PayMode,&S[strlen(S)]);
                    strcat((PCHAR)S,((PCHAR)&(P5->Recs[FR].PaySum)));
                    S[strlen(S)] = sDELIM;
                    strncat((PCHAR)PComBlock,S,ARRAY_SIZE-1);
                    memset(S,0,sizeof(STRING));
                  }
                  CurP += sizeof(TPRecs74);
                  *S='\0'; break;
                }

   case P_FR71:
                {
                  P3=(PFRecs71)CurP;
                  FormByte2HexPChar(P3->RecNum,S);
                  strncat((PCHAR)PComBlock,S,ARRAY_SIZE-1);
                  for (FR=0;FR<P3->RecNum;FR++)
                  {
                    FormWord2HexPChar(P3->Recs[FR].Line,S);
                    FormWord2HexPChar(P3->Recs[FR].Col,&S[strlen(S)]);
                    FormByte2HexPChar(P3->Recs[FR].Font,&S[strlen(S)]);
                    FormByte2HexPChar(P3->Recs[FR].PrintMode,&S[strlen(S)]);
                    FormByte2HexPChar(P3->Recs[FR].JourNo,&S[strlen(S)]);
                    strcat((PCHAR)S,((PCHAR)&(P3->Recs[FR].Info)));
                    S[strlen(S)] = sDELIM;
                    strncat((PCHAR)PComBlock,S,ARRAY_SIZE-1);
                    memset(S,0,sizeof(STRING));
                  }
                  CurP += sizeof(TFRecs71);
                  *S='\0'; break;
                }

   };
   if (strlen (S))
   {
     strncat((PCHAR)PComBlock,S,ARRAY_SIZE-1);
   };
   I++;
 };

 GetCommTail((PCHAR)PComBlock);
 _xfree(S);
 return OK;
}

int GetErrCode(BYTE Command)
{
  BYTE         Rep=0;
  WORD         CRC=0;
  WORD         wt_save=wait_time;

  do
    {
      memset((PCHAR)Answer,0,sizeof(ANSWER_BUFF));
      if (ReadCom((PCHAR)Answer)!=0) return (BYTE) ERR_RTIMEOUT;

      if (CalcBCC((PCHAR)Answer,&CRC))
      {
        PChar2HexWord((PCHAR)&(Answer->ErrorSt),4,(PWORD)&err);
        break;
      } else
      {
        if (Rep==10)
        {
          err=(BYTE) BAD_CONNECT;
          break;
        } else
        {
          if (Command != CM_FreeDocNF)
          {
           wait_time=5;
           WriteCom("\x15",1);
          } else
          {
           delay(1000);
           WriteCom("\x1b\x1b",2);
           ReadCom((PCHAR)Answer);
           err=0x22;
           break;
          }
        }
       Rep++;
      }
  }
  while (1);
  if (Rep!=0) wait_time=wt_save;
  return err;
}

int XchgMessageTCP(PCHAR buffer, WORD len)
{
 BYTE   Rep=0;
 err = (BYTE) BAD_CONNECT;
 do
 {
  Rep++;
  memset((PCHAR)Answer,0,sizeof(ANSWER_BUFF));
  if (RdWrTCP( buffer, len, TCP_W+TCP_R ) == 0)
   {
     PChar2HexWord((PCHAR)&(Answer->ErrorSt),4,(PWORD)&err);
     if (( err!=4) && (err!=0x22)) break; /*retry if communication wrong*/
   }
  }
  while (Rep!=5);

 return err;
}


int XchgMessage(PCHAR buffer, WORD len, WORD Command)
{
 BYTE  Rep=0;

 err = (BYTE) ERR_STIMEOUT;
 do
 {
  if (WriteCom(buffer,len)!=0) return err;
  err=GetErrCode((BYTE)Command);
  if (( err!=4) && (err!=0x22)) return err; /*retry if communication wrong*/
  Rep++;
 }
 while (Rep!=5);

 err = (BYTE) BAD_CONNECT;
 return err;
}

BYTE RunCommand(BYTE Command, PCHAR IParams)
{
 if (!TestInstall()) return (BYTE)INIT_REQUIRE;
 if ((err =(BYTE)DoCommand(Command, IParams))!=0) return err;
 if (base_addr == 555)  err = (BYTE)XchgMessageTCP((PCHAR)PComBlock,strlen((PCHAR)PComBlock)-4); /*without CRC*/
  else  err=(BYTE)XchgMessage((PCHAR)PComBlock,strlen((PCHAR)PComBlock),Command);

 return err;
}