using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base
{
    public enum RegexPatterns
    {
        [Description("فقط حروف فارسی و خط فاصله مجاز است.")]
        [Display(Name = "^[\u0600-\u06FF ]+$")]
        PersianText,

        [Description("فقط اعداد انگلیسی در قالب شماره موبایل مجاز است: 09120000000")]
        [Display(Name = "(09)([0-9]){9}")]
        Mobile,

        [Description("فقط اعداد فارسی و انگلیسی در قالب شماره موبایل مجاز است: 09120000000")]
        [Display(Name = "((09)|(\u06F0\u06F9))([0-9]|[\u06F0-\u06F9]){9}")]
        MobileWitPersian,

        [Description("فقط اعداد فارسی و انگلیسی در قالب شماره موبایل همراه اول مجاز است: 09120000000")]
        [Display(Name = "((091)|(\u06F0\u06F9\u06F1))([0-9]|[\u06F0-\u06F9]){8}")]
        MobileNumberMci,

        [Description("فقط حروف و اعداد فارسی و انگلیسی و خط فاصله مجاز است")]
        [Display(Name = @"^[\u0600-\u06FF\da-zA-Z ]+$")]
        SafeString,

        [Description("فقط حروف و اعداد فارسی و انگلیسی و خط فاصله مجاز است")]
        [Display(Name = @"[\u0600-\u06FF\d\w_\-.]+")]
        SafeFilename,

        [Description("فقط حروف و اعداد فارسی و انگلیسی، نقطه، کاما و خط فاصله مجاز است.")]
        [Display(Name = @"^[\u0600-\u06FF\da-zA-Z0-9() .،-]+$")]
        SafeText,

        [Description("فقط اعداد مجاز می باشد.")]
        [Display(Name = @"^[\d]*$")]
        NumbersOnly,

        [Description("فقط حروف انگلیسی مجاز است.")]
        [Display(Name = "^[a-zA-Z]+$")]
        EnglishText,

        [Description("فقط اعداد انگلیسی در قالب شماره تلفن مجاز است: 021-234234")]
        [Display(Name = "[0-9]{1,}-[1-8]([0-9]){7}")]
        Phone,

        [Description("فقط از اعداد و حروف مجاز در نام اکانتهای شبکه های اجتماعی، مجاز است.  ")]
        [Display(Name = @"[\d\w@._-]+")]
        SocialAccount,

        [Description("فقط اعداد در قالب ساعت مجاز است: 00:30")]
        [Display(Name = "^([01]?[0-9]|2[0-3]):[0-5][0-9]?")]
        Time,

        [Description("فقط اعداد در قالب تاریخ ایرانی مجاز است: 1397/03/02")]
        [Display(Name = "13[0-9]{2}(/)((0([1-9]))|(1([0-2])))(/)((0[1-9])|([1-2][0-9])|(3[0-1]))")]
        IranDate,

        [Description("فقط حروف و اعداد فارسی و انگلیسی و خط تیره مجاز است.")]
        [Display(Name = @"^[\u0600-\u06FF\da-zA-Z0-9-]+$")]
        SafeTextSecured,

        [Description("فقط اعداد در قالب کد ملی مجاز است: 1741231234")]
        [Display(Name = "([0-9]{3})([0-9]{7})")]
        IranNationalCode,

        [Description("فقط اعداد و کاما، در قالب قیمت مجاز است: 2,000")]
        [Display(Name = @"[\d]{1,3}(,[\d]{3})+")]
        Price,

        [Description("فقط اعداد در قالب طول و عرض جغرافیایی مجاز است: 23.342444")]
        [Display(Name = @"^(?:\d+\.\d+)$")]
        Geolocation
    }
}