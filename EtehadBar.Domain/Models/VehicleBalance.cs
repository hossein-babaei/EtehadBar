using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class VehicleBalance
    {

        [Key]
        public long Id { get; set; }

        [Display(Name = "خودرو")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Vehicle))]
        public long VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [Display(Name = "تقویم کاری")]
        [ForeignKey(nameof(Calendar))]
        public long? CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime CreateDateTime { get; set; } = DateTime.Now;

        public long? CustomerId { get; set; }

        public long? LoadFactorId { get; set; }

        public long? BillId { get; set; }

        [Display(Name = "شرح")]
        public string Description { get; set; }

        [Display(Name = "مبلغ")]
        public double Amount { get; set; }

        public DateTime EditDatetime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
