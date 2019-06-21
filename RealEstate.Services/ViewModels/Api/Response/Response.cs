using Newtonsoft.Json;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Api.Response
{
    public class Response<T>
    {
        [JsonProperty("m")]
        public string Message { get; set; }

        [JsonProperty("s")]
        public bool Success { get; set; }

        [JsonProperty("r")]
        public List<T> Result { get; set; }
    }

    public class Response
    {
        [JsonProperty("m")]
        public string Message { get; set; }

        [JsonProperty("s")]
        public bool Success { get; set; }
    }
}