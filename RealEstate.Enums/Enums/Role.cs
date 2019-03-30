using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum Role
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Admin")]
        Admin,

        [Display(ResourceType = typeof(SharedResource), Name = "User")]
        User,

        [Display(ResourceType = typeof(SharedResource), Name = "SuperAdmin")]
        SuperAdmin
    }
}