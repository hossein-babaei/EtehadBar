using ClosedXML.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Domain.Models.LoadFactorCreator;
using EtehadBar.Infra.Data;
using EtehadBar.Infra.Data.Context;
using EtehadBar.Infra.Data.Repository;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
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

        public LoadFactorCreatorController(ICalendarRepository calendarRepository, IBillRepository billRepository, IVehicleRepository vehicleRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICustomerRepository customerRepository)
        {
            _calendarRepository = calendarRepository;
            _billRepository = billRepository;
            _vehicleRepository = vehicleRepository;
            db = context;
            _userManager = userManager;
            _customerRepository = customerRepository;
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
                List<int> takenDays = new();
                while (itemAmount > 0)
                {
                    int day = 0;

                    day = rnd.Next(1, 30);
                    while (takenDays.Contains(day) && takenDays.Count <= 30)
                        day = rnd.Next(1, 30);
                    takenDays.Add(day);

                    var possibleRoutes = routes.Where(a => a.Amount <= itemAmount).ToList();
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

        public async Task<IActionResult> OtherCost_Search(int? p)
        {
            var pageNumber = p ?? 1;
            ViewBag.data = await db.OtherCost.Include(a => a.Calendar).Include(a => a.Vehicle).Include(a => a.Customer).OrderByDescending(a => a.Id).ToPagedListAsync(pageNumber, 20);
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
                    Amount= c.Amount,
                    DriverName= c.DriverName,
                    VehicleId= c.VehicleId,
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
    }
}
