using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.Api.Cryptography;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;

namespace Aveva.Gdp.TokenVault.Cryptography
{
    public class StreamEncryptor : IStreamEncryptor
    {
        #region Private Fields

        private readonly ICryptoFactory cryptoFactory;

        #endregion

        #region Public Constructors

        public StreamEncryptor(ICryptoFactory cryptoFactory)
        {
            this.cryptoFactory = cryptoFactory;
        }

        #endregion

        #region Public Properties

        public int KeySize { get; set; } = 256;
        public int BlockSize { get; set; } = 128;
        public int DerivationIterations { get; set; } = 1_000_000;
        public int BufferSize { get; set; } = 16 * 1024;

        #endregion

        #region Public Methods

        public void Encrypt(Stream inputStream, Stream outputStream, SecureString password)
        {
            // Generate new salt and IV for each encryption (IV - Initialization Vector).
            (byte[] salt, byte[] iv) = GenerateSaltAndIV();

            // Create key from password and salt with a derivation function.
            var key = cryptoFactory.CreateAesKey(password, salt, DerivationIterations, KeySize);

            using (var aes = cryptoFactory.CreateSymmetricAlgorithm())
            using (var encryptor = aes.CreateEncryptor(key, iv))
            using (var tempStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(tempStream, encryptor, CryptoStreamMode.Write))
            {
                // Crypto write.
                inputStream.CopyTo(cryptoStream, BufferSize);
                cryptoStream.FlushFinalBlock();

                // Adding signature.
                var signature = GenerateSignature(key, tempStream);
                outputStream.Write(signature);

                // Write the salt and iv at the beginning of output-stream.
                outputStream.Write(salt);
                outputStream.Write(iv);

                // Copy the cipher content to output-stream.
                tempStream.Position = 0;
                tempStream.CopyTo(outputStream, BufferSize);

                inputStream.Position = outputStream.Position = 0;
            }
        }

        public void Decrypt(Stream inputStream, Stream outputStream, SecureString password)
        {
            var signature = ReadSignature(inputStream);
            (byte[] salt, byte[] iv) = ReadSaltAndIV(inputStream);

            var key = cryptoFactory.CreateAesKey(password, salt, DerivationIterations, KeySize);

            using (var aes = cryptoFactory.CreateSymmetricAlgorithm())
            using (var decryptor = aes.CreateDecryptor(key, iv))
            using (var tempStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(tempStream, decryptor, CryptoStreamMode.Read))
            {
                // Copy input stream to temporary memory stream.
                inputStream.CopyTo(tempStream, BufferSize);

                ValidateSignature(signature, key, tempStream);

                // Crypto read.
                cryptoStream.CopyTo(outputStream, BufferSize);
                inputStream.Position = outputStream.Position = 0;
            }
        }

        #endregion

        #region Private Methods

        private byte[] GenerateSignature(byte[] aesKey, Stream stream)
        {
            try
            {
                stream.Position = 0;

                using (var hmac = cryptoFactory.CreateKeyedHashAlgorithm(aesKey))
                    return hmac.ComputeHash(stream);
            }
            finally
            {
                stream.Position = 0;
            }
        }

        private void ValidateSignature(byte[] signature, byte[] aesKey, Stream stream)
        {
            try
            {
                stream.Position = 0;

                using (var hmac = cryptoFactory.CreateKeyedHashAlgorithm(aesKey))
                {
                    var computedSignature = hmac.ComputeHash(stream);
                    if (!computedSignature.SequenceEqual(signature))
                        throw new IncorrectPasswordException();
                }
            }
            finally
            {
                stream.Position = 0;
            }
        }

        private byte[] ReadSignature(Stream inputStream)
        {
            var signatureSizeInBytes = cryptoFactory.KeyedHashAlgorithmSignatureSize / 8;
            var signature = new byte[signatureSizeInBytes];

            inputStream.Read(signature, 0, signatureSizeInBytes);

            return signature;
        }

        private (byte[] salt, byte[] iv) GenerateSaltAndIV()
        {
            var salt = GenerateRandomBytes(KeySize / 8);
            var iv = GenerateRandomBytes(BlockSize / 8);
            return (salt, iv);
        }

        private (byte[] salt, byte[] iv) ReadSaltAndIV(Stream stream)
        {
            var salt = new byte[KeySize / 8];
            var iv = new byte[BlockSize / 8];

            stream.Read(salt, 0, KeySize / 8);
            stream.Read(iv, 0, BlockSize / 8);

            return (salt, iv);
        }

        private byte[] GenerateRandomBytes(int size)
        {
            var randomBytes = new byte[size];

            using (var rng = cryptoFactory.CreateRandomNumberGenerator())
                rng.GetBytes(randomBytes);

            return randomBytes;
        }

        #endregion
    }
}