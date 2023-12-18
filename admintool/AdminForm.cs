using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace admintool
{
    public partial class AdminForm : Form
    {
        private string cs = @"URI=file:G:\\4kurs\\ПИС\\admintool\\AdminToolDB.db";
        SQLiteConnection con;
        private string cmdText = @"
SELECT Users.id AS 'id', Users.login AS 'Пользователь', GROUP_CONCAT(Function.name, ', ') AS 'Доступные функции'
FROM Users
LEFT JOIN Function_users ON Users.id = Function_users.user
LEFT JOIN Function ON Function.id = Function_users.function
WHERE Users.usergroup = 'Dev'
GROUP BY Users.login;";

        public AdminForm()
        {
            InitializeComponent();
            getDataTable(cmdText);
        }

        private DataTable getDataTable(string cmdText)
        {
            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmdText, con))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvUsers.DataSource = dataTable;
                    dgvUsers.Columns["Доступные функции"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dgvUsers.Columns["id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    return dataTable;
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
            addUserForm.DataAdded += (sender, e) => 
                dgvUsers.DataSource = getDataTable(cmdText);
            addUserForm.Show(this);
            this.Enabled = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            dgvUsers.Refresh();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedCells.Count > 0)
            {
                int rowIndex = dgvUsers.SelectedCells[0].RowIndex;
                DataTable dataTable = getDataTable(cmdText);

                if (rowIndex >= 0 && rowIndex < dataTable.Rows.Count)
                {
                    int userId = Convert.ToInt32(dataTable.Rows[rowIndex]["id"]);

                    DeleteUserById(userId);

                    dataTable.Rows.RemoveAt(rowIndex);

                    dgvUsers.DataSource = null;
                    dgvUsers.DataSource = dataTable;
                }
            }
        }

        private void DeleteUserById(int userId)
        {
            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                string deleteQuery = "DELETE FROM Users WHERE id = @UserId";
                using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
