using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace admintool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /*структура БД:
        table Users: id, login, pass
        table Groups: id, id_U, group_name
         */
        private void Auth()
        {
            string login;
            string password;
            string group = "admin";

            if (group == "admin")
            {
                //File.CreateText(@"C:\1.txt");
            }
            else if (group == "prog")
            {
                ListOfActions();
            }
            else
            {
                MessageBox.Show("Вы не принадлежите ни к одной из групп пользователей!", "Ошибка аутентификации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ListOfActions()
        {

        }

        private void AdminPanel()
        {
            Form1 form = new Form1();
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Auth();
        }
    }
}
