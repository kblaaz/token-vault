using System.IO;
using System.Security;

namespace Aveva.Gdp.TokenVault.Api.Cryptography
{
    public interface IStreamEncryptor
    {
        #region Public Methods

        void Encrypt(Stream inputStream, Stream outputStream, SecureString password);

        void Decrypt(Stream inputStream, Stream outputStream, SecureString password);

        #endregion
    }
}