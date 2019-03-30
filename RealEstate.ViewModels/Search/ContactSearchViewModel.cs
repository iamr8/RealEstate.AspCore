using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RealEstate.Resources;

namespace RealEstate.ViewModels.Search
{
    public class ContactSearchViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [JsonProperty("nm")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [JsonProperty("mob")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [JsonProperty("phn")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [JsonProperty("ad")]
        public string Address { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}