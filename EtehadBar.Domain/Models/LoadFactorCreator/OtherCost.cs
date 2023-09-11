using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models.LoadFactorCreator
{
    public class OtherCost
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string DriverName { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [StringLength(450)]
        public string AdminId { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string EditorId { get; set; }
        public DateTime? EditDateTime { get; set; }

        [Display(Name = "پلاک")]
        [ForeignKey(nameof(Vehicle))]
        public long VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [Display(Name = "مشتری")]
        [ForeignKey(nameof(Customer))]
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }
    }
}
