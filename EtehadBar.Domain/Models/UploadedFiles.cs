using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    public class UploadedFiles
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام")]
        [StringLength(50)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; }

        [Display(Name = "نوع")]
        [StringLength(10)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Type { get; set; }
    }
}
