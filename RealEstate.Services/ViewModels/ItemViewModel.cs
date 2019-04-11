using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels
{
    public class ItemViewModel : BaseLogViewModel<Item>
    {
        public ItemViewModel(Item entity) : base(entity)
        {
            Description = entity.Description;

            var itemRequest = entity.ItemRequests?.OrderByDescending(x => x.DateTime).FirstOrDefault();
            IsRequested = itemRequest?.IsReject == false;
        }

        public ItemViewModel()
        {
        }

        public string Description { get; set; }

        public bool IsRequested { get; set; }
        public CategoryViewModel Category { get; set; }
        public PropertyViewModel Property { get; set; }
        public List<ItemFeatureViewModel> Features { get; set; }
    }
}