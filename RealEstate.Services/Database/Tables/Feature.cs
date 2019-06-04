using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Feature : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public FeatureTypeEnum Type { get; set; }

        public virtual ICollection<PropertyFeature> PropertyFeatures { get; set; }
        public virtual ICollection<ItemFeature> ItemFeatures { get; set; }
        public virtual ICollection<ApplicantFeature> ApplicantFeatures { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}