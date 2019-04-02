using RealEstate.Base;
using RealEstate.Base.Enums;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        public double Value { get; set; }

        public string Text { get; set; }
        public PaymentTypeEnum Type { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
    }
}