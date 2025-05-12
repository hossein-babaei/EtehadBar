using ClosedXML.Excel;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EtehadBar.MVC.Controllers
{
    [Authorize]
    public class BankFileCreatorController : Controller
    {
        [HttpPost]
        public IActionResult MellatToMellatFile(IFormFile excel)
        {
            string str = "";

            var stream = excel.OpenReadStream();
            if (stream.Length != 0)
            {
                using XLWorkbook workbook = new(stream);
                var ws = workbook.Worksheet(1);

                var rowCount = ws.LastRowUsed().RowNumber();
                var columnCount = ws.LastColumnUsed().ColumnNumber();

                var amountList = new List<double>();
                var bankAccountList = new List<string>();

                for (int i = 3; i < rowCount; i++)
                {
                    amountList.Add(ws.Cell(i, 4).GetDouble());
                    bankAccountList.Add(ws.Cell(i, 5).GetString());
                }

                //creating first row
                var amountSum = amountList.Sum();
                var totalRecordCount = amountList.Count;
                string firstRow = "";

                for (int i = 0; i < 10 - totalRecordCount.ToString().Length; i++)
                    firstRow += "0";
                firstRow += totalRecordCount.ToString();

                for (int i = 0; i < 15 - amountSum.ToString().Length; i++) 
                    firstRow += "0";
                firstRow += amountSum.ToString();
                //

                str += $"{firstRow}\n";
                for (int i = 0; i < amountList.Count; i++)
                {
                    string bankAcountNumber = bankAccountList[i];
                    string amount = amountList[i].ToString();
                    //creating bank account number part
                    for (int x = 0; x < 10 - bankAcountNumber.Length; x++)
                        str += "0";
                    str += bankAcountNumber;
                    //
                    //creating amount part
                    for (int x = 0; x < 15 - amount.Length; x++)
                        str += "0";
                    str += $"{amount}\n";
                    //
                }
            }

            byte[] buffer;
            buffer = Encoding.Default.GetBytes(str);
            return File(buffer, "text/plain", $"FL{new PersianDateTime(DateTime.Now).ToString("yyyyMMdd")}.pay.txt");
        }

        [HttpPost]
        public IActionResult TejaratToTejaratFile(IFormFile excel)
        {
            string str = "",
                tempName = new PersianDateTime(DateTime.Now).ToString("yyMMdd");

            var stream = excel.OpenReadStream();
            if (stream.Length != 0)
            {
                using XLWorkbook workbook = new(stream);
                var ws = workbook.Worksheet(1);

                var rowCount = ws.LastRowUsed().RowNumber();
                var columnCount = ws.LastColumnUsed().ColumnNumber();

                var amountList = new List<double>();
                var bankAccountList = new List<string>();

                for (int i = 3; i < rowCount; i++)
                {
                    amountList.Add(ws.Cell(i, 4).GetDouble());
                    bankAccountList.Add(ws.Cell(i, 5).GetString());
                }

                //creating first row
                var amountSum = amountList.Sum();
                string firstRow = "";

                for (int i = 0; i < 26 - amountSum.ToString().Length; i++)
                    firstRow += "0";

                firstRow += $"{amountSum}{tempName}";
                //

                str += $"{firstRow}\n";
                for (int i = 0; i < amountList.Count; i++)
                {
                    string bankAcountNumber = bankAccountList[i];
                    string amount = amountList[i].ToString();
                    //creating bank account number part
                    for (int x = 0; x < 13 - bankAcountNumber.Length; x++)
                        str += "0";
                    str += bankAcountNumber;
                    //
                    //creating amount part
                    for (int x = 0; x < 13 - amount.Length; x++)
                        str += "0";
                    str += $"{amount}{tempName}\n";
                    //
                }
            }

            byte[] buffer;
            buffer = Encoding.Default.GetBytes(str);
            return File(buffer, "text/plain", $"trans.dat.txt");
        }
    }
}
