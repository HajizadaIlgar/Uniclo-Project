using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Helpers;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.Services.Abstracts;
using Ab108Uniqlo.Services.Implements;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<UnicloDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("class"));
            });
            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = false;
                opt.Password.RequiredLength = 3;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Lockout.MaxFailedAccessAttempts = 4;
                //opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(20);
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<UnicloDbContext>();

            builder.Services.AddScoped<IEmailService, EmailService>();
            var opt = new SmtpOptions();
            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.Name));
            builder.Services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/login";
                opt.AccessDeniedPath = "/Home/AccessDenied";
            });

            //builder.Services.AddSession();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            //app.UseUserSeedDatas();
            //statik fayllarimi isletmek ucun 
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseSession();

            app.UseAuthorization();
            app.MapControllerRoute(
            name: "login",
            pattern: "login", new
            {
                Controller = "Account",
                Action = "Login"
            });
            app.MapControllerRoute(
           name: "register",
           pattern: "register", new
           {
               Controller = "Account",
               Action = "Register"
           });
            app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
