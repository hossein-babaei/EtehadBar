using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Customer
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Name { get; set; }

        [Display(Name = "نوع مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public CustomerType CustomerType { get; set; } // customer type enum

        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool Status { get; set; }

        [Display(Name = "اضافه تناژ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool HasAddonTonnage { get; set; } = false;

        [Display(Name = "نوع بار")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool HasLoadType { get; set; } = false;

        public virtual ICollection<AccountBook> AccountBooks { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<CustomerIncome> CustomerIncomes { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
