using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Collections;
using AdminService;

namespace admintool
{
    public partial class EditUserForm : Form
    {
        IAdminService serviceClient;

        public event EventHandler DataUpdated;
        private string originalUsername;

        public EditUserForm(string username, IAdminService serviceClient)
        {
            InitializeComponent();
            originalUsername = username;
            tbLogin.Text = username;
            this.serviceClient = serviceClient;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newUsername = tbLogin.Text;
            string newPassword = tbPass.Text;
            string newPasswordConf = tbPass2.Text;

            if (newPassword != newPasswordConf)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(newPassword) && string.IsNullOrEmpty(newPasswordConf))
            {
                newPassword = "Pa$$w0rd";
            }

            int rowsAffected = serviceClient.EditUser(originalUsername, newUsername, newPassword);
            if (rowsAffected > 0)
            {
                MessageBox.Show("Данные пользователя успешно обновлены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Не удалось обновить данные пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFunct_Click(object sender, EventArgs e)
        {
            showFunctionAdd();
        }

        private void showFunctionAdd()
        {
            AddFunctionsForm addFunctionsForm = new AddFunctionsForm(serviceClient ,originalUsername);
            addFunctionsForm.Tag = this;
            addFunctionsForm.FormClosed += (sender, e) => this.Enabled = true;
            addFunctionsForm.Show(this);
            this.Enabled = false;
        }

        private void EditUserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
