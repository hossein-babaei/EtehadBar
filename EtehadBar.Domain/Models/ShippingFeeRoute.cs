using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class ShippingFeeRoute
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "عنوان (اختیاری)")]
        [MaxLength(256, ErrorMessage = "{0} حداکثر باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "مبدأ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long OriginId { get; set; }
        public LoadRoutes Origin { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long DestinationId { get; set; }
        public LoadRoutes Destination { get; set; }

        [Display(Name = "گروه قیمت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long ShippingFeeGroupId { get; set; }
        public ShippingFeeGroup ShippingFeeGroup { get; set; }

        public string CreatorId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string EditorId { get; set; }
        public DateTime? EditDate { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
