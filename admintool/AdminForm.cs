using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace admintool
{
    public partial class AdminForm : Form
    {
        private string cs = @"URI=file:C:\\Users\\ars_1\\Documents\\dbForAdminProg\\AdminToolDB.db";
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
                    dgvUsers.Columns["id"].Frozen = true;
                    dgvUsers.Columns["Доступные функции"].Frozen = true;
                    dgvUsers.Columns["Пользователь"].Frozen = true;

                    con.Close();
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
            OpenEditUserForm();
        }

        private void OpenEditUserForm()
        {
            if (dgvUsers.SelectedCells.Count > 0)
            {
                int rowIndex = dgvUsers.SelectedCells[0].RowIndex;
                string selectedUsername = dgvUsers.Rows[rowIndex].Cells["Пользователь"].Value.ToString();

                EditUserForm editUserForm = new EditUserForm(selectedUsername);
                editUserForm.Tag = this;
                editUserForm.FormClosed += (sender, e) => this.Enabled = true;
                editUserForm.DataUpdated += (sender, e) => dgvUsers.DataSource = getDataTable(cmdText);
                editUserForm.Show(this);
                this.Enabled = false;
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.");
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            bool containsCheckboxCell = false;

            DialogResult result = MessageBox.Show("Вы уверены, " +
                "что хотите удалить выбранных пользователей?", 
                "Подтверждение удаления", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (DataGridViewCell cell in dgvUsers.SelectedCells)
                {
                    if (cell.OwningColumn.Name == "chbChoice")
                    {
                        containsCheckboxCell = true;
                        break;
                    }
                }

                if (containsCheckboxCell) DeleteSelectedRows();
                else DeleteSelectedCells();
            }
        }

        private void DeleteSelectedRows()
        {
            for (int i = 0; i < dgvUsers.Rows.Count; i++)
            {
                DataGridViewRow row = dgvUsers.Rows[i];
                DataGridViewCheckBoxCell checkbox = row.Cells["chbChoice"] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(checkbox?.Value))
                {
                    int userId = Convert.ToInt32(row.Cells["id"].Value);

                    string deleteQuery = "DELETE FROM Users WHERE id = @UserId";
                    using (con = new SQLiteConnection(cs))
                    {
                        con.Open();

                        using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    dgvUsers.Rows.RemoveAt(i);
                    i--;
                }
            }
        }

        private void DeleteSelectedCells()
        {
            if (dgvUsers.SelectedCells.Count > 0)
            {
                int rowIndex = dgvUsers.SelectedCells[0].RowIndex;

                if (rowIndex >= 0 && rowIndex < dgvUsers.Rows.Count)
                {
                    int userId = Convert.ToInt32(dgvUsers.Rows[rowIndex].Cells["id"].Value);

                    string deleteQuery = "DELETE FROM Users WHERE id = @UserId";

                    using (SQLiteConnection con = new SQLiteConnection(cs))
                    {
                        con.Open();

                        using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    dgvUsers.Rows.RemoveAt(rowIndex);
                }
            }
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvUsers.Columns["chbChoice"].Index && e.RowIndex >= 0)
            {
                DataGridViewCheckBoxCell checkboxCell = dgvUsers.Rows[e.RowIndex].Cells["chbChoice"] as DataGridViewCheckBoxCell;

                if (checkboxCell != null)
                {
                    checkboxCell.Value = checkboxCell.Value == null || !(bool)checkboxCell.Value;
                    dgvUsers.EndEdit();
                }

            }
        }
    }
}
