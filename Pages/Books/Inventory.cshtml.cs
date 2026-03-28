using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IList<Book> Book { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? BookTitle { get; set; }

        public SelectList? Titles { get; set; }

        public async Task OnGetAsync()
        {
            // 1. Sugod sa base query
            var books = from b in _context.Book
                        select b;

            // 2. Filter gamit ang SearchString (Title, Author, or Publisher)
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                // Gigamit nato ang EF.Functions.Like para case-insensitive 
                // ug dili mag-error sa null values sa database.
                books = books.Where(s =>
                    EF.Functions.Like(s.Title, $"%{SearchString}%") ||
                    EF.Functions.Like(s.Author, $"%{SearchString}%") ||
                    EF.Functions.Like(s.Publisher, $"%{SearchString}%"));
            }

            // 3. Filter pinaagi sa specific Title dropdown (kung gigamit sa UI)
            if (!string.IsNullOrEmpty(BookTitle))
            {
                books = books.Where(x => x.Title == BookTitle);
            }

            // 4. Populate ang Titles dropdown (Distinct titles para sa filter)
            IQueryable<string> titleQuery = _context.Book
                                            .OrderBy(b => b.Title)
                                            .Select(b => b.Title);

            Titles = new SelectList(await titleQuery.Distinct().ToListAsync());

            // 5. Execute ang final list
            Book = await books.ToListAsync();
        }

        // ✅ Live search handler para sa AJAX requests (magamit nimo sa JS search)
        public async Task<JsonResult> OnGetSearchAsync(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return new JsonResult(new List<object>());

            var results = await _context.Book
                .Where(b => EF.Functions.Like(b.Title, $"%{searchString}%") ||
                            EF.Functions.Like(b.Author, $"%{searchString}%"))
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