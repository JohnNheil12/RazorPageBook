using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Models;

namespace RazorPageBooks.Data
{
    public class RazorPageBooksContext : IdentityDbContext
    {
        public RazorPageBooksContext (DbContextOptions<RazorPageBooksContext> options)
            : base(options)
        {
        }

        public DbSet<RazorPageBooks.Models.Book> Book { get; set; } = default!;
    }
}
