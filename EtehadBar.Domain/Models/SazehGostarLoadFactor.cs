using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    [Index(nameof(Sequence), IsUnique = true)]
    public class SazehGostarLoadFactor
    {
        [Key]
        [StringLength(50)]
        public long Id { get; set; }

        [Required]
        [Display(Name = "ترتیب")]
        public long Sequence { get; set; }

        [Required]
        [StringLength(128)]
        public string RegisterCode { get; set; }

        [Required]
        [StringLength(128)]
        public string Certain { get; set; }

        [Required]
        [StringLength(128)]
        public string Nature { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }

        [StringLength(50)]
        public string DetailedCostCenter { get; set; }

        public double? InsuranceAmount { get; set; }

        [Required]
        public SazehGostarLoadType SazehLoadType { get; set; }

        [Required]
        public long LoadFactorId { get; set; }
        public virtual LoadFactor LoadFactor { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
