using EtehadBar.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace Helpers
{
    public static class AdminUserAndRoleInit
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByNameAsync("09108897900").Result == null)
            {
                var user = new ApplicationUser()
                {
                    Firstname = "ادمین",
                    Lastname = "سیستم",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "09108897900",
                    PhoneNumberConfirmed = true,
                    RegisterDate = DateTime.Now,
                    UserName = "09108897900",
                    Role = (byte)EtehadBar.Domain.ApplicationRoles.Admin
                };

                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rdAdmin!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Admin"
                };

                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
