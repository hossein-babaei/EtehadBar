using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    public class SaipaPressLoadFactor
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(50)]
        public string EntryNumber { get; set; }

        [Required]
        [StringLength(128)]
        public string LoadType { get; set; }

        [Required]
        [StringLength(50)]
        public string LoadFactorId { get; set; }
        public virtual LoadFactor LoadFactor { get; set; }
    }
}
