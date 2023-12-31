using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class CustomerPeriodicBalanceAddon
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime? Date { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "کسر/اضافه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsPositive { get; set; }

        [Display(Name = "دوره مربوطه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerPeriodicBalanceSummaryId { get; set; }
        public CustomerPeriodicBalanceSummary CustomerPeriodicBalanceSummary { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
