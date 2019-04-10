using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Database.Tables
{
    public class ApplicantFeature : BaseEntity
    {
        public ApplicantFeature()
        {
        }

        [Required]
        public string Value { get; set; }

        public virtual Applicant Applicant { get; set; }

        public string ApplicantId { get; set; }

        public virtual Feature Feature { get; set; }

        public string FeatureId { get; set; }
    }
}