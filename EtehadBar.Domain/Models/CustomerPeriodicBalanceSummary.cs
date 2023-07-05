using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class CustomerPeriodicBalanceSummary
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "از تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تا تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime EndDate { get; set; }

        [Display(Name = "مانده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double BalanceAmount { get; set; }

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Customer))]
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
