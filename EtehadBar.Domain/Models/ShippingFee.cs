using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    public class ShippingFee
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "مبدأ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Destination { get; set; }

        [Display(Name = "خودرو")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Vehicle { get; set; }

        [Display(Name = "کرایه (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Price { get; set; }

        [Display(Name = "کرایه راننده (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double DriverPrice { get; set; }

        [StringLength(50)]
        [Display(Name = "قرارداد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string ContractId { get; set; }
        public virtual Contract Contract { get; set; }
    }
}
