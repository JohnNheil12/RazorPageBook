using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public IList<Book> Book { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public async Task OnGetAsync()
        {
            var books = from b in _context.Book select b;

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                string search = SearchString.ToLower();
                books = books.Where(s =>
                    s.Title.ToLower().Contains(search) ||
                    s.Author.ToLower().Contains(search) ||
                    s.Publisher.ToLower().Contains(search) ||
                    s.YearPublished.ToString().Contains(search)); // Fixed: Year search added
            }

            Book = await books.AsNoTracking().ToListAsync();
        }

        // Keep your AJAX handler for live suggestions
        public async Task<JsonResult> OnGetSearchAsync(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return new JsonResult(new List<object>());

            var results = await _context.Book
                .AsNoTracking()
                .Where(b => b.Title.ToLower().Contains(searchString.ToLower()) ||
                            b.Author.ToLower().Contains(searchString.ToLower()))
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.YearPublished,
                })
                .Take(10)
                .ToListAsync();

            return new JsonResult(results);
        }
    }
}