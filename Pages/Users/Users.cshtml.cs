using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Antiforgery;
using System.Security.Claims;

namespace RazorPageBooks.Pages.Users
{
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
        private readonly IAntiforgery _antiforgery;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            ILogger<IndexModel> logger,
            IAntiforgery antiforgery)
        {
            _userManager = userManager;
            _logger = logger;
            _antiforgery = antiforgery;
        }

        public List<UserViewModel> Users { get; set; } = new();
        public string CurrentUserId { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int ActiveToday { get; set; }
        public int AdminCount { get; set; }
        public int NewThisMonth { get; set; }

        // GET – main page
        public async Task OnGetAsync()
        {
            CurrentUserId = _userManager.GetUserId(HttpContext.User) ?? string.Empty;

            var allUsers = await _userManager.Users.ToListAsync();
            var list = new List<UserViewModel>();

            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var role = roles.Contains("Admin") ? "Admin" : "Staff";

                var claims = await _userManager.GetClaimsAsync(u);

                DateTime? joinedDate = null;
                var joinedClaim = claims.FirstOrDefault(c => c.Type == "JoinedDate");
                if (joinedClaim != null && DateTime.TryParse(joinedClaim.Value, out var pj))
                    joinedDate = pj;

                DateTime? lastLoginDate = null;
                var loginClaim = claims.FirstOrDefault(c => c.Type == "LastLoginDate");
                if (loginClaim != null && DateTime.TryParse(loginClaim.Value, out var pl))
                    lastLoginDate = pl;

                list.Add(new UserViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName ?? u.Email ?? "Unknown",
                    Email = u.Email ?? "---",
                    Role = role,
                    LastLoginDate = lastLoginDate,
                    JoinedDate = joinedDate,
                });
            }

            Users = list.OrderBy(u => u.UserName).ToList();
            TotalUsers = Users.Count;
            AdminCount = Users.Count(u => u.Role == "Admin");
            var now = DateTime.UtcNow;
            ActiveToday = Users.Count(u => u.LastLoginDate.HasValue && u.LastLoginDate.Value.Date == now.Date);
            NewThisMonth = Users.Count(u => u.JoinedDate.HasValue
                                         && u.JoinedDate.Value.Year == now.Year
                                         && u.JoinedDate.Value.Month == now.Month);
        }

        // GET handler=AntiforgeryToken
        // Returns a fresh token as JSON so JS can always use a valid token.
        public IActionResult OnGetAntiforgeryToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return new JsonResult(new { token = tokens.RequestToken });
        }

        // POST handler=DeleteUser
        // Accepts plain FormData: userId + __RequestVerificationToken
        // Returns JSON { success, error }
        public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new JsonResult(new { success = false, error = "Invalid user ID." }) { StatusCode = 400 };

            var currentId = _userManager.GetUserId(HttpContext.User);
            if (userId == currentId)
                return new JsonResult(new { success = false, error = "You cannot delete your own account." }) { StatusCode = 400 };

            var target = await _userManager.FindByIdAsync(userId);
            if (target == null)
                return new JsonResult(new { success = false, error = "User not found." }) { StatusCode = 404 };

            var roles = await _userManager.GetRolesAsync(target);
            if (roles.Contains("Admin"))
                return new JsonResult(new { success = false, error = "Admin accounts cannot be deleted." }) { StatusCode = 403 };

            var result = await _userManager.DeleteAsync(target);

            if (result.Succeeded)
            {
                _logger.LogInformation("Admin deleted user {Id} ({Email})", userId, target.Email);
                return new JsonResult(new { success = true });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new JsonResult(new { success = false, error = errors }) { StatusCode = 400 };
        }
    }
}