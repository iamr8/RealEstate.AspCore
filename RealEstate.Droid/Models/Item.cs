using Newtonsoft.Json;

namespace RealEstate.Droid.Models
{
    public class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}