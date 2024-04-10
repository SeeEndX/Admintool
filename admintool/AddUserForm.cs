using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Collections.Generic;
using AdminService;
using System.Drawing;

namespace admintool
{
    public partial class AddUserForm : Form
    {
        IAdminService serviceClient;
        private string user;

        public event EventHandler DataAdded;

        public event EventHandler FunctButtonClicked;
        private bool isFunctClicked;
        public AddUserForm(IAdminService serviceClient)
        {
            InitializeComponent();
            this.serviceClient = serviceClient;
            FunctButtonClicked += (sender, e) => isFunctClicked = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newUsername = tbLogin.Text;
            string newPassword = tbPass.Text;
            string newPasswordConf = tbPass2.Text;

            if (newUsername != "" && newPassword != "" && newPasswordConf != "")
            {
                if (!serviceClient.IsUserExists(newUsername) && newPassword != newPasswordConf)
                {
                    MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (!serviceClient.IsUserExists(newUsername) && AddUser(newUsername, newPassword))
                {
                    MessageBox.Show("Пользователь успешно добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void OnDataAdded()
        {
            DataAdded?.Invoke(this, EventArgs.Empty);
        }

        private bool AddUser(string login, string password)
        {
            int rowsAffected = serviceClient.AddUser(login, password);
            return rowsAffected > 0;
        }

        private void ClearFields()
        {
            tbLogin.Text = "";
            tbPass.Text = "";
            tbPass2.Text = "";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnFunct_Click(object sender, EventArgs e)
        {
            FunctButtonClicked?.Invoke(this, EventArgs.Empty);
            if (tbLogin.Text == "" || tbPass.Text == "" || tbPass2.Text == "")
            {
                MessageBox.Show("Сначала введите учетные данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (serviceClient.IsUserExists(tbLogin.Text))
            {
                user = tbLogin.Text;
                showFunctionAdd(user);
            }
            else
            {
                user = tbLogin.Text;
                AddUser(tbLogin.Text, tbPass.Text);
                showFunctionAdd(user);
            }
        }

        private void showFunctionAdd(string user)
        {
            List<string> allFunctions = serviceClient.GetFunctions();
            AddFunctionsForm addFunctionsForm = new AddFunctionsForm(serviceClient, user);
            addFunctionsForm.Tag = this;

            addFunctionsForm.FormClosed += (sender, e) => this.Enabled = true;
            addFunctionsForm.Show(this);
            this.Enabled = false;
        }
    }
}
