using System.Collections.Generic;
using RealEstate.Base;
using RealEstate.Services.ViewModels.Json;

namespace RealEstate.Services.ViewModels
{
    public class DealPaymentViewModel : BaseViewModel
    {
        public DealPaymentJsonViewModel Detail { get; set; }

        public List<PictureViewModel> Pictures { get; set; }
    }
}