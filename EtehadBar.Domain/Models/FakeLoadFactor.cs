using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class FakeLoadFactor
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

        [Display(Name = "کرایه راننده (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double DriverFee { get; set; }

        [Display(Name = "شماره بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string LoadNumber { get; set; }

        [StringLength(450)]
        public string AdminId { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string EditorId { get; set; }
        public DateTime? EditDateTime { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }

        [Required]
        [Display(Name = "خودرو")]
        public long VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
