using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class CustomerIncome
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "مبلغ (تومان)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "شرح دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(512, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [StringLength(50)]
        public string Picture { get; set; }

        [StringLength(450)]
        public string AdminId { get; set; }

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [Display(Name = "تقویم")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(50)]
        public long CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
