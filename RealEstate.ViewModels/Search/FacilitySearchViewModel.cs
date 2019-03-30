using System.ComponentModel.DataAnnotations;
using RealEstate.Resources;

namespace RealEstate.ViewModels.Search
{
    public class FacilitySearchViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        public string Name { get; set; }
    }
}