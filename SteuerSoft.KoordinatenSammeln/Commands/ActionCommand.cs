using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SteuerSoft.KoordinatenSammeln.Commands
{
    class ActionCommand : ICommand
    {
        private Action _action;
        private bool _canExecute = true;

        public ActionCommand(Action cmd)
        {
            _action = cmd;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }

        public void SetCanExecute(bool canExecute)
        {
            _canExecute = canExecute;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}
