namespace Lab3
{
    public class Word : Token
    {
        public Word() { }
        public Word(string value) : base(value) { }

        public int Length => Value.Length;

        public bool StartsWithConsonant()
        {
            if (string.IsNullOrEmpty(Value)) return false;
            char first = char.ToLower(Value[0]);
            string vowels = "аеёиоуыэюяaeiou";
            return char.IsLetter(first) && !vowels.Contains(first);
        }

        public override string ToString() => Value;
    }
}