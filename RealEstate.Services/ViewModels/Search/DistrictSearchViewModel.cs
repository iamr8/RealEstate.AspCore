using System.ComponentModel.DataAnnotations;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Search
{
    public class DistrictSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [SearchParameter("districtName")]
        public string Name { get; set; }
    }
}