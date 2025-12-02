using System.Text.RegularExpressions;

namespace Lab3
{
    public class Parser2
    {
        public Text Parse(string inputText)
        {
            var text = new Text();
            string[] sentenceStrings = Regex.Split(inputText, @"(?<=[\.!\?])\s+");

            foreach (var sentenceStr in sentenceStrings)
            {
                if (string.IsNullOrWhiteSpace(sentenceStr))
                    continue;

                var sentence = new Sentence();
                var tokens = Regex.Matches(sentenceStr, @"\p{L}+(?:['’-]\p{L}+)*|[^\p{L}\s'’-]");

                foreach (Match match in tokens)
                {
                    string token = match.Value;
                    if (Regex.IsMatch(token, @"^\p{L}+(?:['’-]\p{L}+)*$"))
                        sentence.AddToken(new Word(token));
                    else
                        sentence.AddToken(new Punctuation(token));
                }

                text.AddSentence(sentence);
            }

            return text;
        }
    }
}