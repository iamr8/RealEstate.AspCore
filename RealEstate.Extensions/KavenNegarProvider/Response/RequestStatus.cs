using Newtonsoft.Json;

namespace RealEstate.Extensions.KavenNegarProvider.Response
{
    public class RequestStatus
    {
        [JsonProperty("status")]
        public int StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}