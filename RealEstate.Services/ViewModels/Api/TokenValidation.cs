using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Api
{
    public class TokenValidation : Response.Response
    {
        [JsonProperty("ui")]
        public string UserId { get; set; }
    }
}