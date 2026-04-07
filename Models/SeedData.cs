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
            // 1. Ensure the database is created
            context.Database.EnsureCreated();

            // 2. Look for any books.
            if (context.Book == null)
            {
                throw new ArgumentNullException("Book DbSet is null in RazorPageBooksContext");
            }
            
            if (context.Book.Any())
            {
                return;   // DB has been seeded
            }

            context.Book.AddRange(
                new Book
                {
                    Title = "Walk into the Shadow",
                    Author = "Estelle Darcy",
                    Category = "Fiction",
                    YearPublished = 2019,
                    Publisher = "Contemporary Books",
                    Price = 576,
                    ImageUrl = "https://marketplace.canva.com/EAFfSnGl7II/2/0/1003w/canva-elegant-dark-woods-fantasy-photo-book-cover-vAt8PH1CmqQ.jpg",
                    Overview = "A fantasy/paranormal story often involving themes of magic, destiny, and dark secrets hidden within a family or a mysterious town.",
                },
                new Book
                {
                    Title = "Harry Potter",
                    Author = "Kirk Villamor",
                    Category = "Fantasy",
                    YearPublished = 2020,
                    Publisher = "Scholastic",
                    Price = 685,
                    ImageUrl = "https://static1.srcdn.com/wordpress/wp-content/uploads/2025/03/harry-potter-2.png",
                    Overview = "The world-famous series following a young orphan who discovers he is a wizard. He attends Hogwarts School of Witchcraft and Wizardry, where he and his friends battle the dark wizard Lord Voldemort.",

                },
                new Book
                {
                    Title = "The Past is Rising",
                    Author = "Kathryn Bywaters",
                    Category = "Drama",
                    YearPublished = 2021,
                    Publisher = "Penguin Random House",
                    Price = 450,
                    ImageUrl = "https://i.pinimg.com/564x/f7/c8/12/f7c812c9b0296cd9f119e33a06d9a256.jpg",
                    Overview = "A YA fantasy epic centered on a young man named Erik who discovers a dark uprising and must embrace a hidden destiny to save his kingdom from ancient magical forces.",
                },
                new Book
                {
                    Title = "Beyond the Moon",
                    Author = "Catherine Taylor",
                    Category = "Historical Fiction",
                    YearPublished = 2022,
                    Publisher = "HarperCollins",
                    Price = 520,
                    ImageUrl = "https://blog-cdn.reedsy.com/directories/gallery/139/large_6b475cacd2ad05ee513a65b91960173b.jpg",
                    Overview = "A time-slip historical novel that connects a soldier in 1916 suffering from \"hysterical blindness\" and a woman in 2017 who finds herself transported back to the same hospital during WWI.",
                },
                new Book
                {
                    Title = "The Adventures of Tom Sawyer",
                    Author = "Mark Twain",
                    Category = "Classics",
                    YearPublished = 2004,
                    Publisher = "Sterling",
                    Price = 398,
                    ImageUrl = "https://tse2.mm.bing.net/th/id/OIP.EYTQlv3NmnyUdfD_WMFCVQHaMC?rs=1&pid=ImgDetMain&o=7&rm=3",
                    Overview = "A classic coming-of-age story about a young boy growing up along the Mississippi River. It’s filled with youthful pranks, imaginative play, and a more serious brush with real-world danger.",
                },
                new Book
                {
                    Title = "Treasure Island",
                    Author = "Robert Louis Stevenson",
                    Category = "Adventure",
                    YearPublished = 1981,
                    Publisher = "Scribner",
                    Price = 299,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjMwNDQ1NzU3OF5BMl5BanBnXkFtZTcwODY3MDc5Nw@@._V1_FMjpg_UX1000_.jpg",
                    Overview = "The quintessential pirate story. Young Jim Hawkins discovers a treasure map and sets sail on the Hispaniola, only to face a mutiny led by the charismatic but dangerous Long John Silver.",
                },
                new Book
                {
                    Title = "Wuthering Heights",
                    Author = "Emily Brunte & Pauline Nestor",
                    Category = "Romance",
                    YearPublished = 1995,
                    Publisher = "Penguin Books",
                    Price = 745,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMWM2ZmM1OTQtZjkzOC00YjdkLTllODctMWI1Njc1NDkyMGY2XkEyXkFqcGc@._V1_FMjpg_UX1000_.jpg",
                    Overview = "A dark, atmospheric novel of intense passion and revenge. It focuses on the turbulent relationship between Catherine Earnshaw and the foundling Heathcliff on the bleak Yorkshire moors.",
                },
                new Book
                {
                    Title = "Jurrasic Park: a novel",
                    Author = "Michael Crichton",
                    Category = "Science Fiction",
                    YearPublished = 1990,
                    Publisher = "Random House",
                    Price = 835,
                    ImageUrl = "https://i.pinimg.com/736x/7a/8e/a2/7a8ea2d26cea70b8f7dde560a14a784b.jpg",
                    Overview = "A cautionary sci-fi thriller about the ethics of genetic engineering. It details the collapse of a theme park where dinosaurs have been brought back to life, leading to a terrifying struggle for survival.",
                },
                new Book
                {
                    Title = "The Mysterious Island",
                    Author = "Jules Verne",
                    Category = "Adventure",
                    YearPublished = 2001,
                    Publisher = "HThe Modern Library",
                    Price = 399,
                    ImageUrl = "https://tse2.mm.bing.net/th/id/OIP._RuLbeXpCi3F_-TcgBnJewHaL0?rs=1&pid=ImgDetMain&o=7&rm=3",
                    Overview = "A \"sequel\" of sorts to 20,000 Leagues Under the Sea. It follows five American Civil War prisoners who escape in a hot air balloon and crash on an uncharted Pacific island, where they must use science to survive.",
                },
                new Book
                {
                    Title = "Dreamcatcher : a novel",
                    Author = "Stephen King",
                    Category = "Horror",
                    YearPublished = 2001,
                    Publisher = "Scribner",
                    Price = 189,
                    ImageUrl = "https://tse3.mm.bing.net/th/id/OIP.09qfU6wntqgIJPL58Kh9YAHaLg?rs=1&pid=ImgDetMain&o=7&rm=3",
                    Overview = "A blend of horror and sci-fi. Four lifelong friends on a hunting trip in Maine encounter a stranded stranger and soon find themselves in the middle of a gruesome alien invasion.",
                },
                new Book
                {
                    Title = "The Goldfinch",
                    Author = "Donna Tartt",
                    Category = "Contemporary Fiction",
                    YearPublished = 2013,
                    Publisher = "Little, Brown and Company",
                    Price = 595,
                    ImageUrl = "https://th.bing.com/th/id/OIP.GBPsJ_fFhvYF8b4IKNtrPAHaLY?o=7rm=3&rs=1&pid=ImgDetMain&o=7&rm=3",
                    Overview = "A Pulitzer Prize-winning novel about Theo Decker, who survives a terrorist attack at an art museum. In the confusion, he steals a small, famous painting, which becomes his secret burden and connection to his lost mother as he grows up.",
                },
                new Book
                {
                    Title = "The Night Circus",
                    Author = "Erin Morgenstern",
                    Category = "Fantasy",
                    YearPublished = 2011,
                    Publisher = "Doubleday",
                    Price = 519,
                    ImageUrl = "https://booksofbrilliance.com/wp-content/uploads/2022/06/Screen-Shot-2022-06-09-at-9.01.44-PM.png",
                    Overview = "A magical, atmospheric story about a mysterious traveling circus that only opens at night. At its center is a high-stakes duel between two young magicians, Celia and Marco, who eventually fall in love.",
                }
            );
            context.SaveChanges();
        }
    }
}
