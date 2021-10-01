namespace Aveva.Gdp.TokenVault.Api
{
    public interface ITokenFactory
    {
        #region Public Methods

        IToken CreateToken(string tokenValue, string description = null);
        IToken EditToken(IToken tokenToEdit, string newTokenValue, string newDescription);

        #endregion
    }
}