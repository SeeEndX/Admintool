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
    public partial class AddFunctionsForm : Form
    {
        private string cs = @"URI=file:C:\\Users\\ars_1\\Documents\\dbForAdminProg\\AdminToolDB.db";
        SQLiteConnection con;
        SQLiteCommand cmd;
        public event EventHandler FunctionsSaved;
        string user;

        public AddFunctionsForm(string username, List<string> assignedFunctionNames)
        {
            InitializeComponent();
            user = username;
            lbl1.Text += username;
        }

        public void SetAssignedFunctions(List<string> allFunctionNames, List<string> assignedFunctionNames)
        {
            functionList.Items.Clear();
            functionList.Items.AddRange(allFunctionNames.ToArray());
            for (int i = 0; i < functionList.Items.Count; i++)
            {
                string functionName = functionList.Items[i].ToString();
                bool isAssigned = assignedFunctionNames.Contains(functionName);
                functionList.SetItemChecked(i, isAssigned);
            }
        }

        private void LoadData(List<string> assignedFunctionNames)
        {
            foreach (string functionName in assignedFunctionNames)
            {
                int index = functionList.FindStringExact(functionName);
                if (index != -1)
                {
                    functionList.SetItemChecked(index, true);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int userId = GetSelectedUserId(user);

            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                string deleteQuery = "DELETE FROM Function_users WHERE user = @UserId;";
                using (SQLiteCommand deleteCmd = new SQLiteCommand(deleteQuery, con))
                {
                    deleteCmd.Parameters.AddWithValue("@UserId", userId);
                    deleteCmd.ExecuteNonQuery();
                }

                string insertQuery = "INSERT INTO Function_users (user, function) VALUES (@UserId, @FunctionId);";
                using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, con))
                {
                    insertCmd.Parameters.AddWithValue("@UserId", userId);

                    foreach (string functionName in functionList.CheckedItems)
                    {
                        int functionId = GetFunctionIdByName(functionName);

                        if (functionId > 0)
                        {
                            insertCmd.Parameters.Clear();
                            insertCmd.Parameters.AddWithValue("@UserId", userId);
                            insertCmd.Parameters.AddWithValue("@FunctionId", functionId);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            FunctionsSaved?.Invoke(this, EventArgs.Empty);
            MessageBox.Show("Данные сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int GetSelectedUserId(string username)
        {
            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                string query = "SELECT id FROM Users WHERE login = @Username;";
                using (cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    object result = cmd.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int userId))
                    {
                        return userId;
                    }

                    return 0;
                }
            }
        }

        private int GetFunctionIdByName(string functionName)
        {
            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                string query = "SELECT id FROM Function WHERE name = @FunctionName;";
                using (cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FunctionName", functionName);
                    object result = cmd.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int functionId))
                    {
                        return functionId;
                    }

                    return 0;
                }
            }
        }
    }
}
