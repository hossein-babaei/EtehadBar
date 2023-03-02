using System;
using System.ComponentModel.DataAnnotations;

namespace EtehadBar.Domain.Models
{
    public class Config
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "تلفن")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Tel { get; set; }

        [Display(Name = "ایمیل")]
        [StringLength(128, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Email { get; set; }

        [Display(Name = "آدرس")]
        [StringLength(512, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Address { get; set; }

        [Display(Name = "مرکز پیام")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string SmsCenter { get; set; }

        [Display(Name = "نام کاربری پیامک")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string SmsUser { get; set; }

        [Display(Name = "کلمه عبور پیامک")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string SmsPass { get; set; }

        [Display(Name = "دامنه SMTP")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MailSmtpDomain { get; set; }

        [Display(Name = "نام کاربری ایمیل")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MailUserName { get; set; }

        [Display(Name = "کلمه عبور ایمیل")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MailPassword { get; set; }

        [Display(Name = "عنوان ایمیل")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MailDisplayName { get; set; }

        [Display(Name = "دامنه سایت")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Domain { get; set; }

        [Display(Name = "سال جاری")]
        public string Year { get; set; }

        [Display(Name = "مالیات ارزش افزوده (%)")]
        public double VAT { get; set; }

        [Display(Name = "مالیات تکلیفی (%)")]
        public double WithholdingTax { get; set; }

        [Required]
        [StringLength(36)]
        public string RowId { get; set; } = Guid.NewGuid().ToString();
    }
}
