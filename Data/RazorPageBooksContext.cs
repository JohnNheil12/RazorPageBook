using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Models;

namespace RazorPageBooks.Data
{
    public class RazorPageBooksContext : DbContext
    {
        public RazorPageBooksContext (DbContextOptions<RazorPageBooksContext> options)
            : base(options)
        {
        }

        public DbSet<RazorPageBooks.Models.Book> Book { get; set; } = default!;
    }
}
