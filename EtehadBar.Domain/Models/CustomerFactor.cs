using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class CustomerFactor
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "شماره فاکتور")]
        public string FactorNumber { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; } = 0;

        [Display(Name = "قرارداد")]
        public long CustomerId { get; set; }

        [Display(Name = "قرارداد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Contract))]
        public long ContractId { get; set; }
        public virtual Contract Contract { get; set; }

        [Required]
        public DateTime CreateDatetime { get; set; } = DateTime.Now;

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
