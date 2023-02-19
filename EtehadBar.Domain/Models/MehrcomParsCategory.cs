using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class MehrcomParsCategory
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }

        public virtual ICollection<MehrcomParsLoadFactor> LoadFactors { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
