using System.Security;
using System.Security.Cryptography;

namespace Aveva.Gdp.TokenVault.Api.Cryptography
{
    public interface ICryptoFactory
    {
        #region Public Methods

        ICryptographer CreateCryptographer();

        SymmetricAlgorithm CreateSymmetricAlgorithm();

        RandomNumberGenerator CreateRandomNumberGenerator();

        byte[] CreateAesKey(SecureString password, byte[] saltBytes, int derivationIterations, int keysize);

        IStreamEncryptor CreateStreamEncryptor();

        KeyedHashAlgorithm CreateKeyedHashAlgorithm(byte[] key);

        int KeyedHashAlgorithmSignatureSize { get; }

        #endregion
    }
}