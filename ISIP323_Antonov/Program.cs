using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreApp
{
    public enum ProductCategory
    {
        Food = 0,
        Electronics = 1,
        Clothes = 2
    }

    public class Product
    {
        private static int _nextCode = 1;

        public int Code { get; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool InStock => Quantity > 0;
        public ProductCategory Category { get; set; }

        public Product(string name, decimal price, int quantity, ProductCategory category)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Название товара не может быть пустым.");
            if (price <= 0) throw new ArgumentException("Цена должна быть положительной.");
            if (quantity < 0) throw new ArgumentException("Количество не может быть отрицательным.");

            Code = _nextCode++;
            Name = name.Trim();
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Код: {Code} | Название: {Name} | Цена: {Price:F2} руб. | Кол-во: {Quantity} | В наличии: {(InStock ? "Да" : "Нет")} | Категория: {Category}");
        }

        public override string ToString() => $"{Code}: {Name} ({Category}) — {Price:F2} руб — {Quantity} шт";
    }

    class Program
    {
        static List<Product> products = new List<Product>();
   
        static Stack<(int Code, string Name, int Quantity, decimal Sum)> salesHistory = new Stack<(int, string, int, decimal)>();

        static void Main(string[] args)
        {
            SeedTestProducts();
            RunMainLoop();
        }

        static void SeedTestProducts()
        {
            products.Add(new Product("Хлеб", 25.5m, 50, ProductCategory.Food));
            products.Add(new Product("Молоко 1л", 65.0m, 30, ProductCategory.Food));
            products.Add(new Product("Наушники", 1999.99m, 10, ProductCategory.Electronics));
            products.Add(new Product("Футболка", 499.0m, 20, ProductCategory.Clothes));
            products.Add(new Product("Телефон", 25999.0m, 5, ProductCategory.Electronics));
        }

        static void RunMainLoop()
        {
            while (true)
            {
                Console.WriteLine("\n=== Учет товаров: меню ===");
                Console.WriteLine("1 - Показать все товары");
                Console.WriteLine("2 - Добавить товар");
                Console.WriteLine("3 - Удалить товар");
                Console.WriteLine("4 - Заказать поставку (пополнить количество)");
                Console.WriteLine("5 - Продать товар");
                Console.WriteLine("6 - Поиск товаров (по коду, названию, категории)");
                Console.WriteLine("7 - История продаж (и отмена последней продажи)");
                Console.WriteLine("8 - Отчёт о продажах");
                Console.WriteLine("0 - Выход");
                Console.Write("Выберите команду: ");

                var cmd = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (cmd)
                {
                    case "1": PrintAllProducts(); break;
                    case "2": AddProduct(); break;
                    case "3": RemoveProduct(); break;
                    case "4": OrderSupply(); break;
                    case "5": SellProduct(); break;
                    case "6": SearchProducts(); break;
                    case "7": ShowSalesHistoryAndUndo(); break;
                    case "8": PrintSalesReport(); break;
                    case "0": return;
                    default: Console.WriteLine("Неверная команда. Попробуйте снова."); break;
                }
            }
        }

        static void PrintAllProducts()
        {
            if (!products.Any()) { Console.WriteLine("Список товаров пуст."); return; }
            foreach (var p in products) p.PrintInfo();
        }

        static void AddProduct()
        {
            try
            {
                var name = ReadNonEmptyString("Название: ");
                var price = ReadPositiveDecimal("Цена: ");
                var qty = ReadNonNegativeInt("Количество: ");
                var category = ReadCategory();
                var prod = new Product(name, price, qty, category);
                products.Add(prod);
                Console.WriteLine("Товар добавлен:");
                prod.PrintInfo();
            }
            catch (ArgumentException ex) { Console.WriteLine("Ошибка: " + ex.Message); }
        }

        static void RemoveProduct()
        {
            int code = ReadPositiveInt("Введите код товара для удаления: ");
            var p = products.FirstOrDefault(x => x.Code == code);
            if (p == null) { Console.WriteLine("Товар не найден."); return; }
            Console.Write($"Подтвердите удаление товара \"{p.Name}\" (y/n): ");
            if ((Console.ReadLine() ?? "").ToLower() == "y")
            {
                products.Remove(p);
                Console.WriteLine("Товар удалён.");
            }
            else Console.WriteLine("Отмена.");
        }

        static void OrderSupply()
        {
            int code = ReadPositiveInt("Код товара для поставки: ");
            var p = products.FirstOrDefault(x => x.Code == code);
            if (p == null) { Console.WriteLine("Товар не найден."); return; }
            int add = ReadPositiveInt("Добавить количество: ");
            p.Quantity += add;
            Console.WriteLine($"Поставка учтена. Текущее количество: {p.Quantity}");
        }

        static void SellProduct()
        {
            int code = ReadPositiveInt("Код товара для продажи: ");
            var p = products.FirstOrDefault(x => x.Code == code);
            if (p == null) { Console.WriteLine("Товар не найден."); return; }
            int sell = ReadPositiveInt("Сколько штук продать: ");
            if (sell > p.Quantity) { Console.WriteLine($"Недостаточно на складе. Доступно: {p.Quantity}"); return; }
            p.Quantity -= sell;
            var sum = sell * p.Price;
            salesHistory.Push((p.Code, p.Name, sell, sum));
            Console.WriteLine($"Продано: {sell} шт. Сумма: {sum:F2} руб. Осталось: {p.Quantity}");
        }

        static void SearchProducts()
        {
            Console.WriteLine("1 - По коду; 2 - По названию; 3 - По категории");
            var mode = Console.ReadLine()?.Trim();
            if (mode == "1")
            {
                int code = ReadPositiveInt("Код: ");
                var p = products.FirstOrDefault(x => x.Code == code);
                if (p == null) Console.WriteLine("Не найдено.");
                else p.PrintInfo();
            }
            else if (mode == "2")
            {
                Console.Write("Введите часть названия: ");
                var q = (Console.ReadLine() ?? "").Trim();
                if (string.IsNullOrEmpty(q)) { Console.WriteLine("Запрос пустой."); return; }
                var res = products.Where(x => x.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                if (!res.Any()) Console.WriteLine("Ничего не найдено.");
                else res.ForEach(x => x.PrintInfo());
            }
            else if (mode == "3")
            {
                var cat = ReadCategory();
                var res = products.Where(x => x.Category == cat).ToList();
                if (!res.Any()) Console.WriteLine("Ничего не найдено.");
                else res.ForEach(x => x.PrintInfo());
            }
            else Console.WriteLine("Неверный режим поиска.");
        }

        static void ShowSalesHistoryAndUndo()
        {
            if (!salesHistory.Any()) { Console.WriteLine("История продаж пуста."); return; }
            Console.WriteLine("История продаж (последняя сверху):");
            foreach (var s in salesHistory) Console.WriteLine($"Код {s.Code} | {s.Name} | Количество: {s.Quantity} | Сумма: {s.Sum:F2}");
            Console.Write("Отменить последнюю продажу? (y/n): ");
            if ((Console.ReadLine() ?? "").ToLower() == "y")
            {
                var last = salesHistory.Pop();
                var prod = products.FirstOrDefault(x => x.Code == last.Code);
                if (prod != null) prod.Quantity += last.Quantity;
                Console.WriteLine($"Последняя продажа отменена. Возвращено {last.Quantity} шт товара \"{last.Name}\".");
            }
        }

        static void PrintSalesReport()
        {
            if (!salesHistory.Any()) { Console.WriteLine("Продаж нет."); return; }
            var grouped = salesHistory
                          .GroupBy(s => s.Code)
                          .Select(g => new { Code = g.Key, Name = g.First().Name, TotalQty = g.Sum(x => x.Quantity), TotalSum = g.Sum(x => x.Sum) })
                          .ToList();

            Console.WriteLine("=== Отчёт о продажах ===");
            decimal grandTotal = 0;
            foreach (var item in grouped)
            {
                Console.WriteLine($"Код {item.Code} | {item.Name} | Продано: {item.TotalQty} шт | Сумма: {item.TotalSum:F2} руб");
                grandTotal += item.TotalSum;
            }
            Console.WriteLine($"Итого по всем продажам: {grandTotal:F2} руб");
        }

        static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.WriteLine("Значение не может быть пустым!");
            }
        }

        static decimal ReadPositiveDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (decimal.TryParse(s, out var v) && v > 0) return v;
                Console.WriteLine("Введите корректную положительную цену (число).");
            }
        }

        static int ReadNonNegativeInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out var v) && v >= 0) return v;
                Console.WriteLine("Введите целое число >= 0.");
            }
        }

        static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out var v) && v > 0) return v;
                Console.WriteLine("Введите целое число > 0.");
            }
        }

        static ProductCategory ReadCategory()
        {
            Console.WriteLine("Доступные категории:");
            foreach (var val in Enum.GetValues(typeof(ProductCategory)))
                Console.WriteLine($"{(int)val} - {val}");
            while (true)
            {
                Console.Write("Введите номер категории: ");
                var s = Console.ReadLine();
                if (int.TryParse(s, out var i) && Enum.IsDefined(typeof(ProductCategory), i)) return (ProductCategory)i;
                Console.WriteLine("Неверный выбор категории.");
            }
        }
    }
}