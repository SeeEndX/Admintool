using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Drawing;

namespace admintool
{
    public partial class AddUserForm : Form
    {
        private string path = "AdminToolDB.db";
        private string cs = @"URI=file:G:\\4kurs\\ПИС\\admintool\\AdminToolDB.db";
        SQLiteConnection con;
        SQLiteCommand cmd;

        public event EventHandler DataAdded;
        public AddUserForm()
        {
            con = new SQLiteConnection(cs);
            con.Open();
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbLogin.Text != "" && tbPass.Text != "" && tbPass2.Text != "")
            {
                if (IsUserExists(tbLogin.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (tbPass.Text != tbPass2.Text)
                {
                    MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                else if (AddUser(tbLogin.Text, tbPass.Text))
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

        protected virtual void OnDataAdded()
        {
            DataAdded?.Invoke(this, EventArgs.Empty);
        }

        private bool IsUserExists(string login)
        {
            string cmdText = "SELECT COUNT(*) FROM Users WHERE login = @Login";

            using (cmd = new SQLiteCommand(cmdText, con))
            {
                cmd.Parameters.AddWithValue("@Login", login);

                int userCount = Convert.ToInt32(cmd.ExecuteScalar());

                return userCount > 0;
            }
        }

        private bool AddUser(string login, string password)
        {
            string cmdText = "INSERT INTO Users (login, password, usergroup) VALUES (@Login, @Password, @Group)";

            using (cmd = new SQLiteCommand(cmdText, con))
            {
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Group", "Dev");

                int rowsAffected = cmd.ExecuteNonQuery();

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
    }
}
