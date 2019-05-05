using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Search
{
    public class FeatureSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [SearchParameter("featureName")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [SearchParameter("type")]
        public FeatureTypeEnum? Type { get; set; }
    }
}