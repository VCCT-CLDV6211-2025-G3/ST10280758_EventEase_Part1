using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using Microsoft.AspNetCore.Identity;
using EventEase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register IWebHostEnvironment and IConfiguration
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var environment = builder.Environment.EnvironmentName;
string? connectionString = environment == "Development"
    ? builder.Configuration.GetConnectionString("LocalConnection")
    : builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured.");
}

builder.Services.AddDbContext<EventEaseDbContext>(options =>
    options.UseSqlServer(connectionString!));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<EventEaseDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Venue}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var context = services.GetRequiredService<EventEaseDbContext>();

        string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var role = new IdentityRole(adminRole);
            var roleResult = await roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
            {
                throw new Exception("Failed to create Admin role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }

        string adminEmail = "admin@eventease.com";
        string adminPassword = "Admin123";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
            else
            {
                throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }

        if (!context.Venue.Any())
        {
            context.Venue.AddRange(
                new Venue { VenueName = "Conference Hall A", Location = "Downtown", Capacity = 500 },
                new Venue { VenueName = "Stadium B", Location = "Uptown", Capacity = 1000 },
                new Venue { VenueName = "Auditorium C", Location = "Midtown", Capacity = 300 }
            );
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
        throw;
    }
}

app.Run();