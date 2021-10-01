using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aveva.Gdp.TokenVault.Api.Model
{
    [Serializable]
    public class TokenSafe : IDisposable
    {
        #region Private Fields

        private bool disposedValue;

        #endregion

        #region Public Properties

        [XmlArray("Tokens")]
        [XmlArrayItem(typeof(Token))]
        public List<Token> Tokens { get; set; }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var token in Tokens)
                    {
                        token.ID = null;
                        token.Value = null;
                        token.Description = null;
                    }

                    Tokens.Clear();
                    Tokens = null;
                }

                disposedValue = true;
            }
        }

        #endregion
    }
}