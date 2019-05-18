using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum PaymentTypeEnum
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Gift")]
        Gift, // هدیه

        [Display(ResourceType = typeof(SharedResource), Name = "Forfeit")]
        Forfeit, // جریمه

        [Display(ResourceType = typeof(SharedResource), Name = "Advance")]
        Advance, // مساعده

        [Display(ResourceType = typeof(SharedResource), Name = "Beneficiary")]
        Beneficiary, // ذینفغ از قرارداد

        [Display(ResourceType = typeof(SharedResource), Name = "Pay")]
        Pay, // پرداخت,

        [Display(ResourceType = typeof(SharedResource), Name = "FixedSalary")]
        FixedSalary,

        [Display(ResourceType = typeof(SharedResource), Name = "Remain")]
        Remain
    }
}