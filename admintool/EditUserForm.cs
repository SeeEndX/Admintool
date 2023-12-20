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

namespace admintool
{
    public partial class EditUserForm : Form
    {
        private string cs = @"URI=file:C:\\Users\\ars_1\\Documents\\dbForAdminProg\\AdminToolDB.db";
        SQLiteConnection con;
        SQLiteCommand cmd;

        public event EventHandler DataUpdated;
        private string originalUsername;

        public EditUserForm(string username)
        {
            InitializeComponent();
            originalUsername = username;
            tbLogin.Text = username;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool IsUserExists(string login)
        {
            string cmdText = "SELECT COUNT(*) FROM Users WHERE login = @Login";
            con = new SQLiteConnection(cs);
            using (cmd = new SQLiteCommand(cmdText, con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@Login", login);
                int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                return userCount > 0;
            }
        }

        private void EditUser(string newUsername,string newPassword, string newPasswordConf)
        {
            DialogResult result = MessageBox.Show("Сохранить, " +
                "новые данные пользователя?",
                "Подтверждение изменения", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (con = new SQLiteConnection(cs))
                {
                    con.Open();

                    string updateQuery = @"
            UPDATE Users
            SET login = @NewUsername, password = @NewPassword
            WHERE login = @OriginalUsername;";

                    using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@NewUsername", newUsername);
                        cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                        cmd.Parameters.AddWithValue("@OriginalUsername", originalUsername);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            DataUpdated?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при обновлении данных со стороны БД.");
                        }
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newUsername = tbLogin.Text;
            string newPassword = tbPass.Text;
            string newPasswordConf = tbPass2.Text;

            bool isUsernameChanged = newUsername != originalUsername;
            bool isPasswordChanged = !string.IsNullOrEmpty(newPassword) && newPassword == newPasswordConf;

            if (isUsernameChanged && isPasswordChanged)
            {
                EditUser(newUsername, newPassword, newPasswordConf);
            }
            else if (isUsernameChanged)
            {
                EditUser(newUsername, null, null);
                MessageBox.Show("Имя пользователя было изменено.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (isPasswordChanged)
            {
                EditUser(originalUsername, newPassword, newPasswordConf);
                MessageBox.Show("Пароль был изменен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (newPassword != newPasswordConf)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                DataUpdated?.Invoke(this, EventArgs.Empty);
                MessageBox.Show("Учетные данные не были изменены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnFunct_Click(object sender, EventArgs e)
        {
            showFunctionAdd();
        }

        private void showFunctionAdd()
        {
            int userId = GetSelectedUserId();
            List<string> assignedFunctionNames = GetFunctionsAssignedToUser(userId);
            
            AddFunctionsForm addFunctionsForm = new AddFunctionsForm(originalUsername,assignedFunctionNames);
            addFunctionsForm.Tag = this;
            addFunctionsForm.FormClosed += (sender, e) => this.Enabled = true;

            List<string> allFunctions = GetAllFunctions();

            addFunctionsForm.SetAssignedFunctions(allFunctions, assignedFunctionNames);


            addFunctionsForm.Show(this);
            this.Enabled = false;
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
            }

            return allFunctions;
        }

        private List<string> GetFunctionsAssignedToUser(int userId)
        {
            List<string> assignedFunctionNames = new List<string>();

            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                string query = @"
            SELECT f.id, f.name
            FROM Function f
            JOIN Function_users fu ON f.id = fu.function
            WHERE fu.user = @UserId;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string functionName = reader.GetString(1);
                            assignedFunctionNames.Add(functionName);
                        }
                    }
                }
            }

            return assignedFunctionNames;
        }

        private int GetSelectedUserId()
        {
            string username = originalUsername;
            string query = "SELECT id FROM Users WHERE login = @username";

            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, con))
                {
                    command.Parameters.AddWithValue("@username", username);

                    object result = command.ExecuteScalar();
                    int userId = Convert.ToInt32(result);
                    return userId;
                }
            }
        }
    }
}
