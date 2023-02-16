using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class LoadFactor
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "مبدأ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Origin))]
        public long OriginId { get; set; }
        public virtual LoadRoutes Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Destination))]
        public long DestinationId { get; set; }
        public virtual LoadRoutes Destination { get; set; }

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
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "شماره خروج")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ExitNumber { get; set; }

        [Display(Name = "مالیات ارزش افزوده (%)")]
        public double VAT { get; set; }

        [Display(Name = "سپرده بیمه (%)")]
        public double LoadFactorDeductions { get; set; }

        [Display(Name = "مالیات تکلیفی (%)")]
        public double WithholdingTax { get; set; }

        [StringLength(450)]
        public string AdminId { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string EditorId { get; set; }
        public DateTime? EditDateTime { get; set; }

        [Required]
        public long ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long ContractId { get; set; }
        public virtual Contract Contract { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }

        [Required]
        [Display(Name = "راننده")]
        [ForeignKey("ApplicationUser")]
        public long DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [Required]
        [Display(Name = "صورت وضعیت")]
        [ForeignKey(nameof(AccountBook))]
        public long AccountBookId { get; set; }
        public virtual AccountBook AccountBook { get; set; }

        [ForeignKey(nameof(MehrcomParsLoadFactor))]
        public long? MehrcomParsLoadFactorId { get; set; }
        public virtual MehrcomParsLoadFactor MehrcomParsLoadFactor { get; set; }

        [ForeignKey(nameof(SaipaPlascoLoadFactor))]
        public long? SaipaPlascoLoadFactorId { get; set; }
        public virtual SaipaPlascoLoadFactor SaipaPlascoLoadFactor { get; set; }

        [ForeignKey(nameof(SaipaPressLoadFactor))]
        public long? SaipaPressLoadFactorId { get; set; }
        public virtual SaipaPressLoadFactor SaipaPressLoadFactor { get; set; }

        [ForeignKey(nameof(SazehGostarLoadFactor))]
        public long? SazehGostarLoadFactorId { get; set; }
        public virtual SazehGostarLoadFactor SazehGostarLoadFactor { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
