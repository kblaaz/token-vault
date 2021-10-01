using Aveva.Gdp.TokenVault.Api.Cryptography;
using Aveva.Gdp.TokenVault.Api.Model;
using System;
using System.IO;
using System.Security;
using System.Xml.Serialization;

namespace Aveva.Gdp.TokenVault.Cryptography
{
    public class DefaultCryptographer : ICryptographer
    {
        #region Private Fields

        private readonly ICryptoFactory cryptoFactory;
        private readonly IStreamEncryptor streamEncryptor;

        #endregion

        #region Public Constructors

        public DefaultCryptographer(ICryptoFactory cryptoFactory)
        {
            this.cryptoFactory = cryptoFactory;
            streamEncryptor = cryptoFactory.CreateStreamEncryptor();
        }

        #endregion

        #region Public Methods

        public void Encrypt(TokenSafe tokenVault, SecureString password, string filePath)
        {
            File.Delete(filePath);
            using (var streamToEncrypt = new MemoryStream())
            using (var tokenVaultStream = File.OpenWrite(filePath))
            {
                var xmlSerializer = new XmlSerializer(typeof(TokenSafe));
                xmlSerializer.Serialize(streamToEncrypt, tokenVault);
                streamToEncrypt.Position = 0;
                streamEncryptor.Encrypt(streamToEncrypt, tokenVaultStream, password);
            }
        }

        public TokenSafe Decrypt(SecureString password, string filePath)
        {
            if (!File.Exists(filePath))
                throw new OperationCanceledException($"The '${filePath}' file does not exist.");

            using (var decryptedStream = new MemoryStream())
            using (var tokenVaultStream = File.OpenRead(filePath))
            {
                streamEncryptor.Decrypt(tokenVaultStream, decryptedStream, password);
                var xmlSerializer = new XmlSerializer(typeof(TokenSafe));
                return xmlSerializer.Deserialize(decryptedStream) as TokenSafe;
            }
        }

        #endregion
    }
}