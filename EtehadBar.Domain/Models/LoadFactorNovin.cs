using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class LoadFactorNovin
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "وضعیت پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsPaied { get; set; } = false;

        [Display(Name = "تاریخ پرداخت")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "وضعیت دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsReceived { get; set; } = false;

        [Display(Name = "تاریخ دریافت")]
        public DateTime? ReceiveDate { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "مبدا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Destination { get; set; }

        [Display(Name = "نام متقاضی")]
        public string ApplicantName { get; set; }

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

        [StringLength(450)]
        public string CreatorId { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string EditorId { get; set; }
        public DateTime? EditDateTime { get; set; }

        public string Attachments { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }
        public Calendar Calendar { get; set; }

        [Required]
        [Display(Name = "راننده")]
        public long DriverId { get; set; }
        public Driver Driver { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        [Required]
        [Display(Name = "شرکت متقاضی")]
        public long? CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
