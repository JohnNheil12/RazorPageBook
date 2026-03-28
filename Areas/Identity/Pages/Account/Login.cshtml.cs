#nullable disable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPageBooks.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<LoginModel> _logger;

        // ── Default admin credentials ──
        private const string DefaultAdminUsername = "admin";
        private const string DefaultAdminEmail = "admin@bookcellar.com";
        private const string DefaultAdminPassword = "admin123";
        private const string AdminRole = "Admin";

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username or Email")]
            public string UsernameOrEmail { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/Index");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            await SeedDefaultAdminAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/Index");

            if (!ModelState.IsValid)
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            await SeedDefaultAdminAsync();

            // Allow login by username OR email
            var user = await _userManager.FindByNameAsync(Input.UsernameOrEmail)
                    ?? await _userManager.FindByEmailAsync(Input.UsernameOrEmail);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {UserName} logged in.", user.UserName);
                    return LocalRedirect(ReturnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {UserName} account locked out.", user.UserName);
                    ModelState.AddModelError(string.Empty, "Account is locked out. Please try again later.");
                    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                    return Page();
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        // ── Ensures the Admin role exists, then creates the default admin account ──
        private async Task SeedDefaultAdminAsync()
        {
            // 1. Create the "Admin" role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync(AdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(AdminRole));
                _logger.LogInformation("Created role: {Role}", AdminRole);
            }

            // 2. Create the "Customer" role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
                _logger.LogInformation("Created role: Customer");
            }

            // 3. Create default admin user if not present
            var existingAdmin = await _userManager.FindByNameAsync(DefaultAdminUsername);
            if (existingAdmin == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = DefaultAdminUsername,
                    Email = DefaultAdminEmail,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(adminUser, DefaultAdminPassword);
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, AdminRole);
                    _logger.LogInformation("Default admin account created and assigned Admin role.");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                        _logger.LogError("Admin seed error: {Error}", error.Description);
                }
            }
            else
            {
                // 4. If admin exists but somehow lost the role, re-assign it
                if (!await _userManager.IsInRoleAsync(existingAdmin, AdminRole))
                {
                    await _userManager.AddToRoleAsync(existingAdmin, AdminRole);
                    _logger.LogInformation("Re-assigned Admin role to existing admin user.");
                }
            }
        }
    }
}
