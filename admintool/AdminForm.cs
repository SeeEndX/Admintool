using AdminService;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace admintool
{
    public partial class AdminForm : Form
    {
        IAdminService serviceClient;

        public AdminForm(IAdminService serviceClient)
        {
            InitializeComponent();
            this.serviceClient = serviceClient;
            GetData();
        }

        private void GetData()
        {
            try
            {
                dgvUsers.DataSource = serviceClient.GetUsersData();
                dataGridSetup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridSetup()
        {
            dgvUsers.Columns["Пользователь"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvUsers.Columns["Доступные функции"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvUsers.Columns["id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvUsers.Columns["id"].Frozen = true;
            dgvUsers.Columns["Доступные функции"].Frozen = true;
            dgvUsers.Columns["Пользователь"].Frozen = true;
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
            AddUserForm addUserForm = new AddUserForm(serviceClient);
            addUserForm.Tag = this;
            addUserForm.FormClosed += AddUserForm_FormClosed;
            addUserForm.Show(this);

            this.Enabled = false;
        }

        private void AddUserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
            GetData();
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

                EditUserForm editUserForm = new EditUserForm(selectedUsername, serviceClient);
                editUserForm.Tag = this;
                editUserForm.FormClosed += EditUserForm_FormClosed;
                editUserForm.Show(this);
                this.Enabled = false;
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.");
            }
        }

        private void EditUserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
            GetData();
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

                    serviceClient.DeleteUser(userId);

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

                    serviceClient.DeleteUser(userId);

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

        private void AdminForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedCells.Count > 0)
            {
                int rowIndex = dgvUsers.SelectedCells[0].RowIndex;
                int userId = Convert.ToInt32(dgvUsers.Rows[rowIndex].Cells["id"].Value);

                GenerateReport(userId);
            }
        }

        private void GenerateReport(int userId)
        {
            ViewReportForm viewReportForm = new ViewReportForm(userId);
            viewReportForm.Tag = this;
            viewReportForm.FormClosed += (sender, e) => this.Enabled = true;
            viewReportForm.Show(this);
            this.Enabled = false;
        }
    }
}
