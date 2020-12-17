using EtehadBar.Domain;
using EtehadBar.Domain.Models;
using EtehadBar.MVC.Models;
using Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EtehadBar.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(
            IMemoryCache memoryCache,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _memoryCache = memoryCache;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("developer-info")]
        public IActionResult DeveloperInfo()
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            return Json(new Dictionary<string, string>
            {
                ["Developed By"] = "Ho3ein Babaei",
                ["Phone Number"] = "+989108897900",
                ["Company"] = "https://parsmvc.ir",
                ["Email"] = "info@parsmvc.ir",
                ["Gmail"] = "hossein.babaei.dev@gmail.com",
                ["Specialty"] = "Expert in Microsoft .NET Framework (ASP.NET MVC & ASP.NET Core MVC) and front end goodies"
            }, settings);
        }
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                TempData["msg"] = "شما در حال حاضر انلاین هستید. |danger";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM Input)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                Input.Password = Input.Password.PersianToEnglish();
                Input.Username = Input.Username.PersianToEnglish();
                var result = await _signInManager.PasswordSignInAsync(userName: Input.Username, password: Input.Password, isPersistent: Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(Input.Username);

                    //if (await _userManager.IsInRoleAsync(user, "Admin"))
                    //{
                    //    return RedirectToAction("Index", "Admin");
                    //}
                    return RedirectToAction("Index", "Home");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    var user = await _userManager.FindByNameAsync(Input.Username);
                    if (!string.IsNullOrWhiteSpace(user.LockoutReason))
                    {
                        TempData["msg"] = user.LockoutReason + " |danger";
                    }
                    return RedirectToAction("Lockout");
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(Input.Username);
                    if (!await _userManager.IsPhoneNumberConfirmedAsync(user))
                    {
                        TempData["msg"] = "شماره تلفن همراه شما تایید نشده است. |danger";
                        return RedirectToAction("SendNewCode", new { phoneNumber = Input.Username });
                    }
                    ModelState.AddModelError(string.Empty, "خطای ورود به سیستم! شماره تلفن همراه یا کلمه عبور را اشتباه وارد کرده باشید.");
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            TempData["msg"] = "با موفقیت از سیستم خارج شدید. |success";
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Lockout()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmPhoneNumber()
        {
            var mobile = _memoryCache.Get("PhoneNumber");
            if (mobile != null)
            {
                ViewData["PhoneNumber"] = (string)mobile;
                return View();
            }
            else
            {
                return RedirectToAction("SendNewCode");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPhoneNumber(ConfirmPhoneNumberVM Input)
        {
            if (ModelState.IsValid)
            {
                Input.Code = Input.Code.PersianToEnglish();
                Input.PhoneNumber = Input.PhoneNumber.PersianToEnglish();
                var user = await _userManager.FindByNameAsync(Input.PhoneNumber);
                var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, Input.Code);
                if (result.Succeeded)
                {
                    await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["msg"] = "شماره موبایل شما تایید شد. لطفا وارد شوید. |success";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["msg"] = "عملیات با خطا مواجه شد. خطا در برقراری ارتباط با سرور. |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. کد وارد شده صحیح نیست. |danger";
                }
            }
            TempData["msg"] = "درخواست شما غیر مجاز است. |danger";

            var mobile = _memoryCache.Get("PhoneNumber");
            if (mobile != null)
            {
                ViewData["PhoneNumber"] = (string)mobile;
                return View();
            }
            else
            {
                return RedirectToAction("SendNewCode");
            }
        }

        [HttpGet]
        public IActionResult SendNewCode(string phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                ViewData["phoneNumber"] = phoneNumber;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendNewCode(SendNewCodeVM Input)
        {
            if (ModelState.IsValid)
            {
                Input.PhoneNumber = Input.PhoneNumber.PersianToEnglish();
                var user = await _userManager.FindByNameAsync(Input.PhoneNumber);
                if (user != null)
                {
                    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

                    var res = /*await _smsSender.SendPattern(user.PhoneNumber, code, "activate");*/ "ok";
                    if (res.Equals("ok"))
                    {

                        ViewData["Code"] = code;

                        _memoryCache.Set("PhoneNumber", user.PhoneNumber, new MemoryCacheEntryOptions { Priority = CacheItemPriority.High, SlidingExpiration = TimeSpan.FromMinutes(20) });
                        ViewData["PhoneNumber"] = Input.PhoneNumber;
                        return RedirectToAction("ConfirmPhoneNumber");
                    }
                }
            }
            TempData["msg"] = "درخواست شما غیر مجاز است. |danger";
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(SendNewCodeVM Input)
        {
            if (ModelState.IsValid)
            {
                Input.PhoneNumber = Input.PhoneNumber.PersianToEnglish();
                var user = await _userManager.FindByNameAsync(Input.PhoneNumber);
                if (user == null || !(await _userManager.IsPhoneNumberConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("ResetPassword");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var res = /*await _smsSender.SendPattern(user.PhoneNumber, code, "reset")*/ "ok";
                if (res.Equals("ok"))
                {

                    ViewData["Code"] = code;

                    return RedirectToAction("ResetPassword");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713

                //var callbackUrl = Url.Page(
                //    "/Account/ResetPassword",
                //    pageHandler: null,
                //    values: new { code },
                //    protocol: Request.Scheme);

                //await _emailSender.SendEmailAsync(
                //    Input.PhoneNumber,
                //    "Reset Password",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM Input)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Input.PhoneNumber = Input.PhoneNumber.PersianToEnglish();
            Input.Code = Input.Code.PersianToEnglish();
            Input.Password = Input.Password.PersianToEnglish();

            var user = await _userManager.FindByNameAsync(Input.PhoneNumber);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
