using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageBooks.Data;
using RazorPageBooks.Models;

namespace RazorPageBooks.Pages.Books
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly RazorPageBooksContext _context;
        public CreateModel(RazorPageBooksContext context) => _context = context;

        public IActionResult OnGet() => Page();

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();           // Returns the form with errors (200 OK with HTML)

            _context.Book.Add(Book);
            await _context.SaveChangesAsync();

            // Always return 200 OK — Dashboard.cshtml fetch handler checks res.ok
            // and reloads the Products tab on success.
            return new OkResult();
        }
    }
}
