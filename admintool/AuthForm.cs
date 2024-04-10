using System;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.SQLite;
using AdminService;
using System.ServiceModel;

namespace admintool
{
    public partial class AuthForm : Form
    {
        IAdminService serviceClient;

        public AuthForm()
        {
            InitializeComponent();
            InitializeServiceClient();
        }

        private void InitializeServiceClient()
        {
            var binding = new NetTcpBinding();
            binding.SendTimeout = TimeSpan.FromSeconds(120);
            ChannelFactory<IAdminService> channelFactory =
                new ChannelFactory<IAdminService>(binding,
                new EndpointAddress("net.tcp://localhost:8000/AdminService"));
            serviceClient = channelFactory.CreateChannel();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Auth();
        }

        private void Auth()
        {
            string login = tbLogin.Text;
            string password = tbPass.Text;

            User user = serviceClient.Authenticate(login, password);

            if (user != null)
            {
                if (user.Group == "Admin")
                {
                    AdminForm adminForm = new AdminForm(serviceClient);
                    adminForm.Tag = this;
                    adminForm.Show(this);
                    Hide();
                }
                else if (user.Group == "Dev")
                {
                    ProgForm progForm = new ProgForm(user.Login);
                    progForm.Tag = this;
                    progForm.Show(this);
                    Hide();
                }
            }
            else
            {
                MessageBox.Show("Неверные данные!");
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void AuthForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
