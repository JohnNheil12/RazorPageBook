using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Data;
using RazorPageBooks.Models;

namespace RazorPageBooks.Pages.Books
{
    // Note: Use [IgnoreAntiforgeryToken] only if you have specific cross-site needs. 
    // Generally, Razor Pages handles this automatically.
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

        // Added this to allow filtering by a specific Title via dropdown if needed
        [BindProperty(SupportsGet = true)]
        public string? BookTitle { get; set; }
        public SelectList? Titles { get; set; }

        public async Task OnGetAsync()
        {
            // 1. Start with a LINQ query
            var books = from b in _context.Book
                        select b;

            // 2. Filter by SearchString (Search in Title OR Author)
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                // We use ToLower() to ensure the search is case-insensitive
                // Note: EF Core usually handles this automatically for SQL Server, 
                // but SQLite and PostgreSQL can be strict.
                var lowerSearch = SearchString.ToLower();

                books = books.Where(s =>
                    (s.Title != null && s.Title.ToLower().Contains(lowerSearch)) ||
                    (s.Author != null && s.Author.ToLower().Contains(lowerSearch)) ||
                    (s.Publisher != null && s.Publisher.ToLower().Contains(lowerSearch)));
            }

            // 3. Optional: Filter by specific Title dropdown (if used in UI)
            if (!string.IsNullOrEmpty(BookTitle))
            {
                books = books.Where(x => x.Title == BookTitle);
            }

            // 4. Populate the SelectList for the dropdown (Distinct titles)
            IQueryable<string> titleQuery = from b in _context.Book
                                            orderby b.Title
                                            select b.Title;

            Titles = new SelectList(await titleQuery.Distinct().ToListAsync());

            // 5. Execute the final list
            Book = await books.ToListAsync();
        }

        // ✅ Live search handler for AJAX requests
        public async Task<JsonResult> OnGetSearchAsync(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return new JsonResult(new List<object>());

            var lowerSearch = searchString.ToLower();

            var results = await _context.Book
                .Where(b =>
                    (b.Title != null && b.Title.ToLower().Contains(lowerSearch)) ||
                    (b.Author != null && b.Author.ToLower().Contains(lowerSearch)))
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