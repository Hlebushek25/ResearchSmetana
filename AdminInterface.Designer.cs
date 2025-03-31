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
            this.Users = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usersTable = new System.Windows.Forms.DataGridView();
            this.Usernames = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Passwords = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.Materials.SuspendLayout();
            this.Users.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersTable)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Materials);
            this.tabControl1.Controls.Add(this.Users);
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
            this.Users.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersTable)).EndInit();
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
    }
}