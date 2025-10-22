using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace AutoServiceGame
{
    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

        public Part() { }

        public Part(string name, int price)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Имя детали не может быть пустым.");
            if (price < 0) throw new ArgumentException("Цена не может быть отрицательной.");
            Name = name;
            Price = price;
        }
    }

    public class Stock
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public int Quantity { get; set; }
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public int Quantity { get; set; }
        public int DayOrdered { get; set; }
        public int DeliveryAfterDays { get; set; }
        public bool Delivered { get; set; }
    }

    public class DbManager
    {
        private string _dbFile;
        private string _connString;

        public DbManager(string dbFile = "autoservice.db")
        {
            _dbFile = dbFile;
            _connString = $"Data Source={_dbFile};Version=3;";
            if (!File.Exists(_dbFile))
            {
                SQLiteConnection.CreateFile(_dbFile);
            }
            EnsureSchema();
        }

        private void EnsureSchema()
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Parts (
                                            PartId INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Name TEXT NOT NULL UNIQUE,
                                            Price INTEGER NOT NULL
                                        );";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Stocks (
                                            StockId INTEGER PRIMARY KEY AUTOINCREMENT,
                                            PartId INTEGER NOT NULL,
                                            Quantity INTEGER NOT NULL,
                                            FOREIGN KEY(PartId) REFERENCES Parts(PartId)
                                        );";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Orders (
                                            OrderId INTEGER PRIMARY KEY AUTOINCREMENT,
                                            PartId INTEGER NOT NULL,
                                            Quantity INTEGER NOT NULL,
                                            DayOrdered INTEGER NOT NULL,
                                            DeliveryAfterDays INTEGER NOT NULL,
                                            Delivered INTEGER NOT NULL DEFAULT 0,
                                            FOREIGN KEY(PartId) REFERENCES Parts(PartId)
                                        );";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ServiceState (
                                            StateId INTEGER PRIMARY KEY CHECK(StateId = 1),
                                            Balance INTEGER NOT NULL
                                        );";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT OR IGNORE INTO ServiceState(StateId, Balance) VALUES(1, 0);";
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public int EnsurePart(string name, int price)
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT PartId FROM Parts WHERE Name = @name;";
                    cmd.Parameters.AddWithValue("@name", name);
                    var res = cmd.ExecuteScalar();
                    if (res != null)
                    {
                        int id = Convert.ToInt32(res);
                        cmd.CommandText = "UPDATE Parts SET Price = @price WHERE PartId = @id;";
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        return id;
                    }
                    else
                    {
                        cmd.CommandText = "INSERT INTO Parts(Name, Price) VALUES(@name, @price); SELECT last_insert_rowid();";
                        cmd.Parameters.AddWithValue("@price", price);
                        long newId = (long)cmd.ExecuteScalar();
                        return (int)newId;
                    }
                }
            }
        }

        public void AddStock(int partId, int qty)
        {
            if (qty <= 0) return;
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT StockId, Quantity FROM Stocks WHERE PartId = @pid;";
                    cmd.Parameters.AddWithValue("@pid", partId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int stockId = reader.GetInt32(0);
                            int oldQ = reader.GetInt32(1);
                            reader.Close();
                            cmd.CommandText = "UPDATE Stocks SET Quantity = @q WHERE StockId = @sid;";
                            cmd.Parameters.AddWithValue("@q", oldQ + qty);
                            cmd.Parameters.AddWithValue("@sid", stockId);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            reader.Close();
                            cmd.CommandText = "INSERT INTO Stocks(PartId, Quantity) VALUES(@pid, @q);";
                            cmd.Parameters.AddWithValue("@q", qty);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                conn.Close();
            }
        }

        public int GetStockQuantity(int partId)
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT Quantity FROM Stocks WHERE PartId = @pid;";
                    cmd.Parameters.AddWithValue("@pid", partId);
                    var res = cmd.ExecuteScalar();
                    conn.Close();
                    if (res == null) return 0;
                    return Convert.ToInt32(res);
                }
            }
        }

        public List<(Part part, int quantity)> LoadAllPartsWithStock()
        {
            var list = new List<(Part part, int quantity)>();
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"SELECT Parts.PartId, Parts.Name, Parts.Price, IFNULL(Stocks.Quantity,0)
                                        FROM Parts LEFT JOIN Stocks ON Parts.PartId = Stocks.PartId;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var p = new Part()
                            {
                                Id = Convert.ToInt32(reader["PartId"]),
                                Name = Convert.ToString(reader["Name"]),
                                Price = Convert.ToInt32(reader["Price"])
                            };
                            int qty = Convert.ToInt32(reader[3]);
                            list.Add((p, qty));
                        }
                    }
                }
                conn.Close();
            }
            return list;
        }

        public int GetBalance()
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Balance FROM ServiceState WHERE StateId = 1;", conn))
                {
                    var res = cmd.ExecuteScalar();
                    conn.Close();
                    if (res == null) return 0;
                    return Convert.ToInt32(res);
                }
            }
        }

        public void SetBalance(int balance)
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE ServiceState SET Balance = @b WHERE StateId = 1;", conn))
                {
                    cmd.Parameters.AddWithValue("@b", balance);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public void CreateOrder(int partId, int qty, int dayOrdered, int deliveryDays)
        {
            if (qty <= 0) return;
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "INSERT INTO Orders(PartId, Quantity, DayOrdered, DeliveryAfterDays, Delivered) VALUES(@pid, @q, @d, @dd, 0);";
                    cmd.Parameters.AddWithValue("@pid", partId);
                    cmd.Parameters.AddWithValue("@q", qty);
                    cmd.Parameters.AddWithValue("@d", dayOrdered);
                    cmd.Parameters.AddWithValue("@dd", deliveryDays);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public List<PurchaseOrder> GetDeliverableOrders(int currentDay)
        {
            var resList = new List<PurchaseOrder>();
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT OrderId, PartId, Quantity, DayOrdered, DeliveryAfterDays, Delivered FROM Orders WHERE Delivered = 0 AND (DayOrdered + DeliveryAfterDays) <= @cur;";
                    cmd.Parameters.AddWithValue("@cur", currentDay);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var po = new PurchaseOrder()
                            {
                                Id = Convert.ToInt32(reader["OrderId"]),
                                PartId = Convert.ToInt32(reader["PartId"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                DayOrdered = Convert.ToInt32(reader["DayOrdered"]),
                                DeliveryAfterDays = Convert.ToInt32(reader["DeliveryAfterDays"]),
                                Delivered = Convert.ToInt32(reader["Delivered"]) != 0
                            };
                            resList.Add(po);
                        }
                    }
                }
                conn.Close();
            }
            return resList;
        }

        public void MarkOrderDelivered(int orderId)
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE Orders SET Delivered = 1 WHERE OrderId = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
    }

    public class AutoService
    {
        private DbManager _db;
        public int Balance { get; set; }
        public List<(Part part, int qty)> CatalogAndStock { get; set; }

        public AutoService(DbManager db, int startingBalance)
        {
            _db = db;
            CatalogAndStock = new List<(Part part, int qty)>();

            var starters = new List<Part>()
            {
                new Part("Двигатель", 5000),
                new Part("Колесо", 800),
                new Part("Тормоз", 1200),
                new Part("Фара", 400)
            };

            foreach (var sp in starters)
            {
                int pid = _db.EnsurePart(sp.Name, sp.Price);
                int existingQty = _db.GetStockQuantity(pid);
                if (existingQty == 0)
                {
                    _db.AddStock(pid, sp.Name == "Колесо" ? 4 : 1);
                }
            }

            LoadFromDb();

            int dbBalance = _db.GetBalance();
            if (dbBalance == 0 && startingBalance > 0)
            {
                _db.SetBalance(startingBalance);
                Balance = startingBalance;
            }
            else
            {
                Balance = dbBalance;
            }
        }

        public void LoadFromDb()
        {
            CatalogAndStock = _db.LoadAllPartsWithStock();
        }

        public void ShowStatus()
        {
            LoadFromDb();
            Console.WriteLine();
            Console.WriteLine("=== Состояние автосервиса (БД) ===");
            Console.WriteLine("Баланс: " + Balance + " руб.");
            Console.WriteLine("Склад:");
            foreach (var item in CatalogAndStock)
            {
                Console.WriteLine($"- {item.part.Name}: {item.qty} шт. (цена {item.part.Price} руб.)");
            }
            Console.WriteLine("==================================");
            Console.WriteLine();
        }

        public Client GenerateClient()
        {
            LoadFromDb();
            var possible = CatalogAndStock.Select(x => x.part.Name).ToArray();
            Random rnd = new Random();
            string broken = possible[rnd.Next(possible.Length)];
            int price = CatalogAndStock.First(p => p.part.Name == broken).part.Price;
            int repairCost = price + 500;
            return new Client(new Car(broken), repairCost);
        }

        public void RepairCar(Client client)
        {
            if (client == null || client.Car == null)
            {
                Console.WriteLine("Некорректный клиент.");
                return;
            }
            LoadFromDb();
            string needed = client.Car.BrokenPartName;
            var entry = CatalogAndStock.FirstOrDefault(x => x.part.Name == needed);
            if (entry.part == null)
            {
                Console.WriteLine("Неизвестная деталь.");
                return;
            }
            int pid = entry.part.Id;
            int qty = entry.qty;
            if (qty > 0)
            {
                _db.AddStock(pid, -1);
                AdjustStockDirect(pid, -1);
                Balance += client.Payment;
                _db.SetBalance(Balance);
                Console.WriteLine($"Ремонт выполнен. Получено {client.Payment} руб. ({needed})");
            }
            else
            {
                Console.WriteLine($"Детали {needed} нет. Клиент уехал. Штраф 300 руб.");
                Balance -= 300;
                _db.SetBalance(Balance);
            }
        }

        private void AdjustStockDirect(int partId, int delta)
        {
            using (var conn = new SQLiteConnection($"Data Source=autoservice.db;Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT Quantity FROM Stocks WHERE PartId = @pid;";
                    cmd.Parameters.AddWithValue("@pid", partId);
                    var res = cmd.ExecuteScalar();
                    if (res == null)
                    {
                        int newQty = Math.Max(0, delta);
                        cmd.CommandText = "INSERT INTO Stocks(PartId, Quantity) VALUES(@pid, @q);";
                        cmd.Parameters.AddWithValue("@q", newQty);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        int cur = Convert.ToInt32(res);
                        int updated = cur + delta;
                        if (updated < 0) updated = 0;
                        cmd.CommandText = "UPDATE Stocks SET Quantity = @q WHERE PartId = @pid;";
                        cmd.Parameters.AddWithValue("@q", updated);
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.Close();
            }
        }

        public void OrderPartsDeferred(string partName, int qty, int currentDay, int deliveryDays)
        {
            if (qty <= 0)
            {
                Console.WriteLine("Количество должно быть положительным.");
                return;
            }

            LoadFromDb();
            var entry = CatalogAndStock.FirstOrDefault(x => x.part.Name == partName);
            if (entry.part == null)
            {
                Console.WriteLine("Такая деталь не найдена.");
                return;
            }
            int pid = entry.part.Id;
            long total = (long)entry.part.Price * qty;
            if (total > int.MaxValue)
            {
                Console.WriteLine("Слишком большая сумма заказа.");
                return;
            }
            if (Balance < total)
            {
                Console.WriteLine("Недостаточно денег для заказа.");
                return;
            }
            Balance -= (int)total;
            _db.SetBalance(Balance);

            _db.CreateOrder(pid, qty, currentDay, deliveryDays);
            Console.WriteLine($"Заказано {qty} шт. {partName}. День заказа: {currentDay}. Прибудет через {deliveryDays} дня(ей).");
        }

        public void ProcessDeliveries(int currentDay)
        {
            var orders = _db.GetDeliverableOrders(currentDay);
            if (orders.Count == 0) return;
            foreach (var o in orders)
            {
                _db.AddStock(o.PartId, o.Quantity);
                _db.MarkOrderDelivered(o.Id);
                Console.WriteLine($"Поставка: добавлено {o.Quantity} шт. (PartId {o.PartId})");
            }
            LoadFromDb();
        }
    }

    public class Car
    {
        public string BrokenPartName { get; set; }

        public Car(string brokenPartName)
        {
            BrokenPartName = brokenPartName;
        }
    }

    public class Client
    {
        public Car Car { get; set; }
        public int Payment { get; set; }

        public Client(Car car, int payment)
        {
            Car = car;
            Payment = payment;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DbManager db = new DbManager("autoservice.db");
            AutoService service = new AutoService(db, 10000);

            Console.WriteLine("Автосервис (с БД) запущен.");
            service.ShowStatus();

            Console.WriteLine("Дальше будет основной цикл с днями и задержкой поставок (следующие коммиты).");
        }
    }
}
