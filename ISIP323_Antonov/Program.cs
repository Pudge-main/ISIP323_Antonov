using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryConsoleApp
{
    public enum Genre
    {
        Fiction,
        NonFiction,
        ScienceFiction,
        Fantasy,
        Romance
    }

    public class Book
    {
        public int Id { get; internal set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre Genre { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\nНазвание: {Title}\nАвтор: {Author}\nЖанр: {Genre}\nГод: {Year}\nЦена: {Price:0.00} руб.\n";
        }
    }

    public class Library
    {
        private List<Book> books = new List<Book>();
        private int nextId = 1;

        public Library()
        {
            SeedTestData();
        }

        public void AddBook(Book book)
        {
            book.Id = nextId++;
            books.Add(book);
        }

        public bool RemoveBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null) return false;
            books.Remove(book);
            return true;
        }

        public List<Book> FindByTitle(string title) =>
            books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Book> FindByAuthor(string author) =>
            books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Book> FindByGenre(Genre genre) =>
            books.Where(b => b.Genre == genre).ToList();

        public List<Book> SortByTitle() => books.OrderBy(b => b.Title).ToList();
        public List<Book> SortByYear() => books.OrderBy(b => b.Year).ToList();

        public Book GetMostExpensiveBook() => books.OrderByDescending(b => b.Price).FirstOrDefault();
        public Book GetCheapestBook() => books.OrderBy(b => b.Price).FirstOrDefault();

        public Dictionary<string, int> GroupByAuthor() =>
            books.GroupBy(b => b.Author).ToDictionary(g => g.Key, g => g.Count());

        public List<Book> GetAllBooks() => books.ToList();

        public void SeedTestData()
        {
            AddBook(new Book { Title = "Преступление и наказание", Author = "Ф. Достоевский", Genre = Genre.Fiction, Year = 1866, Price = 500 });
            AddBook(new Book { Title = "Война и мир", Author = "Л. Толстой", Genre = Genre.Fiction, Year = 1869, Price = 700 });
            AddBook(new Book { Title = "Краткая история времени", Author = "С. Хокинг", Genre = Genre.NonFiction, Year = 1988, Price = 450 });
            AddBook(new Book { Title = "Ночной дозор", Author = "С. Лукьяненко", Genre = Genre.ScienceFiction, Year = 1998, Price = 350 });
            AddBook(new Book { Title = "Гарри Поттер и философский камень", Author = "Д. Роулинг", Genre = Genre.Fantasy, Year = 1997, Price = 600 });
        }
    }

    public class Program
    {
        static Library library = new Library();

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("=== Библиотека ===");
                Console.WriteLine("1 - Показать все книги");
                Console.WriteLine("2 - Добавить книгу");
                Console.WriteLine("3 - Удалить книгу");
                Console.WriteLine("0 - Выход");
                Console.Write("Ваш выбор: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowAll();
                        break;
                    case "2":
                        AddBook();
                        break;
                    case "3":
                        RemoveBook();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }

                Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        static void ShowAll()
        {
            var all = library.GetAllBooks();
            foreach (var b in all)
                Console.WriteLine(b);
        }

        static void AddBook()
        {
            Console.Write("Название: ");
            string title = Console.ReadLine();
            Console.Write("Автор: ");
            string author = Console.ReadLine();
            Console.Write("Год: ");
            int year = int.Parse(Console.ReadLine());
            Console.Write("Цена: ");
            decimal price = decimal.Parse(Console.ReadLine());
            Genre genre = Genre.Fiction;

            library.AddBook(new Book { Title = title, Author = author, Year = year, Price = price, Genre = genre });
            Console.WriteLine("Книга добавлена!");
        }

        static void RemoveBook()
        {
            Console.Write("Введите ID: ");
            int id = int.Parse(Console.ReadLine());
            if (library.RemoveBook(id))
                Console.WriteLine("Книга удалена.");
            else
                Console.WriteLine("Книга не найдена.");
        }
    }
}
