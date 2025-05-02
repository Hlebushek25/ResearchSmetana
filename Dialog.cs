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
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.IO;
using System.Data.SqlTypes;

namespace IssleduemSmetanu
{
    public enum DialogType
    {
        Error,
        ErrorWithTimer,
        YesOrNo
    };
    public partial class Dialog : Form
    {
        public string ActionCode { get; private set; }
        private string message = string.Empty;
        private Timer timer;

        //private DialogType type;

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

        // Константа сообщения для перетаскивания окна
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        public Dialog(string message, DialogType dialogType)
        {
            InitializeComponent();

            this.message = message;

            if (dialogType == DialogType.ErrorWithTimer)
            {
                TimeSpan remainingTime = Properties.Settings.Default.BlockUntil - DateTime.Now;
                label1.Text = message;
                if (remainingTime.Days != 0) label1.Text += remainingTime.Days.ToString() + " д ";
                if (remainingTime.Hours != 0) label1.Text += remainingTime.Hours.ToString() + " ч ";
                if (remainingTime.Minutes != 0) label1.Text += remainingTime.Minutes.ToString() + " м ";
                if (remainingTime.Seconds != 0) label1.Text += remainingTime.Seconds.ToString() + " с ";
            }
            else
            {
                label1.Text = message;
            }

            #region ---------- РАЗВЛЕЧЕНИЯ С РАМОЧКОЙ ----------
            label1.Text = message;

            // Настройки формы
            this.Text = "Ошибка!";
            this.Size = new Size(label1.Width + 150, label1.Height + 100);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 10, 10));

            // Отключаем изменение размеров формы
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;


            Button closeButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Size = new Size(22, 22),
                Location = new Point(this.Width - 27, 5),
                //BackColor = Color.FromArgb(227, 75, 23), // Красный фон
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
                        Color.FromArgb(218, 105, 81), // Начальный цвет (красный стиль XP)
                        Color.FromArgb(210, 72, 20), // Конечный цвет (светлее)
                        LinearGradientMode.Vertical)) // Направление сверху вниз
                {
                    g.FillRectangle(gradientBrush, 0, 0, closeButton.Width - 1, closeButton.Height - 1);
                }

                // Рисуем крестик
                using (Pen pen = new Pen(Color.White, 2))
                {
                    int offset = 5; // Отступ от краев
                    g.DrawLine(pen, offset - 1, offset - 1, closeButton.Width - offset - 1, closeButton.Height - offset - 1);
                    g.DrawLine(pen, closeButton.Width - offset - 1, offset - 1, offset - 1, closeButton.Height - offset - 1);
                }
            };
            // Применяем скругленные углы для кнопки
            closeButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, closeButton.Width, closeButton.Height, 5, 5));
            closeButton.FlatAppearance.BorderSize = 0; // Убираем стандартные границы

            closeButton.Click += (sender, e) => this.Close();


            this.Controls.Add(closeButton);

            // Добавляем верхнюю рамку для перетаскивания
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = System.Drawing.Color.Transparent,
            };

            headerPanel.MouseDown += (sender, e) =>
            {
                // Захватываем мышь и начинаем перетаскивание
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            };

            this.Controls.Add(headerPanel);

            #endregion

            switch (dialogType)
            {
                case DialogType.Error:
                    this.Load += (sender, e) =>
                    {
                        if (!Properties.Settings.Default.IsSoundsTurtedOff)
                        {
                            try
                            {
                                using (MemoryStream wavFile = new MemoryStream(Properties.Resources.Error))
                                using (SoundPlayer player = new SoundPlayer(wavFile))
                                {
                                    player.Play(); // Воспроизведение звука
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка воспроизведения звука: {ex.Message}");
                            }
                        }
                    };
                    okButton.Visible = true;
                    okButton.Location = new Point(this.Width - 75, this.Height - 35);
                    this.Text = "Ошибка!";
                    windowIcon.Image = Properties.Resources.kolobok;
                    break;

                case DialogType.ErrorWithTimer:
                    this.Load += (sender, e) =>
                    {
                        if (!Properties.Settings.Default.IsSoundsTurtedOff)
                        {
                            try
                            {
                                using (MemoryStream wavFile = new MemoryStream(Properties.Resources.Error))
                                using (SoundPlayer player = new SoundPlayer(wavFile))
                                {
                                    player.Play(); // Воспроизведение звука
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка воспроизведения звука: {ex.Message}");
                            }
                        }
                    };
                    okButton.Visible = true;
                    okButton.Location = new Point(this.Width - 75, this.Height - 35);
                    this.Text = "Ошибка!";
                    windowIcon.Image = Properties.Resources.kolobok;

                    timer = new Timer
                    {
                        Interval = 1000
                    };

                    timer.Tick += Timer_Tick;
                    timer.Start();

                    break;

                case DialogType.YesOrNo:
                    this.Load += (sender, e) =>
                    {
                        if (!Properties.Settings.Default.IsSoundsTurtedOff)
                        {
                            try
                            {
                                using (MemoryStream wavFile = new MemoryStream(Properties.Resources.Exclamation))
                                using (SoundPlayer player = new SoundPlayer(wavFile))
                                {
                                    player.Play(); // Воспроизведение звука
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка воспроизведения звука: {ex.Message}");
                            }
                        }
                    };
                    yesButton.Visible = true;
                    noButton.Visible = true;
                    yesButton.Location = new Point(this.Width - 150, this.Height - 35);
                    noButton.Location = new Point(this.Width - 75, this.Height - 35);
                    this.Text = "Продолжить?";
                    windowIcon.Image = Properties.Resources.kolobok_warning;
                    break;
            }
            

            this.KeyPreview = true;
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            };

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
            Rectangle headerRect = new Rectangle(0, 0, this.Width, 30); // Заголовок окна
            using (SolidBrush headerBrush = new SolidBrush(Color.FromArgb(1, 80, 220)))
            {
                g.FillRectangle(headerBrush, headerRect);
            }

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

        private void okButton_Click(object sender, EventArgs e)
        {
            ActionCode = "ok";
            this.Close();
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            ActionCode = "no";
            this.Close();
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            ActionCode = "yes";
            this.Close();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan remainingTime = Properties.Settings.Default.BlockUntil - DateTime.Now;
            label1.Text = message;
            if (remainingTime.Days != 0) label1.Text += remainingTime.Days.ToString() + " д ";
            if (remainingTime.Hours != 0) label1.Text += remainingTime.Hours.ToString() + " ч ";
            if (remainingTime.Minutes != 0) label1.Text += remainingTime.Minutes.ToString() + " м ";
            if (remainingTime.Seconds != 0) label1.Text += remainingTime.Seconds.ToString() + " с ";

            if (remainingTime <= TimeSpan.Zero)
            {
                timer.Stop();
                TimerFinished();
            }
        }

        private void TimerFinished()
        {
            Properties.Settings.Default.LoginTryQuantity = Properties.Settings.Default.DefaultLoginTryQuantity;
            Properties.Settings.Default.BlockUntil = DateTime.MinValue;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
