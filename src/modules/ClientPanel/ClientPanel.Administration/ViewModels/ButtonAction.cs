using System;
using System.Threading.Tasks;

namespace ClientPanel.Administration.ViewModels
{
    public class ButtonAction
    {
        public ButtonAction(string header, Func<Task> action)
        {
            Header = header;
            Action = action;
        }

        public string Header { get; set; }
        public Func<Task> Action { get; set; }
    }
}