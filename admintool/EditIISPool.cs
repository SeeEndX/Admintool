using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace admintool
{
    public partial class EditIISPool : Form
    {
        private string newPoolName;
        private string currentPoolName;
        private int memoryLimit;
        private int intervalMinutes;
        private ManagedPipelineMode mode;

        public EditIISPool(string currentName)
        {
            InitializeComponent();
            this.currentPoolName = currentName;
            cbMode.SelectedItem = ManagedPipelineMode.Classic;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (tbName.Text == "")
                MessageBox.Show("Введите название пула!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                newPoolName = tbName.Text;
                memoryLimit = string.IsNullOrEmpty(tbMemory.Text) ? 0 : int.Parse(tbMemory.Text);
                intervalMinutes = (numInterval.Value == 0) ? 30 : (int)numInterval.Value;
                mode = (cbMode.Text == "Classic") ? ManagedPipelineMode.Classic : ManagedPipelineMode.Integrated;

                IISManager.ModifyPool(currentPoolName, newPoolName, mode, memoryLimit, intervalMinutes);
                Close();
            }
        }
    }
}
