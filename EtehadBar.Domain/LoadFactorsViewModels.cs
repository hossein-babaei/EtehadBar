using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain
{
    public class CSaipaPlascoLoadFactorVM
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

        [Display(Name = "بارنامه اتحاد بار")]
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

        [Required]
        [StringLength(50)]
        [Display(Name = "نرخ بارنامه")]
        public string ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        [StringLength(50)]
        public string ContractId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "تقویم کاری")]
        public string CalendarId { get; set; }

        [Required]
        [StringLength(450)]
        [Display(Name = "راننده")]
        public string DriverId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "خودرو")]
        public string VehicleId { get; set; }
    }

    public class ESaipaPlascoLoadFactorVM
    {
        [Required]
        [StringLength(50)]
        public string Id { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "بارنامه اتحاد بار")]
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

        [Required]
        [StringLength(50)]
        [Display(Name = "نرخ بارنامه")]
        public string ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        [StringLength(50)]
        public string ContractId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "تقویم کاری")]
        public string CalendarId { get; set; }

        [Required]
        [StringLength(450)]
        [Display(Name = "راننده")]
        public string DriverId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "خودرو")]
        public string VehicleId { get; set; }
    }

    public class CSaipaPressLoadFactorVM
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

        [Display(Name = "سند ورود")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(50, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string EntryNumber { get; set; }

        [Display(Name = "نوع بار")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LoadType { get; set; }

        [Display(Name = "بارنامه اتحاد بار")]
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

        [StringLength(50)]
        [Display(Name = "نرخ بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string ShippingFeeId { get; set; }

        [Display(Name = "مشتری")]
        [StringLength(50)]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string ContractId { get; set; }

        [StringLength(50)]
        [Display(Name = "تقویم کاری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string CalendarId { get; set; }

        [StringLength(450)]
        [Display(Name = "راننده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string DriverId { get; set; }

        [StringLength(50)]
        [Display(Name = "خودرو")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string VehicleId { get; set; }
    }

    public class ESaipaPressLoadFactorVM
    {
        [Required]
        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RelationId { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "سند ورود")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(50, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string EntryNumber { get; set; }

        [Display(Name = "نوع بار")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LoadType { get; set; }

        [Display(Name = "بارنامه اتحاد بار")]
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

        [Required]
        [StringLength(50)]
        [Display(Name = "نرخ بارنامه")]
        public string ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        [StringLength(50)]
        public string ContractId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "تقویم کاری")]
        public string CalendarId { get; set; }

        [Required]
        [StringLength(450)]
        [Display(Name = "راننده")]
        public string DriverId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "خودرو")]
        public string VehicleId { get; set; }
    }

    public class CSazehGostarLoadFactorVM
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

        [Display(Name = "کد علت صدور")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string RegisterCode { get; set; }

        [Display(Name = "معین")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Certain { get; set; }

        [Display(Name = "ماهیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Nature { get; set; }

        [Display(Name = "شرح سند")]
        public string Description { get; set; }

        [Display(Name = "تعداد")]
        public int Count { get; set; }

        [Display(Name = "تفضیلی مرکز هزینه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(50, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string DetailedCostCenter { get; set; }

        [Display(Name = "وضعیت")]
        [StringLength(50, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Status { get; set; }

        [Display(Name = "بارنامه اتحاد بار")]
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

        [Required]
        [StringLength(50)]
        [Display(Name = "نرخ بارنامه")]
        public string ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        [StringLength(50)]
        public string ContractId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "تقویم کاری")]
        public string CalendarId { get; set; }

        [Required]
        [StringLength(450)]
        [Display(Name = "راننده")]
        public string DriverId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "خودرو")]
        public string VehicleId { get; set; }
    }

    public class ESazehGostarLoadFactorVM
    {
        [Required]
        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RelationId { get; set; }

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }

        [Display(Name = "کد علت صدور")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string RegisterCode { get; set; }

        [Display(Name = "معین")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Certain { get; set; }

        [Display(Name = "ماهیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Nature { get; set; }

        [Display(Name = "شرح سند")]
        public string Description { get; set; }

        [Display(Name = "تعداد")]
        public int Count { get; set; }

        [Display(Name = "تفضیلی مرکز هزینه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(50, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string DetailedCostCenter { get; set; }

        [Display(Name = "وضعیت")]
        [StringLength(50, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Status { get; set; }

        [Display(Name = "بارنامه اتحاد بار")]
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

        [Required]
        [StringLength(50)]
        [Display(Name = "نرخ بارنامه")]
        public string ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        [StringLength(50)]
        public string ContractId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "تقویم کاری")]
        public string CalendarId { get; set; }

        [Required]
        [StringLength(450)]
        [Display(Name = "راننده")]
        public string DriverId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "خودرو")]
        public string VehicleId { get; set; }
    }
}
