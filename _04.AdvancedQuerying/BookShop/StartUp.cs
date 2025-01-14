﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BookShop.Models;
using BookShop.Models.Enums;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            // var command = Console.ReadLine();
            // Console.WriteLine(GetBooksByAgeRestriction(db, command));
            
            // Console.WriteLine(GetGoldenBooks(db));
            
            // Console.WriteLine(GetBooksByPrice(db));

            // var year = int.Parse(Console.ReadLine());
            // Console.WriteLine(GetBooksNotReleasedIn(db, year));

            // var input = Console.ReadLine();
            // Console.WriteLine(GetBooksByCategory(db, input));

            // var date = Console.ReadLine();
            // Console.WriteLine(GetBooksReleasedBefore(db, date));

            // var sufix = Console.ReadLine();
            // Console.WriteLine(GetAuthorNamesEndingIn(db, sufix));

            // var input = Console.ReadLine();
            // Console.WriteLine(GetBookTitlesContaining(db, input));

            // var sufix = Console.ReadLine();
            // Console.WriteLine(GetBooksByAuthor(db, sufix));

            // var length = int.Parse(Console.ReadLine());
            // Console.WriteLine(CountBooks(db, length));

            // Console.WriteLine(CountCopiesByAuthor(db));

            // Console.WriteLine(GetTotalProfitByCategory(db));

            // Console.WriteLine(GetMostRecentBooks(db));
            
            // IncreasePrices(db);

            Console.WriteLine(RemoveBooks(db));
        }
        
        // 2. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var cmd = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(b => b.AgeRestriction == cmd)
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine(b);

            return sb.ToString().TrimEnd();
        }

        // 3. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var type = Enum.Parse<EditionType>("Gold");

            var books = context.Books
                .Where(b => b.EditionType == type && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine(b);

            return sb.ToString().TrimEnd();
        }

        // 4. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new { Title = b.Title, Price = b.Price })
                .OrderByDescending(b => b.Price)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine($"{b.Title} - ${b.Price:f2}");

            return sb.ToString().TrimEnd();
        }

        // 5. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine(b);

            return sb.ToString().TrimEnd();
        }
        
        // 6. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            HashSet<string> categories = new HashSet<string>(input.Split(), StringComparer.OrdinalIgnoreCase);

            var books = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name)))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine(b);

            return sb.ToString().TrimEnd();
        }
        
        // 7. Released before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value < targetDate)
                .Select(b => new
                {
                    Title = b.Title,
                    Edinition = b.EditionType,
                    Price = b.Price,
                    ReleaseDate = b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine($"{b.Title} - {b.Edinition} - ${b.Price:f2}");

            return sb.ToString().TrimEnd();
        }

        // 8. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new { FullName = a.FirstName + " " + a.LastName })
                .OrderBy(a => a.FullName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var a in authors)
                sb.AppendLine(a.FullName);

            return sb.ToString().TrimEnd();
        }

        // 9. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Title.Contains(input, StringComparison.OrdinalIgnoreCase))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine(b);

            return sb.ToString().TrimEnd();
        }
        
        // 10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    Title = b.Title,
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine($"{b.Title} ({b.AuthorName})");

            return sb.ToString().TrimEnd();
        }
        
        // 11. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
           => context.Books.Count(b => b.Title.Length > lengthCheck);
        
        // 12. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    Name = a.FirstName + " " + a.LastName,
                    CopiesCount = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.CopiesCount)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var a in authors)
                sb.AppendLine($"{a.Name} - {a.CopiesCount}");

            return sb.ToString().TrimEnd();
        }

        // 13. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    Name = c.Name,
                    Profit = c.CategoryBooks.Sum(x => x.Book.Copies * x.Book.Price)
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var c in categories)
                sb.AppendLine($"{c.Name} ${c.Profit}");

            return sb.ToString().TrimEnd();
        }
        
        // 14. Most recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    Name = c.Name,
                    Books = c.CategoryBooks.Select(cb => cb.Book).ToArray()
                })
                .OrderBy(c => c.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            
            foreach (var c in categories)
            {
                sb.AppendLine($"--{c.Name}");
                foreach (var b in c.Books.OrderByDescending(b => b.ReleaseDate).Take(3))
                    sb.AppendLine($"{b.Title} ({b.ReleaseDate.Value.Year})");
            }

            return sb.ToString().TrimEnd();
        }
        
        //15. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            foreach (var b in context.Books.Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010))
            {
                b.Price += 5;
            }

            context.SaveChanges();
        }

        // 16. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context.Books.Where(b => b.Copies < 4200).ToArray();
            var count = booksToRemove.Length;

            foreach (var b in booksToRemove) context.Books.Remove(b);
            context.SaveChanges();

            return count;
        }
    }
}
