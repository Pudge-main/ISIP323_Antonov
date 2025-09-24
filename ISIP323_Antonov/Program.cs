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

            Console.WriteLine("Текст принят!");
            break;
        }
    }
}
