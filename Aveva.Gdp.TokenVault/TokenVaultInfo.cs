using Aveva.Gdp.TokenVault.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Aveva.Gdp.TokenVault
{
    public class TokenVaultInfo : ITokenVaultInfo
    {
        #region Public Constructors

        public TokenVaultInfo(SecureString password, string path, IEnumerable<IToken> tokens = null)
        {
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Tokens = tokens is null ? Tokens = new List<IToken>() : new List<IToken>(tokens.ToArray());
        }

        #endregion

        #region Public Properties

        public SecureString Password { get; }

        public string Path { get; }

        public List<IToken> Tokens { get; }

        #endregion
    }
}