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

        public event EventHandler DataAdded;

        public AddIISWebSite()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            path = tbPath.Text;
            name = tbName.Text;
            port = ((int)numPort.Value);

            IISManager.CreateWebsite(name, path, port);
            OnDataAdded();
            Close();
        }

        protected virtual void OnDataAdded()
        {
            DataAdded?.Invoke(this, EventArgs.Empty);
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
