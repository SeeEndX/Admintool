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

namespace admintool
{
    public partial class EditUserForm : Form
    {
        private string cs = @"URI=file:G:\\4kurs\\ПИС\\admintool\\AdminToolDB.db";
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
                            MessageBox.Show("Данные успешно обновлены.");
                            DataUpdated?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при обновлении данных.");
                        }
                    }
                }
                Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newUsername = tbLogin.Text;
            string newPassword = tbPass.Text;
            string newPasswordConf = tbPass2.Text;

            if (tbLogin.Text != "" && tbPass.Text != "" && tbPass2.Text != "")
            {
                if (tbPass.Text != tbPass2.Text)
                    MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (IsUserExists(newUsername))
                    MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else EditUser(newUsername,newPassword,newPasswordConf);
            }
            else MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
