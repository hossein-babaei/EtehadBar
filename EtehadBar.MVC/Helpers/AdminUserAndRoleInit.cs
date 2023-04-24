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
                    Firstname = "حسین",
                    Lastname = "بابائی",
                    Email = "hossein.babaei.dev@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "09108897900",
                    PhoneNumberConfirmed = true,
                    RegisterDate = DateTime.Now,
                    UserName = "09108897900",
                    Role = EtehadBar.Domain.ApplicationRoleType.Admin
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

            if (!roleManager.RoleExistsAsync("RegisterUser").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "RegisterUser"
                };

                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("User").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "User"
                };

                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Milad").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Milad"
                };

                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            //سرمایه گذار
            if (!roleManager.RoleExistsAsync("Investor").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Investor"
                };

                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }

            //شرکا
            if (!roleManager.RoleExistsAsync("Partner").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Partner"
                };

                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
