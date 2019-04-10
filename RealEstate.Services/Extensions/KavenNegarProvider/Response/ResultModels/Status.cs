using Newtonsoft.Json;
using RealEstate.Services.Extensions.KavenNegarProvider.Enums;

namespace RealEstate.Services.Extensions.KavenNegarProvider.Response.ResultModels
{
    public class Status
    {
        [JsonProperty("messageId")]
        public long MessageId { get; set; }

        [JsonProperty("status")]
        public MessageStatus State { get; set; }

        [JsonProperty("statusText")]
        public string Message { get; set; }
    }
}