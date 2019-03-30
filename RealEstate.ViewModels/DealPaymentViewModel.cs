using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class DealPaymentViewModel : BaseViewModel
    {
        [JsonProperty("tp")]
        public decimal Tip { get; set; }

        [JsonProperty("cms")]
        public decimal Commission { get; set; }

        [JsonProperty("pd")]
        public DateTime PayDate { get; set; }

        [JsonProperty("txt")]
        public string Text { get; set; }

        public List<PictureViewModel> Pictures { get; set; }
    }
}