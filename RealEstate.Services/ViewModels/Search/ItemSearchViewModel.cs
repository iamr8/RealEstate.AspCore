using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Search
{
    public class ItemSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [SearchParameter("itemAddress")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Category")]
        [SearchParameter("categoryId")]
        public string CategoryId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Owner")]
        [SearchParameter("ownerName")]
        public string Owner { get; set; }

        [SearchParameter("id")]
        public string ItemId { get; set; }

        [SearchParameter("customerId")]
        public string CustomerId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Feature")]
        [SearchParameter("itemFeature")]
        public string FeatureName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "From")]
        [SearchParameter("itemFeatureFrom")]
        public double? FromValue { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "To")]
        [SearchParameter("itemFeatureTo")]
        public double? ToValue { get; set; }
    }
}