//---------------------------------------------------------------------------

#ifndef TestHalUnit1H
#define TestHalUnit1H
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ExtCtrls.hpp>
//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
        TButton *Button1;
        TLabeledEdit *LabeledEdit1;
        TButton *Button2;
        TStaticText *StaticText1;
        TStaticText *StaticText2;
        TStaticText *StaticText3;
        TMemo *Memo1;
  TButton *Button3;
        void __fastcall Button1Click(TObject *Sender);
        void __fastcall Button2Click(TObject *Sender);
        void __fastcall FormActivate(TObject *Sender);
  void __fastcall Button3Click(TObject *Sender);
private:	// User declarations
        void __fastcall ClearForm();
public:		// User declarations
        __fastcall TForm1(TComponent* Owner);
        void __fastcall SimulatePayment();
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
