#include "ucommand.h"

/*Begin Session (if use (default - USE))*/
CLIPPER Session(void)
{
  _retni (RunCommand(CM_Start,0));
}

/*Get resources */
CLIPPER Resources(void)
{
  _retni (RunCommand(CM_Resources,0));
}

/*Shift Open Routine
IParams - String 255 characters with nullterminated
 simvol "|" - div string on lines
*/

CLIPPER ShiftOpen(void)
{
  StartShiftIT StartShift;

  StartShift.AdditP=_parc(1);
  if (PCOUNT==2) strncpy(ComH.id_operator,_parc(2),16);
 _retni (RunCommand(CM_StartShift,(PCHAR)&StartShift));
}

/* Set Operator Name*/
CLIPPER SetOper(void)
{
  strncpy(ComH.id_operator,_parc(1),16);
}

/* Set Host Name for TCP *21.09.2000**/
CLIPPER SetHost(void)
{
  strncpy(ComH.id_host,_parc(1),127);
}

/* Get Current operator*/
CLIPPER GetOper(void)
{
 _retc(ComH.id_operator);
}

/*Shift Close Routine*/
CLIPPER ShiftClose(void)
{
 _retni (RunCommand(CM_ZReport,0));
}

/*Shift Close Routine with mony*/
CLIPPER ShiftCloseM(void)
{
 int   err;
 STRING19 tmpS;
 PCHAR tmp=tmpS;

 memset(tmp,0,sizeof(STRING19));
 if ((err=RunCommand(CM_Report2Host,0))==0)
 {
  if (strlen(GetField(tmp,23))!=0)
  {
   if ((err=RunCommand(CM_FromCash,GetField(tmp,23)))==0)
   {
    err=RunCommand(CM_ZReport,0);
   }
  } else err=(BYTE)FIELD_EMPTY;
 }

 _retni (err);
}

/*Print X report*/
CLIPPER XReport(void)
{
 _retni (RunCommand(CM_XReport,0));
}

/*Put money*/
CLIPPER ToCash(void)
{
 _retni (RunCommand(CM_ToCash,_parc(1)));
}

/*Get money*/
CLIPPER FromCash(void)
{
 _retni (RunCommand(CM_FromCash,_parc(1)));
}

/*Make free fiscal doc routines
  Open fiscal doc
  int OpenFiscalDoc(BYTE DocType,BYTE PayType,BYTE FlipFOffs,BYTE PageNum,BYTE HCopyNum,
                       BYTE VCopyNum,WORD LOffs,WORD VGap,BYTE LGap,PCHAR  Sum)*/

CLIPPER OpenFDoc(void)

{

 err=(BYTE) INIT_REQUIRE;
 if (!TestInstall()) goto Quit;

 err=(BYTE) ALLREADY_OPEN;
 if (ComH.id_fiscaldoc=='A') goto Quit;

 memset(PFreeDocSIT,0,sizeof(FreeDocSIT));

 PFreeDocSIT->DocType=(BYTE) _parni(1);
 PFreeDocSIT->PayType=(BYTE) _parni(2);
 PFreeDocSIT->FlipFOffs=(BYTE) _parni(3);
 PFreeDocSIT->PageNum=(BYTE) _parni(4);
 PFreeDocSIT->HCopyNum=(BYTE) _parni(5);
 PFreeDocSIT->VCopyNum=(BYTE) _parni(6);
 PFreeDocSIT->LOffs=(WORD) _parni(7);
 PFreeDocSIT->VGap=(WORD) _parni(8);
 PFreeDocSIT->LGap=(BYTE) _parni(9);

 strcpy(PFreeDocSIT-> OperName,ComH.id_operator);

 strcpy(PFreeDocSIT->Sum,_parc(10));

 PFreeDocSIT->FreeRecs.RecNum=0;
 ComH.id_fiscaldoc='A';
 err=(BYTE) OK;

Quit:
 _retni (err);
}

/*int AddPosField(WORD SerNoLine, WORD SerNoCol, int SerNoFont,
                     WORD DocNoLine, WORD DocNoCol, int DocNoFont,
                     WORD DateLine,  WORD DateCol,  int DateFont,
                     WORD TimeLine,  WORD TimeCol,  int TimeFont,
                     WORD InnLine,   WORD InnCol,   int InnFont,
                     WORD OperLine,  WORD OperCol,  int OperFont,
                     WORD SumLine,   WORD SumCol,   int SumFont)
*/
CLIPPER AddFPos(void)
{

 err=(BYTE) NOT_OPEN;
 if (ComH.id_fiscaldoc!='A') goto Quit;

 PFreeDocSIT->SerNoPos.Line=(WORD) _parni(1,1);
 PFreeDocSIT->SerNoPos.Col=(WORD)  _parni(1,2);
 PFreeDocSIT->SerNoPos.Font=(BYTE) _parni(1,3);

 PFreeDocSIT->DocNoPos.Line=(WORD) _parni(2,1);
 PFreeDocSIT->DocNoPos.Col=(WORD)  _parni(2,2);
 PFreeDocSIT->DocNoPos.Font=(BYTE) _parni(2,3);

 PFreeDocSIT->DatePos.Line=(WORD)  _parni(3,1);
 PFreeDocSIT->DatePos.Col=(WORD)   _parni(3,2);
 PFreeDocSIT->DatePos.Font=(BYTE)  _parni(3,3);

 PFreeDocSIT->TimePos.Line=(WORD)  _parni(4,1);
 PFreeDocSIT->TimePos.Col=(WORD)   _parni(4,2);
 PFreeDocSIT->TimePos.Font=(BYTE)  _parni(4,3);

 PFreeDocSIT->InnPos.Line=(WORD)   _parni(5,1);
 PFreeDocSIT->InnPos.Col=(WORD)    _parni(5,2);
 PFreeDocSIT->InnPos.Font=(BYTE)   _parni(5,3);

 PFreeDocSIT->OperPos.Line=(WORD)  _parni(6,1);
 PFreeDocSIT->OperPos.Col=(WORD)   _parni(6,2);
 PFreeDocSIT->OperPos.Font=(BYTE)  _parni(6,3);

 PFreeDocSIT->SumPos.Line=(WORD)   _parni(7,1);
 PFreeDocSIT->SumPos.Col=(WORD)    _parni(7,2);
 PFreeDocSIT->SumPos.Font=(BYTE)   _parni(7,3);

 ComH.id_fiscaldoc='B';
 err=(BYTE) OK;

Quit:
 _retni (err);
}

/*Added one field
int AddFreeField(WORD Line,WORD Col,BYTE Font,BYTE PrintMode,BYTE JourNo,PCHAR Info)*/
CLIPPER AddFField(void)
{
 int         err;
 PFreeRec71  tmp_rec;

 err=(BYTE) NOT_OPEN;
 if (ComH.id_fiscaldoc!='B') goto Quit;

 err=(BYTE) NO_MEMORY;
 if ( PFreeDocSIT->FreeRecs.RecNum==FIELD_MAX)  goto Quit;

 tmp_rec=_xgrab(sizeof(TFreeRec71));

 tmp_rec->Line=(WORD)_parni(1);
 tmp_rec->Col=(WORD) _parni(2);
 tmp_rec->Font=(BYTE) _parni(3);
 tmp_rec->PrintMode=(BYTE)_parni(4);
 tmp_rec->JourNo=(BYTE) _parni(5);

 strcpy(tmp_rec->Info,_parc(6));

 PFreeDocSIT->FreeRecs.Recs[PFreeDocSIT->FreeRecs.RecNum]=*tmp_rec;
 PFreeDocSIT->FreeRecs.RecNum++;
 err=(BYTE) OK;

 _xfree(tmp_rec);
Quit:
 _retni (err);
}

/*Print fical doc to Slip*/
CLIPPER PrintFSlip(void)
{
 err=(BYTE) NOT_OPEN;
 if ((ComH.id_fiscaldoc!='A')&&(ComH.id_fiscaldoc!='B')) goto Quit;
 err=RunCommand(CM_FreeDoc,(PCHAR)PFreeDocSIT);
 ComH.id_fiscaldoc='N';

Quit:
 _retni (err);
}

/*Print fical doc to Receipt*/
CLIPPER PrintFRec(void)
{

 err= (BYTE) NOT_OPEN;
 if ((ComH.id_fiscaldoc!='A')&&(ComH.id_fiscaldoc!='B'))  goto Quit;
 err=RunCommand(CM_FreeReceipt,(PCHAR)PFreeDocSIT);
 ComH.id_fiscaldoc='N';

Quit:
 _retni (err);
}

/*Make free fiscal doc routines*/

CLIPPER OpenFDoc74(void)
{

 err=(BYTE) INIT_REQUIRE;
 if (!TestInstall()) goto Quit;

 err=(BYTE) ALLREADY_OPEN;
 if (ComH.id_fiscaldoc=='A') goto Quit;

 memset(PFreeDocDIT,0,sizeof(FreeDocDIT));

 PFreeDocDIT->DocType=(BYTE) _parni(1);
 PFreeDocDIT->FlipFOffs=(BYTE) _parni(2);
 PFreeDocDIT->PageNum=(BYTE) _parni(3);
 PFreeDocDIT->HCopyNum=(BYTE) _parni(4);
 PFreeDocDIT->VCopyNum=(BYTE) _parni(5);
 PFreeDocDIT->LOffs=(WORD) _parni(6);
 PFreeDocDIT->VGap=(WORD) _parni(7);
 PFreeDocDIT->LGap=(BYTE) _parni(8);
 PFreeDocDIT->DepartNum=(BYTE) _parni(9);
 PFreeDocDIT->ArticlesNum=(BYTE) _parni(10);

 strcpy(PFreeDocDIT->OperName,ComH.id_operator);

 strcpy(PFreeDocDIT->Sum,_parc(11));

 PFreeDocDIT->FreeRecs.RecNum=0;
 PFreeDocDIT->PayRecs.RecNum=0;
 PFreeDocDIT->SubRecs.RecNum=0;

 ComH.id_fiscaldoc='A';
 err=(BYTE) OK;

Quit:
 _retni (err);
}

/*int AddPosField(WORD SerNoLine, WORD SerNoCol, int SerNoFont,
                     WORD DocNoLine, WORD DocNoCol, int DocNoFont,
                     WORD OperNoLine, WORD OperNoCol, int OperNoFont,
                     WORD DateLine,  WORD DateCol,  int DateFont,
                     WORD TimeLine,  WORD TimeCol,  int TimeFont,
                     WORD InnLine,   WORD InnCol,   int InnFont,
                     WORD DepartLine,  WORD DepartCol,  int DepartFont,
                     WORD ArticleLine,   WORD ArticleCol,   int ArticleFont,
                     WORD OperLine,  WORD OperCol,  int OperFont,
                     WORD SumLine,   WORD SumCol,   int SumFont)
*/
CLIPPER AddFPos74(void)
{

 err=(BYTE) NOT_OPEN;
 if (ComH.id_fiscaldoc!='A') goto Quit;

 PFreeDocDIT->SerNoPos.Line=(WORD)      _parni(1,1);
 PFreeDocDIT->SerNoPos.Col=(WORD)       _parni(1,2);
 PFreeDocDIT->SerNoPos.Font=(BYTE)      _parni(1,3);

 PFreeDocDIT->DocNoPos.Line=(WORD)      _parni(2,1);
 PFreeDocDIT->DocNoPos.Col=(WORD)       _parni(2,2);
 PFreeDocDIT->DocNoPos.Font=(BYTE)      _parni(2,3);

 PFreeDocDIT->OperNoPos.Line=(WORD)     _parni(3,1);
 PFreeDocDIT->OperNoPos.Col=(WORD)      _parni(3,2);
 PFreeDocDIT->OperNoPos.Font=(BYTE)     _parni(3,3);

 PFreeDocDIT->DatePos.Line=(WORD)       _parni(4,1);
 PFreeDocDIT->DatePos.Col=(WORD)        _parni(4,2);
 PFreeDocDIT->DatePos.Font=(BYTE)       _parni(4,3);

 PFreeDocDIT->TimePos.Line=(WORD)       _parni(5,1);
 PFreeDocDIT->TimePos.Col=(WORD)        _parni(5,2);
 PFreeDocDIT->TimePos.Font=(BYTE)       _parni(5,3);

 PFreeDocDIT->InnPos.Line=(WORD)        _parni(6,1);
 PFreeDocDIT->InnPos.Col=(WORD)         _parni(6,2);
 PFreeDocDIT->InnPos.Font=(BYTE)        _parni(6,3);

 PFreeDocDIT->DepartDocPos.Line=(WORD)  _parni(7,1);
 PFreeDocDIT->DepartDocPos.Col=(WORD)   _parni(7,2);
 PFreeDocDIT->DepartDocPos.Font=(BYTE)  _parni(7,3);

 PFreeDocDIT->ArticlesPos.Line=(WORD)   _parni(8,1);
 PFreeDocDIT->ArticlesPos.Col=(WORD)    _parni(8,2);
 PFreeDocDIT->ArticlesPos.Font=(BYTE)   _parni(8,3);

 PFreeDocDIT->OperPos.Line=(WORD)       _parni(9,1);
 PFreeDocDIT->OperPos.Col=(WORD)        _parni(9,2);
 PFreeDocDIT->OperPos.Font=(BYTE)       _parni(9,3);

 PFreeDocDIT->SumPos.Line=(WORD)        _parni(10,1);
 PFreeDocDIT->SumPos.Col=(WORD)         _parni(10,2);
 PFreeDocDIT->SumPos.Font=(BYTE)        _parni(10,3);

 ComH.id_fiscaldoc='B';
 err=(BYTE) OK;

Quit:
 _retni (err);
}

/* Add Pay to command*/
CLIPPER AddPay74(void)
{
 int         err,i,count;
 PPayRec74   tmp_pay;

 err=(BYTE) NOT_OPEN;
 if (ComH.id_fiscaldoc!='B') goto Quit;

 err=(BYTE) NO_MEMORY;
 if (PFreeDocDIT->PayRecs.RecNum==5)  goto Quit;

 count=_parinfo(0);
 if (count+PFreeDocDIT->PayRecs.RecNum>5)  goto Quit;

 tmp_pay=_xgrab(sizeof(TPayRec74));

 for (i=1;i<=count;i++)
 {
   tmp_pay->Line=    (WORD) _parni(i,1);
   tmp_pay->Col=     (WORD) _parni(i,2);
   tmp_pay->Font=    (BYTE) _parni(i,3);
   tmp_pay->PayMode= (BYTE) _parni(i,4);
   strcpy(tmp_pay->PaySum,  _parc(i,5));

   PFreeDocDIT->PayRecs.Recs[PFreeDocDIT->PayRecs.RecNum]=*tmp_pay;
   PFreeDocDIT->PayRecs.RecNum++;
   memset(tmp_pay,0,sizeof(TPayRec74));
 }

 _xfree(tmp_pay);
 ComH.id_fiscaldoc='C';
 err=(BYTE) OK;

Quit:
 _retni (err);
}

/* Add SubDep to command*/
CLIPPER AddSubDep74(void)
{
 int           err, i, count;
 PSubRec74     tmp_dep;

 err=(BYTE) NOT_OPEN;
 if (ComH.id_fiscaldoc!='C') goto Quit;

 err=(BYTE) NO_MEMORY;
 if (PFreeDocDIT->SubRecs.RecNum==5)  goto Quit;

 count=_parinfo(0);
 if (count+PFreeDocDIT->SubRecs.RecNum>5)  goto Quit;

 tmp_dep=_xgrab(sizeof(TSubRec74));

 for (i=1;i<=count;i++)
 {
   tmp_dep->SubNum= (BYTE) _parni(i,1);
   strcpy(tmp_dep->Sum,    _parc(i,2));

   PFreeDocDIT->SubRecs.Recs[PFreeDocDIT->SubRecs.RecNum]=*tmp_dep;
   PFreeDocDIT->SubRecs.RecNum++;
   memset(tmp_dep,0,sizeof(TSubRec74));
 }

 _xfree(tmp_dep);
 err=(BYTE) OK;

Quit:
 _retni (err);
}

/*Added one field
int AddFreeField(WORD Line,WORD Col,BYTE Font,BYTE PrintMode,BYTE JourNo,PCHAR Info)*/
CLIPPER AddField74(void)
{
 int         err, i, count;
 PFreeRec71  tmp_rec;

 err=(BYTE) NOT_OPEN;
 if (ComH.id_fiscaldoc!='C') goto Quit;

 err=(BYTE) NO_MEMORY;
 if ( PFreeDocDIT->FreeRecs.RecNum==FIELD_MAX)  goto Quit;

 count=_parinfo(0);
 if (count+PFreeDocDIT->FreeRecs.RecNum>FIELD_MAX)  goto Quit;

 tmp_rec=_xgrab(sizeof(TFreeRec71));

 for (i=1;i<=count;i++)
 {
   tmp_rec->Line=(WORD)_parni(i,1);
   tmp_rec->Col=(WORD) _parni(i,2);
   tmp_rec->Font=(BYTE) _parni(i,3);
   tmp_rec->PrintMode=(BYTE)_parni(i,4);
   tmp_rec->JourNo=(BYTE) _parni(i,5);

   strcpy(tmp_rec->Info,_parc(i,6));

   PFreeDocDIT->FreeRecs.Recs[PFreeDocDIT->FreeRecs.RecNum]=*tmp_rec;
   PFreeDocDIT->FreeRecs.RecNum++;
   memset(tmp_rec,0,sizeof(TFreeRec71));
 }
 err=(BYTE) OK;

 _xfree(tmp_rec);

Quit:
 _retni (err);
}

/*Print fical doc to Slip*/
CLIPPER PrintFS74(void)
{
 err=(BYTE) NOT_OPEN;

 if (ComH.id_fiscaldoc!='C') goto Quit;
 err=RunCommand(CM_DepDocSlip,(PCHAR)PFreeDocDIT);

Quit:
 ComH.id_fiscaldoc='N';
 _retni (err);
}

/*Print fical doc to Receipt*/
CLIPPER PrintFR74(void)
{

 err= (BYTE) NOT_OPEN;
 if (ComH.id_fiscaldoc!='C')  goto Quit;
 err=RunCommand(CM_DepDocReceipt,(PCHAR)PFreeDocDIT);

Quit:
 ComH.id_fiscaldoc='N';
 _retni (err);
}

CLIPPER CancelFDoc(void)
{
 ComH.id_fiscaldoc='N';
 _retni (OK);
}

/*Print electronical journal (if use)*/
CLIPPER PrintEJ(void)
{
 _retni (RunCommand(CM_PrintEJournal,0));
}

/*Open Free not fiscal doc (you use EPSON commands) SERIES !!!
 FreeDocOpen ()*/
CLIPPER FDocOpen(void)
{
 if (!ISPrinter) err=RunCommand(CM_FreeDocNF,0);
 if (err==OK) ISPrinter = TRUE;
 _retni (err);
}

/*Print Free not fiscal doc
  int FDocPrint(PCHAR IParams, WORD ILen)*/
CLIPPER FDocPrint(void)
{
 WORD  ESC=0x1b1b;
 err=(BYTE) NOT_OPEN;
 if (ISPrinter)
 {
  if ((_parni(2) != 0) && (*((PWORD)&(_parc(1)[_parni(2)-2])) == ESC))
  {
   if (base_addr == 555) err=XchgMessageTCP(_parc(1),(WORD)_parni(2));
    else
     err=XchgMessage(_parc(1),(WORD)_parni(2),0);
   ISPrinter=FALSE;
  }
   else
    if (base_addr == 555) err=RdWrTCP(_parc(1),(WORD)_parni(2),TCP_W);
     else
      err=WriteCom(_parc(1),(WORD)_parni(2));
 }

 _retni(err);
}

/*Close Free not fiscal doc
  FDocClose()*/
CLIPPER FDocClose(void)
{
 WORD  ESC=0x1b1b;

 err=(BYTE) NOT_OPEN;
 if (ISPrinter)
 {
  if (base_addr == 555)  err=XchgMessageTCP((PCHAR)&ESC,2);
   else
    err=XchgMessage((PCHAR)&ESC,2,0);
  ISPrinter=FALSE;
 }
 _retni (err);
}

/*Print Free not fiscal doc (you use EPSON commands)
  int PrintFreeDoc(PCHAR IParams, WORD ILen)*/
CLIPPER PrintFDoc(void)
{
 WORD  ESC=0x1b1b;
 WORD  len=2;
 PCHAR buf=(PCHAR)&ESC;

 if ((err=RunCommand(CM_FreeDocNF,0))!=0) goto Quit;

 if ((_parni(2) != 0) && (*((PWORD)&(_parc(1)[_parni(2)-2])) == ESC))
  {
   buf=_parc(1);
   len=(WORD)_parni(2);
  }

 if (base_addr == 555) err=XchgMessageTCP(buf,len);
  else
   err=XchgMessage(buf,len,0);

Quit:
 _retni (err);
}

CLIPPER SetDeparts(void)
{
 int i, count;
 PDRecs60 tmp_dep;

 tmp_dep=_xgrab(sizeof(TDRecs60));
 memset(tmp_dep,0,sizeof(TDRecs60));
 count=_parinfo(0);

 for (i=1;i<=count;i++)
 {
  tmp_dep->Recs[i-1].DepNum = (BYTE) _parni(i,1);
  strcpy(tmp_dep->Recs[i-1].DepName, _parc(i,2));
 }

 tmp_dep->RecNum=count;
 err=RunCommand(CM_SetDepart,(PCHAR)tmp_dep);
 _xfree(tmp_dep);

 _retni (err);
}

/*
Get Departments
int GetDepart()
*/
CLIPPER GetDepart(void)
{
 _retni (RunCommand(CM_GetDepart,0));
}

CLIPPER SetArticls(void)
{
 int i, count;
 PArticles tmp_art;

 tmp_art=_xgrab(sizeof(TArticles));
 memset(tmp_art,0,sizeof(TArticles));
 count=_parinfo(0);

 tmp_art->DepNum=(BYTE) _parni(1);

 for (i=2;i<=count;i++)
 {
  tmp_art->ArtRecs.Recs[i-2].ArtNum = (BYTE) _parni(i,1);
  tmp_art->ArtRecs.Recs[i-2].ArtFlag = (BYTE) _parni(i,2);
  strcpy(tmp_art->ArtRecs.Recs[i-2].ArtName, _parc(i,3));
 }

 tmp_art->ArtRecs.RecNum=count-1;
 err=RunCommand(CM_SetArticl,(PCHAR)tmp_art);
 _xfree(tmp_art);

 _retni (err);
}

/*
Get Articles
int GetArticles()
*/
CLIPPER GetArticles(void)
{
 _retni (RunCommand(CM_GetArticl,0));
}

/*Set Time Routine*/
CLIPPER SetTime(void)
{
 _retni (RunCommand(CM_SetDateTime,0));
}

/*Set Time Routine*/
CLIPPER SetDTime(void)
{
 _retni (RunCommand(CM_SetDateTime,0));
}

/*
Set Pasram Doc
int SetParam(WORD Misk1,WORD Misk2, WORD Timeout)
*/
CLIPPER SetParam(void)
{
 ParamsT source;

  source.MiscParams1=(WORD) _parni(1);
  source.MiscParams2=(WORD) _parni(2);
  source.SlipTimeOut=(WORD) _parni(3);

 _retni (RunCommand(CM_SetDocSettings,(PCHAR)&source));
}

/*
Get Param Doc
int GetParam()
*/
CLIPPER GetParam(void)
{
 _retni (RunCommand(CM_GetDocSettings,0));
}

/*
Set Driver param
int SetDrvParam(BYTE OnTime,BYTE OffTime, BYTE Misk)
*/
CLIPPER SetDrvParam(void)
{
 DrawerParamsT source;

  source.OnTime=(BYTE) _parni(1);
  source.OffTime=(BYTE) _parni(2);
  source.MiscParams=(BYTE) _parni(3);

 _retni (RunCommand(CM_SetDrawerTime,(PCHAR)&source));
}

/*
Get Param Doc
int GetParam()
*/
CLIPPER GetDrvParam(void)
{
 _retni (RunCommand(CM_GetDrawerTime,0));
}

/*
Make and transmit All FR commands
int RunCmdC(BYTE Command, PCHAR IParams)
*/
CLIPPER RunCmdC(void)
{
 _retni (RunCommand((BYTE) _parni(1), _parc(2)));
}

/*
int InitDisplay (HOST connection)
*/
CLIPPER InitDispC(void)
{
 int   err;
 PCHAR DMode  = "\033=\002";
 PCHAR PMode  = "\033=\001";
 PCHAR PInit  = "\033@";

 err=(BYTE)INIT_REQUIRE;
 if (!TestInstall()) goto Quit;

 err=(BYTE) OK;
 if (base_addr == 555) goto Quit;

 err=ERR_STIMEOUT;
 if (WriteCom(DMode,3)==ER_CommandOk)
 {
  if (WriteCom(PInit,2)==ER_CommandOk)
  {
    err=WriteCom(PMode,3);
  }
 }

Quit:
 _retni (err);
}

/*
int ShowDisplay (HOST connection)
*/
CLIPPER ShowDispH(void)
{
 int   err;
 WORD  StrLen;
 PCHAR StrP;
 PCHAR DMode  = "\033=\002";
 PCHAR PMode  = "\033=\001";
 PCHAR PClear = "\014";

 err=(BYTE)INIT_REQUIRE;
 if (!TestInstall()) goto Quit;

 err=(BYTE) OK;
 if (base_addr == 555) goto Quit;

 err=ERR_STIMEOUT;
 StrLen =(WORD) _parni(2);
 StrP=_parc(1);

 if(StrLen==0)
 {
  StrLen =1;
  StrP=PClear;
 }
 if (WriteCom(DMode,3)==ER_CommandOk)
 {
  if (WriteCom(StrP,StrLen)==ER_CommandOk)
  {
    err=WriteCom(PMode,3);
  }
 }

Quit:
 _retni (err);
}

/* Show message to DISPLAI (printer connection)
  int FDocPrint(PCHAR IParams, WORD ILen)*/
CLIPPER ShowDispP(void)
{
 PCHAR buf;
 PCHAR PEsc="\x1b\x1b";
 PCHAR DMode="\x1b=\x02";
 PCHAR PMode="\x1b=\x01";
 PCHAR PClear = "\x0c";
 WORD  len;

 buf=_xgrab(3+1+_parni(2)+3+2+1);
 memset(buf,0,3+1+_parni(2)+3+2+1);

 strcpy(buf,DMode);
 strcat(buf,PClear);
 strncat(buf,_parc(1),_parni(2));
 strcat(buf,PMode);
 strcat(buf,PEsc);
 len=strlen(buf);

 if ((err=RunCommand(CM_FreeDocNF,0))!=0) goto Quit;

 if (base_addr == 555) err=XchgMessageTCP(buf,len);
  else
   err=XchgMessage(buf,len,0);

Quit:
 _xfree(buf);
 _retni(err);
}

/*
int WriteComC(PCHAR buffer , WORD len )
*/
CLIPPER WriteComC(void)
{
 int  err;
 WORD len;

 err=(BYTE)INIT_REQUIRE;
 if (!TestInstall()) goto Quit;

 len =(WORD) _parni(2);
 err=WriteCom(_parc(1),len);

Quit:
 _retni (err);
}

/*
int ReadComC(PCHAR buffer)
*/
CLIPPER ReadComC(void)
{

 err=(BYTE)INIT_REQUIRE;
 if (!TestInstall()) goto Quit;

  err=ReadCom(_parc(1));

Quit:
 _retni (err);
}

/*
int ReadLastErr()
*/
CLIPPER ReadLastErr(void)
{
 PCHAR tmp="\x15";

 err=(BYTE) INIT_REQUIRE;
 if (!TestInstall()) goto Quit;
 err=(BYTE)XchgMessage(tmp,1,0);
 if (Answer->Packet!=ComH.id_packet) err=(BYTE) BAD_CONNECT;

Quit:
 _retni (err);
}

CLIPPER SetHeader(void)
{
 int              err;
 BYTE             i,count,hsize,cod;
 SetHeaderIT      OldHeaderDoc;
 SetHeaderNewIT   NewHeaderDoc;
 SetTailIT        TailDoc;
 PCHAR            HeaderDoc;

/* Первый байт - тип команды (0-old,1-new,2-tail)*/

 switch ((BYTE)_parni(1))
 {
  case 1: {
           count=6; cod=CM_SetHeaderNew;
           hsize=sizeof(STRING40);
           (PCHAR)HeaderDoc=(PCHAR)&NewHeaderDoc;
           break;
          }
  case 2:
          {
           count=4;cod=CM_SetTail;
           hsize=sizeof(STRING40);
           (PCHAR)HeaderDoc=(PCHAR)&TailDoc;
           break;
          }
  default:
          {
           count=4;cod=CM_SetHeader;
           hsize=sizeof(STRING38);
           (PCHAR)HeaderDoc=(PCHAR)&OldHeaderDoc;
           break;
          }
 }

/* Чистим структурy */

 memset(HeaderDoc,0,hsize*count);

/* Заполняем структуру */

 for (i=0;i<count;i++)
 {
 strncpy((PCHAR)&HeaderDoc[i*hsize],_parc(i+2),hsize-1);
 }
/* Передаем сформированную команду*/

 _retni (RunCommand(cod,HeaderDoc));
}

/*Set Time Routine*/
CLIPPER GetVersion(void)
{
 int ver;
 ver=1;
 _retni (ver);
}

/*
Set Answer Wait time
int SetATime(WORD Timeout)
*/
CLIPPER SetATime(void)
{
  wait_time=(WORD) _parni(1);
 _retni (0);
}

/*Get Answer Wait time*/
CLIPPER GetTimeAnswer(void)
{
 _retni (wait_time);
}

/*
Set Send Wait time
int SetSTime(WORD Timeout)
*/
CLIPPER SetSTime(void)
{
  send_time=(WORD) _parni(1);
 _retni (0);
}

/*Get Send Wait time*/
CLIPPER GetTimeSend(void)
{
 _retni (send_time);
}

/*Receipt open*/
CLIPPER StartDoc(void)
{
 StartReceiptIT StartReceipt;

  StartReceipt.DocType=_parni(1);
  strncpy(StartReceipt.CashRegNo,ComH.id_operator,16);
  strncpy(StartReceipt.TableNo,_parc(2),16);
  strncpy(StartReceipt.PlaceNo,_parc(3),16);
  StartReceipt.Copies=_parni(4);
  strncpy(StartReceipt.AccountNo,_parc(5),30);
  StartReceipt.AdditP=NULL;
  if (PCOUNT==6) StartReceipt.AdditP=_parc(6);
 _retni (RunCommand(CM_ReceiptStart,(PCHAR)&StartReceipt));
}

/*Item document*/
CLIPPER ItemDoc(void)
{
 ItemDocIT ItemDoc;

  strncpy(ItemDoc.WareName,_parc(1),40);
  strncpy(ItemDoc.WareCode,_parc(2),20);
  strncpy(ItemDoc.Price,_parc(3),19);
  strncpy(ItemDoc.Count,_parc(4),19);
  strncpy(ItemDoc.Measure,_parc(5),3);
  ItemDoc.WareType=1;
  strncpy(ItemDoc.SecID,_parc(6),20);
  ItemDoc.AdditP=NULL;

 _retni (RunCommand(CM_ReceiptItem,(PCHAR)&ItemDoc));
}

/*SubTotalDocument*/
CLIPPER STotalDoc(void)
{
 _retni (RunCommand(CM_ReceiptSubtotal,0));
}
/*Total document*/
CLIPPER TotalDoc(void)
{
 _retni (RunCommand(CM_ReceiptTotal,0));
}

/*Tender document*/
CLIPPER TenderDoc(void)
{
 TenderDocIT TenderDoc;

  TenderDoc.PayType=_parni(1);
  strncpy(TenderDoc.TenderSum,_parc(2),19);
  strncpy(TenderDoc.CardName,_parc(3),40);
  if (PCOUNT==4)
   {
    TenderDoc.AdditP=_parc(4);
   } else TenderDoc.AdditP=NULL;
  _retni (RunCommand(CM_ReceiptTender,(PCHAR)&TenderDoc));
}

/*Cash driver open*/
CLIPPER DrvOpen(void)
{
  if (base_addr == 555)
  {
    RdWrTCP( "\005", 1, TCP_W );
  }
  else
  {
   WriteCom("\005",1);
  }
  delay(1000);
}

 /*Close document*/
CLIPPER CloseDoc(void)
{
 _retni (RunCommand(CM_ReceiptClose,0));
}

/*Cancel document (abort)*/
CLIPPER CancelDoc(void)
{
 _retni (RunCommand(CM_ReceiptCancel,0));
}

/*Get string fiald*/
CLIPPER GetSField(void)
{
 STRING20 tmpS;
 memset(tmpS,0,sizeof(STRING20));
 if (FieldCount()>=_parni(1)) GetField(tmpS,(BYTE)_parni(1));
 _retc(tmpS);
}

/*Get digital field*/
CLIPPER GetWField(void)
{
 STRING20 tmpS;
 WORD     W=0;
 BYTE     T;

 memset(tmpS,0,sizeof(STRING20));
 if (FieldCount()>=_parni(1))
  {
   GetField(tmpS,(BYTE)_parni(1));
   T=(_parni(2)==2) || (_parni(2)==4) ?_parni(2):4;
   PChar2HexWord((PCHAR)&tmpS,T,(PWORD)&W);
  } else  W=0xFFFF; /*No valid data*/
 _retnl(W);
}

/*Get real fields count*/
CLIPPER FieldC(void)
{
 _retni(FieldCount());
}

/*Get FULL answer from PRINTER*/
CLIPPER GetAnsw(void)
{
 _retc((PCHAR)Answer);
}

/*Get Day Mony as chars */
CLIPPER DayMony(void)
{
 _retni (RunCommand(CM_Report2Host,0));
}

/*Get doc numbers */
CLIPPER GetNumDoc(void)
{
 _retni (RunCommand(CM_No2Host,0));
}

/*Get last FR error */
CLIPPER GetLastErr(void)
{
 _retni (err);
}

/*Get last TCP error */
CLIPPER GetErrTCP(void)
{
 _retni (tcp_err);
}

#ifdef UCOMMAND_LOG
CLIPPER SetLogPath(void)
{
  if (LogPath == NULL)
   {
    LogPath=_xgrab(80);
    strncpy(LogPath,_parc(1),80);
   }
 _retc ((PCHAR)LogPath);
}
#endif
