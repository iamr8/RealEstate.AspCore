using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum PresenseStatusEnum
    {
        [Display(ResourceType = typeof(SharedResource), Name = "StartWork")]
        Start,

        [Display(ResourceType = typeof(SharedResource), Name = "EndOfWork")]
        End
    }
}