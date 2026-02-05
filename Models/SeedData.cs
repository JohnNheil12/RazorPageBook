using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Data;

namespace RazorPageBooks.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new RazorPageBooksContext(
            serviceProvider.GetRequiredService<DbContextOptions<RazorPageBooksContext>>()))
        {
            // 1. Ensure the database is created before we try to check for books
            context.Database.EnsureCreated();

            // 2. Look for any books.
            if (context.Book == null)
            {
                throw new ArgumentNullException("Book DbSet is null in RazorPageBooksContext");
            }
            if (context.Book.Any())
            {
                return;   
            }
            //context.Book.ExecuteDelete();


            context.Book.AddRange(
                new Book
                {
                    Title = "When Harry Met Sally",
                    Author = "Nora Aunor",
                    YearPublished = 2019,
                    Publisher = "Contemporary Books",
                },
                new Book
                {
                    Title = "Harry Potter",
                    Author = "Kirk Villamor",
                    YearPublished = 2020,
                    Publisher = "Scholastic",
                },
                new Book
                {
                    Title = "When Life Gives You Tangerine",
                    Author = "Euro Step",
                    YearPublished = 2021,
                    Publisher = "Penguin Random House",
                },
                new Book
                {
                    Title = "When Time is Ended",
                    Author = "Kian Kram",
                    YearPublished = 2022,
                    Publisher = "HarperCollins",
                }
            );
            context.SaveChanges();
        }
    }
}