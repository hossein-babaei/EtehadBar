using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    [Index(nameof(Sequence), IsUnique = true)]
    public class SaipaPressLoadFactor
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Display(Name = "ترتیب")]
        public long Sequence { get; set; }

        [StringLength(50)]
        public string EntryNumber { get; set; }

        [Required]
        [StringLength(128)]
        public string LoadType { get; set; }

        [Required]
        public SaipaPressLoadType PressFloorType { get; set; } = SaipaPressLoadType.OneFloor;

        [Required]
        public long LoadFactorId { get; set; }
        public LoadFactor LoadFactor { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
