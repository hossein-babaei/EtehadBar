using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using EtehadBar.Infra.Data.Repository;
using EtehadBar.MVC.Filters;
using Helpers;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace EtehadBar.MVC.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(ActionLogFilter))]
    public class ReportController : Controller
    {
        private readonly ICalendarRepository _calendarRepo;
        private readonly ICostRepository _costRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ILoadFactorRepository _loadFactorRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IFreeLoadFactorRepository _freeLoadFactorRepository;
        private readonly IBillRepository _billRepository;
        private readonly IVehicleBalanceRepository _vehicleBalanceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITurnoverRepository _turnoverRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ICustomerFactorRepository _customerFactorRepository;
        private readonly ITurnoverProfileRepository _turnoverProfileRepository;
        private readonly ILoadRoutesRepository _loadRoutesRepository;
        private readonly ICustomerPeriodicBalanceSummaryRepository _customerPeriodicBalanceSummaryRepository;
        private readonly ICustomerPeriodicBalanceAddonRepository _customerPeriodicBalanceAddonRepository;
        private readonly ILoadFactorNovinRepository _loadFactorNovinRepository;
        private readonly ITurnoverProfilePeriodRepository _turnoverProfilePeriodRepository;
        private readonly IShippingFeeGroupRepository _shippingFeeGroupRepository;
        private readonly IShippingFeeRepository _shippingFeeRepository;

        public ReportController(
            ICalendarRepository calendarRepository,
            ICostRepository costRepository,
            ICustomerRepository customerRepository,
            ILoadFactorRepository loadFactorRepository,
            IVehicleRepository vehicleRepository,
            UserManager<ApplicationUser> userManager,
            IFreeLoadFactorRepository freeLoadFactorRepository,
            IBillRepository billRepository,
            IVehicleBalanceRepository vehicleBalanceRepository,
            ITurnoverRepository turnoverRepository,
            IContractRepository contractRepository,
            ICustomerFactorRepository customerFactorRepository,
            ITurnoverProfileRepository turnoverProfileRepository,
            ILoadRoutesRepository loadRoutesRepository,
            ICustomerPeriodicBalanceSummaryRepository customerPeriodicBalanceSummaryRepository,
            ICustomerPeriodicBalanceAddonRepository customerPeriodicBalanceAddonRepository,
            ILoadFactorNovinRepository loadFactorNovinRepository,
            ITurnoverProfilePeriodRepository turnoverProfilePeriodRepository,
            IShippingFeeGroupRepository shippingFeeGroupRepository,
            IShippingFeeRepository shippingFeeRepository)
        {
            _calendarRepo = calendarRepository;
            _costRepo = costRepository;
            _customerRepo = customerRepository;
            _loadFactorRepo = loadFactorRepository;
            _vehicleRepo = vehicleRepository;
            _userManager = userManager;
            _freeLoadFactorRepository = freeLoadFactorRepository;
            _billRepository = billRepository;
            _vehicleBalanceRepository = vehicleBalanceRepository;
            _turnoverRepository = turnoverRepository;
            _contractRepository = contractRepository;
            _customerFactorRepository = customerFactorRepository;
            _turnoverProfileRepository = turnoverProfileRepository;
            _loadRoutesRepository = loadRoutesRepository;
            _customerPeriodicBalanceSummaryRepository = customerPeriodicBalanceSummaryRepository;
            _customerPeriodicBalanceAddonRepository = customerPeriodicBalanceAddonRepository;
            _loadFactorNovinRepository = loadFactorNovinRepository;
            _turnoverProfilePeriodRepository = turnoverProfilePeriodRepository;
            _shippingFeeGroupRepository = shippingFeeGroupRepository;
            _shippingFeeRepository = shippingFeeRepository;
        }

        [HttpPost]
        public async Task<JsonResult> GetUserListJson()
        {
            return Json(await _userManager.Users.OrderBy(a => a.Lastname).Select(a => new { id = a.Id, fullname = a.Firstname + " " + a.Lastname }).ToListAsync());
        }

        [HttpPost]
        public async Task<JsonResult> GetVehicleListJson()
        {
            return Json(await _vehicleRepo.Vehicles().OrderBy(a => a.LeftNumber).Select(a => new
            {
                a.Id,
                number = $"ایران {a.IranStateNumber} - {a.RightNumber} {a.NumberWord} {a.LeftNumber}",
                a.Type
            }).ToListAsync());
        }

        public async Task<IActionResult> Customer(long? id, string statusNumber, long calendarId)
        {
            if (Request.IsAjaxRequest())
            {
                ViewData["statusNumber"] = statusNumber;

                var calendar = await _calendarRepo.Get(calendarId);
                ViewData["calendar"] = calendar;

                var customer = await _customerRepo.Get(id.Value);
                ViewData["customer"] = customer;

                return customer.CustomerType switch
                {
                    CustomerType.SaipaPlasco => PartialView("~/Views/Report/Customer/_Plasco.cshtml"/*, await db.LoadFactor.Where(a => a.CustomerId.Equals(id.Value) && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Counter).ToListAsync()*/),
                    CustomerType.SaipaPress => PartialView("~/Views/Report/Customer/_SaipaPress.cshtml"),
                    CustomerType.SazehGostar => PartialView("~/Views/Report/Customer/_SazehGostar.cshtml"),
                    _ => NotFound(),
                };
            }
            else
            {
                if (id.HasValue)
                {
                    ViewData["statusNumber"] = statusNumber;

                    var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
                    ViewData["calendars"] = calendars;

                    var customer = await _customerRepo.Get(id.Value);
                    ViewData["customer"] = customer;

                    return customer.CustomerType switch
                    {
                        CustomerType.SaipaPlasco => View("~/Views/Report/Customer/Plasco.cshtml"/*, await db.LoadFactor.Where(a => a.CustomerId.Equals(id.Value) && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Counter).ToListAsync()*/),
                        CustomerType.SaipaPress => View("~/Views/Report/Customer/SaipaPress.cshtml"),
                        CustomerType.SazehGostar => View("~/Views/Report/Customer/SazehGostar.cshtml"),
                        _ => NotFound(),
                    };
                }
                else
                {
                    ViewData["calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();

                    return View("CustomerList", await _customerRepo.GetAll());
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> CustomerIncome(long? id)
        {
            if (!id.HasValue)
                return BadRequest("parameter error");

            if (!await _customerRepo.Customers().AnyAsync(a => a.Id.Equals(id.Value)))
                return NotFound("مشتری پیدا نشد");

            ViewData["customer"] = await _customerRepo.Get(id.Value);

            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;

            var firstCalendar = calendars.First();

            return View(await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CustomerId.Equals(id.Value) && a.Date >= firstCalendar.StartDate && a.Date <= firstCalendar.EndDate).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> CustomerIncome(long? id, long calendarId)
        {
            ViewData["customer"] = await _customerRepo.Get(id.Value);
            var calendar = await _calendarRepo.Get(calendarId);
            ViewData["calendar"] = calendar;

            return PartialView("_CustomerIncome", await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.Date >= calendar.StartDate && a.Date <= calendar.EndDate && a.CustomerId.Equals(id.Value)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detailed()
        {
            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;

            var latestCal = calendars.First();
            ViewData["cost"] = await _costRepo.Costs().Where(a => a.CalendarId.Equals(latestCal.Id)).SumAsync(a => a.Amount);
            var excludedBillTypes = new List<string> { "جابجایی از پاسارگاد", "جابجایی حساب", "واریز شرکا - تامین وجه" };
            ViewData["payment"] = await _billRepository.Query().Where(a => !excludedBillTypes.Contains(a.BillType) &&
            a.Date >= latestCal.StartDate && a.Date <= latestCal.EndDate).SumAsync(a => a.Amount);
            //ViewData["excludedBills"] = await _billRepository.Query().Where(a => excludedBillTypes.Contains(a.BillType) &&
            //a.Date >= latestCal.StartDate && a.Date <= latestCal.EndDate).SumAsync(a => a.Amount);
            ViewData["income"] = await _customerRepo.CustomerIncomes().Where(a => a.Date >= latestCal.StartDate && a.Date <= latestCal.EndDate).SumAsync(a => a.Amount);
            return View(await _loadFactorRepo.LoadFactors()
                .Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Driver).Include(a => a.Vehicle).Include(a => a.Calendar)
                .Include(a => a.Contract).ThenInclude(a => a.Customer)
                .Where(a => a.CalendarId.Equals(latestCal.Id)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detailed(long calendarId)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            ViewData["calendar"] = calendar;

            ViewData["cost"] = await _costRepo.Costs().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            var excludedBillTypes = new List<string> { "جابجایی از پاسارگاد", "جابجایی حساب", "واریز شرکا - تامین وجه" };
            ViewData["payment"] = await _billRepository.Query().Where(a => !excludedBillTypes.Contains(a.BillType) &&
            a.Date >= calendar.StartDate && a.Date <= calendar.EndDate).SumAsync(a => a.Amount);
            //ViewData["excludedBills"] = await _billRepository.Query().Where(a => excludedBillTypes.Contains(a.BillType) &&
            //a.Date >= calendar.StartDate && a.Date <= calendar.EndDate).SumAsync(a => a.Amount);
            ViewData["income"] = await _customerRepo.CustomerIncomes().Where(a => a.Date >= calendar.StartDate && a.Date <= calendar.EndDate).SumAsync(a => a.Amount);

            var data = new List<GlobalLoadFactorVM>();
            var loadFactors = await _loadFactorRepo.LoadFactors()
                .Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Driver).Include(a => a.Vehicle)
                .Include(a => a.Contract).ThenInclude(a => a.Customer)
                .Where(a => a.CalendarId.Equals(calendarId)).Select(a => new GlobalLoadFactorVM
                {
                    Amount = a.Amount,
                    CustomerName = a.Contract.Customer.Name + " " + a.Contract.Number,
                    Date = a.Date,
                    Destination = a.Destination.Title,
                    Origin = a.Origin.Title,
                    DriverFee = a.DriverFee,
                    DriverName = a.Driver.Fullname,
                    LoadFactorDeductions = a.LoadFactorDeductions,
                    LoadNumber = a.LoadNumber,
                    LoadNumberGov = a.LoadNumberGov,
                    Id = a.Id,
                    RowId = a.RowId,
                    VAT = a.VAT,
                    VehicleType = a.Vehicle.Type,
                    WithholdingTax = a.WithholdingTax,
                    Tonnage = a.Tonnage,
                    TonnagePrice = a.TonnagePrice,
                    DriverTonnagePrice = a.DriverTonnagePrice
                }).ToListAsync();

            data.AddRange(loadFactors);

            var freeLoadFactors = await _freeLoadFactorRepository.Query().Where(a => a.CalendarId.Equals(calendarId)).Select(a => new GlobalLoadFactorVM
            {
                Amount = a.Amount,
                CustomerName = a.ApplicantName,
                Date = a.Date,
                Destination = a.Destination,
                Origin = a.Origin,
                DriverFee = a.DriverFee,
                DriverName = a.DriverName,
                LoadFactorDeductions = a.LoadFactorDeductions,
                LoadNumber = a.LoadNumber,
                LoadNumberGov = a.LoadNumberGov,
                Id = a.Id,
                RowId = a.RowId,
                VAT = a.VAT,
                VehicleType = a.VehicleType,
                WithholdingTax = a.WithholdingTax,
                Tonnage = a.Tonnage,
                TonnagePrice = a.TonnagePrice,
                DriverTonnagePrice = a.DriverTonnagePrice
            }).ToListAsync();

            data.AddRange(freeLoadFactors);

            return PartialView("_Detailed", data.OrderBy(a => a.Date).ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> Cost()
        {
            var users = await _userManager.Users.AsNoTracking().Where(a => a.Role != ApplicationRoleType.RegisterUser).OrderBy(a => a.Firstname).ThenBy(a => a.Lastname).ToListAsync();
            ViewData["users"] = users;

            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;

            var query = _costRepo.Costs().Include(a => a.ApplicationUser).AsQueryable();
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.UserId.Equals(_userManager.GetUserId(User)));

            var latestCal = calendars.First();
            return View(await query.Where(a => a.CalendarId.Equals(latestCal.Id)).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad, Accountant")]
        public async Task<IActionResult> Cost(long calendarId, string userId)
        {
            ViewData["userId"] = userId;
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);

            var query = _costRepo.Costs().Include(a => a.ApplicationUser).Where(a => a.CalendarId.Equals(calendarId));
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.UserId.Equals(_userManager.GetUserId(User)));

            if (userId != "all")
                query = query.Where(a => a.UserId.Equals(userId));

            return PartialView("_Cost", await query.ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> VehicleLoadFactor()
        {
            ViewData["calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["customers"] = await _customerRepo.Customers().AsNoTracking().Where(a => a.Status).OrderBy(a => a.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> VehicleLoadFactor(long calendarId, long vehicleId, long customerId, string type)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            if (!string.IsNullOrWhiteSpace(type))
                if (type == "next" && await _calendarRepo.Calendars().AnyAsync(a => a.Sequence > calendar.Sequence))
                {
                    calendar = await _calendarRepo.Calendars().Where(a => a.Sequence == (calendar.Sequence + 1)).FirstOrDefaultAsync();
                    calendarId = calendar.Id;
                }
                else if (type == "previous" && await _calendarRepo.Calendars().AnyAsync(a => a.Sequence < calendar.Sequence))
                {
                    calendar = await _calendarRepo.Calendars().Where(a => a.Sequence == (calendar.Sequence - 1)).FirstOrDefaultAsync();
                    calendarId = calendar.Id;
                }

            ViewData["calendar"] = calendar;
            ViewData["customerId"] = customerId;
            ViewData["vehicle"] = await _vehicleRepo.Get(vehicleId);
            ViewData["Balance"] = await _vehicleBalanceRepository.GetVehicleBalanceSum(vehicleId, calendarId, customerId == 0 ? null : customerId);

            var query = _loadFactorRepo.LoadFactors().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Driver).Include(a => a.Contract).ThenInclude(a => a.Customer)
                .Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId));
            if (customerId > 0)
                query = query.Where(a => a.Contract.CustomerId.Equals(customerId));

            return PartialView("_VehicleLoadFactor", await query.OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> VehicleActivity()
        {
            ViewData["calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["customers"] = await _customerRepo.Customers().AsNoTracking().Where(a => a.Status).OrderBy(a => a.Name).ToListAsync();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> VehicleActivity(long calendarId, long vehicleId, long customerId, string type)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            if (!string.IsNullOrWhiteSpace(type))
                if (type == "next" && await _calendarRepo.Calendars().AnyAsync(a => a.Sequence > calendar.Sequence))
                {
                    calendar = await _calendarRepo.Calendars().Where(a => a.Sequence == (calendar.Sequence + 1)).FirstOrDefaultAsync();
                    calendarId = calendar.Id;
                }
                else if (type == "previous" && await _calendarRepo.Calendars().AnyAsync(a => a.Sequence < calendar.Sequence))
                {
                    calendar = await _calendarRepo.Calendars().Where(a => a.Sequence == (calendar.Sequence - 1)).FirstOrDefaultAsync();
                    calendarId = calendar.Id;
                }

            ViewData["calendar"] = calendar;
            ViewData["customerId"] = customerId;
            ViewData["vehicle"] = await _vehicleRepo.Get(vehicleId);
            ViewData["Balance"] = await _vehicleBalanceRepository.GetVehicleBalanceSum(vehicleId, calendarId, customerId == 0 ? null : customerId);

            var query = _loadFactorRepo.LoadFactors().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Driver).Include(a => a.Contract).ThenInclude(a => a.Customer)
                .Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId));
            if (customerId > 0)
                query = query.Where(a => a.Contract.CustomerId.Equals(customerId));

            return PartialView("_VehicleActivity", await query.OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> FreeLoadFactor()
        {
            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;
            var latestCal = calendars.First();
            return View(await _freeLoadFactorRepository.Query().Where(a => a.CalendarId.Equals(latestCal.Id)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> FreeLoadFactor(long calendarId, string startDate, string endDate)
        {
            startDate = startDate.PersianToEnglish();
            endDate = endDate.PersianToEnglish();
            ViewData["Calendar"] = new Calendar();
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;
            if (calendarId > 0)
            {
                ViewData["Calendar"] = await _calendarRepo.Get(calendarId);
                return PartialView("_FreeLoadFactor", await _freeLoadFactorRepository.Query().Where(a => a.CalendarId.Equals(calendarId)).OrderBy(a => a.Date).ToListAsync());
            }
            else
            {
                var startArr = startDate.Split('/');
                var startD = new PersianDateTime(Convert.ToInt32(startArr[0]), Convert.ToInt32(startArr[1]), Convert.ToInt32(startArr[2]), 0, 0, 0).ToDateTime();
                var endArr = endDate.Split('/');
                var endD = new PersianDateTime(Convert.ToInt32(endArr[0]), Convert.ToInt32(endArr[1]), Convert.ToInt32(endArr[2]), 23, 59, 59).ToDateTime();
                if (startD > endD)
                {
                    var calendar = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).FirstAsync();
                    ViewData["Calendar"] = calendar;
                    return PartialView("_FreeLoadFactor", await _freeLoadFactorRepository.Query().Where(a => a.CalendarId.Equals(calendar.Id)).OrderBy(a => a.Date).ToListAsync());
                }
                else
                {
                    return PartialView("_FreeLoadFactor", await _freeLoadFactorRepository.Query().Where(a => a.Date >= startD && a.Date <= endD).OrderBy(a => a.Date).ToListAsync());
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> NovinLoadFactor()
        {
            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;
            var latestCal = calendars.First();
            return View(await _loadFactorNovinRepository.Query().Include(a => a.Vehicle).Where(a => a.CalendarId.Equals(latestCal.Id)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> NovinLoadFactor(long calendarId, string startDate, string endDate)
        {
            startDate = startDate.PersianToEnglish();
            endDate = endDate.PersianToEnglish();
            ViewData["Calendar"] = new Calendar();
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;
            if (calendarId > 0)
            {
                ViewData["Calendar"] = await _calendarRepo.Get(calendarId);
                return PartialView("_NovinLoadFactor", await _loadFactorNovinRepository.Query().Include(a => a.Vehicle).Where(a => a.CalendarId.Equals(calendarId)).OrderBy(a => a.Date).ToListAsync());
            }
            else
            {
                var startArr = startDate.Split('/');
                var startD = new PersianDateTime(Convert.ToInt32(startArr[0]), Convert.ToInt32(startArr[1]), Convert.ToInt32(startArr[2]), 0, 0, 0).ToDateTime();
                var endArr = endDate.Split('/');
                var endD = new PersianDateTime(Convert.ToInt32(endArr[0]), Convert.ToInt32(endArr[1]), Convert.ToInt32(endArr[2]), 23, 59, 59).ToDateTime();
                if (startD > endD)
                {
                    var calendar = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).FirstAsync();
                    ViewData["Calendar"] = calendar;
                    return PartialView("_NovinLoadFactor", await _loadFactorNovinRepository.Query().Include(a => a.Vehicle).Where(a => a.CalendarId.Equals(calendar.Id)).OrderBy(a => a.Date).ToListAsync());
                }
                else
                {
                    return PartialView("_NovinLoadFactor", await _loadFactorNovinRepository.Query().Include(a => a.Vehicle).Where(a => a.Date >= startD && a.Date <= endD).OrderBy(a => a.Date).ToListAsync());
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Turnover()
        {
            ViewData["TurnoverProfiles"] = await _turnoverProfileRepository.Query().AsNoTracking().OrderBy(a => a.FullName).ToListAsync();
            var startOfPersianYear = new PersianDateTime(new PersianDateTime(DateTime.Now).Year, 1, 1).ToDateTime();
            return View(await _turnoverRepository.Query().Include(a => a.TurnoverProfile).Where(a => a.Date >= startOfPersianYear).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Turnover(string startDate, string endDate, long profileId, TurnoverType turnoverType, long? turnoverProfilePeriodId)
        {
            if (turnoverProfilePeriodId.HasValue)
            {
                var turnoverProfilePeriod = await _turnoverProfilePeriodRepository.Get(turnoverProfilePeriodId.Value);

                ViewData["StartDate"] = new PersianDateTime(turnoverProfilePeriod.StartDate);
                ViewData["EndDate"] = new PersianDateTime(turnoverProfilePeriod.EndDate);

                if (profileId == 0)
                {
                    var turnoverProfiles = await _turnoverProfileRepository.Query().AsNoTracking().Where(a => a.TurnoverType == turnoverType).Select(a => a.Id).ToListAsync();
                    return PartialView("_Turnover", await _turnoverRepository.Query().Include(a => a.TurnoverProfile).Where(a => a.Date >= turnoverProfilePeriod.StartDate && a.Date <= turnoverProfilePeriod.EndDate && turnoverProfiles.Contains(a.TurnoverProfileId)).OrderBy(a => a.Date).ToListAsync());
                }
                else
                {
                    return PartialView("_Turnover", await _turnoverRepository.Query().Include(a => a.TurnoverProfile).Where(a => a.Date >= turnoverProfilePeriod.StartDate && a.Date <= turnoverProfilePeriod.EndDate && a.TurnoverProfileId.Equals(profileId)).OrderBy(a => a.Date).ToListAsync());
                }
            }
            else
            {
                startDate = startDate.PersianToEnglish();
                endDate = endDate.PersianToEnglish();
                ViewData["StartDate"] = startDate;
                ViewData["EndDate"] = endDate;

                var startArr = startDate.Split('/');
                var startD = new PersianDateTime(Convert.ToInt32(startArr[0]), Convert.ToInt32(startArr[1]), Convert.ToInt32(startArr[2]), 0, 0, 0).ToDateTime();
                var endArr = endDate.Split('/');
                var endD = new PersianDateTime(Convert.ToInt32(endArr[0]), Convert.ToInt32(endArr[1]), Convert.ToInt32(endArr[2]), 23, 59, 59).ToDateTime();
                if (startD > endD)
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. تاریخ شروع از تاریخ پایان بزرگتر است. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                    if (profileId == 0)
                    {
                        var turnoverProfiles = await _turnoverProfileRepository.Query().AsNoTracking().Where(a => a.TurnoverType == turnoverType).Select(a => a.Id).ToListAsync();
                        return PartialView("_Turnover", await _turnoverRepository.Query().Include(a => a.TurnoverProfile).Where(a => a.Date >= startD && a.Date <= endD && turnoverProfiles.Contains(a.TurnoverProfileId)).OrderBy(a => a.Date).ToListAsync());
                    }
                    else
                    {
                        return PartialView("_Turnover", await _turnoverRepository.Query().Include(a => a.TurnoverProfile).Where(a => a.Date >= startD && a.Date <= endD && a.TurnoverProfileId.Equals(profileId)).OrderBy(a => a.Date).ToListAsync());
                    }
                }
            }
        }

        #region CustomerPeriodicBalanceSummary
        [HttpGet]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> CustomerPeriodicBalanceSummary(long customerId)
        {
            ViewData["CustomerInfo"] = await _customerRepo.Get(customerId);
            return View(await _customerPeriodicBalanceSummaryRepository.Query().AsNoTracking().Where(a => a.CustomerId.Equals(customerId)).OrderByDescending(a => a.EndDate).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Accountant")]
        public IActionResult CreateCustomerPeriodicBalanceSummary(long customerId)
        {
            ViewData["CustomerId"] = customerId;
            return PartialView("~/Views/Report/Create/CustomerPeriodicBalanceSummary.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> CreateCustomerPeriodicBalanceSummary(CreateCustomerPeriodicBalanceSummaryVM c)
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

                _customerPeriodicBalanceSummaryRepository.Create(new CustomerPeriodicBalanceSummary
                {
                    BalanceAmount = c.BalanceAmount,
                    CustomerId = c.CustomerId,
                    StartDate = startDate,
                    EndDate = endDate,
                    InsuranceBalanceAmount = c.InsuranceBalanceAmount
                });
                try
                {
                    await _customerPeriodicBalanceSummaryRepository.Save();
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
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<PartialViewResult> EditCustomerPeriodicBalanceSummary(int id)
        {
            var item = await _customerPeriodicBalanceSummaryRepository.Get(id);
            var persianStartDate = new PersianDateTime(item.StartDate);
            var persianEndDate = new PersianDateTime(item.EndDate);

            return PartialView("~/Views/Report/Edit/CustomerPeriodicBalanceSummary.cshtml", new EditCustomerPeriodicBalanceSummaryVM
            {
                EndDay = persianEndDate.Day,
                EndMonth = persianEndDate.Month,
                EndYear = persianEndDate.Year,
                Id = item.Id,
                StartDay = persianStartDate.Day,
                StartMonth = persianStartDate.Month,
                StartYear = persianStartDate.Year,
                BalanceAmount = item.BalanceAmount,
                InsuranceBalanceAmount = item.InsuranceBalanceAmount,
                CustomerId = item.CustomerId
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> EditCustomerPeriodicBalanceSummary(EditCustomerPeriodicBalanceSummaryVM c)
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

                var item = await _customerPeriodicBalanceSummaryRepository.Get(c.Id);
                item.StartDate = startDate;
                item.EndDate = endDate;
                item.CustomerId = c.CustomerId;
                item.BalanceAmount = c.BalanceAmount;
                item.InsuranceBalanceAmount = c.InsuranceBalanceAmount;

                _customerPeriodicBalanceSummaryRepository.Update(item);
                try
                {
                    await _customerPeriodicBalanceSummaryRepository.Save();
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
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> DeleteCustomerPeriodicBalanceSummary(int id)
        {
            var item = await _customerPeriodicBalanceSummaryRepository.Get(id);
            if (item == null) return NotFound();

            _customerPeriodicBalanceSummaryRepository.Delete(item);
            try
            {
                await _customerPeriodicBalanceSummaryRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> CustomerPeriodicBalanceAddon(long? id)
        {
            if (id == null) return NotFound();

            ViewData["Id"] = id;

            return View(await _customerPeriodicBalanceAddonRepository.Query().AsNoTracking()
                .Where(a => a.CustomerPeriodicBalanceSummaryId.Equals(id.Value)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Accountant")]
        public IActionResult CreateCustomerPeriodicBalanceAddon(long id)
        {
            ViewData["Id"] = id;
            return PartialView("~/Views/Report/Create/CustomerPeriodicBalanceAddon.cshtml");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> CreateCustomerPeriodicBalanceAddon(CreateCustomerPeriodicBalanceAddonVM c)
        {
            if (ModelState.IsValid)
            {
                DateTime date = new PersianDateTime(c.Year, c.Month, c.Day).ToDateTime();

                _customerPeriodicBalanceAddonRepository.Create(new CustomerPeriodicBalanceAddon
                {
                    Amount = c.Amount,
                    Title = c.Title,
                    CustomerPeriodicBalanceSummaryId = c.CustomerPeriodicBalanceSummaryId,
                    Date = date,
                    IsPositive = c.IsPositive
                });
                try
                {
                    await _customerPeriodicBalanceAddonRepository.Save();
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
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<PartialViewResult> EditCustomerPeriodicBalanceAddon(int id)
        {
            var item = await _customerPeriodicBalanceAddonRepository.Get(id);
            var persianDate = new PersianDateTime(item.Date);

            return PartialView("~/Views/Report/Edit/CustomerPeriodicBalanceAddon.cshtml", new EditCustomerPeriodicBalanceAddonVM
            {
                Day = persianDate.Day,
                Month = persianDate.Month,
                Year = persianDate.Year,
                Id = item.Id,
                Amount = item.Amount,
                Title = item.Title,
                IsPositive = item.IsPositive,
                CustomerPeriodicBalanceSummaryId = item.CustomerPeriodicBalanceSummaryId
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> EditCustomerPeriodicBalanceAddon(EditCustomerPeriodicBalanceAddonVM c)
        {
            if (ModelState.IsValid)
            {
                var item = await _customerPeriodicBalanceAddonRepository.Get(c.Id);
                item.Date = new PersianDateTime(c.Year, c.Month, c.Day).ToDateTime();
                item.Title = c.Title;
                item.IsPositive = c.IsPositive;
                item.CustomerPeriodicBalanceSummaryId = c.CustomerPeriodicBalanceSummaryId;
                item.Amount = c.Amount;

                _customerPeriodicBalanceAddonRepository.Update(item);
                try
                {
                    await _customerPeriodicBalanceAddonRepository.Save();
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
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> DeleteCustomerPeriodicBalanceAddon(int id)
        {
            var item = await _customerPeriodicBalanceAddonRepository.Get(id);
            if (item == null) return NotFound();

            _customerPeriodicBalanceAddonRepository.Delete(item);
            try
            {
                await _customerPeriodicBalanceAddonRepository.Save();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        #endregion

        public async Task<IActionResult> CustomerBalanceByPeriod(long id)
        {
            var periodItem = await _customerPeriodicBalanceSummaryRepository.Query().Include(a => a.Customer).Include(a => a.CustomerPeriodicBalanceAddons).AsNoTracking().SingleOrDefaultAsync(a => a.Id.Equals(id));
            if (periodItem == null) return NotFound();

            ViewData["CustomerPeriodicBalanceAddon"] = periodItem.CustomerPeriodicBalanceAddons.ToList();
            ViewData["BalanceAmount"] = periodItem.BalanceAmount;
            ViewData["InsuranceBalanceAmount"] = periodItem.InsuranceBalanceAmount;
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.Sequence).ToListAsync();

            var startDate = periodItem.StartDate;
            var endDate = periodItem.EndDate > DateTime.Now ? DateTime.Now : periodItem.EndDate;

            var startCalendarSequence = await _calendarRepo.Calendars().Where(a => a.StartDate.Equals(startDate)).Select(a => a.Sequence).FirstOrDefaultAsync();
            var calendars = await _calendarRepo.Calendars().Where(a => a.Sequence >= startCalendarSequence &&
            a.Sequence <= _calendarRepo.Calendars().Where(a => a.StartDate < endDate && a.EndDate >= endDate).Single().Sequence).OrderBy(a => a.Sequence).ToListAsync();

            var customerIncomes = await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CustomerId.Equals(periodItem.CustomerId)).OrderBy(a => a.Date).ToListAsync();

            var customerFactors = await _customerFactorRepository.Query().AsNoTracking().Where(a => a.CustomerId.Equals(periodItem.CustomerId)).OrderBy(a => a.Date).ToListAsync();

            var monthCount = await _calendarRepo.Calendars().AsNoTracking().CountAsync(a => a.Sequence >=
            _calendarRepo.Calendars().AsNoTracking().Where(b => b.StartDate >= startDate).OrderBy(b => b.StartDate).First().Sequence &&
            a.Sequence <= _calendarRepo.Calendars().AsNoTracking().Where(b => b.EndDate <= endDate).OrderByDescending(b => b.EndDate).First().Sequence);

            var data = new CustomerBalanceVM
            {
                Calendars = calendars,
                Customer = periodItem.Customer,
                StartDate = startDate,
                EndDate = endDate,
                Details = new List<CustomerBalanceDetailVM>()
            };

            foreach (var calendar in calendars)
            {
                data.Details.Add(new CustomerBalanceDetailVM
                {
                    CalendarId = calendar.Id,
                    CustomerIncomes = customerIncomes.Where(a => a.Date >= calendar.StartDate && a.Date < calendar.EndDate).ToList(),
                    CustomerFactors = customerFactors.Where(a => a.CalendarId.Equals(calendar.Id)).ToList()
                });
            }

            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerBalance(/*long contractId,*/ long customerId, int? period/*, bool type = false*/)
        {
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.Sequence).ToListAsync();
            //var contract = await _contractRepository.Get(contractId);
            var customer = await _customerRepo.Get(customerId);

            var initialDate = DateTime.Now.AddDays(period ?? 0);

            var endDateInstance = new PersianDateTime(initialDate);
            var startDateInstance = endDateInstance.AddMonths(-3);

            var startDate = new PersianDateTime(startDateInstance.Year, startDateInstance.Month, 1).ToDateTime();
            var endDate = endDateInstance.ToDateTime();

            var startCalendarSequence = await _calendarRepo.Calendars().Where(a => a.StartDate.Equals(startDate)).Select(a => a.Sequence).FirstOrDefaultAsync();
            var beforeStartCalendarIdList = await _calendarRepo.Calendars().Where(a => a.Sequence < startCalendarSequence).Select(a => a.Id).OrderByDescending(a => a).ToListAsync();
            var calendars = await _calendarRepo.Calendars().Where(a => a.Sequence >= startCalendarSequence &&
            a.Sequence <= _calendarRepo.Calendars().Where(a => a.StartDate < endDate && a.EndDate >= endDate).Single().Sequence).OrderBy(a => a.Sequence).ToListAsync();

            var customerIncomes = await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CustomerId.Equals(customerId)).OrderBy(a => a.Date).ToListAsync();

            var customerFactors = await _customerFactorRepository.Query().AsNoTracking().Where(a => a.CustomerId.Equals(customerId)).OrderBy(a => a.Date).ToListAsync();

            var monthCount = await _calendarRepo.Calendars().AsNoTracking().CountAsync(a => a.Sequence >=
            _calendarRepo.Calendars().AsNoTracking().Where(b => b.StartDate >= startDate).OrderBy(b => b.StartDate).First().Sequence &&
            a.Sequence <= _calendarRepo.Calendars().AsNoTracking().Where(b => b.EndDate <= endDate).OrderByDescending(b => b.EndDate).First().Sequence);

            //var now = DateTime.Now;
            //var initialDate = now > contract.EndDate ? contract.EndDate : now;
            //var endDateInstance = new PersianDateTime(initialDate);
            //var endDate = endDateInstance.ToDateTime();
            //var endCalendarSequence = await _calendarRepo.Calendars().Where(a => a.StartDate < startDate && a.EndDate >= endDate).Select(a => a.Sequence).FirstOrDefaultAsync();

            //var startDateCounter = type ? -(monthCount - 1) : monthCount < 3 ? -(monthCount - 1) : -2;

            //var startDateInstance = endDateInstance.AddMonths(startDateCounter);
            //var startDate = new PersianDateTime(startDateInstance.Year, startDateInstance.Month, 1).ToDateTime();

            //var startCalendarSequence = await _calendarRepo.Calendars().Where(a => a.StartDate.Equals(startDate)).Select(a => a.Sequence).FirstOrDefaultAsync();
            //var beforeStartCalendarIdList = await _calendarRepo.Calendars().Where(a => a.Sequence < startCalendarSequence).Select(a => a.Id).OrderByDescending(a => a).ToListAsync();
            //var calendars = await _calendarRepo.Calendars().Where(a => endCalendarSequence > 0 ? a.Sequence >= startCalendarSequence && a.Sequence <= endCalendarSequence : a.Sequence >= startCalendarSequence).OrderBy(a => a.Sequence).ToListAsync();

            var beforeIncomes = customerIncomes.Where(a => a.Date < startDate).ToList();
            var beforeFactors = customerFactors.Where(a => a.Date < startDate).ToList();

            //var afterIncomes = customerIncomes.Where(a => a.Date >= endDate).ToList();
            //var afterFactors = customerFactors.Where(a => a.Date >= endDate).ToList();

            var data = new CustomerBalanceVM
            {
                Calendars = calendars,
                Customer = customer,
                StartDate = startDate,
                EndDate = endDate,
                Details = new List<CustomerBalanceDetailVM>()
            };

            if (beforeFactors.Any() || beforeIncomes.Any())
                data.Details.Add(new CustomerBalanceDetailVM
                {
                    CustomerIncomes = beforeIncomes,
                    CalendarId = -1,
                    CustomerFactors = beforeFactors
                });

            foreach (var calendar in calendars)
            {
                data.Details.Add(new CustomerBalanceDetailVM
                {
                    CalendarId = calendar.Id,
                    CustomerIncomes = customerIncomes.Where(a => a.Date >= calendar.StartDate && a.Date < calendar.EndDate).ToList(),
                    CustomerFactors = customerFactors.Where(a => a.CalendarId.Equals(calendar.Id)).ToList()
                });
            }

            //if (afterFactors.Any() || afterIncomes.Any())
            //    data.Details.Add(new CustomerBalanceDetailVM
            //    {
            //        CustomerIncomes = afterIncomes,
            //        CalendarId = 0,
            //        CustomerFactors = afterFactors
            //    });

            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerBalance(long customerId, long fromCalendarId, long toCalendarId)
        {
            var customer = await _customerRepo.Get(customerId);

            var fromCalendar = await _calendarRepo.Get(fromCalendarId);
            var toCalendar = await _calendarRepo.Get(toCalendarId);

            var startDate = fromCalendar.StartDate;
            var endDate = toCalendar.EndDate;

            var startCalendarSequence = fromCalendar.Sequence;
            var calendars = await _calendarRepo.Calendars().Where(a => a.Sequence >= startCalendarSequence && a.Sequence <= toCalendar.Sequence).OrderBy(a => a.Sequence).ToListAsync();

            var customerIncomes = await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CustomerId.Equals(customerId)
            && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Date).ToListAsync();

            var customerFactors = await _customerFactorRepository.Query().AsNoTracking().Where(a => a.CustomerId.Equals(customerId)
            && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Date).ThenBy(a => a.FactorNumber).ToListAsync();

            //var monthCount = await _calendarRepo.Calendars().AsNoTracking().CountAsync(a => a.Sequence >=
            //_calendarRepo.Calendars().AsNoTracking().Where(b => b.StartDate >= startDate).OrderBy(b => b.StartDate).First().Sequence &&
            //a.Sequence <= _calendarRepo.Calendars().AsNoTracking().Where(b => b.EndDate <= endDate).OrderByDescending(b => b.EndDate).First().Sequence);

            //var now = DateTime.Now;
            //var initialDate = now > contract.EndDate ? contract.EndDate : now;
            //var endDateInstance = new PersianDateTime(initialDate);
            //var endDate = endDateInstance.ToDateTime();
            //var endCalendarSequence = await _calendarRepo.Calendars().Where(a => a.StartDate < startDate && a.EndDate >= endDate).Select(a => a.Sequence).FirstOrDefaultAsync();

            //var startDateCounter = type ? -(monthCount - 1) : monthCount < 3 ? -(monthCount - 1) : -2;

            //var startDateInstance = endDateInstance.AddMonths(startDateCounter);
            //var startDate = new PersianDateTime(startDateInstance.Year, startDateInstance.Month, 1).ToDateTime();

            //var startCalendarSequence = await _calendarRepo.Calendars().Where(a => a.StartDate.Equals(startDate)).Select(a => a.Sequence).FirstOrDefaultAsync();
            //var beforeStartCalendarIdList = await _calendarRepo.Calendars().Where(a => a.Sequence < startCalendarSequence).Select(a => a.Id).OrderByDescending(a => a).ToListAsync();
            //var calendars = await _calendarRepo.Calendars().Where(a => endCalendarSequence > 0 ? a.Sequence >= startCalendarSequence && a.Sequence <= endCalendarSequence : a.Sequence >= startCalendarSequence).OrderBy(a => a.Sequence).ToListAsync();

            //var beforeIncomes = customerIncomes.Where(a => a.Date < startDate).ToList();
            //var beforeFactors = customerFactors.Where(a => a.Date < startDate).ToList();

            //var afterIncomes = customerIncomes.Where(a => a.Date >= endDate).ToList();
            //var afterFactors = customerFactors.Where(a => a.Date >= endDate).ToList();

            var data = new CustomerBalanceVM
            {
                Calendars = calendars,
                Customer = customer,
                StartDate = startDate,
                EndDate = endDate,
                Details = new List<CustomerBalanceDetailVM>()
            };

            //if (beforeFactors.Any() || beforeIncomes.Any())
            //    data.Details.Add(new CustomerBalanceDetailVM
            //    {
            //        CustomerIncomes = beforeIncomes,
            //        CalendarId = -1,
            //        CustomerFactors = beforeFactors
            //    });

            foreach (var calendar in calendars)
            {
                data.Details.Add(new CustomerBalanceDetailVM
                {
                    CalendarId = calendar.Id,
                    CustomerIncomes = customerIncomes.Where(a => a.Date >= calendar.StartDate && a.Date < calendar.EndDate).ToList(),
                    CustomerFactors = customerFactors.Where(a => a.CalendarId.Equals(calendar.Id)).ToList()
                });
            }

            return PartialView("_CustomerBalance", data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> CustomerSeparateRoute(long customerId, long calendarId)
        {
            var routes = await _loadRoutesRepository.LoadRoutes().AsNoTracking().ToListAsync();
            var calendar = await _calendarRepo.Calendars().AsNoTracking().SingleAsync(a => a.Id.Equals(calendarId));
            var customer = await _customerRepo.Customers().AsNoTracking().SingleAsync(a => a.Id.Equals(customerId));
            var contracts = await _contractRepository.Contracts().AsNoTracking().Where(a => a.CustomerId.Equals(customerId)).Select(a => a.Id).ToListAsync();
            var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(calendarId) && contracts.Contains(a.ContractId)).ToListAsync();

            var data = new CustomerSeparateRouteVM
            {
                Title = $"گزارش پرداختی به تفکیک مسیر شرکت {customer.Name} در {calendar.Title}",
                DriverLoadSleepPrice = loadFactors.Where(a => a.DriverLoadSleepPrice.HasValue).Sum(a => a.DriverLoadSleepPrice.Value),
                WeighbridgeAmount = loadFactors.Where(a => a.WeighbridgePrice.HasValue).Sum(a => a.WeighbridgePrice.Value),
                TonnageAmount = loadFactors.Where(a => a.Tonnage.HasValue && a.DriverTonnagePrice.HasValue).Sum(a => a.Tonnage.Value * a.DriverTonnagePrice.Value),
                Details = new List<CustomerSeparateRouteDetailVM>()
            };

            var loadFactorsWithShippingFeeIdExistance = loadFactors.Any(a => a.ShippingFeeId.HasValue);
            var loadFactorsWithShippingFeeGroupIdExistance = loadFactors.Any(a => a.ShippingFeeGroupId.HasValue);

            //old shipping fee
            if (loadFactorsWithShippingFeeIdExistance)
            {
                var prices = loadFactors.Where(a => a.ShippingFeeId.HasValue).DistinctBy(a => a.DriverFee).Select(a => new { a.DriverFee, a.ShippingFeeId }).ToList();
                var shippingFeeIds = loadFactors.Where(a => a.ShippingFeeId.HasValue).DistinctBy(a => a.ShippingFeeId).Select(a => a.ShippingFeeId);
                var shippingFees = await _shippingFeeRepository.ShippingFees().AsNoTracking()
                    .Where(a => shippingFeeIds.Contains(a.Id))
                    .Select(a => new { a.Vehicle, a.Id }).ToListAsync();
                foreach (var item in prices.OrderBy(a => a.DriverFee))
                {
                    var originIdList = loadFactors.Where(a => a.ShippingFeeId.HasValue && a.DriverFee.Equals(item.DriverFee)).DistinctBy(a => a.OriginId).Select(a => a.OriginId).ToList();
                    var origins = routes.Where(a => originIdList.Contains(a.Id)).Select(a => a.Title).ToList();
                    var destinationIdList = loadFactors.Where(a => a.ShippingFeeId.HasValue && a.DriverFee.Equals(item.DriverFee)).DistinctBy(a => a.DestinationId).Select(a => a.DestinationId).ToList();
                    var destinations = routes.Where(a => destinationIdList.Contains(a.Id)).Select(a => a.Title).ToList();

                    data.Details.Add(new CustomerSeparateRouteDetailVM
                    {
                        Amount = item.DriverFee,
                        Quantity = loadFactors.Where(a => a.ShippingFeeId.HasValue).Count(a => a.DriverFee.Equals(item.DriverFee)),
                        Origins = origins,
                        Destinaitons = destinations,
                        Vehicle = shippingFees.Single(a => a.Id.Equals(item.ShippingFeeId.Value)).Vehicle
                    });
                }
            }

            //new shipping fee
            if (loadFactorsWithShippingFeeGroupIdExistance)
            {
                var prices = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue).DistinctBy(a => a.DriverFee).Select(a => new { a.DriverFee, a.ShippingFeeGroupId }).ToList();
                var shippingFeeIds = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue).DistinctBy(a => a.ShippingFeeGroupId).Select(a => a.ShippingFeeGroupId);
                var shippingFees = await _shippingFeeGroupRepository.Query().AsNoTracking()
                    .Where(a => shippingFeeIds.Contains(a.Id))
                    .Select(a => new { a.Vehicle, a.Id }).ToListAsync();
                foreach (var item in prices.OrderBy(a => a.DriverFee))
                {
                    var originIdList = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue && a.DriverFee.Equals(item.DriverFee)).DistinctBy(a => a.OriginId).Select(a => a.OriginId).ToList();
                    var origins = routes.Where(a => originIdList.Contains(a.Id)).Select(a => a.Title).ToList();
                    var destinationIdList = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue && a.DriverFee.Equals(item.DriverFee)).DistinctBy(a => a.DestinationId).Select(a => a.DestinationId).ToList();
                    var destinations = routes.Where(a => destinationIdList.Contains(a.Id)).Select(a => a.Title).ToList();

                    data.Details.Add(new CustomerSeparateRouteDetailVM
                    {
                        Amount = item.DriverFee,
                        Quantity = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue).Count(a => a.DriverFee.Equals(item.DriverFee)),
                        Origins = origins,
                        Destinaitons = destinations,
                        Vehicle = shippingFees.Single(a => a.Id.Equals(item.ShippingFeeGroupId)).Vehicle
                    });
                }
            }

            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> CustomerSeparateRouteIncome(long customerId, long calendarId)
        {
            var routes = await _loadRoutesRepository.LoadRoutes().AsNoTracking().ToListAsync();
            var calendar = await _calendarRepo.Calendars().AsNoTracking().SingleAsync(a => a.Id.Equals(calendarId));
            var customer = await _customerRepo.Customers().AsNoTracking().SingleAsync(a => a.Id.Equals(customerId));
            var contracts = await _contractRepository.Contracts().AsNoTracking().Where(a => a.CustomerId.Equals(customerId)).Select(a => a.Id).ToListAsync();
            var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(calendarId) && contracts.Contains(a.ContractId)).ToListAsync();

            var data = new CustomerSeparateRouteVM
            {
                Title = $"گزارش دریافتی به تفکیک مسیر شرکت {customer.Name} در {calendar.Title}",
                DriverLoadSleepPrice = loadFactors.Where(a => a.LoadSleepPrice.HasValue).Sum(a => a.LoadSleepPrice.Value),
                WeighbridgeAmount = loadFactors.Where(a => a.WeighbridgePrice.HasValue).Sum(a => a.WeighbridgePrice.Value),
                TonnageAmount = loadFactors.Where(a => a.Tonnage.HasValue && a.TonnagePrice.HasValue).Sum(a => a.Tonnage.Value * a.TonnagePrice.Value),
                Details = new List<CustomerSeparateRouteDetailVM>()
            };

            var loadFactorsWithShippingFeeIdExistance = loadFactors.Any(a => a.ShippingFeeId.HasValue);
            var loadFactorsWithShippingFeeGroupIdExistance = loadFactors.Any(a => a.ShippingFeeGroupId.HasValue);

            //old shipping fee
            if (loadFactorsWithShippingFeeIdExistance)
            {
                var prices = loadFactors.Where(a => a.ShippingFeeId.HasValue).DistinctBy(a => a.Amount).Select(a => new { a.Amount, a.ShippingFeeId }).ToList();
                var shippingFeeIds = loadFactors.Where(a => a.ShippingFeeId.HasValue).DistinctBy(a => a.ShippingFeeId).Select(a => a.ShippingFeeId);
                var shippingFees = await _shippingFeeRepository.ShippingFees().AsNoTracking()
                    .Where(a => shippingFeeIds.Contains(a.Id))
                    .Select(a => new { a.Vehicle, a.Id }).ToListAsync();
                foreach (var item in prices.OrderBy(a => a.Amount))
                {
                    var originIdList = loadFactors.Where(a => a.ShippingFeeId.HasValue && a.Amount.Equals(item.Amount)).DistinctBy(a => a.OriginId).Select(a => a.OriginId).ToList();
                    var origins = routes.Where(a => originIdList.Contains(a.Id)).Select(a => a.Title).ToList();
                    var destinationIdList = loadFactors.Where(a => a.ShippingFeeId.HasValue && a.Amount.Equals(item.Amount)).DistinctBy(a => a.DestinationId).Select(a => a.DestinationId).ToList();
                    var destinations = routes.Where(a => destinationIdList.Contains(a.Id)).Select(a => a.Title).ToList();

                    data.Details.Add(new CustomerSeparateRouteDetailVM
                    {
                        Amount = item.Amount,
                        Quantity = loadFactors.Where(a => a.ShippingFeeId.HasValue).Count(a => a.Amount.Equals(item.Amount)),
                        Origins = origins,
                        Destinaitons = destinations,
                        Vehicle = shippingFees.Single(a => a.Id.Equals(item.ShippingFeeId.Value)).Vehicle
                    });
                }
            }

            //new shipping fee
            if (loadFactorsWithShippingFeeGroupIdExistance)
            {
                var prices = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue).DistinctBy(a => a.Amount).Select(a => new { a.Amount, a.ShippingFeeGroupId }).ToList();
                var shippingFeeIds = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue).DistinctBy(a => a.ShippingFeeGroupId).Select(a => a.ShippingFeeGroupId);
                var shippingFees = await _shippingFeeGroupRepository.Query().AsNoTracking()
                    .Where(a => shippingFeeIds.Contains(a.Id))
                    .Select(a => new { a.Vehicle, a.Id }).ToListAsync();
                foreach (var item in prices.OrderBy(a => a.Amount))
                {
                    var originIdList = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue && a.Amount.Equals(item.Amount)).DistinctBy(a => a.OriginId).Select(a => a.OriginId).ToList();
                    var origins = routes.Where(a => originIdList.Contains(a.Id)).Select(a => a.Title).ToList();
                    var destinationIdList = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue && a.Amount.Equals(item.Amount)).DistinctBy(a => a.DestinationId).Select(a => a.DestinationId).ToList();
                    var destinations = routes.Where(a => destinationIdList.Contains(a.Id)).Select(a => a.Title).ToList();

                    data.Details.Add(new CustomerSeparateRouteDetailVM
                    {
                        Amount = item.Amount,
                        Quantity = loadFactors.Where(a => a.ShippingFeeGroupId.HasValue).Count(a => a.Amount.Equals(item.Amount)),
                        Origins = origins,
                        Destinaitons = destinations,
                        Vehicle = shippingFees.Single(a => a.Id.Equals(item.ShippingFeeGroupId.Value)).Vehicle
                    });
                }
            }

            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> GetHasCapacityUnrealVehicles(long calendarId)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            if (calendar == null) return NotFound();

            var unrealVehicles = await _vehicleRepo.Vehicles().AsNoTracking().Where(a => !a.RealStatus && a.Status).OrderBy(a => a.LeftNumber).ThenBy(a => a.RightNumber).ToListAsync();
            var usedVehicles = await _billRepository.Query().AsNoTracking().Where(a => a.CalendarId.Equals(calendarId) && a.VehicleId.HasValue && unrealVehicles.Select(a => a.Id).Contains(a.VehicleId.Value)).Select(a => a.VehicleId.Value).Distinct().ToListAsync();
            using (var db = new ApplicationDbContext())
            {
                var otherCosts = await db.OtherCost.AsNoTracking().Where(a => a.CalendarId.Equals(calendarId) && unrealVehicles.Select(a => a.Id).Contains(a.VehicleId)).Select(a => a.VehicleId).Distinct().ToListAsync();

                usedVehicles.AddRange(otherCosts);
            }

            ViewData["Calendar"] = calendar;

            return View(unrealVehicles.Where(a => !usedVehicles.Contains(a.Id)).ToList());
        }
    }
}
