using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Driver
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Lastname { get; set; }

        [Display(Name = "نام بانک")]
        [StringLength(64, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string AccountBankName { get; set; }

        [Display(Name = "شماره حساب بانکی")]
        [StringLength(64, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string BankAccountNumber { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [StringLength(12, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Phonenumber { get; set; }

        [Display(Name = "کد ملی")]
        [StringLength(10, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string NationalNumber { get; set; }

        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool IsActive { get; set; } = true;

        public DateTime CreateDatetime { get; set; } = DateTime.Now;
        public string CreatorId { get; set; }
        public DateTime? EditDatetime { get; set; }
        public string EditorId { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
