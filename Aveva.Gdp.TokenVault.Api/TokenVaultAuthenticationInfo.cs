using Aveva.Gdp.TokenVault.Api.Cryptography;
using System.Diagnostics;

namespace Aveva.Gdp.TokenVault.Api
{
    public class TokenVaultAuthenticationInfo
    {
        #region Public Constructors

        public TokenVaultAuthenticationInfo(ITokenVaultInfo tokenVaultInfo, ICryptographer cryptographer)
        {
            Debug.Assert(tokenVaultInfo != null);
            Debug.Assert(cryptographer != null);

            TokenVaultInfo = tokenVaultInfo;
            Cryptographer = cryptographer;
        }

        #endregion

        #region Public Properties

        public ITokenVaultInfo TokenVaultInfo { get; }

        public ICryptographer Cryptographer { get; }

        #endregion
    }
}