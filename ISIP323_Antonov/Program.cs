using System;

class Program
{
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

            Console.WriteLine("Количество слов: " + wordCount);
            Console.WriteLine("Самое короткое слово: " + shortestWord);
            Console.WriteLine("Самое длинное слово: " + longestWord);
            Console.WriteLine("Количество предложений: " + sentenceCount);
            Console.WriteLine("Гласные: " + vowelCount);
            Console.WriteLine("Согласные: " + consonantCount);
            break;
        }
    }
}
