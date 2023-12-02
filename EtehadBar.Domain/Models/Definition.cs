using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Definition
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DefinitionType DefinitionType { get; set; } //DefinitionTypeEnum

        public ICollection<Customer> Customers { get; set; }
        public ICollection<VehicleBankAccount> VehicleBankAccounts { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
