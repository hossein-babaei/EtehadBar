using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Payment
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "مبلغ (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "شرح پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(512, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "نوع پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public PaymentType PaymentType { get; set; }

        [StringLength(50)]
        public string Picture { get; set; }

        [Required]
        [StringLength(450)]
        public string AdminId { get; set; }

        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }

        [Display(Name = "خودرو")]
        [ForeignKey(nameof(Vehicle))]
        public long? VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [Display(Name = "کارمند")]
        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
