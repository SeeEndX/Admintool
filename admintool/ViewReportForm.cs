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
        private List<string> reports;

        public ViewReportForm(int userId)
        {
            InitializeComponent();
            user = userId;
        }

        private void GetReport()
        {
            lbActions.Items.Clear();

            reports = GetReportsForUser(user);

            foreach (var report in reports)
            {
                lbActions.Items.Add(report);
            }
        }

        private List<string> GetReportsForUser(int user)
        {
            List<string> functions = new List<string>();

            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                //переписать
                string query = @"
            SELECT f.name
            FROM Function f
            JOIN Function_users fu ON f.id = fu.function
            JOIN Users u ON u.id = fu.user
            WHERE u.login = @User;";

                using (cmd = new SQLiteCommand(query, con))
                {
                    //переписать
                    cmd.Parameters.AddWithValue("@User", user);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //переписать
                            string reports = reader.GetString(0);
                            //переписать
                            functions.Add(reports);
                        }
                    }
                }
            }

            return functions;
        }
    }
}
