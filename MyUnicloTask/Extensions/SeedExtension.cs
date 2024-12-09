using Ab108Uniqlo.Models;
using Ab108Uniqlo.Views.Account.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.Extensions
{
    public static class SeedExtension
    {
        public static void UseUserSeedDatas(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                CreateRoles(roleManager);
                CreateUsers(userManager);

            }
        }
        private static async void CreateUsers(UserManager<AppUser> _userManager)
        {
            if (!await _userManager.Users.AnyAsync(z => z.NormalizedUserName == "ADMIN"))
            {
                AppUser user = new AppUser();
                user.UserName = "admin";
                user.Email = "admin@gmail.com";
                user.Fullname = "admin";
                string role = nameof(Roles.Admin);
                await _userManager.CreateAsync(user, "123");
                await _userManager.AddToRoleAsync(user, role);
            }
        }
        private static async void CreateRoles(RoleManager<IdentityRole> _roleManager)
        {
            if (!await _roleManager.Roles.AnyAsync())
            {
                foreach (Roles role in Enum.GetValues(typeof(Roles)))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.GetRole()));
                }
            }
        }
    }
}
