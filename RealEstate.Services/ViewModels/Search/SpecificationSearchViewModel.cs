using System.ComponentModel.DataAnnotations;
using RealEstate.Base;
using RealEstate.Resources;

namespace RealEstate.Services.ViewModels.Search
{
    public class SpecificationSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        public string Name { get; set; }
    }
}