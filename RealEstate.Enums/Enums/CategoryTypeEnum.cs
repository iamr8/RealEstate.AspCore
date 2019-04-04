using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum CategoryTypeEnum
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Item")]
        Item,

        [Display(ResourceType = typeof(SharedResource), Name = "Property")]
        Property
    }
}