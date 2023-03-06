using ClosedXML.Excel;
using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Repository;
using EtehadBar.MVC.Filters;
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
        private readonly IPaymentRepository _paymentRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IAccountBookRepository _accountBookRepo;
        private readonly IMehrcomParsCategoryRepository _mehrcomParsCategoryRepository;
        private readonly IFreeLoadFactorRepository _freeLoadFactorRepository;

        public ExcelController(
            ICalendarRepository calendarRepository,
            ICostRepository costRepository,
            ICustomerRepository customerRepository,
            ILoadFactorRepository loadFactorRepository,
            IPaymentRepository paymentRepository,
            IVehicleRepository vehicleRepository,
            IAccountBookRepository accountBookRepo,
            IMehrcomParsCategoryRepository mehrcomParsCategoryRepository,
            IFreeLoadFactorRepository freeLoadFactorRepository)
        {
            _calendarRepo = calendarRepository;
            _costRepo = costRepository;
            _customerRepo = customerRepository;
            _loadFactorRepo = loadFactorRepository;
            _paymentRepo = paymentRepository;
            _vehicleRepo = vehicleRepository;
            _accountBookRepo = accountBookRepo;
            _mehrcomParsCategoryRepository = mehrcomParsCategoryRepository;
            _freeLoadFactorRepository = freeLoadFactorRepository;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detailed(long calendarId)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            var cost = await _costRepo.Costs().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            var payment = await _paymentRepo.Payments().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            var income = await _customerRepo.CustomerIncomes().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);

            var loadFactors = new List<GlobalLoadFactorVM>();
            var loadFactorList = await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(calendarId)).Select(a => new GlobalLoadFactorVM
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
                Amount = a.Tonnage.HasValue ? ((a.Tonnage.Value * a.TonnagePrice.Value) + a.Amount) : a.Amount,
                DriverFee = a.Tonnage.HasValue ? ((a.Tonnage.Value * a.DriverTonnagePrice.Value) + a.DriverFee) : a.DriverFee,
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
                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(loadFactors[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).Value = loadFactors[index - 1].LoadNumber;

                if (string.IsNullOrWhiteSpace(loadFactors[index - 1].LoadNumberGov))
                    ws.Cell(index + 2, 4).Value = "---";
                else
                    ws.Cell(index + 2, 4).Value = loadFactors[index - 1].LoadNumberGov;

                ws.Cell(index + 2, 5).Value = loadFactors[index - 1].Origin;
                ws.Cell(index + 2, 6).Value = loadFactors[index - 1].Destination;
                ws.Cell(index + 2, 7).Value = loadFactors[index - 1].Amount.ToString("N0");
                ws.Cell(index + 2, 8).Value = loadFactors[index - 1].DriverFee.ToString("N0");
                //ws.Cell(index + 2, 10).Value = loadFactors[index - 1].VAT;
                ws.Cell(index + 2, 9).Value = loadFactors[index - 1].LoadFactorDeductions;
                ws.Cell(index + 2, 10).Value = loadFactors[index - 1].WithholdingTax;
                ws.Cell(index + 2, 11).Value = loadFactors[index - 1].DriverName;
                ws.Cell(index + 2, 12).Value = loadFactors[index - 1].VehicleType;
                ws.Cell(index + 2, 13).Value = loadFactors[index - 1].CustomerName;
            }

            ws.Cell($"B{loadFactors.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{loadFactors.Count + 3}:L{loadFactors.Count + 3}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 3, 13).Value = loadFactors.Count;

            ws.Cell($"B{loadFactors.Count + 4}").Value = "مبلغ بارنامه ها";
            ws.Range($"B{loadFactors.Count + 4}:L{loadFactors.Count + 4}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 4, 13).Value = loadFactors.Sum(a => a.Amount).ToString("N0");

            ws.Cell($"B{loadFactors.Count + 5}").Value = "مبلغ قابل پرداخت به رانندگان";
            ws.Range($"B{loadFactors.Count + 5}:L{loadFactors.Count + 5}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 5, 13).Value = loadFactors.Sum(a => a.DriverFee).ToString("N0");

            ws.Cell($"B{loadFactors.Count + 6}").Value = "حقوق مساعده پرداختی";
            ws.Range($"B{loadFactors.Count + 6}:L{loadFactors.Count + 6}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 6, 13).Value = payment.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 7}").Value = "هزینه های جاری";
            ws.Range($"B{loadFactors.Count + 7}:L{loadFactors.Count + 7}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 7, 13).Value = cost.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 8}").Value = "کل هزینه ها";
            ws.Range($"B{loadFactors.Count + 8}:L{loadFactors.Count + 8}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 8, 13).Value = (payment + cost).ToString("N0");

            ws.Cell($"B{loadFactors.Count + 9}").Value = "حقوق مساعده پرداختی";
            ws.Range($"B{loadFactors.Count + 9}:L{loadFactors.Count + 9}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 9, 13).Value = payment.ToString("N0");

            //ws.Cell($"B{loadFactors.Count + 10}").Value = "جمع کل دریافتی";
            //ws.Range($"B{loadFactors.Count + 10}:O{loadFactors.Count + 10}").Row(1).Merge();
            //ws.Cell(loadFactors.Count + 10, 16).Value = income.ToString("N0");

            ws.RangeUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:M{loadFactors.Count + 9}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.Columns().AdjustToContents();

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Cost(long calendarId)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            var costs = await _costRepo.Costs().Where(a => a.CalendarId.Equals(calendarId)).OrderBy(a => a.Date).ToListAsync();

            string docTitle = $"گزارش هزینه های جاری در {calendar.Title}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش هزینه های جاری");
            ws.RightToLeft = true;

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "توضیحات";
            ws.Cell(2, 4).Value = "مبلغ";
            ws.Cell(2, 5).Value = "کاربر سیستم";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(costs.Count + 2, 5));
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

            for (int index = 1; index <= costs.Count; index++)
            {
                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(costs[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).Value = costs[index - 1].Description;
                ws.Cell(index + 2, 4).Value = costs[index - 1].Amount.ToString("N0");
                ws.Cell(index + 2, 5).Value = $"{costs[index - 1].ApplicationUser.Firstname} {costs[index - 1].ApplicationUser.Lastname}";
            }

            ws.Cell($"B{costs.Count + 3}").Value = "تعداد کل";
            ws.Range($"B{costs.Count + 3}:D{costs.Count + 3}").Row(1).Merge();
            ws.Cell(costs.Count + 3, 5).Value = costs.Count;

            ws.Cell($"B{costs.Count + 4}").Value = "جمع کل";
            ws.Range($"B{costs.Count + 4}:D{costs.Count + 4}").Row(1).Merge();
            ws.Cell(costs.Count + 4, 5).Value = costs.Sum(a => a.Amount).ToString("N0");

            var rngTable2 = ws.Range($"B{costs.Count + 3}:E{costs.Count + 4}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.Columns().AdjustToContents();

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Payment(long calendarId, byte? type, long vehicleId)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            var payments = await _paymentRepo.PaymentVMList(calendarId, type, vehicleId);

            string docTitle = $"گزارش حقوق و مساعده پرداختی در {calendar.Title}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("حقوق و مساعده پرداختی");
            ws.RightToLeft = true;

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "نوع";
            ws.Cell(2, 4).Value = "توضیحات";
            ws.Cell(2, 5).Value = "مبلغ";
            ws.Cell(2, 6).Value = "کاربر سیستم";
            ws.Cell(2, 7).Value = "خودرو/کارمند";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(payments.Count + 2, 7));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 7)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= payments.Count; index++)
            {
                string typeStr = payments[index - 1].PaymentType switch
                {
                    PaymentType.AdvanceMoney => "مساعده",
                    PaymentType.Salary => "حقوق",
                    _ => ""
                };

                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(payments[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).Value = typeStr;
                ws.Cell(index + 2, 4).Value = payments[index - 1].Description;
                ws.Cell(index + 2, 5).Value = payments[index - 1].Amount.ToString("N0");
                ws.Cell(index + 2, 6).Value = payments[index - 1].AdminName;
                if (payments[index - 1].VehicleId.HasValue)
                    ws.Cell(index + 2, 7).Value = payments[index - 1].Vehicle;
                else
                    ws.Cell(index + 2, 7).Value = payments[index - 1].UserFullname;
            }

            ws.Cell($"B{payments.Count + 3}").Value = "تعداد مساعده پرداختی";
            ws.Range($"B{payments.Count + 3}:D{payments.Count + 3}").Row(1).Merge();
            ws.Cell(payments.Count + 3, 7).Value = payments.Count(a => a.PaymentType.Equals((byte)PaymentType.AdvanceMoney));

            ws.Cell($"B{payments.Count + 4}").Value = "تعداد حقوق پرداختی";
            ws.Range($"B{payments.Count + 4}:D{payments.Count + 4}").Row(1).Merge();
            ws.Cell(payments.Count + 4, 7).Value = payments.Count(a => a.PaymentType.Equals((byte)PaymentType.Salary));

            ws.Cell($"B{payments.Count + 5}").Value = "تعداد کل";
            ws.Range($"B{payments.Count + 5}:D{payments.Count + 5}").Row(1).Merge();
            ws.Cell(payments.Count + 5, 7).Value = payments.Count;

            ws.Cell($"B{payments.Count + 6}").Value = "جمع کل مبلغ مساعده پرداختی";
            ws.Range($"B{payments.Count + 6}:D{payments.Count + 6}").Row(1).Merge();
            ws.Cell(payments.Count + 6, 7).Value = payments.Where(a => a.PaymentType.Equals((byte)PaymentType.AdvanceMoney)).Sum(a => a.Amount).ToString("N0");

            ws.Cell($"B{payments.Count + 7}").Value = "جمع کل مبلغ حقوق پرداختی";
            ws.Range($"B{payments.Count + 7}:D{payments.Count + 7}").Row(1).Merge();
            ws.Cell(payments.Count + 7, 7).Value = payments.Where(a => a.PaymentType.Equals((byte)PaymentType.Salary)).Sum(a => a.Amount).ToString("N0");

            ws.Cell($"B{payments.Count + 8}").Value = "جمع کل پرداختی";
            ws.Range($"B{payments.Count + 8}:F{payments.Count + 8}").Row(1).Merge();
            ws.Cell(payments.Count + 8, 7).Value = payments.Sum(a => a.Amount).ToString("N0");

            var rngTable2 = ws.Range($"B{payments.Count + 3}:G{payments.Count + 8}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.Columns().AdjustToContents();

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        public async Task<IActionResult> VehicleLoadFactor(long calendarId, long vehicleId)
        {
            var vehicle = await _vehicleRepo.Get(vehicleId);
            var calendar = await _calendarRepo.Get(calendarId);
            var payment = await _paymentRepo.Payments().AsNoTracking().Where(a => a.VehicleId.Equals(vehicleId)).SumAsync(a => a.Amount);
            var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId)).OrderBy(a => a.Id).ToListAsync();

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
            ws.Cell(2, 9).Value = "راننده";
            ws.Cell(2, 10).Value = "مشتری - شماره قرارداد";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(loadFactors.Count + 2, 10));
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
                ws.Cell(index + 2, 9).Value = $"{loadFactors[index - 1].Driver.Fullname}";
                ws.Cell(index + 2, 10).Value = $"{loadFactors[index - 1].Contract.Customer.Name} {loadFactors[index - 1].Contract.Number}";
            }

            ws.Cell($"B{loadFactors.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{loadFactors.Count + 3}:I{loadFactors.Count + 3}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 3, 10).Value = loadFactors.Count;

            ws.Cell($"B{loadFactors.Count + 4}").Value = "جمع کرایه عملکرد";
            ws.Range($"B{loadFactors.Count + 4}:I{loadFactors.Count + 4}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 4, 10).Value = loadFactors.Sum(a => a.DriverFee).ToString("N0");

            ws.Cell($"B{loadFactors.Count + 5}").Value = "جمع پرداختی (حقوق و مساعده)";
            ws.Range($"B{loadFactors.Count + 5}:I{loadFactors.Count + 5}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 5, 10).Value = payment.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 6}").Value = "مجموع قابل پرداخت";
            ws.Range($"B{loadFactors.Count + 6}:I{loadFactors.Count + 6}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 6, 10).Value = (loadFactors.Sum(a => a.DriverFee) - payment).ToString("N0");

            var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:J{loadFactors.Count + 6}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.Columns().AdjustToContents();

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
        }

        public async Task<IActionResult> VehicleActivity(long calendarId, long vehicleId)
        {
            var vehicle = await _vehicleRepo.Get(vehicleId);
            var calendar = await _calendarRepo.Get(calendarId);
            var payment = await _paymentRepo.Payments().AsNoTracking().Where(a => a.VehicleId.Equals(vehicleId)).SumAsync(a => a.Amount);
            var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId)).OrderBy(a => a.Id).ToListAsync();

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
            ws.Style.Font.FontName = "B Nazanin";
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

            double driverFeeTotal = loadFactors.Sum(a => a.DriverFee);

            if (loadFactors.Any(a => a.Tonnage.HasValue))
            {
                ws.Cell($"B{routes.Count + 4}").Value = "جمع کل اضافه تناژ";
                ws.Range($"B{routes.Count + 4}:D{routes.Count + 4}").Row(1).Merge();
                ws.Cell(routes.Count + 4, 5).Value = loadFactors.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value);

                ws.Cell($"B{routes.Count + 5}").Value = "جمع کل مبلغ اضافه تناژ";
                ws.Range($"B{routes.Count + 5}:D{routes.Count + 5}").Row(1).Merge();
                ws.Cell(routes.Count + 5, 5).Value = loadFactors.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.DriverTonnagePrice.Value).ToString("N0");

                ws.Cell($"B{routes.Count + 6}").Value = "جمع کل مبلغ بارنامه ها";
                ws.Range($"B{routes.Count + 6}:D{routes.Count + 6}").Row(1).Merge();
                ws.Cell(routes.Count + 6, 5).Value = driverFeeTotal.ToString("N0");

                ws.Cell($"B{routes.Count + 7}").Value = "جمع کرایه عملکرد";
                ws.Range($"B{routes.Count + 7}:D{routes.Count + 7}").Row(1).Merge();

                foreach (var item in loadFactors.Where(a => a.Tonnage.HasValue))
                    driverFeeTotal += item.Tonnage.Value * item.DriverTonnagePrice.Value;

                ws.Cell(routes.Count + 7, 5).Value = driverFeeTotal.ToString("N0");

                ws.Cell($"B{routes.Count + 8}").Value = "جمع پرداختی (حقوق و مساعده)";
                ws.Range($"B{routes.Count + 8}:D{routes.Count + 8}").Row(1).Merge();
                ws.Cell(routes.Count + 8, 5).Value = payment.ToString("N0");

                ws.Cell($"B{routes.Count + 9}").Value = "مجموع قابل پرداخت";
                ws.Range($"B{routes.Count + 9}:D{routes.Count + 9}").Row(1).Merge();
                ws.Cell(routes.Count + 9, 5).Value = (driverFeeTotal - payment).ToString("N0");

                var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:E{loadFactors.Count + 9}");
                rngTable2.RangeUsed().Style
                    .Font.SetBold()
                    .Font.SetFontSize(12);
            }
            else
            {
                ws.Cell($"B{routes.Count + 4}").Value = "جمع کرایه عملکرد";
                ws.Range($"B{routes.Count + 4}:D{routes.Count + 4}").Row(1).Merge();
                ws.Cell(routes.Count + 4, 5).Value = driverFeeTotal.ToString("N0");

                ws.Cell($"B{routes.Count + 5}").Value = "جمع پرداختی (حقوق و مساعده)";
                ws.Range($"B{routes.Count + 5}:D{routes.Count + 5}").Row(1).Merge();
                ws.Cell(routes.Count + 5, 5).Value = payment.ToString("N0");

                ws.Cell($"B{routes.Count + 6}").Value = "مجموع قابل پرداخت";
                ws.Range($"B{routes.Count + 6}:D{routes.Count + 6}").Row(1).Merge();
                ws.Cell(routes.Count + 6, 5).Value = (driverFeeTotal - payment).ToString("N0");

                var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:E{loadFactors.Count + 6}");
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CustomerIncome(long? id, long calendarId)
        {
            if (!id.HasValue)
                return BadRequest("parameter error");

            if (!await _customerRepo.Customers().AnyAsync(a => a.Id.Equals(id.Value)))
                return NotFound("مشتری پیدا نشد");

            var customer = await _customerRepo.Get(id.Value);
            var calendar = await _calendarRepo.Get(calendarId);
            var incomes = await _customerRepo.CustomerIncomes().AsNoTracking().Where(a => a.CalendarId.Equals(calendarId) && a.CustomerId.Equals(id.Value)).OrderBy(a => a.Date).ToListAsync();

            string docTitle = $"گزارش دریافتی های {customer.Name}";

            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Sheet1");
            ws.RightToLeft = true;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell("A1").Value = "ردیف";
            ws.Cell("B1").Value = "تاریخ";
            ws.Cell("C1").Value = "شرح";
            ws.Cell("D1").Value = "مبلغ";

            ws.Range("A1:D1").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                .Font.SetBold(true)
                .Font.SetFontSize(12);

            for (int index = 1; index <= incomes.Count; index++)
            {
                ws.Cell(index + 1, 1).Value = index;
                ws.Cell(index + 1, 2).Value = new PersianDateTime(incomes[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 1, 3).Value = incomes[index - 1].Description;
                ws.Cell(index + 1, 4).Value = incomes[index - 1].Amount.ToString("N0");
            }

            ws.Cell($"A{incomes.Count + 1}").Value = "جمع";
            ws.Range($"A{incomes.Count + 1}:C{incomes.Count + 1}").Row(1).Merge();
            ws.Cell($"D{incomes.Count + 1}").Value = incomes.Sum(a => a.Amount).ToString("N0");
            ws.Range($"A{incomes.Count + 1}:D{incomes.Count + 1}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                .Font.SetBold(true);

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
                ("R", 18)
            };

            if (customer.CustomerType.Equals(CustomerType.SaipaPlasco))
            {
                using var workbook = new XLWorkbook();
                decimal c = Convert.ToDecimal(allLoadFactors.Count / 20f);
                double totalAmount = 0;
                double totalDriverFee = 0;
                for (int i = 1; i <= Convert.ToInt32(Math.Ceiling(c)); i++)
                {
                    var loadFactors = allLoadFactors.Skip((i - 1) * 20).Take(i * 20).ToList();

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
                        rngHeader.Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                        rngHeader.Style.Border.InsideBorderColor = XLColor.Black;

                        rngHeader.LastRow().Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                        rngHeader.LastRow().Style.Border.BottomBorderColor = XLColor.Black;

                        rngHeader.LastColumn().Style.Border.RightBorder = XLBorderStyleValues.Medium;
                        rngHeader.LastColumn().Style.Border.RightBorderColor = XLColor.Black;

                        ws.Range($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}3").Style.Fill.SetBackgroundColor(XLColor.White);
                    }
                    else
                    {
                        var rngHeader = ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}2");
                        rngHeader.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                        rngHeader.Style.Font.SetFontSize(11);

                        rngHeader.Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                        rngHeader.Style.Border.InsideBorderColor = XLColor.Black;

                        rngHeader.LastRow().Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                        rngHeader.LastRow().Style.Border.BottomBorderColor = XLColor.Black;

                        rngHeader.LastColumn().Style.Border.RightBorder = XLBorderStyleValues.Medium;
                        rngHeader.LastColumn().Style.Border.RightBorderColor = XLColor.Black;
                    }

                    int rowIndex = 2;
                    if (i != 1) rowIndex = 3;

                    for (int index = 1; index <= loadFactors.Count; index++)
                    {
                        ws.Cell(index + rowIndex, 1).Value = ((20 * i) + (index - 1)) - (20 - 1);
                        ws.Cell(index + rowIndex, 2).Value = new PersianDateTime(loadFactors[index - 1].Date).ToString("yyyy/MM/dd");
                        ws.Cell(index + rowIndex, 3).Value = loadFactors[index - 1].Driver.Fullname;
                        ws.Cell(index + rowIndex, 4).Value = $"{loadFactors[index - 1].Vehicle.RightNumber} {loadFactors[index - 1].Vehicle.NumberWord} {loadFactors[index - 1].Vehicle.LeftNumber}";
                        if (string.IsNullOrWhiteSpace(loadFactors[index - 1].LoadNumberGov))
                        {
                            ws.Cell(index + rowIndex, 5).Value = loadFactors[index - 1].LoadNumber;
                            ws.Cell(index + rowIndex, 6).Value = "---";
                        }
                        else
                        {
                            ws.Cell(index + rowIndex, 5).Value = loadFactors[index - 1].LoadNumberGov;
                            ws.Cell(index + rowIndex, 6).Value = loadFactors[index - 1].LoadNumber;
                        }
                        ws.Cell(index + rowIndex, 7).Value = loadFactors[index - 1].ExitNumber;
                        ws.Cell(index + rowIndex, 8).Value = loadFactors[index - 1].Origin.Title;
                        ws.Cell(index + rowIndex, 9).Value = loadFactors[index - 1].Destination.Title;
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

                        ws.Cell(index + rowIndex, switchCounter).Value = loadFactors[index - 1].Vehicle.Type;
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
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}{loadFactors.Count + rowIndex + 1}").Value = totalAmount.ToString("N0");
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{loadFactors.Count + rowIndex + 1}").Value = totalDriverFee.ToString("N0");
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black).Border.SetLeftBorder(XLBorderStyleValues.None);

                                ws.Cell($"A{loadFactors.Count + rowIndex + 2}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                                ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge();
                                break;
                            case ExcelExportType.OnlyReceivingPrice:
                                ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "جمع";
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}{loadFactors.Count + rowIndex}").Row(1).Merge();
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{loadFactors.Count + rowIndex + 1}").Value = totalAmount.ToString("N0");
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black).Border.SetLeftBorder(XLBorderStyleValues.None);

                                ws.Cell($"A{loadFactors.Count + rowIndex + 2}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                                ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge();
                                break;
                            case ExcelExportType.OnlyDriverPrice:
                                ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "جمع";
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}{loadFactors.Count + rowIndex}").Row(1).Merge();
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{loadFactors.Count + rowIndex + 1}").Value = totalDriverFee.ToString("N0");
                                ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black).Border.SetLeftBorder(XLBorderStyleValues.None);

                                ws.Cell($"A{loadFactors.Count + rowIndex + 2}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                                ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge();
                                break;
                            case ExcelExportType.WithoutPrice:

                                ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                                ws.Range($"A{loadFactors.Count + rowIndex + 1}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "جمع";
                        ws.Range($"A{loadFactors.Count + rowIndex + 1}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}{loadFactors.Count + rowIndex}").Row(1).Merge();
                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{loadFactors.Count + rowIndex + 1}").Value = totalAmount.ToString("N0");
                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black).Border.SetLeftBorder(XLBorderStyleValues.None);

                        ws.Cell($"A{loadFactors.Count + rowIndex + 2}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                        ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 1}").Row(1).Merge();
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

                    ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 2}").Style.Border.InsideBorder = XLBorderStyleValues.None;
                    ws.Range($"A{loadFactors.Count + rowIndex + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{loadFactors.Count + rowIndex + 2}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Columns().AdjustToContents();
                    ws.Column(8).Width = 17;
                    ws.Column(9).Width = 17;
                    ws.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.CellsUsed().Style.Font.Bold = true;
                    ws.CellsUsed().Style.Font.FontColor = XLColor.Black;
                    ws.RowsUsed().Height = 25;
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

                ws.Cell(1, 1).Value = "ردیف";
                ws.Cell(1, 2).SetValue("کد علت صدور").Style.Font.SetFontSize(7);
                ws.Cell(1, 3).Value = "معین";
                ws.Cell(1, 4).Value = "ماهیت";
                ws.Cell(1, 5).Value = "پلاک";
                ws.Cell(1, 6).Value = "شماره بارنامه";
                ws.Cell(1, 7).Value = "نوع خودرو";
                ws.Cell(1, 8).Value = "روز";
                ws.Cell(1, 9).Value = "ماه";
                ws.Cell(1, 10).Value = "سال";
                ws.Cell(1, 11).Value = "شرح سند";
                ws.Cell(1, 12).Value = "تعداد";
                ws.Cell(1, 13).Value = "تفضیلی مرکز هزینه";

                int switchCounter = 13;
                if (exportType.HasValue)
                {
                    switch (exportType.Value)
                    {
                        case ExcelExportType.WithAllPrices:
                            ws.Cell(1, 14).Value = "نرخ دریافتی";
                            ws.Cell(1, 15).Value = "نرخ پرداختی";
                            switchCounter += 2;
                            break;
                        case ExcelExportType.OnlyReceivingPrice:
                            ws.Cell(1, 14).Value = "مبلغ";
                            switchCounter++;
                            break;
                        case ExcelExportType.OnlyDriverPrice:
                            ws.Cell(1, 14).Value = "مبلغ";
                            switchCounter++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ws.Cell(1, 14).Value = "مبلغ";
                    switchCounter++;
                }

                switchCounter++;
                ws.Cell(1, switchCounter).Value = "شماره درخواست";
                switchCounter++;
                ws.Cell(1, switchCounter).Value = "راننده";
                switchCounter++;
                ws.Cell(1, switchCounter).Value = "پلاک";

                ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}1").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                    .Font.SetBold(true);

                for (int index = 1; index <= allLoadFactors.Count; index++)
                {
                    var pd = new PersianDateTime(allLoadFactors[index - 1].Date);

                    ws.Cell(index + 1, 1).Value = index;
                    ws.Cell(index + 1, 2).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.RegisterCode;
                    ws.Cell(index + 1, 3).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.Certain;
                    ws.Cell(index + 1, 4).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.Nature;
                    ws.Cell(index + 1, 5).Value = "*";
                    ws.Cell(index + 1, 6).Value = allLoadFactors[index - 1].LoadNumber;
                    ws.Cell(index + 1, 7).Value = allLoadFactors[index - 1].Vehicle.Type;
                    ws.Cell(index + 1, 8).Value = pd.Day;
                    ws.Cell(index + 1, 9).Value = pd.Month;
                    ws.Cell(index + 1, 10).Value = pd.Year;
                    ws.Cell(index + 1, 11).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.Description;
                    ws.Cell(index + 1, 12).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.Count;
                    ws.Cell(index + 1, 13).Value = allLoadFactors[index - 1].SazehGostarLoadFactor.DetailedCostCenter;
                    if (exportType.HasValue)
                    {
                        switch (exportType.Value)
                        {
                            case ExcelExportType.WithAllPrices:
                                ws.Cell(index + 1, 14).Value = allLoadFactors[index - 1].Amount.ToString("N0");
                                ws.Cell(index + 1, 15).Value = allLoadFactors[index - 1].DriverFee.ToString("N0");
                                break;
                            case ExcelExportType.OnlyReceivingPrice:
                                ws.Cell(index + 1, 14).Value = allLoadFactors[index - 1].Amount.ToString("N0");
                                break;
                            case ExcelExportType.OnlyDriverPrice:
                                ws.Cell(index + 1, 14).Value = allLoadFactors[index - 1].DriverFee.ToString("N0");
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        ws.Cell(index + 1, 14).Value = allLoadFactors[index - 1].Amount.ToString("N0");

                    ws.Cell(index + 1, switchCounter - 2).Value = allLoadFactors[index - 1].ExitNumber;
                    ws.Cell(index + 1, switchCounter - 1).Value = allLoadFactors[index - 1].Driver.Fullname;
                    ws.Cell(index + 1, switchCounter).Value = $"{allLoadFactors[index - 1].Vehicle.RightNumber} {allLoadFactors[index - 1].Vehicle.NumberWord} {allLoadFactors[index - 1].Vehicle.LeftNumber}";
                }

                ws.Cell($"A{allLoadFactors.Count + 2}").Value = "جمع کل بارنامه ها";
                ws.Range($"A{allLoadFactors.Count + 2}:K{allLoadFactors.Count + 2}").Row(1).Merge();
                ws.Cell($"L{allLoadFactors.Count + 2}").Value = "1";
                ws.Cell($"M{allLoadFactors.Count + 2}").Value = "800720";
                if (exportType.HasValue)
                {
                    switch (exportType.Value)
                    {
                        case ExcelExportType.WithAllPrices:
                            ws.Cell($"N{allLoadFactors.Count + 2}").Value = allLoadFactors.Sum(a => a.Amount).ToString("N0");
                            ws.Cell($"O{allLoadFactors.Count + 2}").Value = allLoadFactors.Sum(a => a.DriverFee).ToString("N0");
                            ws.Range($"P{allLoadFactors.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 2}").Row(1).Merge();
                            break;
                        case ExcelExportType.OnlyReceivingPrice:
                            ws.Cell($"N{allLoadFactors.Count + 2}").Value = allLoadFactors.Sum(a => a.Amount).ToString("N0");
                            ws.Range($"O{allLoadFactors.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 2}").Row(1).Merge();
                            break;
                        case ExcelExportType.OnlyDriverPrice:
                            ws.Cell($"N{allLoadFactors.Count + 2}").Value = allLoadFactors.Sum(a => a.DriverFee).ToString("N0");
                            ws.Range($"O{allLoadFactors.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 2}").Row(1).Merge();
                            break;
                        case ExcelExportType.WithoutPrice:
                            ws.Range($"N{allLoadFactors.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 2}").Row(1).Merge();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ws.Cell($"N{allLoadFactors.Count + 2}").Value = allLoadFactors.Sum(a => a.Amount).ToString("N0");
                    ws.Range($"O{allLoadFactors.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 2}").Row(1).Merge();
                }
                ws.Range($"A{allLoadFactors.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{allLoadFactors.Count + 2}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                    .Font.SetBold(true);

                if (exportType.HasValue)
                {
                    if (exportType.Value == ExcelExportType.WithAllPrices || exportType.Value == ExcelExportType.OnlyReceivingPrice)
                    {
                        ws.Cell($"A{allLoadFactors.Count + 3}").Value = allLoadFactors.Count + 1;
                        ws.Cell($"B{allLoadFactors.Count + 3}").Value = "906";
                        ws.Cell($"C{allLoadFactors.Count + 3}").Value = "1452";
                        ws.Cell($"D{allLoadFactors.Count + 3}").Value = "1";
                        ws.Cell($"E{allLoadFactors.Count + 3}").Value = "*";
                        ws.Cell($"F{allLoadFactors.Count + 3}").Value = "97001";
                        ws.Cell($"H{allLoadFactors.Count + 3}").Value = "0";
                        ws.Cell($"I{allLoadFactors.Count + 3}").Value = "0";
                        ws.Cell($"J{allLoadFactors.Count + 3}").Value = "0";
                        ws.Cell($"L{allLoadFactors.Count + 3}").Value = "0";

                        ws.Cell($"A{allLoadFactors.Count + 4}").Value = allLoadFactors.Count + 2;
                        ws.Cell($"B{allLoadFactors.Count + 4}").Value = "907";
                        ws.Cell($"C{allLoadFactors.Count + 4}").Value = "1453";
                        ws.Cell($"D{allLoadFactors.Count + 4}").Value = "1";
                        ws.Cell($"E{allLoadFactors.Count + 4}").Value = "*";
                        ws.Cell($"F{allLoadFactors.Count + 4}").Value = "97001";
                        ws.Cell($"H{allLoadFactors.Count + 4}").Value = "0";
                        ws.Cell($"I{allLoadFactors.Count + 4}").Value = "0";
                        ws.Cell($"J{allLoadFactors.Count + 4}").Value = "0";
                        ws.Cell($"L{allLoadFactors.Count + 4}").Value = "0";

                        ws.Cell($"A{allLoadFactors.Count + 5}").Value = allLoadFactors.Count + 3;
                        ws.Cell($"B{allLoadFactors.Count + 5}").Value = "472";
                        ws.Cell($"C{allLoadFactors.Count + 5}").Value = "3427";
                        ws.Cell($"D{allLoadFactors.Count + 5}").Value = "1";
                        ws.Cell($"E{allLoadFactors.Count + 5}").Value = "*";
                        ws.Cell($"F{allLoadFactors.Count + 5}").Value = "97001";
                        ws.Cell($"H{allLoadFactors.Count + 5}").Value = "0";
                        ws.Cell($"I{allLoadFactors.Count + 5}").Value = "0";
                        ws.Cell($"J{allLoadFactors.Count + 5}").Value = "0";
                        ws.Cell($"K{allLoadFactors.Count + 5}").Value = $"بیمه 7.8% خلاصه 2706/477 اتحاد بار {allLoadFactors.Count} بارنامه";
                        ws.Cell($"L{allLoadFactors.Count + 5}").Value = "0";
                        ws.Cell($"M{allLoadFactors.Count + 5}").Value = ((allLoadFactors.Sum(a => a.Amount) * 7.8) / 100).ToString("N0");

                        ws.Cell($"A{allLoadFactors.Count + 6}").Value = allLoadFactors.Count + 4;
                        ws.Cell($"B{allLoadFactors.Count + 6}").Value = "080";
                        ws.Cell($"C{allLoadFactors.Count + 6}").Value = "3442";
                        ws.Cell($"D{allLoadFactors.Count + 6}").Value = "1";
                        ws.Cell($"E{allLoadFactors.Count + 6}").Value = "*";
                        ws.Cell($"F{allLoadFactors.Count + 6}").Value = "97001";
                        ws.Cell($"H{allLoadFactors.Count + 6}").Value = "0";
                        ws.Cell($"I{allLoadFactors.Count + 6}").Value = "0";
                        ws.Cell($"J{allLoadFactors.Count + 6}").Value = "0";
                        ws.Cell($"K{allLoadFactors.Count + 6}").Value = $"خالص پرداختی خلاصه 2706/477 {allLoadFactors.Count} بارنامه";
                        ws.Cell($"L{allLoadFactors.Count + 6}").Value = "0";
                        ws.Cell($"M{allLoadFactors.Count + 6}").Value = (allLoadFactors.Sum(a => a.Amount) - ((allLoadFactors.Sum(a => a.Amount) * 7.8) / 100)).ToString("N0");
                    }
                }
                else
                {
                    ws.Cell($"A{allLoadFactors.Count + 3}").Value = allLoadFactors.Count + 1;
                    ws.Cell($"B{allLoadFactors.Count + 3}").Value = "906";
                    ws.Cell($"C{allLoadFactors.Count + 3}").Value = "1452";
                    ws.Cell($"D{allLoadFactors.Count + 3}").Value = "1";
                    ws.Cell($"E{allLoadFactors.Count + 3}").Value = "*";
                    ws.Cell($"F{allLoadFactors.Count + 3}").Value = "97001";
                    ws.Cell($"H{allLoadFactors.Count + 3}").Value = "0";
                    ws.Cell($"I{allLoadFactors.Count + 3}").Value = "0";
                    ws.Cell($"J{allLoadFactors.Count + 3}").Value = "0";
                    ws.Cell($"L{allLoadFactors.Count + 3}").Value = "0";

                    ws.Cell($"A{allLoadFactors.Count + 4}").Value = allLoadFactors.Count + 2;
                    ws.Cell($"B{allLoadFactors.Count + 4}").Value = "907";
                    ws.Cell($"C{allLoadFactors.Count + 4}").Value = "1453";
                    ws.Cell($"D{allLoadFactors.Count + 4}").Value = "1";
                    ws.Cell($"E{allLoadFactors.Count + 4}").Value = "*";
                    ws.Cell($"F{allLoadFactors.Count + 4}").Value = "97001";
                    ws.Cell($"H{allLoadFactors.Count + 4}").Value = "0";
                    ws.Cell($"I{allLoadFactors.Count + 4}").Value = "0";
                    ws.Cell($"J{allLoadFactors.Count + 4}").Value = "0";
                    ws.Cell($"L{allLoadFactors.Count + 4}").Value = "0";

                    ws.Cell($"A{allLoadFactors.Count + 5}").Value = allLoadFactors.Count + 3;
                    ws.Cell($"B{allLoadFactors.Count + 5}").Value = "472";
                    ws.Cell($"C{allLoadFactors.Count + 5}").Value = "3427";
                    ws.Cell($"D{allLoadFactors.Count + 5}").Value = "1";
                    ws.Cell($"E{allLoadFactors.Count + 5}").Value = "*";
                    ws.Cell($"F{allLoadFactors.Count + 5}").Value = "97001";
                    ws.Cell($"H{allLoadFactors.Count + 5}").Value = "0";
                    ws.Cell($"I{allLoadFactors.Count + 5}").Value = "0";
                    ws.Cell($"J{allLoadFactors.Count + 5}").Value = "0";
                    ws.Cell($"K{allLoadFactors.Count + 5}").Value = $"بیمه 7.8% خلاصه 2706/477 اتحاد بار {allLoadFactors.Count} بارنامه";
                    ws.Cell($"L{allLoadFactors.Count + 5}").Value = "0";
                    ws.Cell($"M{allLoadFactors.Count + 5}").Value = ((allLoadFactors.Sum(a => a.Amount) * 7.8) / 100).ToString("N0");

                    ws.Cell($"A{allLoadFactors.Count + 6}").Value = allLoadFactors.Count + 4;
                    ws.Cell($"B{allLoadFactors.Count + 6}").Value = "080";
                    ws.Cell($"C{allLoadFactors.Count + 6}").Value = "3442";
                    ws.Cell($"D{allLoadFactors.Count + 6}").Value = "1";
                    ws.Cell($"E{allLoadFactors.Count + 6}").Value = "*";
                    ws.Cell($"F{allLoadFactors.Count + 6}").Value = "97001";
                    ws.Cell($"H{allLoadFactors.Count + 6}").Value = "0";
                    ws.Cell($"I{allLoadFactors.Count + 6}").Value = "0";
                    ws.Cell($"J{allLoadFactors.Count + 6}").Value = "0";
                    ws.Cell($"K{allLoadFactors.Count + 6}").Value = $"خالص پرداختی خلاصه 2706/477 {allLoadFactors.Count} بارنامه";
                    ws.Cell($"L{allLoadFactors.Count + 6}").Value = "0";
                    ws.Cell($"M{allLoadFactors.Count + 6}").Value = (allLoadFactors.Sum(a => a.Amount) - ((allLoadFactors.Sum(a => a.Amount) * 7.8) / 100)).ToString("N0");
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

                var oneFloor = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.OneFloor && !a.Tonnage.HasValue).ToList();

                var ws = workbook.Worksheets.Add("یک طبقه");
                MakePressSheet(oneFloor, ws, exportType);

                var twoFloor = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.TwoFloor && !a.Tonnage.HasValue).ToList();
                if (twoFloor.Any())
                {
                    var ws2 = workbook.Worksheets.Add("دو طبقه");
                    MakePressSheet(twoFloor, ws2, exportType);
                }

                var oneFloorWithTonnage = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.OneFloor && a.Tonnage.HasValue).ToList();
                if (oneFloorWithTonnage.Any())
                {
                    var ws2 = workbook.Worksheets.Add("یک طبقه با تناژ اضافه");
                    MakePressSheet(oneFloorWithTonnage, ws2, exportType);
                }

                var twoFloorWithTonnage = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.TwoFloor && a.Tonnage.HasValue).ToList();
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
                    ws.Style.Font.FontName = "B Nazanin";
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

                    for (int i = 0; i < allLoadFactors.Count; i++)
                    {
                        var vehicle = allLoadFactors[i].Vehicle;
                        var carNumber = $"{vehicle.RightNumber} {vehicle.NumberWord} {vehicle.LeftNumber} ایران {vehicle.IranStateNumber}";
                        var date = new PersianDateTime(allLoadFactors[i].Date);

                        #region handling sleep time and weighbridge
                        if (allLoadFactors[i].MehrcomParsLoadFactor.WeighbridgePrice.HasValue)
                            allLoadFactors[i].DriverFee += allLoadFactors[i].MehrcomParsLoadFactor.WeighbridgePrice.Value;

                        if (allLoadFactors[i].MehrcomParsLoadFactor.LoadSleepTime.HasValue)
                        {
                            allLoadFactors[i].DriverFee += allLoadFactors[i].MehrcomParsLoadFactor.DriverLoadSleepPrice.Value;

                            allLoadFactors[i].Amount += allLoadFactors[i].MehrcomParsLoadFactor.LoadSleepPrice.Value;
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
                        ws.Cell($"D{i + 5}").Value = allLoadFactors[i].LoadNumber;
                        ws.Cell($"E{i + 5}").Value = vehicle.Type;
                        ws.Cell($"F{i + 5}").Value = allLoadFactors[i].Driver.Fullname;
                        ws.Cell($"G{i + 5}").Value = carNumber;
                        ws.Cell($"H{i + 5}").Value = allLoadFactors[i].Origin.Title;
                        ws.Cell($"I{i + 5}").Value = allLoadFactors[i].Destination.Title;
                        ws.Cell($"J{i + 5}").Value = allLoadFactors[i].LoadNumberGov;
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

                        ws.Cell(i + 5, switchCounter + 1).Value = date.ToString("MMMM");
                        ws.Cell(i + 5, switchCounter + 2).Value = date.ToString("yyyy");

                        #region handling comment
                        string commentText = "";
                        if (allLoadFactors[i].MehrcomParsLoadFactor.WeighbridgePrice.HasValue)
                            commentText += $"مبلغ باسکول: {allLoadFactors[i].MehrcomParsLoadFactor.WeighbridgePrice.Value.ToString("N0")}";

                        if (allLoadFactors[i].MehrcomParsLoadFactor.LoadSleepTime.HasValue)
                        {
                            if (!string.IsNullOrWhiteSpace(commentText))
                                commentText += " | ";

                            commentText += $"زمان خواب: {allLoadFactors[i].MehrcomParsLoadFactor.LoadSleepTime.Value} | مبلغ خواب: {allLoadFactors[i].MehrcomParsLoadFactor.DriverLoadSleepPrice.Value.ToString("N0")}";
                        }

                        if (allLoadFactors[i].Tonnage.HasValue)
                        {
                            if (!string.IsNullOrWhiteSpace(commentText))
                                commentText += " | ";

                            commentText += $"میزان اضافه تناژ: {allLoadFactors[i].Tonnage.Value} تن | مبلغ اضافه تناژ: {allLoadFactors[i].DriverTonnagePrice.Value.ToString("N0")}";
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
                    ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}1").Merge();
                    ws.Range($"A2:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}2").Merge();
                    ws.Range($"A3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}3").Merge();
                    ws.Cell("A1").Value = "اتحاد بار آسیا";
                    ws.Cell("A2").Value = $"اطلاعات زونکن شماره {accountBook.Number}";
                    ws.Range($"A4:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}4").Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}4").Style.Font.SetFontSize(12);
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
                        var pd = new PersianDateTime(loadFactors.First().Calendar.StartDate);
                        endDate = loadFactors.First().Calendar.EndDate;
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
                        ws.Style.Font.FontName = "B Nazanin";
                        ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                        ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                        var data = allLoadFactors.Where(a => a.MehrcomParsLoadFactor.CategoryId.Equals(category.Id)).OrderBy(a => a.Date).ToList();

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

                        int totalCounter = 5;
                        decimal c = Convert.ToDecimal(data.Count / 30f);

                        for (int index = 1; index <= Convert.ToInt32(Math.Ceiling(c)); index++)
                        {
                            var loadFactors = data.Skip((index - 1) * 30).Take(index * 30).ToList();

                            for (int i = 0; i < loadFactors.Count; i++)
                            {
                                #region handling sleep time and weighbridge
                                if (loadFactors[i].MehrcomParsLoadFactor.WeighbridgePrice.HasValue)
                                    loadFactors[i].DriverFee += loadFactors[i].MehrcomParsLoadFactor.WeighbridgePrice.Value;

                                if (loadFactors[i].MehrcomParsLoadFactor.LoadSleepTime.HasValue)
                                {
                                    loadFactors[i].DriverFee += loadFactors[i].MehrcomParsLoadFactor.DriverLoadSleepPrice.Value;

                                    loadFactors[i].Amount += loadFactors[i].MehrcomParsLoadFactor.LoadSleepPrice.Value;
                                }

                                if (loadFactors[i].Tonnage.HasValue)
                                {
                                    loadFactors[i].DriverFee = loadFactors[i].DriverFee + loadFactors[i].Tonnage.Value * loadFactors[i].DriverTonnagePrice.Value;

                                    loadFactors[i].Amount = loadFactors[i].Amount + loadFactors[i].Tonnage.Value * loadFactors[i].TonnagePrice.Value;
                                }
                                #endregion

                                var vehicle = loadFactors[i].Vehicle;
                                var carNumber = $"{vehicle.RightNumber} {vehicle.NumberWord} {vehicle.LeftNumber} ایران {vehicle.IranStateNumber}";
                                var date = new PersianDateTime(loadFactors[i].Date);
                                ws.Cell($"A{totalCounter}").Value = i + 1;
                                ws.Cell($"B{totalCounter}").Value = loadFactors[i].AccountBook.Number;
                                ws.Cell($"C{totalCounter}").Value = date.ToString("yyyy/MM/dd");
                                ws.Cell($"D{totalCounter}").Value = loadFactors[i].LoadNumber;
                                ws.Cell($"E{totalCounter}").Value = vehicle.Type;
                                ws.Cell($"F{totalCounter}").Value = loadFactors[i].Driver.Fullname;
                                ws.Cell($"G{totalCounter}").Value = carNumber;
                                ws.Cell($"H{totalCounter}").Value = loadFactors[i].Origin.Title;
                                ws.Cell($"I{totalCounter}").Value = loadFactors[i].Destination.Title;
                                ws.Cell($"J{totalCounter}").Value = loadFactors[i].LoadNumberGov;
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

                                ws.Cell(totalCounter, switchCounter + 1).Value = date.ToString("MMMM");
                                ws.Cell(totalCounter, switchCounter + 2).Value = date.ToString("yyyy");


                                #region handling comment
                                string commentText = "";
                                if (loadFactors[i].MehrcomParsLoadFactor.WeighbridgePrice.HasValue)
                                    commentText += $"مبلغ باسکول: {loadFactors[i].MehrcomParsLoadFactor.WeighbridgePrice.Value.ToString("N0")}";

                                if (loadFactors[i].MehrcomParsLoadFactor.LoadSleepTime.HasValue)
                                {
                                    if (!string.IsNullOrWhiteSpace(commentText))
                                        commentText += " | ";

                                    commentText += $"زمان خواب: {loadFactors[i].MehrcomParsLoadFactor.LoadSleepTime.Value} | مبلغ خواب: {loadFactors[i].MehrcomParsLoadFactor.DriverLoadSleepPrice.Value.ToString("N0")}";
                                }

                                if (loadFactors[i].Tonnage.HasValue)
                                {
                                    if (!string.IsNullOrWhiteSpace(commentText))
                                        commentText += " | ";

                                    commentText += $"میزان اضافه تناژ: {loadFactors[i].Tonnage.Value} تن | مبلغ اضافه تناژ: {loadFactors[i].DriverTonnagePrice.Value.ToString("N0")}";
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

                        //making header
                        ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}1").Merge();
                        ws.Range($"A2:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}2").Merge();
                        ws.Range($"A3:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}3").Merge();
                        ws.Cell("A1").Value = "اتحاد بار آسیا";
                        ws.Cell("A2").Value = $"فرم ارسال صورت حساب";
                        ws.Cell("A3").Value = $"صورت حساب خدمات حمل {category.Title}";
                        ws.Range($"A4:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}4").Style.Fill.SetBackgroundColor(XLColor.LightGray);
                        ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}4").Style.Font.SetFontSize(12);

                        var table = ws.Range($"A4:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter + 2)).Letter}{totalCounter - 1}").CreateTable();
                        table.Theme = XLTableTheme.None;
                        table.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        table.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                        ws.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.CellsUsed().Style.Font.Bold = true;
                        ws.CellsUsed().Style.Font.FontColor = XLColor.Black;
                        ws.CellsUsed().Style.Border.BottomBorderColor = XLColor.Black;
                        ws.RowsUsed().Height = 25;
                    }
                }

                await using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}.xlsx");
            }
        }

        private static void MakePressSheet(List<LoadFactor> data, IXLWorksheet ws, ExcelExportType? exportType)
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
                ("Q", 17)
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
            ws.Cell(1, 12).Value = "اضافه تناژ";
            ws.Cell(1, 13).Value = "نرخ اضافه تناژ";

            int switchCounter = 13;
            if (exportType.HasValue)
            {
                switch (exportType.Value)
                {
                    case ExcelExportType.WithAllPrices:
                        ws.Cell(1, 14).Value = "نرخ دریافتی";
                        ws.Cell(1, 15).Value = "نرخ پرداختی";
                        switchCounter += 2;
                        break;
                    case ExcelExportType.OnlyReceivingPrice:
                        ws.Cell(1, 14).Value = "نرخ دریافتی";
                        switchCounter++;
                        break;
                    case ExcelExportType.OnlyDriverPrice:
                        ws.Cell(1, 14).Value = "قابل پرداخت";
                        switchCounter++;
                        break;
                    case ExcelExportType.WithoutPrice:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ws.Cell(1, 14).Value = "قابل پرداخت";
                switchCounter++;
            }

            ws.Range($"A1:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}1").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                .Font.SetBold(true)
                .Font.SetFontSize(12);

            for (int index = 1; index <= data.Count; index++)
            {
                ws.Cell(index + 1, 1).Value = index;
                ws.Cell(index + 1, 2).Value = data[index - 1].LoadNumber;
                ws.Cell(index + 1, 3).Value = $"{data[index - 1].Vehicle.RightNumber} {data[index - 1].Vehicle.NumberWord} {data[index - 1].Vehicle.LeftNumber}";
                ws.Cell(index + 1, 4).Value = data[index - 1].Driver.Fullname;
                ws.Cell(index + 1, 5).Value = data[index - 1].Origin.Title;
                ws.Cell(index + 1, 6).Value = data[index - 1].Destination.Title;
                ws.Cell(index + 1, 7).Value = new PersianDateTime(data[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 1, 8).Value = data[index - 1].SaipaPressLoadFactor.EntryNumber;
                ws.Cell(index + 1, 9).Value = data[index - 1].ExitNumber;
                ws.Cell(index + 1, 10).Value = data[index - 1].Vehicle.Type;
                ws.Cell(index + 1, 11).Value = data[index - 1].SaipaPressLoadFactor.LoadType;
                ws.Cell(index + 1, 12).Value = data[index - 1].Tonnage.HasValue ? data[index - 1].Tonnage.Value : "0";
                if (exportType.HasValue)
                {
                    switch (exportType.Value)
                    {
                        case ExcelExportType.WithAllPrices:
                            ws.Cell(index + 1, 13).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].TonnagePrice.Value * data[index - 1].Tonnage.Value).ToString("N0") : "0";
                            ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].Amount + (data[index - 1].Tonnage.Value * data[index - 1].TonnagePrice.Value)).ToString("N0") : data[index - 1].Amount.ToString("N0");
                            ws.Cell(index + 1, 15).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverFee + (data[index - 1].Tonnage.Value * data[index - 1].DriverTonnagePrice.Value)).ToString("N0") : data[index - 1].DriverFee.ToString("N0");
                            break;
                        case ExcelExportType.OnlyReceivingPrice:
                            ws.Cell(index + 1, 13).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].TonnagePrice.Value * data[index - 1].Tonnage.Value).ToString("N0") : "0";
                            ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].Amount + (data[index - 1].Tonnage.Value * data[index - 1].TonnagePrice.Value)).ToString("N0") : data[index - 1].Amount.ToString("N0");
                            break;
                        case ExcelExportType.OnlyDriverPrice:
                            ws.Cell(index + 1, 13).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverTonnagePrice.Value * data[index - 1].Tonnage.Value).ToString("N0") : "0";
                            ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverFee + (data[index - 1].Tonnage.Value * data[index - 1].DriverTonnagePrice.Value)).ToString("N0") : data[index - 1].DriverFee.ToString("N0");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ws.Cell(index + 1, 13).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverTonnagePrice.Value * data[index - 1].Tonnage.Value).ToString("N0") : "0";
                    ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].DriverFee + (data[index - 1].Tonnage.Value * data[index - 1].DriverTonnagePrice.Value)).ToString("N0") : data[index - 1].DriverFee.ToString("N0");
                }
            }

            if (exportType.HasValue)
            {
                switch (exportType.Value)
                {
                    case ExcelExportType.WithAllPrices:
                        ws.Cell($"A{data.Count + 2}").Value = "جمع";
                        ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 2)).Letter}{data.Count + 2}").Row(1).Merge();

                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{data.Count + 2}").Value =
                            (data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.TonnagePrice.Value)
                            + data.Sum(a => a.Amount)).ToString("N0");

                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Value =
                            (data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.DriverTonnagePrice.Value)
                            + data.Sum(a => a.DriverFee)).ToString("N0");

                        ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                            .Font.SetBold(true);
                        break;
                    case ExcelExportType.OnlyReceivingPrice:
                        ws.Cell($"A{data.Count + 2}").Value = "جمع";
                        ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{data.Count + 2}").Row(1).Merge();

                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Value =
                            (data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.TonnagePrice.Value)
                            + data.Sum(a => a.Amount)).ToString("N0");

                        ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                            .Font.SetBold(true);
                        break;
                    case ExcelExportType.OnlyDriverPrice:
                        ws.Cell($"A{data.Count + 2}").Value = "جمع";
                        ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{data.Count + 2}").Row(1).Merge();

                        ws.Cell($"{EnglishNumbers.Single(a => a.Num.Equals(switchCounter)).Letter}{data.Count + 2}").Value =
                            (data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.DriverTonnagePrice.Value)
                            + data.Sum(a => a.DriverFee)).ToString("N0");

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
                ws.Range($"A{data.Count + 2}:{EnglishNumbers.Single(a => a.Num.Equals(switchCounter - 1)).Letter}{data.Count + 2}").Row(1).Merge();
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
    }
}
