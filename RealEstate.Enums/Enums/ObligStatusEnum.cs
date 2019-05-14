using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum ObligStatusEnum
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Obligor")]
        Obligor,

        [Display(ResourceType = typeof(SharedResource), Name = "Obligee")]
        Obligee
    }
}