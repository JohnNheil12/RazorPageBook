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

        // Store ReturnUrl to preserve the redirect URL between requests
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
            // If no returnUrl specified, default to homepage
            ReturnUrl = returnUrl ?? Url.Content("~/Index");

            // Clear any existing external cookie to ensure clean login
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Load any external login schemes (e.g., Google, Facebook)
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Preserve the returnUrl between requests, default to homepage if missing
            ReturnUrl = returnUrl ?? Url.Content("~/Index");

            if (ModelState.IsValid)
            {
                // Attempt to find user by username or email
                IdentityUser user = await _userManager.FindByNameAsync(Input.UsernameOrEmail)
                                    ?? await _userManager.FindByEmailAsync(Input.UsernameOrEmail);

                if (user != null)
                {
                    // Attempt password sign-in
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

                // If login failed
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // If we got here, something failed, redisplay form
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }
    }
}
