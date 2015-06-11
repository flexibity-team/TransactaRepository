//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "TestHalUnit1.h"
#include "PilotThreadUnit.h"

#include "pilot_nt.h"
#include "hal.h"

typedef int (*T_CardAuth_Type)(char *track2, struct auth_answer *auth_answer);
typedef int (*T_CloseDay_Type)(struct auth_answer *auth_answer);
typedef int (*T_GUI_Type)     (void* hText, void* hEdit);
typedef int (*T_Abort_Type)   ();

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm1 *Form1;

HINSTANCE hPilotLib = NULL;
T_CardAuth_Type card_authorize_   = NULL;
T_CloseDay_Type close_day_        = NULL;
T_GUI_Type      SetGUIHandles_    = NULL;
T_Abort_Type    AbortTransaction_ = NULL;

int HalDispFunc(int cmd, char* par1, int par2)
{
  char buf2[256];

  buf2[0] = 0;
  if(par1)
    strcpy(buf2, par1);

  switch(cmd){
    case ICMD_DISP_STRING_1:  // show last two messages
      if(par2 <= 5)
        Form1->StaticText1->Caption = buf2;
      else
        Form1->StaticText3->Caption = buf2;
      break;

    case ICMD_DISP_STRING_2:
      Form1->StaticText2->Caption = buf2;
      break;

    case ICMD_SHOW_INPUT:
      Form1->StaticText2->Visible = par2;
      break;

    case ICMD_DISP_CLEAR:
      Form1->StaticText1->Caption = "";
      Form1->StaticText2->Caption = "";
      Form1->StaticText3->Caption = "";
      break;
  }
  Application->ProcessMessages();
  return 0;
}

//---------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent* Owner)
        : TForm(Owner)
{
  hPilotLib = LoadLibrary("pilot_nt.dll");
  if(hPilotLib){
    card_authorize_    = (T_CardAuth_Type)GetProcAddress(hPilotLib, "_card_authorize");
    close_day_         = (T_CloseDay_Type)GetProcAddress(hPilotLib, "_close_day");
    SetGUIHandles_     = (T_GUI_Type)     GetProcAddress(hPilotLib, "_SetGUIHandles");
    AbortTransaction_  = (T_Abort_Type)   GetProcAddress(hPilotLib, "_AbortTransaction");
  }
}

void __fastcall TForm1::ClearForm()
{
  Form1->StaticText1->Caption = "";
  Form1->StaticText2->Caption = "";
  Form1->StaticText3->Caption = "";
  Memo1->Clear();
}

void __fastcall TForm1::SimulatePayment()
{
  auth_answer a;
  int res;

  ClearForm();
  memset(&a,0,sizeof(a));
  a.TType = 1;
  a.Amount = LabeledEdit1->Text.ToInt();

  if(card_authorize_){
    Button3->Enabled = True;
    res = card_authorize_(0, &a);
    StaticText1->Caption = a.AMessage;
    Button3->Enabled = False;
  }
  else
    StaticText1->Caption = "card_authorize() not found";
  if(a.Check){
    Memo1->Text = a.Check;
    GlobalFree(a.Check);
  }
}

//---------------------------------------------------------------------------
void __fastcall TForm1::Button1Click(TObject *Sender)
{
  new TPaymentThread(false);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button2Click(TObject *Sender)
{
  auth_answer a;
  int res = 8000;

  ClearForm();
  memset(&a,0,sizeof(a));

  if(close_day_){
    res = close_day_(&a);

    if(a.Check){
      Memo1->Text = a.Check;
      GlobalFree(a.Check);
    }
    else
      StaticText1->Caption = AnsiString("Îøèáêà ") + a.RCode;
  }
  else{
    StaticText1->Caption = "close_day() not found";
  }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormActivate(TObject *Sender)
{
/*
  int i = GetDlgCtrlID(StaticText1->Handle);
  int k = GetDlgCtrlID(StaticText2->Handle);

  StaticText1->Caption = i;
  StaticText2->Caption = k;
*/
   Form1->StaticText1->Caption = "";
   Form1->StaticText2->Caption = "";
   Form1->StaticText3->Caption = "";

   if(SetGUIHandles_)
     SetGUIHandles_(HalDispFunc, 0);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button3Click(TObject *Sender)
{
  if(AbortTransaction_)
    AbortTransaction_();
  Button3->Enabled = False;
}
//---------------------------------------------------------------------------


