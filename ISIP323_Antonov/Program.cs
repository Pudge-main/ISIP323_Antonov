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



}