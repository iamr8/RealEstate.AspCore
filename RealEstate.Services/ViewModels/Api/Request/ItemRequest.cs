using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RealEstate.Services.ViewModels.Api.Request
{
    public class ItemRequest : RequestWrapper
    {
        public string ItemCategory { get; set; }

        public string PropertyCategory { get; set; }
    }
}