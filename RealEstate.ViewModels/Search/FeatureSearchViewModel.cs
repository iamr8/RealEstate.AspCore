using System.ComponentModel.DataAnnotations;
using RealEstate.Resources;

namespace RealEstate.ViewModels.Search
{
    public class FeatureSearchViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        public string Name { get; set; }
    }
}