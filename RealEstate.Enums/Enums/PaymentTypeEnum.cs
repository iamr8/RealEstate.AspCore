using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum PaymentTypeEnum
    {
        // Credit => Beneficiary + Gift - Forfeit - Advance = Salary
        // Checkout => Salary ( Above items updates paymentId to checkoutId )
        // Remain => Credit - Checkout

        [Display(ResourceType = typeof(SharedResource), Name = "Salary")]
        Salary, // حقوق

        [Display(ResourceType = typeof(SharedResource), Name = "Gift")]
        Gift, // هدیه

        [Display(ResourceType = typeof(SharedResource), Name = "Forfeit")]
        Forfeit, // جریمه

        [Display(ResourceType = typeof(SharedResource), Name = "Advance")]
        Advance, // مساعده

        [Display(ResourceType = typeof(SharedResource), Name = "Beneficiary")]
        Beneficiary, // ذینفغ از قرارداد

        [Display(ResourceType = typeof(SharedResource), Name = "Checkout")]
        Checkout
    }
}