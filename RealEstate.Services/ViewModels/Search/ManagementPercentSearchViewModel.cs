using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels.Search
{
    public class ManagementPercentSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Percent")]
        [SearchParameter("percent")]
        public int? Percent { get; set; }
    }
}