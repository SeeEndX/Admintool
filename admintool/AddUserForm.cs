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
        private bool isFunctClicked = false;
        public AddUserForm(IAdminService serviceClient)
        {
            InitializeComponent();
            FunctButtonClicked += (sender, e) => isFunctClicked = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newUsername = tbLogin.Text;
            string newPassword = tbPass.Text;
            string newPasswordConf = tbPass2.Text;

            if (isFunctClicked)
            {
                isFunctClicked = false;
                OnDataAdded();
                MessageBox.Show("Пользователь был успешно добавлен при назначении функций.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (newUsername != "" && newPassword != "" && newPasswordConf != "")
                {
                    if (IsUserExists(newUsername))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else if (newPassword != newPasswordConf)
                    {
                        MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int userId = serviceClient.GetSelectedUserId(newUsername);

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        if (userId > 0)
                        {
                            serviceClient.UpdatePass(userId, newPassword);
                            OnDataAdded();
                        }
                        else if (AddUser(newUsername, newPassword))
                        {
                            MessageBox.Show("Пользователь успешно добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            OnDataAdded();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void OnDataAdded()
        {
            DataAdded?.Invoke(this, EventArgs.Empty);
        }

        private bool IsUserExists(string login)
        {
            string cmdText = "SELECT COUNT(*) FROM Users WHERE login = @Login";
            con = new SQLiteConnection(cs);
            con.Open();
            using (cmd = new SQLiteCommand(cmdText, con))
            {
                cmd.Parameters.AddWithValue("@Login", login);
                int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                return userCount > 0;
            }
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
            else
            {
                user = tbLogin.Text;
                AddUser(tbLogin.Text, tbPass.Text);
                showFunctionAdd(user);
            }
        }

        private List<string> GetAllFunctions()
        {
            List<string> allFunctions = new List<string>();

            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                string query = "SELECT name FROM Function;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string functionName = reader.GetString(0);
                            allFunctions.Add(functionName);
                        }
                    }
                }
                con.Close();
            }

            return allFunctions;
        }

        private void showFunctionAdd(string user)
        {
            List<string> allFunctions = GetAllFunctions();
            List<string> assignedFunctions = null;
            AddFunctionsForm addFunctionsForm = new AddFunctionsForm(user);
            addFunctionsForm.Tag = this;

            addFunctionsForm.SetAssignedFunctions(allFunctions, assignedFunctions);

            addFunctionsForm.FormClosed += (sender, e) => this.Enabled = true;
            addFunctionsForm.Show(this);
            this.Enabled = false;
        }
    }
}
