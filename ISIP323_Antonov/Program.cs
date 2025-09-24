using System;
using System.Collections.Generic;

class Program
{
    static List<string> allStatistics = new List<string>();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Введите текст (минимум 100 символов): ");
            string userText = Console.ReadLine();

            if (userText.Length < 100)
            {
                Console.WriteLine("Ошибка! Текст слишком короткий.");
                continue;
            }

            char[] separators = { ' ', ',', '.', '!', '?', ';', ':', '-', '\n', '\r' };
            string[] words = userText.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int wordCount = words.Length;

            string shortestWord = words[0];
            string longestWord = words[0];
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length < shortestWord.Length) shortestWord = words[i];
                if (words[i].Length > longestWord.Length) longestWord = words[i];
            }

            int sentenceCount = 0;
            for (int i = 0; i < userText.Length; i++)
            {
                char c = userText[i];
                if (c == '.' || c == '!' || c == '?')
                {
                    sentenceCount++;
                }
            }

            string vowels = "аеёиоуыэюяАЕЁИОУЫЭЮЯaeiouyAEIOUY";
            int vowelCount = 0;
            int consonantCount = 0;

            for (int i = 0; i < userText.Length; i++)
            {
                char ch = userText[i];
                if (Char.IsLetter(ch))
                {
                    if (vowels.IndexOf(ch) != -1) vowelCount++;
                    else consonantCount++;
                }
            }

            char[] letters = new char[1000];
            int[] counts = new int[1000];
            int uniqueCount = 0;

            for (int i = 0; i < userText.Length; i++)
            {
                char ch = Char.ToLower(userText[i]);
                if (Char.IsLetter(ch))
                {
                    bool found = false;
                    for (int j = 0; j < uniqueCount; j++)
                    {
                        if (letters[j] == ch)
                        {
                            counts[j]++;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        letters[uniqueCount] = ch;
                        counts[uniqueCount] = 1;
                        uniqueCount++;
                    }
                }
            }

            string statistics = "";
            statistics += "Количество слов: " + wordCount + "\n";
            statistics += "Самое короткое слово: " + shortestWord + "\n";
            statistics += "Самое длинное слово: " + longestWord + "\n";
            statistics += "Количество предложений: " + sentenceCount + "\n";
            statistics += "Гласные: " + vowelCount + "\n";
            statistics += "Согласные: " + consonantCount + "\n";
            statistics += "Частота букв:\n";

            for (int i = 0; i < uniqueCount; i++)
            {
                statistics += letters[i] + " = " + counts[i] + "\n";
            }

            Console.WriteLine("\nРезультаты анализа:");
            Console.WriteLine(statistics);

            allStatistics.Add(statistics);

            Console.WriteLine("Хотите ввести новый текст? (да/нет)");
            string answer = Console.ReadLine();
            if (answer.ToLower() != "да")
            {
                Console.WriteLine("Хотите вывести статистику по прошлым текстам? (да/нет)");
                string hist = Console.ReadLine();
                if (hist.ToLower() == "да")
                {
                    Console.WriteLine("\nВсе сохраненные результаты:\n");
                    for (int i = 0; i < allStatistics.Count; i++)
                    {
                        Console.WriteLine("Текст #" + (i + 1));
                        Console.WriteLine(allStatistics[i]);
                    }
                }
                break;
            }
        }
    }
}
