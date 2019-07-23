using RealEstate.Base.Api;

namespace RealEstate.Services.ViewModels.Api.Request
{
    public class ItemRequest : PaginatedRequest
    {
        public string ItemCategory { get; set; }

        public string PropertyCategory { get; set; }
    }
}