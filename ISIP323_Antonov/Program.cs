using System;
using System.Collections.Generic;

namespace AutoServiceGame
{
    //запчасть
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

    //машина клиента
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

    //клиент и его машина
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

    //заказ запчастей
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

    //основной класс управления
    public class AutoService
    {
        public int Balance { get; set; }
        public List<Part> Parts { get; set; }
        public List<PurchaseOrder> PendingOrders { get; set; }

        public AutoService(int startingBalance)
        {
            Balance = startingBalance;
            Parts = new List<Part>();
            PendingOrders = new List<PurchaseOrder>();
        }

        public void ShowStatus() { }
        public Client GenerateClient() { return null; }
        public void RepairCar(Client client) { }
        public void OrderParts(string partName, int qty, int currentDay) { }
        public void ProcessDeliveries(int currentDay) { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Декомпозиция выполнена. Запустите следующие коммиты для полной игры.");
        }
    }
}
