namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Введите номер входного файла (1, 2 или 3): ");
                string? choice = Console.ReadLine();

                if (choice == "oleg")
                    return;

                Game.InputFile = $"{choice}.ChaseData.txt";
                Game.OutFile = $"{choice}.PursuitLog.txt";

                if (!File.Exists(Game.InputFile))
                {
                    Console.WriteLine($"Файл {Game.InputFile} не найден");
                    return;
                }

                string[] lines = File.ReadAllLines(Game.InputFile);

                int size = int.Parse(lines[0].Trim());

                Game game = new Game(size);
                game.Run(lines);
                
                Console.WriteLine($"Результат записан в {Game.OutFile}");
            }
        }
    }
}