using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class ShippingFeeLoadType
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Name { get; set; }

        public ICollection<ShippingFeeGroup> ShippingFeeGroups { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
