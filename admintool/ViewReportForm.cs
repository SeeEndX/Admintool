using System;
using System.Windows.Forms;
using System.Data.SQLite;

namespace admintool
{
    public partial class ViewReportForm : Form
    {
        private string cs = @"URI=file:C:\\Users\\ars_1\\Documents\\dbForAdminProg\\AdminToolDB.db";
        SQLiteConnection con;
        SQLiteCommand cmd;

        private int user;
        private string username;

        public ViewReportForm(int userId)
        {
            InitializeComponent();
            user = userId;
            GetReportsForUser(user);
        }

        private void GetReportsForUser(int user)
        {
            try
            {
                using (con = new SQLiteConnection(cs))
                {
                    con.Open();

                    string query = @"
                SELECT U.login, R.description, R.time
                FROM Reports R
                JOIN Users U ON R.user = U.id
                WHERE R.user = @UserId;";

                    using (cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", user);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string login = reader.GetString(0);
                                string description = reader.GetString(1);
                                string time = reader.GetString(2);
                                username = login;
                                string reportEntry = $"{description}\n в {time}\n\n";
                                rtbReports.Text += reportEntry;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении отчетов для пользователя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            label1.Text += username;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
