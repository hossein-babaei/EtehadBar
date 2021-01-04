using ClosedXML.Excel;
using EtehadBar.Domain.Interfaces;
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

            ws.Cell($"B{loadFactors.Count + 5}").Value = "حقوق مساعده پرداختی";
            ws.Range($"B{loadFactors.Count + 5}:O{loadFactors.Count + 5}").Row(1).Merge();
            ws.Cell(loadFactors.Count + 5, 16).Value = payment.ToString("N0");


            ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            ws.Columns().AdjustToContents();

            await using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{docTitle}-parsmvc.xlsx");
        }
    }
}
