using Aveva.Gdp.TokenVault.Api.Cryptography;
using Aveva.Gdp.TokenVault.Api.Helpers;
using System.Security;
using System.Security.Cryptography;

namespace Aveva.Gdp.TokenVault.Cryptography
{
    public class CryptoFactory : ICryptoFactory
    {
        #region Public Properties

        public int KeyedHashAlgorithmSignatureSize { get; } = 512;

        #endregion

        #region Public Methods

        public RandomNumberGenerator CreateRandomNumberGenerator()
        {
            return new RNGCryptoServiceProvider();
        }

        public SymmetricAlgorithm CreateSymmetricAlgorithm()
        {
            return new RijndaelManaged
            {
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
        }

        public byte[] CreateAesKey(SecureString password, byte[] salt, int iterations, int keysize)
        {
            return SecureStringHelper.ProcessBytes(password, passwordBytes =>
            {
                using (var keyDeriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, iterations))
                    return keyDeriveBytes.GetBytes(keysize / 8);
            });
        }

        public IStreamEncryptor CreateStreamEncryptor() => new StreamEncryptor(this);

        public KeyedHashAlgorithm CreateKeyedHashAlgorithm(byte[] key) => new HMACSHA512(key);

        public ICryptographer CreateCryptographer() => new DefaultCryptographer(this);

        #endregion
    }
}