using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum ItemRequestStatusEnum
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Rejected")]
        Rejected,

        [Display(ResourceType = typeof(SharedResource), Name = "PendingResponse")]
        Requested,

        [Display(ResourceType = typeof(SharedResource), Name = "Completed")]
        Finished
    }
}