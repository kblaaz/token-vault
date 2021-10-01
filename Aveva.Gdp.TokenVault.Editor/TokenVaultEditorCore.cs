using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.Api.Cryptography;
using Aveva.Gdp.TokenVault.Api.Model;
using Aveva.Gdp.TokenVault.Cryptography;
using Aveva.Gdp.TokenVault.Extensions;
using Aveva.Gdp.TokenVault.UI;
using Aveva.Gdp.TokenVault.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows;

namespace Aveva.Gdp.TokenVault.Editor
{
    public class TokenVaultEditorCore : IDisposable
    {
        #region Private Fields

        private readonly ICryptographer cryptographer;
        private readonly ITokenFactory tokenFactory;
        private readonly string coreDirectory;
        private IDialogProvider dialogProvider;
        private bool disposedValue;

        private Window mainWindow;

        #endregion

        #region Public Constructors

        public TokenVaultEditorCore()
        {
            cryptographer = new DefaultCryptographer(new CryptoFactory());
            coreDirectory = Assembly.GetExecutingAssembly().GetDirectory();
            tokenFactory = new TokenFactory();
        }

        #endregion

        #region Public Methods

        public void Run()
        {
            var tokenVaultEditorWindow = new TokenVaultEditorWindow();
            mainWindow = tokenVaultEditorWindow;
            dialogProvider = new DialogProvider(mainWindow);

            var tokenVaultEditorVM = new TokenVaultEditorVM(dialogProvider, coreDirectory)
            {
                NewExecuteFunc = OnNewExecuted,
                OpenExecuteFunc = OnOpenExecuted,
                SaveExecuteAction = OnSaveExecuted,
                AddExecuteFunc = OnAddExecuted,
                EditExecuteFunc = OnEditExecuted,
                CloseAction = tokenVaultEditorWindow.Close
            };

            tokenVaultEditorWindow.Initialize(tokenVaultEditorVM);
            tokenVaultEditorWindow.ShowDialog();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                disposedValue = true;
            }
        }

        #endregion

        #region Private Methods

        private ITokenVaultInfo OnOpenExecuted(string tokenVaultPath)
        {
            var tokenVaultAuthenticationWindow = new TokenVaultAuthenticationWindow(mainWindow);
            // TODO KBL: Add logger as param.
            var tokenVaultAuthenticationVM = new TokenVaultAuthenticationVM(null, cryptographer, tokenVaultPath)
            {
                CloseAction = tokenVaultAuthenticationWindow.Close,
            };

            tokenVaultAuthenticationWindow.Initialize(tokenVaultAuthenticationVM);

            IEnumerable<IToken> tokens = default;
            SecureString password = default;
            tokenVaultAuthenticationVM.Authenticated += (s, e) =>
            {
                tokens = e.TokenVaultInfo.Tokens;
                password = e.TokenVaultInfo.Password;
            };

            tokenVaultAuthenticationWindow.ShowDialog();

            return tokens is null ? null : new TokenVaultInfo(password, tokenVaultPath, tokens);
        }

        private ITokenVaultInfo OnNewExecuted(string filePath, IEnumerable<IToken> tokens)
        {
            var tokenVaultSetPasswordWindow = new TokenVaultSetPasswordWindow(mainWindow);

            SecureString createdPassword = default;
            var tokenVaultSetPasswordVM = new TokenVaultSetPasswordVM(null, filePath)
            {
                CloseAction = tokenVaultSetPasswordWindow.Close,
                PasswordSetAction = (password) => createdPassword = password
            };

            tokenVaultSetPasswordWindow.Initialize(tokenVaultSetPasswordVM);
            tokenVaultSetPasswordWindow.ShowDialog();
            if (createdPassword is null)
                return null;

            var tokenVaultInfo = new TokenVaultInfo(createdPassword, filePath);

            if (tokens != null)
                tokenVaultInfo.Tokens.AddRange(tokens);

            OnSaveExecuted(tokenVaultInfo);
            return tokenVaultInfo;
        }

        private void OnSaveExecuted(ITokenVaultInfo tokenVaultInfo)
        {
            var tokenSafe = new TokenSafe();
            tokenSafe.Tokens = tokenVaultInfo.Tokens.Select(token => token as Token).ToList();

            cryptographer.Encrypt(tokenSafe, tokenVaultInfo.Password, tokenVaultInfo.Path);
        }

        private IToken OnAddExecuted()
        {
            var tokenVaultAddTokenWindow = new TokenVaultAddTokenWindow(mainWindow);

            IToken token = default;
            var tokenVaultAddTokenVM = new TokenVaultAddTokenVM()
            {
                CloseAction = tokenVaultAddTokenWindow.Close,
                TokenGenerateAction = (tokenValue, description) => token = OnTokenGenerate(tokenValue, description)
            };

            tokenVaultAddTokenWindow.Initialize(tokenVaultAddTokenVM);
            tokenVaultAddTokenWindow.ShowDialog();

            return token;
        }

        private IToken OnEditExecuted(IToken tokenToEdit)
        {
            var tokenVaultEditTokenWindow = new TokenVaultEditTokenWindow(mainWindow);

            IToken token = default;
            var tokenVaultEditTokenVM = new TokenVaultEditTokenVM(tokenToEdit)
            {
                CloseAction = tokenVaultEditTokenWindow.Close,
                TokenEditAction = (tokenToEdit, tokenValue, description) => token = OnTokenEdit(tokenToEdit, tokenValue, description),
                Description = tokenToEdit.Description
            };

            tokenVaultEditTokenWindow.Initialize(tokenVaultEditTokenVM);
            tokenVaultEditTokenWindow.ShowDialog();

            return token;
        }

        private IToken OnTokenGenerate(string tokenValue, string description)
        {
            return tokenFactory.CreateToken(tokenValue, description);
        }

        private IToken OnTokenEdit(IToken tokenToEdit, string newTokenValue, string newDescription)
        {
            return tokenFactory.EditToken(tokenToEdit, newTokenValue, newDescription);
        }

        #endregion
    }
}