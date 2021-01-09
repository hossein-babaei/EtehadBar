using ClosedXML.Excel;
using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.MVC.Controllers
{
    public class ExcelController : Controller
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

        public ExcelController(
            ICalendarRepository calendarRepository,
            IConfigRepository configRepository,
            IContractRepository contractRepository,
            ICostRepository costRepository,
            ICustomerRepository customerRepository,
            IDefinitionRepository definitionRepository,
            ILoadFactorRepository loadFactorRepository,
            IPaymentRepository paymentRepository,
            IShippingFeeRepository shippingFeeRepository,
            IVehicleRepository vehicleRepository)
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
        }

        public async Task<IActionResult> Detailed(string calendarId)
        {
            var calendar = await _calendarRepo.Get(calendarId);
            var cost = await _costRepo.Costs().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
            var payment = await _paymentRepo.Payments().Where(a => a.CalendarId.Equals(calendarId)).SumAsync(a => a.Amount);
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
                ws.Cell(index + 2, 2).Value = new PersianDateTime(loadFactors[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).Value = loadFactors[index - 1].LoadNumber;
                ws.Cell(index + 2, 4).Value = loadFactors[index - 1].LoadNumberGov;
                ws.Cell(index + 2, 5).Value = loadFactors[index - 1].ExitNumber;
                ws.Cell(index + 2, 6).Value = loadFactors[index - 1].Origin;
                ws.Cell(index + 2, 7).Value = loadFactors[index - 1].Destination;
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

            var rngTable2 = ws.Range($"B{loadFactors.Count + 3}:P{loadFactors.Count + 9}");
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

        public async Task<IActionResult> Cost(string calendarId)
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

        public async Task<IActionResult> Payment(string calendarId, byte? type, string vehicleId)
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
            ws.Cell(2, 7).Value = "خودرو";

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
                string typeStr = payments[index - 1].Type switch
                {
                    (byte)PaymentType.AdvanceMoney => "مساعده",
                    (byte)PaymentType.Salary => "حقوق",
                    _ => ""
                };

                ws.Cell(index + 2, 1).Value = index;
                ws.Cell(index + 2, 2).Value = new PersianDateTime(payments[index - 1].Date).ToString("yyyy/MM/dd");
                ws.Cell(index + 2, 3).Value = typeStr;
                ws.Cell(index + 2, 4).Value = payments[index - 1].Description;
                ws.Cell(index + 2, 5).Value = payments[index - 1].Amount.ToString("N0");
                ws.Cell(index + 2, 6).Value = payments[index - 1].AdminName;
                ws.Cell(index + 2, 7).Value = payments[index - 1].Vehicle;
            }

            ws.Cell($"B{payments.Count + 3}").Value = "تعداد مساعده پرداختی";
            ws.Range($"B{payments.Count + 3}:D{payments.Count + 3}").Row(1).Merge();
            ws.Cell(payments.Count + 3, 7).Value = payments.Count(a => a.Type.Equals((byte)PaymentType.AdvanceMoney));

            ws.Cell($"B{payments.Count + 4}").Value = "تعداد حقوق پرداختی";
            ws.Range($"B{payments.Count + 4}:D{payments.Count + 4}").Row(1).Merge();
            ws.Cell(payments.Count + 4, 7).Value = payments.Count(a => a.Type.Equals((byte)PaymentType.Salary));

            ws.Cell($"B{payments.Count + 5}").Value = "تعداد کل";
            ws.Range($"B{payments.Count + 5}:D{payments.Count + 5}").Row(1).Merge();
            ws.Cell(payments.Count + 5, 7).Value = payments.Count;

            ws.Cell($"B{payments.Count + 6}").Value = "جمع کل مبلغ مساعده پرداختی";
            ws.Range($"B{payments.Count + 6}:D{payments.Count + 6}").Row(1).Merge();
            ws.Cell(payments.Count + 6, 7).Value = payments.Where(a => a.Type.Equals((byte)PaymentType.AdvanceMoney)).Sum(a => a.Amount).ToString("N0");

            ws.Cell($"B{payments.Count + 7}").Value = "جمع کل مبلغ حقوق پرداختی";
            ws.Range($"B{payments.Count + 7}:D{payments.Count + 7}").Row(1).Merge();
            ws.Cell(payments.Count + 7, 7).Value = payments.Where(a => a.Type.Equals((byte)PaymentType.Salary)).Sum(a => a.Amount).ToString("N0");

            ws.Cell($"B{payments.Count + 8}").Value = "جمع کل پرداختی";
            ws.Range($"B{payments.Count + 8}:F{payments.Count + 8}").Row(1).Merge();
            ws.Cell(payments.Count + 8, 7).Value = payments.Sum(a => a.Amount).ToString();

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

        public async Task<IActionResult> VehicleLoadFactor(string calendarId, string vehicleId)
        {
            var vehicle = await _vehicleRepo.Get(vehicleId);
            var calendar = await _calendarRepo.Get(calendarId);
            var payment = await _paymentRepo.Payments().AsNoTracking().Where(a => a.VehicleId.Equals(vehicleId)).SumAsync(a => a.Amount);
            var loadFactors = await _loadFactorRepo.LoadFactors().Where(a => a.VehicleId.Equals(vehicleId) && a.CalendarId.Equals(calendarId)).OrderBy(a => a.Counter).ToListAsync();

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
                ws.Cell(index + 2, 3).Value = loadFactors[index - 1].LoadNumber;
                ws.Cell(index + 2, 4).Value = loadFactors[index - 1].LoadNumberGov;
                ws.Cell(index + 2, 5).Value = loadFactors[index - 1].ExitNumber;
                ws.Cell(index + 2, 6).Value = loadFactors[index - 1].Origin;
                ws.Cell(index + 2, 7).Value = loadFactors[index - 1].Destination;
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
    }
}
