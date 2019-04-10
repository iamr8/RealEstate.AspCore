using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Search
{
    public class ContactSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [JsonProperty("nm")]
        [SearchParameter("contactName")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [JsonProperty("mob")]
        [SearchParameter("contactMobile")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [JsonProperty("phn")]
        [SearchParameter("contactPhone")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [JsonProperty("ad")]
        [SearchParameter("contactAddress")]
        public string Address { get; set; }

        [JsonProperty("id")]
        [SearchParameter("contactId")]
        public string Id { get; set; }
    }
}