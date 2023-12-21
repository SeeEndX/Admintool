using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Reflection;
using System.IO;

namespace admintool
{
    public partial class ProgForm : Form
    {
        private string cs = @"URI=file:C:\\Users\\ars_1\\Documents\\dbForAdminProg\\AdminToolDB.db";
        SQLiteConnection con;
        SQLiteCommand cmd;

        private List<ActionItem> actionList;
        private string currentUser;

        private Dictionary<string, Action> functionDictionary = new Dictionary<string, Action>();
        FunctionExecutor executor;
        public ProgForm(string user)
        {
            InitializeComponent();
            lbHello.Text += ", "+user+"!";
            currentUser = user;
            showFunctions();
            executor = new FunctionExecutor(functionDictionary);
        }

        private void showFunctions()
        {
            actionList = GetFunctionsForUser(currentUser);

            foreach (var action in actionList)
            {
                funcLB.Items.Add(action.Name);
            }
        }

        private List<ActionItem> GetFunctionsForUser(string user)
        {
            List<ActionItem> functions = new List<ActionItem>();

            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                string query = @"
            SELECT f.name
            FROM Function f
            JOIN Function_users fu ON f.id = fu.function
            JOIN Users u ON u.id = fu.user
            WHERE u.login = @User;";

                using (cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@User", user);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string functionNameFromDb = reader.GetString(0);
                            Action action = () => executor.ExecuteMethodByName(functionNameFromDb);
                            functions.Add(new ActionItem(functionNameFromDb, action));
                        }
                    }
                }
            }

            return functions;
        }

        private int AddReport(string time)
        {
            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                string insertReportQuery = "INSERT INTO Reports (time) VALUES (@Time);";
                using (SQLiteCommand insertReportCmd = new SQLiteCommand(insertReportQuery, con))
                {
                    insertReportCmd.Parameters.AddWithValue("@Time", time);
                    insertReportCmd.ExecuteNonQuery();
                }

                string selectLastIdQuery = "SELECT last_insert_rowid();";
                using (SQLiteCommand selectLastIdCmd = new SQLiteCommand(selectLastIdQuery, con))
                {
                    object result = selectLastIdCmd.ExecuteScalar();
                    int lastInsertedId = Convert.ToInt32(result);
                    return lastInsertedId;
                }
            }
        }

        public void LinkReportWithFunction(int reportId, int userId, string functionName)
        {
            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                string subquery = @"
            SELECT Function_users.id
            FROM Function_users
            JOIN Users ON Users.id = Function_users.user
            JOIN Function ON Function.id = Function_users.function
            WHERE Users.id = @UserId AND Function.name = @FunctionName;";

                using (SQLiteCommand subqueryCmd = new SQLiteCommand(subquery, con))
                {
                    subqueryCmd.Parameters.AddWithValue("@UserId", userId);
                    subqueryCmd.Parameters.AddWithValue("@FunctionName", functionName);

                    object result = subqueryCmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        int functionUserId = Convert.ToInt32(result);

                        string query = "INSERT INTO Reports_functions (report, function_user) VALUES (@ReportId, @FunctionUserId);";

                        using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@ReportId", reportId);
                            cmd.Parameters.AddWithValue("@FunctionUserId", functionUserId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Нет подключения к БД", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAct_Click(object sender, EventArgs e)
        {
            int selectedIndex = funcLB.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < actionList.Count)
            {
                executor.ExecuteMethodByName(actionList[selectedIndex].Name);
                string currentTime = DateTime.Now.ToString();

                int reportId = AddReport(currentTime);
                int userId = GetUserIdByUsername(currentUser);
                int functionUserId = GetFunctionUserId(userId, actionList[selectedIndex].Name);
                LinkReportWithFunction(reportId, functionUserId, actionList[selectedIndex].Name);
            }
            else
            {
                MessageBox.Show("Выберите действие из списка.");
            }
        }

        private int GetUserIdByUsername(string username)
        {
            int userId = -1;

            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                string query = "SELECT id FROM Users WHERE login = @Username;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out userId))
                    {
                        return userId;
                    }
                }
            }

            return userId;
        }

        private int GetFunctionUserId(int userId, string functionName)
        {
            int functionUserId = 0;

            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                string query = @"
            SELECT user
            FROM Function_users
            WHERE user = @UserId AND function = (SELECT id FROM Function WHERE name = @FunctionName);";

                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@FunctionName", functionName);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        functionUserId = Convert.ToInt32(result);
                    }
                }
            }

            return functionUserId;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            AuthForm auth = new AuthForm();
            auth.Tag = this;
            auth.Show(this);
            Hide();
        }

        private void ProgForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }

    public class ActionItem
    {
        public string Name { get; }
        public Action Action { get; }

        public ActionItem(string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }
}
