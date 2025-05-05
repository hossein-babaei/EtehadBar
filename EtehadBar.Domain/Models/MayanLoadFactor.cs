using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Models
{
    [Index(nameof(RowId), IsUnique = true)]
    public class MayanLoadFactor
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "هزینه بارنامه دولتی")]
        public double? LoadFactorGovCost { get; set; }

        [Display(Name = "مبلغ ترافیک")]
        public double? TrafficCost { get; set; }

        [Display(Name = "مبدا")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Origin { get; set; }

        [Display(Name = "مقصد")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Destination { get; set; }

        [Display(Name = "کد ملی راننده")]
        public string DriverNationalNumber { get; set; }

        [Display(Name = "نوع خودرو")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string VehicleType { get; set; }

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

        [Required]
        public long LoadFactorId { get; set; }
        public LoadFactor LoadFactor { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
