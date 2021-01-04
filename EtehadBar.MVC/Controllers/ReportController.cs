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
                    (byte)Customers.SaipaPlasco => PartialView("_Plasco"/*, await db.LoadFactor.Where(a => a.CustomerId.Equals(id.Value) && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Counter).ToListAsync()*/),
                    (byte)Customers.SaipaPress => PartialView("_SaipaPress"),
                    (byte)Customers.SazehGostar => PartialView("_SazehGostar"),
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
                        (byte)Customers.SaipaPlasco => View("Plasco"/*, await db.LoadFactor.Where(a => a.CustomerId.Equals(id.Value) && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Counter).ToListAsync()*/),
                        (byte)Customers.SaipaPress => View("SaipaPress"),
                        (byte)Customers.SazehGostar => View("SazehGostar"),
                        _ => NotFound(),
                    };
                }
                else
                {
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
    }
}
