#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPageBooks.Areas.Identity.Pages.Account
{
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

        [TempData]
        public string SuccessMessage { get; set; }

        public class InputModel
        {

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
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var existingEmail = await _userManager.FindByEmailAsync(Input.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Input.Email", "This email address is already in use.");
                return Page();
            }

            var newUser = new IdentityUser
            {
                UserName = Input.Email,
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

                // ✅ Store JoinedDate as a claim so the Users page can display it
                await _userManager.AddClaimAsync(newUser, new Claim(
                    "JoinedDate",
                    DateTime.UtcNow.ToString("o") // ISO 8601 round-trip format
                ));


                // ✅ Redirect to Dashboard with ?tab=users&registered=true
                // Dashboard's DOMContentLoaded reads these params, opens the Users
                // tab automatically, and shows the success toast.
                return Redirect("/?tab=users&registered=true");
            }

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