using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Contract
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "موضوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Subject { get; set; }

        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Number { get; set; }

        [Display(Name = "از تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تا تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(ParentContract))]
        public long? ParentContractId { get; set; }
        public virtual Contract ParentContract { get; set; }

        public virtual ICollection<Contract> ContractAddons { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ICollection<LoadFactor> LoadFactors { get; set; }
        public virtual ICollection<ShippingFee> ShippingFees { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
