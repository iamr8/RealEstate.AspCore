using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum PersianDayOfWeek
    {
        [Display(Name = "شنبه")]
        Shanbeh = 0,

        [Display(Name = "یک‌شنبه")]
        YekShanbeh = 1,

        [Display(Name = "دوشنبه")]
        DoShanbeh = 2,

        [Display(Name = "سه‌شنبه")]
        SeShanbeh = 3,

        [Display(Name = "چهارشنبه")]
        ChaharShanbeh = 4,

        [Display(Name = "پنج‌شنبه")]
        PanjShanbeh = 5,

        [Display(Name = "جمعه")]
        Jomeh = 6,
    }
}