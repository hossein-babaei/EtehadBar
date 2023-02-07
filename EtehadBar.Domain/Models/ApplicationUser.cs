using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace EtehadBar.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [StringLength(128, MinimumLength = 2)]
        public string Firstname { get; set; }

        [PersonalData]
        [StringLength(128, MinimumLength = 2)]
        public string Lastname { get; set; }

        public DateTime Birth { get; set; }

        public DateTime RegisterDate { get; set; } = DateTime.Now;

        [StringLength(256)]
        public string LockoutReason { get; set; }

        [Display(Name = "کد ملی")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} باید {1} رقم باشد.")]
        public string NationalId { get; set; }

        [Display(Name = "تلفن ثابت")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} باید {1} رقم باشد.")]
        public string Tel { get; set; }

        [Display(Name = "آدرس")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Address { get; set; }

        [StringLength(50)]
        public string Avatar { get; set; }

        public bool Gender { get; set; }

        [Display(Name = "نام بانک")]
        [StringLength(64)]
        public string AccountBankName { get; set; }

        [Display(Name = "شماره حساب بانکی")]
        [StringLength(64)]
        public string BankAccountNumber { get; set; }

        public ApplicationRoleType Role { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<Cost> Costs { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<LoadFactor> LoadFactors { get; set; }
    }
}
