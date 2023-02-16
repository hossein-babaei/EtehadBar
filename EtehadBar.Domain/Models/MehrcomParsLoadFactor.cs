using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    [Index(nameof(Sequence), IsUnique = true)]
    public class MehrcomParsLoadFactor
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Display(Name = "ترتیب")]
        public long Sequence { get; set; }

        [Display(Name = "شماره بارنامه دولتی برگشتی")]
        public string LoadNumberGovReturn { get; set; }

        [Required]
        [Display(Name = "بار")]
        public bool Load { get; set; } = false;

        [Required]
        [Display(Name = "پالت")]
        public bool Palette { get; set; } = false;

        [Required]
        [Display(Name = "برگشتی")]
        public bool Return { get; set; } = false;

        [Display(Name = "نرخ باسکول")]
        public double? WeighbridgePrice { get; set; }

        [Display(Name = "میزان خواب (دقیقه)")]
        public double? LoadSleepTime { get; set; }

        [Display(Name = "مبلغ دریافتی خواب")]
        public double? LoadSleepPrice { get; set; }

        [Display(Name = "مبلغ پرداختی خواب")]
        public double? DriverLoadSleepPrice{ get; set; }

        [Required]
        public long LoadFactorId { get; set; }
        public virtual LoadFactor LoadFactor { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
