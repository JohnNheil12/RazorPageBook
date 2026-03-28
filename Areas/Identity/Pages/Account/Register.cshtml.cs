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
    [Authorize]
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

        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

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

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }
        }

        public IActionResult OnGet()
        {
            if (User.Identity.Name != "admin")
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.Identity.Name != "admin")
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (!ModelState.IsValid)
                return Page();

            // Validate role input
            if (Input.Role != "Admin" && Input.Role != "Staff")
            {
                ModelState.AddModelError(string.Empty, "Invalid role selected.");
                return Page();
            }

            // Check duplicate email
            var existingEmail = await _userManager.FindByEmailAsync(Input.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Email already in use.");
                return Page();
            }

            // Use FirstName.LastName as username internally
            var username = $"{Input.FirstName}.{Input.LastName}".ToLower();

            // Check duplicate username
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "A user with that first and last name already exists.");
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
                StatusMessage = $"✅ User '{Input.FirstName} {Input.LastName}' created successfully as {Input.Role}!";
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