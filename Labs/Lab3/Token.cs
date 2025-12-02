using System.Xml.Serialization;

namespace Lab3
{
    public abstract class Token
    {
        [XmlText]
        public string Value { get; set; }

        protected Token() { }
        protected Token(string value) => Value = value;
    }
}