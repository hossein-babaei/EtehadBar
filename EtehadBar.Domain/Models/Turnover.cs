using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class Turnover
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "تاریخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DateTime Date { get; set; }

        [Display(Name = "شرح")]
        public string Description { get; set; }

        [Display(Name = "دریافت")]
        public double Debtor { get; set; }

        [Display(Name = "پرداخت")]
        public double Creditor { get; set; }

        public string Attachments { get; set; }

        [Required]
        public DateTime CreateDatetime { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string CreatorId { get; set; }

        public DateTime? EditDatetime { get; set; }

        [StringLength(450)]
        public string EditorId { get; set; }

        [Required]
        [ForeignKey(nameof(TurnoverProfile))]
        public long TurnoverProfileId { get; set; }
        public TurnoverProfile TurnoverProfile { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
