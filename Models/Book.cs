using System.ComponentModel.DataAnnotations;

namespace RazorPageBooks.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Title must have a minimum letters of 4.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Author Name is required")]
        [RegularExpression(@"^[a-zA-Z\s\.]+$", ErrorMessage = "Author name should only contain letters")]
        public string? Author { get; set; }

        [Display(Name = "Year Published")]
        [Range(1450, 2026, ErrorMessage = "Please enter a valid year between 1450 and 2026")]
        public int? YearPublished { get; set; }

        [StringLength(50)]
        public string? Publisher { get; set; }
    }
}