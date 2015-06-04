using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RMLib;

namespace Parking.FiscalDevice
{
    public class Start200K : Disposable //, IFiscalDevice
    {
        #region [ const ]

        //#if DEBUG
        //    private const string Prim08Path = @"..\..\Library\Prim08.dll";
        //#else
        private const string AzimuthPath = @".\Library\Azimuth.dll";
        //private const string Prim08Path = "Prim08.dll";
        //#endif

        #endregion

        #region [ imports ]
        /// <summary>
        /// Инициализация ККМ
        /// </summary>
        [DllImport(AzimuthPath, CharSet = CharSet.Ansi)]
        private static extern int OpenDLL(string asOper, string asPassw, string asPort, bool ascii);
        #endregion

    }
}
