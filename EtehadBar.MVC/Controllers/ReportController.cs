using Castle.Core.Resource;
using DocumentFormat.OpenXml.Drawing.Charts;
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
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IFreeLoadFactorRepository _freeLoadFactorRepository;
        private readonly IBillRepository _billRepository;
        private readonly IVehicleBalanceRepository _vehicleBalanceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITurnoverRepository _turnoverRepository;
        private readonly IAccountBookRepository _accountBookRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ICustomerFactorRepository _customerFactorRepository;

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
            IAccountBookRepository accountBookRepository,
            IContractRepository contractRepository,
            ICustomerFactorRepository customerFactorRepository)
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
            _accountBookRepository = accountBookRepository;
            _contractRepository = contractRepository;
            _customerFactorRepository = customerFactorRepository;
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

            var firstCalendar = calendars.First();

            return View(await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CustomerId.Equals(id.Value) && a.Date >= firstCalendar.StartDate && a.Date <= firstCalendar.EndDate).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
            ViewData["income"] = await _customerRepo.CustomerIncomes().Where(a => a.Date >= calendar.StartDate && a.Date <= calendar.EndDate).SumAsync(a => a.Amount);

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
        public async Task<IActionResult> Turnover(string startDate, string endDate, TurnoverType turnoverType)
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerBalance(/*long contractId,*/ long customerId, int? period/*, bool type = false*/)
        {
            ViewData["Calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderBy(a => a.Sequence).ToListAsync();
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
                    CustomerFactors = customerFactors.Where(a => a.Date >= calendar.StartDate && a.Date < calendar.EndDate).ToList()
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
                    CustomerFactors = customerFactors.Where(a => a.Date >= calendar.StartDate && a.Date < calendar.EndDate).ToList()
                });
            }

            return PartialView("_CustomerBalance", data);
        }
    }
}
