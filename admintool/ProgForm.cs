using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

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

        private void showFunctions()
        {
            actionList = new List<ActionItem>
            {
                new ActionItem("Действие 1", () => 
                    createTxt1()),

                new ActionItem("Действие 2", () => 
                    createTxt2()),

                new ActionItem("Действие 3", () => 
                    MessageBox.Show("Выполнено Действие 3"))
            };

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

        private void createTxt1()
        {
            File.CreateText(@"C:\1.txt");
        }

        private void createTxt2()
        {
            File.CreateText(@"C:\2.txt");
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
