using System;
using System.Runtime.InteropServices;
using System.Text;
using RMLib.Log;

namespace Parking.CashDesk
{
  internal class UserDisplay
  {
    #region [ const ]

    private const string VFDDisplayPath = CashDeskPathManager.LibraryPath + "VFDDisplay.dll";

    private const int DefaultCodePage = 1251;

    #endregion

    #region [ imports ]

    [DllImport(VFDDisplayPath, CharSet = CharSet.Ansi)]
    private static extern int Init(int nPort);

    [DllImport(VFDDisplayPath, CharSet = CharSet.Ansi)]
    private static extern int CloseVFD();

    [DllImport(VFDDisplayPath, CharSet = CharSet.Ansi)]
    private static extern int ClearVFD();

    [DllImport(VFDDisplayPath, CharSet = CharSet.Ansi)]
    private static extern int SetVFDString(int line, ref Byte bStr);

    #endregion

    private readonly ILogger logger;
    private int codePage;

    public UserDisplay(ILogger logger)
    {
      this.logger = logger;
      codePage = DefaultCodePage;
    }

    // Инициализация дисплея
    // Параметны: nPortNum - номер порта подключения дисплея
    public int Initialize(int nPortNum, int codePage)
    {
      this.codePage = codePage;

      return Init(nPortNum);
    }

    // Завершение работы с дисплеем
    public int Close()
    {
      return CloseVFD();
    }

    // Очистка экрана дисплея
    public int Clear()
    {
      ClearVFD();
      return 0; 
    }

    // Вывести строку на дисплее
    public int Text(int lineIndex, string str)
    {
      byte[] ba = null;
      try
      {
        ba = Encoding.GetEncoding(codePage).GetBytes(str);
      }
      catch (Exception e)
      {
        logger.Write(e, "Ошибка кодирования строки для вывода на дисплей пользователя");
      }

      return ba == null ? 0 : SetVFDString(lineIndex, ref ba[0]);
    }
  }
}