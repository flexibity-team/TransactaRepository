#ifndef _PILOT_NT
#define _PILOT_NT
/***********************************************************

  PILOT_NT
  -------
  ���������� ��� ������ � ������� �������� � ��������������
  ������� � ��������� ������� (Visa/Maestro/MasterCard/Amex/DinersClub).

  ���������:                             Win32.

************************************************************/

typedef unsigned long  DWORD;
typedef unsigned short USHORT;

//���� ��������
#define OP_PURCHASE     1   //������ �������
#define OP_RETURN       3   //������� ���� ������ �������
#define OP_FUNDS        6   //������.�������

#define OP_PREAUTH     51   //���������������
#define OP_COMPLETION  52   //���������� �������
#define OP_CASHIN      53   //����� ��������
#define OP_CASHIN_COMP 54   //������������� ������


//���� ����
#define CT_USER     0  //����� �� ����

#define CT_VISA     1  //Visa
#define CT_EUROCARD 2  //Eurocard/Mastercard
#define CT_CIRRUS   3  //Cirrus/Maestro
#define CT_AMEX     4  //Amex
#define CT_DINERS   5  //DinersCLub
#define CT_ELECTRON 6  //VisaElectron
#define CT_SBERCARD 9  //Sbercard

//���������, ������������ ��� �������� ����������
// � �������� �����������
#pragma pack(1)
struct auth_answer{
   int TType;             //����: ��� ����������
   unsigned long Amount;  //����: �����
   char RCode[3];         //�����: ��� ���������� �����������
   char AMessage[16];     //�����: ��������� ��������� ����������
   int  CType;            //����/�����: ��� �����
   char* Check;           //�����: ����� ����, ������ �������������
                          // GlobalFree � ���������� ���������
};

//����������� ���������
struct auth_answer2{
   struct auth_answer auth_answ;
   char AuthCode[7];      //�����: ��� �����������
};

//��� ���� ����������� ���������
struct auth_answer3{
  struct auth_answer auth_answ;
  char AuthCode[7]; //�����: ��� �����������
  char CardID[25];  //�����: ������������� �����
};

//��� ����� ����������� ���������
struct auth_answer4{
  struct auth_answer auth_answ;
  char AuthCode[7];   //�����: ��� �����������
  char CardID[25];    //�����: ������������� �����
  int  ErrorCode;     //�����: ��� ������
  char TransDate[20]; //�����: ���� � ����� ��������
  int  TransNumber;   //�����: ����� ��������
};

//��� ����� ����������� ���������
struct auth_answer5{
  struct auth_answer auth_answ;
  char   RRN[13];
  char   AuthCode[7];  // added after communication with HRS
};

//��� ����� ����������� ���������
struct auth_answer6{
  struct auth_answer auth_answ;
  char   AuthCode[7];   //�����: ��� �����������
  char   CardID[25];    //�����: ������������� (�����) �����
  int    ErrorCode;     //�����: ��������� ��� ������
  char   TransDate[20]; //�����: ���� � ����� ��������
  int    TransNumber;   //�����: ����� �������� �� ����
  char   RRN[13];       //����/�����: ��������� ����� ���������������
};

//��� ����� ����������� ���������
struct auth_answer7{
  struct auth_answer auth_answ;
  char   AuthCode[7];   //�����: ��� �����������
  char   CardID [25];   //�����: ������������� (�����) �����
  int    SberOwnCard;   //�����: ���� �������������� ����� ���������
};

#pragma pack()

extern "C"{

//���������� �������� �� ������.
//track2 - ������ ������� ����� � ��������� �������. ���� NULL, ��
//����� ���������� ������� �����
//auth_answer - ��. �������� ����� ���������
 __declspec(dllexport) int card_authorize(char *track2,
			     struct auth_answer *auth_answer);

//�������� ���. ���� TType,Amount,CType ��������� �� �����.
 __declspec(dllexport) int close_day(struct auth_answer *auth_answer);

//��������� �������� ������. ��� �������� ���� TType = 0 �����������
//������� �����, ����� - ������
 __declspec(dllexport) int get_statistics(struct auth_answer *auth_answer);

//���������� �������� �� ������ � ��������� �������������� ������.
//  track2 - ������ ������� ����� � ��������� �������. ���� NULL, ��
//  ����� ���������� ������� �����
//
//  auth_answer2...auth_answer7 - ��. �������� ����� ���������
//
__declspec(dllexport) int  card_authorize2(char *track2,
			     struct auth_answer2 *auth_answer);
__declspec(dllexport) int  card_authorize3(char *track2,
			     struct auth_answer3 *auth_answer);
__declspec(dllexport) int  card_authorize4(char *track2,
			     struct auth_answer4 *auth_answer);

__declspec(dllexport) int  card_authorize5(char *track2,
			     struct auth_answer5 *auth_answer);

__declspec(dllexport) int  card_authorize6(char *track2,
			     struct auth_answer6 *auth_answer);

__declspec(dllexport) int  card_authorize7(char *track2,
		             struct auth_answer7 *auth_answer);

//��������� ������ ������
__declspec(dllexport) unsigned int GetVer();

//���������������
__declspec(dllexport) void Done();

//������ ����� (������������ 4 ��������� ����� � ��� �� ������ �����)
__declspec(dllexport) int ReadCard(char *Last4Digits, char *Hash);

//������ ����� (����������� ������ ����� � ���� �������� ����� � ������� YYMM)
//
//  ����� ����� ����� ����� �� 13 �� 19 ����.
//  ����� ����� ������������ ��� ������ ��� �����������, �� ����� �����
//  �������������� ���:
//
//    sprintf(track2,"%s=%s", CardNo, ValidThru)      
//
__declspec(dllexport) int ReadCardFull(char *CardNo, char *ValidThru);

//������ ������ ������ ������� �����
//
//  ������ ������ ������� ����� ����� ����� �� 40 ��������.
//  ������ ������� ����� ������:
//
//    nnnn...nn=yymmddd...d
//
//  ���     '=' - ������-�����������
//      nnn...n - ����� �����
//      yymm    - ���� �������� ����� (����)
//      ddd...d - ��������� ������ �����
//
__declspec(dllexport) int ReadTrack2(char *Track2);

//������ ����� �������� (����������� ������ ����� ����� � ������� �������)
//
__declspec(dllexport) int ReadSbercard(char *CardNo, char* ClientName);


//������ ����� �����������
//
__declspec(dllexport)int EnableReader (HWND hDestWindow, UINT message);
__declspec(dllexport)int DisableReader();

//�������� ���������� �������
//
__declspec(dllexport) int TestPinpad();

// ������� ��������� ���������� � "�����������" ���������
//
__declspec(dllexport) int SuspendTrx (DWORD dwAmount, char* pAuthCode);

// ����������� ��������� ���������� � "��������" ���������
//
__declspec(dllexport) int CommitTrx  (DWORD dwAmount, char* pAuthCode);

// ������������� ������ ��������� ����������
//
__declspec(dllexport) int RollbackTrx(DWORD dwAmount, char* pAuthCode);

// �������� ����� ���������
//
__declspec(dllexport) int GetTerminalID(char* pTerminalID);

// ���������� ������ ��� ������ �� �����
//
__declspec(dllexport) int SetGUIHandles(void* hText, void* hEdit);

};


#endif
