using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum LogTypeEnum
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Create")]
        Create,

        [Display(ResourceType = typeof(SharedResource), Name = "Delete")]
        Delete,

        [Display(ResourceType = typeof(SharedResource), Name = "Modify")]
        Modify,

        [Display(ResourceType = typeof(SharedResource), Name = "Undelete")]
        Undelete
    }
}