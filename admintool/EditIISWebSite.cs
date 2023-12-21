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
        private int port;
        public event EventHandler DataAdded;

        public EditIISWebSite(string currentName)
        {
            InitializeComponent();
            this.currentName = currentName;
            tbName.Text = currentName;
        }

        protected virtual void OnDataAdded()
        {
            DataAdded?.Invoke(this, EventArgs.Empty);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            path = tbPath.Text;
            newName = tbName.Text;
            port = ((int)numPort.Value);

            IISManager.ModifyWebsite(currentName, newName, path, port);
            OnDataAdded();
            Close();
        }
    }
}
