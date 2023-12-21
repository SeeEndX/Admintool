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
            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                string query = @"
SELECT
    users.login AS UserName,
    function.name AS FunctionName,
    function.description AS FunctionDescription,
    reports.time AS ExecutionTime
FROM reports_functions
JOIN reports ON reports_functions.report = reports.id
JOIN function_users ON reports_functions.function_user = function_users.id
JOIN users ON function_users.user = users.id
JOIN function ON function_users.function = function.id
WHERE users.id = @User;";

                using (cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@User", user);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string userName = reader["UserName"].ToString();
                            string functionName = reader["FunctionName"].ToString();
                            string functionDescription = reader["FunctionDescription"].ToString();
                            string executionTime = reader["ExecutionTime"].ToString();
                            username = userName;
                            string reportEntry = $"Выполнена функция {functionName}\n ({functionDescription})\nв ({executionTime})\n\n";
                            rtbReports.Text += reportEntry;
                        }
                    }
                }
            }
            label1.Text += username;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
