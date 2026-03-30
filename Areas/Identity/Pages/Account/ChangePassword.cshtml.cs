// ============================================================
//  ForceChangePassword.cshtml.cs
//  Location: Areas/Identity/Pages/Account/ForceChangePassword.cshtml.cs
// ============================================================
//
//  WHY THE PREVIOUS VERSION REDIRECTED TO /Index
//  ─────────────────────────────────────────────
//  The middleware ran BEFORE the auth cookie was fully read, or
//  the Index page's [AllowAnonymous] let it bypass the check.
//  The guaranteed fix is to hook the redirect directly inside
//  Login.cshtml.cs — right after PasswordSignInAsync succeeds.
//
// ============================================================
//  COMPLETE SETUP — follow ALL four steps
// ============================================================
//
//  ── STEP 1: This file + ForceChangePassword.cshtml ──────────
//  Place both files at:
//    Areas/Identity/Pages/Account/ForceChangePassword.cshtml
//    Areas/Identity/Pages/Account/ForceChangePassword.cshtml.cs
//
//  ── STEP 2: Patch Login.cshtml.cs ───────────────────────────
//  In your Login.cshtml.cs OnPostAsync(), find the block that
//  handles a successful SignInResult.Succeeded and ADD the
//  claim check BEFORE the normal returnUrl redirect:
//
//    var result = await _signInManager.PasswordSignInAsync(...);
//    if (result.Succeeded)
//    {
//        _logger.LogInformation("User logged in.");
//
//        // ★ ADD THIS BLOCK ★
//        var user = await _userManager.GetUserAsync(User)
//                   ?? await _userManager.FindByEmailAsync(Input.Email);
//        if (user != null)
//        {
//            var claims = await _userManager.GetClaimsAsync(user);
//            if (claims.Any(c => c.Type == "MustChangePassword" && c.Value == "true"))
//                return RedirectToPage("/Account/ForceChangePassword",
//                                      new { area = "Identity" });
//        }
//        // ★ END ADDED BLOCK ★
//
//        return LocalRedirect(returnUrl);
//    }
//
//  ── STEP 3: Program.cs middleware (safety net) ──────────────
//  Add AFTER app.UseAuthentication() AND app.UseAuthorization():
//
//    app.Use(async (context, next) =>
//    {
//        if (context.User.Identity?.IsAuthenticated == true
//            && context.User.HasClaim("MustChangePassword", "true"))
//        {
//            var path = context.Request.Path.Value ?? "";
//            bool allowed =
//                path.StartsWith("/Identity/Account/ForceChangePassword",
//                                StringComparison.OrdinalIgnoreCase) ||
//                path.StartsWith("/Identity/Account/Logout",
//                                StringComparison.OrdinalIgnoreCase) ||
//                path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase) ||
//                path.StartsWith("/css",        StringComparison.OrdinalIgnoreCase) ||
//                path.StartsWith("/js",         StringComparison.OrdinalIgnoreCase) ||
//                path.StartsWith("/lib",        StringComparison.OrdinalIgnoreCase);
//            if (!allowed)
//            {
//                context.Response.Redirect("/Identity/Account/ForceChangePassword");
//                return;
//            }
//        }
//        await next();
//    });
//
//  ── STEP 4: Registration — stamp the claim on new users ─────
//  After _userManager.CreateAsync(newUser, password) succeeds:
//
//    await _userManager.AddClaimAsync(newUser,
//        new System.Security.Claims.Claim("MustChangePassword", "true"));
//
// ============================================================

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

            // Password-reset flow — no current password required
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, NewPassword);

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
