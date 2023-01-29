using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class UploadedFiles
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام")]
        [StringLength(50)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; }

        [Display(Name = "نوع")]
        [StringLength(10)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Type { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
