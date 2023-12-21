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
    public partial class EditIISWebSite : Form
    {
        private string path;
        private string currentName;
        private string newName;

        public EditIISWebSite(string currentName)
        {
            InitializeComponent();
            this.currentName = currentName;
            tbName.Text = currentName;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (tbPath.Text == "" || tbName.Text == "")
            {
                MessageBox.Show("Введите все данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                path = tbPath.Text;
                newName = tbName.Text;
                IISManager.ModifyWebsite(currentName, newName, path);
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
