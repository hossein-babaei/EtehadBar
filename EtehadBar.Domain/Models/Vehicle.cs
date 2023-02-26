using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Vehicle
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Type { get; set; }

        [Display(Name = "اعداد سمت چپ پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LeftNumber { get; set; }

        [Display(Name = "حرف پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(1, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string NumberWord { get; set; }

        [Display(Name = "اعداد سمت راست پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(3, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string RightNumber { get; set; }

        [Display(Name = "کد استان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string IranStateNumber { get; set; }

        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool Status { get; set; }

        [Display(Name = "نام و نام خانوادگی مالک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128)]
        public string VehicleOwnerFullname { get; set; }

        [Display(Name = "نام بانک")]
        [StringLength(64)]
        public string AccountBankName { get; set; }

        [Display(Name = "شماره حساب بانکی")]
        [StringLength(64)]
        public string BankAccountNumber { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<LoadFactor> LoadFactors { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
