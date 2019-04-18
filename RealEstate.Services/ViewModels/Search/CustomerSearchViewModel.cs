using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Search
{
    public class CustomerSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [JsonProperty("nm")]
        [SearchParameter("customerName")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [JsonProperty("mob")]
        [SearchParameter("customerMobile")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [JsonProperty("phn")]
        [SearchParameter("customerPhone")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [JsonProperty("ad")]
        [SearchParameter("customerAddress")]
        public string Address { get; set; }

        [JsonProperty("id")]
        [SearchParameter("customerId")]
        public string Id { get; set; }
    }
}