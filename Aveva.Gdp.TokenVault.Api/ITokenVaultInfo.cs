using System.Collections.Generic;
using System.Security;

namespace Aveva.Gdp.TokenVault.Api
{
    public interface ITokenVaultInfo
    {
        #region Public Properties

        SecureString Password { get; }
        string Path { get; }
        List<IToken> Tokens { get; }

        #endregion
    }
}