using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using Helpers;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace EtehadBar.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminThemeRepository _adminThemeRepo;
        private readonly ICalendarRepository _calendarRepo;
        private readonly IConfigRepository _configRepo;
        private readonly IContractRepository _contractRepo;
        private readonly ICostRepository _costRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IDefinitionRepository _definitionRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            IAdminThemeRepository adminThemeRepository,
            ICalendarRepository calendarRepository,
            IConfigRepository configRepository,
            IContractRepository contractRepository,
            ICostRepository costRepository,
            ICustomerRepository customerRepository,
            IDefinitionRepository definitionRepository,
            IPaymentRepository paymentRepository,
            IVehicleRepository vehicleRepository,
            IWebHostEnvironment environment,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _adminThemeRepo = adminThemeRepository;
            _calendarRepo = calendarRepository;
            _configRepo = configRepository;
            _contractRepo = contractRepository;
            _costRepo = costRepository;
            _customerRepo = customerRepository;
            _definitionRepo = definitionRepository;
            _paymentRepo = paymentRepository;
            _vehicleRepo = vehicleRepository;
            _environment = environment;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("{controller:slugify}/{action:slugify}/{color}")]
        public async Task<IActionResult> ChangeTheme(string color)
        {
            string userId = _userManager.GetUserId(User);
            var theme = await _adminThemeRepo.GetByUserId(userId);
            if (theme != null)
            {
                theme.Theme = color;
                _adminThemeRepo.Update(theme);
            }
            else
            {
                _adminThemeRepo.Create(new AdminTheme
                {
                    Theme = color,
                    UserId = userId
                });
            }

            HttpContext.Request.Cookies.TryGetValue("parsmvcTheme", out string cookie);
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                if (HttpContext.Request.Cookies.ContainsKey("parsmvcTheme"))
                {
                    HttpContext.Response.Cookies.Delete("parsmvcTheme");
                }
            }
            HttpContext.Response.Cookies.Append("parsmvcTheme", color, new CookieOptions
            {
                IsEssential = true,
                Expires = DateTimeOffset.Now.AddYears(1),
                HttpOnly = true,
                Path = HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase.ToString() : "/",
                Secure = HttpContext.Request.IsHttps
            });
            await _adminThemeRepo.Save();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<IActionResult> Config()
        {
            return View(await _configRepo.First());
        }

        [HttpPost]
        public async Task<JsonResult> Config(Config c)
        {
            string status = "danger";
            string msg;

            if (string.IsNullOrWhiteSpace(c.Year))
            {
                msg = "تعداد رقم سال باید 4 رقم باشد.";
                return Json(new { msg, status });
            }
            else
            {
                if (c.Year.Length != 4)
                {
                    msg = "تعداد رقم سال باید 4 رقم باشد.";
                    return Json(new { msg, status });
                }
            }

            if (ModelState.IsValid)
            {
                if (!c.Year.PersianToEnglish().isNumber())
                {
                    msg = "سال باید یک عدد باشد.";
                    return Json(new { msg, status });
                }

                _configRepo.Update(c);
                try
                {
                    await _configRepo.Save();
                    msg = "عملیات موفقیت آمیز بود.";
                    status = "success";
                }
                catch (DbUpdateException e)
                {
                    msg = "عملیات با خطا مواجه شد. جزئیات: " + e.Message;
                }
            }
            else
            {
                msg = "عملیات با خطا مواجه شد. جزئیات: خطای اعتبار سنجی فرم رخ داده است؛ لطفا فرم را بررسی کنید.";
            }
            return Json(new { msg, status });
        }

        #region Users
        [HttpGet]
        public async Task<IActionResult> GetUserListPartial(int? p, string name)
        {
            var data = _userManager.Users.Where(a => a.Status);

            if (!string.IsNullOrWhiteSpace(name))
            {
                data = data.Where(a => (a.Firstname + " " + a.Lastname).Contains(name) || a.Firstname.Contains(name) || a.Lastname.Contains(name));
            }

            var pageNumber = p ?? 1;
            var onePageOfData = await data.OrderByDescending(a => a.RegisterDate).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            ViewBag.name = name;
            return PartialView("_UserList");
        }

        [HttpPost]
        public async Task<ActionResult> UserLock(string id, string note)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(note))
                    {
                        user.LockoutReason = note;
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (!updateResult.Succeeded)
                        {
                            TempData["msg"] = "عملیات با خطا مواجه شد. (update note) |danger";
                            return Redirect(Request.Headers["Referer"].ToString());
                        }
                    }
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    var result = await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(10));
                    if (result.Succeeded)
                    {
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    else
                    {
                        TempData["msg"] = "عملیات با خطا مواجه شد. |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "کاربر پیدا نشد. |danger";
                }
            }
            else
            {
                TempData["msg"] = "درخواست غیر مجاز. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<ActionResult> UserUnlock(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.LockoutReason = "";
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        TempData["msg"] = "عملیات با خطا مواجه شد. (update note) |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                    var result = await _userManager.SetLockoutEndDateAsync(user, null);
                    if (result.Succeeded)
                    {
                        TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    }
                    else
                    {
                        TempData["msg"] = "عملیات با خطا مواجه شد. |danger";
                    }
                }
                else
                {
                    TempData["msg"] = "کاربر پیدا نشد. |danger";
                }
            }
            else
            {
                TempData["msg"] = "درخواست غیر مجاز. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<JsonResult> EditUserRole(string userId, string roleName, bool Value)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (Value)
            {
                var result = await _userManager.AddToRoleAsync(appUser, roleName);
                if (result.Succeeded)
                    return Json(true);
                else
                    return Json(false);

            }
            else
            {
                var result = await _userManager.RemoveFromRoleAsync(appUser, roleName);
                if (result.Succeeded)
                    return Json(true);
                else
                    return Json(false);
            }
        }

        public async Task<IActionResult> Users(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _userManager.Users.OrderByDescending(a => a.RegisterDate).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchUser(int? pageNumber, string param, string filterBy)
        {
            if (!string.IsNullOrWhiteSpace(param))
            {
                IQueryable<ApplicationUser> i;
                if (filterBy == "nationalId")
                {
                    i = _userManager.Users.Where(a => a.NationalId.Contains(param));
                }
                else if (filterBy == "mobile")
                {
                    i = _userManager.Users.Where(a => a.PhoneNumber.Contains(param));
                }
                else
                {
                    i = _userManager.Users.Where(a => (a.Firstname + " " + a.Lastname).Contains(param) || a.Firstname.Contains(param) || a.Lastname.Contains(param));
                }
                var users = i.AsNoTracking().OrderByDescending(a => a.RegisterDate);
                var pageNum = pageNumber ?? 1;
                var onePageOfData = await users.ToPagedListAsync(pageNum, 15);
                ViewBag.data = onePageOfData;
                ViewBag.page = pageNum;
                ViewBag.param = param;
                ViewBag.filterBy = filterBy;
                return PartialView("_User");
            }
            else
            {
                return BadRequest("لطفا یک مقدار برای جستجو انتخاب نمائید.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            ViewData["userRoles"] = await _userManager.GetRolesAsync(user);
            ViewData["roles"] = await _roleManager.Roles.OrderBy(a => a.Name).ToListAsync();

            return View("~/Views/Admin/Edit/User.cshtml", new EditUserVM
            {
                Avatar = user.Avatar,
                Birth = user.Birth,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Firstname = user.Firstname,
                Id = user.Id,
                Lastname = user.Lastname,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                NationalId = user.NationalId,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Tel = user.Tel
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserVM u, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                u.NationalId = u.NationalId.PersianToEnglish();
                if (!u.NationalId.isNumber())
                {
                    TempData["msg"] = "کد ملی وارد شده باید یک عدد باشد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (await _userManager.Users.AnyAsync(a => a.NationalId.Equals(u.NationalId) && !a.Id.Equals(u.Id)))
                {
                    TempData["msg"] = "کد ملی وارد شده در سیستم وجود دارد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                u.PhoneNumber = u.PhoneNumber.PersianToEnglish();
                if (!u.PhoneNumber.isNumber())
                {
                    TempData["msg"] = "شماره تلفن همراه وارد شده باید یک عدد باشد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (await _userManager.Users.AnyAsync(a => a.PhoneNumber.Equals(u.PhoneNumber) && !a.Id.Equals(u.Id)))
                {
                    TempData["msg"] = "شماره تلفن همراه وارد شده در سیستم ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var user = await _userManager.FindByIdAsync(u.Id);

                string[] b = u.BirthString.PersianToEnglish().Split('/');
                user.Birth = new PersianDateTime(Convert.ToInt32(b[0]), Convert.ToInt32(b[1]), Convert.ToInt32(b[2])).ToDateTime();
                user.Firstname = u.Firstname;
                user.Lastname = u.Lastname;
                user.Email = u.Email;
                user.EmailConfirmed = u.EmailConfirmed;
                user.NationalId = u.NationalId;
                user.Tel = u.Tel;
                user.PhoneNumber = u.PhoneNumber;
                user.PhoneNumberConfirmed = u.PhoneNumberConfirmed;

                var validTypes = new string[] { "image/jpeg", "image/png" };
                if (pic != null)
                {
                    if (validTypes.Contains(pic.ContentType))
                    {
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                        using (var stream = new FileStream(Path.Combine(_environment.WebRootPath, "img\\user", fileName), FileMode.Create))
                        {
                            await pic.CopyToAsync(stream);
                        }
                        if (!string.IsNullOrEmpty(user.Avatar))
                        {
                            try
                            {
                                System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\user", user.Avatar));
                            }
                            catch (Exception e)
                            {
                                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                                return Redirect(Request.Headers["Referer"].ToString());
                            }
                        }
                        user.Avatar = fileName;
                    }
                    else
                    {
                        TempData["msg"] = $"فرمت فایل های ارسالی مجاز نیست. باید png یا jpg ارسال شود. |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                }

                var update = await _userManager.UpdateAsync(user);
                if (update.Succeeded)
                {
                    TempData["msg"] = "عملیات موفقیت آمیز بود |success";
                    return RedirectToAction("Users");
                }
                else
                {
                    string error = "";
                    foreach (var item in update.Errors)
                    {
                        if (item.Equals(update.Errors.Last()))
                        {
                            error = error + item.Code + " " + item.Description;
                        }
                        else
                        {
                            error = error + item.Code + " " + item.Description + " | ";
                        }
                    }
                    TempData["msg"] = $"خطا در ذخیره اطلاعات و برقراری ارتباط با پایگاه داده رخ داده است. لطفا مجدد تلاش کنید. جزئیات: {error} |danger";
                }
            }
            else
            {
                TempData["msg"] = "خطای اعتبار سنجی رخ داده است. لطفا فرم را بررسی کنید |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public PartialViewResult GetUserCreateForm(byte type)
        {
            ViewBag.type = type;
            if (type.Equals((byte)ApplicationRoles.Driver))
            {
                return PartialView("~/Views/Admin/Create/Driver.cshtml");
            }
            else
            {
                return PartialView("~/Views/Admin/Create/User.cshtml");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserVM u)
        {
            if (ModelState.IsValid)
            {
                u.NationalId = u.NationalId.PersianToEnglish();
                if (!u.NationalId.isNumber())
                {
                    TempData["msg"] = "کد ملی وارد شده باید یک عدد باشد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (await _userManager.Users.AnyAsync(a => a.NationalId.Equals(u.NationalId)))
                {
                    TempData["msg"] = "کد ملی وارد شده در سیستم وجود دارد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                u.Username = u.Username.PersianToEnglish();
                if (!u.Username.isNumber())
                {
                    TempData["msg"] = "شماره تلفن همراه وارد شده باید یک عدد باشد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (await _userManager.Users.AnyAsync(a => a.PhoneNumber.Equals(u.Username)))
                {
                    TempData["msg"] = "شماره تلفن همراه وارد شده در سیستم ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                ApplicationUser user = new ApplicationUser
                {
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    NationalId = u.NationalId,
                    PhoneNumber = u.Username,
                    PhoneNumberConfirmed = true,
                    Gender = u.Gender,
                    Role = u.Role,
                    Status = true,
                    UserName = u.Username
                };

                string[] b = u.BirthString.PersianToEnglish().Split('/');
                user.Birth = new PersianDateTime(Convert.ToInt32(b[0]), Convert.ToInt32(b[1]), Convert.ToInt32(b[2])).ToDateTime();

                var validTypes = new string[] { "image/jpeg", "image/png" };
                if (u.Pic != null)
                {
                    if (validTypes.Contains(u.Pic.ContentType))
                    {
                        if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\user")))
                        {
                            Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\user"));
                        }
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(u.Pic.FileName).ToLower();
                        using (var stream = new FileStream(Path.Combine(_environment.WebRootPath, "img\\user", fileName), FileMode.Create))
                        {
                            await u.Pic.CopyToAsync(stream);
                        }
                        user.Avatar = fileName;
                    }
                    else
                    {
                        TempData["msg"] = $"فرمت فایل های ارسالی مجاز نیست. باید png یا jpg ارسال شود. |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                }

                var create = await _userManager.CreateAsync(user, u.Password);
                if (create.Succeeded)
                {
                    if (u.Role.Equals((byte)ApplicationRoles.Admin))
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    TempData["msg"] = "عملیات موفقیت آمیز بود |success";
                    return RedirectToAction("Users");
                }
                else
                {
                    string error = "";
                    foreach (var item in create.Errors)
                    {
                        if (item.Equals(create.Errors.Last()))
                        {
                            error = error + item.Code + " " + item.Description;
                        }
                        else
                        {
                            error = error + item.Code + " " + item.Description + " | ";
                        }
                    }
                    TempData["msg"] = $"خطا در ذخیره اطلاعات و برقراری ارتباط با پایگاه داده رخ داده است. لطفا مجدد تلاش کنید. جزئیات: {error} |danger";
                }
            }
            else
            {
                TempData["msg"] = "خطای اعتبار سنجی رخ داده است. لطفا فرم را بررسی کنید |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> CreateDriver(CreateDriverVM u)
        {
            if (ModelState.IsValid)
            {
                u.NationalId = u.NationalId.PersianToEnglish();
                if (!u.NationalId.isNumber())
                {
                    TempData["msg"] = "کد ملی وارد شده باید یک عدد باشد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                if (await _userManager.Users.AnyAsync(a => a.NationalId.Equals(u.NationalId)))
                {
                    TempData["msg"] = "کد ملی وارد شده در سیستم وجود دارد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                u.Username = u.Username.PersianToEnglish();
                if (!u.Username.isNumber())
                {
                    TempData["msg"] = "شماره تلفن همراه وارد شده باید یک عدد باشد |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (await _userManager.Users.AnyAsync(a => a.PhoneNumber.Equals(u.Username)))
                {
                    TempData["msg"] = "شماره تلفن همراه وارد شده در سیستم ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                ApplicationUser user = new ApplicationUser
                {
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    NationalId = u.NationalId,
                    PhoneNumber = u.Username,
                    PhoneNumberConfirmed = true,
                    Gender = u.Gender,
                    Role = u.Role,
                    Status = true,
                    UserName = u.Username
                };

                string[] b = u.BirthString.PersianToEnglish().Split('/');
                user.Birth = new PersianDateTime(Convert.ToInt32(b[0]), Convert.ToInt32(b[1]), Convert.ToInt32(b[2])).ToDateTime();

                var validTypes = new string[] { "image/jpeg", "image/png" };
                if (u.Pic != null)
                {
                    if (validTypes.Contains(u.Pic.ContentType))
                    {
                        if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\user")))
                        {
                            Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\user"));
                        }
                        var fileName = Path.GetRandomFileName() + Path.GetExtension(u.Pic.FileName).ToLower();
                        using (var stream = new FileStream(Path.Combine(_environment.WebRootPath, "img\\user", fileName), FileMode.Create))
                        {
                            await u.Pic.CopyToAsync(stream);
                        }
                        user.Avatar = fileName;
                    }
                    else
                    {
                        TempData["msg"] = $"فرمت فایل های ارسالی مجاز نیست. باید png یا jpg ارسال شود. |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                }

                var create = await _userManager.CreateAsync(user);
                if (create.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Driver");

                    TempData["msg"] = "عملیات موفقیت آمیز بود |success";
                    return RedirectToAction("Users");
                }
                else
                {
                    string error = "";
                    foreach (var item in create.Errors)
                    {
                        if (item.Equals(create.Errors.Last()))
                        {
                            error = error + item.Code + " " + item.Description;
                        }
                        else
                        {
                            error = error + item.Code + " " + item.Description + " | ";
                        }
                    }
                    TempData["msg"] = $"خطا در ذخیره اطلاعات و برقراری ارتباط با پایگاه داده رخ داده است. لطفا مجدد تلاش کنید. جزئیات: {error} |danger";
                }
            }
            else
            {
                TempData["msg"] = "خطای اعتبار سنجی رخ داده است. لطفا فرم را بررسی کنید |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Definition
        [HttpGet]
        public async Task<IActionResult> Definition(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _definitionRepo.Definitions().OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        public PartialViewResult CreateDefinition()
        {
            return PartialView("~/Views/Admin/Create/Definition.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateDefinition(Definition d)
        {
            if (ModelState.IsValid)
            {
                _definitionRepo.Create(d);
                try
                {
                    await _definitionRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<PartialViewResult> EditDefinition(int id)
        {
            return PartialView("~/Views/Admin/Edit/Definition.cshtml", await _definitionRepo.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditDefinition(Definition d)
        {
            if (ModelState.IsValid)
            {
                var item = await _definitionRepo.Get(d.Id);
                item.Title = d.Title;
                item.Type = d.Type;
                _definitionRepo.Update(item);
                try
                {
                    await _definitionRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDefinition(int id)
        {
            var item = await _definitionRepo.Get(id);
            if (item == null) return NotFound();

            _definitionRepo.Delete(item);
            try
            {
                await _definitionRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Vehicle
        [HttpGet]
        public async Task<IActionResult> Vehicle(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _vehicleRepo.Vehicles().OrderBy(a => a.Number).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> CreateVehicle()
        {
            ViewData["Definition"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.Type.Equals((int)DefinitionType.Car)).OrderBy(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Create/Vehicle.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle(Vehicle v)
        {
            if (ModelState.IsValid)
            {
                _vehicleRepo.Create(v);
                try
                {
                    await _vehicleRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<PartialViewResult> EditVehicle(string id)
        {
            ViewData["Definition"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.Type.Equals((int)DefinitionType.Car)).OrderBy(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Edit/Vehicle.cshtml", await _vehicleRepo.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditVehicle(Vehicle v)
        {
            if (ModelState.IsValid)
            {
                var item = await _vehicleRepo.Get(v.Id);
                item.Number = v.Number;
                item.Status = v.Status;
                item.Type = v.Type;
                _vehicleRepo.Update(item);
                try
                {
                    await _vehicleRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Calendar
        [HttpGet]
        public async Task<IActionResult> Calendar(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _calendarRepo.Calendars().OrderBy(a => a.StartDate).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateCalendar()
        {
            ViewBag.year = await _configRepo.CurrentYear();
            return PartialView("~/Views/Admin/Create/Calendar.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCalendar(CreateCalendarVM c)
        {
            if (ModelState.IsValid)
            {
                DateTime startDate = new PersianDateTime(c.StartYear, c.StartMonth, c.StartDay, 0, 0, 0).ToDateTime();
                DateTime endDate = new PersianDateTime(c.EndYear, c.EndMonth, c.EndDay, 23, 59, 59).ToDateTime();

                if (startDate >= endDate)
                {
                    TempData["msg"] = "تاریخ شروع وارد شده از تاریخ پایان بزرگ تر است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (await _calendarRepo.Calendars().AnyAsync(a => a.EndDate >= startDate))
                {
                    TempData["msg"] = "این بازه زمانی قبلا ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _calendarRepo.Create(new Calendar
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Title = c.Title,
                    WithholdingTax = c.WithholdingTax,
                    VAT = c.VAT,
                    LoadFactorDeductions = c.LoadFactorDeductions
                });
                try
                {
                    await _calendarRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<PartialViewResult> EditCalendar(string id)
        {
            var item = await _calendarRepo.Get(id);
            var persianStartDate = new PersianDateTime(item.StartDate);
            var persianEndDate = new PersianDateTime(item.EndDate);

            return PartialView("~/Views/Admin/Edit/Calendar.cshtml", new EditCalendarVM
            {
                EndDay = persianEndDate.Day,
                EndMonth = persianEndDate.Month,
                EndYear = persianEndDate.Year,
                Id = item.Id,
                LoadFactorDeductions = item.LoadFactorDeductions,
                StartDay = persianStartDate.Day,
                StartMonth = persianStartDate.Month,
                StartYear = persianStartDate.Year,
                Title = item.Title,
                VAT = item.VAT,
                WithholdingTax = item.WithholdingTax
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditCalendar(EditCalendarVM c)
        {
            if (ModelState.IsValid)
            {
                DateTime startDate = new PersianDateTime(c.StartYear, c.StartMonth, c.StartDay, 0, 0, 0).ToDateTime();
                DateTime endDate = new PersianDateTime(c.EndYear, c.EndMonth, c.EndDay, 23, 59, 59).ToDateTime();

                if (startDate >= endDate)
                {
                    TempData["msg"] = "تاریخ شروع وارد شده از تاریخ پایان بزرگ تر است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (await _calendarRepo.Calendars().AnyAsync(a => a.EndDate >= startDate && !a.Id.Equals(c.Id)))
                {
                    TempData["msg"] = "این بازه زمانی قبلا ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _calendarRepo.Get(c.Id);
                item.StartDate = startDate;
                item.EndDate = endDate;
                item.Title = c.Title;
                item.VAT = c.VAT;
                item.LoadFactorDeductions = c.LoadFactorDeductions;
                item.WithholdingTax = c.WithholdingTax;
                _calendarRepo.Update(item);
                try
                {
                    await _calendarRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCalendar(string id)
        {
            var item = await _calendarRepo.Get(id);
            if (item == null) return NotFound();

            if (item.Costs.Any() || item.CustomerIncomes.Any() || item.LoadFactors.Any() || item.Payments.Any())
            {
                TempData["msg"] = "این تقویم قابل حذف نیست. |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            _calendarRepo.Delete(item);
            try
            {
                await _calendarRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Cost
        [HttpGet]
        public async Task<IActionResult> Cost(int? p)
        {
            ViewData["UserId"] = _userManager.GetUserId(User);
            ViewData["Year"] = await _configRepo.CurrentYear();
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            var pageNumber = p ?? 1;
            var onePageOfData = await _costRepo.Costs().OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cost(Cost c, int day, int month, int year, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                if (pic != null)
                {
                    if (pic.Length <= 1024000)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\cost")))
                            {
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\cost"));
                            }
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\cost", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            c.Picture = fileName;
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 1 مگابایت است |danger";
                    }
                }

                c.Date = new PersianDateTime(year, month, day).ToDateTime();

                _costRepo.Create(c);
                try
                {
                    await _costRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<PartialViewResult> EditCost(int id)
        {
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return PartialView("~/Views/Admin/Edit/Cost.cshtml", await _costRepo.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditCost(Cost c, int day, int month, int year, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                var item = await _costRepo.Get(c.Id);
                item.Description = c.Description;
                item.Amount = c.Amount;
                item.CalendarId = c.CalendarId;

                item.Date = new PersianDateTime(year, month, day).ToDateTime();

                if (pic != null)
                {
                    if (pic.Length <= 1024000)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\cost")))
                            {
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\cost"));
                            }
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\cost", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            if (!string.IsNullOrEmpty(item.Picture))
                            {
                                try
                                {
                                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\cost", item.Picture));
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }

                            item.Picture = fileName;
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 1 مگابایت است |danger";
                    }
                }

                _costRepo.Update(item);
                try
                {
                    await _costRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCost(int id)
        {
            var item = await _costRepo.Get(id);
            if (!string.IsNullOrEmpty(item.Picture))
            {
                try
                {
                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\cost", item.Picture));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            _costRepo.Delete(item);
            try
            {
                await _costRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Payment
        [HttpGet]
        public async Task<IActionResult> Payment(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _paymentRepo.Payments().OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> CreatePayment(string userId)
        {
            ViewData["AdminId"] = _userManager.GetUserId(User);
            ViewData["UserInfo"] = await _userManager.FindByIdAsync(userId);
            ViewData["Year"] = await _configRepo.CurrentYear();
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return PartialView("~/Views/Admin/Create/Payment.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(Payment p, int day, int month, int year, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                if (pic != null)
                {
                    if (pic.Length <= 1024000)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\payment")))
                            {
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\payment"));
                            }
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\payment", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            p.Picture = fileName;
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 1 مگابایت است |danger";
                    }
                }

                p.Date = new PersianDateTime(year, month, day).ToDateTime();

                _paymentRepo.Create(p);
                try
                {
                    await _paymentRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<PartialViewResult> EditPayment(int id)
        {
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return PartialView("~/Views/Admin/Edit/Payment.cshtml", await _paymentRepo.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditPayment(Payment p, int day, int month, int year, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                var item = await _paymentRepo.Get(p.Id);
                item.AdminId = _userManager.GetUserId(User);
                item.Amount = p.Amount;
                item.Type = p.Type;
                item.CalendarId = p.CalendarId;

                item.Date = new PersianDateTime(year, month, day).ToDateTime();

                if (pic != null)
                {
                    if (pic.Length <= 1024000)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\payment")))
                            {
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\payment"));
                            }
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\payment", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            if (!string.IsNullOrEmpty(item.Picture))
                            {
                                try
                                {
                                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\payment", item.Picture));
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }

                            item.Picture = fileName;
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 1 مگابایت است |danger";
                    }
                }

                _paymentRepo.Update(item);
                try
                {
                    await _paymentRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var item = await _paymentRepo.Get(id);
            if (!string.IsNullOrEmpty(item.Picture))
            {
                try
                {
                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\payment", item.Picture));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            _paymentRepo.Delete(item);
            try
            {
                await _paymentRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Customer
        [HttpGet]
        public async Task<IActionResult> Customer()
        {
            return View(await _customerRepo.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> Customer(Customer c)
        {
            if (ModelState.IsValid)
            {
                _customerRepo.Create(c);
                try
                {
                    await _customerRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<PartialViewResult> EditCustomer(int id)
        {
            return PartialView("~/Views/Admin/Edit/Customer.cshtml", await _customerRepo.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(Customer c)
        {
            if (ModelState.IsValid)
            {
                var item = await _customerRepo.Get(c.Id);
                item.Name = c.Name;
                item.Status = c.Status;

                _customerRepo.Update(item);
                try
                {
                    await _customerRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<IActionResult> CustomerIncome(int id, int? p)
        {
            var customer = await _customerRepo.Get(id);
            if (customer == null)
            {
                return BadRequest();
            }
            ViewData["CustomerInfo"] = customer;
            ViewData["Year"] = await _configRepo.CurrentYear();
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            var pageNumber = p ?? 1;
            var onePageOfData = await _customerRepo.CustomerIncomes().Where(a => a.CustomerId.Equals(id)).OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerIncome([Bind("Amount,Description,CustomerId")] CustomerIncome c, int day, int month, int year, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                if (pic != null)
                {
                    if (pic.Length <= 1024000)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\income")))
                            {
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\income"));
                            }
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\income", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            c.Picture = fileName;
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 1 مگابایت است |danger";
                    }
                }

                c.AdminId = _userManager.GetUserId(User);
                c.Date = new PersianDateTime(year, month, day).ToDateTime();

                _customerRepo.Create(c);
                try
                {
                    await _customerRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<PartialViewResult> EditCustomerIncome(int id)
        {
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return PartialView("~/Views/Admin/Edit/CustomerIncome.cshtml", await _customerRepo.GetIncome(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomerIncome(CustomerIncome p, int day, int month, int year, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                var item = await _customerRepo.GetIncome(p.Id);
                item.AdminId = _userManager.GetUserId(User);
                item.Amount = p.Amount;

                item.Date = new PersianDateTime(year, month, day).ToDateTime();

                if (pic != null)
                {
                    if (pic.Length <= 1024000)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\income")))
                            {
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\income"));
                            }
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\income", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            if (!string.IsNullOrEmpty(item.Picture))
                            {
                                try
                                {
                                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\income", item.Picture));
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }

                            item.Picture = fileName;
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                        }
                    }
                    else
                    {
                        TempData["msg"] = "حجم تصویر بیشتر از 1 مگابایت است |danger";
                    }
                }

                _customerRepo.Update(item);
                try
                {
                    await _customerRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomerIncome(int id)
        {
            var item = await _customerRepo.GetIncome(id);
            if (!string.IsNullOrEmpty(item.Picture))
            {
                try
                {
                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\income", item.Picture));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            _customerRepo.Delete(item);
            try
            {
                await _customerRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Contract
        [HttpGet]
        public async Task<IActionResult> Contract(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _contractRepo.Contracts().Where(a => string.IsNullOrEmpty(a.ParentContractId)).OrderByDescending(a => a.StartDate).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateContract()
        {
            ViewData["Year"] = await _configRepo.CurrentYear();
            ViewData["Customers"] = await _customerRepo.GetAll();
            ViewData["Contracts"] = await _contractRepo.Contracts().Where(a => string.IsNullOrEmpty(a.ParentContractId)).OrderByDescending(a => a.StartDate).ToListAsync();
            return PartialView("~/Views/Admin/Create/Contract.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateContract(CreateContractVM c)
        {
            if (ModelState.IsValid)
            {
                DateTime startDate = new PersianDateTime(c.StartYear, c.StartMonth, c.StartDay, 0, 0, 0).ToDateTime();
                DateTime endDate = new PersianDateTime(c.EndYear, c.EndMonth, c.EndDay, 23, 59, 59).ToDateTime();

                if (startDate >= endDate)
                {
                    TempData["msg"] = "تاریخ شروع وارد شده از تاریخ پایان بزرگ تر است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var contract = new Contract
                {
                    CustomerId = c.CustomerId,
                    EndDate = endDate,
                    StartDate = startDate,
                    Number = c.Number,
                    Subject = c.Subject
                };

                if (c.ParentContractId != "none")
                    contract.ParentContractId = c.ParentContractId;

                _contractRepo.Create(contract);

                try
                {
                    await _contractRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion
    }
}
