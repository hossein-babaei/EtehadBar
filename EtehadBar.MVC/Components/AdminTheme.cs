using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bioop.Components
{
    public class AdminTheme : ViewComponent
    {
        private readonly IAdminThemeRepository _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminTheme(IAdminThemeRepository context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            HttpContext.Request.Cookies.TryGetValue("parsmvcTheme", out string cookie);
            if (cookie != null)
            {
                ViewData["color"] = cookie.ToLower();
            }
            else
            {
                var coockieOptions = new CookieOptions
                {
                    IsEssential = true,
                    Expires = DateTimeOffset.Now.AddYears(1),
                    HttpOnly = true,
                    Path = HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase.ToString() : "/",
                    Secure = HttpContext.Request.IsHttps
                };
                var userTheme = await _context.GetByUserId(_userManager.GetUserId(UserClaimsPrincipal));
                if (userTheme != null)
                {
                    ViewData["color"] = userTheme.Theme.ToLower();
                    HttpContext.Response.Cookies.Append("parsmvcTheme", userTheme.Theme.ToLower(), coockieOptions);
                }
                else
                {
                    ViewData["color"] = "blue";
                    HttpContext.Response.Cookies.Append("parsmvcTheme", "blue", coockieOptions);
                }
            }
            return View("AdminTheme");
        }
    }
}
