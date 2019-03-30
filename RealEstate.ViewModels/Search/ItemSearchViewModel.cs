using System.ComponentModel.DataAnnotations;
using RealEstate.Resources;

namespace RealEstate.ViewModels.Search
{
    public class ItemSearchViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "DealCategory")]
        public string CategoryId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Owner")]
        public string Owner { get; set; }
    }
}