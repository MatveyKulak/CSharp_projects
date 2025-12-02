namespace Lab5.Models
{
    public class Lollipop : Sweet
    {
        public string Flavor { get; protected set; }

        public Lollipop(string id, string name, double weight, double sugarContent, string flavor)
            : base(id, name, weight, sugarContent)
        {
            if (string.IsNullOrWhiteSpace(flavor))
            {
                throw new ArgumentException("Вкус не может быть пустым.", nameof(flavor));
            }
            Flavor = flavor;
        }

        public override string GetDescription()
        {
            return $"{base.GetDescription()}, Вкус: {Flavor}";
        }
    }
}