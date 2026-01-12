using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Data;
using RazorPageBooks.Models;

namespace RazorPageBooks.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly RazorPageBooks.Data.RazorPageBooksContext _context;

        public IndexModel(RazorPageBooks.Data.RazorPageBooksContext context)
        {
            _context = context;
        }

        public IList<Book> Book { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Book = await _context.Book.ToListAsync();
        }
    }
}
