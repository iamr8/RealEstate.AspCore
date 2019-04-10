using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class Applicant : BaseEntity
    {
        public Applicant()
        {
            ApplicantFeatures = new HashSet<ApplicantFeature>();
        }

        public string Description { get; set; }

        [Required]
        public string Name { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public ApplicantTypeEnum Type { get; set; }
        public string UserId { get; set; }
        public string ContactId { get; set; }
        public string ItemRequestId { get; set; }
        public virtual User User { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ItemRequest ItemRequest { get; set; }
        public virtual ICollection<ApplicantFeature> ApplicantFeatures { get; set; }
    }
}