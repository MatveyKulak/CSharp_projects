using Lab5.Models;
using Lab5.Services.Creation;
using System.Text.Json;

namespace Lab5.Services
{
    public class SweetFactory
    {
        private const string ChocolateCandyType = "ChocolateCandy";
        private const string LollipopType = "Lollipop";
        private const string WaffleType = "Waffle";

        private readonly string _configPath;
        private List<Sweet> _sweetsAssortment;

        public SweetFactory(string configPath)
        {
            _configPath = configPath;
            _sweetsAssortment = new List<Sweet>();
            LoadSweetsFromConfig();
        }

        public List<Sweet> GetAvailableSweets()
        {
            return _sweetsAssortment;
        }

        private void LoadSweetsFromConfig()
        {
            _sweetsAssortment.Clear();

            if (!File.Exists(_configPath))
            {
                Console.WriteLine($"Ошибка: Файл конфигурации '{_configPath}' не найден.");
                return;
            }

            string jsonString = File.ReadAllText(_configPath);
            try
            {
                var configEntries = JsonSerializer.Deserialize<List<SweetConfigEntry>>(jsonString);
                foreach (var entry in configEntries)
                {
                    try
                    {
                        _sweetsAssortment.Add(CreateSweetFromEntry(entry));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Предупреждение: Не удалось создать сладость '{entry.name}'. {ex.Message}");
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Ошибка: Не удалось прочитать JSON файл. Проверьте синтаксис. {ex.Message}");
            }
        }

        private Sweet CreateSweetFromEntry(SweetConfigEntry entry)
        {
            string uniqueParamString = entry.uniqueParameter?.ToString();

            if (entry == null || string.IsNullOrWhiteSpace(entry.type))
            {
                throw new ArgumentException("Запись в конфигурации не содержит тип сладости.");
            }

            switch (entry.type)
            {
                case ChocolateCandyType:
                    return new ChocolateCandy(entry.id, entry.name, entry.weight, entry.sugarContent, uniqueParamString);
                case LollipopType:
                    return new Lollipop(entry.id, entry.name, entry.weight, entry.sugarContent, uniqueParamString);
                case WaffleType:
                    bool.TryParse(uniqueParamString, out bool isGlazed);
                    return new Waffle(entry.id, entry.name, entry.weight, entry.sugarContent, isGlazed);
                default:
                    throw new ArgumentException($"Неизвестный тип сладости: '{entry.type}'");
            }
        }

        public void AddSweetToAssortmentAndSave(Sweet newSweet)
        {
            _sweetsAssortment.Add(newSweet);
            SaveChangesToFile();
        }

        private void SaveChangesToFile()
        {
            var configEntries = new List<SweetConfigEntry>();
            foreach (var sweet in _sweetsAssortment)
            {
                object uniqueParam = null;
                string typeName = "";

                if (sweet is ChocolateCandy choco)
                {
                    typeName = ChocolateCandyType;
                    uniqueParam = choco.Filling;
                }
                else if (sweet is Lollipop lolly)
                {
                    typeName = LollipopType;
                    uniqueParam = lolly.Flavor;
                }
                else if (sweet is Waffle waffle)
                {
                    typeName = WaffleType;
                    uniqueParam = waffle.IsGlazed;
                }

                configEntries.Add(new SweetConfigEntry
                {
                    id = sweet.Id,
                    type = typeName,
                    name = sweet.Name,
                    weight = sweet.Weight,
                    sugarContent = sweet.SugarContent,
                    uniqueParameter = uniqueParam
                });
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(configEntries, options);
            File.WriteAllText(_configPath, jsonString);
        }
    }
}