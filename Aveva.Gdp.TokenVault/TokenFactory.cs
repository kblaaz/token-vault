using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.Api.Model;
using System;

namespace Aveva.Gdp.TokenVault
{
    public class TokenFactory : ITokenFactory
    {
        #region Public Methods

        public IToken CreateToken(string tokenValue, string description = null)
        {
            return new Token
            {
                ID = Guid.NewGuid().ToString(),
                Value = tokenValue,
                Description = description
            };
        }

        public IToken EditToken(IToken tokenToEdit, string newTokenValue, string newDescription)
        {
            if (newTokenValue is null)
                newTokenValue = ((Token)tokenToEdit).Value;

            return new Token()
            {
                ID = tokenToEdit.ID,
                Value = newTokenValue,
                Description = newDescription
            };
        }

        #endregion
    }
}