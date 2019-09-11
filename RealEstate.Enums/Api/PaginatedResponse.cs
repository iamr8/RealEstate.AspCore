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
        public new int CurrentPage { get; set; }

        [JsonProperty("pgs")]
        public new int Pages { get; set; }

        [JsonProperty("rs")]
        public new int Rows { get; set; }
    }
}