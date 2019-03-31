using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class ApplicantFeature : BaseEntity
    {
        public ApplicantFeature()
        {
            Logs = new HashSet<Log>();
        }

        [Required]
        public string Value { get; set; }

        public virtual Applicant Applicant { get; set; }

        public string ApplicantId { get; set; }

        public virtual Feature Feature { get; set; }

        public string FeatureId { get; set; }
    }
}