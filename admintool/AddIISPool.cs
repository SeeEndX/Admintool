using AdminService;
using Microsoft.Web.Administration;
using System;
using System.Windows.Forms;

namespace admintool
{
    public partial class AddIISPool : Form
    {
        private string poolName;
        private int memoryLimit;
        private int intervalMinutes;
        private ManagedPipelineMode mode;
        IAdminService adminService;

        public AddIISPool(IAdminService adminService)
        {
            InitializeComponent();
            this.adminService = adminService;
            cbMode.SelectedItem = ManagedPipelineMode.Classic;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbName.Text == "")
                MessageBox.Show("Введите название пула!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                poolName = tbName.Text;
                
                memoryLimit = string.IsNullOrEmpty(tbMemory.Text) ? 0 : int.Parse(tbMemory.Text);
                intervalMinutes = (numInterval.Value == 0) ? 30 : (int)numInterval.Value;
                mode = (cbMode.Text == "Classic") ? ManagedPipelineMode.Classic : ManagedPipelineMode.Integrated;

                adminService.CreatePool(poolName, mode, memoryLimit, intervalMinutes);
                Close();
            }
        }
    }
}
