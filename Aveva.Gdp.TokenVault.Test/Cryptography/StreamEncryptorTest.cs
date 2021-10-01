using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.Cryptography;
using NUnit.Framework;
using System;
using System.IO;
using System.Security;
using System.Text;

namespace Aveva.Gdp.TokenVault.Test.Cryptography
{
    public class StreamEncryptorTest
    {
        #region Private Fields

        private StreamEncryptor streamEncryptor;

        #endregion

        #region Public Methods

        [SetUp]
        public void Setup()
        {
            var factory = new CryptoFactory();
            streamEncryptor = new StreamEncryptor(factory);
        }

        [Test]
        public void EncryptionDecryption_Test()
        {
            using (var inputStream = new MemoryStream())
            using (var outputStream = new MemoryStream())
            using (var decryptedStream = new MemoryStream())
            {
                var secretMessage = "Secret test message 'bcba1127-e2fe-47dd-b413-bc183400e1ba'.";
                var secretMessageBytes = Encoding.UTF8.GetBytes(secretMessage);
                inputStream.Write(secretMessageBytes);
                inputStream.Position = 0;

                var password = ConvertToSecureString(":X$^4/pj7ZTEh+W]");

                streamEncryptor.Encrypt(inputStream, outputStream, password);
                var encryptedMessage = Convert.ToBase64String(outputStream.ToArray());

                streamEncryptor.Decrypt(outputStream, decryptedStream, password);
                var decryptedMessage = Encoding.UTF8.GetString(decryptedStream.ToArray());
                Assert.AreEqual(secretMessage, decryptedMessage);
                Assert.AreNotEqual(secretMessage, encryptedMessage);
            }
        }

        [Test]
        public void DecryptionWithIncorrectPassword_Test()
        {
            using (var inputStream = new MemoryStream())
            using (var outputStream = new MemoryStream())
            using (var decryptedStream = new MemoryStream())
            {
                var secretMessage = Encoding.UTF8.GetBytes("Secret test message.");
                inputStream.Write(secretMessage);
                inputStream.Position = 0;

                var password = ConvertToSecureString("upv5u&CU,=(NrD~(");
                var incorrectPassword = ConvertToSecureString("Upv5u&CU,=(NrD~(");

                streamEncryptor.Encrypt(inputStream, outputStream, password);
                Assert.Throws<IncorrectPasswordException>(() => streamEncryptor.Decrypt(outputStream, decryptedStream, incorrectPassword));
            }
        }

        #endregion

        #region Private Methods

        private SecureString ConvertToSecureString(string text)
        {
            var secureString = new SecureString();
            foreach (var character in text)
                secureString.AppendChar(character);
            return secureString;
        }

        #endregion
    }
}