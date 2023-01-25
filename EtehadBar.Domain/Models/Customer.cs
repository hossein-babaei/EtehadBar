using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

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

        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<CustomerIncome> CustomerIncomes { get; set; }
    }
}
