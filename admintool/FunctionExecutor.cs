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
}
