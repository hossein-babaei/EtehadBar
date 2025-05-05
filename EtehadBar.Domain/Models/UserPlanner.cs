using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class UserPlanner
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; } = DateTime.Now;

        [StringLength(450)]
        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<UserPlannerItem> UserPlannerItems { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
