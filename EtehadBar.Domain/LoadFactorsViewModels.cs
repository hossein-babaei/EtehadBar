using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain
{
    public class CSaipaPlascoLoadFactorVM
    {
        //[Display(Name = "ترتیب")]
        //[Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        //public long Sequence { get; set; }

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
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "شماره خروج")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ExitNumber { get; set; }

        [Required]
        [Display(Name = "نرخ بارنامه")]
        public long ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long ContractId { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "صورت وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long AccountBookId { get; set; }

        [Display(Name = "بارنامه آزاد")]
        public bool IsFreeDriverPrice { get; set; }
    }

    public class ESaipaPlascoLoadFactorVM
    {
        //[Display(Name = "ترتیب")]
        //[Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        //public long Sequence { get; set; }

        [Required]
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

        [Display(Name = "بارنامه اتحاد بار")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "شماره خروج")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ExitNumber { get; set; }

        [Required]
        [Display(Name = "نرخ بارنامه")]
        public long ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long ContractId { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "صورت وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long AccountBookId { get; set; }

        [Display(Name = "بارنامه آزاد")]
        public bool IsFreeDriverPrice { get; set; }
    }

    public class CSaipaPressLoadFactorVM
    {
        [Display(Name = "ترتیب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long Sequence { get; set; }

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
        [StringLength(50, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string EntryNumber { get; set; }

        [Display(Name = "نوع بار")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LoadType { get; set; }

        [Display(Name = "شماره بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "سند خروج")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ExitNumber { get; set; }

        [Display(Name = "نرخ بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long ShippingFeeId { get; set; }

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long ContractId { get; set; }

        [Display(Name = "تقویم کاری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CalendarId { get; set; }

        [Display(Name = "راننده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long DriverId { get; set; }

        [Display(Name = "خودرو")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long VehicleId { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "نرخ تناژ اضافه (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "تناژ اضافه")]
        public double? Tonnage { get; set; } = 0;

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public SaipaPressLoadType PressFloorType { get; set; }

        [Display(Name = "صورت وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long AccountBookId { get; set; }

        [Display(Name = "بارنامه آزاد")]
        public bool IsFreeDriverPrice { get; set; }
    }

    public class ESaipaPressLoadFactorVM
    {
        [Display(Name = "ترتیب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long Sequence { get; set; }

        [Required]
        public long Id { get; set; }

        [Required]
        public long RelationId { get; set; }

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

        [Display(Name = "سند خروج")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ExitNumber { get; set; }

        [Required]
        [Display(Name = "نرخ بارنامه")]
        public long ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long ContractId { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "نرخ تناژ اضافه (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "تناژ اضافه")]
        public double? Tonnage { get; set; } = 0;

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public SaipaPressLoadType PressFloorType { get; set; }

        [Display(Name = "صورت وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long AccountBookId { get; set; }

        [Display(Name = "بارنامه آزاد")]
        public bool IsFreeDriverPrice { get; set; }
    }

    public class CSazehGostarLoadFactorVM
    {
        //[Display(Name = "ترتیب")]
        //[Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        //public long Sequence { get; set; }

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

        [Display(Name = "تعداد")]
        public int Count { get; set; }

        [Display(Name = "تفضیلی مرکز هزینه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(50, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string DetailedCostCenter { get; set; }

        [Display(Name = "شماره بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره درخواست")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ExitNumber { get; set; }

        [Required]
        [Display(Name = "نرخ بارنامه")]
        public long ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long ContractId { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "صورت وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long AccountBookId { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public SazehGostarLoadType SazehLoadType { get; set; }

        [Display(Name = "بارنامه آزاد")]
        public bool IsFreeDriverPrice { get; set; }
    }

    public class ESazehGostarLoadFactorVM
    {
        //[Display(Name = "ترتیب")]
        //[Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        //public long Sequence { get; set; }

        [Required]
        public long Id { get; set; }

        [Required]
        public long RelationId { get; set; }

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

        [Display(Name = "شماره بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره درخواست")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ExitNumber { get; set; }

        [Required]
        [Display(Name = "نرخ بارنامه")]
        public long ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long ContractId { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "صورت وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long AccountBookId { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public SazehGostarLoadType SazehLoadType { get; set; }

        [Display(Name = "بارنامه آزاد")]
        public bool IsFreeDriverPrice { get; set; }
    }

    public class CMehrcomParsLoadFactorVM
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

        [Display(Name = "بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "شماره بارنامه دولتی برگشتی")]
        public string LoadNumberGovReturn { get; set; }

        [Required]
        [Display(Name = "نرخ بارنامه")]
        public long ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long ContractId { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "شماره زونکن")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long AccountBookId { get; set; }
         
        [Display(Name = "بار")]
        public bool Load { get; set; } = false;

        [Display(Name = "پالت")]
        public bool Palette { get; set; } = false;

        [Display(Name = "برگشتی")]
        public bool Return { get; set; } = false;

        [Display(Name = "نرخ باسکول")]
        public double? WeighbridgePrice { get; set; }

        [Display(Name = "میزان خواب (دقیقه)")]
        public double? LoadSleepTime { get; set; }

        [Display(Name = "مبلغ دریافتی خواب")]
        public double? LoadSleepPrice { get; set; }

        [Display(Name = "مبلغ پرداختی خواب")]
        public double? DriverLoadSleepPrice { get; set; }

        [Display(Name = "دسته بندی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CategoryId { get; set; }

        //[Display(Name = "نوع بار")]
        //[Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        //[StringLength(128)]
        //public string LoadType { get; set; }

        [Display(Name = "نرخ تناژ اضافه (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "افزایش 30%")]
        public bool HasAddonMessage { get; set; } = false;

        [Display(Name = "بارنامه آزاد")]
        public bool IsFreeDriverPrice { get; set; }
    }

    public class EMehrcomParsLoadFactorVM
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

        [Display(Name = "بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "شماره بارنامه دولتی برگشتی")]
        public string LoadNumberGovReturn { get; set; }

        [Required]
        [Display(Name = "نرخ بارنامه")]
        public long ShippingFeeId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long ContractId { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        public double DriverFee { get; set; }

        [Display(Name = "شماره زونکن")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long AccountBookId { get; set; }

        [Display(Name = "بار")]
        public bool Load { get; set; }

        [Display(Name = "پالت")]
        public bool Palette { get; set; }

        [Display(Name = "برگشتی")]
        public bool Return { get; set; }

        [Display(Name = "نرخ باسکول")]
        public double? WeighbridgePrice { get; set; }

        [Display(Name = "میزان خواب (دقیقه)")]
        public double? LoadSleepTime { get; set; }

        [Display(Name = "مبلغ دریافتی خواب")]
        public double? LoadSleepPrice { get; set; }

        [Display(Name = "مبلغ پرداختی خواب")]
        public double? DriverLoadSleepPrice { get; set; }

        [Display(Name = "دسته بندی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CategoryId { get; set; }

        //[Display(Name = "نوع بار")]
        //[Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        //[StringLength(128)]
        //public string LoadType { get; set; }

        [Display(Name = "نرخ تناژ اضافه (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "افزایش 30%")]
        public bool HasAddonMessage { get; set; } = false;

        [Display(Name = "بارنامه آزاد")]
        public bool IsFreeDriverPrice { get; set; }
    }

    public class CreateLoadFactorNovinVM
    {
        [Display(Name = "وضعیت پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsPaied { get; set; } = false;

        [Display(Name = "وضعیت دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsReceived { get; set; } = false;

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }


        [Display(Name = "روز پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int PDay { get; set; }

        [Display(Name = "ماه پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int PMonth { get; set; }

        [Display(Name = "سال پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int PYear { get; set; }


        [Display(Name = "روز دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int RDay { get; set; }

        [Display(Name = "ماه دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int RMonth { get; set; }

        [Display(Name = "سال دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int RYear { get; set; }


        [Display(Name = "مبدا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Destination { get; set; }

        [Display(Name = "نام متقاضی")]
        public string ApplicantName { get; set; }

        [Display(Name = "بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "مشتری")]
        public long? CustomerId { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }

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
    }

    public class EditLoadFactorNovinVM
    {
        public long Id { get; set; }

        [Display(Name = "وضعیت پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsPaied { get; set; } = false;

        [Display(Name = "وضعیت دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsReceived { get; set; } = false;

        [Display(Name = "روز")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Day { get; set; }

        [Display(Name = "ماه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Month { get; set; }

        [Display(Name = "سال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Year { get; set; }


        [Display(Name = "روز پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int PDay { get; set; }

        [Display(Name = "ماه پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int PMonth { get; set; }

        [Display(Name = "سال پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int PYear { get; set; }


        [Display(Name = "روز دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int RDay { get; set; }

        [Display(Name = "ماه دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int RMonth { get; set; }

        [Display(Name = "سال دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int RYear { get; set; }

        [Display(Name = "مبدا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Destination { get; set; }

        [Display(Name = "نام متقاضی")]
        public string ApplicantName { get; set; }

        [Display(Name = "بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [Display(Name = "شماره بارنامه دولتی")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumberGov { get; set; }

        [Display(Name = "مشتری")]
        public long? CustomerId { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }

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
    }
}
