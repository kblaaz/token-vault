using Aveva.Gdp.Authentication.Api;
using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.Api.Cryptography;
using Aveva.Gdp.TokenVault.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aveva.Gdp.TokenVault.Extensions
{
    public static class HostBuilderExtensions
    {
        #region Public Methods

        public static IHostBuilder ConfigureTokenVaultAuthentication(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.Configure<TokenVaultConfig>(hostContext.Configuration.GetSection("TokenVaultAuthentication"));
                services.AddSingleton<ICryptoFactory, CryptoFactory>();
                services.AddSingleton<ITokenFactory, TokenFactory>();
                services.AddSingleton<ITokenVaultManager, TokenVaultManager>();
                services.AddSingleton<IAuthenticator, TokenVaultAuthenticator>();
            });
        }

        #endregion
    }
}