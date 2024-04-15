using AdminService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace admintool
{
    public partial class ProgForm : Form
    {
        private string currentUser;

        private Dictionary<string, Action> functionDictionary = new Dictionary<string, Action>();
        Dictionary<string, TabPage> functionTabs;

        IAdminService adminService;
        IISManager iisManager = new IISManager();

        public ProgForm(IAdminService adminService, string user)
        {
            InitializeComponent();
            this.adminService = adminService;
            lbHello.Text += ", "+user+"!";
            currentUser = user;
            InitData();
        }

        private void InitData()
        {
            InitializeTabs();
            //FunctionExecutor executor = new FunctionExecutor(functionDictionary);
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
            List<IISManager.ActionItem> userFunctions = iisManager.GetFunctionsForUser(currentUser);

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
            List<IISManager.SiteInfo> listOfSites = adminService.GetListOfSites();

            if (listOfSites == null || listOfSites.Count == 0)
            {
                MessageBox.Show("Нет ничего");
            }
            else
            {
                dgvSites.Rows.Clear();

                foreach (IISManager.SiteInfo site in listOfSites)
                {
                    dgvSites.Rows.Add(site.Name, site.State, site.Bindings);
                }
            }
        }

        private void ShowTabs(List<IISManager.ActionItem> functionNames)
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
            AddIISWebSite addIISWebSite = new AddIISWebSite(adminService);
            addIISWebSite.Tag = this;
            addIISWebSite.FormClosed += (sender, e) => {
                this.Enabled = true;
                UpdateSitesDataGridView();
                adminService.AddReport(currentUser, "Добавлен сайт на веб-сервер Microsoft IIS Server");
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
            EditIISWebSite editIISWebSite = new EditIISWebSite(adminService,currentName);
            editIISWebSite.Tag = this;
            editIISWebSite.FormClosed += (sender, e) =>
            {
                this.Enabled = true;
                UpdateSitesDataGridView();
                adminService.AddReport(currentUser, $"Изменен сайт {currentName} на веб-сервере Microsoft IIS"); //eventHandler на будущее
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
                    adminService.DeleteWebsite(selectedSiteName);
                    UpdateSitesDataGridView();
                    adminService.AddReport(currentUser, $"С веб-сервера Microsoft IIS был удален сайт {selectedSiteName}");
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
                    adminService.StartSite(selectedSiteName);
                    UpdateSitesDataGridView();
                    adminService.AddReport(currentUser, $"На веб-сервере Microsoft IIS был запущен сайт {selectedSiteName}");
                }
                else
                {
                    MessageBox.Show("Сайт уже запущен или в процессе запуска.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    adminService.AddReport(currentUser, $"На веб-сервере Microsoft IIS была осуществленна попытка запустить сайт {selectedSiteName}, который уже запущен");
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
                    adminService.StopSite(selectedSiteName);
                    UpdateSitesDataGridView();
                    adminService.AddReport(currentUser, $"На веб-сервере Microsoft IIS был остановлен сайт {selectedSiteName}");
                }
                else
                {
                    MessageBox.Show("Сайт уже остановлен или в процессе остановки.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    adminService.AddReport(currentUser, $"На веб-сервере Microsoft IIS была осуществленна попытка остановить сайт {selectedSiteName}, который уже остановлен");
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
            List<IISManager.AppPoolInfo> listOfAppPools = adminService.GetListOfAppPools();

            dgvPool.Rows.Clear();

            foreach (IISManager.AppPoolInfo appPool in listOfAppPools)
            {
                dgvPool.Rows.Add(appPool.Name, appPool.State, appPool.NETCLRVersion, appPool.ManagedPipelineMode);
            }
        }

        private void ShowAddingPool()
        {
            AddIISPool addIISPool = new AddIISPool(adminService);
            addIISPool.Tag = this;
            addIISPool.FormClosed += (sender, e) =>
            {
                this.Enabled = true;
                UpdatePoolDataGridView();
                adminService.AddReport(currentUser, $"На веб-сервере Microsoft IIS был добавлен пул"); //eventHandler на будущее
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
                    adminService.DeletePool(selectedPoolName);
                    UpdatePoolDataGridView();
                    adminService.AddReport(currentUser, $"С веб-сервера Microsoft IIS был удален пул приложений {selectedPoolName}");
                }
            }
            else
            {
                MessageBox.Show("Выберите пул для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowEditingPool(string currentName)
        {
            EditIISPool editIISPool = new EditIISPool(adminService, currentName);
            editIISPool.Tag = this;
            editIISPool.FormClosed += (sender, e) =>
            {
                this.Enabled = true;
                UpdatePoolDataGridView();
                adminService.AddReport(currentUser, $"Изменен пул приложений {currentName} на веб-сервере Microsoft IIS"); //eventHandler на будущее
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

                if (selectedPoolState.Contains("Stopped"))
                {
                    adminService.StartAppPool(selectedPoolName);
                    UpdatePoolDataGridView();
                    adminService.AddReport(currentUser, $"На веб-сервере Microsoft IIS был запущен пул приложений {selectedPoolName}");
                }
                else
                {
                    MessageBox.Show("Пул уже запущен или в процессе запуска.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    adminService.AddReport(currentUser, $"На веб-сервере Microsoft IIS была осуществленна попытка запустить пул приложений {selectedPoolName}, который уже запущен");
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

                if (selectedPoolState.Contains("Started"))
                {
                    adminService.StopAppPool(selectedPoolName);
                    UpdatePoolDataGridView();
                    adminService.AddReport(currentUser, $"На веб-сервере Microsoft IIS был остановлен пул {selectedPoolName}");
                }
                else
                {
                    MessageBox.Show("Пул уже остановлен или в процессе остановки.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    adminService.AddReport(currentUser, $"На веб-сервере Microsoft IIS была осуществленна попытка остановить пул {selectedPoolName}, который уже остановлен");
                }
            }
            else
            {
                MessageBox.Show("Выберите пул для остановки.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
