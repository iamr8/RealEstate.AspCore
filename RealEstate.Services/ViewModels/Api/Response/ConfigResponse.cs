using Newtonsoft.Json;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Api.Response
{
    public class ConfigResponse : UserResponse
    {
        [JsonProperty("uic")]
        public List<string> UserItemCategories { get; set; }

        [JsonProperty("upc")]
        public List<string> UserPropertyCategories { get; set; }

        [JsonProperty("ed")]
        public List<string> EmployeeDivisions { get; set; }
    }
}