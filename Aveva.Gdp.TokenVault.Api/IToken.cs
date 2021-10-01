namespace Aveva.Gdp.TokenVault.Api
{
    public interface IToken
    {
        #region Public Properties

        string ID { get; }
        string Description { get; set; }

        #endregion
    }
}