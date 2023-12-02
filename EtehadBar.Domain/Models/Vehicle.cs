using EtehadBar.Domain.Models.LoadFactorCreator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Vehicle
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Type { get; set; }

        [Display(Name = "اعداد سمت چپ پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LeftNumber { get; set; }

        [Display(Name = "حرف پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(1, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string NumberWord { get; set; }

        [Display(Name = "اعداد سمت راست پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(3, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string RightNumber { get; set; }

        [Display(Name = "کد استان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string IranStateNumber { get; set; }

        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool Status { get; set; }

        [Display(Name = "نام و نام خانوادگی مالک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128)]
        public string VehicleOwnerFullname { get; set; }

        [Display(Name = "کد ملی")]
        [StringLength(10, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string NationalNumber { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [StringLength(12, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Phonenumber { get; set; }

        [Display(Name = "حقیقی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public bool RealStatus { get; set; }

        public DateTime CreateDatetime { get; set; } = DateTime.Now;
        public string CreatorId { get; set; }
        public DateTime? EditDatetime { get; set; }
        public string EditorId { get; set; }

        public ICollection<Bill> Bills { get; set; }
        public ICollection<OtherCost> OtherCosts { get; set; }
        public ICollection<LoadFactor> LoadFactors { get; set; }
        public ICollection<VehicleBalance> VehicleBalances { get; set; }
        public ICollection<LoadFactorNovin> LoadFactorNovins { get; set; }
        public ICollection<VehicleBankAccount> VehicleBankAccounts { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
