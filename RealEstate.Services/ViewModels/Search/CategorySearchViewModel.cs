using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Search
{
    public class CategorySearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Subject")]
        [SearchParameter("categoryName")]
        public string Name { get; set; }

        [SearchParameter("categoryId")]
        public string Id { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Type")]
        [SearchParameter("type")]
        public CategoryTypeEnum? Type { get; set; }
    }
}