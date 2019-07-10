using System.Collections.Generic;
using Newtonsoft.Json;

namespace RealEstate.Services.ViewModels.Api
{
    public class ResponseWrapper<T> : ResponseWrapper
    {
        [JsonProperty("r")]
        public List<T> Result { get; set; }
    }

    public class ResponseWrapper
    {
        [JsonProperty("m")]
        public string Message { get; set; }

        [JsonProperty("s")]
        public bool Success { get; set; }
    }
}