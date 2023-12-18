using System;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.SQLite;

namespace admintool
{
    public partial class AuthForm : Form
    {
        private string path = "AdminToolDB.db";
        private string cs = @"URI=file:G:\\4kurs\\ПИС\\admintool\\AdminToolDB.db";
        SQLiteConnection con;
        SQLiteCommand cmd;

        public AuthForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Auth();
        }

        private void Auth()
        {

            string login = tbLogin.Text;
            string password = tbPass.Text;

            con = new SQLiteConnection(cs);
            con.Open();

            var group = SqlQuery("SELECT [group] FROM Users WHERE login = " +
                "@Login AND password = @Password", con, login, password);
            if (group != null)
            {
                if (group == "Admin")
                {
                    AdminForm adminForm = new AdminForm();
                    adminForm.Tag = this;
                    adminForm.Show(this);
                    Hide();
                }
                else if (group == "Dev")
                {
                    var user = SqlQuery("SELECT [login] FROM Users WHERE login = " +
                "@Login AND password = @Password", con, login, password);
                    
                    ProgForm progForm = new ProgForm(user);
                    progForm.Tag = this;
                    progForm.Show(this);
                    Hide();
                }
            }
            else
            {
                MessageBox.Show("Неверные данные!");
            }
        }

        private string SqlQuery(string cmdText, SQLiteConnection con, string login, string password)
        {
            cmd = new SQLiteCommand(cmdText, con);
            cmd.Parameters.AddWithValue("@Login", login);
            cmd.Parameters.AddWithValue("@Password", password);
            object resultObj = cmd.ExecuteScalar();
            string result = (resultObj != null) ? resultObj.ToString() : null;
            return result;
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
