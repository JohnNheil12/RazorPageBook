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
                    Category = "Fiction",
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
                },
                new Book
                {
                    Title = "The Adventures of Tom Sawyer",
                    Author = "Mark Twain",
                    YearPublished = 2004,
                    Publisher = "Sterling",
                    ImageUrl = "https://tse2.mm.bing.net/th/id/OIP.EYTQlv3NmnyUdfD_WMFCVQHaMC?rs=1&pid=ImgDetMain&o=7&rm=3",
                },
                new Book
                {
                    Title = "Treasure Island",
                    Author = "Robert Louis Stevenson",
                    YearPublished = 1981,
                    Publisher = "Scribner",
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjMwNDQ1NzU3OF5BMl5BanBnXkFtZTcwODY3MDc5Nw@@._V1_FMjpg_UX1000_.jpg",
                },
                new Book
                {
                    Title = "Wuthering Heights",
                    Author = "Emily Brunte & Pauline Nestor",
                    YearPublished = 1995,
                    Publisher = "Penguin Books",
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMWM2ZmM1OTQtZjkzOC00YjdkLTllODctMWI1Njc1NDkyMGY2XkEyXkFqcGc@._V1_FMjpg_UX1000_.jpg",
                },
                 new Book
                 {
                     Title = "Jurrasic Park: a novel",
                     Author = "Michael Crichton",
                     YearPublished = 1990,
                     Publisher = "Random House",
                     ImageUrl = "https://i.pinimg.com/736x/7a/8e/a2/7a8ea2d26cea70b8f7dde560a14a784b.jpg",
                 },
                  new Book
                  {
                      Title = "The Mysterious Island",
                      Author = "Jules Verne",
                      YearPublished = 2001,
                      Publisher = "HThe Modern Library",
                      ImageUrl = "https://tse2.mm.bing.net/th/id/OIP._RuLbeXpCi3F_-TcgBnJewHaL0?rs=1&pid=ImgDetMain&o=7&rm=3",
                  }
                  , 
                  new Book
                  {
                      Title = "Dreamcatcher : a novel",
                      Author = "Stephen King",
                      YearPublished = 2001,
                      Publisher = "Scribner",
                      ImageUrl = "https://tse3.mm.bing.net/th/id/OIP.09qfU6wntqgIJPL58Kh9YAHaLg?rs=1&pid=ImgDetMain&o=7&rm=3",
                  },
                  new Book
                  {
                      Title = "The Goldfinch",
                      Author = "Donna Tartt",
                      YearPublished = 2013,
                      Publisher = "Little, Brown and Company",
                      ImageUrl = "https://th.bing.com/th/id/OIP.GBPsJ_fFhvYF8b4IKNtrPAHaLY?o=7rm=3&rs=1&pid=ImgDetMain&o=7&rm=3",
                  },
                  new Book
                  { 
                      Title = "The Night Circus",
                      Author = "Erin Morgenstern",
                      YearPublished = 2011,
                      Publisher = "Doubleday",
                      ImageUrl = "https://booksofbrilliance.com/wp-content/uploads/2022/06/Screen-Shot-2022-06-09-at-9.01.44-PM.png",
                  }

            );
            context.SaveChanges();
        }
    }
}