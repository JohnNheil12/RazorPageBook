using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPageBooks.Data
{
    public class RazorPageBooksContext : IdentityDbContext<IdentityUser>
    {
        public RazorPageBooksContext (DbContextOptions<RazorPageBooksContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Book { get; set; } = default!;
    }
}
