#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPageBooks.Areas.Identity.Pages.Account
{
    [Authorize]
    public class ForceChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ForceChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
                ErrorMessage = "Password must have 8+ chars, 1 uppercase, 1 lowercase, 1 number, and 1 special character.")]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        // ── GET ─────────────────────────────────────────────────────────
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var claims = await _userManager.GetClaimsAsync(user);
            var hasClaim = claims.Any(c =>
                c.Type == "MustChangePassword" && c.Value == "true");

            // Expose to the view so it can show the right notice
            ViewData["IsForcedChange"] = hasClaim;

            return Page();
        }

        // ── POST ────────────────────────────────────────────────────────
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // Verify the current password before allowing the change
            var passwordCheck = await _userManager.CheckPasswordAsync(user, CurrentPassword);
            if (!passwordCheck)
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                return Page();
            }

            // Change password using the verified current password
            var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
                return Page();
            }

            // Remove the force-change claim
            var claims = await _userManager.GetClaimsAsync(user);
            var claim = claims.FirstOrDefault(c => c.Type == "MustChangePassword");
            if (claim != null)
                await _userManager.RemoveClaimAsync(user, claim);

            // Refresh cookie so claim disappears immediately from HttpContext.User
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToPage("/Index");
        }
    }
}