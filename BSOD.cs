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

namespace IssleduemSmetanu
{
    public partial class BSOD : Form
    {
        // Импорт функции SetWindowLong из user32.dll
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // Импорт функции GetWindowLong из user32.dll
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // Константы для работы с оконными стилями
        private const int GWL_STYLE = -16;
        private const int WS_BORDER = 0x00800000;
        private const int WS_CAPTION = 0x00C00000;

        public BSOD()
        {
            InitializeComponent();

            this.Load += (sender, e) =>
            {
                // Убираем рамку и заголовок формы
                int style = GetWindowLong(this.Handle, GWL_STYLE);
                SetWindowLong(this.Handle, GWL_STYLE, style & ~WS_BORDER & ~WS_CAPTION);

                // Устанавливаем полноэкранный режим
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = FormBorderStyle.None;
            };


        }

        // Импорт функции ShowWindow из user32.dll
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Импорт функции FindWindow из user32.dll
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
