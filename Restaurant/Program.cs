using Applications.MenuCategory_servic;
using Applications.MenuItem_servic;
using Applications.RepoInterfaces;
using Infrastructure.Context;
using Infrastructure.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Restaurant.Midleware;

namespace Restaurant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var conectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<Restaurantdb>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            builder.Services.AddScoped<IMenucateguryRepo, MenuCategoryRepo>();
            builder.Services.AddScoped<IMenuCategoryservis, MenuCategoryserves>();
            builder.Services.AddScoped<IMenuItemRepo, MenuItemRepo>();
            builder.Services.AddScoped<IMenuItem, MenuItemserves>();
            builder.Services.AddScoped<IUserServes, UserService>();
            builder.Services.AddScoped<IOrderRepo, OrderRepo>();
            builder.Services.AddScoped<IOrderServes, OrderServes>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;


            }).AddRoles<IdentityRole>().AddEntityFrameworkStores<Restaurantdb>();



           
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            // app.UseMiddleware<Time>();
            app.UseMiddleware<Totalpudgetof_mounth>();
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
