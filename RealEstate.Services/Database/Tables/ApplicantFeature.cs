using RealEstate.Services.Database.Base;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class ApplicantFeature : BaseEntity
    {
        [Required]
        public string Value { get; set; }

        public virtual Applicant Applicant { get; set; }

        public string ApplicantId { get; set; }

        public virtual Feature Feature { get; set; }

        public string FeatureId { get; set; }

        public override string ToString()
        {
            return Feature.ToString();
        }
    }
}