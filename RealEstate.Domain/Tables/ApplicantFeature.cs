using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class ApplicantFeature : BaseEntity
    {
        [Required]
        public string Value { get; set; }

        public virtual Applicant Applicant { get; set; }

        public string ApplicantId { get; set; }

        public virtual Feature Feature { get; set; }

        public string FeatureId { get; set; }
    }
}