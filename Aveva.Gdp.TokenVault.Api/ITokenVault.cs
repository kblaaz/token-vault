using System.Security;

namespace Aveva.Gdp.TokenVault.Api
{
    public interface ITokenVault
    {
        #region Public Methods

        public SecureString GetToken(string tokenId);

        public void SetToken(string tokenId, SecureString tokenKey);

        #endregion
    }
}