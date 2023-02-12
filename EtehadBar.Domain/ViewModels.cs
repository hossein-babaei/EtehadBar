using EtehadBar.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtehadBar.Domain
{
    public class CreateContractVM
    {
        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "موضوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Subject { get; set; }

        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Number { get; set; }

        [Display(Name = "الحاقیه ای است برای قرارداد")]
        public long? ParentContractId { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public long CustomerId { get; set; }
    }

    public class EditContractVM
    {
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long Id { get; set; }

        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "موضوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Subject { get; set; }

        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Number { get; set; }
    }

    public class CreateCalendarVM
    {
        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "مالیات ارزش افزوده (%)")]
        public double VAT { get; set; }

        [Display(Name = "سپرده بیمه (%)")]
        public double LoadFactorDeductions { get; set; }

        [Display(Name = "مالیات تکلیفی (%)")]
        public double WithholdingTax { get; set; }
    }

    public class EditCalendarVM
    {
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long Id { get; set; }

        [Display(Name = "روز شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartDay { get; set; }

        [Display(Name = "ماه شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartMonth { get; set; }

        [Display(Name = "سال شروع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int StartYear { get; set; }

        [Display(Name = "روز پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndDay { get; set; }

        [Display(Name = "ماه پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndMonth { get; set; }

        [Display(Name = "سال پایان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int EndYear { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, ErrorMessage = "{0} باید {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "مالیات ارزش افزوده (%)")]
        public double VAT { get; set; }

        [Display(Name = "سپرده بیمه (%)")]
        public double LoadFactorDeductions { get; set; }

        [Display(Name = "مالیات تکلیفی (%)")]
        public double WithholdingTax { get; set; }
    }

    public class LoadFactorConfigVM
    {
        public double VAT { get; set; }
        public double LoadFactorDeductions { get; set; }
        public double WithholdingTax { get; set; }
    }

    public class PaymentVM
    {
        public long Id { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "مبلغ (ریال)")]
        public double Amount { get; set; }

        [Display(Name = "شرح پرداخت")]
        public string Description { get; set; }

        [Display(Name = "نوع پرداخت")]
        public PaymentType PaymentType { get; set; }

        public string Picture { get; set; }

        public string AdminId { get; set; }

        [Display(Name = "کاربر سیستم")]
        public string AdminName { get; set; }


        [Display(Name = "خودرو")]
        public long? VehicleId { get; set; }
        public string Vehicle { get; set; }

        [Display(Name = "کارمند")]
        public string UserId { get; set; }
        public string UserFullname { get; set; }
    }

    public class CreateUserVM
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Lastname { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public bool Gender { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید متنی بین {2} الی {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string BirthString { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} باید {1} رقم باشد.")]
        public string NationalId { get; set; }

        [Display(Name = "نام بانک")]
        public string AccountBankName { get; set; }

        [Display(Name = "شماره حساب")]
        public string BankAccountNumber { get; set; }

        public ApplicationRoleType Role { get; set; }

        [Display(Name = "تصویر پرسنلی")]
        public IFormFile Pic { get; set; }
    }

    public class CreateDriverVM
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Lastname { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public bool Gender { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string BirthString { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} باید {1} رقم باشد.")]
        public string NationalId { get; set; }

        public ApplicationRoleType Role { get; set; }

        [Display(Name = "تصویر پرسنلی")]
        public IFormFile Pic { get; set; }
    }

    public class LoginVM
    {
        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "من را به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class RegisterVM
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Lastname { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public bool Gender { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید متنی بین {2} الی {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }
    }

    public class ConfirmPhoneNumberVM
    {
        [Display(Name = "کد فعالسازی")]
        [Required(ErrorMessage = "لطفا کد را وارد نمائید.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "کد وارد شده صحیح نیست.")]
        public string Code { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "لطفا شماره تلفن همراه خود را وارد نمائید.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "شماره تلفن همراه باید 11 رقم باشد.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string PhoneNumber { get; set; }
    }

    public class SendNewCodeVM
    {
        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "لطفا شماره تلفن همراه خود را وارد نمائید.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "شماره تلفن همراه باید 11 رقم باشد.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string PhoneNumber { get; set; }
    }

    public class ResetPasswordVM
    {
        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید متنی بین {2} الی {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "کد 6 رقمی پیامک شده به شما")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "کد وارد شده صحیح نیست.")]
        public string Code { get; set; }
    }

    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور فعلی")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} بین {2} تا {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور جدید")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور جدید")]
        [Compare("NewPassword", ErrorMessage = "{0} با کلمه عبور جدید وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }
    }

    public class EditUserVM
    {
        public string Id { get; set; }

        public DateTime Birth { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string BirthString { get; set; }

        [Display(Name = "نام")]
        [StringLength(128, ErrorMessage = "فیلد {0} باید بین {2} تا {1} حرف باشد.", MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [StringLength(128, ErrorMessage = "فیلد {0} باید بین {2} تا {1} حرف باشد.", MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(10, ErrorMessage = "فیلد {0} باید {2} رقم باشد..", MinimumLength = 10)]
        [Display(Name = "کد ملی")]
        public string NationalId { get; set; }

        [Display(Name = "تلفن همراه")]
        [StringLength(11, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 11)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "تایید تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست.")]
        [StringLength(256, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 5)]
        public string Email { get; set; }

        [Display(Name = "تایید ایمیل")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "تلفن ثابت")]
        [StringLength(11, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 11)]
        public string Tel { get; set; }

        [Display(Name = "عکس پرسنلی")]
        public string Avatar { get; set; }

        [Display(Name = "نام بانک")]
        public string AccountBankName { get; set; }

        [Display(Name = "شماره حساب")]
        public string BankAccountNumber { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }
    }

    public class CreateAccountBookVM
    {
        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Number { get; set; }

        [Display(Name = "شماره فاکتور")]
        public string FactorNumber { get; set; }

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerId { get; set; }
    }

    public class EditAccountBookVM
    {
        [Required]
        public long Id { get; set; }

        [Display(Name = "شماره")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Number { get; set; }

        [Display(Name = "شماره فاکتور")]
        public string FactorNumber { get; set; }

        [Display(Name = "مشتری")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public long CustomerId { get; set; }
    }
}
