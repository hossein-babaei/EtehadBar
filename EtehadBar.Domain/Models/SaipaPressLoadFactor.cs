using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class SaipaPressLoadFactor
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EntryNumber { get; set; }

        [Required]
        [StringLength(128)]
        public string LoadType { get; set; }

        [Required]
        [StringLength(50)]
        public long LoadFactorId { get; set; }
        public virtual LoadFactor LoadFactor { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
