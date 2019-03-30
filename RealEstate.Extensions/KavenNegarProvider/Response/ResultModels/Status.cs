using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RealEstate.Extensions.KavenNegarProvider.Enums;

namespace RealEstate.Extensions.KavenNegarProvider.Response.ResultModels
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