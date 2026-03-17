using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Data;
using static System.Net.WebRequestMethods;

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
                    Title = "Walk into the Shadow",
                    Author = "Estelle Darcy",
                    YearPublished = 2019,
                    Publisher = "Contemporary Books",
                    ImageUrl = "https://marketplace.canva.com/EAFfSnGl7II/2/0/1003w/canva-elegant-dark-woods-fantasy-photo-book-cover-vAt8PH1CmqQ.jpg",
                },
                new Book
                {
                    Title = "Harry Potter",
                    Author = "Kirk Villamor",
                    YearPublished = 2020,
                    Publisher = "Scholastic",
                    ImageUrl = "https://static1.srcdn.com/wordpress/wp-content/uploads/2025/03/harry-potter-2.png",
                },
                new Book
                {
                    Title = "The Past is Rising",
                    Author = "Kathryn Bywaters",
                    YearPublished = 2021,
                    Publisher = "Penguin Random House",
                    ImageUrl = "https://i.pinimg.com/564x/f7/c8/12/f7c812c9b0296cd9f119e33a06d9a256.jpg",
                },
                new Book
                {
                    Title = "Beyond the Moon",
                    Author = "Catherine Taylor",
                    YearPublished = 2022,
                    Publisher = "HarperCollins",
                    ImageUrl = "https://blog-cdn.reedsy.com/directories/gallery/139/large_6b475cacd2ad05ee513a65b91960173b.jpg",
                }
            );
            context.SaveChanges();
        }
    }
}