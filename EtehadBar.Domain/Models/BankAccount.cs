using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class BankAccount
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام بانک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(64, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string AccountBankName { get; set; }

        [Display(Name = "شماره حساب بانکی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(64, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string BankAccountNumber { get; set; }

        [Required]
        public DateTime CreateDatetime { get; set; } = DateTime.Now;

        [StringLength(450)]
        [ForeignKey(nameof(Owner))]
        public string OwnerUserId { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        public DateTime? EditDatetime { get; set; }

        [StringLength(450)]
        public string EditorId { get; set; }

        public virtual ICollection<BankAccountBook> BankAccountBooks { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
