using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Diagnostics;
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
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media.Media3D;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace IssleduemSmetanu
{
    public enum ChartType
    {
        Temperature = 1,
        Viscosity
    };
    public partial class ResearcherInterface : Form
    {
        public string ActionCode { get; private set; }

        double performance = 0;
        double temperature = 0;
        double productViscosity = 0;

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
        
        MathModel smetana = new MathModel();


        public ResearcherInterface()
        {
            InitializeComponent();

            label10.Location = new Point(
                temperatureChart.Location.X + (temperatureChart.Width - label10.Width) / 2,
                temperatureChart.Location.Y + (temperatureChart.Height - label10.Height) / 2
            );

            label12.Location = new Point(
                viscosityChart.Location.X + (viscosityChart.Width - label12.Width) / 2,
                viscosityChart.Location.Y + (viscosityChart.Height - label12.Height) / 2
            );

            #region ---------- Подписка на текстбоксы ----------
            // Геометрические параметры
            SubscribeTextBox(widthTextBox, v => smetana.width = v);
            SubscribeTextBox(heightTextBox, v => smetana.height = v);
            SubscribeTextBox(lengthTextBox, v => smetana.length = v);
            SubscribeTextBox(stepTextBox, v => smetana.step = v);

            // Режимные параметры
            SubscribeTextBox(lidSpeedTextBox, v => smetana.lidSpeed = v);
            SubscribeTextBox(lidTemperatureTextBox, v => smetana.lidTemperature = v);

            // Свойства материала
            SubscribeTextBox(densityTextBox, v => smetana.density = v);
            SubscribeTextBox(specificHeatCapacityTextBox, v => smetana.specificHeatCapacity = v);
            SubscribeTextBox(meltingPointTextBox, v => smetana.meltingPoint = v);

            // Эмпирические коэффициенты
            SubscribeTextBox(viscAtZeroShearAndRefTempTextBox, v => smetana.viscAtZeroShearAndRefTemp = v);
            SubscribeTextBox(viscThermCoeffTextBox, v => smetana.viscThermCoeff = v);
            SubscribeTextBox(castingTempTextBox, v => smetana.castingTemp = v);
            SubscribeTextBox(timeConstTextBox, v => smetana.timeConstant = v);
            SubscribeTextBox(viscosityAnomalyTextBox, v => smetana.viscAnomalyFactor = v);
            SubscribeTextBox(heatTransferRatioTextBox, v => smetana.heatTransferCoefficient = v);
            #endregion

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

            try
            {
                List<Material> materials = InteractionDB.GetAllMaterials();
                materialComboBox.DisplayMember = "nameMaterial";
                materialComboBox.ValueMember = "idMaterial";
                materialComboBox.DataSource = materials;
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка загрузки материалов: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
            }
        }

        #region ---------- Значения в текстбоксах ----------
        private void SubscribeTextBox(TextBox textBox, Action<double> setter)
        {
            textBox.TextChanged += (s, e) => HandleDoubleTextChanged(textBox, setter);
        }

        private void HandleDoubleTextChanged(TextBox textBox, Action<double> setter)
        {
            if (double.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                setter(value);
            }
            else if (!string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                textBox.SelectionStart = textBox.Text.Length;
            }
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
        #endregion

        private void materialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox.SelectedItem == null) return;

            smetana.LoadFromDatabase(((Material)materialComboBox.SelectedItem).idMaterial);

            UpdateTextBoxes();
        }

        private void UpdateTextBoxes()
        {
            widthTextBox.Text = smetana.width.ToString();
            heightTextBox.Text = smetana.height.ToString();
            lengthTextBox.Text = smetana.length.ToString();
            densityTextBox.Text = smetana.density.ToString();
            specificHeatCapacityTextBox.Text = smetana.specificHeatCapacity.ToString();
            meltingPointTextBox.Text = smetana.meltingPoint.ToString();
            lidSpeedTextBox.Text = smetana.lidSpeed.ToString();
            lidTemperatureTextBox.Text = smetana.lidTemperature.ToString();
            viscAtZeroShearAndRefTempTextBox.Text = smetana.viscAtZeroShearAndRefTemp.ToString();
            viscThermCoeffTextBox.Text = smetana.viscThermCoeff.ToString();
            castingTempTextBox.Text = smetana.castingTemp.ToString();
            timeConstTextBox.Text = smetana.timeConstant.ToString();
            viscosityAnomalyTextBox.Text = smetana.viscAnomalyFactor.ToString();
            heatTransferRatioTextBox.Text = smetana.heatTransferCoefficient.ToString();
            stepTextBox.Text = smetana.step.ToString();
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

                textBox.Text = textBox.Text.Replace(',', '.');

                textBox.SelectionStart = Math.Min(cursorPosition, textBox.Text.Length);
                textBox.SelectionLength = 0;
            }
        }

        #region ---------- Сохранение отчета ----------
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

                        worksheet.Cells[1, 5].Value = "Производительность канала, кг/ч";
                        worksheet.Cells[2, 5].Value = "Температура продукта, °С";
                        worksheet.Cells[3, 5].Value = "Вязкость продукта, Па*с";
                        worksheet.Cells[1, 5].Style.Font.Bold = true;
                        worksheet.Cells[2, 5].Style.Font.Bold = true;
                        worksheet.Cells[3, 5].Style.Font.Bold = true;
                        worksheet.Cells[1, 6].Value = performance;
                        worksheet.Cells[2, 6].Value = temperature;
                        worksheet.Cells[3, 6].Value = productViscosity;
                        worksheet.Column(5).AutoFit();
                        worksheet.Column(6).AutoFit();
                        cell = worksheet.Cells[$"E1:F3"];
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[1, 8].Value = "Геометрические параметры канала";
                        worksheet.Cells[1, 8].Style.Font.Bold = true;
                        worksheet.Cells[2, 8].Value = "Ширина, м";
                        worksheet.Cells[3, 8].Value = "Глубина, м";
                        worksheet.Cells[4, 8].Value = "Длина, м";
                        worksheet.Cells[5, 8].Value = "Параметры свойств материала";
                        worksheet.Cells[5, 8].Style.Font.Bold = true;
                        worksheet.Cells[6, 8].Value = "Плотность, кг/м^3";
                        worksheet.Cells[7, 8].Value = "Средняя удельная теплоёмкость, Дж/(кг)*°С";
                        worksheet.Cells[8, 8].Value = "Температура плавления °С";
                        worksheet.Cells[9, 8].Value = "Режимные параметры процесса";
                        worksheet.Cells[9, 8].Style.Font.Bold = true;
                        worksheet.Cells[10, 8].Value = "Скорость крышки, м/с";
                        worksheet.Cells[11, 8].Value = "Температура крышки, °С";
                        worksheet.Cells[12, 8].Value = "Эмперические коэффициенты математической модкли";
                        worksheet.Cells[12, 8].Style.Font.Bold = true;
                        worksheet.Cells[13, 8].Value = "Вязкость материала при нулевой скорости деформации сдвига и температуре приведения, Па*с";
                        worksheet.Cells[14, 8].Value = "Температурный коэффициент вязкости материала, 1/°С";
                        worksheet.Cells[15, 8].Value = "Температура приведения, °С";
                        worksheet.Cells[16, 8].Value = "Постоянная времени, с";
                        worksheet.Cells[17, 8].Value = "Индекс течения материала";
                        worksheet.Cells[18, 8].Value = "Коэффициент теплоотдачи от крышки канала к материалу, Вт/(м^2*°C)";
                        worksheet.Cells[19, 8].Value = "Параметры метода решения уравнений модели";
                        worksheet.Cells[19, 8].Style.Font.Bold = true;
                        worksheet.Cells[20, 8].Value = "Шаг расчёта по длине канала, м";

                        worksheet.Cells[2, 9].Value = smetana.width;
                        worksheet.Cells[3, 9].Value = smetana.height;
                        worksheet.Cells[4, 9].Value = smetana.length;
                        worksheet.Cells[6, 9].Value = smetana.density;
                        worksheet.Cells[7, 9].Value = smetana.specificHeatCapacity;
                        worksheet.Cells[8, 9].Value = smetana.meltingPoint;
                        worksheet.Cells[10, 9].Value = smetana.lidSpeed;
                        worksheet.Cells[11, 9].Value = smetana.lidTemperature;
                        worksheet.Cells[13, 9].Value = smetana.viscAtZeroShearAndRefTemp;
                        worksheet.Cells[14, 9].Value = smetana.viscThermCoeff;
                        worksheet.Cells[15, 9].Value = smetana.castingTemp;
                        worksheet.Cells[16, 9].Value = smetana.timeConstant;
                        worksheet.Cells[17, 9].Value = smetana.viscAnomalyFactor;
                        worksheet.Cells[18, 9].Value = smetana.heatTransferCoefficient;
                        worksheet.Cells[20, 9].Value = smetana.step;

                        worksheet.Column(8).AutoFit();
                        worksheet.Column(9).AutoFit();

                        cell = worksheet.Cells[$"H2:I4"];
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        cell = worksheet.Cells[$"H6:I8"];
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        cell = worksheet.Cells[$"H10:I11"];
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        cell = worksheet.Cells[$"H13:I18"];
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        cell = worksheet.Cells[$"H20:I20"];
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
        #endregion

        private void calculateButton_Click(object sender, EventArgs e)
        {
            List<TextBox> textBoxesToCheck = new List<TextBox>
            {
                widthTextBox, heightTextBox, lengthTextBox, stepTextBox,
                lidSpeedTextBox, lidTemperatureTextBox,
                densityTextBox, specificHeatCapacityTextBox, meltingPointTextBox,
                viscAtZeroShearAndRefTempTextBox, viscThermCoeffTextBox,
                castingTempTextBox, timeConstTextBox, viscosityAnomalyTextBox,
                heatTransferRatioTextBox
            };

            // Проверка на пустые поля
            foreach (var textBox in textBoxesToCheck)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    Dialog error = new Dialog("Не все поля заполнены!\nПожалуйста, проверьте введенные данные.", DialogType.Error);
                    error.ShowDialog();
                    return;
                }
            }

            long memoryBefore = GC.GetTotalMemory(true);
            var (performance, perfTime) = smetana.CalculatePerformance();
            var (tableData, tempTime) = smetana.CalculateTemperature();

            long memoryAfter = GC.GetTotalMemory(true);
            long memoryUsed = memoryAfter - memoryBefore;
            temperature = Math.Round(tableData[1, tableData.GetLength(1) - 1], 1);
            productViscosity = tableData[2, tableData.GetLength(1) - 1];
            DisplayCombinedArrayInTable(resultsTable, tableData);
            DisplayChart(temperatureChart, tableData, ChartType.Temperature);
            DisplayChart(viscosityChart, tableData, ChartType.Viscosity);
            //long memoryAfter = GC.GetTotalMemory(true);
            //long memoryUsed = memoryAfter - memoryBefore;
            int N = (int)Math.Round(smetana.length / smetana.step);
            criteriaIndicatorsLabel.Text = $"Производительность = {performance} [кг/ч]\nТемпература = {temperature} [°C]\nВязкость = {productViscosity} [Па*с]";
            efficiencyLabel.Text = $"Время расчета и визуализации результатов = {perfTime + tempTime} [нс]\nОбъем ОЗУ, необходимой для моделирования объекта = {memoryUsed / 1024.0:F2} КБ\nК-во арифметических операций при расчете = {39+34*N}";
        }

        private void DisplayCombinedArrayInTable(DataGridView resultsTable, double[,] combinedArray)
        {
            if (!int.TryParse(missingStepTextBox.Text, out int skipStep) || skipStep < 1)
            {
                Dialog error = new Dialog("Введите корректный шаг пропуска (целое число ≥ 1)", DialogType.Error);
                error.ShowDialog();
                return;
            }

            int numRecords = combinedArray.GetLength(1);
            resultsTable.ColumnCount = 3;
            resultsTable.Rows.Clear();


            resultsTable.Rows.Add(
                combinedArray[0, 0],
                combinedArray[1, 0],
                combinedArray[2, 0]);

            for (int j = skipStep; j < numRecords - 1; j += skipStep)
            {
                resultsTable.Rows.Add(
                    combinedArray[0, j],
                    combinedArray[1, j],
                    combinedArray[2, j]);
            }

            if (numRecords > 1)
            {
                resultsTable.Rows.Add(
                    combinedArray[0, numRecords - 1],
                    combinedArray[1, numRecords - 1],
                    combinedArray[2, numRecords - 1]);
            }
        }

        private void DisplayChart(Chart chart, double[,] data, ChartType chartType)
        {
            Series series = chartType == ChartType.Temperature ? temperatureChart.Series[0] : viscosityChart.Series[0];
            ChartArea chartArea = chartType == ChartType.Temperature ? temperatureChart.ChartAreas[0] : viscosityChart.ChartAreas[0];
            Label noChartLabel = chartType == ChartType.Temperature ? label10 : label12;
            try
            {
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    series.Points.Add(data[(int)chartType, i]);
                }
                chartArea.AxisY.Minimum = chartType == ChartType.Temperature ? data[1, 0] : data[2, data.GetLength(1) - 1];
                chartArea.AxisY.Maximum = chartType == ChartType.Temperature ? data[1, data.GetLength(1) - 1] : data[2, 0];
                noChartLabel.Visible = false;
            }
            catch
            {
                noChartLabel.Text = "Не удалось построить график (ಥ﹏ಥ)";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.ActionCode = "Logout";
            this.Close();
        }
    }
}
