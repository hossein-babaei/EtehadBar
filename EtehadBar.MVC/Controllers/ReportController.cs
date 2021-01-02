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

        //public async Task<IActionResult> Customer(int? id, string statusNumber, string start, string end)
        //{
        //    if (Request.IsAjaxRequest())
        //    {
        //        var S = start.PersianToEnglish().Split('/');
        //        var startDate = new PersianDateTime(
        //            Convert.ToInt32(S[0]),
        //            Convert.ToInt32(S[1]),
        //            Convert.ToInt32(S[2]), 0, 0, 0, 0).ToDateTime();
        //        var E = end.PersianToEnglish().Split('/');
        //        var endDate = new PersianDateTime(
        //            Convert.ToInt32(E[0]),
        //            Convert.ToInt32(E[1]),
        //            Convert.ToInt32(E[2]), 23, 59, 59, 99).ToDateTime();

        //        ViewData["start"] = startDate;
        //        ViewData["end"] = endDate;
        //        ViewData["statusNumber"] = statusNumber;
        //        var customer = await db.Customer.AsNoTracking().Where(a => a.Id.Equals(id.Value)).SingleAsync();

        //        ViewData["customer"] = customer;

        //        if (customer.Name.Contains("پلاسکو"))
        //        {
        //            return PartialView("_Plasco", await db.LoadFactor.Where(a => a.CustomerId.Equals(id.Value) && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Counter).ToListAsync());
        //        }

        //        return NotFound();
        //    }
        //    else
        //    {
        //        if (id.HasValue)
        //        {
        //            var pdNow = PersianDateTime.Now;
        //            var startDate = new PersianDateTime(pdNow.Year, pdNow.Month, 1, 0, 0, 0, 0).ToDateTime();
        //            var endDate = pdNow.ToDateTime();

        //            ViewData["start"] = startDate;
        //            ViewData["end"] = endDate;
        //            ViewData["statusNumber"] = statusNumber;
        //            var customer = await db.Customer.AsNoTracking().Where(a => a.Id.Equals(id.Value)).SingleAsync();

        //            ViewData["customer"] = customer;

        //            if (customer.Name.Contains("پلاسکو"))
        //            {
        //                return View("Plasco", await db.LoadFactor.Where(a => a.CustomerId.Equals(id.Value) && a.Date >= startDate && a.Date <= endDate).OrderBy(a => a.Counter).ToListAsync());
        //            }

        //            return NotFound();
        //        }
        //        else
        //        {
        //            return View("CustomerList", await db.Customer.OrderBy(a => a.Name).ToListAsync());
        //        }
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> CustomerIncome(int? id)
        {
            if (!id.HasValue)
                return BadRequest("parameter error");

            if (!await _customerRepo.Customers().AnyAsync(a => a.Id.Equals(id.Value)))
                return NotFound("مشتری پیدا نشد");

            ViewData["customer"] = await _customerRepo.Get(id.Value);

            var caledndars = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["calendars"] = caledndars;

            return View(await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CalendarId.Equals(caledndars.First().Id)).OrderBy(a => a.Date).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CustomerIncome(int? id, string calendarId)
        {
            ViewData["customer"] = await _customerRepo.Get(id.Value);
            ViewData["calendar"] = await _calendarRepo.Get(calendarId);

            return PartialView("_CustomerIncome", await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CalendarId.Equals(calendarId)).OrderBy(a => a.Date).ToListAsync());
        }
    }
}
