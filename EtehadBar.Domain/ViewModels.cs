using EtehadBar.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain
{
    public class CustomerBalanceVM
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Customer Customer { get; set; }
        public List<Calendar> Calendars { get; set; }
        public List<CustomerBalanceDetailVM> Details { get; set; }
    }

    public class CustomerBalanceDetailVM
    {
        public long CalendarId { get; set; }
        public List<CustomerFactor> CustomerFactors { get; set; }
        public List<CustomerIncome> CustomerIncomes { get; set; }
    }

    public class ExcelLoadFactorVM
    {
        public long Id { get; set; }
        public string VehicleName { get; set; }
        public string OriginName { get; set; }
        public string DestinationName { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public double DriverFee { get; set; }
        public double? Tonnage { get; set; }
        public double? TonnagePrice { get; set; }
        public double? DriverTonnagePrice { get; set; }
        public string LoadNumber { get; set; }
        public string LoadNumberGov { get; set; }
        public string ExitNumber { get; set; }
        public double VAT { get; set; }
        public double LoadFactorDeductions { get; set; }
        public double WithholdingTax { get; set; }
        public string AdminId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public double? WeighbridgePrice { get; set; }
        public double? LoadSleepTime { get; set; }
        public double? LoadSleepPrice { get; set; }
        public double? DriverLoadSleepPrice { get; set; }
        public bool IsDriverFeeEditedByAdmin { get; set; }
        public bool IsFreeDriverPrice { get; set; }
        public long ContractId { get; set; }
        public long CalendarId { get; set; }
        public DateTime CalendarStartDate { get; set; }
        public DateTime CalendarEndDate { get; set; }
        public string CalendarTitle { get; set; }
        public long DriverId { get; set; }
        public string DriverName { get; set; }
        public long VehicleId { get; set; }
        public string VehicleLeftNumber { get; set; }
        public string VehicleNumberWord { get; set; }
        public string VehicleRightNumber { get; set; }
        public string VehicleIranStateNumber { get; set; }
        public long AccountBookId { get; set; }
        public string AccountBookNumber { get; set; }
        public MehrcomParsLoadFactor MehrcomParsLoadFactor { get; set; }
        public SaipaPlascoLoadFactor SaipaPlascoLoadFactor { get; set; }
        public SaipaPressLoadFactor SaipaPressLoadFactor { get; set; }
        public SazehGostarLoadFactor SazehGostarLoadFactor { get; set; }
    }

    public class ActivityListByCustomerVM
    {
        public string VehicleType { get; set; }
        public string VehicleNumber { get; set; }
        public double VehicleBalance { get; set; }
        public List<VehicleBankAccountVM> BankAccounts { get; set; }
        public List<ActivityListByCustomerRouteVM> Routes { get; set; }
        public List<ActivityListByCustomerDetailVM> Details { get; set; }
    }

    public class VehicleBankAccountVM
    {
        public string Fullname { get; set; }
        public string AccountNumber { get; set; }
        public long VehicleId { get; set; }
        public long BankId { get; set; }
    }

    public class ActivityListByCustomerRouteVM
    {
        public double Amount { get; set; }
        public int Quantity { get; set; }
    }

    public class ActivityListByCustomerDetailVM
    {
        public DateTime Date { get; set; }
        public string DriverName { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public bool IsFreeDriverPrice { get; set; }
        public string LoadFactorNumber { get; set; }
        public double Amount { get; set; }
        public SaipaPressLoadType? PressFloorType { get; set; }
        public double? Tonnage { get; set; }
        public double? TonnagePrice { get; set; }
        public string SazehRequestNumber { get; set; }
        public bool MehrcomLoad { get; set; } = false;
        public bool MehrcomPalette { get; set; } = false;
        public bool MehrcomReturn { get; set; } = false;
        public double? WeighbridgePrice { get; set; }
        public double? DriverLoadSleepPrice { get; set; }
    }

    public class ActivityListVM
    {
        public long VehicleId { get; set; }
        public string VehicleOwnerName { get; set; }
        public string VehicleNumber { get; set; }
        public double Amount { get; set; }
        public double? ActivityAmount { get; set; }
        public string BankAccountNumber { get; set; }
        public string VehicleRightNumber { get; set; }
        public string VehicleLeftNumber { get; set; }
    }

    public class ActivityListPaymentVM
    {
        public long VehicleId { get; set; }
        public double Amount { get; set; }
    }

    public class AdminDashboardVM
    {
        public int RegisteredLoadFactorCount { get; set; }
        public double LoadFactorsAmount { get; set; }
        public double LoadFactorsDriverFee { get; set; }
        public double PaymentAmount { get; set; }
        public double CostAmount { get; set; }
        public double MehrcomParsAmount { get; set; }
        public double MehrcomParsDriverFee { get; set; }
        public double SazehGostarAmount { get; set; }
        public double SazehGostarDriverFee { get; set; }
        public double SaipaPlascoAmount { get; set; }
        public double SaipaPlascoDriverFee { get; set; }
        public double SaipaPressAmount { get; set; }
        public double SaipaPressDeriverFee { get; set; }
        public List<AdminDashboardUserActivityBoxVM> UserActivity { get; set; }
    }

    public class AdminDashboardUserActivityBoxVM
    {
        public string UserId { get; set; }
        public string Avatar { get; set; }
        public string Fullname { get; set; }
        public int LoadFactorRegisterdCount { get; set; }
    }

    public class GlobalLoadFactorVM
    {
        [Display(Name = "مبدا")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        public string Destination { get; set; }

        [Display(Name = "مشتری - شماره قرارداد")]
        public string CustomerName { get; set; }

        [Display(Name = "نام راننده")]
        public string DriverName { get; set; }

        [Display(Name = "نوع خودرو")]
        public string VehicleType { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "تناژ اضافه")]
        public double? Tonnage { get; set; }

        [Display(Name = "نرخ تناژ اضافه (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "شماره بارنامه")]
        public string LoadNumber { get; set; }

        [Display(Name = "بارنامه دولتی")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "ارزش افزوده (%)")]
        public double VAT { get; set; } = 0;

        [Display(Name = "سپرده بیمه (%)")]
        public double LoadFactorDeductions { get; set; } = 0;

        [Display(Name = "مالیات تکلیفی (%)")]
        public double WithholdingTax { get; set; } = 0;

        public long Id { get; set; }

        public string RowId { get; set; }
    }

    public class CreateTurnoverVM
    {
        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "شرح")]
        public string Description { get; set; }

        [Display(Name = "دریافت")]
        public double? Debtor { get; set; } = 0;

        [Display(Name = "پرداخت")]
        public double? Creditor { get; set; } = 0;
        public long TurnoverProfileId { get; set; }
    }

    public class EditTurnoverVM
    {
        public long Id { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "شرح")]
        public string Description { get; set; }

        [Display(Name = "دریافت")]
        public double? Debtor { get; set; } = 0;

        [Display(Name = "پرداخت")]
        public double? Creditor { get; set; } = 0;
        public string Attachments { get; set; }
        public long TurnoverProfileId { get; set; }
    }

    public class CreateBankAccountBookVM
    {
        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "شرح")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Description { get; set; }

        [Display(Name = "شماره پیگیری/مرجع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string ReferenceNo { get; set; }

        [Display(Name = "کارمزد")]
        public double TransferFee { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public BankAccountBookAmountType AmountType { get; set; }

        [Display(Name = "نوع پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public BankAccountBookType AccountBookType { get; set; }

        [Display(Name = "حساب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long BankAccountId { get; set; }
    }

    public class EditBankAccountBookVM
    {
        public long Id { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "شرح")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Description { get; set; }

        [Display(Name = "شماره پیگیری/مرجع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string ReferenceNo { get; set; }

        [Display(Name = "کارمزد")]
        public double TransferFee { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public BankAccountBookAmountType AmountType { get; set; }

        [Display(Name = "نوع پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public BankAccountBookType AccountBookType { get; set; }
    }

    public class CreateFreeLoadFactorVM
    {
        [Display(Name = "مبدا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Destination { get; set; }

        [Display(Name = "نام متقاضی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string ApplicantName { get; set; }

        [Display(Name = "نام راننده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string DriverName { get; set; }

        [Display(Name = "کد ملی راننده")]
        public string DriverNationalNumber { get; set; }

        [Display(Name = "نوع خودرو")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string VehicleType { get; set; }

        [Display(Name = "اعداد سمت چپ پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LeftNumber { get; set; }

        [Display(Name = "حرف پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(1, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string NumberWord { get; set; }

        [Display(Name = "اعداد سمت راست پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(3, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string RightNumber { get; set; }

        [Display(Name = "کد استان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string IranStateNumber { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double DriverFee { get; set; }

        [Display(Name = "تناژ اضافه")]
        public double? Tonnage { get; set; }

        [Display(Name = "نرخ تناژ اضافه (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "شماره بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "تقویم کاری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CalendarId { get; set; }
    }

    public class EditFreeLoadFactorVM
    {
        public long Id { get; set; }

        [Display(Name = "مبدا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Destination { get; set; }

        [Display(Name = "نام متقاضی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string ApplicantName { get; set; }

        [Display(Name = "نام راننده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string DriverName { get; set; }

        [Display(Name = "کد ملی راننده")]
        public string DriverNationalNumber { get; set; }

        [Display(Name = "نوع خودرو")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string VehicleType { get; set; }

        [Display(Name = "اعداد سمت چپ پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LeftNumber { get; set; }

        [Display(Name = "حرف پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(1, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string NumberWord { get; set; }

        [Display(Name = "اعداد سمت راست پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(3, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string RightNumber { get; set; }

        [Display(Name = "کد استان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string IranStateNumber { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double DriverFee { get; set; }

        [Display(Name = "تناژ اضافه")]
        public double? Tonnage { get; set; }

        [Display(Name = "نرخ تناژ اضافه (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "شماره بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "تقویم کاری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CalendarId { get; set; }

        [StringLength(128)]
        public string LoadFactorScan { get; set; }
    }

    public class UserProfileCacheVM
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phonenumber { get; set; }
        public string AccountBankName { get; set; }
        public string BankAccountNumber { get; set; }
        public ApplicationRoleType Role { get; set; }
        public string NationalId { get; set; }
        public string Avatar { get; set; }
    }

    public class LogDetailFormFileVM
    {
        public LogDetailFormFileVM(string fileName, long fileSize, string contentType)
        {
            FileName = fileName;
            FileSize = fileSize;
            ContentType = contentType;
        }

        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
    }

    public class CreateContractVM
    {
        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "موضوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Subject { get; set; }

        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Number { get; set; }

        [Display(Name = "الحاقیه ای است برای قرارداد")]
        public long? ParentContractId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long CustomerId { get; set; }
    }

    public class EditContractVM
    {
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long Id { get; set; }

        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "موضوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Subject { get; set; }

        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Number { get; set; }
    }

    public class CreateCalendarVM
    {
        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "مالیات ارزش افزوده (%)")]
        public double VAT { get; set; }

        [Display(Name = "سپرده بیمه (%)")]
        public double LoadFactorDeductions { get; set; }

        [Display(Name = "مالیات تکلیفی (%)")]
        public double WithholdingTax { get; set; }
    }

    public class EditCalendarVM
    {
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long Id { get; set; }

        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "مالیات ارزش افزوده (%)")]
        public double VAT { get; set; }

        [Display(Name = "سپرده بیمه (%)")]
        public double LoadFactorDeductions { get; set; }

        [Display(Name = "مالیات تکلیفی (%)")]
        public double WithholdingTax { get; set; }
    }

    public class LoadFactorConfigVM
    {
        public double VAT { get; set; }
        public double WithholdingTax { get; set; }
    }

    public class PaymentVM
    {
        public long Id { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "شرح پرداخت")]
        public string Description { get; set; }

        [Display(Name = "نوع پرداخت")]
        public PaymentType PaymentType { get; set; }

        public string Picture { get; set; }

        public string AdminId { get; set; }

        [Display(Name = "کاربر سیستم")]
        public string AdminName { get; set; }


        [Display(Name = "خودرو")]
        public long? VehicleId { get; set; }
        public string Vehicle { get; set; }

        [Display(Name = "کارمند")]
        public string UserId { get; set; }
        public string UserFullname { get; set; }
    }

    public class CreateUserVM
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Lastname { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public bool Gender { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید متنی بین {2} الی {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string BirthString { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} باید {1} رقم باشد.")]
        public string NationalId { get; set; }

        [Display(Name = "نام بانک")]
        public string AccountBankName { get; set; }

        [Display(Name = "شماره حساب")]
        public string BankAccountNumber { get; set; }

        public ApplicationRoleType Role { get; set; }

        [Display(Name = "تصویر پرسنلی")]
        public IFormFile Pic { get; set; }
    }

    public class CreateDriverVM
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Lastname { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public bool Gender { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string BirthString { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} باید {1} رقم باشد.")]
        public string NationalId { get; set; }

        public ApplicationRoleType Role { get; set; }

        [Display(Name = "تصویر پرسنلی")]
        public IFormFile Pic { get; set; }
    }

    public class LoginVM
    {
        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "من را به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class RegisterVM
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Lastname { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public bool Gender { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید متنی بین {2} الی {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }
    }

    public class ConfirmPhoneNumberVM
    {
        [Display(Name = "کد فعالسازی")]
        [Required(ErrorMessage = "لطفا کد را وارد نمائید.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "کد وارد شده صحیح نیست.")]
        public string Code { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "لطفا شماره تلفن همراه خود را وارد نمائید.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "شماره تلفن همراه باید 11 رقم باشد.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string PhoneNumber { get; set; }
    }

    public class SendNewCodeVM
    {
        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "لطفا شماره تلفن همراه خود را وارد نمائید.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "شماره تلفن همراه باید 11 رقم باشد.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string PhoneNumber { get; set; }
    }

    public class ResetPasswordVM
    {
        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید متنی بین {2} الی {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "کد 6 رقمی پیامک شده به شما")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "کد وارد شده صحیح نیست.")]
        public string Code { get; set; }
    }

    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور فعلی")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} بین {2} تا {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور جدید")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور جدید")]
        [Compare("NewPassword", ErrorMessage = "{0} با کلمه عبور جدید وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }
    }

    public class EditUserVM
    {
        public string Id { get; set; }

        public DateTime Birth { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string BirthString { get; set; }

        [Display(Name = "نام")]
        [StringLength(128, ErrorMessage = "فیلد {0} باید بین {2} تا {1} حرف باشد.", MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [StringLength(128, ErrorMessage = "فیلد {0} باید بین {2} تا {1} حرف باشد.", MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(10, ErrorMessage = "فیلد {0} باید {2} رقم باشد..", MinimumLength = 10)]
        [Display(Name = "کد ملی")]
        public string NationalId { get; set; }

        [Display(Name = "تلفن همراه")]
        [StringLength(11, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 11)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "تایید تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست.")]
        [StringLength(256, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 5)]
        public string Email { get; set; }

        [Display(Name = "تایید ایمیل")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "تلفن ثابت")]
        [StringLength(11, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 11)]
        public string Tel { get; set; }

        [Display(Name = "عکس پرسنلی")]
        public string Avatar { get; set; }

        [Display(Name = "نام بانک")]
        public string AccountBankName { get; set; }

        [Display(Name = "شماره حساب")]
        public string BankAccountNumber { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }
    }

    public class CreateAccountBookVM
    {
        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Number { get; set; }

        [Display(Name = "شماره فاکتور")]
        public string FactorNumber { get; set; }

        [Display(Name = "تقویم کاری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CalendarId { get; set; }

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerId { get; set; }

        [Display(Name = "محدودیت تعداد بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int LoadFactorLimit { get; set; }
    }

    public class EditAccountBookVM
    {
        [Required]
        public long Id { get; set; }

        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Number { get; set; }

        [Display(Name = "شماره فاکتور")]
        public string FactorNumber { get; set; }

        [Display(Name = "تقویم کاری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CalendarId { get; set; }

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerId { get; set; }

        [Display(Name = "محدودیت تعداد بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int LoadFactorLimit { get; set; }
    }

    public class VehicleActivityVM
    {
        public string DriverName { get; set; }
        public long OriginId { get; set; }
        public long DestinationId { get; set; }

        [Display(Name = "مبدأ")]
        public string OriginTitle { get; set; }

        [Display(Name = "مقصد")]
        public string DestionationTitle { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "تناژ اضافه")]
        public double? Tonnage { get; set; }

        [Display(Name = "نرخ تناژ اضافه (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "تعداد")]
        public int Count { get; set; }
    }

    public class UserNameAndIdVM
    {
        public string Id { get; set; }
        public string Fullname { get; set; }
    }

    public class AutoCompleteResultVM
    {
        public string value { get; set; }
        public string label { get; set; }
    }

    public class CustomerSeparateRouteVM
    {
        public string Title { get; set; }
        public double TonnageAmount { get; set; } = 0;
        public double WeighbridgeAmount { get; set; } = 0;
        public double DriverLoadSleepPrice { get; set; } = 0;
        public List<CustomerSeparateRouteDetailVM> Details { get; set; }
    }

    public class CustomerSeparateRouteDetailVM
    {
        public List<string> Origins { get; set; }
        public List<string> Destinaitons { get; set; }
        public double Amount { get; set; }
        public int Quantity { get; set; }
        public string Vehicle { get; set; }
    }

    public class CreateTurnoverProfilePeriodVM
    {
        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "عنوان دوره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }

        public long TurnoverProfileId { get; set; }
    }

    public class EditTurnoverProfilePeriodVM
    {
        public long Id { get; set; }

        public long TurnoverProfileId { get; set; }

        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "عنوان دوره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }
    }

    public class CreateCustomerPeriodicBalanceSummaryVM
    {
        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "مانده دوره قبل")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double BalanceAmount { get; set; }

        [Display(Name = "مانده سپرده بیمه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double InsuranceBalanceAmount { get; set; }

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerId { get; set; }
    }

    public class EditCustomerPeriodicBalanceSummaryVM
    {
        public long Id { get; set; }

        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "مانده دوره قبل")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double BalanceAmount { get; set; }

        [Display(Name = "مانده سپرده بیمه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double InsuranceBalanceAmount { get; set; }

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Customer))]
        public long CustomerId { get; set; }
    }

    public class CreateCustomerPeriodicBalanceAddonVM
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "کسر/اضافه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsPositive { get; set; }

        [Display(Name = "دوره مربوطه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerPeriodicBalanceSummaryId { get; set; }
    }

    public class EditCustomerPeriodicBalanceAddonVM
    {
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "کسر/اضافه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsPositive { get; set; }

        [Display(Name = "دوره مربوطه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerPeriodicBalanceSummaryId { get; set; }
    }

    public class VehicleFullActivityVM
    {
        public long VehicleId { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleOwnerName { get; set; }
        public double Amount { get; set; }
        public double? Activity { get; set; }
        public string RightNumber { get; set; }
        public string LeftNumber { get; set; }
    }

    public class GeneralReportVM
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public double FactorsSum { get; set; }
        public double DriversAmountSum { get; set; }
        public double CostSum { get; set; }
    }

    public class GeneralReportDriverFeeVM
    {
        public long ContractId { get; set; }
        public double Amount { get; set; }
    }

    public class ShippingFeeRouteWithPriceVM
    {
        public long Id { get; set; }

        [Display(Name = "عنوان (اختیاری)")]
        [MaxLength(256, ErrorMessage = "{0} حداکثر باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "مبدأ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long OriginId { get; set; }

        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long DestinationId { get; set; }

        public string Destination { get; set; }

        [Display(Name = "خودرو")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Vehicle { get; set; }

        [Display(Name = "کرایه (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Price { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double DriverPrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه تریلی (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه تریلی راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "نوع بار")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string ShippingFeeLoadType { get; set; }
    }

    public class CreateUserPlannerVM
    {
        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }
    }


    public class EditUserPlannerVM
    {
        public long Id { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }
    }

    public class CreateUserPlannerItemVM
    {
        [Display(Name = "اولویت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Priority { get; set; } = 1;

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Content { get; set; }

        [Required]
        public long UserPlannerId { get; set; }
    }

    public class EditUserPlannerItemVM
    {
        public long Id { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Priority { get; set; } = 1;

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Content { get; set; }

        [Required]
        public long UserPlannerId { get; set; }
    }

    public class CreateGroupOtherCostVM
    {
        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerId { get; set; }

        [Display(Name = "تقویم")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CalendarId { get; set; }

        [Display(Name = "مبلغ")]
        public double? Amount { get; set; }

        [Display(Name = "حداقل مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double MinimumAmount { get; set; }

        [Display(Name = "حداکثر مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double MaximumAmount { get; set; }
    }

    public class BillPrintDataVM
    {
        public string ReceiverName { get; set; }
        public string VehicleNumber { get; set; }
        public double Amount { get; set; }
    }
}
