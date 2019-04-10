using RealEstate.Services.Database.Base;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class PropertyFeature : BaseEntity
    {
        [Required]
        public string Value { get; set; }

        public virtual Property Property { get; set; }

        public string PropertyId { get; set; }

        public virtual Feature Feature { get; set; }

        public string FeatureId { get; set; }
    }
}