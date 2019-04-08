using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Base.Enums
{
    public enum ApplicantTypeEnum
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Constructor")]
        Constructor,

        [Display(ResourceType = typeof(SharedResource), Name = "Applicant")]
        Applicant,
    }
}