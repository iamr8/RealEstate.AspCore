using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum FeatureTypeEnum
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Deal")]
        Item,

        [Display(ResourceType = typeof(SharedResource), Name = "Property")]
        Property,

        Applicant
    }
}