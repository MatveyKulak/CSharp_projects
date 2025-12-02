namespace Lab2
{
    enum GameState
    {
        Start,
        End
    }

    class Game
    {
        private readonly int size;
        private Player cat;
        private Player mouse;
        private GameState state;

        public static string InputFile;
        public static string OutFile;

        public Game(int size)
        {
            this.size = size;
            cat = new Player("Cat", size);
            mouse = new Player("Mouse", size);
            state = GameState.Start;
        }

        public void Run(string[] lines)
        {
            List<string> output = new List<string>();

            output.Add("Cat and Mouse \n\n"
                    + "Cat Mouse  Distance\n"
                    + "-------------------");

            for (int i = 1; i < lines.Length && state != GameState.End; i++)
            {
                string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                    continue;

                char cmd = parts[0][0];

                switch (cmd)
                {
                    case 'M':
                        if (parts.Length < 2)
                            continue;

                        int mouseValue = int.Parse(parts[1]);

                        if (mouse.State == State.NotInGame)
                            mouse.SetPosition(mouseValue);
                        else
                            mouse.Move(mouseValue);

                        break;
                        // дублирование кода(отдельный метод)
                    case 'C':
                        if (parts.Length < 2)
                            continue;

                        int catValue = int.Parse(parts[1]);

                        if (cat.State == State.NotInGame)
                            cat.SetPosition(catValue);
                        else
                            cat.Move(catValue);

                        break;

                    case 'P':
                        DoPrintCommand(output);
                        break;
                }

                if (cat.State == State.Playing && mouse.State == State.Playing &&
                    cat.Location == mouse.Location)
                {
                    state = GameState.End;
                }
            }

            output.Add("-------------------"); //через +
            output.Add("");
            output.Add("Distance traveled:   Mouse    Cat");
            output.Add($"{mouse.DistanceTraveled,28}{cat.DistanceTraveled,7}");
            output.Add("");

            if (cat.Location == mouse.Location &&
                cat.State != State.NotInGame &&
                mouse.State != State.NotInGame)
            {
                output.Add($"Mouse caught at: {cat.Location}");
            }
            else
            {
                output.Add("Mouse evaded Cat");
            }

            File.WriteAllLines(OutFile, output);
        }
        private void DoPrintCommand(List<string> output)
        {
            string catPos = cat.State == State.NotInGame ? "??" : cat.Location.ToString();
            string mousePos = mouse.State == State.NotInGame ? "??" : mouse.Location.ToString();
            string distance = (cat.State == State.NotInGame || mouse.State == State.NotInGame)
                ? ""
                : GetDistance().ToString();

            output.Add($"{catPos}\t{mousePos}\t{distance}"); ;
        }

        private int GetDistance()
        {
            if (cat.State == State.NotInGame || mouse.State == State.NotInGame)
                return 0;

            return Math.Abs(cat.Location - mouse.Location);
        }
    }
}