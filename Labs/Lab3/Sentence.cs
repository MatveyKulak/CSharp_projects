using System.Xml.Serialization;

namespace Lab3
{
    public class Sentence
    {
        [XmlElement("word", typeof(Word))]
        [XmlElement("punctuation", typeof(Punctuation))]
        public List<Token> Tokens { get; set; } = new List<Token>();

        public void AddToken(Token token) => Tokens.Add(token);

        public IEnumerable<Word> GetWords() => Tokens.OfType<Word>();

        public int WordCount() => GetWords().Count();

        public int Length => Tokens.Sum(token => token.Value.Length);

        public bool IsQuestion() => Tokens.Any(token => token.Value == "?");

        public override string ToString() =>
            string.Join(" ", Tokens.Select(token => token.Value))
                .Replace(" .", ".")
                .Replace(" ,", ",")
                .Replace(" !", "!")
                .Replace(" ?", "?");
    }
}