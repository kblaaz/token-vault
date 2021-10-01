using System;
using System.Xml.Serialization;

namespace Aveva.Gdp.TokenVault.Api.Model
{
    [Serializable]
    public class Token : IToken
    {
        #region Public Fields

        [XmlElement("ID")]
        public string ID { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        #endregion
    }
}