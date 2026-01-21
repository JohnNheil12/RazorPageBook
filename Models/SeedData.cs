using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Data;

namespace RazorPageBooks.Models;
public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new RazorPageBooksContext(
        serviceProvider.GetRequiredService<
        DbContextOptions<RazorPageBooksContext>>()))
        {
            if (context == null || context.Book == null)
            {
                throw new ArgumentNullException("Null RazorPagesMovieContext");
            }

            // Look for any movies.
            if (context.Book.Any())
            {
                return;
            }
            context.Book.AddRange(
            new Book
            {
                Title = "When Harry Met Sally",
                Author = "Nora Ephron",
                YearPublished = 2019
            },
            new Book
            {
                Title = "Harry Potter",
                Author = "Kirk Villamor",
                YearPublished = 2020
            },
            new Book
            {
                Title = "When Life Gives You Tangerine",
                Author = "Euro Step",
                YearPublished = 2021
            },
            new Book
            {
                Title = "When Time is Ended",
                Author = "Kian Kram",
                YearPublished = 2022
            }
            );
            context.SaveChanges();
        }
    }
}