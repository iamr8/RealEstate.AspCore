using Newtonsoft.Json;
using RealEstate.Base;

namespace RealEstate.ViewModels.Json
{
    public class ApplicantJsonViewModel : BaseViewModel
    {
        [JsonProperty("cti")]
        public string ContactId { get; set; }

        [JsonProperty("ctc")]
        public string Name { get; set; }
    }
}