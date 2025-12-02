using Lab5.Services;
using Lab5.Menu;

namespace Lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            const string configFileName = "sweets_config.json";

            try
            {
                var sweetFactory = new SweetFactory(configFileName);

                var giftManager = new GiftManager(sweetFactory);

                giftManager.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nПроизошла критическая ошибка, приложение будет закрыто.");
                Console.WriteLine($"Подробности: {ex.Message}");
            }
        }
    }
}