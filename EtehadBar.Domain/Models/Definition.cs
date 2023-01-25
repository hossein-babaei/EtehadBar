using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    public class Definition
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public DefinitionType DefinitionType { get; set; } //DefinitionTypeEnum
    }
}
