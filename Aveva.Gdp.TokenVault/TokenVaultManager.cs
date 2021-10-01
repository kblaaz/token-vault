using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.Api.Cryptography;
using Aveva.Gdp.TokenVault.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Aveva.Gdp.TokenVault
{
    public class TokenVaultManager : ITokenVaultManager
    {
        #region Private Fields

        private readonly ITokenFactory tokenFactory;
        private Dictionary<string, IToken> tokenDictionary = new Dictionary<string, IToken>();

        #endregion

        #region Public Constructors

        public TokenVaultManager(ITokenFactory tokenFactory)
        {
            this.tokenFactory = tokenFactory;
        }

        #endregion

        #region Public Properties

        public bool Initialized { get; private set; }

        #endregion

        #region Internal Properties

        internal SecureString Password { get; private set; }
        internal string TokenVaultPath { get; private set; }
        internal ICryptographer Cryptographer { get; private set; }

        #endregion

        #region Public Methods

        public void Initialize(TokenVaultAuthenticationInfo authenticationInfo)
        {
            if (Initialized)
                throw new InvalidOperationException("TokenVaultManager is already initialized.");

            Password = authenticationInfo.TokenVaultInfo.Password;
            TokenVaultPath = authenticationInfo.TokenVaultInfo.Path;
            Cryptographer = authenticationInfo.Cryptographer;
            Initialized = true;
        }

        public void Deinitialize()
        {
            ThrowIfNotInitialized();
            Initialized = false;
        }

        public IEnumerable<IToken> GetAllTokens()
        {
            return ProcessDecryptedSafe(safe => safe.Tokens.ToArray());
        }

        public string GetToken(string tokenID)
        {
            ThrowIfNotInitialized();
            return ProcessDecryptedSafe(safe => safe.Tokens.First(item => item.ID == tokenID).Value);
        }

        public IToken RegisterToken(string tokenValue, string description = null)
        {
            ThrowIfNotInitialized();
            return tokenFactory.CreateToken(tokenValue, description);
        }

        #endregion

        #region Private Methods

        private T ProcessDecryptedSafe<T>(Func<TokenSafe, T> func)
        {
            ThrowIfNotInitialized();
            using (var safe = Cryptographer.Decrypt(Password, TokenVaultPath))
                return func.Invoke(safe);
        }

        private void ThrowIfNotInitialized()
        {
            if (!Initialized)
                throw new InvalidOperationException("TokenVaultManager is not initialized.");
        }

        #endregion
    }
}