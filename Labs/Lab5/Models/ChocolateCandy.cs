namespace Lab5.Models
{
    public class ChocolateCandy : Sweet
    {
        public string Filling { get; protected set; }

        public ChocolateCandy(string id, string name, double weight, double sugarContent, string filling)
            : base(id, name, weight, sugarContent)
        {
            if (string.IsNullOrWhiteSpace(filling))
            {
                Filling = "без начинки";
            }
            else
            {
                Filling = filling;
            }
        }

        public override string GetDescription()
        {
            string baseDescription = base.GetDescription();
            return $"{baseDescription}, Начинка: {Filling}";
        }
    }
}