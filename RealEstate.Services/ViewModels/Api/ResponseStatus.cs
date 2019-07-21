using Newtonsoft.Json;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Api
{
    public class ResponsesWrapper<T> : ResponseStatus
    {
        [JsonProperty("r")]
        public List<T> Result { get; set; }
    }

    public class ResponseWrapper<T> : ResponseStatus
    {
        [JsonProperty("r")]
        public T Result { get; set; }
    }

    public class ResponseStatus
    {
        [JsonProperty("m")]
        public string Message { get; set; }

        [JsonProperty("s")]
        public bool Success { get; set; }
    }
}