using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SachaBarber.CQRS.Demo.WPFClient.Commands
{
    public class SimpleCommand<T1, T2> : ICommand
    {
        private Func<T1, bool> canExecuteMethod;
        private Action<T2> executeMethod;

        public SimpleCommand(Func<T1, bool> canExecuteMethod, Action<T2> executeMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public SimpleCommand(Action<T2> executeMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = (x) => { return true; };
        }


        public bool CanExecute(T1 parameter)
        {
            if (canExecuteMethod == null) return true;
            return canExecuteMethod(parameter);
        }

        public void Execute(T2 parameter)
        {
            if (executeMethod != null)
            {
                executeMethod(parameter);
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecute((T1)parameter);
        }

        public void Execute(object parameter)
        {
            Execute((T2)parameter);
        }


        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }


        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();

        }
    }
}
