using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Cheque
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "تاریخ سررسید")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime Date { get; set; }

        [Display(Name = "تاریخ دریافت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime RecieveDate { get; set; } = DateTime.Now;

        [Display(Name = "تاریخ ارسال به بانک")]
        public DateTime? SendToBankDate { get; set; }

        [Display(Name = "تاریخ پاس شدن")]
        public DateTime? PassDate { get; set; }

        [Display(Name = "صادر کننده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(64, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Issuer { get; set; }

        [Display(Name = "شماره چک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(32, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Number { get; set; }

        [Display(Name = "بانک صادرکننده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(32, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string BankOfOrigin { get; set; }

        [Display(Name = "بانک عامل")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(32, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string SendToBankName { get; set; }

        [Display(Name = "توضیحات")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "وضعیت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public ChequeStatus Status { get; set; } = (byte)ChequeStatus.NotPassed;

        [Display(Name = "مشتری")]
        [ForeignKey(nameof(Customer))]
        public long? CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
