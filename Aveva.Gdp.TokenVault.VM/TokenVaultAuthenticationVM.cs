using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.Api.Cryptography;
using Aveva.Gdp.TokenVault.VM.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security;

namespace Aveva.Gdp.TokenVault.VM
{
    public class TokenVaultAuthenticationVM : BaseVM
    {
        #region Private Fields

        private readonly ILogger logger;
        private readonly ICryptographer cryptographer;
        private readonly string tokenVaultPath;

        private string message;

        private SecureString password;

        #endregion

        #region Public Constructors

        public TokenVaultAuthenticationVM(ILogger logger, ICryptographer cryptographer, string tokenVaultPath)
        {
            this.logger = logger;
            this.cryptographer = cryptographer;
            this.tokenVaultPath = tokenVaultPath;

            AuthenticateCommand = new RelayCommand(Authenticate);
        }

        #endregion

        #region Public Events

        public event EventHandler<TokenVaultAuthenticationInfo> Authenticated;

        #endregion

        #region Public Properties

        public SecureString Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public RelayCommand AuthenticateCommand { get; }

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public Action CloseAction { get; set; }

        #endregion

        #region Private Methods

        private void Authenticate()
        {
            try
            {
                logger?.LogInformation("Authentication...");
                var tokenSafe = cryptographer.Decrypt(Password, tokenVaultPath);
                var tokenVaultInfo = new TokenVaultInfo(Password, tokenVaultPath, tokenSafe.Tokens);
                var authenticationInfo = new TokenVaultAuthenticationInfo(tokenVaultInfo, cryptographer);
                logger?.LogInformation("Authentication successful.");
                OnAuthenticating(authenticationInfo);
            }
            catch
            {
                Message = "Invalid password.";
                logger?.LogError(Message);
            }
        }

        private void OnAuthenticating(TokenVaultAuthenticationInfo authenticationInfo)
        {
            Authenticated?.Invoke(this, authenticationInfo);
            CloseAction?.Invoke();
        }

        #endregion
    }
}