using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IssleduemSmetanu
{
    public partial class AdminInterface: Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        // Импорт функции ReleaseCapture из user32.dll
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private static extern void ReleaseCapture();

        // Импорт функции SendMessage из user32.dll
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern void SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        public AdminInterface()
        {
            InitializeComponent();

            #region ---------- РАЗВЛЕЧЕНИЯ С РАМОЧКОЙ ----------
            this.Text = "Администрируем сметану";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 10, 10));

            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;


            Button closeButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Size = new Size(22, 22),
                Location = new Point(this.Width - 27, 5),
                Text = "",
                TabStop = false,
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                        new Rectangle(0, 0, closeButton.Width, closeButton.Height),
                        Color.FromArgb(218, 105, 81), // Начальный цвет
                        Color.FromArgb(210, 72, 20), // Конечный цвет
                        LinearGradientMode.Vertical))
                {
                    g.FillRectangle(gradientBrush, 0, 0, closeButton.Width - 1, closeButton.Height - 1);
                }

                using (Pen pen = new Pen(Color.White, 2))
                {
                    int offset = 5; // Отступ от краев
                    g.DrawLine(pen, offset - 1, offset - 1, closeButton.Width - offset - 1, closeButton.Height - offset - 1);
                    g.DrawLine(pen, closeButton.Width - offset - 1, offset - 1, offset - 1, closeButton.Height - offset - 1);
                }
            };
            closeButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, closeButton.Width, closeButton.Height, 5, 5));
            closeButton.FlatAppearance.BorderSize = 0;

            closeButton.Click += (sender, e) =>
            {
                this.Close();
            };

            this.Controls.Add(closeButton);

            Button minimizeButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Size = new Size(22, 22),
                Location = new Point(this.Width - 54, 5), // Расположение слева от кнопки закрытия
                Text = "",
                TabStop = false,
                Cursor = Cursors.Hand
            };
            minimizeButton.FlatAppearance.BorderSize = 0;
            minimizeButton.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, minimizeButton.Width, minimizeButton.Height),
                    Color.FromArgb(102, 148, 236), // Начальный цвет
                    Color.FromArgb(23, 122, 243), // Конечный цвет
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(gradientBrush, 0, 0, minimizeButton.Width - 1, minimizeButton.Height - 1);
                }

                using (Pen pen = new Pen(Color.White, 2))
                {
                    int offsetX = 5; // Горизонтальный отступ
                    int offsetY = 14; // Вертикальная позиция линии
                    g.DrawLine(pen, offsetX - 1, offsetY, minimizeButton.Width - offsetX - 4, offsetY); // Горизонтальная линия
                }
            };
            minimizeButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, minimizeButton.Width, minimizeButton.Height, 5, 5));
            minimizeButton.FlatAppearance.BorderSize = 0;

            minimizeButton.Click += (sender, e) =>
            {
                this.WindowState = FormWindowState.Minimized;
            };

            this.Controls.Add(minimizeButton);

            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = System.Drawing.Color.Transparent,
            };

            headerPanel.MouseDown += (sender, e) =>
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            };

            this.Controls.Add(headerPanel);

            #endregion
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(241, 239, 227))) // Светло-голубой цвет XP
            {
                g.FillRectangle(brush, 0, 0, this.Width, this.Height);
            }

            // Рисуем границу в стиле Windows XP
            using (Pen borderPen = new Pen(Color.FromArgb(1, 80, 220), 4))
            {
                g.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1); // Внешняя граница
            }

            // Рисуем градиент верхней панели
            Rectangle headerRect = new Rectangle(0, 0, this.Width, 30);

            using (LinearGradientBrush brush = new LinearGradientBrush(headerRect, Color.FromArgb(17, 120, 238), Color.FromArgb(1, 80, 220), LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, headerRect);
            }

            // Добавляем текст заголовка
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                g.DrawString(this.Text, font, textBrush, new Point(6, 7));
            }

        }
    }
}
