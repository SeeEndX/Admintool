using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace admintool
{
    public partial class ProgForm
    {
        public partial class FunctionExecutor
        {
            private Dictionary<string, Action> functionDictionary;

            public FunctionExecutor(Dictionary<string, Action> dictionary)
            {
                functionDictionary = dictionary;
                initDictionary();
            }

            private void initDictionary()
            {
                functionDictionary["Add a pool"] = AddPool;
            }

            public void ExecuteMethodByName(string methodName)
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
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ProgForm
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ProgForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }
    }
    public partial class ProgForm
    {
        public partial class FunctionExecutor
        {
            private void CreateSite(string siteName, string physicalPath, int port)
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Site newSite = serverManager.Sites.Add(siteName, "http", $"*:{port}:", physicalPath);
                    serverManager.CommitChanges();
                }
            }

            private void AddPool()
            {
                MessageBox.Show("Выполнено Add a pool");
            }
        }
    }
}
