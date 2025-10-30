using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class TurnoverProfile
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string FullName { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public TurnoverType TurnoverType { get; set; }

        [Display(Name = "نوع پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public TurnoverPaymentType TurnoverPaymentType { get; set; } = (byte)TurnoverPaymentType.Rial;

        [Display(Name = "نوبت پرداخت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(64, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string TurnoverTurnType { get; set; } = "هر دریافتی";

        [Display(Name = "درصد/مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double ProfitPercent { get; set; } = 0;

        [Display(Name = "شماره حساب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(64, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string BankAccount { get; set; }

        [Display(Name = "مالک حساب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string BankAccountOwner { get; set; }

        [Display(Name = "تاریخ شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "تاریخ انقضا")]
        public DateTime? ExpireDate { get; set; }

        [Display(Name = "توضیحات")]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [ForeignKey(nameof(Customer))]
        [Display(Name = "مشتری")]
        public long? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<Turnover> Turnovers { get; set; }
        public ICollection<TurnoverProfilePeriod> TurnoverProfilePeriods { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
