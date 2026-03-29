using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Data;
using RazorPageBooks.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPageBooks.Pages.Books
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly RazorPageBooksContext _context;

        public EditModel(RazorPageBooksContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _context.Book.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
                return NotFound();

            Book = book;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(Book.Id))
                {
                    return NotFound();
                }
                else
                {
                    // FIX: Instead of re-throwing (which causes a 500 error), add a
                    // user-friendly validation message and re-display the form.
                    ModelState.AddModelError(string.Empty,
                        "This record was modified by another user while you were editing. " +
                        "Please reload and try again.");
                    return Page();
                }
            }

            // 200 OK tells Dashboard.cshtml fetch handler to reload the Products tab
            return new OkResult();
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
