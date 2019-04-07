using RealEstate.Base.Enums;
using RealEstate.Extensions.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.Search
{
    public class CategorySearchViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [SearchParameter("categoryName")]
        public string Name { get; set; }

        [SearchParameter("categoryId")]
        public string Id { get; set; }

        [SearchParameter("type")]
        public CategoryTypeEnum? Type { get; set; }
    }
}