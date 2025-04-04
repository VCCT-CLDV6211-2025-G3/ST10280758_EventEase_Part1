/*
 * Justin Fussell ST10280758 Group 3
 */
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add the database context service
            builder.Services.AddDbContext<EventEaseDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Venue}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
//*******************************************************END OF FILE*****************************************************************