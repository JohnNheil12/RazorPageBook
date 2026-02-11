using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Add this for .ToListAsync()
using RazorPageBooks.Data;           // Update this to match your actual Data namespace
using RazorPageBooks.Models;

namespace RazorPageBooks.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        // 1. Add the Database Context here
        private readonly RazorPageBooks.Data.RazorPageBooksContext _context;

        // 2. Add context to the constructor
        public IndexModel(RazorPageBooks.Data.RazorPageBooksContext context)
        {
            _context = context;
        }

        public IList<Book> Book { get; set; } = default!;

        // 3. Change OnGet to async to fetch the data
        public async Task OnGetAsync()
        {
            if (_context.Book != null)
            {
                Book = await _context.Book.ToListAsync();
            }
        }
    }
}