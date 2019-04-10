using System.ComponentModel.DataAnnotations;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Search
{
    public class FeatureSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [SearchParameter("featureName")]
        public string Name { get; set; }

        public FeatureTypeEnum? Type { get; set; }
    }
}