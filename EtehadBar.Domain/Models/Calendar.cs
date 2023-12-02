using EtehadBar.Domain.Models.LoadFactorCreator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Calendar
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "ترتیب")]
        public int Sequence { get; set; }

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

        public string CreatorId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string EditorId { get; set; }
        public DateTime? EditDate { get; set; }

        public ICollection<Bill> Bills { get; set; }
        public ICollection<Cost> Costs { get; set; }
        public ICollection<OtherCost> OtherCosts { get; set; }
        public ICollection<LoadFactor> LoadFactors { get; set; }
        public ICollection<AccountBook> AccountBooks { get; set; }
        public ICollection<CustomerFactor> CustomerFactors { get; set; }
        public ICollection<CustomerIncome> CustomerIncomes { get; set; }
        public ICollection<FreeLoadFactor> FreeLoadFactors { get; set; }
        public ICollection<VehicleBalance> VehicleBalances { get; set; }
        public ICollection<LoadFactorNovin> LoadFactorNovins { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
