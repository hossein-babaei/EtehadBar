using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class ShippingFee
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "مبدأ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long OriginId { get; set; }
        public virtual LoadRoutes Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long DestinationId { get; set; }
        public virtual LoadRoutes Destination { get; set; }

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

        [Display(Name = "نرخ تناژ اضافه (ریال)")]
        public double? TonnagePrice { get; set; } = 0;

        [Display(Name = "نرخ تناژ اضافه راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; } = 0;

        [Display(Name = "نوع نرخ")]
        public ShippingFeeType ShippingFeeType { get; set; } = ShippingFeeType.Normal;

        public long ShippingFeeLoadTypeId { get; set; }
        public virtual ShippingFeeLoadType ShippingFeeLoadType { get; set; }

        [Display(Name = "قرارداد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long ContractId { get; set; }
        public virtual Contract Contract { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
