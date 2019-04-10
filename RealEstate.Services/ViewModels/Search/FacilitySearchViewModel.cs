using System.ComponentModel.DataAnnotations;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Search
{
    public class FacilitySearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [SearchParameter("facilityName")]
        public string Name { get; set; }
    }
}