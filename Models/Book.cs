using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPageBooks.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Title must be between 4 and 50 characters.")]
        [Display(Name = "Book Title")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Author name is required")]
        // Refined Regex: Allows names like "J.K. Rowling" or "O'Connor" 
        [RegularExpression(@"^[a-zA-Z\s\.\']+$", ErrorMessage = "Author name can only contain letters, dots, and spaces.")]
        public string? Author { get; set; }

        [Display(Name = "Year Published")]
        [Range(1450, 2026, ErrorMessage = "Please enter a valid year between 1450 and 2026")]
        public int? YearPublished { get; set; }

        [StringLength(50, ErrorMessage = "Publisher name is too long.")]
        public string? Publisher { get; set; }

        [Display(Name = "Cover Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL (e.g., http://example.com/image.jpg)")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")] // Ensures database precision
        public decimal? Price { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Overview cannot exceed 500 characters")]
        public string? Overview { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50)]
        public string? Category { get; set; }
    }
}