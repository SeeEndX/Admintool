using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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
    }

    public partial class ProgForm
    {
        public partial class FunctionExecutor
        {

            private void AddPool()
            {
                MessageBox.Show("Выполнено Add a pool");
            }
        }
    }
}
