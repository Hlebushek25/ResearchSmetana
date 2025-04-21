namespace IssleduemSmetanu
{
    partial class AdminInterface
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Materials = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Users = new System.Windows.Forms.TabPage();
            this.usersTable = new System.Windows.Forms.DataGridView();
            this.Usernames = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Passwords = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Settings = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.specificHeatCapacityLabel = new System.Windows.Forms.Label();
            this.tryQuantityTextBox = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.timeoutHoursTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.timeoutMinutesTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.timeoutSecondsTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.Materials.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.Users.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.usersTable)).BeginInit();
            this.Settings.SuspendLayout();
            this.flowLayoutPanel8.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Materials);
            this.tabControl1.Controls.Add(this.Users);
            this.tabControl1.Controls.Add(this.Settings);
            this.tabControl1.Location = new System.Drawing.Point(4, 36);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(792, 410);
            this.tabControl1.TabIndex = 0;
            // 
            // Materials
            // 
            this.Materials.Controls.Add(this.dataGridView1);
            this.Materials.Location = new System.Drawing.Point(4, 25);
            this.Materials.Name = "Materials";
            this.Materials.Padding = new System.Windows.Forms.Padding(3);
            this.Materials.Size = new System.Drawing.Size(784, 381);
            this.Materials.TabIndex = 0;
            this.Materials.Text = "Материалы";
            this.Materials.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(226)))), ((int)(((byte)(209)))));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(778, 375);
            this.dataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Какой-то столбец";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Ещё какой-то столбец)";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            // 
            // Users
            // 
            this.Users.Controls.Add(this.usersTable);
            this.Users.Location = new System.Drawing.Point(4, 25);
            this.Users.Name = "Users";
            this.Users.Padding = new System.Windows.Forms.Padding(3);
            this.Users.Size = new System.Drawing.Size(784, 381);
            this.Users.TabIndex = 1;
            this.Users.Text = "Пользователи";
            this.Users.UseVisualStyleBackColor = true;
            // 
            // usersTable
            // 
            this.usersTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.usersTable.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(226)))), ((int)(((byte)(209)))));
            this.usersTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.usersTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Usernames,
            this.Passwords});
            this.usersTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usersTable.Location = new System.Drawing.Point(3, 3);
            this.usersTable.Name = "usersTable";
            this.usersTable.RowHeadersWidth = 51;
            this.usersTable.RowTemplate.Height = 24;
            this.usersTable.Size = new System.Drawing.Size(778, 375);
            this.usersTable.TabIndex = 1;
            // 
            // Usernames
            // 
            this.Usernames.HeaderText = "Имена пользователей";
            this.Usernames.MinimumWidth = 6;
            this.Usernames.Name = "Usernames";
            // 
            // Passwords
            // 
            this.Passwords.HeaderText = "Пароли";
            this.Passwords.MinimumWidth = 6;
            this.Passwords.Name = "Passwords";
            // 
            // Settings
            // 
            this.Settings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.Settings.Controls.Add(this.flowLayoutPanel1);
            this.Settings.Controls.Add(this.flowLayoutPanel8);
            this.Settings.Location = new System.Drawing.Point(4, 25);
            this.Settings.Name = "Settings";
            this.Settings.Size = new System.Drawing.Size(784, 381);
            this.Settings.TabIndex = 2;
            this.Settings.Text = "Настройки";
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.AutoSize = true;
            this.flowLayoutPanel8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel8.Controls.Add(this.specificHeatCapacityLabel);
            this.flowLayoutPanel8.Controls.Add(this.tryQuantityTextBox);
            this.flowLayoutPanel8.Location = new System.Drawing.Point(14, 12);
            this.flowLayoutPanel8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(333, 26);
            this.flowLayoutPanel8.TabIndex = 2;
            // 
            // specificHeatCapacityLabel
            // 
            this.specificHeatCapacityLabel.Location = new System.Drawing.Point(3, 0);
            this.specificHeatCapacityLabel.Name = "specificHeatCapacityLabel";
            this.specificHeatCapacityLabel.Size = new System.Drawing.Size(247, 25);
            this.specificHeatCapacityLabel.TabIndex = 0;
            this.specificHeatCapacityLabel.Text = "Количество попыток авторизации";
            this.specificHeatCapacityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tryQuantityTextBox
            // 
            this.tryQuantityTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tryQuantityTextBox.Location = new System.Drawing.Point(256, 2);
            this.tryQuantityTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tryQuantityTextBox.Name = "tryQuantityTextBox";
            this.tryQuantityTextBox.Size = new System.Drawing.Size(74, 22);
            this.tryQuantityTextBox.TabIndex = 104;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.timeoutHoursTextBox);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.timeoutMinutesTextBox);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.timeoutSecondsTextBox);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(14, 42);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(558, 26);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Тайм-аут авторизации";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timeoutHoursTextBox
            // 
            this.timeoutHoursTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeoutHoursTextBox.Location = new System.Drawing.Point(183, 2);
            this.timeoutHoursTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.timeoutHoursTextBox.Name = "timeoutHoursTextBox";
            this.timeoutHoursTextBox.Size = new System.Drawing.Size(57, 22);
            this.timeoutHoursTextBox.TabIndex = 104;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(246, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 25);
            this.label2.TabIndex = 105;
            this.label2.Text = "часов";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timeoutMinutesTextBox
            // 
            this.timeoutMinutesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeoutMinutesTextBox.Location = new System.Drawing.Point(306, 2);
            this.timeoutMinutesTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.timeoutMinutesTextBox.Name = "timeoutMinutesTextBox";
            this.timeoutMinutesTextBox.Size = new System.Drawing.Size(57, 22);
            this.timeoutMinutesTextBox.TabIndex = 106;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(369, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 25);
            this.label3.TabIndex = 107;
            this.label3.Text = "минут";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timeoutSecondsTextBox
            // 
            this.timeoutSecondsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeoutSecondsTextBox.Location = new System.Drawing.Point(429, 2);
            this.timeoutSecondsTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.timeoutSecondsTextBox.Name = "timeoutSecondsTextBox";
            this.timeoutSecondsTextBox.Size = new System.Drawing.Size(57, 22);
            this.timeoutSecondsTextBox.TabIndex = 108;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(492, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 25);
            this.label4.TabIndex = 109;
            this.label4.Text = "секунд";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AdminInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AdminInterface";
            this.Text = "AdminInterface";
            this.tabControl1.ResumeLayout(false);
            this.Materials.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.Users.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.usersTable)).EndInit();
            this.Settings.ResumeLayout(false);
            this.Settings.PerformLayout();
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel8.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Materials;
        private System.Windows.Forms.TabPage Users;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridView usersTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn Usernames;
        private System.Windows.Forms.DataGridViewTextBoxColumn Passwords;
        private System.Windows.Forms.TabPage Settings;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel8;
        private System.Windows.Forms.Label specificHeatCapacityLabel;
        private System.Windows.Forms.TextBox tryQuantityTextBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox timeoutHoursTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox timeoutMinutesTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox timeoutSecondsTextBox;
        private System.Windows.Forms.Label label4;
    }
}