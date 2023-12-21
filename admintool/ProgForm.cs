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
            InitializeTabs();
            executor = new FunctionExecutor(functionDictionary);
            dgvSitesInit();
            dgvPoolInit();
            if (tabManageSite != null)
            {
                UpdateSitesDataGridView();
            }
            if (tabManagePool != null)
            {
                UpdatePoolDataGridView();
            }
        }

        private void dgvSitesInit()
        {
            dgvSites.Columns.Add("SiteName", "Название сайта");
            dgvSites.Columns["SiteName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSites.Columns["SiteName"].Frozen = true;
            dgvSites.Columns.Add("SiteState", "Состояние сайта");
            dgvSites.Columns["SiteState"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSites.Columns["SiteState"].Frozen = true;
            dgvSites.Columns.Add("SiteBinding", "Привязки сайта");
            dgvSites.Columns["SiteBinding"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSites.Columns["SiteBinding"].Frozen = true;
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

        private void UpdateSitesDataGridView()
        {
            List<IISManager.SiteInfo> listOfSites = IISManager.GetListOfSites();

            dgvSites.Rows.Clear();

            foreach (IISManager.SiteInfo site in listOfSites)
            {
                dgvSites.Rows.Add(site.Name, site.State, site.Bindings);
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

        private void AddReport(string description)
        {
            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                string insertReportQuery = "INSERT INTO Reports (user, description, time) VALUES (@UserId, @Description, @Time);";
                using (SQLiteCommand insertReportCmd = new SQLiteCommand(insertReportQuery, con))
                {
                    insertReportCmd.Parameters.AddWithValue("@UserId", GetUserIdByUsername(currentUser));
                    insertReportCmd.Parameters.AddWithValue("@Description", description);
                    insertReportCmd.Parameters.AddWithValue("@Time", DateTime.Now.ToString());

                    insertReportCmd.ExecuteNonQuery();
                }
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
        }

        private void ShowAddingIISWebSite()
        {
            AddIISWebSite addIISWebSite = new AddIISWebSite();
            addIISWebSite.Tag = this;
            addIISWebSite.FormClosed += (sender, e) => {
                this.Enabled = true;
                UpdateSitesDataGridView();
                AddReport("Добавлен сайт на веб-сервер Microsoft IIS Server"); //eventHandler на будущее
            };
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
            editIISWebSite.FormClosed += (sender, e) =>
            {
                this.Enabled = true;
                UpdateSitesDataGridView();
                AddReport($"Изменен сайт {currentName} на веб-сервере Microsoft IIS"); //eventHandler на будущее
            };
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
                    UpdateSitesDataGridView();
                    AddReport($"С веб-сервера Microsoft IIS был удален сайт {selectedSiteName}");
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
                    UpdateSitesDataGridView();
                    AddReport($"На веб-сервере Microsoft IIS был запущен сайт {selectedSiteName}");
                }
                else
                {
                    MessageBox.Show("Сайт уже запущен или в процессе запуска.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AddReport($"На веб-сервере Microsoft IIS была осуществленна попытка запустить сайт {selectedSiteName}, который уже запущен");
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
                    UpdateSitesDataGridView();
                    AddReport($"На веб-сервере Microsoft IIS был остановлен сайт {selectedSiteName}");
                }
                else
                {
                    MessageBox.Show("Сайт уже остановлен или в процессе остановки.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AddReport($"На веб-сервере Microsoft IIS была осуществленна попытка остановить сайт {selectedSiteName}, который уже остановлен");
                }
            }
            else
            {
                MessageBox.Show("Выберите сайт для остановки.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAddPool_Click(object sender, EventArgs e)
        {
            ShowAddingPool();
        }

        private void dgvPoolInit()
        {
            dgvPool.Columns.Add("PoolName", "Название пула");
            dgvPool.Columns["PoolName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPool.Columns["PoolName"].Frozen = true;
            dgvPool.Columns.Add("PoolState", "Состояние пула");
            dgvPool.Columns["PoolState"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPool.Columns["PoolState"].Frozen = true;
            dgvPool.Columns.Add("NetVersion", "Версия CLR");
            dgvPool.Columns["NetVersion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPool.Columns["NetVersion"].Frozen = true;
            dgvPool.Columns.Add("ManagedPipelineMode", "Режим конвейера");
            dgvPool.Columns["ManagedPipelineMode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvPool.Columns["ManagedPipelineMode"].Frozen = true;
        }

        private void UpdatePoolDataGridView()
        {
            List<IISManager.AppPoolInfo> listOfAppPools = IISManager.GetListOfAppPools();

            dgvPool.Rows.Clear();

            foreach (IISManager.AppPoolInfo appPool in listOfAppPools)
            {
                dgvPool.Rows.Add(appPool.Name, appPool.State, appPool.NETCLRVersion, appPool.ManagedPipelineMode);
            }
        }

        private void ShowAddingPool()
        {
            AddIISPool addIISPool = new AddIISPool();
            addIISPool.Tag = this;
            addIISPool.FormClosed += (sender, e) =>
            {
                this.Enabled = true;
                UpdatePoolDataGridView();
                AddReport($"На веб-сервере Microsoft IIS был добавлен пул"); //eventHandler на будущее
            };
            addIISPool.Show(this);
            this.Enabled = false;
        }

        private void btnDeletePool_Click(object sender, EventArgs e)
        {
            if (dgvPool.SelectedCells.Count > 0)
            {
                int rowIndex = dgvPool.SelectedCells[0].RowIndex;
                string selectedPoolName = dgvPool.Rows[rowIndex].Cells["PoolName"].Value.ToString();

                DialogResult result = MessageBox.Show($"Вы уверены, что хотите удалить пул приложений '{selectedPoolName}'?",
                                                      "Подтверждение удаления",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    IISManager.DeletePool(selectedPoolName);
                    UpdatePoolDataGridView();
                    AddReport($"С веб-сервера Microsoft IIS был удален пул приложений {selectedPoolName}");
                }
            }
            else
            {
                MessageBox.Show("Выберите пул для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowEditingPool(string currentName)
        {
            EditIISPool editIISPool = new EditIISPool(currentName);
            editIISPool.Tag = this;
            editIISPool.FormClosed += (sender, e) =>
            {
                this.Enabled = true;
                UpdatePoolDataGridView();
                AddReport($"Изменен пул приложений {currentName} на веб-сервере Microsoft IIS"); //eventHandler на будущее
            };
            editIISPool.Show(this);
            this.Enabled = false;
        }

        private void btnEditPool_Click(object sender, EventArgs e)
        {
            if (dgvPool.SelectedCells.Count > 0)
            {
                int rowIndex = dgvPool.SelectedCells[0].RowIndex;
                string currentName = dgvPool.Rows[rowIndex].Cells["PoolName"].Value.ToString();
                ShowEditingPool(currentName);
            }
            else
            {
                MessageBox.Show("Выберите пул для редактирования.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnStartPool_Click(object sender, EventArgs e)
        {
            if (dgvPool.SelectedCells.Count > 0)
            {
                int rowIndex = dgvPool.SelectedCells[0].RowIndex;
                string selectedPoolName = dgvPool.Rows[rowIndex].Cells["PoolName"].Value.ToString();
                string selectedPoolState = dgvPool.Rows[rowIndex].Cells["PoolState"].Value.ToString();

                if (selectedPoolState == "Stopped")
                {
                    IISManager.StartAppPool(selectedPoolName);
                    UpdateSitesDataGridView();
                    AddReport($"На веб-сервере Microsoft IIS был запущен пул приложений {selectedPoolName}");
                }
                else
                {
                    MessageBox.Show("Пул уже запущен или в процессе запуска.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AddReport($"На веб-сервере Microsoft IIS была осуществленна попытка запустить пул приложений {selectedPoolName}, который уже запущен");
                }
            }
            else
            {
                MessageBox.Show("Выберите пул для запуска.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnStopPool_Click(object sender, EventArgs e)
        {
            if (dgvPool.SelectedCells.Count > 0)
            {
                int rowIndex = dgvPool.SelectedCells[0].RowIndex;
                string selectedPoolName = dgvPool.Rows[rowIndex].Cells["PoolName"].Value.ToString();
                string selectedPoolState = dgvPool.Rows[rowIndex].Cells["PoolState"].Value.ToString();

                if (selectedPoolState == "Started")
                {
                    IISManager.StopAppPool(selectedPoolName);
                    UpdatePoolDataGridView();
                    AddReport($"На веб-сервере Microsoft IIS был остановлен пул {selectedPoolName}");
                }
                else
                {
                    MessageBox.Show("Пул уже остановлен или в процессе остановки.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AddReport($"На веб-сервере Microsoft IIS была осуществленна попытка остановить пул {selectedPoolName}, который уже остановлен");
                }
            }
            else
            {
                MessageBox.Show("Выберите пул для остановки.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
