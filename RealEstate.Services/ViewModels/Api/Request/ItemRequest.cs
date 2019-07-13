using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RealEstate.Services.ViewModels.Api.Request
{
    public class ItemRequest
    {
        public string ItemCategory { get; set; }

        public string PropertyCategory { get; set; }
    }
}