using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class SazehGostarLoadFactor
    {
        [Key]
        [StringLength(50)]
        public long Id { get; set; }

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

        [StringLength(50)]
        public string Status { get; set; }

        [Required]
        [StringLength(50)]
        public long LoadFactorId { get; set; }
        public virtual LoadFactor LoadFactor { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
