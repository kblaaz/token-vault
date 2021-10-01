using System;
using System.Windows.Input;

namespace Aveva.Gdp.TokenVault.VM.Commands
{
    public class RelayCommand<T> : ICommand
    {
        #region Private Fields

        private readonly Action<T> executeAction;
        private readonly Predicate<T> canExecute;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action<T> execute) : this(execute, null) { }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute is null)
                throw new ArgumentNullException("execute");

            executeAction = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.<br/>
        /// Beware - should use weak references if command instance lifetime is longer than lifetime of UI objects that get hooked up to command.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter) => canExecute is null ? true : canExecute((T)parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public void Execute(object parameter) => executeAction((T)parameter);

        #endregion
    }

    public class RelayCommand : ICommand
    {
        #region Private Fields

        private readonly Action executeAction;
        private readonly Func<bool> canExecute;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action execute)
        {
            executeAction = execute;
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            executeAction = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.<br/>
        /// Beware - should use weak references if command instance lifetime is longer than lifetime of UI objects that get hooked up to command.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter = null)
        {
            if (canExecute != null)
            {
                return canExecute();
            }
            if (executeAction != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the
        public void Execute(object parameter = null)
        {
            executeAction?.Invoke();
        }

        #endregion
    }
}