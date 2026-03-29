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
    // FIX #2: Only Admins can register new users (role-based, not username-based)
    [Authorize(Roles = "Admin")]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(UserManager<IdentityUser> userManager,
                             RoleManager<IdentityRole> roleManager,
                             ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        // FIX #2: TempData instead of StatusMessage so the message survives
        // the redirect back to the Users management page.
        [TempData]
        public string SuccessMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "First name is required")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last name is required")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Email address is required")]
            [EmailAddress(ErrorMessage = "Invalid email address format")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Please assign a system role")]
            public string Role { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
                ErrorMessage = "Password must have 8+ chars, 1 uppercase, 1 lowercase, 1 number, and 1 special character.")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Validate role input
            if (Input.Role != "Admin" && Input.Role != "Staff")
            {
                ModelState.AddModelError(string.Empty, "Invalid role selected. Choose Admin or Staff.");
                return Page();
            }

            // Check duplicate email
            var existingEmail = await _userManager.FindByEmailAsync(Input.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Input.Email", "This email address is already in use.");
                return Page();
            }

            // Use FirstName.LastName as username internally
            var username = $"{Input.FirstName.Trim()}.{Input.LastName.Trim()}".ToLower();

            // Check duplicate username
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty,
                    $"A user named '{Input.FirstName} {Input.LastName}' already exists. Try a different name.");
                return Page();
            }

            var newUser = new IdentityUser
            {
                UserName = username,
                Email = Input.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, Input.Password);

            if (result.Succeeded)
            {
                // Ensure role exists then assign it
                if (!await _roleManager.RoleExistsAsync(Input.Role))
                    await _roleManager.CreateAsync(new IdentityRole(Input.Role));

                await _userManager.AddToRoleAsync(newUser, Input.Role);

                _logger.LogInformation("Admin created new user: {Username} with role {Role}", username, Input.Role);

                // FIX #2: Set TempData success message and redirect back to
                // the Users management page so the admin sees the updated list
                // with the same green toast style used by the book CRUD.
                SuccessMessage = $"User '{Input.FirstName} {Input.LastName}' created successfully as {Input.Role}.";
                return RedirectToPage("/Users/Users", new { partial = "true", registered = "true" });
            }

            // Surface Identity errors with friendlier messages
            foreach (var error in result.Errors)
            {
                var message = error.Code switch
                {
                    "PasswordRequiresNonAlphanumeric" => "Password must include at least one special character (e.g. !, @, #).",
                    "PasswordRequiresUpper" => "Password must include at least one uppercase letter.",
                    "PasswordRequiresDigit" => "Password must include at least one number.",
                    "PasswordTooShort" => "Password is too short — minimum 6 characters.",
                    _ => error.Description
                };
                ModelState.AddModelError(string.Empty, message);
            }

            return Page();
        }
    }
}
