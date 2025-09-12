﻿Console.Write("Введите число операций: ");
string C = Console.ReadLine();
int Count = Convert.ToInt32(C);

if (Count < 2 || Count > 40) Console.WriteLine("Некорректное кол-во операций!");
else
{
    double sum = 0;
    double[] summa = new double[Count];
    string[] tovar = new string[Count];
    Console.WriteLine("Введите информацию о операции по шаблону 'Название услуги или товара; Количество денег(в рублях)'");
    for (int i = 0; i < Count; i++)
    {
        Console.Write($"Операция № {i + 1}: ");
        string operation = Console.ReadLine();
        string[] operations = operation.Split(';');
        tovar[i] = operations[0];
        summa[i] = Convert.ToDouble(operations[1]);
        sum += summa[i];
    }

    void Menu()
    {
        Console.WriteLine("=======================================");
        Console.WriteLine("Меню: ");
        Console.WriteLine("Нажмите нужный номер, для выбора:");
        Console.WriteLine("1.Вывод данных");
        Console.WriteLine("2.Статистика (среднее, максимальное, минимальное, сумма)");
        Console.WriteLine("3.Сортировка по цене (пузырьковая сортировка)");
        Console.WriteLine("4.Конвертация валюты (пользователь вводит курс или выбирает из списка)");
        Console.WriteLine("5.Поиск по названию");
        Console.WriteLine("0.Выход");
        Console.WriteLine("=======================================");
        string ch = Console.ReadLine();
        int choice = Convert.ToInt32(ch);

        switch (choice)
        {
            case 1:
                for (int i = 0; i < summa.Length; i++)
                {
                    Console.Write($"|{tovar[i]} -- {summa[i]}Руб.|\n");
                }
                Menu();
                break;
            case 2:
                Console.WriteLine("Сумма всех операций: " + sum);
                Console.WriteLine($"Максимальная стоимость: {summa.Max()}");
                Console.WriteLine($"Минимальная стоимость: {summa.Min()}");
                Console.WriteLine($"Средняя стоимость: {summa.Average()}");
                Menu();
                break;
            case 3:
                for (int i = 0; i < Count - 1; i++)
                {
                    for (int j = 0; j < Count - i - 1; j++)
                    {
                        if (summa[j] > summa[j + 1])
                        {
                            // меняем местами суммы
                            double swapPrice = summa[j];
                            summa[j] = summa[j + 1];
                            summa[j + 1] = swapPrice;

                            // меняем местами названия
                            string swapInf = tovar[j];
                            tovar[j] = tovar[j + 1];
                            tovar[j + 1] = swapInf;
                        }
                    }
                }
                Console.WriteLine("\nОтсортированный список:");
                for (int i = 0; i < Count; i++)
                    Console.WriteLine($"{tovar[i]} — {summa[i]} руб.");
                Menu();
                break;
            case 4:
                Menu();
                break;
            case 5:
                Console.WriteLine("Введите название: ");
                string a = Console.ReadLine();
                a.ToLower();
                int num = 0;
                for (int i = 0; i < tovar.Length; i++)
                {
                    if (tovar[i].Contains(a))
                    {
                        Console.WriteLine(tovar[i]);
                        num++;
                    }
                }
                if (num == 0)
                {
                    Console.WriteLine("Такого нету.");
                }
                Menu();
                break;
            case 0:
                return;
            default:
                Console.WriteLine("Нету такого номера");
                Menu();
                break;
        }
    }
    Menu();

}