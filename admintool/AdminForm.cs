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
    public partial class AdminForm : Form
    {
        private List<User> userList;

        public AdminForm()
        {
            
            userList = new List<User>
            {
                new User { Id = 1, Name = "Пользователь 1", Functions = new List<string> { "Функция 1", "Функция 2" } },
                new User { Id = 2, Name = "Пользователь 2", Functions = new List<string> { "Функция 3", "Функция 4" } },
            };
            InitializeComponent();
            foreach (var user in userList)
            {
                string functionsString = string.Join(", ", user.Functions);
                dgvUsers.Rows.Add(user.Name, functionsString);
            }
        }

        /*структура БД:
        table Users: id, login, pass
        table Groups: id, id_U, group_name
         */
        private void dgwUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvUsers.Columns["Functions"].Index && e.RowIndex >= 0)
            {
                User selectedUser = (User)dgvUsers.Rows[e.RowIndex].DataBoundItem;

                //форма редактирования
                /*EditFunctionsForm editFuncti1onsForm = new EditFunctionsForm(selectedUser);
                editFunctionsForm.ShowDialog();*/

                //обновление данных после изменения
                dgvUsers.Refresh();
            }
        }

/*        private void ListOfActions()
        {

        }*/


        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            AuthForm auth = new AuthForm();
            auth.Tag = this;
            auth.Show(this);
            Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in dgvUsers.SelectedRows)
            {
                string name = Convert.ToString(row.Cells[0].Value);
                User userToRemove = userList.Find(u => u.Name == name);
                userList.Remove(userToRemove);
            }

            // Обновление DataGridView после удаления записей
            dgvUsers.Rows.Clear();
            foreach (var user in userList)
            {
                string functionsString = string.Join(", ", user.Functions);
                dgvUsers.Rows.Add(user.Name, functionsString);
                dgvUsers.Rows[dgvUsers.Rows.Count - 1].Cells["login"].Value = user.Name;
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Functions { get; set; }
    }

    public partial class EditFunctionsForm : Form
    {
        private User user;

        /*public EditFunctionsForm(User selectedUser)
        {
            InitializeComponent();

            user = selectedUser;

            // Отображение функций текущего пользователя в TextBox
            textBoxFunctions.Text = string.Join(", ", user.Functions);
        }*/
/*private void btnUpdateFunctions_Click(object sender, EventArgs e)
        {
            // Обновление списка функций пользователя на основе данных из TextBox
            user.Functions = new List<string>(textBoxFunctions.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            // Закрытие формы редактирования
            Close();
        }*/
        
    }
}
