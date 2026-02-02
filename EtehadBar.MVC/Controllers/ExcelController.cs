using ClosedXML.Excel;
using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Domain.Models.LoadFactorCreator;
using EtehadBar.Infra.Data.Context;
using EtehadBar.MVC.Filters;
using Helpers;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.MVC.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(ActionLogFilter))]
    public class ExcelController : Controller
    {
        private readonly ICalendarRepository _calendarRepo;
        private readonly ICostRepository _costRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ILoadFactorRepository _loadFactorRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IAccountBookRepository _accountBookRepo;
        private readonly IMehrcomParsCategoryRepository _mehrcomParsCategoryRepository;
        private readonly IFreeLoadFactorRepository _freeLoadFactorRepository;
        private readonly IVehicleBalanceRepository _vehicleBalanceRepository;
        private readonly IBillRepository _billRepository;
        private readonly ILoadFactorNovinRepository _loadFactorNovinRepository;
        private readonly ApplicationDbContext db;

        public ExcelController(
            ICalendarRepository calendarRepository,
            ICostRepository costRepository,
            ICustomerRepository customerRepository,
            ILoadFactorRepository loadFactorRepository,
            IVehicleRepository vehicleRepository,
            IAccountBookRepository accountBookRepo,
            IMehrcomParsCategoryRepository mehrcomParsCategoryRepository,
            IFreeLoadFactorRepository freeLoadFactorRepository,
            IVehicleBalanceRepository vehicleBalanceRepository,
            IBillRepository billRepository,
            ApplicationDbContext dbContext,
            ILoadFactorNovinRepository loadFactorNovinRepository)
        {
            _calendarRepo = calendarRepository;
            _costRepo = costRepository;
            _customerRepo = customerRepository;
            _loadFactorRepo = loadFactorRepository;
            _vehicleRepo = vehicleRepository;
            _accountBookRepo = accountBookRepo;
            _mehrcomParsCategoryRepository = mehrcomParsCategoryRepository;
            _freeLoadFactorRepository = freeLoadFactorRepository;
            _vehicleBalanceRepository = vehicleBalanceRepository;
            _billRepository = billRepository;
            db = dbContext;
            _loadFactorNovinRepository = loadFactorNovinRepository;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detailed(long calendarId)
        {
            double amountSum = 0,
                driverSum = 0;

            var calendar = await _calendarRepo.Get(calendarId);
            var cost = await _costRepo.Costs().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            var excludedBillTypes = new List<string> { "جابجایی از پاسارگاد", "جابجایی حساب", "واریز شرکا - تامین وجه" };
            var payment = await _billRepository.Query().Where(a => !excludedBillTypes.Contains(a.BillType) &&
            a.Date >= calendar.StartDate && a.Date <= calendar.EndDate).SumAsync(a => a.Amount);
            var income = await _customerRepo.CustomerIncomes().Where(a => a.Date >= calendar.StartDate && a.Date <= calendar.EndDate).SumAsync(a => a.Amount);

            var loadFactors = new List<GlobalLoadFactorVM>();
            var loadFactorList = await _loadFactorRepo.LoadFactors().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Driver).Include(a => a.Vehicle).Include(a => a.Contract).ThenInclude(a => a.Customer)
                .Where(a => a.CalendarId.Equals(calendarId)).Select(a => new GlobalLoadFactorVM
                {
                    Amount = a.Tonnage.HasValue ? ((a.Tonnage.Value * a.TonnagePrice.Value) + a.Amount) : a.Amount,
                    DriverFee = a.Tonnage.HasValue ? ((a.Tonnage.Value * a.DriverTonnagePrice.Value) + a.DriverFee) : a.DriverFee,
                    CustomerName = a.Contract.Customer.Name + " " + a.Contract.Number,
                    Date = a.Date,
                    Destination = a.Destination.Title,
                    Origin = a.Origin.Title,
                    DriverName = a.Driver.Fullname,
                    LoadFactorDeductions = a.LoadFactorDeductions,
                    LoadNumber = a.LoadNumber,
                    LoadNumberGov = a.LoadNumberGov,
                    Id = a.Id,
                    RowId = a.RowId,
                    VAT = a.VAT,
                    VehicleType = a.Vehicle.Type,
                    WithholdingTax = a.WithholdingTax
                }).OrderBy(a => a.Date).ToListAsync();
            loadFactors.AddRange(loadFactorList);

            var freeLoadFactors = await _freeLoadFactorRepository.Query().Where(a => a.CalendarId.Equals(calendarId)).Select(a => new GlobalLoadFactorVM
            {
                Amount = a.Tonnage.HasValue && a.TonnagePrice.HasValue ? ((a.Tonnage.Value * a.TonnagePrice.Value) + a.Amount) : a.Amount,
                DriverFee = a.Tonnage.HasValue && a.DriverTonnagePrice.HasValue ? ((a.Tonnage.Value * a.DriverTonnagePrice.Value) + a.DriverFee) : a.DriverFee,
                CustomerName = a.ApplicantName,
                Date = a.Date,
                Destination = a.Destination,
                Origin = a.Origin,
                DriverName = a.DriverName,
                LoadFactorDeductions = a.LoadFactorDeductions,
                LoadNumber = a.LoadNumber,
                LoadNumberGov = a.LoadNumberGov,
                Id = a.Id,
                RowId = a.RowId,
                VAT = a.VAT,
                VehicleType = a.VehicleType,
                WithholdingTax = a.WithholdingTax
            }).OrderBy(a => a.Date).ToListAsync();

            loadFactors.AddRange(freeLoadFactors);

            string docTitle = $"گزارش تفصیلی بارنامه در {calendar.Title}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش تفصیلی بارنامه");
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "شماره بارنامه";
            ws.Cell(2, 4).Value = "شماره بارنامه دولتی";
            //ws.Cell(2, 5).Value = "شماره خروج";
            ws.Cell(2, 5).Value = "مبدا";
            ws.Cell(2, 6).Value = "مقصد";
            ws.Cell(2, 7).Value = "مبلغ";
            ws.Cell(2, 8).Value = "کرایه راننده";
            //ws.Cell(2, 10).Value = "مالیات ارزش افزوده";
            ws.Cell(2, 9).Value = "سپرده بیمه";
            ws.Cell(2, 10).Value = "مالیات تکلیفی";
            ws.Cell(2, 11).Value = "راننده";
            ws.Cell(2, 12).Value = "خودرو";
            ws.Cell(2, 13).Value = "مشتری - شماره قرارداد";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(loadFactors.Count + 2, 13));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 13)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= loadFactors.Count; index++)
            {
                var item = loadFactors[index - 1];
                amountSum += item.Amount;
                driverSum += item.DriverFee;
                if (item.Tonnage.HasValue && item.Tonnage.Value > 0)
                {
                    if (item.TonnagePrice.HasValue && item.TonnagePrice.Value > 0)
                        amountSum += item.Tonnage.Value * item.TonnagePrice.Value;

                    if (item.DriverTonnagePrice.HasValue && item.DriverTonnagePrice.Value > 0)
                        driverSum += item.Tonnage.Value * item.DriverTonnagePrice.Value;
                }

                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(item.Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).Value = item.LoadNumber;

                if (string.IsNullOrWhiteSpace(item.LoadNumberGov))
                    ws.Cell(index + 2, 4).Value = "---";
                else
                    ws.Cell(index + 2, 4).Value = item.LoadNumberGov;

                ws.Cell(index + 2, 5).Value = item.Origin;
                ws.Cell(index + 2, 6).Value = item.Destination;
                ws.Cell(index + 2, 7).Value = item.Amount.ToString("N0");
                ws.Cell(index + 2, 8).Value = item.DriverFee.ToString("N0");
                //ws.Cell(index + 2, 10).Value = item.VAT;
                ws.Cell(index + 2, 9).Value = item.LoadFactorDeductions;
                ws.Cell(index + 2, 10).Value = item.WithholdingTax;
                ws.Cell(index + 2, 11).Value = item.DriverName;
                ws.Cell(index + 2, 12).Value = item.VehicleType;
                ws.Cell(index + 2, 13).Value = item.CustomerName;
            }

            ws.Cell($"B{loadFactors.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{loadFactors.Count + 3}:L{loadFactors.Count + 3}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 3, 13).Value = loadFactors.Count;

            ws.Cell($"B{loadFactors.Count + 4}").Value = "مبلغ بارنامه ها";
            ws.Range($"B{loadFactors.Count + 4}:L{loadFactors.Count + 4}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 4, 13).Value = amountSum.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 5}").Value = "مبلغ قابل پرداخت به رانندگان";
            ws.Range($"B{loadFactors.Count + 5}:L{loadFactors.Count + 5}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 5, 13).Value = driverSum.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 6}").Value = "فیش های پرداختی";
            ws.Range($"B{loadFactors.Count + 6}:L{loadFactors.Count + 6}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 6, 13).Value = payment.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 7}").Value = "هزینه های جاری";
            ws.Range($"B{loadFactors.Count + 7}:L{loadFactors.Count + 7}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 7, 13).Value = cost.ToString("N0");

            //ws.Cell($"B{loadFactors.Count + 8}").Value = "کل هزینه ها";
            //ws.Range($"B{loadFactors.Count + 8}:L{loadFactors.Count + 8}").Row(1).Merge();
            //ws.Cell(loadFactors.Count + 8, 13).Value = (payment + cost).ToString("N0");

            //ws.Cell($"B{loadFactors.Count + 9}").Value = "حقوق مساعده پرداختی";
            //ws.Range($"B{loadFactors.Count + 9}:L{loadFactors.Count + 9}").Row(1).Merge();
            //ws.Cell(loadFactors.Count + 9, 13).Value = payment.ToString("N0");

            //ws.Cell($"B{loadFactors.Count + 10}").Value = "جمع کل دریافتی";
            //ws.Range($"B{loadFactors.Count + 10}:O{loadFactors.Count + 10}").Row(1).Merge();
            //ws.Cell(loadFactors.Count + 10, 16).Value = income.ToString("N0");

            ws.RangeUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:M{loadFactors.Count + 7}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.Columns().AdjustToContents();

            ws.PageSetup.SetPageOrientation(XLPageOrientation.Landscape)
                .SetPaperSize(XLPaperSize.A4Paper);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        [Authorize(Roles = "Admin, User, Milad, Accountant")]
        public async Task<IActionResult> Cost(long calendarId, string userId)
        {
            var calendar = await _calendarRepo.Get(calendarId);

            var query = _costRepo.Costs().Include(a => a.ApplicationUser).Include(a => a.Definition).Where(a => a.CalendarId.Equals(calendarId));
            if (userId != "all")
                query = query.Where(a => a.UserId.Equals(userId));

            var costs = await query.OrderBy(a => a.Date).ToListAsync();

            string docTitle = $"صورت هزینه های تنخواه گردان شرکت اتحاد بار آسیا در {calendar.Title}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش هزینه های جاری");

            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "ردیف";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "شرح";
            ws.Cell(2, 4).Value = "مبلغ";
            ws.Cell(2, 5).Value = "کاربر سیستم";
            ws.Cell(2, 6).Value = "حساب";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(costs.Count + 2, 6));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 6)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= costs.Count; index++)
            {
                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(costs[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).Value = costs[index - 1].Description.Replace("*", "");
                ws.Cell(index + 2, 4).Value = costs[index - 1].Amount.ToString("N0");
                ws.Cell(index + 2, 5).Value = $"{costs[index - 1].ApplicationUser.Firstname} {costs[index - 1].ApplicationUser.Lastname}";
                ws.Cell(index + 2, 6).Value = costs[index - 1].Definition.Title;
            }

            ws.Cell($"B{costs.Count + 3}").Value = "جمع کل";
            ws.Cell($"D{costs.Count + 3}").Value = costs.Sum(a => a.Amount).ToString("N0");
            ws.Range($"B{costs.Count + 3}:C{costs.Count + 3}").Row(1).Merge();
            ws.Range($"D{costs.Count + 3}:F{costs.Count + 3}").Row(1).Merge();

            var rngTable2 = ws.Range($"B{costs.Count + 3}:F{costs.Count + 3}");
            rngTable2.RangeUsed().Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Font.SetFontSize(12);

            ws.RangeUsed().Style.Font.SetBold();
            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.RangeUsed().Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                .Border.SetInsideBorderColor(XLColor.Black);

            ws.Columns().AdjustToContents();

            ws.PageSetup.SetPageOrientation(XLPageOrientation.Portrait)
                .SetPaperSize(XLPaperSize.A4Paper)
                .Margins.SetTop(0).SetBottom(0).SetRight(0.5).SetLeft(0).SetHeader(0).SetFooter(0);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> VehicleLoadFactor(long calendarId, long vehicleId, long customerId)
        {
            var vehicle = await _vehicleRepo.Get(vehicleId);
            var calendar = await _calendarRepo.Get(calendarId);
            var balance = await _vehicleBalanceRepository.GetVehicleBalanceSum(vehicleId, calendarId, customerId == 0 ? null : customerId);

            var query = _loadFactorRepo.LoadFactors().Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Driver).Include(a => a.Contract).ThenInclude(a => a.Customer)
                .Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId));
            if (customerId > 0)
                query = query.Where(a => a.Contract.CustomerId.Equals(customerId));

            var loadFactors = await query.OrderBy(a => a.Id).ToListAsync();

            string docTitle = $"گزارش بارنامه های {vehicle.Type} به شماره (ایران {vehicle.IranStateNumber} - {vehicle.RightNumber} {vehicle.NumberWord} {vehicle.LeftNumber}) در {calendar.Title}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش عملکرد");
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "شماره بارنامه";
            ws.Cell(2, 4).Value = "شماره بارنامه دولتی";
            ws.Cell(2, 5).Value = "شماره خروج";
            ws.Cell(2, 6).Value = "مبدا";
            ws.Cell(2, 7).Value = "مقصد";
            ws.Cell(2, 8).Value = "کرایه راننده";
            ws.Cell(2, 9).Value = "موردی";
            ws.Cell(2, 10).Value = "راننده";
            ws.Cell(2, 11).Value = "مشتری - شماره قرارداد";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(loadFactors.Count + 2, 11));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 11)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= loadFactors.Count; index++)
            {
                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(loadFactors[index - 1].Date).ToString("yyyy/MM/dd");
                if (string.IsNullOrWhiteSpace(loadFactors[index - 1].LoadNumberGov))
                {
                    ws.Cell(index + 2, 3).Value = loadFactors[index - 1].LoadNumber;
                    ws.Cell(index + 2, 4).Value = "---";
                }
                else
                {
                    ws.Cell(index + 2, 3).Value = loadFactors[index - 1].LoadNumber;
                    ws.Cell(index + 2, 4).Value = loadFactors[index - 1].LoadNumberGov;
                }
                ws.Cell(index + 2, 5).Value = loadFactors[index - 1].ExitNumber;
                ws.Cell(index + 2, 6).Value = loadFactors[index - 1].Origin.Title;
                ws.Cell(index + 2, 7).Value = loadFactors[index - 1].Destination.Title;
                ws.Cell(index + 2, 8).Value = loadFactors[index - 1].DriverFee.ToString("N0");
                ws.Cell(index + 2, 9).Value = loadFactors[index - 1].IsFreeDriverPrice ? "بلی" : "خیر";
                ws.Cell(index + 2, 10).Value = $"{loadFactors[index - 1].Driver.Fullname}";
                ws.Cell(index + 2, 11).Value = $"{loadFactors[index - 1].Contract.Customer.Name} {loadFactors[index - 1].Contract.Number}";
            }

            ws.Cell($"B{loadFactors.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{loadFactors.Count + 3}:J{loadFactors.Count + 3}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 3, 11).Value = loadFactors.Count;

            ws.Cell($"B{loadFactors.Count + 4}").Value = "جمع کرایه عملکرد";
            ws.Range($"B{loadFactors.Count + 4}:J{loadFactors.Count + 4}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 4, 11).Value = loadFactors.Sum(a => a.DriverFee +
            ((a.Tonnage.HasValue && a.DriverTonnagePrice.HasValue) ? (a.Tonnage.Value * a.DriverTonnagePrice.Value) : 0) +
        (a.WeighbridgePrice.HasValue ? a.WeighbridgePrice.Value : 0) + (a.DriverLoadSleepPrice.HasValue ? a.DriverLoadSleepPrice.Value : 0)).ToString("N0");

            ws.Cell($"B{loadFactors.Count + 5}").Value = "مجموع قابل پرداخت";
            ws.Range($"B{loadFactors.Count + 5}:J{loadFactors.Count + 6}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 5, 11).Value = (balance > 0 ? balance : 0).ToString("N0");

            var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:K{loadFactors.Count + 5}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.RangeUsed().Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                .Border.SetInsideBorderColor(XLColor.Black);

            ws.Columns().AdjustToContents();

            ws.PageSetup.SetPageOrientation(XLPageOrientation.Landscape)
                .SetPaperSize(XLPaperSize.A4Paper);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> VehicleActivity(long calendarId, long vehicleId, long customerId)
        {
            var vehicle = await _vehicleRepo.Get(vehicleId);
            var calendar = await _calendarRepo.Get(calendarId);
            var balance = await _vehicleBalanceRepository.GetVehicleBalanceSum(vehicleId, calendarId, customerId == 0 ? null : customerId);
            var query = _loadFactorRepo.LoadFactors().Include(a => a.Origin).Include(a => a.Destination).Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId));
            if (customerId > 0)
                query = query.Where(a => a.Contract.CustomerId.Equals(customerId));

            var loadFactors = await query.OrderBy(a => a.Id).ToListAsync();

            var routes = new List<VehicleActivityVM>();

            foreach (var origin in loadFactors.DistinctBy(a => a.OriginId))
            {
                var destinations = loadFactors.Where(a => a.OriginId.Equals(origin.OriginId)).ToList();
                foreach (var destination in destinations)
                {
                    routes.Add(new VehicleActivityVM
                    {
                        OriginId = origin.OriginId,
                        DestinationId = destination.DestinationId,
                        DestionationTitle = destination.Destination.Title,
                        DriverFee = destination.DriverFee,
                        DriverTonnagePrice = destination.DriverTonnagePrice,
                        Tonnage = destination.Tonnage,
                        TonnagePrice = destination.TonnagePrice,
                        OriginTitle = origin.Origin.Title,
                        Count = loadFactors.Count(a => a.OriginId.Equals(origin.OriginId) && a.DestinationId.Equals(destination.DestinationId) && a.DriverFee.Equals(destination.DriverFee))
                    });
                }
            }

            routes = routes.DistinctBy(a => new { a.OriginId, a.DestinationId, a.DriverFee }).ToList();

            string docTitle = $"گزارش جزئیات عملکرد {vehicle.Type} به شماره (ایران {vehicle.IranStateNumber} - {vehicle.RightNumber} {vehicle.NumberWord} {vehicle.LeftNumber}) در {calendar.Title}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش جزئیات عملکرد");
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;
            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "مبدا";
            ws.Cell(2, 3).Value = "مقصد";
            ws.Cell(2, 4).Value = "کرایه";
            ws.Cell(2, 5).Value = "تعداد";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(loadFactors.Count + 2, 5));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(14)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 5)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int i = 0; i < routes.Count; i++)
            {
                ws.Cell(i + 3, 1).Value = i + 1;
                ws.Cell(i + 3, 2).Value = routes[i].OriginTitle;
                ws.Cell(i + 3, 3).Value = routes[i].DestionationTitle;
                ws.Cell(i + 3, 4).Value = routes[i].DriverFee.ToString("N0");
                ws.Cell(i + 3, 5).Value = routes[i].Count;
            }

            ws.Cell($"B{routes.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{routes.Count + 3}:D{routes.Count + 3}").Row(1).Merge();
            ws.Cell(routes.Count + 3, 5).Value = loadFactors.Count;

            double driverFeeTotal = loadFactors.Sum(a => a.DriverFee +
            ((a.Tonnage.HasValue && a.DriverTonnagePrice.HasValue) ? (a.Tonnage.Value * a.DriverTonnagePrice.Value) : 0) +
        (a.WeighbridgePrice.HasValue ? a.WeighbridgePrice.Value : 0) + (a.DriverLoadSleepPrice.HasValue ? a.DriverLoadSleepPrice.Value : 0));

            if (loadFactors.Any(a => a.Tonnage.HasValue))
            {
                ws.Cell($"B{routes.Count + 4}").Value = "جمع کل اضافه تناژ";
                ws.Range($"B{routes.Count + 4}:D{routes.Count + 4}").Row(1).Merge();
                ws.Cell(routes.Count + 4, 5).Value = loadFactors.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value);

                ws.Cell($"B{routes.Count + 5}").Value = "جمع کل مبلغ اضافه تناژ";
                ws.Range($"B{routes.Count + 5}:D{routes.Count + 5}").Row(1).Merge();
                ws.Cell(routes.Count + 5, 5).Value = loadFactors.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.DriverTonnagePrice.Value).ToString("N0");

                ws.Cell($"B{routes.Count + 6}").Value = "جمع کرایه عملکرد";
                ws.Range($"B{routes.Count + 6}:D{routes.Count + 6}").Row(1).Merge();
                ws.Cell(routes.Count + 6, 5).Value = driverFeeTotal.ToString("N0");

                ws.Cell($"B{routes.Count + 7}").Value = "مجموع قابل پرداخت";
                ws.Range($"B{routes.Count + 7}:D{routes.Count + 7}").Row(1).Merge();
                ws.Cell(routes.Count + 7, 5).Value = (balance > 0 ? balance : 0).ToString("N0");

                var rngTable2 = ws.Range($"B{routes.Count + 3}:E{routes.Count + 7}");
                rngTable2.RangeUsed().Style
                    .Font.SetBold()
                    .Font.SetFontSize(12);
            }
            else
            {
                ws.Cell($"B{routes.Count + 4}").Value = "جمع کرایه عملکرد";
                ws.Range($"B{routes.Count + 4}:D{routes.Count + 4}").Row(1).Merge();
                ws.Cell(routes.Count + 4, 5).Value = driverFeeTotal.ToString("N0");

                ws.Cell($"B{routes.Count + 5}").Value = "مجموع قابل پرداخت";
                ws.Range($"B{routes.Count + 5}:D{routes.Count + 6}").Row(1).Merge();
                ws.Cell(routes.Count + 5, 5).Value = (balance > 0 ? balance : 0).ToString("N0");

                var rngTable2 = ws.Range($"B{routes.Count + 3}:E{routes.Count + 5}");
                rngTable2.RangeUsed().Style
                    .Font.SetBold()
                    .Font.SetFontSize(12);
            }

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.Columns().AdjustToContents();

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> CustomerIncome(long? id, long calendarId)
        //{
        //    if (!id.HasValue)
        //        return BadRequest("parameter error");

        //    if (!await _customerRepo.Customers().AnyAsync(a => a.Id.Equals(id.Value)))
        //        return NotFound("مشتری پیدا نشد");

        //    var customer = await _customerRepo.Get(id.Value);
        //    var calendar = await _calendarRepo.Get(calendarId);
        //    var incomes = await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CalendarId.Equals(calendarId) && a.CustomerId.Equals(id.Value)).OrderBy(a => a.Date).ToListAsync();

        //    string docTitle = $"گزارش دریافتی های {customer.Name}";

        //    using var workbook = new XLWorkbook();

        //    var ws = workbook.Worksheets.Add("Sheet1");
        //    ws.RightToLeft = true;
        //    ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

        //    ws.Cell("A1").Value = "ردیف";
        //    ws.Cell("B1").Value = "تاریخ";
        //    ws.Cell("C1").Value = "شرح";
        //    ws.Cell("D1").Value = "مبلغ";

        //    ws.Range("A1:D1").Style.Fill.SetBackgroundColor(XLColor.LightGray)
        //        .Font.SetBold(true)
        //        .Font.SetFontSize(12);

        //    for (int index = 1; index <= incomes.Count; index++)
        //    {
        //        ws.Cell(index + 1, 1).Value = index;
        //        ws.Cell(index + 1, 2).Value = new PersianDateTime(incomes[index - 1].Date).ToString("yyyy/MM/dd");
        //        ws.Cell(index + 1, 3).Value = incomes[index - 1].Description;
        //        ws.Cell(index + 1, 4).Value = incomes[index - 1].Amount.ToString("N0");
        //    }

        //    ws.Cell($"A{incomes.Count + 1}").Value = "جمع";
        //    ws.Range($"A{incomes.Count + 1}:C{incomes.Count + 1}").Row(1).Merge();
        //    ws.Cell($"D{incomes.Count + 1}").Value = incomes.Sum(a => a.Amount).ToString("N0");
        //    ws.Range($"A{incomes.Count + 1}:D{incomes.Count + 1}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
        //        .Font.SetBold(true);

        //    ws.Columns().AdjustToContents();
        //    ws.LastColumnUsed().Style.Font.SetBold(true);
        //    ws.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //    ws.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //    ws.CellsUsed().Style.Font.FontColor = XLColor.Black;
        //    ws.RowsUsed().Height = 20;
        //    ws.RangeUsed().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
        //        .Border.SetOutsideBorderColor(XLColor.Black)
        //        .Border.SetInsideBorder(XLBorderStyleValues.Thin)
        //        .Border.SetInsideBorderColor(XLColor.Black);

        //    await using var stream = new MemoryStream();
        //    workbook.SaveAs(stream);
        //    var content = stream.ToArray();

        //    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        //}

        public async Task<IActionResult> Customer(long customerId, long? calendarId, long? accountBookId, ExcelExportType? exportType)
        {
            if (User.IsInRole("Admin"))
            {
                if (!exportType.HasValue)
                    exportType = ExcelExportType.WithAllPrices;
            }
            else if (User.IsInRole("User"))
            {
                if (!exportType.HasValue)
                    exportType = ExcelExportType.OnlyDriverPrice;
            }
            else if (User.IsInRole("RegisterUser"))
                exportType = ExcelExportType.OnlyReceivingPrice;


            var customer = await _customerRepo.Get(customerId);
            if (customer == null) return NotFound("Customer not found");

            var calendar = new Calendar();
            if (calendarId.HasValue)
            {
                calendar = await _calendarRepo.Get(calendarId.Value);
                if (calendar == null) return NotFound("Calendar not found");
            }

            var allLoadFactors = await _loadFactorRepo.LoadFactors(customerId, calendarId, accountBookId, null);

            string docTitle = $"گزارش بارنامه {customer.Name}";
            if (calendarId.HasValue)
                docTitle += $" در {calendar.Title}";

            var accountBook = new AccountBook();
            if (accountBookId.HasValue)
            {
                if (allLoadFactors.Count == 0)
                    return NotFound("بارنامه ای در این صورت وضعیت درج نشده است.");

                accountBook = await _accountBookRepo.Get(accountBookId.Value);
                docTitle += $" با شماره صورت وضعیت {accountBook.Number}";
            }

            var EnglishNumbers = new List<(string Letter, int Num)>
            {
                ("A", 1),
                ("B", 2),
                ("C", 3),
                ("D", 4),
                ("E", 5),
                ("F", 6),
                ("G", 7),
                ("H", 8),
                ("I", 9),
                ("J", 10),
                ("K", 11),
                ("L", 12),
                ("M", 13),
                ("N", 14),
                ("O", 15),
                ("P", 16),
                ("Q", 17),
                ("R", 18),
                ("S", 19)
            };

            if (customer.CustomerType.Equals(CustomerType.SaipaPlasco))
            {
                allLoadFactors = allLoadFactors.OrderBy(a => a.VehicleName).ThenBy(a => a.OriginName).ThenBy(a => a.DestinationName).ThenBy(a => a.Date).ThenBy(a => a.LoadNumber).ToList();
                using var workbook = new XLWorkbook();

                #region first sheet list
                var list = workbook.Worksheets.Add("List");
                list.RightToLeft = true;
                list.Style.Font.FontName = "B Nazanin";
                list.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                list.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                if (accountBookId.HasValue)
                    list.Cell("A1").Value = accountBook.Number;

                list.Range("A1:B1").Row(1).Merge();
                list.Cell("C1").Value = "بارنامه های حمل کالا";
                list.Range("C1:E1").Row(1).Merge();
                list.Cell("H1").Value = "شرکت پلاسکو کار سایپا";
                list.Range("H1:I1").Row(1).Merge();
                list.Range("J1:K1").Row(1).Merge();
                list.Cell("J1").Value = "در تاریخ: ";
                if (calendarId.HasValue)
                    list.Cell("J1").Value = $"در تاریخ: {new PersianDateTime(calendar.EndDate).ToShortDateString()}";

                list.Cell(2, 1).Value = "ردیف";
                list.Cell(2, 2).Value = "تاریخ";
                list.Cell(2, 3).Value = "نام راننده";
                list.Cell(2, 4).Value = "شماره خودرو";
                list.Cell(2, 5).Value = "شماره بارنامه";
                list.Cell(2, 6).Value = "بارنامه اتحاد بار";
                list.Cell(2, 7).Value = "شماره خروجی";
                list.Cell(2, 8).Value = "مبدا";
                list.Cell(2, 9).Value = "مقصد";
                int sCounter = 9;
                if (exportType.HasValue)
                {
                    switch (exportType.Value)
                    {
                        case ExcelExportType.WithAllPrices:
                            list.Cell(2, 10).Value = "نرخ دریافتی";
                            list.Cell(2, 11).Value = "نرخ پرداختی";
                            sCounter += 2;
                            break;
                        case ExcelExportType.OnlyReceivingPrice:
                            list.Cell(2, 10).Value = "مبلغ بارنامه (ریال)";
                            sCounter++;
                            break;
                        case ExcelExportType.OnlyDriverPrice:
                            list.Cell(2, 10).Value = "مبلغ بارنامه (ریال)";
                            sCounter++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    list.Cell(2, 10).Value = "مبلغ بارنامه (ریال)";
                    sCounter++;
                }

                sCounter++;
                list.Cell(2, sCounter).Value = "خودرو";
                sCounter++;
                list.Cell(2, sCounter).Value = "تقویم";

                var rangeHeader = list.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(sCounter)).Letter}2");
                rangeHeader.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                rangeHeader.Style.Font.SetFontSize(11);

                rangeHeader.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium)
                    .Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Medium)
                    .Border.SetOutsideBorderColor(XLColor.Black);

                var rangeContent = list.Range(list.Cell("A3"), list.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(sCounter)).Letter}{allLoadFactors.Count + 2}"));
                rangeContent.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetOutsideBorderColor(XLColor.Black);

                for (int index = 1; index <= allLoadFactors.Count; index++)
                {
                    list.Cell(index + 2, 1).Value = index;
                    list.Cell(index + 2, 2).Value = new PersianDateTime(allLoadFactors[index - 1].Date).ToString("yyyy/MM/dd");
                    list.Cell(index + 2, 3).Value = allLoadFactors[index - 1].DriverName;
                    list.Cell(index + 2, 4).Value = $"{allLoadFactors[index - 1].VehicleRightNumber} {allLoadFactors[index - 1].VehicleNumberWord} {allLoadFactors[index - 1].VehicleLeftNumber}";
                    if (string.IsNullOrWhiteSpace(allLoadFactors[index - 1].LoadNumberGov))
                    {
                        list.Cell(index + 2, 5).SetValue(allLoadFactors[index - 1].LoadNumber);
                        list.Cell(index + 2, 6).SetValue("---");
                    }
                    else
                    {
                        list.Cell(index + 2, 5).SetValue(allLoadFactors[index - 1].LoadNumberGov);
                        list.Cell(index + 2, 6).SetValue(allLoadFactors[index - 1].LoadNumber);
                    }

                    list.Cell(index + 2, 7).SetValue(allLoadFactors[index - 1].ExitNumber);
                    list.Cell(index + 2, 8).Value = allLoadFactors[index - 1].OriginName;
                    list.Cell(index + 2, 9).Value = allLoadFactors[index - 1].DestinationName;
                    if (exportType.HasValue)
                    {
                        switch (exportType.Value)
                        {
                            case ExcelExportType.WithAllPrices:
                                list.Cell(index + 2, 10).Value = allLoadFactors[index - 1].Amount.ToString("N0");
                                list.Cell(index + 2, 11).Value = allLoadFactors[index - 1].DriverFee.ToString("N0");
                                break;
                            case ExcelExportType.OnlyReceivingPrice:
                                list.Cell(index + 2, 10).Value = allLoadFactors[index - 1].Amount.ToString("N0");
                                break;
                            case ExcelExportType.OnlyDriverPrice:
                                list.Cell(index + 2, 10).Value = allLoadFactors[index - 1].DriverFee.ToString("N0");
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        list.Cell(index + 2, 10).Value = allLoadFactors[index - 1].Amount.ToString("N0");

                    list.Cell(index + 2, sCounter - 1).Value = allLoadFactors[index - 1].VehicleName;
                    list.Cell(index + 2, sCounter).Value = allLoadFactors[index - 1].CalendarTitle;
                }

                list.Columns().AdjustToContents();
                list.Column(8).Width = 17;
                list.Column(9).Width = 17;
                list.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                list.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                list.CellsUsed().Style.Font.Bold = true;
                list.CellsUsed().Style.Font.FontColor = XLColor.Black;
                list.RowsUsed().Height = 24;

                list.PageSetup.SetPageOrientation(XLPageOrientation.Landscape)
                    .SetPaperSize(XLPaperSize.A4Paper);
                #endregion

                decimal c = Convert.ToDecimal(allLoadFactors.Count / 20f);
                double totalAmount = 0;
                double totalDriverFee = 0;
                for (int i = 1; i <= Convert.ToInt32(Math.Ceiling(c)); i++)
                {
                    var loadFactors = allLoadFactors.Skip((i - 1) * 20).Take(20).ToList();

                    var ws = workbook.Worksheets.Add($"Sheet{i}");
                    ws.RightToLeft = true;
                    ws.Style.Font.FontName = "B Nazanin";
                    ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                    ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                    if (accountBookId.HasValue)
                        ws.Cell("A1").Value = accountBook.Number;

                    ws.Range("A1:B1").Row(1).Merge();
                    ws.Cell("C1").Value = "بارنامه های حمل کالا";
                    ws.Range("C1:E1").Row(1).Merge();
                    ws.Cell("H1").Value = "شرکت پلاسکو کار سایپا";
                    ws.Range("H1:I1").Row(1).Merge();
                    ws.Range("J1:K1").Row(1).Merge();
                    ws.Cell("J1").Value = "در تاریخ: ";
                    if (calendarId.HasValue)
                        ws.Cell("J1").Value = $"در تاریخ: {new PersianDateTime(calendar.EndDate).ToShortDateString()}";

                    ws.Cell(2, 1).Value = "ردیف";
                    ws.Cell(2, 2).Value = "تاریخ";
                    ws.Cell(2, 3).Value = "نام راننده";
                    ws.Cell(2, 4).Value = "شماره خودرو";
                    ws.Cell(2, 5).Value = "شماره بارنامه";
                    ws.Cell(2, 6).Value = "بارنامه اتحاد بار";
                    ws.Cell(2, 7).Value = "شماره خروجی";
                    ws.Cell(2, 8).Value = "مبدا";
                    ws.Cell(2, 9).Value = "مقصد";

                    int switchCounter = 9;
                    if (exportType.HasValue)
                    {
                        switch (exportType.Value)
                        {
                            case ExcelExportType.WithAllPrices:
                                ws.Cell(2, 10).Value = "نرخ دریافتی";
                                ws.Cell(2, 11).Value = "نرخ پرداختی";
                                switchCounter += 2;
                                break;
                            case ExcelExportType.OnlyReceivingPrice:
                                ws.Cell(2, 10).Value = "مبلغ بارنامه (ریال)";
                                switchCounter++;
                                break;
                            case ExcelExportType.OnlyDriverPrice:
                                ws.Cell(2, 10).Value = "مبلغ بارنامه (ریال)";
                                switchCounter++;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ws.Cell(2, 10).Value = "مبلغ بارنامه (ریال)";
                        switchCounter++;
                    }

                    switchCounter++;
                    ws.Cell(2, switchCounter).Value = "خودرو";
                    switchCounter++;
                    ws.Cell(2, switchCounter).Value = "تقویم";

                    if (i != 1)
                    {
                        ws.Cell("A3").Value = "نقل از صفحه قبل";
                        if (exportType.HasValue)
                        {
                            switch (exportType.Value)
                            {
                                case ExcelExportType.WithAllPrices:
                                    ws.Range($"A3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 3)).Letter}3").Row(1).Merge();
                                    ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}3").Value = totalAmount.ToString("N0");
                                    ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}3").Value = totalDriverFee.ToString("N0");
                                    break;
                                case ExcelExportType.OnlyReceivingPrice:
                                    ws.Range($"A3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}3").Row(1).Merge();
                                    ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}3").Value = totalAmount.ToString("N0");
                                    break;
                                case ExcelExportType.OnlyDriverPrice:
                                    ws.Range($"A3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}3").Row(1).Merge();
                                    ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}3").Value = totalDriverFee.ToString("N0");
                                    break;
                                case ExcelExportType.WithoutPrice:
                                    ws.Range($"A3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}3").Row(1).Merge();
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            ws.Range($"A3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}3").Row(1).Merge();
                            ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}3").Value = totalAmount.ToString("N0");
                        }

                        var rngHeader = ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}3");
                        rngHeader.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                        rngHeader.Style.Font.SetFontSize(11);
                        rngHeader.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium)
                    .Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Medium)
                    .Border.SetOutsideBorderColor(XLColor.Black);

                        ws.Range($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}3").Style.Fill.SetBackgroundColor(XLColor.White);
                    }
                    else
                    {
                        var rngHeader = ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}2");
                        rngHeader.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                        rngHeader.Style.Font.SetFontSize(11);

                        rngHeader.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium)
                    .Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Medium)
                    .Border.SetOutsideBorderColor(XLColor.Black);
                    }

                    int rowIndex = 2;
                    if (i != 1) rowIndex = 3;

                    for (int index = 1; index <= loadFactors.Count; index++)
                    {
                        ws.Cell(index + rowIndex, 1).Value = ((20 * i) + (index - 1)) - (20 - 1);
                        ws.Cell(index + rowIndex, 2).Value = new PersianDateTime(loadFactors[index - 1].Date).ToString("yyyy/MM/dd");
                        ws.Cell(index + rowIndex, 3).Value = loadFactors[index - 1].DriverName;
                        ws.Cell(index + rowIndex, 4).Value = $"{loadFactors[index - 1].VehicleRightNumber} {loadFactors[index - 1].VehicleNumberWord} {loadFactors[index - 1].VehicleLeftNumber}";
                        if (string.IsNullOrWhiteSpace(loadFactors[index - 1].LoadNumberGov))
                        {
                            ws.Cell(index + rowIndex, 5).SetValue(loadFactors[index - 1].LoadNumber);
                            ws.Cell(index + rowIndex, 6).SetValue("---");
                        }
                        else
                        {
                            ws.Cell(index + rowIndex, 5).SetValue(loadFactors[index - 1].LoadNumberGov);
                            ws.Cell(index + rowIndex, 6).SetValue(loadFactors[index - 1].LoadNumber);
                        }

                        ws.Cell(index + rowIndex, 7).SetValue(loadFactors[index - 1].ExitNumber);
                        ws.Cell(index + rowIndex, 8).Value = loadFactors[index - 1].OriginName;
                        ws.Cell(index + rowIndex, 9).Value = loadFactors[index - 1].DestinationName;
                        if (exportType.HasValue)
                        {
                            switch (exportType.Value)
                            {
                                case ExcelExportType.WithAllPrices:
                                    ws.Cell(index + rowIndex, 10).Value = loadFactors[index - 1].Amount.ToString("N0");
                                    ws.Cell(index + rowIndex, 11).Value = loadFactors[index - 1].DriverFee.ToString("N0");
                                    break;
                                case ExcelExportType.OnlyReceivingPrice:
                                    ws.Cell(index + rowIndex, 10).Value = loadFactors[index - 1].Amount.ToString("N0");
                                    break;
                                case ExcelExportType.OnlyDriverPrice:
                                    ws.Cell(index + rowIndex, 10).Value = loadFactors[index - 1].DriverFee.ToString("N0");
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                            ws.Cell(index + rowIndex, 10).Value = loadFactors[index - 1].Amount.ToString("N0");

                        ws.Cell(index + rowIndex, switchCounter - 1).Value = loadFactors[index - 1].VehicleName;
                        ws.Cell(index + rowIndex, switchCounter).Value = loadFactors[index - 1].CalendarTitle;
                    }

                    totalAmount += loadFactors.Sum(a => a.Amount);
                    totalDriverFee += loadFactors.Sum(a => a.DriverFee);

                    if (exportType.HasValue)
                    {
                        switch (exportType.Value)
                        {
                            case ExcelExportType.WithAllPrices:
                                ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "جمع";
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 3)).Letter}{loadFactors.Count + rowIndex}").Row(1).Merge();
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}{loadFactors.Count + rowIndex + 1}").SetValue(totalAmount.ToString("N0"))
                                    .Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black)
                                    .Border.SetLeftBorder(XLBorderStyleValues.Thin).Border.SetLeftBorderColor(XLColor.Black);
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{loadFactors.Count + rowIndex + 1}").SetValue(totalDriverFee.ToString("N0"))
                                    .Style.Border.SetLeftBorder(XLBorderStyleValues.Thin).Border.SetLeftBorderColor(XLColor.Black);
                                //ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black).Border.SetLeftBorder(XLBorderStyleValues.None);

                                ws.Cell($"A{loadFactors.Count + rowIndex + 2}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                                ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge();
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:L{loadFactors.Count + rowIndex + 2}").RowsUsed().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);
                                break;
                            case ExcelExportType.OnlyReceivingPrice:
                                ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "جمع";
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}{loadFactors.Count + rowIndex}").Row(1).Merge();
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{loadFactors.Count + rowIndex + 1}").SetValue(totalAmount.ToString("N0"))
                                    .Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black)
                                    .Border.SetLeftBorder(XLBorderStyleValues.Thin).Border.SetLeftBorderColor(XLColor.Black);
                                //ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black).Border.SetLeftBorder(XLBorderStyleValues.None);

                                ws.Cell($"A{loadFactors.Count + rowIndex + 2}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                                ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge();
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:K{loadFactors.Count + rowIndex + 2}").RowsUsed().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);
                                break;
                            case ExcelExportType.OnlyDriverPrice:
                                ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "جمع";
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}{loadFactors.Count + rowIndex}").Row(1).Merge();
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{loadFactors.Count + rowIndex + 1}").SetValue(totalDriverFee.ToString("N0"))
                                    .Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black)
                                    .Border.SetLeftBorder(XLBorderStyleValues.Thin).Border.SetLeftBorderColor(XLColor.Black);
                                //ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black).Border.SetLeftBorder(XLBorderStyleValues.None);

                                ws.Cell($"A{loadFactors.Count + rowIndex + 2}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                                ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge();
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:K{loadFactors.Count + rowIndex + 2}").RowsUsed().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);
                                break;
                            case ExcelExportType.WithoutPrice:

                                ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "جمع";
                        ws.Range($"A{loadFactors.Count + rowIndex + 1}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}{loadFactors.Count + rowIndex}").Row(1).Merge();
                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{loadFactors.Count + rowIndex + 1}").SetValue(totalAmount.ToString("N0"))
                            .Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black)
                                    .Border.SetLeftBorder(XLBorderStyleValues.Thin).Border.SetLeftBorderColor(XLColor.Black);
                        //ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black).Border.SetLeftBorder(XLBorderStyleValues.None);

                        ws.Cell($"A{loadFactors.Count + rowIndex + 2}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                        ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge();
                        ws.Range($"A{loadFactors.Count + rowIndex + 1}:K{loadFactors.Count + rowIndex + 2}").RowsUsed().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);
                    }

                    if (i == 1)
                    {
                        var rngContent = ws.Range(ws.Cell("A3"), ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + 2}"));
                        rngContent.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetOutsideBorderColor(XLColor.Black);
                    }
                    else
                    {
                        var rngContent = ws.Range(ws.Cell("A4"), ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + 3}"));
                        rngContent.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetOutsideBorderColor(XLColor.Black);
                    }

                    //ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 2}").Style.Border.SetInsideBorder(XLBorderStyleValues.None);
                    //ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 2}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                    ws.Columns().AdjustToContents();
                    ws.Column(8).Width = 17;
                    ws.Column(9).Width = 17;
                    ws.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.CellsUsed().Style.Font.Bold = true;
                    ws.CellsUsed().Style.Font.FontColor = XLColor.Black;
                    ws.RowsUsed().Height = 24;

                    ws.PageSetup.SetPageOrientation(XLPageOrientation.Landscape)
                        .SetPaperSize(XLPaperSize.A4Paper);
                }

                await using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
            }
            else if (customer.CustomerType.Equals(CustomerType.SazehGostar))
            {
                using var workbook = new XLWorkbook();

                var ws = workbook.Worksheets.Add("Sheet1");
                ws.RightToLeft = true;
                ws.Style.Font.FontName = "B Nazanin";
                ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                ws.Cell(2, 1).Value = "ردیف";
                ws.Cell(2, 2).SetValue("کد علت صدور").Style.Font.SetFontSize(7);
                ws.Cell(2, 3).Value = "معین";
                ws.Cell(2, 4).Value = "ماهیت";
                ws.Cell(2, 5).Value = "پلاک";
                ws.Cell(2, 6).Value = "شماره بارنامه";
                ws.Cell(2, 7).Value = "نوع خودرو";
                ws.Cell(2, 8).Value = "روز";
                ws.Cell(2, 9).Value = "ماه";
                ws.Cell(2, 10).Value = "سال";
                ws.Cell(2, 11).Value = "شرح سند";
                ws.Cell(2, 12).Value = "تعداد";
                ws.Cell(2, 13).Value = "تفضیلی مرکز هزینه";

                int switchCounter = 13;
                if (exportType.HasValue)
                {
                    switch (exportType.Value)
                    {
                        case ExcelExportType.WithAllPrices:
                            ws.Cell(2, 14).Value = "نرخ دریافتی";
                            ws.Cell(2, 15).Value = "نرخ پرداختی";
                            switchCounter += 2;
                            break;
                        case ExcelExportType.OnlyReceivingPrice:
                            ws.Cell(2, 14).Value = "مبلغ";
                            switchCounter++;
                            break;
                        case ExcelExportType.OnlyDriverPrice:
                            ws.Cell(2, 14).Value = "مبلغ";
                            switchCounter++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ws.Cell(2, 14).Value = "مبلغ";
                    switchCounter++;
                }

                switchCounter++;
                ws.Cell(2, switchCounter).Value = "شماره درخواست";
                switchCounter++;
                ws.Cell(2, switchCounter).Value = "راننده";
                switchCounter++;
                ws.Cell(2, switchCounter).Value = "پلاک";
                switchCounter++;
                ws.Cell(2, switchCounter).Value = "تقویم";

                string headerText = "";
                if (accountBookId.HasValue)
                    headerText = $"صورت وضعیت شماره {accountBook.Number} شرکت حمل و نقل اتحاد بار آسیا";
                else
                    headerText = $"بارنامه های {calendar.Title} شرکت حمل و نقل اتحاد بار آسیا";

                ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}1").Merge().Value = headerText;
                ws.Range($"A2:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}2").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                    .Font.SetBold(true);

                allLoadFactors = allLoadFactors.OrderBy(a => a.VehicleName).ThenBy(a => a.OriginName).ThenBy(a => a.DestinationName).ThenBy(a => a.SazehGostarLoadFactor.SazehLoadType).ThenBy(a => a.Date).ThenBy(a => a.LoadNumber).ToList();
                for (int index = 1; index <= allLoadFactors.Count; index++)
                {
                    var pd = new PersianDateTime(allLoadFactors[index - 1].Date);

                    ws.Cell(index + 2, 1).Value = index;
                    ws.Cell(index + 2, 2).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.RegisterCode;
                    ws.Cell(index + 2, 3).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.Certain;
                    ws.Cell(index + 2, 4).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.Nature;
                    ws.Cell(index + 2, 5).Value = "*";
                    ws.Cell(index + 2, 6).SetValue(allLoadFactors[index - 1].LoadNumber);
                    ws.Cell(index + 2, 7).Value = allLoadFactors[index - 1].VehicleName;
                    ws.Cell(index + 2, 8).Value = pd.Day;
                    ws.Cell(index + 2, 9).Value = pd.Month;
                    ws.Cell(index + 2, 10).Value = pd.Year;
                    ws.Cell(index + 2, 11).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.Description;
                    ws.Cell(index + 2, 12).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.Count;
                    ws.Cell(index + 2, 13).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.DetailedCostCenter;
                    if (exportType.HasValue)
                    {
                        switch (exportType.Value)
                        {
                            case ExcelExportType.WithAllPrices:
                                ws.Cell(index + 2, 14).Value = allLoadFactors[index - 1].Amount.ToString("N0");
                                ws.Cell(index + 2, 15).Value = allLoadFactors[index - 1].DriverFee.ToString("N0");
                                break;
                            case ExcelExportType.OnlyReceivingPrice:
                                ws.Cell(index + 2, 14).Value = allLoadFactors[index - 1].Amount.ToString("N0");
                                break;
                            case ExcelExportType.OnlyDriverPrice:
                                ws.Cell(index + 2, 14).Value = allLoadFactors[index - 1].DriverFee.ToString("N0");
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        ws.Cell(index + 2, 14).Value = allLoadFactors[index - 1].Amount.ToString("N0");

                    ws.Cell(index + 2, switchCounter - 3).SetValue(allLoadFactors[index - 1].ExitNumber);
                    ws.Cell(index + 2, switchCounter - 2).Value = allLoadFactors[index - 1].DriverName;
                    ws.Cell(index + 2, switchCounter - 1).Value = $"{allLoadFactors[index - 1].VehicleRightNumber} {allLoadFactors[index - 1].VehicleNumberWord} {allLoadFactors[index - 1].VehicleLeftNumber}";
                    ws.Cell(index + 2, switchCounter).Value = allLoadFactors[index - 1].CalendarTitle;
                }

                ws.Cell($"A{allLoadFactors.Count + 3}").Value = "جمع کل بارنامه ها";
                ws.Range($"A{allLoadFactors.Count + 3}:K{allLoadFactors.Count + 3}").Row(1).Merge();
                ws.Cell($"L{allLoadFactors.Count + 3}").Value = "1";
                ws.Cell($"M{allLoadFactors.Count + 3}").Value = "800720";
                if (exportType.HasValue)
                {
                    switch (exportType.Value)
                    {
                        case ExcelExportType.WithAllPrices:
                            ws.Cell($"N{allLoadFactors.Count + 3}").Value = allLoadFactors.Sum(a => a.Amount).ToString("N0");
                            ws.Cell($"O{allLoadFactors.Count + 3}").Value = allLoadFactors.Sum(a => a.DriverFee).ToString("N0");
                            ws.Range($"P{allLoadFactors.Count + 3}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 3}").Row(1).Merge();
                            break;
                        case ExcelExportType.OnlyReceivingPrice:
                            ws.Cell($"N{allLoadFactors.Count + 3}").Value = allLoadFactors.Sum(a => a.Amount).ToString("N0");
                            ws.Range($"O{allLoadFactors.Count + 3}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 3}").Row(1).Merge();
                            break;
                        case ExcelExportType.OnlyDriverPrice:
                            ws.Cell($"N{allLoadFactors.Count + 3}").Value = allLoadFactors.Sum(a => a.DriverFee).ToString("N0");
                            ws.Range($"O{allLoadFactors.Count + 3}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 3}").Row(1).Merge();
                            break;
                        case ExcelExportType.WithoutPrice:
                            ws.Range($"N{allLoadFactors.Count + 3}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 3}").Row(1).Merge();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ws.Cell($"N{allLoadFactors.Count + 3}").Value = allLoadFactors.Sum(a => a.Amount).ToString("N0");
                    ws.Range($"O{allLoadFactors.Count + 3}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 3}").Row(1).Merge();
                }
                ws.Range($"A{allLoadFactors.Count + 3}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 3}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                    .Font.SetBold(true);

                if (exportType.HasValue)
                {
                    if (exportType.Value == ExcelExportType.WithAllPrices || exportType.Value == ExcelExportType.OnlyReceivingPrice)
                    {
                        ws.Cell($"A{allLoadFactors.Count + 4}").Value = allLoadFactors.Count + 1;
                        ws.Cell($"B{allLoadFactors.Count + 4}").Value = "906";
                        ws.Cell($"C{allLoadFactors.Count + 4}").Value = "1452";
                        ws.Cell($"D{allLoadFactors.Count + 4}").Value = "1";
                        ws.Cell($"E{allLoadFactors.Count + 4}").Value = "*";
                        ws.Cell($"F{allLoadFactors.Count + 4}").Value = "97001";
                        ws.Cell($"H{allLoadFactors.Count + 4}").Value = "0";
                        ws.Cell($"I{allLoadFactors.Count + 4}").Value = "0";
                        ws.Cell($"J{allLoadFactors.Count + 4}").Value = "0";
                        ws.Cell($"L{allLoadFactors.Count + 4}").Value = "0";

                        ws.Cell($"A{allLoadFactors.Count + 5}").Value = allLoadFactors.Count + 2;
                        ws.Cell($"B{allLoadFactors.Count + 5}").Value = "907";
                        ws.Cell($"C{allLoadFactors.Count + 5}").Value = "1453";
                        ws.Cell($"D{allLoadFactors.Count + 5}").Value = "1";
                        ws.Cell($"E{allLoadFactors.Count + 5}").Value = "*";
                        ws.Cell($"F{allLoadFactors.Count + 5}").Value = "97001";
                        ws.Cell($"H{allLoadFactors.Count + 5}").Value = "0";
                        ws.Cell($"I{allLoadFactors.Count + 5}").Value = "0";
                        ws.Cell($"J{allLoadFactors.Count + 5}").Value = "0";
                        ws.Cell($"L{allLoadFactors.Count + 5}").Value = "0";

                        ws.Cell($"A{allLoadFactors.Count + 6}").Value = allLoadFactors.Count + 3;
                        ws.Cell($"B{allLoadFactors.Count + 6}").Value = "472";
                        ws.Cell($"C{allLoadFactors.Count + 6}").Value = "3427";
                        ws.Cell($"D{allLoadFactors.Count + 6}").Value = "1";
                        ws.Cell($"E{allLoadFactors.Count + 6}").Value = "*";
                        ws.Cell($"F{allLoadFactors.Count + 6}").Value = "97001";
                        ws.Cell($"H{allLoadFactors.Count + 6}").Value = "0";
                        ws.Cell($"I{allLoadFactors.Count + 6}").Value = "0";
                        ws.Cell($"J{allLoadFactors.Count + 6}").Value = "0";
                        ws.Cell($"K{allLoadFactors.Count + 6}").Value = $"بیمه 7.8% خلاصه {(accountBook != null ? accountBook.Number : "0")} اتحاد بار {allLoadFactors.Count} بارنامه";
                        ws.Cell($"L{allLoadFactors.Count + 6}").Value = "0";
                        ws.Cell($"M{allLoadFactors.Count + 6}").Value = ((allLoadFactors.Sum(a => a.Amount) * 7.8) / 100).ToString("N0");

                        ws.Cell($"A{allLoadFactors.Count + 7}").Value = allLoadFactors.Count + 4;
                        ws.Cell($"B{allLoadFactors.Count + 7}").Value = "080";
                        ws.Cell($"C{allLoadFactors.Count + 7}").Value = "3442";
                        ws.Cell($"D{allLoadFactors.Count + 7}").Value = "1";
                        ws.Cell($"E{allLoadFactors.Count + 7}").Value = "*";
                        ws.Cell($"F{allLoadFactors.Count + 7}").Value = "97001";
                        ws.Cell($"H{allLoadFactors.Count + 7}").Value = "0";
                        ws.Cell($"I{allLoadFactors.Count + 7}").Value = "0";
                        ws.Cell($"J{allLoadFactors.Count + 7}").Value = "0";
                        ws.Cell($"K{allLoadFactors.Count + 7}").Value = $"خالص پرداختی خلاصه {(accountBook != null ? accountBook.Number : "0")} {allLoadFactors.Count} بارنامه";
                        ws.Cell($"L{allLoadFactors.Count + 7}").Value = "0";
                        ws.Cell($"M{allLoadFactors.Count + 7}").Value = (allLoadFactors.Sum(a => a.Amount) - ((allLoadFactors.Sum(a => a.Amount) * 7.8) / 100)).ToString("N0");
                    }
                }
                else
                {
                    ws.Cell($"A{allLoadFactors.Count + 4}").Value = allLoadFactors.Count + 1;
                    ws.Cell($"B{allLoadFactors.Count + 4}").Value = "906";
                    ws.Cell($"C{allLoadFactors.Count + 4}").Value = "1452";
                    ws.Cell($"D{allLoadFactors.Count + 4}").Value = "1";
                    ws.Cell($"E{allLoadFactors.Count + 4}").Value = "*";
                    ws.Cell($"F{allLoadFactors.Count + 4}").Value = "97001";
                    ws.Cell($"H{allLoadFactors.Count + 4}").Value = "0";
                    ws.Cell($"I{allLoadFactors.Count + 4}").Value = "0";
                    ws.Cell($"J{allLoadFactors.Count + 4}").Value = "0";
                    ws.Cell($"L{allLoadFactors.Count + 4}").Value = "0";

                    ws.Cell($"A{allLoadFactors.Count + 5}").Value = allLoadFactors.Count + 2;
                    ws.Cell($"B{allLoadFactors.Count + 5}").Value = "907";
                    ws.Cell($"C{allLoadFactors.Count + 5}").Value = "1453";
                    ws.Cell($"D{allLoadFactors.Count + 5}").Value = "1";
                    ws.Cell($"E{allLoadFactors.Count + 5}").Value = "*";
                    ws.Cell($"F{allLoadFactors.Count + 5}").Value = "97001";
                    ws.Cell($"H{allLoadFactors.Count + 5}").Value = "0";
                    ws.Cell($"I{allLoadFactors.Count + 5}").Value = "0";
                    ws.Cell($"J{allLoadFactors.Count + 5}").Value = "0";
                    ws.Cell($"L{allLoadFactors.Count + 5}").Value = "0";

                    ws.Cell($"A{allLoadFactors.Count + 6}").Value = allLoadFactors.Count + 3;
                    ws.Cell($"B{allLoadFactors.Count + 6}").Value = "472";
                    ws.Cell($"C{allLoadFactors.Count + 6}").Value = "3427";
                    ws.Cell($"D{allLoadFactors.Count + 6}").Value = "1";
                    ws.Cell($"E{allLoadFactors.Count + 6}").Value = "*";
                    ws.Cell($"F{allLoadFactors.Count + 6}").Value = "97001";
                    ws.Cell($"H{allLoadFactors.Count + 6}").Value = "0";
                    ws.Cell($"I{allLoadFactors.Count + 6}").Value = "0";
                    ws.Cell($"J{allLoadFactors.Count + 6}").Value = "0";
                    ws.Cell($"K{allLoadFactors.Count + 6}").Value = $"بیمه 7.8% خلاصه {(accountBook != null ? accountBook.Number : "0")} اتحاد بار {allLoadFactors.Count} بارنامه";
                    ws.Cell($"L{allLoadFactors.Count + 6}").Value = "0";
                    ws.Cell($"M{allLoadFactors.Count + 6}").Value = ((allLoadFactors.Sum(a => a.Amount) * 7.8) / 100).ToString("N0");

                    ws.Cell($"A{allLoadFactors.Count + 7}").Value = allLoadFactors.Count + 4;
                    ws.Cell($"B{allLoadFactors.Count + 7}").Value = "080";
                    ws.Cell($"C{allLoadFactors.Count + 7}").Value = "3442";
                    ws.Cell($"D{allLoadFactors.Count + 7}").Value = "1";
                    ws.Cell($"E{allLoadFactors.Count + 7}").Value = "*";
                    ws.Cell($"F{allLoadFactors.Count + 7}").Value = "97001";
                    ws.Cell($"H{allLoadFactors.Count + 7}").Value = "0";
                    ws.Cell($"I{allLoadFactors.Count + 7}").Value = "0";
                    ws.Cell($"J{allLoadFactors.Count + 7}").Value = "0";
                    ws.Cell($"K{allLoadFactors.Count + 7}").Value = $"خالص پرداختی خلاصه {(accountBook != null ? accountBook.Number : "0")} {allLoadFactors.Count} بارنامه";
                    ws.Cell($"L{allLoadFactors.Count + 7}").Value = "0";
                    ws.Cell($"M{allLoadFactors.Count + 7}").Value = (allLoadFactors.Sum(a => a.Amount) - ((allLoadFactors.Sum(a => a.Amount) * 7.8) / 100)).ToString("N0");
                }

                ws.Columns().AdjustToContents();
                ws.LastColumnUsed().Style.Font.SetBold(true);
                ws.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.CellsUsed().Style.Font.FontColor = XLColor.Black;
                ws.RowsUsed().Height = 20;
                ws.RangeUsed().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetOutsideBorderColor(XLColor.Black)
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetInsideBorderColor(XLColor.Black);

                await using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
            }
            else if (customer.CustomerType.Equals(CustomerType.SaipaPress))
            {
                using var workbook = new XLWorkbook();

                var oneFloor = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.OneFloor && !a.Tonnage.HasValue).OrderBy(a => a.SaipaPressLoadFactor.Sequence).ToList();

                var ws = workbook.Worksheets.Add("یک طبقه");
                MakePressSheet(oneFloor, ws, exportType);

                var twoFloor = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.TwoFloor && !a.Tonnage.HasValue).OrderBy(a => a.SaipaPressLoadFactor.Sequence).ToList();
                if (twoFloor.Any())
                {
                    var ws2 = workbook.Worksheets.Add("دو طبقه");
                    MakePressSheet(twoFloor, ws2, exportType);
                }

                var oneFloorWithTonnage = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.OneFloor && a.Tonnage.HasValue).OrderBy(a => a.SaipaPressLoadFactor.Sequence).ToList();
                if (oneFloorWithTonnage.Any())
                {
                    var ws2 = workbook.Worksheets.Add("یک طبقه با تناژ اضافه");
                    MakePressSheet(oneFloorWithTonnage, ws2, exportType);
                }

                var twoFloorWithTonnage = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.TwoFloor && a.Tonnage.HasValue).OrderBy(a => a.SaipaPressLoadFactor.Sequence).ToList();
                if (twoFloorWithTonnage.Any())
                {
                    var ws2 = workbook.Worksheets.Add("دو طبقه با تناژ اضافه");
                    MakePressSheet(twoFloorWithTonnage, ws2, exportType);
                }

                await using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
            }
            else
            {
                using var workbook = new XLWorkbook();

                if (accountBookId.HasValue)
                {
                    var ws = workbook.Worksheets.Add($"1");
                    ws.RightToLeft = true;
                    ws.Style.Font.FontName = "B Titr";
                    ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                    ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                    ws.Cell("A4").Value = "ردیف";
                    ws.Cell("B4").Value = "شماره زونکن";
                    ws.Cell("C4").Value = "تاریخ";
                    ws.Cell("D4").Value = "شماره بارنامه داخلی";
                    ws.Cell("E4").Value = "نوع خودرو";
                    ws.Cell("F4").Value = "نام راننده";
                    ws.Cell("G4").Value = "شماره خودرو";
                    ws.Cell("H4").Value = "مبدا";
                    ws.Cell("I4").Value = "مقصد";
                    ws.Cell("J4").Value = "دولتی";
                    ws.Cell("K4").Value = "دولتی برگشتی";
                    ws.Cell("L4").Value = "بار";
                    ws.Cell("M4").Value = "پالت";
                    ws.Cell("N4").Value = "برگشتی";
                    int switchCounter = 14;
                    if (exportType.HasValue)
                    {
                        switch (exportType.Value)
                        {
                            case ExcelExportType.WithAllPrices:
                                ws.Cell(4, 15).Value = "نرخ دریافتی";
                                ws.Cell(4, 16).Value = "نرخ پرداختی";
                                switchCounter += 2;
                                break;
                            case ExcelExportType.OnlyReceivingPrice:
                                ws.Cell(4, 15).Value = "مبلغ بارنامه";
                                switchCounter++;
                                break;
                            case ExcelExportType.OnlyDriverPrice:
                                ws.Cell(4, 15).Value = "مبلغ بارنامه";
                                switchCounter++;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ws.Cell(4, 15).Value = "مبلغ بارنامه";
                        switchCounter++;
                    }
                    ws.Cell(4, switchCounter + 1).Value = "ماه";
                    ws.Cell(4, switchCounter + 2).Value = "سال";
                    ws.Cell(4, switchCounter + 3).Value = "تقویم";

                    var calendarPd = new PersianDateTime(allLoadFactors.First().CalendarStartDate);

                    allLoadFactors = allLoadFactors.OrderBy(a => a.VehicleName).ThenBy(a => a.Date).ThenBy(a => a.LoadNumber).ToList();
                    for (int i = 0; i < allLoadFactors.Count; i++)
                    {
                        var carNumber = $"{allLoadFactors[i].VehicleRightNumber}{allLoadFactors[i].VehicleNumberWord}{allLoadFactors[i].VehicleLeftNumber}ایران{allLoadFactors[i].VehicleIranStateNumber}";
                        var date = new PersianDateTime(allLoadFactors[i].Date);

                        #region handling sleep time and weighbridge
                        if (allLoadFactors[i].WeighbridgePrice.HasValue)
                        {
                            allLoadFactors[i].DriverFee += allLoadFactors[i].WeighbridgePrice.Value;

                            allLoadFactors[i].Amount += allLoadFactors[i].WeighbridgePrice.Value;
                        }

                        if (allLoadFactors[i].LoadSleepTime.HasValue)
                        {
                            allLoadFactors[i].DriverFee += allLoadFactors[i].DriverLoadSleepPrice.Value;

                            allLoadFactors[i].Amount += allLoadFactors[i].LoadSleepPrice.Value;
                        }

                        if (allLoadFactors[i].Tonnage.HasValue)
                        {
                            allLoadFactors[i].DriverFee = allLoadFactors[i].DriverFee + (allLoadFactors[i].Tonnage.Value * allLoadFactors[i].DriverTonnagePrice.Value);

                            allLoadFactors[i].Amount = allLoadFactors[i].Amount + (allLoadFactors[i].Tonnage.Value * allLoadFactors[i].TonnagePrice.Value);
                        }
                        #endregion

                        ws.Cell($"A{i + 5}").Value = i + 1;
                        ws.Cell($"B{i + 5}").Value = accountBook.Number;
                        ws.Cell($"C{i + 5}").Value = date.ToString("yyyy/MM/dd");
                        ws.Cell($"D{i + 5}").SetValue(allLoadFactors[i].LoadNumber);
                        ws.Cell($"E{i + 5}").Value = allLoadFactors[i].VehicleName;
                        ws.Cell($"F{i + 5}").Value = allLoadFactors[i].DriverName;
                        ws.Cell($"G{i + 5}").Value = carNumber;
                        ws.Cell($"H{i + 5}").Value = allLoadFactors[i].OriginName;
                        ws.Cell($"I{i + 5}").Value = allLoadFactors[i].DestinationName;
                        ws.Cell($"J{i + 5}").SetValue(allLoadFactors[i].LoadNumberGov);
                        ws.Cell($"K{i + 5}").Value = allLoadFactors[i].MehrcomParsLoadFactor.LoadNumberGovReturn;
                        ws.Cell($"L{i + 5}").Value = allLoadFactors[i].MehrcomParsLoadFactor.Load ? "1" : "0";
                        ws.Cell($"M{i + 5}").Value = allLoadFactors[i].MehrcomParsLoadFactor.Palette ? "1" : "0";
                        ws.Cell($"N{i + 5}").Value = allLoadFactors[i].MehrcomParsLoadFactor.Return ? "1" : "0";
                        if (exportType.HasValue)
                        {
                            switch (exportType.Value)
                            {
                                case ExcelExportType.WithAllPrices:
                                    ws.Cell(i + 5, 15).Value = allLoadFactors[i].Amount.ToString("N0");
                                    ws.Cell(i + 5, 16).Value = allLoadFactors[i].DriverFee.ToString("N0");
                                    break;
                                case ExcelExportType.OnlyReceivingPrice:
                                    ws.Cell(i + 5, 15).Value = allLoadFactors[i].Amount.ToString("N0");
                                    break;
                                case ExcelExportType.OnlyDriverPrice:
                                    ws.Cell(i + 5, 15).Value = allLoadFactors[i].DriverFee.ToString("N0");
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                            ws.Cell(i + 5, 15).Value = allLoadFactors[i].DriverFee.ToString("N0");

                        ws.Cell(i + 5, switchCounter + 1).Value = calendarPd.ToString("MMMM");
                        ws.Cell(i + 5, switchCounter + 2).Value = calendarPd.ToString("yyyy");
                        ws.Cell(i + 5, switchCounter + 3).Value = allLoadFactors[i].CalendarTitle;

                        #region handling comment
                        string commentText = "";
                        if (allLoadFactors[i].WeighbridgePrice.HasValue)
                            commentText += $"مبلغ باسکول: {allLoadFactors[i].WeighbridgePrice.Value.ToString("N0")}";

                        if (allLoadFactors[i].LoadSleepTime.HasValue)
                        {
                            if (!string.IsNullOrWhiteSpace(commentText))
                                commentText += " | ";

                            commentText += $"زمان خواب: {allLoadFactors[i].LoadSleepTime.Value} | مبلغ خواب: {(exportType.HasValue ? (exportType.Value == ExcelExportType.WithAllPrices || exportType.Value == ExcelExportType.OnlyReceivingPrice) ? allLoadFactors[i].LoadSleepPrice.Value.ToString("N0") : allLoadFactors[i].DriverLoadSleepPrice.Value.ToString("N0") : allLoadFactors[i].DriverLoadSleepPrice.Value.ToString("N0"))}";
                        }

                        if (allLoadFactors[i].Tonnage.HasValue)
                        {
                            if (!string.IsNullOrWhiteSpace(commentText))
                                commentText += " | ";

                            commentText += $"مبلغ اضافه تناژ: {(exportType.HasValue ? (exportType.Value == ExcelExportType.WithAllPrices || exportType.Value == ExcelExportType.OnlyReceivingPrice) ? allLoadFactors[i].TonnagePrice.Value.ToString("N0") : allLoadFactors[i].DriverTonnagePrice.Value.ToString("N0") : allLoadFactors[i].DriverTonnagePrice.Value.ToString("N0"))}";
                        }

                        if (!string.IsNullOrWhiteSpace(commentText))
                        {
                            var cell = ws.Cell($"D{i + 5}");
                            cell.Comment.AddText(commentText);
                            cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                        }
                        #endregion
                    }

                    #region making header
                    ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}1").Merge();
                    ws.Range($"A2:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}2").Merge();
                    ws.Range($"A3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}3").Merge();
                    ws.Cell("A1").Value = "اتحاد بار آسیا";
                    ws.Cell("A2").Value = $"اطلاعات زونکن شماره {accountBook.Number}";
                    ws.Range($"A4:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}4").Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}4").Style.Font.SetFontSize(12);
                    #endregion

                    if (exportType.HasValue)
                    {
                        switch (exportType.Value)
                        {
                            case ExcelExportType.WithAllPrices:
                                ws.Cell(allLoadFactors.Count + 5, 15).Value = allLoadFactors.Sum(a => a.Amount).ToString("N0");
                                ws.Cell(allLoadFactors.Count + 5, 16).Value = allLoadFactors.Sum(a => a.DriverFee).ToString("N0");
                                break;
                            case ExcelExportType.OnlyReceivingPrice:
                                ws.Cell(allLoadFactors.Count + 5, 15).Value = allLoadFactors.Sum(a => a.Amount).ToString("N0");
                                break;
                            case ExcelExportType.OnlyDriverPrice:
                                ws.Cell(allLoadFactors.Count + 5, 15).Value = allLoadFactors.Sum(a => a.DriverFee).ToString("N0");
                                break;
                            case ExcelExportType.WithoutPrice:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        ws.Cell(allLoadFactors.Count + 5, 15).Value = allLoadFactors.Sum(a => a.DriverFee).ToString("N0");


                    ws.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.CellsUsed().Style.Font.Bold = true;
                    ws.CellsUsed().Style.Font.FontColor = XLColor.Black;
                    ws.CellsUsed().Style.Border.BottomBorderColor = XLColor.Black;
                    var table = ws.Range($"A4:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}{allLoadFactors.Count + 5}").CreateTable();
                    table.Theme = XLTableTheme.None;
                    table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    table.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                    ws.RowsUsed().Height = 25;

                    ws.Column("A").Width = 4.5;
                    ws.Column("B").Width = 6;
                    ws.Column("C").Width = 10;
                    ws.Column("F").Width = 12.5;
                    ws.Column("H").Width = 10;
                    ws.Column("I").Width = 10;
                    ws.Column("G").Width = 13.5;
                    ws.Column("L").Width = 3;
                    ws.Column("M").Width = 3;
                    ws.Column("N").Width = 3;

                    #region Summary
                    var ws2 = workbook.Worksheets.Add($"2");
                    ws2.RightToLeft = true;
                    ws2.Style.Font.FontName = "B Titr";
                    ws2.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                    ws2.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                    ws2.Range("A1:F1").Merge();
                    ws2.Cell(1, 1).Value = "اتحاد بار آسیا";
                    ws2.Cell(1, 1).Style.Font.SetFontSize(40).Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                    ws2.Range("A2:C2").Merge();
                    ws2.Cell("A2").Value = $"شماره زونکن: {accountBook.Number}";
                    ws2.Range("D2:E2").Merge();
                    ws2.Row(2).Style.Font.SetFontSize(18);

                    ws2.Column(5).Width = 35;
                    ws2.Column(6).Width = 35;
                    ws2.Columns(1, 3).Width = 12;

                    ws2.Cell("A3").Value = "ردیف";
                    ws2.Cell("B3").Value = "ماه";
                    ws2.Cell("C3").Value = "زونکن";
                    ws2.Cell("D3").Value = "تعداد";
                    ws2.Cell("E3").Value = "قیمت";
                    ws2.Cell("F3").Value = "نوع خدمت ارائه شده";

                    ws2.Range("A3:F3").Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    var ws2Data = from a in allLoadFactors
                                  group a by a.MehrcomParsLoadFactor.Category
                                  into b
                                  select b;

                    var totalAmount = 0d;
                    var totalCount = 0;
                    string monthName = "";
                    DateTime endDate = DateTime.Now;

                    for (int i = 0; i < ws2Data.Count(); i++)
                    {
                        var category = ws2Data.ElementAt(i).Key.Title;
                        var loadFactors = ws2Data.ElementAt(i).ToList();
                        totalCount += loadFactors.Count;

                        //var amount = 0d;
                        //foreach (var loadFactor in loadFactors)
                        //{
                        //    if (loadFactor.Tonnage.HasValue)
                        //        loadFactor.Amount = loadFactor.Amount + (loadFactor.Tonnage.Value * loadFactor.TonnagePrice.Value);

                        //    if (loadFactor.MehrcomParsLoadFactor.LoadSleepTime.HasValue)
                        //        loadFactor.Amount += loadFactor.MehrcomParsLoadFactor.LoadSleepPrice.Value;

                        //    if (loadFactor.MehrcomParsLoadFactor.WeighbridgePrice.HasValue)
                        //        loadFactor.Amount += loadFactor.MehrcomParsLoadFactor.WeighbridgePrice.Value;

                        //    amount += loadFactor.Amount;
                        //}
                        var pd = new PersianDateTime(loadFactors.First().CalendarStartDate);
                        endDate = loadFactors.First().CalendarEndDate;
                        monthName = pd.MonthName;

                        totalAmount += loadFactors.Sum(a => a.Amount);

                        ws2.Cell($"A{i + 4}").Value = i + 1;
                        ws2.Cell($"B{i + 4}").Value = pd.Month;
                        ws2.Cell($"C{i + 4}").Value = accountBook.Number;
                        ws2.Cell($"D{i + 4}").Value = loadFactors.Count;
                        ws2.Cell($"E{i + 4}").Value = loadFactors.Sum(a => a.Amount).ToString("N0");
                        ws2.Cell($"F{i + 4}").Value = category;
                    }

                    ws2.Range($"A{ws2Data.Count() + 4}:C{ws2Data.Count() + 4}").Merge().SetValue("مجموع");
                    ws2.Cell($"D{ws2Data.Count() + 4}").SetValue(totalCount);
                    ws2.Range($"E{ws2Data.Count() + 4}:F{ws2Data.Count() + 4}").Merge().SetValue(totalAmount.ToString("N0"));

                    ws2.Cell("D2").Value = $"تاریخ: {new PersianDateTime(endDate).ToString("yyyy/MM/dd")}";
                    ws2.Cell("F2").Value = $"کارکرد: {monthName} ماه";
                    ws2.RowsUsed().Last().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    #endregion
                }
                else
                {
                    var categories = await _mehrcomParsCategoryRepository.Categories().AsNoTracking().OrderBy(a => a.Sequence).ToListAsync();
                    foreach (var category in categories)
                    {
                        var ws = workbook.Worksheets.Add(category.Title);
                        ws.RightToLeft = true;
                        ws.Style.Font.FontName = "B Titr";
                        ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                        ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                        var data = allLoadFactors.Where(a => a.MehrcomParsLoadFactor.CategoryId.Equals(category.Id)).OrderBy(a => a.VehicleName).ThenBy(a => a.Date).ThenBy(a => a.LoadNumber).ToList();

                        ws.Cell("A4").Value = "ردیف";
                        ws.Cell("B4").Value = "شماره زونکن";
                        ws.Cell("C4").Value = "تاریخ";
                        ws.Cell("D4").Value = "شماره بارنامه داخلی";
                        ws.Cell("E4").Value = "نوع خودرو";
                        ws.Cell("F4").Value = "نام راننده";
                        ws.Cell("G4").Value = "شماره خودرو";
                        ws.Cell("H4").Value = "مبدا";
                        ws.Cell("I4").Value = "مقصد";
                        ws.Cell("J4").Value = "دولتی";
                        ws.Cell("K4").Value = "دولتی برگشتی";
                        ws.Cell("L4").Value = "بار";
                        ws.Cell("M4").Value = "پالت";
                        ws.Cell("N4").Value = "برگشتی";
                        int switchCounter = 14;
                        if (exportType.HasValue)
                        {
                            switch (exportType.Value)
                            {
                                case ExcelExportType.WithAllPrices:
                                    ws.Cell(4, 15).Value = "نرخ دریافتی";
                                    ws.Cell(4, 16).Value = "نرخ پرداختی";
                                    switchCounter += 2;
                                    break;
                                case ExcelExportType.OnlyReceivingPrice:
                                    ws.Cell(4, 15).Value = "مبلغ بارنامه";
                                    switchCounter++;
                                    break;
                                case ExcelExportType.OnlyDriverPrice:
                                    ws.Cell(4, 15).Value = "مبلغ بارنامه";
                                    switchCounter++;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            ws.Cell(4, 15).Value = "مبلغ بارنامه";
                            switchCounter++;
                        }
                        ws.Cell(4, switchCounter + 1).Value = "ماه";
                        ws.Cell(4, switchCounter + 2).Value = "سال";
                        ws.Cell(4, switchCounter + 3).Value = "تقویم";

                        int totalCounter = 5;
                        int take = 30;

                        if (category.Title.Contains("کرمانشاه") || category.Title.Contains("خراسان"))
                            take = 15;
                        //joda sazi bar asase noe khodro dar har sheet va jam bastan bar haman asas
                        var carTypes = data.Select(a => a.VehicleName).Distinct().ToList();
                        foreach (var carType in carTypes)
                        {
                            var typeData = data.Where(a => a.VehicleName.Equals(carType)).ToList();
                            decimal c = Convert.ToDecimal(typeData.Count / Convert.ToDecimal(take));

                            for (int index = 1; index <= Convert.ToInt32(Math.Ceiling(c)); index++)
                            {
                                var loadFactors = typeData.Skip((index - 1) * take).Take(take).ToList();
                                var calendarPd = new PersianDateTime(loadFactors.First().CalendarStartDate);

                                for (int i = 0; i < loadFactors.Count; i++)
                                {
                                    #region handling sleep time and weighbridge
                                    if (loadFactors[i].WeighbridgePrice.HasValue)
                                    {
                                        loadFactors[i].DriverFee += loadFactors[i].WeighbridgePrice.Value;

                                        loadFactors[i].Amount += loadFactors[i].WeighbridgePrice.Value;
                                    }

                                    if (loadFactors[i].LoadSleepTime.HasValue)
                                    {
                                        loadFactors[i].DriverFee += loadFactors[i].DriverLoadSleepPrice.Value;

                                        loadFactors[i].Amount += loadFactors[i].LoadSleepPrice.Value;
                                    }

                                    if (loadFactors[i].Tonnage.HasValue)
                                    {
                                        if (loadFactors[i].DriverTonnagePrice.HasValue)
                                            loadFactors[i].DriverFee = loadFactors[i].DriverFee + loadFactors[i].Tonnage.Value * loadFactors[i].DriverTonnagePrice.Value;

                                        if (loadFactors[i].TonnagePrice.HasValue)
                                            loadFactors[i].Amount = loadFactors[i].Amount + loadFactors[i].Tonnage.Value * loadFactors[i].TonnagePrice.Value;
                                    }
                                    #endregion

                                    var carNumber = $"{loadFactors[i].VehicleRightNumber} {loadFactors[i].VehicleNumberWord} {loadFactors[i].VehicleLeftNumber} ایران {loadFactors[i].VehicleIranStateNumber}";
                                    var date = new PersianDateTime(loadFactors[i].Date);
                                    ws.Cell($"A{totalCounter}").Value = i + 1;
                                    ws.Cell($"B{totalCounter}").Value = loadFactors[i].AccountBookNumber;
                                    ws.Cell($"C{totalCounter}").Value = date.ToString("yyyy/MM/dd");
                                    ws.Cell($"D{totalCounter}").SetValue(loadFactors[i].LoadNumber);
                                    ws.Cell($"E{totalCounter}").Value = loadFactors[i].VehicleName;
                                    ws.Cell($"F{totalCounter}").Value = loadFactors[i].DriverName;
                                    ws.Cell($"G{totalCounter}").Value = carNumber;
                                    ws.Cell($"H{totalCounter}").Value = loadFactors[i].OriginName;
                                    ws.Cell($"I{totalCounter}").Value = loadFactors[i].DestinationName;
                                    ws.Cell($"J{totalCounter}").SetValue(loadFactors[i].LoadNumberGov);
                                    ws.Cell($"K{totalCounter}").Value = loadFactors[i].MehrcomParsLoadFactor.LoadNumberGovReturn;
                                    ws.Cell($"L{totalCounter}").Value = loadFactors[i].MehrcomParsLoadFactor.Load ? "1" : "0";
                                    ws.Cell($"M{totalCounter}").Value = loadFactors[i].MehrcomParsLoadFactor.Palette ? "1" : "0";
                                    ws.Cell($"N{totalCounter}").Value = loadFactors[i].MehrcomParsLoadFactor.Return ? "1" : "0";
                                    if (exportType.HasValue)
                                    {
                                        switch (exportType.Value)
                                        {
                                            case ExcelExportType.WithAllPrices:
                                                ws.Cell(totalCounter, 15).Value = loadFactors[i].Amount.ToString("N0");
                                                ws.Cell(totalCounter, 16).Value = loadFactors[i].DriverFee.ToString("N0");
                                                break;
                                            case ExcelExportType.OnlyReceivingPrice:
                                                ws.Cell(totalCounter, 15).Value = loadFactors[i].Amount.ToString("N0");
                                                break;
                                            case ExcelExportType.OnlyDriverPrice:
                                                ws.Cell(totalCounter, 15).Value = loadFactors[i].DriverFee.ToString("N0");
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                        ws.Cell(totalCounter, 15).Value = loadFactors[i].DriverFee.ToString("N0");

                                    ws.Cell(totalCounter, switchCounter + 1).Value = calendarPd.ToString("MMMM");
                                    ws.Cell(totalCounter, switchCounter + 2).Value = calendarPd.ToString("yyyy");
                                    ws.Cell(totalCounter, switchCounter + 3).Value = loadFactors[i].CalendarTitle;


                                    #region handling comment
                                    string commentText = "";
                                    if (loadFactors[i].WeighbridgePrice.HasValue)
                                        commentText += $"مبلغ باسکول: {loadFactors[i].WeighbridgePrice.Value.ToString("N0")}";

                                    if (loadFactors[i].LoadSleepTime.HasValue)
                                    {
                                        if (!string.IsNullOrWhiteSpace(commentText))
                                            commentText += " | ";

                                        commentText += $"زمان خواب: {loadFactors[i].LoadSleepTime.Value} | مبلغ خواب: {(exportType.HasValue ? (exportType.Value == ExcelExportType.WithAllPrices || exportType.Value == ExcelExportType.OnlyReceivingPrice) ? loadFactors[i].LoadSleepPrice.Value.ToString("N0") : loadFactors[i].DriverLoadSleepPrice.Value.ToString("N0") : loadFactors[i].DriverLoadSleepPrice.Value.ToString("N0"))}";
                                    }

                                    if (loadFactors[i].Tonnage.HasValue)
                                    {
                                        if (!string.IsNullOrWhiteSpace(commentText))
                                            commentText += " | ";

                                        commentText += $"مبلغ اضافه تناژ: {(exportType.HasValue ? (exportType.Value == ExcelExportType.WithAllPrices || exportType.Value == ExcelExportType.OnlyReceivingPrice) ? loadFactors[i].TonnagePrice.Value.ToString("N0") : loadFactors[i].DriverTonnagePrice.Value.ToString("N0") : loadFactors[i].DriverTonnagePrice.Value.ToString("N0"))}";
                                    }

                                    if (!string.IsNullOrWhiteSpace(commentText))
                                    {
                                        var cell = ws.Cell($"D{totalCounter}");
                                        cell.Comment.AddText(commentText);
                                        cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                                    }
                                    #endregion

                                    totalCounter++;
                                }
                                if (exportType.HasValue)
                                {
                                    switch (exportType.Value)
                                    {
                                        case ExcelExportType.WithAllPrices:
                                            ws.Cell(totalCounter, 15).Value = loadFactors.Sum(a => a.Amount).ToString("N0");
                                            ws.Cell(totalCounter, 16).Value = loadFactors.Sum(a => a.DriverFee).ToString("N0");
                                            break;
                                        case ExcelExportType.OnlyReceivingPrice:
                                            ws.Cell(totalCounter, 15).Value = loadFactors.Sum(a => a.Amount).ToString("N0");
                                            break;
                                        case ExcelExportType.OnlyDriverPrice:
                                            ws.Cell(totalCounter, 15).Value = loadFactors.Sum(a => a.DriverFee).ToString("N0");
                                            break;
                                        case ExcelExportType.WithoutPrice:
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                    ws.Cell(totalCounter, 15).Value = loadFactors.Sum(a => a.DriverFee).ToString("N0");

                                totalCounter++;
                            }
                        }

                        //making header
                        ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}1").Merge();
                        ws.Range($"A2:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}2").Merge();
                        ws.Range($"A3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}3").Merge();
                        ws.Cell("A1").Value = "اتحاد بار آسیا";
                        ws.Cell("A2").Value = $"فرم ارسال صورت حساب";
                        ws.Cell("A3").Value = $"صورت حساب خدمات حمل {category.Title}";
                        ws.Range($"A4:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}4").Style.Fill.SetBackgroundColor(XLColor.LightGray);
                        ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}4").Style.Font.SetFontSize(12);

                        var table = ws.Range($"A4:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 3)).Letter}{totalCounter - 1}").CreateTable();
                        table.Theme = XLTableTheme.None;
                        table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        table.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                        ws.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.CellsUsed().Style.Font.Bold = true;
                        ws.CellsUsed().Style.Font.FontColor = XLColor.Black;
                        ws.CellsUsed().Style.Border.BottomBorderColor = XLColor.Black;
                        ws.RowsUsed().Height = 25;

                        ws.Column("A").Width = 4.5;
                        ws.Column("B").Width = 6;
                        ws.Column("C").Width = 10;
                        ws.Column("F").Width = 12.5;
                        ws.Column("H").Width = 10;
                        ws.Column("I").Width = 10;
                        ws.Column("G").Width = 13.5;
                        ws.Column("L").Width = 3;
                        ws.Column("M").Width = 3;
                        ws.Column("N").Width = 3;
                    }
                }

                await using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
            }
        }

        private static void MakePressSheet(List<ExcelLoadFactorVM> data, IXLWorksheet ws, ExcelExportType? exportType)
        {
            var EnglishNumbers = new List<(string Letter, int Num)>
            {
                ("A", 1),
                ("B", 2),
                ("C", 3),
                ("D", 4),
                ("E", 5),
                ("F", 6),
                ("G", 7),
                ("H", 8),
                ("I", 9),
                ("J", 10),
                ("K", 11),
                ("L", 12),
                ("M", 13),
                ("N", 14),
                ("O", 15),
                ("P", 16),
                ("Q", 17),
                ("R", 18),
                ("S", 19)
            };

            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = "ردیف";
            ws.Cell(1, 2).Value = "بارنامه";
            ws.Cell(1, 3).Value = "پلاک";
            ws.Cell(1, 4).Value = "راننده";
            ws.Cell(1, 5).Value = "مبدا";
            ws.Cell(1, 6).Value = "مقصد";
            ws.Cell(1, 7).Value = "تاریخ";
            ws.Cell(1, 8).Value = "سند ورود";
            ws.Cell(1, 9).Value = "سند خروج";
            ws.Cell(1, 10).Value = "نوع خودرو";
            ws.Cell(1, 11).Value = "نوع بار";
            ws.Cell(1, 12).Value = "تقویم";

            int switchCounter = 12;
            if (ws.Name.Contains("تناژ"))
            {
                ws.Cell(1, 13).Value = "اضافه تناژ";
                ws.Cell(1, 14).Value = "نرخ اضافه تناژ";
                switchCounter = 14;
            }

            if (exportType.HasValue)
            {
                switch (exportType.Value)
                {
                    case ExcelExportType.WithAllPrices:
                        switchCounter++;
                        ws.Cell(1, switchCounter).Value = "نرخ دریافتی";
                        switchCounter++;
                        ws.Cell(1, switchCounter).Value = "نرخ پرداختی";
                        break;
                    case ExcelExportType.OnlyReceivingPrice:
                        switchCounter++;
                        ws.Cell(1, switchCounter).Value = "نرخ دریافتی";
                        break;
                    case ExcelExportType.OnlyDriverPrice:
                        switchCounter++;
                        ws.Cell(1, switchCounter).Value = "قابل پرداخت";
                        break;
                    case ExcelExportType.WithoutPrice:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switchCounter++;
                ws.Cell(1, switchCounter).Value = "قابل پرداخت";
            }

            ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}1").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                .Font.SetBold(true)
                .Font.SetFontSize(12);

            for (int index = 1; index <= data.Count; index++)
            {
                ws.Cell(index + 1, 1).Value = index;
                ws.Cell(index + 1, 2).SetValue(data[index - 1].LoadNumber);
                ws.Cell(index + 1, 3).Value = $"{data[index - 1].VehicleRightNumber} {data[index - 1].VehicleNumberWord} {data[index - 1].VehicleLeftNumber}";
                ws.Cell(index + 1, 4).Value = data[index - 1].DriverName;
                ws.Cell(index + 1, 5).Value = data[index - 1].OriginName;
                ws.Cell(index + 1, 6).Value = data[index - 1].DestinationName;
                ws.Cell(index + 1, 7).Value = new PersianDateTime(data[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 1, 8).SetValue(data[index - 1].SaipaPressLoadFactor.EntryNumber);
                ws.Cell(index + 1, 9).SetValue(data[index - 1].ExitNumber);
                ws.Cell(index + 1, 10).Value = data[index - 1].VehicleName;
                ws.Cell(index + 1, 11).Value = data[index - 1].SaipaPressLoadFactor.LoadType;
                ws.Cell(index + 1, 12).Value = data[index - 1].CalendarTitle;
                if (ws.Name.Contains("تناژ"))
                    ws.Cell(index + 1, 13).Value = data[index - 1].Tonnage.HasValue ? data[index - 1].Tonnage.Value : "0";
                if (exportType.HasValue)
                {
                    switch (exportType.Value)
                    {
                        case ExcelExportType.WithAllPrices:
                            if (ws.Name.Contains("تناژ"))
                                ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].TonnagePrice.Value * data[index - 1].Tonnage.Value).ToString("N0") : "0";

                            //ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].Amount + (data[index - 1].Tonnage.Value * data[index - 1].TonnagePrice.Value)).ToString("N0") : data[index - 1].Amount.ToString("N0");
                            //ws.Cell(index + 1, 15).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverFee + (data[index - 1].Tonnage.Value * data[index - 1].DriverTonnagePrice.Value)).ToString("N0") : data[index - 1].DriverFee.ToString("N0");
                            ws.Cell(index + 1, switchCounter - 1).Value = data[index - 1].Amount.ToString("N0");
                            ws.Cell(index + 1, switchCounter).Value = data[index - 1].DriverFee.ToString("N0");
                            break;
                        case ExcelExportType.OnlyReceivingPrice:
                            if (ws.Name.Contains("تناژ"))
                                ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].TonnagePrice.Value * data[index - 1].Tonnage.Value).ToString("N0") : "0";
                            //ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].Amount + (data[index - 1].Tonnage.Value * data[index - 1].TonnagePrice.Value)).ToString("N0") : data[index - 1].Amount.ToString("N0");
                            ws.Cell(index + 1, switchCounter).Value = data[index - 1].Amount.ToString("N0");
                            break;
                        case ExcelExportType.OnlyDriverPrice:
                            if (ws.Name.Contains("تناژ"))
                                ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverTonnagePrice.Value * data[index - 1].Tonnage.Value).ToString("N0") : "0";
                            //ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverFee + (data[index - 1].Tonnage.Value * data[index - 1].DriverTonnagePrice.Value)).ToString("N0") : data[index - 1].DriverFee.ToString("N0");
                            ws.Cell(index + 1, switchCounter).Value = data[index - 1].DriverFee.ToString("N0");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (ws.Name.Contains("تناژ"))
                        ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverTonnagePrice.Value * data[index - 1].Tonnage.Value).ToString("N0") : "0";
                    //ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverFee + (data[index - 1].Tonnage.Value * data[index - 1].DriverTonnagePrice.Value)).ToString("N0") : data[index - 1].DriverFee.ToString("N0");
                    ws.Cell(index + 1, switchCounter).Value = data[index - 1].DriverFee.ToString("N0");
                }
            }

            if (exportType.HasValue)
            {
                switch (exportType.Value)
                {
                    case ExcelExportType.WithAllPrices:
                        ws.Cell($"A{data.Count + 2}").Value = "جمع";
                        ws.Range($"A{data.Count + 2}:L{data.Count + 2}").Row(1).Merge();

                        if (ws.Name.Contains("تناژ"))
                        {
                            ws.Cell($"M{data.Count + 2}").SetValue(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value));
                            ws.Cell($"N{data.Count + 2}").SetValue(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.TonnagePrice.Value).ToString("N0"));
                        }

                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{data.Count + 2}").Value = data.Sum(a => a.Amount).ToString("N0");
                        //(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.TonnagePrice.Value)
                        //+ data.Sum(a => a.Amount)).ToString("N0");

                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Value = data.Sum(a => a.DriverFee).ToString("N0");
                        //(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.DriverTonnagePrice.Value)
                        //+ data.Sum(a => a.DriverFee)).ToString("N0");

                        ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                            .Font.SetBold(true);
                        break;
                    case ExcelExportType.OnlyReceivingPrice:
                        ws.Cell($"A{data.Count + 2}").Value = "جمع";
                        ws.Range($"A{data.Count + 2}:L{data.Count + 2}").Row(1).Merge();

                        if (ws.Name.Contains("تناژ"))
                        {
                            ws.Cell($"M{data.Count + 2}").SetValue(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value));
                            ws.Cell($"N{data.Count + 2}").SetValue(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.TonnagePrice.Value).ToString("N0"));
                        }

                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Value = data.Sum(a => a.Amount).ToString("N0");
                        //(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.TonnagePrice.Value)
                        //+ data.Sum(a => a.Amount)).ToString("N0");

                        ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                            .Font.SetBold(true);
                        break;
                    case ExcelExportType.OnlyDriverPrice:
                        ws.Cell($"A{data.Count + 2}").Value = "جمع";
                        ws.Range($"A{data.Count + 2}:L{data.Count + 2}").Row(1).Merge();

                        if (ws.Name.Contains("تناژ"))
                        {
                            ws.Cell($"M{data.Count + 2}").SetValue(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value));
                            ws.Cell($"N{data.Count + 2}").SetValue(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.TonnagePrice.Value).ToString("N0"));
                        }

                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Value = data.Sum(a => a.DriverFee);
                        //(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.DriverTonnagePrice.Value)
                        //+ data.Sum(a => a.DriverFee)).ToString("N0");

                        ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                            .Font.SetBold(true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ws.Cell($"A{data.Count + 2}").Value = "جمع";
                ws.Range($"A{data.Count + 2}:L{data.Count + 2}").Row(1).Merge();

                if (ws.Name.Contains("تناژ"))
                {
                    ws.Cell($"M{data.Count + 2}").SetValue(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value));
                    ws.Cell($"N{data.Count + 2}").SetValue(data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.TonnagePrice.Value).ToString("N0"));
                }

                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Value =
                    (data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.DriverTonnagePrice.Value)
                    + data.Sum(a => a.DriverFee)).ToString("N0");

                ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                    .Font.SetBold(true);
            }

            ws.Columns().AdjustToContents();
            ws.LastColumnUsed().Style.Font.SetBold(true);
            ws.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.CellsUsed().Style.Font.FontColor = XLColor.Black;
            ws.RowsUsed().Height = 20;
            ws.RangeUsed().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Black)
                .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                .Border.SetInsideBorderColor(XLColor.Black);
        }

        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> ActivityList(long customerId, long calendarId, bool hasPayment, bool isFreeDriverFee)
        {
            var data = await _vehicleRepo.ActivityList(customerId, calendarId, hasPayment, isFreeDriverFee);
            var calendar = await _calendarRepo.Get(calendarId);
            var customer = await _customerRepo.Get(customerId);

            using var workbook = new XLWorkbook();
            var docTitle = $"عملکرد {customer.Name} در {calendar.Title}";
            if (hasPayment)
                docTitle = $"قابل پرداخت {customer.Name} در {calendar.Title}";

            var ws = workbook.Worksheets.Add(calendar.Title);
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;
            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "نام و نام خانوادگی";
            ws.Cell(2, 3).Value = "شماره خودرو";
            ws.Cell(2, 4).Value = "مبلغ";
            if (hasPayment)
            {
                ws.Cell(2, 5).Value = "شماره حساب";
                ws.Cell(2, 6).Value = "توضیحات";
            }

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(data.Count + 2, hasPayment ? 6 : 4));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(14)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, hasPayment ? 6 : 4)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 3, 1).Value = i + 1;
                ws.Cell(i + 3, 2).Value = data[i].VehicleOwnerName;
                ws.Cell(i + 3, 3).Value = data[i].VehicleNumber;
                ws.Cell(i + 3, 4).Value = data[i].Amount < 0 ? 0 : data[i].Amount.ToString("N0");
                if (hasPayment)
                {
                    ws.Cell(i + 3, 5).SetValue<string>(string.IsNullOrWhiteSpace(data[i].BankAccountNumber) ? "---" : data[i].BankAccountNumber.Replace('-', '.'));
                    ws.Cell(i + 3, 6).Value = data[i].Amount < 0 ? $"مبغ {(-(data[i].Amount)).ToString("N0")} ریال بدهکار" : "";
                }
            }

            ws.Cell(data.Count + 3, 1).Value = "جمع";
            ws.Range(data.Count + 3, 1, data.Count + 3, 2).Merge();
            ws.Cell(data.Count + 3, 3).Value = data.Sum(a => a.Amount < 0 ? 0 : a.Amount).ToString("N0");
            ws.Range(data.Count + 3, 3, data.Count + 3, hasPayment ? 6 : 4).Merge();

            ws.Column("A").Width = 5;
            ws.Column("B").Width = 18;
            ws.Column("C").Width = 20;
            if (hasPayment)
            {
                ws.Column("D").Width = 11;
                ws.Column("E").Width = 24;
                ws.Column("F").Width = 20;
            }
            else
            {
                ws.Column("D").Width = 20;
            }

            ws.CellsUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var table = ws.Range(2, 1, data.Count + 2, hasPayment ? 6 : 4).CreateTable();
            table.Theme = XLTableTheme.None;
            table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            table.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            ws.PageSetup.SetPageOrientation(XLPageOrientation.Portrait)
                .SetPaperSize(XLPaperSize.A4Paper)
                .Margins.SetTop(0).SetBottom(0).SetRight(0.5).SetLeft(0).SetHeader(0).SetFooter(0);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> FullActivityList(long customerId, long calendarId, int type)
        {
            var finalData = new List<VehicleFullActivityVM>();
            var data = await _vehicleRepo.FullActivityList(customerId, calendarId, type);
            foreach (var item in data)
            {
                var activity = item.ActivityAmount.Value < 0 ? 0 : item.ActivityAmount.Value;
                var amount = item.Amount < 0 ? 0 : item.Amount;

                finalData.Add(new VehicleFullActivityVM
                {
                    Activity = activity,
                    Amount = amount,
                    VehicleNumber = item.VehicleNumber,
                    VehicleId = item.VehicleId,
                    VehicleOwnerName = item.VehicleOwnerName,
                    RightNumber = item.VehicleRightNumber,
                    LeftNumber = item.VehicleLeftNumber
                });
            }

            var bills = await _billRepository.Query().Include(a => a.Vehicle).Include(a => a.Customer)
              .Where(a => a.CalendarId.Equals(calendarId) && a.CustomerId.Value.Equals(customerId) && (a.VehicleId.HasValue &&
          (_vehicleRepo.Vehicles().Where(b => !b.RealStatus).Select(a => a.Id)).Contains(a.VehicleId.Value))).OrderBy(a => a.Vehicle.LeftNumber).ThenBy(a => a.Vehicle.RightNumber).ToListAsync();
            //var slashedLoadFactors = bills.DistinctBy(a => a.VehicleId.Value).ToList();

            foreach (var item in bills)
            {
                finalData.Add(new VehicleFullActivityVM
                {
                    Activity = item.Amount,
                    Amount = item.Amount,
                    VehicleNumber = $"ایران {item.Vehicle.IranStateNumber} - {item.Vehicle.RightNumber} {item.Vehicle.NumberWord} {item.Vehicle.LeftNumber}",
                    VehicleId = item.VehicleId.Value,
                    VehicleOwnerName = item.Vehicle.VehicleOwnerFullname,
                    RightNumber = item.Vehicle.RightNumber,
                    LeftNumber = item.Vehicle.LeftNumber
                });
            }


            var otherCosts = await db.OtherCost.Include(a => a.Vehicle).AsNoTracking().Where(a => a.CalendarId.Equals(calendarId) && a.CustomerId.Equals(customerId)).OrderBy(a => a.Vehicle.LeftNumber).ThenBy(a => a.Vehicle.RightNumber).ToListAsync();

            foreach (var item in otherCosts)
            {
                finalData.Add(new VehicleFullActivityVM
                {
                    Activity = item.Amount,
                    Amount = item.Amount,
                    VehicleNumber = $"ایران {item.Vehicle.IranStateNumber} - {item.Vehicle.RightNumber} {item.Vehicle.NumberWord} {item.Vehicle.LeftNumber}",
                    VehicleId = item.VehicleId,
                    VehicleOwnerName = item.Vehicle.VehicleOwnerFullname,
                    RightNumber = item.Vehicle.RightNumber,
                    LeftNumber = item.Vehicle.LeftNumber
                });
            }

            var loadFactorNovins = await db.LoadFactorNovin.Include(a => a.Vehicle).AsNoTracking()
                .Where(a => a.CalendarId.Equals(calendarId) && a.CustomerId.Equals(customerId))
                .OrderBy(a => a.Vehicle.LeftNumber).ThenBy(a => a.Vehicle.RightNumber).ToListAsync();

            foreach (var item in loadFactorNovins)
            {
                finalData.Add(new VehicleFullActivityVM
                {
                    Activity = item.DriverFee,
                    Amount = item.DriverFee,
                    VehicleNumber = $"ایران {item.Vehicle.IranStateNumber} - {item.Vehicle.RightNumber} {item.Vehicle.NumberWord} {item.Vehicle.LeftNumber}",
                    VehicleId = item.VehicleId,
                    VehicleOwnerName = item.Vehicle.VehicleOwnerFullname,
                    RightNumber = item.Vehicle.RightNumber,
                    LeftNumber = item.Vehicle.LeftNumber
                });
            }

            var distinctedData = finalData.OrderBy(a => a.LeftNumber).ThenBy(a => a.RightNumber).DistinctBy(a => a.VehicleId).ToList();

            var calendar = await _calendarRepo.Get(calendarId);
            var customer = await _customerRepo.Get(customerId);

            using var workbook = new XLWorkbook();
            var docTitle = $"لیست عملکرد کلی {customer.Name} در {calendar.Title}";

            var ws = workbook.Worksheets.Add(calendar.Title);
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;
            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "نام و نام خانوادگی";
            ws.Cell(2, 3).Value = "شماره خودرو";
            ws.Cell(2, 4).Value = "عملکرد";
            ws.Cell(2, 5).Value = "کسر / خسارت";
            ws.Cell(2, 6).Value = "پرداخت شده";
            ws.Cell(2, 7).Value = "توضیحات";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(distinctedData.Count + 2, 7));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(14)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 7)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int i = 0; i < distinctedData.Count; i++)
            {
                var amount = finalData.Where(a => a.VehicleId.Equals(distinctedData[i].VehicleId)).Sum(a => a.Amount);
                var activity = finalData.Where(a => a.VehicleId.Equals(distinctedData[i].VehicleId)).Sum(a => a.Activity.Value);

                ws.Cell(i + 3, 1).Value = i + 1;
                ws.Cell(i + 3, 2).Value = distinctedData[i].VehicleOwnerName.Replace("/", "");
                ws.Cell(i + 3, 3).Value = distinctedData[i].VehicleNumber;
                ws.Cell(i + 3, 4).Value = activity.ToString("N0");
                ws.Cell(i + 3, 5).Value = (amount - activity).ToString("N0");
                ws.Cell(i + 3, 6).Value = amount.ToString("N0");
                ws.Cell(i + 3, 7).Value = "";
            }

            // for (int i = 0; i < data.Count; i++)
            // {
            //     var activity = data[i].ActivityAmount.Value < 0 ? 0 : data[i].ActivityAmount.Value;
            //     var amount = data[i].Amount < 0 ? 0 : data[i].Amount;

            //     ws.Cell(i + 3, 1).Value = i + 1;
            //     ws.Cell(i + 3, 2).Value = data[i].VehicleOwnerName;
            //     ws.Cell(i + 3, 3).Value = data[i].VehicleNumber;
            //     ws.Cell(i + 3, 4).Value = activity.ToString("N0");
            //     ws.Cell(i + 3, 5).Value = (amount - activity).ToString("N0");
            //     ws.Cell(i + 3, 6).Value = amount.ToString("N0");
            //     ws.Cell(i + 3, 7).Value = "";
            // }

            // var bills = await _billRepository.Query().Include(a => a.Vehicle).Include(a => a.Customer)
            //    .Where(a => a.CalendarId.Equals(calendarId) && a.CustomerId.Value.Equals(customerId) && (a.VehicleId.HasValue && 
            //(_vehicleRepo.Vehicles().Where(b => !b.RealStatus).Select(a => a.Id)).Contains(a.VehicleId.Value))).OrderBy(a => a.Vehicle.LeftNumber).ThenBy(a => a.Vehicle.RightNumber).ToListAsync();
            // var slashedLoadFactors = bills.DistinctBy(a => a.VehicleId.Value).ToList();
            // for (int i = 0; i < slashedLoadFactors.Count; i++)
            // {
            //     var amount = bills.Where(a => a.VehicleId.Value.Equals(slashedLoadFactors[i].VehicleId.Value)).Sum(a => a.Amount).ToString("N0");
            //     var vehicleNumber = $"ایران {slashedLoadFactors[i].Vehicle.IranStateNumber} - {slashedLoadFactors[i].Vehicle.RightNumber} {slashedLoadFactors[i].Vehicle.NumberWord} {slashedLoadFactors[i].Vehicle.LeftNumber}";
            //     ws.Cell(i + data.Count + 3, 1).Value = i + data.Count + 1;
            //     ws.Cell(i + data.Count + 3, 2).Value = slashedLoadFactors[i].ReceiverName;
            //     ws.Cell(i + data.Count + 3, 3).Value = vehicleNumber;
            //     ws.Cell(i + data.Count + 3, 4).Value = amount;
            //     ws.Cell(i + data.Count + 3, 5).Value = "0";
            //     ws.Cell(i + data.Count + 3, 6).Value = amount;
            //     ws.Cell(i + data.Count + 3, 7).Value = "";
            // }

            // var otherCosts = await db.OtherCost.Include(a => a.Vehicle).AsNoTracking().Where(a => a.CalendarId.Equals(calendarId) && a.CustomerId.Equals(customerId)).OrderBy(a => a.Vehicle.LeftNumber).ThenBy(a => a.Vehicle.RightNumber).ToListAsync();
            // if (otherCosts.Any())
            // {
            //     for (int i = 0; i < otherCosts.Count; i++)
            //     {
            //         var vehicleNumber = $"ایران {otherCosts[i].Vehicle.IranStateNumber} - {otherCosts[i].Vehicle.RightNumber} {otherCosts[i].Vehicle.NumberWord} {otherCosts[i].Vehicle.LeftNumber}";
            //         ws.Cell(i + slashedLoadFactors.Count + data.Count + 3, 1).Value = i + slashedLoadFactors.Count + data.Count + 1;
            //         ws.Cell(i + slashedLoadFactors.Count + data.Count + 3, 2).Value = otherCosts[i].DriverName;
            //         ws.Cell(i + slashedLoadFactors.Count + data.Count + 3, 3).Value = vehicleNumber;
            //         ws.Cell(i + slashedLoadFactors.Count + data.Count + 3, 4).Value = otherCosts[i].Amount.ToString("N0");
            //         ws.Cell(i + slashedLoadFactors.Count + data.Count + 3, 5).Value = "0";
            //         ws.Cell(i + slashedLoadFactors.Count + data.Count + 3, 6).Value = otherCosts[i].Amount.ToString("N0");
            //         ws.Cell(i + slashedLoadFactors.Count + data.Count + 3, 7).Value = "";
            //     }
            // }

            ws.Cell(/*otherCosts.Count + slashedLoadFactors.Count + */distinctedData.Count + 3, 1).Value = "جمع";
            ws.Range(/*otherCosts.Count + slashedLoadFactors.Count + */distinctedData.Count + 3, 1, /*otherCosts.Count + slashedLoadFactors.Count + */distinctedData.Count + 3, 3).Merge();
            ws.Cell(/*otherCosts.Count + slashedLoadFactors.Count + */distinctedData.Count + 3, 4).Value = (finalData.Sum(a => a.Activity.Value) /*+ bills.Sum(a => a.Amount) + otherCosts.Sum(a => a.Amount)*/).ToString("N0");
            ws.Range(distinctedData.Count + 3, 4, distinctedData.Count + 3, 5).Merge();
            ws.Cell(/*otherCosts.Count + slashedLoadFactors.Count + */distinctedData.Count + 3, 6).Value = (finalData.Sum(a => a.Amount < 0 ? 0 : a.Amount) /*+ bills.Sum(a => a.Amount) + otherCosts.Sum(a => a.Amount)*/).ToString("N0");
            ws.Range(distinctedData.Count + 3, 6, distinctedData.Count + 3, 7).Merge();

            ws.Column("A").Width = 5;
            ws.Column("B").Width = 18;
            ws.Column("C").Width = 16;
            ws.Column("D").Width = 11;
            ws.Column("E").Width = 11;
            ws.Column("F").Width = 11;
            ws.Column("G").Width = 18;

            ws.CellsUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var table = ws.Range(2, 1, /*otherCosts.Count + slashedLoadFactors.Count +*/ distinctedData.Count + 2, 7).CreateTable();
            table.Theme = XLTableTheme.None;
            table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            table.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            table.Style.Font.FontSize = 9;

            ws.PageSetup.SetPageOrientation(XLPageOrientation.Portrait)
                .SetPaperSize(XLPaperSize.A4Paper)
                .Margins.SetTop(0).SetBottom(0).SetRight(0.5).SetLeft(0).SetHeader(0).SetFooter(0);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> VehicleActivityByCustomer(long customerId, long calendarId, bool isFreeDriverFee)
        {
            var data = await _vehicleRepo.ActivityListByCustomer(customerId, calendarId, isFreeDriverFee);
            if (!data.Any())
                return NotFound("بارنامه ای وجود ندارد.");

            var calendar = await _calendarRepo.Get(calendarId);
            var customer = await _customerRepo.Get(customerId);

            using var workbook = new XLWorkbook();
            var docTitle = $"عملکرد {customer.Name} در {calendar.Title}{(isFreeDriverFee ? " | موردی" : "")}";

            foreach (var item in data)
            {
                var ws = workbook.Worksheets.Add(item.VehicleNumber);
                ws.RightToLeft = true;
                ws.Style.Font.FontName = "B Titr";
                ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                ws.Cell(1, 1).Value = $"عملکرد {item.VehicleNumber} ({item.VehicleType}) در {calendar.Title} شرکت {customer.Name}";
                ws.Cell(2, 1).Value = "#";
                ws.Cell(2, 2).Value = "تاریخ";
                ws.Cell(2, 3).Value = "راننده";
                ws.Cell(2, 4).Value = "مبدا";
                ws.Cell(2, 5).Value = "مقصد";
                ws.Cell(2, 6).Value = "بارنامه";
                ws.Cell(2, 7).Value = "موردی";
                ws.Cell(2, 8).Value = "مبلغ";

                int lastCellCount = 8;

                if (customer.CustomerType == CustomerType.SaipaPress)
                {
                    ws.Cell(2, 9).Value = "نوع";
                    ws.Cell(2, 10).Value = "تناژ اضافه";
                    ws.Cell(2, 11).Value = "مبلغ تناژ";
                    lastCellCount = 11;
                }

                if (customer.CustomerType == CustomerType.SazehGostar)
                {
                    ws.Cell(2, 9).Value = "درخواست";
                    lastCellCount = 9;
                }

                if (customer.CustomerType == CustomerType.MehrcomPars)
                {
                    ws.Cell(2, 9).Value = "نوع";
                    ws.Cell(2, 10).Value = "تناژ اضافه";
                    ws.Cell(2, 11).Value = "مبلغ تناژ";
                    ws.Cell(2, 12).Value = "باسکول";
                    ws.Cell(2, 13).Value = "خواب";
                    ws.Cell(2, 14).Value = "بارنامه دولتی";
                    lastCellCount = 14;
                }

                var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(data.Count + 2, lastCellCount));
                rngTable.FirstRow().Merge();

                rngTable.FirstRow().Style
                    .Font.SetBold()
                    .Font.SetFontSize(12)
                        .Fill.SetBackgroundColor(XLColor.LightGray)
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, lastCellCount)); // The address is relative to rngTable (NOT the worksheet)
                rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngHeaders.Style.Font.Bold = true;
                rngHeaders.Style.Font.FontColor = XLColor.Black;

                item.Details = item.Details.OrderBy(a => a.Date).ToList();
                for (int i = 0; i < item.Details.Count; i++)
                {
                    var detail = item.Details[i];
                    ws.Cell(i + 3, 1).SetValue(i + 1);
                    ws.Cell(i + 3, 2).SetValue(new PersianDateTime(detail.Date).ToString("yyyy/MM/dd"));
                    ws.Cell(i + 3, 3).SetValue(detail.DriverName);
                    ws.Cell(i + 3, 4).SetValue(detail.Origin);
                    ws.Cell(i + 3, 5).SetValue(detail.Destination);
                    ws.Cell(i + 3, 6).SetValue(detail.LoadFactorNumber);
                    ws.Cell(i + 3, 7).SetValue(detail.IsFreeDriverPrice ? "بلی" : "خیر");
                    ws.Cell(i + 3, 8).SetValue(detail.Amount.ToString("N0"));

                    if (customer.CustomerType == CustomerType.SazehGostar)
                        ws.Cell(i + 3, 9).SetValue(detail.SazehRequestNumber);

                    if (customer.CustomerType == CustomerType.SaipaPress)
                    {
                        ws.Cell(i + 3, 9).SetValue(detail.PressFloorType == SaipaPressLoadType.OneFloor ? "یک طبقه" : "دو طبقه");
                        ws.Cell(i + 3, 10).SetValue(detail.Tonnage ?? 0);
                        ws.Cell(i + 3, 11).SetValue((detail.Tonnage.HasValue && detail.TonnagePrice.HasValue) ? (detail.Tonnage.Value * detail.TonnagePrice.Value).ToString("N0") : "0");
                    }

                    if (customer.CustomerType == CustomerType.MehrcomPars)
                    {
                        string mehrcomType = "";
                        if (detail.MehrcomLoad)
                            mehrcomType += "بار";
                        if (detail.MehrcomPalette)
                            mehrcomType += " پالت";
                        if (detail.MehrcomReturn)
                            mehrcomType += " برگشت";

                        mehrcomType = mehrcomType.Trim().Replace(" ", "/");

                        ws.Cell(i + 3, 9).SetValue(mehrcomType);
                        ws.Cell(i + 3, 10).SetValue(detail.Tonnage ?? 0);
                        ws.Cell(i + 3, 11).SetValue((detail.Tonnage.HasValue && detail.TonnagePrice.HasValue) ? (detail.Tonnage.Value * detail.TonnagePrice.Value).ToString("N0") : "0");
                        ws.Cell(i + 3, 12).SetValue(detail.WeighbridgePrice ?? 0);
                        ws.Cell(i + 3, 13).SetValue(detail.DriverLoadSleepPrice ?? 0);
                        ws.Cell(i + 3, 14).SetValue(string.IsNullOrWhiteSpace(detail.LoadFactorGovNumber) ? "---" : detail.LoadFactorGovNumber);
                    }
                }

                ws.Cell(item.Details.Count + 3, 1).Value = "جمع";
                ws.Range(item.Details.Count + 3, 1, item.Details.Count + 3, 7).Merge();
                ws.Cell(item.Details.Count + 3, 8).Value = item.Details.Sum(a => a.Amount < 0 ? 0 : a.Amount).ToString("N0");

                if (item.Details.Any(a => a.Tonnage.HasValue))
                    ws.Cell(item.Details.Count + 3, 10).SetValue(item.Details.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value));

                if (item.Details.Any(a => a.Tonnage.HasValue && a.TonnagePrice.HasValue))
                    ws.Cell(item.Details.Count + 3, 11).SetValue(item.Details.Where(a => a.Tonnage.HasValue && a.TonnagePrice.HasValue).Sum(a => (a.Tonnage.Value * a.TonnagePrice.Value)).ToString("N0"));

                if (item.Details.Any(a => a.WeighbridgePrice.HasValue))
                    ws.Cell(item.Details.Count + 3, 12).SetValue(item.Details.Where(a => a.WeighbridgePrice.HasValue).Sum(a => a.WeighbridgePrice.Value).ToString("N0"));

                if (item.Details.Any(a => a.DriverLoadSleepPrice.HasValue))
                    ws.Cell(item.Details.Count + 3, 13).SetValue(item.Details.Where(a => a.DriverLoadSleepPrice.HasValue).Sum(a => a.DriverLoadSleepPrice.Value).ToString("N0"));

                ws.Cell(item.Details.Count + 4, 1).Value = "وضعیت";
                ws.Range(item.Details.Count + 4, 1, item.Details.Count + 4, 5).Merge();
                ws.Cell(item.Details.Count + 4, 6).Value = item.VehicleBalance > 0 ? item.VehicleBalance.ToString("N0") + " طلبکار" : item.VehicleBalance == 0 ? "0" : (-item.VehicleBalance).ToString("N0") + " بدهکار";
                ws.Range(item.Details.Count + 4, 6, item.Details.Count + 4, 8).Merge();

                ws.Column("A").Width = 5;
                ws.Column("B").Width = 8;
                ws.Column("C").Width = 13;
                ws.Column("D").Width = 12;
                ws.Column("E").Width = 12;
                ws.Column("G").Width = 5;
                ws.Column("H").Width = 12;
                //if (customer.CustomerType == CustomerType.MehrcomPars)
                //{
                //    ws.Column("J").Width = 6;
                //    ws.Column("K").Width = 6;
                //    ws.Column("L").Width = 6;
                //    ws.Column("M").Width = 6;
                //}

                ws.CellsUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                var table = ws.Range(2, 1, item.Details.Count + 2, lastCellCount).CreateTable();
                table.Theme = XLTableTheme.None;
                table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                    .Font.SetFontSize(8);

                ws.Range(item.Details.Count + 6, 1, item.Details.Count + 6, 6).Merge().SetValue("کرایه");
                ws.Range(item.Details.Count + 6, 7, item.Details.Count + 6, 8).Merge().SetValue("تعداد");
                for (int i = 0; i < item.Routes.Count; i++)
                {
                    ws.Cell(item.Details.Count + 7 + i, 1).Value = item.Routes[i].Amount.ToString("N0");
                    ws.Range(item.Details.Count + 7 + i, 1, item.Details.Count + 7 + i, 6).Merge();
                    ws.Cell(item.Details.Count + 7 + i, 7).SetValue(item.Routes[i].Quantity);
                    ws.Range(item.Details.Count + 7 + i, 7, item.Details.Count + 7 + i, 8).Merge();
                }
                ws.Range(item.Details.Count + 6, 1, item.Details.Count + item.Routes.Count + 6, 8)
                    .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin);

                int lastRowFirstCellIndex = item.Details.Count + item.Routes.Count + 7;

                var vehicleBankAccount = item.BankAccounts.FirstOrDefault(a => a.BankId.Equals(customer.ActiveBank));

                string txt = "";
                if (vehicleBankAccount is null)
                    txt = $"اینجانب ......................... مالک خودرو {item.VehicleType} به شماره انتظامی {item.VehicleNumber}، مبلغ {(item.VehicleBalance > 0 ? item.VehicleBalance.ToString("N0") : "0")} ریال، کل کارکرد بابت کرایه حمل قطعات به صورت رفت و برگشت پالت خالی در {calendar.Title} در شرکت اتحاد بار آسیا و شرکت های همکار این شرکت که صدور بارنامه های دولتی از سوی آن ها می باشد را به تعداد {item.Details.Count} فقره بارنامه، به شماره حساب ......................... تمام و کمال دریافت نموده و تسویه گردیده است.";
                else
                    txt = $"اینجانب {vehicleBankAccount.Fullname} مالک خودر {item.VehicleType} به شماره انتظامی {item.VehicleNumber}، مبلغ {(item.VehicleBalance > 0 ? item.VehicleBalance.ToString("N0") : "0")} ریال، کل کارکرد بابت کرایه حمل قطعات به صورت رفت و برگشت پالت خالی در {calendar.Title} در شرکت اتحاد بار آسیا و شرکت های همکار این شرکت که صدور بارنامه های دولتی از سوی آن ها می باشد را به تعداد {item.Details.Count} فقره بارنامه، به شماره حساب {vehicleBankAccount.AccountNumber} تمام و کمال دریافت نموده و تسویه گردیده است.";

                ws.Range(lastRowFirstCellIndex + 1, 1, lastRowFirstCellIndex + 1, 8).Merge().SetValue(txt);
                ws.Cell(lastRowFirstCellIndex + 1, 1).Style.Font.SetFontSize(9).Alignment.SetWrapText(true);
                ws.Row(lastRowFirstCellIndex + 1).Height = 75;
            }

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> FreeLoadFactor(long calendarId, string startDate, string endDate)
        {
            string docTitle = "";

            var amountSum = 0D;
            var driverFeeSum = 0D;
            var receivedSum = 0D;
            var paiedSum = 0D;
            var loadFactors = new List<FreeLoadFactor>();

            if (calendarId > 0)
            {
                var calendar = await _calendarRepo.Get(calendarId);

                docTitle = $"گزارش بارنامه های آزاد در {calendar.Title}";
                loadFactors = await _freeLoadFactorRepository.Query().Where(a => a.CalendarId.Equals(calendarId)).OrderBy(a => a.Date).ToListAsync();
            }
            else
            {
                var startArr = startDate.Split('/');
                var startD = new PersianDateTime(Convert.ToInt32(startArr[0]), Convert.ToInt32(startArr[1]), Convert.ToInt32(startArr[2]), 0, 0, 0).ToDateTime();
                var endArr = endDate.Split('/');
                var endD = new PersianDateTime(Convert.ToInt32(startArr[0]), Convert.ToInt32(startArr[1]), Convert.ToInt32(startArr[2]), 23, 59, 59).ToDateTime();
                if (startD > endD)
                {
                    var calendar = await _calendarRepo.Calendars().AsNoTracking().OrderByDescending(a => a.StartDate).FirstAsync();
                    docTitle = $"گزارش بارنامه های آزاد در {calendar.Title}";
                    loadFactors = await _freeLoadFactorRepository.Query().Where(a => a.CalendarId.Equals(calendar.Id)).OrderBy(a => a.Date).ToListAsync();
                }
                else
                {
                    docTitle = $"گزارش بارنامه های آزاد از {startDate} الی {endDate}";
                    loadFactors = await _freeLoadFactorRepository.Query().Where(a => a.Date >= startD && a.Date <= endD).OrderBy(a => a.Date).ToListAsync();
                }
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش");
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "شماره بارنامه";
            ws.Cell(2, 4).Value = "شماره بارنامه دولتی";
            ws.Cell(2, 5).Value = "مبلغ";
            ws.Cell(2, 6).Value = "کرایه راننده";
            ws.Cell(2, 7).Value = "نام متقاضی";
            ws.Cell(2, 8).Value = "مبدا";
            ws.Cell(2, 9).Value = "مقصد";
            ws.Cell(2, 10).Value = "نام راننده";
            ws.Cell(2, 11).Value = "کد ملی";
            ws.Cell(2, 12).Value = "نوع خودرو";
            ws.Cell(2, 13).Value = "پلاک";
            ws.Cell(2, 14).Value = "تناژ اضافه";
            ws.Cell(2, 15).Value = "نرخ تناژ اضافه (ریال)";
            ws.Cell(2, 16).Value = "نرخ تناژ اضافه راننده (ریال)";
            ws.Cell(2, 17).Value = "جمع کل قابل پرداخت";
            ws.Cell(2, 18).Value = "جمع کل قابل دریافت";
            ws.Cell(2, 19).Value = "وضعیت پرداخت";
            ws.Cell(2, 20).Value = "وضعیت دریافت";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(loadFactors.Count + 2, 19));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 19)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= loadFactors.Count; index++)
            {
                var item = loadFactors[index - 1];
                var payment = item.DriverFee;
                if (item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue)
                    payment = item.DriverFee + (item.Tonnage.Value * item.DriverTonnagePrice.Value);

                var received = item.Amount;
                if (item.Tonnage.HasValue && item.TonnagePrice.HasValue)
                    received = item.Amount + (item.Tonnage.Value * item.TonnagePrice.Value);

                amountSum += received;
                driverFeeSum += payment;

                if (item.IsReceived)
                    receivedSum += received;

                if (item.IsPaied)
                    paiedSum += payment;

                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(item.Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).Value = item.LoadNumber;

                if (string.IsNullOrWhiteSpace(item.LoadNumberGov))
                    ws.Cell(index + 2, 4).Value = "---";
                else
                    ws.Cell(index + 2, 4).Value = item.LoadNumberGov;

                ws.Cell(index + 2, 5).Value = item.Amount.ToString("N0");
                ws.Cell(index + 2, 6).Value = item.DriverFee.ToString("N0");
                ws.Cell(index + 2, 7).Value = item.ApplicantName;
                ws.Cell(index + 2, 8).Value = item.Origin;
                ws.Cell(index + 2, 9).Value = item.Destination;
                ws.Cell(index + 2, 10).Value = item.DriverName;
                ws.Cell(index + 2, 11).Value = item.DriverNationalNumber;
                ws.Cell(index + 2, 12).Value = item.VehicleType;
                ws.Cell(index + 2, 13).Value = item.RightNumber + " " + item.NumberWord + " " + item.LeftNumber + " - " + item.IranStateNumber;
                ws.Cell(index + 2, 14).Value = item.Tonnage.HasValue ? item.Tonnage.Value : 0;
                ws.Cell(index + 2, 15).Value = item.TonnagePrice.HasValue ? item.TonnagePrice.Value.ToString("N0") : 0;
                ws.Cell(index + 2, 16).Value = item.DriverTonnagePrice.HasValue ? item.DriverTonnagePrice.Value.ToString("N0") : 0;
                ws.Cell(index + 2, 17).Value = payment.ToString("N0");
                ws.Cell(index + 2, 18).Value = received.ToString("N0");
                ws.Cell(index + 2, 19).Value = item.IsPaied ? "بلی" : "خیر";
                ws.Cell(index + 2, 20).Value = item.IsReceived ? "بلی" : "خیر";
            }

            ws.Cell($"B{loadFactors.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{loadFactors.Count + 3}:R{loadFactors.Count + 3}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 3, 20).Value = loadFactors.Count;

            ws.Cell($"B{loadFactors.Count + 4}").Value = "قابل دریافت";
            ws.Range($"B{loadFactors.Count + 4}:R{loadFactors.Count + 4}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 4, 20).Value = amountSum.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 5}").Value = "دریافت شده";
            ws.Range($"B{loadFactors.Count + 5}:R{loadFactors.Count + 5}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 5, 20).Value = receivedSum.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 6}").Value = "قابل پرداخت";
            ws.Range($"B{loadFactors.Count + 6}:R{loadFactors.Count + 6}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 6, 20).Value = driverFeeSum.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 7}").Value = "قابل پرداخت";
            ws.Range($"B{loadFactors.Count + 7}:R{loadFactors.Count + 7}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 7, 20).Value = paiedSum.ToString("N0");

            ws.RangeUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:S{loadFactors.Count + 20}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.Columns().AdjustToContents();

            ws.PageSetup.SetPageOrientation(XLPageOrientation.Landscape)
                .SetPaperSize(XLPaperSize.A4Paper);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> NovinLoadFactor(long calendarId, string startDate, string endDate)
        {
            string docTitle = "";

            var amountSum = 0D;
            var driverFeeSum = 0D;
            var receivedSum = 0D;
            var paiedSum = 0D;
            var loadFactors = new List<LoadFactorNovin>();

            if (calendarId > 0)
            {
                var calendar = await _calendarRepo.Get(calendarId);

                docTitle = $"گزارش بارنامه های نوین در {calendar.Title}";
                loadFactors = await _loadFactorNovinRepository.Query().Include(a => a.Vehicle).Where(a => a.CalendarId.Equals(calendarId)).OrderBy(a => a.Date).ToListAsync();
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
                    docTitle = $"گزارش بارنامه های نوین در {calendar.Title}";
                    loadFactors = await _loadFactorNovinRepository.Query().Include(a => a.Vehicle).Where(a => a.CalendarId.Equals(calendar.Id)).OrderBy(a => a.Date).ToListAsync();
                }
                else
                {
                    docTitle = $"گزارش بارنامه های نوین از {startDate} الی {endDate}";
                    loadFactors = await _loadFactorNovinRepository.Query().Include(a => a.Vehicle).Where(a => a.Date >= startD && a.Date <= endD).OrderBy(a => a.Date).ToListAsync();
                }
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش");
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "شماره بارنامه";
            ws.Cell(2, 4).Value = "شماره بارنامه دولتی";
            ws.Cell(2, 5).Value = "مبلغ";
            ws.Cell(2, 6).Value = "کرایه راننده";
            ws.Cell(2, 7).Value = "نام متقاضی";
            ws.Cell(2, 8).Value = "مبدا";
            ws.Cell(2, 9).Value = "مقصد";
            ws.Cell(2, 10).Value = "نام راننده";
            ws.Cell(2, 11).Value = "کد ملی";
            ws.Cell(2, 12).Value = "نوع خودرو";
            ws.Cell(2, 13).Value = "پلاک";
            ws.Cell(2, 14).Value = "تناژ اضافه";
            ws.Cell(2, 15).Value = "نرخ تناژ اضافه (ریال)";
            ws.Cell(2, 16).Value = "نرخ تناژ اضافه راننده (ریال)";
            ws.Cell(2, 17).Value = "جمع کل قابل پرداخت";
            ws.Cell(2, 18).Value = "جمع کل قابل دریافت";
            ws.Cell(2, 19).Value = "وضعیت پرداخت";
            ws.Cell(2, 20).Value = "وضعیت دریافت";
            ws.Cell(2, 21).Value = "کد";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(loadFactors.Count + 2, 21));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 21)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= loadFactors.Count; index++)
            {
                var item = loadFactors[index - 1];
                var payment = item.DriverFee;
                if (item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue)
                    payment = item.DriverFee + (item.Tonnage.Value * item.DriverTonnagePrice.Value);

                var received = item.Amount;
                if (item.Tonnage.HasValue && item.TonnagePrice.HasValue)
                    received = item.Amount + (item.Tonnage.Value * item.TonnagePrice.Value);

                amountSum += received;
                driverFeeSum += payment;

                if (item.IsReceived)
                    receivedSum += received;

                if (item.IsPaied)
                    paiedSum += payment;

                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(item.Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).SetValue(item.LoadNumber);

                if (string.IsNullOrWhiteSpace(item.LoadNumberGov))
                    ws.Cell(index + 2, 4).Value = "---";
                else
                    ws.Cell(index + 2, 4).Value = item.LoadNumberGov;

                ws.Cell(index + 2, 5).Value = item.Amount.ToString("N0");
                ws.Cell(index + 2, 6).Value = item.DriverFee.ToString("N0");
                ws.Cell(index + 2, 7).Value = item.ApplicantName;
                ws.Cell(index + 2, 8).Value = item.Origin;
                ws.Cell(index + 2, 9).Value = item.Destination;
                ws.Cell(index + 2, 10).Value = item.Vehicle.VehicleOwnerFullname;
                ws.Cell(index + 2, 11).SetValue(item.Vehicle.NationalNumber);
                ws.Cell(index + 2, 12).Value = item.Vehicle.Type;
                ws.Cell(index + 2, 13).Value = item.Vehicle.RightNumber + " " + item.Vehicle.NumberWord + " " + item.Vehicle.LeftNumber + " - " + item.Vehicle.IranStateNumber;
                ws.Cell(index + 2, 14).Value = item.Tonnage.HasValue ? item.Tonnage.Value : 0;
                ws.Cell(index + 2, 15).Value = item.TonnagePrice.HasValue ? item.TonnagePrice.Value.ToString("N0") : 0;
                ws.Cell(index + 2, 16).Value = item.DriverTonnagePrice.HasValue ? item.DriverTonnagePrice.Value.ToString("N0") : 0;
                ws.Cell(index + 2, 17).Value = payment.ToString("N0");
                ws.Cell(index + 2, 18).Value = received.ToString("N0");
                ws.Cell(index + 2, 19).Value = item.IsPaied ? "بلی" : "خیر";
                ws.Cell(index + 2, 20).Value = item.IsReceived ? "بلی" : "خیر";
                ws.Cell(index + 2, 21).Value = item.Code;
            }

            ws.Cell($"B{loadFactors.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{loadFactors.Count + 3}:R{loadFactors.Count + 3}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 3, 21).Value = loadFactors.Count;

            ws.Cell($"B{loadFactors.Count + 4}").Value = "قابل دریافت";
            ws.Range($"B{loadFactors.Count + 4}:R{loadFactors.Count + 4}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 4, 21).Value = amountSum.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 5}").Value = "دریافت شده";
            ws.Range($"B{loadFactors.Count + 5}:R{loadFactors.Count + 5}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 5, 21).Value = receivedSum.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 6}").Value = "قابل پرداخت";
            ws.Range($"B{loadFactors.Count + 6}:R{loadFactors.Count + 6}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 6, 21).Value = driverFeeSum.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 7}").Value = "قابل پرداخت";
            ws.Range($"B{loadFactors.Count + 7}:R{loadFactors.Count + 7}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 7, 21).Value = paiedSum.ToString("N0");

            ws.RangeUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:S{loadFactors.Count + 21}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Columns().AdjustToContents();

            ws.PageSetup.SetPageOrientation(XLPageOrientation.Landscape)
                .SetPaperSize(XLPaperSize.A4Paper);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> SlashedLoadFactor(long calendarId)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            string docTitle = $"گزارش {calendar.Title}";
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(docTitle);
            var bills = await _billRepository.Query().Include(a => a.Vehicle).Include(a => a.Customer)
                .Where(a => a.CalendarId.Equals(calendarId) && (a.VehicleId.HasValue &&
            (_vehicleRepo.Vehicles().Where(b => !b.RealStatus).Select(a => a.Id)).Contains(a.VehicleId.Value))).OrderBy(a => a.Date).ToListAsync();

            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;
            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "نام و نام خانوادگی";
            ws.Cell(2, 3).Value = "شماره خودرو";
            ws.Cell(2, 4).Value = "عملکرد";
            ws.Cell(2, 5).Value = "مشتری";

            //var data = bills.DistinctBy(a => a.VehicleId.Value).ToList();

            var counter = 0;
            var customers = await _customerRepo.Customers().AsNoTracking().Select(a => a.Id).ToListAsync();
            foreach (var customer in customers)
            {
                var data = bills.Where(a => a.CustomerId.HasValue && a.CustomerId.Value.Equals(customer)).DistinctBy(a => a.VehicleId.Value).ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    var vehicleNumber = $"ایران {data[i].Vehicle.IranStateNumber} - {data[i].Vehicle.RightNumber} {data[i].Vehicle.NumberWord} {data[i].Vehicle.LeftNumber}";
                    ws.Cell(counter + 3, 1).Value = i + 1;
                    ws.Cell(counter + 3, 2).Value = data[i].ReceiverName;
                    ws.Cell(counter + 3, 3).Value = vehicleNumber;
                    ws.Cell(counter + 3, 4).Value = bills.Where(a => a.VehicleId.Value.Equals(data[i].VehicleId.Value) && a.CustomerId.HasValue && a.CustomerId.Value.Equals(customer)).Sum(a => a.Amount).ToString("N0");
                    ws.Cell(counter + 3, 5).Value = data[i].CustomerId.HasValue ? data[i].Customer.Name : "---";
                    counter++;
                }
            }

            var unsetCustomerData = bills.Where(a => !a.CustomerId.HasValue).DistinctBy(a => a.VehicleId.Value).ToList();

            if (unsetCustomerData.Any())
            {
                for (int i = 0; i < unsetCustomerData.Count; i++)
                {
                    var vehicleNumber = $"ایران {unsetCustomerData[i].Vehicle.IranStateNumber} - {unsetCustomerData[i].Vehicle.RightNumber} {unsetCustomerData[i].Vehicle.NumberWord} {unsetCustomerData[i].Vehicle.LeftNumber}";
                    ws.Cell(counter + 3, 1).Value = i + 1;
                    ws.Cell(counter + 3, 2).Value = unsetCustomerData[i].ReceiverName;
                    ws.Cell(counter + 3, 3).Value = vehicleNumber;
                    ws.Cell(counter + 3, 4).Value = bills.Where(a => a.VehicleId.Value.Equals(unsetCustomerData[i].VehicleId.Value) && !a.CustomerId.HasValue).Sum(a => a.Amount).ToString("N0");
                    ws.Cell(counter + 3, 5).Value = unsetCustomerData[i].CustomerId.HasValue ? unsetCustomerData[i].Customer.Name : "---";
                    counter++;
                }
            }

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(counter + 2, 5));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(14)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 5)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            var otherCosts = await db.OtherCost.Include(a => a.Vehicle).Include(a => a.Customer).AsNoTracking().Where(a => a.CalendarId.Equals(calendar.Id)).Select(a => new
            {
                a.Amount,
                a.DriverName,
                Customer = a.Customer.Name,
                VehicleNumber = $"ایران {a.Vehicle.IranStateNumber} - {a.Vehicle.RightNumber} {a.Vehicle.NumberWord} {a.Vehicle.LeftNumber}"
            }).ToListAsync();

            var distinctedOtherCosts = otherCosts.DistinctBy(a => a.VehicleNumber).ToList();

            for (int i = 0; i < distinctedOtherCosts.Count; i++)
            {
                var j = counter + i;
                ws.Cell(j + 3, 1).Value = j + 1;
                ws.Cell(j + 3, 2).Value = distinctedOtherCosts[i].DriverName;
                ws.Cell(j + 3, 3).Value = distinctedOtherCosts[i].VehicleNumber;
                ws.Cell(j + 3, 4).Value = otherCosts.Where(a => a.VehicleNumber.Equals(distinctedOtherCosts[i].VehicleNumber)).Sum(a => a.Amount).ToString("N0");
                ws.Cell(j + 3, 5).Value = distinctedOtherCosts[i].Customer;
            }

            ws.Cell(counter + distinctedOtherCosts.Count + 3, 1).Value = "جمع";
            ws.Range(counter + distinctedOtherCosts.Count + 3, 1, counter + distinctedOtherCosts.Count + 3, 3).Merge();
            ws.Cell(counter + distinctedOtherCosts.Count + 3, 4).Value = (bills.Sum(a => a.Amount) + otherCosts.Sum(a => a.Amount)).ToString("N0");

            ws.Column("A").Width = 5;
            ws.Column("B").Width = 20;
            ws.Column("C").Width = 20;
            ws.Column("D").Width = 20;
            ws.Column("E").Width = 15;

            ws.CellsUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var table = ws.Range(2, 1, counter + distinctedOtherCosts.Count + 2, 5).CreateTable();
            table.Theme = XLTableTheme.None;
            table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            table.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> Vehicles(string type)
        {
            string docTitle = $"لیست خودرو های اتحاد بار آسیا";
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(docTitle);

            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;
            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "نام و نام خانوادگی";
            ws.Cell(2, 3).Value = "شماره خودرو";
            ws.Cell(2, 4).Value = "کد ملی";

            var query = _vehicleRepo.Vehicles();
            if (type == "all")
                query = query.Where(a => a.RealStatus);

            var vehicles = await query.AsNoTracking().Include(a => a.VehicleBankAccounts).OrderBy(a => a.LeftNumber).ThenBy(a => a.RightNumber).ToListAsync();


            for (int i = 0; i < vehicles.Count; i++)
            {
                var vehicleNumber = $"ایران {vehicles[i].IranStateNumber} - {vehicles[i].RightNumber} {vehicles[i].NumberWord} {vehicles[i].LeftNumber}";
                ws.Cell(i + 3, 1).Value = i + 1;
                ws.Cell(i + 3, 2).Value = vehicles[i].VehicleOwnerFullname;
                ws.Cell(i + 3, 3).Value = vehicleNumber;
                ws.Cell(i + 3, 4).SetValue<string>(vehicles[i].NationalNumber);

                //var tejarat = vehicles[i].VehicleBankAccounts.FirstOrDefault(a => a.BankId.Equals(43));
                //if (tejarat != null)
                //{
                //    ws.Cell(i + 3, 5).SetValue<string>(tejarat.AccountNumber);
                //}

                if (!vehicles[i].VehicleBankAccounts.Any())
                    ws.Cell(i + 3, 2).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                if (!vehicles[i].RealStatus)
                    ws.Cell(i + 3, 2).Style.Fill.SetBackgroundColor(XLColor.LightSkyBlue);
            }

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(vehicles.Count + 2, 4));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(14)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 4)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            ws.Column("A").Width = 5;
            ws.Column("B").Width = 25;
            ws.Column("C").Width = 25;
            ws.Column("D").Width = 25;

            ws.CellsUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var table = ws.Range(2, 1, vehicles.Count + 2, 4).CreateTable();
            table.Theme = XLTableTheme.None;
            table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            table.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> GetHasCapacityUnrealVehicles(long calendarId)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            if (calendar == null) return NotFound();

            var unrealVehicles = await _vehicleRepo.Vehicles().AsNoTracking().Where(a => !a.RealStatus && a.Status).OrderBy(a => a.LeftNumber).ThenBy(a => a.RightNumber).ToListAsync();
            var usedVehicles = await _billRepository.Query().AsNoTracking().Where(a => a.CalendarId.Equals(calendarId) && a.VehicleId.HasValue && unrealVehicles.Select(a => a.Id).Contains(a.VehicleId.Value)).Select(a => a.VehicleId.Value).Distinct().ToListAsync();

            var vehicles = unrealVehicles.Where(a => !usedVehicles.Contains(a.Id)).ToList();

            string docTitle = $"لیست خودرو های دارای ظرفیت";
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(docTitle);

            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = $"لیست خودرو های دارای ظرفیت در {calendar.Title}";
            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "نوع";
            ws.Cell(2, 3).Value = "شماره خودرو";
            ws.Cell(2, 4).Value = "نام و نام خانوادگی مالک";


            for (int i = 0; i < vehicles.Count; i++)
            {
                var vehicleNumber = $"ایران {vehicles[i].IranStateNumber} - {vehicles[i].RightNumber} {vehicles[i].NumberWord} {vehicles[i].LeftNumber}";
                ws.Cell(i + 3, 1).Value = i + 1;
                ws.Cell(i + 3, 2).Value = vehicles[i].Type;
                ws.Cell(i + 3, 3).Value = vehicleNumber;
                ws.Cell(i + 3, 4).Value = vehicles[i].VehicleOwnerFullname;
            }

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(vehicles.Count + 2, 4));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(14)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 4)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            ws.Column("A").Width = 5;
            ws.Column("B").Width = 25;
            ws.Column("C").Width = 25;
            ws.Column("D").Width = 25;

            ws.CellsUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var table = ws.Range(2, 1, vehicles.Count + 2, 4).CreateTable();
            table.Theme = XLTableTheme.None;
            table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            table.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> GlobalDetailedReport(long customerId, long calendarId)
        {
            var data = new List<LoadFactorModel>();

            var calendar = await _calendarRepo.Get(calendarId);
            var customer = await _customerRepo.Get(customerId);

            var persianDate = new PersianDateTime(calendar.StartDate);

            #region LoadFactorCreatorBot
            var routes = await db.StaticRouteFee.AsNoTracking().ToListAsync();

            var bills = await _billRepository.Query().Include(a => a.Vehicle)
                .Where(a => a.CalendarId.Equals(calendarId) && (a.CustomerId.HasValue && a.CustomerId.Value.Equals(customerId)) && (a.VehicleId.HasValue &&
            (_vehicleRepo.Vehicles().Where(b => !b.RealStatus).Select(a => a.Id)).Contains(a.VehicleId.Value))).ToListAsync();
            var distinctedBills = bills.DistinctBy(a => a.VehicleId.Value).ToList();

            foreach (var item in distinctedBills)
            {
                data.Add(new LoadFactorModel
                {
                    DriverName = item.ReceiverName,
                    CustomerName = "",
                    VehicleId = item.VehicleId.Value,
                    VehicleLeftNumber = item.Vehicle.LeftNumber,
                    VehicleRightNumber = item.Vehicle.RightNumber,
                    VehicleNumber = $"ایران {item.Vehicle.IranStateNumber} - {item.Vehicle.RightNumber} {item.Vehicle.NumberWord} {item.Vehicle.LeftNumber}",
                    Amount = bills.Where(a => a.VehicleId.Value.Equals(item.VehicleId.Value)).Sum(a => a.Amount)
                });
            }

            var otherCosts = await db.OtherCost.Include(a => a.Vehicle).AsNoTracking().Where(a => a.CustomerId.Equals(customerId) && a.CalendarId.Equals(calendar.Id)).Select(a => new
            {
                a.Amount,
                a.DriverName,
                CustomerName = "",
                a.VehicleId,
                VehicleLeftNumber = a.Vehicle.LeftNumber,
                VehicleRightNumber = a.Vehicle.RightNumber,
                VehicleNumber = $"ایران {a.Vehicle.IranStateNumber} - {a.Vehicle.RightNumber} {a.Vehicle.NumberWord} {a.Vehicle.LeftNumber}"
            }).ToListAsync();
            var distinctedOtherCosts = otherCosts.DistinctBy(a => a.VehicleNumber).ToList();

            foreach (var item in distinctedOtherCosts)
            {
                //removing duplicate vehicles in bills and other costs
                if (data.Any(a => a.VehicleId.Equals(item.VehicleId)))
                {
                    var dataItem = data.Single(a => a.VehicleId.Equals(item.VehicleId));
                    dataItem.Amount += item.Amount;
                }
                else
                    data.Add(new LoadFactorModel
                    {
                        VehicleLeftNumber = item.VehicleLeftNumber,
                        VehicleRightNumber = item.VehicleRightNumber,
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

                if (bestRouteAmount > routes.Max(a => a.Amount))
                    bestRouteAmount = routes.Max(a => a.Amount);

                List<int> takenDays = new();
                while (itemAmount > 0)
                {
                    int day = 0;

                    day = rnd.Next(1, 30);

                    while (takenDays.Contains(day))
                    {
                        if (takenDays.Count >= 29)
                            break;

                        day = rnd.Next(1, 30);
                    }

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
                            DriverName = item.DriverName,
                            LoadFactorNumber = $"{persianDate.Year}/{rnd.Next(11111111, 59999999)}"
                        });
                    }
                    else
                    {
                        item.Details.Add(new LoadFactorDetailModel
                        {
                            Day = 32,
                            Amount = itemAmount,
                            Date = "---",
                            Origin = "---",
                            Destination = "---",
                            DriverName = "---",
                            LoadFactorNumber = "سایر / تناژ"
                        });
                        itemAmount = 0;
                    }
                }
            }

            #endregion

            var loadFactorNovin = await (from a in db.LoadFactorNovin
                                         join b in db.Vehicles on a.VehicleId equals b.Id
                                         join c in db.Driver on a.DriverId equals c.Id
                                         where a.CalendarId.Equals(calendarId) && a.CustomerId.Equals(customerId)
                                         select new
                                         {
                                             VehicleId = b.Id,
                                             b.LeftNumber,
                                             b.RightNumber,
                                             a.Date,
                                             DriverName = c.Fullname,
                                             a.Origin,
                                             a.Destination,
                                             a.Tonnage,
                                             a.DriverTonnagePrice,
                                             a.DriverFee,
                                             VehicleNumber = $"ایران {b.IranStateNumber} - {b.RightNumber} {b.NumberWord} {b.LeftNumber}",
                                             a.LoadNumber,
                                             a.LoadNumberGov,
                                         }).AsNoTracking().OrderBy(a => a.LeftNumber).ToListAsync();

            var vehicleDataNovin = loadFactorNovin.GroupBy(a => a.VehicleNumber).ToList();

            var vehicleIdsNovin = loadFactorNovin.Select(a => a.VehicleId).Distinct().ToList();
            var vehicleDetailsNovin = await _vehicleRepo.Vehicles().Where(a => vehicleIdsNovin.Contains(a.Id)).AsNoTracking().ToArrayAsync();

            foreach (var vehicle in vehicleDataNovin)
            {
                var vehicleId = vehicle.ElementAt(0).VehicleId;

                if (data.Any(a => a.VehicleId.Equals(vehicleId)))
                {
                    var dataItem = data.Single(a => a.VehicleId.Equals(vehicleId));

                    var otherFee = 0D;
                    for (int i = 0; i < vehicle.Count(); i++)
                    {
                        var item = vehicle.ElementAt(i);

                        if (item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue)
                            otherFee += item.Tonnage.Value * item.DriverTonnagePrice.Value;

                        dataItem.Details.Add(new LoadFactorDetailModel
                        {
                            Day = 0,
                            IsFreeDriverPrice = true,
                            DriverName = item.DriverName,
                            Amount = item.DriverFee,
                            Date = new PersianDateTime(item.Date).ToString("yyyy/MM/dd"),
                            Origin = item.Origin,
                            Destination = item.Destination,
                            LoadFactorNumber = string.IsNullOrWhiteSpace(item.LoadNumberGov) ? $"{persianDate.Year}/{rnd.Next(11111111, 59999999)}" : item.LoadNumberGov,
                        });
                    }

                    if (otherFee > 0)
                    {
                        if (dataItem.Details.Any(a => a.Day == 32))
                        {
                            var detailItem = dataItem.Details.Single(a => a.Day == 32);
                            detailItem.Amount += otherFee;
                        }
                        else
                        {
                            dataItem.Details.Add(new LoadFactorDetailModel
                            {
                                Day = 32,
                                Amount = otherFee,
                                Date = "---",
                                Origin = "---",
                                Destination = "---",
                                LoadFactorNumber = "سایر / تناژ"
                            });
                        }
                    }
                }
                else
                {
                    var vehicleItem = vehicleDetailsNovin.Single(a => a.Id.Equals(vehicleId));

                    var loadFactorModelItem = new LoadFactorModel
                    {
                        DriverName = vehicleItem.VehicleOwnerFullname,
                        CustomerName = customer.Name,
                        VehicleId = vehicleId,
                        VehicleLeftNumber = vehicleItem.LeftNumber,
                        VehicleRightNumber = vehicleItem.RightNumber,
                        VehicleNumber = $"ایران {vehicleItem.IranStateNumber} - {vehicleItem.RightNumber} {vehicleItem.NumberWord} {vehicleItem.LeftNumber}",
                        Amount = 0
                    };

                    var otherFee = 0D;
                    for (int i = 0; i < vehicle.Count(); i++)
                    {
                        var item = vehicle.ElementAt(i);

                        if (item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue)
                            otherFee += item.Tonnage.Value * item.DriverTonnagePrice.Value;

                        loadFactorModelItem.Details.Add(new LoadFactorDetailModel
                        {
                            Day = 0,
                            Amount = item.DriverFee,
                            DriverName = item.DriverName,
                            IsFreeDriverPrice = true,
                            Date = new PersianDateTime(item.Date).ToString("yyyy/MM/dd"),
                            Origin = item.Origin,
                            Destination = item.Destination,
                            LoadFactorNumber = string.IsNullOrWhiteSpace(item.LoadNumberGov) ? $"{persianDate.Year}/{rnd.Next(11111111, 59999999)}" : item.LoadNumberGov,
                        });
                    }

                    if (otherFee > 0)
                    {
                        loadFactorModelItem.Details.Add(new LoadFactorDetailModel
                        {
                            Day = 32,
                            Amount = otherFee,
                            Date = "---",
                            Origin = "---",
                            Destination = "---",
                            LoadFactorNumber = "سایر / تناژ"
                        });
                    }

                    data.Add(loadFactorModelItem);
                }
            }


            var query = await (from a in db.LoadFactor
                               join b in db.Contract on a.ContractId equals b.Id
                               join c in db.Vehicles on a.VehicleId equals c.Id
                               join d in db.Driver on a.DriverId equals d.Id
                               where a.CalendarId.Equals(calendarId) && b.CustomerId.Equals(customerId)
                               select new
                               {
                                   VehicleId = c.Id,
                                   c.LeftNumber,
                                   c.RightNumber,
                                   a.Date,
                                   DriverName = d.Fullname,
                                   Origin = a.Origin.Title,
                                   Destination = a.Destination.Title,
                                   a.IsFreeDriverPrice,
                                   a.Tonnage,
                                   a.DriverTonnagePrice,
                                   a.DriverFee,
                                   VehicleNumber = $"ایران {c.IranStateNumber} - {c.RightNumber} {c.NumberWord} {c.LeftNumber}",
                                   a.WeighbridgePrice,
                                   a.DriverLoadSleepPrice,
                                   a.LoadNumber,
                                   a.LoadNumberGov,

                               }).AsNoTracking().OrderBy(a => a.LeftNumber).ToListAsync();

            var vehicleData = query.GroupBy(a => a.VehicleNumber).ToList();

            var vehicleIds = query.Select(a => a.VehicleId).Distinct().ToList();
            var vehicleDetails = await _vehicleRepo.Vehicles().Where(a => vehicleIds.Contains(a.Id)).AsNoTracking().ToArrayAsync();

            foreach (var vehicle in vehicleData)
            {
                var vehicleId = vehicle.ElementAt(0).VehicleId;

                if (data.Any(a => a.VehicleId.Equals(vehicleId)))
                {
                    var dataItem = data.Single(a => a.VehicleId.Equals(vehicleId));

                    var otherFee = 0D;
                    for (int i = 0; i < vehicle.Count(); i++)
                    {
                        var item = vehicle.ElementAt(i);

                        if (item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue)
                            otherFee += item.Tonnage.Value * item.DriverTonnagePrice.Value;

                        if (item.WeighbridgePrice.HasValue)
                            otherFee += item.WeighbridgePrice.Value;

                        if (item.DriverLoadSleepPrice.HasValue)
                            otherFee += item.DriverLoadSleepPrice.Value;

                        dataItem.Details.Add(new LoadFactorDetailModel
                        {
                            Day = 0,
                            IsFreeDriverPrice = item.IsFreeDriverPrice,
                            DriverName = item.DriverName,
                            Amount = item.DriverFee,
                            Date = new PersianDateTime(item.Date).ToString("yyyy/MM/dd"),
                            Origin = item.Origin,
                            Destination = item.Destination,
                            LoadFactorNumber = customer.CustomerType == CustomerType.SaipaPlasco ? (string.IsNullOrWhiteSpace(item.LoadNumberGov) ? item.LoadNumber : item.LoadNumberGov) : item.LoadNumber,
                        });
                    }

                    if (otherFee > 0)
                    {
                        if (dataItem.Details.Any(a => a.Day == 32))
                        {
                            var detailItem = dataItem.Details.Single(a => a.Day == 32);
                            detailItem.Amount += otherFee;
                        }
                        else
                        {
                            dataItem.Details.Add(new LoadFactorDetailModel
                            {
                                Day = 32,
                                Amount = otherFee,
                                Date = "---",
                                Origin = "---",
                                Destination = "---",
                                LoadFactorNumber = "سایر / تناژ"
                            });
                        }
                    }
                }
                else
                {
                    var vehicleItem = vehicleDetails.Single(a => a.Id.Equals(vehicleId));

                    var loadFactorModelItem = new LoadFactorModel
                    {
                        DriverName = vehicleItem.VehicleOwnerFullname,
                        CustomerName = customer.Name,
                        VehicleId = vehicleId,
                        VehicleLeftNumber = vehicleItem.LeftNumber,
                        VehicleRightNumber = vehicleItem.RightNumber,
                        VehicleNumber = $"ایران {vehicleItem.IranStateNumber} - {vehicleItem.RightNumber} {vehicleItem.NumberWord} {vehicleItem.LeftNumber}",
                        Amount = 0
                    };

                    var otherFee = 0D;
                    for (int i = 0; i < vehicle.Count(); i++)
                    {
                        var item = vehicle.ElementAt(i);

                        if (item.Tonnage.HasValue && item.DriverTonnagePrice.HasValue)
                            otherFee += item.Tonnage.Value * item.DriverTonnagePrice.Value;

                        if (item.WeighbridgePrice.HasValue)
                            otherFee += item.WeighbridgePrice.Value;

                        if (item.DriverLoadSleepPrice.HasValue)
                            otherFee += item.DriverLoadSleepPrice.Value;

                        loadFactorModelItem.Details.Add(new LoadFactorDetailModel
                        {
                            Day = 0,
                            Amount = item.DriverFee,
                            DriverName = item.DriverName,
                            IsFreeDriverPrice = item.IsFreeDriverPrice,
                            Date = new PersianDateTime(item.Date).ToString("yyyy/MM/dd"),
                            Origin = item.Origin,
                            Destination = item.Destination,
                            LoadFactorNumber = customer.CustomerType == CustomerType.SaipaPlasco ? (string.IsNullOrWhiteSpace(item.LoadNumberGov) ? item.LoadNumber : item.LoadNumberGov) : item.LoadNumber,
                        });
                    }

                    if (otherFee > 0)
                    {
                        loadFactorModelItem.Details.Add(new LoadFactorDetailModel
                        {
                            Day = 32,
                            Amount = otherFee,
                            Date = "---",
                            Origin = "---",
                            Destination = "---",
                            LoadFactorNumber = "سایر / تناژ"
                        });
                    }

                    data.Add(loadFactorModelItem);
                }
            }

            using var workbook = new XLWorkbook();
            var docTitle = $"عملکرد {customer.Name} در {calendar.Title}";

            foreach (var item in data.OrderBy(a => a.VehicleLeftNumber).ThenBy(a => a.VehicleRightNumber))
            {
                var ws = workbook.Worksheets.Add(item.VehicleNumber);
                ws.RightToLeft = true;
                ws.Style.Font.FontName = "B Titr";
                ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                ws.Cell(1, 1).Value = $"عملکرد {item.VehicleNumber} در {calendar.Title} شرکت {customer.Name}";
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

                item.Details = item.Details.OrderBy(a => a.Day).ThenBy(a => a.Date).ToList();
                for (int i = 0; i < item.Details.Count; i++)
                {
                    var detail = item.Details[i];
                    ws.Cell(i + 3, 1).SetValue(i + 1);
                    ws.Cell(i + 3, 2).SetValue(detail.Date);
                    ws.Cell(i + 3, 3).SetValue(detail.DriverName);
                    ws.Cell(i + 3, 4).SetValue(detail.Origin);
                    ws.Cell(i + 3, 5).SetValue(detail.Destination);
                    ws.Cell(i + 3, 6).SetValue(detail.LoadFactorNumber);
                    ws.Cell(i + 3, 7).SetValue(detail.IsFreeDriverPrice ? "بلی" : "خیر");
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
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");

        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> DriversPeriodicActivity(long fromCalendarId, long toCalendarId)
        {
            var calendars = await _calendarRepo.Calendars().AsNoTracking().Where(a => a.Id.Equals(fromCalendarId) || a.Id.Equals(toCalendarId)).ToListAsync();
            var fromCalendar = calendars.Single(a => a.Id.Equals(fromCalendarId));
            var toCalendar = calendars.Single(a => a.Id.Equals(toCalendarId));

            if (fromCalendar.Sequence > toCalendar.Sequence)
                return BadRequest("تقویم به درستی انتخاب نشده است");

            var calendarIdList = new List<long>();

            if (toCalendar.Sequence != fromCalendar.Sequence)
            {
                var betweenCalendars = await _calendarRepo.Calendars().AsNoTracking().Where(a => a.Sequence > fromCalendar.Sequence && a.Sequence < toCalendar.Sequence).Select(a => a.Id).ToListAsync();
                if (betweenCalendars.Any())
                    calendarIdList.AddRange(betweenCalendars);
            }
            calendarIdList.Add(fromCalendar.Id);
            calendarIdList.Add(toCalendar.Id);

            calendarIdList = calendarIdList.Distinct().ToList();

            var bills = await _billRepository.Query().AsNoTracking().Where(a => calendarIdList.Contains(a.CalendarId) && a.VehicleId.HasValue).Select(a => new { VehicleId = a.VehicleId.Value, a.Amount }).ToListAsync();
            var vehicleIdList = bills.Select(a => a.VehicleId).Distinct().ToList();

            var otherCosts = await db.OtherCost.AsNoTracking().Where(a => calendarIdList.Contains(a.CalendarId)).Select(a => new { VehicleId = a.VehicleId, a.Amount }).ToListAsync();
            vehicleIdList.AddRange(otherCosts.Select(a => a.VehicleId).Distinct());

            var loadFactors = await _loadFactorRepo.LoadFactors().AsNoTracking().Where(a => calendarIdList.Contains(a.CalendarId)).Select(a => new
            {
                a.VehicleId,
                Amount = a.DriverFee +
                (a.Tonnage.HasValue ? a.Tonnage.Value * a.DriverTonnagePrice.Value : 0) +
                (a.WeighbridgePrice.HasValue ? a.WeighbridgePrice.Value : 0) +
                (a.DriverLoadSleepPrice.HasValue ? a.DriverLoadSleepPrice.Value : 0)
            }).ToListAsync();

            vehicleIdList.AddRange(loadFactors.Select(a => a.VehicleId).Distinct());
            vehicleIdList = vehicleIdList.Distinct().ToList();

            var vehicles = await _vehicleRepo.Vehicles().Include(a => a.VehicleBankAccounts).AsNoTracking().Where(a => vehicleIdList.Contains(a.Id)).OrderBy(a => a.LeftNumber).ThenBy(a => a.RightNumber).ToListAsync();


            string docTitle = $"لیست عملکرد دوره ای خودرو ها";
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(docTitle);

            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = $"لیست عملکرد خودرو ها از {fromCalendar.Title} الی {toCalendar.Title}";
            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "نام و نام خانوادگی";
            ws.Cell(2, 3).Value = "شماره خودرو";
            ws.Cell(2, 4).Value = "کد ملی";
            ws.Cell(2, 5).Value = "مبلغ عملکرد";
            ws.Cell(2, 6).Value = "مبلغ پرداختی";

            for (int i = 0; i < vehicles.Count; i++)
            {
                var vehicleNumber = $"ایران {vehicles[i].IranStateNumber} - {vehicles[i].RightNumber} {vehicles[i].NumberWord} {vehicles[i].LeftNumber}";
                ws.Cell(i + 3, 1).Value = i + 1;
                ws.Cell(i + 3, 2).Value = vehicles[i].VehicleOwnerFullname;
                ws.Cell(i + 3, 3).Value = vehicleNumber;
                ws.Cell(i + 3, 4).SetValue<string>(vehicles[i].NationalNumber);
                ws.Cell(i + 3, 5).SetValue<string>(loadFactors.Any(a => a.VehicleId.Equals(vehicles[i].Id)) ?
                    loadFactors.Where(a => a.VehicleId.Equals(vehicles[i].Id)).Sum(a => a.Amount).ToString("N0") :
                    (bills.Where(a => a.VehicleId.Equals(vehicles[i].Id)).Sum(a => a.Amount) +
                    otherCosts.Where(a => a.VehicleId.Equals(vehicles[i].Id)).Sum(a => a.Amount)).ToString("N0"));

                ws.Cell(i + 3, 6).SetValue<string>(bills.Where(a => a.VehicleId.Equals(vehicles[i].Id)).Sum(a => a.Amount).ToString("N0"));

                if (!vehicles[i].VehicleBankAccounts.Any())
                    ws.Cell(i + 3, 2).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                if (!vehicles[i].RealStatus)
                    ws.Cell(i + 3, 2).Style.Fill.SetBackgroundColor(XLColor.LightSkyBlue);
            }

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(vehicles.Count + 2, 6));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(13)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 6)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            ws.Column("A").Width = 5;
            ws.Column("B").Width = 20;
            ws.Column("C").Width = 18;
            ws.Column("D").Width = 15;
            ws.Column("E").Width = 17;
            ws.Column("F").Width = 17;

            ws.CellsUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var table = ws.Range(2, 1, vehicles.Count + 2, 6).CreateTable();
            table.Theme = XLTableTheme.None;
            table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            table.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");

        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> LoadFactorGov(long customerId, string startDate, string endDate)
        {
            var startArr = startDate.PersianToEnglish().Split('/');
            var startD = new PersianDateTime(Convert.ToInt32(startArr[0]), Convert.ToInt32(startArr[1]), Convert.ToInt32(startArr[2]), 0, 0, 0).ToDateTime();
            var endArr = endDate.PersianToEnglish().Split('/');
            var endD = new PersianDateTime(Convert.ToInt32(endArr[0]), Convert.ToInt32(endArr[1]), Convert.ToInt32(endArr[2]), 23, 59, 59).ToDateTime();
            if (startD > endD)
            {
                TempData["msg"] = $"تاریخ شروع از تاریخ پایان بزرگتر است! |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var thisCustomerContractIdList = await db.Contract.AsNoTracking().Where(a => a.CustomerId.Equals(customerId)).Select(a => a.Id).ToListAsync();
            var data = await _loadFactorRepo.LoadFactors().AsNoTracking()
                .Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Vehicle).Include(a => a.LoadFactorGovRegistor)
                .Where(a => (!string.IsNullOrWhiteSpace(a.LoadNumberGov) && a.LoadFactorGovAmount.HasValue)
                && a.LoadFactorGovDate.HasValue && a.LoadFactorGovDate.Value >= startD && a.LoadFactorGovDate.Value <= endD && thisCustomerContractIdList.Contains(a.ContractId))
                .OrderBy(a => a.Date).ToListAsync();

            string docTitle = $"گزارش بارنامه های دولتی از {startDate} الی {endDate}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش");
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "شماره بارنامه";
            ws.Cell(2, 4).Value = "مبلغ";
            ws.Cell(2, 5).Value = "شرکت صادر کننده";
            ws.Cell(2, 6).Value = "مبدا";
            ws.Cell(2, 7).Value = "مقصد";
            ws.Cell(2, 8).Value = "نام راننده";
            ws.Cell(2, 9).Value = "نوع خودرو";
            ws.Cell(2, 10).Value = "پلاک";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(data.Count + 2, 10));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 10)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            var amountSum = 0d;
            for (int index = 1; index <= data.Count; index++)
            {
                var item = data[index - 1];
                var amount = item.LoadFactorGovAmount ?? 0;
                amountSum += amount;

                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(item.LoadFactorGovDate.Value).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).SetValue(item.LoadNumberGov);
                ws.Cell(index + 2, 4).Value = amount.ToString("N0");
                ws.Cell(index + 2, 5).Value = item.LoadFactorGovRegistorId.HasValue ? item.LoadFactorGovRegistor.Title : "---";
                ws.Cell(index + 2, 6).Value = item.Origin.Title;
                ws.Cell(index + 2, 7).Value = item.Destination.Title;
                ws.Cell(index + 2, 8).Value = item.Vehicle.VehicleOwnerFullname;
                ws.Cell(index + 2, 9).Value = item.Vehicle.Type;
                ws.Cell(index + 2, 10).Value = item.Vehicle.RightNumber + " " + item.Vehicle.NumberWord + " " + item.Vehicle.LeftNumber + " - " + item.Vehicle.IranStateNumber;
            }

            ws.Cell($"B{data.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{data.Count + 3}:I{data.Count + 3}").Row(1).Merge();
            ws.Cell(data.Count + 3, 10).Value = data.Count;

            ws.Cell($"B{data.Count + 4}").Value = "جمع کل مبلغ";
            ws.Range($"B{data.Count + 4}:I{data.Count + 4}").Row(1).Merge();
            ws.Cell(data.Count + 4, 10).Value = amountSum.ToString("N0");

            ws.RangeUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngTable2 = ws.Range($"B{data.Count + 3}:J{data.Count + 10}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Columns().AdjustToContents();

            ws.PageSetup.SetPaperSize(XLPaperSize.A4Paper);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> LoadFactorGovReportByCompany(long companyId, int sYear, int sMonth, int sDay, int eYear, int eMonth, int eDay)
        {
            var startD = new PersianDateTime(sYear, sMonth, sDay, 0, 0, 0).ToDateTime();
            var endD = new PersianDateTime(eYear, eMonth, eDay, 23, 59, 59).ToDateTime();
            if (startD > endD)
            {
                TempData["msg"] = $"تاریخ شروع از تاریخ پایان بزرگتر است! |danger";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var data = await _loadFactorRepo.LoadFactors().AsNoTracking()
                .Include(a => a.Origin).Include(a => a.Destination).Include(a => a.Vehicle).Include(a => a.LoadFactorGovRegistor)
                .Where(a => (!string.IsNullOrWhiteSpace(a.LoadNumberGov) && a.LoadFactorGovAmount.HasValue && a.LoadFactorGovRegistorId.HasValue)
                && a.LoadFactorGovDate.HasValue && a.LoadFactorGovDate.Value >= startD && a.LoadFactorGovDate.Value <= endD && a.LoadFactorGovRegistorId.Value.Equals(companyId))
                .OrderBy(a => a.Date).ToListAsync();

            var companyName = await db.Definition.AsNoTracking().Where(a => a.Id.Equals(companyId)).Select(a => a.Title).FirstOrDefaultAsync();

            string docTitle = $"گزارش بارنامه های دولتی شرکت {companyName} از {sYear}/{sMonth}/{sDay} الی {eYear}/{eMonth}/{eDay}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش");
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "شماره بارنامه";
            ws.Cell(2, 4).Value = "مبلغ";
            ws.Cell(2, 5).Value = "شرکت صادر کننده";
            ws.Cell(2, 6).Value = "مبدا";
            ws.Cell(2, 7).Value = "مقصد";
            ws.Cell(2, 8).Value = "نام راننده";
            ws.Cell(2, 9).Value = "نوع خودرو";
            ws.Cell(2, 10).Value = "پلاک";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(data.Count + 2, 10));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 10)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            var amountSum = 0d;
            for (int index = 1; index <= data.Count; index++)
            {
                var item = data[index - 1];
                var amount = item.LoadFactorGovAmount ?? 0;
                amountSum += amount;

                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(item.LoadFactorGovDate.Value).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).SetValue(item.LoadNumberGov);
                ws.Cell(index + 2, 4).Value = amount.ToString("N0");
                ws.Cell(index + 2, 5).Value = item.LoadFactorGovRegistorId.HasValue ? item.LoadFactorGovRegistor.Title : "---";
                ws.Cell(index + 2, 6).Value = item.Origin.Title;
                ws.Cell(index + 2, 7).Value = item.Destination.Title;
                ws.Cell(index + 2, 8).Value = item.Vehicle.VehicleOwnerFullname;
                ws.Cell(index + 2, 9).Value = item.Vehicle.Type;
                ws.Cell(index + 2, 10).Value = item.Vehicle.RightNumber + " " + item.Vehicle.NumberWord + " " + item.Vehicle.LeftNumber + " - " + item.Vehicle.IranStateNumber;
            }

            ws.Cell($"B{data.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{data.Count + 3}:I{data.Count + 3}").Row(1).Merge();
            ws.Cell(data.Count + 3, 10).Value = data.Count;

            ws.Cell($"B{data.Count + 4}").Value = "جمع کل مبلغ";
            ws.Range($"B{data.Count + 4}:I{data.Count + 4}").Row(1).Merge();
            ws.Cell(data.Count + 4, 10).Value = amountSum.ToString("N0");

            ws.RangeUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngTable2 = ws.Range($"B{data.Count + 3}:J{data.Count + 10}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Columns().AdjustToContents();

            ws.PageSetup.SetPaperSize(XLPaperSize.A4Paper);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad")]
        public async Task<IActionResult> General(long calendarId)
        {
            var customers = await _customerRepo.Customers().AsNoTracking().Where(a => a.Status).Select(a => new { a.Id, a.Name }).ToListAsync();
            var contracts = await db.Contract.AsNoTracking().Where(a => customers.Select(a => a.Id).Contains(a.CustomerId)).Select(a => new { a.Id, a.CustomerId }).ToListAsync();
            var calendar = await _calendarRepo.Get(calendarId);
            var customerFactors = await db.CustomerFactor.AsNoTracking().Where(a => a.CalendarId.Equals(calendarId)).ToListAsync();
            var loadFactors = await db.LoadFactor.AsNoTracking().Where(a => a.CalendarId.Equals(calendarId)).Select(a => new
            {
                a.ContractId,
                a.DriverFee,
                a.Tonnage,
                a.DriverTonnagePrice,
                a.DriverLoadSleepPrice,
                a.WeighbridgePrice,
                a.LoadSleepTime
            }).ToListAsync();

            var driverData = new List<GeneralReportDriverFeeVM>();
            foreach (var item in loadFactors)
            {
                var amount = item.DriverFee;

                if (item.Tonnage.HasValue)
                    amount += item.Tonnage.Value * (item.DriverTonnagePrice ?? 0);

                if (item.LoadSleepTime.HasValue)
                    amount += item.DriverLoadSleepPrice ?? 0;

                if (item.WeighbridgePrice.HasValue)
                    amount += item.WeighbridgePrice.Value;
                driverData.Add(new GeneralReportDriverFeeVM
                {
                    Amount = amount,
                    ContractId = item.ContractId
                });
            }
            var cost = await _costRepo.Costs().AsNoTracking().Where(a => a.CalendarId.Equals(calendarId) && !a.Description.Contains("*")).SumAsync(a => a.Amount);
            var otherCosts = await _billRepository.Query().AsNoTracking().Where(a => a.Date >= calendar.StartDate && a.Date <= calendar.EndDate && a.BillType.Equals("جاری")).SumAsync(a => a.Amount);
            cost += otherCosts;

            var data = new List<GeneralReportVM>();
            foreach (var customer in customers)
            {
                data.Add(new GeneralReportVM
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
                    CostSum = cost / 4,
                    DriversAmountSum = driverData.Where(a => contracts.Where(a => a.CustomerId.Equals(customer.Id)).Select(a => a.Id).ToList().Contains(a.ContractId)).Sum(a => a.Amount),
                    FactorsSum = customerFactors.Where(a => a.CustomerId.Equals(customer.Id)).Sum(a => a.Amount)
                });
            }

            string docTitle = $"گزارش کلی عملکرد از {calendar.Title}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش");
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "نام";
            ws.Cell(2, 2).Value = "جمع دریافتی";
            ws.Cell(2, 3).Value = "جمع پرداختی";
            ws.Cell(2, 4).Value = "جمع هزینه های جاری";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(data.Count + 2, 4));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 4)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= data.Count; index++)
            {
                var item = data[index - 1];

                ws.Cell(index + 2, 1).Value = item.CustomerName;
                ws.Cell(index + 2, 2).Value = item.FactorsSum.ToString("N0");
                ws.Cell(index + 2, 3).Value = item.DriversAmountSum.ToString("N0");
                ws.Cell(index + 2, 4).Value = item.CostSum.ToString("N0");
            }

            ws.Cell($"B{data.Count + 3}").Value = "جمع کل دریافتی";
            ws.Range($"B{data.Count + 3}:C{data.Count + 3}").Row(1).Merge();
            ws.Cell(data.Count + 3, 4).Value = customerFactors.Sum(a => a.Amount).ToString("N0");

            ws.Cell($"B{data.Count + 4}").Value = "جمع کل پرداختی";
            ws.Range($"B{data.Count + 4}:C{data.Count + 4}").Row(1).Merge();
            ws.Cell(data.Count + 4, 4).Value = driverData.Sum(a => a.Amount).ToString("N0");

            ws.Cell($"B{data.Count + 5}").Value = "جمع کل هزینه ها";
            ws.Range($"B{data.Count + 5}:C{data.Count + 5}").Row(1).Merge();
            ws.Cell(data.Count + 5, 4).Value = cost.ToString("N0");

            ws.RangeUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngTable2 = ws.Range($"B{data.Count + 3}:D{data.Count + 5}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Columns().AdjustToContents();

            ws.PageSetup.SetPaperSize(XLPaperSize.A4Paper);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Accountant")]
        public async Task<IActionResult> BillList(string id)
        {
            var billItem = await _billRepository.Query().AsNoTracking().Include(a => a.OtherCosts).ThenInclude(a => a.Vehicle).FirstOrDefaultAsync(a => a.RowId.Equals(id));

            List<BillPrintDataVM> billItemList = new();
            if (billItem.OtherCosts.Any())
            {
                foreach (var item in billItem.OtherCosts)
                    billItemList.Add(new BillPrintDataVM
                    {
                        Amount = item.Amount,
                        ReceiverName = item.DriverName,
                        VehicleLeftNumber = item.Vehicle.LeftNumber,
                        VehicleRightNumber = item.Vehicle.RightNumber,
                        VehicleNumber = $"ایران {item.Vehicle.IranStateNumber} - {item.Vehicle.RightNumber} {item.Vehicle.NumberWord} {item.Vehicle.LeftNumber}"
                    });
            }
            else
            {
                billItemList = await _billRepository.Query().AsNoTracking().Include(a => a.Vehicle)
                    .Where(a => a.BillNo.Equals(billItem.BillNo)).Select(a => new BillPrintDataVM
                    {
                        Amount = a.Amount,
                        ReceiverName = a.ReceiverName,
                        VehicleLeftNumber = a.Vehicle.LeftNumber,
                        VehicleRightNumber = a.Vehicle.RightNumber,
                        VehicleNumber = $"ایران {a.Vehicle.IranStateNumber} - {a.Vehicle.RightNumber} {a.Vehicle.NumberWord} {a.Vehicle.LeftNumber}"
                    }).ToListAsync();
            }

            billItemList = billItemList.OrderBy(a => a.VehicleLeftNumber).ThenBy(a => a.VehicleRightNumber).ToList();
            string docTitle = $"لیست قبض پرداختی به شماره {billItem.BillNo}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(billItem.BillNo);

            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            // page header
            string bankPresidentName = "",
                phrase = "";
            if (billItem.BankBranch.Contains("ملت"))
            {
                bankPresidentName = "جناب آقای طحان پور";
                phrase = "ریاست محترم بانک ملت شعبه پارس خودرو کد 67603";
            }
            else if (billItem.BankBranch.Contains("تجارت"))
            {
                bankPresidentName = "سرکار خانم کیانی";
                phrase = "ریاست محترم بانک تجارت شعبه چیتگر کد 409";
            }
            else if (billItem.BankBranch.Contains("سامان"))
            {
                bankPresidentName = "جناب آقای باقری نیا";
                phrase = "ریاست محترم بانک سامان شعبه شهرک راه آهن کد 840";
            }
            else if (billItem.BankBranch.Contains("پاسارگاد"))
            {
                bankPresidentName = "جناب آقای قربانی";
                phrase = "ریاست محترم بانک پاسارگاد شعبه شهرک راه آهن کد 241";
            }

            ws.Cell(1, 1).Value = bankPresidentName;
            ws.Cell(2, 1).Value = phrase;
            ws.Cell(3, 1).Value = "با سلام";
            ws.Cell(4, 1).Value = $"احتراماً به پیوست لیست پرداختی به مبلغ {billItemList.Sum(a => a.Amount):N0} ریال";
            ws.Cell(5, 1).Value = "در وجه خودتان جهت واریز به حساب رانندگان مشروحه ذیل تقدیم می گردد.";
            //

            ws.Cell(6, 1).Value = "ردیف";
            ws.Cell(6, 2).Value = "نام و نام خانوادگی";
            ws.Cell(6, 3).Value = "شماره خودرو";
            ws.Cell(6, 4).Value = "مبلغ";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(billItemList.Count + 6, 4));
            rngTable.FirstRow().Merge();
            ws.Range(2, 1, 2, 4).Merge();
            ws.Range(3, 1, 3, 4).Merge();
            ws.Range(4, 1, 4, 4).Merge();
            ws.Range(5, 1, 5, 4).Merge();

            var rngHeaders = rngTable.Range(rngTable.Cell(1, 1), rngTable.Cell(5, 4)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= billItemList.Count; index++)
            {
                ws.Cell(index + 6, 1).Value = index;
                ws.Cell(index + 6, 2).Value = billItemList[index - 1].ReceiverName.Replace("/", "");
                ws.Cell(index + 6, 3).Value = billItemList[index - 1].VehicleNumber;
                ws.Cell(index + 6, 4).Value = billItemList[index - 1].Amount.ToString("N0");
            }

            ws.Cell($"A{billItemList.Count + 7}").Value = "جمع کل";
            ws.Cell($"D{billItemList.Count + 7}").Value = billItemList.Sum(a => a.Amount).ToString("N0");
            ws.Range($"A{billItemList.Count + 7}:C{billItemList.Count + 7}").Row(1).Merge();

            var rngTable2 = ws.Range($"A{billItemList.Count + 7}:D{billItemList.Count + 7}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12);

            ws.Cell($"D{billItemList.Count + 8}").Value = "با تشکر";
            ws.Cell($"D{billItemList.Count + 9}").Value = "امیر سعادت";

            var contentRange = ws.Range(6, 1, billItemList.Count + 7, 4);
            contentRange.Style.Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            contentRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            contentRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                .Border.SetInsideBorderColor(XLColor.Black);

            ws.Column(1).Width = 5.22;
            ws.Column(2).Width = 26;
            ws.Column(3).Width = 26.67;
            ws.Column(4).Width = 25;

            ws.PageSetup.SetPageOrientation(XLPageOrientation.Portrait)
                .SetPaperSize(XLPaperSize.A4Paper)
                .Margins.SetTop(2.5).SetBottom(0.8).SetRight(1).SetLeft(0.5).SetHeader(0.25).SetFooter(0.25);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Milad, Accountant")]
        public async Task<IActionResult> DetailedCost(long customer, string date)
        {
            var dateArr = date.Split('|');
            var startArr = dateArr[0].PersianToEnglish().Split('/');
            var startDate = new PersianDateTime(Convert.ToInt32(startArr[0]), Convert.ToInt32(startArr[1]), Convert.ToInt32(startArr[2]), 0, 0, 0).ToDateTime();
            var endArr = dateArr[1].PersianToEnglish().Split('/');
            var endDate = new PersianDateTime(Convert.ToInt32(endArr[0]), Convert.ToInt32(endArr[1]), Convert.ToInt32(endArr[2]), 23, 59, 59).ToDateTime();

            var calendars = await _calendarRepo.Calendars().AsNoTracking().Where(a => a.StartDate >= startDate && a.EndDate <= endDate).OrderBy(a => a.Id).ToListAsync();
            var customerInfo = await _customerRepo.Get(customer);
            var contractIdList = await db.Contract.AsNoTracking().Where(a => a.CustomerId.Equals(customer)).Select(a => a.Id).ToListAsync();
            var data = new List<CustomerDetailedCostVM>();

            foreach (var item in calendars)
            {
                var loadFactors = await _loadFactorRepo.LoadFactors().AsNoTracking().Where(a => a.CalendarId.Equals(item.Id) && contractIdList.Contains(a.ContractId))
                    .Select(a => new { a.DriverFee, a.Tonnage, a.WeighbridgePrice, a.DriverTonnagePrice, a.DriverLoadSleepPrice }).ToListAsync();

                var driverFee = 0d;
                foreach (var fee in loadFactors)
                {
                    driverFee += fee.DriverFee;
                    if (fee.Tonnage.HasValue)
                        driverFee += fee.Tonnage.Value * fee.DriverTonnagePrice.Value;

                    if (fee.WeighbridgePrice.HasValue)
                        driverFee += fee.WeighbridgePrice.Value;

                    if (fee.DriverLoadSleepPrice.HasValue)
                        driverFee += fee.DriverLoadSleepPrice.Value;
                }

                var otherCosts = await db.OtherCost.AsNoTracking().Where(a => a.CalendarId.Equals(item.Id) && a.CustomerId.Equals(customer)).SumAsync(a => a.Amount);

                var loadFactorNovins = await db.LoadFactorNovin.AsNoTracking().Where(a => a.CalendarId.Equals(item.Id) && a.CustomerId.Equals(customer)).SumAsync(a => a.Amount);

                var bills = await _billRepository.Query().AsNoTracking().Where(a => a.CalendarId.Equals(item.Id) && a.CustomerId.Equals(customer) && a.VehicleId.HasValue).SumAsync(a => a.Amount);

                data.Add(new CustomerDetailedCostVM
                {
                    BillSum = bills,
                    CalendarTitle = item.Title,
                    CostSum = driverFee + otherCosts + loadFactorNovins
                });
            }

            string docTitle = $"گزارش هزینه های {customerInfo.Name} از {dateArr[0]} الی {dateArr[1]}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش");
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "دوره";
            ws.Cell(2, 3).Value = "هزینه ثبت شده";
            ws.Cell(2, 4).Value = "هزینه پرداخت شده";
            ws.Cell(2, 5).Value = "هزینه پرداخت نشده";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(data.Count + 2, 5));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 5)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= data.Count; index++)
            {
                var item = data[index - 1];

                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = item.CalendarTitle;
                ws.Cell(index + 2, 3).SetValue(item.CostSum.ToString("N0"));
                ws.Cell(index + 2, 4).SetValue(item.BillSum.ToString("N0"));
                ws.Cell(index + 2, 5).SetValue((item.CostSum - item.BillSum).ToString("N0"));
            }

            ws.RangeUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            //var rngTable2 = ws.Range($"B{data.Count + 3}:J{data.Count + 10}");
            //rngTable2.RangeUsed().Style
            //    .Font.SetBold()
            //    .Font.SetFontSize(12)
            //    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Columns().AdjustToContents();

            ws.PageSetup.SetPaperSize(XLPaperSize.A4Paper);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InvestorCustomerReport(long calendarId, long customerId)
        {
            var customer = await _customerRepo.Get(customerId);
            var calendar = await _calendarRepo.Get(calendarId);
            var data = await (from a in db.TurnoverProfile
                              join b in db.Turnover on a.Id equals b.TurnoverProfileId
                              where a.CustomerId.Equals(customerId) && (b.Date >= calendar.StartDate && b.Date <= calendar.EndDate)
                              select new
                              {
                                  a.FullName,
                                  b.Date,
                                  b.Creditor,
                                  b.Description
                              }).OrderBy(a => a.Date).ToListAsync();

            string docTitle = $"گزارش سرمایه گذاران شرکت {customer.Name} در {calendar.Title}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add($"{calendar.Title}");

            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Titr";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell(1, 1).Value = docTitle;
            ws.Cell(2, 1).Value = "ردیف";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "نام و نام خانوادگی";
            ws.Cell(2, 4).Value = "مبلغ";


            for (int index = 0; index < data.Count; index++)
            {
                var pd = new PersianDateTime(data[index].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 3, 1).Value = index + 1;
                ws.Cell(index + 3, 2).Value = pd;
                ws.Cell(index + 3, 3).Value = data[index].FullName;
                ws.Cell(index + 3, 4).Value = data[index].Creditor.ToString("N0");
            }

            ws.Cell(data.Count + 3, 1).Value = "جمع کل";
            ws.Range(data.Count + 3, 1, data.Count + 3, 3).Merge();
            ws.Cell(data.Count + 3, 4).Value = data.Sum(a => a.Creditor).ToString("N0");


            ws.Column("A").Width = 5;
            ws.Column("B").Width = 25;
            ws.Column("C").Width = 27;
            ws.Column("D").Width = 31;

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(data.Count + 3, 4));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(14)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;
            rngTable.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            rngTable.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            ws.CellsUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.PageSetup.SetPageOrientation(XLPageOrientation.Portrait)
                .SetPaperSize(XLPaperSize.A4Paper)
                .Margins.SetTop(0).SetBottom(0).SetRight(0.5).SetLeft(0).SetHeader(0).SetFooter(0);

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}_{new PersianDateTime(DateTime.Now):yyyyMMddHHmmss}.xlsx");
        }
    }
}
