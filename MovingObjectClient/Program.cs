using System;
using System.Windows.Forms;

namespace MovingObjectClient
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormClient()); // jalankan FormClient
        }
    }
}
