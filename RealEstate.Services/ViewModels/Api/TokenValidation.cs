using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Api
{
    public class TokenValidation
    {
        [JsonProperty("m")]
        public string Message { get; set; }

        [JsonProperty("s")]
        public bool Success { get; set; }

        [JsonProperty("ui")]
        public string UserId { get; set; }
    }
}