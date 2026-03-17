using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Data;
using RazorPageBooks.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddRazorPages();

// 2. Database connection
builder.Services.AddDbContext<RazorPageBooksContext>(options =>
    options.UseSqlite("Data Source=RazorPageBooks.db"));

// 3. Identity configuration
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<RazorPageBooksContext>();

// ? 4. Cookie configuration for Remember Me
builder.Services.ConfigureApplicationCookie(options =>
{
    // When RememberMe is checked ? stay logged in for 30 days
    options.ExpireTimeSpan = TimeSpan.FromDays(30);

    // Resets the 30-day timer on each visit
    options.SlidingExpiration = true;

    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
});

var app = builder.Build();

// 5. Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Naay sayop sa pag-seed sa database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();