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
        private readonly ILogger<LoginModel> _logger;

        // ✅ Default admin credentials
        private const string DefaultAdminUsername = "admin";
        private const string DefaultAdminEmail = "admin@bookcellar.com";
        private const string DefaultAdminPassword = "admin123";

        public LoginModel(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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

            // ✅ Seed the default admin account if it doesn't exist yet
            await SeedDefaultAdminAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/Index");

            if (ModelState.IsValid)
            {
                // ✅ Seed admin in case it was never created
                await SeedDefaultAdminAsync();

                IdentityUser user = await _userManager.FindByNameAsync(Input.UsernameOrEmail)
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
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(ReturnUrl);
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        // ✅ Creates the default admin account if it doesn't exist
        private async Task SeedDefaultAdminAsync()
        {
            var existingUser = await _userManager.FindByNameAsync(DefaultAdminUsername);
            if (existingUser == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = DefaultAdminUsername,
                    Email = DefaultAdminEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, DefaultAdminPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Default admin account created.");
                }
            }
        }
    }
}