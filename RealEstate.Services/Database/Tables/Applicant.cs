using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
{
    public class Applicant : BaseEntity
    {
        public Applicant()
        {
            ApplicantFeatures = new HashSet<ApplicantFeature>();
        }

        public string Description { get; set; }

        public ApplicantTypeEnum Type { get; set; }
        public string UserId { get; set; }
        public string CustomerId { get; set; }
        public string DealId { get; set; }
        public virtual User User { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Deal Deal { get; set; }
        public virtual ICollection<ApplicantFeature> ApplicantFeatures { get; set; }
    }
}