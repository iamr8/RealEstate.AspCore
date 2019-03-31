using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class ItemFeature : BaseEntity
    {
        public ItemFeature()
        {
            Logs = new HashSet<Log>();
        }

        [Required]
        public string Value { get; set; }

        public virtual Item Item { get; set; }

        public string ItemId { get; set; }

        public virtual Feature Feature { get; set; }

        public string FeatureId { get; set; }
    }
}