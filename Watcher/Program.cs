using System;
using System.Windows.Forms;

namespace CryptoCurrency.Watcher
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CCWatcher());
        }
    }
}