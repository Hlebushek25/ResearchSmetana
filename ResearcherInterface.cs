using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace IssleduemSmetanu
{
    public partial class ResearcherInterface : Form
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

        private bool isNavigateButton = false;

        public ResearcherInterface()
        {
            InitializeComponent();

            #region ---------- РАЗВЛЕЧЕНИЯ С РАМОЧКОЙ ----------
            this.Text = "Исследуем сметану";
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
                    g.DrawLine(pen, offset-1, offset-1, closeButton.Width - offset-1, closeButton.Height - offset-1);
                    g.DrawLine(pen, closeButton.Width - offset-1, offset-1, offset-1, closeButton.Height - offset - 1);
                }
            };
            closeButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, closeButton.Width, closeButton.Height, 5, 5));
            closeButton.FlatAppearance.BorderSize = 0;

            closeButton.Click += (sender, e) =>
            {
                isNavigateButton = true;
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
                    g.DrawLine(pen, offsetX-1, offsetY, minimizeButton.Width - offsetX - 4, offsetY); // Горизонтальная линия
                }
            };
            minimizeButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, minimizeButton.Width, minimizeButton.Height, 5, 5));
            minimizeButton.FlatAppearance.BorderSize = 0;

            minimizeButton.Click += (sender, e) =>
            {
                isNavigateButton = true;
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

            button2.Click += async (sender, e) =>
            {
                Dialog error = new Dialog("Произошла критическая ошибка\nпри запуске программы", DialogType.Error);
                error.Show();

                await Task.Delay(2000);

                BSOD bsod = new BSOD();
                bsod.Show();

                await Task.Delay(2000);
                error.Close();
            };

            this.KeyPreview = true;
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.ActiveControl = null;
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

        private void button1_Click(object sender, EventArgs e)
        {
            Dialog error = new Dialog("АААААААААА ОШИБКА\nВот прям на две строки\nИли даже и того больше", DialogType.Error);
            error.ShowDialog();
        }

        private string callDialog(string message, DialogType type)
        {
            Dialog error = new Dialog(message, type);
            error.ShowDialog();
            return error.ActionCode;
        }

        private void textBoxLeave(object sender, EventArgs e)
        {
            CheckValues.checkValue(sender, e);
        }

        private void replaceDecimalSeparator(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                int cursorPosition = textBox.SelectionStart;

                textBox.Text = textBox.Text.Replace('.', ',');

                textBox.SelectionStart = Math.Min(cursorPosition, textBox.Text.Length);
                textBox.SelectionLength = 0;
            }
        }

        private void saveToExcelButton_Click(object sender, EventArgs e)
        {
            bool isContinue = true;
            if (resultsTable.Rows.Count == 1)
            {
                isContinue = callDialog("В таблице нет значений\nУверены, что хотите сохранить?", DialogType.YesOrNo) == "yes";
            }

            if (isContinue)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "xlsx (*xlsx)|*.xlsx|Все файлы (*.*)|*.* ";
                saveFileDialog.Title = "Сохранение данных";
                saveFileDialog.FileName = "Результаты расчётов.xlsx";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filepath = saveFileDialog.FileName;
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Результаты расчётов");

                        worksheet.Cells[1, 1].Value = "Координата по длине канала, м";
                        worksheet.Cells[1, 2].Value = "Температура, °С";
                        worksheet.Cells[1, 3].Value = "Вязкость, Па*с";
                        worksheet.Cells[1, 1].Style.Font.Bold = true;
                        worksheet.Cells[1, 2].Style.Font.Bold = true;
                        worksheet.Cells[1, 3].Style.Font.Bold = true;
                        for (int i = 0; i < resultsTable.Rows.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = resultsTable.Rows[i].Cells[0].Value;
                            worksheet.Cells[i + 2, 2].Value = resultsTable.Rows[i].Cells[1].Value;
                            worksheet.Cells[i + 2, 3].Value = resultsTable.Rows[i].Cells[2].Value;
                        }
                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        var cell = worksheet.Cells[$"A1:C{resultsTable.Rows.Count}"];
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        FileInfo file = new FileInfo(filepath);
                        try
                        {
                            excelPackage.SaveAs(file);
                        }
                        catch (Exception ex)
                        {
                            callDialog("Не удалось сохранить файл (ಥ﹏ಥ)\nВозможно он открыт в другой программе", DialogType.Error);
                        }
                    }
                }
            }
        }
    }
}
