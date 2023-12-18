﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace admintool
{
    public partial class AdminForm : Form
    {
        private string path = "AdminToolDB.db";
        private string cs = @"URI=file:G:\\4kurs\\ПИС\\admintool\\AdminToolDB.db";
        SQLiteConnection con;
        SQLiteCommand cmd;

        public AdminForm()
        {
            InitializeComponent();
            FillUsers();
        }

        private void FillUsers()
        {
            string cmdText = @"
SELECT Users.login, GROUP_CONCAT(Function.name, ', ') AS 'Доступные функции'
FROM Function_users
JOIN Users ON Users.id = Function_users.user
JOIN Function ON Function.id = Function_users.function
GROUP BY Users.login;";

            SqlQuery(cmdText);
            dgvUsers.Columns["Доступные функции"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void dgwUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvUsers.Columns["Functions"].Index && e.RowIndex >= 0)
            {
                User selectedUser = (User)dgvUsers.Rows[e.RowIndex].DataBoundItem;

                dgvUsers.Refresh();
            }
        }

        private void SqlQuery(string cmdText)
        {
            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmdText, con))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvUsers.DataSource = dataTable;
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            AuthForm auth = new AuthForm();
            auth.Tag = this;
            auth.Show(this);
            Hide();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenAddUserForm();
        }

        private void OpenAddUserForm()
        {
            AddUserForm addUserForm = new AddUserForm();
            addUserForm.Tag = this;
            addUserForm.FormClosed += (sender, e) => this.Enabled = true;
            addUserForm.Show(this);
            this.Enabled = false;
        }

    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Functions { get; set; }
    }

    public partial class EditFunctionsForm : Form
    {
        private User user;

        /*public EditFunctionsForm(User selectedUser)
        {
            InitializeComponent();

            user = selectedUser;

            // Отображение функций текущего пользователя в TextBox
            textBoxFunctions.Text = string.Join(", ", user.Functions);
        }*/
/*private void btnUpdateFunctions_Click(object sender, EventArgs e)
        {
            // Обновление списка функций пользователя на основе данных из TextBox
            user.Functions = new List<string>(textBoxFunctions.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            // Закрытие формы редактирования
            Close();
        }*/
        
    }
}
