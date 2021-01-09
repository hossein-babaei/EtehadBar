using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using Helpers;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.MVC.Controllers
{
    public class ReportController : Controller
    {
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
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(
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
            UserManager<ApplicationUser> userManager)
        {
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
            _userManager = userManager;
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

        public async Task<IActionResult> Customer(int? id, string statusNumber, string calendarId)
        {
            if (Request.IsAjaxRequest())
            {
                ViewData["statusNumber"] = statusNumber;

                var calendar = await _calendarRepo.Get(calendarId);
                ViewData["calendar"] = calendar;

                var customer = await _customerRepo.Get(id.Value);
                ViewData["customer"] = customer;

                return customer.Type switch
                {
                    (byte)Customers.SaipaPlasco => PartialView("~/Views/Report/Customer/_Plasco.cshtml"/*, await db.LoadFactor.Where(a => a.CustomerId.Equals(id.Value) && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Counter).ToListAsync()*/),
                    (byte)Customers.SaipaPress => PartialView("~/Views/Report/Customer/_SaipaPress.cshtml"),
                    (byte)Customers.SazehGostar => PartialView("~/Views/Report/Customer/_SazehGostar.cshtml"),
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

                    return customer.Type switch
                    {
                        (byte)Customers.SaipaPlasco => View("~/Views/Report/Customer/Plasco.cshtml"/*, await db.LoadFactor.Where(a => a.CustomerId.Equals(id.Value) && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Counter).ToListAsync()*/),
                        (byte)Customers.SaipaPress => View("~/Views/Report/Customer/SaipaPress.cshtml"),
                        (byte)Customers.SazehGostar => View("~/Views/Report/Customer/SazehGostar.cshtml"),
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
        public async Task<IActionResult> CustomerIncome(int? id)
        {
            if (!id.HasValue)
                return BadRequest("parameter error");

            if (!await _customerRepo.Customers().AnyAsync(a => a.Id.Equals(id.Value)))
                return NotFound("مشتری پیدا نشد");

            ViewData["customer"] = await _customerRepo.Get(id.Value);

            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;

            return View(await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CalendarId.Equals(calendars.First().Id)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CustomerIncome(int? id, string calendarId)
        {
            ViewData["customer"] = await _customerRepo.Get(id.Value);
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);

            return PartialView("_CustomerIncome", await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CalendarId.Equals(calendarId)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Detailed()
        {
            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;

            var latestCal = calendars.First();
            ViewData["cost"] = await _costRepo.Costs().Where(a => a.CalendarId.Equals(latestCal.Id)).SumAsync(a => a.Amount);
            ViewData["payment"] = await _paymentRepo.Payments().Where(a => a.CalendarId.Equals(latestCal.Id)).SumAsync(a => a.Amount);
            return View(await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(latestCal.Id)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Detailed(string calendarId)
        {
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);

            ViewData["cost"] = await _costRepo.Costs().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            ViewData["payment"] = await _paymentRepo.Payments().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            return PartialView("_Detailed", await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(calendarId)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Cost()
        {
            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;

            var latestCal = calendars.First();
            return View(await _costRepo.Costs().Where(a => a.CalendarId.Equals(latestCal.Id)).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Cost(string calendarId)
        {
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);
            return PartialView("_Cost", await _costRepo.Costs().Where(a => a.CalendarId.Equals(calendarId)).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            var calendars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = calendars;

            var latestCal = calendars.First();
            return View(await _paymentRepo.PaymentVMList(latestCal.Id, null, ""));
        }

        [HttpPost]
        public async Task<IActionResult> Payment(string calendarId, byte type, string vehicleId)
        {
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);
            ViewData["type"] = type;
            ViewData["vehicleId"] = vehicleId;

            return PartialView("_Payment", await _paymentRepo.PaymentVMList(calendarId, type, vehicleId));
        }

        [HttpGet]
        public async Task<IActionResult> VehicleLoadFactor()
        {
            ViewData["calendars"] = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VehicleLoadFactor(string calendarId, string vehicleId)
        {
            ViewData["vehicle"] = await _vehicleRepo.Get(vehicleId);
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);
            ViewData["payment"] = await _paymentRepo.Payments().AsNoTracking().Where(a => a.VehicleId.Equals(vehicleId)).SumAsync(a => a.Amount);
            return PartialView("_VehicleLoadFactor", await _loadFactorRepo.LoadFactors().Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId)).OrderBy(a => a.Counter).ToListAsync());
        }
    }
}
