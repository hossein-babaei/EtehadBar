using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class UserPlannerItem
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Priority { get; set; } = 1;

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Content { get; set; }

        [Required]
        public long UserPlannerId { get; set; }
        public UserPlanner UserPlanner { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
