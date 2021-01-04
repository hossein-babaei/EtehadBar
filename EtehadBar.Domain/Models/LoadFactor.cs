using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    public class LoadFactor
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long Counter { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "مبدأ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Destination { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double DriverFee { get; set; }

        [Display(Name = "شماره بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "شماره خروج")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
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

        [Required]
        [StringLength(50)]
        public string ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        [StringLength(50)]
        public string ContractId { get; set; }
        public virtual Contract Contract { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        [StringLength(50)]
        public string CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }

        [Required]
        [StringLength(450)]
        [Display(Name = "راننده")]
        [ForeignKey("ApplicationUser")]
        public string DriverId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "خودرو")]
        public string VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [StringLength(50)]
        [ForeignKey(nameof(SaipaPressLoadFactor))]
        public string SaipaPressLoadFactorId { get; set; }
        public virtual SaipaPressLoadFactor SaipaPressLoadFactor { get; set; }

        [StringLength(50)]
        [ForeignKey(nameof(SazehGostarLoadFactor))]
        public string SazehGostarLoadFactorId { get; set; }
        public virtual SazehGostarLoadFactor SazehGostarLoadFactor { get; set; }
    }
}
