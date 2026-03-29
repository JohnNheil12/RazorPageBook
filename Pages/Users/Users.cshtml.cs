using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RazorPageBooks.Pages.Users
{
    // ── Moved OUTSIDE IndexModel to fix CS0246 ──
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Staff";
        public DateTime? LastLoginDate { get; set; }
        public DateTime? JoinedDate { get; set; }
    }

    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<IdentityUser> userManager, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public List<UserViewModel> Users { get; set; } = new();
        public string CurrentUserId { get; set; } = string.Empty;

        public int TotalUsers { get; set; }
        public int ActiveToday { get; set; }
        public int AdminCount { get; set; }
        public int NewThisMonth { get; set; }

        // ────────────────────────────────────────────
        // GET
        // ────────────────────────────────────────────
        public async Task OnGetAsync()
        {
            // HttpContext.User avoids CS0119 ("User is a type" error on PageModel)
            CurrentUserId = _userManager.GetUserId(HttpContext.User) ?? string.Empty;

            var allUsers = await _userManager.Users.ToListAsync();

            var list = new List<UserViewModel>();

            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var role = roles.Contains("Admin") ? "Admin" : "Staff";

                // Standard IdentityUser has no LastLoginDate / JoinedDate columns.
                // They will show as null / "Never" in the UI.
                // See the note at the bottom of this file if you want to add them.
                list.Add(new UserViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName ?? u.Email ?? "Unknown",
                    Email = u.Email ?? "—",
                    Role = role,
                    LastLoginDate = null,
                    JoinedDate = null,
                });
            }

            Users = list.OrderBy(u => u.UserName).ToList();

            TotalUsers = Users.Count;
            AdminCount = Users.Count(u => u.Role == "Admin");
            ActiveToday = 0; // requires LastLoginDate column
            NewThisMonth = 0; // requires JoinedDate column
        }

        // ────────────────────────────────────────────
        // POST – Delete
        // ────────────────────────────────────────────
        public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["DeleteError"] = "Invalid user ID.";
                return RedirectToPage();
            }

            var currentId = _userManager.GetUserId(HttpContext.User);
            if (userId == currentId)
            {
                TempData["DeleteError"] = "You cannot delete your own account.";
                return RedirectToPage();
            }

            var target = await _userManager.FindByIdAsync(userId);
            if (target == null)
            {
                TempData["DeleteError"] = "User not found.";
                return RedirectToPage();
            }

            var result = await _userManager.DeleteAsync(target);

            if (result.Succeeded)
            {
                _logger.LogInformation("Admin deleted user {Id} ({Email})", userId, target.Email);
                TempData["DeleteSuccess"] = true;
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["DeleteError"] = $"Delete failed: {errors}";
            }

            return RedirectToPage();
        }
    }
}
