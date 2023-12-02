using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class VehicleBankAccount
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام صاحب حساب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Fullname { get; set; }

        [Display(Name = "شماره حساب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string AccountNumber { get; set; }

        [Display(Name = "خودرو")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Vehicle))]
        public long VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        [Display(Name = "نام بانک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [ForeignKey(nameof(Definition))]
        public long BankId { get; set; }
        public Definition Definition { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
