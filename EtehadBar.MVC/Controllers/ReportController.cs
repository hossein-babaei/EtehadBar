using Castle.Core.Resource;
using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
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
        private readonly IPaymentRepository _paymentRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IFreeLoadFactorRepository _freeLoadFactorRepository;
        private readonly IBillRepository _billRepository;
        private readonly IVehicleBalanceRepository _vehicleBalanceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITurnoverRepository _turnoverRepository;

        public ReportController(
            ICalendarRepository calendarRepository,
            ICostRepository costRepository,
            ICustomerRepository customerRepository,
            ILoadFactorRepository loadFactorRepository,
            IPaymentRepository paymentRepository,
            IVehicleRepository vehicleRepository,
            UserManager<ApplicationUser> userManager,
            IFreeLoadFactorRepository freeLoadFactorRepository,
            IBillRepository billRepository,
            IVehicleBalanceRepository vehicleBalanceRepository,
            ITurnoverRepository turnoverRepository)
        {
            _calendarRepo = calendarRepository;
            _costRepo = costRepository;
            _customerRepo = customerRepository;
            _loadFactorRepo = loadFactorRepository;
            _paymentRepo = paymentRepository;
            _vehicleRepo = vehicleRepository;
            _userManager = userManager;
            _freeLoadFactorRepository = freeLoadFactorRepository;
            _billRepository = billRepository;
            _vehicleBalanceRepository = vehicleBalanceRepository;
            _turnoverRepository = turnoverRepository;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerIncome(long? id)
        {
            if (!id.HasValue)
                return BadRequest("parameter error");

            if (!await _customerRepo.Customers().AnyAsync(a => a.Id.Equals(id.Value)))
                return NotFound("مشتری پیدا نشد");

            ViewData["customer"] = await _customerRepo.Get(id.Value);

            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;

            return View(await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CalendarId.Equals(calendars.First().Id) && a.CustomerId.Equals(id.Value)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerIncome(long? id, long calendarId)
        {
            ViewData["customer"] = await _customerRepo.Get(id.Value);
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);

            return PartialView("_CustomerIncome", await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CalendarId.Equals(calendarId) && a.CustomerId.Equals(id.Value)).OrderBy(a => a.Date).ToListAsync());
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
            ViewData["income"] = await _customerRepo.CustomerIncomes().Where(a => a.CalendarId.Equals(latestCal.Id)).SumAsync(a => a.Amount);
            return View(await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(latestCal.Id)).OrderBy(a => a.Date).ToListAsync());
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
            ViewData["income"] = await _customerRepo.CustomerIncomes().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);

            var data = new List<GlobalLoadFactorVM>();
            var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(calendarId)).Select(a => new GlobalLoadFactorVM
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

            var query = _costRepo.Costs();
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.UserId.Equals(_userManager.GetUserId(User)));

            var latestCal = calendars.First();
            return View(await query.Where(a => a.CalendarId.Equals(latestCal.Id)).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User, Milad")]
        public async Task<IActionResult> Cost(long calendarId, string userId)
        {
            ViewData["userId"] = userId;
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);

            var query = _costRepo.Costs().Where(a => a.CalendarId.Equals(calendarId));
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.UserId.Equals(_userManager.GetUserId(User)));

            if (userId != "all")
                query = query.Where(a => a.UserId.Equals(userId));

            return PartialView("_Cost", await query.ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> Payment()
        {
            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;

            var latestCal = calendars.First();
            return View(await _paymentRepo.PaymentVMList(latestCal.Id, null, null));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> Payment(long calendarId, byte type, long vehicleId)
        {
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);
            ViewData["type"] = type;
            ViewData["vehicleId"] = vehicleId;

            return PartialView("_Payment", await _paymentRepo.PaymentVMList(calendarId, type, vehicleId));
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> VehicleLoadFactor()
        {
            ViewData["calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["customers"] = await _customerRepo.Customers().AsNoTracking().Where(a => a.Status).OrderBy(a => a.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> VehicleLoadFactor(long calendarId, long vehicleId, long customerId)
        {
            ViewData["vehicle"] = await _vehicleRepo.Get(vehicleId);
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);
            ViewData["Balance"] = await _vehicleBalanceRepository.GetVehicleBalanceSum(vehicleId, calendarId, customerId == 0 ? null : customerId);
            ViewData["customerId"] = customerId;

            var query = _loadFactorRepo.LoadFactors().Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId));
            if (customerId > 0)
                query = query.Where(a => a.Contract.CustomerId.Equals(customerId));

            return PartialView("_VehicleLoadFactor", await query.OrderBy(a => a.Id).ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> VehicleActivity()
        {
            ViewData["calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["customers"] = await _customerRepo.Customers().AsNoTracking().Where(a => a.Status).OrderBy(a => a.Name).ToListAsync();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> VehicleActivity(long calendarId, long vehicleId, long customerId)
        {
            ViewData["vehicle"] = await _vehicleRepo.Get(vehicleId);
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);
            ViewData["Balance"] = await _vehicleBalanceRepository.GetVehicleBalanceSum(vehicleId, calendarId, customerId == 0 ? null : customerId);
            ViewData["customerId"] = customerId;

            var query = _loadFactorRepo.LoadFactors().Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId));
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Turnover()
        {
            var startOfPersianYear = new PersianDateTime(new PersianDateTime(DateTime.Now).Year, 1, 1).ToDateTime();
            return View(await _turnoverRepository.Query().Where(a => a.Date >= startOfPersianYear).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Turnover(string startDate, string endDate)
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
                return PartialView("_Turnover", await _turnoverRepository.Query().Where(a => a.Date >= startD && a.Date <= endD).OrderBy(a => a.Date).ToListAsync());
            }
        }
    }
}
