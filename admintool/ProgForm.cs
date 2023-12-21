using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Reflection;
using System.IO;

namespace admintool
{
    public partial class ProgForm : Form
    {
        private string cs = @"URI=file:C:\\Users\\ars_1\\Documents\\dbForAdminProg\\AdminToolDB.db";
        SQLiteConnection con;
        SQLiteCommand cmd;

        private List<ActionItem> actionList;
        private string currentUser;

        private Dictionary<string, Action> functionDictionary = new Dictionary<string, Action>();
        
        public ProgForm(string user)
        {
            InitializeComponent();
            lbHello.Text += ", "+user+"!";
            currentUser = user;
            showFunctions();
            initDictionary();
        }

        private void initDictionary()
        {
            functionDictionary["Create txt 1"] = СreateTxt1;
            functionDictionary["Create txt 2"] = CreateTxt2;
            functionDictionary["Add a pool"] = AddPool;
        }

        private void showFunctions()
        {
            actionList = GetFunctionsForUser(currentUser);

            foreach (var action in actionList)
            {
                funcLB.Items.Add(action.Name);
            }
        }

        private List<ActionItem> GetFunctionsForUser(string user)
        {
            List<ActionItem> functions = new List<ActionItem>();

            using (con = new SQLiteConnection(cs))
            {
                con.Open();

                string query = @"
            SELECT f.name
            FROM Function f
            JOIN Function_users fu ON f.id = fu.function
            JOIN Users u ON u.id = fu.user
            WHERE u.login = @User;";

                using (cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@User", user);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string functionNameFromDb = reader.GetString(0);
                            Action action = () => ExecuteMethodByName(functionNameFromDb);
                            functions.Add(new ActionItem(functionNameFromDb, action));
                        }
                    }
                }
            }

            return functions;
        }

        private void btnAct_Click(object sender, EventArgs e)
        {
            int selectedIndex = funcLB.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < actionList.Count)
            {
                ExecuteMethodByName(actionList[selectedIndex].Name);
            }
            else
            {
                MessageBox.Show("Выберите действие из списка.");
            }
        }

        private void ExecuteMethodByName(string methodName)
        {
            if (functionDictionary.TryGetValue(methodName, out Action action))
            {
                action?.Invoke();
            }
            else
            {
                MessageBox.Show($"Метод '{methodName}' не найден.");
            }
        }

        private void СreateTxt1()
        {
            File.CreateText(@"C:\1.txt");
            MessageBox.Show("Выполнено Create Txt 1");
        }

        private void CreateTxt2()
        {
            File.CreateText(@"C:\2.txt");
            MessageBox.Show("Выполнено Create Txt 2");
        }

        private void AddPool()
        {
            MessageBox.Show("Выполнено Add a pool");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            AuthForm auth = new AuthForm();
            auth.Tag = this;
            auth.Show(this);
            Hide();
        }

        private void ProgForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
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
