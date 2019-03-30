using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class ItemFeature : BaseEntity
    {
        [Required]
        public string Value { get; set; }

        public virtual Item Item { get; set; }

        public string ItemId { get; set; }

        public virtual Feature Feature { get; set; }

        public string FeatureId { get; set; }
    }
}