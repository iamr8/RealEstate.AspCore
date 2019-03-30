using Newtonsoft.Json;

namespace RealEstate.Extensions.KavenNegarProvider.Response
{
    public class Response<T>
    {
        [JsonProperty("return")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("entries")]
        public T Result { get; set; }
    }
}