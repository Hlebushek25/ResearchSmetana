using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IssleduemSmetanu
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Properties.Settings.Default.LoginTryQuantity = Properties.Settings.Default.DefaultLoginTryQuantity;
            //Properties.Settings.Default.BlockUntil = DateTime.MinValue;
            //Properties.Settings.Default.Save();

            if (Properties.Settings.Default.BlockUntil <= DateTime.Now && Properties.Settings.Default.LoginTryQuantity == 0)
            {
                Properties.Settings.Default.LoginTryQuantity = Properties.Settings.Default.DefaultLoginTryQuantity;
                Properties.Settings.Default.BlockUntil = DateTime.MinValue;
                Properties.Settings.Default.Save();
            }

            Application.Run(new MyApplicationContext());
        }
    }
}
