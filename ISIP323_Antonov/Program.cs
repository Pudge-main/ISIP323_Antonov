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

            int sentenceCount = 0;
            for (int i = 0; i < userText.Length; i++)
            {
                char c = userText[i];
                if (c == '.' || c == '!' || c == '?')
                {
                    sentenceCount++;
                }
            }

            Console.WriteLine("Количество слов: " + wordCount);
            Console.WriteLine("Количество предложений: " + sentenceCount);
            break;
        }
    }
}
