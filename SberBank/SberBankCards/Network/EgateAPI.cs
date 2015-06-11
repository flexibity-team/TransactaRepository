namespace Parking.SberBank.Network
{
    using System;
    using System.Runtime.InteropServices;

    internal static class EgateAPI
   {
    #region [ const ]

    public const int ProtocolId = 15;

    public const char ParameterSeparator = ' ';
    public const string AsyncRequestStartedString = "530 START";
    public const char StatusResult = '&';
    public const char StatusBusy = '#';
    public const char StatusMifare = '*';
    public const char StatusWaitCard = '0';
    public const char StatusCardEntered = '1';
    public const char StatusPinEntered = '2';
    public const char StatusAuthCompleted = '3';

    public const string EgateConfigPath = @"C:\GCS\EMVGSSL\emvgate.cfg";
    private const string EgateLibraryPath = @"C:\GCS\EMVGSSL\emvgatessl.dll";

    #endregion

    [DllImport(EgateLibraryPath)]
    public static extern uint egGetVersion();

    [DllImport(EgateLibraryPath)] //, CharSet = CharSet.Ansi)]
    public static extern IntPtr egGetVersionDescription();

    [DllImport(EgateLibraryPath)]
    public static extern IntPtr egGetErrorDescription(int nErrCode);

    [DllImport(EgateLibraryPath)]
    public static extern void egTest(int nTestId);

    [DllImport(EgateLibraryPath)]
    public static extern IntPtr egDlgGetRecNumber();

    [DllImport(EgateLibraryPath)]
    public static extern int egInitInstance(string pszCfgFileName);

    [DllImport(EgateLibraryPath)]
    public static extern int egGetLastError(int nIdInst);

    [DllImport(EgateLibraryPath)]
    public static extern bool egReleaseInstance(int nIdInst);

    [DllImport(EgateLibraryPath)]
    public static extern IntPtr egAuthRequest(int nIdInst, int nProtId, string pszRequest);

    [DllImport(EgateLibraryPath)]
    public static extern IntPtr egAuthRequestAsync(int nIdInst, int nProtId, string pszRequest);

    [DllImport(EgateLibraryPath)]
    public static extern IntPtr egGetOpStatus(int nIdInst, bool bIsCansel);

    [DllImport(EgateLibraryPath)]
    public static extern IntPtr egGetAuthReceipt(int nIdInst);

    [DllImport(EgateLibraryPath)]
    public static extern IntPtr egGetAuthResult(int nIdInst);

    #region [ helper ]

    public static string GetString(this IntPtr p)
    {
      return Marshal.PtrToStringAnsi(p);
    }

    public static string GetString(this IntPtr p, out byte[] ba)
    {
      string s = Marshal.PtrToStringAnsi(p);

      ba = new byte[s.Length];
      Marshal.Copy(p, ba, 0, s.Length);

      return s;
    }

    #endregion
  }
}