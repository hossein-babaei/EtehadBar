using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Domain.Models.LoadFactorCreator;
using EtehadBar.Infra.Data.Context;
using EtehadBar.Infra.Data.Repository;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace EtehadBar.MVC.Controllers
{
    [Authorize(Roles = "Admin,Milad")]
    public class LoadFactorCreatorController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICalendarRepository _calendarRepository;
        private readonly IBillRepository _billRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerFactorRepository _customerFactorRepository;

        public LoadFactorCreatorController(ICalendarRepository calendarRepository, IBillRepository billRepository, IVehicleRepository vehicleRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICustomerRepository customerRepository, ICustomerFactorRepository customerFactorRepository)
        {
            _calendarRepository = calendarRepository;
            _billRepository = billRepository;
            _vehicleRepository = vehicleRepository;
            db = context;
            _userManager = userManager;
            _customerRepository = customerRepository;
            _customerFactorRepository = customerFactorRepository;
        }

        public async Task<IActionResult> Index(int? p)
        {
            //var origins = LoadFactorCreatorStaticData.GetOrigins();
            //foreach (var item in origins)
            //{
            //    var price = 85000000;
            //    db.StaticRouteFee.Add(new Domain.Models.LoadFactorCreator.StaticRouteFee
            //    {
            //        Origin = item.Name,
            //        Destination = "اصفهان",
            //        Amount = item.Name == "کردان" ? price + 10000000 : price
            //    });
            //}
            //await db.SaveChangesAsync();

            var pageNumber = p ?? 1;
            ViewBag.data = await _calendarRepository.Calendars().OrderBy(a => a.StartDate).ToPagedListAsync(pageNumber, 15);
            return View();
        }

        public async Task<IActionResult> Create(long id)
        {
            #region OldLogic
            //var origins = LoadFactorCreatorStaticData.GetOrigins();
            //var destination = LoadFactorCreatorStaticData.GetDestinations();
            //var ranges = LoadFactorCreatorStaticData.GetPriceRanges();
            //var data = new List<LoadFactorModel>();

            //var calendar = await _calendarRepository.Get(id);
            //var persianDate = new PersianDateTime(calendar.StartDate);

            //var bills = await _billRepository.Query().Where(a => a.CalendarId.Equals(id) && (a.VehicleId.HasValue &&
            //(_vehicleRepository.Vehicles().Where(b => !b.RealStatus).Select(a => a.Id)).Contains(a.VehicleId.Value))).OrderBy(a => a.Date).ToListAsync();
            //var distinctedBills = bills.DistinctBy(a => a.VehicleId.Value).ToList();

            //foreach (var item in distinctedBills)
            //{
            //    data.Add(new LoadFactorModel
            //    {
            //        DriverName = item.ReceiverName,
            //        VehicleId = item.VehicleId.Value,
            //        VehicleNumber = $"ایران {item.Vehicle.IranStateNumber} - {item.Vehicle.RightNumber} {item.Vehicle.NumberWord} {item.Vehicle.LeftNumber}",
            //        Amount = bills.Where(a => a.VehicleId.Value.Equals(item.VehicleId.Value)).Sum(a => a.Amount)
            //    });
            //}

            //var rnd = new Random();
            //foreach (var item in data)
            //{
            //    var range = ranges.Where(a => a.Minimum <= item.Amount && a.Maximum >= item.Amount).Single();
            //    var amount = item.Amount / range.Divider;

            //    for (int i = 0; i < range.Divider; i++)
            //    {
            //        int day = rnd.Next(1, 30);
            //        item.Details.Add(new LoadFactorDetailModel
            //        {
            //            Day = day,
            //            Amount = amount,
            //            Date = $"{persianDate.ToString("yyyy/MM")}/{(day < 10 ? $"0{day}" : day)}",
            //            Origin = origins.ElementAt(rnd.Next(0, origins.Count - 1)).Name,
            //            Destination = destination.ElementAt(rnd.Next(0, destination.Count - 1)).Name,
            //            LoadFactorNumber = $"{persianDate.Year}/{rnd.Next(rnd.Next(11111111, 19999999))}"
            //        });
            //    }
            //}
            #endregion

            var routes = await db.StaticRouteFee.AsNoTracking().ToListAsync();
            var data = new List<LoadFactorModel>();

            var calendar = await _calendarRepository.Get(id);
            var persianDate = new PersianDateTime(calendar.StartDate);

            var bills = await _billRepository.Query().Include(a => a.Vehicle).Include(a => a.Customer)
                .Where(a => a.CalendarId.Equals(id) && (a.VehicleId.HasValue &&
            (_vehicleRepository.Vehicles().Where(b => !b.RealStatus).Select(a => a.Id)).Contains(a.VehicleId.Value))).ToListAsync();
            var distinctedBills = bills.DistinctBy(a => a.VehicleId.Value).ToList();

            foreach (var item in distinctedBills)
            {
                data.Add(new LoadFactorModel
                {
                    DriverName = item.ReceiverName,
                    CustomerName = item.Customer.Name,
                    VehicleId = item.VehicleId.Value,
                    VehicleLeftNumber = item.Vehicle.LeftNumber,
                    VehicleRightNumber = item.Vehicle.RightNumber,
                    VehicleNumber = $"ایران {item.Vehicle.IranStateNumber} - {item.Vehicle.RightNumber} {item.Vehicle.NumberWord} {item.Vehicle.LeftNumber}",
                    Amount = bills.Where(a => a.VehicleId.Value.Equals(item.VehicleId.Value)).Sum(a => a.Amount)
                });
            }

            var otherCosts = await db.OtherCost.Include(a => a.Vehicle).Include(a => a.Customer).AsNoTracking().Where(a => a.CalendarId.Equals(calendar.Id)).Select(a => new
            {
                a.Amount,
                a.DriverName,
                CustomerName = a.Customer.Name,
                a.VehicleId,
                VehicleLeftNumber = a.Vehicle.LeftNumber,
                VehicleRightNumber = a.Vehicle.RightNumber,
                VehicleNumber = $"ایران {a.Vehicle.IranStateNumber} - {a.Vehicle.RightNumber} {a.Vehicle.NumberWord} {a.Vehicle.LeftNumber}"
            }).ToListAsync();
            var distinctedOtherCosts = otherCosts.DistinctBy(a => a.VehicleNumber).ToList();

            foreach (var item in distinctedOtherCosts)
            {
                //removing duplicate vehicles in bills and other costs
                if (data.Any(a => a.VehicleId.Equals(item.VehicleId) && a.CustomerName.Equals(item.CustomerName)))
                {
                    var dataItem = data.Single(a => a.VehicleId.Equals(item.VehicleId));
                    dataItem.Amount += item.Amount;
                }
                else
                    data.Add(new LoadFactorModel
                    {
                        VehicleRightNumber = item.VehicleRightNumber,
                        VehicleLeftNumber = item.VehicleLeftNumber,
                        DriverName = item.DriverName,
                        VehicleId = item.VehicleId,
                        VehicleNumber = item.VehicleNumber,
                        CustomerName = item.CustomerName,
                        Amount = otherCosts.Where(a => a.VehicleNumber.Equals(item.VehicleNumber)).Sum(a => a.Amount)
                    });
            }

            var rnd = new Random();
            var minimumRouteAmount = routes.Min(a => a.Amount);

            foreach (var item in data)
            {
                var itemAmount = item.Amount;
                var bestRouteAmount = itemAmount / 30;

                List<int> takenDays = new();
                while (itemAmount > 0)
                {
                    int day = 0;

                    day = rnd.Next(1, 30);
                    while (takenDays.Contains(day) && takenDays.Count <= 30)
                        day = rnd.Next(1, 30);
                    takenDays.Add(day);

                    var possibleRoutes = new List<StaticRouteFee>();

                    if (itemAmount >= bestRouteAmount)
                        possibleRoutes = routes.Where(a => a.Amount <= itemAmount && a.Amount >= bestRouteAmount).ToList();
                    else
                        possibleRoutes = routes.Where(a => a.Amount <= itemAmount).ToList();

                    if (possibleRoutes.Any())
                    {
                        var route = possibleRoutes.ElementAt(rnd.Next(0, possibleRoutes.Count - 1));
                        itemAmount -= route.Amount;

                        item.Details.Add(new LoadFactorDetailModel
                        {
                            Day = day,
                            Amount = route.Amount,
                            Date = $"{persianDate.ToString("yyyy/MM")}/{(day < 10 ? $"0{day}" : day)}",
                            Origin = route.Origin,
                            Destination = route.Destination,
                            LoadFactorNumber = $"{persianDate.Year}/{rnd.Next(11111111, 59999999)}"
                        });
                    }
                    else
                    {
                        item.Details.Add(new LoadFactorDetailModel
                        {
                            Day = 31,
                            Amount = itemAmount,
                            Date = "---",
                            Origin = "---",
                            Destination = "---",
                            LoadFactorNumber = "سایر / تناژ"
                        });
                        itemAmount = 0;
                    }
                }
            }

            using var workbook = new XLWorkbook();
            var docTitle = $"عملکرد در {calendar.Title}";

            foreach (var item in data.OrderBy(a => a.VehicleLeftNumber).ThenBy(a => a.VehicleRightNumber))
            {
                var ws = workbook.Worksheets.Add($"{item.VehicleNumber}");
                ws.RightToLeft = true;
                ws.Style.Font.FontName = "B Titr";
                ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                ws.Cell(1, 1).Value = $"عملکرد {item.VehicleNumber} در {calendar.Title} در شرکت {item.CustomerName}";
                ws.Cell(2, 1).Value = "#";
                ws.Cell(2, 2).Value = "تاریخ";
                ws.Cell(2, 3).Value = "راننده";
                ws.Cell(2, 4).Value = "مبدا";
                ws.Cell(2, 5).Value = "مقصد";
                ws.Cell(2, 6).Value = "بارنامه";
                ws.Cell(2, 7).Value = "موردی";
                ws.Cell(2, 8).Value = "مبلغ";

                var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(data.Count + 2, 8));
                rngTable.FirstRow().Merge();

                rngTable.FirstRow().Style
                    .Font.SetBold()
                    .Font.SetFontSize(12)
                        .Fill.SetBackgroundColor(XLColor.LightGray)
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 8)); // The address is relative to rngTable (NOT the worksheet)
                rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngHeaders.Style.Font.Bold = true;
                rngHeaders.Style.Font.FontColor = XLColor.Black;

                item.Details = item.Details.OrderBy(a => a.Day).ToList();
                for (int i = 0; i < item.Details.Count; i++)
                {
                    var detail = item.Details[i];
                    ws.Cell(i + 3, 1).SetValue(i + 1);
                    ws.Cell(i + 3, 2).SetValue(detail.Date);
                    ws.Cell(i + 3, 3).SetValue(detail.Day == 31 ? "---" : item.DriverName);
                    ws.Cell(i + 3, 4).SetValue(detail.Origin);
                    ws.Cell(i + 3, 5).SetValue(detail.Destination);
                    ws.Cell(i + 3, 6).SetValue(detail.LoadFactorNumber);
                    ws.Cell(i + 3, 7).SetValue(detail.Day == 31 ? "---" : "بلی");
                    ws.Cell(i + 3, 8).SetValue(detail.Amount.ToString("N0"));
                }

                ws.Cell(item.Details.Count + 3, 1).Value = "جمع";
                ws.Range(item.Details.Count + 3, 1, item.Details.Count + 3, 7).Merge();
                ws.Cell(item.Details.Count + 3, 8).Value = item.Details.Sum(a => a.Amount < 0 ? 0 : a.Amount).ToString("N0");

                ws.Column("A").Width = 5;
                ws.Column("B").Width = 8;
                ws.Column("C").Width = 13;
                ws.Column("D").Width = 12;
                ws.Column("E").Width = 12;
                ws.Column("F").Width = 11;
                ws.Column("G").Width = 5;
                ws.Column("H").Width = 12;

                ws.CellsUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                var table = ws.Range(2, 1, item.Details.Count + 2, 8).CreateTable();
                table.Theme = XLTableTheme.None;
                table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                    .Font.SetFontSize(8);
            }

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        public async Task<IActionResult> OtherCost(int? p)
        {
            var pageNumber = p ?? 1;
            ViewBag.data = await db.OtherCost.Include(a => a.Calendar).Include(a => a.Vehicle).Include(a => a.Customer).OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 20);
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> OtherCost_Search()
        {
            var customers = await _customerRepository.GetAllActive();
            var calendars = await _calendarRepository.Calendars().OrderByDescending(a => a.Id).ToListAsync();
            return Json(new { customers, calendars });
        }

        [HttpPost]
        public async Task<IActionResult> OtherCost_Search(int? p, long? calendarId, long? customerId, int bill)
        {
            var pageNumber = p ?? 1;
            var query = db.OtherCost.AsNoTracking();

            if (calendarId.HasValue)
                query = query.Where(a => a.CalendarId.Equals(calendarId.Value));
            if (customerId.HasValue)
                query = query.Where(a => a.CustomerId.Equals(customerId.Value));
            if (bill == 1)
                query = query.Where(a => a.BillId.HasValue);
            else if (bill == 2)
                query = query.Where(a => !a.BillId.HasValue);


            ViewBag.CustomerId = customerId;
            ViewBag.CalendarId = calendarId;
            ViewBag.Cost = bill;
            ViewBag.data = await query.Include(a => a.Calendar).Include(a => a.Vehicle).Include(a => a.Customer).OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 20);
            return PartialView();
        }

        public async Task<IActionResult> GetOtherCostDriverNames()
        {
            return Json(await db.OtherCost.AsNoTracking().Select(a => a.DriverName.Replace("/", "")).Distinct().ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> CreateOtherCost()
        {
            ViewData["Customers"] = await _customerRepository.Customers().AsNoTracking().OrderByDescending(a => a.Name).ToListAsync();
            ViewData["Calendars"] = await _calendarRepository.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepository.Vehicles().AsNoTracking().OrderBy(a => a.LeftNumber).ThenBy(a => a.RightNumber).ToListAsync();
            return PartialView("~/Views/LoadFactorCreator/Create/OtherCost.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOtherCost(OtherCost c)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                db.Add(new OtherCost
                {
                    Amount = c.Amount,
                    DriverName = c.DriverName,
                    VehicleId = c.VehicleId,
                    CustomerId = c.CustomerId,
                    CalendarId = c.CalendarId,
                    AdminId = _userManager.GetUserId(User)
                });
                try
                {
                    await db.SaveChangesAsync();
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
        public async Task<PartialViewResult> EditOtherCost(long id)
        {
            ViewData["Customers"] = await _customerRepository.Customers().AsNoTracking().OrderByDescending(a => a.Name).ToListAsync();
            ViewData["Calendars"] = await _calendarRepository.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();
            ViewData["Vehicles"] = await _vehicleRepository.Vehicles().AsNoTracking().OrderBy(a => a.LeftNumber).ThenBy(a => a.RightNumber).ToListAsync();

            return PartialView("~/Views/LoadFactorCreator/Edit/OtherCost.cshtml", await db.OtherCost.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditOtherCost(OtherCost c)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                var item = await db.OtherCost.FindAsync(c.Id);
                item.EditorId = _userManager.GetUserId(User);
                item.EditDateTime = DateTime.Now;
                item.CalendarId = c.CalendarId;
                item.CustomerId = c.CustomerId;
                item.VehicleId = c.VehicleId;
                item.Amount = c.Amount;
                item.DriverName = c.DriverName;

                db.Update(item);
                try
                {
                    await db.SaveChangesAsync();


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
        public async Task<IActionResult> DeleteOtherCost(long id)
        {
            var item = await db.OtherCost.FindAsync(id);

            db.Remove(item);
            try
            {
                await db.SaveChangesAsync();
                TempData["msg"] = "عملیات موفقیت آمیز بود. |success";
            }
            catch (Exception e)
            {
                TempData["msg"] = $"عملیات با خطا مواجه شد. جزئیات: {e.Message} |danger";
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<IActionResult> CreateGroupOtherCost()
        {
            ViewData["Customers"] = await _customerRepository.Customers().AsNoTracking().OrderByDescending(a => a.Name).ToListAsync();
            ViewData["Calendars"] = await _calendarRepository.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).ToListAsync();

            return PartialView("~/Views/LoadFactorCreator/Create/GroupOtherCost.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupOtherCost(CreateGroupOtherCostVM c)
        {
            string msg;
            string status = "danger";
            if (ModelState.IsValid)
            {
                var customerFactorsSum = await _customerFactorRepository.Query().AsNoTracking().Where(a => a.CalendarId.Equals(c.CalendarId) && a.CustomerId.Equals(c.CustomerId)).SumAsync(a => a.Amount);

                var driverFeeList = await (from a in db.LoadFactor
                                           join b in db.Contract on a.ContractId equals b.Id
                                           where a.CalendarId.Equals(c.CalendarId) && b.CustomerId.Equals(c.CustomerId)
                                           select new
                                           {
                                               a.Tonnage,
                                               a.DriverTonnagePrice,
                                               a.DriverFee,
                                               a.WeighbridgePrice,
                                               a.DriverLoadSleepPrice
                                           }).AsNoTracking().ToListAsync();

                double driverFee = 0;
                foreach (var item in driverFeeList)
                {
                    driverFee += item.DriverFee;
                    if (item.Tonnage.HasValue)
                        driverFee += item.Tonnage.Value * item.DriverTonnagePrice.Value;

                    if (item.WeighbridgePrice.HasValue)
                        driverFee += item.WeighbridgePrice.Value;

                    if (item.DriverLoadSleepPrice.HasValue)
                        driverFee += item.DriverLoadSleepPrice.Value;
                }

                var billsSum = await _billRepository.Query().Include(a => a.Vehicle).Include(a => a.Customer)
                .Where(a => a.CalendarId.Equals(c.CalendarId) && a.CustomerId.Value.Equals(c.CustomerId) && (a.VehicleId.HasValue &&
          (_vehicleRepository.Vehicles().Where(b => !b.RealStatus).Select(a => a.Id)).Contains(a.VehicleId.Value))).SumAsync(a => a.Amount);

                var otherCostsSum = await db.OtherCost.Include(a => a.Vehicle).AsNoTracking().Where(a => a.CalendarId.Equals(c.CalendarId) && a.CustomerId.Equals(c.CustomerId)).SumAsync(a => a.Amount);

                var loadFactorNovinsSum = await db.LoadFactorNovin.Include(a => a.Vehicle).AsNoTracking()
                    .Where(a => a.CalendarId.Equals(c.CalendarId) && a.CustomerId.Equals(c.CustomerId)).SumAsync(a => a.DriverFee);

                var costSum = driverFee + billsSum + otherCostsSum + loadFactorNovinsSum;

                var requiredAmount = Math.Truncate(c.Amount.HasValue ? c.Amount.Value : (customerFactorsSum - (customerFactorsSum * 0.16)) - costSum);
                var rnd = new Random();
                var minimumNumber = c.MinimumAmount / 1000000;
                var maximumNumber = c.MaximumAmount / 1000000;
                double calculatedAmount = 0;
                var amountList = new List<double>();
                while (calculatedAmount < requiredAmount)
                {
                    var difference = requiredAmount - calculatedAmount;
                    if (difference <= c.MaximumAmount)
                    {
                        if (c.Amount.HasValue)
                        {
                            amountList.Add(difference);
                            calculatedAmount += difference;
                        }
                        else
                        {
                            var x = Convert.ToInt32(difference / 1000000);
                            var y = x * 1000000;

                            amountList.Add(y);
                            calculatedAmount += y;
                            break;
                        }
                    }
                    else
                    {
                        var x = rnd.Next(Convert.ToInt32(minimumNumber), Convert.ToInt32(maximumNumber));
                        var y = x * 1000000;

                        amountList.Add(y);
                        calculatedAmount += y;
                    }
                }

                List<OtherCost> otherCosts = new();
                string userId = _userManager.GetUserId(User);

                var unrealVehicles = await _vehicleRepository.Vehicles().AsNoTracking()
                    .Where(a => !a.RealStatus && !a.VehicleOwnerFullname.EndsWith("//"))
                    .Select(a => new { a.Id, a.VehicleOwnerFullname }).ToListAsync();

                var usedVehicles = await _billRepository.Query().AsNoTracking()
                    .Where(a => a.CalendarId.Equals(c.CalendarId) && a.VehicleId.HasValue && unrealVehicles.Select(a => a.Id).Contains(a.VehicleId.Value))
                    .Select(a => a.VehicleId.Value).Distinct().ToListAsync();

                var oCosts = await db.OtherCost.AsNoTracking().Where(a => a.CalendarId.Equals(c.CalendarId) && unrealVehicles.Select(a => a.Id).Contains(a.VehicleId))
                    .Select(a => a.VehicleId).Distinct().ToListAsync();

                usedVehicles.AddRange(oCosts);

                unrealVehicles = unrealVehicles.Where(a => !usedVehicles.Contains(a.Id)).ToList();

                for (int i = 0; i < amountList.Count; i++)
                {
                    int index = rnd.Next(0, unrealVehicles.Count - 1);
                    var vehicleId = unrealVehicles[index].Id;

                    otherCosts.Add(new Domain.Models.LoadFactorCreator.OtherCost
                    {
                        AdminId = userId,
                        Amount = amountList[i],
                        CalendarId = c.CalendarId,
                        CustomerId = c.CustomerId,
                        DriverName = unrealVehicles[index].VehicleOwnerFullname.Replace("/", ""),
                        VehicleId = vehicleId
                    });

                    unrealVehicles.Remove(unrealVehicles[index]);
                }

                await db.AddRangeAsync(otherCosts);

                try
                {
                    await db.SaveChangesAsync();
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
        public async Task<IActionResult> GetUnBillOtherCost(long billId)
        {
            var bill = await _billRepository.Query().AsNoTracking().Include(a => a.OtherCosts).FirstOrDefaultAsync(a => a.Id.Equals(billId));
            if (!bill.OtherCosts.Any())
            {
                var data = await db.OtherCost.AsNoTracking().Include(a => a.Vehicle)
                    .Where(a => a.CustomerId.Equals(bill.CustomerId) && a.CalendarId.Equals(bill.CalendarId) && !a.BillId.HasValue).ToListAsync();

                ViewData["BillId"] = billId;
                return PartialView("OtherCost_Relation", data);
            }
            else
                return BadRequest("قبلا ثبت شده است");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBillCostRelation(long BillId, long[] IdList)
        {
            try
            {
                var otherCosts = await db.OtherCost.Where(a => !a.BillId.HasValue && IdList.Contains(a.Id)).ToListAsync();

                if (otherCosts.Count == IdList.Length)
                {
                    foreach (var item in otherCosts)
                        item.BillId = BillId;

                    await db.SaveChangesAsync();
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
        public async Task<IActionResult> CalculateOtherCostSum(long customerId, long calendarId, int bill)
        {
            var query = db.OtherCost.AsNoTracking().Where(a => a.CustomerId.Equals(customerId) && a.CalendarId.Equals(calendarId));

            if (bill == 1)
                query = query.Where(a => a.BillId.HasValue);
            else if (bill == 2)
                query = query.Where(a => !a.BillId.HasValue);

            var sum = await query.SumAsync(a => a.Amount);

            return Json(sum.ToString("N0"));
        }
    }
}
