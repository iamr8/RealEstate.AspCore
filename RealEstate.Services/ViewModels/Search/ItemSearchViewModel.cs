using System.ComponentModel.DataAnnotations;
using RealEstate.Base;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Search
{
    public class ItemSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "DealCategory")]
        public string CategoryId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Owner")]
        public string Owner { get; set; }
    }
}