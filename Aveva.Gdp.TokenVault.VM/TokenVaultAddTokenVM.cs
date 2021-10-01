using Aveva.Gdp.TokenVault.VM.Commands;
using System;

namespace Aveva.Gdp.TokenVault.VM
{
    public class TokenVaultAddTokenVM : BaseVM
    {
        #region Private Fields

        private string description;

        private string message;

        #endregion

        #region Public Constructors

        public TokenVaultAddTokenVM()
        {
            GenerateCommand = new RelayCommand<string>(Generate);
        }

        #endregion

        #region Public Properties

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public RelayCommand<string> GenerateCommand { get; }
        public Action<string, string> TokenGenerateAction { get; set; }
        public Action CloseAction { get; set; }

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        #endregion

        #region Protected Methods

        protected virtual void OnTokenGenerateAction(string tokenValue, string description)
        {
            TokenGenerateAction?.Invoke(tokenValue, description);
            CloseAction?.Invoke();
        }

        #endregion

        #region Private Methods

        private void Generate(string tokenValue)
        {
            if (string.IsNullOrWhiteSpace(tokenValue))
            {
                Message = "Token field cannot be empty.";
                return;
            }
            OnTokenGenerateAction(tokenValue, Description);
        }

        #endregion
    }
}