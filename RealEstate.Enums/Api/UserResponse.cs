using Newtonsoft.Json;

namespace RealEstate.Base.Api
{
    public class UserResponse
    {
        [JsonProperty("r")]
        public string Role { get; set; }

        [JsonProperty("mn")]
        public string MobileNumber { get; set; }

        [JsonProperty("fn")]
        public string FirstName { get; set; }

        [JsonProperty("ln")]
        public string LastName { get; set; }

        [JsonProperty("ui")]
        public string UserId { get; set; }

        [JsonProperty("un")]
        public string Username { get; set; }
    }
}