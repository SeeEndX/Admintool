using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Collections.Generic;
using AdminService;
using System.Drawing;

namespace admintool
{
    public partial class ViewReportForm : Form
    {
        private int user;
        IAdminService serviceClient;

        public ViewReportForm(IAdminService serviceClient, int userId)
        {
            InitializeComponent();
            user = userId;
            this.serviceClient = serviceClient;
            GetReports(user);
        }

        private void GetReports(int user)
        {
            (List<string> reports, string username) = serviceClient.GetReportsForUser(user);
            if (reports.Count > 0)
            {
                label1.Text = "Отчеты для пользователя ";
                label2.Text = username;
                foreach (string reportEntry in reports)
                {
                    rtbReports.AppendText(reportEntry);
                }
            }
            else
            {
                label1.Text = $"Отчеты отсутствуют у ";
                label2.Text = username;
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
