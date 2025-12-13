using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
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
using System.Text;
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
        private readonly IBillRepository _billRepository;
        private readonly IVehicleBalanceRepository _vehicleBalanceRepository;
        private readonly ICustomerFactorRepository _customerFactorRepository;
        private readonly ITurnoverProfileRepository _turnoverProfileRepository;
        private readonly ILoadFactorNovinRepository _loadFactorNovinRepository;
        private readonly IVehicleBankAccountRepository _vehicleBankAccountRepository;
        private readonly ITurnoverProfilePeriodRepository _turnoverProfilePeriodRepository;
        private readonly IShippingFeeGroupRepository _shippingFeeGroupRepository;
        private readonly IShippingFeeRouteRepository _shippingFeeRouteRepository;
        private readonly IUserPlannerRepository _userPlannerRepository;
        private readonly IUserPlannerItemRepository _userPlannerItemRepository;
        private readonly ApplicationDbContext _context;

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
            ITurnoverRepository turnoverRepository,
            IBillRepository billRepository,
            IVehicleBalanceRepository vehicleBalanceRepository,
            ICustomerFactorRepository customerFactorRepository,
            ITurnoverProfileRepository turnoverProfileRepository,
            ILoadFactorNovinRepository loadFactorNovinRepository,
            IVehicleBankAccountRepository vehicleBankAccountRepository,
            ApplicationDbContext context,
            ITurnoverProfilePeriodRepository turnoverProfilePeriodRepository,
            IShippingFeeGroupRepository shippingFeeGroupRepository,
            IShippingFeeRouteRepository shippingFeeRouteRepository,
            IUserPlannerRepository userPlannerRepository,
            IUserPlannerItemRepository userPlannerItemRepository)
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
            _billRepository = billRepository;
            _vehicleBalanceRepository = vehicleBalanceRepository;
            _customerFactorRepository = customerFactorRepository;
            _turnoverProfileRepository = turnoverProfileRepository;
            _loadFactorNovinRepository = loadFactorNovinRepository;
            _vehicleBankAccountRepository = vehicleBankAccountRepository;
            _context = context;
            _turnoverProfilePeriodRepository = turnoverProfilePeriodRepository;
            _shippingFeeGroupRepository = shippingFeeGroupRepository;
            _shippingFeeRouteRepository = shippingFeeRouteRepository;
            _userPlannerRepository = userPlannerRepository;
            _userPlannerItemRepository = userPlannerItemRepository;
        }

        private long CalcNextSequenceForLoadFactor(long sequence)
        {
            double x = sequence / 5;
            sequence = Convert.ToInt64((Math.Floor(x) + 1) * 5);
            return sequence;
        }

        public async Task<IActionResult> Index(int? dayLimit)
        {
            //var loadfactors = await _context.LoadFactor.Where(a => a.ShippingFeeId.HasValue).AsParallel().ToListAsync();
            //var shippingFees = await _context.ShippingFee.AsNoTracking().AsParallel().ToListAsync();

            //foreach (var item in loadfactors)
            //{
            //    item.VehicleType = shippingFees.Single(a => a.Id.Equals(item.ShippingFeeId)).Vehicle;
            //}

            //_context.LoadFactor.UpdateRange(loadfactors);
            //await _context.SaveChangesAsync();


            //#region edit fee by price
            //var date = new DateTime(2023, 05, 22);
            //var contractId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.RowId == "62153ffa-328f-48f0-aaed-dc497d058402")
            //    .Select(a => a.Id).FirstOrDefaultAsync();

            //var shippingFees = await _shippingFeeRepo.ShippingFees().Where(a => a.DriverPrice.Equals(28500000) 
            //    && a.ContractId.Equals(contractId)).ToListAsync();

            //var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => shippingFees.Select(b => b.Id).Contains(a.ShippingFeeId) && a.Date >= date && !a.IsFreeDriverPrice).ToListAsync();
            //var vehicleBalances = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && loadFactors.Select(b => b.Id).Contains(a.LoadFactorId.Value)).ToListAsync();

            //foreach (var item in shippingFees)
            //{
            //    item.DriverPrice = 37000000;
            //}

            //foreach (var item in loadFactors)
            //{
            //    item.DriverFee = 37000000;

            //    var vb = vehicleBalances.Single(a => a.LoadFactorId.Value.Equals(item.Id));
            //    vb.Amount = item.DriverFee +
            //            ((item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue) ? item.Tonnage.Value * item.DriverTonnagePrice.Value : 0) +
            //            (item.WeighbridgePrice.HasValue ? item.WeighbridgePrice.Value : 0) +
            //            (item.DriverLoadSleepPrice.HasValue ? item.DriverLoadSleepPrice.Value : 0);
            //    vb.EditDatetime = DateTime.Now;
            //}

            //await _shippingFeeRepo.Save();
            //await _loadFactorRepo.Save();
            //await _vehicleBalanceRepository.Save();
            //#endregion

            //عملکرد
            //var x = await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(4) && a.SaipaPressLoadFactor != null).SumAsync(a => a.Amount + ((a.Tonnage.HasValue && a.TonnagePrice.HasValue) ? a.Tonnage.Value * a.TonnagePrice.Value : 0));
            //var y = await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(4) && a.SaipaPressLoadFactor != null).SumAsync(a => a.DriverFee + ((a.Tonnage.HasValue && a.DriverTonnagePrice.HasValue) ? a.Tonnage.Value * a.DriverTonnagePrice.Value : 0));

            //var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.ContractId == 3).ToListAsync();
            //var fees = await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId == 3).ToListAsync();
            //foreach (var item in loadFactors)
            //{
            //    item.Amount = fees.Where(a => a.Id.Equals(item.ShippingFeeId)).Select(a => a.Price).First();
            //}

            //await _loadFactorRepo.Save();

            //var data = await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(2) && a.ShippingFeeLoadTypeId == 5).ToListAsync();

            //foreach (var item in data)
            //{
            //    var feeToCreate = new Domain.Models.ShippingFee
            //    {
            //        ContractId = item.ContractId,
            //        CreateDate = item.CreateDate,
            //        DestinationId = item.DestinationId,
            //        DriverPrice = item.Vehicle == "خاور" ? item.DriverPrice - 350000 : item.DriverPrice - 1000000,
            //        OriginId = item.OriginId,
            //        DriverTonnagePrice = item.DriverTonnagePrice,
            //        Price = item.Price,
            //        ShippingFeeLoadTypeId = item.ShippingFeeLoadTypeId,
            //        ShippingFeeType = item.ShippingFeeType,
            //        TonnagePrice = item.TonnagePrice,
            //        Title = "دو طبقه",
            //        CreatorId = item.CreatorId,
            //        Vehicle = item.Vehicle
            //    };
            //    _shippingFeeRepo.Create(feeToCreate);

            //    item.Title = "یک طبقه";

            //    await _shippingFeeRepo.Save();

            //    var loadFactors2 = await _loadFactorRepo.LoadFactors().Where(a => a.ShippingFeeId.Equals(item.Id) && a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.TwoFloor && !a.IsDriverFeeEditedByAdmin).ToListAsync();
            //    foreach (var lf in loadFactors2)
            //    {
            //        lf.ShippingFeeId = feeToCreate.Id;
            //        lf.DriverFee = feeToCreate.DriverPrice;
            //    }

            //    await _loadFactorRepo.Save();
            //}


            //var bills = await _billRepository.Query().Where(a => a.VehicleId.HasValue).ToListAsync();
            //foreach (var item in bills)
            //{
            //    await _vehicleBalanceRepository.Create(new VehicleBalance
            //    {
            //        Amount = -item.Amount,
            //        BillId = item.Id,
            //        CalendarId = item.CalendarId,
            //        VehicleId = item.VehicleId.Value,
            //        CreateDateTime = item.Date
            //    });
            //}


            //var loadFactors = await _loadFactorRepo.LoadFactors().ToListAsync();
            //foreach (var item in loadFactors)
            //{
            //    await _vehicleBalanceRepository.Create(new VehicleBalance
            //    {
            //        Amount = item.DriverFee +
            //        ((item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue) ? item.Tonnage.Value * item.DriverTonnagePrice.Value : 0) +
            //        (item.WeighbridgePrice.HasValue ? item.WeighbridgePrice.Value : 0) +
            //        (item.DriverLoadSleepPrice.HasValue ? item.DriverLoadSleepPrice.Value : 0),
            //        LoadFactorId = item.Id,
            //        CustomerId = item.Contract.CustomerId,
            //        CalendarId = item.CalendarId,
            //        VehicleId = item.VehicleId,
            //        CreateDateTime = item.Date
            //    });
            //}

            //await _vehicleBalanceRepository.Save();


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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCostModalLists()
        {
            var customers = await _customerRepo.GetAll();
            var dates = new List<CustomerCostDateVM>();

            var currentPersianDate = new PersianDateTime(DateTime.Now);
            var currentPersianYear = currentPersianDate.Year;
            var lastDayOfYear = 29;
            if (currentPersianDate.IsLeapYear)
                lastDayOfYear = 30;

            dates.Add(new CustomerCostDateVM
            {
                StartDate = $"{currentPersianYear}/01/01",
                EndDate = $"{currentPersianYear}/12/{lastDayOfYear}",
                Title = currentPersianYear.ToString()
            });

            currentPersianYear -= 1;
            currentPersianDate = currentPersianDate.AddYears(-1);

            while (currentPersianYear >= 1403)
            {
                lastDayOfYear = 29;
                if (currentPersianDate.IsLeapYear)
                    lastDayOfYear = 30;
                dates.Add(new CustomerCostDateVM
                {
                    StartDate = $"{currentPersianYear}/01/01",
                    EndDate = $"{currentPersianYear}/12/{lastDayOfYear}",
                    Title = currentPersianYear.ToString()
                });
                currentPersianYear--;
            }

            return Json(new { dates, customers });
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

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> GetLoadFactorCompany()
        {
            return Json(await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType == DefinitionType.LoadFactorOrigin).Select(a => new { a.Id, a.Title }).ToListAsync());
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
        public async Task<IActionResult> Vehicle(int? p, bool type = true)
        {
            ViewData["Type"] = type;
            var query = _vehicleRepo.Vehicles().Where(a => a.RealStatus == type);

            var pageNumber = p ?? 1;
            var onePageOfData = await query.OrderBy(a => a.LeftNumber).ToPagedListAsync(pageNumber, type ? 15 : await query.CountAsync());
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
                if (await _vehicleRepo.Vehicles().AnyAsync(a => a.Type.Equals(v.Type) && a.IranStateNumber.Equals(v.IranStateNumber) && a.RightNumber.Equals(v.RightNumber) && a.NumberWord.Equals(v.NumberWord) && a.LeftNumber.Equals(v.LeftNumber)))
                {
                    TempData["msg"] = "شماره خودرو وارد شده قبلا ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                v.CreatorId = _userManager.GetUserId(User);
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
                if (await _vehicleRepo.Vehicles().AnyAsync(a => !a.Id.Equals(v.Id) && a.Type.Equals(v.Type) && a.IranStateNumber.Equals(v.IranStateNumber) && a.RightNumber.Equals(v.RightNumber) && a.NumberWord.Equals(v.NumberWord) && a.LeftNumber.Equals(v.LeftNumber)))
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
                item.VehicleOwnerFullname = v.VehicleOwnerFullname;
                item.RealStatus = v.RealStatus;
                item.NationalNumber = v.NationalNumber;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDatetime = DateTime.Now;
                item.Phonenumber = v.Phonenumber;
                item.VehicleCardNo = v.VehicleCardNo;
                item.DriverCardNo = v.DriverCardNo;

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

        #region VehicleBankAccount

        [HttpGet]
        public async Task<IActionResult> VehicleBankAccount(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            ViewData["Vehicle"] = await _vehicleRepo.Get(id.Value);
            return View(await _vehicleBankAccountRepository.Query().Include(a => a.Definition).AsNoTracking().Where(a => a.VehicleId.Equals(id.Value)).OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        public async Task<PartialViewResult> CreateVehicleBankAccount(long vehicleId)
        {
            ViewData["VehicleId"] = vehicleId;
            ViewData["Definition"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType.Equals(DefinitionType.BankName)).OrderBy(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Create/VehicleBankAccount.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicleBankAccount(VehicleBankAccount v)
        {
            if (ModelState.IsValid)
            {

                if (await _vehicleBankAccountRepository.Query().AnyAsync(a => a.VehicleId.Equals(v.VehicleId) && a.BankId.Equals(v.BankId)))
                {
                    TempData["msg"] = "برای بانک انتخابی، قبلا یک حساب درج شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (await _vehicleBankAccountRepository.Query().AnyAsync(a => a.VehicleId.Equals(v.VehicleId) && a.AccountNumber.Equals(v.AccountNumber)))
                {
                    TempData["msg"] = "شماره حساب وارد شده قبلا ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                await _vehicleBankAccountRepository.Create(v);
                try
                {
                    await _vehicleBankAccountRepository.Save();
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
        public async Task<PartialViewResult> EditVehicleBankAccount(int id)
        {
            ViewData["Definition"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType.Equals(DefinitionType.BankName)).OrderBy(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Edit/VehicleBankAccount.cshtml", await _vehicleBankAccountRepository.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditVehicleBankAccount(VehicleBankAccount v)
        {
            if (ModelState.IsValid)
            {

                if (await _vehicleBankAccountRepository.Query().AnyAsync(a => !a.Id.Equals(v.Id) && a.VehicleId.Equals(v.VehicleId) && a.BankId.Equals(v.BankId)))
                {
                    TempData["msg"] = "برای بانک انتخابی، قبلا یک حساب درج شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                if (await _vehicleBankAccountRepository.Query().AnyAsync(a => !a.Id.Equals(v.Id) && a.VehicleId.Equals(v.VehicleId) && a.AccountNumber.Equals(v.AccountNumber)))
                {
                    TempData["msg"] = "شماره حساب وارد شده قبلا ثبت شده است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _vehicleBankAccountRepository.Get(v.Id);
                item.AccountNumber = v.AccountNumber;
                item.BankId = v.BankId;
                item.Fullname = v.Fullname;

                _vehicleBankAccountRepository.Update(item);
                try
                {
                    await _vehicleBankAccountRepository.Save();
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
            var onePageOfData = await _calendarRepo.Calendars().OrderByDescending(a => a.StartDate).ToPagedListAsync(pageNumber, 15);
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
                    Sequence = await _calendarRepo.Calendars().AsNoTracking().MaxAsync(a => a.Sequence) + 1,
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

            if (item.Costs.Any() || item.CustomerIncomes.Any() || item.LoadFactors.Any() || item.Bills.Any())
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
        [Authorize(Roles = "Admin, User, Milad, Accountant")]
        public async Task<IActionResult> Cost(int? p)
        {
            ViewData["UserId"] = _userManager.GetUserId(User);
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["CostAccount"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType == DefinitionType.CostAccount).ToListAsync();

            var query = _costRepo.Costs().Include(a => a.Calendar).Include(a => a.ApplicationUser).Include(a => a.Definition).AsQueryable();
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.UserId.Equals(_userManager.GetUserId(User)));

            var pageNumber = p ?? 1;
            var onePageOfData = await query.OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad, Accountant")]
        public async Task<IActionResult> Cost_Search(int? p)
        {
            var query = _costRepo.Costs().Include(a => a.Calendar).Include(a => a.ApplicationUser).Include(a => a.Definition).AsQueryable();
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.UserId.Equals(_userManager.GetUserId(User)));

            var pageNumber = p ?? 1;
            var onePageOfData = await query.OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return PartialView();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad, Accountant")]
        public async Task<IActionResult> Cost(Cost c, int day, int month, int year)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                string fileNames = "";
                var files = Request.Form.Files;
                if (files != null)
                {
                    foreach (var pic in files)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\cost")))
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\cost"));

                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\cost", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            fileNames += (files.Count == 1 || pic == files.Last()) ? fileName : $"{fileName};;";
                        }
                        else
                        {
                            msg = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                            return Json(new { msg, status });
                        }
                    }
                }

                c.Picture = fileNames;
                c.Date = new PersianDateTime(year, month, day).ToDateTime();
                _costRepo.Create(c);
                try
                {
                    await _costRepo.Save();
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
        [Authorize(Roles = "Admin, User, Milad, Accountant")]
        public async Task<PartialViewResult> EditCost(int id)
        {
            ViewData["Calendar"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["CostAccount"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType == DefinitionType.CostAccount).ToListAsync();
            return PartialView("~/Views/Admin/Edit/Cost.cshtml", await _costRepo.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad, Accountant")]
        public async Task<IActionResult> EditCost(Cost c, int day, int month, int year)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                var item = await _costRepo.Get(c.Id);
                item.Description = c.Description;
                item.Amount = c.Amount;
                item.CalendarId = c.CalendarId;
                item.CostAccountId = c.CostAccountId;

                item.Date = new PersianDateTime(year, month, day).ToDateTime();

                string fileNames = "";
                var files = Request.Form.Files;
                if (files != null && files.Count > 0)
                {
                    foreach (var pic in files)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\cost", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            fileNames += (files.Count == 1 || pic == files.Last()) ? fileName : $"{fileName};;";
                        }
                        else
                        {
                            msg = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                            return Json(new { msg, status });
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Picture))
                    {
                        foreach (var pic in item.Picture.Split(";;", StringSplitOptions.RemoveEmptyEntries))
                        {
                            try
                            {
                                System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\cost", pic));
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }

                    item.Picture = fileNames;
                }

                _costRepo.Update(item);
                try
                {
                    await _costRepo.Save();
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

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad, Accountant")]
        public async Task<IActionResult> DeleteCost(int id)
        {
            var item = await _costRepo.Get(id);
            if (!string.IsNullOrEmpty(item.Picture))
            {
                foreach (var pic in item.Picture.Split(";;", StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\cost", pic));
                    }
                    catch (Exception)
                    {
                        throw;
                    }
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

        #region Customer
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Customer()
        {
            return View(await _customerRepo.Customers().AsNoTracking().Include(a => a.Definition).OrderBy(a => a.Name).ToListAsync());
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
            ViewData["Banks"] = await _definitionRepo.Definitions().Where(a => a.DefinitionType.Equals(DefinitionType.BankName)).OrderBy(a => a.Title).ToListAsync();
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
                item.ActiveBank = c.ActiveBank;

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
            ViewData["Contracts"] = await _contractRepo.Contracts().Where(a => a.CustomerId.Equals(id) && !a.ParentContractId.HasValue).OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["BankBranches"] = await _definitionRepo.Definitions().Where(a => a.DefinitionType == DefinitionType.BankBranch).AsNoTracking().OrderByDescending(a => a.Title).ToListAsync();
            var pageNumber = p ?? 1;
            var onePageOfData = await _customerRepo.CustomerIncomes().Where(a => a.CustomerId.Equals(id)).OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerIncome([Bind("BankName,Amount,Description,CustomerId")] CustomerIncome c, int day, int month, int year, IFormFile pic)
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
            var item = await _customerRepo.GetIncome(id);
            ViewData["Contracts"] = await _contractRepo.Contracts().Where(a => a.CustomerId.Equals(item.CustomerId) && !a.ParentContractId.HasValue).OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["BankBranches"] = await _definitionRepo.Definitions().Where(a => a.DefinitionType == DefinitionType.BankBranch).AsNoTracking().OrderByDescending(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Edit/CustomerIncome.cshtml", item);
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
                item.BankName = p.BankName;
                item.Description = p.Description;
                //item.ContractId = p.ContractId;

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

        [HttpPost]
        public async Task<IActionResult> GetCustomersJson()
        {
            return Json(await _customerRepo.Customers().AsNoTracking().OrderByDescending(a => a.Name).Select(a => new { a.Id, a.Name }).ToListAsync());
        }
        #endregion

        #region Contract
        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> Contract(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _contractRepo.Contracts().Include(a => a.ContractAddons).Include(a => a.Customer)
                .Where(a => !a.ParentContractId.HasValue).OrderByDescending(a => a.StartDate).ToPagedListAsync(pageNumber, 15);
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

                var contract = new EtehadBar.Domain.Models.Contract
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

                    return RedirectToAction("ShippingFeeGroup", new { contractId = contract.RowId });
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

        [HttpPost]
        public async Task<IActionResult> GetContractsJson()
        {
            return Json(await _contractRepo.Contracts().AsNoTracking().Where(a => !a.ParentContractId.HasValue).Select(a => new
            {
                a.Number,
                a.Id,
                a.CustomerId
            }).ToListAsync());
        }
        #endregion

        #region ShippingFeeGroup
        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> ShippingFeeGroup(string contractId)
        {
            if (string.IsNullOrWhiteSpace(contractId)) return BadRequest();

            var contract = await _contractRepo.Contracts().Where(a => a.RowId.Equals(contractId)).Include(a => a.ContractAddons).Include(a => a.Customer).FirstOrDefaultAsync();
            if (contract == null) return NotFound();

            ViewData["Contract"] = contract;
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return View(await _shippingFeeGroupRepository.Query().Include(a => a.ShippingFeeLoadType).Where(a => a.ContractId.Equals(contract.Id))
                .OrderBy(a => a.Vehicle).ThenByDescending(a => a.Price).ThenBy(a => a.Origin).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> ShippingFeeGroupPartial(string contractId)
        {
            var contract = await _contractRepo.Contracts().Include(a => a.Customer).AsNoTracking().FirstOrDefaultAsync(a => a.RowId.Equals(contractId));
            if (contract == null) return NotFound();

            ViewData["Contract"] = contract;
            return PartialView("_ShippingFeeGroup", await _shippingFeeGroupRepository.Query().Include(a => a.ShippingFeeLoadType).Where(a => a.ContractId.Equals(contract.Id))
                .OrderBy(a => a.Vehicle).ThenByDescending(a => a.Price).ThenBy(a => a.Origin).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> ShippingFeeGroup_Search()
        {
            var vehicleTypes = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType == DefinitionType.Car)
                .Select(a => new
                {
                    a.Title
                }).ToListAsync();

            var loadRoutes = await _loadRouteRepo.LoadRoutes().AsNoTracking().Where(a => a.RealStatus).OrderBy(a => a.Title).ToListAsync();
            return Json(vehicleTypes);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> ShippingFeeGroup_Search(int? p, long contractId, string vehicleType, string title, double? amount, double? driverFee, string origin, string destination)
        {
            var pageNumber = p ?? 1;
            ViewData["Contract"] = await _contractRepo.Contracts().Include(a => a.Customer).FirstOrDefaultAsync(a => a.Id.Equals(contractId));

            var query = _shippingFeeGroupRepository.Query().Include(a => a.ShippingFeeLoadType).Where(a => a.ContractId.Equals(contractId));

            if (vehicleType != "all")
                query = query.Where(a => a.Vehicle.Equals(vehicleType));
            if (!string.IsNullOrWhiteSpace(title) && title != null)
                query = query.Where(a => a.Title.Contains(title));
            if (driverFee.HasValue && driverFee.Value >= 0)
                query = query.Where(a => a.DriverPrice.Equals(driverFee.Value));
            if (amount.HasValue && amount.Value >= 0)
                query = query.Where(a => a.Price.Equals(amount.Value));
            if (!string.IsNullOrWhiteSpace(origin))
                query = query.Where(a => a.Origin.Contains(origin));
            if (!string.IsNullOrWhiteSpace(destination))
                query = query.Where(a => a.Origin.Contains(destination));


            return PartialView(await query.OrderBy(a => a.Vehicle).ThenByDescending(a => a.Price).ThenBy(a => a.Origin).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateShippingFeeGroup(string contractId)
        {
            ViewData["Contract"] = await _contractRepo.Contracts().Where(a => a.RowId.Equals(contractId)).Include(a => a.Customer).FirstOrDefaultAsync();
            List<DefinitionType> types = new()
            {
                DefinitionType.Car
            };
            ViewData["Data"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => types.Contains(a.DefinitionType)).ToListAsync();
            ViewData["LoadTypes"] = await _shippingFeeLoadTypeRepo.ShippingFeeLoadTypes().AsNoTracking().ToListAsync();

            return PartialView("~/Views/Admin/Create/ShippingFeeGroup.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateShippingFeeGroup(ShippingFeeGroup s)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                if (await _shippingFeeGroupRepository.Query().AsNoTracking()
                    .AnyAsync(a => a.ContractId.Equals(s.ContractId) && a.ShippingFeeLoadTypeId.Equals(s.ShippingFeeLoadTypeId) && a.Vehicle.Equals(s.Vehicle) && a.Origin.Equals(s.Origin) && a.Destination.Equals(s.Destination) && a.DriverPrice.Equals(s.DriverPrice) && a.Title.Equals(s.Title)))
                    return Json(new { msg = "نرخ حمل و نقل ثبت شده تکراری است.", status });

                s.CreatorId = _userManager.GetUserId(User);
                _shippingFeeGroupRepository.Create(s);

                try
                {
                    await _shippingFeeGroupRepository.Save();
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
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditShippingFeeGroup(int id)
        {
            List<DefinitionType> types = new()
            {
                DefinitionType.Car
            };
            ViewData["Data"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => types.Contains(a.DefinitionType)).ToListAsync();
            ViewData["LoadTypes"] = await _shippingFeeLoadTypeRepo.ShippingFeeLoadTypes().AsNoTracking().ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.Sequence).ToListAsync();

            return PartialView("~/Views/Admin/Edit/ShippingFeeGroup.cshtml", await _shippingFeeGroupRepository.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditShippingFeeGroup(ShippingFeeGroup s, string DateLimit, long CalendarLimit)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                if (await _shippingFeeGroupRepository.Query().AsNoTracking()
                    .AnyAsync(a => !a.Id.Equals(s.Id) && a.ContractId.Equals(s.ContractId) && a.ShippingFeeLoadTypeId.Equals(s.ShippingFeeLoadTypeId) && a.Vehicle.Equals(s.Vehicle) && a.Origin.Equals(s.Origin) && a.Destination.Equals(s.Destination) && a.DriverPrice.Equals(s.DriverPrice) && a.Title.Equals(s.Title)))
                    return Json(new { msg = "نرخ حمل و نقل ثبت شده تکراری است.", status });

                var item = await _shippingFeeGroupRepository.Get(s.Id);


                bool isDriverFeeChanged = false;

                if (item.DriverPrice != s.DriverPrice || item.DriverTonnagePrice != s.DriverTonnagePrice)
                    isDriverFeeChanged = true;

                item.DriverPrice = s.DriverPrice;
                item.Price = s.Price;
                item.Vehicle = s.Vehicle;
                item.TonnagePrice = s.TonnagePrice;
                item.DriverTonnagePrice = s.DriverTonnagePrice;
                item.ShippingFeeLoadTypeId = s.ShippingFeeLoadTypeId;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDate = DateTime.Now;
                item.Title = s.Title;
                item.Origin = s.Origin;
                item.Destination = s.Destination;

                _shippingFeeGroupRepository.Update(item);

                try
                {
                    await _shippingFeeGroupRepository.Save();

                    var latestContractAddon = await _contractRepo.Contracts().AsNoTracking().Where(a => a.ParentContractId.Equals(item.ContractId)).OrderByDescending(a => a.StartDate).FirstOrDefaultAsync();
                    if (latestContractAddon == null)
                        latestContractAddon = await _contractRepo.Get(item.ContractId);

                    var shippingFeeRoutes = await _shippingFeeRouteRepository.Query().Where(a => a.ShippingFeeGroupId.Equals(item.Id)).ToListAsync();
                    var loadFactorQuery = _loadFactorRepo.LoadFactors().Where(a => a.ContractId.Equals(item.ContractId) && shippingFeeRoutes.Select(a => a.Id).Contains(a.ShippingFeeRouteId.Value) && a.Date >= latestContractAddon.StartDate && !a.IsDriverFeeEditedByAdmin);
                    if (!string.IsNullOrWhiteSpace(DateLimit))
                    {
                        DateLimit = DateLimit.PersianToEnglish();
                        var dateLimitArr = DateLimit.Split("/");
                        var date = new PersianDateTime(Convert.ToInt32(dateLimitArr[0]), Convert.ToInt32(dateLimitArr[1]), Convert.ToInt32(dateLimitArr[2])).ToDateTime();
                        loadFactorQuery = loadFactorQuery.Where(a => a.Date >= date);
                    }

                    if (CalendarLimit > 0)
                    {
                        var calendars = await _calendarRepo.Calendars().Where(a => a.Sequence >= _calendarRepo.Calendars().Single(a => a.Id.Equals(CalendarLimit)).Sequence).Select(a => a.Id).ToListAsync();
                        loadFactorQuery = loadFactorQuery.Where(a => calendars.Contains(a.CalendarId));
                    }

                    var loadFactors = await loadFactorQuery.ToListAsync();
                    if (loadFactors.Any())
                    {
                        var vehicleBalances = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && loadFactors.Select(a => a.Id).Contains(a.LoadFactorId.Value)).ToListAsync();
                        foreach (var factor in loadFactors)
                        {
                            var shippingFeeRoute = shippingFeeRoutes.Single(a => a.Id.Equals(factor.ShippingFeeRouteId));
                            factor.OriginId = shippingFeeRoute.OriginId;
                            factor.DestinationId = shippingFeeRoute.DestinationId;
                            factor.DriverFee = item.DriverPrice;
                            factor.Amount = item.Price;
                            factor.TonnagePrice = item.TonnagePrice;
                            factor.DriverTonnagePrice = item.DriverTonnagePrice;

                            if (isDriverFeeChanged)
                            {
                                var balance = vehicleBalances.Single(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(factor.Id));
                                balance.Amount = factor.DriverFee +
                                    ((factor.Tonnage.HasValue && factor.DriverTonnagePrice.HasValue) ? factor.Tonnage.Value * factor.DriverTonnagePrice.Value : 0) +
                                    (factor.WeighbridgePrice.HasValue ? factor.WeighbridgePrice.Value : 0) +
                                    (factor.DriverLoadSleepPrice.HasValue ? factor.DriverLoadSleepPrice.Value : 0);
                                balance.EditDatetime = DateTime.Now;
                            }
                        }
                        _loadFactorRepo.UpdateRange(loadFactors);
                        await _loadFactorRepo.Save();

                        await _vehicleBalanceRepository.Save();
                    }

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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShippingFeeGroup(int id)
        {
            var item = await _shippingFeeGroupRepository.Get(id);

            if (item == null) return NotFound();

            _shippingFeeGroupRepository.Delete(item);
            try
            {
                await _shippingFeeGroupRepository.Save();
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
        public async Task<IActionResult> ChangeShippingFeeGroup(long contractId, string rounding,
            string amountDate, double amount, string type,
            string driverAmountDate, double driverAmount, string driverType,
            string tonnageAmountDate, double tonnageAmount, string tonnageType,
            string tonnageDriverAmountDate, double tonnageDriverAmount, string driverTonnageType)
        {
            string msg;
            string status = "danger";
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

            var contract = await _contractRepo.Get(contractId);

            //var latestContractAddon = await _contractRepo.Contracts().AsNoTracking().Where(a => a.ParentContractId.Equals(contractId)).OrderByDescending(a => a.StartDate).FirstOrDefaultAsync();
            //if (latestContractAddon == null)
            //    latestContractAddon = await _contractRepo.Get(contractId);

            //var exludedRoutes = new List<long> { 44, 45, 47, 30301 };
            //var feeList = await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(contractId) /*&& !exludedRoutes.Contains(a.OriginId) && !exludedRoutes.Contains(a.DestinationId)*/).ToListAsync();
            var feeList = await _shippingFeeGroupRepository.Query().Where(a => a.ContractId.Equals(contractId) && a.Price > 10).ToListAsync();
            var loadFactors = await _loadFactorRepo.GetLoadFactorsByContractId(contractId, contract.StartDate);
            var vehicleBalances = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && loadFactors.Select(b => b.Id).ToList().Contains(a.LoadFactorId.Value)).ToListAsync();

            foreach (var fee in feeList)
            {
                if (amount != 0)
                {
                    if (type.Equals("percent"))
                    {
                        var a = fee.Price * amount / 100;

                        if (rounding == "floor")
                            a = Math.Floor(a);
                        else if (rounding == "ceil")
                            a = Math.Ceiling(a);

                        int b = Convert.ToInt32(a);
                        fee.Price += b;
                    }
                    else
                        fee.Price += amount;
                }

                if (driverAmount != 0)
                {
                    if (driverType.Equals("percent"))
                    {
                        var a = fee.DriverPrice * driverAmount / 100;

                        if (rounding == "floor")
                            a = Math.Floor(a);
                        else if (rounding == "ceil")
                            a = Math.Ceiling(a);

                        int b = Convert.ToInt32(a);
                        fee.DriverPrice += b;
                    }
                    else fee.DriverPrice += driverAmount;
                }

                if (fee.TonnagePrice.HasValue && tonnageAmount != 0)
                {
                    if (tonnageType.Equals("percent"))
                    {
                        var a = fee.TonnagePrice.Value * tonnageAmount / 100;

                        if (rounding == "floor")
                            a = Math.Floor(a);
                        else if (rounding == "ceil")
                            a = Math.Ceiling(a);

                        int b = Convert.ToInt32(a);
                        fee.TonnagePrice = fee.TonnagePrice.Value + b;
                    }
                    else fee.TonnagePrice = fee.TonnagePrice.Value + tonnageAmount;
                }

                if (fee.DriverTonnagePrice.HasValue && tonnageDriverAmount != 0)
                {
                    if (driverTonnageType.Equals("percent"))
                    {
                        var a = fee.DriverTonnagePrice.Value * tonnageDriverAmount / 100;

                        if (rounding == "floor")
                            a = Math.Floor(a);
                        else if (rounding == "ceil")
                            a = Math.Ceiling(a);

                        int b = Convert.ToInt32(a);
                        fee.DriverTonnagePrice = fee.DriverTonnagePrice.Value + b;
                    }
                    else fee.DriverTonnagePrice = fee.DriverTonnagePrice.Value + tonnageDriverAmount;
                }

                var thisLoadFactor = loadFactors.Where(l => l.ShippingFeeRouteId.HasValue && _shippingFeeRouteRepository.Query().Where(a => a.ShippingFeeGroupId.Equals(fee.Id)).Select(a => a.Id).Contains(l.ShippingFeeRouteId.Value)).ToList();
                if (thisLoadFactor.Any())
                {
                    foreach (var loadFactor in thisLoadFactor)
                    {
                        bool isEdited = false;

                        if (loadFactor.Date >= amountDatetime /*&& loadFactor.CalendarId >= 5*/)
                            loadFactor.Amount = fee.Price;

                        if (loadFactor.Date >= driverAmountDatetime && !(loadFactor.IsFreeDriverPrice || loadFactor.IsDriverFeeEditedByAdmin))
                        {
                            isEdited = true;
                            loadFactor.DriverFee = fee.DriverPrice;

                            //if (loadFactor.MehrcomParsLoadFactor is not null && loadFactor.MehrcomParsLoadFactor.HasAddonMessage)
                            //    loadFactor.DriverFee += (loadFactor.DriverFee * 0.3);
                        }

                        if (loadFactor.Date >= tonnageAmountDatetime && loadFactor.TonnagePrice.HasValue)
                        {
                            loadFactor.TonnagePrice = fee.TonnagePrice;
                        }

                        if (loadFactor.Date >= tonnageDriverAmountDatetime && loadFactor.DriverTonnagePrice.HasValue && !(loadFactor.IsFreeDriverPrice || loadFactor.IsDriverFeeEditedByAdmin))
                        {
                            isEdited = true;
                            loadFactor.DriverTonnagePrice = fee.DriverTonnagePrice;
                        }

                        if (isEdited)
                        {
                            var balanceItem = vehicleBalances.Single(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(loadFactor.Id));

                            balanceItem.Amount = loadFactor.DriverFee +
                        ((loadFactor.Tonnage.HasValue && loadFactor.DriverTonnagePrice.HasValue) ? loadFactor.Tonnage.Value * loadFactor.DriverTonnagePrice.Value : 0) +
                        (loadFactor.WeighbridgePrice.HasValue ? loadFactor.WeighbridgePrice.Value : 0) +
                        (loadFactor.DriverLoadSleepPrice.HasValue ? loadFactor.DriverLoadSleepPrice.Value : 0);

                            balanceItem.EditDatetime = DateTime.Now;
                        }
                    }
                    _shippingFeeGroupRepository.UpdateLoadFactors(thisLoadFactor);
                }
            }

            _shippingFeeGroupRepository.UpdateRange(feeList);

            try
            {
                await _shippingFeeGroupRepository.Save();
                await _vehicleBalanceRepository.Save();
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
        public async Task<IActionResult> ChangeShippingFeeGroupByAmount(long contractId, string type, string amountDate, long calendarId, double oldAmount, double newAmount)
        {
            var amountDateArray = amountDate.PersianToEnglish().Split('/');
            var amountDatetime = new PersianDateTime(Convert.ToInt32(amountDateArray[0]), Convert.ToInt32(amountDateArray[1]), Convert.ToInt32(amountDateArray[2])).ToDateTime();

            var latestContractAddon = await _contractRepo.Contracts().AsNoTracking().Where(a => a.ParentContractId.Equals(contractId)).OrderByDescending(a => a.StartDate).FirstOrDefaultAsync();
            if (latestContractAddon == null)
                latestContractAddon = await _contractRepo.Get(contractId);

            var feeQuery = _shippingFeeGroupRepository.Query().Where(a => a.ContractId.Equals(contractId));
            if (type == "driver")
                feeQuery = feeQuery.Where(a => a.DriverPrice.Equals(oldAmount));
            else
                feeQuery = feeQuery.Where(a => a.Price.Equals(oldAmount));
            var feeList = await feeQuery.ToListAsync();

            var loadFactorQuery = _loadFactorRepo.LoadFactors().Where(a => a.ContractId.Equals(contractId));
            if (type == "driver")
                loadFactorQuery = loadFactorQuery.Where(a => a.DriverFee.Equals(oldAmount));
            else
                loadFactorQuery = loadFactorQuery.Where(a => a.Amount.Equals(oldAmount));

            if (calendarId > 0)
                loadFactorQuery = loadFactorQuery.Where(a => a.CalendarId >= calendarId);
            else
                loadFactorQuery = loadFactorQuery.Where(a => a.Date >= amountDatetime);
            var loadFactors = await loadFactorQuery.ToListAsync();

            var vehicleBalances = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && loadFactors.Select(b => b.Id).ToList().Contains(a.LoadFactorId.Value)).ToListAsync();

            foreach (var fee in feeList)
            {
                if (type == "amount")
                {
                    fee.Price = newAmount;
                }
                else
                {
                    fee.DriverPrice = newAmount;
                }
            }

            if (type == "amount")
            {
                foreach (var loadFactor in loadFactors)
                {
                    loadFactor.Amount = newAmount;
                }
            }
            else
            {
                foreach (var loadFactor in loadFactors)
                {
                    loadFactor.DriverFee = newAmount;

                    var balanceItem = vehicleBalances.Single(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(loadFactor.Id));
                    balanceItem.Amount = loadFactor.DriverFee +
                ((loadFactor.Tonnage.HasValue && loadFactor.DriverTonnagePrice.HasValue) ? loadFactor.Tonnage.Value * loadFactor.DriverTonnagePrice.Value : 0) +
                (loadFactor.WeighbridgePrice.HasValue ? loadFactor.WeighbridgePrice.Value : 0) +
                (loadFactor.DriverLoadSleepPrice.HasValue ? loadFactor.DriverLoadSleepPrice.Value : 0);
                    balanceItem.EditDatetime = DateTime.Now;
                }
            }

            _shippingFeeGroupRepository.UpdateRange(feeList);
            _loadFactorRepo.UpdateRange(loadFactors);

            try
            {
                await _shippingFeeGroupRepository.Save();
                await _loadFactorRepo.Save();
                await _vehicleBalanceRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateShippingFeeGroupTypeFromNormal(string contractRowId, long customerId)
        {
            var latestContracts = await _contractRepo.Contracts().AsNoTracking().Where(a => !a.RowId.Equals(contractRowId) && a.CustomerId.Equals(customerId))
                .Select(a => new { a.Number, a.RowId, a.EndDate }).OrderByDescending(a => a.EndDate).ToListAsync();
            if (latestContracts.Count == 0)
                return NotFound("قراردادی وجود ندارد.");

            return Json(latestContracts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DoCreateShippingFeeGroupTypeFromNormal(string contractRowId, string newContractRowId)
        {
            var newContract = await _contractRepo.Get(newContractRowId);
            var contract = await _contractRepo.Contracts().Include(a => a.ShippingFeeGroups).ThenInclude(a => a.ShippingFeeRoutes).Where(a => a.RowId.Equals(contractRowId)).FirstOrDefaultAsync();
            foreach (var item in contract.ShippingFeeGroups)
            {
                var newShippingFeeGroup = new ShippingFeeGroup
                {
                    ContractId = newContract.Id,
                    CreateDate = DateTime.Now,
                    CreatorId = _userManager.GetUserId(User),
                    Destination = item.Destination,
                    DriverPrice = item.DriverPrice,
                    DriverTonnagePrice = item.DriverTonnagePrice,
                    Origin = item.Origin,
                    Price = item.Price,
                    ShippingFeeLoadTypeId = item.ShippingFeeLoadTypeId,
                    Title = item.Title,
                    TonnagePrice = item.TonnagePrice,
                    Vehicle = item.Vehicle,
                    ShippingFeeRoutes = new List<ShippingFeeRoute>()
                };

                if (item.ShippingFeeRoutes.Any())
                    foreach (var route in item.ShippingFeeRoutes)
                    {
                        newShippingFeeGroup.ShippingFeeRoutes.Add(new ShippingFeeRoute
                        {
                            CreateDate = DateTime.Now,
                            CreatorId = newShippingFeeGroup.CreatorId,
                            Title = route.Title,
                            DestinationId = route.DestinationId,
                            OriginId = route.OriginId
                        });
                    }

                _shippingFeeGroupRepository.Create(newShippingFeeGroup);
            }
            try
            {
                await _shippingFeeGroupRepository.Save();
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
        public async Task<IActionResult> DeleteGroupShippingFeeGroup(long[] idList)
        {
            if (idList.Length > 0)
            {
                if (await _loadFactorRepo.LoadFactors().AnyAsync(a => idList.Contains(a.ShippingFeeId.Value)))
                {
                    TempData["msg"] = $"نرخ های انتخابی دارای بارنامه هستند و قابلیت حذف ندارند. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var shippingFees = await _shippingFeeGroupRepository.Query().Where(a => idList.Contains(a.Id)).ToListAsync();
                foreach (var item in shippingFees)
                {
                    _shippingFeeGroupRepository.Delete(item);
                }

                try
                {
                    await _shippingFeeGroupRepository.Save();

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

        #region ShippingFeeRoute
        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> ShippingFeeRoute(int? id)
        {
            if (!id.HasValue) return NotFound();

            var shippingFeeGroup = await _shippingFeeGroupRepository.Query().AsNoTracking()
                .Include(a => a.Contract).ThenInclude(a => a.Customer).Include(a => a.ShippingFeeLoadType)
                .Include(a => a.ShippingFeeRoutes).ThenInclude(a => a.Origin)
                .Include(a => a.ShippingFeeRoutes).ThenInclude(a => a.Destination).FirstOrDefaultAsync(a => a.Id.Equals(id.Value));

            if (shippingFeeGroup is null) return NotFound();

            return View(shippingFeeGroup);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> ShippingFeeRoutePartial(long? shippingFeeGroupId)
        {
            if (!shippingFeeGroupId.HasValue) return NotFound();

            return PartialView("_ShippingFeeRoute", await _shippingFeeRouteRepository.Query()
                .Where(a => a.ShippingFeeGroupId.Equals(shippingFeeGroupId.Value)).Include(a => a.Origin).Include(a => a.Destination).OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateShippingFeeRoute(int shippingFeeGroupId)
        {
            ViewData["ShippingFeeGroup"] = await _shippingFeeGroupRepository.Get(shippingFeeGroupId);
            ViewData["LoadRoutes"] = await _loadRouteRepo.LoadRoutes().Where(a => a.RealStatus).AsNoTracking().ToListAsync();

            return PartialView("~/Views/Admin/Create/ShippingFeeRoute.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateShippingFeeRoute(ShippingFeeRoute s)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                if (await _shippingFeeRouteRepository.Query().AsNoTracking()
                    .AnyAsync(a => a.ShippingFeeGroupId.Equals(s.ShippingFeeGroupId) && a.OriginId.Equals(s.OriginId) && a.DestinationId.Equals(s.DestinationId) && a.Title.Equals(s.Title)))
                    return Json(new { msg = "نرخ حمل و نقل ثبت شده تکراری است.", status });

                s.CreatorId = _userManager.GetUserId(User);
                _shippingFeeRouteRepository.Create(s);

                try
                {
                    await _shippingFeeRouteRepository.Save();
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
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditShippingFeeRoute(int id)
        {
            ViewData["LoadRoutes"] = await _loadRouteRepo.LoadRoutes().Where(a => a.RealStatus).AsNoTracking().ToListAsync();

            return PartialView("~/Views/Admin/Edit/ShippingFeeRoute.cshtml", await _shippingFeeRouteRepository.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditShippingFeeRoute(ShippingFeeRoute s)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                if (s.DestinationId == s.OriginId)
                    return Json(new { msg = "مبدا و مقصد نمی تواند یکی باشد.", status });

                if (await _shippingFeeRouteRepository.Query().AsNoTracking()
                    .AnyAsync(a => !a.Id.Equals(s.Id) && a.ShippingFeeGroupId.Equals(s.ShippingFeeGroupId) && a.OriginId.Equals(s.OriginId) && a.DestinationId.Equals(s.DestinationId) && a.Title.Equals(s.Title)))
                    return Json(new { msg = "مسیر حمل و نقل ثبت شده تکراری است.", status });

                var item = await _shippingFeeRouteRepository.Get(s.Id);

                item.Title = s.Title;
                item.DestinationId = s.DestinationId;
                item.OriginId = s.OriginId;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDate = DateTime.Now;

                _shippingFeeRouteRepository.Update(item);

                try
                {
                    await _shippingFeeRouteRepository.Save();

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

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> DeleteShippingFeeRoute(int id)
        {
            var item = await _shippingFeeRouteRepository.Get(id);

            if (item == null) return NotFound();

            if (_loadFactorRepo.LoadFactors().Any(a => a.ShippingFeeRouteId.HasValue && a.ShippingFeeRouteId.Value.Equals(id)))
            {
                TempData["msg"] = "این مسیر دارای بارنامه است و حذف آن ممکن نیست. ابتدا بارنامه های آن را به مسیری دیگر انتقال دهید. |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            _shippingFeeRouteRepository.Delete(item);
            try
            {
                await _shippingFeeRouteRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<JsonResult> GetShippingFeeRouteJson(long contractId)
        {
            return Json(await _shippingFeeRouteRepository.ShippingFeeRouteWithPrice(contractId));
        }
        #endregion

        #region ShippingFee
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShippingFee(string contractId)
        {
            if (string.IsNullOrWhiteSpace(contractId)) return BadRequest();

            var contract = await _contractRepo.Contracts().Where(a => a.RowId.Equals(contractId)).Include(a => a.ContractAddons).Include(a => a.Customer).FirstOrDefaultAsync();
            if (contract == null) return NotFound();

            ViewData["Contract"] = contract;
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return View(await _shippingFeeRepo.ShippingFees().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.ShippingFeeLoadType).Where(a => a.ContractId.Equals(contract.Id)).OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShippingFeePartial(string contractId)
        {
            var contract = await _contractRepo.Contracts().Include(a => a.Customer).AsNoTracking().FirstOrDefaultAsync(a => a.RowId.Equals(contractId));
            if (contract == null) return NotFound();

            ViewData["Contract"] = contract;
            return PartialView("_ShippingFee", await _shippingFeeRepo.ShippingFees().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.ShippingFeeLoadType).Where(a => a.ContractId.Equals(contract.Id)).OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShippingFee_Search()
        {
            var vehicleTypes = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType == DefinitionType.Car)
                .Select(a => new
                {
                    a.Title
                }).ToListAsync();

            var loadRoutes = await _loadRouteRepo.LoadRoutes().AsNoTracking().Where(a => a.RealStatus).OrderBy(a => a.Title).ToListAsync();
            return Json(new
            {
                vehicleTypes = vehicleTypes,
                origins = loadRoutes.Where(a => a.RouteType == LoadRouteType.Origin).Select(a => new { a.Id, a.Title }).ToList(),
                destinations = loadRoutes.Where(a => a.RouteType == LoadRouteType.Destionation).Select(a => new { a.Id, a.Title }).ToList()
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShippingFee_Search(int? p, long contractId, long originId, long destinationId, string vehicleType, string title, double? amount, double? driverFee)
        {
            var pageNumber = p ?? 1;
            ViewData["Contract"] = await _contractRepo.Contracts().Include(a => a.Customer).FirstOrDefaultAsync(a => a.Id.Equals(contractId));

            var query = _shippingFeeRepo.ShippingFees().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.ShippingFeeLoadType).Where(a => a.ContractId.Equals(contractId));

            if (vehicleType != "all")
                query = query.Where(a => a.Vehicle.Equals(vehicleType));
            if (!string.IsNullOrWhiteSpace(title) && title != null)
                query = query.Where(a => a.Title.Contains(title));
            if (originId > 0)
                query = query.Where(a => a.OriginId.Equals(originId));
            if (destinationId > 0)
                query = query.Where(a => a.DestinationId.Equals(destinationId));
            if (driverFee.HasValue && driverFee.Value >= 0)
                query = query.Where(a => a.DriverPrice.Equals(driverFee.Value));
            if (amount.HasValue && amount.Value >= 0)
                query = query.Where(a => a.Price.Equals(amount.Value));

            return PartialView(await query.OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateShippingFee(string contractId)
        {
            ViewData["Contract"] = await _contractRepo.Contracts().Where(a => a.RowId.Equals(contractId)).Include(a => a.Customer).FirstOrDefaultAsync();
            List<DefinitionType> types = new()
            {
                DefinitionType.Car
            };
            ViewData["Data"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => types.Contains(a.DefinitionType)).ToListAsync();
            ViewData["LoadRoutes"] = await _loadRouteRepo.LoadRoutes().Where(a => a.RealStatus).AsNoTracking().ToListAsync();
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
                    .AnyAsync(a => a.ContractId.Equals(s.ContractId) && a.ShippingFeeType == s.ShippingFeeType && a.ShippingFeeLoadTypeId.Equals(s.ShippingFeeLoadTypeId) && a.Vehicle.Equals(s.Vehicle) && a.OriginId.Equals(s.OriginId) && a.DestinationId.Equals(s.DestinationId) && a.DriverPrice.Equals(s.DriverPrice) && a.Title.Equals(s.Title)))
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
            ViewData["LoadRoutes"] = await _loadRouteRepo.LoadRoutes().Where(a => a.RealStatus).AsNoTracking().ToListAsync();
            ViewData["LoadTypes"] = await _shippingFeeLoadTypeRepo.ShippingFeeLoadTypes().AsNoTracking().ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.Sequence).ToListAsync();

            return PartialView("~/Views/Admin/Edit/ShippingFee.cshtml", await _shippingFeeRepo.Get(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditShippingFee(ShippingFee s, string DateLimit, long CalendarLimit)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                if (s.DestinationId == s.OriginId)
                    return Json(new { msg = "مبدا و مقصد نمی تواند یکی باشد.", status });

                if (await _shippingFeeRepo.ShippingFees().AsNoTracking()
                    .AnyAsync(a => !a.Id.Equals(s.Id) && a.ContractId.Equals(s.ContractId) && a.ShippingFeeLoadTypeId.Equals(s.ShippingFeeLoadTypeId) && a.ShippingFeeType == s.ShippingFeeType && a.Vehicle.Equals(s.Vehicle) && a.OriginId.Equals(s.OriginId) && a.DestinationId.Equals(s.DestinationId) && a.DriverPrice.Equals(s.DriverPrice) && a.Title.Equals(s.Title)))
                    return Json(new { msg = "نرخ حمل و نقل ثبت شده تکراری است.", status });

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

                    var loadFactorQuery = _loadFactorRepo.LoadFactors().Where(a => a.ContractId.Equals(item.ContractId) && a.ShippingFeeId.Equals(item.Id) && a.Date >= latestContractAddon.StartDate && !a.IsDriverFeeEditedByAdmin);
                    if (!string.IsNullOrWhiteSpace(DateLimit))
                    {
                        DateLimit = DateLimit.PersianToEnglish();
                        var dateLimitArr = DateLimit.Split("/");
                        var date = new PersianDateTime(Convert.ToInt32(dateLimitArr[0]), Convert.ToInt32(dateLimitArr[1]), Convert.ToInt32(dateLimitArr[2])).ToDateTime();
                        loadFactorQuery = loadFactorQuery.Where(a => a.Date >= date);
                    }

                    if (CalendarLimit > 0)
                    {
                        var calendars = await _calendarRepo.Calendars().Where(a => a.Sequence >= _calendarRepo.Calendars().Single(a => a.Id.Equals(CalendarLimit)).Sequence).Select(a => a.Id).ToListAsync();
                        loadFactorQuery = loadFactorQuery.Where(a => calendars.Contains(a.CalendarId));
                    }

                    var loadFactors = await loadFactorQuery.ToListAsync();
                    if (loadFactors.Any())
                    {
                        var vehicleBalances = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && loadFactors.Select(a => a.Id).Contains(a.LoadFactorId.Value)).ToListAsync();
                        foreach (var factor in loadFactors)
                        {
                            factor.OriginId = item.OriginId;
                            factor.DestinationId = item.DestinationId;
                            factor.DriverFee = item.DriverPrice;
                            factor.Amount = item.Price;
                            factor.TonnagePrice = item.TonnagePrice;
                            factor.DriverTonnagePrice = item.DriverTonnagePrice;

                            var balance = vehicleBalances.Single(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(factor.Id));
                            balance.Amount = factor.DriverFee +
                    ((factor.Tonnage.HasValue && factor.DriverTonnagePrice.HasValue) ? factor.Tonnage.Value * factor.DriverTonnagePrice.Value : 0) +
                    (factor.WeighbridgePrice.HasValue ? factor.WeighbridgePrice.Value : 0) +
                    (factor.DriverLoadSleepPrice.HasValue ? factor.DriverLoadSleepPrice.Value : 0);
                            balance.EditDatetime = DateTime.Now;
                        }
                        _loadFactorRepo.UpdateRange(loadFactors);
                        await _loadFactorRepo.Save();

                        await _vehicleBalanceRepository.Save();
                    }

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

            //var exludedRoutes = new List<long> { 44, 45, 47, 30301 };
            var feeList = await _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(contractId) /*&& !exludedRoutes.Contains(a.OriginId) && !exludedRoutes.Contains(a.DestinationId)*/).ToListAsync();
            var loadFactors = await _loadFactorRepo.GetLoadFactorsByContractId(contractId, latestContractAddon.StartDate);
            var vehicleBalances = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && loadFactors.Select(b => b.Id).ToList().Contains(a.LoadFactorId.Value)).ToListAsync();

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

                var thisLoadFactor = loadFactors.Where(a => a.ShippingFeeId.Equals(fee.Id)).ToList();
                if (thisLoadFactor.Any())
                {
                    foreach (var loadFactor in thisLoadFactor)
                    {
                        bool isEdited = false;

                        if (loadFactor.Date >= amountDatetime /*&& loadFactor.CalendarId >= 5*/)
                        {
                            loadFactor.Amount = fee.Price;
                        }

                        if (loadFactor.Date >= driverAmountDatetime && !loadFactor.IsFreeDriverPrice)
                        {
                            isEdited = true;
                            loadFactor.DriverFee = fee.DriverPrice;

                            //if (loadFactor.MehrcomParsLoadFactor is not null && loadFactor.MehrcomParsLoadFactor.HasAddonMessage)
                            //    loadFactor.DriverFee += (loadFactor.DriverFee * 0.3);
                        }

                        if (loadFactor.Date >= tonnageAmountDatetime && loadFactor.TonnagePrice.HasValue)
                        {
                            loadFactor.TonnagePrice = fee.TonnagePrice;
                        }

                        if (loadFactor.Date >= tonnageDriverAmountDatetime && loadFactor.DriverTonnagePrice.HasValue && !loadFactor.IsFreeDriverPrice)
                        {
                            isEdited = true;
                            loadFactor.DriverTonnagePrice = fee.DriverTonnagePrice;
                        }

                        if (isEdited)
                        {
                            var balanceItem = vehicleBalances.Single(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(loadFactor.Id));
                            balanceItem.Amount = loadFactor.DriverFee +
                        ((loadFactor.Tonnage.HasValue && loadFactor.DriverTonnagePrice.HasValue) ? loadFactor.Tonnage.Value * loadFactor.DriverTonnagePrice.Value : 0) +
                        (loadFactor.WeighbridgePrice.HasValue ? loadFactor.WeighbridgePrice.Value : 0) +
                        (loadFactor.DriverLoadSleepPrice.HasValue ? loadFactor.DriverLoadSleepPrice.Value : 0);
                            balanceItem.EditDatetime = DateTime.Now;
                        }
                    }
                    _shippingFeeRepo.UpdateLoadFactors(thisLoadFactor);
                }
            }

            _shippingFeeRepo.UpdateRange(feeList);

            try
            {
                await _shippingFeeRepo.Save();
                await _vehicleBalanceRepository.Save();
                msg = "عملیات موفقیت آمیز بود.";
                status = "success";
            }
            catch (Exception e)
            {
                msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
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
            var contract = await _contractRepo.Contracts().Include(a => a.ShippingFees).Where(a => a.RowId.Equals(contractRowId)).FirstOrDefaultAsync();
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeShippingFeeByAmount(long contractId, string type, string amountDate, long calendarId, double oldAmount, double newAmount)
        {
            var amountDateArray = amountDate.PersianToEnglish().Split('/');
            var amountDatetime = new PersianDateTime(Convert.ToInt32(amountDateArray[0]), Convert.ToInt32(amountDateArray[1]), Convert.ToInt32(amountDateArray[2])).ToDateTime();

            var latestContractAddon = await _contractRepo.Contracts().AsNoTracking().Where(a => a.ParentContractId.Equals(contractId)).OrderByDescending(a => a.StartDate).FirstOrDefaultAsync();
            if (latestContractAddon == null)
                latestContractAddon = await _contractRepo.Get(contractId);

            var feeQuery = _shippingFeeRepo.ShippingFees().Where(a => a.ContractId.Equals(contractId));
            if (type == "driver")
                feeQuery = feeQuery.Where(a => a.DriverPrice.Equals(oldAmount));
            else
                feeQuery = feeQuery.Where(a => a.Price.Equals(oldAmount));
            var feeList = await feeQuery.ToListAsync();

            var loadFactorQuery = _loadFactorRepo.LoadFactors().Where(a => a.ContractId.Equals(contractId));
            if (type == "driver")
                loadFactorQuery = loadFactorQuery.Where(a => a.DriverFee.Equals(oldAmount));
            else
                loadFactorQuery = loadFactorQuery.Where(a => a.Amount.Equals(oldAmount));

            if (calendarId > 0)
                loadFactorQuery = loadFactorQuery.Where(a => a.CalendarId >= calendarId);
            else
                loadFactorQuery = loadFactorQuery.Where(a => a.Date >= amountDatetime);
            var loadFactors = await loadFactorQuery.ToListAsync();

            var vehicleBalances = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && loadFactors.Select(b => b.Id).ToList().Contains(a.LoadFactorId.Value)).ToListAsync();

            foreach (var fee in feeList)
            {
                if (type == "amount")
                {
                    fee.Price = newAmount;
                }
                else
                {
                    fee.DriverPrice = newAmount;
                }
            }

            if (type == "amount")
            {
                foreach (var loadFactor in loadFactors)
                {
                    loadFactor.Amount = newAmount;
                }
            }
            else
            {
                foreach (var loadFactor in loadFactors)
                {
                    loadFactor.DriverFee = newAmount;

                    var balanceItem = vehicleBalances.Single(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(loadFactor.Id));
                    balanceItem.Amount = loadFactor.DriverFee +
                ((loadFactor.Tonnage.HasValue && loadFactor.DriverTonnagePrice.HasValue) ? loadFactor.Tonnage.Value * loadFactor.DriverTonnagePrice.Value : 0) +
                (loadFactor.WeighbridgePrice.HasValue ? loadFactor.WeighbridgePrice.Value : 0) +
                (loadFactor.DriverLoadSleepPrice.HasValue ? loadFactor.DriverLoadSleepPrice.Value : 0);
                    balanceItem.EditDatetime = DateTime.Now;
                }
            }

            _shippingFeeRepo.UpdateRange(feeList);
            _loadFactorRepo.UpdateRange(loadFactors);

            try
            {
                await _shippingFeeRepo.Save();
                await _loadFactorRepo.Save();
                await _vehicleBalanceRepository.Save();
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
        public async Task<IActionResult> DeleteGroupShippingFee(long[] idList)
        {
            if (idList.Length > 0)
            {
                if (await _loadFactorRepo.LoadFactors().AnyAsync(a => idList.Contains(a.ShippingFeeId.Value)))
                {
                    TempData["msg"] = $"نرخ های انتخابی دارای بارنامه هستند و قابلیت حذف ندارند. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var shippingFees = await _shippingFeeRepo.ShippingFees().Where(a => idList.Contains(a.Id)).ToListAsync();
                foreach (var item in shippingFees)
                {
                    _shippingFeeRepo.Delete(item);
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

            var query = _loadFactorRepo.LoadFactors().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Vehicle).Include(a => a.Driver).Include(a => a.Contract).ThenInclude(a => a.Customer).AsQueryable();
            if (User.IsInRole("RegisterUser"))
                query = query.Where(a => a.AdminId.Equals(_userManager.GetUserId(User)));

            ViewBag.data = await query.OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 15);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadFactorPartial(int? p)
        {
            var pageNumber = p ?? 1;

            var query = _loadFactorRepo.LoadFactors().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Vehicle).Include(a => a.Driver).Include(a => a.Contract).ThenInclude(a => a.Customer).AsQueryable();
            if (User.IsInRole("RegisterUser"))
                query = query.Where(a => a.AdminId.Equals(_userManager.GetUserId(User)));

            ViewBag.data = await query.OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 15);
            ViewBag.isSearch = false;
            return PartialView("_LoadFactor");
        }

        [HttpGet]
        public async Task<PartialViewResult> LoadFactorDetail(int id)
        {
            var item = await _loadFactorRepo.LoadFactors().Include(a => a.Driver).Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Vehicle).Include(a => a.Calendar).Include(a => a.Contract).Include(a => a.LoadFactorGovRegistor).Where(a => a.Id.Equals(id)).SingleOrDefaultAsync();
            ViewData["Admin"] = await _userManager.FindByIdAsync(item.AdminId);
            return PartialView("_LoadFactorDetail", item);
        }

        [HttpGet]
        public async Task<IActionResult> SearchLoadFactor(int? p, string exitNumber, string loadNumber, string vehicleNumber, bool isFreeDriverPrice, long? calendar)
        {
            if (!string.IsNullOrWhiteSpace(exitNumber) || !string.IsNullOrWhiteSpace(loadNumber) || !string.IsNullOrWhiteSpace(vehicleNumber) || calendar.HasValue)
            {
                var pageNum = p ?? 1;

                var query = _loadFactorRepo.LoadFactors().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Vehicle).Include(a => a.Driver).Include(a => a.Contract).ThenInclude(a => a.Customer).AsQueryable();

                if (!string.IsNullOrWhiteSpace(exitNumber))
                    query = query.Where(a => a.ExitNumber.Contains(exitNumber));
                if (!string.IsNullOrWhiteSpace(loadNumber))
                    query = query.Where(a => a.LoadNumber.Contains(loadNumber) || a.LoadNumberGov.Contains(loadNumber));
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

            ViewData["Fees"] = await _shippingFeeRouteRepository.ShippingFeeRouteWithPrice(activeContract);

            ViewData["Contracts"] = contracts;

            var accountBooks = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.CustomerId.Equals(customerId) && a.IsOpen).OrderBy(a => a.Number).ThenByDescending(a => a.Id).ToListAsync();
            if (!accountBooks.Any())
                return NotFound($"صورت وضعیت باز در سیستم وجود ندارد.");
            ViewData["AccountBooks"] = accountBooks;

            ViewData["Drivers"] = await _driverRepository.Drivers().AsNoTracking().Where(a => a.IsActive).OrderBy(a => a.Fullname).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepo.Vehicles().AsNoTracking().Where(a => a.Status && a.RealStatus).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["LoadFactorRegistors"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType == DefinitionType.LoadFactorOrigin).ToListAsync();

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
                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov) && input.LoadFactorGovAmount is null)
                    return NotFound("کرایه بارنامه دولتی را وارد کنید.");

                if (input.LoadFactorGovRegistorId.HasValue && input.GovYear < 1400)
                    return NotFound("تاریخ وارد شده برای بارنامه دولتی را بررسی نمائید.");

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                var customerId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.Id.Equals(input.ContractId)).Select(a => a.CustomerId).FirstOrDefaultAsync();
                var customerLoadFactorDeduction = await _customerRepo.Customers().AsNoTracking().Where(a => a.Id.Equals(customerId)).Select(a => a.LoadFactorDeductions).FirstOrDefaultAsync();

                var fee = await _shippingFeeRouteRepository.GetWithGroup(input.ShippingFeeRouteId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.ShippingFeeGroup.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                int.TryParse(input.ExitNumber, out int exitNumber);
                if (exitNumber > 0)
                    if (await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPlascoLoadFactor).AsNoTracking().AnyAsync(a => a.ExitNumber.Equals(exitNumber.ToString()) && a.SaipaPlascoLoadFactor != null))
                        return NotFound("شماره خروج درج شده تکراری است.");

                if (await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPlascoLoadFactor).AsNoTracking().AnyAsync(a => a.LoadNumber.Equals(input.LoadNumber) && a.SaipaPlascoLoadFactor != null))
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
                    VehicleType = fee.ShippingFeeGroup.Vehicle,
                    ShippingFeeRouteId = input.ShippingFeeRouteId,
                    ShippingFeeGroupId = fee.ShippingFeeGroupId,
                    WithholdingTax = config.WithholdingTax,
                    VAT = config.VAT,
                    LoadFactorDeductions = customerLoadFactorDeduction,
                    AccountBookId = input.AccountBookId,
                    IsDriverFeeEditedByAdmin = false,
                    IsFreeDriverPrice = input.IsFreeDriverPrice,
                    LoadFactorGovAmount = input.LoadFactorGovAmount,
                    LoadFactorGovRegistorId = input.LoadFactorGovRegistorId.Value == 0 ? null : input.LoadFactorGovRegistorId.Value,
                    LoadFactorGovDate = input.LoadFactorGovRegistorId.Value > 0 ? new PersianDateTime(input.GovYear, input.GovMonth, input.GovDay, 0, 0, 0).ToDateTime() : null
                };

                //if (fee.ShippingFeeType == ShippingFeeType.Custom)
                //{
                //    loadFactor.Amount = input.Amount;
                //    loadFactor.DriverFee = input.DriverFee;
                //}
                //else
                //{
                //    loadFactor.Amount = fee.Price;
                //    loadFactor.DriverFee = fee.DriverPrice;
                //}
                loadFactor.Amount = fee.ShippingFeeGroup.Price;
                loadFactor.DriverFee = fee.ShippingFeeGroup.DriverPrice;
                loadFactor.SaipaPlascoLoadFactor = new SaipaPlascoLoadFactor
                {
                    LoadFactor = loadFactor,
                    Sequence = await _loadFactorRepo.GetBiggestSequenceInSaipaPlasco() + 1
                };


                _loadFactorRepo.Create(loadFactor);

                try
                {
                    await _loadFactorRepo.Save();

                    await _vehicleBalanceRepository.Create(new VehicleBalance
                    {
                        Amount = loadFactor.DriverFee,
                        CalendarId = loadFactor.CalendarId,
                        VehicleId = loadFactor.VehicleId,
                        CreateDateTime = loadFactor.Date,
                        LoadFactorId = loadFactor.Id,
                        CustomerId = customerId
                    });

                    await _vehicleBalanceRepository.Save();

                    await _accountBookRepository.UpdateAmount(loadFactor.AccountBookId);
                    await _accountBookRepository.Save();

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

                var relatedContractIds = await _contractRepo.GetAllContractIdListForSameCustomer(input.ContractId);
                if (input.PressFloorType == SaipaPressLoadType.OneFloor)
                {
                    if (!string.IsNullOrWhiteSpace(input.EntryNumber) && await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPressLoadFactor).AnyAsync(a => a.SaipaPressLoadFactor.EntryNumber.Equals(input.EntryNumber)))
                        return NotFound("شماره ورود تکراری است.");

                    if (!string.IsNullOrWhiteSpace(input.ExitNumber) && await _loadFactorRepo.LoadFactors().AnyAsync(a => relatedContractIds.Contains(a.ContractId) && a.ExitNumber.Equals(input.ExitNumber)))
                        return NotFound("شماره خروج تکراری است.");
                }

                if (input.PressFloorType == SaipaPressLoadType.TwoFloor)
                {
                    if (!string.IsNullOrWhiteSpace(input.EntryNumber) && await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPressLoadFactor).CountAsync(a => a.SaipaPressLoadFactor.EntryNumber.Equals(input.EntryNumber)) >= 2)
                        return NotFound("شماره ورود تکراری است.");

                    if (!string.IsNullOrWhiteSpace(input.ExitNumber) && await _loadFactorRepo.LoadFactors().CountAsync(a => relatedContractIds.Contains(a.ContractId) && a.ExitNumber.Equals(input.ExitNumber)) >= 2)
                        return NotFound("شماره خروج تکراری است.");
                }

                var fee = await _shippingFeeRouteRepository.GetWithGroup(input.ShippingFeeRouteId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.ShippingFeeGroup.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                if (await _loadFactorRepo.LoadFactors().AsNoTracking().Include(a => a.SaipaPressLoadFactor).AnyAsync(a => a.LoadNumber.Equals(input.LoadNumber) && a.SaipaPressLoadFactor != null))
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
                    VehicleType = fee.ShippingFeeGroup.Vehicle,
                    ShippingFeeRouteId = input.ShippingFeeRouteId,
                    ShippingFeeGroupId = fee.ShippingFeeGroupId,
                    WithholdingTax = config.WithholdingTax,
                    VAT = config.VAT,
                    LoadFactorDeductions = customerLoadFactorDeduction,
                    Tonnage = input.Tonnage,
                    AccountBookId = input.AccountBookId,
                    IsDriverFeeEditedByAdmin = false,
                    IsFreeDriverPrice = input.IsFreeDriverPrice
                };

                //if (fee.ShippingFeeType == ShippingFeeType.Custom)
                //{
                //    loadFactor.Amount = input.Amount;
                //    loadFactor.DriverFee = input.DriverFee;
                //    loadFactor.TonnagePrice = input.TonnagePrice;
                //    loadFactor.DriverTonnagePrice = input.DriverTonnagePrice;
                //}
                //else
                //{
                //    loadFactor.Amount = fee.Price;
                //    loadFactor.DriverFee = fee.DriverPrice;
                //    loadFactor.TonnagePrice = fee.TonnagePrice;
                //    loadFactor.DriverTonnagePrice = fee.DriverTonnagePrice;
                //}

                loadFactor.Amount = fee.ShippingFeeGroup.Price;
                loadFactor.DriverFee = fee.ShippingFeeGroup.DriverPrice;
                loadFactor.TonnagePrice = fee.ShippingFeeGroup.TonnagePrice;
                loadFactor.DriverTonnagePrice = fee.ShippingFeeGroup.DriverTonnagePrice;

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

                    await _vehicleBalanceRepository.Create(new VehicleBalance
                    {
                        Amount = loadFactor.DriverFee + ((loadFactor.Tonnage.HasValue && loadFactor.DriverTonnagePrice.HasValue) ? loadFactor.Tonnage.Value * loadFactor.DriverTonnagePrice.Value : 0),
                        CalendarId = loadFactor.CalendarId,
                        VehicleId = loadFactor.VehicleId,
                        CreateDateTime = loadFactor.Date,
                        LoadFactorId = loadFactor.Id,
                        CustomerId = customerId
                    });

                    await _vehicleBalanceRepository.Save();

                    await _accountBookRepository.UpdateAmount(loadFactor.AccountBookId);
                    await _accountBookRepository.Save();

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
                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov) && input.LoadFactorGovAmount is null)
                    return NotFound("کرایه بارنامه دولتی را وارد کنید.");

                if (input.LoadFactorGovRegistorId.HasValue && input.GovYear < 1400)
                    return NotFound("تاریخ وارد شده برای بارنامه دولتی را بررسی نمائید.");

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                var customerId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.Id.Equals(input.ContractId)).Select(a => a.CustomerId).FirstOrDefaultAsync();
                var customerLoadFactorDeduction = await _customerRepo.Customers().AsNoTracking().Where(a => a.Id.Equals(customerId)).Select(a => a.LoadFactorDeductions).FirstOrDefaultAsync();

                var fee = await _shippingFeeRouteRepository.GetWithGroupAndLoadRoute(input.ShippingFeeRouteId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                if ((input.SazehLoadType == SazehGostarLoadType.OneWay && fee.ShippingFeeGroup.Title.Equals("رفت و برگشت")) ||
                    (input.SazehLoadType == SazehGostarLoadType.TwoWay && fee.ShippingFeeGroup.Title.Equals("رفت")))
                    return NotFound("نوع انتخابی بارنامه با نوع رفت و برگشت درج شده در نرخ انتخابی تطابق ندارد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.ShippingFeeGroup.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => a.LoadNumber.Equals(input.LoadNumber) && a.SazehGostarLoadFactor != null))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => a.ExitNumber.Equals(input.ExitNumber) && a.SazehGostarLoadFactor != null))
                    return NotFound("شماره درخواست درج شده تکراری است.");

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
                    ShippingFeeRouteId = input.ShippingFeeRouteId,
                    ShippingFeeGroupId = fee.ShippingFeeGroupId,
                    VehicleType = fee.ShippingFeeGroup.Vehicle,
                    WithholdingTax = config.WithholdingTax,
                    VAT = config.VAT,
                    LoadFactorDeductions = customerLoadFactorDeduction,
                    AccountBookId = input.AccountBookId,
                    IsDriverFeeEditedByAdmin = false,
                    IsFreeDriverPrice = input.IsFreeDriverPrice,
                    LoadFactorGovAmount = input.LoadFactorGovAmount,
                    LoadFactorGovRegistorId = input.LoadFactorGovRegistorId.Value == 0 ? null : input.LoadFactorGovRegistorId.Value,
                    LoadFactorGovDate = input.LoadFactorGovRegistorId.Value > 0 ? new PersianDateTime(input.GovYear, input.GovMonth, input.GovDay, 0, 0, 0).ToDateTime() : null
                };

                //if (fee.ShippingFeeType == ShippingFeeType.Custom)
                //{
                //    loadFactor.Amount = input.Amount;
                //    loadFactor.DriverFee = input.DriverFee;
                //}
                //else
                //{
                //    loadFactor.Amount = fee.Price;
                //    loadFactor.DriverFee = fee.DriverPrice;
                //}
                loadFactor.Amount = fee.ShippingFeeGroup.Price;
                loadFactor.DriverFee = fee.ShippingFeeGroup.DriverPrice;

                loadFactor.SazehGostarLoadFactor = new SazehGostarLoadFactor
                {
                    LoadFactorId = loadFactor.Id,
                    Certain = input.Certain,
                    Count = input.Count,
                    Description = $"حمل کالا از {fee.Origin.Title} به {fee.Destination.Title}{(input.SazehLoadType == SazehGostarLoadType.TwoWay ? " رفت و برگشت" : "")} ({fee.ShippingFeeGroup.Vehicle})",
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

                    await _vehicleBalanceRepository.Create(new VehicleBalance
                    {
                        Amount = loadFactor.DriverFee,
                        CalendarId = loadFactor.CalendarId,
                        VehicleId = loadFactor.VehicleId,
                        CreateDateTime = loadFactor.Date,
                        LoadFactorId = loadFactor.Id,
                        CustomerId = customerId
                    });

                    await _vehicleBalanceRepository.Save();

                    await _accountBookRepository.UpdateAmount(loadFactor.AccountBookId);
                    await _accountBookRepository.Save();

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
                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov) && input.LoadFactorGovAmount is null)
                    return NotFound("کرایه بارنامه دولتی را وارد کنید.");

                if (input.LoadFactorGovRegistorId.HasValue && input.GovYear < 1400)
                    return NotFound("تاریخ وارد شده برای بارنامه دولتی را بررسی نمائید.");

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                var customerId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.Id.Equals(input.ContractId)).Select(a => a.CustomerId).FirstOrDefaultAsync();
                var customerLoadFactorDeduction = await _customerRepo.Customers().AsNoTracking().Where(a => a.Id.Equals(customerId)).Select(a => a.LoadFactorDeductions).FirstOrDefaultAsync();

                var fee = await _shippingFeeRouteRepository.GetWithGroupAndLoadRoute(input.ShippingFeeRouteId);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.ShippingFeeGroup.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                string loadType = fee.ShippingFeeGroup.ShippingFeeLoadType.Name;

                if ((!input.Load && !input.Palette && !input.Return) ||
                    (input.Load && input.Palette && !input.Return && !loadType.Contains("بار/پالت")) ||
                    (input.Load && !input.Palette && !input.Return && (!loadType.Contains("بار") || loadType.Contains("پالت") || loadType.Contains("برگشت"))) ||
                    (!input.Load && input.Palette && !input.Return && (!loadType.Contains("پالت") || loadType.Contains("بار") || loadType.Contains("برگشت"))) ||
                    (!input.Load && !input.Palette && input.Return && (!loadType.Contains("برگشت") || loadType.Contains("پالت") || loadType.Contains("بار"))) ||
                    (input.Load && !input.Palette && input.Return && !loadType.Contains("بار/برگشت")) ||
                    (!input.Load && input.Palette && input.Return && !loadType.Contains("پالت/برگشت"))
                    )
                    return NotFound("مقادیر بار/پالت/برگشت با نرخ انتخابی تناسب ندارد.");

                if (await _loadFactorRepo.LoadFactors().Include(a => a.MehrcomParsLoadFactor).AsNoTracking().AnyAsync(a => a.LoadNumber.Equals(input.LoadNumber) && a.MehrcomParsLoadFactor != null))
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
                    VehicleType = fee.ShippingFeeGroup.Vehicle,
                    ShippingFeeRouteId = input.ShippingFeeRouteId,
                    ShippingFeeGroupId = fee.ShippingFeeGroupId,
                    WithholdingTax = config.WithholdingTax,
                    VAT = config.VAT,
                    LoadFactorDeductions = customerLoadFactorDeduction,
                    AccountBookId = input.AccountBookId,
                    Tonnage = (input.TonnagePrice.HasValue && input.DriverTonnagePrice.HasValue) ? 1 : null,
                    TonnagePrice = input.TonnagePrice,
                    DriverTonnagePrice = input.DriverTonnagePrice,
                    WeighbridgePrice = input.WeighbridgePrice,
                    LoadSleepTime = input.LoadSleepTime,
                    LoadSleepPrice = input.LoadSleepPrice,
                    DriverLoadSleepPrice = input.DriverLoadSleepPrice,
                    IsDriverFeeEditedByAdmin = false,
                    IsFreeDriverPrice = input.IsFreeDriverPrice,
                    LoadFactorGovAmount = input.LoadFactorGovAmount,
                    LoadFactorGovRegistorId = input.LoadFactorGovRegistorId.Value == 0 ? null : input.LoadFactorGovRegistorId.Value,
                    LoadFactorGovDate = input.LoadFactorGovRegistorId.Value > 0 ? new PersianDateTime(input.GovYear, input.GovMonth, input.GovDay, 0, 0, 0).ToDateTime() : null
                };

                //if (fee.ShippingFeeType == ShippingFeeType.Custom)
                //{
                //    loadFactor.Amount = input.HasAddonMessage ? Math.Floor(input.Amount + (input.Amount * 0.3)) : input.Amount;
                //    loadFactor.DriverFee = input.HasAddonMessage ? Math.Floor(input.DriverFee + (input.DriverFee * 0.3)) : input.DriverFee;
                //}
                //else
                //{
                //    loadFactor.Amount = input.HasAddonMessage ? Math.Floor(fee.Price + (fee.Price * 0.3)) : fee.Price;
                //    loadFactor.DriverFee = input.HasAddonMessage ? Math.Floor(fee.DriverPrice + (fee.DriverPrice * 0.3)) : fee.DriverPrice;
                //}

                loadFactor.Amount = input.HasAddonMessage ? Math.Floor(fee.ShippingFeeGroup.Price + (fee.ShippingFeeGroup.Price * 0.3)) : fee.ShippingFeeGroup.Price;
                loadFactor.DriverFee = input.HasAddonMessage ? Math.Floor(fee.ShippingFeeGroup.DriverPrice + (fee.ShippingFeeGroup.DriverPrice * 0.3)) : fee.ShippingFeeGroup.DriverPrice;

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

                    await _vehicleBalanceRepository.Create(new VehicleBalance
                    {
                        Amount = loadFactor.DriverFee +
                    ((loadFactor.Tonnage.HasValue && loadFactor.DriverTonnagePrice.HasValue) ? loadFactor.Tonnage.Value * loadFactor.DriverTonnagePrice.Value : 0) +
                    (loadFactor.WeighbridgePrice.HasValue ? loadFactor.WeighbridgePrice.Value : 0) +
                    (loadFactor.DriverLoadSleepPrice.HasValue ? loadFactor.DriverLoadSleepPrice.Value : 0),
                        CalendarId = loadFactor.CalendarId,
                        VehicleId = loadFactor.VehicleId,
                        CreateDateTime = loadFactor.Date,
                        LoadFactorId = loadFactor.Id,
                        CustomerId = customerId
                    });

                    await _vehicleBalanceRepository.Save();

                    await _accountBookRepository.UpdateAmount(loadFactor.AccountBookId);
                    await _accountBookRepository.Save();

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
        public async Task<IActionResult> EditLoadFactor(long loadFactorId)
        {
            var loadFactor = await _loadFactorRepo.LoadFactors().Include(a => a.Contract).ThenInclude(a => a.Customer).FirstOrDefaultAsync(a => a.Id.Equals(loadFactorId));

            if (loadFactor == null) return NotFound("بارنامه پیدا نشد.");

            if (!loadFactor.ShippingFeeRouteId.HasValue) return NotFound("بارنامه قدیمی است و قابلیت ویرایش ندارد");

            var customer = loadFactor.Contract.Customer;
            var contracts = await _contractRepo.Contracts().AsNoTracking().Where(a => a.CustomerId.Equals(customer.Id)).OrderByDescending(a => a.StartDate).ToListAsync();
            if (!contracts.Any()) return NotFound("قراردادی پیدا نشد.");

            ViewData["Contracts"] = contracts;
            ViewData["Drivers"] = await _driverRepository.Drivers().AsNoTracking().OrderBy(a => a.Fullname).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepo.Vehicles().AsNoTracking().Where(a => a.Status && a.RealStatus).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();

            ViewData["Fees"] = await _shippingFeeRouteRepository.ShippingFeeRouteWithPrice(loadFactor.ContractId);

            if (customer.CustomerType == CustomerType.SaipaPress)
                ViewData["LoadTypes"] = await _shippingFeeLoadTypeRepo.ShippingFeeLoadTypes().AsNoTracking().OrderBy(a => a.Name).ToListAsync();
            else if (customer.CustomerType == CustomerType.MehrcomPars)
                ViewData["Categories"] = await _mehrcomParsCategoryRepository.Categories().AsNoTracking().OrderBy(a => a.Title).ToListAsync();

            var accountBooks = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.CustomerId.Equals(customer.Id)).OrderByDescending(a => a.IsOpen).ThenByDescending(a => a.Id).ToListAsync();
            if (!accountBooks.Any())
                return NotFound($"صورت وضعیت باز در سیستم برای {customer.Name} وجود ندارد.");
            ViewData["AccountBooks"] = accountBooks;

            ViewData["LoadFactorRegistors"] = await _definitionRepo.Definitions().AsNoTracking().Where(a => a.DefinitionType == DefinitionType.LoadFactorOrigin).ToListAsync();

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
                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov) && input.LoadFactorGovAmount is null)
                    return NotFound("کرایه بارنامه دولتی را وارد کنید.");

                if (input.LoadFactorGovRegistorId.HasValue && input.LoadFactorGovRegistorId.Value > 0 && input.GovYear < 1400)
                    return NotFound("تاریخ وارد شده برای بارنامه دولتی را بررسی نمائید.");

                if (await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPlascoLoadFactor).AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumber.Equals(input.LoadNumber) && a.SaipaPlascoLoadFactor != null))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov))
                    if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumberGov.Equals(input.LoadNumberGov)))
                        return NotFound("شماره بارنامه دولتی درج شده تکراری است.");

                var fee = await _shippingFeeRouteRepository.GetWithGroup(input.ShippingFeeRouteId.Value);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.ShippingFeeGroup.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                var item = await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPlascoLoadFactor).Include(a => a.Contract).FirstAsync(a => a.Id.Equals(input.Id));
                if (item == null) return NotFound();

                int.TryParse(input.ExitNumber, out int exitNumber);
                if (exitNumber > 0)
                    if (await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPlascoLoadFactor).AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.ExitNumber.Equals(exitNumber.ToString()) && a.SaipaPlascoLoadFactor != null))
                        return NotFound("شماره خروج درج شده تکراری است.");

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (item.AccountBookId != input.AccountBookId && await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");


                //if (await _loadFactorRepo.SequenceExistInSaipaPlasco(item.Id, input.Sequence))
                //    return NotFound("ترتیب وارد شده برای بارنامه تکراری است");

                var oldAccountBookId = item.AccountBookId;

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
                item.VehicleType = fee.ShippingFeeGroup.Vehicle;
                item.ShippingFeeRouteId = input.ShippingFeeRouteId;
                item.ShippingFeeGroupId = fee.ShippingFeeGroupId;
                item.AccountBookId = input.AccountBookId;
                item.IsFreeDriverPrice = input.IsFreeDriverPrice;
                item.LoadFactorGovAmount = input.LoadFactorGovAmount;
                if (input.LoadFactorGovRegistorId.Value > 0)
                {
                    item.LoadFactorGovRegistorId = input.LoadFactorGovRegistorId.Value;
                    item.LoadFactorGovDate = new PersianDateTime(input.GovYear, input.GovMonth, input.GovDay, 0, 0, 0).ToDateTime();
                }

                //item.SaipaPlascoLoadFactor.Sequence = input.Sequence;

                //if (fee.ShippingFeeType == ShippingFeeType.Custom)
                //{
                //    item.Amount = input.Amount;
                //    if (!item.IsDriverFeeEditedByAdmin)
                //        item.DriverFee = input.DriverFee;
                //}
                //else
                //{
                //    item.Amount = fee.Price;
                //    if (!item.IsDriverFeeEditedByAdmin)
                //        item.DriverFee = fee.DriverPrice;
                //}
                item.Amount = fee.ShippingFeeGroup.Price;
                if (!item.IsDriverFeeEditedByAdmin)
                    item.DriverFee = fee.ShippingFeeGroup.DriverPrice;

                _loadFactorRepo.Update(item);
                try
                {
                    await _loadFactorRepo.Save();

                    var balanceItem = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(item.Id)).FirstOrDefaultAsync();
                    if (balanceItem != null)
                    {
                        balanceItem.Amount = item.DriverFee;
                        balanceItem.CreateDateTime = item.Date;
                        balanceItem.EditDatetime = DateTime.Now;
                        balanceItem.VehicleId = item.VehicleId;
                        balanceItem.CustomerId = item.Contract.CustomerId;
                        balanceItem.CalendarId = item.CalendarId;

                        _vehicleBalanceRepository.Update(balanceItem);
                    }
                    else
                    {
                        await _vehicleBalanceRepository.Create(new VehicleBalance
                        {
                            Amount = item.DriverFee,
                            CalendarId = item.CalendarId,
                            VehicleId = item.VehicleId,
                            CreateDateTime = item.Date,
                            LoadFactorId = item.Id,
                            CustomerId = item.Contract.CustomerId
                        });
                    }

                    await _vehicleBalanceRepository.Save();

                    if (item.AccountBookId != oldAccountBookId)
                    {
                        await _accountBookRepository.UpdateAmount(item.AccountBookId);
                        await _accountBookRepository.UpdateAmount(oldAccountBookId);
                    }
                    else
                    {
                        await _accountBookRepository.UpdateAmount(item.AccountBookId);
                    }
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

        [HttpPost]
        public async Task<IActionResult> EditSaipaPressLoadFactor(ESaipaPressLoadFactorVM input, bool HasNumber)
        {
            if (ModelState.IsValid)
            {
                if (HasNumber && string.IsNullOrWhiteSpace(input.EntryNumber) && string.IsNullOrWhiteSpace(input.ExitNumber))
                    return NotFound("لطفا شماره ورود یا خروج را وارد نمائید.");

                var relatedContractIds = await _contractRepo.GetAllContractIdListForSameCustomer(input.ContractId);
                if (input.PressFloorType == SaipaPressLoadType.OneFloor)
                {
                    if (!string.IsNullOrWhiteSpace(input.EntryNumber) && await _loadFactorRepo.LoadFactors().AnyAsync(a => !a.Id.Equals(input.Id) && a.SaipaPressLoadFactor.EntryNumber.Equals(input.EntryNumber)))
                        return NotFound("شماره ورود تکراری است.");

                    if (!string.IsNullOrWhiteSpace(input.ExitNumber) && await _loadFactorRepo.LoadFactors().AnyAsync(a => !a.Id.Equals(input.Id) && relatedContractIds.Contains(a.ContractId) && a.ExitNumber.Equals(input.ExitNumber)))
                        return NotFound("شماره خروج تکراری است.");
                }

                if (input.PressFloorType == SaipaPressLoadType.TwoFloor)
                {
                    if (!string.IsNullOrWhiteSpace(input.EntryNumber) && await _loadFactorRepo.LoadFactors().CountAsync(a => !a.Id.Equals(input.Id) && a.SaipaPressLoadFactor.EntryNumber.Equals(input.EntryNumber)) >= 2)
                        return NotFound("شماره ورود تکراری است.");

                    if (!string.IsNullOrWhiteSpace(input.ExitNumber) && await _loadFactorRepo.LoadFactors().CountAsync(a => !a.Id.Equals(input.Id) && relatedContractIds.Contains(a.ContractId) && a.ExitNumber.Equals(input.ExitNumber)) >= 2)
                        return NotFound("شماره خروج تکراری است.");
                }

                if (await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPressLoadFactor).AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumber.Equals(input.LoadNumber) && a.SaipaPressLoadFactor != null))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                var fee = await _shippingFeeRouteRepository.GetWithGroup(input.ShippingFeeRouteId.Value);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.ShippingFeeGroup.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                var item = await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPressLoadFactor).Include(a => a.Contract).FirstAsync(a => a.Id.Equals(input.Id)); ;
                if (item == null) return NotFound();

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (item.AccountBookId != input.AccountBookId && await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                if (await _loadFactorRepo.SequenceExistInSaipaPress(item.Id, input.Sequence))
                    return NotFound("ترتیب وارد شده برای بارنامه تکراری است");

                var oldAccountBookId = item.AccountBookId;

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
                item.VehicleType = fee.ShippingFeeGroup.Vehicle;
                item.ShippingFeeRouteId = input.ShippingFeeRouteId;
                item.ShippingFeeGroupId = fee.ShippingFeeGroupId;
                item.Tonnage = input.Tonnage;
                item.AccountBookId = input.AccountBookId;
                item.IsFreeDriverPrice = input.IsFreeDriverPrice;

                item.SaipaPressLoadFactor.Sequence = input.Sequence;
                item.SaipaPressLoadFactor.EntryNumber = input.EntryNumber;
                item.SaipaPressLoadFactor.LoadType = input.LoadType;
                item.SaipaPressLoadFactor.PressFloorType = input.PressFloorType;

                //if (fee.ShippingFeeType == ShippingFeeType.Custom)
                //{
                //    item.Amount = input.Amount;
                //    if (!item.IsDriverFeeEditedByAdmin)
                //    {
                //        item.DriverFee = input.DriverFee;
                //        item.DriverTonnagePrice = input.DriverTonnagePrice;
                //    }
                //    item.TonnagePrice = input.TonnagePrice;
                //}
                //else
                //{
                //    item.Amount = fee.Price;
                //    if (!item.IsDriverFeeEditedByAdmin)
                //    {
                //        item.DriverFee = fee.DriverPrice;
                //        item.DriverTonnagePrice = fee.DriverTonnagePrice;
                //    }
                //    item.TonnagePrice = fee.TonnagePrice;
                //}

                item.Amount = fee.ShippingFeeGroup.Price;
                if (!item.IsDriverFeeEditedByAdmin)
                {
                    item.DriverFee = fee.ShippingFeeGroup.DriverPrice;
                    item.DriverTonnagePrice = fee.ShippingFeeGroup.DriverTonnagePrice;
                }
                item.TonnagePrice = fee.ShippingFeeGroup.TonnagePrice;

                _loadFactorRepo.Update(item);
                try
                {
                    await _loadFactorRepo.Save();

                    var balanceItem = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(item.Id)).FirstOrDefaultAsync();
                    if (balanceItem != null)
                    {
                        balanceItem.Amount = item.DriverFee + ((item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue) ? item.Tonnage.Value * item.DriverTonnagePrice.Value : 0);
                        balanceItem.CreateDateTime = item.Date;
                        balanceItem.EditDatetime = DateTime.Now;
                        balanceItem.VehicleId = item.VehicleId;
                        balanceItem.CustomerId = item.Contract.CustomerId;
                        balanceItem.CalendarId = item.CalendarId;

                        _vehicleBalanceRepository.Update(balanceItem);
                    }
                    else
                    {
                        await _vehicleBalanceRepository.Create(new VehicleBalance
                        {
                            Amount = item.DriverFee + ((item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue) ? item.Tonnage.Value * item.DriverTonnagePrice.Value : 0),
                            CalendarId = item.CalendarId,
                            VehicleId = item.VehicleId,
                            CreateDateTime = item.Date,
                            LoadFactorId = item.Id,
                            CustomerId = item.Contract.CustomerId
                        });
                    }

                    await _vehicleBalanceRepository.Save();

                    if (item.AccountBookId != oldAccountBookId)
                    {
                        await _accountBookRepository.UpdateAmount(item.AccountBookId);
                        await _accountBookRepository.UpdateAmount(oldAccountBookId);
                    }
                    else
                    {
                        await _accountBookRepository.UpdateAmount(item.AccountBookId);
                    }
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

        [HttpPost]
        public async Task<IActionResult> EditSazehGostarLoadFactor(ESazehGostarLoadFactorVM input)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov) && input.LoadFactorGovAmount is null)
                    return NotFound("کرایه بارنامه دولتی را وارد کنید.");

                if (input.LoadFactorGovRegistorId.HasValue && input.LoadFactorGovRegistorId.Value > 0 && input.GovYear < 1400)
                    return NotFound("تاریخ وارد شده برای بارنامه دولتی را بررسی نمائید.");

                if (await _loadFactorRepo.LoadFactors().Include(a => a.SazehGostarLoadFactor).AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumber.Equals(input.LoadNumber) && a.SazehGostarLoadFactor != null))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                if (await _loadFactorRepo.LoadFactors().Include(a => a.SazehGostarLoadFactor).AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.ExitNumber.Equals(input.ExitNumber) && a.SazehGostarLoadFactor != null))
                    return NotFound("شماره درخواست درج شده تکراری است.");

                var fee = await _shippingFeeRouteRepository.GetWithGroupAndLoadRoute(input.ShippingFeeRouteId.Value);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                if ((input.SazehLoadType == SazehGostarLoadType.OneWay && fee.ShippingFeeGroup.Title.Equals("رفت و برگشت")) ||
                    (input.SazehLoadType == SazehGostarLoadType.TwoWay && fee.ShippingFeeGroup.Title.Equals("رفت")))
                    return NotFound("نوع انتخابی بارنامه با نوع رفت و برگشت درج شده در نرخ انتخابی تطابق ندارد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.ShippingFeeGroup.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                var item = await _loadFactorRepo.LoadFactors().Include(a => a.SazehGostarLoadFactor).Include(a => a.Contract).FirstAsync(a => a.Id.Equals(input.Id)); ;
                if (item == null) return NotFound();

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (item.AccountBookId != input.AccountBookId && await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                //if (await _loadFactorRepo.SequenceExistInSazehGostar(item.Id, input.Sequence))
                //    return NotFound("ترتیب وارد شده برای بارنامه تکراری است");

                var oldAccountBookId = item.AccountBookId;

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
                item.VehicleType = fee.ShippingFeeGroup.Vehicle;
                item.ShippingFeeRouteId = input.ShippingFeeRouteId;
                item.ShippingFeeGroupId = fee.ShippingFeeGroupId;
                item.AccountBookId = input.AccountBookId;
                item.IsFreeDriverPrice = input.IsFreeDriverPrice;
                item.LoadFactorGovAmount = input.LoadFactorGovAmount;
                if (input.LoadFactorGovRegistorId.Value > 0)
                {
                    item.LoadFactorGovRegistorId = input.LoadFactorGovRegistorId.Value;
                    item.LoadFactorGovDate = new PersianDateTime(input.GovYear, input.GovMonth, input.GovDay, 0, 0, 0).ToDateTime();
                }

                //item.SazehGostarLoadFactor.Sequence = input.Sequence;
                item.SazehGostarLoadFactor.Certain = input.Certain;
                item.SazehGostarLoadFactor.Count = input.Count;
                item.SazehGostarLoadFactor.Description = $"حمل کالا از {fee.Origin.Title} به {fee.Destination.Title}{(input.SazehLoadType == SazehGostarLoadType.TwoWay ? " رفت و برگشت" : "")} ({fee.ShippingFeeGroup.Vehicle})";
                item.SazehGostarLoadFactor.DetailedCostCenter = input.DetailedCostCenter;
                item.SazehGostarLoadFactor.Nature = input.Nature;
                item.SazehGostarLoadFactor.RegisterCode = input.RegisterCode;
                item.SazehGostarLoadFactor.SazehLoadType = input.SazehLoadType;
                item.SazehGostarLoadFactor.InsuranceAmount = input.InsuranceAmount;

                //if (fee.ShippingFeeType == ShippingFeeType.Custom)
                //{
                //    item.Amount = input.Amount;
                //    if (!item.IsDriverFeeEditedByAdmin)
                //        item.DriverFee = input.DriverFee;
                //}
                //else
                //{
                //    item.Amount = fee.Price;
                //    if (!item.IsDriverFeeEditedByAdmin)
                //        item.DriverFee = fee.DriverPrice;
                //}
                item.Amount = fee.ShippingFeeGroup.Price;
                if (!item.IsDriverFeeEditedByAdmin)
                    item.DriverFee = fee.ShippingFeeGroup.DriverPrice;

                _loadFactorRepo.Update(item);
                try
                {
                    await _loadFactorRepo.Save();

                    var balanceItem = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(item.Id)).FirstOrDefaultAsync();
                    if (balanceItem != null)
                    {
                        balanceItem.Amount = item.DriverFee;
                        balanceItem.CreateDateTime = item.Date;
                        balanceItem.EditDatetime = DateTime.Now;
                        balanceItem.VehicleId = item.VehicleId;
                        balanceItem.CustomerId = item.Contract.CustomerId;
                        balanceItem.CalendarId = item.CalendarId;

                        _vehicleBalanceRepository.Update(balanceItem);
                    }
                    else
                    {
                        await _vehicleBalanceRepository.Create(new VehicleBalance
                        {
                            Amount = item.DriverFee,
                            CalendarId = item.CalendarId,
                            VehicleId = item.VehicleId,
                            CreateDateTime = item.Date,
                            LoadFactorId = item.Id,
                            CustomerId = item.Contract.CustomerId
                        });
                    }

                    await _vehicleBalanceRepository.Save();

                    if (item.AccountBookId != oldAccountBookId)
                    {
                        await _accountBookRepository.UpdateAmount(item.AccountBookId);
                        await _accountBookRepository.UpdateAmount(oldAccountBookId);
                    }
                    else
                    {
                        await _accountBookRepository.UpdateAmount(item.AccountBookId);
                    }
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

        [HttpPost]
        public async Task<IActionResult> EditMehrcomParsLoadFactor(EMehrcomParsLoadFactorVM input)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov) && input.LoadFactorGovAmount is null)
                    return NotFound("کرایه بارنامه دولتی را وارد کنید.");

                if (input.LoadFactorGovRegistorId.HasValue && input.LoadFactorGovRegistorId.Value > 0 && input.GovYear < 1400)
                    return NotFound("تاریخ وارد شده برای بارنامه دولتی را بررسی نمائید.");

                if (await _loadFactorRepo.LoadFactors().Include(a => a.MehrcomParsLoadFactor).AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumber.Equals(input.LoadNumber) && a.MehrcomParsLoadFactor != null))
                    return NotFound("شماره بارنامه درج شده تکراری است.");

                if (!string.IsNullOrWhiteSpace(input.LoadNumberGov))
                    if (await _loadFactorRepo.LoadFactors().AsNoTracking().AnyAsync(a => !a.Id.Equals(input.Id) && a.LoadNumberGov.Equals(input.LoadNumberGov)))
                        return NotFound("شماره بارنامه دولتی درج شده تکراری است.");

                var fee = await _shippingFeeRouteRepository.GetWithGroupAndLoadRoute(input.ShippingFeeRouteId.Value);
                if (fee == null) return NotFound("نرخ انتخابی شما پیدا نشد. ممکن است حذف شده باشد.");

                var vehicle = await _vehicleRepo.Get(input.VehicleId);
                if (!vehicle.Type.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].Equals(fee.ShippingFeeGroup.Vehicle.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]))
                    return NotFound("نوع خودرو نرخ انتخابی با خودرو انتخابی متفاوت است.");

                string loadType = fee.ShippingFeeGroup.ShippingFeeLoadType.Name;

                if ((!input.Load && !input.Palette && !input.Return) ||
                    (input.Load && input.Palette && !input.Return && !loadType.Contains("بار/پالت")) ||
                    (input.Load && !input.Palette && !input.Return && (!loadType.Contains("بار") || loadType.Contains("پالت") || loadType.Contains("برگشت"))) ||
                    (!input.Load && input.Palette && !input.Return && (!loadType.Contains("پالت") || loadType.Contains("بار") || loadType.Contains("برگشت"))) ||
                    (!input.Load && !input.Palette && input.Return && (!loadType.Contains("برگشت") || loadType.Contains("پالت") || loadType.Contains("بار"))) ||
                    (input.Load && !input.Palette && input.Return && !loadType.Contains("بار/برگشت")) ||
                    (!input.Load && input.Palette && input.Return && !loadType.Contains("پالت/برگشت"))
                    )
                    return NotFound("مقادیر بار/پالت/برگشت با نرخ انتخابی تناسب ندارد.");

                var item = await _loadFactorRepo.LoadFactors().Include(a => a.MehrcomParsLoadFactor).Include(a => a.Contract).FirstAsync(a => a.Id.Equals(input.Id)); ;
                if (item == null) return NotFound();

                var accountBookLoadFactorLimit = await _accountBookRepository.AccountBooks().AsNoTracking().Where(a => a.Id.Equals(input.AccountBookId)).Select(a => a.LoadFactorLimit).SingleAsync();
                if (item.AccountBookId != input.AccountBookId && await _loadFactorRepo.LoadFactors().CountAsync(a => a.AccountBookId.Equals(input.AccountBookId)) >= accountBookLoadFactorLimit)
                    return NotFound("صورت وضعیت / زونکن شما پر شده است. لطفا صورت وضعیت / زونکن دیگری را انتخاب کنید.");

                if (item.ContractId != input.ContractId)
                {
                    item.ContractId = input.ContractId;
                    item.Contract = await _contractRepo.Get(input.ContractId);
                }

                var oldAccountBookId = item.AccountBookId;

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
                item.VehicleType = fee.ShippingFeeGroup.Vehicle;
                item.ShippingFeeRouteId = input.ShippingFeeRouteId;
                item.ShippingFeeGroupId = fee.ShippingFeeGroupId;
                item.AccountBookId = input.AccountBookId;
                item.Tonnage = (input.TonnagePrice.HasValue && input.DriverTonnagePrice.HasValue) ? 1 : null;
                item.TonnagePrice = input.TonnagePrice;
                item.DriverTonnagePrice = input.DriverTonnagePrice;
                item.IsFreeDriverPrice = input.IsFreeDriverPrice;
                item.LoadFactorGovAmount = input.LoadFactorGovAmount;
                if (input.LoadFactorGovRegistorId.Value > 0)
                {
                    item.LoadFactorGovRegistorId = input.LoadFactorGovRegistorId.Value;
                    item.LoadFactorGovDate = new PersianDateTime(input.GovYear, input.GovMonth, input.GovDay, 0, 0, 0).ToDateTime();
                }

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

                //if (fee.ShippingFeeType == ShippingFeeType.Custom)
                //{
                //    item.Amount = input.HasAddonMessage ? Math.Floor(input.Amount + (input.Amount * 0.3)) : input.Amount;
                //    if (!item.IsDriverFeeEditedByAdmin)
                //    {
                //        item.DriverFee = input.HasAddonMessage ? Math.Floor(input.DriverFee + (input.DriverFee * 0.3)) : input.DriverFee;
                //    }
                //}
                //else
                //{
                //    item.Amount = input.HasAddonMessage ? Math.Floor(fee.Price + (fee.Price * 0.3)) : fee.Price;
                //    if (!item.IsDriverFeeEditedByAdmin)
                //    {
                //        item.DriverFee = input.HasAddonMessage ? Math.Floor(fee.DriverPrice + (fee.DriverPrice * 0.3)) : fee.DriverPrice;
                //    }
                //}

                item.Amount = input.HasAddonMessage ? Math.Floor(fee.ShippingFeeGroup.Price + (fee.ShippingFeeGroup.Price * 0.3)) : fee.ShippingFeeGroup.Price;
                if (!item.IsDriverFeeEditedByAdmin)
                {
                    item.DriverFee = input.HasAddonMessage ? Math.Floor(fee.ShippingFeeGroup.DriverPrice + (fee.ShippingFeeGroup.DriverPrice * 0.3)) : fee.ShippingFeeGroup.DriverPrice;
                }

                _loadFactorRepo.Update(item);
                try
                {
                    await _loadFactorRepo.Save();

                    var balanceItem = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(item.Id)).FirstOrDefaultAsync();
                    if (balanceItem != null)
                    {
                        balanceItem.Amount = item.DriverFee +
                    ((item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue) ? item.Tonnage.Value * item.DriverTonnagePrice.Value : 0) +
                    (item.WeighbridgePrice.HasValue ? item.WeighbridgePrice.Value : 0) +
                    (item.DriverLoadSleepPrice.HasValue ? item.DriverLoadSleepPrice.Value : 0);
                        balanceItem.CreateDateTime = item.Date;
                        balanceItem.EditDatetime = DateTime.Now;
                        balanceItem.VehicleId = item.VehicleId;
                        balanceItem.CustomerId = item.Contract.CustomerId;
                        balanceItem.CalendarId = item.CalendarId;

                        _vehicleBalanceRepository.Update(balanceItem);
                    }
                    else
                    {
                        await _vehicleBalanceRepository.Create(new VehicleBalance
                        {
                            Amount = item.DriverFee +
                    ((item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue) ? item.Tonnage.Value * item.DriverTonnagePrice.Value : 0) +
                    (item.WeighbridgePrice.HasValue ? item.WeighbridgePrice.Value : 0) +
                    (item.DriverLoadSleepPrice.HasValue ? item.DriverLoadSleepPrice.Value : 0),
                            CalendarId = item.CalendarId,
                            VehicleId = item.VehicleId,
                            CreateDateTime = item.Date,
                            LoadFactorId = item.Id,
                            CustomerId = item.Contract.CustomerId
                        });
                    }

                    await _vehicleBalanceRepository.Save();

                    if (item.AccountBookId != oldAccountBookId)
                    {
                        await _accountBookRepository.UpdateAmount(item.AccountBookId);
                        await _accountBookRepository.UpdateAmount(oldAccountBookId);
                    }
                    else
                    {
                        await _accountBookRepository.UpdateAmount(item.AccountBookId);
                    }
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

        [HttpPost]
        public async Task<JsonResult> GetShippingFeeJson(long contractId)
        {
            var query = _shippingFeeRepo.ShippingFees().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.ShippingFeeLoadType).Where(a => a.ContractId.Equals(contractId));
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
                Title = string.IsNullOrWhiteSpace(a.Title) ? "" : $"({a.Title})",
                LoadType = a.ShippingFeeLoadType.Name
            }).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLoadFactor(long id)
        {
            var item = await _loadFactorRepo.LoadFactors().Include(a => a.SaipaPressLoadFactor).Include(a => a.SaipaPressLoadFactor).Include(a => a.MehrcomParsLoadFactor).Include(a => a.Origin).Include(a => a.Destination).SingleOrDefaultAsync(a => a.Id.Equals(id));
            if (item == null) return NotFound();

            if (item.SazehGostarLoadFactor != null)
                _loadFactorRepo.DeleteSazehGostar(item.SazehGostarLoadFactor);

            if (item.SaipaPressLoadFactor != null)
                _loadFactorRepo.DeleteSaipaPress(item.SaipaPressLoadFactor);

            Log.Information($"بارنامه با شماره {item.LoadNumber} و مبدا {item.Origin.Title} و مقصد {item.Destination.Title} حذف شد.");

            _loadFactorRepo.Delete(item);
            try
            {
                var balanceItem = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(id)).FirstOrDefaultAsync();

                await _loadFactorRepo.Save();
                if (balanceItem != null)
                {
                    _vehicleBalanceRepository.Delete(balanceItem);
                    await _vehicleBalanceRepository.Save();
                }

                await _accountBookRepository.UpdateAmount(item.AccountBookId);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MoveLoadFactorsToNewContract(string contractRowId, string newContractRowId, string dateString)
        {
            var dateArr = dateString.PersianToEnglish().Split("/");
            var date = new PersianDateTime(Convert.ToInt32(dateArr[0]), Convert.ToInt32(dateArr[1]), Convert.ToInt32(dateArr[2])).ToDateTime();

            var newContract = await _contractRepo.Contracts().Include(a => a.ShippingFees).Include(a => a.ShippingFeeGroups).ThenInclude(a => a.ShippingFeeRoutes).FirstOrDefaultAsync(a => a.RowId.Equals(newContractRowId));
            var newShippingFees = newContract.ShippingFees;
            var newShippingFeeGroups = newContract.ShippingFeeGroups;
            var contract = await _contractRepo.Contracts().Include(a => a.ShippingFees).Include(a => a.ShippingFeeGroups).ThenInclude(a => a.ShippingFeeRoutes).FirstOrDefaultAsync(a => a.RowId.Equals(contractRowId));
            var shippingFees = contract.ShippingFees;
            var shippingFeeGroups = contract.ShippingFeeGroups;

            var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.ContractId.Equals(contract.Id) && a.Date >= date).ToListAsync();
            var vehicleBalances = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && loadFactors.Select(a => a.Id).Contains(a.LoadFactorId.Value)).ToListAsync();
            foreach (var item in loadFactors)
            {
                if (item.ShippingFeeId.HasValue)
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

                        var balance = vehicleBalances.Single(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(item.Id));
                        balance.Amount = item.DriverFee +
                        ((item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue) ? item.Tonnage.Value * item.DriverTonnagePrice.Value : 0) +
                        (item.WeighbridgePrice.HasValue ? item.WeighbridgePrice.Value : 0) +
                        (item.DriverLoadSleepPrice.HasValue ? item.DriverLoadSleepPrice.Value : 0);
                        balance.EditDatetime = DateTime.Now;
                    }
                }
                else
                {
                    var shippingFee = shippingFeeGroups.Single(a => a.Id.Equals(item.ShippingFeeGroupId.Value));
                    var newShippingFeeQuery = newShippingFeeGroups.Where(a => a.Vehicle.Equals(shippingFee.Vehicle)
                    && a.Origin.Equals(shippingFee.Origin) && a.Destination.Equals(shippingFee.Destination)
                    && a.ShippingFeeLoadTypeId.Equals(shippingFee.ShippingFeeLoadTypeId));

                    if (!string.IsNullOrWhiteSpace(shippingFee.Title))
                        newShippingFeeQuery = newShippingFeeQuery.Where(a => a.Title.Equals(shippingFee.Title));

                    var newShippingFee = newShippingFeeQuery.Single();

                    var shippingFeeRoute = shippingFee.ShippingFeeRoutes.Single(a => a.Id.Equals(item.ShippingFeeRouteId.Value));
                    var newShippingFeeRoutes = newShippingFee.ShippingFeeRoutes.Where(a => a.OriginId.Equals(shippingFeeRoute.OriginId) && a.DestinationId.Equals(shippingFeeRoute.DestinationId)).ToList();
                    if (!string.IsNullOrWhiteSpace(shippingFeeRoute.Title))
                    {
                        newShippingFeeRoutes = newShippingFeeRoutes.Where(a => a.Title.Equals(shippingFee.Title)).ToList();
                    }
                    var newShippingFeeRoute = newShippingFeeRoutes.First();

                    item.ContractId = newContract.Id;
                    item.ShippingFeeId = null;
                    item.ShippingFeeGroupId = newShippingFee.Id;
                    item.ShippingFeeRouteId = newShippingFeeRoute.Id;

                    item.Amount = newShippingFee.Price;
                    if (!item.IsDriverFeeEditedByAdmin)
                        item.DriverFee = newShippingFee.DriverPrice;

                    item.TonnagePrice = newShippingFee.TonnagePrice;
                    item.DriverTonnagePrice = newShippingFee.DriverTonnagePrice;

                    var balance = vehicleBalances.Single(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(item.Id));
                    balance.Amount = item.DriverFee +
                    ((item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue) ? item.Tonnage.Value * item.DriverTonnagePrice.Value : 0) +
                    (item.WeighbridgePrice.HasValue ? item.WeighbridgePrice.Value : 0) +
                    (item.DriverLoadSleepPrice.HasValue ? item.DriverLoadSleepPrice.Value : 0);
                    balance.EditDatetime = DateTime.Now;
                }
                _loadFactorRepo.Update(item);
            }

            try
            {
                await _loadFactorRepo.Save();
                await _vehicleBalanceRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditLoadFactorDriverFee(long Id, double Fee, double TonnageFee, bool IsFree)
        {
            string msg;
            string status = "danger";

            var item = await _loadFactorRepo.Get(Id);
            item.DriverFee = Fee;
            item.DriverTonnagePrice = TonnageFee > 0 ? TonnageFee : item.DriverTonnagePrice;
            item.IsFreeDriverPrice = IsFree;
            item.IsDriverFeeEditedByAdmin = true;
            item.EditDateTime = DateTime.Now;
            item.EditorId = _userManager.GetUserId(User);

            _loadFactorRepo.Update(item);
            try
            {
                await _loadFactorRepo.Save();

                var balanceItem = await _vehicleBalanceRepository.Query().Where(a => a.LoadFactorId.HasValue && a.LoadFactorId.Value.Equals(item.Id)).FirstOrDefaultAsync();

                balanceItem.Amount = item.DriverFee +
                    ((item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue) ? item.Tonnage.Value * item.DriverTonnagePrice.Value : 0) +
                    (item.WeighbridgePrice.HasValue ? item.WeighbridgePrice.Value : 0) +
                    (item.DriverLoadSleepPrice.HasValue ? item.DriverLoadSleepPrice.Value : 0);
                balanceItem.CreateDateTime = item.Date;
                balanceItem.EditDatetime = DateTime.Now;

                _vehicleBalanceRepository.Update(balanceItem);

                await _vehicleBalanceRepository.Save();

                msg = "عملیات موفقیت آمیز بود.";
                status = "success";
            }
            catch (Exception e)
            {
                msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
            }
            return Json(new { msg, status, fee = Fee.ToString("N0") });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> EditLoadFactorAmount(long Id, double Amount)
        {
            string msg;
            string status = "danger";

            var item = await _loadFactorRepo.Get(Id);
            item.Amount = Amount;
            item.IsDriverFeeEditedByAdmin = true;
            item.EditDateTime = DateTime.Now;
            item.EditorId = _userManager.GetUserId(User);
            _loadFactorRepo.Update(item);
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
            return Json(new { msg, status, amount = Amount.ToString("N0") });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> ShippingFeeRouteLoadFactors(long id)
        {
            //var shippingFee = await _shippingFeeRepo.ShippingFees().Include(a => a.Contract).Include(a => a.Origin).Include(a => a.Destination).SingleOrDefaultAsync(a => a.Id.Equals(id));
            //ViewData["ShippingFee"] = shippingFee;

            //return View(await _loadFactorRepo.LoadFactors().Where(a => a.ShippingFeeId.Equals(id))
            //    .Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Driver).Include(a => a.Vehicle).Include(a => a.Contract).ThenInclude(a => a.Customer)
            //    .OrderBy(a => a.Vehicle.Type).ThenBy(a => a.Date).ThenBy(a => a.LoadNumber).ToListAsync());

            var shippingFeeRoute = await _shippingFeeRouteRepository.Query()
                .Include(a => a.ShippingFeeGroup).ThenInclude(a => a.Contract)
                .Include(a => a.Origin).Include(a => a.Destination).FirstOrDefaultAsync(a => a.Id.Equals(id));
            ViewData["ShippingFeeRoute"] = shippingFeeRoute;

            return View(await _loadFactorRepo.LoadFactors().Where(a => a.ShippingFeeRouteId.Equals(id))
                .Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Driver).Include(a => a.Vehicle).Include(a => a.Contract).ThenInclude(a => a.Customer)
                .OrderBy(a => a.Vehicle.Type).ThenBy(a => a.Date).ThenBy(a => a.LoadNumber).ToListAsync());
        }
        #endregion

        #region LoadFactorNovin
        [HttpGet]
        public async Task<IActionResult> LoadFactorNovin(int? p)
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

            var query = _loadFactorNovinRepository.Query().Include(a => a.Vehicle).Include(a => a.Driver).Include(a => a.Customer).AsQueryable();

            ViewBag.data = await query.OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 15);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadFactorNovin_Search(int? p, string loadNumber, string vehicleNumber, long? calendar, long? customer, long? isReceived, long? isPaied)
        {
            var pageNumber = p ?? 1;

            var query = _loadFactorNovinRepository.Query().Include(a => a.Vehicle).Include(a => a.Driver).Include(a => a.Customer).AsQueryable();

            if (!string.IsNullOrWhiteSpace(loadNumber))
                query = query.Where(a => a.LoadNumber.Contains(loadNumber) || a.LoadNumberGov.Contains(loadNumber));
            if (!string.IsNullOrWhiteSpace(vehicleNumber))
                query = query.Where(a => vehicleNumber == (a.Vehicle.LeftNumber + " " + a.Vehicle.NumberWord + " " + a.Vehicle.RightNumber));
            if (calendar.HasValue && calendar.Value > 0)
                query = query.Where(a => a.CalendarId.Equals(calendar.Value));
            if (customer.HasValue && customer.Value > 0)
                query = query.Where(a => a.CustomerId.Equals(customer.Value));
            if (isReceived.HasValue && isReceived.Value > 0)
            {
                bool flag = false;
                if (isReceived.Value == 1) flag = true;
                query = query.Where(a => a.IsReceived.Equals(flag));
            }
            if (isPaied.HasValue && isPaied.Value > 0)
            {
                bool flag = false;
                if (isPaied.Value == 1) flag = true;
                query = query.Where(a => a.IsPaied.Equals(flag));
            }

            ViewBag.page = p;
            ViewBag.loadNumber = loadNumber;
            ViewBag.vehicleNumber = vehicleNumber;
            ViewBag.calendar = calendar;
            ViewBag.customer = customer;
            ViewBag.isReceived = isReceived;
            ViewBag.isPaied = isPaied;


            ViewBag.data = await query.OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 15);
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> CreateLoadFactorNovin()
        {
            ViewData["Drivers"] = await _driverRepository.Drivers().AsNoTracking().Where(a => a.IsActive).OrderBy(a => a.Fullname).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepo.Vehicles().AsNoTracking().Where(a => a.Status).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["Customers"] = await _customerRepo.GetAllActive();

            return PartialView("~/Views/Admin/Create/LoadFactorNovin.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateLoadFactorNovin(CreateLoadFactorNovinVM v)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                string fileNames = "";
                var files = Request.Form.Files;
                if (files != null)
                {
                    foreach (var pic in files)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\novin")))
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\novin"));

                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\novin", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            fileNames += (files.Count == 1 || pic == files.Last()) ? fileName : $"{fileName};;";
                        }
                        else
                        {
                            msg = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                            return Json(new { msg, status });
                        }
                    }
                }

                var item = new LoadFactorNovin
                {
                    Date = new PersianDateTime(v.Year, v.Month, v.Day).ToDateTime(),
                    PaymentDate = (v.PYear > 0 && v.PMonth > 0 && v.PDay > 0) ? new PersianDateTime(v.PYear, v.PMonth, v.PDay).ToDateTime() : null,
                    ReceiveDate = (v.RYear > 0 && v.RMonth > 0 && v.RDay > 0) ? new PersianDateTime(v.RYear, v.RMonth, v.RDay).ToDateTime() : null,
                    IsPaied = v.IsPaied,
                    IsReceived = v.IsReceived,
                    CreateDateTime = DateTime.Now,
                    CreatorId = _userManager.GetUserId(User),
                    CustomerId = v.CustomerId,
                    Amount = v.Amount,
                    ApplicantName = v.ApplicantName,
                    CalendarId = v.CalendarId,
                    Destination = v.Destination,
                    DriverId = v.DriverId,
                    DriverFee = v.DriverFee,
                    DriverTonnagePrice = v.DriverTonnagePrice,
                    LoadNumber = v.LoadNumber,
                    LoadNumberGov = v.LoadNumberGov,
                    Origin = v.Origin,
                    Tonnage = v.Tonnage,
                    TonnagePrice = v.TonnagePrice,
                    VehicleId = v.VehicleId,
                    Code = v.Code,
                    Attachments = fileNames
                };

                _loadFactorNovinRepository.Create(item);
                try
                {
                    await _loadFactorNovinRepository.Save();
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
        public async Task<PartialViewResult> EditLoadFactorNovin(long id)
        {
            var data = await _loadFactorNovinRepository.GetEditData(id);

            ViewData["Drivers"] = await _driverRepository.Drivers().AsNoTracking().Where(a => a.IsActive).OrderBy(a => a.Fullname).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepo.Vehicles().AsNoTracking().Where(a => a.Status).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["Customers"] = await _customerRepo.GetAllActive();

            return PartialView("~/Views/Admin/Edit/LoadFactorNovin.cshtml", data);
        }

        [HttpPost]
        public async Task<IActionResult> EditLoadFactorNovin(EditLoadFactorNovinVM v)
        {
            if (ModelState.IsValid)
            {
                var item = await _loadFactorNovinRepository.Get(v.Id);
                item.Code = v.Code;
                item.Amount = v.Amount;
                item.LoadNumber = v.LoadNumber;
                item.ApplicantName = v.ApplicantName;
                item.TonnagePrice = v.TonnagePrice;
                item.DriverTonnagePrice = v.DriverTonnagePrice;
                item.DriverId = v.DriverId;
                item.CalendarId = v.CalendarId;
                item.CustomerId = v.CustomerId;
                item.Destination = v.Destination;
                item.DriverFee = v.DriverFee;
                item.EditDateTime = DateTime.Now;
                item.EditorId = _userManager.GetUserId(User);
                item.LoadNumberGov = v.LoadNumberGov;
                item.Origin = v.Origin;
                item.Tonnage = v.Tonnage;
                item.VehicleId = v.VehicleId;
                item.IsReceived = v.IsReceived;
                item.IsPaied = v.IsPaied;

                item.Date = new PersianDateTime(v.Year, v.Month, v.Day).ToDateTime();

                item.PaymentDate = (v.PYear > 0 && v.PMonth > 0 && v.PDay > 0) ? new PersianDateTime(v.PYear, v.PMonth, v.PDay).ToDateTime() : null;
                item.ReceiveDate = (v.RYear > 0 && v.RMonth > 0 && v.RDay > 0) ? new PersianDateTime(v.RYear, v.RMonth, v.RDay).ToDateTime() : null;

                string fileNames = "";
                var files = Request.Form.Files;
                if (files != null)
                {
                    foreach (var pic in files)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\novin", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            fileNames += (files.Count == 1 || pic == files.Last()) ? fileName : $"{fileName};;";
                        }
                        else
                        {
                            TempData["msg"] = "لطفا از فرمت jpg  یا png استفاده کنید |success";
                            return Redirect(Request.Headers["Referer"].ToString());
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Attachments))
                    {
                        foreach (var pic in item.Attachments.Split(";;", StringSplitOptions.RemoveEmptyEntries))
                        {
                            try
                            {
                                System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\novin", pic));
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                }

                _loadFactorNovinRepository.Update(item);
                try
                {
                    await _loadFactorNovinRepository.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            else
            {
                TempData["msg"] = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید. |success";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLoadFactorNovin(long id)
        {
            var item = await _loadFactorNovinRepository.Get(id);
            if (item is not null)
            {
                if (!string.IsNullOrEmpty(item.Attachments))
                {
                    foreach (var pic in item.Attachments.Split(";;", StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\novin", pic));
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                _loadFactorNovinRepository.Delete(item);
                try
                {
                    await _loadFactorNovinRepository.Save();
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

        #region AccountBookLoadFactor
        public async Task<IActionResult> AccountBookLoadFactor(string id)
        {
            var accountBook = await _accountBookRepository.AccountBooks().AsNoTracking().SingleOrDefaultAsync(a => a.RowId.Equals(id));
            ViewData["AccountBook"] = accountBook;

            return View(await _loadFactorRepo.LoadFactors().Where(a => a.AccountBookId.Equals(accountBook.Id))
                .Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Driver).Include(a => a.Vehicle).Include(a => a.Contract).ThenInclude(a => a.Customer)
                .OrderBy(a => a.Vehicle.Type).ThenBy(a => a.Date).ThenBy(a => a.LoadNumber).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> MoveAccountBookLoadFactor(string accountBookRowId, long[] idList)
        {
            if (idList.Length > 0)
            {
                var accountBookId = await _accountBookRepository.AccountBooks().Where(a => a.RowId.Equals(accountBookRowId)).Select(a => a.Id).SingleOrDefaultAsync();
                var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => idList.Contains(a.Id)).ToListAsync();
                var oldAccountBookId = loadFactors.First().AccountBookId;

                foreach (var item in loadFactors)
                {
                    item.AccountBookId = accountBookId;
                }

                try
                {
                    await _loadFactorRepo.Save();

                    await _accountBookRepository.UpdateAmount(accountBookId);
                    await _accountBookRepository.UpdateAmount(oldAccountBookId);
                    await _accountBookRepository.Save();

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
                item.RealStatus = v.RealStatus;

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
            var query = _accountBookRepository.AccountBooks().Include(a => a.Customer).Include(a => a.Calendar).AsQueryable();
            if (!User.IsInRole("Admin") && !User.IsInRole("Milad"))
                query = query.Where(a => a.CreatorId.Equals(_userManager.GetUserId(User)));

            var onePageOfData = await query.OrderByDescending(a => a.Number).ToPagedListAsync(pageNumber, 20);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchAccountBook(int? p, string param)
        {
            var pageNum = p ?? 1;
            var query = _accountBookRepository.AccountBooks().Include(a => a.Customer).Include(a => a.Calendar).Where(a => a.Number.Contains(param) || a.FactorNumber.Contains(param));
            if (!User.IsInRole("Admin") && !User.IsInRole("Milad"))
                query = query.Where(a => a.CreatorId.Equals(_userManager.GetUserId(User)));

            var onePageOfData = await query.OrderByDescending(a => a.Number).ToPagedListAsync(pageNum, 15);
            ViewBag.data = onePageOfData;
            ViewBag.param = param;

            return PartialView("_AccountBook");
        }

        [HttpGet]
        public async Task<IActionResult> CreateAccountBook()
        {
            ViewData["Customers"] = await _customerRepo.Customers().AsNoTracking().OrderBy(a => a.Name).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
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
                    CalendarId = c.CalendarId,
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
            var item = await _accountBookRepository.Get(id);

            ViewData["Customers"] = await _customerRepo.Customers().AsNoTracking().OrderBy(a => a.Name).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();

            return PartialView("~/Views/Admin/Edit/AccountBook.cshtml", new EditAccountBookVM
            {
                Id = item.Id,
                CustomerId = item.CustomerId,
                FactorNumber = item.FactorNumber,
                Number = item.Number,
                LoadFactorLimit = item.LoadFactorLimit,
                CalendarId = item.CalendarId
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
                item.CalendarId = c.CalendarId;
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
            var query = _accountBookRepository.AccountBooks().Include(a => a.Customer).Where(a => a.IsOpen);
            if (User.IsInRole("RegisterUser"))
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(a => a.CreatorId.Equals(userId));
            }
            return Json(await query.Select(a => new { a.RowId, a.Number, Customer = a.Customer.Name }).ToListAsync());
        }
        #endregion

        #region CustomerFactor
        [HttpGet]
        public async Task<IActionResult> CustomerFactor(int? p)
        {
            var pageNumber = p ?? 1;
            ViewBag.data = await _customerFactorRepository.Query().Include(a => a.Calendar).Include(a => a.Customer).OrderByDescending(a => a.FactorNumber).ToPagedListAsync(pageNumber, 20);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerFactorPartial(int? p)
        {
            var pageNumber = p ?? 1;
            ViewBag.data = await _customerFactorRepository.Query().Include(a => a.Calendar).Include(a => a.Customer).OrderByDescending(a => a.FactorNumber).ToPagedListAsync(pageNumber, 20);
            return PartialView("_CustomerFactor");
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerSearchModalData()
        {
            var customers = await _customerRepo.GetAll();
            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.Sequence).ToListAsync();

            return Json(new { customers, calendars });
        }

        [HttpPost]
        public async Task<IActionResult> CustomerFactor_Search(int? p, long customerId, long calendarId)
        {
            var pageNum = p ?? 1;
            var query = _customerFactorRepository.Query().Include(a => a.Calendar).Include(a => a.Customer).AsNoTracking();

            if (customerId > 0)
                query = query.Where(a => a.CustomerId.Equals(customerId));

            if (calendarId > 0)
                query = query.Where(a => a.CalendarId.Equals(calendarId));

            var onePageOfData = await query.OrderByDescending(a => a.FactorNumber).ToPagedListAsync(pageNum, 15);
            ViewBag.data = onePageOfData;
            ViewBag.customerId = customerId;
            ViewBag.calendarId = calendarId;

            return PartialView("_CustomerFactor");
        }

        [HttpGet]
        public async Task<IActionResult> CreateCustomerFactor()
        {
            ViewData["Calendars"] = await _calendarRepo.Calendars().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["Customers"] = await _customerRepo.GetAll();
            return PartialView("~/Views/Admin/Create/CustomerFactor.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerFactor(CustomerFactor c, int day, int month, int year)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                if (await _customerFactorRepository.Query().AnyAsync(a => a.FactorNumber.Equals(c.FactorNumber)))
                {
                    msg = "شماره فاکتور وارد شده تکراری است.";
                    return Json(new { msg, status });
                }

                c.Date = new PersianDateTime(year, month, day).ToDateTime();
                c.CreatorId = _userManager.GetUserId(User);
                //c.CustomerId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.Id.Equals(c.ContractId)).Select(a => a.CustomerId).FirstAsync();
                _customerFactorRepository.Create(c);
                try
                {
                    await _customerFactorRepository.Save();
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
        public async Task<PartialViewResult> EditCustomerFactor(long id)
        {
            var item = await _customerFactorRepository.Get(id);

            ViewData["Calendars"] = await _calendarRepo.Calendars().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["Customers"] = await _customerRepo.GetAll();

            return PartialView("~/Views/Admin/Edit/CustomerFactor.cshtml", item);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomerFactor(CustomerFactor c, int day, int month, int year)
        {
            if (ModelState.IsValid)
            {
                if (await _customerFactorRepository.Query().AnyAsync(a => !a.Id.Equals(c.Id) && a.FactorNumber.Equals(c.FactorNumber)))
                {
                    TempData["msg"] = "شماره صورت وضعیت تکراری است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _customerFactorRepository.Get(c.Id);
                item.FactorNumber = c.FactorNumber;
                item.Amount = c.Amount;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDatetime = DateTime.Now;
                item.CalendarId = c.CalendarId;
                item.CustomerId = c.CustomerId;
                //item.CustomerId = await _contractRepo.Contracts().AsNoTracking().Where(a => a.Id.Equals(c.ContractId)).Select(a => a.CustomerId).FirstAsync();
                item.Date = new PersianDateTime(year, month, day).ToDateTime();

                _customerFactorRepository.Update(item);
                try
                {
                    await _customerFactorRepository.Save();
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
        public async Task<IActionResult> DeleteCustomerFactor(string id)
        {
            var item = await _customerFactorRepository.Get(id);

            _customerFactorRepository.Delete(item);
            try
            {
                await _customerFactorRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }

            return Redirect(Request.Headers["Referer"].ToString());
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
                item.Tonnage = v.Tonnage;
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
        public async Task<IActionResult> TurnoverProfile(TurnoverType type)
        {
            ViewData["Type"] = type;
            return View(await _turnoverProfileRepository.Query().Include(a => a.Customer).Where(a => a.TurnoverType == type).OrderBy(a => a.Customer.Name).ThenBy(a => a.FullName).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<PartialViewResult> CreateTurnoverProfile()
        {
            ViewData["Customer"] = await _customerRepo.GetAll();
            return PartialView("~/Views/Admin/Create/TurnoverProfile.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTurnoverProfile(CreateTurnoverProfileVM v)
        {
            if (ModelState.IsValid)
            {
                var startDate = new PersianDateTime(v.StartYear, v.StartMonth, v.StartDay);
                var expireDate = new PersianDateTime(v.ExpireYear, v.ExpireMonth, v.ExpireDay);

                if (expireDate < startDate)
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. لطفا تاریخ انقضا را بزرگتر از تاریخ شروع وارد کنید. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = new TurnoverProfile
                {
                    BankAccount = v.BankAccount,
                    BankAccountOwner = v.BankAccountOwner,
                    CustomerId = v.CustomerId.Value == 0 ? null : v.CustomerId.Value,
                    ExpireDate = expireDate.ToDateTime(),
                    StartDate = startDate.ToDateTime(),
                    FullName = v.FullName,
                    ProfitPercent = v.ProfitPercent,
                    TurnoverType = v.TurnoverType,
                    TurnoverPaymentType = v.TurnoverPaymentType,
                    TurnoverTurnType = v.TurnoverTurnType,
                    Description = v.Description
                };

                _turnoverProfileRepository.Create(item);
                try
                {
                    await _turnoverProfileRepository.Save();
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
        public async Task<PartialViewResult> EditTurnoverProfile(int id)
        {
            ViewData["Customer"] = await _customerRepo.GetAll();
            var item = await _turnoverProfileRepository.Get(id);
            var startDate = new PersianDateTime(item.StartDate);
            var expireDate = new PersianDateTime(item.ExpireDate);

            return PartialView("~/Views/Admin/Edit/TurnoverProfile.cshtml", new EditTurnoverProfileVM
            {
                Id = item.Id,
                BankAccount = item.BankAccount,
                BankAccountOwner = item.BankAccountOwner,
                CustomerId = item.CustomerId ?? 0,
                ExpireDay = expireDate.Day,
                ExpireMonth = expireDate.Month,
                ExpireYear = expireDate.Year,
                StartDay = startDate.Day,
                StartMonth = startDate.Month,
                StartYear = startDate.Year,
                FullName = item.FullName,
                ProfitPercent = item.ProfitPercent,
                TurnoverType = item.TurnoverType,
                TurnoverPaymentType = item.TurnoverPaymentType,
                TurnoverTurnType = item.TurnoverTurnType,
                Description = item.Description
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTurnoverProfile(EditTurnoverProfileVM v)
        {
            if (ModelState.IsValid)
            {
                var startDate = new PersianDateTime(v.StartYear, v.StartMonth, v.StartDay);
                var expireDate = new PersianDateTime(v.ExpireYear, v.ExpireMonth, v.ExpireDay);

                if (expireDate < startDate)
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. لطفا تاریخ انقضا را بزرگتر از تاریخ شروع وارد کنید. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _turnoverProfileRepository.Get(v.Id);

                item.TurnoverType = v.TurnoverType;
                item.FullName = v.FullName;
                item.ProfitPercent = v.ProfitPercent;
                item.StartDate = startDate;
                item.ExpireDate = expireDate;
                item.BankAccount = v.BankAccount;
                item.BankAccountOwner = v.BankAccountOwner;
                item.CustomerId = v.CustomerId.Value == 0 ? null : v.CustomerId.Value;
                item.TurnoverPaymentType = v.TurnoverPaymentType;
                item.TurnoverTurnType = v.TurnoverTurnType;
                item.Description = v.Description;

                _turnoverProfileRepository.Update(item);
                try
                {
                    await _turnoverProfileRepository.Save();
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
        public async Task<IActionResult> DeleteTurnoverProfile(long id)
        {
            var item = await _turnoverProfileRepository.Query().Include(a => a.Turnovers).Include(a => a.TurnoverProfilePeriods).Where(a => a.Id.Equals(id)).FirstOrDefaultAsync();
            if (item is not null)
            {
                _turnoverProfileRepository.Delete(item);
                try
                {
                    await _turnoverProfileRepository.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                }
                catch (Exception e)
                {
                    TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Turnover(string id, int? p)
        {
            var profile = await _turnoverProfileRepository.Query().Where(a => a.RowId.Equals(id)).SingleOrDefaultAsync();
            ViewData["Profile"] = profile;

            var pageNumber = p ?? 1;
            ViewBag.data = await _turnoverRepository.Query().Where(a => a.TurnoverProfileId.Equals(profile.Id))
            .OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 15);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Turnover_Search(string id, int? p)
        {
            var profile = await _turnoverProfileRepository.Query().Where(a => a.RowId.Equals(id)).SingleOrDefaultAsync();
            ViewData["Profile"] = profile;

            var pageNumber = p ?? 1;
            ViewBag.data = await _turnoverRepository.Query().Where(a => a.TurnoverProfileId.Equals(profile.Id))
            .OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 15);
            return PartialView();
        }

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> GetTurnoverFullnames()
        //{
        //    return Json(await _turnoverRepository.Query().AsNoTracking().Select(a => a.FullName).Distinct().ToListAsync());
        //}

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateTurnover()
        {
            return PartialView("~/Views/Admin/Create/Turnover.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTurnover(CreateTurnoverVM v)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                string fileNames = "";
                var files = Request.Form.Files;
                if (files != null)
                {
                    foreach (var pic in files)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "img\\turnover")))
                                Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "img\\turnover"));

                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\turnover", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            fileNames += (files.Count == 1 || pic == files.Last()) ? fileName : $"{fileName};;";
                        }
                        else
                        {
                            msg = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                            return Json(new { msg, status });
                        }
                    }
                }

                var item = new Turnover
                {
                    Date = new PersianDateTime(v.Year, v.Month, v.Day).ToDateTime(),
                    CreateDatetime = DateTime.Now,
                    CreatorId = _userManager.GetUserId(User),
                    Creditor = v.Creditor ?? 0,
                    Debtor = v.Debtor ?? 0,
                    Description = v.Description,
                    TurnoverProfileId = v.TurnoverProfileId,
                    Attachments = fileNames
                };

                _turnoverRepository.Create(item);
                try
                {
                    await _turnoverRepository.Save();
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
        public async Task<PartialViewResult> EditTurnover(long id)
        {
            var data = await _turnoverRepository.GetEditData(id);

            return PartialView("~/Views/Admin/Edit/Turnover.cshtml", data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTurnover(EditTurnoverVM v)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                var item = await _turnoverRepository.Get(v.Id);
                item.Creditor = v.Creditor ?? 0;
                item.Date = new PersianDateTime(v.Year, v.Month, v.Day).ToDateTime();
                item.Debtor = v.Debtor ?? 0;
                item.Description = v.Description;
                item.EditorId = _userManager.GetUserId(User);
                item.EditDatetime = DateTime.Now;
                item.TurnoverProfileId = v.TurnoverProfileId;

                string fileNames = "";
                var files = Request.Form.Files;
                if (files != null)
                {
                    foreach (var pic in files)
                    {
                        if (pic.ContentType == "image/jpeg" || pic.ContentType == "image/png")
                        {
                            var fileName = Path.GetRandomFileName() + Path.GetExtension(pic.FileName).ToLower();
                            var path = Path.Combine(_environment.WebRootPath, "img\\turnover", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await pic.CopyToAsync(stream);
                            }

                            fileNames += (files.Count == 1 || pic == files.Last()) ? fileName : $"{fileName};;";
                        }
                        else
                        {
                            msg = "لطفا از فرمت jpg  یا png استفاده کنید |danger";
                            return Json(new { msg, status });
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Attachments))
                    {
                        foreach (var pic in item.Attachments.Split(";;", StringSplitOptions.RemoveEmptyEntries))
                        {
                            try
                            {
                                System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\turnover", pic));
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                }

                _turnoverRepository.Update(item);
                try
                {
                    await _turnoverRepository.Save();
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTurnover(long id)
        {
            var item = await _turnoverRepository.Get(id);
            if (item is not null)
            {
                if (!string.IsNullOrEmpty(item.Attachments))
                {
                    foreach (var pic in item.Attachments.Split(";;", StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "img\\turnover", pic));
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                _turnoverRepository.Delete(item);
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
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region TurnoverProfilePeriod
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TurnoverProfilePeriod(long id)
        {
            ViewData["TurnoverProfileInfo"] = await _turnoverProfileRepository.Get(id);
            return View(await _turnoverProfilePeriodRepository.Query().AsNoTracking().Where(a => a.TurnoverProfileId.Equals(id)).OrderByDescending(a => a.EndDate).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateTurnoverProfilePeriod(long id)
        {
            ViewData["TurnoverProfileId"] = id;
            return PartialView("~/Views/Admin/Create/TurnoverProfilePeriod.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTurnoverProfilePeriod(CreateTurnoverProfilePeriodVM c)
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

                _turnoverProfilePeriodRepository.Create(new TurnoverProfilePeriod
                {
                    Title = c.Title,
                    TurnoverProfileId = c.TurnoverProfileId,
                    StartDate = startDate,
                    EndDate = endDate
                });
                try
                {
                    await _turnoverProfilePeriodRepository.Save();
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
        public async Task<PartialViewResult> EditTurnoverProfilePeriod(int id)
        {
            var item = await _turnoverProfilePeriodRepository.Get(id);
            var persianStartDate = new PersianDateTime(item.StartDate);
            var persianEndDate = new PersianDateTime(item.EndDate);

            return PartialView("~/Views/Admin/Edit/TurnoverProfilePeriod.cshtml", new EditTurnoverProfilePeriodVM
            {
                EndDay = persianEndDate.Day,
                EndMonth = persianEndDate.Month,
                EndYear = persianEndDate.Year,
                Id = item.Id,
                StartDay = persianStartDate.Day,
                StartMonth = persianStartDate.Month,
                StartYear = persianStartDate.Year,
                Title = item.Title,
                TurnoverProfileId = item.TurnoverProfileId
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTurnoverProfilePeriod(EditTurnoverProfilePeriodVM c)
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

                var item = await _turnoverProfilePeriodRepository.Get(c.Id);
                item.StartDate = startDate;
                item.EndDate = endDate;
                item.Title = c.Title;

                _turnoverProfilePeriodRepository.Update(item);
                try
                {
                    await _turnoverProfilePeriodRepository.Save();
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
        public async Task<IActionResult> DeleteTurnoverProfilePeriod(int id)
        {
            var item = await _turnoverProfilePeriodRepository.Get(id);
            if (item == null) return NotFound();

            _turnoverProfilePeriodRepository.Delete(item);
            try
            {
                await _turnoverProfilePeriodRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Bill
        [HttpGet]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> Bill(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _billRepository.Query().Include(a => a.Vehicle).Include(a => a.Customer).Include(a => a.Calendar)
                .OrderByDescending(a => a.Date).ThenByDescending(a => a.BillNo).ToPagedListAsync(pageNumber, 50);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> Bill_Search()
        {
            var definitions = await _definitionRepo.Definitions().Where(a => a.DefinitionType == DefinitionType.BillType || a.DefinitionType == DefinitionType.BankBranch).AsNoTracking()
                .Select(a => new
                {
                    a.DefinitionType,
                    a.Title
                }).OrderBy(a => a.Title).ToListAsync();

            var vehicles = await _vehicleRepo.Vehicles().Where(a => a.Status).AsNoTracking()
                .Select(a => new
                {
                    id = a.Id,
                    a.RightNumber,
                    number = $"ایران {a.IranStateNumber} - {a.RightNumber} {a.NumberWord} {a.LeftNumber}",
                    owner = a.VehicleOwnerFullname
                }).OrderBy(a => a.RightNumber).ToListAsync();

            return Json(new
            {
                vehicles,
                customers = await _customerRepo.Customers().AsNoTracking().Select(a => new { id = a.Id, name = a.Name }).OrderBy(a => a.name).ToListAsync(),
                calendars = await _calendarRepo.Calendars().AsNoTracking().Select(a => new { id = a.Id, title = a.Title }).OrderByDescending(a => a.id).ToListAsync(),
                names = await _billRepository.Query().AsNoTracking().Select(a => a.ReceiverName).Distinct().OrderBy(a => a).ToListAsync(),
                bankBranches = definitions.Where(a => a.DefinitionType == DefinitionType.BankBranch).Select(a => a.Title).ToList(),
                billTypes = definitions.Where(a => a.DefinitionType == DefinitionType.BillType).Select(a => a.Title).ToList()
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> Bill_Search(int? p, long customerId, long vehicleId, long calendarId, string name, string bankBranch, string billType, string description, string bankBillNo, string billNo, int realVehicle = -1)
        {
            var pageNumber = p ?? 1;
            var query = _billRepository.Query().Include(a => a.Vehicle).Include(a => a.Customer).Include(a => a.Calendar).AsQueryable();

            if (realVehicle == 0)
            {
                var fakeVehicles = await _vehicleRepo.Vehicles().Where(a => !a.RealStatus).Select(a => a.Id).ToListAsync();
                query = query.Where(a => a.VehicleId.HasValue && fakeVehicles.Contains(a.VehicleId.Value));
            }

            if (name != "all" && name != null)
                query = query.Where(a => a.ReceiverName.Contains(name));
            if (bankBranch != "all" && bankBranch != null)
                query = query.Where(a => a.BankBranch.Equals(bankBranch));
            if (billType != "all" && billType != null)
                query = query.Where(a => a.BillType.Equals(billType));
            if (!string.IsNullOrWhiteSpace(description) && description != null)
                query = query.Where(a => a.Description.Contains(description));
            if (!string.IsNullOrWhiteSpace(bankBillNo) && bankBillNo != null)
                query = query.Where(a => a.BankBillNo.Contains(bankBillNo));
            if (!string.IsNullOrWhiteSpace(billNo) && billNo != null)
                query = query.Where(a => a.BillNo.Contains(billNo));
            if (calendarId > 0)
                query = query.Where(a => a.CalendarId.Equals(calendarId));
            if (vehicleId > 0)
                query = query.Where(a => a.VehicleId.HasValue && a.VehicleId.Equals(vehicleId));
            if (customerId > 0)
                query = query.Where(a => a.CustomerId.HasValue && a.CustomerId.Equals(customerId));


            if (calendarId > 0)
            {
                var queryCount = await query.CountAsync();
                ViewBag.data = await query.OrderBy(a => a.Date).ThenByDescending(a => a.BillNo).ThenByDescending(a => a.Id).ToPagedListAsync(pageNumber, queryCount == 0 ? 1 : queryCount);
            }
            else
                ViewBag.data = await query.OrderByDescending(a => a.Date).ThenByDescending(a => a.BillNo).ThenByDescending(a => a.Id).ToPagedListAsync(pageNumber, 100);

            ViewBag.Name = name;
            ViewBag.BillType = billType;
            ViewBag.CustomerId = customerId;
            ViewBag.VehicleId = vehicleId;
            ViewBag.BankBranch = bankBranch;
            ViewBag.CalendarId = calendarId;
            ViewBag.RealVehicle = realVehicle;
            ViewBag.BillNo = billNo;
            ViewBag.BankBillNo = bankBillNo;
            ViewBag.Description = description;

            return PartialView();
        }

        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> Bill_Print(string id, string type)
        {
            var item = await _billRepository.Query().AsNoTracking().Include(a => a.Vehicle).Include(a => a.Calendar).Include(a => a.Customer).FirstOrDefaultAsync(a => a.RowId.Equals(id));
            if (item.BillType.Contains("گروهی"))
            {
                var amountSum = await _billRepository.Query().AsNoTracking().Where(a => a.BillNo.Equals(item.BillNo)).SumAsync(a => a.Amount);
                item.Amount = amountSum;
            }

            ViewData["Type"] = type;
            return View(item);
        }

        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> GetBillReceiverNames()
        {
            return Json(await _billRepository.Query().AsNoTracking().Select(a => a.ReceiverName.Replace("/", "")).Distinct().ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<JsonResult> ChangeBillIsReturned(long id)
        {
            string msg;
            string status = "danger";

            var bill = await _billRepository.Get(id);
            bill.IsReturned = !bill.IsReturned;
            _billRepository.Update(bill);
            try
            {
                await _billRepository.Save();

                msg = "عملیات موفقیت آمیز بود.";
                status = "success";
            }
            catch (Exception e)
            {
                msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
            }
            return Json(new { msg, status, });
        }

        [HttpGet]
        public async Task<IActionResult> Bill_EditGroup()
        {
            ViewData["Definitions"] = await _definitionRepo.Definitions().Where(a => a.DefinitionType == DefinitionType.BillType || a.DefinitionType == DefinitionType.BankBranch).AsNoTracking().OrderBy(a => a.Title).ToListAsync();
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Bill_EditGroup(string BillNo, string BankBillNo, int Day, int Month, int Year, string BankBranch, string BillType)
        {
            string msg;
            string status = "danger";
            if (string.IsNullOrWhiteSpace(BillNo) && string.IsNullOrWhiteSpace(BankBillNo))
            {
                msg = "عملیات با خطا مواجه شد. لطفا مقادیر فرم را بررسی و دوباره ارسال کنید.";
                return Json(new { msg, status });
            }

            var query = _billRepository.Query();

            if (!string.IsNullOrWhiteSpace(BillNo))
                query = query.Where(a => a.BillNo.Equals(BillNo));

            if (!string.IsNullOrWhiteSpace(BankBillNo))
                query = query.Where(a => a.BankBillNo.Equals(BankBillNo));

            var bills = await query.ToListAsync();

            if (!bills.Any())
            {
                msg = "عملیات با خطا مواجه شد. موردی پیدا نشد.";
                return Json(new { msg, status });
            }

            if (Year > 0 && Month > 0 && Day > 0)
            {
                var date = new PersianDateTime(Year, Month, Day).ToDateTime();
                foreach (var item in bills)
                {
                    item.Date = date;
                    item.EditDatetime = DateTime.Now;
                    item.EditorId = _userManager.GetUserId(User);
                }
            }

            if (BankBranch != "0")
                foreach (var item in bills)
                    item.BankBranch = BankBranch;

            if (BillType != "0")
                foreach (var item in bills)
                    item.BillType = BillType;

            _billRepository.UpdateRange(bills);
            try
            {
                await _billRepository.Save();

                msg = "عملیات موفقیت آمیز بود.";
                status = "success";
            }
            catch (Exception e)
            {
                msg = $"عملیات با خطا مواجه شد. جزئیات: {e.Message}";
            }

            return Json(new { msg, status });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<PartialViewResult> CreateBill()
        {
            ViewData["LastBillNo"] = await _billRepository.Query().AsNoTracking().MaxAsync(a => a.BillNo);
            ViewData["Customers"] = await _customerRepo.Customers().AsNoTracking().OrderByDescending(a => a.Name).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepo.Vehicles().AsNoTracking().OrderByDescending(a => a.LeftNumber).ToListAsync();
            ViewData["Definitions"] = await _definitionRepo.Definitions().Where(a => a.DefinitionType == DefinitionType.BillType || a.DefinitionType == DefinitionType.BankBranch).AsNoTracking().OrderBy(a => a.Title).ToListAsync();

            return PartialView("~/Views/Admin/Create/Bill.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> CreateBill(CreateBillVM b)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                var bill = new Bill
                {
                    Amount = b.Amount,
                    BankBillNo = b.BankBillNo,
                    BillNo = b.BillNo,
                    BankBranch = b.BankBranch,
                    BillType = b.BillType,
                    CalendarId = b.CalendarId,
                    CustomerId = b.CustomerId == 0 ? null : b.CustomerId.Value,
                    Description = b.Description,
                    IsReturned = b.IsReturned,
                    ReceiverName = b.ReceiverName,
                    VehicleId = b.VehicleId,
                    CreatorId = _userManager.GetUserId(User),
                    Date = new PersianDateTime(b.Year, b.Month, b.Day).ToDateTime()
                };

                if (!string.IsNullOrWhiteSpace(b.RealReceiverName))
                {
                    bill.BillDetail = new BillDetail
                    {
                        Bill = bill,
                        ReceiverBankAccount = b.ReceiverBankAccount,
                        ReceiverName = b.RealReceiverName
                    };
                }

                _billRepository.Create(bill);
                try
                {
                    await _billRepository.Save();

                    if (b.VehicleId.HasValue && !b.BillType.Contains("نوین بار"))
                    {
                        await _vehicleBalanceRepository.Create(new VehicleBalance
                        {
                            Amount = -b.Amount,
                            BillId = bill.Id,
                            CalendarId = b.CalendarId,
                            VehicleId = b.VehicleId.Value,
                            CreateDateTime = bill.Date,
                            Description = b.Description,
                            CustomerId = b.CustomerId
                        });

                        await _vehicleBalanceRepository.Save();
                    }

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
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<PartialViewResult> EditBill(long id)
        {
            ViewData["Customers"] = await _customerRepo.Customers().AsNoTracking().OrderByDescending(a => a.Name).ToListAsync();
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepo.Vehicles().AsNoTracking().OrderByDescending(a => a.LeftNumber).ToListAsync();
            ViewData["Definitions"] = await _definitionRepo.Definitions().Where(a => a.DefinitionType == DefinitionType.BillType || a.DefinitionType == DefinitionType.BankBranch).AsNoTracking().OrderBy(a => a.Title).ToListAsync();
            return PartialView("~/Views/Admin/Edit/Bill.cshtml", await _billRepository.GetEditData(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> EditBill(EditBillVM b)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                b.CustomerId = b.CustomerId.Value == 0 ? null : b.CustomerId;

                var item = await _billRepository.GetIncludedDetail(b.Id);

                var isVehicleChanged = false;
                if (item.VehicleId.HasValue && !b.VehicleId.HasValue)
                    isVehicleChanged = true;

                item.EditorId = _userManager.GetUserId(User);
                item.EditDatetime = DateTime.Now;
                item.CalendarId = b.CalendarId;
                item.VehicleId = b.VehicleId;
                item.BankBranch = b.BankBranch;
                item.Amount = b.Amount;
                item.BankBillNo = b.BankBillNo;
                item.BillNo = b.BillNo;
                item.BillType = b.BillType;
                item.Description = b.Description;
                item.ReceiverName = b.ReceiverName;
                item.CustomerId = b.CustomerId;
                item.IsReturned = b.IsReturned;

                item.Date = new PersianDateTime(b.Year, b.Month, b.Day).ToDateTime();
                if (item.BillDetail != null && !string.IsNullOrWhiteSpace(b.RealReceiverName))
                {
                    item.BillDetail.ReceiverName = b.RealReceiverName;
                    item.BillDetail.ReceiverBankAccount = b.ReceiverBankAccount;
                }
                else if (item.BillDetail != null && string.IsNullOrWhiteSpace(b.RealReceiverName))
                {
                    _billRepository.DeleteBillDetail(item.BillDetail);
                }
                else if (item.BillDetail == null && !string.IsNullOrWhiteSpace(b.RealReceiverName))
                {
                    item.BillDetail = new BillDetail
                    {
                        BillId = item.Id,
                        ReceiverBankAccount = b.ReceiverBankAccount,
                        ReceiverName = b.RealReceiverName
                    };
                }

                _billRepository.Update(item);
                try
                {
                    await _billRepository.Save();

                    var balanceItem = await _vehicleBalanceRepository.Query().Where(a => a.BillId.HasValue && a.BillId.Value.Equals(item.Id)).FirstOrDefaultAsync();

                    if (isVehicleChanged && b.BillType == "نوین بار")
                    {
                        _vehicleBalanceRepository.Delete(balanceItem);
                        await _vehicleBalanceRepository.Save();
                    }

                    if (b.VehicleId.HasValue && b.BillType != "نوین بار")
                    {
                        if (balanceItem != null)
                        {
                            balanceItem.Amount = -b.Amount;
                            balanceItem.CreateDateTime = item.Date;
                            balanceItem.EditDatetime = DateTime.Now;
                            balanceItem.CalendarId = b.CalendarId;
                            balanceItem.VehicleId = b.VehicleId.Value;
                            balanceItem.Description = b.Description;
                            balanceItem.CustomerId = b.CustomerId;

                            _vehicleBalanceRepository.Update(balanceItem);
                        }
                        else
                        {
                            await _vehicleBalanceRepository.Create(new VehicleBalance
                            {
                                Amount = -b.Amount,
                                BillId = b.Id,
                                CalendarId = b.CalendarId,
                                VehicleId = b.VehicleId.Value,
                                CreateDateTime = item.Date,
                                Description = b.Description,
                                CustomerId = b.CustomerId
                            });
                        }

                        await _vehicleBalanceRepository.Save();
                    }

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

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> DeleteBill(long id)
        {
            var item = await _billRepository.Query().Include(a => a.BillDetail).FirstOrDefaultAsync(a => a.Id.Equals(id));
            _billRepository.Delete(item);
            try
            {
                await _billRepository.Save();

                var balanceItem = await _vehicleBalanceRepository.Query().FirstOrDefaultAsync(a => a.BillId.HasValue && a.BillId.Value.Equals(id));
                if (balanceItem != null)
                {
                    _vehicleBalanceRepository.Delete(balanceItem);
                    await _vehicleBalanceRepository.Save();
                }
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<JsonResult> CalculateBillSum(string billNo)
        {
            var sum = await _billRepository.Query().Where(a => a.BillNo.Equals(billNo)).SumAsync(a => a.Amount);
            return Json(sum.ToString("N0"));
        }

        public async Task<JsonResult> GetSelectedVehicleBalanceInBillForm(long customerId, long calendarId, long vehicleId)
        {
            var thisCalendarSequence = await _calendarRepo.Calendars().AsNoTracking().Where(a => a.Id.Equals(calendarId)).Select(a => a.Sequence).SingleAsync();
            var calendars = await _calendarRepo.Calendars().AsNoTracking().Where(a => a.Sequence <= thisCalendarSequence).Select(a => a.Id).ToListAsync();
            var vehicleBalance = await _vehicleBalanceRepository.Query()
                .Where(a => (a.CustomerId.HasValue ? a.CustomerId.Value.Equals(customerId) : true) &&
                a.CalendarId.HasValue && calendars.Contains(a.CalendarId.Value) && a.VehicleId.Equals(vehicleId))
                .Select(a => a.Amount).SumAsync();

            //var bills = await _billRepository.Query().AsNoTracking().Where(a => a.CustomerId.HasValue && a.CustomerId.Value.Equals(customerId)
            //    && a.VehicleId.HasValue && a.VehicleId.Value.Equals(vehicleId) && calendars.Contains(a.CalendarId)).Select(a => a.Amount).SumAsync();

            return Json(vehicleBalance > 0 ? vehicleBalance : "");
        }
        #endregion

        #region VehicleBalance
        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> VehicleBalanceByCustomer(long customerId, long calendarId, bool isFreeDriverPrice)
        {
            if (!await _customerRepo.Customers().AnyAsync(a => a.Id.Equals(customerId)))
                return NotFound("Customer not found");

            if (!await _calendarRepo.Calendars().AnyAsync(a => a.Id.Equals(calendarId)))
                return NotFound("Calendar not found");

            var data = await _vehicleRepo.ActivityList(customerId, calendarId, true, isFreeDriverPrice);
            ViewData["Calendar"] = await _calendarRepo.Get(calendarId);
            ViewData["Customer"] = await _customerRepo.Get(customerId);

            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateVehicleBalance(
            long CustomerId, long CalendarId, long VehicleId, string AmountType, double Amount, string Description)
        {
            await _vehicleBalanceRepository.Create(new VehicleBalance
            {
                Amount = AmountType == "minus" ? -(Amount) : Amount,
                Description = Description,
                CalendarId = CalendarId,
                VehicleId = VehicleId,
                CustomerId = CustomerId,
                CreateDateTime = DateTime.Now
            });
            try
            {
                await _vehicleBalanceRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> CreateVehicleBalanceList(
            List<long> idList, long CustomerId, long CalendarId, string AmountType, double Amount, string Description)
        {
            if (!idList.Any())
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: لطفا یک یا چند وسیله نقلیه را انتخاب کنید. |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            if (CustomerId <= 0 || CalendarId <= 0)
            {
                TempData["msg"] = $"خطا در فرم. مشتری یا تقویم کاری. |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            foreach (var vehicleId in idList)
            {
                await _vehicleBalanceRepository.Create(new VehicleBalance
                {
                    Amount = AmountType == "minus" ? -(Amount) : Amount,
                    Description = Description,
                    CalendarId = CalendarId,
                    VehicleId = vehicleId,
                    CustomerId = CustomerId,
                    CreateDateTime = DateTime.Now
                });
            }

            try
            {
                await _vehicleBalanceRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> VehicleBalanceList(long? id, long? calendarId, long? customerId)
        {
            if (!id.HasValue)
                return NotFound();

            ViewData["Vehicle"] = await _vehicleRepo.Get(id.Value);

            var query = _vehicleBalanceRepository.Query().Where(a => a.VehicleId.Equals(id.Value));
            if (customerId.HasValue)
            {
                query = query.Where(a => a.CustomerId.Equals(customerId.Value));
            }
            if (calendarId.HasValue)
            {
                var beforeCalendars = await _calendarRepo.Calendars().AsNoTracking().Where(a => a.Sequence < _calendarRepo.Calendars().Single(b => b.Id.Equals(calendarId.Value)).Sequence).Select(a => a.Id).ToListAsync();
                ViewData["BeforeBalanceSum"] = await query.Where(a => a.CalendarId.HasValue && beforeCalendars.Contains(a.CalendarId.Value)).SumAsync(a => a.Amount);
                query = query.Where(a => a.CalendarId.HasValue && a.CalendarId.Equals(calendarId.Value));
            }
            return View(await query.OrderBy(a => a.CreateDateTime).ToListAsync());
        }

        #endregion

        #region UserPlanner
        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> UserPlanner(int? p)
        {
            var pageNumber = p ?? 1;
            var onePageOfData = await _userPlannerRepository.Query().Where(a => a.UserId.Equals(_userManager.GetUserId(User))).OrderByDescending(a => a.Date).ToPagedListAsync(pageNumber, 15);
            ViewBag.data = onePageOfData;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> UserPlanner_Search(int Year, int Month, int Day)
        {
            var date = new PersianDateTime(Year, Month, Day).ToDateTime();
            return PartialView(await _userPlannerRepository.Query().Where(a => a.Date.Equals(date) && a.UserId.Equals(_userManager.GetUserId(User))).FirstOrDefaultAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public PartialViewResult CreateUserPlanner()
        {
            ViewData["UserId"] = _userManager.GetUserId(User);
            return PartialView("~/Views/Admin/Create/UserPlanner.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> CreateUserPlanner(CreateUserPlannerVM v)
        {
            if (ModelState.IsValid)
            {
                var date = new PersianDateTime(v.Year, v.Month, v.Day).ToDateTime();
                string userId = _userManager.GetUserId(User);

                if (await _userPlannerRepository.Query().AnyAsync(a => a.Date.Equals(date) && a.UserId.Equals(userId)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. رکورد تکراری درج کرده اید. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _userPlannerRepository.Create(new UserPlanner
                {
                    Date = date,
                    UserId = userId
                });
                try
                {
                    await _userPlannerRepository.Save();
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
        public async Task<PartialViewResult> EditUserPlanner(int id)
        {
            return PartialView("~/Views/Admin/Edit/UserPlanner.cshtml", await _userPlannerRepository.GetUserPlannerEditData(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> EditUserPlanner(EditUserPlannerVM v)
        {
            if (ModelState.IsValid)
            {
                var item = await _userPlannerRepository.Get(v.Id);

                var date = new PersianDateTime(v.Year, v.Month, v.Day).ToDateTime();
                item.Date = date;

                _userPlannerRepository.Update(item);
                try
                {
                    await _userPlannerRepository.Save();
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
        public async Task<IActionResult> DeleteUserPlanner(long id)
        {
            var item = await _userPlannerRepository.Query().Include(a => a.UserPlannerItems).FirstOrDefaultAsync(a => a.Id.Equals(a.Id));
            _userPlannerRepository.Delete(item);
            try
            {
                await _userPlannerItemRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region UserPlannerItem
        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> UserPlannerItem(string id)
        {
            ViewData["UserPlannerRowId"] = id;
            var userPlannerId = await _userPlannerRepository.Query().AsNoTracking().Where(a => a.RowId.Equals(id)).Select(a => a.Id).FirstOrDefaultAsync();
            return View(await _userPlannerItemRepository.Query().Where(a => a.UserPlannerId.Equals(userPlannerId)).OrderBy(a => a.Priority).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> GetUserPlannerItemDetail(long id)
        {
            return Json(new { Content = await _userPlannerItemRepository.Query().Where(a => a.Id.Equals(id)).Select(a => a.Content).FirstOrDefaultAsync() });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> CreateUserPlannerItem(string id)
        {
            ViewData["UserPlanner"] = await _userPlannerRepository.Query().AsNoTracking().FirstOrDefaultAsync(a => a.RowId.Equals(id));
            return View("~/Views/Admin/Create/UserPlannerItem.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> CreateUserPlannerItem([Bind("Priority", "Title", "Content", "UserPlannerId")] UserPlannerItem v)
        {
            if (ModelState.IsValid)
            {
                if (await _userPlannerItemRepository.Query().AnyAsync(a => a.UserPlannerId.Equals(v.UserPlannerId) && a.Priority.Equals(v.Priority)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. لطفا اولویت تکراری ارسال نکنید. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _userPlannerItemRepository.Create(v);
                try
                {
                    await _userPlannerItemRepository.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    return RedirectToAction(nameof(UserPlannerItem),
                        new { id = await _userPlannerRepository.Query().Where(a => a.Id.Equals(v.UserPlannerId)).Select(a => a.RowId).FirstAsync() }
                        );
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
        public async Task<IActionResult> EditUserPlannerItem(long id)
        {
            return View("~/Views/Admin/Edit/UserPlannerItem.cshtml", await _userPlannerItemRepository.Query().Include(a => a.UserPlanner).FirstOrDefaultAsync(a => a.Id.Equals(id)));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> EditUserPlannerItem(UserPlannerItem v)
        {
            if (ModelState.IsValid)
            {
                if (await _userPlannerItemRepository.Query().AnyAsync(a => !a.Id.Equals(v.Id) && a.UserPlannerId.Equals(v.UserPlannerId) && a.Priority.Equals(v.Priority)))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. لطفا اولویت تکراری ارسال نکنید. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var item = await _userPlannerItemRepository.Get(v.Id);

                item.Priority = v.Priority;
                item.Title = v.Title;
                item.Content = v.Content;

                _userPlannerItemRepository.Update(item);
                try
                {
                    await _userPlannerItemRepository.Save();
                    TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
                    return RedirectToAction(nameof(UserPlannerItem),
                        new { id = await _userPlannerRepository.Query().Where(a => a.Id.Equals(v.UserPlannerId)).Select(a => a.RowId).FirstAsync() }
                        );
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
        public async Task<IActionResult> DeleteUserPlannerItem(long id)
        {
            var item = await _userPlannerItemRepository.Get(id);
            _userPlannerItemRepository.Delete(item);
            try
            {
                await _userPlannerItemRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion
    }
}
