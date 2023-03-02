using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class BankAccountBook
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime Date { get; set; }

        [Display(Name = "شرح")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Description { get; set; }

        [Display(Name = "شماره پیگیری/مرجع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string ReferenceNo { get; set; }

        [Display(Name = "بدهکار")]
        public double Debtor { get; set; }

        [Display(Name = "بستانکار")]
        public double Creditor { get; set; }

        [Display(Name = "مانده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Balance { get; set; }

        [Display(Name = "حساب")]
        [ForeignKey(nameof(BankAccount))]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long BankAccountId { get; set; }
        public virtual BankAccount BankAccount { get; set; }

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
