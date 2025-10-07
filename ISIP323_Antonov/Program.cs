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
                Console.WriteLine("4 - Найти по названию");
                Console.WriteLine("5 - Найти по автору");
                Console.WriteLine("6 - Найти по жанру");
                Console.WriteLine("7 - Сортировать по названию");
                Console.WriteLine("8 - Сортировать по году");
                Console.WriteLine("9 - Самая дорогая книга");
                Console.WriteLine("10 - Самая дешёвая книга");
                Console.WriteLine("11 - Группировка по авторам");
                Console.WriteLine("0 - Выход");
                Console.Write("Ваш выбор: ");

                var choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1": ShowAll(); break;
                    case "2": AddBook(); break;
                    case "3": RemoveBook(); break;
                    case "4": FindByTitle(); break;
                    case "5": FindByAuthor(); break;
                    case "6": FindByGenre(); break;
                    case "7": ShowBooks(library.SortByTitle()); break;
                    case "8": ShowBooks(library.SortByYear()); break;
                    case "9": ShowBook(library.GetMostExpensiveBook(), "Самая дорогая книга"); break;
                    case "10": ShowBook(library.GetCheapestBook(), "Самая дешёвая книга"); break;
                    case "11": GroupByAuthor(); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("Неверный выбор."); break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        static void ShowAll() => ShowBooks(library.GetAllBooks());

        static void AddBook()
        {
            Console.Write("Название: ");
            string title = ReadNotEmpty();
            Console.Write("Автор: ");
            string author = ReadNotEmpty();
            int year = ReadInt("Год издания: ", 0, DateTime.Now.Year);
            decimal price = ReadDecimal("Цена: ", 0);
            Genre genre = ReadGenre();

            library.AddBook(new Book { Title = title, Author = author, Year = year, Price = price, Genre = genre });
            Console.WriteLine("Книга успешно добавлена!");
        }

        static void RemoveBook()
        {
            int id = ReadInt("Введите ID книги для удаления: ", 1, int.MaxValue);
            if (library.RemoveBook(id))
                Console.WriteLine("Книга удалена.");
            else
                Console.WriteLine("Книга не найдена.");
        }

        static void FindByTitle()
        {
            Console.Write("Введите название книги: ");
            ShowBooks(library.FindByTitle(Console.ReadLine()));
        }

        static void FindByAuthor()
        {
            Console.Write("Введите автора: ");
            ShowBooks(library.FindByAuthor(Console.ReadLine()));
        }

        static void FindByGenre()
        {
            Genre genre = ReadGenre();
            ShowBooks(library.FindByGenre(genre));
        }

        static void GroupByAuthor()
        {
            var groups = library.GroupByAuthor();
            foreach (var g in groups)
                Console.WriteLine($"{g.Key} — {g.Value} книг(и)");
        }

        static void ShowBooks(List<Book> books)
        {
            if (books.Count == 0)
                Console.WriteLine("Книг не найдено.");
            else
                foreach (var b in books)
                    Console.WriteLine(b);
        }

        static void ShowBook(Book book, string title)
        {
            Console.WriteLine(title);
            if (book == null)
                Console.WriteLine("Книга не найдена.");
            else
                Console.WriteLine(book);
        }

        static string ReadNotEmpty()
        {
            string s;
            do
            {
                s = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(s))
                    Console.Write("Пустое значение! Повторите ввод: ");
            }
            while (string.IsNullOrWhiteSpace(s));
            return s;
        }

        static int ReadInt(string message, int min, int max)
        {
            int value;
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out value) && value >= min && value <= max)
                    return value;
                Console.WriteLine($"Ошибка! Введите число от {min} до {max}.");
            }
        }

        static decimal ReadDecimal(string message, decimal min)
        {
            decimal value;
            while (true)
            {
                Console.Write(message);
                if (decimal.TryParse(Console.ReadLine(), out value) && value >= min)
                    return value;
                Console.WriteLine($"Ошибка! Цена должна быть не меньше {min}.");
            }
        }

        static Genre ReadGenre()
        {
            Console.WriteLine("Выберите жанр:");
            var genres = Enum.GetValues(typeof(Genre)).Cast<Genre>().ToList();
            for (int i = 0; i < genres.Count; i++)
                Console.WriteLine($"{i + 1}. {genres[i]}");

            int choice = ReadInt("Ваш выбор: ", 1, genres.Count);
            return genres[choice - 1];
        }
    }
}
