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
        public void AddBook(Book book) { }
        public bool RemoveBook(int id) { return false; }
        public List<Book> FindByTitle(string title) { return new List<Book>(); }
        public List<Book> FindByAuthor(string author) { return new List<Book>(); }
        public List<Book> FindByGenre(Genre genre) { return new List<Book>(); }
        public List<Book> SortByTitle() { return new List<Book>(); }
        public List<Book> SortByYear() { return new List<Book>(); }
        public Book GetMostExpensiveBook() { return null; }
        public Book GetCheapestBook() { return null; }
        public Dictionary<string, int> GroupByAuthor() { return new Dictionary<string, int>(); }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
        }
    }
}
