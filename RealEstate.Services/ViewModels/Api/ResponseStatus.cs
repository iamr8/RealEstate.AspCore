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

        public ResponsesWrapper<T> ToResponsesWrapperOf<T>() where T : class
        {
            var result = new ResponsesWrapper<T>
            {
                Success = this.Success,
                Message = this.Message
            };
            return result;
        }

        public ResponseWrapper<T> ToResponseWrapperOf<T>() where T : class
        {
            var result = new ResponseWrapper<T>
            {
                Success = this.Success,
                Message = this.Message
            };
            return result;
        }
    }
}