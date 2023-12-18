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
    public partial class ProgForm : Form
    {
        private List<ActionItem> actionList;
        public ProgForm(string user)
        {
            InitializeComponent();
            lbHello.Text += ", "+user+"!";
            showFunctions();
        }

        private void funcLB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void showFunctions()
        {
            actionList = new List<ActionItem>
            {
                new ActionItem("Действие 1", () => 
                    MessageBox.Show("Выполнено Действие 1")),

                new ActionItem("Действие 2", () => 
                    MessageBox.Show("Выполнено Действие 2")),

                new ActionItem("Действие 3", () => 
                    MessageBox.Show("Выполнено Действие 3"))
            };

            // Заполнение ListBox названиями действий
            foreach (var action in actionList)
            {
                funcLB.Items.Add(action.Name);
            }
        }

        private void btnAct_Click(object sender, EventArgs e)
        {
            int selectedIndex = funcLB.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < actionList.Count)
            {
                actionList[selectedIndex].Action.Invoke();
            }
            else
            {
                MessageBox.Show("Выберите действие из списка.");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            AuthForm auth = new AuthForm();
            auth.Tag = this;
            auth.Show(this);
            Hide();
        }
    }

    public class ActionItem
    {
        public string Name { get; }
        public Action Action { get; }

        public ActionItem(string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }
}
