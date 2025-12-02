namespace Lesson19
{
    public class Program
    {
        public static void Main()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Выберите тему для отображения информации:");
                Console.Write("Введите номер: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Lesson_19.Events.EventHandlerDemo.HandleTasks();
                        Lesson_19.Events.EventHandlerDemo.HandleStorePurchases();
                        Lesson_19.Events.EventHandlerDemo.HandleButtonClicks();
                        Lesson_19.Events.EventHandlerDemo.HandleProductPriceChange();
                        Lesson_19.Events.EventHandlerDemo.HandleFileDownload();
                        break;
                    case "5":
                        exit = true;
                        Console.WriteLine("Выход из программы...");
                        break;
                    default:
                        Console.WriteLine("Некорректный ввод, попробуйте снова.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }
    }
}
