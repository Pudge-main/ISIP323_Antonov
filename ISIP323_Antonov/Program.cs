using System;
using System.Collections.Generic;

namespace LibraryConsoleApp
{
    public enum Genre
    {
        Fiction,
        NonFiction,
        ScienceFiction
    }

    public class Book
    {
        public int Id { get; private set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre Genre { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }

        public override string ToString() { return ""; }
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
