namespace Lab5.Models
{
    public class Waffle : Sweet
    {
        public bool IsGlazed { get; protected set; }

        public Waffle(string id, string name, double weight, double sugarContent, bool isGlazed)
            : base(id, name, weight, sugarContent)
        {
            IsGlazed = isGlazed;
        }

        public override string GetDescription()
        {
            string glazeInfo = IsGlazed ? "в глазури" : "без глазури";
            return $"{base.GetDescription()}, Тип: {glazeInfo}";
        }
    }
}