using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Turnover
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "تقویم کاری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }

        [Display(Name = "تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime Date { get; set; }

        [Display(Name = "شرح")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Description { get; set; }

        [Display(Name = "دریافت")]
        public double Debtor { get; set; }

        [Display(Name = "پرداخت")]
        public double Creditor { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public TurnoverType TurnoverType { get; set; }

        [Required]
        public DateTime CreateDatetime { get; set; } = DateTime.Now;

        [Display(Name = "طرف حساب")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [StringLength(450)]
        public string CreatorId { get; set; }

        public DateTime? EditDatetime { get; set; }

        [StringLength(450)]
        public string EditorId { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
