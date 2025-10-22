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

        public void RepairCar(Client client) { }
        public void OrderParts(string partName, int qty, int currentDay) { }
        public void ProcessDeliveries(int currentDay) { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AutoService service = new AutoService(10000);
            Console.WriteLine("Автосервис запущен (стартовые данные).");
            service.ShowStatus();
            Console.WriteLine("Дальше будет логика клиентов, ремонта и закупок (следующие коммиты).");
        }
    }
}
