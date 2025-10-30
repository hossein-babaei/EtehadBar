using EtehadBar.Domain.Models.LoadFactorCreator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Display(Name = "خواب بار")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool HasLoadSleep { get; set; } = false;

        [Display(Name = "سپرده بیمه (%)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double LoadFactorDeductions { get; set; }

        [Display(Name = "بانک فعال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Definition))]
        public long ActiveBank { get; set; }
        public Definition Definition { get; set; }

        public ICollection<AccountBook> AccountBooks { get; set; }
        public ICollection<Contract> Contracts { get; set; }
        public ICollection<OtherCost> OtherCosts { get; set; }
        public ICollection<CustomerIncome> CustomerIncomes { get; set; }
        public ICollection<CustomerPeriodicBalanceSummary> CustomerPeriodicBalanceSummaries { get; set; }
        public ICollection<LoadFactorNovin> LoadFactorNovins { get; set; }
        public ICollection<CustomerFactor> CustomerFactors { get; set; }
        public ICollection<TurnoverProfile> TurnoverProfiles { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
