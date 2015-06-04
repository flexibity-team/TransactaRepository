#include "main.h"
#include "umaindef.h"
#include "uinit.h"
#include "ureceive.h"
#include "usend.h"
#include "ureset.h"
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <dos.h>
#define LOG


STRING10 LogPath="file.log";
BYTE    command_id;
WORD    send_time;
WORD    wait_time;
WORD    base_addr;
pBUF    Command ;
pBUF    Answer;
BYTE    err;
STRING20 operator_id;

extern Init(int port);
extern Reset();

void DTR_ON(void)
{
  outp(base_addr+MCR, inp(base_addr+MCR) | 0x01);
}

void DTR_OFF(void)
{
  outp(base_addr+MCR, inp(base_addr+MCR) & 0xfe);
}


// поменять байты местами
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

// конвертируем строку в HEX
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
PCHAR Long2Asc(int Value, PCHAR StrP, BYTE Radix, BOOLEAN UpCase)
{
 PBYTE  Buff;
 BYTE  B;
 int   I,Num;
 PCHAR TmpP=StrP;

 Buff=malloc(sizeof(STRING34));
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
 free(Buff);
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

 aStr=malloc(sizeof(STRING4));

 aWord=Source&0x00ff;
 Byte2HexPChar((BYTE)aWord, aStr);
 Dest[0]=aStr[0]; Dest[1]=aStr[1];
 aWord=Source >> 8;
 Byte2HexPChar((BYTE)aWord, aStr);
 Dest[2]=aStr[0]; Dest[3]=aStr[1];
 Dest[4]=0;

 free(aStr);
}

// добавляем байт в строку
void FormByte2HexPChar(BYTE Source, PCHAR Dest)
{
  Byte2HexPChar(Source,&Dest[strlen(Dest)]);
  Dest[strlen(Dest)] = sDELIM;
}

// добавляем слово в строку
void FormWord2HexPChar(WORD Source, PCHAR Dest)
{
  Word2HexPChar(Source,&Dest[strlen(Dest)]);
  Dest[strlen(Dest)] = sDELIM;
}

// добавляем число в строку
void FormIntPChar(int Source, PCHAR Dest)
{
 PCHAR aStr;

  aStr=malloc(sizeof(STRING255));

  itoa(Source,aStr,10);
  if ( strlen(aStr) == 1) strcat(aStr,"0");
  strcat(Dest,aStr);
  free(aStr);

  Dest[strlen(Dest)] = sDELIM;
}

// добавляем строку в строку
void FormPChar(PCHAR Source, PCHAR Dest)
{
  strcat(Dest,Source);
  Dest[strlen(Dest)] = sDELIM;
}

// рассчет контрольной суммы
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
	PChar2HexWord((PCHAR)&(Answer[13]),4,(PWORD)&err);
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

// начало команды
void GetCommHeader(PCHAR S,BYTE Command)
{
 PCHAR S1;
 S1=malloc(sizeof(STRING2));
 *S=sSTART;
 strcat(S,"AERF");
 command_id++;
 if ((command_id<0x30)||(command_id>0x39)) command_id=0x30;
 S[strlen(S)] = command_id;
 Byte2HexPChar(Command,S1);
 strcat(S,S1);
 S[strlen(S)] = sDELIM;

 free(S1);
}

//конец команды
void GetCommTail(PCHAR S)
{
WORD BCC=0;

 if (S[(strlen(S))-1] != sDELIM) S[strlen(S)] = sDELIM;
 S[strlen(S)] = sSTOP;
 CalcBCC(S,&BCC);
 Word2HexPChar(BCC,(PCHAR)&S[strlen(S)]);
}

// получить время
void Get_Time(PCHAR S)
{
  time_t  timetuday;
  struct tm* t;
  PCHAR   S1;

  S1=malloc(sizeof(STRING10));

  timetuday= time(NULL);
  t=localtime(&timetuday);

  if (t->tm_hour<10) strcat(S,"0");
  strcat(S,itoa(t->tm_hour,S1,10));

  if (t->tm_min<10) strcat(S,"0");
  strcat(S,itoa(t->tm_min,S1,10));

  free(S1);
}

// получить дату
void Get_Date(PCHAR S)
{
  static time_t  timetuday;
  struct  tm* d;
  PCHAR   S1;

  S1=malloc(sizeof(STRING10));

  timetuday= time(NULL);
  d=localtime(&timetuday);

  if (d->tm_mday<10) strcat(S,"0");
  strcat(S,itoa(d->tm_mday,S1,10));

  if (d->tm_mon<9) strcat(S,"0");
  strcat(S,itoa(d->tm_mon+1,S1,10));

  itoa(d->tm_year,(PCHAR)S1,10);
  if (d->tm_year>99) strcpy(S1,(PCHAR)&(S1[1]));
  strcat(S,(PCHAR)S1);

  free(S1);
}

//получить дату и время команды
void GetDateTime(PCHAR S)
{
  Get_Date(S);
  S[strlen(S)] = sDELIM;
  Get_Time(S);
  S[strlen(S)] = sDELIM;
}

int Send_Receive(void)
{
/*
 err = WriteCom(Command,strlen(Command));
 if ((err) == 0)
 {
  err=ReadCom(Answer);
  if ((err) == 0)
   err=
*/
  err=XchgMessage(Command,strlen(Command),0);
 return(err);
}

// команда начало сеанса
int StartSeans(void)
{
 int i;
  memset(Command,0,sizeof(Command));
  GetCommHeader(Command,CM_Start);
  GetDateTime(Command);
  GetCommTail(Command);
  i=Send_Receive();
  return(i);
}

// команда X отчета
int XReport(void)
{
 int i;
  memset(Command,0,sizeof(Command));
  GetCommHeader(Command,CM_XReport);
  GetDateTime(Command);
  GetCommTail(Command);
  i=Send_Receive();
  return(i);
}

// Начало чека
int StartReceipt(void)
{
 int i;
  memset(Command,0,sizeof(Command));
  GetCommHeader(Command,CM_ReceiptStart);
  GetDateTime(Command);
  FormByte2HexPChar(00,Command); // продажа
  FormPChar("Петров",Command); // оператор
  FormPChar("",Command); //
  FormPChar("",Command); //
  FormByte2HexPChar(01,Command); // число копий
  FormPChar("",Command); //
  GetCommTail(Command);
  i=Send_Receive();
  return(i);
}

// строка чека
int ItemReceipt(void)
{
 int i;
  memset(Command,0,sizeof(Command));
  GetCommHeader(Command,CM_ReceiptItem);
  FormPChar("Товар",Command); //
  FormPChar("Артикул",Command); //
  FormPChar("250.11",Command); // сумма
  FormPChar("1",Command); // количество
  FormPChar("шт.",Command); // еденицы
  FormByte2HexPChar(01,Command); // индекс отдела
  FormPChar("Отдел",Command); // наименование отдела

  GetCommTail(Command);

  i=Send_Receive();
  return(i);
}

// Итог по чеку

int TotalReceipt(void)
{
 int i;
  memset(Command,0,sizeof(Command));
  GetCommHeader(Command,CM_ReceiptTotal);
  GetCommTail(Command);

  i=Send_Receive();
  return(i);
}

// расчет на чеке
int TenderReceipt(void)
{
 int i;
  memset(Command,0,sizeof(Command));
  GetCommHeader(Command,CM_ReceiptTender);
  FormByte2HexPChar(00,Command); // оплата наличными
  FormPChar("500",Command); // сумма
  FormPChar("",Command); //

  GetCommTail(Command);

  i=Send_Receive();
  return(i);
}
// закрытие чека
int CloseReceipt(void)
{
 int i;
  memset(Command,0,sizeof(Command));
  GetCommHeader(Command,CM_ReceiptClose);
  GetCommTail(Command);

  i=Send_Receive();
  return(i);
}

// аннулирование чека
int CancelReceipt(void)
{
 int i;
  memset(Command,0,sizeof(Command));
  GetCommHeader(Command,CM_ReceiptCancel);
  GetCommTail(Command);

  i=Send_Receive();
  return(i);
}

void main()
{
 InitCom(1); // для СОМ2
 printf("%d\n",StartSeans());
// printf("%d\n",XReport());
 printf("%d\n",StartReceipt());
 printf("%d\n",ItemReceipt());
 printf("%d\n",TotalReceipt());
 printf("%d\n",TenderReceipt());
 printf("%d\n",CloseReceipt());

 printf("%d\n",StartReceipt());
 printf("%d\n",CancelReceipt());


 ResetCom();
}
