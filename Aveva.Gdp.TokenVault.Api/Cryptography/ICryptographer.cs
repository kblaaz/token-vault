using Aveva.Gdp.TokenVault.Api.Model;
using System.Security;

namespace Aveva.Gdp.TokenVault.Api.Cryptography
{
    public interface ICryptographer
    {
        #region Public Methods

        void Encrypt(TokenSafe tokenVault, SecureString password, string filePath);

        TokenSafe Decrypt(SecureString password, string filePath);

        #endregion
    }
}