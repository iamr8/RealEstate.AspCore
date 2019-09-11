using System.Collections.Generic;
using Newtonsoft.Json;

namespace RealEstate.Base.Api
{
    public class PaginatedResponse<TModel> : PaginatedResponse where TModel : class
    {
        [JsonProperty("itm")]
        public List<TModel> Items { get; set; }
    }

    public class PaginatedResponse : PaginationViewModel
    {
        [JsonProperty("p")]
        public int CurrentPage { get; set; }

        [JsonProperty("pgs")]
        public int Pages { get; set; }

        [JsonProperty("rs")]
        public int Rows { get; set; }
    }
}