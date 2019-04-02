using RealEstate.Base;
using RealEstate.ViewModels.Json;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class DealPaymentViewModel : BaseViewModel
    {
        public DealPaymentJsonViewModel Detail { get; set; }

        public List<PictureViewModel> Pictures { get; set; }
    }
}