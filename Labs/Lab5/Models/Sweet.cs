namespace Lab5.Models
{
    public abstract class Sweet
    {
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public double Weight { get; protected set; }
        public double SugarContent { get; protected set; }

        protected Sweet(string id, string name, double weight, double sugarContent)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название не может быть пустым.", nameof(name));
            if (weight <= 0)
                throw new ArgumentOutOfRangeException(nameof(weight), "Вес должен быть положительным числом.");
            if (sugarContent < 0)
                throw new ArgumentOutOfRangeException(nameof(sugarContent), "Содержание сахара не может быть отрицательным.");

            Id = id;
            Name = name;
            Weight = weight;
            SugarContent = sugarContent;
        }

        public virtual string GetDescription()
        {
            return $"Название: '{Name}', Вес: {Weight} г, Сахар: {SugarContent} г/100г";
        }

        public override string ToString()
        {
            return GetDescription();
        }
    }
}