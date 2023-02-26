using EtehadBar.Domain;
using EtehadBar.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.MVC.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class InitialCacheMiddleware
    {
        private readonly RequestDelegate _next;

        public InitialCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IMemoryCache memoryCache, UserManager<ApplicationUser> userManager)
        {
            if (!memoryCache.TryGetValue(SystemCacheNames.UserProfileList.ToString(), out List<UserProfileCacheVM> profiles))
            {
                var data = await userManager.Users.AsNoTracking().Select(a => new UserProfileCacheVM
                {
                    Id = a.Id,
                    AccountBankName = a.AccountBankName,
                    BankAccountNumber = a.BankAccountNumber,
                    Firstname = a.Firstname,
                    Lastname = a.Lastname,
                    NationalId = a.NationalId,
                    Phonenumber = a.UserName,
                    Role = a.Role
                }).ToListAsync();
                memoryCache.Set(SystemCacheNames.UserProfileList.ToString(), data, new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(600)
                });
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class InitialCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseInitialCacheMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InitialCacheMiddleware>();
        }
    }
}
