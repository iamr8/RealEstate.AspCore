using RealEstate.Base.Enums;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Search
{
    public class CategorySearchViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        public string Name { get; set; }

        public string Id { get; set; }
        public CategoryTypeEnum? Type { get; set; }
    }
}