using System.Collections.Generic;
using System.Security;

namespace Aveva.Gdp.TokenVault.Api
{
    public interface ITokenVaultManager
    {
        #region Public Methods

        void Initialize(TokenVaultAuthenticationInfo authenticationInfo);

        void Deinitialize();

        string GetToken(string tokenID);

        IEnumerable<IToken> GetAllTokens();

        IToken RegisterToken(string tokenValue, string description = null);

        #endregion
    }
}