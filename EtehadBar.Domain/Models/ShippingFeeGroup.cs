using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class ShippingFeeGroup
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "عنوان (اختیاری)")]
        [MaxLength(256, ErrorMessage = "{0} حداکثر باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "مبدأ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
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

        [Display(Name = "نرخ تناژ اضافه تریلی (ریال)")]
        public double? TonnagePrice { get; set; }

        [Display(Name = "نرخ تناژ اضافه تریلی راننده (ریال)")]
        public double? DriverTonnagePrice { get; set; }

        [Display(Name = "نوع بار")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long ShippingFeeLoadTypeId { get; set; }
        public ShippingFeeLoadType ShippingFeeLoadType { get; set; }

        [Display(Name = "قرارداد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long ContractId { get; set; }
        public Contract Contract { get; set; }

        public string CreatorId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string EditorId { get; set; }
        public DateTime? EditDate { get; set; }

        public ICollection<ShippingFeeRoute> ShippingFeeRoutes { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
