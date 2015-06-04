using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestStart200K
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
//        private static extern int OpenDLL(ref char asOper, ref Byte asPassw, ref char asPort, bool ascii);
        private static extern int OpenDLL(string asOper, string asPassw, string asPort, bool ascii);
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        private static extern int CloseDLL();
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        private static extern int StartSeans();
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        private static extern int ShiftOpen(string buf);
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        private static extern int ShiftClose();
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        private static extern byte[] GetAnswer(ref byte buff);
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        private static extern double GetFldFloat(byte field);
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        private static extern int GetFldWord(byte field);
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        private static extern byte GetFldByte(byte field);
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        public static extern int GetEReport(byte com, byte param);
        [DllImport("Azimuth.dll", CharSet = CharSet.Ansi)]
        private static extern int GetFldsCount();
        private void btnStart_Click(object sender, EventArgs e)
        {
             string s = OpenDLL("a", "AERF", "COM4", false).ToString();
            tbAnsver.Text = s;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            tbAnsver.Text = CloseDLL().ToString();
        }

        private void btnSeans_Click(object sender, EventArgs e)
        {
            tbAnsver.Text = StartSeans().ToString();
        }

        private void Shift_Click(object sender, EventArgs e)
        {
            tbAnsver.Text = ShiftOpen("").ToString();
        }

        private void ShiftClose_Click(object sender, EventArgs e)
        {

            tbAnsver.Text = ShiftClose().ToString();
        }

        private void GetBuff_Click(object sender, EventArgs e)
        {
            StartSeans();
  byte[] buf = new byte[128];

             GetAnswer(ref buf[0]);
        }

        private void GetInt_Click(object sender, EventArgs e)
        {
            StartSeans();
 //           var a = GetFldByte(2);
            var b = GetFldWord(0);
            var b1 = GetFldWord(1);
            var b2 = GetFldWord(2);
            var b3 = GetFldWord(3);
            var b4 = GetFldWord(4);
            var bb = BitConverter.GetBytes(b2);
            var r = bb[1] & 8;
            var d = GetEReport(0x34, 0);
            var count = GetFldsCount();
 
            double c = GetFldFloat(23);
        }
    }
}
