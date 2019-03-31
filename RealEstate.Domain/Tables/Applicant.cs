using RealEstate.Base.Enums;
using RealEstate.Domain.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class Applicant : BaseEntity
    {
        public Applicant()
        {
            ApplicantFeatures = new HashSet<ApplicantFeature>();
            Logs = new HashSet<Log>();
        }

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