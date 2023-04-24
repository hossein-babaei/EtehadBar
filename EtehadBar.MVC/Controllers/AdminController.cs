using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.MVC.Filters;
using Helpers;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace EtehadBar.MVC.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(ActionLogFilter))]
    public class AdminController : Controller
    {
        private readonly IAccountBookRepository _accountBookRepository;
        private readonly IAdminThemeRepository _adminThemeRepo;
        private readonly ICalendarRepository _calendarRepo;
        private readonly IConfigRepository _configRepo;
        private readonly IContractRepository _contractRepo;
        private readonly ICostRepository _costRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IDefinitionRepository _definitionRepo;
        private readonly ILoadFactorRepository _loadFactorRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IShippingFeeRepository _shippingFeeRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShippingFeeLoadTypeRepository _shippingFeeLoadTypeRepo;
        private readonly ILoadRoutesRepository _loadRouteRepo;
        private readonly IDriverRepository _driverRepository;
        private readonly IMehrcomParsCategoryRepository _mehrcomParsCategoryRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IFreeLoadFactorRepository _freeLoadFactorRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IBankAccountBookRepository _bankAccountBookRepository;
        private readonly IAdminDashboardRepository _adminDashboardRepository;
        private readonly ITurnoverRepository _turnoverRepository;

        public AdminController(
            IAccountBookRepository accountBookRepository,
            IAdminThemeRepository adminThemeRepository,
            ICalendarRepository calendarRepository,
            IConfigRepository configRepository,
            IContractRepository contractRepository,
            ICostRepository costRepository,
            ICustomerRepository customerRepository,
            IDefinitionRepository definitionRepository,
            ILoadFactorRepository loadFactorRepository,
            IPaymentRepository paymentRepository,
            IShippingFeeRepository shippingFeeRepository,
            IVehicleRepository vehicleRepository,
            IWebHostEnvironment environment,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IShippingFeeLoadTypeRepository shippingFeeLoadTypeRepo,
            ILoadRoutesRepository loadRouteRepo,
            IDriverRepository driverRepository,
            IMehrcomParsCategoryRepository mehrcomParsCategoryRepository,
            SignInManager<ApplicationUser> signInManager,
            IFreeLoadFactorRepository freeLoadFactorRepository,
            IBankAccountRepository bankAccountRepository,
            IBankAccountBookRepository bankAccountBookRepository,
            IAdminDashboardRepository adminDashboardRepository,
            ITurnoverRepository turnoverRepository)
        {
            _accountBookRepository = accountBookRepository;
            _adminThemeRepo = adminThemeRepository;
            _calendarRepo = calendarRepository;
            _configRepo = configRepository;
            _contractRepo = contractRepository;
            _costRepo = costRepository;
            _customerRepo = customerRepository;
            _definitionRepo = definitionRepository;
            _loadFactorRepo = loadFactorRepository;
            _paymentRepo = paymentRepository;
            _shippingFeeRepo = shippingFeeRepository;
            _vehicleRepo = vehicleRepository;
            _environment = environment;
            _roleManager = roleManager;
            _userManager = userManager;
            _shippingFeeLoadTypeRepo = shippingFeeLoadTypeRepo;
            _loadRouteRepo = loadRouteRepo;
            _driverRepository = driverRepository;
            _mehrcomParsCategoryRepository = mehrcomParsCategoryRepository;
            _signInManager = signInManager;
            _freeLoadFactorRepository = freeLoadFactorRepository;
            _bankAccountRepository = bankAccountRepository;
            _bankAccountBookRepository = bankAccountBookRepository;
            _adminDashboardRepository = adminDashboardRepository;
            _turnoverRepository = turnoverRepository;
        }

        private long CalcNextSequenceForLoadFactor(long sequence)
        {
            double x = sequence / 5;
            sequence = Convert.ToInt64((Math.Floor(x) + 1) * 5);
            return sequence;
        }

        public async Task<IActionResult> Index(int? dayLimit)
        {
            ViewData["DayLimit"] = dayLimit;

            if (User.IsInRole("Admin"))
                return View("AdminDashboard", await _adminDashboardRepository.GetAdminData(dayLimit));
            else if (User.IsInRole("User") || User.IsInRole("Milad"))
                return View("UserDashboard", await _adminDashboardRepository.GetUserData(dayLimit));
            else
                return View("RegisterUserDashboard", await _adminDashboardRepository.GetRegisterUserData(_userManager.GetUserId(User), dayLimit));
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
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> Config()
        {
            return View(await _configRepo.First());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
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
                data = data.Where(a => (a.Firstname + " " + a.Lastname).Contains(name) || a.Firstname.Contains(name) || a.Lastname.Contains(name));

            var pageNumber = p ?? 1;
            var onePageOfData = await data.OrderByDescending(a => a.RegisterDate).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            ViewBag.name = name;
            return PartialView("_UserList");
        }

        [HttpGet]
        public async Task<IActionResult> GetUserListExceptDriversPartial(int? p, string name)
        {
            var data = _userManager.Users.Where(a => a.Status);

            if (!string.IsNullOrWhiteSpace(name))
                data = data.Where(a => (a.Firstname + " " + a.Lastname).Contains(name) || a.Firstname.Contains(name) || a.Lastname.Contains(name));

            var pageNumber = p ?? 1;
            var onePageOfData = await data.OrderByDescending(a => a.RegisterDate).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            ViewBag.name = name;
            return PartialView("_UserListExceptDriver");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
                AccountBankName = user.AccountBankName,
                BankAccountNumber = user.BankAccountNumber,
                Tel = user.Tel
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
                user.AccountBankName = u.AccountBankName;
                user.BankAccountNumber = u.BankAccountNumber;

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
        [Authorize(Roles = "Admin")]
        public PartialViewResult GetUserCreateForm(ApplicationRoleType type)
        {
            ViewBag.type = type;
            return PartialView("~/Views/Admin/Create/User.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

                ApplicationUser user = new()
                {
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    NationalId = u.NationalId,
                    PhoneNumber = u.Username,
                    PhoneNumberConfirmed = true,
                    Gender = u.Gender,
                    Role = u.Role,
                    Status = true,
                    UserName = u.Username,
                    BankAccountNumber = u.BankAccountNumber,
                    AccountBankName = u.AccountBankName
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
                    if (u.Role == ApplicationRoleType.Admin)
                        await _userManager.AddToRoleAsync(user, "Admin");
                    else if (u.Role == ApplicationRoleType.RegisterUser)
                        await _userManager.AddToRoleAsync(user, "RegisterUser");
                    else
                        await _userManager.AddToRoleAsync(user, "User");

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

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM m)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                TempData["msg"] = "کاربر پیدا نشد |danger";

            m.OldPassword = m.OldPassword.PersianToEnglish();
            m.NewPassword = m.NewPassword.PersianToEnglish();
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, m.OldPassword, m.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                string msg = "";
                foreach (var error in changePasswordResult.Errors)
                    msg += $"{error.Description} - ";

                TempData["msg"] = $"{msg} |danger";
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["msg"] = "عملیات موفقیت آمیز بود |success";

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, "1234@User");

                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Definition
        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> Definition(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _definitionRepo.Definitions().OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public PartialViewResult CreateDefinition()
        {
            return PartialView("~/Views/Admin/Create/Definition.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
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
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<PartialViewResult> EditDefinition(int id)
        {
            return PartialView("~/Views/Admin/Edit/Definition.cshtml", await _definitionRepo.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> EditDefinition(Definition d)
        {
            if (ModelState.IsValid)
            {
                var item = await _definitionRepo.Get(d.Id);
                item.Title = d.Title;
                item.DefinitionType = d.DefinitionType;
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
        [Authorize(Roles = "Admin, User, Milad")]
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
        public async Task<IActionResult> GetVehicleListPartial(int? p, string rightNumber)
        {
            var data = _vehicleRepo.Vehicles();

            if (!string.IsNullOrWhiteSpace(rightNumber))
            {
                data = data.Where(a => a.RightNumber.Contains(rightNumber));
            }

            var pageNumber = p ?? 1;
            var onePageOfData = await data.OrderBy(a => a.LeftNumber).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            ViewBag.rightNumber = rightNumber;
            return PartialView("_VehicleList");
        }

        [HttpGet]
        public async Task<IActionResult> Vehicle(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _vehicleRepo.Vehicles().OrderBy(a => a.LeftNumber).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchVehicle(int? p, string param)
        {
            var pageNum = p ?? 1;
            var onePageOfData = await _vehicleRepo.Vehicles().Where(a => a.VehicleOwnerFullname.Contains(param) || a.RightNumber.Contains(param)).OrderByDescending(a => a.Id).ToPagedListAsync(pageNum, 15);
            ViewBag.data = onePageOfData;
            ViewBag.param = param;

            return PartialView("_Vehicle");
        }

        [HttpGet]
        public async Task<PartialViewResult> CreateVehicle()
        {
            ViewData["Definition"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType.Equals(DefinitionType.Car)).OrderBy(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Create/Vehicle.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle(Vehicle v)
        {
            if (ModelState.IsValid)
            {
                if (await _vehicleRepo.Vehicles().AnyAsync(a => a.IranStateNumber.Equals(v.IranStateNumber) && a.RightNumber.Equals(v.RightNumber) && a.NumberWord.Equals(v.NumberWord) && a.LeftNumber.Equals(v.LeftNumber)))
                {
                    TempData["msg"] = "شماره خودرو وارد شده قبلا ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

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
        public async Task<PartialViewResult> EditVehicle(int id)
        {
            ViewData["Definition"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType.Equals(DefinitionType.Car)).OrderBy(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Edit/Vehicle.cshtml", await _vehicleRepo.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditVehicle(Vehicle v)
        {
            if (ModelState.IsValid)
            {
                if (await _vehicleRepo.Vehicles().AnyAsync(a => !a.Id.Equals(v.Id) && a.IranStateNumber.Equals(v.IranStateNumber) && a.RightNumber.Equals(v.RightNumber) && a.NumberWord.Equals(v.NumberWord) && a.LeftNumber.Equals(v.LeftNumber)))
                {
                    TempData["msg"] = "شماره خودرو وارد شده قبلا ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _vehicleRepo.Get(v.Id);
                item.IranStateNumber = v.IranStateNumber;
                item.LeftNumber = v.LeftNumber;
                item.NumberWord = v.NumberWord;
                item.RightNumber = v.RightNumber;
                item.Status = v.Status;
                item.Type = v.Type;
                item.AccountBankName = v.AccountBankName;
                item.BankAccountNumber = v.BankAccountNumber;
                item.VehicleOwnerFullname = v.VehicleOwnerFullname;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Calendar(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _calendarRepo.Calendars().OrderBy(a => a.StartDate).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCalendar()
        {
            ViewBag.year = await _configRepo.CurrentYear();
            return PartialView("~/Views/Admin/Create/Calendar.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

                _calendarRepo.Create(new Domain.Models.Calendar
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Title = c.Title,
                    CreatorId = _userManager.GetUserId(User)
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
        [Authorize(Roles = "Admin")]
        public async Task<PartialViewResult> EditCalendar(int id)
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
                StartDay = persianStartDate.Day,
                StartMonth = persianStartDate.Month,
                StartYear = persianStartDate.Year,
                Title = item.Title
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
                item.EditorId = _userManager.GetUserId(User);
                item.EditDate = DateTime.Now;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCalendar(int id)
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

        [HttpPost]
        public async Task<IActionResult> GetCalendarsJson()
        {
            return Json(await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).Select(a => new { a.Id, a.Title }).ToListAsync());
        }
        #endregion

        #region Cost
        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> Cost(int? p)
        {
            ViewData["UserId"] = _userManager.GetUserId(User);
            ViewData["Year"] = await _configRepo.CurrentYear();
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();

            var query = _costRepo.Costs();
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.UserId.Equals(_userManager.GetUserId(User)));

            var pageNumber = p ?? 1;
            var onePageOfData = await query.OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> Cost(Cost c, int day, int month, int year, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                if (pic != null)
                {
                    if (pic.Length <= 10240000)
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
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<PartialViewResult> EditCost(int id)
        {
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return PartialView("~/Views/Admin/Edit/Cost.cshtml", await _costRepo.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
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
                    if (pic.Length <= 10240000)
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
        [Authorize(Roles = "Admin, User, Milad")]
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
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> Payment(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _paymentRepo.Payments().OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<PartialViewResult> CreatePayment(string itemId, string type)
        {
            ViewData["AdminId"] = _userManager.GetUserId(User);
            ViewData["Year"] = await _configRepo.CurrentYear();
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            if (type == "vehicle")
                ViewData["VehicleInfo"] = await _vehicleRepo.Get(Convert.ToInt64(itemId));
            else
                ViewData["UserInfo"] = await _userManager.Users.FirstOrDefaultAsync(a => a.Id == itemId);

            ViewData["Type"] = type;
            return PartialView("~/Views/Admin/Create/Payment.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
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
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<PartialViewResult> EditPayment(int id)
        {
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return PartialView("~/Views/Admin/Edit/Payment.cshtml", await _paymentRepo.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> EditPayment(Payment p, int day, int month, int year, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                var item = await _paymentRepo.Get(p.Id);
                item.AdminId = _userManager.GetUserId(User);
                item.Amount = p.Amount;
                item.PaymentType = p.PaymentType;
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
        [Authorize(Roles = "Admin, User, Milad")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Customer()
        {
            return View(await _customerRepo.GetAll());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<PartialViewResult> EditCustomer(int id)
        {
            return PartialView("~/Views/Admin/Edit/Customer.cshtml", await _customerRepo.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCustomer(Customer c)
        {
            if (ModelState.IsValid)
            {
                var item = await _customerRepo.Get(c.Id);
                item.Name = c.Name;
                item.Status = c.Status;
                item.HasAddonTonnage = c.HasAddonTonnage;
                item.HasLoadType = c.HasLoadType;

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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<PartialViewResult> EditCustomerIncome(int id)
        {
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return PartialView("~/Views/Admin/Edit/CustomerIncome.cshtml", await _customerRepo.GetIncome(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> Contract(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _contractRepo.Contracts().Where(a => !a.ParentContractId.HasValue).OrderByDescending(a => a.StartDate).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateContract()
        {
            ViewData["Year"] = await _configRepo.CurrentYear();
            var customers = await _customerRepo.GetAll();
            if (!customers.Any())
                return NotFound("ابتدا مشتری ثبت کنید");

            ViewData["Customers"] = customers;
            ViewData["Contracts"] = await _contractRepo.Contracts().Where(a => !a.ParentContractId.HasValue && a.CustomerId.Equals(customers.First().Id)).OrderByDescending(a => a.StartDate).ToListAsync();
            return PartialView("~/Views/Admin/Create/Contract.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
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

                if (c.ParentContractId.HasValue && c.ParentContractId.Value != 0)
                {
                    contract.ParentContractId = c.ParentContractId;

                    var parentContract = await _contractRepo.Get(c.ParentContractId.Value);
                    if (parentContract.EndDate < endDate)
                    {
                        TempData["msg"] = "تاریخ پایان الحاقیه از تاریخ پایان قرارداد اصلی بزرگ تر است. |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                }

                _contractRepo.Create(contract);

                try
                {
                    await _contractRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";

                    //if (!c.ParentContractId.HasValue || (c.ParentContractId.HasValue && c.ParentContractId.Value == 0))
                    //    return RedirectToAction("ShippingFee", new { contractId = contract.RowId });
                    //else
                    //{
                    //    string parentRowId = await _contractRepo.Contracts().Where(a => a.Id.Equals(contract.ParentContractId.Value)).Select(a => a.RowId).FirstAsync();
                    //    return RedirectToAction("ShippingFee", new { contractId = parentRowId });
                    //}

                    return RedirectToAction("ShippingFee", new { contractId = contract.RowId });
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
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditContract(int id)
        {
            var item = await _contractRepo.Get(id);
            var persianStartDate = new PersianDateTime(item.StartDate);
            var persianEndDate = new PersianDateTime(item.EndDate);

            ViewData["Year"] = await _configRepo.CurrentYear();

            return PartialView("~/Views/Admin/Edit/Contract.cshtml", new EditContractVM
            {
                EndDay = persianEndDate.Day,
                EndMonth = persianEndDate.Month,
                EndYear = persianEndDate.Year,
                Id = item.Id,
                StartDay = persianStartDate.Day,
                StartMonth = persianStartDate.Month,
                StartYear = persianStartDate.Year,
                Number = item.Number,
                Subject = item.Subject
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditContract(EditContractVM c)
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

                var contract = await _contractRepo.Get(c.Id);

                if (contract.ParentContractId.HasValue)
                {
                    var parentContract = await _contractRepo.Get(contract.ParentContractId.Value);
                    if (parentContract.EndDate < endDate)
                    {
                        TempData["msg"] = "تاریخ پایان الحاقیه از تاریخ پایان قرارداد اصلی بزرگ تر است. |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                }

                contract.EndDate = endDate;
                contract.StartDate = startDate;
                contract.Number = c.Number;
                contract.Subject = c.Subject;

                _contractRepo.Update(contract);

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

        [HttpPost]
        public async Task<JsonResult> GetContractAddonsJson(int customerId)
        {
            return Json(await _contractRepo.Contracts().AsNoTracking().Where(a => !a.ParentContractId.HasValue && a.CustomerId.Equals(customerId)).OrderByDescending(a => a.StartDate).Select(a => new
            {
                a.Number,
                a.Subject,
                a.Id
            }).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var item = await _contractRepo.Get(id);

            if (item == null) return NotFound();

            if (item.LoadFactors.Any())
            {
                TempData["msg"] = $"برای این قرارداد {item.LoadFactors.Count} بارنامه وجود دارد و قابل حذف نیست. |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            _contractRepo.Delete(item);
            try
            {
                await _contractRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region ShippingFee
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShippingFee(string contractId)
        {
            if (string.IsNullOrWhiteSpace(contractId)) return BadRequest();

            var contract = await _contractRepo.Get(contractId);
            if (contract == null) return NotFound();

            ViewData["Contract"] = contract;
            return View(await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(contract.Id)).OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShippingFeePartial(string contractId)
        {
            var contract = await _contractRepo.Get(contractId);
            if (contract == null) return NotFound();

            ViewData["Contract"] = contract;
            return PartialView("_ShippingFee", await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(contract.Id)).OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateShippingFee(string contractId)
        {
            ViewData["Contract"] = await _contractRepo.Get(contractId);
            List<DefinitionType> types = new()
            {
                DefinitionType.Car
            };
            ViewData["Data"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => types.Contains(a.DefinitionType)).ToListAsync();
            ViewData["LoadRoutes"] = await _loadRouteRepo.LoadRoutes().AsNoTracking().ToListAsync();
            ViewData["LoadTypes"] = await _shippingFeeLoadTypeRepo.ShippingFeeLoadTypes().AsNoTracking().ToListAsync();

            return PartialView("~/Views/Admin/Create/ShippingFee.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateShippingFee(ShippingFee s)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                if (s.DestinationId == s.OriginId)
                    return Json(new { msg = "مبدا و مقصد نمی تواند یکی باشد.", status });

                if (await _shippingFeeRepo.ShippingFees().AsNoTracking()
                    .AnyAsync(a => a.ShippingFeeType == s.ShippingFeeType && a.ShippingFeeLoadTypeId.Equals(s.ShippingFeeLoadTypeId) && a.Vehicle.Equals(s.Vehicle) && a.OriginId.Equals(s.OriginId) && a.DestinationId.Equals(s.DestinationId) && a.DriverPrice.Equals(s.DriverPrice) && a.Title.Equals(s.Title)))
                    return Json(new { msg = "نرخ حمل و نقل ثبت شده تکراری است.", status });

                if (s.ShippingFeeType == ShippingFeeType.Custom)
                {
                    s.Price = 0;
                    s.DriverPrice = 0;
                    s.TonnagePrice = null;
                    s.DriverTonnagePrice = null;
                }

                s.CreatorId = _userManager.GetUserId(User);
                _shippingFeeRepo.Create(s);

                try
                {
                    await _shippingFeeRepo.Save();
                    msg = "عملیات موفقیت آمیز بود.";
                    status = "success";
                }
                catch (Exception e)
                {
                    msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
                }
            }
            else
            {
                msg = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید.";
            }
            return Json(new { msg, status });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditShippingFee(int id)
        {
            List<DefinitionType> types = new()
            {
                DefinitionType.Car
            };
            ViewData["Data"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => types.Contains(a.DefinitionType)).ToListAsync();
            ViewData["LoadRoutes"] = await _loadRouteRepo.LoadRoutes().AsNoTracking().ToListAsync();
            ViewData["LoadTypes"] = await _shippingFeeLoadTypeRepo.ShippingFeeLoadTypes().AsNoTracking().ToListAsync();

            return PartialView("~/Views/Admin/Edit/ShippingFee.cshtml", await _shippingFeeRepo.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditShippingFee(ShippingFee s)
        {
            if (ModelState.IsValid)
            {
                if (s.DestinationId == s.OriginId)
                {
                    TempData["msg"] = "مبدا و مقصد نمی تواند یکی باشد. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (await _shippingFeeRepo.ShippingFees().AsNoTracking()
                    .AnyAsync(a => !a.Id.Equals(s.Id) && a.ShippingFeeLoadTypeId.Equals(s.ShippingFeeLoadTypeId) && a.ShippingFeeType == s.ShippingFeeType && a.Vehicle.Equals(s.Vehicle) && a.OriginId.Equals(s.OriginId) && a.DestinationId.Equals(s.DestinationId) && a.DriverPrice.Equals(s.DriverPrice) && a.Title.Equals(s.Title)))
                {
                    TempData["msg"] = "نرخ حمل و نقل ثبت شده تکراری است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (s.ShippingFeeType == ShippingFeeType.Custom)
                {
                    s.Price = 0;
                    s.DriverPrice = 0;
                    s.TonnagePrice = null;
                    s.DriverTonnagePrice = null;
                }

                var item = await _shippingFeeRepo.Get(s.Id);

                item.DestinationId = s.DestinationId;
                item.OriginId = s.OriginId;
                item.DriverPrice = s.DriverPrice;
                item.Price = s.Price;
                item.Vehicle = s.Vehicle;
                item.TonnagePrice = s.TonnagePrice;
                item.DriverTonnagePrice = s.DriverTonnagePrice;
                item.ShippingFeeLoadTypeId = s.ShippingFeeLoadTypeId;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDate = DateTime.Now;
                item.Title = s.Title;

                _shippingFeeRepo.Update(item);

                try
                {
                    await _shippingFeeRepo.Save();

                    var latestContractAddon = await _contractRepo.Contracts().AsNoTracking().Where(a => a.ParentContractId.Equals(item.ContractId)).OrderByDescending(a => a.StartDate).FirstOrDefaultAsync();
                    if (latestContractAddon == null)
                        latestContractAddon = await _contractRepo.Get(item.ContractId);

                    var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.ContractId.Equals(item.ContractId) && a.ShippingFeeId.Equals(item.Id) && a.Date >= latestContractAddon.StartDate && !a.IsDriverFeeEditedByAdmin).ToListAsync();
                    if (loadFactors.Any())
                    {
                        foreach (var factor in loadFactors)
                        {
                            factor.OriginId = item.OriginId;
                            factor.DestinationId = item.DestinationId;
                            factor.DriverFee = item.DriverPrice;
                            factor.Amount = item.Price;
                            factor.TonnagePrice = item.TonnagePrice;
                            factor.DriverTonnagePrice = item.DriverTonnagePrice;
                        }
                        _loadFactorRepo.UpdateRange(loadFactors);
                        await _loadFactorRepo.Save();
                    }

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShippingFee(int id)
        {
            var item = await _shippingFeeRepo.Get(id);

            if (item == null) return NotFound();

            _shippingFeeRepo.Delete(item);
            try
            {
                await _shippingFeeRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateShippingFreeFeeTypeFromNormal(long id)
        {
            string msg;
            string status = "danger";

            var item = await _shippingFeeRepo.Get(id);

            if (await _shippingFeeRepo.ShippingFees().AnyAsync(a => a.ContractId.Equals(item.ContractId)
            && a.DestinationId.Equals(item.DestinationId) && a.OriginId.Equals(item.OriginId) && a.ShippingFeeLoadTypeId.Equals(item.ShippingFeeLoadTypeId)
            && a.Vehicle.Equals(item.Vehicle) && a.ShippingFeeType == ShippingFeeType.Custom))
            {
                msg = "نرخ آزاد برای این مورد وجود دارد.";
                return Json(new { msg, status });
            }

            _shippingFeeRepo.Create(new ShippingFee
            {
                ContractId = item.ContractId,
                DestinationId = item.DestinationId,
                OriginId = item.OriginId,
                ShippingFeeType = ShippingFeeType.Custom,
                ShippingFeeLoadTypeId = item.ShippingFeeLoadTypeId,
                DriverPrice = 0,
                Price = 0,
                TonnagePrice = null,
                DriverTonnagePrice = null,
                Vehicle = item.Vehicle,
                CreateDate = DateTime.Now,
                Title = item.Title,
                CreatorId = _userManager.GetUserId(User)
            });

            try
            {
                await _shippingFeeRepo.Save();
                msg = "عملیات موفقیت آمیز بود.";
                status = "success";
            }
            catch (Exception e)
            {
                msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
            }
            return Json(new { msg, status });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeShippingFee(int contractId,
            string amountDate, double amount, string type,
            string driverAmountDate, double driverAmount, string driverType,
            string tonnageAmountDate, double tonnageAmount, string tonnageType,
            string tonnageDriverAmountDate, double tonnageDriverAmount, string driverTonnageType)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                if (amount == 0 && driverAmount == 0 && tonnageAmount == 0 && tonnageDriverAmount == 0)
                    return Json(new { msg = "میزان تغییر باید بزرگتر یا کوچکتر از صفر باشد.", status });

                if (type.Equals("percent") && (amount > 100 || amount < -100))
                    return Json(new { msg = "درصد تغییر نرخ مناسب نیست.", status });

                if (driverType.Equals("percent") && (driverAmount > 100 || driverAmount < -100))
                    return Json(new { msg = "درصد تغییر نرخ راننده مناسب نیست.", status });

                if (tonnageType.Equals("percent") && (tonnageAmount > 100 || tonnageAmount < -100))
                    return Json(new { msg = "درصد تغییر نرخ تناژ مناسب نیست.", status });

                if (driverTonnageType.Equals("percent") && (tonnageDriverAmount > 100 || tonnageDriverAmount < -100))
                    return Json(new { msg = "درصد تغییر نرخ تناژ راننده مناسب نیست.", status });

                var amountDateArray = amountDate.PersianToEnglish().Split('/');
                var amountDatetime = new PersianDateTime(Convert.ToInt32(amountDateArray[0]), Convert.ToInt32(amountDateArray[1]), Convert.ToInt32(amountDateArray[2])).ToDateTime();

                var driverAmountDateArray = driverAmountDate.PersianToEnglish().Split('/');
                var driverAmountDatetime = new PersianDateTime(Convert.ToInt32(driverAmountDateArray[0]), Convert.ToInt32(driverAmountDateArray[1]), Convert.ToInt32(driverAmountDateArray[2])).ToDateTime();

                var tonnageAmountDateArray = tonnageAmountDate.PersianToEnglish().Split('/');
                var tonnageAmountDatetime = new PersianDateTime(Convert.ToInt32(tonnageAmountDateArray[0]), Convert.ToInt32(tonnageAmountDateArray[1]), Convert.ToInt32(tonnageAmountDateArray[2])).ToDateTime();

                var tonnageDriverAmountDateArray = tonnageDriverAmountDate.PersianToEnglish().Split('/');
                var tonnageDriverAmountDatetime = new PersianDateTime(Convert.ToInt32(tonnageDriverAmountDateArray[0]), Convert.ToInt32(tonnageDriverAmountDateArray[1]), Convert.ToInt32(tonnageDriverAmountDateArray[2])).ToDateTime();

                var latestContractAddon = await _contractRepo.Contracts().AsNoTracking().Where(a => a.ParentContractId.Equals(contractId)).OrderByDescending(a => a.StartDate).FirstOrDefaultAsync();
                if (latestContractAddon == null)
                    latestContractAddon = await _contractRepo.Get(contractId);

                var feeList = await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(contractId)).ToListAsync();
                var loadFactors = await _shippingFeeRepo.GetLoadFactorsByContractId(contractId, latestContractAddon.StartDate);

                foreach (var fee in feeList)
                {
                    if (amount != 0)
                    {
                        if (type.Equals("percent"))
                        {
                            var a = fee.Price * amount / 100;
                            fee.Price += a;
                        }
                        else fee.Price += amount;
                    }

                    if (driverAmount != 0)
                    {
                        if (driverType.Equals("percent"))
                        {
                            var a = fee.DriverPrice * driverAmount / 100;
                            fee.DriverPrice += a;
                        }
                        else fee.DriverPrice += driverAmount;
                    }

                    if (fee.TonnagePrice.HasValue && tonnageAmount != 0)
                    {
                        if (tonnageType.Equals("percent"))
                        {
                            var a = fee.TonnagePrice.Value * tonnageAmount / 100;
                            fee.TonnagePrice = fee.TonnagePrice.Value + a;
                        }
                        else fee.TonnagePrice = fee.TonnagePrice.Value + tonnageAmount;
                    }

                    if (fee.DriverTonnagePrice.HasValue && tonnageDriverAmount != 0)
                    {
                        if (driverTonnageType.Equals("percent"))
                        {
                            var a = fee.DriverTonnagePrice.Value * tonnageDriverAmount / 100;
                            fee.DriverTonnagePrice = fee.DriverTonnagePrice.Value + a;
                        }
                        else fee.DriverTonnagePrice = fee.DriverTonnagePrice.Value + tonnageDriverAmount;
                    }

                    var thisLoadFactor = loadFactors.Where(a => a.ShippingFeeId.Equals(fee.Id) && !a.IsDriverFeeEditedByAdmin).ToList();
                    if (thisLoadFactor.Any())
                    {
                        foreach (var loadFactor in thisLoadFactor)
                        {
                            if (loadFactor.Date >= amountDatetime)
                                loadFactor.Amount = fee.Price;

                            if (loadFactor.Date >= driverAmountDatetime)
                                loadFactor.DriverFee = fee.DriverPrice;

                            if (loadFactor.Date >= tonnageAmountDatetime && loadFactor.TonnagePrice.HasValue)
                                loadFactor.TonnagePrice = fee.TonnagePrice;

                            if (loadFactor.Date >= tonnageDriverAmountDatetime && loadFactor.DriverTonnagePrice.HasValue)
                                loadFactor.DriverTonnagePrice = fee.DriverTonnagePrice;
                        }
                        _shippingFeeRepo.UpdateLoadFactors(thisLoadFactor);
                    }
                }

                _shippingFeeRepo.UpdateRange(feeList);

                try
                {
                    await _shippingFeeRepo.Save();
                    msg = "عملیات موفقیت آمیز بود.";
                    status = "success";
                }
                catch (Exception e)
                {
                    msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
                }
            }
            else
            {
                msg = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید.";
            }
            return Json(new { msg, status });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateShippingFeeTypeFromNormal(string contractRowId, long customerId)
        {
            var latestContracts = await _contractRepo.Contracts().AsNoTracking().Where(a => !a.RowId.Equals(contractRowId) && a.CustomerId.Equals(customerId))
                .Select(a => new { a.Number, a.RowId, a.EndDate }).OrderByDescending(a => a.EndDate).ToListAsync();
            if (latestContracts.Count == 0)
                return NotFound("قراردادی وجود ندارد.");

            return Json(latestContracts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DoCreateShippingFeeTypeFromNormal(string contractRowId, string newContractRowId)
        {
            var newContract = await _contractRepo.Get(newContractRowId);
            var contract = await _contractRepo.Get(contractRowId);
            foreach (var item in contract.ShippingFees)
            {
                _shippingFeeRepo.Create(new Domain.Models.ShippingFee
                {
                    ContractId = newContract.Id,
                    CreateDate = DateTime.Now,
                    CreatorId = _userManager.GetUserId(User),
                    DestinationId = item.DestinationId,
                    DriverPrice = item.DriverPrice,
                    DriverTonnagePrice = item.DriverTonnagePrice,
                    OriginId = item.OriginId,
                    Price = item.Price,
                    ShippingFeeLoadTypeId = item.ShippingFeeLoadTypeId,
                    ShippingFeeType = item.ShippingFeeType,
                    Title = item.Title,
                    TonnagePrice = item.TonnagePrice,
                    Vehicle = item.Vehicle
                });
            }
            try
            {
                await _shippingFeeRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region ShippingFeeLoadType
        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> ShippingFeeLoadType(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _shippingFeeLoadTypeRepo.ShippingFeeLoadTypes().OrderBy(a => a.Name).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public PartialViewResult CreateShippingFeeLoadType()
        {
            return PartialView("~/Views/Admin/Create/ShippingFeeLoadType.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateShippingFeeLoadType(ShippingFeeLoadType v)
        {
            if (ModelState.IsValid)
            {
                if (await _shippingFeeLoadTypeRepo.CheckNameExist(v.Name))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. نام در سیستم وجود دارد. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _shippingFeeLoadTypeRepo.Create(v);
                try
                {
                    await _shippingFeeLoadTypeRepo.Save();
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
        [Authorize(Roles = "Admin, Milad")]
        public async Task<PartialViewResult> EditShippingFeeLoadType(int id)
        {
            return PartialView("~/Views/Admin/Edit/ShippingFeeLoadType.cshtml", await _shippingFeeLoadTypeRepo.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditShippingFeeLoadType(ShippingFeeLoadType v)
        {
            if (ModelState.IsValid)
            {
                if (await _shippingFeeLoadTypeRepo.CheckNameExist(v.Name))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. نام در سیستم وجود دارد. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _shippingFeeLoadTypeRepo.Get(v.Id);
                item.Name = v.Name;

                _shippingFeeLoadTypeRepo.Update(item);
                try
                {
                    await _shippingFeeLoadTypeRepo.Save();
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

        #region LoadFactor
        [HttpGet]
        public async Task<IActionResult> LoadFactor(int? p)
        {
            var pageNumber = p ?? 1;
            if (pageNumber == 1)
            {
                if (!await _vehicleRepo.Vehicles().AnyAsync())
                {
                    TempData["msg"] = "برای ثبت بارنامه، باید حداقل یک خودرو ثبت نمائید. |danger";
                    return RedirectToAction("Vehicle");
                }

                if (!await _driverRepository.Drivers().AnyAsync())
                {
                    TempData["msg"] = "برای ثبت بارنامه، باید حداقل یک راننده ثبت نمائید. |danger";
                    return RedirectToAction("Driver");
                }
            }
            ViewData["Customer"] = await _customerRepo.GetAllActive();

            var query = _loadFactorRepo.LoadFactors();
            if (User.IsInRole("RegisterUser"))
                query = query.Where(a => a.AdminId.Equals(_userManager.GetUserId(User)));

            ViewBag.data = await query.OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 15);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadFactorPartial(int? p)
        {
            var pageNumber = p ?? 1;

            var query = _loadFactorRepo.LoadFactors();
            if (User.IsInRole("RegisterUser"))
                query = query.Where(a => a.AdminId.Equals(_userManager.GetUserId(User)));

            ViewBag.data = await query.OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 15);
            ViewBag.isSearch = false;
            return PartialView("_LoadFactor");
        }

        [HttpGet]
        public async Task<PartialViewResult> LoadFactorDetail(int id)
        {
            var item = await _loadFactorRepo.Get(id);
            ViewData["Admin"] = await _userManager.FindByIdAsync(item.AdminId);
            return PartialView("_LoadFactorDetail", item);
        }

        [HttpGet]
        public async Task<IActionResult> SearchLoadFactor(int? p, string exitNumber, string loadNumber, string vehicleNumber, bool isFreeDriverPrice, long? calendar)
        {
            if (!string.IsNullOrWhiteSpace(exitNumber) || !string.IsNullOrWhiteSpace(loadNumber) || !string.IsNullOrWhiteSpace(vehicleNumber) || calendar.HasValue)
            {
                var pageNum = p ?? 1;

                var query = _loadFactorRepo.LoadFactors();

                if (!string.IsNullOrWhiteSpace(exitNumber))
                    query = query.Where(a => a.ExitNumber.Contains(exitNumber));
                if (!string.IsNullOrWhiteSpace(loadNumber))
                    query = query.Where(a => a.LoadNumber.Contains(loadNumber));
                if (!string.IsNullOrWhiteSpace(vehicleNumber))
                    query = query.Where(a => vehicleNumber == (a.Vehicle.LeftNumber + " " + a.Vehicle.NumberWord + " " + a.Vehicle.RightNumber));
                if (calendar.HasValue && calendar.Value > 0)
                    query = query.Where(a => a.CalendarId.Equals(calendar.Value));

                if (isFreeDriverPrice)
                    query = query.Where(a => a.IsFreeDriverPrice);

                if (User.IsInRole("RegisterUser"))
                    query = query.Where(a => a.AdminId.Equals(_userManager.GetUserId(User)));

                ViewBag.data = await query.OrderByDescending(a => a.Id).ToPagedListAsync(pageNum, 15);
                ViewBag.page = pageNum;
                ViewBag.exitNumber = exitNumber;
                ViewBag.loadNumber = loadNumber;
                ViewBag.vehicleNumber = vehicleNumber;
                ViewBag.calendar = calendar;
                ViewBag.isFreeDriverPrice = isFreeDriverPrice;
                ViewBag.isSearch = true;
                return PartialView("_LoadFactor");
            }
            else
            {
                return BadRequest("لطفا یک مقدار برای جستجو انتخاب نمائید.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateLoadFactor(int customerId, byte customerType)
        {
            var contracts = await _contractRepo.Contracts().AsNoTracking().Where(a => a.CustomerId.Equals(customerId)).OrderByDescending(a => a.StartDate).ToListAsync();
            if (!contracts.Any()) return NotFound("قراردادی پیدا نشد.");

            var activeContract = contracts.OrderByDescending(a => a.StartDate).First().Id;
            if (User.IsInRole("Admin"))
                ViewData["Fees"] = await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(activeContract)).OrderBy(a => a.Origin).ToListAsync();
            else
                ViewData["Fees"] = await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(activeContract) && a.ShippingFeeType == ShippingFeeType.Normal).OrderBy(a => a.Origin).ToListAsync();

            ViewData["Contracts"] = contracts;

            var accountBooks = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.CustomerId.Equals(customerId)).OrderBy(a => a.IsOpen).ThenByDescending(a => a.Id).ToListAsync();
            if (!accountBooks.Any(a => a.IsOpen))
                return NotFound($"صورت وضعیت باز در سیستم وجود ندارد.");
            ViewData["AccountBooks"] = accountBooks;

            ViewData["Year"] = await _configRepo.CurrentYear();
            ViewData["Drivers"] = await _driverRepository.Drivers().AsNoTracking().Where(a => a.IsActive).OrderBy(a => a.Fullname).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepo.Vehicles().AsNoTracking().Where(a => a.Status).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();

            long sequence;
            switch (customerType)
            {
                case (byte)CustomerType.SaipaPlasco:
                    sequence = await _loadFactorRepo.GetBiggestSequenceInSaipaPlasco();
                    ViewData["Sequence"] = CalcNextSequenceForLoadFactor(sequence);
                    return PartialView("~/Views/Admin/Create/LoadFactor/SaipaPlasco.cshtml");
                case (byte)CustomerType.SaipaPress:
                    sequence = await _loadFactorRepo.GetBiggestSequenceInSaipaPress();
                    ViewData["LoadTypes"] = await _shippingFeeLoadTypeRepo.ShippingFeeLoadTypes().AsNoTracking().OrderBy(a => a.Name).ToListAsync();
                    ViewData["Sequence"] = CalcNextSequenceForLoadFactor(sequence);
                    return PartialView("~/Views/Admin/Create/LoadFactor/SaipaPress.cshtml");
                case (byte)CustomerType.SazehGostar:
                    sequence = await _loadFactorRepo.GetBiggestSequenceInSazehGostar();
                    ViewData["Sequence"] = CalcNextSequenceForLoadFactor(sequence);
                    return PartialView("~/Views/Admin/Create/LoadFactor/SazehGostar.cshtml");
                case (byte)CustomerType.MehrcomPars:
                    ViewData["Categories"] = await _mehrcomParsCategoryRepository.Categories().AsNoTracking().OrderBy(a => a.Title).ToListAsync();
                    return PartialView("~/Views/Admin/Create/LoadFactor/MehrcomPars.cshtml");
                default:
                    return NoContent();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSaipaPlascoLoadFactor(CSaipaPlascoLoadFactorVM input)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                var customerId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.Id.Equals(input.ContractId)).Select(a => a.CustomerId).FirstOrDefaultAsync();
                var customerLoadFactorDeduction = await _customerRepo.Customers().AsNoTracking().Where(a => a.Id.Equals(customerId)).Select(a => a.LoadFactorDeductions).FirstOrDefaultAsync();

                var fee = await _shippingFeeRepo.Get(input.ShippingFeeId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => a.LoadNumber.Equals(input.LoadNumber) && a.SaipaPlascoLoadFactor != null))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov))
                    if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => a.LoadNumberGov.Equals(input.LoadNumberGov)))
                        return NotFound("شماره بارنامه دولتی درج شده تکراری است.");

                var config = await _configRepo.LoadFactorTax();
                var loadFactor = new LoadFactor
                {
                    AdminId = _userManager.GetUserId(User),
                    OriginId = fee.OriginId,
                    DestinationId = fee.DestinationId,
                    CalendarId = input.CalendarId,
                    ContractId = input.ContractId,
                    Date = new PersianDateTime(input.Year, input.Month, input.Day, 0, 0, 0).ToDateTime(),
                    DriverId = input.DriverId,
                    ExitNumber = input.ExitNumber,
                    LoadNumber = input.LoadNumber,
                    LoadNumberGov = input.LoadNumberGov,
                    VehicleId = input.VehicleId,
                    ShippingFeeId = input.ShippingFeeId,
                    WithholdingTax = config.WithholdingTax,
                    VAT = config.VAT,
                    LoadFactorDeductions = customerLoadFactorDeduction,
                    AccountBookId = input.AccountBookId,
                    IsDriverFeeEditedByAdmin = false,
                    IsFreeDriverPrice = input.IsFreeDriverPrice
                };

                if (fee.ShippingFeeType == ShippingFeeType.Custom)
                {
                    loadFactor.Amount = input.Amount;
                    loadFactor.DriverFee = input.DriverFee;
                }
                else
                {
                    loadFactor.Amount = fee.Price;
                    loadFactor.DriverFee = fee.DriverPrice;
                }
                loadFactor.SaipaPlascoLoadFactor = new SaipaPlascoLoadFactor
                {
                    LoadFactor = loadFactor,
                    Sequence = await _loadFactorRepo.GetBiggestSequenceInSaipaPlasco() + 1
                };


                _loadFactorRepo.Create(loadFactor);

                try
                {
                    await _loadFactorRepo.Save();
                    msg = "عملیات موفقیت آمیز بود.";
                    status = "success";
                }
                //catch (SqlException sqlException)
                //{
                //    if (sqlException.Number == 2601 || sqlException.Number == 2627)
                //    {
                //        bool done = false;
                //        while (!done)
                //        {
                //            input.Sequence = input.Sequence++;
                //            loadFactor.SaipaPlascoLoadFactor.Sequence = input.Sequence;

                //            _loadFactorRepo.Create(loadFactor);
                //            try
                //            {
                //                await _loadFactorRepo.Save();
                //                done = true;
                //            }
                //            catch (SqlException sqlException2)
                //            {
                //                if (sqlException2.Number != 2601 || sqlException2.Number != 2627)
                //                {
                //                    msg = $"عملیات با خطا مواجه شد. جزئیات: {sqlException2.Message} #{sqlException2.Number}";
                //                    break;
                //                }
                //            }
                //            catch (Exception e)
                //            {
                //                msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
                //                break;
                //            }
                //        }

                //        msg = "عملیات موفقیت آمیز بود.";
                //        status = "success";
                //    }
                //    else
                //    {
                //        msg = $"عملیات با خطا مواجه شد. جزئیات: {sqlException.Message} #{sqlException.Number}";
                //    }
                //}
                catch (Exception e)
                {
                    msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
                }
            }
            else
            {
                msg = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید.";
            }
            return Json(new { msg, status, sequence = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSaipaPressLoadFactor(CSaipaPressLoadFactorVM input, bool HasNumber)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                var customerId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.Id.Equals(input.ContractId)).Select(a => a.CustomerId).FirstOrDefaultAsync();
                var customerLoadFactorDeduction = await _customerRepo.Customers().AsNoTracking().Where(a => a.Id.Equals(customerId)).Select(a => a.LoadFactorDeductions).FirstOrDefaultAsync();

                if (HasNumber && string.IsNullOrWhiteSpace(input.EntryNumber) && string.IsNullOrWhiteSpace(input.ExitNumber))
                    return NotFound("لطفا شماره ورود یا خروج را وارد نمائید.");

                if (input.PressFloorType == SaipaPressLoadType.OneFloor)
                {
                    if (!string.IsNullOrWhiteSpace(input.EntryNumber) && await _loadFactorRepo.LoadFactors().AnyAsync(a => a.SaipaPressLoadFactor.EntryNumber.Equals(input.EntryNumber)))
                        return NotFound("شماره ورود تکراری است.");

                    if (!string.IsNullOrWhiteSpace(input.ExitNumber) && await _loadFactorRepo.LoadFactors().AnyAsync(a => a.ExitNumber.Equals(input.ExitNumber)))
                        return NotFound("شماره خروج تکراری است.");
                }

                if (input.PressFloorType == SaipaPressLoadType.TwoFloor)
                {
                    if (!string.IsNullOrWhiteSpace(input.EntryNumber) && await _loadFactorRepo.LoadFactors().CountAsync(a => a.SaipaPressLoadFactor.EntryNumber.Equals(input.EntryNumber)) >= 2)
                        return NotFound("شماره ورود تکراری است.");

                    if (!string.IsNullOrWhiteSpace(input.ExitNumber) && await _loadFactorRepo.LoadFactors().CountAsync(a => a.ExitNumber.Equals(input.ExitNumber)) >= 2)
                        return NotFound("شماره خروج تکراری است.");
                }

                var fee = await _shippingFeeRepo.Get(input.ShippingFeeId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => a.LoadNumber.Equals(input.LoadNumber) && a.SaipaPressLoadFactor != null))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                var config = await _configRepo.LoadFactorTax();
                var loadFactor = new LoadFactor
                {
                    AdminId = _userManager.GetUserId(User),
                    OriginId = fee.OriginId,
                    DestinationId = fee.DestinationId,
                    CalendarId = input.CalendarId,
                    ContractId = input.ContractId,
                    Date = new PersianDateTime(input.Year, input.Month, input.Day, 0, 0, 0).ToDateTime(),
                    DriverId = input.DriverId,
                    ExitNumber = input.ExitNumber,
                    LoadNumber = input.LoadNumber,
                    VehicleId = input.VehicleId,
                    ShippingFeeId = input.ShippingFeeId,
                    WithholdingTax = config.WithholdingTax,
                    VAT = config.VAT,
                    LoadFactorDeductions = customerLoadFactorDeduction,
                    Tonnage = input.Tonnage,
                    AccountBookId = input.AccountBookId,
                    IsDriverFeeEditedByAdmin = false,
                    IsFreeDriverPrice = input.IsFreeDriverPrice
                };

                if (fee.ShippingFeeType == ShippingFeeType.Custom)
                {
                    loadFactor.Amount = input.Amount;
                    loadFactor.DriverFee = input.DriverFee;
                    loadFactor.TonnagePrice = input.TonnagePrice;
                    loadFactor.DriverTonnagePrice = input.DriverTonnagePrice;
                }
                else
                {
                    loadFactor.Amount = fee.Price;
                    loadFactor.DriverFee = fee.DriverPrice;
                    loadFactor.TonnagePrice = fee.TonnagePrice;
                    loadFactor.DriverTonnagePrice = fee.DriverTonnagePrice;
                }

                loadFactor.SaipaPressLoadFactor = new SaipaPressLoadFactor
                {
                    LoadFactorId = loadFactor.Id,
                    EntryNumber = input.EntryNumber,
                    LoadType = input.LoadType,
                    LoadFactor = loadFactor,
                    Sequence = input.Sequence,
                    PressFloorType = input.PressFloorType
                };

                _loadFactorRepo.Create(loadFactor);

                try
                {
                    await _loadFactorRepo.Save();
                    msg = "عملیات موفقیت آمیز بود.";
                    status = "success";
                }
                catch (SqlException sqlException)
                {
                    if (sqlException.Number == 2601 || sqlException.Number == 2627)
                    {
                        bool done = false;
                        while (!done)
                        {
                            input.Sequence = input.Sequence++;
                            loadFactor.SaipaPressLoadFactor.Sequence = input.Sequence;

                            _loadFactorRepo.Create(loadFactor);
                            try
                            {
                                await _loadFactorRepo.Save();
                                done = true;
                            }
                            catch (SqlException sqlException2)
                            {
                                if (sqlException2.Number != 2601 || sqlException2.Number != 2627)
                                {
                                    msg = $"عملیات با خطا مواجه شد. جزئیات: {sqlException2.Message} #{sqlException2.Number}";
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
                                break;
                            }
                        }

                        msg = "عملیات موفقیت آمیز بود.";
                        status = "success";
                    }
                    else
                    {
                        msg = $"عملیات با خطا مواجه شد. جزئیات: {sqlException.Message} #{sqlException.Number}";
                    }
                }
                catch (Exception e)
                {
                    msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
                }
            }
            else
            {
                msg = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید.";
            }
            return Json(new { msg, status, sequence = CalcNextSequenceForLoadFactor(input.Sequence) });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSazehGostarLoadFactor(CSazehGostarLoadFactorVM input)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                var customerId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.Id.Equals(input.ContractId)).Select(a => a.CustomerId).FirstOrDefaultAsync();
                var customerLoadFactorDeduction = await _customerRepo.Customers().AsNoTracking().Where(a => a.Id.Equals(customerId)).Select(a => a.LoadFactorDeductions).FirstOrDefaultAsync();

                var fee = await _shippingFeeRepo.Get(input.ShippingFeeId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                if ((input.SazehLoadType == SazehGostarLoadType.OneWay && fee.Title.Equals("رفت و برگشت")) ||
                    (input.SazehLoadType == SazehGostarLoadType.TwoWay && fee.Title.Equals("رفت")))
                    return NotFound("نوع انتخابی بارنامه با نوع رفت و برگشت درج شده در نرخ انتخابی تطابق ندارد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => a.LoadNumber.Equals(input.LoadNumber) && a.SazehGostarLoadFactor != null))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                var config = await _configRepo.LoadFactorTax();
                var loadFactor = new LoadFactor
                {
                    AdminId = _userManager.GetUserId(User),
                    OriginId = fee.OriginId,
                    DestinationId = fee.DestinationId,
                    CalendarId = input.CalendarId,
                    ContractId = input.ContractId,
                    Date = new PersianDateTime(input.Year, input.Month, input.Day, 0, 0, 0).ToDateTime(),
                    DriverId = input.DriverId,
                    ExitNumber = input.ExitNumber,
                    LoadNumber = input.LoadNumber,
                    VehicleId = input.VehicleId,
                    ShippingFeeId = input.ShippingFeeId,
                    WithholdingTax = config.WithholdingTax,
                    VAT = config.VAT,
                    LoadFactorDeductions = customerLoadFactorDeduction,
                    AccountBookId = input.AccountBookId,
                    IsDriverFeeEditedByAdmin = false,
                    IsFreeDriverPrice = input.IsFreeDriverPrice
                };

                if (fee.ShippingFeeType == ShippingFeeType.Custom)
                {
                    loadFactor.Amount = input.Amount;
                    loadFactor.DriverFee = input.DriverFee;
                }
                else
                {
                    loadFactor.Amount = fee.Price;
                    loadFactor.DriverFee = fee.DriverPrice;
                }

                loadFactor.SazehGostarLoadFactor = new SazehGostarLoadFactor
                {
                    LoadFactorId = loadFactor.Id,
                    Certain = input.Certain,
                    Count = input.Count,
                    Description = $"حمل کالا از {fee.Origin.Title} به {fee.Destination.Title}{(input.SazehLoadType == SazehGostarLoadType.TwoWay ? " رفت و برگشت" : "")} ({fee.Vehicle})",
                    DetailedCostCenter = input.DetailedCostCenter,
                    Nature = input.Nature,
                    RegisterCode = input.RegisterCode,
                    LoadFactor = loadFactor,
                    Sequence = await _loadFactorRepo.GetBiggestSequenceInSazehGostar() + 1,
                    SazehLoadType = input.SazehLoadType
                };

                _loadFactorRepo.Create(loadFactor);

                try
                {
                    await _loadFactorRepo.Save();
                    msg = "عملیات موفقیت آمیز بود.";
                    status = "success";
                }
                //catch (SqlException sqlException)
                //{
                //    if (sqlException.Number == 2601 || sqlException.Number == 2627)
                //    {
                //        bool done = false;
                //        while (!done)
                //        {
                //            input.Sequence = input.Sequence++;
                //            loadFactor.SazehGostarLoadFactor.Sequence = input.Sequence;

                //            _loadFactorRepo.Create(loadFactor);
                //            try
                //            {
                //                await _loadFactorRepo.Save();
                //                done = true;
                //            }
                //            catch (SqlException sqlException2)
                //            {
                //                if (sqlException2.Number != 2601 || sqlException2.Number != 2627)
                //                {
                //                    msg = $"عملیات با خطا مواجه شد. جزئیات: {sqlException2.Message} #{sqlException2.Number}";
                //                    break;
                //                }
                //            }
                //            catch (Exception e)
                //            {
                //                msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
                //                break;
                //            }
                //        }

                //        msg = "عملیات موفقیت آمیز بود.";
                //        status = "success";
                //    }
                //    else
                //    {
                //        msg = $"عملیات با خطا مواجه شد. جزئیات: {sqlException.Message} #{sqlException.Number}";
                //    }
                //}
                catch (Exception e)
                {
                    msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
                }
            }
            else
            {
                msg = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید.";
            }
            return Json(new { msg, status, sequence = 1 /*CalcNextSequenceForLoadFactor(input.Sequence)*/ });
        }

        [HttpPost]
        public async Task<IActionResult> CreateMehrcomParsLoadFactor(CMehrcomParsLoadFactorVM input)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                var customerId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.Id.Equals(input.ContractId)).Select(a => a.CustomerId).FirstOrDefaultAsync();
                var customerLoadFactorDeduction = await _customerRepo.Customers().AsNoTracking().Where(a => a.Id.Equals(customerId)).Select(a => a.LoadFactorDeductions).FirstOrDefaultAsync();

                var fee = await _shippingFeeRepo.Get(input.ShippingFeeId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                string loadType = fee.ShippingFeeLoadType.Name;

                if ((!input.Load && !input.Palette && !input.Return) ||
                    (input.Load && input.Palette && !input.Return && !loadType.Contains("بار/پالت")) ||
                    (input.Load && !input.Palette && !input.Return && (!loadType.Contains("بار") || loadType.Contains("پالت") || loadType.Contains("برگشت"))) ||
                    (!input.Load && input.Palette && !input.Return && (!loadType.Contains("پالت") || loadType.Contains("بار") || loadType.Contains("برگشت"))) ||
                    (!input.Load && !input.Palette && input.Return && (!loadType.Contains("برگشت") || loadType.Contains("پالت") || loadType.Contains("بار"))) ||
                    (input.Load && !input.Palette && input.Return && !loadType.Contains("بار/برگشت")) ||
                    (!input.Load && input.Palette && input.Return && !loadType.Contains("پالت/برگشت"))
                    )
                    return NotFound("مقادیر بار/پالت/برگشت با نرخ انتخابی تناسب ندارد.");

                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => a.LoadNumber.Equals(input.LoadNumber) && a.MehrcomParsLoadFactor != null))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov))
                    if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => a.LoadNumberGov.Equals(input.LoadNumberGov)))
                        return NotFound("شماره بارنامه دولتی درج شده تکراری است.");

                if (!string.IsNullOrWhiteSpace(input.LoadNumberGovReturn))
                    if (await _loadFactorRepo.CheckMehrcomParsLoadFactorGovNumber(input.LoadNumberGovReturn))
                        return NotFound("شماره بارنامه برگشتی درج شده تکراری است.");

                var config = await _configRepo.LoadFactorTax();
                var loadFactor = new LoadFactor
                {
                    AdminId = _userManager.GetUserId(User),
                    OriginId = fee.OriginId,
                    DestinationId = fee.DestinationId,
                    CalendarId = input.CalendarId,
                    ContractId = input.ContractId,
                    Date = new PersianDateTime(input.Year, input.Month, input.Day, 0, 0, 0).ToDateTime(),
                    DriverId = input.DriverId,
                    LoadNumber = input.LoadNumber,
                    LoadNumberGov = input.LoadNumberGov,
                    VehicleId = input.VehicleId,
                    ShippingFeeId = input.ShippingFeeId,
                    WithholdingTax = config.WithholdingTax,
                    VAT = config.VAT,
                    LoadFactorDeductions = customerLoadFactorDeduction,
                    AccountBookId = input.AccountBookId,
                    Tonnage = input.Tonnage,
                    WeighbridgePrice = input.WeighbridgePrice,
                    LoadSleepTime = input.LoadSleepTime,
                    LoadSleepPrice = input.LoadSleepPrice,
                    DriverLoadSleepPrice = input.DriverLoadSleepPrice,
                    IsDriverFeeEditedByAdmin = false,
                    IsFreeDriverPrice = input.IsFreeDriverPrice
                };

                if (fee.ShippingFeeType == ShippingFeeType.Custom)
                {
                    loadFactor.Amount = input.HasAddonMessage ? input.Amount + (input.Amount * 0.3) : input.Amount;
                    loadFactor.DriverFee = input.HasAddonMessage ? input.DriverFee + (input.DriverFee * 0.3) : input.DriverFee;
                    loadFactor.TonnagePrice = input.TonnagePrice;
                    loadFactor.DriverTonnagePrice = input.DriverTonnagePrice;
                }
                else
                {
                    loadFactor.Amount = input.HasAddonMessage ? fee.Price + (fee.Price * 0.3) : fee.Price;
                    loadFactor.DriverFee = input.HasAddonMessage ? fee.DriverPrice + (fee.DriverPrice * 0.3) : fee.DriverPrice;
                    loadFactor.TonnagePrice = fee.TonnagePrice;
                    loadFactor.DriverTonnagePrice = fee.DriverTonnagePrice;
                }
                loadFactor.MehrcomParsLoadFactor = new MehrcomParsLoadFactor
                {
                    Load = input.Load,
                    LoadNumberGovReturn = input.LoadNumberGovReturn,
                    Palette = input.Palette,
                    Return = input.Return,
                    LoadFactor = loadFactor,
                    LoadFactorId = loadFactor.Id,
                    CategoryId = input.CategoryId,
                    HasAddonMessage = input.HasAddonMessage,
                    Sequence = await _loadFactorRepo.GetBiggestSequenceInMehrcomPars() + 1
                };

                _loadFactorRepo.Create(loadFactor);

                try
                {
                    await _loadFactorRepo.Save();
                    msg = "عملیات موفقیت آمیز بود.";
                    status = "success";
                }
                catch (Exception e)
                {
                    msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
                }
            }
            else
            {
                msg = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید.";
            }
            return Json(new { msg, status, sequence = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> EditLoadFactor(int loadFactorId)
        {
            var loadFactor = await _loadFactorRepo.Get(loadFactorId);

            if (loadFactor == null) return NotFound("بارنامه پیدا نشد.");

            var customer = loadFactor.Contract.Customer;
            var contracts = await _contractRepo.Contracts().AsNoTracking().Where(a => a.CustomerId.Equals(customer.Id)).OrderByDescending(a => a.StartDate).ToListAsync();
            if (!contracts.Any()) return NotFound("قراردادی پیدا نشد.");

            ViewData["Contracts"] = contracts;
            ViewData["Drivers"] = await _driverRepository.Drivers().AsNoTracking().OrderBy(a => a.Fullname).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepo.Vehicles().AsNoTracking().Where(a => a.Status).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            if (User.IsInRole("Admin"))
                ViewData["Fees"] = await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(loadFactor.ContractId)).OrderBy(a => a.Origin).ToListAsync();
            else
                ViewData["Fees"] = await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(loadFactor.ContractId) && a.ShippingFeeType == ShippingFeeType.Normal).OrderBy(a => a.Origin).ToListAsync();

            if (customer.CustomerType == CustomerType.SaipaPress)
                ViewData["LoadTypes"] = await _shippingFeeLoadTypeRepo.ShippingFeeLoadTypes().AsNoTracking().OrderBy(a => a.Name).ToListAsync();
            else if (customer.CustomerType == CustomerType.MehrcomPars)
                ViewData["Categories"] = await _mehrcomParsCategoryRepository.Categories().AsNoTracking().OrderBy(a => a.Title).ToListAsync();

            var accountBooks = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.CustomerId.Equals(customer.Id)).OrderBy(a => a.IsOpen).ThenByDescending(a => a.Id).ToListAsync();
            if (!accountBooks.Any())
                return NotFound($"صورت وضعیت باز در سیستم برای {customer.Name} وجود ندارد.");
            ViewData["AccountBooks"] = accountBooks;

            return customer.CustomerType switch
            {
                CustomerType.SaipaPlasco => PartialView("~/Views/Admin/Edit/LoadFactor/SaipaPlasco.cshtml", await _loadFactorRepo.GetSaipaPlascoLoadFactor(loadFactorId)),
                CustomerType.SaipaPress => PartialView("~/Views/Admin/Edit/LoadFactor/SaipaPress.cshtml", await _loadFactorRepo.GetSaipaPressLoadFactor(loadFactorId)),
                CustomerType.SazehGostar => PartialView("~/Views/Admin/Edit/LoadFactor/SazehGostar.cshtml", await _loadFactorRepo.GetSazehGostarLoadFactor(loadFactorId)),
                CustomerType.MehrcomPars => PartialView("~/Views/Admin/Edit/LoadFactor/MehrcomPars.cshtml", await _loadFactorRepo.GetMehrcomParsLoadFactor(loadFactorId)),
                _ => NoContent(),
            };
        }

        [HttpPost]
        public async Task<IActionResult> EditSaipaPlascoLoadFactor(ESaipaPlascoLoadFactorVM input)
        {
            if (ModelState.IsValid)
            {
                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && (a.LoadNumber.Equals(input.LoadNumber))))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov))
                    if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumberGov.Equals(input.LoadNumberGov)))
                        return NotFound("شماره بارنامه دولتی درج شده تکراری است.");

                var fee = await _shippingFeeRepo.Get(input.ShippingFeeId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                var item = await _loadFactorRepo.Get(input.Id);
                if (item == null) return NotFound();

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (item.AccountBookId != input.AccountBookId && await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");


                //if (await _loadFactorRepo.SequenceExistInSaipaPlasco(item.Id, input.Sequence))
                //    return NotFound("ترتیب وارد شده برای بارنامه تکراری است");

                item.EditorId = _userManager.GetUserId(User);
                item.EditDateTime = DateTime.Now;
                item.OriginId = fee.OriginId;
                item.DestinationId = fee.DestinationId;
                item.CalendarId = input.CalendarId;
                item.ContractId = input.ContractId;
                item.Date = new PersianDateTime(input.Year, input.Month, input.Day, 0, 0, 0).ToDateTime();
                item.DriverId = input.DriverId;
                item.ExitNumber = input.ExitNumber;
                item.LoadNumber = input.LoadNumber;
                item.LoadNumberGov = input.LoadNumberGov;
                item.VehicleId = input.VehicleId;
                item.ShippingFeeId = input.ShippingFeeId;
                item.AccountBookId = input.AccountBookId;
                item.IsFreeDriverPrice = input.IsFreeDriverPrice;

                //item.SaipaPlascoLoadFactor.Sequence = input.Sequence;

                if (fee.ShippingFeeType == ShippingFeeType.Custom)
                {
                    item.Amount = input.Amount;
                    if (!item.IsDriverFeeEditedByAdmin)
                        item.DriverFee = input.DriverFee;
                }
                else
                {
                    item.Amount = fee.Price;
                    if (!item.IsDriverFeeEditedByAdmin)
                        item.DriverFee = fee.DriverPrice;
                }

                _loadFactorRepo.Update(item);
                try
                {
                    await _loadFactorRepo.Save();
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
        public async Task<IActionResult> EditSaipaPressLoadFactor(ESaipaPressLoadFactorVM input, bool HasNumber)
        {
            if (ModelState.IsValid)
            {
                if (HasNumber && string.IsNullOrWhiteSpace(input.EntryNumber) && string.IsNullOrWhiteSpace(input.ExitNumber))
                    return NotFound("لطفا شماره ورود یا خروج را وارد نمائید.");

                if (input.PressFloorType == SaipaPressLoadType.OneFloor)
                {
                    if (!string.IsNullOrWhiteSpace(input.EntryNumber) && await _loadFactorRepo.LoadFactors().AnyAsync(a => !a.Id.Equals(input.Id) && a.SaipaPressLoadFactor.EntryNumber.Equals(input.EntryNumber)))
                        return NotFound("شماره ورود تکراری است.");

                    if (!string.IsNullOrWhiteSpace(input.ExitNumber) && await _loadFactorRepo.LoadFactors().AnyAsync(a => !a.Id.Equals(input.Id) && a.ExitNumber.Equals(input.ExitNumber)))
                        return NotFound("شماره خروج تکراری است.");
                }

                if (input.PressFloorType == SaipaPressLoadType.TwoFloor)
                {
                    if (!string.IsNullOrWhiteSpace(input.EntryNumber) && await _loadFactorRepo.LoadFactors().CountAsync(a => !a.Id.Equals(input.Id) && a.SaipaPressLoadFactor.EntryNumber.Equals(input.EntryNumber)) >= 2)
                        return NotFound("شماره ورود تکراری است.");

                    if (!string.IsNullOrWhiteSpace(input.ExitNumber) && await _loadFactorRepo.LoadFactors().CountAsync(a => !a.Id.Equals(input.Id) && a.ExitNumber.Equals(input.ExitNumber)) >= 2)
                        return NotFound("شماره خروج تکراری است.");
                }

                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumber.Equals(input.LoadNumber)))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                var fee = await _shippingFeeRepo.Get(input.ShippingFeeId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                var item = await _loadFactorRepo.Get(input.Id);
                if (item == null) return NotFound();

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (item.AccountBookId != input.AccountBookId && await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                if (await _loadFactorRepo.SequenceExistInSaipaPress(item.Id, input.Sequence))
                    return NotFound("ترتیب وارد شده برای بارنامه تکراری است");

                item.EditorId = _userManager.GetUserId(User);
                item.EditDateTime = DateTime.Now;
                item.OriginId = fee.OriginId;
                item.DestinationId = fee.DestinationId;
                item.CalendarId = input.CalendarId;
                item.ContractId = input.ContractId;
                item.Date = new PersianDateTime(input.Year, input.Month, input.Day, 0, 0, 0).ToDateTime();
                item.DriverId = input.DriverId;
                item.ExitNumber = input.ExitNumber;
                item.LoadNumber = input.LoadNumber;
                item.VehicleId = input.VehicleId;
                item.ShippingFeeId = input.ShippingFeeId;
                item.Tonnage = input.Tonnage;
                item.AccountBookId = input.AccountBookId;
                item.IsFreeDriverPrice = input.IsFreeDriverPrice;

                item.SaipaPressLoadFactor.Sequence = input.Sequence;
                item.SaipaPressLoadFactor.EntryNumber = input.EntryNumber;
                item.SaipaPressLoadFactor.LoadType = input.LoadType;
                item.SaipaPressLoadFactor.PressFloorType = input.PressFloorType;

                if (fee.ShippingFeeType == ShippingFeeType.Custom)
                {
                    item.Amount = input.Amount;
                    if (!item.IsDriverFeeEditedByAdmin)
                    {
                        item.DriverFee = input.DriverFee;
                        item.DriverTonnagePrice = input.DriverTonnagePrice;
                    }
                    item.TonnagePrice = input.TonnagePrice;
                }
                else
                {
                    item.Amount = fee.Price;
                    if (!item.IsDriverFeeEditedByAdmin)
                    {
                        item.DriverFee = fee.DriverPrice;
                        item.DriverTonnagePrice = fee.DriverTonnagePrice;
                    }
                    item.TonnagePrice = fee.TonnagePrice;
                }

                _loadFactorRepo.Update(item);
                try
                {
                    await _loadFactorRepo.Save();
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
        public async Task<IActionResult> EditSazehGostarLoadFactor(ESazehGostarLoadFactorVM input)
        {
            if (ModelState.IsValid)
            {
                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumber.Equals(input.LoadNumber)))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                var fee = await _shippingFeeRepo.Get(input.ShippingFeeId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                if ((input.SazehLoadType == SazehGostarLoadType.OneWay && fee.Title.Equals("رفت و برگشت")) ||
                    (input.SazehLoadType == SazehGostarLoadType.TwoWay && fee.Title.Equals("رفت")))
                    return NotFound("نوع انتخابی بارنامه با نوع رفت و برگشت درج شده در نرخ انتخابی تطابق ندارد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                var item = await _loadFactorRepo.Get(input.Id);
                if (item == null) return NotFound();

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (item.AccountBookId != input.AccountBookId && await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                //if (await _loadFactorRepo.SequenceExistInSazehGostar(item.Id, input.Sequence))
                //    return NotFound("ترتیب وارد شده برای بارنامه تکراری است");

                item.EditorId = _userManager.GetUserId(User);
                item.EditDateTime = DateTime.Now;
                item.OriginId = fee.OriginId;
                item.DestinationId = fee.DestinationId;
                item.CalendarId = input.CalendarId;
                item.ContractId = input.ContractId;
                item.Date = new PersianDateTime(input.Year, input.Month, input.Day, 0, 0, 0).ToDateTime();
                item.DriverId = input.DriverId;
                item.ExitNumber = input.ExitNumber;
                item.LoadNumber = input.LoadNumber;
                item.VehicleId = input.VehicleId;
                item.ShippingFeeId = input.ShippingFeeId;
                item.AccountBookId = input.AccountBookId;
                item.IsFreeDriverPrice = input.IsFreeDriverPrice;

                //item.SazehGostarLoadFactor.Sequence = input.Sequence;
                item.SazehGostarLoadFactor.Certain = input.Certain;
                item.SazehGostarLoadFactor.Count = input.Count;
                item.SazehGostarLoadFactor.Description = $"حمل کالا از {fee.Origin.Title} به {fee.Destination.Title}{(input.SazehLoadType == SazehGostarLoadType.TwoWay ? " رفت و برگشت" : "")} ({fee.Vehicle})";
                item.SazehGostarLoadFactor.DetailedCostCenter = input.DetailedCostCenter;
                item.SazehGostarLoadFactor.Nature = input.Nature;
                item.SazehGostarLoadFactor.RegisterCode = input.RegisterCode;
                item.SazehGostarLoadFactor.SazehLoadType = input.SazehLoadType;

                if (fee.ShippingFeeType == ShippingFeeType.Custom)
                {
                    item.Amount = input.Amount;
                    if (!item.IsDriverFeeEditedByAdmin)
                        item.DriverFee = input.DriverFee;
                }
                else
                {
                    item.Amount = fee.Price;
                    if (!item.IsDriverFeeEditedByAdmin)
                        item.DriverFee = fee.DriverPrice;
                }

                _loadFactorRepo.Update(item);
                try
                {
                    await _loadFactorRepo.Save();
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
        public async Task<IActionResult> EditMehrcomParsLoadFactor(EMehrcomParsLoadFactorVM input)
        {
            if (ModelState.IsValid)
            {
                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && (a.LoadNumber.Equals(input.LoadNumber))))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov))
                    if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumberGov.Equals(input.LoadNumberGov)))
                        return NotFound("شماره بارنامه دولتی درج شده تکراری است.");

                var fee = await _shippingFeeRepo.Get(input.ShippingFeeId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                string loadType = fee.ShippingFeeLoadType.Name;

                if ((!input.Load && !input.Palette && !input.Return) ||
                    (input.Load && input.Palette && !input.Return && !loadType.Contains("بار/پالت")) ||
                    (input.Load && !input.Palette && !input.Return && (!loadType.Contains("بار") || loadType.Contains("پالت") || loadType.Contains("برگشت"))) ||
                    (!input.Load && input.Palette && !input.Return && (!loadType.Contains("پالت") || loadType.Contains("بار") || loadType.Contains("برگشت"))) ||
                    (!input.Load && !input.Palette && input.Return && (!loadType.Contains("برگشت") || loadType.Contains("پالت") || loadType.Contains("بار"))) ||
                    (input.Load && !input.Palette && input.Return && !loadType.Contains("بار/برگشت")) ||
                    (!input.Load && input.Palette && input.Return && !loadType.Contains("پالت/برگشت"))
                    )
                    return NotFound("مقادیر بار/پالت/برگشت با نرخ انتخابی تناسب ندارد.");

                var item = await _loadFactorRepo.Get(input.Id);
                if (item == null) return NotFound();

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (item.AccountBookId != input.AccountBookId && await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                item.EditorId = _userManager.GetUserId(User);
                item.EditDateTime = DateTime.Now;
                item.OriginId = fee.OriginId;
                item.DestinationId = fee.DestinationId;
                item.CalendarId = input.CalendarId;
                item.ContractId = input.ContractId;
                item.Date = new PersianDateTime(input.Year, input.Month, input.Day, 0, 0, 0).ToDateTime();
                item.DriverId = input.DriverId;
                item.LoadNumber = input.LoadNumber;
                item.LoadNumberGov = input.LoadNumberGov;
                item.VehicleId = input.VehicleId;
                item.ShippingFeeId = input.ShippingFeeId;
                item.AccountBookId = input.AccountBookId;
                item.Tonnage = input.Tonnage;
                item.IsFreeDriverPrice = input.IsFreeDriverPrice;

                item.MehrcomParsLoadFactor.LoadNumberGovReturn = input.LoadNumberGovReturn;
                item.MehrcomParsLoadFactor.Return = input.Return;
                item.MehrcomParsLoadFactor.Palette = input.Palette;
                item.MehrcomParsLoadFactor.Load = input.Load;
                item.WeighbridgePrice = input.WeighbridgePrice;
                item.MehrcomParsLoadFactor.CategoryId = input.CategoryId;
                item.DriverLoadSleepPrice = input.DriverLoadSleepPrice;
                item.LoadSleepPrice = input.LoadSleepPrice;
                item.LoadSleepTime = input.LoadSleepTime;
                item.MehrcomParsLoadFactor.HasAddonMessage = input.HasAddonMessage;

                if (fee.ShippingFeeType == ShippingFeeType.Custom)
                {
                    item.Amount = input.HasAddonMessage ? input.Amount + (input.Amount * 0.3) : input.Amount;
                    if (!item.IsDriverFeeEditedByAdmin)
                    {
                        item.DriverFee = input.HasAddonMessage ? input.DriverFee + (input.DriverFee * 0.3) : input.DriverFee;
                        item.DriverTonnagePrice = input.DriverTonnagePrice;
                    }
                    item.TonnagePrice = input.TonnagePrice;
                }
                else
                {
                    item.Amount = input.HasAddonMessage ? fee.Price + (fee.Price * 0.3) : fee.Price;
                    if (!item.IsDriverFeeEditedByAdmin)
                    {
                        item.DriverFee = input.HasAddonMessage ? fee.DriverPrice + (fee.DriverPrice * 0.3) : fee.DriverPrice;
                        item.DriverTonnagePrice = fee.DriverTonnagePrice;
                    }
                    item.TonnagePrice = fee.TonnagePrice;
                }

                _loadFactorRepo.Update(item);
                try
                {
                    await _loadFactorRepo.Save();
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
        public async Task<JsonResult> GetShippingFeeJson(string contractId)
        {
            var query = _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(contractId));
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.ShippingFeeType == ShippingFeeType.Normal);

            return Json(await query.OrderBy(a => a.Origin).Select(a => new
            {
                Destination = a.Destination.Title,
                driverPrice = a.DriverPrice.ToString("N0"),
                a.Id,
                Origin = a.Origin.Title,
                price = a.Price.ToString("N0"),
                a.Vehicle,
                a.Title,
                LoadType = a.ShippingFeeLoadType.Name
            }).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLoadFactor(int id)
        {
            var item = await _loadFactorRepo.Get(id);
            if (item == null) return NotFound();

            if (item.SazehGostarLoadFactor != null)
                _loadFactorRepo.DeleteSazehGostar(item.SazehGostarLoadFactor);

            if (item.SaipaPressLoadFactor != null)
                _loadFactorRepo.DeleteSaipaPress(item.SaipaPressLoadFactor);

            Log.Information($"بارنامه با شماره {item.LoadNumber} و مبدا {item.Origin.Title} و مقصد {item.Destination.Title} حذف شد.");

            _loadFactorRepo.Delete(item);
            try
            {
                await _loadFactorRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MoveLoadFactorsToNewContract(string contractRowId, string newContractRowId, string dateString)
        {
            var dateArr = dateString.PersianToEnglish().Split("/");
            var date = new PersianDateTime(Convert.ToInt32(dateArr[0]), Convert.ToInt32(dateArr[1]), Convert.ToInt32(dateArr[2])).ToDateTime();

            var newContract = await _contractRepo.Get(newContractRowId);
            var newShippingFees = newContract.ShippingFees;
            var contract = await _contractRepo.Get(contractRowId);
            var shippingFees = contract.ShippingFees;

            var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.ContractId.Equals(contract.Id) && a.Date >= date).ToListAsync();

            foreach (var item in loadFactors)
            {
                var shippingFee = shippingFees.Single(a => a.Id.Equals(item.ShippingFeeId));
                var newShippingFeeQuery = newShippingFees.Where(a => a.Vehicle.Equals(shippingFee.Vehicle)
                && a.OriginId.Equals(shippingFee.OriginId) && a.DestinationId.Equals(shippingFee.DestinationId)
                && a.ShippingFeeLoadTypeId.Equals(shippingFee.ShippingFeeLoadTypeId) && a.ShippingFeeType.Equals(shippingFee.ShippingFeeType));

                if (!string.IsNullOrWhiteSpace(shippingFee.Title))
                    newShippingFeeQuery = newShippingFeeQuery.Where(a => a.Title.Equals(shippingFee.Title));

                var newShippingFee = newShippingFeeQuery.Single();

                item.ContractId = newContract.Id;
                item.ShippingFeeId = newShippingFee.Id;

                if (newShippingFee.ShippingFeeType != ShippingFeeType.Custom)
                {
                    item.Amount = newShippingFee.Price;
                    if (!item.IsDriverFeeEditedByAdmin)
                        item.DriverFee = newShippingFee.DriverPrice;

                    item.TonnagePrice = newShippingFee.TonnagePrice;
                    item.DriverTonnagePrice = newShippingFee.DriverTonnagePrice;
                }
                _loadFactorRepo.Update(item);
            }


            try
            {
                await _loadFactorRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditLoadFactorDriverFee(long Id, double Fee, double TonnageFee, bool IsFree)
        {
            var item = await _loadFactorRepo.Get(Id);
            item.DriverFee = Fee;
            item.DriverTonnagePrice = TonnageFee > 0 ? TonnageFee : item.DriverTonnagePrice;
            item.IsFreeDriverPrice = IsFree;
            item.IsDriverFeeEditedByAdmin = true;
            _loadFactorRepo.Update(item);
            try
            {
                await _loadFactorRepo.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region AccountBookLoadFactor
        public async Task<IActionResult> AccountBookLoadFactor(string id)
        {
            var accountBook = await _accountBookRepository.AccountBooks().AsNoTracking().SingleOrDefaultAsync(a => a.RowId.Equals(id));
            ViewData["AccountBook"] = accountBook;

            return View(await _loadFactorRepo.LoadFactors().Where(a => a.AccountBookId.Equals(accountBook.Id)).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> MoveAccountBookLoadFactor(string accountBookRowId, long[] idList)
        {
            if (idList.Length > 0)
            {
                var accountBookId = await _accountBookRepository.AccountBooks().Where(a => a.RowId.Equals(accountBookRowId)).Select(a => a.Id).SingleOrDefaultAsync();
                var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => idList.Contains(a.Id)).ToListAsync();

                foreach (var item in loadFactors)
                {
                    item.AccountBookId = accountBookId;
                }

                try
                {
                    await _loadFactorRepo.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region LoadRoute
        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> LoadRoute(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _loadRouteRepo.LoadRoutes().OrderBy(a => a.Title).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public PartialViewResult CreateLoadRoute()
        {
            return PartialView("~/Views/Admin/Create/LoadRoute.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateLoadRoute(LoadRoutes v)
        {
            if (ModelState.IsValid)
            {
                if (await _loadRouteRepo.LoadRoutes().AnyAsync(a => a.Title.Equals(v.Title) && a.RouteType.Equals(v.RouteType)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. عنوان در سیستم وجود دارد. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _loadRouteRepo.Create(v);
                try
                {
                    await _loadRouteRepo.Save();
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
        [Authorize(Roles = "Admin, Milad")]
        public async Task<PartialViewResult> EditLoadRoute(int id)
        {
            return PartialView("~/Views/Admin/Edit/LoadRoute.cshtml", await _loadRouteRepo.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditLoadRoute(LoadRoutes v)
        {
            if (ModelState.IsValid)
            {
                if (await _loadRouteRepo.LoadRoutes().AnyAsync(a => !a.Id.Equals(v.Id) && a.Title.Equals(v.Title) && a.RouteType.Equals(v.RouteType)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. عنوان در سیستم وجود دارد. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _loadRouteRepo.Get(v.Id);
                item.Title = v.Title;
                item.RouteType = v.RouteType;

                _loadRouteRepo.Update(item);
                try
                {
                    await _loadRouteRepo.Save();
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

        #region AccountBook

        [HttpGet]
        public async Task<IActionResult> AccountBook(int? p)
        {
            var pageNumber = p ?? 1;
            var query = _accountBookRepository.AccountBooks();
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.CreatorId.Equals(_userManager.GetUserId(User)));

            var onePageOfData = await query.OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchAccountBook(int? p, string param)
        {
            var pageNum = p ?? 1;
            var query = _accountBookRepository.AccountBooks().Where(a => a.Number.Contains(param) || a.FactorNumber.Contains(param));
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.CreatorId.Equals(_userManager.GetUserId(User)));

            var onePageOfData = await query.OrderByDescending(a => a.Id).ToPagedListAsync(pageNum, 15);
            ViewBag.data = onePageOfData;
            ViewBag.param = param;

            return PartialView("_AccountBook");
        }

        [HttpGet]
        public async Task<IActionResult> CreateAccountBook()
        {
            ViewData["Customers"] = await _customerRepo.Customers().AsNoTracking().OrderBy(a => a.Name).ToListAsync();
            return PartialView("~/Views/Admin/Create/AccountBook.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccountBook(CreateAccountBookVM c)
        {
            if (ModelState.IsValid)
            {
                if (await _accountBookRepository.AccountBooks().AnyAsync(a => a.Number.Equals(c.Number)))
                {
                    TempData["msg"] = "شماره صورت وضعیت وارد شده تکراری است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _accountBookRepository.Create(new AccountBook
                {
                    Number = c.Number,
                    CreateDatetime = DateTime.Now,
                    CustomerId = c.CustomerId,
                    IsOpen = true,
                    FactorNumber = c.FactorNumber,
                    LoadFactorLimit = c.LoadFactorLimit,
                    CreatorId = _userManager.GetUserId(User)
                });
                try
                {
                    await _accountBookRepository.Save();
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
        public async Task<PartialViewResult> EditAccountBook(long id)
        {
            ViewData["Customers"] = await _customerRepo.Customers().AsNoTracking().OrderBy(a => a.Name).ToListAsync();

            var item = await _accountBookRepository.Get(id);

            return PartialView("~/Views/Admin/Edit/AccountBook.cshtml", new EditAccountBookVM
            {
                Id = item.Id,
                CustomerId = item.CustomerId,
                FactorNumber = item.FactorNumber,
                Number = item.Number,
                LoadFactorLimit = item.LoadFactorLimit
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditAccountBook(EditAccountBookVM c)
        {
            if (ModelState.IsValid)
            {
                if (await _accountBookRepository.AccountBooks().AnyAsync(a => !a.Id.Equals(c.Id) && a.Number.Equals(c.Number)))
                {
                    TempData["msg"] = "شماره صورت وضعیت تکراری است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _accountBookRepository.Get(c.Id);
                item.FactorNumber = c.FactorNumber;
                item.Number = c.Number;
                item.CustomerId = c.CustomerId;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDatetime = DateTime.Now;
                item.LoadFactorLimit = c.LoadFactorLimit;
                _accountBookRepository.Update(item);
                try
                {
                    await _accountBookRepository.Save();
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
        public async Task<IActionResult> CloseAccountBook(string rowId)
        {
            var item = await _accountBookRepository.Get(rowId);
            item.IsOpen = false;
            try
            {
                await _accountBookRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<IActionResult> OpenAccountBook(string rowId)
        {
            var item = await _accountBookRepository.Get(rowId);
            item.IsOpen = true;
            try
            {
                await _accountBookRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<JsonResult> GetAccountBookListByCustomerId(long customerId)
        {
            return Json(await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.CustomerId.Equals(customerId))
                .Select(a => new
                {
                    a.Id,
                    a.Number,
                    a.IsOpen,
                    Status = a.IsOpen ? "باز" : "بسته"
                }).OrderBy(a => a.IsOpen).ThenByDescending(a => a.Id).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountBook(string id)
        {
            var item = await _accountBookRepository.Get(id);
            if (!item.LoadFactors.Any())
            {
                _accountBookRepository.Delete(item);
                try
                {
                    await _accountBookRepository.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "این صورت وضعیت دارای بارنامه می باشد. |danger";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> GetAccountBookJson()
        {
            var query = _accountBookRepository.AccountBooks().Where(a => a.IsOpen);
            if (User.IsInRole("RegisterUser"))
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(a => a.CreatorId.Equals(userId));
            }
            return Json(await query.Select(a => new { a.RowId, a.Number, Customer = a.Customer.Name }).ToListAsync());
        }
        #endregion

        #region Driver
        [HttpGet]
        public async Task<IActionResult> Driver(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _driverRepository.Drivers().OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchDriver(int? p, string param)
        {
            var pageNum = p ?? 1;
            var onePageOfData = await _driverRepository.Drivers().Where(a => a.Fullname.Contains(param) || a.NationalNumber.Contains(param)).OrderByDescending(a => a.Id).ToPagedListAsync(pageNum, 15);
            ViewBag.data = onePageOfData;
            ViewBag.param = param;

            return PartialView("_Driver");
        }

        [HttpGet]
        public async Task<IActionResult> CreateDriver()
        {
            ViewData["Customers"] = await _customerRepo.Customers().AsNoTracking().OrderBy(a => a.Name).ToListAsync();
            return PartialView("~/Views/Admin/Create/Driver.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateDriver(Driver c)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(c.NationalNumber))
                    if (await _driverRepository.Drivers().AnyAsync(a => a.NationalNumber.Equals(c.NationalNumber)))
                    {
                        TempData["msg"] = "کد ملی وارد شده تکراری است. |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }

                _driverRepository.Create(new Driver
                {
                    AccountBankName = c.AccountBankName,
                    BankAccountNumber = c.BankAccountNumber,
                    Fullname = c.Fullname,
                    Phonenumber = c.Phonenumber,
                    NationalNumber = c.NationalNumber,
                    IsActive = c.IsActive,
                    CreatorId = _userManager.GetUserId(User)
                });
                try
                {
                    await _driverRepository.Save();
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
        public async Task<PartialViewResult> EditDriver(long id)
        {
            ViewData["Customers"] = await _driverRepository.Drivers().AsNoTracking().OrderBy(a => a.Fullname).ToListAsync();

            var item = await _driverRepository.Get(id);

            return PartialView("~/Views/Admin/Edit/Driver.cshtml", new Driver
            {
                Id = item.Id,
                AccountBankName = item.AccountBankName,
                BankAccountNumber = item.BankAccountNumber,
                Fullname = item.Fullname,
                Phonenumber = item.Phonenumber,
                NationalNumber = item.NationalNumber,
                IsActive = item.IsActive,
                EditDatetime = DateTime.Now,
                EditorId = _userManager.GetUserId(User)
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditDriver(Driver c)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(c.NationalNumber))
                    if (await _driverRepository.Drivers().AnyAsync(a => !a.Id.Equals(c.Id) && a.NationalNumber.Equals(c.NationalNumber)))
                    {
                        TempData["msg"] = "کد ملی وارد شده تکراری است. |danger";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }

                var item = await _driverRepository.Get(c.Id);
                item.Fullname = c.Fullname;
                item.Phonenumber = c.Phonenumber;
                item.NationalNumber = c.NationalNumber;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDatetime = DateTime.Now;
                item.IsActive = c.IsActive;
                item.BankAccountNumber = c.BankAccountNumber;
                item.AccountBankName = c.AccountBankName;
                _driverRepository.Update(item);
                try
                {
                    await _driverRepository.Save();
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
        public async Task<JsonResult> GetDriverListAsJson()
        {
            return Json(await _driverRepository.Drivers().AsNoTracking()
                .Select(a => new { a.Fullname, a.Id, a.NationalNumber })
                .OrderBy(a => a.Fullname).ToListAsync());
        }
        #endregion

        #region MehrcomParsCategory
        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> MehrcomParsCategory(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _mehrcomParsCategoryRepository.Categories().OrderBy(a => a.Sequence).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<PartialViewResult> CreateMehrcomParsCategory()
        {
            if (await _mehrcomParsCategoryRepository.Categories().AsNoTracking().AnyAsync())
                ViewData["Sequence"] = await _mehrcomParsCategoryRepository.Categories().AsNoTracking().MaxAsync(a => a.Sequence) + 1;
            else
                ViewData["Sequence"] = 1;

            return PartialView("~/Views/Admin/Create/MehrcomParsCategory.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateMehrcomParsCategory(MehrcomParsCategory v)
        {
            if (ModelState.IsValid)
            {
                if (await _mehrcomParsCategoryRepository.Categories().AnyAsync(a => a.Title.Equals(v.Title)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. عنوان در سیستم وجود دارد. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (await _mehrcomParsCategoryRepository.Categories().AnyAsync(a => a.Sequence.Equals(v.Sequence)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. ترتیب تکراری است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _mehrcomParsCategoryRepository.Create(v);
                try
                {
                    await _mehrcomParsCategoryRepository.Save();
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
        [Authorize(Roles = "Admin, Milad")]
        public async Task<PartialViewResult> EditMehrcomParsCategory(int id)
        {
            return PartialView("~/Views/Admin/Edit/MehrcomParsCategory.cshtml", await _mehrcomParsCategoryRepository.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditMehrcomParsCategory(MehrcomParsCategory v)
        {
            if (ModelState.IsValid)
            {
                if (await _mehrcomParsCategoryRepository.Categories().AnyAsync(a => !a.Id.Equals(v.Id) && a.Title.Equals(v.Title)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. نام در سیستم وجود دارد. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (await _mehrcomParsCategoryRepository.Categories().AnyAsync(a => !a.Id.Equals(v.Id) && a.Sequence.Equals(v.Sequence)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. ترتیب تکراری است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _mehrcomParsCategoryRepository.Get(v.Id);
                item.Title = v.Title;
                item.Sequence = v.Sequence;

                _mehrcomParsCategoryRepository.Update(item);
                try
                {
                    await _mehrcomParsCategoryRepository.Save();
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

        #region FreeLoadFactor
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FreeLoadFactor(int? p)
        {
            var pageNumber = p ?? 1;

            var onePageOfData = await _freeLoadFactorRepository.Query().OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchFreeLoadFactor(int? p, string param)
        {
            var pageNum = p ?? 1;
            var onePageOfData = await _freeLoadFactorRepository.Query().Where(a => a.LoadNumber.Contains(param) || a.LoadNumberGov.Contains(param)).OrderByDescending(a => a.Id).ToPagedListAsync(pageNum, 15);
            ViewBag.data = onePageOfData;
            ViewBag.param = param;

            return PartialView("_FreeLoadFactor");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<PartialViewResult> FreeLoadFactorDetail(long id)
        {
            var item = await _freeLoadFactorRepository.Get(id);
            ViewData["Admin"] = await _userManager.FindByIdAsync(item.CreatorId);
            return PartialView("_FreeLoadFactorDetail", item);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<PartialViewResult> CreateFreeLoadFactor()
        {
            ViewData["Year"] = await _configRepo.CurrentYear();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["VehicleTypes"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType.Equals(DefinitionType.Car)).OrderBy(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Create/FreeLoadFactor.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFreeLoadFactor(CreateFreeLoadFactorVM v, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                if (await _freeLoadFactorRepository.Query().AnyAsync(a => a.LoadNumber.Equals(v.LoadNumber)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. شماره بارنامه تکراری است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (!string.IsNullOrWhiteSpace(v.LoadNumberGov))
                    if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => a.LoadNumberGov.Equals(v.LoadNumberGov)))
                        return NotFound("شماره بارنامه دولتی درج شده تکراری است.");

                var config = await _configRepo.LoadFactorTax();
                var item = new FreeLoadFactor
                {
                    Amount = v.Amount,
                    ApplicantName = v.ApplicantName,
                    CalendarId = v.CalendarId,
                    CreateDatetime = DateTime.Now,
                    CreatorId = _userManager.GetUserId(User),
                    Destination = v.Destination,
                    DriverFee = v.DriverFee,
                    DriverName = v.DriverName,
                    DriverTonnagePrice = v.DriverTonnagePrice,
                    Origin = v.Origin,
                    LoadNumber = v.LoadNumber,
                    LoadNumberGov = v.LoadNumberGov,
                    Tonnage = v.Tonnage,
                    TonnagePrice = v.TonnagePrice,
                    IranStateNumber = v.IranStateNumber,
                    NumberWord = v.NumberWord,
                    RightNumber = v.RightNumber,
                    LeftNumber = v.LeftNumber,
                    DriverNationalNumber = v.DriverNationalNumber,
                    VehicleType = v.VehicleType,
                    WithholdingTax = config.WithholdingTax,
                    LoadFactorDeductions = 0,
                    VAT = config.VAT,
                    Date = new PersianDateTime(v.Year, v.Month, v.Day, 0, 0, 0).ToDateTime()
                };

                if (pic != null)
                {
                    if (pic.Length <= 10240000)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\freeloadfactor")))
                            {
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\freeloadfactor"));
                            }
                            var fileName = $"{v.ApplicantName.Replace(" ", "_")}_{v.LoadNumber}_{Path.GetRandomFileName()}{Path.GetExtension(pic.FileName).ToLower()}";
                            var path = Path.Combine(_environment.WebRootPath, "img\\freeloadfactor", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            item.LoadFactorScan = fileName;
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

                _freeLoadFactorRepository.Create(item);
                try
                {
                    await _freeLoadFactorRepository.Save();
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
        [Authorize(Roles = "Admin")]
        public async Task<PartialViewResult> EditFreeLoadFactor(long id)
        {
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["VehicleTypes"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType.Equals(DefinitionType.Car)).OrderBy(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Edit/FreeLoadFactor.cshtml", await _freeLoadFactorRepository.GetEditData(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditFreeLoadFactor(EditFreeLoadFactorVM v, IFormFile pic)
        {
            if (ModelState.IsValid)
            {
                if (await _freeLoadFactorRepository.Query().AnyAsync(a => !a.Id.Equals(v.Id) && a.LoadNumber.Equals(v.LoadNumber)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. شماره بارنامه تکراری است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (!string.IsNullOrWhiteSpace(v.LoadNumberGov))
                    if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => !a.Id.Equals(v.Id) && a.LoadNumberGov.Equals(v.LoadNumberGov)))
                        return NotFound("شماره بارنامه دولتی درج شده تکراری است.");

                var item = await _freeLoadFactorRepository.Get(v.Id);

                if (pic != null)
                {
                    if (pic.Length <= 10240000)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\freeloadfactor")))
                            {
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\freeloadfactor"));
                            }

                            var fileName = $"{v.ApplicantName.Replace(" ", "_")}_{v.LoadNumber}_{Path.GetRandomFileName()}{Path.GetExtension(pic.FileName).ToLower()}";
                            var path = Path.Combine(_environment.WebRootPath, "img\\freeloadfactor", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            if (!string.IsNullOrEmpty(item.LoadFactorScan))
                            {
                                try
                                {
                                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\freeloadfactor", item.LoadFactorScan));
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }

                            item.LoadFactorScan = fileName;
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
                else if (pic == null && (v.LoadNumber != item.LoadNumber || v.ApplicantName != item.ApplicantName))
                {
                    var fileInfo = new FileInfo(Path.Combine(_environment.WebRootPath, "img\\freeloadfactor", item.LoadFactorScan));
                    string ext = item.LoadFactorScan.Split('.', StringSplitOptions.RemoveEmptyEntries)[item.LoadFactorScan.Split('.', StringSplitOptions.RemoveEmptyEntries).Length - 1];
                    var fileName = $"{v.ApplicantName.Replace(" ", "_")}_{v.LoadNumber}_{Path.GetRandomFileName()}.{ext}";
                    if (fileInfo.Exists)
                        fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + fileName);

                    item.LoadFactorScan = fileName;
                }

                item.LoadNumber = v.LoadNumber;
                item.LoadNumberGov = v.LoadNumberGov;
                item.ApplicantName = v.ApplicantName;
                item.TonnagePrice = v.TonnagePrice;
                item.DriverTonnagePrice = v.DriverTonnagePrice;
                item.Amount = v.Amount;
                item.Date = new PersianDateTime(v.Year, v.Month, v.Day, 0, 0, 0).ToDateTime();
                item.Destination = v.Destination;
                item.Origin = v.Origin;
                item.EditDatetime = DateTime.Now;
                item.EditorId = _userManager.GetUserId(User);
                item.DriverNationalNumber = v.DriverNationalNumber;
                item.IranStateNumber = v.IranStateNumber;
                item.NumberWord = v.NumberWord;
                item.RightNumber = v.RightNumber;
                item.LeftNumber = v.LeftNumber;
                item.DriverNationalNumber = v.DriverNationalNumber;
                item.VehicleType = v.VehicleType;
                item.DriverFee = v.DriverFee;
                item.DriverName = v.DriverName;
                item.CalendarId = v.CalendarId;

                _freeLoadFactorRepository.Update(item);
                try
                {
                    await _freeLoadFactorRepository.Save();
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFreeLoadFactor(long id)
        {
            var item = await _freeLoadFactorRepository.Get(id);
            if (item == null) return NotFound();

            _freeLoadFactorRepository.Delete(item);
            try
            {
                await _freeLoadFactorRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region BankAccount
        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> BankAccount(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _bankAccountRepository.Query().Where(a => a.OwnerUserId.Equals(_userManager.GetUserId(User))).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public PartialViewResult CreateBankAccount()
        {
            ViewData["UserId"] = _userManager.GetUserId(User);
            return PartialView("~/Views/Admin/Create/BankAccount.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> CreateBankAccount(BankAccount v)
        {
            if (ModelState.IsValid)
            {
                _bankAccountRepository.Create(v);
                try
                {
                    await _bankAccountRepository.Save();
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
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<PartialViewResult> EditBankAccount(int id)
        {
            return PartialView("~/Views/Admin/Edit/BankAccount.cshtml", await _bankAccountRepository.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> EditBankAccount(BankAccount v)
        {
            if (ModelState.IsValid)
            {
                var item = await _bankAccountRepository.Get(v.Id);
                item.AccountBankName = v.AccountBankName;
                item.BankAccountNumber = v.BankAccountNumber;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDatetime = DateTime.Now;

                _bankAccountRepository.Update(item);
                try
                {
                    await _bankAccountRepository.Save();
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

        #region BankAccountBook
        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> BankAccountBook(string id, int? p)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Redirect(Request.Headers["Referer"].ToString());

            var bankAccountId = await _bankAccountRepository.Query().AsNoTracking()
                .Where(a => a.RowId.Equals(id)).Select(a => a.Id).FirstOrDefaultAsync();

            ViewData["BankAccountId"] = bankAccountId;
            ViewData["BankAccountRowId"] = id;
            var pageNumber = p ?? 1;
            var onePageOfData = await _bankAccountBookRepository.Query().Where(a => a.BankAccountId.Equals(bankAccountId)).OrderByDescending(a => a.Sequence).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> BankAccountBook_Search(long bankAccountId, string number, string description, string fromDate, string toDate)
        {
            var query = _bankAccountBookRepository.Query().Where(a => a.BankAccountId.Equals(bankAccountId));

            if (!string.IsNullOrWhiteSpace(number))
                query = query.Where(a => a.ReferenceNo.Equals(number));
            else if (!string.IsNullOrWhiteSpace(description))
                query = query.Where(a => a.Description.Contains(description));
            else if (!string.IsNullOrWhiteSpace(fromDate))
            {
                var dateArr = fromDate.PersianToEnglish().Split('/');
                var fDate = new PersianDateTime(Convert.ToInt32(dateArr[0]), Convert.ToInt32(dateArr[1]), Convert.ToInt32(dateArr[2]));
                query = query.Where(a => a.Date >= fDate);
            }
            else if (!string.IsNullOrWhiteSpace(toDate))
            {
                var dateArr = toDate.PersianToEnglish().Split('/');
                var tDate = new PersianDateTime(Convert.ToInt32(dateArr[0]), Convert.ToInt32(dateArr[1]), Convert.ToInt32(dateArr[2]));
                query = query.Where(a => a.Date <= tDate);
            }

            return PartialView(await query.AsNoTracking().OrderBy(a => a.Sequence).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public IActionResult CreateBankAccountBook()
        {
            return PartialView("~/Views/Admin/Create/BankAccountBook.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> CreateBankAccountBook(CreateBankAccountBookVM v)
        {
            if (ModelState.IsValid)
            {
                var lastItem = await _bankAccountBookRepository.Query().AsNoTracking().Where(a => a.BankAccountId.Equals(v.BankAccountId))
                    .OrderByDescending(a => a.Id).FirstOrDefaultAsync();

                var balance = lastItem is null ? 0 : lastItem.Balance;

                _bankAccountBookRepository.Create(new Domain.Models.BankAccountBook
                {
                    Sequence = lastItem is null ? 1 : lastItem.Sequence + 1,
                    Date = new PersianDateTime(v.Year, v.Month, v.Day).ToDateTime(),
                    BankAccountId = v.BankAccountId,
                    CreatorId = _userManager.GetUserId(User),
                    ReferenceNo = v.ReferenceNo,
                    Description = v.Description,
                    TransferFee = v.TransferFee,
                    AccountBookType = v.AccountBookType,
                    Creditor = v.AmountType == BankAccountBookAmountType.Creditor ? v.Amount : 0,
                    Debtor = v.AmountType == BankAccountBookAmountType.Debtor ? v.Amount : 0,
                    Balance = (v.AmountType == BankAccountBookAmountType.Debtor ? v.Amount + balance : balance - v.Amount) - v.TransferFee
                });
                try
                {
                    await _bankAccountBookRepository.Save();
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
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<PartialViewResult> EditBankAccountBook(int id)
        {
            return PartialView("~/Views/Admin/Edit/BankAccountBook.cshtml", await _bankAccountBookRepository.GetEdit(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> EditBankAccountBook(EditBankAccountBookVM v)
        {
            if (ModelState.IsValid)
            {
                var item = await _bankAccountBookRepository.Get(v.Id);
                item.ReferenceNo = v.ReferenceNo;
                item.Description = v.Description;
                item.Date = new PersianDateTime(v.Year, v.Month, v.Day).ToDateTime();
                item.EditorId = _userManager.GetUserId(User);
                item.EditDatetime = DateTime.Now;
                item.AccountBookType = v.AccountBookType;

                var diffrence = item.Debtor > 0 ? (item.Debtor - v.Amount) : (item.Creditor - v.Amount);
                var transferFeeDeffrence = item.TransferFee - v.TransferFee;

                if (transferFeeDeffrence != 0 || diffrence != 0)
                {
                    var nextItems = await _bankAccountBookRepository.Query().Where(a => a.Id > item.Id && a.BankAccountId.Equals(item.BankAccountId)).OrderBy(a => a.Id).ToListAsync();
                    foreach (var next in nextItems)
                    {
                        next.Balance += diffrence;
                        if (transferFeeDeffrence != 0)
                            next.Balance += transferFeeDeffrence;
                    }

                    if (transferFeeDeffrence != 0)
                    {
                        item.TransferFee = v.TransferFee;
                        item.Balance += transferFeeDeffrence;
                    }

                    if (item.Debtor > 0)
                    {
                        item.Balance += diffrence;
                        item.Debtor = v.Amount;
                    }
                    else
                    {
                        item.Balance += diffrence;
                        item.Creditor = v.Amount;
                    }
                }

                _bankAccountBookRepository.Update(item);
                try
                {
                    await _bankAccountBookRepository.Save();
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
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> DeleteBankAccountBook(int id)
        {
            var item = await _bankAccountBookRepository.Get(id);
            var nextItems = await _bankAccountBookRepository.Query().Where(a => a.Id > item.Id && a.BankAccountId.Equals(item.BankAccountId)).OrderBy(a => a.Id).ToListAsync();
            for (int i = 0; i < nextItems.Count; i++)
            {
                var next = nextItems[i];

                next.Sequence = item.Sequence + i;
                if (item.Creditor > 0)
                    next.Balance -= item.Creditor;
                else if (item.Debtor > 0)
                    next.Balance += item.Debtor;
            }

            _bankAccountBookRepository.Delete(item);
            try
            {
                await _bankAccountBookRepository.Save();
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        //public async Task<IActionResult> MoveBankAccountBook()
        #endregion

        #region Turnover
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Turnover(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _bankAccountRepository.Query().Where(a => a.OwnerUserId.Equals(_userManager.GetUserId(User))).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public PartialViewResult CreateTurnover()
        {
            ViewData["UserId"] = _userManager.GetUserId(User);
            return PartialView("~/Views/Admin/Create/Turnover.cshtml");
        }

        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> CreateTurnover(Turnover v)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var item = new Turnover
        //        {

        //        };
        //        new PersianDateTime(v.Year, v.Month, v.Day).ToDateTime(),
        //        _turnoverRepository.Create(v);
        //        try
        //        {
        //            await _turnoverRepository.Save();
        //            TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
        //        }
        //        catch (Exception e)
        //        {
        //            TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
        //        }
        //    }
        //    else
        //    {
        //        TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |danger";
        //    }
        //    return Redirect(Request.Headers["Referer"].ToString());
        //}

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<PartialViewResult> EditTurnover(int id)
        {
            return PartialView("~/Views/Admin/Edit/Turnover.cshtml", await _turnoverRepository.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTurnover(Turnover v)
        {
            if (ModelState.IsValid)
            {
                var item = await _turnoverRepository.Get(v.Id);
                item.TurnoverType = v.TurnoverType;
                item.CalnedarId = v.CalnedarId;
                item.Creditor = v.Creditor;
                item.Date = v.Date;
                item.Debtor = v.Debtor;
                item.Description = v.Description;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDatetime = DateTime.Now;

                _turnoverRepository.Update(item);
                try
                {
                    await _turnoverRepository.Save();
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
