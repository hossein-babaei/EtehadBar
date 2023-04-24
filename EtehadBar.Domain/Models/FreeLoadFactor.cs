using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class FreeLoadFactor
    {
        [Key]
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

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; } = DateTime.Now;

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
        [StringLength(128,  ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "مالیات ارزش افزوده (%)")]
        public double VAT { get; set; } = 0;

        [Display(Name = "سپرده بیمه (%)")]
        public double LoadFactorDeductions { get; set; } = 0;

        [Display(Name = "مالیات تکلیفی (%)")]
        public double WithholdingTax { get; set; } = 0;

        [Required]
        public DateTime CreateDatetime { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string CreatorId { get; set; }

        public DateTime? EditDatetime { get; set; }

        [StringLength(450)]
        public string EditorId { get; set; }

        [Display(Name = "تقویم کاری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Calendar))]
        public long CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }

        [StringLength(128)]
        public string LoadFactorScan { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
