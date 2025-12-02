namespace Lab2
{
    enum State
    {
        Winner,
        Loser,
        Playing,
        NotInGame
    }
    class Player
    {
        public string Name { get; }
        public int Location { get; private set; }
        public State State { get; private set; } = State.NotInGame;
        public int DistanceTraveled { get; private set; } = 0;

        private int size;

        public Player(string name, int size)
        {
            Name = name;
            this.size = size;
            Location = -1;
        }

        public void SetPosition(int pos)
        {
            Location = Normalize(pos);
            State = State.Playing;
        }

        public void Move(int steps)
        {
            if (State == State.NotInGame) 
                return;

            Location = Normalize(Location + steps);
            DistanceTraveled += Math.Abs(steps);
        }

        private int Normalize(int pos)
        {
            return ((pos - 1) % size + size) % size + 1;
        }
    }
}