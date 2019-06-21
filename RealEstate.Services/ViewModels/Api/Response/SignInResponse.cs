using System.Collections.Generic;
using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Api.Response
{
    public class SignInResponse
    {
        [JsonProperty("t")]
        public string Token { get; set; }

        [JsonProperty("fn")]
        public string FirstName { get; set; }

        [JsonProperty("ln")]
        public string LastName { get; set; }

        [JsonProperty("uic")]
        public List<string> UserItemCategories { get; set; }

        [JsonProperty("upc")]
        public List<string> UserPropertyCategories { get; set; }

        [JsonProperty("ed")]
        public List<string> EmployeeDivisions { get; set; }

        [JsonProperty("ei")]
        public string EmployeeId { get; set; }

        [JsonProperty("r")]
        public string Role { get; set; }

        [JsonProperty("ep")]
        public string EncryptedPassword { get; set; }
        [JsonProperty("mn")]
        public string MobileNumber { get; set; }
        [JsonProperty("ui")]
        public string UserId { get; set; }
    }
}