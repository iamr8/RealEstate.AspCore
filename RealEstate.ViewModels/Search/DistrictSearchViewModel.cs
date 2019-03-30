using System.ComponentModel.DataAnnotations;
using RealEstate.Resources;

namespace RealEstate.ViewModels.Search
{
    public class DistrictSearchViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        public string Name { get; set; }
    }
}