using EtehadBar.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EtehadBar.MVC.Filters
{
    public class ActionLogFilter : Attribute, IActionFilter
    {
        private readonly IMemoryCache _memoryCache;
        private readonly List<UserProfileCacheVM> profiles;

        public ActionLogFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            profiles = _memoryCache.Get<List<UserProfileCacheVM>>(SystemCacheNames.UserProfileList.ToString());
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var httpContext = context.HttpContext;
            var routeValues = (ControllerActionDescriptor)context.ActionDescriptor;
            string controllerName = routeValues.ControllerName;
            string actionName = routeValues.ActionName;

            string userId = ExtractUserId(httpContext);

            if (!actionName.ToLower().Equals("beforeunload") && !actionName.ToLower().Equals("adduseractivity") && !actionName.ToLower().Equals("getclientsidetranslates"))
            {
                string userFullname = ExtractUserFullname(userId);

                Log.ForContext("EventType", (int)SystemLogEventType.ActionExecuted)
                        .ForContext("UserId", userId)
                        .ForContext("UserFullname", userFullname)
                        .Information("Request From [{@Method}][{@Path}] Has Been Completed.", httpContext.Request.Method, $"/{controllerName}/{actionName}");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var routeValues = (ControllerActionDescriptor)context.ActionDescriptor;
            string controllerName = routeValues.ControllerName;
            string actionName = routeValues.ActionName;

            string userId = ExtractUserId(httpContext);

            if (!controllerName.ToLower().Equals("home"))
            {
                var formFiles = new List<LogDetailFormFileVM>();
                if (httpContext.Request.HasFormContentType)
                    if (httpContext.Request.Form.Count > 0)
                    {
                        var form = httpContext.Request.Form;
                        //foreach (var item in form.Keys.Where(a => !a.Equals("__RequestVerificationToken")))
                        //    formData.Add(new LogDetailFormData(item, form[item].ToString().Replace("[", "").Replace("]", "")));

                        if (form.Files.Any())
                            foreach (var file in form.Files)
                                formFiles.Add(new LogDetailFormFileVM(file.FileName, file.Length, file.ContentType));
                    }

                string userFullname = ExtractUserFullname(userId);

                Log.ForContext("EventType", (int)SystemLogEventType.ActionExecuting)
                        .ForContext("UserId", userId)
                        .ForContext("UserFullname", userFullname)
                        .ForContext("FormFiles", formFiles, true)
                        .Information("Request From [{@Method}][{@Path}] Has Been Started.", httpContext.Request.Method, $"/{controllerName}/{actionName}");
            }
        }

        private static string ExtractUserId(HttpContext httpContext)
        {
            string userId;
            if (httpContext.User.Identity.IsAuthenticated)
                userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            else
            {
                if (httpContext.Request.Cookies.ContainsKey("Activity"))
                    userId = httpContext.Request.Cookies["Activity"];
                else
                {
                    userId = DateTime.Now.Ticks.ToString();
                    httpContext.Response.Cookies.Append("Activity", userId, new CookieOptions
                    {
                        IsEssential = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddYears(1)
                    });
                }
            }

            return userId;
        }

        private string ExtractUserFullname(string userId)
        {
            string userFullname = "Anonymous";
            var userProfile = profiles.SingleOrDefault(a => a.Id.Equals(userId));
            if (userProfile is not null)
                userFullname = userProfile.Firstname + " " + userProfile.Lastname;
            return userFullname;
        }
    }
}
