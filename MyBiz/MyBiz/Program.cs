using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBiz_Business.Services.Abstracts;
using MyBiz_Business.Services.Concretes;
using MyBiz_Core.Models;
using MyBiz_Core.RepositoryAbstracts;
using MyBiz_Data.DAL;
using MyBiz_Data.RepositoryConcretes;

namespace MyBiz
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 7;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.User.RequireUniqueEmail = true;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                opt.Lockout.MaxFailedAccessAttempts = 3;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<ITeamRepository, TeamRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
             );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
