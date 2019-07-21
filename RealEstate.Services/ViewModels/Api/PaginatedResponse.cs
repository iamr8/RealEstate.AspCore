using Newtonsoft.Json;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.Api
{
    public class PaginatedResponse<TModel> where TModel : class
    {
        [JsonProperty("p")]
        public int PageNumber { get; set; }

        [JsonProperty("pgs")]
        public int Pages { get; set; }

        [JsonProperty("itm")]
        public List<TModel> Items { get; set; }
    }
}