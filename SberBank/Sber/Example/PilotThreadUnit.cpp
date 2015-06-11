//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "PilotThreadUnit.h"
#include "TestHalUnit1.h"

#pragma package(smart_init)
//---------------------------------------------------------------------------

//   Important: Methods and properties of objects in VCL can only be
//   used in a method called using Synchronize, for example:
//
//      Synchronize(UpdateCaption);
//
//   where UpdateCaption could look like:
//
//      void __fastcall TPaymentThread::UpdateCaption()
//      {
//        Form1->Caption = "Updated in a thread";
//      }
//---------------------------------------------------------------------------

__fastcall TPaymentThread::TPaymentThread(bool CreateSuspended)
  : TThread(CreateSuspended)
{
}
//---------------------------------------------------------------------------
void TPaymentThread::SetName()
{
  ;
}

void __fastcall TPaymentThread::DoThePayment(void)
{
  Form1->SimulatePayment();
}

//---------------------------------------------------------------------------
void __fastcall TPaymentThread::Execute()
{
  SetName();

  DoThePayment();
  //---- Place thread code here ----
//  Synchronize((TThreadMethod)&DoThePayment);
}
//---------------------------------------------------------------------------
