using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Collections.Generic;

namespace admintool
{
    public partial class AddUserForm : Form
    {
        private string cs = @"URI=file:C:\\Users\\ars_1\\Documents\\dbForAdminProg\\AdminToolDB.db";
        SQLiteConnection con;
        SQLiteCommand cmd;
        private string user;

        public event EventHandler DataAdded;

        public event EventHandler FunctButtonClicked;
        private bool isFunctClicked = false;
        public AddUserForm()
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
                MessageBox.Show("Пользователь был успешно добавлен при назначении функций.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (newUsername != "" && newPasswordConf != "")
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

                    int userId = GetSelectedUserId(newUsername);

                    using (con = new SQLiteConnection(cs))
                    {
                        con.Open();

                        if (!string.IsNullOrEmpty(newPassword))
                        {
                            if (userId > 0)
                            {
                                string updatePasswordQuery = "UPDATE Users SET password = @Password WHERE id = @UserId;";
                                using (SQLiteCommand updatePasswordCmd = new SQLiteCommand(updatePasswordQuery, con))
                                {
                                    updatePasswordCmd.Parameters.AddWithValue("@UserId", userId);
                                    updatePasswordCmd.Parameters.AddWithValue("@Password", newPassword);
                                    updatePasswordCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                if (AddUser(newUsername, newPassword))
                                {
                                    MessageBox.Show("Пользователь успешно добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearFields();
                                    OnDataAdded();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private int GetSelectedUserId(string username)
        {
            int userId = -1;

            string query = "SELECT id FROM Users WHERE login = @username";

            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, con))
                {
                    command.Parameters.Add(new SQLiteParameter("@username", username));

                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out userId))
                    {
                        return userId;
                    }
                }
                con.Close();
            }

            return userId;
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
            string cmdText = "INSERT INTO Users (login, password, usergroup) VALUES (@Login, @Password, @Group)";
            con = new SQLiteConnection(cs);
            con.Open();
            using (cmd = new SQLiteCommand(cmdText, con))
            {
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Group", "Dev");

                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                return rowsAffected > 0;
            }
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
