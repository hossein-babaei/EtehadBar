using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class AccountBook
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Number { get; set; }

        [Display(Name = "شماره فاکتور")]
        public string FactorNumber { get; set; }

        [Display(Name = "محدودیت تعداد بارنامه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int LoadFactorLimit { get; set; } = 150;

        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsOpen { get; set; } = true;

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Customer))]
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ICollection<LoadFactor> LoadFactors { get; set; }

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
