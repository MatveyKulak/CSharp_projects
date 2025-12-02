using System.Text.Json;

namespace Lab5.Services.Creation
{
    public class SweetConfigEntry
    {
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public double weight { get; set; }
        public double sugarContent { get; set; }

        public object uniqueParameter { get; set; }
    }
}