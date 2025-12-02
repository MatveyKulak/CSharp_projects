namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Выберите текст для обработки:");
                Console.WriteLine("1 — Русский текст (input_text.txt)");
                Console.WriteLine("2 — Английский текст (input_text1.txt)");
                Console.WriteLine("0 — Выход");
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();

                if (choice == "0") break;

                string inputFile = null;
                string stopWordsFile = null;

                switch (choice)
                {
                    case "1":
                        inputFile = "input_text.txt";
                        stopWordsFile = "stopwords_ru.txt";
                        break;
                    case "2":
                        inputFile = "input_text1.txt";
                        stopWordsFile = "stopwords_en.txt";
                        break;
                    default:
                        Console.WriteLine("Неверный выбор текста!");
                        continue;
                }

                if (!File.Exists(inputFile))
                {
                    Console.WriteLine($"Файл {inputFile} не найден!");
                    continue;
                }

                string originalText = File.ReadAllText(inputFile);
                if (string.IsNullOrWhiteSpace(originalText))
                {
                    Console.WriteLine("Файл пуст!");
                    continue;
                }

                var parser = new Parser2();
                var text = parser.Parse(originalText);

                while (true)
                {
                    Console.WriteLine("\nВыберите операцию:");
                    Console.WriteLine("1 — Сортировка предложений по количеству слов");
                    Console.WriteLine("2 — Сортировка предложений по длине");
                    Console.WriteLine("3 — Поиск слов в вопросительных предложениях по длине");
                    Console.WriteLine("4 — Удаление слов по длине (начинающихся с согласной)");
                    Console.WriteLine("5 — Замена слов в первом предложении");
                    Console.WriteLine("6 — Удаление стоп-слов (русские и английские)");
                    Console.WriteLine("7 — Экспорт текста в XML");
                    Console.WriteLine("8 — Построение конкорданса");
                    Console.WriteLine("9 — Восстановить исходный текст");
                    Console.WriteLine("0 — Выход в главное меню");
                    Console.Write("Ваш выбор: ");

                    string methodChoice = Console.ReadLine();
                    Console.WriteLine();

                    if (methodChoice == "0") break;
                    if (methodChoice == "9")
                    {
                        text = parser.Parse(originalText);
                        Console.WriteLine("Текст восстановлен!");
                        continue;
                    }

                    switch (methodChoice)
                    {
                        case "1":
                            var sortedByWords = text.SortByWordCount();
                            File.WriteAllLines("output_sort_wordcount.txt", sortedByWords.Select(sentence => sentence.ToString()));
                            Console.WriteLine("Предложения, отсортированные по количеству слов, сохранены в output_sort_wordcount.txt");
                            break;

                        case "2":
                            var sortedByLength = text.SortByLength();
                            File.WriteAllLines("output_sort_length.txt", sortedByLength.Select(sentence => sentence.ToString()));
                            Console.WriteLine("Предложения, отсортированные по длине, сохранены в output_sort_length.txt");
                            break;

                        case "3":
                            Console.Write("Введите длину слов для поиска: ");
                            if (int.TryParse(Console.ReadLine(), out int questionWordLength))
                            {
                                var foundWords = text.FindWordsInQuestionsByLength(questionWordLength);
                                File.WriteAllLines("output_question_words.txt", foundWords);
                                Console.WriteLine($"Слова длиной {questionWordLength} сохранены в output_question_words.txt");
                            }
                            break;

                        case "4":
                            Console.Write("Введите длину слов, которые нужно удалить (начинающихся с согласной): ");
                            if (int.TryParse(Console.ReadLine(), out int deleteWordLength))
                            {
                                text.DeleteWords(deleteWordLength);
                                File.WriteAllText("output_removed_words.txt", text.ToString());
                                Console.WriteLine($"Слова длиной {deleteWordLength} удалены. Результат в output_removed_words.txt");
                            }
                            break;

                        case "5":
                            Console.Write("Введите длину слов для замены в первом предложении: ");
                            if (int.TryParse(Console.ReadLine(), out int replaceLength))
                            {
                                string replacement = new string('Y', replaceLength);
                                text.ReplaceWordsInSentence(0, replaceLength, replacement);
                                File.WriteAllText("output_replaced_words.txt", text.ToString());
                                Console.WriteLine($"Слова длиной {replaceLength} заменены на '{replacement}'. Результат в output_replaced_words.txt");
                            }
                            break;

                        case "6":
                            string[] stopFiles = { "stopwords_ru.txt", "stopwords_en.txt" };
                            var stopWords = new HashSet<string>();
                            foreach (var file in stopFiles)
                            {
                                if (File.Exists(file))
                                {
                                    foreach (var word in File.ReadAllLines(file))
                                    {
                                        string clean = word.Trim().ToLower();
                                        if (!string.IsNullOrWhiteSpace(clean))
                                            stopWords.Add(clean);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Файл {file} не найден. Пропуск.");
                                }
                            }

                            if (stopWords.Count > 0)
                            {
                                text.RemoveStopWords(stopWords);
                                File.WriteAllText("output_stopwords_removed.txt", text.ToString());
                                Console.WriteLine("Стоп-слова удалены. Результат сохранён в output_stopwords_removed.txt");
                            }
                            else
                            {
                                Console.WriteLine("Нет доступных файлов стоп-слов. Пропуск.");
                            }
                            break;

                        case "7":
                            text.SaveToXmlFile("output.xml");
                            Console.WriteLine("Текст экспортирован в XML-файл output.xml");
                            break;

                        case "8":
                            var concordance = text.BuildConcordance();
                            var lines = concordance
                                .OrderBy(entry => entry.Key)
                                .Select(entry =>
                                    $"{entry.Key,-20}{entry.Value.count}: {string.Join(" ", entry.Value.lines)}"
                                );
                            File.WriteAllLines("output_concordance.txt", lines);
                            Console.WriteLine("Конкорданс сохранён в output_concordance.txt");
                            break;

                        default:
                            Console.WriteLine("Неверный выбор операции!");
                            break;
                    }
                }
            }
        }
    }
}
