using RealEstate.Base;
using RealEstate.Base.Enums;

namespace RealEstate.ViewModels.Input
{
    public class PaymentInputViewModel : BaseViewModel
    {
        public double Value { get; set; }
        public PaymentTypeEnum Type { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
    }
}