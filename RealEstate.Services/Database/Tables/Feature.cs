using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Feature : BaseEntity
    {
        public Feature()
        {
            PropertyFeatures = new HashSet<PropertyFeature>();
            ItemFeatures = new HashSet<ItemFeature>();
            ApplicantFeatures = new HashSet<ApplicantFeature>();
        }

        [Required]
        public string Name { get; set; }

        public FeatureTypeEnum Type { get; set; }

        public virtual ICollection<PropertyFeature> PropertyFeatures { get; set; }
        public virtual ICollection<ItemFeature> ItemFeatures { get; set; }
        public virtual ICollection<ApplicantFeature> ApplicantFeatures { get; set; }
    }
}