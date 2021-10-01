using Aveva.Gdp.Authentication.Api;
using Aveva.Gdp.Common.Extensions;
using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.Api.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Aveva.Gdp.TokenVault
{
    public class TokenVaultAuthenticator : IAuthenticator
    {
        #region Private Fields

        private readonly ITokenVaultManager tokenVaultManager;
        private readonly ILogger<IAuthenticator> logger;
        private readonly ICryptoFactory cryptoFactory;
        private readonly string tokenVaultPath;
        private readonly SecureString tokenVaultPassword;

        #endregion

        #region Public Constructors

        public TokenVaultAuthenticator(ILogger<IAuthenticator> logger,
                                       ITokenVaultManager tokenVaultManager,
                                       ICryptoFactory cryptoFactory, IOptions<TokenVaultConfig> tokenVaultCfg)
        {
            this.logger = logger;
            this.tokenVaultManager = tokenVaultManager;
            this.cryptoFactory = cryptoFactory;

            tokenVaultPath = Path.GetFullPath(tokenVaultCfg.Value.VaultPath);
            tokenVaultPassword = tokenVaultCfg.Value.VaultKey.ToSecureString();
        }

        #endregion

        #region Public Methods

        public async Task<AuthenticateResponse> AuthenticateAsync(CancellationToken cancelToken = default)
        {
            var errorMessage = "Authentication failed.";
            try
            {
                // TODO KBL: Implement canceler.
                if (!File.Exists(tokenVaultPath))
                    throw new InvalidOperationException($"Token Vault '{tokenVaultPath}' does not exist.");

                if (tokenVaultPassword is null || tokenVaultPassword.Length == 0)
                    throw new InvalidOperationException("Token Vault password was not provided.");

                await PrivateAuthenticate(tokenVaultPath, tokenVaultPassword);

                return new AuthenticateResponse(AuthenticatorResponseType.Success, "Authentication succeed.");
            }
            catch (Exception ex)
            {
                return new AuthenticateResponse(AuthenticatorResponseType.Failure, errorMessage, ex);
            }
        }

        public async Task<GetTokenResponse> GetTokenAsync(string tokenID, CancellationToken cancelToken = default)
        {
            // TODO KBL: Implement canceler.
            try
            {
                var token = await Task.Run(() => tokenVaultManager.GetToken(tokenID));
                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentException($"Invalid token value under the token id '{tokenID}'.");
                return new GetTokenResponse(token, AuthenticatorResponseType.Success);
            }
            catch (Exception ex)
            {
                return new GetTokenResponse(null, AuthenticatorResponseType.Failure, ex.Message, ex);
            }
        }

        #endregion

        #region Private Methods

        private async Task PrivateAuthenticate(string tokenVaultPath, SecureString tokenVaultPassword)
        {
            var cryptographer = cryptoFactory.CreateCryptographer();

            try
            {
                logger.LogInformation("Authentication...");
                var tokenSafe = await Task.Run(() => cryptographer.Decrypt(tokenVaultPassword, tokenVaultPath));
                var tokenVaultInfo = new TokenVaultInfo(tokenVaultPassword, tokenVaultPath, tokenSafe.Tokens);
                var authenticationInfo = new TokenVaultAuthenticationInfo(tokenVaultInfo, cryptographer);
                logger.LogInformation("Authentication successful.");
                tokenVaultManager.Initialize(authenticationInfo);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        #endregion
    }
}