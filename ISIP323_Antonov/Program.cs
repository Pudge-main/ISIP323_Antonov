using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoServiceGame
{
    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }

        public Part() { }

        public Part(string name, int price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Имя детали не может быть пустым.");
            if (price < 0) throw new ArgumentException("Цена не может быть отрицательной.");
            if (quantity < 0) throw new ArgumentException("Количество не может быть отрицательным.");

            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }

    public class Car
    {
        public int Id { get; set; }
        public string BrokenPartName { get; set; }

        public Car() { }

        public Car(string brokenPartName)
        {
            if (string.IsNullOrWhiteSpace(brokenPartName)) throw new ArgumentException("Сломанная деталь не может быть пустой.");
            BrokenPartName = brokenPartName;
        }
    }

    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Car Car { get; set; }
        public int Payment { get; set; }

        public Client() { }

        public Client(Car car, int payment)
        {
            if (payment < 0) throw new ArgumentException("Оплата не может быть отрицательной.");
            Car = car;
            Payment = payment;
        }
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PartName { get; set; }
        public int Quantity { get; set; }
        public int DayOrdered { get; set; }
        public int DeliveryAfterDays { get; set; }
        public bool Delivered { get; set; }

        public PurchaseOrder() { }
    }

    public class AutoService
    {
        public int Balance { get; set; }
        public List<Part> Parts { get; set; }
        public List<PurchaseOrder> PendingOrders { get; set; }

        public AutoService(int startingBalance)
        {
            if (startingBalance < 0) startingBalance = 0;
            Balance = startingBalance;
            Parts = new List<Part>()
            {
                new Part("Двигатель", 5000, 1),
                new Part("Колесо", 800, 4),
                new Part("Тормоз", 1200, 2),
                new Part("Фара", 400, 3)
            };
            PendingOrders = new List<PurchaseOrder>();
        }

        public void ShowStatus()
        {
            Console.WriteLine();
            Console.WriteLine("=== Состояние автосервиса ===");
            Console.WriteLine("Баланс: " + Balance + " руб.");
            Console.WriteLine("Склад запчастей:");
            foreach (var p in Parts)
            {
                Console.WriteLine($"- {p.Name}: {p.Quantity} шт. (цена {p.Price} руб.)");
            }
            Console.WriteLine("=============================");
            Console.WriteLine();
        }

        public Client GenerateClient()
        {
            string[] possible = Parts.Select(x => x.Name).ToArray();
            Random rnd = new Random();
            string broken = possible[rnd.Next(possible.Length)];
            int repairCost = Parts.First(p => p.Name == broken).Price + 500;
            return new Client(new Car(broken), repairCost);
        }

        public void RepairCar(Client client)
        {
            if (client == null || client.Car == null)
            {
                Console.WriteLine("Некорректный клиент или машина.");
                return;
            }

            string needed = client.Car.BrokenPartName;
            var part = Parts.FirstOrDefault(p => p.Name == needed);
            if (part != null && part.Quantity > 0)
            {
                part.Quantity = part.Quantity - 1;
                Balance = Balance + client.Payment;
                Console.WriteLine($"✅ Ремонт выполнен. Получено {client.Payment} руб. Деталь: {needed} использована.");
            }
            else
            {
                Console.WriteLine($"❌ Нужной детали ({needed}) нет на складе. Клиент уехал. Штраф за отказ: 300 руб.");
                Balance = Balance - 300;
            }
        }

        public void OrderPartsSimple()
        {
            Console.WriteLine("\n=== Меню покупки деталей ===");
            for (int i = 0; i < Parts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Parts[i].Name} — цена {Parts[i].Price} руб.");
            }
            Console.WriteLine("0. Отмена");
            Console.Write("Выберите номер детали: ");
            string input = Console.ReadLine();
            int idx;
            if (!int.TryParse(input, out idx) || idx < 0 || idx > Parts.Count)
            {
                Console.WriteLine("Неверный ввод.");
                return;
            }
            if (idx == 0) return;
            Console.Write("Введите количество: ");
            string qIn = Console.ReadLine();
            int qty;
            if (!int.TryParse(qIn, out qty) || qty <= 0)
            {
                Console.WriteLine("Некорректное количество.");
                return;
            }
            var sel = Parts[idx - 1];
            long total = (long)sel.Price * qty;
            if (total > int.MaxValue)
            {
                Console.WriteLine("Слишком большая сумма покупки.");
                return;
            }
            if (Balance >= total)
            {
                Balance -= (int)total;
                sel.Quantity += qty;
                Console.WriteLine($"Куплено {qty} шт. {sel.Name}. Потрачено {total} руб.");
            }
            else
            {
                Console.WriteLine("Недостаточно денег для покупки.");
            }
        }

        public void OrderParts(string partName, int qty, int currentDay) { }
        public void ProcessDeliveries(int currentDay) { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AutoService service = new AutoService(10000);

            while (true)
            {
                service.ShowStatus();
                Client client = service.GenerateClient();
                Console.WriteLine($"Клиент приехал с поломкой: {client.Car.BrokenPartName}");
                Console.WriteLine($"Клиент готов заплатить: {client.Payment} руб.");
                Console.WriteLine("1 - Починить, 2 - Отказать, 3 - Купить детали, 0 - Выход");
                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    service.RepairCar(client);
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Вы отказали клиенту. Штраф 200 руб.");
                    service.Balance -= 200;
                }
                else if (choice == "3")
                {
                    service.OrderPartsSimple();
                }
                else if (choice == "0")
                {
                    Console.WriteLine("Выход. Спасибо за игру.");
                    break;
                }
                else
                {
                    Console.WriteLine("Неверный выбор.");
                }

                if (service.Balance <= 0)
                {
                    Console.WriteLine("Вы разорились. Игра окончена.");
                    break;
                }
            }
        }
    }
}
