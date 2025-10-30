using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class BillDetail
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام دریافت کننده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string ReceiverName { get; set; }

        [Display(Name = "شماره حساب دریافت کننده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(64, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string ReceiverBankAccount { get; set; }

        [ForeignKey(nameof(Bill))]
        [Required]
        public long BillId { get; set; }
        public Bill Bill { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
