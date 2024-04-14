using AdminService;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using static AdminService.AdministrativeService;

namespace admintool
{
    public partial class AddFunctionsForm : Form
    {
        public event EventHandler FunctionsSaved;
        string user;
        IAdminService serviceClient;

        public AddFunctionsForm(IAdminService serviceClient , string username)
        {
            InitializeComponent();
            user = username;
            lbl1.Text += username;
            this.serviceClient = serviceClient;
            RefreshFunctionList();
        }

        private void RefreshFunctionList()
        {
            List<string> allFunctionNames = serviceClient.GetAllFunctionNames();
            List<string> assignedFunctionNames = serviceClient.GetAssignedFunctionNames(user);

            functionList.Items.Clear();
            if (allFunctionNames != null)
            {
                functionList.Items.AddRange(allFunctionNames.ToArray());
            }

            if (assignedFunctionNames != null)
            {
                foreach (string functionName in assignedFunctionNames)
                {
                    int index = functionList.Items.IndexOf(functionName);
                    if (index >= 0)
                    {
                        functionList.SetItemChecked(index, true);
                    }
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<string> selectedFunctionNames = new List<string>();
            foreach (string functionName in functionList.CheckedItems)
            {
                selectedFunctionNames.Add(functionName);
            }

            serviceClient.SaveAssignedFunctions(user, selectedFunctionNames);
            FunctionsSaved?.Invoke(this, EventArgs.Empty);
            MessageBox.Show("Данные сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
