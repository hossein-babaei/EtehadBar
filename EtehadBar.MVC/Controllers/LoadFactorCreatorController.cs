using ClosedXML.Excel;
using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data;
using EtehadBar.Infra.Data.Context;
using EtehadBar.Infra.Data.Repository;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ICalendarRepository _calendarRepository;
        private readonly IBillRepository _billRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public LoadFactorCreatorController(ICalendarRepository calendarRepository, IBillRepository billRepository, IVehicleRepository vehicleRepository, ApplicationDbContext context)
        {
            _calendarRepository = calendarRepository;
            _billRepository = billRepository;
            _vehicleRepository = vehicleRepository;
            db = context;
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
            var origins = LoadFactorCreatorStaticData.GetOrigins();
            var destination = LoadFactorCreatorStaticData.GetDestinations();
            var ranges = LoadFactorCreatorStaticData.GetPriceRanges();
            var data = new List<LoadFactorModel>();

            var calendar = await _calendarRepository.Get(id);
            var persianDate = new PersianDateTime(calendar.StartDate);

            var bills = await _billRepository.Query().Where(a => a.CalendarId.Equals(id) && (a.VehicleId.HasValue &&
            (_vehicleRepository.Vehicles().Where(b => !b.RealStatus).Select(a => a.Id)).Contains(a.VehicleId.Value))).OrderBy(a => a.Date).ToListAsync();
            var distinctedBills = bills.DistinctBy(a => a.VehicleId.Value).ToList();

            foreach (var item in distinctedBills)
            {
                data.Add(new LoadFactorModel
                {
                    DriverName = item.ReceiverName,
                    VehicleId = item.VehicleId.Value,
                    VehicleNumber = $"ایران {item.Vehicle.IranStateNumber} - {item.Vehicle.RightNumber} {item.Vehicle.NumberWord} {item.Vehicle.LeftNumber}",
                    Amount = bills.Where(a => a.VehicleId.Value.Equals(item.VehicleId.Value)).Sum(a => a.Amount)
                });
            }

            var rnd = new Random();
            foreach (var item in data)
            {
                var range = ranges.Where(a => a.Minimum <= item.Amount && a.Maximum >= item.Amount).Single();
                var amount = item.Amount / range.Divider;

                for (int i = 0; i < range.Divider; i++)
                {
                    int day = rnd.Next(1, 30);
                    item.Details.Add(new LoadFactorDetailModel
                    {
                        Day = day,
                        Amount = amount,
                        Date = $"{persianDate.ToString("yyyy/MM")}/{(day < 10 ? $"0{day}" : day)}",
                        Origin = origins.ElementAt(rnd.Next(0, origins.Count - 1)).Name,
                        Destination = destination.ElementAt(rnd.Next(0, destination.Count - 1)).Name,
                        LoadFactorNumber = $"{persianDate.Year}/{rnd.Next(rnd.Next(11111111, 19999999))}"
                    });
                }
            }

            using var workbook = new XLWorkbook();
            var docTitle = $"عملکرد در {calendar.Title}";

            foreach (var item in data)
            {
                var ws = workbook.Worksheets.Add(item.VehicleNumber);
                ws.RightToLeft = true;
                ws.Style.Font.FontName = "B Titr";
                ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                ws.Cell(1, 1).Value = $"عملکرد {item.VehicleNumber} در {calendar.Title}";
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
                    ws.Cell(i + 3, 3).SetValue(item.DriverName);
                    ws.Cell(i + 3, 4).SetValue(detail.Origin);
                    ws.Cell(i + 3, 5).SetValue(detail.Destination);
                    ws.Cell(i + 3, 6).SetValue(detail.LoadFactorNumber);
                    ws.Cell(i + 3, 7).SetValue("بلی");
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
    }
}
