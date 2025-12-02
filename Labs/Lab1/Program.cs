namespace Lab1
{
    class Program
    {
        static List<GeneticData> data = new List<GeneticData>();
        static StreamWriter writer;

        static string GetFormula(string proteinName)
        {
            foreach (GeneticData item in data)
            {
                if (item.name.Equals(proteinName)) return item.formula;
            }
            return null;
        }
        static void ReadGeneticData(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Ошибка: файл {filename} не найден");
                return;
            }

            StreamReader reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] fragments = line.Split('\t');
                GeneticData protein;
                protein.name = fragments[0];
                protein.organism = fragments[1];
                protein.formula = fragments[2];
                protein.formula = Decoding(fragments[2]);
                data.Add(protein);
            }
            reader.Close();
        }
        static void ReadHandleCommands(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            int counter = 0;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine(); counter++;
                string[] command = line.Split('\t');

                switch (command[0])
                {
                    case "search":
                        writer.WriteLine($"{counter:D3}   search   {Decoding(command[1])}");
                        Search(command[1]);
                        writer.WriteLine("================================================");
                        break;

                    case "diff":
                        writer.WriteLine($"{counter:D3}   diff   {command[1]}   {command[2]}");
                        Diff(command[1], command[2]);
                        writer.WriteLine("================================================");
                        break;

                    case "mode":
                        writer.WriteLine($"{counter:D3}   mode   {command[1]}");
                        Mode(command[1]);
                        writer.WriteLine("================================================");
                        break;

                    default:
                        writer.WriteLine($"{counter:D3}   unknown command: {command[0]}");
                        writer.WriteLine("================================================");
                        break;
                }
            }
            reader.Close();
        }
        static string Decoding(string formula)
        {
            string decoded = String.Empty;

            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]) && i + 1 < formula.Length)
                {
                    char letter = formula[i + 1];
                    int conversion = formula[i] - '0';
                    for (int j = 0; j < conversion; j++)
                        decoded += letter;
                    i++;
                }
                else decoded = decoded + formula[i];
            }
            return decoded;
        }
        static void Search(string amino_acid)
        {
            string decoded = Decoding(amino_acid);
            bool found = false;

            foreach (var item in data)
            {
                if (item.formula.Contains(decoded))
                {
                    writer.WriteLine($"{item.organism}    {item.name}");
                    found = true;
                }
            }

            if (!found) writer.WriteLine("NOT FOUND");
        }
        static void Diff(string protein1, string protein2)
        {
            string seq1 = GetFormula(protein1);
            string seq2 = GetFormula(protein2);

            if (seq1 == null || seq2 == null)
            {
                writer.Write("amino-acids difference: MISSING:");
                if (seq1 == null) 
                    writer.Write($" {protein1}");
                if (seq2 == null)
                    writer.Write($" {protein2}");
                writer.WriteLine();
                return;
            }

            int minLength = Math.Min(seq1.Length, seq2.Length);
            int difference = 0;

            for (int i = 0; i < minLength; i++)
            {
                if (seq1[i] != seq2[i])
                    difference++;
            }

            difference += Math.Abs(seq1.Length - seq2.Length);
            writer.WriteLine($"amino-acids difference: {difference}");
        }
        static void Mode(string proteinName)
        {
            string sequence = GetFormula(proteinName);

            if (sequence == null)
            {
                Console.WriteLine($"amino-acid occurs: MISSING: {proteinName}");
                return;
            }

            Dictionary<char, int> countMap = new Dictionary<char, int>();

            foreach (char c in sequence)
            {
                if (countMap.ContainsKey(c))
                    countMap[c]++;
                else
                    countMap[c] = 1;
            }

            int maxCount = 0;
            char mostFrequentChar = ' ';

            foreach (var pair in countMap)
            {
                if (pair.Value > maxCount ||
                    (pair.Value == maxCount && pair.Key < mostFrequentChar))
                {
                    maxCount = pair.Value;
                    mostFrequentChar = pair.Key;
                }
            }

            writer.WriteLine($"amino-acid occurs: {mostFrequentChar} {maxCount}");
        }
        static void Main(string[] args)
        {
            Console.Write("Введите индекс набора (0, 1 или 2): ");
            string index = Console.ReadLine();

            ReadGeneticData($"sequences.{index}.txt");

            writer = new StreamWriter($"genedata.{index}.txt");
            writer.WriteLine("Кулак Матвей");
            writer.WriteLine("Генетический поиск");

            ReadHandleCommands($"commands.{index}.txt");

            writer.Close();

            Console.WriteLine($"Результаты записаны в genedata.{index}.txt");
        }
    }
}