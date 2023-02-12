using ClosedXML.Excel;
using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.MVC.Controllers
{
    public class ExcelController : Controller
    {
        private readonly ICalendarRepository _calendarRepo;
        private readonly ICostRepository _costRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ILoadFactorRepository _loadFactorRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IAccountBookRepository _accountBookRepo;

        public ExcelController(
            ICalendarRepository calendarRepository,
            ICostRepository costRepository,
            ICustomerRepository customerRepository,
            ILoadFactorRepository loadFactorRepository,
            IPaymentRepository paymentRepository,
            IVehicleRepository vehicleRepository,
            IAccountBookRepository accountBookRepo)
        {
            _calendarRepo = calendarRepository;
            _costRepo = costRepository;
            _customerRepo = customerRepository;
            _loadFactorRepo = loadFactorRepository;
            _paymentRepo = paymentRepository;
            _vehicleRepo = vehicleRepository;
            _accountBookRepo = accountBookRepo;
        }

        public async Task<IActionResult> Detailed(long calendarId)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            var cost = await _costRepo.Costs().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            var payment = await _paymentRepo.Payments().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            var income = await _customerRepo.CustomerIncomes().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.CalendarId.Equals(calendarId)).OrderBy(a => a.Date).ToListAsync();

            string docTitle = $"گزارش تفصیلی بارنامه در {calendar.Title}";

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("گزارش تفصیلی بارنامه");
            ws.RightToLeft = true;

            ws.Cell(1, 1).Value = docTitle;

            ws.Cell(2, 1).Value = "#";
            ws.Cell(2, 2).Value = "تاریخ";
            ws.Cell(2, 3).Value = "شماره بارنامه";
            ws.Cell(2, 4).Value = "شماره بارنامه دولتی";
            ws.Cell(2, 5).Value = "شماره خروج";
            ws.Cell(2, 6).Value = "مبدا";
            ws.Cell(2, 7).Value = "مقصد";
            ws.Cell(2, 8).Value = "مبلغ";
            ws.Cell(2, 9).Value = "کرایه راننده";
            ws.Cell(2, 10).Value = "مالیات ارزش افزوده";
            ws.Cell(2, 11).Value = "سپرده بیمه";
            ws.Cell(2, 12).Value = "مالیات تکلیفی";
            ws.Cell(2, 13).Value = "راننده";
            ws.Cell(2, 14).Value = "خودرو";
            ws.Cell(2, 15).Value = "تقویم کاری";
            ws.Cell(2, 16).Value = "مشتری - شماره قرارداد";

            var rngTable = ws.Range(ws.Cell(1, 1), ws.Cell(loadFactors.Count + 2, 16));
            rngTable.FirstRow().Merge();

            rngTable.FirstRow().Style
                .Font.SetBold()
                .Font.SetFontSize(15)
                    .Fill.SetBackgroundColor(XLColor.LightGray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngHeaders = rngTable.Range(rngTable.Cell(2, 1), rngTable.Cell(2, 16)); // The address is relative to rngTable (NOT the worksheet)
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Font.FontColor = XLColor.Black;

            for (int index = 1; index <= loadFactors.Count; index++)
            {
                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(loadFactors[index - 1].Date).ToString("yyyy/MM/dd"); if (string.IsNullOrWhiteSpace(loadFactors[index - 1].LoadNumberGov))
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
                ws.Cell(index + 2, 8).Value = loadFactors[index - 1].Amount.ToString("N0");
                ws.Cell(index + 2, 9).Value = loadFactors[index - 1].DriverFee.ToString("N0");
                ws.Cell(index + 2, 10).Value = loadFactors[index - 1].VAT;
                ws.Cell(index + 2, 11).Value = loadFactors[index - 1].LoadFactorDeductions;
                ws.Cell(index + 2, 12).Value = loadFactors[index - 1].WithholdingTax;
                ws.Cell(index + 2, 13).Value = $"{loadFactors[index - 1].ApplicationUser.Firstname} {loadFactors[index - 1].ApplicationUser.Lastname}";
                ws.Cell(index + 2, 14).Value = loadFactors[index - 1].Vehicle.Type;
                ws.Cell(index + 2, 15).Value = loadFactors[index - 1].Calendar.Title;
                ws.Cell(index + 2, 16).Value = $"{loadFactors[index - 1].Contract.Customer.Name} {loadFactors[index - 1].Contract.Number}";
            }

            ws.Cell($"B{loadFactors.Count + 3}").Value = "تعداد کل بارنامه ها";
            ws.Range($"B{loadFactors.Count + 3}:O{loadFactors.Count + 3}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 3, 16).Value = loadFactors.Count;

            ws.Cell($"B{loadFactors.Count + 4}").Value = "مبلغ بارنامه ها";
            ws.Range($"B{loadFactors.Count + 4}:O{loadFactors.Count + 4}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 4, 16).Value = loadFactors.Sum(a => a.Amount).ToString("N0");

            ws.Cell($"B{loadFactors.Count + 5}").Value = "مبلغ قابل پرداخت به رانندگان";
            ws.Range($"B{loadFactors.Count + 5}:O{loadFactors.Count + 5}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 5, 16).Value = loadFactors.Sum(a => a.DriverFee).ToString("N0");

            ws.Cell($"B{loadFactors.Count + 6}").Value = "حقوق مساعده پرداختی";
            ws.Range($"B{loadFactors.Count + 6}:O{loadFactors.Count + 6}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 6, 16).Value = payment.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 7}").Value = "هزینه های جاری";
            ws.Range($"B{loadFactors.Count + 7}:O{loadFactors.Count + 7}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 7, 16).Value = cost.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 8}").Value = "کل هزینه ها";
            ws.Range($"B{loadFactors.Count + 8}:O{loadFactors.Count + 8}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 8, 16).Value = (payment + cost).ToString("N0");

            ws.Cell($"B{loadFactors.Count + 9}").Value = "حقوق مساعده پرداختی";
            ws.Range($"B{loadFactors.Count + 9}:O{loadFactors.Count + 9}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 9, 16).Value = payment.ToString("N0");

            ws.Cell($"B{loadFactors.Count + 10}").Value = "جمع کل دریافتی";
            ws.Range($"B{loadFactors.Count + 10}:O{loadFactors.Count + 10}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 10, 16).Value = income.ToString("N0");

            var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:P{loadFactors.Count + 10}");
            rngTable2.RangeUsed().Style
                .Font.SetBold()
                .Font.SetFontSize(12);

            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.Columns().AdjustToContents();

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}-parsmvc.xlsx");
        }

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

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}-parsmvc.xlsx");
        }

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

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}-parsmvc.xlsx");
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
                ws.Cell(index + 2, 9).Value = $"{loadFactors[index - 1].ApplicationUser.Firstname} {loadFactors[index - 1].ApplicationUser.Lastname}";
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

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}-parsmvc.xlsx");
        }

        public async Task<IActionResult> Customer(long customerId, long? calendarId, string statusNumber, long? accountBookId)
        {
            var customer = await _customerRepo.Get(customerId);
            if (customer == null) return NotFound("Customer not found");

            var calendar = new Calendar();
            if (calendarId.HasValue)
            {
                calendar = await _calendarRepo.Get(calendarId.Value);
                if (calendar == null) return NotFound("Calendar not found");
            }

            var allLoadFactors = await _loadFactorRepo.LoadFactors(customerId, calendarId, accountBookId);

            string docTitle = $"گزارش بارنامه {customer.Name}";
            if (calendarId.HasValue)
                docTitle += $" در {calendar.Title}";

            if (customer.CustomerType.Equals(CustomerType.SaipaPlasco))
            {
                if (string.IsNullOrWhiteSpace(statusNumber))
                {
                    TempData["msg"] = "عملیات با خطا مواجه شد. لطفا شماره صورت وضعیت را ارسال کنید. |danger";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                using var workbook = new XLWorkbook();
                decimal c = Convert.ToDecimal(allLoadFactors.Count / 20f);
                double totalAmount = 0;
                for (int i = 1; i <= Convert.ToInt32(Math.Ceiling(c)); i++)
                {
                    var loadFactors = allLoadFactors.Skip((i - 1) * 20).Take(i * 20).ToList();

                    var ws = workbook.Worksheets.Add($"Sheet{i}");
                    ws.RightToLeft = true;
                    ws.Style.Font.FontName = "B Nazanin";
                    ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                    ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                    ws.Cell("A1").Value = statusNumber;
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
                    ws.Cell(2, 10).Value = "مبلغ بارنامه (ریال)";
                    ws.Cell(2, 11).Value = "خودرو";

                    if (i != 1)
                    {
                        ws.Cell("A3").Value = "نقل از صفحه قبل";
                        ws.Range("A3:I3").Row(1).Merge();
                        ws.Cell("J3").Value = totalAmount.ToString("N0");

                        var rngHeader = ws.Range("A1:K3");
                        rngHeader.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                        rngHeader.Style.Font.SetFontSize(11);
                        rngHeader.Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                        rngHeader.Style.Border.InsideBorderColor = XLColor.Black;

                        rngHeader.LastRow().Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                        rngHeader.LastRow().Style.Border.BottomBorderColor = XLColor.Black;

                        rngHeader.LastColumn().Style.Border.RightBorder = XLBorderStyleValues.Medium;
                        rngHeader.LastColumn().Style.Border.RightBorderColor = XLColor.Black;

                        ws.Range("J3:K3").Style.Fill.SetBackgroundColor(XLColor.White);
                    }
                    else
                    {
                        var rngHeader = ws.Range("A1:K2");
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
                        ws.Cell(index + rowIndex, 3).Value = loadFactors[index - 1].ApplicationUser.Lastname;
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
                        ws.Cell(index + rowIndex, 10).Value = loadFactors[index - 1].Amount.ToString("N0");
                        ws.Cell(index + rowIndex, 11).Value = loadFactors[index - 1].Vehicle.Type;
                    }

                    totalAmount += loadFactors.Sum(a => a.Amount);

                    ws.Cell($"A{loadFactors.Count + rowIndex + 1}").Value = "جمع";
                    ws.Range($"A{loadFactors.Count + rowIndex + 1}:I{loadFactors.Count + rowIndex}").Row(1).Merge();
                    ws.Cell($"J{loadFactors.Count + rowIndex + 1}").Value = totalAmount.ToString("N0");
                    ws.Cell($"K{loadFactors.Count + rowIndex + 1}").Style.Border.SetRightBorder(XLBorderStyleValues.Thin).Border.SetRightBorderColor(XLColor.Black).Border.SetLeftBorder(XLBorderStyleValues.None);


                    ws.Cell($"A{loadFactors.Count + rowIndex + 2}").Value = "تهیه کننده:                                          تایید:                                         تصویب:                                         رسیدگی امور مالی:";
                    ws.Range($"A{loadFactors.Count + rowIndex + 2}:K{loadFactors.Count + rowIndex + 1}").Row(1).Merge();

                    if (i == 1)
                    {
                        var rngContent = ws.Range(ws.Cell("A3"), ws.Cell($"K{loadFactors.Count + 2}"));
                        rngContent.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetOutsideBorderColor(XLColor.Black);
                    }
                    else
                    {
                        var rngContent = ws.Range(ws.Cell("A4"), ws.Cell($"K{loadFactors.Count + 3}"));
                        rngContent.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .Border.SetOutsideBorderColor(XLColor.Black);
                    }

                    ws.Range($"A{loadFactors.Count + rowIndex + 2}:K{loadFactors.Count + rowIndex + 2}").Style.Border.InsideBorder = XLBorderStyleValues.None;
                    ws.Range($"A{loadFactors.Count + rowIndex + 2}:K{loadFactors.Count + rowIndex + 2}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

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

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}-parsmvc.xlsx");
            }
            else if (customer.CustomerType.Equals(CustomerType.SazehGostar))
            {
                using var workbook = new XLWorkbook();

                var ws = workbook.Worksheets.Add("Sheet1");
                ws.RightToLeft = true;
                ws.Style.Font.FontName = "B Nazanin";
                ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
                ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

                ws.Cell("A1").Value = "ردیف";
                ws.Cell("B1").SetValue("کد علت صدور").Style.Font.SetFontSize(7);
                ws.Cell("C1").Value = "معین";
                ws.Cell("D1").Value = "ماهیت";
                ws.Cell("E1").Value = "پلاک";
                ws.Cell("F1").Value = "شماره بارنامه";
                ws.Cell("G1").Value = "نوع خودرو";
                ws.Cell("H1").Value = "روز";
                ws.Cell("I1").Value = "ماه";
                ws.Cell("J1").Value = "سال";
                ws.Cell("K1").Value = "شرح سند";
                ws.Cell("L1").Value = "تعداد";
                ws.Cell("M1").Value = "تفضیلی مرکز هزینه";
                ws.Cell("N1").Value = "مبلغ";
                ws.Cell("O1").Value = "شماره درخواست";
                ws.Cell("P1").Value = "راننده";
                ws.Cell("Q1").Value = "پلاک";

                ws.Range("A1:Q1").Style.Fill.SetBackgroundColor(XLColor.LightGray)
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
                    ws.Cell(index + 1, 14).Value = allLoadFactors[index - 1].Amount.ToString("N0");
                    ws.Cell(index + 1, 15).Value = allLoadFactors[index - 1].ExitNumber;
                    ws.Cell(index + 1, 16).Value = allLoadFactors[index - 1].ApplicationUser.Lastname;
                    ws.Cell(index + 1, 17).Value = $"{allLoadFactors[index - 1].Vehicle.RightNumber} {allLoadFactors[index - 1].Vehicle.NumberWord} {allLoadFactors[index - 1].Vehicle.LeftNumber}";
                }

                ws.Cell($"A{allLoadFactors.Count + 2}").Value = "جمع کل بارنامه ها";
                ws.Range($"A{allLoadFactors.Count + 2}:K{allLoadFactors.Count + 2}").Row(1).Merge();
                ws.Cell($"L{allLoadFactors.Count + 2}").Value = "1";
                ws.Cell($"M{allLoadFactors.Count + 2}").Value = "800720";
                ws.Cell($"N{allLoadFactors.Count + 2}").Value = allLoadFactors.Sum(a => a.Amount).ToString("N0");
                ws.Range($"O{allLoadFactors.Count + 2}:Q{allLoadFactors.Count + 2}").Row(1).Merge();
                ws.Range($"A{allLoadFactors.Count + 2}:Q{allLoadFactors.Count + 2}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                    .Font.SetBold(true);

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

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}-parsmvc.xlsx");
            }
            else
            {
                using var workbook = new XLWorkbook();

                var oneFloor = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.OneFloor && !a.Tonnage.HasValue).ToList();

                var ws = workbook.Worksheets.Add("یک طبقه");
                MakePressSheet(oneFloor, ws);

                var twoFloor = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.TwoFloor && !a.Tonnage.HasValue).ToList();
                if (twoFloor.Any())
                {
                    var ws2 = workbook.Worksheets.Add("دو طبقه");
                    MakePressSheet(twoFloor, ws2);
                }

                var oneFloorWithTonnage = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.OneFloor && a.Tonnage.HasValue).ToList();
                if (oneFloorWithTonnage.Any())
                {
                    var ws2 = workbook.Worksheets.Add("یک طبقه با تناژ اضافه");
                    MakePressSheet(oneFloorWithTonnage, ws2);
                }

                var twoFloorWithTonnage = allLoadFactors.Where(a => a.SaipaPressLoadFactor.PressFloorType == SaipaPressLoadType.TwoFloor && a.Tonnage.HasValue).ToList();
                if (twoFloorWithTonnage.Any())
                {
                    var ws2 = workbook.Worksheets.Add("دو طبقه با تناژ اضافه");
                    MakePressSheet(twoFloorWithTonnage, ws2);
                }

                await using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}-parsmvc.xlsx");
            }
        }

        private static void MakePressSheet(List<LoadFactor> data, IXLWorksheet ws)
        {
            ws.RightToLeft = true;
            ws.Style.Font.FontName = "B Nazanin";
            ws.Style.Font.FontCharSet = XLFontCharSet.Arabic;
            ws.Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);

            ws.Cell("A1").Value = "ردیف";
            ws.Cell("B1").Value = "بارنامه";
            ws.Cell("C1").Value = "پلاک";
            ws.Cell("D1").Value = "راننده";
            ws.Cell("E1").Value = "مبدا";
            ws.Cell("F1").Value = "مقصد";
            ws.Cell("G1").Value = "تاریخ";
            ws.Cell("H1").Value = "سند ورود";
            ws.Cell("I1").Value = "سند خروج";
            ws.Cell("J1").Value = "نوع خودرو";
            ws.Cell("K1").Value = "نوع بار";
            ws.Cell("L1").Value = "اضافه تناژ";
            ws.Cell("M1").Value = "نرخ اضافه تناژ";
            ws.Cell("N1").Value = "قابل پرداخت";

            ws.Range("A1:N1").Style.Fill.SetBackgroundColor(XLColor.LightGray)
                .Font.SetBold(true)
                .Font.SetFontSize(12);

            for (int index = 1; index <= data.Count; index++)
            {
                ws.Cell(index + 1, 1).Value = index;
                ws.Cell(index + 1, 2).Value = data[index - 1].LoadNumber;
                ws.Cell(index + 1, 3).Value = $"{data[index - 1].Vehicle.RightNumber} {data[index - 1].Vehicle.NumberWord} {data[index - 1].Vehicle.LeftNumber}";
                ws.Cell(index + 1, 4).Value = data[index - 1].ApplicationUser.Lastname;
                ws.Cell(index + 1, 5).Value = data[index - 1].Origin.Title;
                ws.Cell(index + 1, 6).Value = data[index - 1].Destination.Title;
                ws.Cell(index + 1, 7).Value = new PersianDateTime(data[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 1, 8).Value = data[index - 1].SaipaPressLoadFactor.EntryNumber;
                ws.Cell(index + 1, 9).Value = data[index - 1].ExitNumber;
                ws.Cell(index + 1, 10).Value = data[index - 1].Vehicle.Type;
                ws.Cell(index + 1, 11).Value = data[index - 1].SaipaPressLoadFactor.LoadType;
                ws.Cell(index + 1, 12).Value = data[index - 1].Tonnage.HasValue ? data[index - 1].Tonnage.Value : "0";
                ws.Cell(index + 1, 13).Value = data[index - 1].DriverTonnagePrice.HasValue ? data[index - 1].DriverTonnagePrice.Value.ToString("N0") : "0";
                ws.Cell(index + 1, 14).Value = data[index - 1].Tonnage.HasValue ? (data[index - 1].Amount + (data[index - 1].Tonnage.Value * data[index - 1].DriverTonnagePrice.Value)).ToString("N0") : data[index - 1].Amount.ToString("N0");
            }

            ws.Cell($"A{data.Count + 2}").Value = "جمع";
            ws.Range($"A{data.Count + 2}:M{data.Count + 2}").Row(1).Merge();
            ws.Cell($"N{data.Count + 2}").Value =
                (data.Where(a => a.Tonnage.HasValue).Sum(a => a.Tonnage.Value * a.DriverTonnagePrice.Value)
                + data.Sum(a => a.Amount)).ToString("N0");
            ws.Range($"A{data.Count + 2}:N{data.Count + 2}").Style.Fill.SetBackgroundColor(XLColor.LightGray)
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
        }

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

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}-parsmvc.xlsx");
        }
    }
}
