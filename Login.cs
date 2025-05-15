using IssleduemSmetanu.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace IssleduemSmetanu
{
    public partial class Login : Form
    {
        public string ActionCode { get; private set; }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private static extern void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern void SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        List<User> users = InteractionDB.GetAllUsers();

        string previousUsername = default(string);
        string previousPassword = default(string);

        public Login()
        {
            InitializeComponent();

            #region ---------- РАЗВЛЕЧЕНИЯ С РАМОЧКОЙ ----------

            this.StartPosition = FormStartPosition.CenterScreen;

            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 10, 10));

            Button closeButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Size = new Size(22, 22),
                Location = new Point(24, this.Height - 46),
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

                Rectangle rect = new Rectangle(4, 4, closeButton.Width - 10, closeButton.Height - 10); // Уменьшение размера круга

                // Рисуем круг
                using (Pen pen = new Pen(Color.White, 2)) // Толщина линии уменьшена для маленькой кнопки
                {
                    g.DrawEllipse(pen, rect);
                }

                int lineX = rect.Left + rect.Width / 2;
                int lineYStart = rect.Top + rect.Height / 4;
                int lineYEnd = rect.Top + rect.Height / 4 * 3;

                using (Pen pen = new Pen(Color.White, 2)) // Уменьшение толщины линии
                {
                    g.DrawLine(pen, new Point(lineX, lineYStart), new Point(lineX, lineYEnd));
                }


            };
            closeButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, closeButton.Width, closeButton.Height, 5, 5));
            closeButton.FlatAppearance.BorderSize = 0;

            this.Controls.Add(closeButton);

            Button continueButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Size = new Size(22, 22),
                Location = new Point(this.Width - 46, this.Height - 46),
                Text = "",
                TabStop = false,
                Cursor = Cursors.Hand
            };
            continueButton.FlatAppearance.BorderSize = 0;
            continueButton.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                        new Rectangle(0, 0, continueButton.Width, continueButton.Height),
                        Color.FromArgb(116, 179, 108), // Начальный цвет
                        Color.FromArgb(11, 141, 8), // Конечный цвет
                        LinearGradientMode.Vertical))
                {
                    g.FillRectangle(gradientBrush, 0, 0, continueButton.Width - 1, continueButton.Height - 1);
                }

                using (Pen pen = new Pen(Color.White, 2))
                {
                    int offset = 5; // Отступ от краев
                    g.DrawLine(pen, offset - 1, closeButton.Height / 2 - 1, closeButton.Width - offset - 1, closeButton.Height / 2 - 1);
                    g.DrawLine(pen, closeButton.Width / 2, offset, closeButton.Width - offset - 1, closeButton.Height / 2 - 1);
                    g.DrawLine(pen, closeButton.Width / 2, closeButton.Height - offset - 2, closeButton.Width - offset - 1, closeButton.Height / 2 - 1);
                }

            };
            continueButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, continueButton.Width, continueButton.Height, 5, 5));
            continueButton.FlatAppearance.BorderSize = 0;

            this.Controls.Add(continueButton);

            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = System.Drawing.Color.Transparent,
            };

            Panel footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = System.Drawing.Color.Transparent,
            };

            headerPanel.MouseDown += (sender, e) =>
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            };

            footerPanel.MouseDown += (sender, e) =>
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            };

            this.Controls.Add(headerPanel);
            this.Controls.Add(footerPanel);
            #endregion

            checkBox1.Checked = Properties.Settings.Default.IsSoundsTurtedOff;

            closeButton.Click += (sender, e) =>
            {
                this.ActionCode = "Exit";
                this.Close();
            };

            continueButton.Click += (sender, e) =>
            {
                string username = usernameTextBox.Text;
                string password = passwordTextBox.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    Dialog dialog = new Dialog("Введите логин и пароль!", DialogType.Error);
                    dialog.ShowDialog();
                    return;
                }

                try
                {
                    User user = users.FirstOrDefault(u => u.login.Equals(username, StringComparison.OrdinalIgnoreCase) && u.password == password);

                    uint loginTryQuantity = Properties.Settings.Default.LoginTryQuantity;
                    if (user == null && loginTryQuantity > 0)
                    {
                        Random rand = new Random();
                        string errorMessage = default(string);
                        string errorEnd = default(string);
                        if (username == previousUsername && password == previousPassword && rand.Next(1, 6) == 3)
                        {
                            errorMessage = "Ты совсем дурында?\nВведи что-нибудь другое\n(осталось ";
                            switch (loginTryQuantity % 10)
                            {
                                case 1:
                                    if (loginTryQuantity % 100 / 10 == 1)
                                        errorMessage += $"{loginTryQuantity} попыток)";
                                    else
                                        errorMessage += $"{loginTryQuantity} попытка)";
                                    break;
                                case 2:
                                case 3:
                                case 4:
                                    if (loginTryQuantity % 100 / 10 == 1)
                                        errorMessage += $"{loginTryQuantity} попыток)";
                                    else
                                        errorMessage += $"{loginTryQuantity} попытки)";
                                    break;
                                default:
                                    errorMessage += $"{loginTryQuantity} попыток)";
                                    break;
                            }
                            if (callDialog(errorMessage, DialogType.YesOrNo) == "yes")
                            {
                                Dialog error = new Dialog("Пизда", DialogType.Error);
                                error.ShowDialog();
                            }
                        }
                        else
                        {
                            errorMessage = "Неверный логин или пароль!\nОсталось ";
                            switch (loginTryQuantity % 10)
                            {
                                case 1:
                                    if (loginTryQuantity % 100 / 10 == 1)
                                        errorMessage += $"{loginTryQuantity} попыток.";
                                    else
                                        errorMessage += $"{loginTryQuantity} попытка.";
                                    break;
                                case 2:
                                case 3:
                                case 4:
                                    if (loginTryQuantity % 100 / 10 == 1)
                                        errorMessage += $"{loginTryQuantity} попыток.";
                                    else
                                        errorMessage += $"{loginTryQuantity} попытки.";
                                    break;
                                default:
                                    errorMessage += $"{loginTryQuantity} попыток.";
                                    break;
                            }
                            Dialog error = new Dialog(errorMessage, DialogType.Error);
                            error.ShowDialog();
                        }

                        
                        loginTryQuantity -= 1;
                        Properties.Settings.Default.LoginTryQuantity = loginTryQuantity;
                        Properties.Settings.Default.Save();
                        previousUsername = username;
                        previousPassword = password;
                        return;
                    }
                    else if (loginTryQuantity <= 0)
                    {
                        if (Properties.Settings.Default.BlockUntil == null || Properties.Settings.Default.BlockUntil < DateTime.Now)
                        {
                            Properties.Settings.Default.BlockUntil = DateTime.Now.AddSeconds(Properties.Settings.Default.LoginTimeout);

                            Properties.Settings.Default.Save();
                        }

                        Dialog error = new Dialog("Вам здесь больше не рады!\nВозвращайтесь через ", DialogType.ErrorWithTimer);
                        
                        Random random = new Random();
                        if (random.Next(1, 11) == 1)
                        {
                            error.Show();
                            this.ActionCode = "error";
                            this.Close();
                        }
                        else
                        {
                            error.ShowDialog();
                        }
                        return;
                    }
                    else
                    {
                        this.ActionCode = user.role.Equals("admin", StringComparison.OrdinalIgnoreCase)
                        ? "ContinueAsAdmin"
                        : "ContinueAsResearcher";

                        Properties.Settings.Default.LoginTryQuantity = Properties.Settings.Default.DefaultLoginTryQuantity;
                        Properties.Settings.Default.Save();

                        // ----- НЕРЕАЛЬНЫЙ РАНДОМ -----
                        Random random = new Random();
                        if (random.Next(1, 21) == 1)
                        {
                            this.ActionCode = "error";
                        }
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    Dialog dialog = new Dialog(ex.Message, DialogType.Error);
                    dialog.ShowDialog();
                }
            };

            this.KeyPreview = true;
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    closeButton.PerformClick();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    continueButton.PerformClick();
                }
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            Rectangle footerRect = new Rectangle(0, this.Height - 70, this.Width, this.Height);

            using (LinearGradientBrush brush = new LinearGradientBrush(footerRect, Color.FromArgb(54, 50, 170), Color.FromArgb(0, 46, 160), LinearGradientMode.Horizontal))
            {
                g.FillRectangle(brush, footerRect);
            }

            using (Pen borderPen = new Pen(Color.FromArgb(1, 80, 220), 4))
            {
                g.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias; // Сглаживание для круга
            

        }

        public string callDialog(string message, DialogType type)
        {
            Dialog error = new Dialog(message, type);
            error.ShowDialog();
            return error.ActionCode;
        }

        private void usernameTextBox_Enter(object sender, EventArgs e)
        {
            usernameLabel.Font = new Font(usernameLabel.Font.FontFamily, usernameLabel.Font.Size, FontStyle.Bold);
        }

        private void usernameTextBox_Leave(object sender, EventArgs e)
        {
            usernameLabel.Font = new Font(usernameLabel.Font.FontFamily, usernameLabel.Font.Size, FontStyle.Regular);               
            avatarPictureBox.Image = Properties.Resources.Doge;

            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                return;
            }

            try
            {
                //// Ищем пользователя в базе данных
                var user = users.FirstOrDefault(u => u.login.Equals(usernameTextBox.Text, StringComparison.OrdinalIgnoreCase));

                if (user != null)
                {
                    // Меняем аватар в зависимости от роли
                    switch (user.role?.ToLower()) // Приводим роль к нижнему регистру для унификации
                    {
                        case "admin":
                            avatarPictureBox.Image = Properties.Resources.Admin;
                            break;
                        case "researcher":
                            avatarPictureBox.Image = Properties.Resources.Researcher;
                            break;
                        default:
                            avatarPictureBox.Image = Properties.Resources.Doge;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем работу приложения
                Debug.WriteLine($"Ошибка при проверке пользователя: {ex.Message}");
                avatarPictureBox.Image = Properties.Resources.Doge;
            }
        }

        private void passwordTextBox_Enter(object sender, EventArgs e)
        {
            passwordLabel.Font = new Font(passwordLabel.Font.FontFamily, passwordLabel.Font.Size, FontStyle.Bold);
        }

        private void passwordTextBox_Leave(object sender, EventArgs e)
        {
            passwordLabel.Font = new Font(passwordLabel.Font.FontFamily, passwordLabel.Font.Size, FontStyle.Regular);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ActionCode = "ContinueAsAdmin";
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.ActionCode = "ContinueAsResearcher";
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IsSoundsTurtedOff = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }
    }

}
