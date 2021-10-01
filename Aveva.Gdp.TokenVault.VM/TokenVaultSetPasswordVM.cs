using Aveva.Gdp.TokenVault.Api.Helpers;
using Aveva.Gdp.TokenVault.VM.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Security;

namespace Aveva.Gdp.TokenVault.VM
{
    public class TokenVaultSetPasswordVM : BaseVM
    {
        #region Private Fields

        private readonly ILogger logger;
        private readonly string tokenVaultFilePath;

        private string message;

        private SecureString password;

        private SecureString repeatedPassword;

        #endregion

        #region Public Constructors

        public TokenVaultSetPasswordVM(ILogger logger, string tokenVaultFilePath)
        {
            this.logger = logger;
            this.tokenVaultFilePath = tokenVaultFilePath;
            SaveCommand = new RelayCommand(Save);
        }

        #endregion

        #region Public Properties

        public SecureString Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public SecureString RepeatedPassword
        {
            get => repeatedPassword;
            set => SetProperty(ref repeatedPassword, value);
        }

        public Action<SecureString> PasswordSetAction { get; set; }

        public RelayCommand SaveCommand { get; }

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public Action CloseAction { get; set; }

        #endregion

        #region Private Methods

        private void Save()
        {
            if (!Validate(Password, RepeatedPassword))
                return;

            OnPasswordSetAction(password);
        }

        private void OnPasswordSetAction(SecureString password)
        {
            PasswordSetAction?.Invoke(password);
            CloseAction?.Invoke();
        }

        private bool Validate(SecureString password, SecureString repeatedPassword)
        {
            if (password is null || repeatedPassword is null)
            {
                Message = "One of the boxes is emtpy.";
                return false;
            }
            else if (!SecureStringHelper.Equals(password, repeatedPassword))
            {
                Message = "The password does not match.";
                return false;
            }
            return true;
        }

        #endregion
    }
}