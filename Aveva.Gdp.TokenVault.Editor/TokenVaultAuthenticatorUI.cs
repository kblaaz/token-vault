//using Aveva.Gdp.Authentication.Api;
//using Aveva.Gdp.TokenVault.Api;
//using Aveva.Gdp.TokenVault.Api.Cryptography;
//using Aveva.Gdp.TokenVault.Extensions;
//using Aveva.Gdp.TokenVault.UI;
//using Aveva.Gdp.TokenVault.VM;
//using Microsoft.Extensions.Logging;
//using System;
//using System.IO;
//using System.Reflection;
//using System.Threading;

//namespace Aveva.Gdp.TokenVault.Editor.Authenticators
//{
//    public class TokenVaultAuthenticatorUI : IAuthenticator
//    {
//        #region Private Fields

//        private readonly ITokenVaultManager tokenVaultManager;
//        private readonly string tokenVaultPath;
//        private readonly ILogger<IAuthenticator> logger;
//        private readonly ICryptoFactory cryptoFactory;

//        #endregion

//        #region Public Constructors

//        public TokenVaultAuthenticatorUI(ILogger<IAuthenticator> logger,
//                                       ITokenVaultManager tokenVaultManager,
//                                       ICryptoFactory cryptoFactory)
//        {
//            this.logger = logger;
//            this.tokenVaultManager = tokenVaultManager;
//            this.cryptoFactory = cryptoFactory;

//            // TODO KBL: Retrieve token vault path from configuration.
//            tokenVaultPath = Path.Combine(Assembly.GetExecutingAssembly().GetDirectory(), "tokens.vault");
//        }

//        #endregion

//        #region Public Methods

//        public void Authenticate()
//        {
//            if (!File.Exists(tokenVaultPath))
//                throw new InvalidOperationException("Token Vault does not exist");

//            PrivateAuthenticate();
//        }

//        public string GetToken(string tokenId) => tokenVaultManager.GetToken(tokenId);

//        #endregion

//        #region Private Methods

//        private void PrivateAuthenticate()
//        {
//            var parametrizedThreadStart = new ParameterizedThreadStart(obj =>
//            {
//                var cryptographer = cryptoFactory.CreateCryptographer();
//                var tokenVaultAuthenticationWindow = new TokenVaultAuthenticationWindow();
//                var tokenVaultAuthenticationVM = new TokenVaultAuthenticationVM(logger, cryptographer, tokenVaultPath)
//                {
//                    CloseAction = tokenVaultAuthenticationWindow.Close
//                };

//                try
//                {
//                    tokenVaultAuthenticationVM.Authenticated += TokenVaultLoginVM_Authenticated;
//                    tokenVaultAuthenticationWindow.Initialize(tokenVaultAuthenticationVM);
//                    tokenVaultAuthenticationWindow.ShowDialog();
//                }
//                finally
//                {
//                    tokenVaultAuthenticationVM.Authenticated -= TokenVaultLoginVM_Authenticated;
//                }
//            });

//            var staThread = new Thread(parametrizedThreadStart);
//            staThread.SetApartmentState(ApartmentState.STA);
//            staThread.Start();
//            staThread.Join();
//        }

//        private void TokenVaultLoginVM_Authenticated(object sender, TokenVaultAuthenticationInfo authenticationInfo)
//        {
//            tokenVaultManager.Initialize(authenticationInfo);
//        }

//        #endregion
//    }
//}