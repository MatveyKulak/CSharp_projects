using System.Xml.Serialization;

namespace Lab3
{
    [XmlRoot("text")]
    public class Text
    {
        [XmlElement("sentence")]
        public List<Sentence> Sentences { get; set; } = new List<Sentence>();

        public void AddSentence(Sentence sentence) => Sentences.Add(sentence);

        public IEnumerable<Sentence> SortByWordCount() =>
            Sentences.OrderBy(sentence => sentence.WordCount());

        public IEnumerable<Sentence> SortByLength() =>
            Sentences.OrderBy(sentence => sentence.Length);

        public IEnumerable<string> FindWordsInQuestionsByLength(int length)
        {
            return Sentences
                .Where(sentence => sentence.IsQuestion())
                .SelectMany(sentence => sentence.GetWords())
                .Where(word => word.Length == length)
                .Select(word => word.Value.ToLower())
                .Distinct();
        }

        public void DeleteWords(int length)
        {
            foreach (var sentence in Sentences)
            {
                sentence.Tokens = sentence.Tokens
                    .Where(token => !(token is Word word && word.Length == length && word.StartsWithConsonant()))
                    .ToList();
            }
        }

        public void ReplaceWordsInSentence(int sentenceIndex, int wordLength, string replacement)
        {
            if (sentenceIndex < 0 || sentenceIndex >= Sentences.Count) return;

            var sentence = Sentences[sentenceIndex];
            for (int i = 0; i < sentence.Tokens.Count; i++)
            {
                if (sentence.Tokens[i] is Word word && word.Length == wordLength)
                {
                    sentence.Tokens[i] = new Word(replacement);
                }
            }
        }

        public void RemoveStopWords(HashSet<string> stopWords)
        {
            if (stopWords == null || stopWords.Count == 0)
            {
                Console.WriteLine("Стоп-слова не заданы.");
                return;
            }

            foreach (var sentence in Sentences)
            {
                sentence.Tokens = sentence.Tokens
                    .Where(token =>
                    {
                        if (token is Word word)
                        {
                            string normalized = word.Value.ToLower().Trim();
                            return !stopWords.Contains(normalized);
                        }
                        return true;
                    })
                    .ToList();
            }
        }

        public void SaveToXmlFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(Text));
            var fileStream = new FileStream(filePath, FileMode.Create);
            serializer.Serialize(fileStream, this);
            fileStream.Close();
        }

        public Dictionary<string, (int count, SortedSet<int> lines)> BuildConcordance()
        {
            var concordance = new Dictionary<string, (int count, SortedSet<int> lines)>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < Sentences.Count; i++)
            {
                var lineNumber = i + 1;
                foreach (var word in Sentences[i].GetWords())
                {
                    string lower = word.Value.ToLower();

                    if (!concordance.ContainsKey(lower))
                        concordance[lower] = (0, new SortedSet<int>());

                    var entry = concordance[lower];
                    entry.count++;
                    entry.lines.Add(lineNumber);
                    concordance[lower] = entry;
                }
            }

            return concordance;
        }

        public override string ToString() =>
            string.Join(" ", Sentences.Select(sentence => sentence.ToString()));
    }
}