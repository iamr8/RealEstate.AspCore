using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class PropertyFeature : BaseEntity
    {
        public PropertyFeature()
        {
            Logs = new HashSet<Log>();
        }

        [Required]
        public string Value { get; set; }

        public virtual Property Property { get; set; }

        public string PropertyId { get; set; }

        public virtual Feature Feature { get; set; }

        public string FeatureId { get; set; }
    }
}