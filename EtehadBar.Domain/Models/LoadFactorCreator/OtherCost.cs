using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models.LoadFactorCreator
{
    public class OtherCost
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string DriverName { get; set; }

        [Display(Name = "اعداد سمت چپ پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string LeftNumber { get; set; }

        [Display(Name = "حرف پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(1, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string NumberWord { get; set; }

        [Display(Name = "اعداد سمت راست پلاک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(3, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string RightNumber { get; set; }

        [Display(Name = "کد استان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(2, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string IranStateNumber { get; set; }

        [Display(Name = "مبلغ (ریال)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public double Amount { get; set; }

        [StringLength(450)]
        public string AdminId { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string EditorId { get; set; }
        public DateTime? EditDateTime { get; set; }

        [Required]
        [Display(Name = "تقویم کاری")]
        public long CalendarId { get; set; }
        public virtual Calendar Calendar { get; set; }
    }
}
