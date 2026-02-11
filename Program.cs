using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPageBooks.Data;
using RazorPageBooks.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Database connection
builder.Services.AddDbContext<RazorPageBooksContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("RazorPageBooksContext")
        ?? throw new InvalidOperationException("Connection string 'RazorPageBooksContext' not found.")
    ));

// Identity configuration
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // ?? Allow login without email confirmation
    options.SignIn.RequireConfirmedAccount = false;

    // Optional password settings (you can adjust)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<RazorPageBooksContext>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

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
