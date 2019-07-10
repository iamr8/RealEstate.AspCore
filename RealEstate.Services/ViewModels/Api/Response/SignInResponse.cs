using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Api.Response
{
    public class SignInResponse
    {
        [JsonProperty("t")]
        public string Token { get; set; }
    }
}