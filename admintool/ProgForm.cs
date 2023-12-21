using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SQLite;

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
        Dictionary<string, TabPage> functionTabs;

        public ProgForm(string user)
        {
            InitializeComponent();
            lbHello.Text += ", "+user+"!";
            currentUser = user;
            InitData();
        }

        private void InitData()
        {
            showFunctions();
            InitializeTabs();
            executor = new FunctionExecutor(functionDictionary);
            dgvSites.Columns.Add("SiteName", "Название сайта");
            dgvSites.Columns["SiteName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSites.Columns["SiteName"].Frozen = true;
            dgvSites.Columns.Add("SiteState", "Состояние сайта");
            dgvSites.Columns["SiteState"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSites.Columns["SiteState"].Frozen = true;

            if (tabManageSite != null)
            {
                PopulateDataGridView();
            }
        }

        private void InitializeTabs()
        {
            functionTabs = new Dictionary<string, TabPage>
            {
                { "Конфигурация сервера", tabManageServer },
                { "Управление сайтами IIS", tabManageSite },
                { "Управление пулами", tabManagePool }
            };
            HideAllTabs();
            List<ActionItem> userFunctions = GetFunctionsForUser(currentUser);

            ShowTabs(userFunctions);

            if (tabsCtrl.TabPages.Count < 1)
            {
                tabsCtrl.Hide();
                label2.Hide();
                label3.Visible = true;
            }
        }

        private void PopulateDataGridView()
        {
            List<IISManager.SiteInfo> listOfSites = IISManager.GetListOfSites();

            dgvSites.Rows.Clear();

            foreach (IISManager.SiteInfo site in listOfSites)
            {
                dgvSites.Rows.Add(site.Name, site.State);
            }
        }

        private void ShowTabs(List<ActionItem> functionNames)
        {
            foreach (var functionName in functionNames)
            {
                if (functionTabs.ContainsKey(functionName.Name))
                {
                    TabPage tabPage = functionTabs[functionName.Name];

                    if (!tabsCtrl.TabPages.Contains(tabPage))
                    {
                        tabsCtrl.TabPages.Add(tabPage);
                    }
                }
            }
        }

        private void HideAllTabs()
        {
            foreach (TabPage tabPage in tabsCtrl.TabPages)
            {
                tabsCtrl.TabPages.Remove(tabPage);
            }
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

        private void btnAddSite_Click(object sender, EventArgs e)
        {
            ShowAddingIISWebSite();
            PopulateDataGridView();
        }

        private void ShowAddingIISWebSite()
        {
            AddIISWebSite addIISWebSite = new AddIISWebSite();
            addIISWebSite.Tag = this;
            addIISWebSite.FormClosed += (sender, e) => this.Enabled = true;
            addIISWebSite.Show(this);
            this.Enabled = false;
        }

        private void btnEditSite_Click(object sender, EventArgs e)
        {
            if (dgvSites.SelectedCells.Count > 0)
            {
                int rowIndex = dgvSites.SelectedCells[0].RowIndex;
                string currentName = dgvSites.Rows[rowIndex].Cells["SiteName"].Value.ToString();
                ShowEditingIISWebSite(currentName);
                PopulateDataGridView();
            }
            else
            {
                MessageBox.Show("Выберите сайт для редактирования.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowEditingIISWebSite(string currentName)
        {
            EditIISWebSite editIISWebSite = new EditIISWebSite(currentName);
            editIISWebSite.Tag = this;
            editIISWebSite.FormClosed += (sender, e) => this.Enabled = true;
            editIISWebSite.Show(this);
            this.Enabled = false;
        }

        private void btnDeleteSite_Click(object sender, EventArgs e)
        {
            if (dgvSites.SelectedCells.Count > 0)
            {
                int rowIndex = dgvSites.SelectedCells[0].RowIndex;
                string selectedSiteName = dgvSites.Rows[rowIndex].Cells["SiteName"].Value.ToString();

                DialogResult result = MessageBox.Show($"Вы уверены, что хотите удалить сайт '{selectedSiteName}'?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    IISManager.DeleteWebsite(selectedSiteName);
                    PopulateDataGridView();
                }
            }
            else
            {
                MessageBox.Show("Выберите сайт для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (dgvSites.SelectedCells.Count > 0)
            {
                int rowIndex = dgvSites.SelectedCells[0].RowIndex;
                string selectedSiteName = dgvSites.Rows[rowIndex].Cells["SiteName"].Value.ToString();
                string selectedSiteState = dgvSites.Rows[rowIndex].Cells["SiteState"].Value.ToString();

                if (selectedSiteState == "Stopped")
                {
                    IISManager.StartSite(selectedSiteName);
                    PopulateDataGridView();
                }
                else
                {
                    MessageBox.Show("Сайт уже запущен или в процессе запуска.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Выберите сайт для запуска.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (dgvSites.SelectedCells.Count > 0)
            {
                int rowIndex = dgvSites.SelectedCells[0].RowIndex;
                string selectedSiteName = dgvSites.Rows[rowIndex].Cells["SiteName"].Value.ToString();
                string selectedSiteState = dgvSites.Rows[rowIndex].Cells["SiteState"].Value.ToString();

                if (selectedSiteState == "Started")
                {
                    IISManager.StopSite(selectedSiteName);
                    PopulateDataGridView();
                }
                else
                {
                    MessageBox.Show("Сайт уже остановлен или в процессе остановки.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Выберите ячейку с сайтом для остановки.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
