using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Bill
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام دریافت کننده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string ReceiverName { get; set; }

        [Display(Name = "شماره فیش")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(64, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string BillNo { get; set; }

        [Display(Name = "شماره رسید")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(64, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string BankBillNo { get; set; }

        [Display(Name = "تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime Date { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [Display(Name = "توضیحات")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "نوع هزینه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string BillType { get; set; } // definition table

        [Display(Name = "نام بانک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string BankBranch { get; set; } // definition table

        [Display(Name = "تقویم کاری")]
        [ForeignKey(nameof(BankBranch))]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }

        [Display(Name = "پلاک")]
        [ForeignKey(nameof(Vehicle))]
        public long? VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [Required]
        public DateTime CreateDatetime { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string CreatorId { get; set; }

        public DateTime? EditDatetime { get; set; }

        [StringLength(450)]
        public string EditorId { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
