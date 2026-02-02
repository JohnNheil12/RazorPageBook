using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Data;
using RazorPageBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? Title { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? BookTitle { get; set; }

        public async Task OnGetAsync()
        {
            var books = from b in _context.Book
                        select b;

            if (!string.IsNullOrEmpty(SearchString))
            {
                books = books.Where(s => (s.Title != null && s.Title.Contains(SearchString))
                          || (s.Author != null && s.Author.Contains(SearchString)));
            }

            Book = await books.ToListAsync();
        }
    }
}
