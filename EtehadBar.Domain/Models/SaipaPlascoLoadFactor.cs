using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    [Index(nameof(Sequence), IsUnique = true)]
    public class SaipaPlascoLoadFactor
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Display(Name = "ترتیب")]
        public long Sequence { get; set; }

        [Required]
        public long LoadFactorId { get; set; }
        public virtual LoadFactor LoadFactor { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
