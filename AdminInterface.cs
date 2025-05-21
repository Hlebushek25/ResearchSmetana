using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace IssleduemSmetanu
{
    public partial class AdminInterface: Form
    {
        public string ActionCode { get; private set; }

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

        private ContextMenuStrip contextMenu;
        private DataGridViewRow selectedRow;

        // Штука для обработки ошибки загрузки данных в таблицу с пользователями
        private bool isErrorHandled = false;
        private int previousTabIndex = 0;
        private bool emptyRowsHandled = false;

        // Массивы с данными для таблиц
        List<User> users = InteractionDB.GetAllUsers();
        List<Role> roles = InteractionDB.GetAllRoles();
        List<Material> materials = InteractionDB.GetAllMaterials();
        List<MaterialCharacteristic> characteristics = InteractionDB.GetAllMaterialCharacteristics();
        List<MaterialCharacteristicValue> characteristicValues = InteractionDB.GetAllMaterialCharacteristicsValues();
        List<EmpericalCoef> coefs = InteractionDB.GetAllEmpericalCoef();
        List<EmpericalCoefValue> coefValues = InteractionDB.GetAllEmpericalCoefValues();

        public AdminInterface()
        {
            InitializeComponent();

            userTable.CellValueChanged += (sender, e) => SaveChanges();
            materialTable.CellValueChanged += (sender, e) => SaveChanges();
            materialCharacteristicsTable.CellValueChanged += (sender, e) => SaveChanges();
            materialCharacteristicsValuesTable.CellValueChanged += (sender, e) => SaveChanges();
            empiricalCoefTable.CellValueChanged += (sender, e) => SaveChanges();
            empiricalCoefValuesTable.CellValueChanged += (sender, e) => SaveChanges();

            try
            {
                DataGridViewComboBoxColumn comboColumn = (DataGridViewComboBoxColumn)userTable.Columns["Role"];
                //comboColumn.Items.AddRange("Исследователь", "Администратор", "");
                foreach (var role in roles)
                {
                    comboColumn.Items.Add(role.nameRole);
                }
            }
            catch
            {
                Dialog dialog = new Dialog("В таблице \"Пользователи\" отсутствует столбец \"Роль\"", DialogType.Error);
                dialog.ShowDialog();
            }

            try
            {
                DataGridViewComboBoxColumn comboColumn2 = (DataGridViewComboBoxColumn)materialCharacteristicsValuesTable.Columns["materialID_inCharValues"];
                foreach (var material in materials)
                {
                    comboColumn2.Items.Add(material.nameMaterial.ToString());
                }
                DataGridViewComboBoxColumn comboColumn3 = (DataGridViewComboBoxColumn)materialCharacteristicsValuesTable.Columns["characteristicID_inCharValues"];
                foreach (var characteristic in characteristics)
                {
                    comboColumn3.Items.Add(characteristic.name.ToString());
                }
            }
            catch
            {
                Dialog dialog = new Dialog("В таблице \"Значения параметров свойств\" отсутствуют столбцы \"id материала\" и/или \"id свойства\"", DialogType.Error);
                dialog.ShowDialog();
            }

            try
            {
                DataGridViewComboBoxColumn comboColumn4 = (DataGridViewComboBoxColumn)empiricalCoefValuesTable.Columns["materialID_inEmpCoefValue"];
                foreach (var material in materials)
                {
                    comboColumn4.Items.Add(material.nameMaterial.ToString());
                }
                DataGridViewComboBoxColumn comboColumn5 = (DataGridViewComboBoxColumn)empiricalCoefValuesTable.Columns["empiricalCoefID_inEmpCoefValues"];
                foreach (var coef in coefs)
                {
                    comboColumn5.Items.Add(coef.name.ToString());
                }
            }
            catch
            {
                Dialog dialog = new Dialog("В таблице \"Значения параметров свойств\" отсутствуют столбцы \"id материала\" и/или \"id коэффициента\"", DialogType.Error);
                dialog.ShowDialog();
            }

            // меню для удаляшки строк
            contextMenu = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("Удалить строку");
            deleteItem.Click += DeleteRow_Click;
            contextMenu.Items.Add(deleteItem);

            LoadUsersToTable();
            LoadMaterialsToTable();
            LoadMaterialCharacteristicsToTable();
            LoadEmpericalCoefToTable();
            LoadMaterialCharacteristicsValuesToTable();
            LoadEmpericalCoefValuesToTable();

            string smetanaBackupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "smetana.backup");
            string usersBackupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "users.backup");
            if (File.Exists(smetanaBackupPath) && File.Exists(usersBackupPath))
            {
                lastBackupLabel.Text = $"Последнее копирование: {Properties.Settings.Default.LastBackupTime.ToString("dd.MM.yyyy HH:mm")}";
            }
            else if (!File.Exists(smetanaBackupPath) && !File.Exists(usersBackupPath))
            {
                lastBackupLabel.Text = "Резервная копия отсутствует";
                loadBackupButton.Enabled = false;
            }
            else
            {
                lastBackupLabel.Text = "Часть резервной копии не найдена!";
            }


                tryQuantityTextBox.Text = (Properties.Settings.Default.DefaultLoginTryQuantity + 1).ToString();

            switch (Properties.Settings.Default.LoginTimeout)
            {
                case 60:
                    comboBox1.SelectedIndex = 0;
                    break;
                case 120:
                    comboBox1.SelectedIndex = 1;
                    break;
                case 300:
                    comboBox1.SelectedIndex = 2;
                    break;
                case 600:
                    comboBox1.SelectedIndex = 3;
                    break;
                case 900:
                    comboBox1.SelectedIndex = 4;
                    break;
                case 1800:
                    comboBox1.SelectedIndex = 5;
                    break;
                case 3600:
                    comboBox1.SelectedIndex = 6;
                    break;
                case 7200:
                    comboBox1.SelectedIndex = 7;
                    break;
                case 14400:
                    comboBox1.SelectedIndex = 8;
                    break;
                case 43200:
                    comboBox1.SelectedIndex = 9;
                    break;
                case 86400:
                    comboBox1.SelectedIndex = 10;
                    break;
                case 259200:
                    comboBox1.SelectedIndex = 11;
                    break;
                case 604800:
                    comboBox1.SelectedIndex = 12;
                    break;
                case 2592000:
                    comboBox1.SelectedIndex = 13;
                    break;
                case 31557600:
                    comboBox1.SelectedIndex = 14;
                    break;
                case 157788000:
                    comboBox1.SelectedIndex = 15;
                    break;
            }

            #region ---------- РАЗВЛЕЧЕНИЯ С РАМОЧКОЙ ----------
            this.Text = "Заведующий исследователями сметаны";
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

        public string callDialog(string message, DialogType type)
        {
            Dialog error = new Dialog(message, type);
            error.ShowDialog();
            return error.ActionCode;
        }

        private void tryQuantityTextBox_Leave(object sender, EventArgs e)
        {
            if (uint.TryParse(tryQuantityTextBox.Text, out uint result))
            {
                if (result <= 50)
                {
                    Properties.Settings.Default.DefaultLoginTryQuantity = result - 1;
                    Properties.Settings.Default.LoginTryQuantity = result - 1;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Dialog dialog = new Dialog("Максимальное количество попыток 50", DialogType.Error);
                    dialog.ShowDialog();
                    tryQuantityTextBox.Focus();
                }
            }
            else
            {
                Dialog dialog = new Dialog("Введённое значение некорректно", DialogType.Error);
                dialog.ShowDialog();
                tryQuantityTextBox.Focus();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.LoginTimeout = 60;
                    break;
                case 1:
                    Properties.Settings.Default.LoginTimeout = 120;
                    break;
                case 2:
                    Properties.Settings.Default.LoginTimeout = 300;
                    break;
                case 3:
                    Properties.Settings.Default.LoginTimeout = 600;
                    break;
                case 4:
                    Properties.Settings.Default.LoginTimeout = 900;
                    break;
                case 5:
                    Properties.Settings.Default.LoginTimeout = 1800;
                    break;
                case 6:
                    Properties.Settings.Default.LoginTimeout = 3600;
                    break;
                case 7:
                    Properties.Settings.Default.LoginTimeout = 7200;
                    break;
                case 8:
                    Properties.Settings.Default.LoginTimeout = 14400;
                    break;
                case 9:
                    Properties.Settings.Default.LoginTimeout = 43200;
                    break;
                case 10:
                    Properties.Settings.Default.LoginTimeout = 86400;
                    break;
                case 11:
                    Properties.Settings.Default.LoginTimeout = 259200;
                    break;
                case 12:
                    Properties.Settings.Default.LoginTimeout = 604800;
                    break;
                case 13:
                    Properties.Settings.Default.LoginTimeout = 2592000;
                    break;
                case 14:
                    Properties.Settings.Default.LoginTimeout = 31557600;
                    break;
                case 15:
                    Properties.Settings.Default.LoginTimeout = 157788000;
                    break;
            }

            Properties.Settings.Default.Save();
        }

        // Автоматическое заполнение ID
        private void RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridView table = sender as DataGridView;
            int maxValue = 0;

            foreach (DataGridViewRow row in table.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    int value = Convert.ToInt32(row.Cells[0].Value);
                    if (value > maxValue)
                    {
                        maxValue = value;
                    }
                }
            }

            table.Rows[table.RowCount - 2].Cells[0].Value = maxValue + 1;
        }

        // Ввод только чисел в некоторые столбцы
        private void CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //DataGridView table = sender as DataGridView;
            //string columnName = table.Columns[e.ColumnIndex].Name;
            //switch (columnName)
            //{
            //    //case "materialID_inCharValues": case "characteristicID_inCharValues": case "materialID_inEmpCoefValue": case "empiricalCoefID_inEmpCoefValues":
            //    //    if (e.FormattedValue.ToString() != "")
            //    //    {
            //    //        if (!int.TryParse(e.FormattedValue.ToString(), out _))
            //    //        {
            //    //            e.Cancel = true;
            //    //            Dialog dialog = new Dialog("Значение в данном столбце должно быть целым числом.", DialogType.Error);
            //    //            dialog.ShowDialog();
            //    //        }
            //    //    }
            //    //    break;
            //    case "craracteristicValue": case "empiricalCoefValue":
            //        if (e.FormattedValue.ToString() != "")
            //        {
            //            if (!double.TryParse(e.FormattedValue.ToString(), out _))
            //            {
            //                e.Cancel = true;
            //                Dialog dialog = new Dialog("Значение в данном столбце должно быть числом.", DialogType.Error);
            //                dialog.ShowDialog();
            //            }
            //        }
            //    break;
            //}

            DataGridView table = sender as DataGridView;
            string columnName = table.Columns[e.ColumnIndex].Name;

            try
            {
                switch (columnName)
                {
                    case "craracteristicValue":
                    case "empiricalCoefValue":
                        if (e.FormattedValue.ToString() != "")
                        {
                            if (!double.TryParse(e.FormattedValue.ToString(), out _))
                            {
                                e.Cancel = true;
                                Dialog dialog = new Dialog("Значение в данном столбце должно быть числом.", DialogType.Error);
                                dialog.ShowDialog();
                            }
                        }
                        break;

                    case "Role":
                        if (e.FormattedValue.ToString() != "")
                        {
                            if (!roles.Any(r => r.nameRole == e.FormattedValue.ToString()))
                            {
                                e.Cancel = true;
                                Dialog dialog = new Dialog("Указанной роли не существует.", DialogType.Error);
                                dialog.ShowDialog();
                            }
                        }
                        break;

                    case "materialID_inCharValues":
                    case "materialID_inEmpCoefValue":
                        if (e.FormattedValue.ToString() != "")
                        {
                            if (!materials.Any(m => m.nameMaterial == e.FormattedValue.ToString()))
                            {
                                e.Cancel = true;
                                Dialog dialog = new Dialog("Указанного материала не существует.", DialogType.Error);
                                dialog.ShowDialog();
                            }
                        }
                        break;

                    case "characteristicID_inCharValues":
                        if (e.FormattedValue.ToString() != "")
                        {
                            if (!characteristics.Any(c => c.name == e.FormattedValue.ToString()))
                            {
                                e.Cancel = true;
                                Dialog dialog = new Dialog("Указанной характеристики не существует.", DialogType.Error);
                                dialog.ShowDialog();
                            }
                        }
                        break;

                    case "empiricalCoefID_inEmpCoefValues":
                        if (e.FormattedValue.ToString() != "")
                        {
                            if (!coefs.Any(c => c.name == e.FormattedValue.ToString()))
                            {
                                e.Cancel = true;
                                Dialog dialog = new Dialog("Указанного коэффициента не существует.", DialogType.Error);
                                dialog.ShowDialog();
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка при проверке данных: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
                e.Cancel = true;
            }
        }

        #region Удаляшка строчек
        private void TableMouseDown(object sender, MouseEventArgs e)
        {
            DataGridView table = sender as DataGridView;
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = table.HitTest(e.X, e.Y);
                if (hitTestInfo.RowIndex >= 0)
                {
                    selectedRow = table.Rows[hitTestInfo.RowIndex];
                    contextMenu.Show(table, e.Location);
                }
            }
        }

        private void DeleteRow_Click(object sender, EventArgs e)
        {
            //if (selectedRow != null)
            //{
            //    try
            //    {
            //        DataGridView table = selectedRow.DataGridView;
            //        table.Rows.Remove(selectedRow);
            //        selectedRow = null;
            //    }
            //    catch
            //    {
            //        Dialog dialog = new Dialog("Ты кого удаляешь, дурында??", DialogType.Error);
            //        dialog.ShowDialog();
            //    }
            //}

            if (selectedRow != null)
            {
                try
                {
                    DataGridView table = selectedRow.DataGridView;

                    // Проверяем, можно ли удалять из этой таблицы
                    if (table != userTable && table != materialCharacteristicsValuesTable && table != empiricalCoefValuesTable)
                    {
                        Dialog dialog = new Dialog("Удаление записей из этой таблицы запрещено!", DialogType.Error);
                        dialog.ShowDialog();
                        return;
                    }

                    // Получаем ID удаляемой записи
                    int id = Convert.ToInt32(selectedRow.Cells[0].Value);

                    // Удаляем из базы данных
                    if (table == userTable)
                    {
                        InteractionDB.DeleteUser(id);
                        users = InteractionDB.GetAllUsers(); // Обновляем список пользователей
                    }
                    else if (table == materialCharacteristicsValuesTable)
                    {
                        InteractionDB.DeleteMaterialCharacteristicValue(id);
                        characteristicValues = InteractionDB.GetAllMaterialCharacteristicsValues(); // Обновляем список
                    }
                    else if (table == empiricalCoefValuesTable)
                    {
                        InteractionDB.DeleteEmpericalCoefValue(id);
                        coefValues = InteractionDB.GetAllEmpericalCoefValues(); // Обновляем список
                    }

                    // Удаляем из таблицы
                    table.Rows.Remove(selectedRow);
                    selectedRow = null;
                }
                catch (Exception ex)
                {
                    Dialog dialog = new Dialog($"Ошибка при удалении: {ex.Message}", DialogType.Error);
                    dialog.ShowDialog();
                }
            }
        }

        private void EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox textBox)
            {
                textBox.ContextMenuStrip = new ContextMenuStrip();
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            this.ActionCode = "Logout";
            this.Close();
        }


        private void LoadUsersToTable()
        {
            try
            {
                //List<User> users = InteractionDB.GetAllUsers();
                foreach (User user in users)
                {
                    //switch (user.role)
                    //{
                    //    case "admin":
                    //        user.role = "Администратор";
                    //        break;
                    //    case "researcher":
                    //        user.role = "Исследователь";
                    //        break;
                    //    default:
                    //        Dialog dialog = new Dialog($"Ошибка при загрузке данных: Неизвестная роль {user.role}", DialogType.Error);
                    //        dialog.ShowDialog();
                    //        break;
                    //}
                    userTable.Rows.Add(user.login, user.password, user.role);
                }
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка при загрузке данных: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
            }
        }

        private void LoadMaterialsToTable()
        {
            try
            {
                //List<Material> materials = InteractionDB.GetAllMaterials();
                foreach (Material material in materials)
                {
                    int rowIndex = materialTable.Rows.Add();
                    materialTable.Rows[rowIndex].Cells[0].Value = material.idMaterial;
                    materialTable.Rows[rowIndex].Cells[1].Value = material.nameMaterial;
                }
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка при загрузке данных: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
            }
        }

        private void LoadMaterialCharacteristicsToTable()
        {
            try
            {
                //List<MaterialCharacteristic> characteristics = InteractionDB.GetAllMaterialCharacteristics();
                foreach (MaterialCharacteristic characteristic in characteristics)
                {
                    int rowIndex = materialCharacteristicsTable.Rows.Add();
                    materialCharacteristicsTable.Rows[rowIndex].Cells[0].Value = characteristic.id;
                    materialCharacteristicsTable.Rows[rowIndex].Cells[1].Value = characteristic.name;
                    materialCharacteristicsTable.Rows[rowIndex].Cells[2].Value = characteristic.unit;
                }
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка при загрузке данных: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
            }
        }

        private void LoadMaterialCharacteristicsValuesToTable()
        {
            try
            {
                //List<MaterialCharacteristicValue> characteristicValues = InteractionDB.GetAllMaterialCharacteristicsValues();

                //foreach (MaterialCharacteristicValue characteristicValue in characteristicValues)
                //{
                //    int rowIndex = materialCharacteristicsValuesTable.Rows.Add();
                //    materialCharacteristicsValuesTable.Rows[rowIndex].Cells[1].Value = characteristicValue.nameMaterial;
                //    materialCharacteristicsValuesTable.Rows[rowIndex].Cells[2].Value = characteristicValue.nameCharacteristic;
                //    materialCharacteristicsValuesTable.Rows[rowIndex].Cells[3].Value = characteristicValue.value;
                //}
                foreach (MaterialCharacteristicValue characteristicValue in characteristicValues)
                {
                    int rowIndex = materialCharacteristicsValuesTable.Rows.Add();
                    materialCharacteristicsValuesTable.Rows[rowIndex].Cells[0].Value = characteristicValue.id;
                    materialCharacteristicsValuesTable.Rows[rowIndex].Cells[1].Value = characteristicValue.nameMaterial;
                    materialCharacteristicsValuesTable.Rows[rowIndex].Cells[2].Value = characteristicValue.nameCharacteristic;
                    materialCharacteristicsValuesTable.Rows[rowIndex].Cells[3].Value = characteristicValue.value;
                }
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка при загрузке данных: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
            }
        }

        private void LoadEmpericalCoefToTable()
        {
            try
            {
                //List<EmpericalCoef> coefs = InteractionDB.GetAllEmpericalCoef();
                foreach (EmpericalCoef coef in coefs)
                {
                    int rowIndex = empiricalCoefTable.Rows.Add();
                    empiricalCoefTable.Rows[rowIndex].Cells[0].Value = coef.id;
                    empiricalCoefTable.Rows[rowIndex].Cells[1].Value = coef.name;
                    empiricalCoefTable.Rows[rowIndex].Cells[2].Value = coef.unit;
                }
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка при загрузке данных: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
            }
        }
        private void LoadEmpericalCoefValuesToTable()
        {
            try
            {
                //List<EmpericalCoefValue> coefValues = InteractionDB.GetAllEmpericalCoefValues();
                foreach (EmpericalCoefValue coefValue in coefValues)
                {
                    int rowIndex = empiricalCoefValuesTable.Rows.Add();
                    empiricalCoefValuesTable.Rows[rowIndex].Cells[0].Value = coefValue.id;
                    empiricalCoefValuesTable.Rows[rowIndex].Cells[1].Value = coefValue.nameMaterial;
                    empiricalCoefValuesTable.Rows[rowIndex].Cells[2].Value = coefValue.nameEmpericalCoef;
                    empiricalCoefValuesTable.Rows[rowIndex].Cells[3].Value = coefValue.value;
                }
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка при загрузке данных: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (emptyRowsHandled)
            {
                emptyRowsHandled = false;
                previousTabIndex = tabControl1.SelectedIndex;

                return;
            }
            isErrorHandled = false;
            DataGridView table = null;
            switch (previousTabIndex)
            {
                case 0:
                    table = materialTable;
                    break;
                case 1:
                    table = materialCharacteristicsTable;
                    break;
                case 2:
                    table = materialCharacteristicsValuesTable;
                    break;
                case 3:
                    table = userTable;
                    break;
                case 5:
                    table = empiricalCoefTable;
                    break;
                case 6:
                    table = empiricalCoefValuesTable;
                    break;
            }
            if (table == null)
            {
                previousTabIndex = tabControl1.SelectedIndex;
                return;
            }
            else
            {
                
                if (table.Rows.Count >= 2)
                {
                    List<int> emptyRows = new List<int>();
                    for (int i = table.Rows.Count - 2; i >= 0; i--) // Проходим по строкам с конца
                    {
                        foreach (DataGridViewCell cell in table.Rows[i].Cells)
                        {
                            if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                            {
                                emptyRows.Add(i); // Добавляем индекс пустой строки в список
                                break; // Можно сразу выйти, если найдена пустая ячейка
                            }
                        }
                    }

                    if (emptyRows.Count != 0)
                    {
                        if (callDialog("У Вас остались незаполненные строки!\nПри переходе в другую вкладку они удалятся.\nУверены, что хотите перейти?", DialogType.YesOrNo) == "yes")
                        {
                            foreach (int rowIndex in emptyRows)
                            {
                                table.Rows.RemoveAt(rowIndex);
                            }
                            // ТУТА НАДА УДАЛЯШКУ 1111111111111!!!!!!!!!!
                        }
                        else
                        {
                            emptyRowsHandled = true;
                            tabControl1.SelectedIndex = previousTabIndex; // Возвращаемся на предыдущую вкладку
                            foreach (int rowIndex in emptyRows)
                            {
                                table.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Yellow;
                            }
                            return;
                        }
                    }
                }
            }
            previousTabIndex = tabControl1.SelectedIndex;
        }

        private void userTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!isErrorHandled)
            {
                Dialog dialog = new Dialog($"Ошибка при загрузке данных (ಥ﹏ಥ)\nУбедитесь, что столбцы расположены в правильном порядке (логин, пароль, роль) и роли пользователей указаны корректно!\nСтроки с ошибками были выделены красным, а все пользователям с некорректными ролями была выдана роль \"Исследователь\"", DialogType.Error);
                dialog.ShowDialog();
                isErrorHandled = true;
            }
            userTable.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            e.Cancel = true;
        }

        private void materialCharacteristicsValuesTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!isErrorHandled)
            {
                Dialog dialog = new Dialog($"Ошибка при загрузке данных (ಥ﹏ಥ)\nУбедитесь, что столбцы расположены в правильном порядке (логин, пароль, роль) и роли пользователей указаны корректно!\nСтроки с ошибками были выделены красным, а все поля с ошибками заполнены значениями по умолчанию", DialogType.Error);
                dialog.ShowDialog();
                isErrorHandled = true;
            }
            materialCharacteristicsValuesTable.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            e.Cancel = true;
        }

        private void empiricalCoefValuesTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!isErrorHandled)
            {
                Dialog dialog = new Dialog($"Ошибка при загрузке данных (ಥ﹏ಥ)\nУбедитесь, что столбцы расположены в правильном порядке (логин, пароль, роль) и роли пользователей указаны корректно!\nСтроки с ошибками были выделены красным, а все поля с ошибками заполнены значениями по умолчанию", DialogType.Error);
                dialog.ShowDialog();
                isErrorHandled = true;
            }
            empiricalCoefValuesTable.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            e.Cancel = true;
        }

        private void incompleteRowsCheck(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView table = sender as DataGridView;
            int lastRowIndex = table.AllowUserToAddRows ? table.Rows.Count - 2 : table.Rows.Count - 1;

            for (int i = 0; i <= lastRowIndex; i++) // Исключаем последнюю строку
            {
                DataGridViewRow row = table.Rows[i];
                bool isRowComplete = true;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        isRowComplete = false;
                        break;
                    }
                }

                row.DefaultCellStyle.BackColor = isRowComplete ? Color.White : Color.Yellow;
            }
        }

        ////////////
        private void SaveChanges()
        {
            try
            {
                if (tabControl1.SelectedTab == Users) 
                {
                    SaveUsersChanges();
                }
                else if (tabControl1.SelectedTab == Materials) 
                {
                    SaveMaterialsChanges();
                }
                else if (tabControl1.SelectedTab == MaterialCharacteristics) 
                {
                    SaveMaterialCharacteristicsChanges();
                }
                else if (tabControl1.SelectedTab == MaterialCharacteristicValues) 
                {
                    SaveMaterialCharacteristicsValuesChanges();
                }
                else if (tabControl1.SelectedTab == EmpiricalCoef) 
                {
                    SaveEmpericalCoefChanges();
                }
                else if (tabControl1.SelectedTab == EmpiricalCoefValues) 
                {
                    SaveEmpericalCoefValuesChanges();
                }
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка при сохранении данных: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
            }
        }

        private void SaveUsersChanges()
        {
            List<User> currentUsers = InteractionDB.GetAllUsers();

            foreach (DataGridViewRow row in userTable.Rows)
            {
                if (row.IsNewRow) continue;

                string login = row.Cells[0].Value?.ToString();
                string password = row.Cells[1].Value?.ToString();
                string role = row.Cells[2].Value?.ToString();

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
                    continue;

                var existingUser = currentUsers.FirstOrDefault(u => u.login == login);

                if (existingUser != null)
                {
                    existingUser.password = password;
                    existingUser.role = role;
                    InteractionDB.UpdateUser(existingUser, roles);
                }
                else
                {
                    User newUser = new User
                    {
                        login = login,
                        password = password,
                        role = role
                    };
                    InteractionDB.AddUser(newUser, roles);
                }
            }

            var tableLogins = new List<string>();
            foreach (DataGridViewRow row in userTable.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells[0].Value != null)
                    tableLogins.Add(row.Cells[0].Value.ToString());
            }

            foreach (var user in currentUsers)
            {
                if (!tableLogins.Contains(user.login))
                {
                    InteractionDB.DeleteUser(user.idUser);
                }
            }

            users = InteractionDB.GetAllUsers();
        }

        private void SaveMaterialsChanges()
        {
            List<Material> currentMaterials = InteractionDB.GetAllMaterials();

            foreach (DataGridViewRow row in materialTable.Rows)
            {
                if (row.IsNewRow) continue;

                int id = Convert.ToInt32(row.Cells[0].Value);
                string name = row.Cells[1].Value?.ToString();

                if (string.IsNullOrEmpty(name))
                    continue;

                var existingMaterial = currentMaterials.FirstOrDefault(m => m.idMaterial == id);

                if (existingMaterial != null)
                {
                    if (existingMaterial.nameMaterial != name)
                    {
                        existingMaterial.nameMaterial = name;
                        InteractionDB.UpdateMaterial(existingMaterial);
                    }
                }
                else
                {
                    Material newMaterial = new Material
                    {
                        idMaterial = id,
                        nameMaterial = name
                    };
                    InteractionDB.AddMaterial(newMaterial);
                }
            }

            materials = InteractionDB.GetAllMaterials();
        }

        private void SaveMaterialCharacteristicsChanges()
        {
            List<MaterialCharacteristic> currentCharacteristics = InteractionDB.GetAllMaterialCharacteristics();

            foreach (DataGridViewRow row in materialCharacteristicsTable.Rows)
            {
                if (row.IsNewRow) continue;

                int id = Convert.ToInt32(row.Cells[0].Value);
                string name = row.Cells[1].Value?.ToString();
                string unit = row.Cells[2].Value?.ToString();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(unit))
                    continue;

                var existingChar = currentCharacteristics.FirstOrDefault(c => c.id == id);

                if (existingChar != null)
                {
                    if (existingChar.name != name || existingChar.unit != unit)
                    {
                        existingChar.name = name;
                        existingChar.unit = unit;
                        InteractionDB.UpdateMaterialCharacteristic(existingChar);
                    }
                }
                else
                {
                    MaterialCharacteristic newChar = new MaterialCharacteristic
                    {
                        id = id,
                        name = name,
                        unit = unit
                    };
                    InteractionDB.AddMaterialCharacteristic(newChar);
                }
            }

            characteristics = InteractionDB.GetAllMaterialCharacteristics();
        }

        private void SaveMaterialCharacteristicsValuesChanges()
        {
            List<MaterialCharacteristicValue> currentValues = InteractionDB.GetAllMaterialCharacteristicsValues();

            foreach (DataGridViewRow row in materialCharacteristicsValuesTable.Rows)
            {
                if (row.IsNewRow) continue;

                int id = Convert.ToInt32(row.Cells[0].Value);
                string materialName = row.Cells[1].Value?.ToString();
                string charName = row.Cells[2].Value?.ToString();
                double value = Convert.ToDouble(row.Cells[3].Value);

                if (string.IsNullOrEmpty(materialName) || string.IsNullOrEmpty(charName))
                    continue;

                int materialId = materials.First(m => m.nameMaterial == materialName).idMaterial;
                int charId = characteristics.First(c => c.name == charName).id;

                var existingValue = currentValues.FirstOrDefault(v => v.id == id);

                if (existingValue != null)
                {
                    if (existingValue.idMaterial != materialId || existingValue.idCharacteristic != charId || existingValue.value != value)
                    {
                        existingValue.idMaterial = materialId;
                        existingValue.idCharacteristic = charId;
                        existingValue.value = value;
                        InteractionDB.UpdateMaterialCharacteristicValue(existingValue);
                    }
                }
                else
                {
                    MaterialCharacteristicValue newValue = new MaterialCharacteristicValue
                    {
                        id = id,
                        idMaterial = materialId,
                        idCharacteristic = charId,
                        value = value
                    };
                    InteractionDB.AddMaterialCharacteristicValue(newValue);
                }
            }

            var tableIds = new List<int>();
            foreach (DataGridViewRow row in materialCharacteristicsValuesTable.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells[0].Value != null)
                    tableIds.Add(Convert.ToInt32(row.Cells[0].Value));
            }

            foreach (var value in currentValues)
            {
                if (!tableIds.Contains(value.id))
                {
                    InteractionDB.DeleteMaterialCharacteristicValue(value.id);
                }
            }

            characteristicValues = InteractionDB.GetAllMaterialCharacteristicsValues();
        }

        private void SaveEmpericalCoefChanges()
        {
            List<EmpericalCoef> currentCoefs = InteractionDB.GetAllEmpericalCoef();

            foreach (DataGridViewRow row in empiricalCoefTable.Rows)
            {
                if (row.IsNewRow) continue;

                int id = Convert.ToInt32(row.Cells[0].Value);
                string name = row.Cells[1].Value?.ToString();
                string unit = row.Cells[2].Value?.ToString();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(unit))
                    continue;

                var existingCoef = currentCoefs.FirstOrDefault(c => c.id == id);

                if (existingCoef != null)
                {
                    if (existingCoef.name != name || existingCoef.unit != unit)
                    {
                        existingCoef.name = name;
                        existingCoef.unit = unit;
                        InteractionDB.UpdateEmpericalCoef(existingCoef);
                    }
                }
                else
                {
                    EmpericalCoef newCoef = new EmpericalCoef
                    {
                        id = id,
                        name = name,
                        unit = unit
                    };
                    InteractionDB.AddEmpericalCoef(newCoef);
                }
            }

            coefs = InteractionDB.GetAllEmpericalCoef();
        }

        private void SaveEmpericalCoefValuesChanges()
        {
            List<EmpericalCoefValue> currentValues = InteractionDB.GetAllEmpericalCoefValues();

            foreach (DataGridViewRow row in empiricalCoefValuesTable.Rows)
            {
                if (row.IsNewRow) continue;

                int id = Convert.ToInt32(row.Cells[0].Value);
                string materialName = row.Cells[1].Value?.ToString();
                string coefName = row.Cells[2].Value?.ToString();
                double value = Convert.ToDouble(row.Cells[3].Value);

                if (string.IsNullOrEmpty(materialName) || string.IsNullOrEmpty(coefName))
                    continue;

                int materialId = materials.First(m => m.nameMaterial == materialName).idMaterial;
                int coefId = coefs.First(c => c.name == coefName).id;

                var existingValue = currentValues.FirstOrDefault(v => v.id == id);

                if (existingValue != null)
                {
                    if (existingValue.idMaterial != materialId || existingValue.idEmpericalCoef != coefId || existingValue.value != value)
                    {
                        existingValue.idMaterial = materialId;
                        existingValue.idEmpericalCoef = coefId;
                        existingValue.value = value;
                        InteractionDB.UpdateEmpericalCoefValue(existingValue);
                    }
                }
                else
                {
                    EmpericalCoefValue newValue = new EmpericalCoefValue
                    {
                        id = id,
                        idMaterial = materialId,
                        idEmpericalCoef = coefId,
                        value = value
                    };
                    InteractionDB.AddEmpericalCoefValue(newValue);
                }
            }

            var tableIds = new List<int>();
            foreach (DataGridViewRow row in empiricalCoefValuesTable.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells[0].Value != null)
                    tableIds.Add(Convert.ToInt32(row.Cells[0].Value));
            }

            foreach (var value in currentValues)
            {
                if (!tableIds.Contains(value.id))
                {
                    InteractionDB.DeleteEmpericalCoefValue(value.id);
                }
            }

            coefValues = InteractionDB.GetAllEmpericalCoefValues();
        }

        private void createBackupButton_Click(object sender, EventArgs e)
        {
            string smetanaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "Smetana.db");
            string smetanaBackupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "smetana.backup");
            string usersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "Users.db");
            string usersBackupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "users.backup");

            try
            {
                File.Copy(smetanaPath, smetanaBackupPath, true);
                File.Copy(usersPath, usersBackupPath, true);
                DateTime currentDate = DateTime.Now;
                lastBackupLabel.Text = $"Последнее копирование: {currentDate.ToString("dd.MM.yyyy HH:mm")}";
                Properties.Settings.Default.LastBackupTime = currentDate;
                Properties.Settings.Default.Save();
                loadBackupButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Dialog dialog = new Dialog($"Ошибка при создании резервной копии: {ex.Message}", DialogType.Error);
                dialog.ShowDialog();
            }
        }

        private async void loadBackupButton_Click(object sender, EventArgs e)
        {
            string smetanaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "Smetana.db");
            string smetanaBackupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "smetana.backup");
            string usersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "Users.db");
            string usersBackupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Databases", "users.backup");
            string previousText = lastBackupLabel.Text;

            if (File.Exists(smetanaBackupPath) && File.Exists(usersBackupPath))
            {
                File.Copy(smetanaBackupPath, smetanaPath, true);
                File.Copy(usersBackupPath, usersPath, true);
                lastBackupLabel.Text = "Данные успешно восстановлены!";
                await Task.Delay(5000);
                lastBackupLabel.Text = previousText;
            }
            else if (!File.Exists(smetanaBackupPath) && File.Exists(usersBackupPath))
            {
                if (callDialog("Резервная копия базы данных материалов и свойств не найдена!\nХотите восстановить только базу данных пользователей?", DialogType.YesOrNo) == "yes")
                {
                    File.Copy(usersBackupPath, usersPath, true);
                    lastBackupLabel.Text = "Данные успешно восстановлены!";
                    await Task.Delay(5000);
                    lastBackupLabel.Text = previousText;
                }
            }
            else if (File.Exists(smetanaBackupPath) && !File.Exists(usersBackupPath))
            {
                if (callDialog("Резервная копия базы данных пользователей не найдена!\nХотите восстановить только базу данных материалов и свойств?", DialogType.YesOrNo) == "yes")
                {
                    File.Copy(smetanaBackupPath, smetanaPath, true);
                    lastBackupLabel.Text = "Данные успешно восстановлены!";
                    await Task.Delay(5000);
                    lastBackupLabel.Text = previousText;
                }
            }
            else
            {
                callDialog("Резервные копии баз данных не найдены!", DialogType.Error);
            }
        }
    }
}
