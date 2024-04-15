using AdminService;
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
    public partial class AddIISWebSite : Form
    {
        private string path;
        private string name;
        private int port;
        IAdminService adminService;

        public AddIISWebSite(IAdminService adminService)
        {
            this.adminService = adminService;
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbName.Text == "" || tbPath.Text == "")
            {
                MessageBox.Show("Введите все данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                path = tbPath.Text;
                name = tbName.Text;
                port = ((int)numPort.Value);

                adminService.CreateWebsite(name, path, port);
                Close();
            }
        }

        private void ChooseDirectory()
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.ShowNewFolderButton = false;
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = FBD.SelectedPath;
            }
        }

        private void btnDirect_Click(object sender, EventArgs e)
        {
            ChooseDirectory();
        }
    }
}
