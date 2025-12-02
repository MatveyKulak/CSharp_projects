using Lab5.Models;
using Lab5.Services;
using Lab5.Services.Logic.Interfaces;
using Lab5.Services.Logic.Realisations;
using System.Text.Json;

public class GiftConfigEntry
{
    public string name { get; set; }
    public List<string> sweetIds { get; set; }
}

namespace Lab5.Menu
{
    public class GiftManager
    {
        private readonly SweetFactory _sweetFactory;
        private List<Gift> _gifts;
        private List<Sweet> _availableSweets;

        public GiftManager(SweetFactory sweetFactory)
        {
            _sweetFactory = sweetFactory;
            _gifts = new List<Gift>();
            _availableSweets = _sweetFactory.GetAvailableSweets();
            LoadInitialGifts();
        }

        private void LoadInitialGifts()
        {
            const string giftsConfigPath = "gifts_config.json";
            var sweetsById = _availableSweets.ToDictionary(sweet => sweet.Id);
            if (!File.Exists(giftsConfigPath) || !sweetsById.Any()) return;
            string jsonString = File.ReadAllText(giftsConfigPath);
            var giftConfigs = JsonSerializer.Deserialize<List<GiftConfigEntry>>(jsonString);
            foreach (var config in giftConfigs)
            {
                var newGift = new Gift(config.name);
                foreach (var sweetId in config.sweetIds)
                {
                    if (sweetsById.TryGetValue(sweetId, out Sweet sweetToAdd))
                    {
                        newGift.AddSweet(sweetToAdd);
                    }
                }
                _gifts.Add(newGift);
            }
        }

        public void Run()
        {
            Console.WriteLine("Конструктор Новогодних Подарков");
            bool isRunning = true;
            while (isRunning)
            {
                ShowMainMenu();
                switch (Console.ReadLine())
                {
                    case "1": ViewGiftsMenu(); break;
                    case "2": AddGift(); break;
                    case "3": DeleteGift(); break;
                    case "0": isRunning = false; break;
                    default: Console.WriteLine("Неверный ввод."); break;
                }
            }
        }

        private void ShowMainMenu()
        {
            Console.WriteLine("\nГлавное Меню");
            Console.WriteLine("1. Просмотреть подарки");
            Console.WriteLine("2. Добавить подарок");
            Console.WriteLine("3. Удалить подарок");
            Console.WriteLine("0. Выход");
            Console.Write("Ваш выбор: ");
        }

        private void AddGift()
        {
            Console.WriteLine("\nСоздание нового подарка");
            Console.Write("Введите имя для нового подарка: ");
            string giftName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(giftName))
            {
                Console.WriteLine("Имя не может быть пустым. Создание отменено.");
                return;
            }
            var newGift = new Gift(giftName);
            _gifts.Add(newGift);
            Console.WriteLine($"Пустой подарок '{giftName}' создан. Теперь вы можете управлять им из главного меню.");
            SaveChangesToFile();
        }

        private void ViewGiftsMenu()
        {
            Console.WriteLine("\nСписок доступных подарков");
            if (!_gifts.Any())
            {
                Console.WriteLine("У вас пока нет ни одного подарка.");
                return;
            }
            PrintNumberedList(_gifts);
            int giftIndex = GetUserChoice(_gifts.Count, "Введите номер подарка для управления");
            if (giftIndex != -1)
            {
                ManageSelectedGift(_gifts[giftIndex]);
            }
        }

        private void DeleteGift()
        {
            Console.WriteLine("\nУдаление подарка");
            if (!_gifts.Any())
            {
                Console.WriteLine("У вас пока нет ни одного подарка для удаления.");
                return;
            }
            PrintNumberedList(_gifts);
            int giftIndex = GetUserChoice(_gifts.Count, "Введите номер подарка для удаления");
            if (giftIndex != -1)
            {
                _gifts.RemoveAt(giftIndex);
                Console.WriteLine("Подарок успешно удален.");
                SaveChangesToFile();
            }
        }

        private void ManageSelectedGift(Gift gift)
        {
            bool inGiftMenu = true;
            while (inGiftMenu)
            {
                Console.Clear();
                Console.WriteLine($"Управление подарком: '{gift.Name}'");
                ShowGiftContents(gift);
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Добавить конфету");
                Console.WriteLine("2. Редактировать конфету (заменить)");
                Console.WriteLine("3. Удалить конфету");
                Console.WriteLine("4. Отсортировать");
                Console.WriteLine("5. Найти по сахару");
                Console.WriteLine("0. Вернуться в главное меню");
                Console.Write("Ваш выбор: ");
                switch (Console.ReadLine())
                {
                    case "1": AddSweetToGift(gift); break;
                    case "2": EditSweetInGift(gift); break;
                    case "3": RemoveSweetFromGift(gift); break;
                    case "4": SortGiftMenu(gift); break;
                    case "5": FindSweetsMenu(gift); break;
                    case "0": inGiftMenu = false; break;
                    default: Console.WriteLine("Неверный ввод."); break;
                }
                if (inGiftMenu)
                {
                    Console.WriteLine("\nНажмите Enter для продолжения");
                    Console.ReadLine();
                }
            }
        }

        private void ShowGiftContents(Gift gift)
        {
            Console.WriteLine("\nСостав Подарка:");
            var sweets = gift.GetSweets();
            if (!sweets.Any())
            {
                Console.WriteLine("(пока пусто)");
            }
            else
            {
                var groupedSweets = sweets.GroupBy(sweet => sweet.Id);
                foreach (var group in groupedSweets)
                {
                    var firstSweetInGroup = group.First();
                    var count = group.Count();
                    if (count > 1)
                    {
                        Console.WriteLine($"- {firstSweetInGroup.GetDescription()} (x{count})");
                    }
                    else
                    {
                        Console.WriteLine($"- {firstSweetInGroup.GetDescription()}");
                    }
                }
            }
            Console.WriteLine($"\n{gift.ToString()}");
        }

        private void AddSweetToGift(Gift gift)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Добавить из существующего ассортимента");
            Console.WriteLine("2. Создать новую конфету и добавить");
            Console.Write("Ваш выбор (0 для отмены): ");
            switch (Console.ReadLine())
            {
                case "1":
                    AddExistingSweet(gift);
                    break;
                case "2":
                    AddNewSweet(gift);
                    break;
                default:
                    return;
            }
        }

        private void AddExistingSweet(Gift gift)
        {
            Console.WriteLine("\nДоступные сладости для добавления:");
            PrintNumberedList(_availableSweets);
            int choice = GetUserChoice(_availableSweets.Count, "Введите номер конфеты для добавления");
            if (choice != -1)
            {
                var selectedSweet = _availableSweets[choice];
                Console.Write($"Сколько штук '{selectedSweet.Name}' вы хотите добавить? ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        gift.AddSweet(selectedSweet);
                    }
                    Console.WriteLine($"{quantity} шт. '{selectedSweet.Name}' добавлено в подарок.");
                    SaveChangesToFile();
                }
                else
                {
                    Console.WriteLine("Неверное количество. Добавление отменено.");
                }
            }
        }

        private void AddNewSweet(Gift gift)
        {
            Sweet newSweet = CreateNewSweetWizard();
            if (newSweet != null)
            {
                gift.AddSweet(newSweet);
                Console.WriteLine($"Новая конфета '{newSweet.Name}' создана и добавлена в подарок.");
                SaveChangesToFile();
            }
        }

        private Sweet CreateNewSweetWizard()
        {
            try
            {
                Console.WriteLine("\nДобавление новой конфеты в ассортимент");

                Console.WriteLine("Выберите тип конфеты: 1. Шоколадная, 2. Леденец, 3. Вафля");
                string typeChoice = Console.ReadLine();
                string typeName = "";
                switch (typeChoice)
                {
                    case "1": typeName = "ChocolateCandy"; break;
                    case "2": typeName = "Lollipop"; break;
                    case "3": typeName = "Waffle"; break;
                    default: Console.WriteLine("Неверный тип."); return null;
                }

                Console.Write("Введите название: ");
                string name = Console.ReadLine();

                Console.Write("Введите вес (в граммах): ");
                if (!double.TryParse(Console.ReadLine(), out double weight))
                {
                    Console.WriteLine("Ошибка! Неверный формат веса."); return null;
                }

                Console.Write("Введите содержание сахара (г/100г): ");
                if (!double.TryParse(Console.ReadLine(), out double sugar))
                {
                    Console.WriteLine("Ошибка! Неверный формат сахара."); return null;
                }

                string id = $"{typeName.ToLower().Substring(0, 2)}_{name.Replace(" ", "").ToLower()}_{DateTime.Now.Ticks}";
                Sweet newSweet = null;
                switch (typeName)
                {
                    case "ChocolateCandy":
                        Console.Write("Введите тип начинки: ");
                        string filling = Console.ReadLine();
                        newSweet = new ChocolateCandy(id, name, weight, sugar, filling);
                        break;
                    case "Lollipop":
                        Console.Write("Введите вкус: ");
                        string flavor = Console.ReadLine();
                        newSweet = new Lollipop(id, name, weight, sugar, flavor);
                        break;
                    case "Waffle":
                        Console.Write("Покрыта ли глазурью? (да/нет): ");
                        bool isGlazed = Console.ReadLine().ToLower() == "да";
                        newSweet = new Waffle(id, name, weight, sugar, isGlazed);
                        break;
                }

                if (newSweet != null)
                {
                    _sweetFactory.AddSweetToAssortmentAndSave(newSweet);
                    _availableSweets = _sweetFactory.GetAvailableSweets();
                    Console.WriteLine($"Конфета '{name}' успешно добавлена в общий ассортимент.");
                    return newSweet;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
            return null;
        }

        private void EditSweetInGift(Gift gift)
        {
            var currentSweets = gift.GetSweets().ToList();
            if (!currentSweets.Any()) { Console.WriteLine("В подарке нет конфет для замены."); return; }
            Console.WriteLine("\nКакую конфету вы хотите заменить?");
            PrintNumberedList(currentSweets);
            int sweetToEditIndex = GetUserChoice(currentSweets.Count, "Введите номер конфеты для замены");
            if (sweetToEditIndex != -1)
            {
                Console.WriteLine("\nНа какую конфету из ассортимента заменить?");
                PrintNumberedList(_availableSweets);
                int newSweetIndex = GetUserChoice(_availableSweets.Count, "Введите номер новой конфеты");
                if (newSweetIndex != -1)
                {
                    gift.ReplaceSweetAt(sweetToEditIndex, _availableSweets[newSweetIndex]);
                    Console.WriteLine("Конфета заменена.");
                    SaveChangesToFile();
                }
            }
        }

        private void RemoveSweetFromGift(Gift gift)
        {
            var currentSweets = gift.GetSweets().ToList();
            if (!currentSweets.Any()) { Console.WriteLine("В подарке нет конфет для удаления."); return; }
            Console.WriteLine("\nКакую конфету вы хотите удалить?");
            PrintNumberedList(currentSweets);
            int choice = GetUserChoice(currentSweets.Count, "Введите номер конфеты для удаления");
            if (choice != -1)
            {
                gift.RemoveSweetAt(choice);
                Console.WriteLine("Конфета удалена.");
                SaveChangesToFile();
            }
        }

        private void SortGiftMenu(Gift gift)
        {
            Console.WriteLine("\nСортировка");
            Console.WriteLine("1. Сортировать по весу");
            Console.WriteLine("2. Сортировать по содержанию сахара");
            Console.Write("Ваш выбор: ");
            ISortable sorter = null;
            switch (Console.ReadLine())
            {
                case "1": sorter = new SortByWeight(); break;
                case "2": sorter = new SortBySugarContent(); break;
                default: Console.WriteLine("Неверный выбор."); return;
            }
            gift.SortSweets(sorter);
            Console.WriteLine("Подарок отсортирован!");
            SaveChangesToFile();
        }

        private void FindSweetsMenu(Gift gift)
        {
            Console.WriteLine("\nПоиск по сахару");
            Console.Write("Введите минимальное содержание сахара: ");
            if (!double.TryParse(Console.ReadLine(), out double minSugar))
            {
                Console.WriteLine("Ошибка! Введено не число."); return;
            }
            Console.Write("Введите максимальное содержание сахара: ");
            if (!double.TryParse(Console.ReadLine(), out double maxSugar))
            {
                Console.WriteLine("Ошибка! Введено не число."); return;
            }
            try
            {
                IFilterable filter = new SugarRangeFilter(minSugar, maxSugar);
                var foundSweets = gift.FindSweets(filter);
                Console.WriteLine("\nНайденные конфеты");
                if (!foundSweets.Any())
                {
                    Console.WriteLine("Конфеты в заданном диапазоне не найдены.");
                }
                else
                {
                    foreach (var sweet in foundSweets)
                    {
                        Console.WriteLine(sweet.GetDescription());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        private void PrintNumberedList<T>(List<T> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {items[i].ToString()}");
            }
        }

        private int GetUserChoice(int maxChoice, string prompt)
        {
            while (true)
            {
                Console.Write($"\n{prompt} (или 0 для отмены): ");
                string input = Console.ReadLine();
                if (input == "0")
                {
                    return -1;
                }
                if (int.TryParse(input, out int choice) && choice > 0 && choice <= maxChoice)
                {
                    return choice - 1;
                }
                Console.WriteLine("Неверный ввод. Пожалуйста, попробуйте еще раз.");
            }
        }

        private void SaveChangesToFile()
        {
            const string giftsConfigPath = "gifts_config.json";
            var giftConfigs = new List<GiftConfigEntry>();
            foreach (var gift in _gifts)
            {
                giftConfigs.Add(new GiftConfigEntry
                {
                    name = gift.Name,
                    sweetIds = gift.GetSweets().Select(sweet => sweet.Id).ToList()
                });
            }
            var options = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            string jsonString = JsonSerializer.Serialize(giftConfigs, options);
            try
            {
                File.WriteAllText(giftsConfigPath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка! Не удалось сохранить изменения в файл: {ex.Message}");
            }
        }
    }
}