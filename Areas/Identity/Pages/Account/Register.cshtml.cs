#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPageBooks.Areas.Identity.Pages.Account
{
    [Authorize] // 🔒 Must be logged in
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(UserManager<IdentityUser> userManager,
                             ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGet()
        {
            // 🔒 Block non-admin even on GET
            if (User.Identity.Name != "admin")
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 🔒 Block non-admin on POST
            if (User.Identity.Name != "admin")
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (!ModelState.IsValid)
                return Page();

            // Check duplicate username
            var existingUser = await _userManager.FindByNameAsync(Input.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Username already taken.");
                return Page();
            }

            // Check duplicate email
            var existingEmail = await _userManager.FindByEmailAsync(Input.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Email already in use.");
                return Page();
            }

            var newUser = new IdentityUser
            {
                UserName = Input.Username,
                Email = Input.Email,
                EmailConfirmed = true // ✅ No email verification needed
            };

            var result = await _userManager.CreateAsync(newUser, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Admin created new user: {Username}", Input.Username);
                StatusMessage = $"✅ User '{Input.Username}' created successfully!";
                ModelState.Clear();
                Input = null;
                return Page();
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}