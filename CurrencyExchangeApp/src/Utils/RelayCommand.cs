using System;
using System.Windows.Input;

namespace CurrencyExchangeApp.Utils
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
                
            if (parameter == null && typeof(T).IsValueType)
                return false;
                
            // Handle the case when parameter is null but T is nullable
            if (parameter == null)
                return _canExecute(default);
                
            // Check if parameter can be cast to T
            if (parameter is T typedParameter)
                return _canExecute(typedParameter);
                
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                if (typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
                {
                    throw new ArgumentNullException(nameof(parameter), 
                        $"Value cannot be null because {typeof(T)} is a non-nullable value type");
                }
                
                _execute(default);
                return;
            }
            
            if (parameter is T typedParameter)
            {
                _execute(typedParameter);
                return;
            }
            
            // Try to convert the parameter
            try
            {
                var convertedParameter = (T)Convert.ChangeType(parameter, typeof(T));
                _execute(convertedParameter);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException($"Cannot convert {parameter.GetType()} to {typeof(T)}", nameof(parameter));
            }
        }
    }
}