using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    public class Calendar
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "از تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تا تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime EndDate { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "مالیات ارزش افزوده (%)")]
        public double VAT { get; set; }

        [Display(Name = "سپرده بیمه (%)")]
        public double LoadFactorDeductions { get; set; }

        [Display(Name = "مالیات تکلیفی (%)")]
        public double WithholdingTax { get; set; }

        public virtual ICollection<Cost> Costs { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<LoadFactor> LoadFactors { get; set; }
        public virtual ICollection<CustomerIncome> CustomerIncomes { get; set; }
    }
}
